using System;
using UnityEngine.Scripting;

// Token: 0x020002E7 RID: 743
[Preserve]
public class DialogActionTrader : BaseDialogAction
{
	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06001560 RID: 5472 RVA: 0x00080864 File Offset: 0x0007EA64
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.Trader;
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x00080868 File Offset: 0x0007EA68
	public override void PerformAction(EntityPlayer player)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal);
		EntityNPC respondent = uiforPlayer.xui.Dialog.Respondent;
		if (respondent != null)
		{
			EntityTrader entityTrader = respondent as EntityTrader;
			if (entityTrader != null)
			{
				if (base.ID.EqualsCaseInsensitive("restock"))
				{
					entityTrader.TraderData.lastInventoryUpdate = 0UL;
				}
				else if (base.ID.EqualsCaseInsensitive("trade"))
				{
					entityTrader.SetNextTraderWindow(EntityTrader.TraderWindowState.Trade);
					uiforPlayer.windowManager.CloseAllOpenModalWindows(null, false);
				}
				else if (base.ID.EqualsCaseInsensitive("reset_quests"))
				{
					entityTrader.ClearActiveQuests(player.entityId);
				}
			}
			EntityDrone entityDrone = respondent as EntityDrone;
			if (entityDrone != null)
			{
				if (base.ID.EqualsCaseInsensitive("drone_storage"))
				{
					entityDrone.OpenStorageFromDialog(player);
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_command_follow") || base.ID.EqualsCaseInsensitive("drone_command_stay"))
				{
					entityDrone.ToggleOrderState();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_dont_heal_allies") || base.ID.EqualsCaseInsensitive("drone_heal_allies"))
				{
					entityDrone.ToggleHealAllies();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_attack_mode_passive") || base.ID.EqualsCaseInsensitive("drone_attack_mode_aggressive"))
				{
					entityDrone.ToggleAttackMode();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_light_on") || base.ID.EqualsCaseInsensitive("drone_light_off"))
				{
					entityDrone.ToggleLightAction();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_command_heal"))
				{
					entityDrone.HealRequest();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("trader_response_nevermind"))
				{
					entityDrone.ProcessDialog(base.ID);
				}
			}
		}
	}

	// Token: 0x04000E5C RID: 3676
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
