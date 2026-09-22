using System;

namespace SandboxOptions
{
	// Token: 0x020018A5 RID: 6309
	public class SandboxOptionValueSetInt : SandboxOptionValueSet
	{
		// Token: 0x0600C2BC RID: 49852 RVA: 0x00483458 File Offset: 0x00481658
		public override bool GetValue(int index, out object val)
		{
			int num;
			bool intValue = this.GetIntValue(index, out num);
			val = num;
			return intValue;
		}

		// Token: 0x0600C2BD RID: 49853 RVA: 0x00483476 File Offset: 0x00481676
		public override bool GetIntValue(int index, out int val)
		{
			if (index >= 0 && index < this.IntValues.Length)
			{
				val = this.IntValues[index];
				return true;
			}
			val = 0;
			return false;
		}

		// Token: 0x0600C2BE RID: 49854 RVA: 0x00483498 File Offset: 0x00481698
		public override int GetIntIndex(int val)
		{
			for (int i = 0; i < this.IntValues.Length; i++)
			{
				if (val == this.IntValues[i])
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600C2BF RID: 49855 RVA: 0x004834C6 File Offset: 0x004816C6
		public override bool IsValidIndex(int index)
		{
			return index >= 0 && index < this.IntValues.Length;
		}

		// Token: 0x0600C2C0 RID: 49856 RVA: 0x004834D9 File Offset: 0x004816D9
		public override int GetValueCount()
		{
			return this.IntValues.Length;
		}

		// Token: 0x0600C2C1 RID: 49857 RVA: 0x004834E4 File Offset: 0x004816E4
		public override string GetDisplayAtIndex(int index, string languageName = null)
		{
			if (index < 0 || index >= this.IntValues.Length)
			{
				return "";
			}
			string text = null;
			if (this.AlternateDisplayValues != null && this.AlternateDisplayValues.Length > index)
			{
				text = this.AlternateDisplayValues[index];
			}
			string format;
			if (this.DisplayValues != null && this.DisplayValues.Length > index && this.DisplayValues[index] != null)
			{
				format = Localization.Get(this.DisplayValues[index], false, languageName);
			}
			else if (this.DisplayFormat != null)
			{
				format = Localization.Get(this.DisplayFormat, false, languageName);
			}
			else
			{
				format = "{0}";
			}
			return string.Format(format, (text != null) ? text : this.IntValues[index]);
		}

		// Token: 0x0600C2C2 RID: 49858 RVA: 0x0048358A File Offset: 0x0048178A
		public override string GetValueListString()
		{
			return string.Join<int>(", ", this.IntValues);
		}

		// Token: 0x0400940B RID: 37899
		public int[] IntValues;
	}
}
