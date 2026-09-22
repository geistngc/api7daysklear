using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Platform.XBL.Save.MasterFileTable.V05
{
	// Token: 0x02001C6B RID: 7275
	public sealed class Node : IDisposable, IMigratable
	{
		// Token: 0x17001AB0 RID: 6832
		// (get) Token: 0x0600D77C RID: 55164 RVA: 0x004DB7AF File Offset: 0x004D99AF
		// (set) Token: 0x0600D77D RID: 55165 RVA: 0x004DB7B7 File Offset: 0x004D99B7
		public ushort Version { get; [PublicizedFrom(EAccessModifier.Private)] set; } = 5;

		// Token: 0x17001AB1 RID: 6833
		// (get) Token: 0x0600D77E RID: 55166 RVA: 0x004DB7C0 File Offset: 0x004D99C0
		// (set) Token: 0x0600D77F RID: 55167 RVA: 0x004DB7C8 File Offset: 0x004D99C8
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001AB2 RID: 6834
		// (get) Token: 0x0600D780 RID: 55168 RVA: 0x004DB7D1 File Offset: 0x004D99D1
		public IReadOnlyList<BlobRef> BlobRefs
		{
			get
			{
				return this.m_blobRefs;
			}
		}

		// Token: 0x17001AB3 RID: 6835
		// (get) Token: 0x0600D781 RID: 55169 RVA: 0x004DB7D9 File Offset: 0x004D99D9
		// (set) Token: 0x0600D782 RID: 55170 RVA: 0x004DB7E1 File Offset: 0x004D99E1
		public byte[] FutureData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = Array.Empty<byte>();

		// Token: 0x17001AB4 RID: 6836
		// (get) Token: 0x0600D783 RID: 55171 RVA: 0x004DB7EA File Offset: 0x004D99EA
		public IReadOnlyDictionary<string, Node> Children
		{
			get
			{
				return this.m_children;
			}
		}

		// Token: 0x0600D784 RID: 55172 RVA: 0x004DB7F4 File Offset: 0x004D99F4
		public Node()
		{
			this.Name = string.Empty;
			this.LastWriteTimeUtc = Node.DefaultLastWriteTime;
			this.Attributes = NodeAttributes.None;
			this.m_blobRefs = Array.Empty<BlobRef>();
			this.m_blobLock = new object();
			this.m_children = new StringSpanDictionary<Node>(new SortedDictionary<string, Node>());
			this.m_lock = new object();
			this.Parent = null;
		}

		// Token: 0x0600D785 RID: 55173 RVA: 0x004DB870 File Offset: 0x004D9A70
		public void SetBlobRefs(BlobRef[] blobRefs)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_blobRefs = blobRefs;
			}
		}

		// Token: 0x0600D786 RID: 55174 RVA: 0x004DB8B4 File Offset: 0x004D9AB4
		public override string ToString()
		{
			object @lock = this.m_lock;
			string result;
			lock (@lock)
			{
				result = string.Format("{0}[{1}=\"{2}\", {3}=\"{4}\", {5}=\"{6}\", {7}=[{8}], #{9}={10}]", new object[]
				{
					"Node",
					"Name",
					this.Name,
					"LastWriteTimeUtc",
					this.LastWriteTimeUtc,
					"Attributes",
					this.Attributes,
					"BlobRefs",
					string.Join<BlobRef>(", ", this.BlobRefs),
					"Children",
					this.Children.Count
				});
			}
			return result;
		}

		// Token: 0x0600D787 RID: 55175 RVA: 0x004DB980 File Offset: 0x004D9B80
		public void Write(PooledBinaryWriter writer)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				writer.Write(this.Version);
				int value = 0;
				long position = writer.BaseStream.Position;
				writer.Write(value);
				long position2 = writer.BaseStream.Position;
				writer.Write(this.Name);
				writer.Write(this.LastWriteTimeUtc.Ticks);
				writer.Write(Convert.ToUInt32(this.Attributes));
				BlobRef[] blobRefs = this.m_blobRefs;
				ushort value2 = (ushort)blobRefs.Length;
				writer.Write(value2);
				BlobRef[] array = blobRefs;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Write(writer);
				}
				writer.Write(this.FutureData);
				long position3 = writer.BaseStream.Position;
				value = (int)(position3 - position2);
				writer.BaseStream.Position = position;
				writer.Write(value);
				writer.BaseStream.Position = position3;
				ushort value3 = (ushort)this.Children.Count;
				writer.Write(value3);
				foreach (Node node in this.Children.Values)
				{
					node.Write(writer);
				}
			}
		}

		// Token: 0x0600D788 RID: 55176 RVA: 0x004DBB00 File Offset: 0x004D9D00
		public void Read(PooledBinaryReader reader)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.Version = reader.ReadUInt16();
				int num = reader.ReadInt32();
				long position = reader.BaseStream.Position;
				this.Name = reader.ReadString();
				this.LastWriteTimeUtc = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
				this.Attributes = (NodeAttributes)reader.ReadUInt32();
				ushort num2 = reader.ReadUInt16();
				BlobRef[] array = new BlobRef[(int)num2];
				for (int i = 0; i < (int)num2; i++)
				{
					BlobRef blobRef = new BlobRef();
					Migrator.ReadMigrate(reader.BaseStream, blobRef, "BlobRef", Migrator.s_blobRefMigrators);
					array[i] = blobRef;
				}
				this.m_blobRefs = array;
				long position2 = reader.BaseStream.Position;
				this.FutureData = new byte[(int)((long)num - (position2 - position))];
				int num3;
				if (!reader.TryReadAllBytes(this.FutureData, out num3))
				{
					throw new IOException(string.Format("Expected {0} bytes to be read for future data but only got {1} bytes.", this.FutureData.Length, num3));
				}
				ushort num4 = reader.ReadUInt16();
				for (int j = 0; j < (int)num4; j++)
				{
					Node node = new Node();
					Migrator.ReadMigrate(reader.BaseStream, node, "Node", Migrator.s_nodeMigrators);
					this.m_children.Add(node.Name, node);
					node.Parent = this;
				}
			}
		}

		// Token: 0x0600D789 RID: 55177 RVA: 0x004DBC8C File Offset: 0x004D9E8C
		public void Dispose()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				foreach (Node node in this.Children.Values)
				{
					node.Dispose();
				}
				this.m_children.Clear();
				this.Parent = null;
				this.m_blobRefs = Array.Empty<BlobRef>();
				this.Name = string.Empty;
			}
		}

		// Token: 0x0600D78A RID: 55178 RVA: 0x004DBD2C File Offset: 0x004D9F2C
		public bool IsDirectory()
		{
			return this.Attributes.HasFlag(NodeAttributes.Directory);
		}

		// Token: 0x0600D78B RID: 55179 RVA: 0x004DBD44 File Offset: 0x004D9F44
		public bool IsFile()
		{
			return !this.IsDirectory();
		}

		// Token: 0x0600D78C RID: 55180 RVA: 0x004DBD50 File Offset: 0x004D9F50
		public Node GetChildNode(StringSpan name)
		{
			object @lock = this.m_lock;
			Node result;
			lock (@lock)
			{
				Node node;
				if (!this.m_children.TryGetValue(name, out node))
				{
					result = null;
				}
				else
				{
					result = node;
				}
			}
			return result;
		}

		// Token: 0x0600D78D RID: 55181 RVA: 0x004DBDA4 File Offset: 0x004D9FA4
		public Node GetOrCreateChildNode(StringSpan name, bool createDirectory)
		{
			bool flag;
			return this.GetOrCreateChildNode(name, createDirectory, out flag);
		}

		// Token: 0x0600D78E RID: 55182 RVA: 0x004DBDBC File Offset: 0x004D9FBC
		public Node GetOrCreateChildNode(StringSpan name, bool createDirectory, out bool wasCreated)
		{
			object @lock = this.m_lock;
			Node result;
			lock (@lock)
			{
				Node node;
				if (this.m_children.TryGetValue(name, out node))
				{
					wasCreated = false;
					result = node;
				}
				else
				{
					string text = name.ToString();
					node = new Node
					{
						Name = text
					};
					node.Parent = this;
					this.m_children.Add(text, node);
					this.LastWriteTimeUtc = (node.LastWriteTimeUtc = DateTime.UtcNow);
					this.Attributes |= NodeAttributes.Directory;
					if (createDirectory)
					{
						node.Attributes |= NodeAttributes.Directory;
					}
					wasCreated = true;
					result = node;
				}
			}
			return result;
		}

		// Token: 0x0600D78F RID: 55183 RVA: 0x004DBE7C File Offset: 0x004DA07C
		public Node DeleteChildNode(StringSpan name)
		{
			object @lock = this.m_lock;
			Node result;
			lock (@lock)
			{
				Node node;
				if (!this.m_children.TryGetValue(name, out node) || !this.m_children.Remove(name))
				{
					result = null;
				}
				else
				{
					this.LastWriteTimeUtc = DateTime.UtcNow;
					result = node;
				}
			}
			return result;
		}

		// Token: 0x0600D790 RID: 55184 RVA: 0x004DBEE8 File Offset: 0x004DA0E8
		public IEnumerable<Node> Enumerate(bool includeSelf, bool recursive)
		{
			List<Node> list = new List<Node>();
			if (!recursive)
			{
				if (includeSelf)
				{
					list.Add(this);
				}
				object @lock = this.m_lock;
				lock (@lock)
				{
					list.AddRange(this.Children.Values);
					return list;
				}
			}
			Stack<Node> stack = new Stack<Node>();
			if (includeSelf)
			{
				stack.Push(this);
			}
			else
			{
				Node.<Enumerate>g__AddNodeChildrenToStack|37_0(stack, this);
			}
			while (stack.Count > 0)
			{
				Node node = stack.Pop();
				list.Add(node);
				Node.<Enumerate>g__AddNodeChildrenToStack|37_0(stack, node);
			}
			return list;
		}

		// Token: 0x0600D791 RID: 55185 RVA: 0x004DBF84 File Offset: 0x004DA184
		public void MoveChild(Node child)
		{
			this.MoveChild(child, child.Name);
		}

		// Token: 0x0600D792 RID: 55186 RVA: 0x004DBF98 File Offset: 0x004DA198
		public void MoveChild(Node child, StringSpan newName)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				object lock2 = child.m_lock;
				lock (lock2)
				{
					Node parent = child.Parent;
					if (parent == null || parent == this)
					{
						Node.<MoveChild>g__MoveChildInternal|39_0(parent, this, child, newName);
					}
					else
					{
						object lock3 = parent.m_lock;
						lock (lock3)
						{
							Node.<MoveChild>g__MoveChildInternal|39_0(parent, this, child, newName);
						}
					}
				}
			}
		}

		// Token: 0x0600D794 RID: 55188 RVA: 0x004DC060 File Offset: 0x004DA260
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <Enumerate>g__AddNodeChildrenToStack|37_0(Stack<Node> stack, Node currentNode)
		{
			object @lock = currentNode.m_lock;
			lock (@lock)
			{
				foreach (Node item in currentNode.Children.Values.Reverse<Node>())
				{
					stack.Push(item);
				}
			}
		}

		// Token: 0x0600D795 RID: 55189 RVA: 0x004DC0E0 File Offset: 0x004DA2E0
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <MoveChild>g__MoveChildInternal|39_0(Node oldParent, Node newParent, Node child, StringSpan newName)
		{
			if (oldParent == newParent && child.Name == newName)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (oldParent != null)
			{
				oldParent.m_children.Remove(child.Name);
				oldParent.LastWriteTimeUtc = utcNow;
			}
			if (child.Name != newName)
			{
				child.Name = newName.ToString();
			}
			child.Parent = newParent;
			Node node;
			newParent.m_children.Remove(child.Name, out node);
			if (node != null && node != child)
			{
				node.Dispose();
			}
			newParent.m_children.Add(child.Name, child);
			newParent.LastWriteTimeUtc = utcNow;
		}

		// Token: 0x0400A443 RID: 42051
		public const ushort VERSION = 5;

		// Token: 0x0400A444 RID: 42052
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly DateTime DefaultLastWriteTime = new DateTime(0L, DateTimeKind.Utc);

		// Token: 0x0400A447 RID: 42055
		public DateTime LastWriteTimeUtc;

		// Token: 0x0400A448 RID: 42056
		public NodeAttributes Attributes;

		// Token: 0x0400A449 RID: 42057
		[PublicizedFrom(EAccessModifier.Private)]
		public BlobRef[] m_blobRefs;

		// Token: 0x0400A44B RID: 42059
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StringSpanDictionary<Node> m_children;

		// Token: 0x0400A44C RID: 42060
		public readonly object m_blobLock;

		// Token: 0x0400A44D RID: 42061
		public readonly object m_lock;

		// Token: 0x0400A44E RID: 42062
		public Node Parent;
	}
}
