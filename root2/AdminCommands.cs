using System;
using System.Collections.Generic;
using System.Xml;

// Token: 0x0200008B RID: 139
public class AdminCommands : AdminSectionAbs
{
	// Token: 0x060002AC RID: 684 RVA: 0x00014DCF File Offset: 0x00012FCF
	public AdminCommands(AdminTools _parent) : base(_parent, "commands")
	{
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00014DF9 File Offset: 0x00012FF9
	public override void Clear()
	{
		this.commands.Clear();
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00014E08 File Offset: 0x00013008
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminCommands.CommandPermission commandPermission;
		if (AdminCommands.CommandPermission.TryParse(_childElement, out commandPermission))
		{
			this.commands[commandPermission.Command] = commandPermission;
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00014E34 File Offset: 0x00013034
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement(this.SectionTypeName);
		xmlElement.AddXmlComment(" <permission cmd=\"dm\" permission_level=\"0\" /> ");
		xmlElement.AddXmlComment(" <permission cmd=\"kick\" permission_level=\"1\" /> ");
		xmlElement.AddXmlComment(" <permission cmd=\"say\" permission_level=\"1000\" /> ");
		foreach (KeyValuePair<string, AdminCommands.CommandPermission> keyValuePair in this.commands)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00014EC4 File Offset: 0x000130C4
	public void AddCommand(string _cmd, int _permissionLevel, bool _save = true)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminCommands.CommandPermission value = new AdminCommands.CommandPermission(_cmd, _permissionLevel);
			this.commands[_cmd] = value;
			if (_save)
			{
				this.Parent.Save();
			}
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x00014F24 File Offset: 0x00013124
	public bool RemoveCommand(string[] _cmds)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = false;
			foreach (string key in _cmds)
			{
				flag2 |= this.commands.Remove(key);
			}
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00014FA0 File Offset: 0x000131A0
	public bool IsPermissionDefined(string[] _cmds)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			foreach (string key in _cmds)
			{
				if (this.commands.ContainsKey(key))
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00015008 File Offset: 0x00013208
	public Dictionary<string, AdminCommands.CommandPermission> GetCommands()
	{
		AdminTools parent = this.Parent;
		Dictionary<string, AdminCommands.CommandPermission> result;
		lock (parent)
		{
			result = this.commands;
		}
		return result;
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0001504C File Offset: 0x0001324C
	public int GetCommandPermissionLevel(string[] _cmdNames)
	{
		AdminTools parent = this.Parent;
		int permissionLevel;
		lock (parent)
		{
			permissionLevel = this.GetAdminToolsCommandPermission(_cmdNames).PermissionLevel;
		}
		return permissionLevel;
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00015094 File Offset: 0x00013294
	public AdminCommands.CommandPermission GetAdminToolsCommandPermission(string[] _cmdNames)
	{
		AdminTools parent = this.Parent;
		AdminCommands.CommandPermission result;
		lock (parent)
		{
			foreach (string text in _cmdNames)
			{
				if (!string.IsNullOrEmpty(text) && this.commands.ContainsKey(text))
				{
					return this.commands[text];
				}
			}
			result = this.defaultCommandPermission;
		}
		return result;
	}

	// Token: 0x0400035C RID: 860
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AdminCommands.CommandPermission> commands = new Dictionary<string, AdminCommands.CommandPermission>();

	// Token: 0x0400035D RID: 861
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AdminCommands.CommandPermission defaultCommandPermission = new AdminCommands.CommandPermission("", 0);

	// Token: 0x0200008C RID: 140
	public readonly struct CommandPermission
	{
		// Token: 0x060002B6 RID: 694 RVA: 0x00015118 File Offset: 0x00013318
		public CommandPermission(string _cmd, int _permissionLevel)
		{
			this.Command = _cmd;
			this.PermissionLevel = _permissionLevel;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00015128 File Offset: 0x00013328
		public void ToXml(XmlElement _parent)
		{
			_parent.AddXmlElement("permission").SetAttrib("cmd", this.Command).SetAttrib("permission_level", this.PermissionLevel.ToString());
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0001516C File Offset: 0x0001336C
		public static bool TryParse(XmlElement _element, out AdminCommands.CommandPermission _result)
		{
			_result = default(AdminCommands.CommandPermission);
			string attribute = _element.GetAttribute("cmd");
			if (string.IsNullOrEmpty(attribute))
			{
				Log.Warning("Ignoring permission-entry because of missing or empty 'cmd' attribute: " + _element.OuterXml);
				return false;
			}
			if (!_element.HasAttribute("permission_level"))
			{
				Log.Warning("Ignoring permission-entry because of missing 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			int permissionLevel;
			if (!int.TryParse(_element.GetAttribute("permission_level"), out permissionLevel))
			{
				Log.Warning("Ignoring permission-entry because of invalid (non-numeric) value for 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			_result = new AdminCommands.CommandPermission(attribute, permissionLevel);
			return true;
		}

		// Token: 0x0400035E RID: 862
		public readonly string Command;

		// Token: 0x0400035F RID: 863
		public readonly int PermissionLevel;
	}
}
