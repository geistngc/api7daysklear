using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x02001777 RID: 6007
	[Preserve]
	public class UAIConsiderationBase
	{
		// Token: 0x1700169C RID: 5788
		// (get) Token: 0x0600BA52 RID: 47698 RVA: 0x004587E8 File Offset: 0x004569E8
		// (set) Token: 0x0600BA53 RID: 47699 RVA: 0x004587F0 File Offset: 0x004569F0
		public string Name { get; set; }

		// Token: 0x0600BA54 RID: 47700 RVA: 0x004587FC File Offset: 0x004569FC
		public virtual void Init(Dictionary<string, string> parameters)
		{
			if (parameters.ContainsKey("curve"))
			{
				this.curveType = EnumUtils.Parse<CurveType>(parameters["curve"], true);
			}
			else
			{
				this.curveType = CurveType.Linear;
			}
			if (parameters.ContainsKey("flip_x"))
			{
				this.flipX = StringParsers.ParseBool(parameters["flip_x"], 0, -1, true);
			}
			else
			{
				this.flipX = false;
			}
			if (parameters.ContainsKey("flip_y"))
			{
				this.flipY = StringParsers.ParseBool(parameters["flip_y"], 0, -1, true);
			}
			else
			{
				this.flipY = false;
			}
			if (parameters.ContainsKey("x_intercept"))
			{
				this.xIntercept = StringParsers.ParseFloat(parameters["x_intercept"], 0, -1, NumberStyles.Any);
			}
			if (parameters.ContainsKey("y_intercept"))
			{
				this.yIntercept = StringParsers.ParseFloat(parameters["y_intercept"], 0, -1, NumberStyles.Any);
			}
			if (parameters.ContainsKey("slope_intercept"))
			{
				this.slopeIntercept = StringParsers.ParseFloat(parameters["slope_intercept"], 0, -1, NumberStyles.Any);
			}
			if (parameters.ContainsKey("exponent"))
			{
				this.exponent = StringParsers.ParseFloat(parameters["exponent"], 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600BA55 RID: 47701 RVA: 0x00040FA0 File Offset: 0x0003F1A0
		public virtual float GetScore(Context _context, object currentTargetConsideration)
		{
			return 1f;
		}

		// Token: 0x0600BA56 RID: 47702 RVA: 0x0045893C File Offset: 0x00456B3C
		public float ComputeResponseCurve(float x)
		{
			if (this.flipX)
			{
				x = 1f - x;
			}
			float num = 0f;
			switch (this.curveType)
			{
			case CurveType.Constant:
				num = this.yIntercept;
				break;
			case CurveType.Linear:
				num = this.slopeIntercept * (x - this.xIntercept) + this.yIntercept;
				break;
			case CurveType.Quadratic:
				num = this.slopeIntercept * x * Mathf.Pow(Mathf.Abs(x + this.xIntercept), this.exponent) + this.yIntercept;
				break;
			case CurveType.Logistic:
				num = this.exponent * (1f / (1f + Mathf.Pow(Mathf.Abs(1000f * this.slopeIntercept), -1f * x + this.xIntercept + 0.5f))) + this.yIntercept;
				break;
			case CurveType.Logit:
				num = -Mathf.Log(1f / Mathf.Pow(Mathf.Abs(x - this.xIntercept), this.exponent) - 1f) * 0.05f * this.slopeIntercept + (0.5f + this.yIntercept);
				break;
			case CurveType.Threshold:
				num = ((x > this.xIntercept) ? (1f - this.yIntercept) : (0f - (1f - this.slopeIntercept)));
				break;
			case CurveType.Sine:
				num = Mathf.Sin(this.slopeIntercept * Mathf.Pow(x + this.xIntercept, this.exponent)) * 0.5f + 0.5f + this.yIntercept;
				break;
			case CurveType.Parabolic:
				num = Mathf.Pow(this.slopeIntercept * (x + this.xIntercept), 2f) + this.exponent * (x + this.xIntercept) + this.yIntercept;
				break;
			case CurveType.NormalDistribution:
				num = this.exponent / Mathf.Sqrt(6.283192f) * Mathf.Pow(2f, -(1f / (Mathf.Abs(this.slopeIntercept) * 0.01f)) * Mathf.Pow(x - (this.xIntercept + 0.5f), 2f)) + this.yIntercept;
				break;
			case CurveType.Bounce:
				num = Mathf.Abs(Mathf.Sin(6.28f * this.exponent * (x + this.xIntercept + 1f) * (x + this.xIntercept + 1f)) * (1f - x) * this.slopeIntercept) + this.yIntercept;
				break;
			}
			if (this.flipY)
			{
				num = 1f - num;
			}
			return Mathf.Clamp01(num);
		}

		// Token: 0x04008BFC RID: 35836
		[PublicizedFrom(EAccessModifier.Protected)]
		public CurveType curveType;

		// Token: 0x04008BFD RID: 35837
		[PublicizedFrom(EAccessModifier.Private)]
		public float xIntercept;

		// Token: 0x04008BFE RID: 35838
		[PublicizedFrom(EAccessModifier.Private)]
		public float yIntercept;

		// Token: 0x04008BFF RID: 35839
		[PublicizedFrom(EAccessModifier.Private)]
		public float slopeIntercept = 1f;

		// Token: 0x04008C00 RID: 35840
		[PublicizedFrom(EAccessModifier.Private)]
		public float exponent = 1f;

		// Token: 0x04008C01 RID: 35841
		[PublicizedFrom(EAccessModifier.Private)]
		public bool flipY;

		// Token: 0x04008C02 RID: 35842
		[PublicizedFrom(EAccessModifier.Private)]
		public bool flipX;
	}
}
