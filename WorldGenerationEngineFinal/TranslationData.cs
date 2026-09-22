using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001740 RID: 5952
	public struct TranslationData
	{
		// Token: 0x0600B926 RID: 47398 RVA: 0x004502F4 File Offset: 0x0044E4F4
		public TranslationData(int _x, int _y, float _randomScaleMin = 0.5f, float _randomScaleMax = 1.5f)
		{
			this.x = _x;
			this.y = _y;
			Rand instance = Rand.Instance;
			this.scale = instance.Range(_randomScaleMin, _randomScaleMax);
			this.rotation = instance.Angle();
		}

		// Token: 0x0600B927 RID: 47399 RVA: 0x00450330 File Offset: 0x0044E530
		public TranslationData(int _x, int _y, float _scale, int _rotation)
		{
			this.x = _x;
			this.y = _y;
			this.scale = _scale;
			this.rotation = _rotation;
		}

		// Token: 0x04008AAB RID: 35499
		public int x;

		// Token: 0x04008AAC RID: 35500
		public int y;

		// Token: 0x04008AAD RID: 35501
		public float scale;

		// Token: 0x04008AAE RID: 35502
		public int rotation;
	}
}
