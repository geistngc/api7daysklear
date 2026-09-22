using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Unity.XGamingRuntime;

namespace Platform.XBL.Save.Storage.Files
{
	// Token: 0x02001C54 RID: 7252
	public sealed class SaveStorageStorageContainerFiles : ISaveStorageContainer, IDisposable
	{
		// Token: 0x0600D6DE RID: 55006 RVA: 0x004D8605 File Offset: 0x004D6805
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogInfo(string text)
		{
			Log.Out("[XBL: SaveStorageStorageContainerFiles] " + text);
		}

		// Token: 0x0600D6DF RID: 55007 RVA: 0x004D8617 File Offset: 0x004D6817
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogError(string text)
		{
			Log.Error("[XBL: SaveStorageStorageContainerFiles] " + text);
		}

		// Token: 0x0600D6E0 RID: 55008 RVA: 0x004D8605 File Offset: 0x004D6805
		[Conditional("DEBUG_SAVE_DATA_MANAGER")]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogTrace(string text)
		{
			Log.Out("[XBL: SaveStorageStorageContainerFiles] " + text);
		}

		// Token: 0x0600D6E1 RID: 55009 RVA: 0x004D862C File Offset: 0x004D682C
		public SaveStorageStorageContainerFiles(string gameSaveFolder, string containerName)
		{
			using (this.m_lastAccessHelper.CreateScope())
			{
				bool flag = false;
				try
				{
					this.m_containerName = containerName;
					this.m_containerPath = Path.GetFullPath(Path.Join(gameSaveFolder, this.m_containerName));
					Directory.CreateDirectory(this.m_containerPath);
					if (!Directory.Exists(this.m_containerPath))
					{
						SaveStorageStorageContainerFiles.LogError("Container '" + containerName + "' failed to create directory at: " + this.m_containerPath);
					}
					else
					{
						SaveStorageStorageContainerFiles.LogInfo("Container '" + containerName + "' exists at: " + this.m_containerPath);
						flag = true;
					}
				}
				finally
				{
					if (!flag)
					{
						this.Dispose();
					}
				}
			}
		}

		// Token: 0x0600D6E2 RID: 55010 RVA: 0x004D870C File Offset: 0x004D690C
		public void Dispose()
		{
			this.m_disposed = true;
		}

		// Token: 0x17001A9B RID: 6811
		// (get) Token: 0x0600D6E3 RID: 55011 RVA: 0x004D8715 File Offset: 0x004D6915
		public bool IsDisposed
		{
			get
			{
				return this.m_disposed;
			}
		}

		// Token: 0x17001A9C RID: 6812
		// (get) Token: 0x0600D6E4 RID: 55012 RVA: 0x004D871D File Offset: 0x004D691D
		public string Name
		{
			get
			{
				return this.m_containerName;
			}
		}

		// Token: 0x17001A9D RID: 6813
		// (get) Token: 0x0600D6E5 RID: 55013 RVA: 0x004D8725 File Offset: 0x004D6925
		public DateTime LastAccessed
		{
			get
			{
				return this.m_lastAccessHelper.Time;
			}
		}

		// Token: 0x0600D6E6 RID: 55014 RVA: 0x000027FC File Offset: 0x000009FC
		public void Flush(bool waitForFlush)
		{
		}

		// Token: 0x0600D6E7 RID: 55015 RVA: 0x004D8734 File Offset: 0x004D6934
		public bool TryEnumerateBlobInfos(out XGameSaveBlobInfo[] blobInfos)
		{
			bool result;
			using (this.m_lastAccessHelper.CreateScope())
			{
				try
				{
					blobInfos = (from fi in new DirectoryInfo(this.m_containerPath).EnumerateFiles()
					select new XGameSaveBlobInfo
					{
						Name = fi.Name,
						Size = (uint)fi.Length
					}).ToArray<XGameSaveBlobInfo>();
					result = true;
				}
				catch (IOException arg)
				{
					SaveStorageStorageContainerFiles.LogError(string.Format("{0} failed: {1}", "TryEnumerateBlobInfos", arg));
					blobInfos = null;
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600D6E8 RID: 55016 RVA: 0x004D87D4 File Offset: 0x004D69D4
		public XGameSaveBlobInfo GetBlobInfo(string blobName)
		{
			XGameSaveBlobInfo result;
			using (this.m_lastAccessHelper.CreateScope())
			{
				FileInfo fileInfo = new FileInfo(Path.Join(this.m_containerPath, blobName));
				if (!fileInfo.Exists)
				{
					result = null;
				}
				else
				{
					result = new XGameSaveBlobInfo
					{
						Name = blobName,
						Size = (uint)fileInfo.Length
					};
				}
			}
			return result;
		}

		// Token: 0x0600D6E9 RID: 55017 RVA: 0x004D8850 File Offset: 0x004D6A50
		public RefCountedBuffer[] GetBlobs(string[] blobNames, StringSpan debugIdentifier)
		{
			RefCountedBuffer[] result;
			using (this.m_lastAccessHelper.CreateScope())
			{
				bool flag = false;
				RefCountedBuffer[] array = new RefCountedBuffer[blobNames.Length];
				try
				{
					for (int i = 0; i < blobNames.Length; i++)
					{
						string blobName = blobNames[i];
						using (FileStream fileStream = File.Open(this.GetBlobPath(blobName), FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							int num = (int)fileStream.Length;
							using (RefCountedBuffer refCountedBuffer = RefCountedBuffer.CreatePooled(num))
							{
								int num2;
								for (int j = 0; j < num; j += num2)
								{
									num2 = fileStream.Read(refCountedBuffer.Span.Slice(j, num - j));
									if (num2 <= 0)
									{
										throw new IOException("Unexpected end of file stream.");
									}
								}
								array[i] = refCountedBuffer.CreateRef();
							}
						}
					}
					flag = true;
					result = array;
				}
				finally
				{
					if (!flag)
					{
						foreach (RefCountedBuffer refCountedBuffer2 in array)
						{
							if (refCountedBuffer2 != null)
							{
								refCountedBuffer2.Dispose();
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600D6EA RID: 55018 RVA: 0x004D8990 File Offset: 0x004D6B90
		public void SetBlob(string blobName, RefCountedBuffer blobData)
		{
			using (this.m_lastAccessHelper.CreateScope())
			{
				using (FileStream fileStream = File.Open(this.GetBlobPath(blobName), FileMode.Create, FileAccess.Write))
				{
					fileStream.Write(blobData.Span);
				}
			}
		}

		// Token: 0x0600D6EB RID: 55019 RVA: 0x004D8A00 File Offset: 0x004D6C00
		public void DeleteBlob(string blobName)
		{
			using (this.m_lastAccessHelper.CreateScope())
			{
				File.Delete(this.GetBlobPath(blobName));
			}
		}

		// Token: 0x0600D6EC RID: 55020 RVA: 0x004D8A48 File Offset: 0x004D6C48
		[PublicizedFrom(EAccessModifier.Private)]
		public string GetBlobPath(string blobName)
		{
			return Path.Join(this.m_containerPath, blobName);
		}

		// Token: 0x0400A3DD RID: 41949
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LastAccessHelper m_lastAccessHelper = new LastAccessHelper();

		// Token: 0x0400A3DE RID: 41950
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string m_containerName;

		// Token: 0x0400A3DF RID: 41951
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string m_containerPath;

		// Token: 0x0400A3E0 RID: 41952
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;
	}
}
