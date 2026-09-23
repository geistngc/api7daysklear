using System;
using UnityEngine;

// Token: 0x020004AA RID: 1194
public abstract class EntityEnemy : EntityAlive
{
	// Token: 0x0600259A RID: 9626 RVA: 0x000DF26C File Offset: 0x000DD46C
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000E5BD7 File Offset: 0x000E3DD7
	public override void PostInit()
	{
		base.PostInit();
		if (!this.isEntityRemote)
		{
			this.IsBloodMoon = this.world.aiDirector.BloodMoonComponent.BloodMoonActive;
		}
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsDrawMapIcon()
	{
		return true;
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000E5C02 File Offset: 0x000E3E02
	public override Vector3 GetMapIconScale()
	{
		return new Vector3(0.75f, 0.75f, 1f);
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000E5C18 File Offset: 0x000E3E18
	public override bool IsSavedToFile()
	{
		return (base.GetSpawnerSource() != EnumSpawnerSource.Dynamic || this.IsDead()) && base.IsSavedToFile();
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000E5C33 File Offset: 0x000E3E33
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool canDespawn()
	{
		return (!this.IsHordeZombie || this.world.GetPlayers().Count == 0) && base.canDespawn();
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x000E5C57 File Offset: 0x000E3E57
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityTargeted(EntityAlive target)
	{
		base.OnEntityTargeted(target);
		if (!this.isEntityRemote && base.GetSpawnerSource() != EnumSpawnerSource.Dynamic && target is EntityPlayer)
		{
			this.world.aiDirector.NotifyIntentToAttack(this, target as EntityPlayer);
		}
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x000D9FB0 File Offset: 0x000D81B0
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale)
	{
		return base.DamageEntity(_damageSource, _strength, _criticalHit, _impulseScale);
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x000DB788 File Offset: 0x000D9988
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityEnemy()
	{
	}

	// Token: 0x04001BF9 RID: 7161
	public bool IsHordeZombie;
}
