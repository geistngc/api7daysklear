using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001982 RID: 6530
	[Preserve]
	public class ActionExplodePosition : BaseAction
	{
		// Token: 0x0600C889 RID: 51337 RVA: 0x0049B7F4 File Offset: 0x004999F4
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (base.Owner.TargetPosition.y == 0f)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			Vector3 targetPosition = base.Owner.TargetPosition;
			EntityAlive alive = base.Owner.Target as EntityAlive;
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
			GameManager.Instance.ExplosionServer(targetPosition, World.worldToBlockPos(targetPosition), Quaternion.identity, explosionData, -1, 0.1f, false, null);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C88A RID: 51338 RVA: 0x0049B8FC File Offset: 0x00499AFC
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionExplodePosition.PropBlastPower, ref this.blastPowerText);
			properties.ParseString(ActionExplodePosition.PropBlockDamage, ref this.blockDamageText);
			properties.ParseString(ActionExplodePosition.PropBlockRadius, ref this.blockRadiusText);
			properties.ParseString(ActionExplodePosition.PropEntityDamage, ref this.entityDamageText);
			properties.ParseString(ActionExplodePosition.PropEntityRadius, ref this.entityRadiusText);
			properties.ParseString(ActionExplodePosition.PropBlockTags, ref this.blockTags);
			properties.ParseInt(ActionExplodePosition.PropParticleIndex, ref this.particleIndex);
			properties.ParseBool(ActionExplodePosition.PropIgnoreHeatMap, ref this.ignoreHeatMap);
		}

		// Token: 0x0600C88B RID: 51339 RVA: 0x0049B998 File Offset: 0x00499B98
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionExplodePosition
			{
				blastPowerText = this.blastPowerText,
				blockDamageText = this.blockDamageText,
				blockRadiusText = this.blockRadiusText,
				entityDamageText = this.entityDamageText,
				entityRadiusText = this.entityRadiusText,
				particleIndex = this.particleIndex,
				blockTags = this.blockTags
			};
		}

		// Token: 0x040097B7 RID: 38839
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blastPowerText;

		// Token: 0x040097B8 RID: 38840
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockDamageText;

		// Token: 0x040097B9 RID: 38841
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockRadiusText;

		// Token: 0x040097BA RID: 38842
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityDamageText;

		// Token: 0x040097BB RID: 38843
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityRadiusText;

		// Token: 0x040097BC RID: 38844
		[PublicizedFrom(EAccessModifier.Protected)]
		public int particleIndex = 13;

		// Token: 0x040097BD RID: 38845
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool ignoreHeatMap = true;

		// Token: 0x040097BE RID: 38846
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockTags = "";

		// Token: 0x040097BF RID: 38847
		public static string PropBlastPower = "blast_power";

		// Token: 0x040097C0 RID: 38848
		public static string PropBlockDamage = "block_damage";

		// Token: 0x040097C1 RID: 38849
		public static string PropBlockRadius = "block_radius";

		// Token: 0x040097C2 RID: 38850
		public static string PropBlockTags = "block_tags";

		// Token: 0x040097C3 RID: 38851
		public static string PropEntityDamage = "entity_damage";

		// Token: 0x040097C4 RID: 38852
		public static string PropEntityRadius = "entity_radius";

		// Token: 0x040097C5 RID: 38853
		public static string PropParticleIndex = "particle_index";

		// Token: 0x040097C6 RID: 38854
		public static string PropIgnoreHeatMap = "ignore_heatmap";
	}
}
