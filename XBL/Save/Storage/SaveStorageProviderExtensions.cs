using System;
using Platform.XBL.Save.Storage.Blobs;
using Platform.XBL.Save.Storage.Files;

namespace Platform.XBL.Save.Storage
{
	// Token: 0x02001C53 RID: 7251
	public static class SaveStorageProviderExtensions
	{
		// Token: 0x0600D6DD RID: 55005 RVA: 0x004D85B8 File Offset: 0x004D67B8
		public static ISaveStorageProvider Create(this SaveStorageProvider storageProvider)
		{
			ISaveStorageProvider result;
			if (storageProvider != SaveStorageProvider.Blobs)
			{
				if (storageProvider != SaveStorageProvider.Files)
				{
					throw new ArgumentOutOfRangeException("storageProvider", storageProvider, string.Format("Unknown {0}: {1}", "SaveStorageProvider", storageProvider));
				}
				result = new SaveStorageStorageProviderFiles();
			}
			else
			{
				result = new SaveStorageStorageProviderBlobs();
			}
			return result;
		}
	}
}
