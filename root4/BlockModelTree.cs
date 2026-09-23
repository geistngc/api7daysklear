using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000130 RID: 304
[Preserve]
public class BlockModelTree : BlockPlantGrowing
{
	// Token: 0x0600082D RID: 2093 RVA: 0x0003A63C File Offset: 0x0003883C
	public BlockModelTree()
	{
		this.fertileLevel = 1;
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0003A64C File Offset: 0x0003884C
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("FallOver"))
		{
			this.bFallOver = StringParsers.ParseBool(base.Properties.Values["FallOver"], 0, -1, true);
		}
		else
		{
			this.bFallOver = true;
		}
		this.IsTerrainDecoration = true;
		this.CanDecorateOnSlopes = true;
		this.CanPlayersSpawnOn = false;
		this.CanMobsSpawnOn = false;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0003A6BE File Offset: 0x000388BE
	public override bool UpdateTick(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		return !_blockValue.ischild && base.UpdateTick(_world, _blockPos, _blockValue, _bRandomTick, _ticksIfLoaded, _rnd);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0003A6DA File Offset: 0x000388DA
	public override void OnNeighborBlockChange(WorldBase _world, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		if (!_myBlockValue.ischild)
		{
			base.OnNeighborBlockChange(_world, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
		}
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0003A6F4 File Offset: 0x000388F4
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		if (!_newBlockValue.ischild)
		{
			base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
		}
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0003A70C File Offset: 0x0003890C
	public override bool CheckPlantAlive(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		return this.isPlantGrowingRandom || _world.IsRemote() || base.CheckPlantAlive(_world, _blockPos, _blockValue);
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0003A72C File Offset: 0x0003892C
	public override bool CanPlaceBlockAt(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (!base.CanPlaceBlockAt(_world, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		for (int i = _blockPos.x - 3; i <= _blockPos.x + 3; i++)
		{
			for (int j = _blockPos.z - 3; j <= _blockPos.z + 3; j++)
			{
				for (int k = _blockPos.y - 6; k <= _blockPos.y + 6; k++)
				{
					if (_world.GetBlock(new Vector3i(i, k, j)).Block is BlockModelTree)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0003A7B6 File Offset: 0x000389B6
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		this.removeAllTrunks(_world, _chunk, _blockPos, _blockValue, -1, false);
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0003A7D0 File Offset: 0x000389D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeAllTrunks(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, int _entityid, bool _bDropItemsAndStartParticle)
	{
		if (_chunk == null)
		{
			_chunk = (Chunk)_world.GetChunkFromWorldPos(_blockPos);
			if (_chunk == null)
			{
				return;
			}
		}
		if (_bDropItemsAndStartParticle)
		{
			this.dropItems(_world, _blockPos, _blockValue, _entityid);
			float lightBrightness = _world.GetLightBrightness(_blockPos);
			this.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, lightBrightness, base.GetColorForSide(_blockValue, BlockFace.Top), _entityid);
		}
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0003A828 File Offset: 0x00038A28
	[PublicizedFrom(EAccessModifier.Private)]
	public void dropItems(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, int _entityId)
	{
		_blockValue.Block.DropItemsOnEvent(_world, _blockValue, EnumDropEvent.Destroy, 1f, World.blockToTransformPos(_blockPos), new Vector3(0.5f, 0f, 0.5f), 0f, _entityId, true);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0003A86B File Offset: 0x00038A6B
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _playerThatStartedExpl)
	{
		this.startToFall(_world, _bvRef, _blockValue, -1);
		return Block.DestroyedResult.Keep;
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0003A880 File Offset: 0x00038A80
	public override void OnBlockStartsToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		if (this.OnBlockDestroyedBy(_world, _blockPos, _blockValue, -1, false) != Block.DestroyedResult.Keep)
		{
			base.OnBlockStartsToFall(_world, _blockPos, _blockValue);
			return;
		}
		float lightBrightness = _world.GetLightBrightness(_blockPos);
		this.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, lightBrightness, base.GetColorForSide(_blockValue, BlockFace.Top), -1);
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0003A8D4 File Offset: 0x00038AD4
	public override void SpawnDestroyParticleEffect(WorldBase _world, BlockValue _blockValue, BlockValueRef _bvRef, float _lightValue, Color _color, int _entityIdThatCaused)
	{
		base.SpawnDestroyParticleEffect(_world, _blockValue, _bvRef, _lightValue, _color, _entityIdThatCaused);
		_world.GetGameManager().PlaySoundAtPositionServer(_bvRef.ToVector3(_world), "trunkbreak", AudioRolloffMode.Logarithmic, 100, -1, 1f);
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool ShowModelOnFall()
	{
		return false;
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0003A906 File Offset: 0x00038B06
	public override BlockValue OnBlockPlaced(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.rotation = BiomeBlockDecoration.GetRandomRotation(_rnd.RandomFloat, 7);
		return _blockValue;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0003A91D File Offset: 0x00038B1D
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		if (!this.bFallOver)
		{
			return Block.DestroyedResult.Downgrade;
		}
		if (!this.startToFall(_world, _bvRef, _blockValue, _entityId))
		{
			return Block.DestroyedResult.Downgrade;
		}
		return Block.DestroyedResult.Keep;
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0003A940 File Offset: 0x00038B40
	[PublicizedFrom(EAccessModifier.Private)]
	public bool startToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, int _entityId)
	{
		Transform transform;
		if (!DecoManager.Instance.IsEnabled || !_blockValue.Block.IsDistantDecoration)
		{
			BlockEntityData blockEntity = ((Chunk)_world.GetChunkFromWorldPos(_blockPos)).GetBlockEntity(_blockPos);
			if (blockEntity == null || !blockEntity.bHasTransform)
			{
				return false;
			}
			transform = blockEntity.transform;
		}
		else
		{
			transform = DecoManager.Instance.GetDecorationTransform(_blockPos, false);
		}
		if (!transform)
		{
			_world.SetBlockRPC(_blockPos, BlockValue.Air);
			return false;
		}
		_blockValue.damage = _blockValue.Block.MaxDamage;
		_world.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_blockPos, _blockValue, false, true)
		});
		Entity entity = _world.GetEntity(_entityId);
		Vector3 vector = transform.position + Origin.position;
		EntityCreationData entityCreationData = new EntityCreationData();
		entityCreationData.entityClass = "fallingTree".GetHashCode();
		entityCreationData.blockPos = _blockPos;
		entityCreationData.fallTreeDir = (entity ? (vector - entity.GetPosition()) : _world.GetGameRandom().RandomOnUnitCircleXZ);
		entityCreationData.fallTreeDir.y = 0f;
		entityCreationData.fallTreeDir = entityCreationData.fallTreeDir.normalized;
		entityCreationData.pos = vector;
		entityCreationData.rot = transform.rotation.eulerAngles;
		entityCreationData.id = -1;
		_world.GetGameManager().RequestToSpawnEntityServer(entityCreationData);
		return true;
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0003AAA0 File Offset: 0x00038CA0
	public override int OnBlockDamaged(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (!_world.IsRemote() && _blockValue.damage > _blockValue.Block.MaxDamage + 100)
		{
			_world.SetBlockRPC(_bvRef, BlockValue.Air);
		}
		return base.OnBlockDamaged(_world, _bvRef, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x0400094C RID: 2380
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFallOver;
}
