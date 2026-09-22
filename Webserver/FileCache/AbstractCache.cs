using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Webserver.FileCache
{
	// Token: 0x02001B0C RID: 6924
	public abstract class AbstractCache
	{
		// Token: 0x0600D00D RID: 53261
		public abstract byte[] GetFileContent(string _filename);

		// Token: 0x0600D00E RID: 53262
		[return: TupleElementNames(new string[]
		{
			"filesDropped",
			"bytesDropped"
		})]
		public abstract ValueTuple<int, int> Invalidate();

		// Token: 0x0600D00F RID: 53263 RVA: 0x004BE64E File Offset: 0x004BC84E
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractCache()
		{
			AbstractCache.caches.Add(this);
		}

		// Token: 0x0600D010 RID: 53264 RVA: 0x004BE664 File Offset: 0x004BC864
		public static ValueTuple<int, int> InvalidateAllCaches()
		{
			int num = 0;
			int num2 = 0;
			foreach (AbstractCache abstractCache in AbstractCache.caches)
			{
				ValueTuple<int, int> valueTuple = abstractCache.Invalidate();
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				num += item;
				num2 += item2;
			}
			return new ValueTuple<int, int>(num, num2);
		}

		// Token: 0x04009E4B RID: 40523
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly List<AbstractCache> caches = new List<AbstractCache>();
	}
}
