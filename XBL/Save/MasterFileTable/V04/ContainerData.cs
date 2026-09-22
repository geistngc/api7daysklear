using System;

namespace Platform.XBL.Save.MasterFileTable.V04
{
	// Token: 0x02001C6E RID: 7278
	public sealed class ContainerData : IDisposable, IMigratable
	{
		// Token: 0x17001AB5 RID: 6837
		// (get) Token: 0x0600D7A4 RID: 55204 RVA: 0x00080864 File Offset: 0x0007EA64
		public ushort Version
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17001AB6 RID: 6838
		// (get) Token: 0x0600D7A5 RID: 55205 RVA: 0x004DC535 File Offset: 0x004DA735
		// (set) Token: 0x0600D7A6 RID: 55206 RVA: 0x004DC53D File Offset: 0x004DA73D
		public Node RootNode { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Node
		{
			Attributes = NodeAttributes.Directory
		};

		// Token: 0x0600D7A7 RID: 55207 RVA: 0x004DC546 File Offset: 0x004DA746
		public void Write(PooledBinaryWriter writer)
		{
			this.RootNode.Attributes = NodeAttributes.Directory;
			writer.Write(4);
			this.RootNode.Write(writer);
		}

		// Token: 0x0600D7A8 RID: 55208 RVA: 0x004DC567 File Offset: 0x004DA767
		public void Read(PooledBinaryReader reader)
		{
			reader.ReadUInt16();
			this.RootNode.Read(reader);
			this.RootNode.Attributes = NodeAttributes.Directory;
		}

		// Token: 0x0600D7A9 RID: 55209 RVA: 0x004DC588 File Offset: 0x004DA788
		public void Dispose()
		{
			Node rootNode = this.RootNode;
			if (rootNode != null)
			{
				rootNode.Dispose();
			}
			this.RootNode = null;
		}

		// Token: 0x0400A459 RID: 42073
		public const ushort VERSION = 4;
	}
}
