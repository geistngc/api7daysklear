using System;

namespace Platform
{
	// Token: 0x02001B97 RID: 7063
	public interface IRemoteFileStorage
	{
		// Token: 0x0600D312 RID: 54034
		void Init(IPlatform _owner);

		// Token: 0x17001A16 RID: 6678
		// (get) Token: 0x0600D313 RID: 54035
		bool IsReady { get; }

		// Token: 0x17001A17 RID: 6679
		// (get) Token: 0x0600D314 RID: 54036
		bool Unavailable { get; }

		// Token: 0x0600D315 RID: 54037
		void GetFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback);

		// Token: 0x0600D316 RID: 54038
		void GetCachedFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback);

		// Token: 0x0600D317 RID: 54039
		bool CancelGetFile(string _filename, string reason);

		// Token: 0x02001B98 RID: 7064
		public enum EFileDownloadResult
		{
			// Token: 0x0400A12C RID: 41260
			Ok,
			// Token: 0x0400A12D RID: 41261
			EmptyFilename,
			// Token: 0x0400A12E RID: 41262
			FileNotFound,
			// Token: 0x0400A12F RID: 41263
			Other
		}

		// Token: 0x02001B99 RID: 7065
		// (Invoke) Token: 0x0600D319 RID: 54041
		public delegate void FileDownloadCompleteCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorName, byte[] _data);
	}
}
