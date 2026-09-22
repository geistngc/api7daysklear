using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Platform.XBL.Save.MasterFileTable.V03
{
	// Token: 0x02001C72 RID: 7282
	public sealed class Node : IDisposable
	{
		// Token: 0x17001ABC RID: 6844
		// (get) Token: 0x0600D7C8 RID: 55240 RVA: 0x004DCEAD File Offset: 0x004DB0AD
		// (set) Token: 0x0600D7C9 RID: 55241 RVA: 0x004DCEB5 File Offset: 0x004DB0B5
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001ABD RID: 6845
		// (get) Token: 0x0600D7CA RID: 55242 RVA: 0x004DCEBE File Offset: 0x004DB0BE
		public IReadOnlyList<ulong> BlobIds
		{
			get
			{
				return this.m_blobIds;
			}
		}

		// Token: 0x17001ABE RID: 6846
		// (get) Token: 0x0600D7CB RID: 55243 RVA: 0x004DCEC6 File Offset: 0x004DB0C6
		public IReadOnlyDictionary<string, Node> Children
		{
			get
			{
				return this.m_children;
			}
		}

		// Token: 0x0600D7CC RID: 55244 RVA: 0x004DCED0 File Offset: 0x004DB0D0
		public Node()
		{
			this.Name = string.Empty;
			this.LastWriteTimeUtc = Node.DefaultLastWriteTime;
			this.Attributes = NodeAttributes.None;
			this.m_blobIds = Array.Empty<ulong>();
			this.m_blobLock = new object();
			this.m_children = new StringSpanDictionary<Node>(new SortedDictionary<string, Node>());
			this.m_lock = new object();
			this.Parent = null;
		}

		// Token: 0x0600D7CD RID: 55245 RVA: 0x004DCF38 File Offset: 0x004DB138
		public void SetBlobIds(ulong[] blobIds)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_blobIds = blobIds;
			}
		}

		// Token: 0x0600D7CE RID: 55246 RVA: 0x004DCF7C File Offset: 0x004DB17C
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
					"BlobIds",
					string.Join(", ", this.BlobIds.Select(new Func<ulong, string>(SaveContainer.IdToString))),
					"Children",
					this.Children.Count
				});
			}
			return result;
		}

		// Token: 0x0600D7CF RID: 55247 RVA: 0x004DD058 File Offset: 0x004DB258
		public void Write(PooledBinaryWriter writer)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				writer.Write(this.Name);
				writer.Write(this.LastWriteTimeUtc.Ticks);
				writer.Write(Convert.ToUInt32(this.Attributes));
				ulong[] blobIds = this.m_blobIds;
				ushort value = (ushort)blobIds.Length;
				writer.Write(value);
				foreach (ulong value2 in blobIds)
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
		}

		// Token: 0x0600D7D0 RID: 55248 RVA: 0x004DD150 File Offset: 0x004DB350
		public void Read(PooledBinaryReader reader)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.Name = reader.ReadString();
				this.LastWriteTimeUtc = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
				this.Attributes = (NodeAttributes)reader.ReadUInt32();
				ushort num = reader.ReadUInt16();
				ulong[] array = new ulong[(int)num];
				for (int i = 0; i < (int)num; i++)
				{
					ulong num2 = reader.ReadUInt64();
					array[i] = num2;
				}
				this.m_blobIds = array;
				ushort num3 = reader.ReadUInt16();
				for (int j = 0; j < (int)num3; j++)
				{
					Node node = new Node();
					node.Read(reader);
					this.m_children.Add(node.Name, node);
					node.Parent = this;
				}
			}
		}

		// Token: 0x0600D7D1 RID: 55249 RVA: 0x004DD22C File Offset: 0x004DB42C
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
				this.m_blobIds = Array.Empty<ulong>();
				this.Name = string.Empty;
			}
		}

		// Token: 0x0600D7D2 RID: 55250 RVA: 0x004DD2CC File Offset: 0x004DB4CC
		public bool IsDirectory()
		{
			return this.Attributes.HasFlag(NodeAttributes.Directory);
		}

		// Token: 0x0600D7D3 RID: 55251 RVA: 0x004DD2E4 File Offset: 0x004DB4E4
		public bool IsFile()
		{
			return !this.IsDirectory();
		}

		// Token: 0x0600D7D4 RID: 55252 RVA: 0x004DD2F0 File Offset: 0x004DB4F0
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

		// Token: 0x0600D7D5 RID: 55253 RVA: 0x004DD344 File Offset: 0x004DB544
		public Node GetOrCreateChildNode(StringSpan name, bool createDirectory)
		{
			bool flag;
			return this.GetOrCreateChildNode(name, createDirectory, out flag);
		}

		// Token: 0x0600D7D6 RID: 55254 RVA: 0x004DD35C File Offset: 0x004DB55C
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

		// Token: 0x0600D7D7 RID: 55255 RVA: 0x004DD41C File Offset: 0x004DB61C
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

		// Token: 0x0600D7D8 RID: 55256 RVA: 0x004DD488 File Offset: 0x004DB688
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

		// Token: 0x0600D7D9 RID: 55257 RVA: 0x004DD524 File Offset: 0x004DB724
		public void MoveChild(Node child)
		{
			this.MoveChild(child, child.Name);
		}

		// Token: 0x0600D7DA RID: 55258 RVA: 0x004DD538 File Offset: 0x004DB738
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

		// Token: 0x0600D7DC RID: 55260 RVA: 0x004DD600 File Offset: 0x004DB800
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

		// Token: 0x0600D7DD RID: 55261 RVA: 0x004DD680 File Offset: 0x004DB880
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

		// Token: 0x0400A469 RID: 42089
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly DateTime DefaultLastWriteTime = new DateTime(0L, DateTimeKind.Utc);

		// Token: 0x0400A46B RID: 42091
		public DateTime LastWriteTimeUtc;

		// Token: 0x0400A46C RID: 42092
		public NodeAttributes Attributes;

		// Token: 0x0400A46D RID: 42093
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong[] m_blobIds;

		// Token: 0x0400A46E RID: 42094
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StringSpanDictionary<Node> m_children;

		// Token: 0x0400A46F RID: 42095
		public readonly object m_blobLock;

		// Token: 0x0400A470 RID: 42096
		public readonly object m_lock;

		// Token: 0x0400A471 RID: 42097
		public Node Parent;
	}
}
