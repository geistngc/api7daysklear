using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001928 RID: 6440
	[Preserve]
	public class RequirementCVar : BaseOperationRequirement
	{
		// Token: 0x0600C6D3 RID: 50899 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C6D4 RID: 50900 RVA: 0x00493934 File Offset: 0x00491B34
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			return (entityAlive != null) ? entityAlive.Buffs.GetCustomVar(this.cvar) : 0f;
		}

		// Token: 0x0600C6D5 RID: 50901 RVA: 0x00493968 File Offset: 0x00491B68
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetFloatValue(target as EntityAlive, this.valueText, 0f);
		}

		// Token: 0x0600C6D6 RID: 50902 RVA: 0x00493985 File Offset: 0x00491B85
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementCVar.PropCvar, ref this.cvar);
			properties.ParseString(RequirementCVar.PropValue, ref this.valueText);
		}

		// Token: 0x0600C6D7 RID: 50903 RVA: 0x004939B0 File Offset: 0x00491BB0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementCVar
			{
				Invert = this.Invert,
				operation = this.operation,
				cvar = this.cvar,
				valueText = this.valueText
			};
		}

		// Token: 0x04009628 RID: 38440
		[PublicizedFrom(EAccessModifier.Protected)]
		public string cvar = "";

		// Token: 0x04009629 RID: 38441
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x0400962A RID: 38442
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropCvar = "cvar";

		// Token: 0x0400962B RID: 38443
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
