using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001904 RID: 6404
	[Preserve]
	public class ChallengeObjectiveEnterBiome : BaseChallengeObjective
	{
		// Token: 0x17001867 RID: 6247
		// (get) Token: 0x0600C59A RID: 50586 RVA: 0x00081B10 File Offset: 0x0007FD10
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.EnterBiome;
			}
		}

		// Token: 0x17001868 RID: 6248
		// (get) Token: 0x0600C59B RID: 50587 RVA: 0x0048E5FD File Offset: 0x0048C7FD
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveEnter", false, null) + " " + Localization.Get("biome_" + this.biome, false, null) + ":";
			}
		}

		// Token: 0x0600C59C RID: 50588 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C59D RID: 50589 RVA: 0x0048E631 File Offset: 0x0048C831
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.BiomeEnter += this.Current_BiomeEnter;
		}

		// Token: 0x0600C59E RID: 50590 RVA: 0x0048E649 File Offset: 0x0048C849
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.BiomeEnter -= this.Current_BiomeEnter;
		}

		// Token: 0x0600C59F RID: 50591 RVA: 0x0048E664 File Offset: 0x0048C864
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BiomeEnter(BiomeDefinition biomeDef)
		{
			if (biomeDef != null && biomeDef.m_sBiomeName == this.biome)
			{
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600C5A0 RID: 50592 RVA: 0x0048E6B9 File Offset: 0x0048C8B9
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("biome"))
			{
				this.biome = e.GetAttribute("biome");
			}
		}

		// Token: 0x0600C5A1 RID: 50593 RVA: 0x0048E6EA File Offset: 0x0048C8EA
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveEnterBiome
			{
				biome = this.biome
			};
		}

		// Token: 0x04009597 RID: 38295
		[PublicizedFrom(EAccessModifier.Private)]
		public string biome;
	}
}
