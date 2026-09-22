using System;

namespace SharpEXR
{
	// Token: 0x020016D2 RID: 5842
	public struct EXRVersion
	{
		// Token: 0x0600B698 RID: 46744 RVA: 0x0044070C File Offset: 0x0043E90C
		public EXRVersion(int version, bool multiPart, bool longNames, bool nonImageParts, bool isSingleTiled = false)
		{
			this.Value = (EXRVersionFlags)(version & 255);
			if (version == 1)
			{
				if (multiPart || nonImageParts)
				{
					throw new EXRFormatException("Invalid or corrupt EXR version: Version 1 EXR files cannot be multi part or have non image parts.");
				}
				if (isSingleTiled)
				{
					this.Value |= EXRVersionFlags.IsSinglePartTiled;
				}
				if (longNames)
				{
					this.Value |= EXRVersionFlags.LongNames;
				}
			}
			else
			{
				if (isSingleTiled)
				{
					this.Value |= EXRVersionFlags.IsSinglePartTiled;
				}
				if (longNames)
				{
					this.Value |= EXRVersionFlags.LongNames;
				}
				if (nonImageParts)
				{
					this.Value |= EXRVersionFlags.NonImageParts;
				}
				if (multiPart)
				{
					this.Value |= EXRVersionFlags.MultiPart;
				}
			}
			this.Verify();
		}

		// Token: 0x0600B699 RID: 46745 RVA: 0x004407C4 File Offset: 0x0043E9C4
		public EXRVersion(int value)
		{
			this.Value = (EXRVersionFlags)value;
			this.Verify();
		}

		// Token: 0x0600B69A RID: 46746 RVA: 0x004407D3 File Offset: 0x0043E9D3
		[PublicizedFrom(EAccessModifier.Private)]
		public void Verify()
		{
			if (this.IsSinglePartTiled && (this.IsMultiPart || this.HasNonImageParts))
			{
				throw new EXRFormatException("Invalid or corrupt EXR version: Version's single part bit was set, but multi part and/or non image data bits were also set.");
			}
		}

		// Token: 0x1700164D RID: 5709
		// (get) Token: 0x0600B69B RID: 46747 RVA: 0x004407F8 File Offset: 0x0043E9F8
		public int Version
		{
			get
			{
				return (int)(this.Value & (EXRVersionFlags)255);
			}
		}

		// Token: 0x1700164E RID: 5710
		// (get) Token: 0x0600B69C RID: 46748 RVA: 0x00440806 File Offset: 0x0043EA06
		public bool IsSinglePartTiled
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.IsSinglePartTiled);
			}
		}

		// Token: 0x1700164F RID: 5711
		// (get) Token: 0x0600B69D RID: 46749 RVA: 0x00440822 File Offset: 0x0043EA22
		public bool HasLongNames
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.LongNames);
			}
		}

		// Token: 0x17001650 RID: 5712
		// (get) Token: 0x0600B69E RID: 46750 RVA: 0x0044083E File Offset: 0x0043EA3E
		public bool HasNonImageParts
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.NonImageParts);
			}
		}

		// Token: 0x17001651 RID: 5713
		// (get) Token: 0x0600B69F RID: 46751 RVA: 0x0044085A File Offset: 0x0043EA5A
		public bool IsMultiPart
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.MultiPart);
			}
		}

		// Token: 0x17001652 RID: 5714
		// (get) Token: 0x0600B6A0 RID: 46752 RVA: 0x00440876 File Offset: 0x0043EA76
		public int MaxNameLength
		{
			get
			{
				if (!this.HasLongNames)
				{
					return 31;
				}
				return 255;
			}
		}

		// Token: 0x040088D3 RID: 35027
		public readonly EXRVersionFlags Value;
	}
}
