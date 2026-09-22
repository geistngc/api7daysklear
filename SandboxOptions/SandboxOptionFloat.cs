using System;

namespace SandboxOptions
{
	// Token: 0x02001899 RID: 6297
	public class SandboxOptionFloat : BaseSandboxOption
	{
		// Token: 0x170017DD RID: 6109
		// (get) Token: 0x0600C22F RID: 49711 RVA: 0x0002F184 File Offset: 0x0002D384
		public override BaseSandboxOption.OptionTypes OptionType
		{
			get
			{
				return BaseSandboxOption.OptionTypes.Float;
			}
		}

		// Token: 0x0600C230 RID: 49712 RVA: 0x0047D980 File Offset: 0x0047BB80
		public SandboxOptionFloat(SandboxOptions option, string optionName, string categoryName, string valueSetName, float defaultValue, bool newUISection = false, SandboxOptionFloat.DisabledOptionsOnValue disabledOptions = null) : base(option, optionName, categoryName, valueSetName, newUISection)
		{
			this.DefaultValue = defaultValue;
			this.DisabledOptions = disabledOptions;
		}

		// Token: 0x0600C231 RID: 49713 RVA: 0x0047D9AA File Offset: 0x0047BBAA
		public override float GetFloatValue()
		{
			return this.CurrentValue;
		}

		// Token: 0x0600C232 RID: 49714 RVA: 0x0047D9B2 File Offset: 0x0047BBB2
		public override int GetIntValue()
		{
			return (int)(this.CurrentValue * 100f);
		}

		// Token: 0x0600C233 RID: 49715 RVA: 0x0047D9C1 File Offset: 0x0047BBC1
		public override object GetValue()
		{
			return this.GetFloatValue();
		}

		// Token: 0x170017DE RID: 6110
		// (get) Token: 0x0600C234 RID: 49716 RVA: 0x0047D9CE File Offset: 0x0047BBCE
		public override string ValueText
		{
			get
			{
				return this.CurrentValue.ToString();
			}
		}

		// Token: 0x0600C235 RID: 49717 RVA: 0x0047D9DC File Offset: 0x0047BBDC
		public override void SetValue(string value)
		{
			float num;
			if (StringParsers.TryParseFloat(value, out num) && base.ValueOptions.GetFloatIndex(num) != -1)
			{
				this.CurrentValue = num;
			}
		}

		// Token: 0x0600C236 RID: 49718 RVA: 0x0047DA09 File Offset: 0x0047BC09
		public override void SetFloat(float value)
		{
			this.CurrentValue = value;
		}

		// Token: 0x0600C237 RID: 49719 RVA: 0x0047DA12 File Offset: 0x0047BC12
		public override bool IsChanged()
		{
			return this.CurrentValue != this.DefaultValue;
		}

		// Token: 0x0600C238 RID: 49720 RVA: 0x0047DA25 File Offset: 0x0047BC25
		public override object GetDefaultValue()
		{
			return this.DefaultValue;
		}

		// Token: 0x0600C239 RID: 49721 RVA: 0x0047DA32 File Offset: 0x0047BC32
		public override float GetDefaultFloatValue()
		{
			return this.DefaultValue;
		}

		// Token: 0x0600C23A RID: 49722 RVA: 0x0047DA3A File Offset: 0x0047BC3A
		public override int GetDefaultIntValue()
		{
			return (int)(this.DefaultValue * 100f);
		}

		// Token: 0x0600C23B RID: 49723 RVA: 0x0047DA49 File Offset: 0x0047BC49
		public override int GetDefaultIndex()
		{
			return base.GetValueSet().GetFloatIndex(this.DefaultValue);
		}

		// Token: 0x0600C23C RID: 49724 RVA: 0x0047DA5C File Offset: 0x0047BC5C
		public override void SetToDefault()
		{
			this.CurrentValue = this.DefaultValue;
		}

		// Token: 0x0600C23D RID: 49725 RVA: 0x0047DA6A File Offset: 0x0047BC6A
		public override string GetValueText()
		{
			return string.Format("{0} | default:{1}", this.CurrentValue, this.DefaultValue);
		}

		// Token: 0x0600C23E RID: 49726 RVA: 0x0047DA8C File Offset: 0x0047BC8C
		public override int GetValueIndex()
		{
			return base.GetValueSet().GetFloatIndex(this.CurrentValue);
		}

		// Token: 0x0600C23F RID: 49727 RVA: 0x0047DAA0 File Offset: 0x0047BCA0
		public override int GetValueIndex(string value)
		{
			float val;
			if (StringParsers.TryParseFloat(value, out val))
			{
				return base.GetValueSet().GetFloatIndex(val);
			}
			return base.GetValueSet().GetFloatIndex(this.DefaultValue);
		}

		// Token: 0x0600C240 RID: 49728 RVA: 0x0047DAD5 File Offset: 0x0047BCD5
		public override void SetValueFromIndex(int index)
		{
			if (!base.GetValueSet().GetFloatValue(index, out this.CurrentValue))
			{
				this.CurrentValue = this.DefaultValue;
			}
		}

		// Token: 0x0600C241 RID: 49729 RVA: 0x0047DAF7 File Offset: 0x0047BCF7
		public override SandboxOptions[] GetAlwaysShowOptions()
		{
			if (this.DisabledOptions == null || !this.DisabledOptions.AlwaysShowValuesOnEnabled)
			{
				return null;
			}
			return this.DisabledOptions.DisabledOptions;
		}

		// Token: 0x0400932A RID: 37674
		public float DefaultValue;

		// Token: 0x0400932B RID: 37675
		public float CurrentValue = 1f;

		// Token: 0x0400932C RID: 37676
		public bool IsPercent;

		// Token: 0x0400932D RID: 37677
		public SandboxOptionFloat.DisabledOptionsOnValue DisabledOptions;

		// Token: 0x0200189A RID: 6298
		public class DisabledOptionsOnValue
		{
			// Token: 0x0600C242 RID: 49730 RVA: 0x0047DB1B File Offset: 0x0047BD1B
			public DisabledOptionsOnValue(SandboxOptions[] options, float value, bool inverted = false, bool alwaysShowValuesOnEnabled = false)
			{
				this.DisabledOptions = options;
				this.Value = value;
				this.Inverted = inverted;
				this.AlwaysShowValuesOnEnabled = alwaysShowValuesOnEnabled;
			}

			// Token: 0x0400932E RID: 37678
			public float Value;

			// Token: 0x0400932F RID: 37679
			public bool Inverted;

			// Token: 0x04009330 RID: 37680
			public SandboxOptions[] DisabledOptions;

			// Token: 0x04009331 RID: 37681
			public bool AlwaysShowValuesOnEnabled;
		}
	}
}
