using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200195E RID: 6494
	[Preserve]
	public class ActionAddEntitiesToGroup : BaseAction
	{
		// Token: 0x0600C7E3 RID: 51171 RVA: 0x004973A8 File Offset: 0x004955A8
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			FastTags<TagGroup.Global> tags = (this.tag == "") ? FastTags<TagGroup.Global>.none : FastTags<TagGroup.Global>.Parse(this.tag);
			World world = GameManager.Instance.World;
			Vector3 size = (this.yHeight == -1f) ? (Vector3.one * 2f * this.maxDistance) : new Vector3(2f * this.maxDistance, this.yHeight, 2f * this.maxDistance);
			Vector3 vector = (base.Owner.Target != null) ? base.Owner.Target.position : base.Owner.TargetPosition;
			if (this.yHeight != -1f)
			{
				vector += Vector3.one * (this.yHeight * 0.5f);
			}
			List<Entity> entitiesInBounds = world.GetEntitiesInBounds(this.excludeTarget ? base.Owner.Target : null, new Bounds(vector, size), this.currentState == ActionAddEntitiesToGroup.EntityStates.Live);
			List<Entity> list = new List<Entity>();
			if (this.targetIsOwner && base.Owner.Target != null)
			{
				PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerDataFromEntityID(base.Owner.Target.entityId);
				for (int i = 0; i < entitiesInBounds.Count; i++)
				{
					if (entitiesInBounds[i].HasAnyTags(tags))
					{
						EntityVehicle entityVehicle = entitiesInBounds[i] as EntityVehicle;
						if (entityVehicle != null)
						{
							if (entityVehicle.GetOwner() == null)
							{
								goto IL_1E2;
							}
							if (this.targetIsOwner && !entityVehicle.IsOwner(playerDataFromEntityID.PrimaryId))
							{
								goto IL_1E2;
							}
						}
						else
						{
							EntityTurret entityTurret = entitiesInBounds[i] as EntityTurret;
							if (entityTurret == null || entityTurret.OwnerID == null || (this.targetIsOwner && !entityTurret.OwnerID.Equals(playerDataFromEntityID.PrimaryId)))
							{
								goto IL_1E2;
							}
						}
						list.Add(entitiesInBounds[i]);
					}
					IL_1E2:;
				}
			}
			else
			{
				for (int j = 0; j < entitiesInBounds.Count; j++)
				{
					Entity entity = entitiesInBounds[j];
					if (tags.IsEmpty)
					{
						if (entity is EntityEnemyAnimal || entity is EntityEnemy || (this.allowPlayers && entity is EntityPlayer))
						{
							list.Add(entity);
						}
					}
					else if (entity.HasAnyTags(tags))
					{
						list.Add(entity);
					}
				}
			}
			base.Owner.AddEntitiesToGroup(this.groupName, list, this.twitchNegative);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C7E4 RID: 51172 RVA: 0x00497630 File Offset: 0x00495830
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddEntitiesToGroup.PropGroupName, ref this.groupName);
			properties.ParseString(ActionAddEntitiesToGroup.PropTag, ref this.tag);
			properties.ParseFloat(ActionAddEntitiesToGroup.PropMaxDistance, ref this.maxDistance);
			properties.ParseBool(ActionAddEntitiesToGroup.PropTwitchNegative, ref this.twitchNegative);
			properties.ParseBool(ActionAddEntitiesToGroup.PropTargetIsOwner, ref this.targetIsOwner);
			properties.ParseBool(ActionAddEntitiesToGroup.PropExcludeTarget, ref this.excludeTarget);
			properties.ParseBool(ActionAddEntitiesToGroup.PropAllowPlayers, ref this.allowPlayers);
			properties.ParseFloat(ActionAddEntitiesToGroup.PropYHeight, ref this.yHeight);
			properties.ParseEnum<ActionAddEntitiesToGroup.EntityStates>(ActionAddEntitiesToGroup.PropEntityState, ref this.currentState);
		}

		// Token: 0x0600C7E5 RID: 51173 RVA: 0x004976E0 File Offset: 0x004958E0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddEntitiesToGroup
			{
				maxDistance = this.maxDistance,
				tag = this.tag,
				groupName = this.groupName,
				twitchNegative = this.twitchNegative,
				targetIsOwner = this.targetIsOwner,
				yHeight = this.yHeight,
				currentState = this.currentState,
				excludeTarget = this.excludeTarget,
				allowPlayers = this.allowPlayers
			};
		}

		// Token: 0x040096DF RID: 38623
		[PublicizedFrom(EAccessModifier.Protected)]
		public string groupName = "";

		// Token: 0x040096E0 RID: 38624
		[PublicizedFrom(EAccessModifier.Protected)]
		public string tag = "";

		// Token: 0x040096E1 RID: 38625
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxDistance = 10f;

		// Token: 0x040096E2 RID: 38626
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool twitchNegative = true;

		// Token: 0x040096E3 RID: 38627
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool targetIsOwner;

		// Token: 0x040096E4 RID: 38628
		[PublicizedFrom(EAccessModifier.Protected)]
		public float yHeight = -1f;

		// Token: 0x040096E5 RID: 38629
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool excludeTarget = true;

		// Token: 0x040096E6 RID: 38630
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool allowPlayers;

		// Token: 0x040096E7 RID: 38631
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionAddEntitiesToGroup.EntityStates currentState;

		// Token: 0x040096E8 RID: 38632
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGroupName = "group_name";

		// Token: 0x040096E9 RID: 38633
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTag = "entity_tags";

		// Token: 0x040096EA RID: 38634
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEntityState = "entity_state";

		// Token: 0x040096EB RID: 38635
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxDistance = "max_distance";

		// Token: 0x040096EC RID: 38636
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTwitchNegative = "twitch_negative";

		// Token: 0x040096ED RID: 38637
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetIsOwner = "target_is_owner";

		// Token: 0x040096EE RID: 38638
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropYHeight = "y_height";

		// Token: 0x040096EF RID: 38639
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropExcludeTarget = "exclude_target";

		// Token: 0x040096F0 RID: 38640
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAllowPlayers = "allow_players";

		// Token: 0x0200195F RID: 6495
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum EntityStates
		{
			// Token: 0x040096F2 RID: 38642
			Live,
			// Token: 0x040096F3 RID: 38643
			Dead
		}
	}
}
