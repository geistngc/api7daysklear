using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

// Token: 0x0200053D RID: 1341
public class DamageMultiplier
{
	// Token: 0x06002C26 RID: 11302 RVA: 0x001173B5 File Offset: 0x001155B5
	public DamageMultiplier()
	{
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x001173C8 File Offset: 0x001155C8
	public DamageMultiplier(DynamicProperties _properties)
	{
		if (_properties.Classes.ContainsKey("DamageBonus"))
		{
			foreach (KeyValuePair<string, string> keyValuePair in _properties.Classes["DamageBonus"].Values)
			{
				float value = StringParsers.ParseFloat(keyValuePair.Value, 0, -1, NumberStyles.Any);
				this.addMultiplier(keyValuePair.Key, value);
			}
		}
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x00117468 File Offset: 0x00115668
	[PublicizedFrom(EAccessModifier.Private)]
	public void addMultiplier(string _name, float _value)
	{
		this.damageMultiplier[_name] = _value;
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x00117477 File Offset: 0x00115677
	public float Get(string _group)
	{
		if (_group == null || this.damageMultiplier == null || !this.damageMultiplier.ContainsKey(_group))
		{
			return 1f;
		}
		return this.damageMultiplier[_group];
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x001174A4 File Offset: 0x001156A4
	public void Read(BinaryReader _br)
	{
		this.damageMultiplier.Clear();
		int num = (int)_br.ReadInt16();
		for (int i = 0; i < num; i++)
		{
			string key = _br.ReadString();
			float value = _br.ReadSingle();
			this.damageMultiplier.Add(key, value);
		}
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x001174EC File Offset: 0x001156EC
	public void Write(BinaryWriter _bw)
	{
		_bw.Write((short)this.damageMultiplier.Count);
		foreach (KeyValuePair<string, float> keyValuePair in this.damageMultiplier)
		{
			_bw.Write(keyValuePair.Key);
			_bw.Write(keyValuePair.Value);
		}
	}

	// Token: 0x040021C3 RID: 8643
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, float> damageMultiplier = new Dictionary<string, float>();
}
