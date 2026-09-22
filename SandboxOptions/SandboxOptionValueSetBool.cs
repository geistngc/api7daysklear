using System;

namespace SandboxOptions
{
	// Token: 0x020018A3 RID: 6307
	public class SandboxOptionValueSetBool : SandboxOptionValueSet
	{
		// Token: 0x0600C2AA RID: 49834 RVA: 0x004831FC File Offset: 0x004813FC
		public override bool GetValue(int index, out object val)
		{
			bool flag;
			bool boolValue = this.GetBoolValue(index, out flag);
			val = flag;
			return boolValue;
		}

		// Token: 0x0600C2AB RID: 49835 RVA: 0x0048321A File Offset: 0x0048141A
		public override bool GetBoolValue(int index, out bool val)
		{
			if (index >= 0 && index < this.BoolValues.Length)
			{
				val = this.BoolValues[index];
				return true;
			}
			val = false;
			return false;
		}

		// Token: 0x0600C2AC RID: 49836 RVA: 0x0048323B File Offset: 0x0048143B
		public override bool GetBoolValue(int index)
		{
			return this.BoolValues[index];
		}

		// Token: 0x0600C2AD RID: 49837 RVA: 0x00483248 File Offset: 0x00481448
		public override int GetBoolIndex(bool val)
		{
			for (int i = 0; i < this.BoolValues.Length; i++)
			{
				if (val == this.BoolValues[i])
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600C2AE RID: 49838 RVA: 0x00483276 File Offset: 0x00481476
		public override bool IsValidIndex(int index)
		{
			return index >= 0 && index < this.BoolValues.Length;
		}

		// Token: 0x0600C2AF RID: 49839 RVA: 0x00483289 File Offset: 0x00481489
		public override int GetValueCount()
		{
			return this.BoolValues.Length;
		}

		// Token: 0x0600C2B0 RID: 49840 RVA: 0x00483293 File Offset: 0x00481493
		public override string GetDisplayAtIndex(int index, string languageName = null)
		{
			if (index < 0 || index >= this.DisplayValues.Length)
			{
				return "";
			}
			return Localization.Get(this.DisplayValues[index], false, languageName);
		}

		// Token: 0x0600C2B1 RID: 49841 RVA: 0x004832B9 File Offset: 0x004814B9
		public override string GetValueListString()
		{
			return string.Join<bool>(", ", this.BoolValues);
		}

		// Token: 0x04009409 RID: 37897
		public bool[] BoolValues;
	}
}
