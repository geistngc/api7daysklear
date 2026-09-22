using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x0200182C RID: 6188
	public class CooldownPreset
	{
		// Token: 0x0600BF25 RID: 48933 RVA: 0x0046BA90 File Offset: 0x00469C90
		public void AddCooldownMaxEntry(int start, int end, int cooldownMax, int cooldownTime)
		{
			if (this.CooldownMaxEntries == null)
			{
				this.CooldownMaxEntries = new List<TwitchCooldownEntry>();
			}
			this.CooldownMaxEntries.Add(new TwitchCooldownEntry
			{
				StartGameStage = start,
				EndGameStage = end,
				CooldownMax = cooldownMax,
				CooldownTime = cooldownTime
			});
		}

		// Token: 0x0600BF26 RID: 48934 RVA: 0x0046BAE0 File Offset: 0x00469CE0
		public void SetupCooldownInfo(int gameStage, EntityPlayerLocal localPlayer)
		{
			if (localPlayer == null)
			{
				return;
			}
			for (int i = 0; i < this.CooldownMaxEntries.Count; i++)
			{
				if (gameStage >= this.CooldownMaxEntries[i].StartGameStage && (gameStage <= this.CooldownMaxEntries[i].EndGameStage || this.CooldownMaxEntries[i].EndGameStage == -1))
				{
					float num = 1f;
					if (localPlayer.Party != null)
					{
						int num2 = 0;
						for (int j = 0; j < localPlayer.Party.MemberList.Count; j++)
						{
							if (localPlayer.Party.MemberList[j].TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Disabled)
							{
								num2++;
							}
						}
						num += (float)(num2 - 1) * 0.5f;
					}
					this.CooldownFillMax = (float)this.CooldownMaxEntries[i].CooldownMax * num;
					this.NextCooldownTime = this.CooldownMaxEntries[i].CooldownTime;
					return;
				}
			}
			this.CooldownFillMax = 100f;
			this.NextCooldownTime = 180;
		}

		// Token: 0x0600BF27 RID: 48935 RVA: 0x0046BBF4 File Offset: 0x00469DF4
		public virtual void ParseProperties(DynamicProperties properties)
		{
			if (properties.Values.ContainsKey(CooldownPreset.PropName))
			{
				this.Name = properties.Values[CooldownPreset.PropName];
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropTitle))
			{
				this.Title = properties.Values[CooldownPreset.PropTitle];
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropTitleKey))
			{
				this.Title = Localization.Get(properties.Values[CooldownPreset.PropTitleKey], false, null);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropCooldownType))
			{
				this.CooldownType = (CooldownPreset.CooldownTypes)Enum.Parse(typeof(CooldownPreset.CooldownTypes), properties.Values[CooldownPreset.PropCooldownType], true);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropIsDefault))
			{
				this.IsDefault = StringParsers.ParseBool(properties.Values[CooldownPreset.PropIsDefault], 0, -1, true);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropStartCooldown))
			{
				this.StartCooldownTime = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropStartCooldown], 0, -1, NumberStyles.Integer);
			}
			else
			{
				this.StartCooldownTime = 300;
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropDeathCooldown))
			{
				this.AfterDeathCooldownTime = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropDeathCooldown], 0, -1, NumberStyles.Integer);
			}
			else
			{
				this.AfterDeathCooldownTime = 180;
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropBMStartOffset))
			{
				this.BMStartOffset = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropBMStartOffset], 0, -1, NumberStyles.Integer);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropBMEndOffset))
			{
				this.BMEndOffset = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropBMEndOffset], 0, -1, NumberStyles.Integer);
			}
		}

		// Token: 0x04008FB6 RID: 36790
		public static string PropName = "name";

		// Token: 0x04008FB7 RID: 36791
		public static string PropTitle = "title";

		// Token: 0x04008FB8 RID: 36792
		public static string PropTitleKey = "title_key";

		// Token: 0x04008FB9 RID: 36793
		public static string PropCooldownType = "cooldown_type";

		// Token: 0x04008FBA RID: 36794
		public static string PropIsDefault = "is_default";

		// Token: 0x04008FBB RID: 36795
		public static string PropStartCooldown = "start_cooldown";

		// Token: 0x04008FBC RID: 36796
		public static string PropDeathCooldown = "death_cooldown";

		// Token: 0x04008FBD RID: 36797
		public static string PropBMStartOffset = "bm_start_offset";

		// Token: 0x04008FBE RID: 36798
		public static string PropBMEndOffset = "bm_end_offset";

		// Token: 0x04008FBF RID: 36799
		public string Name;

		// Token: 0x04008FC0 RID: 36800
		public bool IsDefault;

		// Token: 0x04008FC1 RID: 36801
		public string Title;

		// Token: 0x04008FC2 RID: 36802
		public CooldownPreset.CooldownTypes CooldownType = CooldownPreset.CooldownTypes.Fill;

		// Token: 0x04008FC3 RID: 36803
		public float CooldownFillMax;

		// Token: 0x04008FC4 RID: 36804
		public int NextCooldownTime;

		// Token: 0x04008FC5 RID: 36805
		public int StartCooldownTime;

		// Token: 0x04008FC6 RID: 36806
		public int AfterDeathCooldownTime;

		// Token: 0x04008FC7 RID: 36807
		public int BMStartOffset;

		// Token: 0x04008FC8 RID: 36808
		public int BMEndOffset;

		// Token: 0x04008FC9 RID: 36809
		public List<TwitchCooldownEntry> CooldownMaxEntries = new List<TwitchCooldownEntry>();

		// Token: 0x0200182D RID: 6189
		public enum CooldownTypes
		{
			// Token: 0x04008FCB RID: 36811
			Always,
			// Token: 0x04008FCC RID: 36812
			Fill,
			// Token: 0x04008FCD RID: 36813
			None
		}
	}
}
