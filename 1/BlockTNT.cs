using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200015A RID: 346
[Preserve]
public class BlockTNT : Block
{
	// Token: 0x0600098E RID: 2446 RVA: 0x00042063 File Offset: 0x00040263
	public override void Init()
	{
		base.Init();
		this.explosion = new ExplosionData(base.Properties, null);
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00042080 File Offset: 0x00040280
	public override int OnBlockDamaged(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _damagePoints, int _entityId, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (_world.GetGameRandom().RandomFloat <= (float)_damagePoints / (float)_blockValue.Block.MaxDamage)
		{
			this.explode(_world, _bvRef, _entityId, 0.1f);
		}
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache != null)
		{
			chunkCache.InvokeOnBlockDamagedDelegates(_bvRef, _blockValue, _damagePoints, _entityId);
		}
		return _blockValue.damage;
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x000420D6 File Offset: 0x000402D6
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, BlockValueRef _pos, BlockValue _blockValue, int _playerIdx)
	{
		base.OnBlockDestroyedByExplosion(_world, _pos, _blockValue, _playerIdx);
		this.explode(_world, _pos, _playerIdx, _world.GetGameRandom().RandomFloat * 0.5f + 0.3f);
		return Block.DestroyedResult.Remove;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00042108 File Offset: 0x00040308
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(WorldBase _world, BlockValueRef _bvRef, int _entityId, float _delay)
	{
		Vector3 worldPos = _bvRef.ToVector3Center(_world);
		_world.GetGameManager().ExplosionServer(worldPos, _bvRef, Quaternion.identity, this.explosion, _entityId, _delay, true, null);
	}

	// Token: 0x040009D5 RID: 2517
	[PublicizedFrom(EAccessModifier.Private)]
	public ExplosionData explosion;
}
