using System;

namespace Platform.Local
{
	// Token: 0x02001CCA RID: 7370
	public class Api : IPlatformApi
	{
		// Token: 0x17001B52 RID: 6994
		// (get) Token: 0x0600DACA RID: 56010 RVA: 0x004E7414 File Offset: 0x004E5614
		public EApiStatus ClientApiStatus { get; }

		// Token: 0x1400013C RID: 316
		// (add) Token: 0x0600DACB RID: 56011 RVA: 0x004E741C File Offset: 0x004E561C
		// (remove) Token: 0x0600DACC RID: 56012 RVA: 0x000027FC File Offset: 0x000009FC
		public event Action ClientApiInitialized
		{
			add
			{
				lock (this)
				{
					value();
				}
			}
			remove
			{
			}
		}

		// Token: 0x0600DACD RID: 56013 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600DACE RID: 56014 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool InitClientApis()
		{
			return true;
		}

		// Token: 0x0600DACF RID: 56015 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool InitServerApis()
		{
			return true;
		}

		// Token: 0x0600DAD0 RID: 56016 RVA: 0x000027FC File Offset: 0x000009FC
		public void ServerApiLoaded()
		{
		}

		// Token: 0x0600DAD1 RID: 56017 RVA: 0x000027FC File Offset: 0x000009FC
		public void Update()
		{
		}

		// Token: 0x0600DAD2 RID: 56018 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0600DAD3 RID: 56019 RVA: 0x00040FA0 File Offset: 0x0003F1A0
		public float GetScreenBoundsValueFromSystem()
		{
			return 1f;
		}
	}
}
