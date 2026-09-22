using System;
using System.IO;

// Token: 0x02000B1F RID: 2847
public class ChunkCustomData
{
	// Token: 0x06005610 RID: 22032 RVA: 0x0000640C File Offset: 0x0000460C
	public ChunkCustomData()
	{
	}

	// Token: 0x06005611 RID: 22033 RVA: 0x0020EEF5 File Offset: 0x0020D0F5
	public ChunkCustomData(string _key, ulong _expiresInWorldTime, bool _isSavedToNetwork)
	{
		this.key = _key;
		this.expiresInWorldTime = _expiresInWorldTime;
		this.isSavedToNetwork = _isSavedToNetwork;
	}

	// Token: 0x06005612 RID: 22034 RVA: 0x0020EF14 File Offset: 0x0020D114
	public void Read(BinaryReader _br)
	{
		this.key = _br.ReadString();
		this.expiresInWorldTime = _br.ReadUInt64();
		this.isSavedToNetwork = _br.ReadBoolean();
		int num = (int)_br.ReadUInt16();
		if (num > 0)
		{
			this.data = _br.ReadBytes(num);
			return;
		}
		this.data = null;
	}

	// Token: 0x06005613 RID: 22035 RVA: 0x0020EF68 File Offset: 0x0020D168
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.key);
		_bw.Write(this.expiresInWorldTime);
		_bw.Write(this.isSavedToNetwork);
		if (this.TriggerWriteDataDelegate != null)
		{
			this.TriggerWriteDataDelegate();
		}
		_bw.Write((ushort)((this.data != null) ? this.data.Length : 0));
		if (this.data != null && this.data.Length != 0)
		{
			_bw.Write(this.data);
		}
	}

	// Token: 0x06005614 RID: 22036 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnRemove(Chunk chunk)
	{
	}

	// Token: 0x04004288 RID: 17032
	public string key;

	// Token: 0x04004289 RID: 17033
	public ulong expiresInWorldTime;

	// Token: 0x0400428A RID: 17034
	public bool isSavedToNetwork;

	// Token: 0x0400428B RID: 17035
	public byte[] data;

	// Token: 0x0400428C RID: 17036
	public ChunkCustomData.TriggerWriteData TriggerWriteDataDelegate;

	// Token: 0x02000B20 RID: 2848
	// (Invoke) Token: 0x06005616 RID: 22038
	public delegate void TriggerWriteData();
}
