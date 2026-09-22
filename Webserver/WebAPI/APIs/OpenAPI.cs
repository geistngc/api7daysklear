using System;
using System.Net;
using UnityEngine.Scripting;

namespace Webserver.WebAPI.APIs
{
	// Token: 0x02001AD8 RID: 6872
	[Preserve]
	public class OpenAPI : AbsRestApi
	{
		// Token: 0x0600CEF2 RID: 52978 RVA: 0x004B7ECC File Offset: 0x004B60CC
		public OpenAPI(Web _parentWeb) : base(_parentWeb, null)
		{
		}

		// Token: 0x0600CEF3 RID: 52979 RVA: 0x004B7ED8 File Offset: 0x004B60D8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			string text;
			if (!this.ParentWeb.OpenApiHelpers.TryGetOpenApiSpec(requestPath, out text))
			{
				WebUtils.WriteText(_context.Response, "Spec for " + requestPath + " not found", HttpStatusCode.NotFound, null);
				return;
			}
			WebUtils.WriteText(_context.Response, text, HttpStatusCode.OK, "text/x-yaml");
		}

		// Token: 0x0600CEF4 RID: 52980 RVA: 0x004B7F39 File Offset: 0x004B6139
		public override int DefaultPermissionLevel()
		{
			return 2000;
		}
	}
}
