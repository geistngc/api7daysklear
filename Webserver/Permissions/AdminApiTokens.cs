using System;
using System.Collections.Generic;
using System.Xml;

namespace Webserver.Permissions
{
	// Token: 0x02001B02 RID: 6914
	public class AdminApiTokens : AdminSectionAbs
	{
		// Token: 0x17001990 RID: 6544
		// (get) Token: 0x0600CFD0 RID: 53200 RVA: 0x004BD377 File Offset: 0x004BB577
		public static AdminApiTokens Instance
		{
			get
			{
				return GameManager.Instance.adminTools.ApiTokens;
			}
		}

		// Token: 0x0600CFD1 RID: 53201 RVA: 0x004BD388 File Offset: 0x004BB588
		public AdminApiTokens(AdminTools _parent) : base(_parent, "apitokens")
		{
		}

		// Token: 0x0600CFD2 RID: 53202 RVA: 0x004BD3A1 File Offset: 0x004BB5A1
		public override void Clear()
		{
			this.tokens.Clear();
		}

		// Token: 0x0600CFD3 RID: 53203 RVA: 0x004BD3B0 File Offset: 0x004BB5B0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void ParseElement(XmlElement _childElement)
		{
			AdminApiTokens.ApiToken apiToken;
			if (AdminApiTokens.ApiToken.TryParse(_childElement, out apiToken))
			{
				this.tokens[apiToken.Name] = apiToken;
			}
		}

		// Token: 0x0600CFD4 RID: 53204 RVA: 0x004BD3DC File Offset: 0x004BB5DC
		public override void Save(XmlElement _root)
		{
			XmlElement xmlElement = _root.AddXmlElement(this.SectionTypeName);
			xmlElement.AddXmlComment(" <token name=\"adminuser1\" secret=\"supersecrettoken\" permission_level=\"0\" /> ");
			foreach (KeyValuePair<string, AdminApiTokens.ApiToken> keyValuePair in this.tokens)
			{
				string text;
				AdminApiTokens.ApiToken apiToken;
				keyValuePair.Deconstruct(out text, out apiToken);
				AdminApiTokens.ApiToken apiToken2 = apiToken;
				apiToken2.ToXml(xmlElement);
			}
		}

		// Token: 0x0600CFD5 RID: 53205 RVA: 0x004BD458 File Offset: 0x004BB658
		public void AddToken(string _name, string _secret, int _permissionLevel)
		{
			AdminTools parent = this.Parent;
			lock (parent)
			{
				AdminApiTokens.ApiToken value = new AdminApiTokens.ApiToken(_name, _secret, _permissionLevel);
				this.tokens[_name] = value;
				this.Parent.Save();
			}
		}

		// Token: 0x0600CFD6 RID: 53206 RVA: 0x004BD4B4 File Offset: 0x004BB6B4
		public bool RemoveToken(string _name)
		{
			AdminTools parent = this.Parent;
			bool result;
			lock (parent)
			{
				bool flag2 = this.tokens.Remove(_name);
				if (flag2)
				{
					this.Parent.Save();
				}
				result = flag2;
			}
			return result;
		}

		// Token: 0x0600CFD7 RID: 53207 RVA: 0x004BD50C File Offset: 0x004BB70C
		public Dictionary<string, AdminApiTokens.ApiToken> GetTokens()
		{
			AdminTools parent = this.Parent;
			Dictionary<string, AdminApiTokens.ApiToken> result;
			lock (parent)
			{
				result = this.tokens;
			}
			return result;
		}

		// Token: 0x0600CFD8 RID: 53208 RVA: 0x004BD550 File Offset: 0x004BB750
		public int GetPermissionLevel(string _name, string _secret)
		{
			AdminTools parent = this.Parent;
			int result;
			lock (parent)
			{
				AdminApiTokens.ApiToken apiToken;
				if (this.tokens.TryGetValue(_name, out apiToken) && apiToken.Secret == _secret)
				{
					result = apiToken.PermissionLevel;
				}
				else if (this.IsCommandlineToken(_name, _secret))
				{
					result = 0;
				}
				else
				{
					result = int.MaxValue;
				}
			}
			return result;
		}

		// Token: 0x0600CFD9 RID: 53209 RVA: 0x004BD5C8 File Offset: 0x004BB7C8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsCommandlineToken(string _name, string _secret)
		{
			if (!this.commandlineChecked)
			{
				this.commandlineTokenName = GameUtils.GetLaunchArgument("webapitokenname");
				this.commandlineTokenSecret = GameUtils.GetLaunchArgument("webapitokensecret");
				this.commandlineChecked = true;
			}
			return !string.IsNullOrEmpty(this.commandlineTokenName) && !string.IsNullOrEmpty(this.commandlineTokenSecret) && _name == this.commandlineTokenName && _secret == this.commandlineTokenSecret;
		}

		// Token: 0x04009E30 RID: 40496
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, AdminApiTokens.ApiToken> tokens = new CaseInsensitiveStringDictionary<AdminApiTokens.ApiToken>();

		// Token: 0x04009E31 RID: 40497
		[PublicizedFrom(EAccessModifier.Private)]
		public bool commandlineChecked;

		// Token: 0x04009E32 RID: 40498
		[PublicizedFrom(EAccessModifier.Private)]
		public string commandlineTokenName;

		// Token: 0x04009E33 RID: 40499
		[PublicizedFrom(EAccessModifier.Private)]
		public string commandlineTokenSecret;

		// Token: 0x02001B03 RID: 6915
		public readonly struct ApiToken
		{
			// Token: 0x0600CFDA RID: 53210 RVA: 0x004BD63C File Offset: 0x004BB83C
			public ApiToken(string _name, string _secret, int _permissionLevel)
			{
				this.Name = _name;
				this.Secret = _secret;
				this.PermissionLevel = _permissionLevel;
			}

			// Token: 0x0600CFDB RID: 53211 RVA: 0x004BD654 File Offset: 0x004BB854
			public void ToXml(XmlElement _parent)
			{
				_parent.AddXmlElement("token").SetAttrib("name", this.Name).SetAttrib("secret", this.Secret).SetAttrib("permission_level", this.PermissionLevel.ToString());
			}

			// Token: 0x0600CFDC RID: 53212 RVA: 0x004BD6A8 File Offset: 0x004BB8A8
			public static bool TryParse(XmlElement _element, out AdminApiTokens.ApiToken _result)
			{
				_result = default(AdminApiTokens.ApiToken);
				string name;
				if (!_element.TryGetAttribute("name", out name))
				{
					Log.Warning("[Web] [Perms] Ignoring apitoken-entry because of missing 'name' attribute: " + _element.OuterXml);
					return false;
				}
				string secret;
				if (!_element.TryGetAttribute("secret", out secret))
				{
					Log.Warning("[Web] [Perms] Ignoring apitoken-entry because of missing 'secret' attribute: " + _element.OuterXml);
					return false;
				}
				string s;
				if (!_element.TryGetAttribute("permission_level", out s))
				{
					Log.Warning("[Web] [Perms] Ignoring apitoken-entry because of missing 'permission_level' attribute: " + _element.OuterXml);
					return false;
				}
				int permissionLevel;
				if (!int.TryParse(s, out permissionLevel))
				{
					Log.Warning("[Web] [Perms] Ignoring apitoken-entry because of invalid (non-numeric) value for 'permission_level' attribute: " + _element.OuterXml);
					return false;
				}
				_result = new AdminApiTokens.ApiToken(name, secret, permissionLevel);
				return true;
			}

			// Token: 0x04009E34 RID: 40500
			public readonly string Name;

			// Token: 0x04009E35 RID: 40501
			public readonly string Secret;

			// Token: 0x04009E36 RID: 40502
			public readonly int PermissionLevel;
		}
	}
}
