using System;

namespace SandboxOptions
{
	// Token: 0x020018A1 RID: 6305
	public abstract class SandboxOptionValueSet
	{
		// Token: 0x0600C29B RID: 49819 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Init()
		{
		}

		// Token: 0x0600C29C RID: 49820
		public abstract bool GetValue(int index, out object val);

		// Token: 0x0600C29D RID: 49821 RVA: 0x004831E3 File Offset: 0x004813E3
		public virtual bool GetBoolValue(int index, out bool val)
		{
			val = false;
			return false;
		}

		// Token: 0x0600C29E RID: 49822 RVA: 0x004831E9 File Offset: 0x004813E9
		public virtual bool GetIntValue(int index, out int val)
		{
			val = 0;
			return false;
		}

		// Token: 0x0600C29F RID: 49823 RVA: 0x004831EF File Offset: 0x004813EF
		public virtual bool GetFloatValue(int index, out float val)
		{
			val = 0f;
			return false;
		}

		// Token: 0x0600C2A0 RID: 49824 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool GetBoolValue(int index)
		{
			return false;
		}

		// Token: 0x0600C2A1 RID: 49825 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetBoolIndex(bool val)
		{
			return 0;
		}

		// Token: 0x0600C2A2 RID: 49826 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetFloatIndex(float val)
		{
			return 0;
		}

		// Token: 0x0600C2A3 RID: 49827 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetIntIndex(int val)
		{
			return 0;
		}

		// Token: 0x0600C2A4 RID: 49828 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool IsValidIndex(int index)
		{
			return false;
		}

		// Token: 0x0600C2A5 RID: 49829
		public abstract int GetValueCount();

		// Token: 0x0600C2A6 RID: 49830
		public abstract string GetDisplayAtIndex(int index, string languageName = null);

		// Token: 0x0600C2A7 RID: 49831 RVA: 0x00032163 File Offset: 0x00030363
		public virtual string GetValueListString()
		{
			return "";
		}

		// Token: 0x0600C2A8 RID: 49832 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public SandboxOptionValueSet()
		{
		}

		// Token: 0x04009404 RID: 37892
		public string[] DisplayValues;

		// Token: 0x04009405 RID: 37893
		public string[] AlternateDisplayValues;

		// Token: 0x04009406 RID: 37894
		public string DisplayFormat;

		// Token: 0x020018A2 RID: 6306
		public class DisplayOverride
		{
			// Token: 0x04009407 RID: 37895
			public int Index;

			// Token: 0x04009408 RID: 37896
			public string DisplayText;
		}
	}
}
