using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Webserver.Permissions
{
	// Token: 0x02001B06 RID: 6918
	public class AdminWebUsers : AdminSectionAbs
	{
		// Token: 0x17001992 RID: 6546
		// (get) Token: 0x0600CFF1 RID: 53233 RVA: 0x004BE06A File Offset: 0x004BC26A
		public static AdminWebUsers Instance
		{
			get
			{
				return GameManager.Instance.adminTools.WebUsers;
			}
		}

		// Token: 0x0600CFF2 RID: 53234 RVA: 0x004BE07B File Offset: 0x004BC27B
		public AdminWebUsers(AdminTools _parent) : base(_parent, "webusers")
		{
		}

		// Token: 0x0600CFF3 RID: 53235 RVA: 0x004BE094 File Offset: 0x004BC294
		public override void Clear()
		{
			this.users.Clear();
		}

		// Token: 0x0600CFF4 RID: 53236 RVA: 0x004BE0A4 File Offset: 0x004BC2A4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void ParseElement(XmlElement _childElement)
		{
			AdminWebUsers.WebUser webUser;
			if (AdminWebUsers.WebUser.TryParse(_childElement, out webUser))
			{
				this.users[webUser.Name] = webUser;
			}
		}

		// Token: 0x0600CFF5 RID: 53237 RVA: 0x004BE0D0 File Offset: 0x004BC2D0
		public override void Save(XmlElement _root)
		{
			XmlElement parent = _root.AddXmlElement(this.SectionTypeName);
			foreach (KeyValuePair<string, AdminWebUsers.WebUser> keyValuePair in this.users)
			{
				string text;
				AdminWebUsers.WebUser webUser;
				keyValuePair.Deconstruct(out text, out webUser);
				AdminWebUsers.WebUser webUser2 = webUser;
				webUser2.ToXml(parent);
			}
		}

		// Token: 0x0600CFF6 RID: 53238 RVA: 0x004BE140 File Offset: 0x004BC340
		public void AddUser(string _name, string _password, PlatformUserIdentifierAbs _userIdentifier, PlatformUserIdentifierAbs _crossPlatformIdentifier)
		{
			AdminTools parent = this.Parent;
			lock (parent)
			{
				AdminWebUsers.WebUser value = new AdminWebUsers.WebUser(_name, _password, _userIdentifier, _crossPlatformIdentifier);
				this.users[_name] = value;
				this.Parent.Save();
			}
		}

		// Token: 0x0600CFF7 RID: 53239 RVA: 0x004BE1A0 File Offset: 0x004BC3A0
		public bool RemoveUser(string _name)
		{
			AdminTools parent = this.Parent;
			bool result;
			lock (parent)
			{
				bool flag2 = this.users.Remove(_name);
				if (flag2)
				{
					this.Parent.Save();
				}
				result = flag2;
			}
			return result;
		}

		// Token: 0x0600CFF8 RID: 53240 RVA: 0x004BE1F8 File Offset: 0x004BC3F8
		public Dictionary<string, AdminWebUsers.WebUser> GetUsers()
		{
			AdminTools parent = this.Parent;
			Dictionary<string, AdminWebUsers.WebUser> result;
			lock (parent)
			{
				result = this.users;
			}
			return result;
		}

		// Token: 0x0600CFF9 RID: 53241 RVA: 0x004BE23C File Offset: 0x004BC43C
		[PublicizedFrom(EAccessModifier.Private)]
		public static byte[] Hash(string _input)
		{
			return new MD5Cng().ComputeHash(Encoding.UTF8.GetBytes(_input));
		}

		// Token: 0x0600CFFA RID: 53242 RVA: 0x004BE254 File Offset: 0x004BC454
		public bool TryGetUser(string _name, string _password, out AdminWebUsers.WebUser _result)
		{
			AdminTools parent = this.Parent;
			bool result;
			lock (parent)
			{
				if (this.users.TryGetValue(_name, out _result) && _result.ValidatePassword(_password))
				{
					result = true;
				}
				else
				{
					_result = default(AdminWebUsers.WebUser);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600CFFB RID: 53243 RVA: 0x004BE2B4 File Offset: 0x004BC4B4
		public bool HasUser(PlatformUserIdentifierAbs _platformUser, PlatformUserIdentifierAbs _crossPlatformUser, out AdminWebUsers.WebUser _result)
		{
			AdminTools parent = this.Parent;
			bool result;
			lock (parent)
			{
				_result = default(AdminWebUsers.WebUser);
				foreach (KeyValuePair<string, AdminWebUsers.WebUser> keyValuePair in this.users)
				{
					string text;
					AdminWebUsers.WebUser webUser;
					keyValuePair.Deconstruct(out text, out webUser);
					AdminWebUsers.WebUser webUser2 = webUser;
					if (object.Equals(webUser2.PlatformUser, _platformUser) && object.Equals(webUser2.CrossPlatformUser, _crossPlatformUser))
					{
						_result = webUser2;
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04009E44 RID: 40516
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, AdminWebUsers.WebUser> users = new CaseInsensitiveStringDictionary<AdminWebUsers.WebUser>();

		// Token: 0x02001B07 RID: 6919
		public readonly struct WebUser
		{
			// Token: 0x0600CFFC RID: 53244 RVA: 0x004BE36C File Offset: 0x004BC56C
			public WebUser(string _name, byte[] _passwordHash, PlatformUserIdentifierAbs _platformUser, PlatformUserIdentifierAbs _crossPlatformUser)
			{
				this.Name = _name;
				this.PasswordHash = _passwordHash;
				this.PlatformUser = _platformUser;
				this.CrossPlatformUser = _crossPlatformUser;
			}

			// Token: 0x0600CFFD RID: 53245 RVA: 0x004BE38B File Offset: 0x004BC58B
			public WebUser(string _name, string _password, PlatformUserIdentifierAbs _platformUser, PlatformUserIdentifierAbs _crossPlatformUser)
			{
				this.Name = _name;
				this.PasswordHash = AdminWebUsers.Hash(_password);
				this.PlatformUser = _platformUser;
				this.CrossPlatformUser = _crossPlatformUser;
			}

			// Token: 0x0600CFFE RID: 53246 RVA: 0x004BE3B0 File Offset: 0x004BC5B0
			public void ToXml(XmlElement _parent)
			{
				XmlElement xmlElement = _parent.AddXmlElement("user");
				xmlElement.SetAttrib("name", this.Name).SetAttrib("pass", Convert.ToBase64String(this.PasswordHash));
				this.PlatformUser.ToXml(xmlElement, "");
				PlatformUserIdentifierAbs crossPlatformUser = this.CrossPlatformUser;
				if (crossPlatformUser == null)
				{
					return;
				}
				crossPlatformUser.ToXml(xmlElement, "cross");
			}

			// Token: 0x0600CFFF RID: 53247 RVA: 0x004BE418 File Offset: 0x004BC618
			public static bool TryParse(XmlElement _element, out AdminWebUsers.WebUser _result)
			{
				_result = default(AdminWebUsers.WebUser);
				string name;
				if (!_element.TryGetAttribute("name", out name))
				{
					Log.Warning("[Web] [Perms] Ignoring user-entry because of missing 'name' attribute: " + _element.OuterXml);
					return false;
				}
				string s;
				if (!_element.TryGetAttribute("pass", out s))
				{
					Log.Warning("[Web] [Perms] Ignoring user-entry because of missing 'pass' attribute: " + _element.OuterXml);
					return false;
				}
				PlatformUserIdentifierAbs platformUserIdentifierAbs = PlatformUserIdentifierAbs.FromXml(_element, false, null);
				if (platformUserIdentifierAbs == null)
				{
					Log.Warning("[Web] [Perms] Ignoring user-entry because of missing 'platform' or 'userid' attribute: " + _element.OuterXml);
					return false;
				}
				PlatformUserIdentifierAbs crossPlatformUser = PlatformUserIdentifierAbs.FromXml(_element, false, "cross");
				byte[] passwordHash = Convert.FromBase64String(s);
				_result = new AdminWebUsers.WebUser(name, passwordHash, platformUserIdentifierAbs, crossPlatformUser);
				return true;
			}

			// Token: 0x0600D000 RID: 53248 RVA: 0x004BE4C1 File Offset: 0x004BC6C1
			public bool ValidatePassword(string _password)
			{
				return Utils.ArrayEquals(AdminWebUsers.Hash(_password), this.PasswordHash);
			}

			// Token: 0x04009E45 RID: 40517
			public readonly string Name;

			// Token: 0x04009E46 RID: 40518
			public readonly byte[] PasswordHash;

			// Token: 0x04009E47 RID: 40519
			public readonly PlatformUserIdentifierAbs PlatformUser;

			// Token: 0x04009E48 RID: 40520
			public readonly PlatformUserIdentifierAbs CrossPlatformUser;
		}
	}
}
