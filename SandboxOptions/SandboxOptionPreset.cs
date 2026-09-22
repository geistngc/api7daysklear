using System;
using System.Collections.Generic;
using System.Text;

namespace SandboxOptions
{
	// Token: 0x020018A0 RID: 6304
	public class SandboxOptionPreset
	{
		// Token: 0x170017E5 RID: 6117
		// (get) Token: 0x0600C295 RID: 49813 RVA: 0x004830A5 File Offset: 0x004812A5
		public string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(this.LocalizedName))
				{
					return this.Name;
				}
				return Localization.Get(this.LocalizedName, false, null);
			}
		}

		// Token: 0x170017E6 RID: 6118
		// (get) Token: 0x0600C296 RID: 49814 RVA: 0x004830C8 File Offset: 0x004812C8
		public string DisplayDescription
		{
			get
			{
				if (string.IsNullOrEmpty(this.DescriptionKey))
				{
					return this.Description;
				}
				return Localization.Get(this.DescriptionKey, false, null);
			}
		}

		// Token: 0x170017E7 RID: 6119
		// (get) Token: 0x0600C297 RID: 49815 RVA: 0x004830EB File Offset: 0x004812EB
		public string SandboxCode
		{
			get
			{
				return this.saveOptionsToCode();
			}
		}

		// Token: 0x0600C298 RID: 49816 RVA: 0x004830F3 File Offset: 0x004812F3
		public bool ForceShowOption(SandboxOptions option)
		{
			return this.AlwaysShow != null && this.AlwaysShow.Contains(option);
		}

		// Token: 0x0600C299 RID: 49817 RVA: 0x0048310C File Offset: 0x0048130C
		[PublicizedFrom(EAccessModifier.Private)]
		public string saveOptionsToCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(SandboxOptionManager.currentVersion);
			foreach (SandboxOptions sandboxOptions in this.PresetValues.Keys)
			{
				stringBuilder.Append(string.Format("{0}{1}", SandboxOptionManager.IndexToAlpha2((int)sandboxOptions), SandboxOptionManager.IndexToAlpha(this.PresetValues[sandboxOptions])));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040093F7 RID: 37879
		public string Name;

		// Token: 0x040093F8 RID: 37880
		public string LocalizedName;

		// Token: 0x040093F9 RID: 37881
		public string Description = "";

		// Token: 0x040093FA RID: 37882
		public string DescriptionKey = "";

		// Token: 0x040093FB RID: 37883
		public string Icon = "";

		// Token: 0x040093FC RID: 37884
		public string Group = "";

		// Token: 0x040093FD RID: 37885
		public List<SandboxOptions> AlwaysShow;

		// Token: 0x040093FE RID: 37886
		public bool IsDefault;

		// Token: 0x040093FF RID: 37887
		public bool IsUserPreset;

		// Token: 0x04009400 RID: 37888
		public bool IsModded;

		// Token: 0x04009401 RID: 37889
		public bool IsCustomPreset;

		// Token: 0x04009402 RID: 37890
		public short DifficultyRating;

		// Token: 0x04009403 RID: 37891
		public readonly Dictionary<SandboxOptions, int> PresetValues = new Dictionary<SandboxOptions, int>();
	}
}
