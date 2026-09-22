using System;

namespace SandboxOptions
{
	// Token: 0x0200189B RID: 6299
	public class SandboxOptionInt : BaseSandboxOption
	{
		// Token: 0x170017DF RID: 6111
		// (get) Token: 0x0600C243 RID: 49731 RVA: 0x0002003D File Offset: 0x0001E23D
		public override BaseSandboxOption.OptionTypes OptionType
		{
			get
			{
				return BaseSandboxOption.OptionTypes.Int;
			}
		}

		// Token: 0x0600C244 RID: 49732 RVA: 0x0047DB40 File Offset: 0x0047BD40
		public SandboxOptionInt(SandboxOptions option, string optionName, string categoryName, string valueSetName, int defaultValue, bool newUISection = false, SandboxOptionInt.DisabledOptionsOnValue disabledOptions = null) : base(option, optionName, categoryName, valueSetName, newUISection)
		{
			this.DefaultValue = defaultValue;
			this.DisabledOptions = disabledOptions;
		}

		// Token: 0x0600C245 RID: 49733 RVA: 0x0047DB66 File Offset: 0x0047BD66
		public override int GetIntValue()
		{
			return this.CurrentValue;
		}

		// Token: 0x170017E0 RID: 6112
		// (get) Token: 0x0600C246 RID: 49734 RVA: 0x0047DB6E File Offset: 0x0047BD6E
		public override string ValueText
		{
			get
			{
				return this.CurrentValue.ToString();
			}
		}

		// Token: 0x0600C247 RID: 49735 RVA: 0x0047DB7B File Offset: 0x0047BD7B
		public override float GetFloatValue()
		{
			return (float)this.CurrentValue / 100f;
		}

		// Token: 0x0600C248 RID: 49736 RVA: 0x0047DB8A File Offset: 0x0047BD8A
		public override object GetValue()
		{
			return this.GetIntValue();
		}

		// Token: 0x0600C249 RID: 49737 RVA: 0x0047DB98 File Offset: 0x0047BD98
		public override void SetValue(string value)
		{
			int num;
			if (StringParsers.TryParseSInt32(value, out num) && base.ValueOptions.GetIntIndex(num) != -1)
			{
				this.CurrentValue = num;
			}
		}

		// Token: 0x0600C24A RID: 49738 RVA: 0x0047DBC5 File Offset: 0x0047BDC5
		public override void SetInt(int value)
		{
			this.CurrentValue = value;
		}

		// Token: 0x0600C24B RID: 49739 RVA: 0x0047DBCE File Offset: 0x0047BDCE
		public override bool IsChanged()
		{
			return this.CurrentValue != this.DefaultValue;
		}

		// Token: 0x0600C24C RID: 49740 RVA: 0x0047DBE1 File Offset: 0x0047BDE1
		public override object GetDefaultValue()
		{
			return this.DefaultValue;
		}

		// Token: 0x0600C24D RID: 49741 RVA: 0x0047DBEE File Offset: 0x0047BDEE
		public override int GetDefaultIntValue()
		{
			return this.DefaultValue;
		}

		// Token: 0x0600C24E RID: 49742 RVA: 0x0047DBF6 File Offset: 0x0047BDF6
		public override int GetDefaultIndex()
		{
			return base.GetValueSet().GetIntIndex(this.DefaultValue);
		}

		// Token: 0x0600C24F RID: 49743 RVA: 0x0047DC09 File Offset: 0x0047BE09
		public override void SetToDefault()
		{
			this.CurrentValue = this.DefaultValue;
		}

		// Token: 0x0600C250 RID: 49744 RVA: 0x0047DC17 File Offset: 0x0047BE17
		public override string GetValueText()
		{
			return string.Format("{0} | default:{1}", this.CurrentValue, this.DefaultValue);
		}

		// Token: 0x0600C251 RID: 49745 RVA: 0x0047DC39 File Offset: 0x0047BE39
		public override int GetValueIndex()
		{
			return base.GetValueSet().GetIntIndex(this.CurrentValue);
		}

		// Token: 0x0600C252 RID: 49746 RVA: 0x0047DC4C File Offset: 0x0047BE4C
		public override int GetValueIndex(string value)
		{
			int val;
			if (StringParsers.TryParseSInt32(value, out val))
			{
				return base.GetValueSet().GetIntIndex(val);
			}
			return base.GetValueSet().GetIntIndex(this.DefaultValue);
		}

		// Token: 0x0600C253 RID: 49747 RVA: 0x0047DC81 File Offset: 0x0047BE81
		public override void SetValueFromIndex(int index)
		{
			if (!base.GetValueSet().GetIntValue(index, out this.CurrentValue))
			{
				this.CurrentValue = this.DefaultValue;
			}
		}

		// Token: 0x0600C254 RID: 49748 RVA: 0x0047DCA3 File Offset: 0x0047BEA3
		public override SandboxOptions[] GetAlwaysShowOptions()
		{
			if (this.DisabledOptions == null || !this.DisabledOptions.AlwaysShowValuesOnEnabled)
			{
				return null;
			}
			return this.DisabledOptions.DisabledOptions;
		}

		// Token: 0x04009332 RID: 37682
		public int DefaultValue;

		// Token: 0x04009333 RID: 37683
		public int CurrentValue = 1;

		// Token: 0x04009334 RID: 37684
		public SandboxOptionInt.DisabledOptionsOnValue DisabledOptions;

		// Token: 0x0200189C RID: 6300
		public class DisabledOptionsOnValue
		{
			// Token: 0x0600C255 RID: 49749 RVA: 0x0047DCC7 File Offset: 0x0047BEC7
			public DisabledOptionsOnValue(SandboxOptions[] options, int value, bool inverted = false, bool alwaysShowValuesOnEnabled = false)
			{
				this.DisabledOptions = options;
				this.Value = value;
				this.Inverted = inverted;
				this.AlwaysShowValuesOnEnabled = alwaysShowValuesOnEnabled;
			}

			// Token: 0x04009335 RID: 37685
			public int Value;

			// Token: 0x04009336 RID: 37686
			public bool Inverted;

			// Token: 0x04009337 RID: 37687
			public SandboxOptions[] DisabledOptions;

			// Token: 0x04009338 RID: 37688
			public bool AlwaysShowValuesOnEnabled;
		}
	}
}
