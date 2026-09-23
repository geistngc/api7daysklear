using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200014E RID: 334
[Preserve]
public class BlockSleepingBag : BlockSiblingRemove
{
	// Token: 0x06000937 RID: 2359 RVA: 0x000403C8 File Offset: 0x0003E5C8
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i rotationToAddVector(int _rotation)
	{
		switch (_rotation)
		{
		case 0:
			return new Vector3i(0, 0, 1);
		case 1:
			return new Vector3i(1, 0, 0);
		case 2:
			return new Vector3i(0, 0, -1);
		case 3:
			return new Vector3i(-1, 0, 0);
		default:
			return Vector3i.zero;
		}
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00040418 File Offset: 0x0003E618
	public override bool CanPlaceBlockAt(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (!base.CanPlaceBlockAt(_world, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		Vector3i vector3i = _blockPos + this.rotationToAddVector((int)_blockValue.rotation);
		Block block = _blockValue.Block;
		if (block.isMultiBlock)
		{
			vector3i = _blockPos + block.multiBlockPos.Get(0, _blockValue.type, (int)_blockValue.rotation);
		}
		BlockValue block2 = _world.GetBlock(vector3i);
		if (!block2.isair && !block2.Block.CanBlocksReplaceOrGroundCover())
		{
			return false;
		}
		BlockValue block3 = _world.GetBlock(_blockPos - Vector3i.up);
		BlockValue block4 = _world.GetBlock(vector3i - Vector3i.up);
		return !block3.isair && !block4.isair && block3.Block.blockMaterial.StabilitySupport && block4.Block.blockMaterial.StabilitySupport;
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x000404FC File Offset: 0x0003E6FC
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _bpResult, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _bpResult, _ea);
		EntityPlayerLocal entityPlayerLocal = _ea as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.BedrollPlaced, 1);
			}
			entityPlayerLocal.selectedSpawnPointKey = (long)_ea.entityId;
			if (!this.SiblingBlock.isair && !this.isMultiBlock)
			{
				BlockValue siblingBlock = this.SiblingBlock;
				siblingBlock.rotation = _bpResult.blockValue.rotation;
				_world.SetBlockRPC(_bpResult.blockPos + this.rotationToAddVector((int)_bpResult.blockValue.rotation), siblingBlock);
			}
		}
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00040598 File Offset: 0x0003E798
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs GetOwningPlayer(Vector3i _blockPos, out bool _ownedByOtherPlayer)
	{
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in GameManager.Instance.GetPersistentPlayerList().Players)
		{
			if (keyValuePair.Value.BedrollPos.Equals(_blockPos))
			{
				_ownedByOtherPlayer = !keyValuePair.Key.Equals(PlatformManager.InternalLocalUserIdentifier);
				return keyValuePair.Key;
			}
		}
		_ownedByOtherPlayer = false;
		return null;
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00040624 File Offset: 0x0003E824
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag;
		PlatformUserIdentifierAbs owningPlayer = this.GetOwningPlayer(_blockPos, out flag);
		if (!flag)
		{
			return base.GetActivationText(_world, _blockValue, _blockPos, _entityFocusing);
		}
		return string.Format(Localization.Get("sleepingBagOwnership", false, null), this.GetLocalizedBlockName(), GameUtils.SafeStringFormat(GameManager.Instance.GetPersistentPlayerList().GetPlayerData(owningPlayer).PlayerName.DisplayName));
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00040680 File Offset: 0x0003E880
	public override bool OnBlockActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		bool flag;
		this.GetOwningPlayer(_blockPos, out flag);
		return !flag && base.OnBlockActivated(_world, _blockPos, _blockValue, _player);
	}
}
