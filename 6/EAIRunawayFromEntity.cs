using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000451 RID: 1105
[Preserve]
public class EAIRunawayFromEntity : EAIRunAway
{
	// Token: 0x060021BA RID: 8634 RVA: 0x000C7418 File Offset: 0x000C5618
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 1;
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x000CC01C File Offset: 0x000CA21C
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		string names;
		if (data.TryGetValue("flags", out names))
		{
			EntityClass.ParseEntityFlags(names, ref this.flags);
		}
		if (data.TryGetValue("safeFlags", out names))
		{
			EntityClass.ParseEntityFlags(names, ref this.safeFlags);
		}
		base.GetData(data, "safeDistance", ref this.safeDistance);
		base.GetData(data, "dangerDistance", ref this.dangerDistance);
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000CC08A File Offset: 0x000CA28A
	public override bool CanExecute()
	{
		this.FindEnemy();
		return this.enemy && base.CanExecute();
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000CC0A8 File Offset: 0x000CA2A8
	public void FindEnemy()
	{
		this.enemy = null;
		if (this.theEntity.noisePlayer && this.theEntity.noisePlayerVolume >= 8f)
		{
			this.enemy = this.theEntity.noisePlayer;
			return;
		}
		float radius = Utils.FastMin(this.theEntity.GetSeeDistance(), this.safeDistance) * 0.8f;
		this.theEntity.world.GetEntitiesAround(EntityFlags.None, this.safeFlags, this.theEntity.position, radius, EAIRunawayFromEntity.entityList);
		float num = float.MaxValue;
		for (int i = 0; i < EAIRunawayFromEntity.entityList.Count; i++)
		{
			Entity entity = EAIRunawayFromEntity.entityList[i];
			if ((entity.entityFlags & this.flags) != EntityFlags.None)
			{
				EntityPlayer entityPlayer = entity as EntityPlayer;
				if (entityPlayer != null)
				{
					float seeDistance = this.manager.GetSeeDistance(entityPlayer);
					float num2 = seeDistance * seeDistance;
					if (num2 < num && this.theEntity.CanSee(entityPlayer) && this.theEntity.CanSeeStealth(seeDistance, entityPlayer.Stealth.lightLevel) && !entityPlayer.IsIgnoredByAI())
					{
						num = num2;
						this.enemy = entityPlayer;
					}
				}
				else
				{
					EntityAlive entityAlive = entity as EntityAlive;
					if (entityAlive != null)
					{
						float distanceSq = this.theEntity.GetDistanceSq(entityAlive);
						if (distanceSq < num && (distanceSq <= this.dangerDistance * this.dangerDistance || (this.theEntity.CanSee(entityAlive) && !entityAlive.IsIgnoredByAI())))
						{
							num = distanceSq;
							this.enemy = entityAlive;
						}
					}
				}
			}
		}
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000CC237 File Offset: 0x000CA437
	public override bool Continue()
	{
		return this.theEntity.GetDistanceSq(this.enemy) < this.safeDistance * this.safeDistance && base.Continue();
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x000CC261 File Offset: 0x000CA461
	public override void Update()
	{
		base.Update();
		this.theEntity.navigator.setMoveSpeed(this.theEntity.IsSwimming() ? this.theEntity.GetMoveSpeed() : this.theEntity.GetMoveSpeedPanic());
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x000CC29E File Offset: 0x000CA49E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 GetFleeFromPos()
	{
		return this.enemy.position;
	}

	// Token: 0x0400174F RID: 5967
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRunNoiseVolume = 8;

	// Token: 0x04001750 RID: 5968
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityFlags flags;

	// Token: 0x04001751 RID: 5969
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityFlags safeFlags;

	// Token: 0x04001752 RID: 5970
	[PublicizedFrom(EAccessModifier.Private)]
	public float safeDistance = 38f;

	// Token: 0x04001753 RID: 5971
	[PublicizedFrom(EAccessModifier.Private)]
	public float dangerDistance = 4.5f;

	// Token: 0x04001754 RID: 5972
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Entity> entityList = new List<Entity>();
}
