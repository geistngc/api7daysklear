using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000483 RID: 1155
[Preserve]
public abstract class EntityAnimal : EntityAlive
{
	// Token: 0x060023E4 RID: 9188 RVA: 0x000D9E79 File Offset: 0x000D8079
	public void SetDistressed(bool _isDistressed, float _minTime, float _maxTime, int _playerId)
	{
		this.isDistressed = _isDistressed;
		this.minStressTime = _minTime;
		this.maxStressTime = _maxTime;
		this.playerId = _playerId;
		this.timer = 2.5f;
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000D9EA4 File Offset: 0x000D80A4
	public override void ClearDistressed()
	{
		this.isDistressed = false;
		EntityPlayerLocal entityPlayerLocal = this.getEntityPlayerLocal();
		if (entityPlayerLocal)
		{
			entityPlayerLocal.Waypoints.TryRemoveLastKnownPositionWaypoint(this.entityId);
		}
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000D9EDC File Offset: 0x000D80DC
	public override void OnUpdateLive()
	{
		base.GetEntitySenses().Clear();
		base.OnUpdateLive();
		if (this.isDistressed && base.IsAlive())
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.timer += this.rand.RandomRange(this.minStressTime, this.maxStressTime);
				GameManager.Instance.PlaySoundAtPositionServer(this.position, base.GetSoundDistressed(), AudioRolloffMode.Linear, 1, this.entityId, 1f);
			}
			EntityPlayerLocal entityPlayerLocal = this.getEntityPlayerLocal();
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.Waypoints.UpdateEntityAnimalWayPoint(this, true);
			}
		}
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsDrawMapIcon()
	{
		return false;
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000D9F93 File Offset: 0x000D8193
	public override Color GetMapIconColor()
	{
		return new Color(1f, 0.8235294f, 0.34117648f);
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000D9FA9 File Offset: 0x000D81A9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float getNextStepSoundDistance()
	{
		return 0.8f;
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000D9FB0 File Offset: 0x000D81B0
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000D9FC0 File Offset: 0x000D81C0
	public override void OnEntityDeath()
	{
		if (this.PhysicsTransform)
		{
			this.PhysicsTransform.gameObject.SetActive(false);
		}
		base.OnEntityDeath();
		EntityPlayerLocal entityPlayerLocal = this.getEntityPlayerLocal();
		if (entityPlayerLocal)
		{
			entityPlayerLocal.Waypoints.TryRemoveLastKnownPositionWaypoint(this.entityId);
		}
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000DA014 File Offset: 0x000D8214
	public override void OnEntityUnload()
	{
		base.OnEntityUnload();
		EntityPlayerLocal entityPlayerLocal = this.getEntityPlayerLocal();
		if (entityPlayerLocal)
		{
			entityPlayerLocal.Waypoints.TryRemoveLastKnownPositionWaypoint(this.entityId);
		}
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000DA048 File Offset: 0x000D8248
	public override void OnCollectServer(int _playerId)
	{
		GameManager.Instance.World.RemoveEntity(this.entityId, EnumRemoveEntityReason.Captured);
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000DA064 File Offset: 0x000D8264
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal getEntityPlayerLocal()
	{
		EntityPlayerLocal entityPlayerLocal = GameManager.Instance.World.GetPrimaryPlayer();
		if (entityPlayerLocal && entityPlayerLocal.entityId != this.playerId)
		{
			entityPlayerLocal = null;
		}
		return entityPlayerLocal;
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000DA09A File Offset: 0x000D829A
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAnimal()
	{
	}

	// Token: 0x04001986 RID: 6534
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float minStressTime = 2.5f;

	// Token: 0x04001987 RID: 6535
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float maxStressTime = 1f;

	// Token: 0x04001988 RID: 6536
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDistressed;

	// Token: 0x04001989 RID: 6537
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int playerId = -1;

	// Token: 0x0400198A RID: 6538
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timer;
}
