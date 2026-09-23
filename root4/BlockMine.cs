using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200012F RID: 303
[Preserve]
public class BlockMine : Block
{
	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000823 RID: 2083 RVA: 0x0003A264 File Offset: 0x00038464
	public ExplosionData Explosion
	{
		get
		{
			return this.explosion;
		}
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0003A26C File Offset: 0x0003846C
	public override void Init()
	{
		base.Init();
		this.explosion = new ExplosionData(base.Properties, null);
		this.BaseEntityDamage = this.explosion.EntityDamage;
		if (base.Properties.Values.ContainsKey(BlockMine.PropTriggerDelay))
		{
			this.TriggerDelay = StringParsers.ParseFloat(base.Properties.Values[BlockMine.PropTriggerDelay], 0, -1, NumberStyles.Any);
		}
		if (base.Properties.Values.ContainsKey(BlockMine.PropTriggerSound))
		{
			this.TriggerSound = base.Properties.Values[BlockMine.PropTriggerSound];
		}
		base.Properties.ParseBool(BlockMine.PropNoImmunity, ref this.NoImmunity);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0003A328 File Offset: 0x00038528
	public override void OnEntityWalking(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue, Entity entity)
	{
		if (this.NoImmunity || EffectManager.GetValue(PassiveEffects.LandMineImmunity, null, 0f, entity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 0f)
		{
			if (entity as EntityPlayer != null)
			{
				if ((entity as EntityPlayer).IsSpectator)
				{
					return;
				}
				GameManager.Instance.PlaySoundAtPositionServer(new Vector3((float)_x, (float)_y, (float)_z), this.TriggerSound, AudioRolloffMode.Linear, 5, entity.entityId, 1f);
			}
			float num = this.TriggerDelay;
			if (entity as EntityAlive != null)
			{
				num = EffectManager.GetValue(PassiveEffects.LandMineTriggerDelay, null, this.TriggerDelay, entity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			this.explosion.EntityDamage = EffectManager.GetValue(PassiveEffects.TrapIncomingDamage, null, this.BaseEntityDamage, entity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			_world.GetWBT().AddScheduledBlockUpdate(new Vector3i(_x, _y, _z), this.blockID, (ulong)(num * 20f));
		}
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0003A450 File Offset: 0x00038650
	public override int OnBlockDamaged(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (_damagePoints >= 0)
		{
			float num = (float)Utils.FastClamp(_damagePoints, 1, _blockValue.Block.MaxDamage - 1);
			if (_world.GetGameRandom().RandomFloat <= num / (float)_blockValue.Block.MaxDamage)
			{
				this.explode(_world, _bvRef, _entityIdThatDamaged);
			}
		}
		else
		{
			base.OnBlockDamaged(_world, _bvRef, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
		}
		return _blockValue.damage;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0003A4BE File Offset: 0x000386BE
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _playerIdx)
	{
		if (_world.GetGameRandom().RandomFloat < 0.33f)
		{
			this.explode(_world, _bvRef, _playerIdx);
			return Block.DestroyedResult.Remove;
		}
		return Block.DestroyedResult.Keep;
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0003A4E0 File Offset: 0x000386E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(WorldBase _world, BlockValueRef _bvRef, int _entityId)
	{
		Vector3 worldPos = _bvRef.ToVector3Center(_world);
		_world.GetGameManager().ExplosionServer(worldPos, World.worldToBlockPos(worldPos), Quaternion.identity, this.explosion, -1, 0.1f, true, null);
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue blockDef, BlockFace face)
	{
		return false;
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0003A51B File Offset: 0x0003871B
	public override bool UpdateTick(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		this.explode(_world, _blockPos, -1);
		return true;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0003A52C File Offset: 0x0003872C
	public void TriggerMine(Entity _entity, WorldBase _world, Vector3i _blockPos, bool useTrigger)
	{
		if (useTrigger)
		{
			float num = this.TriggerDelay;
			EntityAlive entityAlive = _entity as EntityAlive;
			if (entityAlive != null)
			{
				GameManager.Instance.PlaySoundAtPositionServer(_blockPos, this.TriggerSound, AudioRolloffMode.Linear, 5, 1f);
				num = EffectManager.GetValue(PassiveEffects.LandMineTriggerDelay, null, this.TriggerDelay, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				this.explosion.EntityDamage = EffectManager.GetValue(PassiveEffects.TrapIncomingDamage, null, this.BaseEntityDamage, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			else
			{
				this.explosion.EntityDamage = EffectManager.GetValue(PassiveEffects.TrapIncomingDamage, null, this.BaseEntityDamage, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			_world.GetWBT().AddScheduledBlockUpdate(_blockPos, this.blockID, (ulong)(num * 20f));
			return;
		}
		this.explode(_world, _blockPos, -1);
	}

	// Token: 0x04000944 RID: 2372
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTriggerDelay = "TriggerDelay";

	// Token: 0x04000945 RID: 2373
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTriggerSound = "TriggerSound";

	// Token: 0x04000946 RID: 2374
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropNoImmunity = "NoImmunity";

	// Token: 0x04000947 RID: 2375
	[PublicizedFrom(EAccessModifier.Protected)]
	public ExplosionData explosion;

	// Token: 0x04000948 RID: 2376
	[PublicizedFrom(EAccessModifier.Private)]
	public float TriggerDelay = 0.6f;

	// Token: 0x04000949 RID: 2377
	[PublicizedFrom(EAccessModifier.Private)]
	public string TriggerSound = "landmine_trigger";

	// Token: 0x0400094A RID: 2378
	[PublicizedFrom(EAccessModifier.Private)]
	public float BaseEntityDamage;

	// Token: 0x0400094B RID: 2379
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NoImmunity;
}
