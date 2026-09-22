using System;
using Webserver.Permissions;

namespace Webserver.WebAPI
{
	// Token: 0x02001AD0 RID: 6864
	public abstract class AbsWebAPI
	{
		// Token: 0x0600CEC2 RID: 52930 RVA: 0x004B6C7D File Offset: 0x004B4E7D
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsWebAPI(string _name = null) : this(null, _name)
		{
		}

		// Token: 0x0600CEC3 RID: 52931 RVA: 0x004B6C88 File Offset: 0x004B4E88
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsWebAPI(Web _parentWeb, string _name = null)
		{
			this.Name = (_name ?? base.GetType().Name);
			this.ParentWeb = _parentWeb;
			this.CachedApiModuleName = "webapi." + this.Name;
			this.RegisterPermissions();
		}

		// Token: 0x0600CEC4 RID: 52932 RVA: 0x004B6CD4 File Offset: 0x004B4ED4
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void RegisterPermissions()
		{
			AdminWebModules.Instance.AddKnownModule(new AdminWebModules.WebModule(this.CachedApiModuleName, this.DefaultPermissionLevel(), true));
		}

		// Token: 0x0600CEC5 RID: 52933
		public abstract void HandleRequest(RequestContext _context);

		// Token: 0x0600CEC6 RID: 52934 RVA: 0x004B6CF2 File Offset: 0x004B4EF2
		public virtual bool Authorized(RequestContext _context)
		{
			return AdminWebModules.Instance.GetModule(this.CachedApiModuleName).LevelGlobal >= _context.PermissionLevel;
		}

		// Token: 0x0600CEC7 RID: 52935 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int DefaultPermissionLevel()
		{
			return 0;
		}

		// Token: 0x04009CFD RID: 40189
		public readonly string Name;

		// Token: 0x04009CFE RID: 40190
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly Web ParentWeb;

		// Token: 0x04009CFF RID: 40191
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly string CachedApiModuleName;
	}
}
