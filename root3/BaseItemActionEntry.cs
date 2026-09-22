using System;
using UnityEngine.Scripting;

// Token: 0x02000D50 RID: 3408
[Preserve]
public class BaseItemActionEntry
{
	// Token: 0x17000B08 RID: 2824
	// (get) Token: 0x060067D8 RID: 26584 RVA: 0x0028FACC File Offset: 0x0028DCCC
	// (set) Token: 0x060067D9 RID: 26585 RVA: 0x0028FAD4 File Offset: 0x0028DCD4
	public string ActionName { get; set; }

	// Token: 0x17000B09 RID: 2825
	// (get) Token: 0x060067DA RID: 26586 RVA: 0x0028FADD File Offset: 0x0028DCDD
	// (set) Token: 0x060067DB RID: 26587 RVA: 0x0028FAE5 File Offset: 0x0028DCE5
	public string IconName { get; set; }

	// Token: 0x17000B0A RID: 2826
	// (get) Token: 0x060067DC RID: 26588 RVA: 0x0028FAEE File Offset: 0x0028DCEE
	// (set) Token: 0x060067DD RID: 26589 RVA: 0x0028FAF6 File Offset: 0x0028DCF6
	public bool Enabled { get; set; }

	// Token: 0x17000B0B RID: 2827
	// (get) Token: 0x060067DE RID: 26590 RVA: 0x0028FAFF File Offset: 0x0028DCFF
	// (set) Token: 0x060067DF RID: 26591 RVA: 0x0028FB07 File Offset: 0x0028DD07
	public string SoundName { get; set; }

	// Token: 0x17000B0C RID: 2828
	// (get) Token: 0x060067E0 RID: 26592 RVA: 0x0028FB10 File Offset: 0x0028DD10
	// (set) Token: 0x060067E1 RID: 26593 RVA: 0x0028FB18 File Offset: 0x0028DD18
	public string DisabledSound { get; set; }

	// Token: 0x17000B0D RID: 2829
	// (get) Token: 0x060067E2 RID: 26594 RVA: 0x0028FB21 File Offset: 0x0028DD21
	// (set) Token: 0x060067E3 RID: 26595 RVA: 0x0028FB29 File Offset: 0x0028DD29
	public XUiController ItemController { get; set; }

	// Token: 0x17000B0E RID: 2830
	// (get) Token: 0x060067E4 RID: 26596 RVA: 0x0028FB32 File Offset: 0x0028DD32
	// (set) Token: 0x060067E5 RID: 26597 RVA: 0x0028FB3A File Offset: 0x0028DD3A
	public XUiC_ItemActionEntry ParentItem { get; set; }

	// Token: 0x17000B0F RID: 2831
	// (get) Token: 0x060067E6 RID: 26598 RVA: 0x0028FB43 File Offset: 0x0028DD43
	// (set) Token: 0x060067E7 RID: 26599 RVA: 0x0028FB4B File Offset: 0x0028DD4B
	public XUiC_ItemActionList ParentActionList { get; set; }

	// Token: 0x17000B10 RID: 2832
	// (get) Token: 0x060067E8 RID: 26600 RVA: 0x0028FB54 File Offset: 0x0028DD54
	// (set) Token: 0x060067E9 RID: 26601 RVA: 0x0028FB5C File Offset: 0x0028DD5C
	public BaseItemActionEntry.GamepadShortCut ShortCut { get; set; }

	// Token: 0x060067EA RID: 26602 RVA: 0x0028FB68 File Offset: 0x0028DD68
	public BaseItemActionEntry(XUiController itemController, string actionName, string spriteName, BaseItemActionEntry.GamepadShortCut shortcut = BaseItemActionEntry.GamepadShortCut.None, string soundName = "crafting/craft_click_craft", string disabledSoundName = "ui/ui_denied")
	{
		this.ItemController = itemController;
		this.ActionName = Localization.Get(actionName, false, null);
		this.IconName = spriteName;
		this.SoundName = soundName;
		this.DisabledSound = disabledSoundName;
		this.Enabled = true;
		this.ShortCut = shortcut;
	}

	// Token: 0x060067EB RID: 26603 RVA: 0x0028FBB6 File Offset: 0x0028DDB6
	public virtual void RefreshEnabled()
	{
		if (this.ItemController is XUiC_ItemStack)
		{
			this.Enabled = !((XUiC_ItemStack)this.ItemController).StackLock;
		}
	}

	// Token: 0x060067EC RID: 26604 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnActivated()
	{
	}

	// Token: 0x060067ED RID: 26605 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnDisabledActivate()
	{
	}

	// Token: 0x060067EE RID: 26606 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnTimerCompleted()
	{
	}

	// Token: 0x060067EF RID: 26607 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void DisableEvents()
	{
	}

	// Token: 0x02000D51 RID: 3409
	public enum GamepadShortCut
	{
		// Token: 0x04004FDE RID: 20446
		DPadUp,
		// Token: 0x04004FDF RID: 20447
		DPadLeft,
		// Token: 0x04004FE0 RID: 20448
		DPadRight,
		// Token: 0x04004FE1 RID: 20449
		DPadDown,
		// Token: 0x04004FE2 RID: 20450
		None,
		// Token: 0x04004FE3 RID: 20451
		Max
	}
}
