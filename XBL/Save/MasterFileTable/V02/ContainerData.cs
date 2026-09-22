using System;

namespace Platform.XBL.Save.MasterFileTable.V02
{
	// Token: 0x02001C74 RID: 7284
	public sealed class ContainerData : IDisposable, IMigratable
	{
		// Token: 0x17001ABF RID: 6847
		// (get) Token: 0x0600D7DE RID: 55262 RVA: 0x0002F184 File Offset: 0x0002D384
		public ushort Version
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17001AC0 RID: 6848
		// (get) Token: 0x0600D7DF RID: 55263 RVA: 0x004DD72E File Offset: 0x004DB92E
		// (set) Token: 0x0600D7E0 RID: 55264 RVA: 0x004DD736 File Offset: 0x004DB936
		public Node RootNode { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Node();

		// Token: 0x0600D7E1 RID: 55265 RVA: 0x004DD73F File Offset: 0x004DB93F
		public void Write(PooledBinaryWriter writer)
		{
			writer.Write(2);
			this.RootNode.Write(writer);
		}

		// Token: 0x0600D7E2 RID: 55266 RVA: 0x004DD754 File Offset: 0x004DB954
		public void Read(PooledBinaryReader reader)
		{
			reader.ReadUInt16();
			this.RootNode.Read(reader);
		}

		// Token: 0x0600D7E3 RID: 55267 RVA: 0x004DD769 File Offset: 0x004DB969
		public void Dispose()
		{
			Node rootNode = this.RootNode;
			if (rootNode != null)
			{
				rootNode.Dispose();
			}
			this.RootNode = null;
		}

		// Token: 0x0400A475 RID: 42101
		public const ushort VERSION = 2;
	}
}
