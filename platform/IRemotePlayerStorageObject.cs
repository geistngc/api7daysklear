using System;
using System.IO;

namespace Platform
{
	// Token: 0x02001B9A RID: 7066
	public interface IRemotePlayerStorageObject
	{
		// Token: 0x0600D31C RID: 54044
		void ReadInto(BinaryReader _reader);

		// Token: 0x0600D31D RID: 54045
		void WriteFrom(BinaryWriter _writer);
	}
}
