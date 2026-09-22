using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Platform.XBL.Save.MasterFileTable.V02
{
	// Token: 0x02001C75 RID: 7285
	public sealed class Node : IDisposable
	{
		// Token: 0x17001AC1 RID: 6849
		// (get) Token: 0x0600D7E5 RID: 55269 RVA: 0x004DD796 File Offset: 0x004DB996
		// (set) Token: 0x0600D7E6 RID: 55270 RVA: 0x004DD79E File Offset: 0x004DB99E
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600D7E7 RID: 55271 RVA: 0x004DD7A8 File Offset: 0x004DB9A8
		public Node()
		{
			this.Name = string.Empty;
			this.LastWriteTimeUtc = Node.DefaultLastWriteTime;
			this.BlobIds = new List<ulong>();
			this.Children = new SortedDictionary<string, Node>();
			this.m_lock = new object();
			this.Parent = null;
		}

		// Token: 0x0600D7E8 RID: 55272 RVA: 0x004DD7FC File Offset: 0x004DB9FC
		public override string ToString()
		{
			object @lock = this.m_lock;
			string result;
			lock (@lock)
			{
				result = string.Format("{0}[{1}=\"{2}\", {3}=[{4}], #{5}={6}]", new object[]
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
			return result;
		}

		// Token: 0x0600D7E9 RID: 55273 RVA: 0x004DD8A8 File Offset: 0x004DBAA8
		public void Write(PooledBinaryWriter writer)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				writer.Write(this.Name);
				writer.Write(this.LastWriteTimeUtc.Ticks);
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
		}

		// Token: 0x0600D7EA RID: 55274 RVA: 0x004DD9B4 File Offset: 0x004DBBB4
		public void Read(PooledBinaryReader reader)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.Name = reader.ReadString();
				this.LastWriteTimeUtc = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
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
		}

		// Token: 0x0600D7EB RID: 55275 RVA: 0x004DDA78 File Offset: 0x004DBC78
		public void Dispose()
		{
			object @lock = this.m_lock;
			lock (@lock)
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
		}

		// Token: 0x0600D7EC RID: 55276 RVA: 0x004DDB20 File Offset: 0x004DBD20
		public bool IsDirectory()
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = (this.BlobIds.Count == 0);
			}
			return result;
		}

		// Token: 0x0600D7ED RID: 55277 RVA: 0x004DDB6C File Offset: 0x004DBD6C
		public bool IsFile()
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = (this.Children.Count == 0);
			}
			return result;
		}

		// Token: 0x0600D7EE RID: 55278 RVA: 0x004DDBB8 File Offset: 0x004DBDB8
		public Node GetChildNode(string name)
		{
			object @lock = this.m_lock;
			Node result;
			lock (@lock)
			{
				Node node;
				if (!this.Children.TryGetValue(name, out node))
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

		// Token: 0x0600D7EF RID: 55279 RVA: 0x004DDC0C File Offset: 0x004DBE0C
		public Node GetOrCreateChildNode(string name)
		{
			object @lock = this.m_lock;
			Node result;
			lock (@lock)
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
				result = node;
			}
			return result;
		}

		// Token: 0x0600D7F0 RID: 55280 RVA: 0x004DDC7C File Offset: 0x004DBE7C
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
				Node.<Enumerate>g__AddNodeChildrenToStack|19_0(stack, this);
			}
			while (stack.Count > 0)
			{
				Node node = stack.Pop();
				list.Add(node);
				Node.<Enumerate>g__AddNodeChildrenToStack|19_0(stack, node);
			}
			return list;
		}

		// Token: 0x0600D7F2 RID: 55282 RVA: 0x004DDD28 File Offset: 0x004DBF28
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <Enumerate>g__AddNodeChildrenToStack|19_0(Stack<Node> stack, Node currentNode)
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

		// Token: 0x0400A477 RID: 42103
		public static readonly DateTime DefaultLastWriteTime = new DateTime(0L, DateTimeKind.Utc);

		// Token: 0x0400A479 RID: 42105
		public DateTime LastWriteTimeUtc;

		// Token: 0x0400A47A RID: 42106
		public readonly List<ulong> BlobIds;

		// Token: 0x0400A47B RID: 42107
		public readonly SortedDictionary<string, Node> Children;

		// Token: 0x0400A47C RID: 42108
		public readonly object m_lock;

		// Token: 0x0400A47D RID: 42109
		public Node Parent;
	}
}
