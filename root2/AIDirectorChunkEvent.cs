using System;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x0200040D RID: 1037
[Preserve]
public class AIDirectorChunkEvent
{
	// Token: 0x06002018 RID: 8216 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorChunkEvent()
	{
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000C2928 File Offset: 0x000C0B28
	public AIDirectorChunkEvent(EnumAIDirectorChunkEvent _type, Vector3i _position, float _value, float _duration)
	{
		this.EventType = _type;
		this.Position = _position;
		this.Value = _value;
		this.Duration = _duration;
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000C2950 File Offset: 0x000C0B50
	public void Write(BinaryWriter _stream)
	{
		_stream.Write(2);
		_stream.Write(this.Position.x);
		_stream.Write(this.Position.y);
		_stream.Write(this.Position.z);
		_stream.Write(this.Value);
		_stream.Write((byte)this.EventType);
		_stream.Write(this.Duration);
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000C29BC File Offset: 0x000C0BBC
	public static AIDirectorChunkEvent Read(BinaryReader _stream)
	{
		int num = _stream.ReadInt32();
		AIDirectorChunkEvent aidirectorChunkEvent = new AIDirectorChunkEvent();
		aidirectorChunkEvent.Position.x = _stream.ReadInt32();
		aidirectorChunkEvent.Position.y = _stream.ReadInt32();
		aidirectorChunkEvent.Position.z = _stream.ReadInt32();
		aidirectorChunkEvent.Value = _stream.ReadSingle();
		aidirectorChunkEvent.EventType = (EnumAIDirectorChunkEvent)_stream.ReadByte();
		if (num >= 2)
		{
			aidirectorChunkEvent.Duration = _stream.ReadSingle();
		}
		else
		{
			_stream.ReadUInt64();
		}
		return aidirectorChunkEvent;
	}

	// Token: 0x040015D5 RID: 5589
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 2;

	// Token: 0x040015D6 RID: 5590
	public EnumAIDirectorChunkEvent EventType;

	// Token: 0x040015D7 RID: 5591
	public Vector3i Position;

	// Token: 0x040015D8 RID: 5592
	public float Value;

	// Token: 0x040015D9 RID: 5593
	public float Duration;
}
