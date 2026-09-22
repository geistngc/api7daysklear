using System;
using System.Text;
using UnityEngine.Scripting;

namespace Webserver.WebAPI
{
	// Token: 0x02001AD3 RID: 6867
	[Preserve]
	public class Null : AbsWebAPI
	{
		// Token: 0x0600CED6 RID: 52950 RVA: 0x004B7148 File Offset: 0x004B5348
		public Null(string _name) : base(_name)
		{
		}

		// Token: 0x0600CED7 RID: 52951 RVA: 0x004B7154 File Offset: 0x004B5354
		public override void HandleRequest(RequestContext _context)
		{
			_context.Response.ContentLength64 = 0L;
			_context.Response.ContentType = "text/plain";
			_context.Response.ContentEncoding = Encoding.ASCII;
			_context.Response.OutputStream.Write(Array.Empty<byte>(), 0, 0);
		}
	}
}
