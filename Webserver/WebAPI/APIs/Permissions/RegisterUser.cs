using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.Permissions;
using Webserver.UrlHandlers;

namespace Webserver.WebAPI.APIs.Permissions
{
	// Token: 0x02001AE8 RID: 6888
	[Preserve]
	public class RegisterUser : AbsRestApi
	{
		// Token: 0x0600CF38 RID: 53048 RVA: 0x004B7ECC File Offset: 0x004B60CC
		public RegisterUser(Web _parentWeb) : base(_parentWeb, null)
		{
		}

		// Token: 0x0600CF39 RID: 53049 RVA: 0x004B98C4 File Offset: 0x004B7AC4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			if (string.IsNullOrEmpty(requestPath))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, null, EApiErrorCode.MISSING_TOKEN, null);
				return;
			}
			UserRegistrationTokens.RegistrationData registrationData;
			if (!UserRegistrationTokens.TryValidate(requestPath, out registrationData))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.NotFound, null, EApiErrorCode.INVALID_OR_EXPIRED_TOKEN, null);
				return;
			}
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(RegisterUser.jsonPlayerNameKey);
			jsonWriter.WriteString(registrationData.PlayerName);
			jsonWriter.WriteRaw(RegisterUser.jsonExpirationKey);
			jsonWriter.WriteDouble((registrationData.ExpiryTime - DateTime.Now).TotalSeconds);
			jsonWriter.WriteEndObject();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CF3A RID: 53050 RVA: 0x004B996C File Offset: 0x004B7B6C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestPost(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			string token;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "token", out token))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.MISSING_TOKEN, null);
				return;
			}
			string text;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "username", out text))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.MISSING_USERNAME, null);
				return;
			}
			string text2;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "password", out text2))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.MISSING_PASSWORD, null);
				return;
			}
			UserRegistrationTokens.RegistrationData registrationData;
			if (!UserRegistrationTokens.TryValidate(token, out registrationData))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Unauthorized, null, EApiErrorCode.INVALID_OR_EXPIRED_TOKEN, null);
				return;
			}
			if (!RegisterUser.userValidationRegex.IsMatch(text))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Unauthorized, _jsonInputData, EApiErrorCode.INVALID_USERNAME, null);
				return;
			}
			if (!RegisterUser.passValidationRegex.IsMatch(text2))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Unauthorized, _jsonInputData, EApiErrorCode.INVALID_PASSWORD, null);
				return;
			}
			AdminWebUsers.WebUser webUser;
			if (AdminWebUsers.Instance.GetUsers().TryGetValue(text, out webUser) && (!object.Equals(webUser.PlatformUser, registrationData.PlatformUserId) || !object.Equals(webUser.CrossPlatformUser, registrationData.CrossPlatformUserId)))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Unauthorized, _jsonInputData, EApiErrorCode.DUPLICATE_USERNAME, null);
				return;
			}
			string text3 = (registrationData.CrossPlatformUserId == null) ? "" : (", crossplatform ID " + registrationData.CrossPlatformUserId.CombinedString);
			Log.Out(string.Concat(new string[]
			{
				"[Web] User registered: Username '",
				text,
				"' for platform ID ",
				registrationData.PlatformUserId.CombinedString,
				text3
			}));
			AdminWebUsers.WebUser webUser2;
			if (AdminWebUsers.Instance.HasUser(registrationData.PlatformUserId, registrationData.CrossPlatformUserId, out webUser2))
			{
				Log.Out("[Web] Re-registration, replacing existing username '" + webUser2.Name + "'");
				AdminWebUsers.Instance.RemoveUser(webUser2.Name);
			}
			AdminWebUsers.Instance.AddUser(text, text2, registrationData.PlatformUserId, registrationData.CrossPlatformUserId);
			string remoteEndpointString = _context.Request.RemoteEndPoint.ToString();
			SessionHandler.HandleUserIdLogin(this.ParentWeb.ConnectionHandler, _context, remoteEndpointString, "User/pass", text, registrationData.PlatformUserId, registrationData.CrossPlatformUserId);
			_context.Response.StatusCode = 201;
			_context.Response.ContentType = "text/plain";
			_context.Response.ContentEncoding = Encoding.UTF8;
			_context.Response.ContentLength64 = 0L;
		}

		// Token: 0x0600CF3B RID: 53051 RVA: 0x004B7F39 File Offset: 0x004B6139
		public override int DefaultPermissionLevel()
		{
			return 2000;
		}

		// Token: 0x04009D8F RID: 40335
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonPlayerNameKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("playerName");

		// Token: 0x04009D90 RID: 40336
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonExpirationKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("expirationSeconds");

		// Token: 0x04009D91 RID: 40337
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex userValidationRegex = new Regex("^\\w{4,16}$", RegexOptions.Compiled | RegexOptions.ECMAScript);

		// Token: 0x04009D92 RID: 40338
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex passValidationRegex = new Regex("^\\w{4,16}$", RegexOptions.Compiled | RegexOptions.ECMAScript);
	}
}
