using System;
using Unity.Burst;
using Unity.Collections;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001719 RID: 5913
	[BurstCompile(CompileSynchronously = true)]
	public struct PathNodePool
	{
		// Token: 0x0600B823 RID: 47139 RVA: 0x00447F2C File Offset: 0x0044612C
		public PathNodePool(int _size)
		{
			this.pool = new NativeList<PathNode>(_size, Allocator.Persistent);
			PathNode pathNode = default(PathNode);
			this.pool.AddReplicate(pathNode, _size);
			this.free = 0;
		}

		// Token: 0x0600B824 RID: 47140 RVA: 0x00447F68 File Offset: 0x00446168
		public void Init(int _size)
		{
			this.pool = new NativeList<PathNode>(_size, Allocator.Persistent);
			PathNode pathNode = default(PathNode);
			this.pool.AddReplicate(pathNode, _size);
			this.free = 0;
		}

		// Token: 0x0600B825 RID: 47141 RVA: 0x00447FA4 File Offset: 0x004461A4
		public int Alloc()
		{
			int length = this.pool.Length;
			if (this.free >= length)
			{
				this.pool.ResizeUninitialized(length + 10000);
			}
			int num = this.free;
			this.free = num + 1;
			return num;
		}

		// Token: 0x0600B826 RID: 47142 RVA: 0x00447FE9 File Offset: 0x004461E9
		public void ReturnAll()
		{
			this.free = 0;
		}

		// Token: 0x0600B827 RID: 47143 RVA: 0x00447FF2 File Offset: 0x004461F2
		public void Node(int _index, out PathNode _node)
		{
			_node = this.pool[_index];
		}

		// Token: 0x0600B828 RID: 47144 RVA: 0x00448006 File Offset: 0x00446206
		public ref PathNode Node(int _index)
		{
			return this.pool.ElementAt(_index);
		}

		// Token: 0x0600B829 RID: 47145 RVA: 0x00448014 File Offset: 0x00446214
		public void Cleanup()
		{
			this.pool.Dispose();
		}

		// Token: 0x0600B82A RID: 47146 RVA: 0x00448021 File Offset: 0x00446221
		public void LogStats()
		{
			Log.Out(string.Format("PathNodePool: Capacity={0}, Allocated={1}, Free={2}", this.pool.Capacity, this.pool.Length, this.free));
		}

		// Token: 0x040089DC RID: 35292
		[PublicizedFrom(EAccessModifier.Private)]
		public NativeList<PathNode> pool;

		// Token: 0x040089DD RID: 35293
		[PublicizedFrom(EAccessModifier.Private)]
		public int free;
	}
}
