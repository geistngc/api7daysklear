using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000439 RID: 1081
[Preserve]
public class EAIDodge : EAIBase
{
	// Token: 0x06002123 RID: 8483 RVA: 0x000C8117 File Offset: 0x000C6317
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.executeDelay = 0.1f;
		this.cooldown = 3f;
		this.actionkDuration = 1f;
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x000C8144 File Offset: 0x000C6344
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		string str;
		if (data.TryGetValue("tags", out str))
		{
			this.tags = FastTags<TagGroup.Global>.Parse(str);
		}
		base.GetData(data, "maxXZDistance", ref this.maxXZDistance);
		base.GetData(data, "cooldown", ref this.baseCooldown);
		base.GetData(data, "duration", ref this.actionkDuration);
		base.GetData(data, "minRange", ref this.minRange);
		base.GetData(data, "maxRange", ref this.maxRange);
		base.GetData(data, "unreachableRange", ref this.unreachableRange);
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x000C81E0 File Offset: 0x000C63E0
	public override bool CanExecute()
	{
		if (this.theEntity.IsDancing)
		{
			return false;
		}
		if (this.cooldown > 0f)
		{
			this.cooldown -= this.executeWaitTime;
			return false;
		}
		this.theEntity.world.GetEntitiesInBounds(this.tags, BoundsUtils.ExpandBounds(this.theEntity.boundingBox, this.maxXZDistance, 8f, this.maxXZDistance), EAIDodge.entityList);
		this.entityTarget = null;
		for (int i = 0; i < EAIDodge.entityList.Count; i++)
		{
			EntityAlive entityAlive = EAIDodge.entityList[i] as EntityAlive;
			if (entityAlive && !entityAlive.IsDead() && entityAlive.emodel.avatarController.IsAnimationToDodge())
			{
				this.entityTarget = entityAlive;
				break;
			}
		}
		EAIDodge.entityList.Clear();
		return !(this.entityTarget == null) && this.InRange() && this.theEntity.CanSee(this.entityTarget);
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000C82E7 File Offset: 0x000C64E7
	public override void Start()
	{
		this.actionTime = 0f;
		this.theEntity.emodel.avatarController.StartAnimationDodge(base.Random.RandomFloat);
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000C8314 File Offset: 0x000C6514
	public override bool Continue()
	{
		return this.entityTarget && this.entityTarget.IsAlive() && this.actionTime < this.actionkDuration && this.theEntity.hasBeenAttackedTime <= 0;
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000C8354 File Offset: 0x000C6554
	public override void Update()
	{
		this.actionTime += 0.05f;
		if (this.actionTime < this.actionkDuration * 0.5f)
		{
			Vector3 headPosition = this.entityTarget.getHeadPosition();
			if (this.theEntity.IsInFrontOfMe(headPosition))
			{
				this.theEntity.SetLookPosition(headPosition);
			}
		}
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000027FC File Offset: 0x000009FC
	public override void Reset()
	{
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000C83B0 File Offset: 0x000C65B0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool InRange()
	{
		float distanceSq = this.entityTarget.GetDistanceSq(this.theEntity);
		return distanceSq >= this.minRange * this.minRange && distanceSq <= this.maxRange * this.maxRange;
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000C83F4 File Offset: 0x000C65F4
	public override string ToString()
	{
		bool flag = this.entityTarget && this.InRange();
		return string.Format("{0} {1}, inRange{2}, Time {3}", new object[]
		{
			base.ToString(),
			this.entityTarget ? this.entityTarget.EntityName : "",
			flag,
			this.actionTime.ToCultureInvariantString("0.00")
		});
	}

	// Token: 0x040016CA RID: 5834
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tags;

	// Token: 0x040016CB RID: 5835
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxXZDistance = 100f;

	// Token: 0x040016CC RID: 5836
	[PublicizedFrom(EAccessModifier.Private)]
	public float baseCooldown;

	// Token: 0x040016CD RID: 5837
	[PublicizedFrom(EAccessModifier.Private)]
	public float cooldown;

	// Token: 0x040016CE RID: 5838
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entityTarget;

	// Token: 0x040016CF RID: 5839
	[PublicizedFrom(EAccessModifier.Private)]
	public float actionTime;

	// Token: 0x040016D0 RID: 5840
	[PublicizedFrom(EAccessModifier.Private)]
	public float actionkDuration;

	// Token: 0x040016D1 RID: 5841
	[PublicizedFrom(EAccessModifier.Private)]
	public float minRange = 4f;

	// Token: 0x040016D2 RID: 5842
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxRange = 25f;

	// Token: 0x040016D3 RID: 5843
	[PublicizedFrom(EAccessModifier.Private)]
	public float unreachableRange;

	// Token: 0x040016D4 RID: 5844
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> entityList = new List<Entity>();
}
