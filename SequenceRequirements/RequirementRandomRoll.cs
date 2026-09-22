using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001946 RID: 6470
	[Preserve]
	public class RequirementRandomRoll : BaseOperationRequirement
	{
		// Token: 0x0600C764 RID: 51044 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C765 RID: 51045 RVA: 0x00494C0C File Offset: 0x00492E0C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			float randomFloat = GameEventManager.Current.Random.RandomFloat;
			return Mathf.Lerp(this.minMax.x, this.minMax.y, randomFloat);
		}

		// Token: 0x0600C766 RID: 51046 RVA: 0x00494C4A File Offset: 0x00492E4A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetFloatValue(target as EntityAlive, this.valueText, 0f);
		}

		// Token: 0x0600C767 RID: 51047 RVA: 0x00494C67 File Offset: 0x00492E67
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseVec(RequirementRandomRoll.PropMinMax, ref this.minMax);
			properties.ParseString(RequirementRandomRoll.PropValue, ref this.valueText);
		}

		// Token: 0x0600C768 RID: 51048 RVA: 0x00494C92 File Offset: 0x00492E92
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementRandomRoll
			{
				Invert = this.Invert,
				operation = this.operation,
				minMax = this.minMax,
				valueText = this.valueText
			};
		}

		// Token: 0x04009666 RID: 38502
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector2 minMax;

		// Token: 0x04009667 RID: 38503
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameRandom rand;

		// Token: 0x04009668 RID: 38504
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04009669 RID: 38505
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMinMax = "min_max";

		// Token: 0x0400966A RID: 38506
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
