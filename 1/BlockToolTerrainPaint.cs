using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class BlockToolTerrainPaint : IBlockTool
{
	// Token: 0x06000DC7 RID: 3527 RVA: 0x0005AF31 File Offset: 0x00059131
	public BlockToolTerrainPaint(BlockTools.Brush _brush, NguiWdwTerrainEditor _parentWindow)
	{
		this.brush = _brush;
		this.parentWindow = _parentWindow;
		this.isButtonDown = false;
		this.paintBlock = BlockValue.Air;
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x000027FC File Offset: 0x000009FC
	public void CheckSpecialKeys(Event ev, PlayerActionsLocal _playerAction)
	{
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0005AF64 File Offset: 0x00059164
	public void CheckKeys(ItemInventoryData _data, WorldRayHitInfo _hitInfo, PlayerActionsLocal _playerAction)
	{
		if (_data == null || _data.actionData == null || _data.actionData[0] == null)
		{
			return;
		}
		ItemStack item = GameManager.Instance.World.GetPrimaryPlayer().inventory.GetItem(7);
		this.paintBlock = item.itemValue.ToBlockValue(false);
		this.parentWindow.lastPosition = new Vector3i(_hitInfo.hit.pos);
		this.parentWindow.lastDirection = _hitInfo.ray.direction;
		if (!Input.GetMouseButton(0) || !this.isButtonDown)
		{
			if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && this.isButtonDown)
			{
				this.isButtonDown = false;
			}
			return;
		}
		if (Time.time - _data.actionData[0].lastUseTime < 0.1f)
		{
			return;
		}
		_data.actionData[0].lastUseTime = Time.time;
		if (!_hitInfo.bHitValid || !GameUtils.IsBlockOrTerrain(_hitInfo.tag))
		{
			return;
		}
		this.blockChanges.Clear();
		BlockTools.PaintTerrain(_data.world, _hitInfo.hit.blockPos, this.brush, this.blockChanges, this.paintBlock);
		_data.world.SetBlocksRPC(this.blockChanges);
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00010E62 File Offset: 0x0000F062
	public bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput)
	{
		return false;
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00032163 File Offset: 0x00030363
	public string GetDebugOutput()
	{
		return "";
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x0005B0AE File Offset: 0x000592AE
	public virtual bool ExecuteAttackAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal _playerAction)
	{
		this.isButtonDown = true;
		this.lastData = _data;
		return true;
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0005B0AE File Offset: 0x000592AE
	public bool ExecuteUseAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal _playerAction)
	{
		this.isButtonDown = true;
		this.lastData = _data;
		return true;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x0005B0C0 File Offset: 0x000592C0
	[PublicizedFrom(EAccessModifier.Private)]
	public sbyte[] GetLocalDensityMap(WorldBase _world, Vector3i blockTargetPos, Vector3i[] localArea)
	{
		sbyte[] array = new sbyte[localArea.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _world.GetDensity(localArea[i].x, localArea[i].y, localArea[i].z);
		}
		return array;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0005AF2A File Offset: 0x0005912A
	public override string ToString()
	{
		return "Dig/Place Terrain";
	}

	// Token: 0x04000BEB RID: 3051
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTools.Brush brush;

	// Token: 0x04000BEC RID: 3052
	[PublicizedFrom(EAccessModifier.Private)]
	public NguiWdwTerrainEditor parentWindow;

	// Token: 0x04000BED RID: 3053
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemInventoryData lastData;

	// Token: 0x04000BEE RID: 3054
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isButtonDown;

	// Token: 0x04000BEF RID: 3055
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockChangeInfo> blockChanges = new List<BlockChangeInfo>();

	// Token: 0x04000BF0 RID: 3056
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue paintBlock;
}
