using System;

namespace Webserver.UrlHandlers
{
	// Token: 0x02001AFA RID: 6906
	public class SimpleRedirectHandler : AbsHandler
	{
		// Token: 0x0600CFB0 RID: 53168 RVA: 0x004BC634 File Offset: 0x004BA834
		public SimpleRedirectHandler(string _target) : base(null, 0)
		{
			this.target = _target;
		}

		// Token: 0x0600CFB1 RID: 53169 RVA: 0x004BC645 File Offset: 0x004BA845
		public override void HandleRequest(RequestContext _context)
		{
			_context.Response.Redirect(this.target);
		}

		// Token: 0x04009E0A RID: 40458
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string target;
	}
}
