using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform.XBL.Save
{
	// Token: 0x02001C42 RID: 7234
	public sealed class PathEnumerationInfo
	{
		// Token: 0x0600D676 RID: 54902 RVA: 0x004D5BA6 File Offset: 0x004D3DA6
		public PathEnumerationInfo(string relativePath, bool isDirectory, bool isFile)
		{
			this.RelativePath = relativePath;
			this.IsDirectory = isDirectory;
			this.IsFile = isFile;
		}

		// Token: 0x0600D677 RID: 54903 RVA: 0x004D5BC4 File Offset: 0x004D3DC4
		public override string ToString()
		{
			return string.Format("{0}[{1}=\"{2}\", {3}={4}, {5}={6}]", new object[]
			{
				"PathEnumerationInfo",
				"RelativePath",
				this.RelativePath,
				"IsDirectory",
				this.IsDirectory,
				"IsFile",
				this.IsFile
			});
		}

		// Token: 0x0600D678 RID: 54904 RVA: 0x004D5C28 File Offset: 0x004D3E28
		public void UsedOnlyForAOTCodeGeneration()
		{
			IEnumerable<SaveDataManagedPath> source = from _ in Enumerable.Empty<PathEnumerationInfo>()
			select null;
			from _ in source
			select null;
			from _ in source
			select null;
			from _ in source
			select null;
			throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
		}

		// Token: 0x0400A39A RID: 41882
		public readonly string RelativePath;

		// Token: 0x0400A39B RID: 41883
		public readonly bool IsDirectory;

		// Token: 0x0400A39C RID: 41884
		public readonly bool IsFile;
	}
}
