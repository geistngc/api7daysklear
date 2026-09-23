using System;
using System.IO;
using UnityEngine;

// Token: 0x02001422 RID: 5154
public interface IBinaryReaderOrWriter
{
	// Token: 0x170012F9 RID: 4857
	// (get) Token: 0x0600A1EC RID: 41452
	Stream BaseStream { get; }

	// Token: 0x0600A1ED RID: 41453
	bool ReadWrite(bool _value);

	// Token: 0x0600A1EE RID: 41454
	byte ReadWrite(byte _value);

	// Token: 0x0600A1EF RID: 41455
	sbyte ReadWrite(sbyte _value);

	// Token: 0x0600A1F0 RID: 41456
	char ReadWrite(char _value);

	// Token: 0x0600A1F1 RID: 41457
	short ReadWrite(short _value);

	// Token: 0x0600A1F2 RID: 41458
	ushort ReadWrite(ushort _value);

	// Token: 0x0600A1F3 RID: 41459
	int ReadWrite(int _value);

	// Token: 0x0600A1F4 RID: 41460
	uint ReadWrite(uint _value);

	// Token: 0x0600A1F5 RID: 41461
	long ReadWrite(long _value);

	// Token: 0x0600A1F6 RID: 41462
	ulong ReadWrite(ulong _value);

	// Token: 0x0600A1F7 RID: 41463
	float ReadWrite(float _value);

	// Token: 0x0600A1F8 RID: 41464
	double ReadWrite(double _value);

	// Token: 0x0600A1F9 RID: 41465
	decimal ReadWrite(decimal _value);

	// Token: 0x0600A1FA RID: 41466
	string ReadWrite(string _value);

	// Token: 0x0600A1FB RID: 41467
	void ReadWrite(byte[] _buffer, int _index, int _count);

	// Token: 0x0600A1FC RID: 41468
	Vector3 ReadWrite(Vector3 _value);
}
