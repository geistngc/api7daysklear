using System;
using Unity.XGamingRuntime;

namespace Platform.XBL.Save.Storage
{
	// Token: 0x02001C4F RID: 7247
	public interface ISaveStorageContainer : IDisposable
	{
		// Token: 0x17001A96 RID: 6806
		// (get) Token: 0x0600D6CB RID: 54987
		bool IsDisposed { get; }

		// Token: 0x17001A97 RID: 6807
		// (get) Token: 0x0600D6CC RID: 54988
		string Name { get; }

		// Token: 0x17001A98 RID: 6808
		// (get) Token: 0x0600D6CD RID: 54989
		DateTime LastAccessed { get; }

		// Token: 0x0600D6CE RID: 54990
		void Flush(bool waitForFlush);

		// Token: 0x0600D6CF RID: 54991
		bool TryEnumerateBlobInfos(out XGameSaveBlobInfo[] blobInfos);

		// Token: 0x0600D6D0 RID: 54992
		XGameSaveBlobInfo GetBlobInfo(string blobName);

		// Token: 0x0600D6D1 RID: 54993 RVA: 0x004D85A0 File Offset: 0x004D67A0
		RefCountedBuffer GetBlob(string blobName, StringSpan debugIdentifier)
		{
			return this.GetBlobs(new string[]
			{
				blobName
			}, debugIdentifier)[0];
		}

		// Token: 0x0600D6D2 RID: 54994
		RefCountedBuffer[] GetBlobs(string[] blobNames, StringSpan debugIdentifier);

		// Token: 0x0600D6D3 RID: 54995
		void SetBlob(string blobName, RefCountedBuffer blobData);

		// Token: 0x0600D6D4 RID: 54996
		void DeleteBlob(string blobName);

		// Token: 0x0400A3D9 RID: 41945
		public const string RootSaveContainerName = "root";
	}
}
