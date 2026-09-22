using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Platform.XBL.Save.MasterFileTable.V04
{
	// Token: 0x02001C6F RID: 7279
	public sealed class Node : IDisposable
	{
		// Token: 0x17001AB7 RID: 6839
		// (get) Token: 0x0600D7AB RID: 55211 RVA: 0x004DC5BC File Offset: 0x004DA7BC
		// (set) Token: 0x0600D7AC RID: 55212 RVA: 0x004DC5C4 File Offset: 0x004DA7C4
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001AB8 RID: 6840
		// (get) Token: 0x0600D7AD RID: 55213 RVA: 0x004DC5CD File Offset: 0x004DA7CD
		public IReadOnlyList<BlobRef> BlobRefs
		{
			get
			{
				return this.m_blobRefs;
			}
		}

		// Token: 0x17001AB9 RID: 6841
		// (get) Token: 0x0600D7AE RID: 55214 RVA: 0x004DC5D5 File Offset: 0x004DA7D5
		public IReadOnlyDictionary<string, Node> Children
		{
			get
			{
				return this.m_children;
			}
		}

		// Token: 0x0600D7AF RID: 55215 RVA: 0x004DC5E0 File Offset: 0x004DA7E0
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

		// Token: 0x0600D7B0 RID: 55216 RVA: 0x004DC648 File Offset: 0x004DA848
		public void SetBlobRefs(BlobRef[] blobRefs)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_blobRefs = blobRefs;
			}
		}

		// Token: 0x0600D7B1 RID: 55217 RVA: 0x004DC68C File Offset: 0x004DA88C
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

		// Token: 0x0600D7B2 RID: 55218 RVA: 0x004DC758 File Offset: 0x004DA958
		public void Write(PooledBinaryWriter writer)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				writer.Write(this.Name);
				writer.Write(this.LastWriteTimeUtc.Ticks);
				writer.Write(Convert.ToUInt32(this.Attributes));
				BlobRef[] blobRefs = this.m_blobRefs;
				ushort value = (ushort)blobRefs.Length;
				writer.Write(value);
				BlobRef[] array = blobRefs;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Write(writer);
				}
				ushort value2 = (ushort)this.Children.Count;
				writer.Write(value2);
				foreach (Node node in this.Children.Values)
				{
					node.Write(writer);
				}
			}
		}

		// Token: 0x0600D7B3 RID: 55219 RVA: 0x004DC84C File Offset: 0x004DAA4C
		public void Read(PooledBinaryReader reader)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.Name = reader.ReadString();
				this.LastWriteTimeUtc = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
				this.Attributes = (NodeAttributes)reader.ReadUInt32();
				ushort num = reader.ReadUInt16();
				BlobRef[] array = new BlobRef[(int)num];
				for (int i = 0; i < (int)num; i++)
				{
					array[i] = BlobRef.Read(reader);
				}
				this.m_blobRefs = array;
				ushort num2 = reader.ReadUInt16();
				for (int j = 0; j < (int)num2; j++)
				{
					Node node = new Node();
					node.Read(reader);
					this.m_children.Add(node.Name, node);
					node.Parent = this;
				}
			}
		}

		// Token: 0x0600D7B4 RID: 55220 RVA: 0x004DC924 File Offset: 0x004DAB24
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

		// Token: 0x0600D7B5 RID: 55221 RVA: 0x004DC9C4 File Offset: 0x004DABC4
		public bool IsDirectory()
		{
			return this.Attributes.HasFlag(NodeAttributes.Directory);
		}

		// Token: 0x0600D7B6 RID: 55222 RVA: 0x004DC9DC File Offset: 0x004DABDC
		public bool IsFile()
		{
			return !this.IsDirectory();
		}

		// Token: 0x0600D7B7 RID: 55223 RVA: 0x004DC9E8 File Offset: 0x004DABE8
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

		// Token: 0x0600D7B8 RID: 55224 RVA: 0x004DCA3C File Offset: 0x004DAC3C
		public Node GetOrCreateChildNode(StringSpan name, bool createDirectory)
		{
			bool flag;
			return this.GetOrCreateChildNode(name, createDirectory, out flag);
		}

		// Token: 0x0600D7B9 RID: 55225 RVA: 0x004DCA54 File Offset: 0x004DAC54
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

		// Token: 0x0600D7BA RID: 55226 RVA: 0x004DCB14 File Offset: 0x004DAD14
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

		// Token: 0x0600D7BB RID: 55227 RVA: 0x004DCB80 File Offset: 0x004DAD80
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
				Node.<Enumerate>g__AddNodeChildrenToStack|28_0(stack, this);
			}
			while (stack.Count > 0)
			{
				Node node = stack.Pop();
				list.Add(node);
				Node.<Enumerate>g__AddNodeChildrenToStack|28_0(stack, node);
			}
			return list;
		}

		// Token: 0x0600D7BC RID: 55228 RVA: 0x004DCC1C File Offset: 0x004DAE1C
		public void MoveChild(Node child)
		{
			this.MoveChild(child, child.Name);
		}

		// Token: 0x0600D7BD RID: 55229 RVA: 0x004DCC30 File Offset: 0x004DAE30
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
						Node.<MoveChild>g__MoveChildInternal|30_0(parent, this, child, newName);
					}
					else
					{
						object lock3 = parent.m_lock;
						lock (lock3)
						{
							Node.<MoveChild>g__MoveChildInternal|30_0(parent, this, child, newName);
						}
					}
				}
			}
		}

		// Token: 0x0600D7BF RID: 55231 RVA: 0x004DCCF8 File Offset: 0x004DAEF8
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <Enumerate>g__AddNodeChildrenToStack|28_0(Stack<Node> stack, Node currentNode)
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

		// Token: 0x0600D7C0 RID: 55232 RVA: 0x004DCD78 File Offset: 0x004DAF78
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <MoveChild>g__MoveChildInternal|30_0(Node oldParent, Node newParent, Node child, StringSpan newName)
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

		// Token: 0x0400A45B RID: 42075
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly DateTime DefaultLastWriteTime = new DateTime(0L, DateTimeKind.Utc);

		// Token: 0x0400A45D RID: 42077
		public DateTime LastWriteTimeUtc;

		// Token: 0x0400A45E RID: 42078
		public NodeAttributes Attributes;

		// Token: 0x0400A45F RID: 42079
		[PublicizedFrom(EAccessModifier.Private)]
		public BlobRef[] m_blobRefs;

		// Token: 0x0400A460 RID: 42080
		public readonly object m_blobLock;

		// Token: 0x0400A461 RID: 42081
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StringSpanDictionary<Node> m_children;

		// Token: 0x0400A462 RID: 42082
		public readonly object m_lock;

		// Token: 0x0400A463 RID: 42083
		public Node Parent;
	}
}
