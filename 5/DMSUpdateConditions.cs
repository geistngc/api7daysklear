using System;

// Token: 0x020003C4 RID: 964
public struct DMSUpdateConditions
{
	// Token: 0x1700037B RID: 891
	// (set) Token: 0x06001D01 RID: 7425 RVA: 0x000AEE63 File Offset: 0x000AD063
	public bool DoesPlayerExist
	{
		set
		{
			this.SetBoolHolder(value, 128);
		}
	}

	// Token: 0x1700037C RID: 892
	// (set) Token: 0x06001D02 RID: 7426 RVA: 0x000AEE71 File Offset: 0x000AD071
	public bool IsGameUnPaused
	{
		set
		{
			this.SetBoolHolder(value, 64);
		}
	}

	// Token: 0x1700037D RID: 893
	// (set) Token: 0x06001D03 RID: 7427 RVA: 0x000AEE7C File Offset: 0x000AD07C
	public bool IsDMSInitialized
	{
		set
		{
			this.SetBoolHolder(value, 32);
		}
	}

	// Token: 0x1700037E RID: 894
	// (set) Token: 0x06001D04 RID: 7428 RVA: 0x000AEE87 File Offset: 0x000AD087
	public bool IsDMSEnabled
	{
		set
		{
			this.SetBoolHolder(value, 16);
		}
	}

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06001D05 RID: 7429 RVA: 0x000AEE92 File Offset: 0x000AD092
	public bool CanUpdate
	{
		get
		{
			return this.BoolHolder == 240;
		}
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x000AEEA1 File Offset: 0x000AD0A1
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetBoolHolder(bool _value, byte _place)
	{
		if (_value)
		{
			this.BoolHolder |= _place;
			return;
		}
		this.BoolHolder &= ~_place;
	}

	// Token: 0x040012EB RID: 4843
	[PublicizedFrom(EAccessModifier.Private)]
	public byte BoolHolder;
}
