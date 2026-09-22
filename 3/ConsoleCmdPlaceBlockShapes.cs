using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200025F RID: 607
[Preserve]
public class ConsoleCmdPlaceBlockShapes : ConsoleCmdAbstract
{
	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x06001206 RID: 4614 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00070AE3 File Offset: 0x0006ECE3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Places all shapes of the currently held variant helper block";
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00070AEA File Offset: 0x0006ECEA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Places all variants of the helper block you currently hold in your hand. Starts\nat the current selection box and spreads out towards the right relative to the\ncurrent view direction of the player, starting a new row behind those every\n" + string.Format("{0} blocks. Spaces out each block by 1m meter.", 25);
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x00070B07 File Offset: 0x0006ED07
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"placeblockshapes",
			"pbs"
		};
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x00070B20 File Offset: 0x0006ED20
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on clients");
			return;
		}
		if (!BlockToolSelection.Instance.SelectionActive)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No selection active. Running this command requires an active 1x1x1 selection box.");
			return;
		}
		Vector3i selectionSize = BlockToolSelection.Instance.SelectionSize;
		Vector3i selectionStart = BlockToolSelection.Instance.SelectionStart;
		if (selectionSize != Vector3i.one)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Selection box size is not 1x1x1.");
			return;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		BlockValue blockValue = primaryPlayer.inventory.holdingItemItemValue.ToBlockValue(false);
		Block block = blockValue.Block;
		ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData = primaryPlayer.inventory.holdingItemData as ItemClassBlock.ItemBlockInventoryData;
		if (blockValue.isair || itemBlockInventoryData == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Player is not holding a block.");
			return;
		}
		if (block.AlternateBlockCount() == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Block does not have any shapes");
			return;
		}
		float y = primaryPlayer.rotation.y;
		Vector3i zero = Vector3i.zero;
		Vector3i zero2 = Vector3i.zero;
		switch (GameUtils.GetClosestDirection(y, true))
		{
		case GameUtils.DirEightWay.N:
			zero.x = 2;
			zero2.z = 2;
			goto IL_16E;
		case GameUtils.DirEightWay.E:
			zero.z = -2;
			zero2.x = 2;
			goto IL_16E;
		case GameUtils.DirEightWay.S:
			zero.x = -2;
			zero2.z = -2;
			goto IL_16E;
		case GameUtils.DirEightWay.W:
			zero.z = 2;
			zero2.x = -2;
			goto IL_16E;
		}
		throw new ArgumentOutOfRangeException();
		IL_16E:
		Block[] altBlocks = block.GetAltBlocks();
		for (int i = 0; i < altBlocks.Length; i++)
		{
			Block block2 = altBlocks[i];
			Vector3i placementPos = selectionStart + i % 25 * zero + i / 25 * zero2;
			ConsoleCmdPlaceBlockShapes.PlaceBlock(block2.ToBlockValue(), itemBlockInventoryData, placementPos, primaryPlayer);
		}
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x00070CF0 File Offset: 0x0006EEF0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PlaceBlock(BlockValue _blockValue, ItemClassBlock.ItemBlockInventoryData _holdingData, Vector3i _placementPos, EntityPlayerLocal _player)
	{
		BlockPlacement.Result result = new BlockPlacement.Result
		{
			blockValue = _blockValue,
			blockPos = _placementPos
		};
		Block block = _blockValue.Block;
		block.OnBlockPlaceBefore(GameManager.Instance.World, ref result, _player, GameManager.Instance.World.GetGameRandom());
		_blockValue = result.blockValue;
		if (_holdingData.itemValue.TextureFullArray.IsDefault || Block.list[_holdingData.itemValue.type].SelectAlternates)
		{
			block.PlaceBlock(GameManager.Instance.World, result, _player);
			return;
		}
		BlockChangeInfo item = new BlockChangeInfo(_placementPos, _blockValue)
		{
			textureFull = _holdingData.itemValue.TextureFullArray,
			bChangeTexture = true
		};
		GameManager.Instance.World.SetBlocksRPC(new List<BlockChangeInfo>
		{
			item
		});
	}

	// Token: 0x04000D09 RID: 3337
	[PublicizedFrom(EAccessModifier.Private)]
	public const int BlocksPerRow = 25;
}
