using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004F7 RID: 1271
[Preserve]
public class EntityZombieDog : EntityEnemyAnimal
{
	// Token: 0x0600299C RID: 10652 RVA: 0x00106550 File Offset: 0x00104750
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		Transform transform = base.transform.Find("Graphics/BlobShadowProjector");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x00106588 File Offset: 0x00104788
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		this.timeToDie = this.world.worldTime + 1800UL + (ulong)(22000f * this.rand.RandomFloat);
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x001065BE File Offset: 0x001047BE
	public override void OnUpdateLive()
	{
		base.OnUpdateLive();
		if (this.world.worldTime >= this.timeToDie && !this.isEntityRemote)
		{
			this.Kill(DamageResponse.New(true));
		}
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x001065ED File Offset: 0x001047ED
	[PublicizedFrom(EAccessModifier.Protected)]
	public override int GetMaxAttackTime()
	{
		return 30;
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x001065F1 File Offset: 0x001047F1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityTargeted(EntityAlive target)
	{
		base.OnEntityTargeted(target);
	}

	// Token: 0x04001F96 RID: 8086
	public ulong timeToDie;
}
