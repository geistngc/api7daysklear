using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000456 RID: 1110
[Preserve]
public class EAISetNearestEntityAsTarget : EAITarget
{
	// Token: 0x060021DA RID: 8666 RVA: 0x000CC9F2 File Offset: 0x000CABF2
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity, 25f, true);
		this.MutexBits = 1;
		this.sorter = new EAISetNearestEntityAsTargetSorter(_theEntity);
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x000CCA14 File Offset: 0x000CAC14
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		this.targetClasses = new List<EAISetNearestEntityAsTarget.TargetClass>();
		string text;
		if (data.TryGetValue("class", out text))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i += 3)
			{
				EAISetNearestEntityAsTarget.TargetClass targetClass;
				targetClass.type = EntityFactory.GetEntityType(array[i]);
				targetClass.hearDistMax = 0f;
				if (i + 1 < array.Length)
				{
					targetClass.hearDistMax = StringParsers.ParseFloat(array[i + 1], 0, -1, NumberStyles.Any);
				}
				if (targetClass.hearDistMax == 0f)
				{
					targetClass.hearDistMax = 50f;
				}
				targetClass.seeDistMax = 0f;
				if (i + 2 < array.Length)
				{
					targetClass.seeDistMax = StringParsers.ParseFloat(array[i + 2], 0, -1, NumberStyles.Any);
				}
				if (targetClass.type == typeof(EntityPlayer))
				{
					this.playerTargetClassIndex = this.targetClasses.Count;
				}
				this.targetClasses.Add(targetClass);
			}
		}
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x000CCB18 File Offset: 0x000CAD18
	public void SetTargetOnlyPlayers(float _distance)
	{
		this.targetClasses.Clear();
		EAISetNearestEntityAsTarget.TargetClass item = default(EAISetNearestEntityAsTarget.TargetClass);
		item.type = typeof(EntityPlayer);
		item.hearDistMax = _distance;
		item.seeDistMax = -_distance;
		this.targetClasses.Add(item);
		this.playerTargetClassIndex = 0;
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x000CCB70 File Offset: 0x000CAD70
	public override bool CanExecute()
	{
		if (this.theEntity.distraction != null)
		{
			return false;
		}
		this.FindTarget();
		if (!this.closeTargetEntity)
		{
			return false;
		}
		this.targetEntity = this.closeTargetEntity;
		this.targetPlayer = (this.closeTargetEntity as EntityPlayer);
		return true;
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x000CCBC8 File Offset: 0x000CADC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void FindTarget()
	{
		this.closeTargetDist = float.MaxValue;
		this.closeTargetEntity = null;
		float seeDistance = this.theEntity.GetSeeDistance();
		for (int i = 0; i < this.targetClasses.Count; i++)
		{
			EAISetNearestEntityAsTarget.TargetClass targetClass = this.targetClasses[i];
			float num = seeDistance;
			if (targetClass.seeDistMax != 0f)
			{
				float v = (targetClass.seeDistMax < 0f) ? (-targetClass.seeDistMax) : (targetClass.seeDistMax * this.theEntity.senseScale);
				num = Utils.FastMin(num, v);
			}
			if (targetClass.type == typeof(EntityPlayer))
			{
				this.FindTargetPlayer(num);
				if (this.theEntity.noisePlayer && this.theEntity.noisePlayer != this.closeTargetEntity)
				{
					if (this.closeTargetEntity)
					{
						if (this.theEntity.noisePlayerVolume >= this.theEntity.sleeperNoiseToWake)
						{
							Vector3 position = this.theEntity.noisePlayer.position;
							float magnitude = (this.theEntity.position - position).magnitude;
							if (magnitude < this.closeTargetDist)
							{
								this.closeTargetDist = magnitude;
								this.closeTargetEntity = this.theEntity.noisePlayer;
							}
						}
					}
					else if (!this.theEntity.IsSleeping)
					{
						this.SeekNoise(this.theEntity.noisePlayer);
					}
				}
				if (this.closeTargetEntity)
				{
					EntityPlayer entityPlayer = (EntityPlayer)this.closeTargetEntity;
					if (entityPlayer.IsBloodMoonDead && entityPlayer.currentLife >= 0.5f)
					{
						Log.Out("Player {0}, living {1}, lost BM immunity", new object[]
						{
							entityPlayer.GetDebugName(),
							entityPlayer.currentLife * 60f
						});
						entityPlayer.IsBloodMoonDead = false;
					}
				}
				if (this.theEntity.attractPlayer && !this.closeTargetEntity && !this.theEntity.IsSleeping && !this.theEntity.HasInvestigatePosition)
				{
					this.SeekBreadcrumb(this.theEntity.attractPlayer, 15f);
				}
				if (this.theEntity.smellPlayer && !this.closeTargetEntity && !this.theEntity.IsSleeping && !this.theEntity.HasInvestigatePosition && this.theEntity.smellPlayer.currentLife > 1f)
				{
					this.SeekBreadcrumb(this.theEntity.smellPlayer, 24f);
				}
			}
			else if (!this.theEntity.IsSleeping && !this.theEntity.HasInvestigatePosition)
			{
				this.theEntity.world.GetEntitiesInBounds(targetClass.type, BoundsUtils.ExpandBounds(this.theEntity.boundingBox, num, 4f, num), EAISetNearestEntityAsTarget.list);
				EAISetNearestEntityAsTarget.list.Sort(this.sorter);
				int j = 0;
				while (j < EAISetNearestEntityAsTarget.list.Count)
				{
					EntityAlive entityAlive = (EntityAlive)EAISetNearestEntityAsTarget.list[j];
					if (!(entityAlive is EntityDrone) && base.check(entityAlive))
					{
						float distance = this.theEntity.GetDistance(entityAlive);
						if (distance < this.closeTargetDist)
						{
							this.closeTargetDist = distance;
							this.closeTargetEntity = entityAlive;
							this.lastSeenPos = entityAlive.position;
							break;
						}
						break;
					}
					else
					{
						j++;
					}
				}
				EAISetNearestEntityAsTarget.list.Clear();
			}
		}
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000CCF5C File Offset: 0x000CB15C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SeekNoise(EntityPlayer player)
	{
		float num = (player.position - this.theEntity.position).magnitude;
		if (this.playerTargetClassIndex >= 0)
		{
			float num2 = this.targetClasses[this.playerTargetClassIndex].hearDistMax;
			num2 *= this.theEntity.senseScale;
			num2 *= player.DetectUsScale(this.theEntity);
			if (num > num2)
			{
				return;
			}
		}
		num *= 0.9f;
		if (num > this.manager.noiseSeekDist)
		{
			num = this.manager.noiseSeekDist;
		}
		if (this.theEntity.IsBloodMoon)
		{
			num = this.manager.noiseSeekDist * 0.25f;
		}
		Vector3 breadcrumbPos = player.GetBreadcrumbPos(num * base.RandomFloat);
		int ticks = this.theEntity.CalcInvestigateTicks((int)(30f + base.RandomFloat * 30f) * 20, player);
		this.theEntity.SetInvestigatePosition(breadcrumbPos, ticks, true);
		this.PlaySoundSense();
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000CD058 File Offset: 0x000CB258
	[PublicizedFrom(EAccessModifier.Private)]
	public void SeekBreadcrumb(EntityPlayer player, float _maxDist)
	{
		float num = Mathf.Pow(base.RandomFloat, 2.1f);
		Vector3 breadcrumbPos = player.GetBreadcrumbPos(1f + _maxDist * num);
		int ticks = this.theEntity.CalcInvestigateTicks((int)(10f + base.RandomFloat * 10f) * 20, player);
		this.theEntity.SetInvestigatePosition(breadcrumbPos, ticks, true);
		this.PlaySoundSense();
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000CD0C0 File Offset: 0x000CB2C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlaySoundSense()
	{
		float time = Time.time;
		if (this.senseSoundTime - time < 0f)
		{
			this.senseSoundTime = time + 10f + base.RandomFloat * 10f;
			this.theEntity.PlayOneShot(this.theEntity.GetSoundSense(), false, false, false, null, 1f);
		}
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000CD11C File Offset: 0x000CB31C
	[PublicizedFrom(EAccessModifier.Private)]
	public void FindTargetPlayer(float seeDist)
	{
		if (this.theEntity.IsSleeperPassive)
		{
			return;
		}
		this.theEntity.world.GetPlayersAround(this.theEntity.position, seeDist, EAISetNearestEntityAsTarget.playerList);
		if (this.theEntity.IsSleeping)
		{
			EAISetNearestEntityAsTarget.playerList.Sort(this.sorter);
			EntityPlayer x = null;
			float num = float.MaxValue;
			bool flag = false;
			if (this.theEntity.noisePlayer != null)
			{
				if (this.theEntity.noisePlayerVolume >= this.theEntity.sleeperNoiseToWake)
				{
					x = this.theEntity.noisePlayer;
					num = this.theEntity.noisePlayerDistance;
				}
				else if (this.theEntity.noisePlayerVolume >= this.theEntity.sleeperNoiseToSense)
				{
					flag = true;
				}
			}
			for (int i = 0; i < EAISetNearestEntityAsTarget.playerList.Count; i++)
			{
				EntityPlayer entityPlayer = EAISetNearestEntityAsTarget.playerList[i];
				if (this.theEntity.CanSee(entityPlayer) && !entityPlayer.IsIgnoredByAI())
				{
					float distance = this.theEntity.GetDistance(entityPlayer);
					int sleeperDisturbedLevel = this.theEntity.GetSleeperDisturbedLevel(distance, entityPlayer.Stealth.lightLevel);
					if (sleeperDisturbedLevel >= 2)
					{
						if (distance < num)
						{
							x = entityPlayer;
							num = distance;
						}
					}
					else if (sleeperDisturbedLevel >= 1)
					{
						flag = true;
					}
				}
			}
			if (x != null)
			{
				this.closeTargetDist = num;
				this.closeTargetEntity = x;
			}
			else if (flag)
			{
				this.theEntity.Groan();
			}
			else
			{
				this.theEntity.Snore();
			}
		}
		else
		{
			for (int j = 0; j < EAISetNearestEntityAsTarget.playerList.Count; j++)
			{
				EntityPlayer entityPlayer2 = EAISetNearestEntityAsTarget.playerList[j];
				if (entityPlayer2.IsAlive() && !entityPlayer2.IsIgnoredByAI())
				{
					float seeDistance = this.manager.GetSeeDistance(entityPlayer2);
					if (seeDistance < this.closeTargetDist && this.theEntity.CanSee(entityPlayer2) && this.theEntity.CanSeeStealth(seeDistance, entityPlayer2.Stealth.lightLevel))
					{
						this.closeTargetDist = seeDistance;
						this.closeTargetEntity = entityPlayer2;
					}
				}
			}
		}
		EAISetNearestEntityAsTarget.playerList.Clear();
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000CD336 File Offset: 0x000CB536
	public override void Start()
	{
		this.theEntity.SetAttackTarget(this.targetEntity, 200);
		this.theEntity.ConditionalTriggerSleeperWakeUp();
		this.PlaySoundSense();
		base.Start();
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000CD368 File Offset: 0x000CB568
	public override bool Continue()
	{
		if (this.targetEntity.IsDead() || this.theEntity.distraction != null)
		{
			if (this.theEntity.GetAttackTarget() == this.targetEntity)
			{
				this.theEntity.SetAttackTarget(null, 0);
			}
			return false;
		}
		this.findTime += 0.05f;
		if (this.findTime > 2f)
		{
			this.findTime = 0f;
			this.FindTarget();
			if (this.closeTargetEntity && this.closeTargetEntity != this.targetEntity)
			{
				return false;
			}
		}
		if (this.theEntity.GetAttackTarget() != this.targetEntity)
		{
			return false;
		}
		if (base.check(this.targetEntity) && (this.targetPlayer == null || this.theEntity.CanSeeStealth(this.manager.GetSeeDistance(this.targetEntity), this.targetPlayer.Stealth.lightLevel)))
		{
			this.theEntity.SetAttackTarget(this.targetEntity, 600);
			this.lastSeenPos = this.targetEntity.position;
			return true;
		}
		if (this.theEntity.GetDistanceSq(this.lastSeenPos) < 2.25f)
		{
			this.lastSeenPos = Vector3.zero;
		}
		this.theEntity.SetAttackTarget(null, 0);
		int ticks = this.theEntity.CalcInvestigateTicks(Constants.cEnemySenseMemory * 20, this.targetEntity);
		if (this.lastSeenPos != Vector3.zero)
		{
			this.theEntity.SetInvestigatePosition(this.lastSeenPos, ticks, true);
		}
		return false;
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000CD50B File Offset: 0x000CB70B
	public override void Reset()
	{
		this.targetEntity = null;
		this.targetPlayer = null;
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000CD51B File Offset: 0x000CB71B
	public override string ToString()
	{
		return string.Format("{0}, {1}", base.ToString(), this.targetEntity ? this.targetEntity.EntityName : "");
	}

	// Token: 0x04001760 RID: 5984
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cHearDistMax = 50f;

	// Token: 0x04001761 RID: 5985
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAISetNearestEntityAsTarget.TargetClass> targetClasses;

	// Token: 0x04001762 RID: 5986
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerTargetClassIndex = -1;

	// Token: 0x04001763 RID: 5987
	[PublicizedFrom(EAccessModifier.Private)]
	public float closeTargetDist;

	// Token: 0x04001764 RID: 5988
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive closeTargetEntity;

	// Token: 0x04001765 RID: 5989
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive targetEntity;

	// Token: 0x04001766 RID: 5990
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer targetPlayer;

	// Token: 0x04001767 RID: 5991
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 lastSeenPos;

	// Token: 0x04001768 RID: 5992
	[PublicizedFrom(EAccessModifier.Private)]
	public float findTime;

	// Token: 0x04001769 RID: 5993
	[PublicizedFrom(EAccessModifier.Private)]
	public float senseSoundTime;

	// Token: 0x0400176A RID: 5994
	[PublicizedFrom(EAccessModifier.Private)]
	public EAISetNearestEntityAsTargetSorter sorter;

	// Token: 0x0400176B RID: 5995
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> list = new List<Entity>();

	// Token: 0x0400176C RID: 5996
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<EntityPlayer> playerList = new List<EntityPlayer>();

	// Token: 0x02000457 RID: 1111
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TargetClass
	{
		// Token: 0x0400176D RID: 5997
		public Type type;

		// Token: 0x0400176E RID: 5998
		public float hearDistMax;

		// Token: 0x0400176F RID: 5999
		public float seeDistMax;
	}
}
