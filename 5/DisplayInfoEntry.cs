using System;

// Token: 0x020010DB RID: 4315
public class DisplayInfoEntry
{
	// Token: 0x17000FF1 RID: 4081
	// (get) Token: 0x0600897A RID: 35194 RVA: 0x0034709B File Offset: 0x0034529B
	// (set) Token: 0x0600897B RID: 35195 RVA: 0x003470A3 File Offset: 0x003452A3
	public FastTags<TagGroup.Global> Tags
	{
		get
		{
			return this.tags;
		}
		set
		{
			this.tags = value;
			this.TagsSet = true;
		}
	}

	// Token: 0x0400662D RID: 26157
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tags = FastTags<TagGroup.Global>.none;

	// Token: 0x0400662E RID: 26158
	public bool TagsSet;

	// Token: 0x0400662F RID: 26159
	public PassiveEffects StatType;

	// Token: 0x04006630 RID: 26160
	public string CustomName = "";

	// Token: 0x04006631 RID: 26161
	public string TitleOverride;

	// Token: 0x04006632 RID: 26162
	public DisplayInfoEntry.DisplayTypes DisplayType;

	// Token: 0x04006633 RID: 26163
	public bool ShowInverted;

	// Token: 0x04006634 RID: 26164
	public bool NegativePreferred;

	// Token: 0x04006635 RID: 26165
	public bool DisplayLeadingPlus;

	// Token: 0x020010DC RID: 4316
	public enum DisplayTypes
	{
		// Token: 0x04006637 RID: 26167
		Integer,
		// Token: 0x04006638 RID: 26168
		Decimal1,
		// Token: 0x04006639 RID: 26169
		Decimal2,
		// Token: 0x0400663A RID: 26170
		Bool,
		// Token: 0x0400663B RID: 26171
		Percent,
		// Token: 0x0400663C RID: 26172
		Time
	}
}
