using System;

namespace MusicUtils
{
	// Token: 0x02001A19 RID: 6681
	public class LogarithmicCurve : LinearCurve
	{
		// Token: 0x0600CB28 RID: 52008 RVA: 0x004A8A68 File Offset: 0x004A6C68
		public LogarithmicCurve(double _base, double _scale, float _start, float _end, float _startX, float _endX) : base((float)Math.Pow(_base, (double)_start / _scale), (float)Math.Pow(_base, (double)_end / _scale), _startX, _endX)
		{
			this.b = Math.Pow(_base, 1.0 / _scale);
			if (_start < _end)
			{
				this.min = _start;
				this.max = _end;
				return;
			}
			this.min = _end;
			this.max = _start;
		}

		// Token: 0x0600CB29 RID: 52009 RVA: 0x004A8AD1 File Offset: 0x004A6CD1
		public override float GetMixerValue(float _param)
		{
			return Utils.FastClamp((float)Math.Log((double)Math.Max(base.GetLine(_param), 0f), this.b), this.min, this.max);
		}

		// Token: 0x04009AAA RID: 39594
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly double b;

		// Token: 0x04009AAB RID: 39595
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float min;

		// Token: 0x04009AAC RID: 39596
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float max;
	}
}
