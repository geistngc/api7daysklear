using System;

namespace SandboxOptions
{
	// Token: 0x020018A4 RID: 6308
	public class SandboxOptionValueSetFloat : SandboxOptionValueSet
	{
		// Token: 0x0600C2B3 RID: 49843 RVA: 0x004832D4 File Offset: 0x004814D4
		public override bool GetValue(int index, out object val)
		{
			float num;
			bool floatValue = this.GetFloatValue(index, out num);
			val = num;
			return floatValue;
		}

		// Token: 0x0600C2B4 RID: 49844 RVA: 0x004832F2 File Offset: 0x004814F2
		public override bool GetFloatValue(int index, out float val)
		{
			if (index >= 0 && index < this.FloatValues.Length)
			{
				val = this.FloatValues[index];
				return true;
			}
			val = 0f;
			return false;
		}

		// Token: 0x0600C2B5 RID: 49845 RVA: 0x00483317 File Offset: 0x00481517
		public override bool GetIntValue(int index, out int val)
		{
			if (index >= 0 && index < this.FloatValues.Length)
			{
				val = (int)(this.FloatValues[index] * 100f);
				return true;
			}
			val = 0;
			return false;
		}

		// Token: 0x0600C2B6 RID: 49846 RVA: 0x00483340 File Offset: 0x00481540
		public override int GetFloatIndex(float val)
		{
			for (int i = 0; i < this.FloatValues.Length; i++)
			{
				if (val == this.FloatValues[i])
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600C2B7 RID: 49847 RVA: 0x0048336E File Offset: 0x0048156E
		public override bool IsValidIndex(int index)
		{
			return index >= 0 && index < this.FloatValues.Length;
		}

		// Token: 0x0600C2B8 RID: 49848 RVA: 0x00483381 File Offset: 0x00481581
		public override int GetValueCount()
		{
			return this.FloatValues.Length;
		}

		// Token: 0x0600C2B9 RID: 49849 RVA: 0x0048338C File Offset: 0x0048158C
		public override string GetDisplayAtIndex(int index, string languageName = null)
		{
			if (index < 0 || index >= this.FloatValues.Length)
			{
				return "";
			}
			string text = "";
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
			return string.Format(format, (text != "") ? text : (this.FloatValues[index] * 100f));
		}

		// Token: 0x0600C2BA RID: 49850 RVA: 0x00483446 File Offset: 0x00481646
		public override string GetValueListString()
		{
			return string.Join<float>(", ", this.FloatValues);
		}

		// Token: 0x0400940A RID: 37898
		public float[] FloatValues;
	}
}
