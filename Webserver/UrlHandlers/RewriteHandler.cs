using System;

namespace Webserver.UrlHandlers
{
	// Token: 0x02001AF8 RID: 6904
	public class RewriteHandler : AbsHandler
	{
		// Token: 0x0600CFA7 RID: 53159 RVA: 0x004BC0A4 File Offset: 0x004BA2A4
		public RewriteHandler(string _target, bool _fixedTarget = false) : base(null, 0)
		{
			this.target = _target;
			this.fixedTarget = _fixedTarget;
		}

		// Token: 0x0600CFA8 RID: 53160 RVA: 0x004BC0BC File Offset: 0x004BA2BC
		public override void HandleRequest(RequestContext _context)
		{
			_context.RequestPath = (this.fixedTarget ? this.target : (this.target + _context.RequestPath.Remove(0, this.urlBasePath.Length)));
			this.parent.ApplyPathHandler(_context);
		}

		// Token: 0x04009E00 RID: 40448
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string target;

		// Token: 0x04009E01 RID: 40449
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool fixedTarget;
	}
}
