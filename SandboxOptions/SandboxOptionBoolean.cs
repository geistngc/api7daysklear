using System;

namespace SandboxOptions
{
	// Token: 0x02001897 RID: 6295
	public class SandboxOptionBoolean : BaseSandboxOption
	{
		// Token: 0x170017DB RID: 6107
		// (get) Token: 0x0600C21D RID: 49693 RVA: 0x00080864 File Offset: 0x0007EA64
		public override BaseSandboxOption.OptionTypes OptionType
		{
			get
			{
				return BaseSandboxOption.OptionTypes.Bool;
			}
		}

		// Token: 0x0600C21E RID: 49694 RVA: 0x0047D7E0 File Offset: 0x0047B9E0
		public SandboxOptionBoolean(SandboxOptions option, string optionName, string categoryName, string valueSetName, bool defaultValue, bool newUISection = false, SandboxOptionBoolean.DisabledOptionsOnValue disabledOptions = null) : base(option, optionName, categoryName, valueSetName, newUISection)
		{
			this.DefaultValue = defaultValue;
			this.CurrentValue = defaultValue;
			this.DisabledOptions = disabledOptions;
		}

		// Token: 0x0600C21F RID: 49695 RVA: 0x0047D813 File Offset: 0x0047BA13
		public override bool GetBoolValue()
		{
			return this.CurrentValue;
		}

		// Token: 0x0600C220 RID: 49696 RVA: 0x0047D81B File Offset: 0x0047BA1B
		public override object GetValue()
		{
			return this.GetBoolValue();
		}

		// Token: 0x170017DC RID: 6108
		// (get) Token: 0x0600C221 RID: 49697 RVA: 0x0047D828 File Offset: 0x0047BA28
		public override string ValueText
		{
			get
			{
				return this.CurrentValue.ToString();
			}
		}

		// Token: 0x0600C222 RID: 49698 RVA: 0x0047D838 File Offset: 0x0047BA38
		public override void SetValue(string value)
		{
			bool currentValue;
			if (StringParsers.TryParseBool(value, out currentValue))
			{
				this.CurrentValue = currentValue;
			}
		}

		// Token: 0x0600C223 RID: 49699 RVA: 0x0047D856 File Offset: 0x0047BA56
		public override void SetBool(bool value)
		{
			this.CurrentValue = value;
		}

		// Token: 0x0600C224 RID: 49700 RVA: 0x0047D85F File Offset: 0x0047BA5F
		public override bool IsChanged()
		{
			return this.CurrentValue != this.DefaultValue;
		}

		// Token: 0x0600C225 RID: 49701 RVA: 0x0047D872 File Offset: 0x0047BA72
		public override object GetDefaultValue()
		{
			return this.DefaultValue;
		}

		// Token: 0x0600C226 RID: 49702 RVA: 0x0047D87F File Offset: 0x0047BA7F
		public override bool GetDefaultBoolValue()
		{
			return this.DefaultValue;
		}

		// Token: 0x0600C227 RID: 49703 RVA: 0x0047D887 File Offset: 0x0047BA87
		public override int GetDefaultIndex()
		{
			return base.GetValueSet().GetBoolIndex(this.DefaultValue);
		}

		// Token: 0x0600C228 RID: 49704 RVA: 0x0047D89A File Offset: 0x0047BA9A
		public override void SetToDefault()
		{
			this.CurrentValue = this.DefaultValue;
		}

		// Token: 0x0600C229 RID: 49705 RVA: 0x0047D8A8 File Offset: 0x0047BAA8
		public override string GetValueText()
		{
			return string.Format("{0} | default:{1}", this.CurrentValue, this.DefaultValue);
		}

		// Token: 0x0600C22A RID: 49706 RVA: 0x0047D8CA File Offset: 0x0047BACA
		public override int GetValueIndex()
		{
			return base.GetValueSet().GetBoolIndex(this.CurrentValue);
		}

		// Token: 0x0600C22B RID: 49707 RVA: 0x0047D8E0 File Offset: 0x0047BAE0
		public override int GetValueIndex(string value)
		{
			bool val;
			if (StringParsers.TryParseBool(value, out val))
			{
				return base.GetValueSet().GetBoolIndex(val);
			}
			return base.GetValueSet().GetBoolIndex(this.DefaultValue);
		}

		// Token: 0x0600C22C RID: 49708 RVA: 0x0047D915 File Offset: 0x0047BB15
		public override void SetValueFromIndex(int index)
		{
			if (!base.GetValueSet().GetBoolValue(index, out this.CurrentValue))
			{
				this.CurrentValue = this.DefaultValue;
			}
		}

		// Token: 0x0600C22D RID: 49709 RVA: 0x0047D937 File Offset: 0x0047BB37
		public override SandboxOptions[] GetAlwaysShowOptions()
		{
			if (this.DisabledOptions == null || !this.DisabledOptions.AlwaysShowValuesOnEnabled)
			{
				return null;
			}
			return this.DisabledOptions.DisabledOptions;
		}

		// Token: 0x04009323 RID: 37667
		public bool DefaultValue;

		// Token: 0x04009324 RID: 37668
		public bool CurrentValue;

		// Token: 0x04009325 RID: 37669
		public SandboxOptionBoolean.DisabledOptionsOnValue DisabledOptions;

		// Token: 0x02001898 RID: 6296
		public class DisabledOptionsOnValue
		{
			// Token: 0x0600C22E RID: 49710 RVA: 0x0047D95B File Offset: 0x0047BB5B
			public DisabledOptionsOnValue(SandboxOptions[] options, bool value, bool inverted = false, bool alwaysShowValuesOnEnabled = false)
			{
				this.DisabledOptions = options;
				this.Value = value;
				this.Inverted = inverted;
				this.AlwaysShowValuesOnEnabled = alwaysShowValuesOnEnabled;
			}

			// Token: 0x04009326 RID: 37670
			public bool Value;

			// Token: 0x04009327 RID: 37671
			public bool Inverted;

			// Token: 0x04009328 RID: 37672
			public SandboxOptions[] DisabledOptions;

			// Token: 0x04009329 RID: 37673
			public bool AlwaysShowValuesOnEnabled;
		}
	}
}
