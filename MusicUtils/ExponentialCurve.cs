using System;

namespace MusicUtils
{
	// Token: 0x02001A18 RID: 6680
	public class ExponentialCurve : LinearCurve
	{
		// Token: 0x0600CB26 RID: 52006 RVA: 0x004A89F0 File Offset: 0x004A6BF0
		public ExponentialCurve(double _base, float _start, float _end, float _startX, float _endX) : base((float)Math.Log((double)_start, _base), (float)Math.Log((double)_end, _base), _startX, _endX)
		{
			this.b = _base;
			if (_start < _end)
			{
				this.min = _start;
				this.max = _end;
				return;
			}
			this.min = _end;
			this.max = _start;
		}

		// Token: 0x0600CB27 RID: 52007 RVA: 0x004A8A41 File Offset: 0x004A6C41
		public override float GetMixerValue(float _param)
		{
			return Utils.FastClamp((float)Math.Pow(this.b, (double)base.GetLine(_param)), this.min, this.max);
		}

		// Token: 0x04009AA7 RID: 39591
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly double b;

		// Token: 0x04009AA8 RID: 39592
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float min;

		// Token: 0x04009AA9 RID: 39593
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float max;
	}
}
