using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

// Token: 0x02000546 RID: 1350
public struct ExplosionData
{
	// Token: 0x06002C53 RID: 11347 RVA: 0x00118CF8 File Offset: 0x00116EF8
	public ExplosionData(byte[] _explosionDataAsArr)
	{
		this.ParticleIndex = 0;
		this.Duration = 0f;
		this.BlockRadius = 0f;
		this.EntityRadius = 0;
		this.EntityDamage = 0f;
		this.BlockDamage = 0f;
		this.BlockTags = string.Empty;
		this.BlastPower = 100;
		this.damageMultiplier = null;
		this.BuffActions = null;
		this.IgnoreHeatMap = false;
		this.DamageType = EnumDamageTypes.Heat;
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			pooledBinaryReader.SetBaseStream(new MemoryStream(_explosionDataAsArr));
			this.Read(pooledBinaryReader);
		}
	}

	// Token: 0x06002C54 RID: 11348 RVA: 0x00118DAC File Offset: 0x00116FAC
	public ExplosionData(DynamicProperties _properties, MinEffectController _effects = null)
	{
		this.ParticleIndex = 0;
		this.Duration = 0f;
		this.BlockRadius = 0f;
		this.EntityRadius = 0;
		this.EntityDamage = 0f;
		this.BlockDamage = 0f;
		this.BlockTags = string.Empty;
		this.BlastPower = 100;
		this.damageMultiplier = null;
		this.BuffActions = null;
		this.IgnoreHeatMap = false;
		this.DamageType = EnumDamageTypes.Heat;
		if (!_properties.Classes.ContainsKey("Explosion"))
		{
			return;
		}
		DynamicProperties dynamicProperties = _properties.Classes["Explosion"];
		this.ParticleIndex = 0;
		dynamicProperties.ParseInt("ParticleIndex", ref this.ParticleIndex);
		this.Duration = 0f;
		dynamicProperties.ParseFloat("Duration", ref this.Duration);
		this.BlockRadius = 1f;
		dynamicProperties.ParseFloat("RadiusBlocks", ref this.BlockRadius);
		if (dynamicProperties.Values.ContainsKey("BlockDamage"))
		{
			this.BlockDamage = StringParsers.ParseFloat(dynamicProperties.Values["BlockDamage"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.BlockDamage = this.BlockRadius * this.BlockRadius;
		}
		this.BlockTags = string.Empty;
		dynamicProperties.ParseString("BlockTags", ref this.BlockTags);
		this.EntityRadius = 0;
		if (dynamicProperties.Values.ContainsKey("RadiusEntities"))
		{
			this.EntityRadius = (int)StringParsers.ParseFloat(dynamicProperties.Values["RadiusEntities"], 0, -1, NumberStyles.Any);
		}
		if (dynamicProperties.Values.ContainsKey("EntityDamage"))
		{
			this.EntityDamage = StringParsers.ParseFloat(dynamicProperties.Values["EntityDamage"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.EntityDamage = 20f * (float)this.EntityRadius;
		}
		this.BlastPower = 100;
		if (dynamicProperties.Values.ContainsKey("BlastPower"))
		{
			this.BlastPower = (int)StringParsers.ParseFloat(dynamicProperties.Values["BlastPower"], 0, -1, NumberStyles.Any);
		}
		this.BuffActions = null;
		dynamicProperties.ParseFloat("RadiusBlocks", ref this.BlockRadius);
		if (dynamicProperties.Values.ContainsKey("Buff"))
		{
			string[] array = dynamicProperties.Values["Buff"].Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				if (this.BuffActions == null)
				{
					this.BuffActions = new List<string>();
				}
				this.BuffActions.Add(array[i]);
			}
		}
		this.damageMultiplier = new DamageMultiplier(dynamicProperties);
		this.IgnoreHeatMap = false;
		dynamicProperties.ParseBool("IgnoreHeatMap", ref this.IgnoreHeatMap);
		this.DamageType = EnumDamageTypes.Heat;
		dynamicProperties.ParseEnum<EnumDamageTypes>("DamageType", ref this.DamageType);
		if (_effects != null)
		{
			MinEffectGroup minEffectGroup = new MinEffectGroup
			{
				OwnerTiered = false
			};
			if (!_effects.PassivesIndex.Contains(PassiveEffects.ExplosionBlockDamage))
			{
				PassiveEffect pe = PassiveEffect.CreateEmptyPassiveEffect(PassiveEffects.ExplosionBlockDamage);
				MinEffectGroup.AddPassiveEffectToGroup(minEffectGroup, pe);
			}
			if (!_effects.PassivesIndex.Contains(PassiveEffects.ExplosionEntityDamage))
			{
				PassiveEffect pe2 = PassiveEffect.CreateEmptyPassiveEffect(PassiveEffects.ExplosionEntityDamage);
				MinEffectGroup.AddPassiveEffectToGroup(minEffectGroup, pe2);
			}
			if (minEffectGroup.PassiveEffects.Count > 0)
			{
				_effects.AddEffectGroup(minEffectGroup, 0, false);
			}
		}
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x001190D4 File Offset: 0x001172D4
	public byte[] ToByteArray()
	{
		MemoryStream memoryStream = new MemoryStream();
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(memoryStream);
			this.Write(pooledBinaryWriter);
		}
		return memoryStream.ToArray();
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x00119124 File Offset: 0x00117324
	public void Read(BinaryReader _br)
	{
		this.ParticleIndex = (int)_br.ReadInt16();
		this.Duration = (float)_br.ReadInt16() * 0.1f;
		this.BlockRadius = (float)_br.ReadInt16() * 0.05f;
		this.EntityRadius = (int)_br.ReadInt16();
		this.BlastPower = (int)_br.ReadInt16();
		this.BlockDamage = _br.ReadSingle();
		this.EntityDamage = _br.ReadSingle();
		this.BlockTags = _br.ReadString();
		this.IgnoreHeatMap = _br.ReadBoolean();
		this.DamageType = (EnumDamageTypes)_br.ReadInt16();
		this.damageMultiplier = new DamageMultiplier();
		this.damageMultiplier.Read(_br);
		int num = (int)_br.ReadByte();
		if (num > 0)
		{
			this.BuffActions = new List<string>();
			for (int i = 0; i < num; i++)
			{
				this.BuffActions.Add(_br.ReadString());
			}
			return;
		}
		this.BuffActions = null;
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x0011920C File Offset: 0x0011740C
	public void Write(BinaryWriter _bw)
	{
		_bw.Write((short)this.ParticleIndex);
		_bw.Write((short)(this.Duration * 10f));
		_bw.Write((short)(this.BlockRadius * 20f));
		_bw.Write((short)this.EntityRadius);
		_bw.Write((short)this.BlastPower);
		_bw.Write(this.BlockDamage);
		_bw.Write(this.EntityDamage);
		_bw.Write(this.BlockTags);
		_bw.Write(this.IgnoreHeatMap);
		_bw.Write((short)this.DamageType);
		this.damageMultiplier.Write(_bw);
		if (this.BuffActions != null)
		{
			_bw.Write((byte)this.BuffActions.Count);
			for (int i = 0; i < this.BuffActions.Count; i++)
			{
				_bw.Write(this.BuffActions[i]);
			}
			return;
		}
		_bw.Write(0);
	}

	// Token: 0x040021FE RID: 8702
	public const string PropExplosion = "Explosion";

	// Token: 0x040021FF RID: 8703
	public const string PropParticleIndex = "ParticleIndex";

	// Token: 0x04002200 RID: 8704
	public const string PropDuration = "Duration";

	// Token: 0x04002201 RID: 8705
	public const string PropRadiusBlocks = "RadiusBlocks";

	// Token: 0x04002202 RID: 8706
	public const string PropRadiusEntities = "RadiusEntities";

	// Token: 0x04002203 RID: 8707
	public const string PropBlockDamage = "BlockDamage";

	// Token: 0x04002204 RID: 8708
	public const string PropEntityDamage = "EntityDamage";

	// Token: 0x04002205 RID: 8709
	public const string PropBlockTags = "BlockTags";

	// Token: 0x04002206 RID: 8710
	public const string PropBlastPower = "BlastPower";

	// Token: 0x04002207 RID: 8711
	public const string PropBuff = "Buff";

	// Token: 0x04002208 RID: 8712
	public const string PropIgnoreHeatMap = "IgnoreHeatMap";

	// Token: 0x04002209 RID: 8713
	public const string PropDamageType = "DamageType";

	// Token: 0x0400220A RID: 8714
	public const int cMaxBlastPower = 100;

	// Token: 0x0400220B RID: 8715
	public int ParticleIndex;

	// Token: 0x0400220C RID: 8716
	public float Duration;

	// Token: 0x0400220D RID: 8717
	public float BlockRadius;

	// Token: 0x0400220E RID: 8718
	public int EntityRadius;

	// Token: 0x0400220F RID: 8719
	public int BlastPower;

	// Token: 0x04002210 RID: 8720
	public float EntityDamage;

	// Token: 0x04002211 RID: 8721
	public float BlockDamage;

	// Token: 0x04002212 RID: 8722
	public string BlockTags;

	// Token: 0x04002213 RID: 8723
	public bool IgnoreHeatMap;

	// Token: 0x04002214 RID: 8724
	public EnumDamageTypes DamageType;

	// Token: 0x04002215 RID: 8725
	public DamageMultiplier damageMultiplier;

	// Token: 0x04002216 RID: 8726
	public List<string> BuffActions;
}
