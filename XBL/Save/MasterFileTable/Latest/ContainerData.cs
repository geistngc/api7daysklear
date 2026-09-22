using System;
using System.IO;

namespace Platform.XBL.Save.MasterFileTable.Latest
{
	// Token: 0x02001C79 RID: 7289
	public sealed class ContainerData : IDisposable, IMigratable
	{
		// Token: 0x17001AC8 RID: 6856
		// (get) Token: 0x0600D817 RID: 55319 RVA: 0x004DE5D7 File Offset: 0x004DC7D7
		// (set) Token: 0x0600D818 RID: 55320 RVA: 0x004DE5DF File Offset: 0x004DC7DF
		public ushort Version { get; [PublicizedFrom(EAccessModifier.Private)] set; } = 6;

		// Token: 0x17001AC9 RID: 6857
		// (get) Token: 0x0600D819 RID: 55321 RVA: 0x004DE5E8 File Offset: 0x004DC7E8
		// (set) Token: 0x0600D81A RID: 55322 RVA: 0x004DE5F0 File Offset: 0x004DC7F0
		public byte[] FutureData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = Array.Empty<byte>();

		// Token: 0x17001ACA RID: 6858
		// (get) Token: 0x0600D81B RID: 55323 RVA: 0x004DE5F9 File Offset: 0x004DC7F9
		// (set) Token: 0x0600D81C RID: 55324 RVA: 0x004DE601 File Offset: 0x004DC801
		public Node RootNode { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Node
		{
			Attributes = NodeAttributes.Directory
		};

		// Token: 0x0600D81D RID: 55325 RVA: 0x004DE60C File Offset: 0x004DC80C
		public void Write(PooledBinaryWriter writer)
		{
			this.RootNode.Attributes = NodeAttributes.Directory;
			writer.Write(this.Version);
			int value = 0;
			long position = writer.BaseStream.Position;
			writer.Write(value);
			long position2 = writer.BaseStream.Position;
			writer.Write(this.FutureData);
			long position3 = writer.BaseStream.Position;
			value = (int)(position3 - position2);
			writer.BaseStream.Position = position;
			writer.Write(value);
			writer.BaseStream.Position = position3;
			this.RootNode.Write(writer);
		}

		// Token: 0x0600D81E RID: 55326 RVA: 0x004DE69C File Offset: 0x004DC89C
		public void Read(PooledBinaryReader reader)
		{
			this.Version = reader.ReadUInt16();
			int num = reader.ReadInt32();
			long position = reader.BaseStream.Position;
			long position2 = reader.BaseStream.Position;
			this.FutureData = new byte[(int)((long)num - (position2 - position))];
			int num2;
			if (!reader.TryReadAllBytes(this.FutureData, out num2))
			{
				throw new IOException(string.Format("Expected {0} bytes to be read for future data but only got {1} bytes.", this.FutureData.Length, num2));
			}
			Migrator.ReadMigrate(reader.BaseStream, this.RootNode, "Node", Migrator.s_nodeMigrators);
			this.RootNode.Attributes = NodeAttributes.Directory;
		}

		// Token: 0x0600D81F RID: 55327 RVA: 0x004DE745 File Offset: 0x004DC945
		public void Dispose()
		{
			Node rootNode = this.RootNode;
			if (rootNode != null)
			{
				rootNode.Dispose();
			}
			this.RootNode = null;
		}

		// Token: 0x0400A48E RID: 42126
		public const ushort VERSION = 6;
	}
}
