using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Profiling;
using Webserver.WebAPI;

namespace Webserver.UrlHandlers
{
	// Token: 0x02001AF2 RID: 6898
	public class ApiHandler : AbsHandler
	{
		// Token: 0x0600CF80 RID: 53120 RVA: 0x004BB0F5 File Offset: 0x004B92F5
		public ApiHandler() : base(null, 0)
		{
		}

		// Token: 0x0600CF81 RID: 53121 RVA: 0x004BB10C File Offset: 0x004B930C
		public override void SetBasePathAndParent(Web _parent, string _relativePath)
		{
			base.SetBasePathAndParent(_parent, _relativePath);
			ApiHandler.apiWithParentCtorArgs[0] = _parent;
			ReflectionHelpers.FindTypesImplementingBase(typeof(AbsWebAPI), new Action<Type>(this.apiFoundCallback), false);
			this.addApi(new Null("viewallclaims"));
			this.addApi(new Null("viewallplayers"));
		}

		// Token: 0x0600CF82 RID: 53122 RVA: 0x004BB168 File Offset: 0x004B9368
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiFoundCallback(Type _type)
		{
			ConstructorInfo constructor = _type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ApiHandler.apiWithParentCtorTypes, null);
			if (constructor != null)
			{
				AbsWebAPI api = (AbsWebAPI)constructor.Invoke(ApiHandler.apiWithParentCtorArgs);
				this.addApi(api);
				return;
			}
			constructor = _type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ApiHandler.apiEmptyCtorTypes, null);
			if (constructor != null)
			{
				AbsWebAPI api2 = (AbsWebAPI)constructor.Invoke(ApiHandler.apiEmptyCtorArgs);
				this.addApi(api2);
			}
		}

		// Token: 0x0600CF83 RID: 53123 RVA: 0x004BB1D8 File Offset: 0x004B93D8
		[PublicizedFrom(EAccessModifier.Private)]
		public void addApi(AbsWebAPI _api)
		{
			this.apis.Add(_api.Name, _api);
			this.parent.OpenApiHelpers.LoadOpenApiSpec(_api);
		}

		// Token: 0x0600CF84 RID: 53124 RVA: 0x004BB200 File Offset: 0x004B9400
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HandleCors(RequestContext _context)
		{
			string text;
			_context.Request.Headers.TryGetValue("Origin", out text);
			_context.Response.AddHeader("Access-Control-Allow-Origin", text ?? "*");
			if (_context.Method != ERequestMethod.OPTIONS)
			{
				return false;
			}
			string text2;
			if (!_context.Request.Headers.TryGetValue("Access-Control-Request-Method", out text2))
			{
				return false;
			}
			_context.Response.AddHeader("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE, OPTIONS, HEAD");
			_context.Response.AddHeader("Access-Control-Allow-Headers", "X-SDTD-API-TOKENNAME, X-SDTD-API-SECRET");
			_context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
			return true;
		}

		// Token: 0x0600CF85 RID: 53125 RVA: 0x004BB2A8 File Offset: 0x004B94A8
		public override void HandleRequest(RequestContext _context)
		{
			string requestPath = null;
			int num = _context.RequestPath.IndexOf('/', this.urlBasePath.Length);
			string text;
			if (num >= 0)
			{
				text = _context.RequestPath.Substring(this.urlBasePath.Length, num - this.urlBasePath.Length);
				requestPath = _context.RequestPath.Substring(num + 1);
			}
			else
			{
				text = _context.RequestPath.Substring(this.urlBasePath.Length);
			}
			AbsWebAPI absWebAPI;
			if (!this.apis.TryGetValue(text, out absWebAPI))
			{
				Log.Warning("[Web] In ApiHandler.HandleRequest(): No handler found for API \"" + text + "\"");
				_context.Response.StatusCode = 404;
				return;
			}
			if (this.HandleCors(_context))
			{
				return;
			}
			_context.RequestPath = requestPath;
			if (!absWebAPI.Authorized(_context))
			{
				_context.Response.StatusCode = 403;
				WebConnection connection = _context.Connection;
				return;
			}
			try
			{
				absWebAPI.HandleRequest(_context);
			}
			catch (Exception e)
			{
				Log.Error("[Web] In ApiHandler.HandleRequest(): Handler " + absWebAPI.Name + " threw an exception:");
				Log.Exception(e);
				_context.Response.StatusCode = 500;
			}
		}

		// Token: 0x04009DCC RID: 40396
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, AbsWebAPI> apis = new CaseInsensitiveStringDictionary<AbsWebAPI>();

		// Token: 0x04009DCD RID: 40397
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Type[] apiWithParentCtorTypes = new Type[]
		{
			typeof(Web)
		};

		// Token: 0x04009DCE RID: 40398
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly object[] apiWithParentCtorArgs = new object[1];

		// Token: 0x04009DCF RID: 40399
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Type[] apiEmptyCtorTypes = new Type[0];

		// Token: 0x04009DD0 RID: 40400
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly object[] apiEmptyCtorArgs = new object[0];

		// Token: 0x04009DD1 RID: 40401
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly CustomSampler apiHandlerSampler = CustomSampler.Create("API_Handler", false);
	}
}
