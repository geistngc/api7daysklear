using System;
using System.IO;

namespace Platform.XBL.Save.MasterFileTable.V05
{
	// Token: 0x02001C6A RID: 7274
	public sealed class ContainerData : IDisposable, IMigratable
	{
		// Token: 0x17001AAD RID: 6829
		// (get) Token: 0x0600D772 RID: 55154 RVA: 0x004DB5FB File Offset: 0x004D97FB
		// (set) Token: 0x0600D773 RID: 55155 RVA: 0x004DB603 File Offset: 0x004D9803
		public ushort Version { get; [PublicizedFrom(EAccessModifier.Private)] set; } = 5;

		// Token: 0x17001AAE RID: 6830
		// (get) Token: 0x0600D774 RID: 55156 RVA: 0x004DB60C File Offset: 0x004D980C
		// (set) Token: 0x0600D775 RID: 55157 RVA: 0x004DB614 File Offset: 0x004D9814
		public byte[] FutureData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = Array.Empty<byte>();

		// Token: 0x17001AAF RID: 6831
		// (get) Token: 0x0600D776 RID: 55158 RVA: 0x004DB61D File Offset: 0x004D981D
		// (set) Token: 0x0600D777 RID: 55159 RVA: 0x004DB625 File Offset: 0x004D9825
		public Node RootNode { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Node
		{
			Attributes = NodeAttributes.Directory
		};

		// Token: 0x0600D778 RID: 55160 RVA: 0x004DB630 File Offset: 0x004D9830
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

		// Token: 0x0600D779 RID: 55161 RVA: 0x004DB6C0 File Offset: 0x004D98C0
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

		// Token: 0x0600D77A RID: 55162 RVA: 0x004DB769 File Offset: 0x004D9969
		public void Dispose()
		{
			Node rootNode = this.RootNode;
			if (rootNode != null)
			{
				rootNode.Dispose();
			}
			this.RootNode = null;
		}

		// Token: 0x0400A43F RID: 42047
		public const ushort VERSION = 5;
	}
}
