using System;
using System.Collections.Generic;
using System.Xml;

// Token: 0x0200008F RID: 143
public class AdminUsers : AdminSectionAbs
{
	// Token: 0x060002CC RID: 716 RVA: 0x00015B44 File Offset: 0x00013D44
	public AdminUsers(AdminTools _parent) : base(_parent, "users")
	{
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00015B68 File Offset: 0x00013D68
	public override void Clear()
	{
		this.userPermissions.Clear();
		this.groupPermissions.Clear();
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00015B80 File Offset: 0x00013D80
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminUsers.UserPermission userPermission;
		if (_childElement.Name == "group")
		{
			AdminUsers.GroupPermission groupPermission;
			if (AdminUsers.GroupPermission.TryParse(_childElement, out groupPermission))
			{
				this.groupPermissions[groupPermission.SteamIdGroup] = groupPermission;
				return;
			}
		}
		else if (AdminUsers.UserPermission.TryParse(_childElement, out userPermission))
		{
			this.userPermissions[userPermission.UserIdentifier] = userPermission;
		}
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00015BD8 File Offset: 0x00013DD8
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement(this.SectionTypeName);
		xmlElement.AddXmlComment(" <user platform=\"Steam\" userid=\"76561198021925107\" name=\"Hint on who this user is\" permission_level=\"0\" /> ");
		xmlElement.AddXmlComment(" <group steamID=\"103582791434672565\" name=\"Steam Universe\" permission_level_default=\"1000\" permission_level_mod=\"0\" /> ");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminUsers.UserPermission> keyValuePair in this.userPermissions)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
		foreach (KeyValuePair<string, AdminUsers.GroupPermission> keyValuePair2 in this.groupPermissions)
		{
			keyValuePair2.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00015CA8 File Offset: 0x00013EA8
	public void AddUser(string _name, PlatformUserIdentifierAbs _identifier, int _permissionLevel)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminUsers.UserPermission value = new AdminUsers.UserPermission(_name, _identifier, _permissionLevel);
			this.userPermissions[_identifier] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00015D04 File Offset: 0x00013F04
	public bool RemoveUser(PlatformUserIdentifierAbs _identifier, bool _save = true)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.userPermissions.Remove(_identifier);
			if (flag2 && _save)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00015D5C File Offset: 0x00013F5C
	public bool HasEntry(ClientInfo _clientInfo)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			result = (this.userPermissions.ContainsKey(_clientInfo.PlatformId) || this.userPermissions.ContainsKey(_clientInfo.CrossplatformId));
		}
		return result;
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x00015DC0 File Offset: 0x00013FC0
	public Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission> GetUsers()
	{
		AdminTools parent = this.Parent;
		Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission> result;
		lock (parent)
		{
			result = this.userPermissions;
		}
		return result;
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00015E04 File Offset: 0x00014004
	public void AddGroup(string _name, string _steamId, int _permissionLevelDefault, int _permissionLevelMod)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminUsers.GroupPermission value = new AdminUsers.GroupPermission(_name, _steamId, _permissionLevelDefault, _permissionLevelMod);
			this.groupPermissions[_steamId] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00015E64 File Offset: 0x00014064
	public bool RemoveGroup(string _steamId)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.groupPermissions.Remove(_steamId);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00015EBC File Offset: 0x000140BC
	public Dictionary<string, AdminUsers.GroupPermission> GetGroups()
	{
		AdminTools parent = this.Parent;
		Dictionary<string, AdminUsers.GroupPermission> result;
		lock (parent)
		{
			result = this.groupPermissions;
		}
		return result;
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00015F00 File Offset: 0x00014100
	public int GetUserPermissionLevel(PlatformUserIdentifierAbs _userId)
	{
		AdminTools parent = this.Parent;
		int result;
		lock (parent)
		{
			AdminUsers.UserPermission userPermission;
			if (this.userPermissions.TryGetValue(_userId, out userPermission))
			{
				result = userPermission.PermissionLevel;
			}
			else
			{
				result = 1000;
			}
		}
		return result;
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00015F5C File Offset: 0x0001415C
	public int GetUserPermissionLevel(ClientInfo _clientInfo)
	{
		AdminTools parent = this.Parent;
		int result;
		lock (parent)
		{
			int num = 1000;
			AdminUsers.UserPermission userPermission;
			if (this.userPermissions.TryGetValue(_clientInfo.PlatformId, out userPermission))
			{
				num = userPermission.PermissionLevel;
			}
			AdminUsers.UserPermission userPermission2;
			if (_clientInfo.CrossplatformId != null && this.userPermissions.TryGetValue(_clientInfo.CrossplatformId, out userPermission2))
			{
				num = Math.Min(num, userPermission2.PermissionLevel);
			}
			if (this.groupPermissions.Count > 0 && _clientInfo.groupMemberships.Count > 0)
			{
				int num2 = int.MaxValue;
				foreach (KeyValuePair<string, int> keyValuePair in _clientInfo.groupMemberships)
				{
					AdminUsers.GroupPermission groupPermission;
					if (this.groupPermissions.TryGetValue(keyValuePair.Key, out groupPermission))
					{
						num2 = Math.Min(num2, (keyValuePair.Value == 2) ? groupPermission.PermissionLevelMods : groupPermission.PermissionLevelNormal);
					}
				}
				num = Math.Min(num, num2);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x0400036F RID: 879
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission> userPermissions = new Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission>();

	// Token: 0x04000370 RID: 880
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AdminUsers.GroupPermission> groupPermissions = new Dictionary<string, AdminUsers.GroupPermission>();

	// Token: 0x02000090 RID: 144
	public readonly struct UserPermission
	{
		// Token: 0x060002D9 RID: 729 RVA: 0x00016090 File Offset: 0x00014290
		public UserPermission(string _name, PlatformUserIdentifierAbs _userIdentifier, int _permissionLevel)
		{
			this.Name = _name;
			this.UserIdentifier = _userIdentifier;
			this.PermissionLevel = _permissionLevel;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x000160A8 File Offset: 0x000142A8
		public void ToXml(XmlElement _parent)
		{
			XmlElement xmlElement = _parent.AddXmlElement("user");
			this.UserIdentifier.ToXml(xmlElement, "");
			if (this.Name != null)
			{
				xmlElement.SetAttrib("name", this.Name);
			}
			xmlElement.SetAttrib("permission_level", this.PermissionLevel.ToString());
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00016108 File Offset: 0x00014308
		public static bool TryParse(XmlElement _element, out AdminUsers.UserPermission _result)
		{
			_result = default(AdminUsers.UserPermission);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("permission_level"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			int permissionLevel;
			if (!int.TryParse(_element.GetAttribute("permission_level"), out permissionLevel))
			{
				Log.Warning("Ignoring admin-entry because of invalid (non-numeric) value for 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			PlatformUserIdentifierAbs platformUserIdentifierAbs = AdminTools.ParseUserIdentifier(_element);
			if (platformUserIdentifierAbs == null)
			{
				return false;
			}
			_result = new AdminUsers.UserPermission(text, platformUserIdentifierAbs, permissionLevel);
			return true;
		}

		// Token: 0x04000371 RID: 881
		public readonly string Name;

		// Token: 0x04000372 RID: 882
		public readonly PlatformUserIdentifierAbs UserIdentifier;

		// Token: 0x04000373 RID: 883
		public readonly int PermissionLevel;
	}

	// Token: 0x02000091 RID: 145
	public readonly struct GroupPermission
	{
		// Token: 0x060002DC RID: 732 RVA: 0x0001619C File Offset: 0x0001439C
		public GroupPermission(string _name, string _steamIdGroup, int _permissionLevelNormal, int _permissionLevelMods)
		{
			this.Name = _name;
			this.SteamIdGroup = _steamIdGroup;
			this.PermissionLevelNormal = _permissionLevelNormal;
			this.PermissionLevelMods = _permissionLevelMods;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x000161BC File Offset: 0x000143BC
		public void ToXml(XmlElement _parent)
		{
			XmlElement element = _parent.AddXmlElement("group");
			element.SetAttrib("steamID", this.SteamIdGroup);
			if (this.Name != null)
			{
				element.SetAttrib("name", this.Name);
			}
			element.SetAttrib("permission_level_default", this.PermissionLevelNormal.ToString());
			element.SetAttrib("permission_level_mod", this.PermissionLevelMods.ToString());
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00016238 File Offset: 0x00014438
		public static bool TryParse(XmlElement _element, out AdminUsers.GroupPermission _result)
		{
			_result = default(AdminUsers.GroupPermission);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("steamID"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'steamID' attribute: " + _element.OuterXml);
				return false;
			}
			string attribute = _element.GetAttribute("steamID");
			if (!_element.HasAttribute("permission_level_default"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'permission_level_default' attribute on group: " + _element.OuterXml);
				return false;
			}
			int permissionLevelNormal;
			if (!int.TryParse(_element.GetAttribute("permission_level_default"), out permissionLevelNormal))
			{
				Log.Warning("Ignoring admin-entry because of invalid (non-numeric) value for 'permission_level_default' attribute on group: " + _element.OuterXml);
				return false;
			}
			if (!_element.HasAttribute("permission_level_mod"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'permission_level_mod' attribute on group: " + _element.OuterXml);
				return false;
			}
			int permissionLevelMods;
			if (!int.TryParse(_element.GetAttribute("permission_level_mod"), out permissionLevelMods))
			{
				Log.Warning("Ignoring admin-entry because of invalid (non-numeric) value for 'permission_level_mod' attribute on group: " + _element.OuterXml);
				return false;
			}
			_result = new AdminUsers.GroupPermission(text, attribute, permissionLevelNormal, permissionLevelMods);
			return true;
		}

		// Token: 0x04000374 RID: 884
		public readonly string Name;

		// Token: 0x04000375 RID: 885
		public readonly string SteamIdGroup;

		// Token: 0x04000376 RID: 886
		public readonly int PermissionLevelNormal;

		// Token: 0x04000377 RID: 887
		public readonly int PermissionLevelMods;
	}
}
