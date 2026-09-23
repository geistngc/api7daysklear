using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using SandboxOptions;

// Token: 0x020001CE RID: 462
public class EntityStats
{
	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000E47 RID: 3655 RVA: 0x0005E0CB File Offset: 0x0005C2CB
	public float AmountEnclosed
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_amountEnclosed;
		}
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0005E0D3 File Offset: 0x0005C2D3
	public EntityStats()
	{
		this.Health = new Stat();
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x0005E0EE File Offset: 0x0005C2EE
	public EntityStats(EntityAlive _ea)
	{
		this.m_entity = _ea;
		this.Init();
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x0005E10C File Offset: 0x0005C30C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Init()
	{
		int num = (int)EffectManager.GetValue(PassiveEffects.HealthMax, null, 100f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Health = new Stat(Stat.StatTypes.Health, this.m_entity, (float)num, (float)num)
		{
			MaxPassive = PassiveEffects.HealthMax,
			GainPassive = PassiveEffects.HealthGain,
			LossPassive = PassiveEffects.HealthLoss
		};
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x0005E16E File Offset: 0x0005C36E
	public virtual void CopyFrom(EntityStats _newStats)
	{
		this.Health.CopyFrom(_newStats.Health);
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x0005E181 File Offset: 0x0005C381
	public virtual EntityStats SimpleClone()
	{
		EntityStats entityStats = new EntityStats();
		entityStats.Health.CopyFrom(this.Health);
		return entityStats;
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void EntityBuffAdded(BuffValue _buff)
	{
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void EntityBuffRemoved(BuffValue _buff)
	{
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x0005E19C File Offset: 0x0005C39C
	public void Tick(ulong worldTime)
	{
		if (this.m_entity.isEntityRemote || this.m_entity.IsDead())
		{
			return;
		}
		int num = this.waitTicks + 1;
		this.waitTicks = num;
		if (num >= 10)
		{
			this.waitTicks = 0;
		}
		this.TickWait(worldTime);
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x0005E1E8 File Offset: 0x0005C3E8
	public virtual void TickWait(ulong worldTime)
	{
		float dt = 0.5f;
		if (this.waitTicks == 1)
		{
			this.UpdateNPCStatsOverTime(dt);
			this.Health.Tick(dt);
		}
		if (this.waitTicks == 2 && this.Health.Changed)
		{
			this.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Health);
			this.Health.Changed = false;
		}
		if (this.waitTicks == 6)
		{
			if (this.netSyncWaitTicks > 0)
			{
				this.netSyncWaitTicks--;
				return;
			}
			this.netSyncWaitTicks = 10;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStatsBuff>().Setup(this.m_entity, null), false, -1, -1, -1, null, 192, false);
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityStatsBuff>().Setup(this.m_entity, null), false);
		}
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x0005E2C0 File Offset: 0x0005C4C0
	public void UpdateNPCStatsOverTime(float dt)
	{
		List<EffectManager.ModifierValuesAndSources> valuesAndSources = EffectManager.GetValuesAndSources(PassiveEffects.HealthChangeOT, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true);
		for (int i = 0; i < valuesAndSources.Count; i++)
		{
			EffectManager.ModifierValuesAndSources modifierValuesAndSources = valuesAndSources[i];
			if (modifierValuesAndSources.ParentType == MinEffectController.SourceParentType.BuffClass)
			{
				BuffValue buff = this.m_entity.Buffs.GetBuff((string)modifierValuesAndSources.Source);
				if (buff != null && buff.BuffClass != null)
				{
					BuffClass buffClass = buff.BuffClass;
					float num = 0f;
					float num2 = 1f;
					buffClass.ModifyValue(this.m_entity, PassiveEffects.HealthChangeOT, buff, ref num, ref num2, FastTags<TagGroup.Global>.none);
					float num3 = num * num2 * dt;
					if (num3 < 0f)
					{
						float num4 = -num3 + this.buffDamageRemainder;
						int num5 = (int)num4;
						this.buffDamageRemainder = num4 - (float)num5;
						if (num5 > 0)
						{
							DamageSource damageSource = new DamageSource(buffClass.DamageSource, buffClass.DamageType);
							damageSource.BuffClass = buffClass;
							this.m_entity.DamageEntity(damageSource, num5, false, 0f);
						}
					}
					else if (num3 > 0f)
					{
						this.Health.Value += num3;
					}
				}
			}
			else
			{
				this.Health.Value += modifierValuesAndSources.Value * dt;
			}
		}
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x000027FC File Offset: 0x000009FC
	public void ResetStats()
	{
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x0005E41C File Offset: 0x0005C61C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendStatChangePacket(NetPackageEntityStatChanged.EnumStat enumStat)
	{
		int num;
		if (GameManager.IsDedicatedServer)
		{
			num = -1;
		}
		else
		{
			num = this.m_entity.world.GetPrimaryPlayer().entityId;
		}
		NetPackageEntityStatChanged package = NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(this.m_entity, num, enumStat);
		if (this.m_entity.world.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		this.m_entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.m_entity.entityId, num, package, enumStat > NetPackageEntityStatChanged.EnumStat.Health);
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x0005E4A2 File Offset: 0x0005C6A2
	public virtual void Read(BinaryReader stream)
	{
		stream.ReadInt32();
		this.Health.Read(stream);
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x0005E4B7 File Offset: 0x0005C6B7
	public virtual void Write(BinaryWriter stream)
	{
		stream.Write(11);
		this.Health.Write(stream);
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x0005E4D0 File Offset: 0x0005C6D0
	public void UpdateSandboxOptions()
	{
		this.Stamina.GainSandboxModifier = SandboxOptionManager.GetFloat(SandboxOptions.StaminaRegen);
		this.Stamina.LossSandboxModifier = SandboxOptionManager.GetFloat(SandboxOptions.StaminaUsage);
		this.Food.LossSandboxModifier = SandboxOptionManager.GetFloat(SandboxOptions.HungerMultiplier);
		this.Water.LossSandboxModifier = SandboxOptionManager.GetFloat(SandboxOptions.ThirstMultiplier);
	}

	// Token: 0x04000C31 RID: 3121
	public const int cVersion = 11;

	// Token: 0x04000C32 RID: 3122
	public static bool WeatherSurvivalEnabled;

	// Token: 0x04000C33 RID: 3123
	public static bool NewWeatherSurvivalEnabled = true;

	// Token: 0x04000C34 RID: 3124
	public Stat Health;

	// Token: 0x04000C35 RID: 3125
	public Stat Stamina;

	// Token: 0x04000C36 RID: 3126
	public Stat Water;

	// Token: 0x04000C37 RID: 3127
	public Stat Food;

	// Token: 0x04000C38 RID: 3128
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAlive m_entity;

	// Token: 0x04000C39 RID: 3129
	[PublicizedFrom(EAccessModifier.Protected)]
	public float m_amountEnclosed;

	// Token: 0x04000C3A RID: 3130
	[PublicizedFrom(EAccessModifier.Private)]
	public float buffDamageRemainder;

	// Token: 0x04000C3B RID: 3131
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int cWaitTicks = 10;

	// Token: 0x04000C3C RID: 3132
	[PublicizedFrom(EAccessModifier.Protected)]
	public int waitTicks;

	// Token: 0x04000C3D RID: 3133
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int cNetSyncWaitTicks = 10;

	// Token: 0x04000C3E RID: 3134
	[PublicizedFrom(EAccessModifier.Protected)]
	public int netSyncWaitTicks = 10;
}
