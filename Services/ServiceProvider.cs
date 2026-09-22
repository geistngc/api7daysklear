using System;
using System.Collections.Generic;

namespace Services
{
	// Token: 0x02001681 RID: 5761
	public class ServiceProvider
	{
		// Token: 0x170015AB RID: 5547
		// (get) Token: 0x0600B483 RID: 46211 RVA: 0x00439DF0 File Offset: 0x00437FF0
		// (set) Token: 0x0600B484 RID: 46212 RVA: 0x00439DF7 File Offset: 0x00437FF7
		public static ServiceProvider Instance { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B485 RID: 46213 RVA: 0x00439DFF File Offset: 0x00437FFF
		public static void Init()
		{
			if (ServiceProvider.Instance != null)
			{
				Log.Out("ServiceProvider has already exists");
				return;
			}
			ServiceProvider.Instance = new ServiceProvider();
			ServiceProvider.Instance.InternalInit();
		}

		// Token: 0x0600B486 RID: 46214 RVA: 0x00439E27 File Offset: 0x00438027
		[PublicizedFrom(EAccessModifier.Private)]
		public void InternalInit()
		{
			this._services = new Dictionary<Type, object>();
		}

		// Token: 0x0600B487 RID: 46215 RVA: 0x00439E34 File Offset: 0x00438034
		public void Register(Type type, object obj)
		{
			this._services.Add(type, obj);
		}

		// Token: 0x0600B488 RID: 46216 RVA: 0x00439E43 File Offset: 0x00438043
		public T Get<T>()
		{
			return (T)((object)this._services[typeof(T)]);
		}

		// Token: 0x040087B3 RID: 34739
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<Type, object> _services;
	}
}
