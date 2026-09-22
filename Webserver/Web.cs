using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SpaceWizards.HttpListener;
using UnityEngine;
using UnityEngine.Profiling;
using Webserver.FileCache;
using Webserver.Permissions;
using Webserver.UrlHandlers;
using Webserver.WebAPI;

namespace Webserver
{
	// Token: 0x02001AC8 RID: 6856
	public class Web : IConsoleServer
	{
		// Token: 0x1400011B RID: 283
		// (add) Token: 0x0600CE85 RID: 52869 RVA: 0x004B5808 File Offset: 0x004B3A08
		// (remove) Token: 0x0600CE86 RID: 52870 RVA: 0x004B583C File Offset: 0x004B3A3C
		public static event Action<Web> ServerInitialized;

		// Token: 0x0600CE87 RID: 52871 RVA: 0x004B5870 File Offset: 0x004B3A70
		public Web()
		{
			try
			{
				if (!GamePrefs.GetBool(EnumUtils.Parse<EnumGamePrefs>("WebDashboardEnabled", false)))
				{
					Log.Out("[Web] Webserver not started, WebDashboardEnabled set to false");
				}
				else
				{
					int @int = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("WebDashboardPort", false));
					if (@int < 1 || @int > 65533)
					{
						Log.Out("[Web] Webserver not started (WebDashboardPort not within 1-65535)");
					}
					else if (!SpaceWizards.HttpListener.HttpListener.IsSupported)
					{
						Log.Out("[Web] Webserver not started (HttpListener.IsSupported returned false)");
					}
					else
					{
						if (string.IsNullOrEmpty(GamePrefs.GetString(EnumUtils.Parse<EnumGamePrefs>("WebDashboardUrl", false))))
						{
							Log.Warning("[Web] WebDashboardUrl not set. Recommended to set it to the public URL pointing to your dashboard / reverse proxy");
						}
						this.ConnectionHandler = new ConnectionHandler();
						this.OpenApiHelpers = new OpenApiHelpers();
						this.RegisterDefaultHandlers();
						Action<Web> serverInitialized = Web.ServerInitialized;
						if (serverInitialized != null)
						{
							serverInitialized(this);
						}
						this.listener.Prefixes.Add(string.Format("http://+:{0}/", @int));
						this.listener.Start();
						this.handleRequestDelegate = new AsyncCallback(this.HandleRequest);
						this.listener.BeginGetContext(this.handleRequestDelegate, this.listener);
						SingletonMonoBehaviour<SdtdConsole>.Instance.RegisterServer(this);
						Log.Out(string.Format("[Web] Started Webserver on port {0}", @int));
					}
				}
			}
			catch (Exception e)
			{
				Log.Error("[Web] Error in Web.ctor: ");
				Log.Exception(e);
			}
		}

		// Token: 0x0600CE88 RID: 52872 RVA: 0x004B5A3C File Offset: 0x004B3C3C
		[PublicizedFrom(EAccessModifier.Private)]
		public void RegisterDefaultHandlers()
		{
			bool flag = StringParsers.ParseBool("false", 0, -1, true);
			string filePath = Web.DetectWebserverFolder();
			this.RegisterPathHandler("/", new RewriteHandler("/files/", false));
			this.RegisterPathHandler("/app", new RewriteHandler("/files/index.html", true));
			this.RegisterWebMods(flag);
			this.RegisterPathHandler("/session/", new SessionHandler());
			this.RegisterPathHandler("/userstatus", new UserStatusHandler(null));
			this.RegisterPathHandler("/sse/", new SseHandler(null));
			this.RegisterPathHandler("/files/", new StaticHandler(filePath, flag ? new SimpleCache() : new DirectAccess(), false, null));
			this.RegisterPathHandler("/itemicons/", new ItemIconHandler(true, null));
			this.RegisterPathHandler("/api/", new ApiHandler());
		}

		// Token: 0x0600CE89 RID: 52873 RVA: 0x004B5B08 File Offset: 0x004B3D08
		[PublicizedFrom(EAccessModifier.Private)]
		public static string DetectWebserverFolder()
		{
			string text = GameIO.GetGameDir("Data/Web") + "/webroot";
			foreach (Mod mod in ModManager.GetLoadedMods())
			{
				string text2 = mod.Path + "/webroot";
				if (Directory.Exists(text2))
				{
					text = text2;
				}
			}
			Log.Out("[Web] Serving basic webserver files from " + text);
			return text;
		}

		// Token: 0x0600CE8A RID: 52874 RVA: 0x004B5B94 File Offset: 0x004B3D94
		public void RegisterPathHandler(string _urlBasePath, AbsHandler _handler)
		{
			using (List<AbsHandler>.Enumerator enumerator = this.handlers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!(enumerator.Current.UrlBasePath != _urlBasePath))
					{
						Log.Error("[Web] Handler for relative path " + _urlBasePath + " already registerd.");
						return;
					}
				}
			}
			this.handlers.Add(_handler);
			_handler.SetBasePathAndParent(this, _urlBasePath);
		}

		// Token: 0x0600CE8B RID: 52875 RVA: 0x004B5C18 File Offset: 0x004B3E18
		[PublicizedFrom(EAccessModifier.Private)]
		public void RegisterWebMods(bool _useStaticCache)
		{
			foreach (Mod mod in ModManager.GetLoadedMods())
			{
				try
				{
					try
					{
						WebMod item = new WebMod(this, mod, _useStaticCache);
						this.WebMods.Add(item);
					}
					catch (InvalidDataException ex)
					{
						Log.Error("[Web] Could not load webmod from mod " + mod.Name + ": " + ex.Message);
					}
				}
				catch (Exception e)
				{
					Log.Error("[Web] Failed loading web mods from mod " + mod.Name);
					Log.Exception(e);
				}
			}
		}

		// Token: 0x0600CE8C RID: 52876 RVA: 0x004B5CD4 File Offset: 0x004B3ED4
		public void Disconnect()
		{
			if (this.shutdown)
			{
				return;
			}
			this.shutdown = true;
			try
			{
				foreach (AbsHandler absHandler in this.handlers)
				{
					absHandler.Shutdown();
				}
				this.listener.Stop();
				this.listener.Close();
			}
			catch (Exception arg)
			{
				Log.Out(string.Format("[Web] Error in Web.Disconnect: {0}", arg));
			}
		}

		// Token: 0x0600CE8D RID: 52877 RVA: 0x004B5D6C File Offset: 0x004B3F6C
		public void SendLine(string _line)
		{
			this.ConnectionHandler.SendLine(_line);
		}

		// Token: 0x0600CE8E RID: 52878 RVA: 0x000027FC File Offset: 0x000009FC
		public void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
		{
		}

		// Token: 0x0600CE8F RID: 52879 RVA: 0x004B5D7C File Offset: 0x004B3F7C
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleRequest(IAsyncResult _result)
		{
			SpaceWizards.HttpListener.HttpListener httpListener = (SpaceWizards.HttpListener.HttpListener)_result.AsyncState;
			if (!httpListener.IsListening)
			{
				return;
			}
			SpaceWizards.HttpListener.HttpListenerContext httpListenerContext = httpListener.EndGetContext(_result);
			httpListener.BeginGetContext(this.handleRequestDelegate, httpListener);
			try
			{
				SpaceWizards.HttpListener.HttpListenerRequest request = httpListenerContext.Request;
				SpaceWizards.HttpListener.HttpListenerResponse response = httpListenerContext.Response;
				response.SendChunked = false;
				response.ProtocolVersion = this.httpProtocolVersion;
				if (GameManager.Instance.World == null)
				{
					response.StatusCode = 503;
				}
				else if (request.Url == null)
				{
					response.StatusCode = 400;
				}
				else
				{
					WebConnection webConnection;
					int permissionLevel = this.DoAuthentication(request, out webConnection);
					if (webConnection != null)
					{
						Cookie cookie = new Cookie("sid", webConnection.SessionID, "/")
						{
							Expired = false,
							Expires = DateTime.MinValue,
							HttpOnly = true,
							Secure = false
						};
						response.AppendCookie(cookie);
					}
					string absolutePath = request.Url.AbsolutePath;
					if (absolutePath.Length < 2)
					{
						response.Redirect("/app");
					}
					else
					{
						request.ContentEncoding = Encoding.UTF8;
						RequestContext requestContext = new RequestContext(absolutePath, request, response, webConnection, permissionLevel);
						if (requestContext.Method == ERequestMethod.Other)
						{
							requestContext.Response.StatusCode = 400;
						}
						else
						{
							this.ApplyPathHandler(requestContext);
						}
					}
				}
			}
			catch (IOException ex)
			{
				if (ex.InnerException is SocketException)
				{
					Log.Out("[Web] Error in Web.HandleRequest(): Remote host closed connection: " + ex.InnerException.Message);
				}
				else
				{
					Log.Out(string.Format("[Web] Error (IO) in Web.HandleRequest(): {0}", ex));
				}
			}
			catch (Exception e)
			{
				Log.Error("[Web] Error in Web.HandleRequest(): ");
				Log.Exception(e);
			}
			finally
			{
				if (!httpListenerContext.Response.SendChunked)
				{
					httpListenerContext.Response.Close();
				}
			}
		}

		// Token: 0x0600CE90 RID: 52880 RVA: 0x004B5F7C File Offset: 0x004B417C
		public void ApplyPathHandler(RequestContext _context)
		{
			int i = this.handlers.Count - 1;
			while (i >= 0)
			{
				AbsHandler absHandler = this.handlers[i];
				if (_context.RequestPath.StartsWith(absHandler.UrlBasePath))
				{
					if (!absHandler.IsAuthorizedForHandler(_context))
					{
						_context.Response.StatusCode = 403;
						WebConnection connection = _context.Connection;
						return;
					}
					absHandler.HandleRequest(_context);
					return;
				}
				else
				{
					i--;
				}
			}
			_context.Response.StatusCode = 404;
		}

		// Token: 0x0600CE91 RID: 52881 RVA: 0x004B5FFC File Offset: 0x004B41FC
		[PublicizedFrom(EAccessModifier.Private)]
		public int DoAuthentication(SpaceWizards.HttpListener.HttpListenerRequest _req, out WebConnection _con)
		{
			_con = null;
			Cookie cookie = _req.Cookies["sid"];
			string text = (cookie != null) ? cookie.Value : null;
			IPEndPoint remoteEndPoint = _req.RemoteEndPoint;
			if (remoteEndPoint == null)
			{
				Log.Warning("[Web] No RemoteEndPoint on web request");
				return 2000;
			}
			if (!string.IsNullOrEmpty(text))
			{
				_con = this.ConnectionHandler.IsLoggedIn(text, remoteEndPoint.Address);
				if (_con != null)
				{
					int userPermissionLevel = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_con.UserId);
					int val = int.MaxValue;
					if (_con.CrossplatformUserId != null)
					{
						val = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_con.CrossplatformUserId);
					}
					return Math.Min(userPermissionLevel, val);
				}
			}
			string name;
			string secret;
			if (!_req.Headers.TryGetValue("X-SDTD-API-TOKENNAME", out name) || !_req.Headers.TryGetValue("X-SDTD-API-SECRET", out secret))
			{
				return 2000;
			}
			int permissionLevel = AdminApiTokens.Instance.GetPermissionLevel(name, secret);
			if (permissionLevel < 2147483647)
			{
				return permissionLevel;
			}
			Log.Warning(string.Format("[Web] Invalid Admintoken used from {0}", remoteEndPoint));
			return 2000;
		}

		// Token: 0x04009CC2 RID: 40130
		public const string DataPath = "Data/Web";

		// Token: 0x04009CC3 RID: 40131
		[PublicizedFrom(EAccessModifier.Private)]
		public const string indexPageUrl = "/app";

		// Token: 0x04009CC4 RID: 40132
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<AbsHandler> handlers = new List<AbsHandler>();

		// Token: 0x04009CC5 RID: 40133
		public readonly List<WebMod> WebMods = new List<WebMod>();

		// Token: 0x04009CC6 RID: 40134
		public readonly ConnectionHandler ConnectionHandler;

		// Token: 0x04009CC7 RID: 40135
		public readonly OpenApiHelpers OpenApiHelpers;

		// Token: 0x04009CC8 RID: 40136
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly SpaceWizards.HttpListener.HttpListener listener = new SpaceWizards.HttpListener.HttpListener();

		// Token: 0x04009CC9 RID: 40137
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Version httpProtocolVersion = new Version(1, 1);

		// Token: 0x04009CCA RID: 40138
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AsyncCallback handleRequestDelegate;

		// Token: 0x04009CCB RID: 40139
		[PublicizedFrom(EAccessModifier.Private)]
		public bool shutdown;

		// Token: 0x04009CCC RID: 40140
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler authSampler = CustomSampler.Create("Auth", false);

		// Token: 0x04009CCD RID: 40141
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler cookieSampler = CustomSampler.Create("ConCookie", false);

		// Token: 0x04009CCE RID: 40142
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler handlerSampler = CustomSampler.Create("Handler", false);
	}
}
