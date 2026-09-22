using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.Permissions;

namespace Webserver.WebAPI.APIs.Permissions
{
	// Token: 0x02001AEA RID: 6890
	[Preserve]
	public class WebApiTokens : AbsRestApi
	{
		// Token: 0x17001980 RID: 6528
		// (get) Token: 0x0600CF47 RID: 53063 RVA: 0x004B9FE1 File Offset: 0x004B81E1
		public static AdminApiTokens ApiTokensInstance
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return AdminApiTokens.Instance;
			}
		}

		// Token: 0x0600CF48 RID: 53064 RVA: 0x004B9FE8 File Offset: 0x004B81E8
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
				foreach (KeyValuePair<string, AdminApiTokens.ApiToken> keyValuePair in WebApiTokens.ApiTokensInstance.GetTokens())
				{
					string text;
					AdminApiTokens.ApiToken apiToken;
					keyValuePair.Deconstruct(out text, out apiToken);
					AdminApiTokens.ApiToken token = apiToken;
					if (!flag)
					{
						jsonWriter.WriteValueSeparator();
					}
					flag = false;
					this.writeTokenJson(ref jsonWriter, token);
				}
				jsonWriter.WriteEndArray();
				AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
				return;
			}
			jsonWriter.WriteRaw(WebUtils.JsonEmptyData);
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.BadRequest, null, null, null);
		}

		// Token: 0x0600CF49 RID: 53065 RVA: 0x004BA0B0 File Offset: 0x004B82B0
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeTokenJson(ref JsonWriter _writer, AdminApiTokens.ApiToken _token)
		{
			_writer.WriteRaw(WebApiTokens.jsonKeyName);
			_writer.WriteString(_token.Name);
			_writer.WriteRaw(WebApiTokens.jsonKeySecret);
			_writer.WriteString(_token.Secret);
			_writer.WriteRaw(WebApiTokens.jsonKeyPermissionLevel);
			_writer.WriteInt32(_token.PermissionLevel);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF4A RID: 53066 RVA: 0x004BA108 File Offset: 0x004B8308
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestPost(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			string requestPath = _context.RequestPath;
			if (string.IsNullOrEmpty(requestPath))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_NAME, null);
				return;
			}
			string secret;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "secret", out secret))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_OR_INVALID_SECRET, null);
				return;
			}
			int permissionLevel;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "permissionLevel", out permissionLevel))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_OR_INVALID_PERMISSION_LEVEL, null);
				return;
			}
			WebApiTokens.ApiTokensInstance.AddToken(requestPath, secret, permissionLevel);
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Created, null, null, null);
		}

		// Token: 0x0600CF4B RID: 53067 RVA: 0x004BA18C File Offset: 0x004B838C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestDelete(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			AbsRestApi.SendEmptyResponse(_context, WebApiTokens.ApiTokensInstance.RemoveToken(requestPath) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound, null, null, null);
		}

		// Token: 0x17001981 RID: 6529
		// (get) Token: 0x0600CF4C RID: 53068 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool AllowPostWithId
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return true;
			}
		}

		// Token: 0x0600CF4D RID: 53069 RVA: 0x004B9519 File Offset: 0x004B7719
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

		// Token: 0x0600CF4E RID: 53070 RVA: 0x004B25BC File Offset: 0x004B07BC
		public WebApiTokens() : base(null)
		{
		}

		// Token: 0x04009DA1 RID: 40353
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyName = "name";

		// Token: 0x04009DA2 RID: 40354
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertySecret = "secret";

		// Token: 0x04009DA3 RID: 40355
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyPermissionLevel = "permissionLevel";

		// Token: 0x04009DA4 RID: 40356
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyName = JsonWriter.GetEncodedPropertyNameWithBeginObject("name");

		// Token: 0x04009DA5 RID: 40357
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeySecret = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("secret");

		// Token: 0x04009DA6 RID: 40358
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPermissionLevel = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("permissionLevel");
	}
}
