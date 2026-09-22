using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Platform.XBL.Save.MasterFileTable.Latest
{
	// Token: 0x02001C7A RID: 7290
	public sealed class Node : IDisposable, IMigratable
	{
		// Token: 0x17001ACB RID: 6859
		// (get) Token: 0x0600D821 RID: 55329 RVA: 0x004DE78B File Offset: 0x004DC98B
		// (set) Token: 0x0600D822 RID: 55330 RVA: 0x004DE793 File Offset: 0x004DC993
		public ushort Version { get; [PublicizedFrom(EAccessModifier.Private)] set; } = 6;

		// Token: 0x17001ACC RID: 6860
		// (get) Token: 0x0600D823 RID: 55331 RVA: 0x004DE79C File Offset: 0x004DC99C
		// (set) Token: 0x0600D824 RID: 55332 RVA: 0x004DE7A4 File Offset: 0x004DC9A4
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001ACD RID: 6861
		// (get) Token: 0x0600D825 RID: 55333 RVA: 0x004DE7AD File Offset: 0x004DC9AD
		public IReadOnlyList<BlobRef> BlobRefs
		{
			get
			{
				return this.m_blobRefs;
			}
		}

		// Token: 0x17001ACE RID: 6862
		// (get) Token: 0x0600D826 RID: 55334 RVA: 0x004DE7B5 File Offset: 0x004DC9B5
		// (set) Token: 0x0600D827 RID: 55335 RVA: 0x004DE7BD File Offset: 0x004DC9BD
		public byte[] FutureData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = Array.Empty<byte>();

		// Token: 0x17001ACF RID: 6863
		// (get) Token: 0x0600D828 RID: 55336 RVA: 0x004DE7C6 File Offset: 0x004DC9C6
		public IReadOnlyDictionary<string, Node> Children
		{
			get
			{
				return this.m_children;
			}
		}

		// Token: 0x0600D829 RID: 55337 RVA: 0x004DE7D0 File Offset: 0x004DC9D0
		public Node()
		{
			this.Name = string.Empty;
			this.CreationTimeUtc = DateTime.UtcNow;
			this.LastWriteTimeUtc = Node.DefaultLastWriteTime;
			this.Attributes = NodeAttributes.None;
			this.m_blobRefs = Array.Empty<BlobRef>();
			this.m_blobLock = new object();
			this.m_children = new StringSpanDictionary<Node>(new SortedDictionary<string, Node>());
			this.m_lock = new object();
			this.Parent = null;
		}

		// Token: 0x0600D82A RID: 55338 RVA: 0x004DE858 File Offset: 0x004DCA58
		public void SetBlobRefs(BlobRef[] blobRefs)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_blobRefs = blobRefs;
			}
		}

		// Token: 0x0600D82B RID: 55339 RVA: 0x004DE89C File Offset: 0x004DCA9C
		public override string ToString()
		{
			object @lock = this.m_lock;
			string result;
			lock (@lock)
			{
				result = string.Format("{0}[{1}=\"{2}\", {3}=\"{4}\", {5}=\"{6}\", {7}=\"{8}\", {9}=[{10}], #{11}={12}]", new object[]
				{
					"Node",
					"Name",
					this.Name,
					"CreationTimeUtc",
					this.CreationTimeUtc,
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

		// Token: 0x0600D82C RID: 55340 RVA: 0x004DE980 File Offset: 0x004DCB80
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
				writer.Write(this.CreationTimeUtc.Ticks);
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

		// Token: 0x0600D82D RID: 55341 RVA: 0x004DEB10 File Offset: 0x004DCD10
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
				this.CreationTimeUtc = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
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

		// Token: 0x0600D82E RID: 55342 RVA: 0x004DECAC File Offset: 0x004DCEAC
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

		// Token: 0x0600D82F RID: 55343 RVA: 0x004DED4C File Offset: 0x004DCF4C
		public bool IsDirectory()
		{
			return this.Attributes.HasFlag(NodeAttributes.Directory);
		}

		// Token: 0x0600D830 RID: 55344 RVA: 0x004DED64 File Offset: 0x004DCF64
		public bool IsFile()
		{
			return !this.IsDirectory();
		}

		// Token: 0x0600D831 RID: 55345 RVA: 0x004DED70 File Offset: 0x004DCF70
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

		// Token: 0x0600D832 RID: 55346 RVA: 0x004DEDC4 File Offset: 0x004DCFC4
		public Node GetOrCreateChildNode(StringSpan name, bool createDirectory)
		{
			bool flag;
			return this.GetOrCreateChildNode(name, createDirectory, out flag);
		}

		// Token: 0x0600D833 RID: 55347 RVA: 0x004DEDDC File Offset: 0x004DCFDC
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

		// Token: 0x0600D834 RID: 55348 RVA: 0x004DEE9C File Offset: 0x004DD09C
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

		// Token: 0x0600D835 RID: 55349 RVA: 0x004DEF08 File Offset: 0x004DD108
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
				Node.<Enumerate>g__AddNodeChildrenToStack|38_0(stack, this);
			}
			while (stack.Count > 0)
			{
				Node node = stack.Pop();
				list.Add(node);
				Node.<Enumerate>g__AddNodeChildrenToStack|38_0(stack, node);
			}
			return list;
		}

		// Token: 0x0600D836 RID: 55350 RVA: 0x004DEFA4 File Offset: 0x004DD1A4
		public void MoveChild(Node child)
		{
			this.MoveChild(child, child.Name);
		}

		// Token: 0x0600D837 RID: 55351 RVA: 0x004DEFB8 File Offset: 0x004DD1B8
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
						Node.<MoveChild>g__MoveChildInternal|40_0(parent, this, child, newName);
					}
					else
					{
						object lock3 = parent.m_lock;
						lock (lock3)
						{
							Node.<MoveChild>g__MoveChildInternal|40_0(parent, this, child, newName);
						}
					}
				}
			}
		}

		// Token: 0x0600D839 RID: 55353 RVA: 0x004DF080 File Offset: 0x004DD280
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <Enumerate>g__AddNodeChildrenToStack|38_0(Stack<Node> stack, Node currentNode)
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

		// Token: 0x0600D83A RID: 55354 RVA: 0x004DF100 File Offset: 0x004DD300
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <MoveChild>g__MoveChildInternal|40_0(Node oldParent, Node newParent, Node child, StringSpan newName)
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

		// Token: 0x0400A492 RID: 42130
		public const ushort VERSION = 6;

		// Token: 0x0400A493 RID: 42131
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly DateTime DefaultLastWriteTime = new DateTime(0L, DateTimeKind.Utc);

		// Token: 0x0400A496 RID: 42134
		public DateTime LastWriteTimeUtc;

		// Token: 0x0400A497 RID: 42135
		public NodeAttributes Attributes;

		// Token: 0x0400A498 RID: 42136
		[PublicizedFrom(EAccessModifier.Private)]
		public BlobRef[] m_blobRefs;

		// Token: 0x0400A499 RID: 42137
		public DateTime CreationTimeUtc;

		// Token: 0x0400A49B RID: 42139
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StringSpanDictionary<Node> m_children;

		// Token: 0x0400A49C RID: 42140
		public readonly object m_blobLock;

		// Token: 0x0400A49D RID: 42141
		public readonly object m_lock;

		// Token: 0x0400A49E RID: 42142
		public Node Parent;
	}
}
