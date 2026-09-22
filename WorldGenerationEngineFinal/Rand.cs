using System;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001728 RID: 5928
	public class Rand
	{
		// Token: 0x17001667 RID: 5735
		// (get) Token: 0x0600B860 RID: 47200 RVA: 0x0044945F File Offset: 0x0044765F
		public static Rand Instance
		{
			get
			{
				if (Rand.instance == null)
				{
					Rand.instance = new Rand();
				}
				return Rand.instance;
			}
		}

		// Token: 0x0600B861 RID: 47201 RVA: 0x00449477 File Offset: 0x00447677
		public Rand()
		{
			this.gameRandom = GameRandomManager.Instance.CreateGameRandom();
		}

		// Token: 0x0600B862 RID: 47202 RVA: 0x0044948F File Offset: 0x0044768F
		public Rand(int seed)
		{
			this.gameRandom = GameRandomManager.Instance.CreateGameRandom();
			this.SetSeed(seed);
		}

		// Token: 0x0600B863 RID: 47203 RVA: 0x004494AE File Offset: 0x004476AE
		public void Cleanup()
		{
			GameRandomManager.Instance.FreeGameRandom(this.gameRandom);
			Rand.instance = null;
		}

		// Token: 0x0600B864 RID: 47204 RVA: 0x004494C6 File Offset: 0x004476C6
		public void Free()
		{
			GameRandomManager.Instance.FreeGameRandom(this.gameRandom);
		}

		// Token: 0x0600B865 RID: 47205 RVA: 0x004494D8 File Offset: 0x004476D8
		public void SetSeed(int seed)
		{
			this.gameRandom.SetSeed(seed);
		}

		// Token: 0x0600B866 RID: 47206 RVA: 0x004494E6 File Offset: 0x004476E6
		public float Float()
		{
			return this.gameRandom.RandomFloat;
		}

		// Token: 0x0600B867 RID: 47207 RVA: 0x004494F3 File Offset: 0x004476F3
		public int Int()
		{
			return this.gameRandom.RandomInt;
		}

		// Token: 0x0600B868 RID: 47208 RVA: 0x00449500 File Offset: 0x00447700
		public int Range(int min, int max)
		{
			return this.gameRandom.RandomRange(min, max);
		}

		// Token: 0x0600B869 RID: 47209 RVA: 0x0044950F File Offset: 0x0044770F
		public int Range(int max)
		{
			return this.gameRandom.RandomRange(max);
		}

		// Token: 0x0600B86A RID: 47210 RVA: 0x0044951D File Offset: 0x0044771D
		public float Range(float min, float max)
		{
			return this.gameRandom.RandomRange(min, max);
		}

		// Token: 0x0600B86B RID: 47211 RVA: 0x0044952C File Offset: 0x0044772C
		public int Angle()
		{
			return this.gameRandom.RandomRange(360);
		}

		// Token: 0x0600B86C RID: 47212 RVA: 0x0044953E File Offset: 0x0044773E
		public Vector2 RandomOnUnitCircle()
		{
			return this.gameRandom.RandomOnUnitCircle;
		}

		// Token: 0x0600B86D RID: 47213 RVA: 0x0044954B File Offset: 0x0044774B
		public int PeekSample()
		{
			return this.gameRandom.PeekSample();
		}

		// Token: 0x04008A13 RID: 35347
		[PublicizedFrom(EAccessModifier.Private)]
		public static Rand instance;

		// Token: 0x04008A14 RID: 35348
		public GameRandom gameRandom;
	}
}
