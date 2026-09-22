using System;
using System.IO;

namespace Webserver.FileCache
{
	// Token: 0x02001B0D RID: 6925
	public class DirectAccess : AbstractCache
	{
		// Token: 0x0600D012 RID: 53266 RVA: 0x004BE6E4 File Offset: 0x004BC8E4
		public override byte[] GetFileContent(string _filename)
		{
			try
			{
				return File.Exists(_filename) ? File.ReadAllBytes(_filename) : null;
			}
			catch (Exception arg)
			{
				Log.Out(string.Format("Error in DirectAccess.GetFileContent: {0}", arg));
			}
			return null;
		}

		// Token: 0x0600D013 RID: 53267 RVA: 0x004B234C File Offset: 0x004B054C
		public override ValueTuple<int, int> Invalidate()
		{
			return new ValueTuple<int, int>(0, 0);
		}
	}
}
