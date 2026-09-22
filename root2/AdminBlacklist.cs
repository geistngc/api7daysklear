using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

// Token: 0x02000088 RID: 136
public class AdminBlacklist : AdminSectionAbs
{
	// Token: 0x0600029E RID: 670 RVA: 0x000149C1 File Offset: 0x00012BC1
	public AdminBlacklist(AdminTools _parent) : base(_parent, "blacklist")
	{
	}

	// Token: 0x0600029F RID: 671 RVA: 0x000149DA File Offset: 0x00012BDA
	public override void Clear()
	{
		this.bannedUsers.Clear();
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x000149E8 File Offset: 0x00012BE8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminBlacklist.BannedUser bannedUser;
		if (AdminBlacklist.BannedUser.TryParse(_childElement, out bannedUser))
		{
			this.bannedUsers[bannedUser.UserIdentifier] = bannedUser;
		}
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00014A14 File Offset: 0x00012C14
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement("blacklist");
		xmlElement.AddXmlComment(" <blacklisted platform=\"\" userid=\"\" name=\"\" unbandate=\"\" reason=\"\" /> ");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminBlacklist.BannedUser> keyValuePair in this.bannedUsers)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x00014A88 File Offset: 0x00012C88
	public void AddBan(string _name, PlatformUserIdentifierAbs _identifier, DateTime _banUntil, string _banReason)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminBlacklist.BannedUser value = new AdminBlacklist.BannedUser(_name, _identifier, _banUntil, _banReason);
			this.bannedUsers[_identifier] = value;
			if (_banUntil > DateTime.Now)
			{
				this.Parent.Users.RemoveUser(_identifier, false);
			}
			this.Parent.Save();
		}
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00014B08 File Offset: 0x00012D08
	public bool RemoveBan(PlatformUserIdentifierAbs _identifier)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.bannedUsers.Remove(_identifier);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x00014B60 File Offset: 0x00012D60
	public bool IsBanned(PlatformUserIdentifierAbs _identifier, out DateTime _bannedUntil, out string _reason)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			if (this.bannedUsers.ContainsKey(_identifier))
			{
				AdminBlacklist.BannedUser bannedUser = this.bannedUsers[_identifier];
				if (bannedUser.BannedUntil > DateTime.Now)
				{
					_bannedUntil = bannedUser.BannedUntil;
					_reason = bannedUser.BanReason;
					return true;
				}
			}
			_bannedUntil = DateTime.Now;
			_reason = string.Empty;
			result = false;
		}
		return result;
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x00014BF4 File Offset: 0x00012DF4
	public List<AdminBlacklist.BannedUser> GetBanned()
	{
		AdminTools parent = this.Parent;
		List<AdminBlacklist.BannedUser> result;
		lock (parent)
		{
			result = (from _b in this.bannedUsers.Values
			where _b.BannedUntil > DateTime.Now
			select _b).ToList<AdminBlacklist.BannedUser>();
		}
		return result;
	}

	// Token: 0x04000355 RID: 853
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, AdminBlacklist.BannedUser> bannedUsers = new Dictionary<PlatformUserIdentifierAbs, AdminBlacklist.BannedUser>();

	// Token: 0x02000089 RID: 137
	public readonly struct BannedUser
	{
		// Token: 0x060002A6 RID: 678 RVA: 0x00014C64 File Offset: 0x00012E64
		public BannedUser(string _name, PlatformUserIdentifierAbs _userIdentifier, DateTime _banUntil, string _banReason)
		{
			this.Name = _name;
			this.UserIdentifier = _userIdentifier;
			this.BannedUntil = _banUntil;
			this.BanReason = (_banReason ?? string.Empty);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00014C8C File Offset: 0x00012E8C
		public void ToXml(XmlElement _parent)
		{
			XmlElement xmlElement = _parent.AddXmlElement("blacklisted");
			this.UserIdentifier.ToXml(xmlElement, "");
			if (this.Name != null)
			{
				xmlElement.SetAttrib("name", this.Name);
			}
			xmlElement.SetAttrib("unbandate", this.BannedUntil.ToCultureInvariantString());
			xmlElement.SetAttrib("reason", this.BanReason);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00014CFC File Offset: 0x00012EFC
		public static bool TryParse(XmlElement _element, out AdminBlacklist.BannedUser _result)
		{
			_result = default(AdminBlacklist.BannedUser);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("unbandate"))
			{
				Log.Warning("Ignoring blacklist-entry because of missing 'unbandate' attribute: " + _element.OuterXml);
				return false;
			}
			DateTime banUntil;
			if (!StringParsers.TryParseDateTime(_element.GetAttribute("unbandate"), out banUntil) && !DateTime.TryParse(_element.GetAttribute("unbandate"), out banUntil))
			{
				Log.Warning("Ignoring blacklist-entry because of invalid value for 'unbandate' attribute: " + _element.OuterXml);
				return false;
			}
			PlatformUserIdentifierAbs platformUserIdentifierAbs = AdminTools.ParseUserIdentifier(_element);
			if (platformUserIdentifierAbs == null)
			{
				return false;
			}
			string attribute = _element.GetAttribute("reason");
			_result = new AdminBlacklist.BannedUser(text, platformUserIdentifierAbs, banUntil, attribute);
			return true;
		}

		// Token: 0x04000356 RID: 854
		public readonly string Name;

		// Token: 0x04000357 RID: 855
		public readonly PlatformUserIdentifierAbs UserIdentifier;

		// Token: 0x04000358 RID: 856
		public readonly DateTime BannedUntil;

		// Token: 0x04000359 RID: 857
		public readonly string BanReason;
	}
}
