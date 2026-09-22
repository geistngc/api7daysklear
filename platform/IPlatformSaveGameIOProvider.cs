using System;
using System.Collections.Generic;
using System.IO;

namespace Platform
{
	// Token: 0x02001B8F RID: 7055
	public interface IPlatformSaveGameIOProvider
	{
		// Token: 0x0600D2F4 RID: 54004
		void ManagedFileRead(SaveDataManagedPath path, Stream dest);

		// Token: 0x0600D2F5 RID: 54005
		void ManagedFileWrite(SaveDataManagedPath path, Stream src);

		// Token: 0x0600D2F6 RID: 54006
		void ManagedFileCopy(SaveDataManagedPath sourceFileName, SaveDataManagedPath destFileName, bool overwrite = false);

		// Token: 0x0600D2F7 RID: 54007
		void ManagedFileDelete(SaveDataManagedPath path);

		// Token: 0x0600D2F8 RID: 54008
		bool ManagedFileExists(SaveDataManagedPath path);

		// Token: 0x0600D2F9 RID: 54009
		DateTime ManagedFileGetLastWriteTimeUtc(SaveDataManagedPath path);

		// Token: 0x0600D2FA RID: 54010
		void ManagedFileMove(SaveDataManagedPath sourceFileName, SaveDataManagedPath destFileName);

		// Token: 0x0600D2FB RID: 54011
		SdDirectoryInfo ManagedDirectoryCreateDirectory(SaveDataManagedPath path);

		// Token: 0x0600D2FC RID: 54012
		DateTime ManagedDirectoryGetLastWriteTimeUtc(SaveDataManagedPath path);

		// Token: 0x0600D2FD RID: 54013
		bool ManagedDirectoryExists(SaveDataManagedPath path);

		// Token: 0x0600D2FE RID: 54014
		IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

		// Token: 0x0600D2FF RID: 54015
		IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

		// Token: 0x0600D300 RID: 54016
		IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFileSystemEntries(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

		// Token: 0x0600D301 RID: 54017
		void ManagedDirectoryDelete(SaveDataManagedPath path, bool recursive);

		// Token: 0x0600D302 RID: 54018
		long ManagedFileInfoLength(SaveDataManagedPath path);

		// Token: 0x0600D303 RID: 54019
		IEnumerable<SdDirectoryInfo> ManagedDirectoryInfoEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

		// Token: 0x0600D304 RID: 54020
		IEnumerable<SdFileInfo> ManagedDirectoryInfoEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

		// Token: 0x0600D305 RID: 54021
		IEnumerable<SdFileSystemInfo> ManagedDirectoryInfoEnumerateFileSystemInfos(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);
	}
}
