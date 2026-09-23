using System;
using System.Collections.Generic;
using System.Text;
using GamePath;
using UnityEngine;

// Token: 0x02000449 RID: 1097
public class EAIManager
{
	// Token: 0x06002179 RID: 8569 RVA: 0x000C9AB4 File Offset: 0x000C7CB4
	public EAIManager(EntityAlive _entity)
	{
		this.entity = _entity;
		this.random = _entity.world.aiDirector.random;
		this.entity.rand = this.random;
		this.tasks = new EAITaskList(this);
		this.targetTasks = new EAITaskList(this);
		this.interestDistance = 10f;
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x000C9B24 File Offset: 0x000C7D24
	public void AddItemTasks(ItemClass ic)
	{
		EAITaskList eaitaskList = new EAITaskList(this);
		string @string = ic.Properties.GetString("AITask");
		if (@string.Length > 0)
		{
			this.ParseTasks(@string, eaitaskList);
		}
		for (int i = 0; i < eaitaskList.Tasks.Count; i++)
		{
			EAITaskEntry eaitaskEntry = eaitaskList.Tasks[i];
			EAIItemTask eaiitemTask = eaitaskEntry.action as EAIItemTask;
			if (eaiitemTask != null)
			{
				eaiitemTask.ItemKey = ic.Name;
			}
			else
			{
				eaitaskList.RemoveTask(eaitaskEntry);
			}
		}
		this.tasks.AddTaskList(eaitaskList);
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x000C9BB0 File Offset: 0x000C7DB0
	public void RemoveItemTasks(string itemKey)
	{
		List<EAITaskEntry> list = this.tasks.Tasks.FindAll((EAITaskEntry t) => t.action is EAIItemTask && (t.action as EAIItemTask).ItemKey.Equals(itemKey));
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				this.tasks.RemoveTask(list[i]);
			}
		}
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x000C9C10 File Offset: 0x000C7E10
	public void CopyPropertiesFromEntityClass(EntityClass ec)
	{
		ec.Properties.ParseFloat(EntityClass.PropAIFeralSense, ref this.feralSense);
		ec.Properties.ParseFloat(EntityClass.PropAIGroupCircle, ref this.groupCircle);
		ec.Properties.ParseFloat(EntityClass.PropAINoiseSeekDist, ref this.noiseSeekDist);
		ec.Properties.ParseFloat(EntityClass.PropAISeeOffset, ref this.seeOffset);
		Vector2 vector = new Vector2(1f, 1f);
		ec.Properties.ParseVec(EntityClass.PropAIPathCostScale, ref vector);
		this.pathCostScale = this.random.RandomRange(vector.x, vector.y);
		this.partialPathHeightScale = 1f - this.pathCostScale;
		string @string = ec.Properties.GetString("AITask");
		if (@string.Length <= 0)
		{
			int num = 1;
			string text;
			for (;;)
			{
				string key = EntityClass.PropAITask + num.ToString();
				if (!ec.Properties.Values.TryGetValue(key, out text) || text.Length == 0)
				{
					goto IL_194;
				}
				EAIBase eaibase = EAIManager.CreateInstance(text);
				if (eaibase == null)
				{
					break;
				}
				eaibase.Init(this.entity);
				Dictionary<string, string> dictionary = ec.Properties.ParseKeyData(key);
				if (dictionary != null)
				{
					try
					{
						eaibase.SetData(dictionary);
					}
					catch (Exception ex)
					{
						Log.Error("EAIManager {0} SetData error {1}", new object[]
						{
							text,
							ex
						});
					}
				}
				this.tasks.AddTask(num, eaibase);
				num++;
			}
			throw new Exception("Class '" + text + "' not found!");
		}
		this.ParseTasks(@string, this.tasks);
		IL_194:
		string string2 = ec.Properties.GetString("AITarget");
		if (string2.Length > 0)
		{
			this.ParseTasks(string2, this.targetTasks);
			return;
		}
		int num2 = 1;
		string text2;
		for (;;)
		{
			string key2 = EntityClass.PropAITargetTask + num2.ToString();
			if (!ec.Properties.Values.TryGetValue(key2, out text2) || text2.Length == 0)
			{
				return;
			}
			EAIBase eaibase2 = EAIManager.CreateInstance(text2);
			if (eaibase2 == null)
			{
				break;
			}
			eaibase2.Init(this.entity);
			Dictionary<string, string> dictionary2 = ec.Properties.ParseKeyData(key2);
			if (dictionary2 != null)
			{
				try
				{
					eaibase2.SetData(dictionary2);
				}
				catch (Exception ex2)
				{
					Log.Error("EAIManager {0} SetData error {1}", new object[]
					{
						text2,
						ex2
					});
				}
			}
			this.targetTasks.AddTask(num2, eaibase2);
			num2++;
		}
		throw new Exception("Class '" + text2 + "' not found!");
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x000C9EB8 File Offset: 0x000C80B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParseTasks(string _str, EAITaskList _list)
	{
		int num = 1;
		for (int i = 0; i < _str.Length; i++)
		{
			if (char.IsLetter(_str[i]))
			{
				int num2 = _str.IndexOf('|', i + 1);
				if (num2 < 0)
				{
					num2 = _str.Length;
				}
				string text = _str.Substring(i, num2 - i);
				string text2 = text;
				string text3 = null;
				int num3 = text.IndexOf(' ');
				if (num3 >= 0)
				{
					text2 = text.Substring(0, num3);
					text3 = text.Substring(num3 + 1);
				}
				EAIBase eaibase = EAIManager.CreateInstance(text2);
				if (eaibase == null)
				{
					throw new Exception("Class '" + text2 + "' not found!");
				}
				eaibase.Init(this.entity);
				if (text3 != null)
				{
					Dictionary<string, string> dictionary = DynamicProperties.ParseData(text3);
					if (dictionary != null)
					{
						try
						{
							eaibase.SetData(dictionary);
						}
						catch (Exception ex)
						{
							Log.Error("EAIManager {0} SetData error {1}", new object[]
							{
								text2,
								ex
							});
						}
					}
				}
				_list.AddTask(num, eaibase);
				num++;
				i = num2;
			}
		}
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000C9FC8 File Offset: 0x000C81C8
	[PublicizedFrom(EAccessModifier.Private)]
	public static EAIBase CreateInstance(string _className)
	{
		return (EAIBase)Activator.CreateInstance(EAIManager.GetType(_className));
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000C9FDC File Offset: 0x000C81DC
	[PublicizedFrom(EAccessModifier.Private)]
	public static Type GetType(string _className)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_className);
		if (num <= 2294454340U)
		{
			if (num <= 1005439377U)
			{
				if (num <= 220555081U)
				{
					if (num != 87276885U)
					{
						if (num == 220555081U)
						{
							if (_className == "SetNearestEntityAsTarget")
							{
								return typeof(EAISetNearestEntityAsTarget);
							}
						}
					}
					else if (_className == "BlockIf")
					{
						return typeof(EAIBlockIf);
					}
				}
				else if (num != 244691017U)
				{
					if (num != 1003460836U)
					{
						if (num == 1005439377U)
						{
							if (_className == "ApproachSpot")
							{
								return typeof(EAIApproachSpot);
							}
						}
					}
					else if (_className == "DroneItemModHealWeapon")
					{
						return typeof(EAIDroneItemModHealWeapon);
					}
				}
				else if (_className == "BreakBlock")
				{
					return typeof(EAIBreakBlock);
				}
			}
			else if (num <= 1771441078U)
			{
				if (num != 1340592684U)
				{
					if (num != 1728706612U)
					{
						if (num == 1771441078U)
						{
							if (_className == "Look")
							{
								return typeof(EAILook);
							}
						}
					}
					else if (_className == "Wander")
					{
						return typeof(EAIWander);
					}
				}
				else if (_className == "Territorial")
				{
					return typeof(EAITerritorial);
				}
			}
			else if (num != 1968763134U)
			{
				if (num != 1994098438U)
				{
					if (num == 2294454340U)
					{
						if (_className == "DestroyArea")
						{
							return typeof(EAIDestroyArea);
						}
					}
				}
				else if (_className == "BlockingTargetTask")
				{
					return typeof(EAIBlockingTargetTask);
				}
			}
			else if (_className == "PathTest")
			{
				return typeof(EAIPathTest);
			}
		}
		else if (num <= 3546899167U)
		{
			if (num <= 2423584467U)
			{
				if (num != 2414274217U)
				{
					if (num == 2423584467U)
					{
						if (_className == "Leap")
						{
							return typeof(EAILeap);
						}
					}
				}
				else if (_className == "RunawayWhenHurt")
				{
					return typeof(EAIRunawayWhenHurt);
				}
			}
			else if (num != 2454737095U)
			{
				if (num != 2757341598U)
				{
					if (num == 3546899167U)
					{
						if (_className == "ApproachAndAttackTarget")
						{
							return typeof(EAIApproachAndAttackTarget);
						}
					}
				}
				else if (_className == "DroneItemModStunWeapon")
				{
					return typeof(EAIDroneItemModStunWeapon);
				}
			}
			else if (_className == "ApproachDistraction")
			{
				return typeof(EAIApproachDistraction);
			}
		}
		else if (num <= 3729640902U)
		{
			if (num != 3549489919U)
			{
				if (num != 3618649518U)
				{
					if (num == 3729640902U)
					{
						if (_className == "MeleeAttackTarget")
						{
							return typeof(EAIMeleeAttackTarget);
						}
					}
				}
				else if (_className == "SetNearestCorpseAsTarget")
				{
					return typeof(EAISetNearestCorpseAsTarget);
				}
			}
			else if (_className == "RangedAttackTarget")
			{
				return typeof(EAIRangedAttackTarget);
			}
		}
		else if (num != 3938759995U)
		{
			if (num != 4112963184U)
			{
				if (num == 4183380984U)
				{
					if (_className == "SetAsTargetIfHurt")
					{
						return typeof(EAISetAsTargetIfHurt);
					}
				}
			}
			else if (_className == "Dodge")
			{
				return typeof(EAIDodge);
			}
		}
		else if (_className == "RunawayFromEntity")
		{
			return typeof(EAIRunawayFromEntity);
		}
		Log.Warning("EAIManager GetType slow lookup for {0}", new object[]
		{
			_className
		});
		return Type.GetType("EAI" + _className);
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000CA426 File Offset: 0x000C8626
	public void Update()
	{
		this.interestDistance = Utils.FastMoveTowards(this.interestDistance, 10f, 0.008333334f);
		this.targetTasks.OnUpdateTasks();
		this.tasks.OnUpdateTasks();
		this.UpdateDebugName();
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x000CA460 File Offset: 0x000C8660
	public void UpdateDebugName()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuShowTasks))
		{
			EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			this.entity.DebugNameInfo = this.MakeDebugName(primaryPlayer);
		}
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x000CA498 File Offset: 0x000C8698
	public string MakeDebugName(EntityPlayer player)
	{
		EntityMoveHelper moveHelper = this.entity.moveHelper;
		StringBuilder stringBuilder = new StringBuilder(256);
		if (this.entity.IsSleeper)
		{
			stringBuilder.AppendFormat("\nSleeper {0}{1}", this.entity.IsSleeping ? "Sleep " : "", this.entity.IsSleeperPassive ? "Passive" : "");
		}
		float distance = this.entity.GetDistance(player);
		stringBuilder.AppendFormat("\nHealth {0} / {1}, Dist {2}", this.entity.Health, this.entity.GetMaxHealth(), distance.ToCultureInvariantString("0.00"));
		stringBuilder.AppendFormat("\nPCost {0}, InterestD {1}", this.pathCostScale.ToCultureInvariantString(".00"), this.interestDistance.ToCultureInvariantString("0.00"));
		string text = string.Format("\n{0}{1}{2}{3}", new object[]
		{
			this.entity.IsAlert ? string.Format("Alert {0}, ", ((float)this.entity.GetAlertTicks() * 0.05f).ToCultureInvariantString("0.00")) : "",
			this.entity.HasInvestigatePosition ? string.Format("Investigate {0}, ", ((float)this.entity.GetInvestigatePositionTicks() * 0.05f).ToCultureInvariantString("0.00")) : "",
			this.entity.attractPlayer ? string.Format("Attract {0}, Dist {1}, {2}", this.entity.attractPlayer.EntityName, this.entity.attractPlayerDistance.ToCultureInvariantString("0.00"), ((float)this.entity.attractPlayerTimeoutTicks * 0.05f).ToCultureInvariantString("0.00")) : "",
			this.entity.smellPlayer ? string.Format("Smell {0}, Dist {1}, {2}", this.entity.smellPlayer.EntityName, this.entity.smellPlayerDistance.ToCultureInvariantString("0.00"), ((float)this.entity.smellPlayerTimeoutTicks * 0.05f).ToCultureInvariantString("0.00")) : ""
		});
		if (text.Length > 1)
		{
			stringBuilder.Append(text);
		}
		string text2 = string.Format("\n{0}{1}{2}{3}{4}{5}", new object[]
		{
			moveHelper.IsActive ? string.Format("Move {0} {1},", this.entity.GetMoveSpeedAggro().ToCultureInvariantString(".00"), this.entity.GetSpeedModifier().ToCultureInvariantString(".00")) : "",
			(moveHelper.BlockedFlags > 0) ? string.Format("Blocked {0}, {1}", moveHelper.BlockedFlags, moveHelper.BlockedTime.ToCultureInvariantString("0.00")) : "",
			moveHelper.CanBreakBlocks ? "CanBrk, " : "",
			moveHelper.IsUnreachableAbove ? "UnreachAbove, " : "",
			moveHelper.IsUnreachableSide ? "UnreachSide, " : "",
			moveHelper.IsUnreachableSideJump ? "UnreachSideJump" : ""
		});
		if (text2.Length > 1)
		{
			stringBuilder.Append(text2);
		}
		if (this.entity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			stringBuilder.AppendFormat("\nStun {0}, {1}", this.entity.bodyDamage.CurrentStun.ToStringCached<EnumEntityStunType>(), this.entity.bodyDamage.StunDuration.ToCultureInvariantString("0.00"));
		}
		if (this.entity.emodel && this.entity.emodel.IsRagdollActive)
		{
			stringBuilder.Append("\nRagdoll " + this.entity.emodel.GetRagdollDebugInfo());
		}
		for (int i = 0; i < this.tasks.GetExecutingTasks().Count; i++)
		{
			EAITaskEntry eaitaskEntry = this.tasks.GetExecutingTasks()[i];
			stringBuilder.Append("\n1 " + eaitaskEntry.action.ToString());
		}
		for (int j = 0; j < this.targetTasks.GetExecutingTasks().Count; j++)
		{
			EAITaskEntry eaitaskEntry2 = this.targetTasks.GetExecutingTasks()[j];
			stringBuilder.Append("\n2 " + eaitaskEntry2.action.ToString());
		}
		if (this.entity.IsSleeping)
		{
			float value;
			float value2;
			this.entity.GetSleeperDebugScale(distance, out value, out value2);
			string value3 = string.Format("\nLight {0:0} groan{1:0} wake{2:0}, Noise {3:0} groan{4:0} wake{5:0}", new object[]
			{
				player.Stealth.lightLevel.ToCultureInvariantString(),
				value2.ToCultureInvariantString(),
				value.ToCultureInvariantString(),
				this.entity.noisePlayerVolume.ToCultureInvariantString(),
				this.entity.sleeperNoiseToSense.ToCultureInvariantString(),
				this.entity.sleeperNoiseToWake.ToCultureInvariantString()
			});
			stringBuilder.Append(value3);
		}
		else
		{
			float seeDistance = this.GetSeeDistance(player);
			float seeStealthDebugScale = this.entity.GetSeeStealthDebugScale(seeDistance);
			string value4 = string.Format("\nLight {0:0} sight {1:0}, noise {2:0} dist {3:0}", new object[]
			{
				player.Stealth.lightLevel.ToCultureInvariantString(),
				seeStealthDebugScale.ToCultureInvariantString(),
				this.entity.noisePlayerVolume.ToCultureInvariantString(),
				this.entity.noisePlayerDistance.ToCultureInvariantString()
			});
			stringBuilder.Append(value4);
		}
		stringBuilder.Append(this.entity.MakeDebugNameInfo());
		return stringBuilder.ToString();
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000CAA40 File Offset: 0x000C8C40
	public bool CheckPath(PathInfo pathInfo)
	{
		List<EAITaskEntry> executingTasks = this.tasks.GetExecutingTasks();
		for (int i = 0; i < executingTasks.Count; i++)
		{
			if (executingTasks[i].action.IsPathUsageBlocked(pathInfo.path))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000CAA86 File Offset: 0x000C8C86
	public void DamagedByEntity()
	{
		EAIDestroyArea task = this.tasks.GetTask<EAIDestroyArea>();
		if (task == null)
		{
			return;
		}
		task.Stop();
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x000CAAA0 File Offset: 0x000C8CA0
	public void SleeperWokeUp()
	{
		for (int i = 0; i < this.targetTasks.Tasks.Count; i++)
		{
			this.targetTasks.Tasks[i].executeTime = 0f;
		}
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000CAAE4 File Offset: 0x000C8CE4
	public void FallHitGround(float distance)
	{
		if (distance >= 0.8f)
		{
			this.entity.ConditionalTriggerSleeperWakeUp();
		}
		if (distance >= 2.5f)
		{
			EntityMoveHelper moveHelper = this.entity.moveHelper;
			if (moveHelper.IsActive && (moveHelper.IsUnreachableSide || moveHelper.IsMoveToAbove()))
			{
				this.ClearTaskDelay<EAIDestroyArea>(this.tasks);
				moveHelper.UnreachablePercent += 0.3f;
				moveHelper.IsDestroyAreaTryUnreachable = true;
				Bounds bb = new Bounds(this.entity.position, new Vector3(20f, 10f, 20f));
				this.entity.world.GetEntitiesInBounds(typeof(EntityHuman), bb, this.allies);
				if (this.allies.Count >= 3)
				{
					for (int i = 0; i < 2; i++)
					{
						int index = this.entity.rand.RandomRange(this.allies.Count);
						EntityHuman entityHuman = (EntityHuman)this.allies[index];
						entityHuman.moveHelper.UnreachablePercent += 0.12f;
						entityHuman.moveHelper.IsDestroyAreaTryUnreachable = true;
					}
				}
				this.allies.Clear();
			}
		}
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000CAC1A File Offset: 0x000C8E1A
	public float GetSeeDistance(Entity _seeEntity)
	{
		return this.entity.GetDistance(_seeEntity) - this.seeOffset;
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x000CAC30 File Offset: 0x000C8E30
	public static float CalcSenseScale()
	{
		switch (EAIManager.FeralSense)
		{
		case 1:
			if (GameManager.Instance.World.IsDaytime())
			{
				return 1f;
			}
			break;
		case 2:
			if (GameManager.Instance.World.IsDark())
			{
				return 1f;
			}
			break;
		case 3:
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x000CAC94 File Offset: 0x000C8E94
	public void SetTargetOnlyPlayers(float _distance)
	{
		List<EAITaskEntry> list = this.tasks.Tasks;
		for (int i = 0; i < list.Count; i++)
		{
			EAIApproachAndAttackTarget eaiapproachAndAttackTarget = list[i].action as EAIApproachAndAttackTarget;
			if (eaiapproachAndAttackTarget != null)
			{
				eaiapproachAndAttackTarget.SetTargetOnlyPlayers();
			}
		}
		List<EAITaskEntry> list2 = this.targetTasks.Tasks;
		for (int j = 0; j < list2.Count; j++)
		{
			EAISetNearestEntityAsTarget eaisetNearestEntityAsTarget = list2[j].action as EAISetNearestEntityAsTarget;
			if (eaisetNearestEntityAsTarget != null)
			{
				eaisetNearestEntityAsTarget.SetTargetOnlyPlayers(_distance);
			}
		}
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x000CAD1A File Offset: 0x000C8F1A
	public List<T> GetTasks<T>() where T : class
	{
		return this.getTaskTypes<T>(this.tasks);
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000CAD28 File Offset: 0x000C8F28
	public T GetTargetTask<T>() where T : class
	{
		return this.getTaskType<T>(this.targetTasks);
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000CAD36 File Offset: 0x000C8F36
	public List<T> GetTargetTasks<T>() where T : class
	{
		return this.getTaskTypes<T>(this.targetTasks);
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000CAD44 File Offset: 0x000C8F44
	[PublicizedFrom(EAccessModifier.Private)]
	public T getTaskType<T>(EAITaskList taskList) where T : class
	{
		for (int i = 0; i < taskList.Tasks.Count; i++)
		{
			EAITaskEntry eaitaskEntry = taskList.Tasks[i];
			if (eaitaskEntry.action is T)
			{
				return eaitaskEntry.action as T;
			}
		}
		return default(T);
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000CAD9C File Offset: 0x000C8F9C
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T> getTaskTypes<T>(EAITaskList taskList) where T : class
	{
		List<T> list = new List<T>();
		for (int i = 0; i < taskList.Tasks.Count; i++)
		{
			EAITaskEntry eaitaskEntry = taskList.Tasks[i];
			if (eaitaskEntry.action is T)
			{
				list.Add(eaitaskEntry.action as T);
			}
		}
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000CAE04 File Offset: 0x000C9004
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearTaskDelay<T>(EAITaskList taskList) where T : class
	{
		for (int i = 0; i < taskList.Tasks.Count; i++)
		{
			EAITaskEntry eaitaskEntry = taskList.Tasks[i];
			if (eaitaskEntry.action is T)
			{
				eaitaskEntry.executeTime = 0f;
			}
		}
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000CAE4C File Offset: 0x000C904C
	public void StopAllTasks()
	{
		this.tasks.StopAllTasks();
		this.targetTasks.StopAllTasks();
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000CAE64 File Offset: 0x000C9064
	public static void ToggleAnimFreeze()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		EAIManager.isAnimFreeze = !EAIManager.isAnimFreeze;
		List<Entity> list = world.Entities.list;
		for (int i = 0; i < list.Count; i++)
		{
			EntityAlive entityAlive = list[i] as EntityAlive;
			if (entityAlive && entityAlive.aiManager != null && !entityAlive.emodel.IsRagdollActive && entityAlive.emodel.avatarController)
			{
				Animator animator = entityAlive.emodel.avatarController.GetAnimator();
				if (animator)
				{
					animator.enabled = !EAIManager.isAnimFreeze;
				}
			}
		}
	}

	// Token: 0x04001705 RID: 5893
	public const float cInterestDistanceMax = 10f;

	// Token: 0x04001706 RID: 5894
	public float interestDistance;

	// Token: 0x04001707 RID: 5895
	public float lookTime;

	// Token: 0x04001708 RID: 5896
	public const float cSenseScaleMax = 1.6f;

	// Token: 0x04001709 RID: 5897
	public float feralSense;

	// Token: 0x0400170A RID: 5898
	public float groupCircle;

	// Token: 0x0400170B RID: 5899
	public float noiseSeekDist;

	// Token: 0x0400170C RID: 5900
	public float pathCostScale;

	// Token: 0x0400170D RID: 5901
	public float partialPathHeightScale;

	// Token: 0x0400170E RID: 5902
	public float seeOffset;

	// Token: 0x0400170F RID: 5903
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entity;

	// Token: 0x04001710 RID: 5904
	public GameRandom random;

	// Token: 0x04001711 RID: 5905
	[PublicizedFrom(EAccessModifier.Private)]
	public EAITaskList tasks;

	// Token: 0x04001712 RID: 5906
	[PublicizedFrom(EAccessModifier.Private)]
	public EAITaskList targetTasks;

	// Token: 0x04001713 RID: 5907
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> allies = new List<Entity>();

	// Token: 0x04001714 RID: 5908
	public static int FeralSense;

	// Token: 0x04001715 RID: 5909
	public static bool isAnimFreeze;
}
