using System;
using System.Collections.Generic;

namespace MusicUtils
{
	// Token: 0x02001A1B RID: 6683
	public static class FileCleanupUtils
	{
		// Token: 0x0600CB2B RID: 52011 RVA: 0x004A8B14 File Offset: 0x004A6D14
		public static void CleanUpAllWaveFiles()
		{
			for (int i = 0; i < FileCleanupUtils.paths.Count; i++)
			{
				FileCleanupUtils.CleanUpWaveFile(FileCleanupUtils.paths[i]);
			}
		}

		// Token: 0x0600CB2C RID: 52012 RVA: 0x004A8B46 File Offset: 0x004A6D46
		public static void CleanUpWaveFile(string file)
		{
			WaveCleanUp.Create().GetComponent<WaveCleanUp>().FilePath = file;
		}

		// Token: 0x04009AAF RID: 39599
		public static List<string> paths = new List<string>();
	}
}
