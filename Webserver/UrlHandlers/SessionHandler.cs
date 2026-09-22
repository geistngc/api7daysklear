using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Platform.Steam;
using Utf8Json;
using Webserver.Permissions;

namespace Webserver.UrlHandlers
{
	// Token: 0x02001AF9 RID: 6905
	public class SessionHandler : AbsHandler
	{
		// Token: 0x0600CFA9 RID: 53161 RVA: 0x004BC10D File Offset: 0x004BA30D
		public SessionHandler() : base(null, 0)
		{
		}

		// Token: 0x0600CFAA RID: 53162 RVA: 0x004BC118 File Offset: 0x004BA318
		public override void HandleRequest(RequestContext _context)
		{
			if (_context.Request.RemoteEndPoint == null)
			{
				WebUtils.WriteText(_context.Response, "NoRemoteEndpoint", HttpStatusCode.BadRequest, null);
				return;
			}
			string text = _context.RequestPath.Remove(0, this.urlBasePath.Length);
			string remoteEndpointString = _context.Request.RemoteEndPoint.ToString();
			if (text.StartsWith("verifysteamopenid"))
			{
				if (SessionHandler.HandleSteamVerification(this.parent.ConnectionHandler, _context, remoteEndpointString))
				{
					_context.Response.Redirect("/app");
					return;
				}
				_context.Response.Redirect("/app/error/SteamLoginFailed");
				return;
			}
			else
			{
				if (text.StartsWith("logout"))
				{
					SessionHandler.HandleLogout(this.parent.ConnectionHandler, _context, "/app");
					return;
				}
				if (text.StartsWith("loginsteam"))
				{
					SessionHandler.HandleSteamLogin(_context, this.urlBasePath + "verifysteamopenid");
					return;
				}
				if (text.StartsWith("login"))
				{
					SessionHandler.HandleUserPassLogin(this.parent.ConnectionHandler, _context, remoteEndpointString);
					return;
				}
				WebUtils.WriteText(_context.Response, "InvalidSessionsCommand", HttpStatusCode.BadRequest, null);
				return;
			}
		}

		// Token: 0x0600CFAB RID: 53163 RVA: 0x004BC238 File Offset: 0x004BA438
		public static bool HandleUserPassLogin(ConnectionHandler _connectionHandler, RequestContext _context, string _remoteEndpointString)
		{
			if (!_context.Request.HasEntityBody)
			{
				WebUtils.WriteText(_context.Response, "NoLoginData", HttpStatusCode.BadRequest, null);
				return false;
			}
			Stream inputStream = _context.Request.InputStream;
			byte[] array = new byte[_context.Request.ContentLength64];
			inputStream.Read(array, 0, (int)_context.Request.ContentLength64);
			IDictionary<string, object> dictionary;
			try
			{
				dictionary = JsonSerializer.Deserialize<IDictionary<string, object>>(array);
			}
			catch (Exception e)
			{
				Log.Error("Error deserializing JSON from user/password login:");
				Log.Exception(e);
				WebUtils.WriteText(_context.Response, "InvalidLoginJson", HttpStatusCode.BadRequest, null);
				return false;
			}
			object obj;
			if (dictionary.TryGetValue("username", out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					if (dictionary.TryGetValue("password", out obj))
					{
						string text2 = obj as string;
						if (text2 != null)
						{
							AdminWebUsers.WebUser webUser;
							if (!AdminWebUsers.Instance.TryGetUser(text, text2, out webUser))
							{
								WebUtils.WriteText(_context.Response, "UserPassInvalid", HttpStatusCode.Unauthorized, null);
								Log.Out("[Web] User/pass login failed from " + _remoteEndpointString);
								return false;
							}
							bool flag = SessionHandler.HandleUserIdLogin(_connectionHandler, _context, _remoteEndpointString, "User/pass", webUser.Name, webUser.PlatformUser, webUser.CrossPlatformUser);
							if (flag)
							{
								WebUtils.WriteText(_context.Response, "", HttpStatusCode.OK, null);
								return flag;
							}
							WebUtils.WriteText(_context.Response, "LoginError", HttpStatusCode.InternalServerError, null);
							return flag;
						}
					}
					WebUtils.WriteText(_context.Response, "InvalidLoginJson", HttpStatusCode.BadRequest, null);
					return false;
				}
			}
			WebUtils.WriteText(_context.Response, "InvalidLoginJson", HttpStatusCode.BadRequest, null);
			return false;
		}

		// Token: 0x0600CFAC RID: 53164 RVA: 0x004BC3D4 File Offset: 0x004BA5D4
		public static void HandleSteamLogin(RequestContext _context, string _verificationCallbackUrl)
		{
			string text = (WebUtils.IsSslRedirected(_context.Request) ? "https://" : "http://") + _context.Request.UserHostName;
			string openIdLoginUrl = OpenID.GetOpenIdLoginUrl(text, text + _verificationCallbackUrl);
			_context.Response.Redirect(openIdLoginUrl);
		}

		// Token: 0x0600CFAD RID: 53165 RVA: 0x004BC424 File Offset: 0x004BA624
		public static bool HandleLogout(ConnectionHandler _connectionHandler, RequestContext _context, string _pageBase)
		{
			Cookie cookie = new Cookie("sid", "", "/")
			{
				Expired = true
			};
			_context.Response.AppendCookie(cookie);
			if (_context.Connection == null)
			{
				_context.Response.Redirect(_pageBase);
				return false;
			}
			_connectionHandler.LogOut(_context.Connection.SessionID);
			_context.Response.Redirect(_pageBase);
			return true;
		}

		// Token: 0x0600CFAE RID: 53166 RVA: 0x004BC490 File Offset: 0x004BA690
		public static bool HandleSteamVerification(ConnectionHandler _connectionHandler, RequestContext _context, string _remoteEndpointString)
		{
			ulong num;
			try
			{
				num = OpenID.Validate(_context.Request);
			}
			catch (Exception e)
			{
				Log.Error("[Web] Error validating Steam login from " + _remoteEndpointString + ":");
				Log.Exception(e);
				return false;
			}
			if (num <= 0UL)
			{
				Log.Out("[Web] Steam OpenID login failed (invalid ID) from " + _remoteEndpointString);
				return false;
			}
			UserIdentifierSteam userIdentifierSteam = new UserIdentifierSteam(num);
			return SessionHandler.HandleUserIdLogin(_connectionHandler, _context, _remoteEndpointString, "Steam OpenID", userIdentifierSteam.ToString(), userIdentifierSteam, null);
		}

		// Token: 0x0600CFAF RID: 53167 RVA: 0x004BC510 File Offset: 0x004BA710
		public static bool HandleUserIdLogin(ConnectionHandler _connectionHandler, RequestContext _context, string _remoteEndpointString, string _loginName, string _username, PlatformUserIdentifierAbs _userId, PlatformUserIdentifierAbs _crossUserId = null)
		{
			try
			{
				WebConnection webConnection = _connectionHandler.LogIn(_context.Request.RemoteEndPoint.Address, _username, _userId, _crossUserId);
				int userPermissionLevel = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_userId);
				int val = int.MaxValue;
				if (_crossUserId != null)
				{
					val = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_crossUserId);
				}
				int num = Math.Min(userPermissionLevel, val);
				Log.Out(string.Format("[Web] {0} login from {1}, name {2} with ID {3}, CID {4}, permission level {5}", new object[]
				{
					_loginName,
					_remoteEndpointString,
					_username,
					_userId,
					(_crossUserId != null) ? _crossUserId.ToString() : "none",
					num
				}));
				Cookie cookie = new Cookie("sid", webConnection.SessionID, "/")
				{
					Expired = false,
					Expires = DateTime.MinValue,
					HttpOnly = true,
					Secure = false
				};
				_context.Response.AppendCookie(cookie);
				return true;
			}
			catch (Exception e)
			{
				Log.Error("[Web] Error during " + _loginName + " login:");
				Log.Exception(e);
			}
			return false;
		}

		// Token: 0x04009E02 RID: 40450
		[PublicizedFrom(EAccessModifier.Private)]
		public const string pageBasePath = "/app";

		// Token: 0x04009E03 RID: 40451
		[PublicizedFrom(EAccessModifier.Private)]
		public const string pageErrorPath = "/app/error/";

		// Token: 0x04009E04 RID: 40452
		[PublicizedFrom(EAccessModifier.Private)]
		public const string steamOpenIdVerifyUrl = "verifysteamopenid";

		// Token: 0x04009E05 RID: 40453
		[PublicizedFrom(EAccessModifier.Private)]
		public const string steamLoginUrl = "loginsteam";

		// Token: 0x04009E06 RID: 40454
		[PublicizedFrom(EAccessModifier.Private)]
		public const string steamLoginName = "Steam OpenID";

		// Token: 0x04009E07 RID: 40455
		[PublicizedFrom(EAccessModifier.Private)]
		public const string steamLoginFailedPage = "SteamLoginFailed";

		// Token: 0x04009E08 RID: 40456
		[PublicizedFrom(EAccessModifier.Private)]
		public const string userPassLoginUrl = "login";

		// Token: 0x04009E09 RID: 40457
		public const string userPassLoginName = "User/pass";
	}
}
