using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.Permissions;

namespace Webserver.WebAPI.APIs.Permissions
{
	// Token: 0x02001AEB RID: 6891
	[Preserve]
	public class WebModules : AbsRestApi
	{
		// Token: 0x0600CF50 RID: 53072 RVA: 0x004BA1F4 File Offset: 0x004B83F4
		[PublicizedFrom(EAccessModifier.Private)]
		static WebModules()
		{
			for (int i = 0; i < WebModules.jsonMethodNameKeys.Length; i++)
			{
				ERequestMethod enumValue = (ERequestMethod)i;
				WebModules.jsonMethodNameKeys[i] = JsonWriter.GetEncodedPropertyName(enumValue.ToStringCached<ERequestMethod>());
			}
		}

		// Token: 0x17001982 RID: 6530
		// (get) Token: 0x0600CF51 RID: 53073 RVA: 0x004BA26E File Offset: 0x004B846E
		public static AdminWebModules ModulesInstance
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return AdminWebModules.Instance;
			}
		}

		// Token: 0x0600CF52 RID: 53074 RVA: 0x004BA278 File Offset: 0x004B8478
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			if (string.IsNullOrEmpty(requestPath))
			{
				jsonWriter.WriteBeginArray();
				bool flag = true;
				foreach (AdminWebModules.WebModule module in WebModules.ModulesInstance.GetModules())
				{
					if (!flag)
					{
						jsonWriter.WriteValueSeparator();
					}
					flag = false;
					this.writeModuleJson(ref jsonWriter, module);
				}
				jsonWriter.WriteEndArray();
				AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
				return;
			}
			jsonWriter.WriteRaw(WebUtils.JsonEmptyData);
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.BadRequest, null, null, null);
		}

		// Token: 0x0600CF53 RID: 53075 RVA: 0x004BA330 File Offset: 0x004B8530
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeModuleJson(ref JsonWriter _writer, AdminWebModules.WebModule _module)
		{
			_writer.WriteRaw(WebModules.jsonKeyModule);
			_writer.WriteString(_module.Name);
			_writer.WriteRaw(WebModules.jsonKeyPermissionLevelGlobal);
			_writer.WriteInt32(_module.LevelGlobal);
			_writer.WriteRaw(WebModules.jsonKeyPermissionLevelPerMethod);
			_writer.WriteBeginObject();
			if (_module.LevelPerMethod != null)
			{
				bool flag = true;
				for (int i = 0; i < _module.LevelPerMethod.Length; i++)
				{
					int num = _module.LevelPerMethod[i];
					if (num != -2147483647)
					{
						if (!flag)
						{
							_writer.WriteValueSeparator();
						}
						flag = false;
						_writer.WriteRaw(WebModules.jsonMethodNameKeys[i]);
						if (num == -2147483648)
						{
							_writer.WriteString("inherit");
						}
						else
						{
							_writer.WriteInt32(num);
						}
					}
				}
			}
			_writer.WriteEndObject();
			_writer.WriteRaw(WebModules.jsonKeyIsDefault);
			_writer.WriteBoolean(_module.IsDefault);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF54 RID: 53076 RVA: 0x004BA404 File Offset: 0x004B8604
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestPost(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			string requestPath = _context.RequestPath;
			if (string.IsNullOrEmpty(requestPath))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_MODULE, null);
				return;
			}
			if (!AdminWebModules.Instance.IsKnownModule(requestPath))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.INVALID_MODULE, null);
				return;
			}
			AdminWebModules.WebModule webModule = AdminWebModules.Instance.GetModule(requestPath);
			if (_jsonInput.ContainsKey("permissionLevelGlobal"))
			{
				int levelGlobal;
				if (!JsonCommons.TryGetJsonField(_jsonInput, "permissionLevelGlobal", out levelGlobal))
				{
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.INVALID_PERMISSION_LEVEL_GLOBAL, null);
					return;
				}
				webModule = webModule.SetLevelGlobal(levelGlobal);
			}
			object obj;
			if (_jsonInput.TryGetValue("permissionLevelPerMethod", out obj))
			{
				IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
				if (dictionary == null)
				{
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.INVALID_PERMISSION_LEVEL_PER_METHOD_PROPERTY, null);
					return;
				}
				foreach (KeyValuePair<string, object> keyValuePair in dictionary)
				{
					string text;
					object obj2;
					keyValuePair.Deconstruct(out text, out obj2);
					string name = text;
					object obj3 = obj2;
					ERequestMethod erequestMethod;
					if (!EnumUtils.TryParse<ERequestMethod>(name, out erequestMethod, true))
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.INVALID_METHOD_NAME, null);
						return;
					}
					if (webModule.LevelPerMethod == null || webModule.LevelPerMethod[(int)erequestMethod] == -2147483647)
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.UNSUPPORTED_METHOD, null);
						return;
					}
					string text2 = obj3 as string;
					int level;
					if (text2 == null)
					{
						if (obj3 is double)
						{
							double num = (double)obj3;
							try
							{
								level = (int)num;
								goto IL_191;
							}
							catch (Exception)
							{
								AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.INVALID_PERMISSION_VALUE, null);
								return;
							}
						}
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.INVALID_PERMISSION_VALUE_TYPE, null);
						return;
					}
					if (!text2.EqualsCaseInsensitive("inherit"))
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.INVALID_PERMISSION_STRING, null);
						return;
					}
					level = int.MinValue;
					IL_191:
					webModule = webModule.SetLevelForMethod(erequestMethod, level);
				}
			}
			WebModules.ModulesInstance.AddModule(webModule);
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Created, null, null, null);
		}

		// Token: 0x0600CF55 RID: 53077 RVA: 0x004BA600 File Offset: 0x004B8800
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestDelete(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			AbsRestApi.SendEmptyResponse(_context, WebModules.ModulesInstance.RemoveModule(requestPath) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound, null, null, null);
		}

		// Token: 0x17001983 RID: 6531
		// (get) Token: 0x0600CF56 RID: 53078 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool AllowPostWithId
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return true;
			}
		}

		// Token: 0x0600CF57 RID: 53079 RVA: 0x004B9519 File Offset: 0x004B7719
		public override int[] DefaultMethodPermissionLevels()
		{
			return new int[]
			{
				-2147483647,
				int.MinValue,
				int.MinValue,
				-2147483647,
				int.MinValue
			};
		}

		// Token: 0x0600CF58 RID: 53080 RVA: 0x004B25BC File Offset: 0x004B07BC
		public WebModules() : base(null)
		{
		}

		// Token: 0x04009DA7 RID: 40359
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyModule = "module";

		// Token: 0x04009DA8 RID: 40360
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyPermissionLevelGlobal = "permissionLevelGlobal";

		// Token: 0x04009DA9 RID: 40361
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyPermissionLevelPerMethod = "permissionLevelPerMethod";

		// Token: 0x04009DAA RID: 40362
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyIsDefault = "isDefault";

		// Token: 0x04009DAB RID: 40363
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyModule = JsonWriter.GetEncodedPropertyNameWithBeginObject("module");

		// Token: 0x04009DAC RID: 40364
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPermissionLevelGlobal = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("permissionLevelGlobal");

		// Token: 0x04009DAD RID: 40365
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPermissionLevelPerMethod = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("permissionLevelPerMethod");

		// Token: 0x04009DAE RID: 40366
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyIsDefault = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("isDefault");

		// Token: 0x04009DAF RID: 40367
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[][] jsonMethodNameKeys = new byte[7][];
	}
}
