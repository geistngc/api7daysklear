using System;
using System.Collections.Generic;
using System.Xml;

// Token: 0x02000092 RID: 146
public class AdminWhitelist : AdminSectionAbs
{
	// Token: 0x060002DF RID: 735 RVA: 0x00016340 File Offset: 0x00014540
	public AdminWhitelist(AdminTools _parent) : base(_parent, "whitelist")
	{
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00016364 File Offset: 0x00014564
	public override void Clear()
	{
		this.whitelistedUsers.Clear();
		this.whitelistedGroups.Clear();
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0001637C File Offset: 0x0001457C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminWhitelist.WhitelistUser whitelistUser;
		if (_childElement.Name == "group")
		{
			AdminWhitelist.WhitelistGroup whitelistGroup;
			if (AdminWhitelist.WhitelistGroup.TryParse(_childElement, out whitelistGroup))
			{
				this.whitelistedGroups[whitelistGroup.SteamIdGroup] = whitelistGroup;
				return;
			}
		}
		else if (AdminWhitelist.WhitelistUser.TryParse(_childElement, out whitelistUser))
		{
			this.whitelistedUsers[whitelistUser.UserIdentifier] = whitelistUser;
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x000163D4 File Offset: 0x000145D4
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement(this.SectionTypeName);
		xmlElement.AddXmlComment(" ONLY PUT ITEMS IN WHITELIST IF YOU WANT WHITELIST ONLY ENABLED!!! ");
		xmlElement.AddXmlComment(" If there are any items in the whitelist, the whitelist only mode is enabled ");
		xmlElement.AddXmlComment(" Nobody can join that ISN'T in the whitelist or admins once whitelist only mode is enabled ");
		xmlElement.AddXmlComment(" Name is optional for display purposes only ");
		xmlElement.AddXmlComment(" <user platform=\"\" userid=\"\" name=\"\" /> ");
		xmlElement.AddXmlComment(" <group steamID=\"\" name=\"\" /> ");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> keyValuePair in this.whitelistedUsers)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
		foreach (KeyValuePair<string, AdminWhitelist.WhitelistGroup> keyValuePair2 in this.whitelistedGroups)
		{
			keyValuePair2.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x000164D4 File Offset: 0x000146D4
	public void AddUser(string _name, PlatformUserIdentifierAbs _identifier)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminWhitelist.WhitelistUser value = new AdminWhitelist.WhitelistUser(_name, _identifier);
			this.whitelistedUsers[_identifier] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00016530 File Offset: 0x00014730
	public bool RemoveUser(PlatformUserIdentifierAbs _identifier)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.whitelistedUsers.Remove(_identifier);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x00016588 File Offset: 0x00014788
	public Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> GetUsers()
	{
		AdminTools parent = this.Parent;
		Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> result;
		lock (parent)
		{
			result = this.whitelistedUsers;
		}
		return result;
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x000165CC File Offset: 0x000147CC
	public void AddGroup(string _name, string _steamId)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminWhitelist.WhitelistGroup value = new AdminWhitelist.WhitelistGroup(_name, _steamId);
			this.whitelistedGroups[_steamId] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00016628 File Offset: 0x00014828
	public bool RemoveGroup(string _steamId)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.whitelistedGroups.Remove(_steamId);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00016680 File Offset: 0x00014880
	public Dictionary<string, AdminWhitelist.WhitelistGroup> GetGroups()
	{
		AdminTools parent = this.Parent;
		Dictionary<string, AdminWhitelist.WhitelistGroup> result;
		lock (parent)
		{
			result = this.whitelistedGroups;
		}
		return result;
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x000166C4 File Offset: 0x000148C4
	public bool IsWhitelisted(ClientInfo _clientInfo)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			if (this.whitelistedUsers.ContainsKey(_clientInfo.PlatformId) || this.whitelistedUsers.ContainsKey(_clientInfo.CrossplatformId))
			{
				result = true;
			}
			else
			{
				foreach (KeyValuePair<string, int> keyValuePair in _clientInfo.groupMemberships)
				{
					if (this.whitelistedGroups.ContainsKey(keyValuePair.Key))
					{
						return true;
					}
				}
				result = false;
			}
		}
		return result;
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00016780 File Offset: 0x00014980
	public bool IsWhiteListEnabled()
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			result = (this.whitelistedUsers.Count > 0 || this.whitelistedGroups.Count > 0);
		}
		return result;
	}

	// Token: 0x04000378 RID: 888
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> whitelistedUsers = new Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser>();

	// Token: 0x04000379 RID: 889
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AdminWhitelist.WhitelistGroup> whitelistedGroups = new Dictionary<string, AdminWhitelist.WhitelistGroup>();

	// Token: 0x02000093 RID: 147
	public readonly struct WhitelistUser
	{
		// Token: 0x060002EB RID: 747 RVA: 0x000167DC File Offset: 0x000149DC
		public WhitelistUser(string _name, PlatformUserIdentifierAbs _userIdentifier)
		{
			this.Name = _name;
			this.UserIdentifier = _userIdentifier;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x000167EC File Offset: 0x000149EC
		public void ToXml(XmlElement _parent)
		{
			XmlElement xmlElement = _parent.AddXmlElement("user");
			this.UserIdentifier.ToXml(xmlElement, "");
			if (this.Name != null)
			{
				xmlElement.SetAttrib("name", this.Name);
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00016830 File Offset: 0x00014A30
		public static bool TryParse(XmlElement _element, out AdminWhitelist.WhitelistUser _result)
		{
			_result = default(AdminWhitelist.WhitelistUser);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			PlatformUserIdentifierAbs platformUserIdentifierAbs = AdminTools.ParseUserIdentifier(_element);
			if (platformUserIdentifierAbs == null)
			{
				return false;
			}
			_result = new AdminWhitelist.WhitelistUser(text, platformUserIdentifierAbs);
			return true;
		}

		// Token: 0x0400037A RID: 890
		public readonly string Name;

		// Token: 0x0400037B RID: 891
		public readonly PlatformUserIdentifierAbs UserIdentifier;
	}

	// Token: 0x02000094 RID: 148
	public readonly struct WhitelistGroup
	{
		// Token: 0x060002EE RID: 750 RVA: 0x00016874 File Offset: 0x00014A74
		public WhitelistGroup(string _name, string _steamIdGroup)
		{
			this.Name = _name;
			this.SteamIdGroup = _steamIdGroup;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00016884 File Offset: 0x00014A84
		public void ToXml(XmlElement _parent)
		{
			XmlElement element = _parent.AddXmlElement("group");
			element.SetAttrib("steamID", this.SteamIdGroup);
			if (this.Name != null)
			{
				element.SetAttrib("name", this.Name);
			}
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x000168CC File Offset: 0x00014ACC
		public static bool TryParse(XmlElement _element, out AdminWhitelist.WhitelistGroup _result)
		{
			_result = default(AdminWhitelist.WhitelistGroup);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("steamID"))
			{
				Log.Warning("Ignoring whitelist-entry because of missing 'steamID' attribute: " + _element.OuterXml);
				return false;
			}
			string attribute = _element.GetAttribute("steamID");
			_result = new AdminWhitelist.WhitelistGroup(text, attribute);
			return true;
		}

		// Token: 0x0400037C RID: 892
		public readonly string Name;

		// Token: 0x0400037D RID: 893
		public readonly string SteamIdGroup;
	}
}
