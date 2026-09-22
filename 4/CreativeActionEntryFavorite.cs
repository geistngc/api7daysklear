using System;
using UnityEngine.Scripting;

// Token: 0x02000D52 RID: 3410
[Preserve]
public class CreativeActionEntryFavorite : BaseItemActionEntry
{
	// Token: 0x060067F0 RID: 26608 RVA: 0x0028FBDE File Offset: 0x0028DDDE
	public CreativeActionEntryFavorite(XUiController _controller, int _stackID) : base(_controller, "lblContextActionFavorite", "server_favorite", BaseItemActionEntry.GamepadShortCut.DPadRight, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.stackID = (ushort)_stackID;
	}

	// Token: 0x060067F1 RID: 26609 RVA: 0x0028FC04 File Offset: 0x0028DE04
	public override void OnActivated()
	{
		EntityPlayer entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
		if (entityPlayer.favoriteCreativeStacks.Contains(this.stackID))
		{
			entityPlayer.favoriteCreativeStacks.Remove(this.stackID);
		}
		else
		{
			entityPlayer.favoriteCreativeStacks.Add(this.stackID);
		}
		XUiC_Creative2Window childByType = base.ItemController.WindowGroup.Controller.GetChildByType<XUiC_Creative2Window>();
		if (childByType == null)
		{
			return;
		}
		childByType.RefreshView();
	}

	// Token: 0x04004FE4 RID: 20452
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ushort stackID;
}
