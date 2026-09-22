using System;
using UnityEngine;

namespace Assets.DuckType.Jiggle
{
	// Token: 0x02001D65 RID: 7525
	[PublicizedFrom(EAccessModifier.Internal)]
	public class ProceduralAnimation : MonoBehaviour
	{
		// Token: 0x0600DE7A RID: 56954 RVA: 0x004FD743 File Offset: 0x004FB943
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			this.m_RestPos = base.transform.position;
		}

		// Token: 0x0600DE7B RID: 56955 RVA: 0x004FD758 File Offset: 0x004FB958
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			float num = this.MoveAlongX ? (Time.time * this.TranslationMultiplier) : 0f;
			if (this.ForwardAndBackward)
			{
				num += this.GetSineValue(this.Bounce, this.TranslationMultiplier);
			}
			base.transform.position = this.m_RestPos + new Vector3(num, this.UpAndDown ? this.GetSineValue(this.Bounce, this.TranslationMultiplier) : 0f, this.SideToSide ? this.GetSineValue(this.Bounce, this.TranslationMultiplier) : 0f);
			base.transform.rotation = Quaternion.Euler(this.RotateX ? (Mathf.Sin(Time.time * 6f) * 30f * this.RotationMultiplier) : base.transform.eulerAngles.x, this.RotateY ? (Mathf.Sin(Time.time * 6f) * 30f * this.RotationMultiplier) : base.transform.eulerAngles.y, base.transform.eulerAngles.z);
		}

		// Token: 0x0600DE7C RID: 56956 RVA: 0x004FD88C File Offset: 0x004FBA8C
		[PublicizedFrom(EAccessModifier.Private)]
		public float GetSineValue(bool bounce, float mult)
		{
			float num = Mathf.Sin(Time.time * 6f) * 3f * mult;
			if (!bounce)
			{
				return num;
			}
			return Mathf.Abs(num);
		}

		// Token: 0x0400A91A RID: 43290
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 m_RestPos;

		// Token: 0x0400A91B RID: 43291
		public bool MoveAlongX;

		// Token: 0x0400A91C RID: 43292
		public bool ForwardAndBackward;

		// Token: 0x0400A91D RID: 43293
		public bool UpAndDown;

		// Token: 0x0400A91E RID: 43294
		public bool SideToSide;

		// Token: 0x0400A91F RID: 43295
		public bool Bounce;

		// Token: 0x0400A920 RID: 43296
		public float TranslationMultiplier = 1f;

		// Token: 0x0400A921 RID: 43297
		public bool RotateX;

		// Token: 0x0400A922 RID: 43298
		public bool RotateY;

		// Token: 0x0400A923 RID: 43299
		public float RotationMultiplier = 1f;
	}
}
