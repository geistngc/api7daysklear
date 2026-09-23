using System;
using System.Collections.Generic;
using System.Diagnostics;
using Twitch;
using UnityEngine;

// Token: 0x02000543 RID: 1347
public class Explosion
{
	// Token: 0x06002C4A RID: 11338 RVA: 0x00117D60 File Offset: 0x00115F60
	public Explosion(World _world, Vector3 _worldPos, Vector3i _blockPos, ExplosionData _explosionData, int _entityId)
	{
		this.world = _world;
		this.worldPos = _worldPos;
		this.blockPos = _blockPos;
		this.explosionData = _explosionData;
		this.entityId = _entityId;
		this.damagedBlockPositions = new HashSet<Vector3i>();
		this.ChangedBlockPositions = new Dictionary<Vector3i, BlockChangeInfo>();
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x00117DB0 File Offset: 0x00115FB0
	public void AttackBlocks(int _entityThatCausedExplosion, ItemValue _itemValueExplosionSource)
	{
		ChunkCluster chunkCache = this.world.ChunkCache;
		if (chunkCache == null)
		{
			return;
		}
		EntityAlive entityAlive = this.world.GetEntity(_entityThatCausedExplosion) as EntityAlive;
		PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_entityThatCausedExplosion);
		FastTags<TagGroup.Global> fastTags = Explosion.explosionTag;
		if (_itemValueExplosionSource != null)
		{
			ItemClass itemClass = _itemValueExplosionSource.ItemClass;
			fastTags |= itemClass.ItemTags;
		}
		float num = EffectManager.GetValue(PassiveEffects.ExplosionRadius, _itemValueExplosionSource, this.explosionData.BlockRadius, entityAlive, null, fastTags, true, true, true, true, true, 1, true, false);
		if (num == 0f)
		{
			num = 0.01f;
		}
		int num2 = Mathf.CeilToInt(num);
		FastTags<TagGroup.Global> other = (this.explosionData.BlockTags.Length > 0) ? FastTags<TagGroup.Global>.Parse(this.explosionData.BlockTags) : FastTags<TagGroup.Global>.none;
		bool flag = !other.IsEmpty;
		if (chunkCache.GetBlock(this.blockPos).Block.shape.IsTerrain() && !chunkCache.GetBlock(this.blockPos + Vector3i.up).Block.shape.IsTerrain())
		{
			this.blockPos.y = this.blockPos.y + 1;
		}
		Vector3 vector = this.blockPos.ToVector3Center();
		bool flag2 = false;
		for (int i = -num2; i <= num2; i++)
		{
			for (int j = -num2; j <= num2; j++)
			{
				for (int k = -num2; k <= num2; k++)
				{
					Vector3 a = vector;
					Vector3 vector2 = new Vector3((float)i, (float)j, (float)k);
					float num3 = vector2.magnitude + 0.001f;
					vector2 *= 0.51f / num3;
					while (num3 >= -0.1f)
					{
						Vector3i vector3i = World.worldToBlockPos(a);
						BlockValue block = chunkCache.GetBlock(vector3i);
						if (!block.isair && !block.isWater)
						{
							if (this.damagedBlockPositions.Contains(vector3i))
							{
								break;
							}
							Block block2 = block.Block;
							if (!block2.StabilityIgnore)
							{
								if (vector3i != this.blockPos)
								{
									this.damagedBlockPositions.Add(vector3i);
								}
								else
								{
									if (flag2)
									{
										goto IL_5F8;
									}
									flag2 = true;
								}
								float num4 = (vector3i.ToVector3Center() - this.worldPos).magnitude - 0.5f;
								if (num4 < 0f)
								{
									num4 = 0f;
								}
								FastTags<TagGroup.Global> fastTags2 = fastTags;
								fastTags2 |= block2.Tags;
								float num5 = EffectManager.GetValue(PassiveEffects.ExplosionBlockDamage, _itemValueExplosionSource, this.explosionData.BlockDamage, entityAlive, null, fastTags2, true, true, true, true, true, 1, true, false);
								if (entityAlive)
								{
									num5 = num5 * entityAlive.GetBlockDamageScale(block.isTerrain) + 0.5f;
								}
								if (Utils.FastMax(num5, 1f) / (float)(2 * num2 + 1) > 0f)
								{
									float num6 = (1f - num4 / num) * num5;
									if (flag && block2.Tags.Test_AnySet(other) && this.explosionData.damageMultiplier != null)
									{
										num6 *= this.explosionData.damageMultiplier.Get("tags");
									}
									if (num6 <= 0f)
									{
										break;
									}
									if (block.ischild)
									{
										vector3i = block2.multiBlockPos.GetParentPos(vector3i, block);
										block = this.world.GetBlock(vector3i);
										block2 = block.Block;
									}
									BlockChangeInfo blockChangeInfo;
									if (!this.ChangedBlockPositions.TryGetValue(vector3i, out blockChangeInfo))
									{
										blockChangeInfo = new BlockChangeInfo(vector3i, block);
										blockChangeInfo.bChangeDamage = true;
										if (blockChangeInfo.blockValue.isWater)
										{
											goto IL_5F8;
										}
										this.ChangedBlockPositions[vector3i] = blockChangeInfo;
									}
									if (!blockChangeInfo.blockValue.isair && (World.SandboxUseTraderArea != TraderAreaStates.Default || !this.world.IsWithinTraderArea(vector3i)) && this.world.InBoundsForPlayersPercent(vector3i.ToVector3CenterXZ()) >= 0.5f)
									{
										Block block3 = blockChangeInfo.blockValue.Block;
										float explosionResistance = block3.GetExplosionResistance();
										float hardness = block3.GetHardness();
										float landProtectionHardnessModifier = this.world.GetLandProtectionHardnessModifier(vector3i, entityAlive, playerDataFromEntityID);
										int num7 = 0;
										if (hardness > 0f)
										{
											float num8 = (1f - explosionResistance) * num6 / (hardness * landProtectionHardnessModifier);
											if (this.explosionData.damageMultiplier != null)
											{
												num8 *= this.explosionData.damageMultiplier.Get(block3.blockMaterial.DamageCategory);
											}
											num7 = Mathf.RoundToInt(num8);
										}
										else if (landProtectionHardnessModifier > 0f)
										{
											num7 = Mathf.RoundToInt((float)block2.MaxDamage / landProtectionHardnessModifier);
										}
										if (num7 > 0)
										{
											if (num7 + blockChangeInfo.blockValue.damage >= block2.MaxDamage)
											{
												num7 = num7 + blockChangeInfo.blockValue.damage - block2.MaxDamage;
												blockChangeInfo.bChangeDamage = false;
												Block.DestroyedResult destroyedResult = block3.OnBlockDestroyedByExplosion(this.world, vector3i, blockChangeInfo.blockValue, this.entityId);
												if (!block3.DowngradeBlock.isair && destroyedResult == Block.DestroyedResult.Downgrade)
												{
													do
													{
														BlockValue blockValue = blockChangeInfo.blockValue.Block.DowngradeBlock;
														blockValue = BlockPlaceholderMap.Instance.Replace(blockValue, this.world.GetGameRandom(), vector3i.x, vector3i.z, false);
														blockValue.rotation = blockChangeInfo.blockValue.rotation;
														blockValue.meta = blockChangeInfo.blockValue.meta;
														blockChangeInfo.blockValue = blockValue;
														blockChangeInfo.blockValue.damage = num7;
														num7 -= blockChangeInfo.blockValue.Block.MaxDamage;
													}
													while (num7 > 0 && !blockChangeInfo.blockValue.Block.DowngradeBlock.isair);
													if (num7 >= 0)
													{
														blockChangeInfo.blockValue = BlockValue.Air;
													}
												}
												else
												{
													blockChangeInfo.blockValue = BlockValue.Air;
												}
												if (!blockChangeInfo.blockValue.isair)
												{
													break;
												}
												this.damagedBlockPositions.Remove(vector3i);
											}
											else
											{
												blockChangeInfo.blockValue.damage = num7 + blockChangeInfo.blockValue.damage;
											}
											if (!blockChangeInfo.blockValue.isair)
											{
												break;
											}
										}
									}
								}
							}
						}
						IL_5F8:
						num3 -= 0.51f;
						a += vector2;
					}
				}
			}
		}
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x00118404 File Offset: 0x00116604
	public void AttackEntites(int _entityThatCausedExplosion, ItemValue _itemValueExplosionSource, EnumDamageTypes damageType)
	{
		EntityAlive entityAlive = this.world.GetEntity(_entityThatCausedExplosion) as EntityAlive;
		FastTags<TagGroup.Global> fastTags = Explosion.explosionTag;
		if (_itemValueExplosionSource != null)
		{
			ItemClass itemClass = _itemValueExplosionSource.ItemClass;
			fastTags |= itemClass.ItemTags;
		}
		float value = EffectManager.GetValue(PassiveEffects.ExplosionEntityDamage, _itemValueExplosionSource, this.explosionData.EntityDamage, entityAlive, null, fastTags, true, true, true, true, true, 1, true, false);
		float value2 = EffectManager.GetValue(PassiveEffects.ExplosionRadius, _itemValueExplosionSource, (float)this.explosionData.EntityRadius, entityAlive, null, fastTags, true, true, true, true, true, 1, true, false);
		Collider[] array = Physics.OverlapSphere(this.worldPos - Origin.position, value2, -538480653);
		bool flag = false;
		Vector3i vector3i = World.worldToBlockPos(this.worldPos);
		foreach (Collider collider in array)
		{
			if (collider)
			{
				Transform transform = collider.transform;
				if (transform.CompareTag("Item"))
				{
					EntityItem component = transform.GetComponent<EntityItem>();
					if (!component)
					{
						RootTransformRefEntity component2 = transform.GetComponent<RootTransformRefEntity>();
						if (component2)
						{
							component = component2.RootTransform.GetComponent<EntityItem>();
						}
					}
					if (component && !component.IsDead() && !Explosion.hitEntities.ContainsKey(component.entityId))
					{
						Explosion.hitEntities.Add(component.entityId, default(Explosion.DamageRecord));
						component.OnDamagedByExplosion();
						component.SetDead();
					}
				}
				else
				{
					string tag = transform.tag;
					if (tag.StartsWith("E_BP_"))
					{
						flag = true;
						Transform hitRootTransform = GameUtils.GetHitRootTransform(tag, transform);
						EntityAlive entityAlive2 = hitRootTransform ? hitRootTransform.GetComponent<EntityAlive>() : null;
						if (entityAlive2)
						{
							entityAlive2.ConditionalTriggerSleeperWakeUp();
							Vector3 vector = transform.position + Origin.position - this.worldPos;
							float magnitude = vector.magnitude;
							vector.Normalize();
							if (!Voxel.Raycast(this.world, new Ray(this.worldPos, vector), magnitude, 65536, 66, 0f))
							{
								EntityClass entityClass = EntityClass.list[entityAlive2.entityClass];
								float num;
								if (transform.CompareTag("E_BP_LArm") || transform.CompareTag("E_BP_RArm"))
								{
									num = entityClass.ArmsExplosionDamageMultiplier;
								}
								else if (transform.CompareTag("E_BP_LLeg") || transform.CompareTag("E_BP_RLeg"))
								{
									num = entityClass.LegsExplosionDamageMultiplier;
								}
								else if (transform.CompareTag("E_BP_Head"))
								{
									num = entityClass.HeadExplosionDamageMultiplier;
								}
								else
								{
									num = entityClass.ChestExplosionDamageMultiplier;
								}
								float num2 = value * num;
								num2 *= 1f - magnitude / value2;
								num2 = (float)((int)EffectManager.GetValue(PassiveEffects.ExplosionIncomingDamage, null, num2, entityAlive2, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
								if (num2 >= 3f)
								{
									Explosion.DamageRecord damageRecord;
									if (!Explosion.hitEntities.TryGetValue(entityAlive2.entityId, out damageRecord))
									{
										damageRecord.entity = entityAlive2;
									}
									EnumBodyPartHit enumBodyPartHit = DamageSource.TagToBodyPart(tag);
									if (num2 > damageRecord.damage)
									{
										damageRecord.damage = num2;
										damageRecord.dir = vector;
										damageRecord.hitTransform = transform;
										damageRecord.mainPart = enumBodyPartHit;
									}
									damageRecord.parts |= enumBodyPartHit;
									Explosion.hitEntities[entityAlive2.entityId] = damageRecord;
								}
							}
						}
					}
				}
			}
		}
		int num3 = 0;
		int num4 = 0;
		float num5 = (float)Explosion.hitEntities.Count * 0.8f;
		foreach (KeyValuePair<int, Explosion.DamageRecord> keyValuePair in Explosion.hitEntities)
		{
			EntityAlive entity = keyValuePair.Value.entity;
			if (entity != null)
			{
				bool flag2 = entity.IsDead();
				int health = entity.Health;
				bool flag3 = keyValuePair.Value.damage > (float)entity.GetMaxHealth() * 0.1f;
				EnumBodyPartHit mainPart = keyValuePair.Value.mainPart;
				EnumBodyPartHit enumBodyPartHit2 = keyValuePair.Value.parts;
				enumBodyPartHit2 &= ~mainPart;
				float num6 = flag2 ? 0.85f : 0.6f;
				for (int j = 0; j < 11; j++)
				{
					int num7 = 1 << j;
					if ((enumBodyPartHit2 & (EnumBodyPartHit)num7) != EnumBodyPartHit.None && entity.rand.RandomFloat <= num6)
					{
						enumBodyPartHit2 &= (EnumBodyPartHit)(~(EnumBodyPartHit)num7);
					}
				}
				if (entity is EntityEnemy && ((float)num3 >= 4f || flag2))
				{
					if ((entity.position - this.worldPos).sqrMagnitude < value2 * 0.67f)
					{
						entity.canDisintegrate = true;
					}
					else if ((float)num4 < num5)
					{
						entity.canDisintegrate = true;
					}
				}
				entity.DamageEntity(new DamageSourceEntity(EnumDamageSource.External, damageType, _entityThatCausedExplosion, keyValuePair.Value.dir, keyValuePair.Value.hitTransform.name, Vector3.zero, Vector2.zero)
				{
					bodyParts = (mainPart | enumBodyPartHit2),
					AttackingItem = _itemValueExplosionSource,
					DismemberChance = (flag3 ? 0.5f : 0f),
					BlockPosition = vector3i
				}, (int)keyValuePair.Value.damage, flag3, 1f);
				if (entity.isDisintegrated)
				{
					num4++;
				}
				if (entityAlive != null)
				{
					MinEventTypes eventType = (health != entity.Health) ? MinEventTypes.onSelfExplosionDamagedOther : MinEventTypes.onSelfExplosionAttackedOther;
					entityAlive.MinEventContext.Self = entityAlive;
					entityAlive.MinEventContext.Other = entity;
					entityAlive.MinEventContext.ItemValue = _itemValueExplosionSource;
					if (_itemValueExplosionSource != null)
					{
						_itemValueExplosionSource.FireEvent(eventType, entityAlive.MinEventContext);
					}
					entityAlive.FireEvent(eventType, false);
				}
				if (this.explosionData.BuffActions != null)
				{
					for (int k = 0; k < this.explosionData.BuffActions.Count; k++)
					{
						BuffClass buff = BuffManager.GetBuff(this.explosionData.BuffActions[k]);
						if (buff != null)
						{
							float num8 = 1f;
							num8 = EffectManager.GetValue(PassiveEffects.BuffProcChance, null, num8, entityAlive, null, FastTags<TagGroup.Global>.Parse(buff.Name), true, true, true, true, true, 1, true, false);
							if (entity.rand.RandomFloat <= num8)
							{
								entity.Buffs.AddBuff(this.explosionData.BuffActions[k], _entityThatCausedExplosion, true, false, -1f);
							}
						}
					}
				}
				if (!flag2 && entity.IsDead())
				{
					num3++;
					EntityPlayer entityPlayer = entity as EntityPlayer;
					if (entityPlayer != null)
					{
						TwitchManager.Current.CheckKiller(entityPlayer, entityAlive, vector3i);
					}
					if (entityAlive != null)
					{
						if (entityAlive.isEntityRemote)
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageMinEventFire>().Setup(entityAlive.entityId, entity.entityId, MinEventTypes.onSelfKilledOther, _itemValueExplosionSource), false, entityAlive.entityId, -1, -1, null, 192, false);
						}
						else
						{
							entityAlive.FireEvent(MinEventTypes.onSelfKilledOther, true);
						}
					}
				}
			}
		}
		if (!flag && entityAlive != null)
		{
			if (entityAlive.isEntityRemote)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageMinEventFire>().Setup(entityAlive.entityId, -1, MinEventTypes.onSelfExplosionMissEntity, _itemValueExplosionSource), false, entityAlive.entityId, -1, -1, null, 192, false);
			}
			else
			{
				entityAlive.MinEventContext.Self = entityAlive;
				entityAlive.MinEventContext.Other = null;
				entityAlive.MinEventContext.ItemValue = _itemValueExplosionSource;
				entityAlive.FireEvent(MinEventTypes.onSelfExplosionMissEntity, true);
			}
		}
		Explosion.hitEntities.Clear();
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x00118B94 File Offset: 0x00116D94
	[Conditional("DEBUG_EXPLOSION_LOG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void log(string format, params object[] args)
	{
		format = string.Format("{0} {1} Explosion {2}", GameManager.frameTime.ToCultureInvariantString(), GameManager.frameCount, format);
		Log.Warning(format, args);
	}

	// Token: 0x040021E9 RID: 8681
	public Dictionary<Vector3i, BlockChangeInfo> ChangedBlockPositions;

	// Token: 0x040021EA RID: 8682
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x040021EB RID: 8683
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 worldPos;

	// Token: 0x040021EC RID: 8684
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x040021ED RID: 8685
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x040021EE RID: 8686
	[PublicizedFrom(EAccessModifier.Private)]
	public ExplosionData explosionData;

	// Token: 0x040021EF RID: 8687
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<Vector3i> damagedBlockPositions;

	// Token: 0x040021F0 RID: 8688
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> explosionTag = FastTags<TagGroup.Global>.Parse("explosion");

	// Token: 0x040021F1 RID: 8689
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDisintegrationEpicenterPercent = 0.67f;

	// Token: 0x040021F2 RID: 8690
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cMinZombiesForDisintegration = 4f;

	// Token: 0x040021F3 RID: 8691
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cMinZombiesForDisintegrationPercent = 0.8f;

	// Token: 0x040021F4 RID: 8692
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cStepDistance = 0.51f;

	// Token: 0x040021F5 RID: 8693
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<int, Explosion.DamageRecord> hitEntities = new Dictionary<int, Explosion.DamageRecord>();

	// Token: 0x02000544 RID: 1348
	[PublicizedFrom(EAccessModifier.Private)]
	public struct DamageRecord
	{
		// Token: 0x040021F6 RID: 8694
		public EntityAlive entity;

		// Token: 0x040021F7 RID: 8695
		public float damage;

		// Token: 0x040021F8 RID: 8696
		public Vector3 dir;

		// Token: 0x040021F9 RID: 8697
		public Transform hitTransform;

		// Token: 0x040021FA RID: 8698
		public EnumBodyPartHit mainPart;

		// Token: 0x040021FB RID: 8699
		public EnumBodyPartHit parts;
	}
}
