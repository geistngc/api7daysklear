using System;
using System.IO;

// Token: 0x020000CA RID: 202
public struct AnimParamData
{
	// Token: 0x060004B0 RID: 1200 RVA: 0x00020EA3 File Offset: 0x0001F0A3
	public AnimParamData(int _nameHash, AnimParamData.ValueTypes _valueType, bool _value)
	{
		this.NameHash = _nameHash;
		this.ValueType = _valueType;
		this.FloatValue = 0f;
		this.IntValue = (_value ? 1 : 0);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00020ECB File Offset: 0x0001F0CB
	public AnimParamData(int _nameHash, AnimParamData.ValueTypes _valueType, float _value)
	{
		this.NameHash = _nameHash;
		this.ValueType = _valueType;
		this.FloatValue = _value;
		this.IntValue = 0;
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x00020EE9 File Offset: 0x0001F0E9
	public AnimParamData(int _nameHash, AnimParamData.ValueTypes _valueType, int _value)
	{
		this.NameHash = _nameHash;
		this.ValueType = _valueType;
		this.FloatValue = 0f;
		this.IntValue = _value;
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x00020F0C File Offset: 0x0001F10C
	public static AnimParamData CreateFromBinary(BinaryReader _br)
	{
		int nameHash = _br.ReadInt32();
		AnimParamData.ValueTypes valueTypes = (AnimParamData.ValueTypes)_br.ReadByte();
		switch (valueTypes)
		{
		case AnimParamData.ValueTypes.Bool:
		case AnimParamData.ValueTypes.Trigger:
			return new AnimParamData(nameHash, valueTypes, _br.ReadBoolean());
		case AnimParamData.ValueTypes.Float:
		case AnimParamData.ValueTypes.DataFloat:
			return new AnimParamData(nameHash, valueTypes, _br.ReadSingle());
		case AnimParamData.ValueTypes.Int:
			return new AnimParamData(nameHash, valueTypes, _br.ReadInt32());
		default:
		{
			string str = "Invalid Value Type: ";
			byte b = (byte)valueTypes;
			throw new InvalidDataException(str + b.ToString());
		}
		}
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x00020F88 File Offset: 0x0001F188
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.NameHash);
		_bw.Write((byte)this.ValueType);
		switch (this.ValueType)
		{
		case AnimParamData.ValueTypes.Bool:
		case AnimParamData.ValueTypes.Trigger:
			_bw.Write(this.IntValue != 0);
			return;
		case AnimParamData.ValueTypes.Float:
		case AnimParamData.ValueTypes.DataFloat:
			_bw.Write(this.FloatValue);
			return;
		case AnimParamData.ValueTypes.Int:
			_bw.Write(this.IntValue);
			return;
		default:
			return;
		}
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x00020FF8 File Offset: 0x0001F1F8
	public string ToString(AvatarController _controller)
	{
		string parameterName = _controller.GetParameterName(this.NameHash);
		return string.Format("{0} {1}, {2}, f{3}, i{4}", new object[]
		{
			parameterName,
			this.NameHash,
			this.ValueType,
			this.FloatValue,
			this.IntValue
		});
	}

	// Token: 0x04000548 RID: 1352
	public readonly int NameHash;

	// Token: 0x04000549 RID: 1353
	public readonly AnimParamData.ValueTypes ValueType;

	// Token: 0x0400054A RID: 1354
	public readonly float FloatValue;

	// Token: 0x0400054B RID: 1355
	public readonly int IntValue;

	// Token: 0x020000CB RID: 203
	public enum ValueTypes : byte
	{
		// Token: 0x0400054D RID: 1357
		Bool,
		// Token: 0x0400054E RID: 1358
		Trigger,
		// Token: 0x0400054F RID: 1359
		Float,
		// Token: 0x04000550 RID: 1360
		Int,
		// Token: 0x04000551 RID: 1361
		DataFloat
	}
}
