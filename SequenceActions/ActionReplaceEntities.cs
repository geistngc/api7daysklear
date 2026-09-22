using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019AD RID: 6573
	[Preserve]
	public class ActionReplaceEntities : ActionBaseTargetAction
	{
		// Token: 0x0600C937 RID: 51511 RVA: 0x004A027C File Offset: 0x0049E47C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			base.OnInit();
			string[] array = this.entityNames.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
				{
					if (keyValuePair.Value.entityClassName == array[i])
					{
						this.entityIDs.Add(keyValuePair.Key);
						if (this.entityIDs.Count == array.Length)
						{
							break;
						}
					}
				}
			}
			if (this.singleChoice && this.selectedEntityIndex == -1)
			{
				this.selectedEntityIndex = UnityEngine.Random.Range(0, this.entityIDs.Count);
			}
		}

		// Token: 0x0600C938 RID: 51512 RVA: 0x004A0354 File Offset: 0x0049E554
		public override void StartTargetAction()
		{
			this.newList = new List<Entity>();
		}

		// Token: 0x0600C939 RID: 51513 RVA: 0x004A0361 File Offset: 0x0049E561
		public override void EndTargetAction()
		{
			if (this.targetGroup != "")
			{
				base.Owner.AddEntitiesToGroup(this.targetGroup, this.newList, false);
			}
		}

		// Token: 0x0600C93A RID: 51514 RVA: 0x004A0390 File Offset: 0x0049E590
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			World world = GameManager.Instance.World;
			if (target != null && !(target is EntityPlayer))
			{
				int index = (this.selectedEntityIndex == -1) ? UnityEngine.Random.Range(0, this.entityIDs.Count) : this.selectedEntityIndex;
				Entity entity = EntityFactory.CreateEntity(this.entityIDs[index], target.position, target.rotation, (base.Owner.Target != null) ? base.Owner.Target.entityId : -1, base.Owner.ExtraData);
				entity.SetSpawnerSource(EnumSpawnerSource.Dynamic);
				world.SpawnEntityInWorld(entity);
				this.newList.Add(entity);
				if (this.attackTarget)
				{
					EntityAlive entityAlive = entity as EntityAlive;
					if (entityAlive != null)
					{
						EntityAlive entityAlive2 = base.Owner.Target as EntityAlive;
						if (entityAlive2 != null)
						{
							GameEventManager.Current.RegisterSpawnedEntity(entityAlive, entityAlive2, base.Owner.Requester, base.Owner, true);
							entityAlive.SetAttackTarget(entityAlive2, 12000);
							entityAlive.aiManager.SetTargetOnlyPlayers(100f);
							if (base.Owner.Requester != null)
							{
								GameEventActionSequence gameEventActionSequence = (base.Owner.OwnerSequence == null) ? base.Owner : base.Owner.OwnerSequence;
								if (base.Owner.Requester is EntityPlayerLocal)
								{
									GameEventManager.Current.HandleGameEntitySpawned(gameEventActionSequence.Name, entity.entityId, gameEventActionSequence.Tag);
								}
								else
								{
									SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(gameEventActionSequence.Name, gameEventActionSequence.Target.entityId, gameEventActionSequence.ExtraData, gameEventActionSequence.Tag, NetPackageGameEventResponse.ResponseTypes.EntitySpawned, entity.entityId, -1, false, ""), false, gameEventActionSequence.Requester.entityId, -1, -1, null, 192, false);
								}
							}
						}
					}
				}
				this.HandleRemoveData(target);
				GameManager.Instance.StartCoroutine(this.removeLater(target));
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C93B RID: 51515 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleRemoveData(Entity ent)
		{
		}

		// Token: 0x0600C93C RID: 51516 RVA: 0x004A059E File Offset: 0x0049E79E
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator removeLater(Entity e)
		{
			yield return new WaitForSeconds(0.25f);
			if (e is EntityVehicle)
			{
				(e as EntityVehicle).Kill();
			}
			GameManager.Instance.World.RemoveEntity(e.entityId, EnumRemoveEntityReason.Killed);
			yield break;
		}

		// Token: 0x0600C93D RID: 51517 RVA: 0x004A05B0 File Offset: 0x0049E7B0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionReplaceEntities.PropEntityNames))
			{
				this.entityNames = properties.Values[ActionReplaceEntities.PropEntityNames];
			}
			if (properties.Values.ContainsKey(ActionReplaceEntities.PropSingleChoice))
			{
				this.singleChoice = StringParsers.ParseBool(properties.Values[ActionReplaceEntities.PropSingleChoice], 0, -1, true);
			}
			properties.ParseBool(ActionReplaceEntities.PropAttackTarget, ref this.attackTarget);
		}

		// Token: 0x0600C93E RID: 51518 RVA: 0x004A0630 File Offset: 0x0049E830
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionReplaceEntities
			{
				entityNames = this.entityNames,
				entityIDs = this.entityIDs,
				singleChoice = this.singleChoice,
				targetGroup = this.targetGroup,
				selectedEntityIndex = this.selectedEntityIndex,
				attackTarget = this.attackTarget
			};
		}

		// Token: 0x040098BB RID: 39099
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityNames = "";

		// Token: 0x040098BC RID: 39100
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool singleChoice;

		// Token: 0x040098BD RID: 39101
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEntityNames = "entity_names";

		// Token: 0x040098BE RID: 39102
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSingleChoice = "single_choice";

		// Token: 0x040098BF RID: 39103
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAttackTarget = "attack_target";

		// Token: 0x040098C0 RID: 39104
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<int> entityIDs = new List<int>();

		// Token: 0x040098C1 RID: 39105
		[PublicizedFrom(EAccessModifier.Protected)]
		public int selectedEntityIndex = -1;

		// Token: 0x040098C2 RID: 39106
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<Entity> newList;

		// Token: 0x040098C3 RID: 39107
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool attackTarget = true;
	}
}
