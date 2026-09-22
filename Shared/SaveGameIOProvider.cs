using System;
using System.Collections.Generic;
using System.IO;

namespace Platform.Shared
{
	// Token: 0x02001CB4 RID: 7348
	public abstract class SaveGameIOProvider : IPlatformSaveGameIOProvider
	{
		// Token: 0x0600D9F4 RID: 55796
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract string GetPath(SaveDataManagedPath path);

		// Token: 0x0600D9F5 RID: 55797 RVA: 0x004E5624 File Offset: 0x004E3824
		public void ManagedFileRead(SaveDataManagedPath path, Stream dest)
		{
			using (FileStream fileStream = File.OpenRead(this.GetPath(path)))
			{
				StreamUtils.StreamCopy(fileStream, dest, null, true);
			}
		}

		// Token: 0x0600D9F6 RID: 55798 RVA: 0x004E5664 File Offset: 0x004E3864
		public void ManagedFileWrite(SaveDataManagedPath path, Stream src)
		{
			using (FileStream fileStream = File.Open(this.GetPath(path), FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				StreamUtils.StreamCopy(src, fileStream, null, true);
			}
		}

		// Token: 0x0600D9F7 RID: 55799 RVA: 0x004E56A8 File Offset: 0x004E38A8
		public void ManagedFileCopy(SaveDataManagedPath sourceFileName, SaveDataManagedPath destFileName, bool overwrite = false)
		{
			File.Copy(this.GetPath(sourceFileName), this.GetPath(destFileName), overwrite);
		}

		// Token: 0x0600D9F8 RID: 55800 RVA: 0x004E56BE File Offset: 0x004E38BE
		public void ManagedFileDelete(SaveDataManagedPath path)
		{
			File.Delete(this.GetPath(path));
		}

		// Token: 0x0600D9F9 RID: 55801 RVA: 0x004E56CC File Offset: 0x004E38CC
		public bool ManagedFileExists(SaveDataManagedPath path)
		{
			return File.Exists(this.GetPath(path));
		}

		// Token: 0x0600D9FA RID: 55802 RVA: 0x004E56DA File Offset: 0x004E38DA
		public DateTime ManagedFileGetLastWriteTimeUtc(SaveDataManagedPath path)
		{
			return File.GetLastWriteTimeUtc(this.GetPath(path));
		}

		// Token: 0x0600D9FB RID: 55803 RVA: 0x004E56E8 File Offset: 0x004E38E8
		public void ManagedFileMove(SaveDataManagedPath sourceFileName, SaveDataManagedPath destFileName)
		{
			File.Move(this.GetPath(sourceFileName), this.GetPath(destFileName));
		}

		// Token: 0x0600D9FC RID: 55804 RVA: 0x004E56FD File Offset: 0x004E38FD
		public SdDirectoryInfo ManagedDirectoryCreateDirectory(SaveDataManagedPath path)
		{
			return new SdDirectoryInfo(Directory.CreateDirectory(this.GetPath(path)));
		}

		// Token: 0x0600D9FD RID: 55805 RVA: 0x004E5710 File Offset: 0x004E3910
		public DateTime ManagedDirectoryGetLastWriteTimeUtc(SaveDataManagedPath path)
		{
			return Directory.GetLastWriteTimeUtc(this.GetPath(path));
		}

		// Token: 0x0600D9FE RID: 55806 RVA: 0x004E571E File Offset: 0x004E391E
		public bool ManagedDirectoryExists(SaveDataManagedPath path)
		{
			return Directory.Exists(this.GetPath(path));
		}

		// Token: 0x0600D9FF RID: 55807 RVA: 0x004E572C File Offset: 0x004E392C
		public IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			string path2 = this.GetPath(path);
			return SaveGameProviderHelper.GetManagedPathsFromBaseAndSubPaths(path, path2, Directory.EnumerateDirectories(path2, searchPattern, searchOption));
		}

		// Token: 0x0600DA00 RID: 55808 RVA: 0x004E5750 File Offset: 0x004E3950
		public IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			string path2 = this.GetPath(path);
			return SaveGameProviderHelper.GetManagedPathsFromBaseAndSubPaths(path, path2, Directory.EnumerateFiles(path2, searchPattern, searchOption));
		}

		// Token: 0x0600DA01 RID: 55809 RVA: 0x004E5774 File Offset: 0x004E3974
		public IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFileSystemEntries(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			string path2 = this.GetPath(path);
			return SaveGameProviderHelper.GetManagedPathsFromBaseAndSubPaths(path, path2, Directory.EnumerateFileSystemEntries(path2, searchPattern, searchOption));
		}

		// Token: 0x0600DA02 RID: 55810 RVA: 0x004E5798 File Offset: 0x004E3998
		public void ManagedDirectoryDelete(SaveDataManagedPath path, bool recursive)
		{
			Directory.Delete(this.GetPath(path), recursive);
		}

		// Token: 0x0600DA03 RID: 55811 RVA: 0x004E57A7 File Offset: 0x004E39A7
		public long ManagedFileInfoLength(SaveDataManagedPath path)
		{
			return new FileInfo(this.GetPath(path)).Length;
		}

		// Token: 0x0600DA04 RID: 55812 RVA: 0x004E57BA File Offset: 0x004E39BA
		public IEnumerable<SdDirectoryInfo> ManagedDirectoryInfoEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			string basePath = this.GetPath(path);
			DirectoryInfo directoryInfo = new DirectoryInfo(basePath);
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.EnumerateDirectories(searchPattern, searchOption))
			{
				string fullName = directoryInfo2.FullName;
				SaveDataManagedPath managedPathFromBaseAndSubPath = SaveGameProviderHelper.GetManagedPathFromBaseAndSubPath(path, basePath, fullName);
				yield return new SdDirectoryInfo(managedPathFromBaseAndSubPath);
			}
			IEnumerator<DirectoryInfo> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600DA05 RID: 55813 RVA: 0x004E57DF File Offset: 0x004E39DF
		public IEnumerable<SdFileInfo> ManagedDirectoryInfoEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			string basePath = this.GetPath(path);
			DirectoryInfo directoryInfo = new DirectoryInfo(basePath);
			foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles(searchPattern, searchOption))
			{
				string fullName = fileInfo.FullName;
				SaveDataManagedPath managedPathFromBaseAndSubPath = SaveGameProviderHelper.GetManagedPathFromBaseAndSubPath(path, basePath, fullName);
				yield return new SdFileInfo(managedPathFromBaseAndSubPath);
			}
			IEnumerator<FileInfo> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600DA06 RID: 55814 RVA: 0x004E5804 File Offset: 0x004E3A04
		public IEnumerable<SdFileSystemInfo> ManagedDirectoryInfoEnumerateFileSystemInfos(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			string basePath = this.GetPath(path);
			DirectoryInfo directoryInfo = new DirectoryInfo(basePath);
			foreach (FileSystemInfo fileSystemInfo in directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption))
			{
				string fullName = fileSystemInfo.FullName;
				SaveDataManagedPath managedPathFromBaseAndSubPath = SaveGameProviderHelper.GetManagedPathFromBaseAndSubPath(path, basePath, fullName);
				SdFileSystemInfo sdFileSystemInfo;
				if (!(fileSystemInfo is FileInfo))
				{
					if (!(fileSystemInfo is DirectoryInfo))
					{
						throw new NotImplementedException("Unsupported implementation of FileSystemInfo: " + fileSystemInfo.GetType().FullName + ".");
					}
					sdFileSystemInfo = new SdDirectoryInfo(managedPathFromBaseAndSubPath);
				}
				else
				{
					sdFileSystemInfo = new SdFileInfo(managedPathFromBaseAndSubPath);
				}
				yield return sdFileSystemInfo;
			}
			IEnumerator<FileSystemInfo> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600DA07 RID: 55815 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public SaveGameIOProvider()
		{
		}
	}
}
