using System;
using System.Collections.Generic;
using SharpEXR.AttributeTypes;

namespace SharpEXR
{
	// Token: 0x020016CB RID: 5835
	public class EXRHeader
	{
		// Token: 0x1700163F RID: 5695
		// (get) Token: 0x0600B64B RID: 46667 RVA: 0x0043ECC4 File Offset: 0x0043CEC4
		// (set) Token: 0x0600B64C RID: 46668 RVA: 0x0043ECCC File Offset: 0x0043CECC
		public Dictionary<string, EXRAttribute> Attributes { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600B64D RID: 46669 RVA: 0x0043ECD5 File Offset: 0x0043CED5
		public EXRHeader()
		{
			this.Attributes = new Dictionary<string, EXRAttribute>();
		}

		// Token: 0x0600B64E RID: 46670 RVA: 0x0043ECE8 File Offset: 0x0043CEE8
		public void Read(EXRFile file, IEXRReader reader)
		{
			EXRAttribute exrattribute;
			while (EXRAttribute.Read(file, reader, out exrattribute))
			{
				this.Attributes[exrattribute.Name] = exrattribute;
			}
		}

		// Token: 0x0600B64F RID: 46671 RVA: 0x0043ED14 File Offset: 0x0043CF14
		public bool TryGetAttribute<T>(string name, out T result)
		{
			EXRAttribute exrattribute;
			if (!this.Attributes.TryGetValue(name, out exrattribute))
			{
				result = default(T);
				return false;
			}
			if (exrattribute.Value == null)
			{
				result = default(T);
				return !typeof(T).IsClass && !typeof(T).IsInterface && !typeof(T).IsArray;
			}
			if (typeof(T).IsAssignableFrom(exrattribute.Value.GetType()))
			{
				result = (T)((object)exrattribute.Value);
				return true;
			}
			result = default(T);
			return false;
		}

		// Token: 0x17001640 RID: 5696
		// (get) Token: 0x0600B650 RID: 46672 RVA: 0x0043EDB7 File Offset: 0x0043CFB7
		public bool IsEmpty
		{
			get
			{
				return this.Attributes.Count == 0;
			}
		}

		// Token: 0x17001641 RID: 5697
		// (get) Token: 0x0600B651 RID: 46673 RVA: 0x0043EDC8 File Offset: 0x0043CFC8
		public int ChunkCount
		{
			get
			{
				int result;
				if (!this.TryGetAttribute<int>("chunkCount", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing chunkCount attribute.");
				}
				return result;
			}
		}

		// Token: 0x17001642 RID: 5698
		// (get) Token: 0x0600B652 RID: 46674 RVA: 0x0043EDF0 File Offset: 0x0043CFF0
		public Box2I DataWindow
		{
			get
			{
				Box2I result;
				if (!this.TryGetAttribute<Box2I>("dataWindow", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing dataWindow attribute.");
				}
				return result;
			}
		}

		// Token: 0x17001643 RID: 5699
		// (get) Token: 0x0600B653 RID: 46675 RVA: 0x0043EE18 File Offset: 0x0043D018
		public EXRCompression Compression
		{
			get
			{
				EXRCompression result;
				if (!this.TryGetAttribute<EXRCompression>("compression", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing compression attribute.");
				}
				return result;
			}
		}

		// Token: 0x17001644 RID: 5700
		// (get) Token: 0x0600B654 RID: 46676 RVA: 0x0043EE40 File Offset: 0x0043D040
		public PartType Type
		{
			get
			{
				PartType result;
				if (!this.TryGetAttribute<PartType>("type", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing type attribute.");
				}
				return result;
			}
		}

		// Token: 0x17001645 RID: 5701
		// (get) Token: 0x0600B655 RID: 46677 RVA: 0x0043EE68 File Offset: 0x0043D068
		public ChannelList Channels
		{
			get
			{
				ChannelList result;
				if (!this.TryGetAttribute<ChannelList>("channels", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing channels attribute.");
				}
				return result;
			}
		}

		// Token: 0x17001646 RID: 5702
		// (get) Token: 0x0600B656 RID: 46678 RVA: 0x0043EE90 File Offset: 0x0043D090
		public Chromaticities Chromaticities
		{
			get
			{
				foreach (EXRAttribute exrattribute in this.Attributes.Values)
				{
					if (exrattribute.Type == "chromaticities" && exrattribute.Value is Chromaticities)
					{
						return (Chromaticities)exrattribute.Value;
					}
				}
				return EXRHeader.DefaultChromaticities;
			}
		}

		// Token: 0x040088C0 RID: 35008
		public static readonly Chromaticities DefaultChromaticities = new Chromaticities(0.64f, 0.33f, 0.3f, 0.6f, 0.15f, 0.06f, 0.3127f, 0.329f);
	}
}
