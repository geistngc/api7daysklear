using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001983 RID: 6531
	[Preserve]
	public class ActionExplodeTarget : ActionBaseTargetAction
	{
		// Token: 0x0600C88E RID: 51342 RVA: 0x0049BA80 File Offset: 0x00499C80
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			EntityAlive alive = base.Owner.Target as EntityAlive;
			if (entityAlive != null)
			{
				ExplosionData explosionData = new ExplosionData
				{
					BlastPower = GameEventManager.GetIntValue(alive, this.blastPowerText, 75),
					BlockDamage = GameEventManager.GetFloatValue(alive, this.blockDamageText, 1f),
					BlockRadius = GameEventManager.GetFloatValue(alive, this.blockRadiusText, 4f),
					BlockTags = this.blockTags,
					EntityDamage = GameEventManager.GetFloatValue(alive, this.entityDamageText, 5000f),
					EntityRadius = GameEventManager.GetIntValue(alive, this.entityRadiusText, 3),
					ParticleIndex = this.particleIndex,
					IgnoreHeatMap = this.ignoreHeatMap
				};
				GameManager.Instance.ExplosionServer(entityAlive.position, entityAlive.GetBlockPosition(), entityAlive.qrotation, explosionData, -1, 0.1f, false, null);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C88F RID: 51343 RVA: 0x0049BB7C File Offset: 0x00499D7C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionExplodeTarget.PropBlastPower, ref this.blastPowerText);
			properties.ParseString(ActionExplodeTarget.PropBlockDamage, ref this.blockDamageText);
			properties.ParseString(ActionExplodeTarget.PropBlockRadius, ref this.blockRadiusText);
			properties.ParseString(ActionExplodeTarget.PropEntityDamage, ref this.entityDamageText);
			properties.ParseString(ActionExplodeTarget.PropEntityRadius, ref this.entityRadiusText);
			properties.ParseString(ActionExplodeTarget.PropBlockTags, ref this.blockTags);
			properties.ParseInt(ActionExplodeTarget.PropParticleIndex, ref this.particleIndex);
			properties.ParseBool(ActionExplodeTarget.PropIgnoreHeatMap, ref this.ignoreHeatMap);
		}

		// Token: 0x0600C890 RID: 51344 RVA: 0x0049BC18 File Offset: 0x00499E18
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionExplodeTarget
			{
				targetGroup = this.targetGroup,
				blastPowerText = this.blastPowerText,
				blockDamageText = this.blockDamageText,
				blockRadiusText = this.blockRadiusText,
				entityDamageText = this.entityDamageText,
				entityRadiusText = this.entityRadiusText,
				particleIndex = this.particleIndex,
				blockTags = this.blockTags
			};
		}

		// Token: 0x040097C7 RID: 38855
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blastPowerText;

		// Token: 0x040097C8 RID: 38856
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockDamageText;

		// Token: 0x040097C9 RID: 38857
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockRadiusText;

		// Token: 0x040097CA RID: 38858
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityDamageText;

		// Token: 0x040097CB RID: 38859
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityRadiusText;

		// Token: 0x040097CC RID: 38860
		[PublicizedFrom(EAccessModifier.Protected)]
		public int particleIndex = 13;

		// Token: 0x040097CD RID: 38861
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool ignoreHeatMap = true;

		// Token: 0x040097CE RID: 38862
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockTags = "";

		// Token: 0x040097CF RID: 38863
		public static string PropBlastPower = "blast_power";

		// Token: 0x040097D0 RID: 38864
		public static string PropBlockDamage = "block_damage";

		// Token: 0x040097D1 RID: 38865
		public static string PropBlockRadius = "block_radius";

		// Token: 0x040097D2 RID: 38866
		public static string PropBlockTags = "block_tags";

		// Token: 0x040097D3 RID: 38867
		public static string PropEntityDamage = "entity_damage";

		// Token: 0x040097D4 RID: 38868
		public static string PropEntityRadius = "entity_radius";

		// Token: 0x040097D5 RID: 38869
		public static string PropParticleIndex = "particle_index";

		// Token: 0x040097D6 RID: 38870
		public static string PropIgnoreHeatMap = "ignore_heatmap";
	}
}
