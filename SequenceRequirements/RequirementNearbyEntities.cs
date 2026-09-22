using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001942 RID: 6466
	[Preserve]
	public class RequirementNearbyEntities : BaseRequirement
	{
		// Token: 0x0600C752 RID: 51026 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C753 RID: 51027 RVA: 0x0049482C File Offset: 0x00492A2C
		public override bool CanPerform(Entity target)
		{
			FastTags<TagGroup.Global> tags = (this.tag == "") ? FastTags<TagGroup.Global>.none : FastTags<TagGroup.Global>.Parse(this.tag);
			List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(target, new Bounds(target.position, Vector3.one * 2f * this.maxDistance), this.currentState == RequirementNearbyEntities.EntityStates.Live);
			if (this.targetIsOwner)
			{
				PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerDataFromEntityID(this.Owner.Target.entityId);
				for (int i = 0; i < entitiesInBounds.Count; i++)
				{
					if (entitiesInBounds[i].HasAnyTags(tags))
					{
						EntityVehicle entityVehicle = entitiesInBounds[i] as EntityVehicle;
						if (entityVehicle != null)
						{
							if (this.targetIsOwner && !entityVehicle.IsOwner(playerDataFromEntityID.PrimaryId))
							{
								goto IL_106;
							}
						}
						else
						{
							EntityTurret entityTurret = entitiesInBounds[i] as EntityTurret;
							if (entityTurret == null || (this.targetIsOwner && entityTurret.OwnerID != null && !entityTurret.OwnerID.Equals(playerDataFromEntityID.PrimaryId)))
							{
								goto IL_106;
							}
						}
						return true;
					}
					IL_106:;
				}
			}
			else
			{
				for (int j = 0; j < entitiesInBounds.Count; j++)
				{
					Entity entity = entitiesInBounds[j];
					if (tags.IsEmpty)
					{
						if (entity is EntityAnimal || entity is EntityEnemy)
						{
							return true;
						}
					}
					else if (entity.HasAnyTags(tags))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600C754 RID: 51028 RVA: 0x00494997 File Offset: 0x00492B97
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementNearbyEntities.PropTag, ref this.tag);
			properties.ParseFloat(RequirementNearbyEntities.PropMaxDistance, ref this.maxDistance);
			properties.ParseEnum<RequirementNearbyEntities.EntityStates>(RequirementNearbyEntities.PropEntityState, ref this.currentState);
		}

		// Token: 0x0600C755 RID: 51029 RVA: 0x004949D3 File Offset: 0x00492BD3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementNearbyEntities
			{
				maxDistance = this.maxDistance,
				tag = this.tag,
				currentState = this.currentState
			};
		}

		// Token: 0x04009654 RID: 38484
		[PublicizedFrom(EAccessModifier.Protected)]
		public string tag = "";

		// Token: 0x04009655 RID: 38485
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxDistance = 10f;

		// Token: 0x04009656 RID: 38486
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool targetIsOwner;

		// Token: 0x04009657 RID: 38487
		[PublicizedFrom(EAccessModifier.Protected)]
		public RequirementNearbyEntities.EntityStates currentState;

		// Token: 0x04009658 RID: 38488
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTag = "entity_tags";

		// Token: 0x04009659 RID: 38489
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEntityState = "entity_state";

		// Token: 0x0400965A RID: 38490
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxDistance = "max_distance";

		// Token: 0x0400965B RID: 38491
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetIsOwner = "target_is_owner";

		// Token: 0x02001943 RID: 6467
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum EntityStates
		{
			// Token: 0x0400965D RID: 38493
			Live,
			// Token: 0x0400965E RID: 38494
			Dead
		}
	}
}
