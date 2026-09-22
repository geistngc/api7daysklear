using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform.XBL.Save.MasterFileTable.V01
{
	// Token: 0x02001C77 RID: 7287
	public sealed class Node : IDisposable
	{
		// Token: 0x17001AC4 RID: 6852
		// (get) Token: 0x0600D7FA RID: 55290 RVA: 0x004DDE10 File Offset: 0x004DC010
		// (set) Token: 0x0600D7FB RID: 55291 RVA: 0x004DDE18 File Offset: 0x004DC018
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600D7FC RID: 55292 RVA: 0x004DDE21 File Offset: 0x004DC021
		public Node()
		{
			this.Name = string.Empty;
			this.BlobIds = new List<ulong>();
			this.Children = new SortedDictionary<string, Node>();
			this.Parent = null;
		}

		// Token: 0x0600D7FD RID: 55293 RVA: 0x004DDE54 File Offset: 0x004DC054
		public override string ToString()
		{
			return string.Format("{0}[{1}=\"{2}\", {3}=[{4}], #{5}={6}]", new object[]
			{
				"Node",
				"Name",
				this.Name,
				"BlobIds",
				string.Join(", ", this.BlobIds.Select(new Func<ulong, string>(SaveContainer.IdToString))),
				"Children",
				this.Children.Count
			});
		}

		// Token: 0x0600D7FE RID: 55294 RVA: 0x004DDED4 File Offset: 0x004DC0D4
		public void Write(PooledBinaryWriter writer)
		{
			writer.Write(this.Name);
			ushort value = (ushort)this.BlobIds.Count;
			writer.Write(value);
			foreach (ulong value2 in this.BlobIds)
			{
				writer.Write(value2);
			}
			ushort value3 = (ushort)this.Children.Count;
			writer.Write(value3);
			foreach (Node node in this.Children.Values)
			{
				node.Write(writer);
			}
		}

		// Token: 0x0600D7FF RID: 55295 RVA: 0x004DDFA4 File Offset: 0x004DC1A4
		public void Read(PooledBinaryReader reader)
		{
			this.Name = reader.ReadString();
			ushort num = reader.ReadUInt16();
			for (int i = 0; i < (int)num; i++)
			{
				ulong item = reader.ReadUInt64();
				this.BlobIds.Add(item);
			}
			ushort num2 = reader.ReadUInt16();
			for (int j = 0; j < (int)num2; j++)
			{
				Node node = new Node();
				node.Read(reader);
				this.Children.Add(node.Name, node);
				node.Parent = this;
			}
		}

		// Token: 0x0600D800 RID: 55296 RVA: 0x004DE028 File Offset: 0x004DC228
		public void Dispose()
		{
			foreach (Node node in this.Children.Values)
			{
				node.Dispose();
			}
			this.Children.Clear();
			this.Parent = null;
			this.BlobIds.Clear();
			this.Name = string.Empty;
		}

		// Token: 0x0600D801 RID: 55297 RVA: 0x004DE0A8 File Offset: 0x004DC2A8
		public bool IsDirectory()
		{
			return this.BlobIds.Count == 0;
		}

		// Token: 0x0600D802 RID: 55298 RVA: 0x004DE0B8 File Offset: 0x004DC2B8
		public bool IsFile()
		{
			return this.Children.Count == 0;
		}

		// Token: 0x0600D803 RID: 55299 RVA: 0x004DE0C8 File Offset: 0x004DC2C8
		public Node GetChildNode(string name)
		{
			Node result;
			if (!this.Children.TryGetValue(name, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600D804 RID: 55300 RVA: 0x004DE0E8 File Offset: 0x004DC2E8
		public Node GetOrCreateChildNode(string name)
		{
			Node node;
			if (!this.Children.TryGetValue(name, out node))
			{
				node = new Node
				{
					Name = name
				};
				node.Parent = this;
				this.Children.Add(name, node);
			}
			return node;
		}

		// Token: 0x0400A481 RID: 42113
		public readonly List<ulong> BlobIds;

		// Token: 0x0400A482 RID: 42114
		public readonly SortedDictionary<string, Node> Children;

		// Token: 0x0400A483 RID: 42115
		public Node Parent;
	}
}
