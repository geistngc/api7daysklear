using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x0200177D RID: 6013
	[Preserve]
	public class UAIConsiderationTargetFactionStanding : UAIConsiderationBase
	{
		// Token: 0x0600BA68 RID: 47720 RVA: 0x00459294 File Offset: 0x00457494
		public override void Init(Dictionary<string, string> parameters)
		{
			base.Init(parameters);
			if (parameters.ContainsKey("min"))
			{
				this.min = StringParsers.ParseFloat(parameters["min"], 0, -1, NumberStyles.Any);
			}
			else
			{
				this.min = 0f;
			}
			if (parameters.ContainsKey("max"))
			{
				this.max = StringParsers.ParseFloat(parameters["max"], 0, -1, NumberStyles.Any);
				return;
			}
			this.max = 255f;
		}

		// Token: 0x0600BA69 RID: 47721 RVA: 0x00459318 File Offset: 0x00457518
		public override float GetScore(Context _context, object target)
		{
			if (target is EntityAlive)
			{
				EntityAlive targetEntity = UAIUtils.ConvertToEntityAlive(target);
				return (FactionManager.Instance.GetRelationshipValue(_context.Self, targetEntity) - this.min) / (this.max - this.min);
			}
			return 0f;
		}

		// Token: 0x04008C12 RID: 35858
		[PublicizedFrom(EAccessModifier.Private)]
		public float min;

		// Token: 0x04008C13 RID: 35859
		[PublicizedFrom(EAccessModifier.Private)]
		public float max;
	}
}
