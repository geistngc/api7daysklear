using System;

namespace Quests.Requirements
{
	// Token: 0x020018A7 RID: 6311
	public abstract class BaseRequirement
	{
		// Token: 0x170017E8 RID: 6120
		// (get) Token: 0x0600C2C9 RID: 49865 RVA: 0x004837BC File Offset: 0x004819BC
		// (set) Token: 0x0600C2CA RID: 49866 RVA: 0x004837C4 File Offset: 0x004819C4
		public string ID { get; set; }

		// Token: 0x170017E9 RID: 6121
		// (get) Token: 0x0600C2CB RID: 49867 RVA: 0x004837CD File Offset: 0x004819CD
		// (set) Token: 0x0600C2CC RID: 49868 RVA: 0x004837D5 File Offset: 0x004819D5
		public string Value { get; set; }

		// Token: 0x170017EA RID: 6122
		// (get) Token: 0x0600C2CD RID: 49869 RVA: 0x004837DE File Offset: 0x004819DE
		// (set) Token: 0x0600C2CE RID: 49870 RVA: 0x004837E6 File Offset: 0x004819E6
		public bool Complete { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170017EB RID: 6123
		// (get) Token: 0x0600C2CF RID: 49871 RVA: 0x004837EF File Offset: 0x004819EF
		// (set) Token: 0x0600C2D0 RID: 49872 RVA: 0x004837F7 File Offset: 0x004819F7
		public Quest OwnerQuest { get; set; }

		// Token: 0x170017EC RID: 6124
		// (get) Token: 0x0600C2D1 RID: 49873 RVA: 0x00483800 File Offset: 0x00481A00
		// (set) Token: 0x0600C2D2 RID: 49874 RVA: 0x00483808 File Offset: 0x00481A08
		public QuestClass Owner { get; set; }

		// Token: 0x170017ED RID: 6125
		// (get) Token: 0x0600C2D3 RID: 49875 RVA: 0x00483811 File Offset: 0x00481A11
		// (set) Token: 0x0600C2D4 RID: 49876 RVA: 0x00483819 File Offset: 0x00481A19
		public string Description { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170017EE RID: 6126
		// (get) Token: 0x0600C2D5 RID: 49877 RVA: 0x00483822 File Offset: 0x00481A22
		// (set) Token: 0x0600C2D6 RID: 49878 RVA: 0x0048382A File Offset: 0x00481A2A
		public string StatusText { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170017EF RID: 6127
		// (get) Token: 0x0600C2D7 RID: 49879 RVA: 0x00483833 File Offset: 0x00481A33
		// (set) Token: 0x0600C2D8 RID: 49880 RVA: 0x0048383B File Offset: 0x00481A3B
		public int Phase { get; set; }

		// Token: 0x0600C2D9 RID: 49881 RVA: 0x00483844 File Offset: 0x00481A44
		public virtual void HandleVariables()
		{
			this.ID = this.OwnerQuest.ParseVariable(this.ID);
			this.Value = this.OwnerQuest.ParseVariable(this.Value);
		}

		// Token: 0x0600C2DA RID: 49882 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void SetupRequirement()
		{
		}

		// Token: 0x0600C2DB RID: 49883 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool CheckRequirement()
		{
			return false;
		}

		// Token: 0x0600C2DC RID: 49884 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual BaseRequirement Clone()
		{
			return null;
		}

		// Token: 0x0600C2DD RID: 49885 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseRequirement()
		{
		}

		// Token: 0x0400941A RID: 37914
		public DynamicProperties Properties;

		// Token: 0x020018A8 RID: 6312
		public enum RequirementTypes
		{
			// Token: 0x0400941C RID: 37916
			Buff,
			// Token: 0x0400941D RID: 37917
			Holding,
			// Token: 0x0400941E RID: 37918
			Level,
			// Token: 0x0400941F RID: 37919
			Wearing
		}
	}
}
