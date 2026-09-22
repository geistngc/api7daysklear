using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Challenges
{
	// Token: 0x020018F1 RID: 6385
	public class ChallengeCategory
	{
		// Token: 0x0600C4E1 RID: 50401 RVA: 0x0048B88F File Offset: 0x00489A8F
		public ChallengeCategory(string name)
		{
			this.Name = name;
		}

		// Token: 0x17001848 RID: 6216
		// (get) Token: 0x0600C4E2 RID: 50402 RVA: 0x0048B8B4 File Offset: 0x00489AB4
		public bool IsActive
		{
			get
			{
				if (this.showType == ChallengeCategory.ShowTypes.BiomeProgression)
				{
					return World.BiomeProgressionEnabled;
				}
				return ChallengeJournal.AllowChallenges;
			}
		}

		// Token: 0x0600C4E3 RID: 50403 RVA: 0x0048B8CC File Offset: 0x00489ACC
		public bool CanShow(EntityPlayer player)
		{
			if (this.showType == ChallengeCategory.ShowTypes.Twitch)
			{
				return player.TwitchEnabled;
			}
			if (this.showType == ChallengeCategory.ShowTypes.CVar)
			{
				return player.Buffs.GetCustomVar(this.showValue) > 0f;
			}
			if (this.showType == ChallengeCategory.ShowTypes.BiomeProgression)
			{
				return World.BiomeProgressionEnabled;
			}
			return ChallengeJournal.AllowChallenges;
		}

		// Token: 0x0600C4E4 RID: 50404 RVA: 0x0048B920 File Offset: 0x00489B20
		public void ParseElement(XElement e)
		{
			if (e.HasAttribute("title_key"))
			{
				this.Title = Localization.Get(e.GetAttribute("title_key"), false, null);
			}
			else if (e.HasAttribute("title"))
			{
				this.Title = e.GetAttribute("title");
			}
			else
			{
				this.Title = this.Name;
			}
			if (e.HasAttribute("icon"))
			{
				this.Icon = e.GetAttribute("icon");
			}
			if (e.HasAttribute("show_type"))
			{
				this.showType = (ChallengeCategory.ShowTypes)Enum.Parse(typeof(ChallengeCategory.ShowTypes), e.GetAttribute("show_type"), true);
			}
			if (e.HasAttribute("show_value"))
			{
				this.showValue = e.GetAttribute("show_value");
			}
			if (e.HasAttribute("display_key"))
			{
				this.DisplayKey = e.GetAttribute("display_key");
			}
		}

		// Token: 0x0400950D RID: 38157
		public static Dictionary<string, ChallengeCategory> s_ChallengeCategories = new CaseInsensitiveStringDictionary<ChallengeCategory>();

		// Token: 0x0400950E RID: 38158
		public string Name;

		// Token: 0x0400950F RID: 38159
		public string Icon;

		// Token: 0x04009510 RID: 38160
		public string Title;

		// Token: 0x04009511 RID: 38161
		public string DisplayKey = "";

		// Token: 0x04009512 RID: 38162
		[PublicizedFrom(EAccessModifier.Private)]
		public ChallengeCategory.ShowTypes showType;

		// Token: 0x04009513 RID: 38163
		[PublicizedFrom(EAccessModifier.Private)]
		public string showValue = "";

		// Token: 0x020018F2 RID: 6386
		public enum ShowTypes
		{
			// Token: 0x04009515 RID: 38165
			Normal,
			// Token: 0x04009516 RID: 38166
			CVar,
			// Token: 0x04009517 RID: 38167
			Twitch,
			// Token: 0x04009518 RID: 38168
			BiomeProgression
		}
	}
}
