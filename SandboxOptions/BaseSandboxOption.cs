using System;

namespace SandboxOptions
{
	// Token: 0x02001895 RID: 6293
	public class BaseSandboxOption
	{
		// Token: 0x170017D6 RID: 6102
		// (get) Token: 0x0600C1FF RID: 49663 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual BaseSandboxOption.OptionTypes OptionType
		{
			get
			{
				return BaseSandboxOption.OptionTypes.Invalid;
			}
		}

		// Token: 0x170017D7 RID: 6103
		// (get) Token: 0x0600C200 RID: 49664 RVA: 0x0047D5B8 File Offset: 0x0047B7B8
		public string OptionNameText
		{
			get
			{
				if (this.optionNameText == null)
				{
					if (this.OverrideOptionName == null)
					{
						string key = "go" + this.Option.ToString();
						this.optionNameText = (Localization.Exists(key, false) ? Localization.Get(key, false, null) : ("*" + this.OptionName));
					}
					else
					{
						this.optionNameText = (Localization.Exists(this.OverrideOptionName, false) ? Localization.Get(this.OverrideOptionName, false, null) : "");
					}
				}
				return this.optionNameText;
			}
		}

		// Token: 0x170017D8 RID: 6104
		// (get) Token: 0x0600C201 RID: 49665 RVA: 0x0047D64C File Offset: 0x0047B84C
		public string DescriptionText
		{
			get
			{
				if (this.descriptionText == null)
				{
					if (this.OverrideDescriptionName == null)
					{
						string key = "go" + this.Option.ToString() + "Desc";
						this.descriptionText = (Localization.Exists(key, false) ? Localization.Get(key, false, null) : "");
					}
					else
					{
						this.descriptionText = (Localization.Exists(this.OverrideDescriptionName, false) ? Localization.Get(this.OverrideDescriptionName, false, null) : "");
					}
				}
				return this.descriptionText;
			}
		}

		// Token: 0x170017D9 RID: 6105
		// (get) Token: 0x0600C202 RID: 49666 RVA: 0x0047D6D8 File Offset: 0x0047B8D8
		// (set) Token: 0x0600C203 RID: 49667 RVA: 0x0047D6E0 File Offset: 0x0047B8E0
		public SandboxOptionValueSet ValueOptions { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170017DA RID: 6106
		// (get) Token: 0x0600C204 RID: 49668 RVA: 0x00032163 File Offset: 0x00030363
		public virtual string ValueText
		{
			get
			{
				return "";
			}
		}

		// Token: 0x0600C205 RID: 49669 RVA: 0x0047D6EC File Offset: 0x0047B8EC
		public BaseSandboxOption(SandboxOptions option, string name, string categoryName, string valueSetName, bool newUISection)
		{
			this.Option = option;
			this.OptionName = name;
			this.CategoryName = categoryName;
			this.ValueSetName = valueSetName;
			this.NewUISection = newUISection;
		}

		// Token: 0x0600C206 RID: 49670 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool GetBoolValue()
		{
			return false;
		}

		// Token: 0x0600C207 RID: 49671 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
		public virtual float GetFloatValue()
		{
			return 0f;
		}

		// Token: 0x0600C208 RID: 49672 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetIntValue()
		{
			return 0;
		}

		// Token: 0x0600C209 RID: 49673 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual object GetValue()
		{
			return null;
		}

		// Token: 0x0600C20A RID: 49674 RVA: 0x0047D74C File Offset: 0x0047B94C
		public SandboxOptionValueSet GetValueSet()
		{
			if (this.ValueOptions == null)
			{
				if (!SandboxOptionManager.Current.ValueSets.ContainsKey(this.ValueSetName))
				{
					throw new Exception("BaseSandboxOption: ValueOption " + this.ValueSetName + " does not exist.");
				}
				this.ValueOptions = SandboxOptionManager.Current.ValueSets[this.ValueSetName];
			}
			return this.ValueOptions;
		}

		// Token: 0x0600C20B RID: 49675 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void SetValueFromIndex(int index)
		{
		}

		// Token: 0x0600C20C RID: 49676 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void SetValue(string value)
		{
		}

		// Token: 0x0600C20D RID: 49677 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void SetFloat(float value)
		{
		}

		// Token: 0x0600C20E RID: 49678 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void SetInt(int value)
		{
		}

		// Token: 0x0600C20F RID: 49679 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void SetBool(bool value)
		{
		}

		// Token: 0x0600C210 RID: 49680 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool IsChanged()
		{
			return false;
		}

		// Token: 0x0600C211 RID: 49681 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual object GetDefaultValue()
		{
			return null;
		}

		// Token: 0x0600C212 RID: 49682 RVA: 0x0047D7B6 File Offset: 0x0047B9B6
		public virtual float GetDefaultFloatValue()
		{
			return -1f;
		}

		// Token: 0x0600C213 RID: 49683 RVA: 0x001008C9 File Offset: 0x000FEAC9
		public virtual int GetDefaultIntValue()
		{
			return -1;
		}

		// Token: 0x0600C214 RID: 49684 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool GetDefaultBoolValue()
		{
			return false;
		}

		// Token: 0x0600C215 RID: 49685 RVA: 0x001008C9 File Offset: 0x000FEAC9
		public virtual int GetDefaultIndex()
		{
			return -1;
		}

		// Token: 0x0600C216 RID: 49686 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void SetToDefault()
		{
		}

		// Token: 0x0600C217 RID: 49687 RVA: 0x0047D7BD File Offset: 0x0047B9BD
		public virtual string GetDefaultValueText(string languageName = null)
		{
			return this.GetValueSet().GetDisplayAtIndex(this.GetDefaultIndex(), languageName);
		}

		// Token: 0x0600C218 RID: 49688 RVA: 0x0047D7D1 File Offset: 0x0047B9D1
		public virtual string GetValueTextFromIndex(int index, string languageName = null)
		{
			return this.GetValueSet().GetDisplayAtIndex(index, languageName);
		}

		// Token: 0x0600C219 RID: 49689 RVA: 0x00032163 File Offset: 0x00030363
		public virtual string GetValueText()
		{
			return "";
		}

		// Token: 0x0600C21A RID: 49690 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetValueIndex()
		{
			return 0;
		}

		// Token: 0x0600C21B RID: 49691 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetValueIndex(string value)
		{
			return 0;
		}

		// Token: 0x0600C21C RID: 49692 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual SandboxOptions[] GetAlwaysShowOptions()
		{
			return null;
		}

		// Token: 0x04009311 RID: 37649
		public string OptionName;

		// Token: 0x04009312 RID: 37650
		[PublicizedFrom(EAccessModifier.Private)]
		public string optionNameText;

		// Token: 0x04009313 RID: 37651
		public string OverrideOptionName;

		// Token: 0x04009314 RID: 37652
		public string DisabledByText = "";

		// Token: 0x04009315 RID: 37653
		public bool IsEnabled = true;

		// Token: 0x04009316 RID: 37654
		[PublicizedFrom(EAccessModifier.Private)]
		public string descriptionText;

		// Token: 0x04009317 RID: 37655
		public string OverrideDescriptionName;

		// Token: 0x04009318 RID: 37656
		[PublicizedFrom(EAccessModifier.Private)]
		public string ValueSetName = "";

		// Token: 0x0400931A RID: 37658
		public SandboxOptions Option;

		// Token: 0x0400931B RID: 37659
		public string CategoryName = "";

		// Token: 0x0400931C RID: 37660
		public bool NewUISection;

		// Token: 0x02001896 RID: 6294
		public enum OptionTypes
		{
			// Token: 0x0400931E RID: 37662
			Invalid,
			// Token: 0x0400931F RID: 37663
			Int,
			// Token: 0x04009320 RID: 37664
			Float,
			// Token: 0x04009321 RID: 37665
			String,
			// Token: 0x04009322 RID: 37666
			Bool
		}
	}
}
