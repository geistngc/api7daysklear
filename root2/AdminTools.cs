using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using Force.Crc32;
using Platform;
using Platform.Steam;
using Webserver.Permissions;

// Token: 0x0200008E RID: 142
public class AdminTools
{
	// Token: 0x060002BE RID: 702 RVA: 0x000152B4 File Offset: 0x000134B4
	public AdminTools()
	{
		this.Users = new AdminUsers(this);
		this.Whitelist = new AdminWhitelist(this);
		this.Blacklist = new AdminBlacklist(this);
		this.Commands = new AdminCommands(this);
		this.ApiTokens = new AdminApiTokens(this);
		this.WebModules = new AdminWebModules(this);
		this.WebUsers = new AdminWebUsers(this);
		this.adminFileStorage = PlatformManager.MultiPlatform.UserDataRoaming.DefaultSaveStorage;
		this.registerModules();
		SdDirectory.CreateDirectory(this.GetFilePath());
		this.InitFileWatcher();
		this.Load();
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00015364 File Offset: 0x00013564
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerModules()
	{
		this.modules.Add(this.Users.SectionTypeName, this.Users);
		this.modules.Add(this.Whitelist.SectionTypeName, this.Whitelist);
		this.modules.Add(this.Blacklist.SectionTypeName, this.Blacklist);
		this.modules.Add(this.Commands.SectionTypeName, this.Commands);
		this.modules.Add(this.ApiTokens.SectionTypeName, this.ApiTokens);
		this.modules.Add(this.WebModules.SectionTypeName, this.WebModules);
		this.modules.Add(this.WebUsers.SectionTypeName, this.WebUsers);
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00015435 File Offset: 0x00013635
	public bool CommandAllowedFor(string[] _cmdNames, ClientInfo _clientInfo)
	{
		return this.Commands.GetCommandPermissionLevel(_cmdNames) >= this.Users.GetUserPermissionLevel(_clientInfo);
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00015454 File Offset: 0x00013654
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitFileWatcher()
	{
		if (this.adminFileStorage == UserDataStorageType.DeviceLocal)
		{
			this.fileWatcher = new FileSystemWatcher(this.GetFilePath(), this.GetFileName());
			this.fileWatcher.Changed += this.OnFileChanged;
			this.fileWatcher.Created += this.OnFileChanged;
			this.fileWatcher.Deleted += this.OnFileChanged;
			this.fileWatcher.EnableRaisingEvents = true;
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x000154D1 File Offset: 0x000136D1
	public void DestroyFileWatcher()
	{
		if (this.fileWatcher != null)
		{
			this.fileWatcher.Dispose();
			this.fileWatcher = null;
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x000154ED File Offset: 0x000136ED
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnFileChanged(object _source, FileSystemEventArgs _e)
	{
		Log.Out("Reloading serveradmin.xml");
		this.Load();
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x000154FF File Offset: 0x000136FF
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetFilePath()
	{
		return GameIO.GetSaveGameRootDir(this.adminFileStorage);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0001550C File Offset: 0x0001370C
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetFileName()
	{
		return GamePrefs.GetString(EnumGamePrefs.AdminFileName);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x00015515 File Offset: 0x00013715
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetFullPath()
	{
		return this.GetFilePath() + "/" + this.GetFileName();
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00015530 File Offset: 0x00013730
	[PublicizedFrom(EAccessModifier.Private)]
	public void Load()
	{
		try
		{
			lock (this)
			{
				if (!SdFile.Exists(this.GetFullPath()))
				{
					Log.Out("Permissions file '" + this.GetFileName() + "' not found, creating.");
					this.Save();
					return;
				}
				Log.Out("Loading permissions file at '" + this.GetFullPath() + "'");
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					using (Crc32Algorithm crc32Algorithm = new Crc32Algorithm())
					{
						using (Stream stream = SdFile.OpenRead(this.GetFullPath()))
						{
							using (CryptoStream cryptoStream = new CryptoStream(stream, crc32Algorithm, CryptoStreamMode.Read))
							{
								xmlDocument.Load(cryptoStream);
							}
							uint num = crc32Algorithm.HashUint();
							if (this.lastHash == num)
							{
								Log.Out("Permissions file unchanged, skipping reloading");
								return;
							}
							this.lastHash = num;
						}
					}
				}
				catch (XmlException ex)
				{
					Log.Error("Failed loading permissions file: " + ex.Message);
					return;
				}
				catch (IOException ex2)
				{
					Log.Error("Failed loading permissions file: " + ex2.Message);
					return;
				}
				if (xmlDocument.DocumentElement == null)
				{
					Log.Warning("Permissions file has no root XML element.");
					return;
				}
				this.unknownSections.Clear();
				foreach (KeyValuePair<string, AdminSectionAbs> keyValuePair in this.modules)
				{
					string text;
					AdminSectionAbs adminSectionAbs;
					keyValuePair.Deconstruct(out text, out adminSectionAbs);
					adminSectionAbs.Clear();
				}
				foreach (object obj in xmlDocument.DocumentElement.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType != XmlNodeType.Comment)
					{
						if (xmlNode.NodeType != XmlNodeType.Element)
						{
							Log.Warning("Unexpected top level XML node found: " + xmlNode.OuterXml);
						}
						else
						{
							XmlElement childNode = (XmlElement)xmlNode;
							this.ParseSection(childNode);
						}
					}
				}
			}
			Log.Out("Loading permissions file done.");
		}
		catch (Exception e)
		{
			Log.Error("Exception while trying to load serveradmins.xml:");
			Log.Exception(e);
		}
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00015838 File Offset: 0x00013A38
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParseSection(XmlElement _childNode)
	{
		string text = _childNode.Name;
		string text2;
		if (!(text == "admins"))
		{
			if (!(text == "permissions"))
			{
				text2 = text;
			}
			else
			{
				text2 = "commands";
			}
		}
		else
		{
			text2 = "users";
		}
		text = text2;
		AdminSectionAbs adminSectionAbs;
		if (!this.modules.TryGetValue(text, out adminSectionAbs))
		{
			Log.Warning("Ignoring unknown section in permissions file: " + text);
			this.unknownSections.Add(_childNode);
			return;
		}
		adminSectionAbs.Parse(_childNode);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x000158B0 File Offset: 0x00013AB0
	public static PlatformUserIdentifierAbs ParseUserIdentifier(XmlElement _lineItem)
	{
		PlatformUserIdentifierAbs platformUserIdentifierAbs = PlatformUserIdentifierAbs.FromXml(_lineItem, false, null);
		if (platformUserIdentifierAbs != null)
		{
			return platformUserIdentifierAbs;
		}
		if (_lineItem.HasAttribute("steamID"))
		{
			string attribute = _lineItem.GetAttribute("steamID");
			try
			{
				return new UserIdentifierSteam(attribute);
			}
			catch (ArgumentException)
			{
				Log.Warning("Ignoring entry because of invalid 'steamID' attribute value: " + _lineItem.OuterXml);
				return null;
			}
		}
		Log.Warning("Ignoring entry because of missing 'platform' or 'userid' attribute: " + _lineItem.OuterXml);
		return null;
	}

	// Token: 0x060002CA RID: 714 RVA: 0x00015930 File Offset: 0x00013B30
	public void Save()
	{
		try
		{
			lock (this)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.CreateXmlDeclaration();
				xmlDocument.AddXmlComment("\r\n\tThis file holds the settings for who is banned, whitelisted, admins and server command permissions. The\r\n\tadmin and whitelist sections can contain both individual Steam users as well as Steam groups.\r\n\r\n\tIt is recommended to modify this file only through the respective console commands, like \"admin\", or\r\n\tthe Web Dashboard.\r\n\r\n\r\n\tUSER ID INSTRUCTIONS:\r\n\t===============================================================\r\n\tAny user entry uses two elements to identify whom it applies to.\r\n\t- platform: Identifier of the platform the User ID belongs to, i.e. \"EOS\", \"Steam\", \"XBL\", \"PSN\"\r\n\t- userid: The actual ID of the user on that platform. Examples:\r\n\t  - EOS: \"0002604bc42244e099c1bf05145fb71f\"\r\n\t  - Steam: SteamID64, e.g. \"76561198021925107\", see below\r\n\tYou can look up the IDs in the logs, e.g. whenever a user logs in the ID is logged.\r\n\r\n\tSTEAM ID INSTRUCTIONS:\r\n\t===============================================================\r\n\tYou can find the SteamID64 of any user with one of the following pages:\r\n\thttps://steamdb.info/calculator/, https://steamid.io/lookup, https://steamid.co/\r\n\thttps://steamid.co/ instructions:\r\n\tInput the player's name in the search field. example: Kinyajuu\r\n\tIf the name doesn't work, you can also use the url of their steam page.\r\n\tYou will want the STEAM64ID. example: 76561198021925107\r\n\r\n\tSTEAM GROUP ID INSTRUCTIONS:\r\n\t===============================================================\r\n\tYou can find the SteamID64 of any group by taking its address and adding\r\n\t  /memberslistxml/?xml=1\r\n\tto the end. You will get the XML information of the group which should have an entry\r\n\tmemberList->groupID64.\r\n\tExample: The 'Steam Universe' group has the address\r\n\t  https://steamcommunity.com/groups/steamuniverse\r\n\tSo you point your browser to\r\n\t  https://steamcommunity.com/groups/steamuniverse/memberslistxml/?xml=1\r\n\tAnd see that the groupID64 is 103582791434672565.\r\n\r\n\tPERMISSION LEVEL INSTRUCTIONS:\r\n\t===============================================================\r\n\tpermission level : 0-1000, a user may run any command equal to or above their permission level.\r\n\tUsers not given a permission level in this file will have a default permission level of 1000!\r\n\r\n\tCOMMAND PERMISSIONS INSTRUCTIONS:\r\n\t===============================================================\r\n\tcmd : This is the command name, any command not in this list will not be usable by anyone but the server.\r\n\tpermission level : 0-1000, a user may run any command equal to or above their permission level.\r\n\tCommands not specified in this file will have a default permission level of 0!\r\n\r\n\tEVERYTHING BETWEEN <!- - and - -> IS COMMENTED OUT! THE ENTRIES BELOW ARE EXAMPLES THAT ARE NOT ACTIVE!!!\r\n");
				XmlElement xmlElement = xmlDocument.AddXmlElement("adminTools");
				xmlElement.AddXmlComment(" Name in any entries is optional for display purposes only ");
				this.WriteSections(xmlElement);
				for (int i = 0; i < this.unknownSections.Count; i++)
				{
					XmlElement node = this.unknownSections[i];
					XmlNode newChild = xmlDocument.ImportNode(node, true);
					xmlElement.AppendChild(newChild);
				}
				if (this.fileWatcher != null)
				{
					this.fileWatcher.EnableRaisingEvents = false;
				}
				using (Crc32Algorithm crc32Algorithm = new Crc32Algorithm())
				{
					using (Stream stream = SdFile.Open(this.GetFullPath(), FileMode.Create, FileAccess.Write, FileShare.Read))
					{
						using (CryptoStream cryptoStream = new CryptoStream(stream, crc32Algorithm, CryptoStreamMode.Write))
						{
							xmlDocument.Save(cryptoStream);
						}
						this.lastHash = crc32Algorithm.HashUint();
						if (this.fileWatcher != null)
						{
							this.fileWatcher.EnableRaisingEvents = true;
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			Log.Error("Exception while trying to save serveradmins.xml:");
			Log.Exception(e);
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00015AE4 File Offset: 0x00013CE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteSections(XmlElement _root)
	{
		foreach (KeyValuePair<string, AdminSectionAbs> keyValuePair in this.modules)
		{
			string text;
			AdminSectionAbs adminSectionAbs;
			keyValuePair.Deconstruct(out text, out adminSectionAbs);
			adminSectionAbs.Save(_root);
		}
	}

	// Token: 0x04000362 RID: 866
	[PublicizedFrom(EAccessModifier.Private)]
	public const string XmlHeader = "\r\n\tThis file holds the settings for who is banned, whitelisted, admins and server command permissions. The\r\n\tadmin and whitelist sections can contain both individual Steam users as well as Steam groups.\r\n\r\n\tIt is recommended to modify this file only through the respective console commands, like \"admin\", or\r\n\tthe Web Dashboard.\r\n\r\n\r\n\tUSER ID INSTRUCTIONS:\r\n\t===============================================================\r\n\tAny user entry uses two elements to identify whom it applies to.\r\n\t- platform: Identifier of the platform the User ID belongs to, i.e. \"EOS\", \"Steam\", \"XBL\", \"PSN\"\r\n\t- userid: The actual ID of the user on that platform. Examples:\r\n\t  - EOS: \"0002604bc42244e099c1bf05145fb71f\"\r\n\t  - Steam: SteamID64, e.g. \"76561198021925107\", see below\r\n\tYou can look up the IDs in the logs, e.g. whenever a user logs in the ID is logged.\r\n\r\n\tSTEAM ID INSTRUCTIONS:\r\n\t===============================================================\r\n\tYou can find the SteamID64 of any user with one of the following pages:\r\n\thttps://steamdb.info/calculator/, https://steamid.io/lookup, https://steamid.co/\r\n\thttps://steamid.co/ instructions:\r\n\tInput the player's name in the search field. example: Kinyajuu\r\n\tIf the name doesn't work, you can also use the url of their steam page.\r\n\tYou will want the STEAM64ID. example: 76561198021925107\r\n\r\n\tSTEAM GROUP ID INSTRUCTIONS:\r\n\t===============================================================\r\n\tYou can find the SteamID64 of any group by taking its address and adding\r\n\t  /memberslistxml/?xml=1\r\n\tto the end. You will get the XML information of the group which should have an entry\r\n\tmemberList->groupID64.\r\n\tExample: The 'Steam Universe' group has the address\r\n\t  https://steamcommunity.com/groups/steamuniverse\r\n\tSo you point your browser to\r\n\t  https://steamcommunity.com/groups/steamuniverse/memberslistxml/?xml=1\r\n\tAnd see that the groupID64 is 103582791434672565.\r\n\r\n\tPERMISSION LEVEL INSTRUCTIONS:\r\n\t===============================================================\r\n\tpermission level : 0-1000, a user may run any command equal to or above their permission level.\r\n\tUsers not given a permission level in this file will have a default permission level of 1000!\r\n\r\n\tCOMMAND PERMISSIONS INSTRUCTIONS:\r\n\t===============================================================\r\n\tcmd : This is the command name, any command not in this list will not be usable by anyone but the server.\r\n\tpermission level : 0-1000, a user may run any command equal to or above their permission level.\r\n\tCommands not specified in this file will have a default permission level of 0!\r\n\r\n\tEVERYTHING BETWEEN <!- - and - -> IS COMMENTED OUT! THE ENTRIES BELOW ARE EXAMPLES THAT ARE NOT ACTIVE!!!\r\n";

	// Token: 0x04000363 RID: 867
	public readonly AdminUsers Users;

	// Token: 0x04000364 RID: 868
	public readonly AdminWhitelist Whitelist;

	// Token: 0x04000365 RID: 869
	public readonly AdminBlacklist Blacklist;

	// Token: 0x04000366 RID: 870
	public readonly AdminCommands Commands;

	// Token: 0x04000367 RID: 871
	public readonly AdminApiTokens ApiTokens;

	// Token: 0x04000368 RID: 872
	public readonly AdminWebModules WebModules;

	// Token: 0x04000369 RID: 873
	public readonly AdminWebUsers WebUsers;

	// Token: 0x0400036A RID: 874
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XmlElement> unknownSections = new List<XmlElement>();

	// Token: 0x0400036B RID: 875
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AdminSectionAbs> modules = new Dictionary<string, AdminSectionAbs>();

	// Token: 0x0400036C RID: 876
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UserDataStorageType adminFileStorage;

	// Token: 0x0400036D RID: 877
	[PublicizedFrom(EAccessModifier.Private)]
	public FileSystemWatcher fileWatcher;

	// Token: 0x0400036E RID: 878
	[PublicizedFrom(EAccessModifier.Private)]
	public uint lastHash;
}
