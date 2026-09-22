using System;

namespace MusicUtils
{
	// Token: 0x02001A17 RID: 6679
	public class LinearCurve : Curve
	{
		// Token: 0x0600CB23 RID: 52003 RVA: 0x004A89B0 File Offset: 0x004A6BB0
		public LinearCurve(float _startY, float _endY, float _startX, float _endX) : base(_startY, _endY, _startX, _endX)
		{
		}

		// Token: 0x0600CB24 RID: 52004 RVA: 0x004A89BD File Offset: 0x004A6BBD
		public override float GetMixerValue(float _param)
		{
			return Utils.FastClamp(this.GetLine(_param), this.linearStart, this.linearEnd);
		}

		// Token: 0x0600CB25 RID: 52005 RVA: 0x004A89D7 File Offset: 0x004A6BD7
		[PublicizedFrom(EAccessModifier.Protected)]
		public float GetLine(float _param)
		{
			return this.rate * (_param - this.startX) + this.linearStart;
		}
	}
}
