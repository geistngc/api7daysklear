using System;
using System.IO;

namespace Platform.Local
{
	// Token: 0x02001CCD RID: 7373
	public class RemoteFileStorage : IRemoteFileStorage
	{
		// Token: 0x0600DADF RID: 56031 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x17001B53 RID: 6995
		// (get) Token: 0x0600DAE0 RID: 56032 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool IsReady
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001B54 RID: 6996
		// (get) Token: 0x0600DAE1 RID: 56033 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool Unavailable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600DAE2 RID: 56034 RVA: 0x004E7508 File Offset: 0x004E5708
		public void GetFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback)
		{
			if (_callback == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(_filename))
			{
				_callback(IRemoteFileStorage.EFileDownloadResult.EmptyFilename, null, null);
				return;
			}
			string path = GameIO.GetApplicationPath() + "/fakeRemoteFileStorage/" + _filename;
			if (!File.Exists(path))
			{
				_callback(IRemoteFileStorage.EFileDownloadResult.FileNotFound, "", null);
				return;
			}
			try
			{
				byte[] data = File.ReadAllBytes(path);
				_callback(IRemoteFileStorage.EFileDownloadResult.Ok, null, data);
			}
			catch (Exception ex)
			{
				Log.Error("[Local] ReadFile (" + _filename + ") failed:");
				Log.Exception(ex);
				_callback(IRemoteFileStorage.EFileDownloadResult.Other, ex.Message, null);
			}
		}

		// Token: 0x0600DAE3 RID: 56035 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetCachedFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DAE4 RID: 56036 RVA: 0x000880CC File Offset: 0x000862CC
		public bool CancelGetFile(string _filename, string _cancelReason)
		{
			throw new NotImplementedException();
		}
	}
}
