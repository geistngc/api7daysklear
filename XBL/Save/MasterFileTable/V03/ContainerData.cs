using System;

namespace Platform.XBL.Save.MasterFileTable.V03
{
	// Token: 0x02001C71 RID: 7281
	public sealed class ContainerData : IDisposable, IMigratable
	{
		// Token: 0x17001ABA RID: 6842
		// (get) Token: 0x0600D7C1 RID: 55233 RVA: 0x00046EF6 File Offset: 0x000450F6
		public ushort Version
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17001ABB RID: 6843
		// (get) Token: 0x0600D7C2 RID: 55234 RVA: 0x004DCE26 File Offset: 0x004DB026
		// (set) Token: 0x0600D7C3 RID: 55235 RVA: 0x004DCE2E File Offset: 0x004DB02E
		public Node RootNode { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Node
		{
			Attributes = NodeAttributes.Directory
		};

		// Token: 0x0600D7C4 RID: 55236 RVA: 0x004DCE37 File Offset: 0x004DB037
		public void Write(PooledBinaryWriter writer)
		{
			this.RootNode.Attributes = NodeAttributes.Directory;
			writer.Write(3);
			this.RootNode.Write(writer);
		}

		// Token: 0x0600D7C5 RID: 55237 RVA: 0x004DCE58 File Offset: 0x004DB058
		public void Read(PooledBinaryReader reader)
		{
			reader.ReadUInt16();
			this.RootNode.Read(reader);
			this.RootNode.Attributes = NodeAttributes.Directory;
		}

		// Token: 0x0600D7C6 RID: 55238 RVA: 0x004DCE79 File Offset: 0x004DB079
		public void Dispose()
		{
			Node rootNode = this.RootNode;
			if (rootNode != null)
			{
				rootNode.Dispose();
			}
			this.RootNode = null;
		}

		// Token: 0x0400A467 RID: 42087
		public const ushort VERSION = 3;
	}
}
