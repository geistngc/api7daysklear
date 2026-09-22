using System;

namespace Audio
{
	// Token: 0x02001B1E RID: 6942
	public class NoiseData
	{
		// Token: 0x0600D04B RID: 53323 RVA: 0x004BF8A3 File Offset: 0x004BDAA3
		public NoiseData()
		{
			this.volume = 0f;
			this.time = 1f;
			this.heatMapStrength = 0f;
			this.heatMapTime = 100UL;
			this.crouchMuffle = 1f;
		}

		// Token: 0x04009EF8 RID: 40696
		public float volume;

		// Token: 0x04009EF9 RID: 40697
		public float time;

		// Token: 0x04009EFA RID: 40698
		public float heatMapStrength;

		// Token: 0x04009EFB RID: 40699
		public ulong heatMapTime;

		// Token: 0x04009EFC RID: 40700
		public float crouchMuffle;
	}
}
