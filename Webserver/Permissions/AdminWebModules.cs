using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using UnityEngine;

namespace Webserver.Permissions
{
	// Token: 0x02001B04 RID: 6916
	public class AdminWebModules : AdminSectionAbs
	{
		// Token: 0x17001991 RID: 6545
		// (get) Token: 0x0600CFDD RID: 53213 RVA: 0x004BD75E File Offset: 0x004BB95E
		public static AdminWebModules Instance
		{
			get
			{
				return GameManager.Instance.adminTools.WebModules;
			}
		}

		// Token: 0x0600CFDE RID: 53214 RVA: 0x004BD770 File Offset: 0x004BB970
		public AdminWebModules(AdminTools _parent) : base(_parent, "webmodules")
		{
		}

		// Token: 0x0600CFDF RID: 53215 RVA: 0x004BD7BC File Offset: 0x004BB9BC
		public override void Clear()
		{
			this.allModulesList.Clear();
			this.modules.Clear();
		}

		// Token: 0x0600CFE0 RID: 53216 RVA: 0x004BD7D4 File Offset: 0x004BB9D4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void ParseElement(XmlElement _childElement)
		{
			this.allModulesList.Clear();
			AdminWebModules.WebModule webModule;
			if (!AdminWebModules.WebModule.TryParse(_childElement, out webModule))
			{
				return;
			}
			AdminWebModules.WebModule knownModule;
			if (this.knownModules.TryGetValue(webModule.Name, out knownModule))
			{
				webModule = webModule.FixPermissionLevelsFromKnownModule(knownModule);
			}
			this.modules[webModule.Name] = webModule;
		}

		// Token: 0x0600CFE1 RID: 53217 RVA: 0x004BD828 File Offset: 0x004BBA28
		public override void Save(XmlElement _root)
		{
			XmlElement parent = _root.AddXmlElement(this.SectionTypeName);
			foreach (KeyValuePair<string, AdminWebModules.WebModule> keyValuePair in this.modules)
			{
				string text;
				AdminWebModules.WebModule webModule;
				keyValuePair.Deconstruct(out text, out webModule);
				AdminWebModules.WebModule webModule2 = webModule;
				webModule2.ToXml(parent);
			}
		}

		// Token: 0x0600CFE2 RID: 53218 RVA: 0x004BD898 File Offset: 0x004BBA98
		public void AddModule(AdminWebModules.WebModule _module)
		{
			AdminTools parent = this.Parent;
			lock (parent)
			{
				this.allModulesList.Clear();
				this.modules[_module.Name] = _module;
				this.Parent.Save();
			}
		}

		// Token: 0x0600CFE3 RID: 53219 RVA: 0x004BD8FC File Offset: 0x004BBAFC
		public bool RemoveModule(string _module)
		{
			AdminTools parent = this.Parent;
			bool result;
			lock (parent)
			{
				this.allModulesList.Clear();
				bool flag2 = this.modules.Remove(_module);
				if (flag2)
				{
					this.Parent.Save();
				}
				result = flag2;
			}
			return result;
		}

		// Token: 0x0600CFE4 RID: 53220 RVA: 0x004BD960 File Offset: 0x004BBB60
		public List<AdminWebModules.WebModule> GetModules()
		{
			AdminTools parent = this.Parent;
			List<AdminWebModules.WebModule> result;
			lock (parent)
			{
				if (this.allModulesList.Count != 0)
				{
					result = this.allModulesList;
				}
				else
				{
					foreach (KeyValuePair<string, AdminWebModules.WebModule> keyValuePair in this.knownModules)
					{
						string text;
						AdminWebModules.WebModule webModule;
						keyValuePair.Deconstruct(out text, out webModule);
						string key = text;
						AdminWebModules.WebModule webModule2 = webModule;
						AdminWebModules.WebModule webModule3;
						this.allModulesList.Add(this.modules.TryGetValue(key, out webModule3) ? webModule3 : webModule2);
					}
					result = this.allModulesList;
				}
			}
			return result;
		}

		// Token: 0x0600CFE5 RID: 53221 RVA: 0x004BDA2C File Offset: 0x004BBC2C
		public void AddKnownModule(AdminWebModules.WebModule _module)
		{
			if (!_module.IsDefault)
			{
				Log.Warning("Call to AddKnownModule with IsDefault==false! From:\n" + StackTraceUtility.ExtractStackTrace());
			}
			if (string.IsNullOrEmpty(_module.Name))
			{
				return;
			}
			AdminTools parent = this.Parent;
			lock (parent)
			{
				this.allModulesList.Clear();
				this.knownModules[_module.Name] = _module;
				AdminWebModules.WebModule value;
				if (this.modules.TryGetValue(_module.Name, out value))
				{
					value = value.FixPermissionLevelsFromKnownModule(_module);
					this.modules[_module.Name] = value;
				}
			}
		}

		// Token: 0x0600CFE6 RID: 53222 RVA: 0x004BDAE0 File Offset: 0x004BBCE0
		public bool IsKnownModule(string _module)
		{
			if (string.IsNullOrEmpty(_module))
			{
				return false;
			}
			AdminTools parent = this.Parent;
			bool result;
			lock (parent)
			{
				result = this.knownModules.ContainsKey(_module);
			}
			return result;
		}

		// Token: 0x0600CFE7 RID: 53223 RVA: 0x004BDB34 File Offset: 0x004BBD34
		public bool ModuleAllowedWithLevel(string _module, int _level)
		{
			AdminTools parent = this.Parent;
			bool result;
			lock (parent)
			{
				result = (this.GetModule(_module).LevelGlobal >= _level);
			}
			return result;
		}

		// Token: 0x0600CFE8 RID: 53224 RVA: 0x004BDB84 File Offset: 0x004BBD84
		public AdminWebModules.WebModule GetModule(string _module)
		{
			AdminTools parent = this.Parent;
			AdminWebModules.WebModule result;
			lock (parent)
			{
				AdminWebModules.WebModule webModule;
				if (this.modules.TryGetValue(_module, out webModule))
				{
					result = webModule;
				}
				else
				{
					result = (this.knownModules.TryGetValue(_module, out webModule) ? webModule : this.defaultModulePermission);
				}
			}
			return result;
		}

		// Token: 0x04009E37 RID: 40503
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, AdminWebModules.WebModule> modules = new CaseInsensitiveStringDictionary<AdminWebModules.WebModule>();

		// Token: 0x04009E38 RID: 40504
		public const int MethodLevelInheritGlobal = -2147483648;

		// Token: 0x04009E39 RID: 40505
		public const string MethodLevelInheritKeyword = "inherit";

		// Token: 0x04009E3A RID: 40506
		public const int MethodLevelNotSupported = -2147483647;

		// Token: 0x04009E3B RID: 40507
		public const int PermissionLevelUser = 1000;

		// Token: 0x04009E3C RID: 40508
		public const int PermissionLevelGuest = 2000;

		// Token: 0x04009E3D RID: 40509
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, AdminWebModules.WebModule> knownModules = new CaseInsensitiveStringDictionary<AdminWebModules.WebModule>();

		// Token: 0x04009E3E RID: 40510
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<AdminWebModules.WebModule> allModulesList = new List<AdminWebModules.WebModule>();

		// Token: 0x04009E3F RID: 40511
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AdminWebModules.WebModule defaultModulePermission = new AdminWebModules.WebModule("", 0, true);

		// Token: 0x02001B05 RID: 6917
		public readonly struct WebModule
		{
			// Token: 0x0600CFE9 RID: 53225 RVA: 0x004BDBF0 File Offset: 0x004BBDF0
			public WebModule(string _name, int _level, bool _isDefault)
			{
				this.LevelPerMethod = null;
				this.Name = _name;
				this.LevelGlobal = _level;
				this.IsDefault = _isDefault;
			}

			// Token: 0x0600CFEA RID: 53226 RVA: 0x004BDC10 File Offset: 0x004BBE10
			public WebModule(string _name, int _levelGlobal, int[] _levelPerMethod, bool _isDefault)
			{
				if (_levelPerMethod != null && _levelPerMethod.Length != 7)
				{
					this.LevelPerMethod = AdminWebModules.WebModule.createDefaultPerMethodArray();
					for (int i = 0; i < 7; i++)
					{
						if (_levelPerMethod != null && i < _levelPerMethod.Length)
						{
							this.LevelPerMethod[i] = _levelPerMethod[i];
						}
					}
				}
				else
				{
					this.LevelPerMethod = _levelPerMethod;
				}
				this.Name = _name;
				this.LevelGlobal = _levelGlobal;
				this.IsDefault = _isDefault;
			}

			// Token: 0x0600CFEB RID: 53227 RVA: 0x004BDC70 File Offset: 0x004BBE70
			public void ToXml(XmlElement _parent)
			{
				bool flag = this.LevelPerMethod != null;
				XmlElement node = _parent.AddXmlElement("module").SetAttrib("name", this.Name).SetAttrib("permission_level", this.LevelGlobal.ToString());
				if (!flag)
				{
					return;
				}
				for (int i = 0; i < this.LevelPerMethod.Length; i++)
				{
					ERequestMethod enumValue = (ERequestMethod)i;
					int num = this.LevelPerMethod[i];
					if (num != -2147483647)
					{
						node.AddXmlElement("method").SetAttrib("name", enumValue.ToStringCached<ERequestMethod>()).SetAttrib("permission_level", (num == int.MinValue) ? "inherit" : num.ToString());
					}
				}
			}

			// Token: 0x0600CFEC RID: 53228 RVA: 0x004BDD24 File Offset: 0x004BBF24
			public static bool TryParse(XmlElement _element, out AdminWebModules.WebModule _result)
			{
				_result = default(AdminWebModules.WebModule);
				string name;
				if (!_element.TryGetAttribute("name", out name))
				{
					Log.Warning("[Web] [Perms] Ignoring module-entry because of missing 'name' attribute: " + _element.OuterXml);
					return false;
				}
				string text;
				if (!_element.TryGetAttribute("permission_level", out text))
				{
					Log.Warning("[Web] [Perms] Ignoring module-entry because of missing 'permission_level' attribute: " + _element.OuterXml);
					return false;
				}
				int levelGlobal;
				if (!int.TryParse(text, out levelGlobal))
				{
					Log.Warning("[Web] [Perms] Ignoring module-entry because of invalid (non-numeric) value for 'permission_level' attribute: " + _element.OuterXml);
					return false;
				}
				int[] array = null;
				foreach (object obj in _element.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						string name2;
						ERequestMethod erequestMethod;
						if (xmlElement.Name != "method")
						{
							Log.Warning("[Web] [Perms] Ignoring module child element, invalid element name: " + xmlElement.OuterXml);
						}
						else if (!xmlElement.TryGetAttribute("name", out name2))
						{
							Log.Warning("[Web] [Perms] Ignoring module child element, missing 'name' attribute: " + xmlElement.OuterXml);
						}
						else if (!EnumUtils.TryParse<ERequestMethod>(name2, out erequestMethod, true))
						{
							Log.Warning("[Web] [Perms] Ignoring module child element, unknown method name in 'name' attribute: " + xmlElement.OuterXml);
						}
						else if (erequestMethod >= ERequestMethod.Count)
						{
							Log.Warning("[Web] [Perms] Ignoring module child element, invalid method name in 'name' attribute: " + xmlElement.OuterXml);
						}
						else if (!xmlElement.TryGetAttribute("permission_level", out text))
						{
							Log.Warning("[Web] [Perms] Ignoring module child element, missing 'permission_level' attribute: " + xmlElement.OuterXml);
						}
						else
						{
							int minValue;
							if (text.EqualsCaseInsensitive("inherit"))
							{
								minValue = int.MinValue;
							}
							else if (!int.TryParse(text, out minValue))
							{
								Log.Warning("[Web] [Perms] Ignoring module child element, invalid (non-numeric) value for 'permission_level' attribute: " + xmlElement.OuterXml);
								continue;
							}
							if (array == null)
							{
								array = AdminWebModules.WebModule.createDefaultPerMethodArray();
							}
							array[(int)erequestMethod] = minValue;
						}
					}
				}
				_result = new AdminWebModules.WebModule(name, levelGlobal, array, false);
				return true;
			}

			// Token: 0x0600CFED RID: 53229 RVA: 0x004BDF38 File Offset: 0x004BC138
			[MustUseReturnValue]
			public AdminWebModules.WebModule SetLevelGlobal(int _level)
			{
				int[] array = (this.LevelPerMethod == null) ? null : new int[this.LevelPerMethod.Length];
				if (array != null)
				{
					Array.Copy(this.LevelPerMethod, array, array.Length);
				}
				return new AdminWebModules.WebModule(this.Name, _level, array, false);
			}

			// Token: 0x0600CFEE RID: 53230 RVA: 0x004BDF80 File Offset: 0x004BC180
			[MustUseReturnValue]
			public AdminWebModules.WebModule SetLevelForMethod(ERequestMethod _method, int _level)
			{
				int[] array = AdminWebModules.WebModule.createDefaultPerMethodArray();
				if (this.LevelPerMethod != null)
				{
					Array.Copy(this.LevelPerMethod, array, array.Length);
				}
				array[(int)_method] = _level;
				return new AdminWebModules.WebModule(this.Name, this.LevelGlobal, array, false);
			}

			// Token: 0x0600CFEF RID: 53231 RVA: 0x004BDFC4 File Offset: 0x004BC1C4
			[PublicizedFrom(EAccessModifier.Private)]
			public static int[] createDefaultPerMethodArray()
			{
				int[] array = new int[7];
				for (int i = 0; i < 7; i++)
				{
					array[i] = -2147483647;
				}
				return array;
			}

			// Token: 0x0600CFF0 RID: 53232 RVA: 0x004BDFF0 File Offset: 0x004BC1F0
			[MustUseReturnValue]
			public AdminWebModules.WebModule FixPermissionLevelsFromKnownModule(AdminWebModules.WebModule _knownModule)
			{
				if (_knownModule.LevelPerMethod != null)
				{
					AdminWebModules.WebModule webModule = this;
					for (int i = 0; i < _knownModule.LevelPerMethod.Length; i++)
					{
						if (webModule.LevelPerMethod == null || webModule.LevelPerMethod[i] == -2147483647)
						{
							webModule = webModule.SetLevelForMethod((ERequestMethod)i, _knownModule.LevelPerMethod[i]);
						}
					}
					return webModule;
				}
				if (this.LevelPerMethod != null)
				{
					return new AdminWebModules.WebModule(this.Name, this.LevelGlobal, false);
				}
				return this;
			}

			// Token: 0x04009E40 RID: 40512
			public readonly string Name;

			// Token: 0x04009E41 RID: 40513
			public readonly int LevelGlobal;

			// Token: 0x04009E42 RID: 40514
			public readonly int[] LevelPerMethod;

			// Token: 0x04009E43 RID: 40515
			public readonly bool IsDefault;
		}
	}
}
