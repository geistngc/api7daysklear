using System;
using System.IO;
using System.Text;

namespace Platform
{
	// Token: 0x02001BD5 RID: 7125
	public class PlatformLobbyId
	{
		// Token: 0x0600D400 RID: 54272 RVA: 0x004CBFAB File Offset: 0x004CA1AB
		public PlatformLobbyId(EPlatformIdentifier _platformId, string _lobbyId)
		{
			this.PlatformIdentifier = _platformId;
			this.LobbyId = _lobbyId;
		}

		// Token: 0x0600D401 RID: 54273 RVA: 0x004CBFC1 File Offset: 0x004CA1C1
		public int GetWriteLength(Encoding encoding)
		{
			return 1 + this.LobbyId.GetBinaryWriterLength(encoding);
		}

		// Token: 0x0600D402 RID: 54274 RVA: 0x004CBFD1 File Offset: 0x004CA1D1
		public void Write(BinaryWriter _writer)
		{
			_writer.Write((byte)this.PlatformIdentifier);
			if (this.PlatformIdentifier != EPlatformIdentifier.None)
			{
				_writer.Write(this.LobbyId);
			}
		}

		// Token: 0x0600D403 RID: 54275 RVA: 0x004CBFF4 File Offset: 0x004CA1F4
		public static PlatformLobbyId Read(BinaryReader _reader)
		{
			byte b = _reader.ReadByte();
			string lobbyId = (b != 0) ? _reader.ReadString() : string.Empty;
			return new PlatformLobbyId((EPlatformIdentifier)b, lobbyId);
		}

		// Token: 0x0400A1A5 RID: 41381
		public static readonly PlatformLobbyId None = new PlatformLobbyId(EPlatformIdentifier.None, string.Empty);

		// Token: 0x0400A1A6 RID: 41382
		public readonly EPlatformIdentifier PlatformIdentifier;

		// Token: 0x0400A1A7 RID: 41383
		public readonly string LobbyId;
	}
}
