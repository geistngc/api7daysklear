using System;
using System.Collections.Generic;
using System.IO;

namespace Webserver.FileCache
{
	// Token: 0x02001B0F RID: 6927
	public class SimpleCache : AbstractCache
	{
		// Token: 0x0600D01A RID: 53274 RVA: 0x004BE794 File Offset: 0x004BC994
		public override byte[] GetFileContent(string _filename)
		{
			try
			{
				Dictionary<string, byte[]> obj = this.fileCache;
				lock (obj)
				{
					byte[] result;
					if (this.fileCache.TryGetValue(_filename, out result))
					{
						return result;
					}
					if (!File.Exists(_filename))
					{
						return null;
					}
					byte[] array = File.ReadAllBytes(_filename);
					this.fileCache.Add(_filename, array);
					return array;
				}
			}
			catch (Exception arg)
			{
				Log.Out(string.Format("Error in SimpleCache.GetFileContent: {0}", arg));
			}
			return null;
		}

		// Token: 0x0600D01B RID: 53275 RVA: 0x004BE82C File Offset: 0x004BCA2C
		public override ValueTuple<int, int> Invalidate()
		{
			ValueTuple<int, int> result = new ValueTuple<int, int>(0, 0);
			Dictionary<string, byte[]> obj = this.fileCache;
			lock (obj)
			{
				result.Item1 = this.fileCache.Count;
				foreach (KeyValuePair<string, byte[]> keyValuePair in this.fileCache)
				{
					string text;
					byte[] array;
					keyValuePair.Deconstruct(out text, out array);
					byte[] array2 = array;
					result.Item2 += array2.Length;
				}
				this.fileCache.Clear();
			}
			return result;
		}

		// Token: 0x04009E4C RID: 40524
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, byte[]> fileCache = new Dictionary<string, byte[]>();
	}
}
