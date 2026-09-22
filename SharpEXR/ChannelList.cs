using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpEXR
{
	// Token: 0x020016C6 RID: 5830
	public class ChannelList : IEnumerable<Channel>, IEnumerable
	{
		// Token: 0x17001635 RID: 5685
		// (get) Token: 0x0600B623 RID: 46627 RVA: 0x0043D815 File Offset: 0x0043BA15
		// (set) Token: 0x0600B624 RID: 46628 RVA: 0x0043D81D File Offset: 0x0043BA1D
		public List<Channel> Channels { get; set; }

		// Token: 0x0600B625 RID: 46629 RVA: 0x0043D826 File Offset: 0x0043BA26
		public ChannelList()
		{
			this.Channels = new List<Channel>();
		}

		// Token: 0x0600B626 RID: 46630 RVA: 0x0043D83C File Offset: 0x0043BA3C
		public void Read(EXRFile file, IEXRReader reader, int size)
		{
			int num = 0;
			Channel item;
			int num2;
			while (this.ReadChannel(file, reader, out item, out num2))
			{
				this.Channels.Add(item);
				num += num2;
				if (num > size)
				{
					throw new EXRFormatException(string.Concat(new string[]
					{
						"Read ",
						num.ToString(),
						" bytes but Size was ",
						size.ToString(),
						"."
					}));
				}
			}
			num += num2;
			if (num != size)
			{
				throw new EXRFormatException(string.Concat(new string[]
				{
					"Read ",
					num.ToString(),
					" bytes but Size was ",
					size.ToString(),
					"."
				}));
			}
		}

		// Token: 0x0600B627 RID: 46631 RVA: 0x0043D8F4 File Offset: 0x0043BAF4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool ReadChannel(EXRFile file, IEXRReader reader, out Channel channel, out int bytesRead)
		{
			int position = reader.Position;
			string text = reader.ReadNullTerminatedString(255);
			if (text == "")
			{
				channel = null;
				bytesRead = reader.Position - position;
				return false;
			}
			channel = new Channel(text, (PixelType)reader.ReadInt32(), reader.ReadByte() > 0, reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadInt32(), reader.ReadInt32());
			bytesRead = reader.Position - position;
			return true;
		}

		// Token: 0x0600B628 RID: 46632 RVA: 0x0043D972 File Offset: 0x0043BB72
		public IEnumerator<Channel> GetEnumerator()
		{
			return this.Channels.GetEnumerator();
		}

		// Token: 0x0600B629 RID: 46633 RVA: 0x0043D984 File Offset: 0x0043BB84
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x17001636 RID: 5686
		public Channel this[int index]
		{
			get
			{
				return this.Channels[index];
			}
			set
			{
				this.Channels[index] = value;
			}
		}
	}
}
