using System;
using System.Collections.Generic;

// Token: 0x02000412 RID: 1042
public abstract class AIDirectorData
{
	// Token: 0x0600203C RID: 8252 RVA: 0x000C3172 File Offset: 0x000C1372
	public static void InitStatic()
	{
		AIDirectorData.noisySounds = new CaseInsensitiveStringDictionary<AIDirectorData.Noise>();
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x000C317E File Offset: 0x000C137E
	public static void Cleanup()
	{
		if (AIDirectorData.noisySounds != null)
		{
			AIDirectorData.noisySounds.Clear();
		}
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000C3191 File Offset: 0x000C1391
	public static void AddNoisySound(string _name, AIDirectorData.Noise _noise)
	{
		AIDirectorData.noisySounds.Add(_name, _noise);
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000C319F File Offset: 0x000C139F
	public static bool FindNoise(string name, out AIDirectorData.Noise noise)
	{
		if (name == null)
		{
			noise = default(AIDirectorData.Noise);
			return false;
		}
		return AIDirectorData.noisySounds.TryGetValue(name, out noise);
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public AIDirectorData()
	{
	}

	// Token: 0x04001609 RID: 5641
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, AIDirectorData.Noise> noisySounds;

	// Token: 0x02000413 RID: 1043
	public struct Noise
	{
		// Token: 0x06002041 RID: 8257 RVA: 0x000C31B9 File Offset: 0x000C13B9
		public Noise(string _source, float _volume, float _duration, float _muffledWhenCrouched, float _heatMapStrength, ulong _heatMapWorldTimeToLive)
		{
			this.volume = _volume;
			this.duration = _duration;
			this.muffledWhenCrouched = _muffledWhenCrouched;
			this.heatMapStrength = _heatMapStrength;
			this.heatMapWorldTimeToLive = _heatMapWorldTimeToLive;
		}

		// Token: 0x0400160A RID: 5642
		public float volume;

		// Token: 0x0400160B RID: 5643
		public float duration;

		// Token: 0x0400160C RID: 5644
		public float muffledWhenCrouched;

		// Token: 0x0400160D RID: 5645
		public float heatMapStrength;

		// Token: 0x0400160E RID: 5646
		public ulong heatMapWorldTimeToLive;
	}
}
