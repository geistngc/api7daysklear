using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public interface IBlockTool
{
	// Token: 0x06000DE8 RID: 3560
	void CheckKeys(ItemInventoryData _data, WorldRayHitInfo _hitInfo, PlayerActionsLocal playerActions);

	// Token: 0x06000DE9 RID: 3561
	void CheckSpecialKeys(Event _event, PlayerActionsLocal playerActions);

	// Token: 0x06000DEA RID: 3562
	bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput);

	// Token: 0x06000DEB RID: 3563
	bool ExecuteAttackAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions);

	// Token: 0x06000DEC RID: 3564
	bool ExecuteUseAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions);

	// Token: 0x06000DED RID: 3565
	string GetDebugOutput();
}
