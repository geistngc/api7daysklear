using System;

// Token: 0x020002E0 RID: 736
public abstract class BaseDialogAction
{
	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06001544 RID: 5444 RVA: 0x00080413 File Offset: 0x0007E613
	// (set) Token: 0x06001545 RID: 5445 RVA: 0x0008041B File Offset: 0x0007E61B
	public string ID { get; set; }

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06001546 RID: 5446 RVA: 0x00080424 File Offset: 0x0007E624
	// (set) Token: 0x06001547 RID: 5447 RVA: 0x0008042C File Offset: 0x0007E62C
	public string Value { get; set; }

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06001548 RID: 5448 RVA: 0x00080435 File Offset: 0x0007E635
	// (set) Token: 0x06001549 RID: 5449 RVA: 0x0008043D File Offset: 0x0007E63D
	public Dialog OwnerDialog { get; set; }

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x0600154A RID: 5450 RVA: 0x00080446 File Offset: 0x0007E646
	// (set) Token: 0x0600154B RID: 5451 RVA: 0x0008044E File Offset: 0x0007E64E
	public DialogResponse Owner { get; set; }

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x0600154C RID: 5452 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.AddBuff;
		}
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x00080457 File Offset: 0x0007E657
	public BaseDialogAction()
	{
		this.ID = "";
		this.Value = "";
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00080475 File Offset: 0x0007E675
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CopyValues(BaseDialogAction action)
	{
		action.ID = this.ID;
		action.Value = this.Value;
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetupAction()
	{
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void PerformAction(EntityPlayer player)
	{
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual BaseDialogAction Clone()
	{
		return null;
	}

	// Token: 0x020002E1 RID: 737
	public enum ActionTypes
	{
		// Token: 0x04000E4F RID: 3663
		AddBuff,
		// Token: 0x04000E50 RID: 3664
		AddItem,
		// Token: 0x04000E51 RID: 3665
		AddQuest,
		// Token: 0x04000E52 RID: 3666
		CompleteQuest,
		// Token: 0x04000E53 RID: 3667
		Trader,
		// Token: 0x04000E54 RID: 3668
		Voice
	}
}
