using System;

namespace SharpEXR
{
	// Token: 0x020016C4 RID: 5828
	public class Channel
	{
		// Token: 0x1700162F RID: 5679
		// (get) Token: 0x0600B614 RID: 46612 RVA: 0x0043D732 File Offset: 0x0043B932
		// (set) Token: 0x0600B615 RID: 46613 RVA: 0x0043D73A File Offset: 0x0043B93A
		public string Name { get; set; }

		// Token: 0x17001630 RID: 5680
		// (get) Token: 0x0600B616 RID: 46614 RVA: 0x0043D743 File Offset: 0x0043B943
		// (set) Token: 0x0600B617 RID: 46615 RVA: 0x0043D74B File Offset: 0x0043B94B
		public PixelType Type { get; set; }

		// Token: 0x17001631 RID: 5681
		// (get) Token: 0x0600B618 RID: 46616 RVA: 0x0043D754 File Offset: 0x0043B954
		// (set) Token: 0x0600B619 RID: 46617 RVA: 0x0043D75C File Offset: 0x0043B95C
		public bool Linear { get; set; }

		// Token: 0x17001632 RID: 5682
		// (get) Token: 0x0600B61A RID: 46618 RVA: 0x0043D765 File Offset: 0x0043B965
		// (set) Token: 0x0600B61B RID: 46619 RVA: 0x0043D76D File Offset: 0x0043B96D
		public int XSampling { get; set; }

		// Token: 0x17001633 RID: 5683
		// (get) Token: 0x0600B61C RID: 46620 RVA: 0x0043D776 File Offset: 0x0043B976
		// (set) Token: 0x0600B61D RID: 46621 RVA: 0x0043D77E File Offset: 0x0043B97E
		public int YSampling { get; set; }

		// Token: 0x17001634 RID: 5684
		// (get) Token: 0x0600B61E RID: 46622 RVA: 0x0043D787 File Offset: 0x0043B987
		// (set) Token: 0x0600B61F RID: 46623 RVA: 0x0043D78F File Offset: 0x0043B98F
		public byte[] Reserved { get; set; }

		// Token: 0x0600B620 RID: 46624 RVA: 0x0043D798 File Offset: 0x0043B998
		public Channel(string name, PixelType type, bool linear, int xSampling, int ySampling) : this(name, type, linear, 0, 0, 0, xSampling, ySampling)
		{
		}

		// Token: 0x0600B621 RID: 46625 RVA: 0x0043D7B5 File Offset: 0x0043B9B5
		public Channel(string name, PixelType type, bool linear, byte reserved0, byte reserved1, byte reserved2, int xSampling, int ySampling)
		{
			this.Name = name;
			this.Type = type;
			this.Linear = linear;
			this.Reserved = new byte[]
			{
				reserved0,
				reserved1,
				reserved2
			};
		}

		// Token: 0x0600B622 RID: 46626 RVA: 0x0043D7ED File Offset: 0x0043B9ED
		public override string ToString()
		{
			return string.Format("{0} {1} {2}", base.GetType().Name, this.Name, this.Type);
		}
	}
}
