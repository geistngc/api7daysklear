using System;
using System.Collections.Generic;

// Token: 0x020002F0 RID: 752
public abstract class BaseDialogRequirement
{
	// Token: 0x17000266 RID: 614
	// (get) Token: 0x0600158B RID: 5515 RVA: 0x000813E4 File Offset: 0x0007F5E4
	// (set) Token: 0x0600158C RID: 5516 RVA: 0x000813EC File Offset: 0x0007F5EC
	public string ID { get; set; }

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x0600158D RID: 5517 RVA: 0x000813F5 File Offset: 0x0007F5F5
	// (set) Token: 0x0600158E RID: 5518 RVA: 0x000813FD File Offset: 0x0007F5FD
	public string Value { get; set; }

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x0600158F RID: 5519 RVA: 0x00081406 File Offset: 0x0007F606
	// (set) Token: 0x06001590 RID: 5520 RVA: 0x0008140E File Offset: 0x0007F60E
	public string Tag { get; set; }

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001591 RID: 5521 RVA: 0x00081417 File Offset: 0x0007F617
	// (set) Token: 0x06001592 RID: 5522 RVA: 0x0008141F File Offset: 0x0007F61F
	public Dialog Owner { get; set; }

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001593 RID: 5523 RVA: 0x00081428 File Offset: 0x0007F628
	// (set) Token: 0x06001594 RID: 5524 RVA: 0x00081430 File Offset: 0x0007F630
	public BaseDialogRequirement.RequirementVisibilityTypes RequirementVisibilityType { get; set; }

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001595 RID: 5525 RVA: 0x00081439 File Offset: 0x0007F639
	// (set) Token: 0x06001596 RID: 5526 RVA: 0x00081441 File Offset: 0x0007F641
	public string Description { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001597 RID: 5527 RVA: 0x0008144A File Offset: 0x0007F64A
	// (set) Token: 0x06001598 RID: 5528 RVA: 0x00081452 File Offset: 0x0007F652
	public string StatusText { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x06001599 RID: 5529 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual List<string> GetRequirementIDTypes()
	{
		return null;
	}

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x0600159A RID: 5530 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Buff;
		}
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x00032163 File Offset: 0x00030363
	public virtual string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetupRequirement()
	{
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return false;
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual BaseDialogRequirement Clone()
	{
		return null;
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public BaseDialogRequirement()
	{
	}

	// Token: 0x020002F1 RID: 753
	public enum RequirementTypes
	{
		// Token: 0x04000E85 RID: 3717
		Buff,
		// Token: 0x04000E86 RID: 3718
		QuestStatus,
		// Token: 0x04000E87 RID: 3719
		QuestsAvailable,
		// Token: 0x04000E88 RID: 3720
		QuestTier,
		// Token: 0x04000E89 RID: 3721
		QuestTierHighest,
		// Token: 0x04000E8A RID: 3722
		QuestEditorTag,
		// Token: 0x04000E8B RID: 3723
		Skill,
		// Token: 0x04000E8C RID: 3724
		Admin,
		// Token: 0x04000E8D RID: 3725
		DroneState,
		// Token: 0x04000E8E RID: 3726
		DroneStateExclude,
		// Token: 0x04000E8F RID: 3727
		CVar,
		// Token: 0x04000E90 RID: 3728
		CanTrade
	}

	// Token: 0x020002F2 RID: 754
	public enum RequirementVisibilityTypes
	{
		// Token: 0x04000E92 RID: 3730
		AlternateText,
		// Token: 0x04000E93 RID: 3731
		Hide
	}
}
