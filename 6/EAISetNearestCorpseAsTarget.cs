using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000455 RID: 1109
[Preserve]
public class EAISetNearestCorpseAsTarget : EAITarget
{
	// Token: 0x060021D3 RID: 8659 RVA: 0x000CC7D4 File Offset: 0x000CA9D4
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity, 50f, true);
		this.executeDelay = 0.8f;
		this.rndTimeout = 0;
		this.MutexBits = 1;
		this.sorter = new EAISetNearestEntityAsTargetSorter(_theEntity);
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x000CC808 File Offset: 0x000CAA08
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		string names;
		if (data.TryGetValue("flags", out names))
		{
			EntityClass.ParseEntityFlags(names, ref this.targetFlags);
		}
		base.GetData(data, "maxDistance2d", ref this.maxXZDistance);
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000CC84C File Offset: 0x000CAA4C
	public override bool CanExecute()
	{
		if (this.theEntity.HasInvestigatePosition)
		{
			return false;
		}
		if (this.theEntity.IsSleeping)
		{
			return false;
		}
		if (this.rndTimeout > 0 && base.GetRandom(this.rndTimeout) != 0)
		{
			return false;
		}
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (attackTarget is EntityPlayer && attackTarget.IsAlive() && base.RandomFloat < 0.95f)
		{
			return false;
		}
		float radius = this.theEntity.IsSleeper ? 7f : this.maxXZDistance;
		this.theEntity.world.GetEntitiesAround(this.targetFlags, this.targetFlags, this.theEntity.position, radius, EAISetNearestCorpseAsTarget.entityList);
		EAISetNearestCorpseAsTarget.entityList.Sort(this.sorter);
		EntityAlive entityAlive = null;
		for (int i = 0; i < EAISetNearestCorpseAsTarget.entityList.Count; i++)
		{
			EntityAlive entityAlive2 = EAISetNearestCorpseAsTarget.entityList[i] as EntityAlive;
			if (entityAlive2 && entityAlive2.IsDead() && (EAISetNearestCorpseAsTarget.ZombiesEatAnimalCorpses || (!(entityAlive2 is EntityAnimal) && !(entityAlive2 is EntityEnemyAnimal))))
			{
				entityAlive = entityAlive2;
				break;
			}
		}
		EAISetNearestCorpseAsTarget.entityList.Clear();
		this.targetEntity = entityAlive;
		return this.targetEntity != null;
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000CC989 File Offset: 0x000CAB89
	public override void Start()
	{
		base.Start();
		this.theEntity.SetAttackTarget(this.targetEntity, 600);
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000CC9A7 File Offset: 0x000CABA7
	public override bool Continue()
	{
		return this.targetEntity && this.targetEntity.IsDead() && !(this.targetEntity != this.theEntity.GetAttackTarget());
	}

	// Token: 0x0400175A RID: 5978
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive targetEntity;

	// Token: 0x0400175B RID: 5979
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityFlags targetFlags;

	// Token: 0x0400175C RID: 5980
	[PublicizedFrom(EAccessModifier.Private)]
	public int rndTimeout;

	// Token: 0x0400175D RID: 5981
	[PublicizedFrom(EAccessModifier.Private)]
	public EAISetNearestEntityAsTargetSorter sorter;

	// Token: 0x0400175E RID: 5982
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> entityList = new List<Entity>();

	// Token: 0x0400175F RID: 5983
	public static bool ZombiesEatAnimalCorpses = true;
}
