using System;

namespace Platform.XBL.Save.MasterFileTable.V01
{
	// Token: 0x02001C76 RID: 7286
	public sealed class ContainerData : IDisposable, IMigratable
	{
		// Token: 0x17001AC2 RID: 6850
		// (get) Token: 0x0600D7F3 RID: 55283 RVA: 0x0002003D File Offset: 0x0001E23D
		public ushort Version
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001AC3 RID: 6851
		// (get) Token: 0x0600D7F4 RID: 55284 RVA: 0x004DDDA8 File Offset: 0x004DBFA8
		// (set) Token: 0x0600D7F5 RID: 55285 RVA: 0x004DDDB0 File Offset: 0x004DBFB0
		public Node RootNode { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Node();

		// Token: 0x0600D7F6 RID: 55286 RVA: 0x004DDDB9 File Offset: 0x004DBFB9
		public void Write(PooledBinaryWriter writer)
		{
			writer.Write(1);
			this.RootNode.Write(writer);
		}

		// Token: 0x0600D7F7 RID: 55287 RVA: 0x004DDDCE File Offset: 0x004DBFCE
		public void Read(PooledBinaryReader reader)
		{
			reader.ReadUInt16();
			this.RootNode.Read(reader);
		}

		// Token: 0x0600D7F8 RID: 55288 RVA: 0x004DDDE3 File Offset: 0x004DBFE3
		public void Dispose()
		{
			Node rootNode = this.RootNode;
			if (rootNode != null)
			{
				rootNode.Dispose();
			}
			this.RootNode = null;
		}

		// Token: 0x0400A47E RID: 42110
		public const ushort VERSION = 1;
	}
}
