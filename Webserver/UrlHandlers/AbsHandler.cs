using System;
using Webserver.Permissions;

namespace Webserver.UrlHandlers
{
	// Token: 0x02001AF1 RID: 6897
	public abstract class AbsHandler
	{
		// Token: 0x17001988 RID: 6536
		// (get) Token: 0x0600CF7A RID: 53114 RVA: 0x004BB089 File Offset: 0x004B9289
		public string UrlBasePath
		{
			get
			{
				return this.urlBasePath;
			}
		}

		// Token: 0x0600CF7B RID: 53115 RVA: 0x004BB091 File Offset: 0x004B9291
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsHandler(string _moduleName, int _defaultPermissionLevel = 0)
		{
			this.ModuleName = _moduleName;
			AdminWebModules.Instance.AddKnownModule(new AdminWebModules.WebModule(_moduleName, _defaultPermissionLevel, true));
		}

		// Token: 0x0600CF7C RID: 53116
		public abstract void HandleRequest(RequestContext _context);

		// Token: 0x0600CF7D RID: 53117 RVA: 0x004BB0B2 File Offset: 0x004B92B2
		public virtual bool IsAuthorizedForHandler(RequestContext _context)
		{
			return this.ModuleName == null || AdminWebModules.Instance.ModuleAllowedWithLevel(this.ModuleName, _context.PermissionLevel);
		}

		// Token: 0x0600CF7E RID: 53118 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Shutdown()
		{
		}

		// Token: 0x0600CF7F RID: 53119 RVA: 0x004BB0D4 File Offset: 0x004B92D4
		public virtual void SetBasePathAndParent(Web _parent, string _relativePath)
		{
			this.parent = _parent;
			this.urlBasePath = _relativePath;
			this.parent.OpenApiHelpers.LoadOpenApiSpec(this);
		}

		// Token: 0x04009DC9 RID: 40393
		public readonly string ModuleName;

		// Token: 0x04009DCA RID: 40394
		[PublicizedFrom(EAccessModifier.Protected)]
		public string urlBasePath;

		// Token: 0x04009DCB RID: 40395
		[PublicizedFrom(EAccessModifier.Protected)]
		public Web parent;
	}
}
