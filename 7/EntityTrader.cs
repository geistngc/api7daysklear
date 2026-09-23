using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004DA RID: 1242
[Preserve]
public class EntityTrader : EntityNPC, ITrader
{
	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x06002869 RID: 10345 RVA: 0x000FAEB5 File Offset: 0x000F90B5
	// (set) Token: 0x0600286A RID: 10346 RVA: 0x000FAEBD File Offset: 0x000F90BD
	public TraderData TraderData { get; set; }

	// Token: 0x17000480 RID: 1152
	// (get) Token: 0x0600286B RID: 10347 RVA: 0x000FAEC6 File Offset: 0x000F90C6
	public int PreferredDistanceIndex
	{
		get
		{
			return this.preferredDistanceIndex;
		}
	}

	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x0600286C RID: 10348 RVA: 0x000FAECE File Offset: 0x000F90CE
	public TraderInfo TraderInfo
	{
		get
		{
			if (this.TraderData != null)
			{
				return this.TraderData.TraderInfo;
			}
			return null;
		}
	}

	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x0600286D RID: 10349 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsValidAimAssistSnapTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000FAEE5 File Offset: 0x000F90E5
	public override void InitLocation(Vector3 _pos, Vector3 _rot)
	{
		_pos.y = Mathf.Floor(_pos.y);
		base.InitLocation(_pos, _rot);
		this.PhysicsTransform.gameObject.SetActive(false);
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000FAF14 File Offset: 0x000F9114
	public override void PostInit()
	{
		base.PostInit();
		if (this.TraderData == null)
		{
			this.TraderData = new TraderData();
		}
		this.SetupStartingItems();
		if (base.NPCInfo != null && base.NPCInfo.TraderID > 0)
		{
			this.TraderData.TraderID = base.NPCInfo.TraderID;
			this.IsGodMode.Value = true;
		}
		this.inventory.SetHoldingItemIdx(0);
		this.emodel.avatarController.SetArchetypeStance(base.NPCInfo.CurrentStance);
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x000FAFA4 File Offset: 0x000F91A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetupStartingItems()
	{
		for (int i = 0; i < this.itemsOnEnterGame.Count; i++)
		{
			ItemStack itemStack = this.itemsOnEnterGame[i];
			ItemClass forId = ItemClass.GetForId(itemStack.itemValue.type);
			if (forId.HasQuality)
			{
				itemStack.itemValue = new ItemValue(itemStack.itemValue.type, 1, 6, false, null, 1f);
			}
			else
			{
				itemStack.count = forId.MaxCount;
			}
			this.inventory.SetItem(i, itemStack);
		}
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x000FB028 File Offset: 0x000F9228
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitLocalActivationCommands(Action<EntityActivationCommand> _addCallback)
	{
		_addCallback(new EntityActivationCommand("talk", "talk", null, null));
		_addCallback(new EntityActivationCommand("trade", "map_trader", null, null));
		_addCallback(new EntityActivationCommand("remove", "x", null, null));
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000FB07A File Offset: 0x000F927A
	public void SetupActiveQuestsForPlayer(EntityPlayer player, int overrideFactionPoints = -1)
	{
		this.activeQuests = this.PopulateActiveQuests(player, -1, overrideFactionPoints);
		QuestEventManager.Current.SetupQuestList(this, player.entityId, this.activeQuests);
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x000FB0A4 File Offset: 0x000F92A4
	public override bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		if (this.IsDead() || base.NPCInfo == null)
		{
			return false;
		}
		if (base.CommandIs(_commandName, "talk") || base.CommandIs(_commandName, "trade"))
		{
			return true;
		}
		if (base.CommandIs(_commandName, "remove"))
		{
			return GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) && !GameUtils.IsPlaytesting();
		}
		return base.AllowActivationCommand(_commandName, _playerFocusing);
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x000FB10C File Offset: 0x000F930C
	public override string GetActivationText()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		string arg = primaryPlayer.playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + primaryPlayer.playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		return string.Format(Localization.Get("npcTooltipTalk", false, null), arg, this.LocalizedEntityName);
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x000FB174 File Offset: 0x000F9374
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityActivated(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
		if (!this.TraderData.TraderInfo.IsTraderActivitiesOpen)
		{
			GameManager.ShowTooltip(_playerFocusing, this.GetNextTimeMessage(), string.Empty, "ui_denied", null, true, false, 0f);
			return;
		}
		if (_playerFocusing != null && (!_playerFocusing.PlayerUI.windowManager.IsModalWindowOpen() || _playerFocusing.PlayerUI.windowManager.GetModalWindow().Id == "radial"))
		{
			LockManager.Instance.LockRequestLocal(this, new EntityTrader.EntityTraderLockContext(_command.commandId.ToString(), this.TraderData), 0);
		}
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x000027FC File Offset: 0x000009FC
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000FB210 File Offset: 0x000F9410
	public string GetNextTimeMessage()
	{
		return string.Format(Localization.Get("ttNoInteractTrader", false, null), GameUtils.WorldTimeToHourMinutesString(this.TraderInfo.GetOpenTime()));
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000FB234 File Offset: 0x000F9434
	public override void OnUpdateLive()
	{
		base.OnUpdateLive();
		if (this.questDictionary.Count == 0)
		{
			this.PopulateQuestList();
		}
		if (this.nativeCollider)
		{
			this.nativeCollider.enabled = true;
			this.nativeCollider.includeLayers = 17825792;
		}
		if (!GameManager.IsDedicatedServer)
		{
			EntityPlayerLocal primaryPlayer = this.world.GetPrimaryPlayer();
			this.emodel.SetLookAt(primaryPlayer.getHeadPosition());
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (this.traderArea == null)
			{
				this.traderArea = this.world.GetTraderAreaAt(new Vector3i(this.position));
			}
			if (this.traderArea != null)
			{
				if (this.updateTime <= 0f)
				{
					this.updateTime = Time.time + 3f;
				}
				if (Time.time > this.updateTime)
				{
					this.updateTime = Time.time + 1f;
					List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(this, new Bounds(this.position, Vector3.one * 10f));
					if (entitiesInBounds.Count > 0)
					{
						for (int i = 0; i < entitiesInBounds.Count; i++)
						{
							if (entitiesInBounds[i] is EntityNPC && entitiesInBounds[i].EntityClass == base.EntityClass)
							{
								if (entitiesInBounds[i].entityId < this.entityId)
								{
									this.IsDespawned = true;
									this.MarkToUnload();
								}
							}
							else if (entitiesInBounds[i] is EntityPlayer)
							{
								EntityPlayer entityPlayer = entitiesInBounds[i] as EntityPlayer;
								if (base.CanSee(entityPlayer))
								{
									if (this.GreetingDictionary.ContainsKey(entityPlayer))
									{
										if (Time.time < this.GreetingDictionary[entityPlayer])
										{
											this.GreetingDictionary[entityPlayer] = Time.time + EntityTrader.traderTalkDelayTime;
											goto IL_2BD;
										}
										this.GreetingDictionary[entityPlayer] = Time.time + EntityTrader.traderTalkDelayTime;
									}
									else
									{
										this.GreetingDictionary.Add(entityPlayer, Time.time + EntityTrader.traderTalkDelayTime);
									}
									int worldHour = this.world.WorldHour;
									if (this.world.isEventBloodMoon)
									{
										this.PlayVoiceSetEntry("greetbloodmoon", entityPlayer, false, true);
									}
									else if (worldHour >= 4 && worldHour <= 11)
									{
										this.PlayVoiceSetEntry("greetmorn", entityPlayer, false, true);
									}
									else if (worldHour >= 12 && worldHour <= 16)
									{
										this.PlayVoiceSetEntry("greetaft", entityPlayer, false, true);
									}
									else if (worldHour >= 17 && worldHour <= 19)
									{
										this.PlayVoiceSetEntry("greeteve", entityPlayer, false, true);
									}
									else if (worldHour >= 20)
									{
										this.PlayVoiceSetEntry("greetnightfall", entityPlayer, false, true);
									}
									else
									{
										this.PlayVoiceSetEntry("greeting", entityPlayer, false, true);
									}
									this.SendAnimReaction(1);
								}
							}
							IL_2BD:;
						}
					}
					if (this.TraderInfo == null)
					{
						return;
					}
					if (!this.traderArea.IsClosed)
					{
						if (this.TraderInfo.IsWarningTime)
						{
							if (!this.warningPlayed)
							{
								this.warningPlayed = true;
								this.traderArea.HandleWarning(this.world, this);
							}
						}
						else
						{
							this.warningPlayed = false;
						}
					}
					bool flag = !this.TraderInfo.IsOpen;
					if (this.traderArea.IsClosed != flag || this.firstTime)
					{
						bool playSound;
						if (flag)
						{
							playSound = this.TraderInfo.ShouldPlayCloseSound;
							if (LockManager.Instance.IsLockedServer(this, 0))
							{
								LockManager.Instance.ForceUnlockLockTarget(this);
							}
						}
						else
						{
							playSound = this.TraderInfo.ShouldPlayOpenSound;
						}
						this.firstTime = !this.traderArea.SetClosed(this.world, flag, this, playSound);
					}
				}
			}
		}
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x000FB5DC File Offset: 0x000F97DC
	public void PopulateQuestList()
	{
		if (base.NPCInfo == null || base.NPCInfo.Quests == null)
		{
			return;
		}
		this.specialQuestList = new List<QuestEntry>();
		this.questDictionary.Clear();
		for (int i = 0; i < base.NPCInfo.Quests.Count; i++)
		{
			string questID = base.NPCInfo.Quests[i].QuestID;
			if (QuestClass.GetQuest(questID).CheckCriteriaQuestGiver(this))
			{
				QuestEntry questEntry = base.NPCInfo.Quests[i];
				questEntry.QuestID = questID;
				if (questEntry.QuestClass.UniqueKey == "")
				{
					if (!this.questDictionary.ContainsKey((int)questEntry.QuestClass.DifficultyTier))
					{
						this.questDictionary.Add((int)questEntry.QuestClass.DifficultyTier, new List<QuestEntry>());
					}
					this.questDictionary[(int)questEntry.QuestClass.DifficultyTier].Add(questEntry);
				}
				else
				{
					this.specialQuestList.Add(questEntry);
				}
			}
		}
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x000FB6EC File Offset: 0x000F98EC
	public bool UpdateLocations(int tier, List<Vector2> pois)
	{
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			List<PrefabInstance> prefabsForTrader = QuestEventManager.Current.GetPrefabsForTrader(this.traderArea, tier, i, null);
			num += ((prefabsForTrader != null) ? prefabsForTrader.Count : 0);
		}
		return pois.Count >= num;
	}

	// Token: 0x0600287B RID: 10363 RVA: 0x000FB738 File Offset: 0x000F9938
	public void SetActiveQuests(EntityPlayer player, NetPackageNPCQuestList.QuestPacketEntry[] questList)
	{
		if (this.activeQuests == null)
		{
			this.activeQuests = new List<Quest>();
		}
		this.activeQuests.Clear();
		if (questList != null)
		{
			for (int i = 0; i < questList.Length; i++)
			{
				Quest quest = QuestClass.GetQuest(questList[i].QuestID).CreateQuest();
				quest.QuestGiverID = this.entityId;
				quest.QuestFaction = base.NPCInfo.QuestFaction;
				quest.SetPosition(this, questList[i].QuestLocation, questList[i].QuestSize);
				quest.SetPositionData(Quest.PositionDataTypes.QuestGiver, this.position);
				quest.SetPositionData(Quest.PositionDataTypes.TraderPosition, questList[i].TraderPos);
				quest.DataVariables.Add("POIName", Localization.Get(questList[i].POIName, false, null));
				this.activeQuests.Add(quest);
			}
		}
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x000FB820 File Offset: 0x000F9A20
	public void ClearActiveQuests(int playerID)
	{
		try
		{
			this.activeQuests = null;
		}
		catch
		{
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			QuestEventManager.Current.ClearQuestListForPlayer(this.entityId, playerID);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageNPCQuestList>().Setup(this.entityId, playerID), false);
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x000FB884 File Offset: 0x000F9A84
	public List<Quest> PopulateActiveQuests(EntityPlayer player, int currentTier = -1, int questFactionPoints = -1)
	{
		if (this.questDictionary.Count == 0)
		{
			this.PopulateQuestList();
			if (this.questDictionary.Count == 0)
			{
				return null;
			}
		}
		bool @bool = GameStats.GetBool(EnumGameStats.EnemySpawnMode);
		List<Quest> list = new List<Quest>();
		this.uniqueKeysUsed.Clear();
		Vector2 vector;
		if (this.traderArea != null)
		{
			vector = new Vector2((float)this.traderArea.Position.x, (float)this.traderArea.Position.z);
		}
		else
		{
			vector = new Vector2(this.position.x, this.position.z);
		}
		if (currentTier == -1)
		{
			currentTier = player.QuestJournal.GetCurrentFactionTier(base.NPCInfo.QuestFaction, 0, false);
		}
		if (questFactionPoints == -1)
		{
			questFactionPoints = player.QuestJournal.GlobalFactionPoints;
		}
		QuestTraderData traderData = player.QuestJournal.GetTraderData(vector);
		if (traderData != null)
		{
			traderData.CheckReset(player);
		}
		this.usedPOILocations.Clear();
		List<QuestEntry> list2 = new List<QuestEntry>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int i = 1; i <= currentTier; i++)
		{
			dictionary.Clear();
			List<Vector2> usedPOIs = player.QuestJournal.GetUsedPOIs(vector, i);
			if (usedPOIs != null)
			{
				if (this.UpdateLocations(i, usedPOIs))
				{
					if (traderData != null)
					{
						traderData.ClearTier(i);
						if (!(player is EntityPlayerLocal))
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNPCQuestList>().SetupClear(player.entityId, vector, i), false, player.entityId, -1, -1, null, 192, false);
						}
					}
				}
				else
				{
					this.usedPOILocations.AddRange(usedPOIs);
				}
			}
			list2.Clear();
			List<QuestEntry> list3 = this.questDictionary[i];
			for (int j = 0; j < list3.Count; j++)
			{
				QuestEntry questEntry = list3[j];
				if ((questEntry.StartStage == -1 || questEntry.StartStage <= questFactionPoints) && (questEntry.EndStage == -1 || questEntry.EndStage >= questFactionPoints) && questEntry.CheckRequirement(player))
				{
					list2.Add(questEntry);
				}
			}
			int num = 0;
			int num2 = 0;
			while (num2 < 100 && list2.Count != 0)
			{
				this.preferredDistanceIndex = this.distanceIndices[num];
				int index = this.rand.RandomRange(list2.Count);
				QuestEntry questEntry2 = list2[index];
				QuestClass questClass = questEntry2.QuestClass;
				bool flag = false;
				int num3;
				if (dictionary.TryGetValue(questClass.Name, out num3))
				{
					if (questClass.MaxQuestCount == 0 || num3 < questClass.MaxQuestCount)
					{
						dictionary[questClass.Name] = num3 + 1;
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					dictionary[questClass.Name] = 1;
				}
				if (!flag && this.rand.RandomFloat < questEntry2.Prob)
				{
					Quest quest = questClass.CreateQuest();
					quest.QuestGiverID = this.entityId;
					quest.QuestFaction = base.NPCInfo.QuestFaction;
					quest.SetPositionData(Quest.PositionDataTypes.QuestGiver, this.position);
					quest.SetPositionData(Quest.PositionDataTypes.TraderPosition, (this.traderArea != null) ? this.traderArea.Position : this.position);
					quest.SetupTags();
					if (@bool || !quest.QuestTags.Test_AnySet(QuestEventManager.clearTag))
					{
						if (quest.SetupPosition(this, player, this.usedPOILocations, player.entityId))
						{
							this.preferredDistanceIndex = (this.preferredDistanceIndex + 1) % 3;
							if (questClass.SingleQuest)
							{
								list2.RemoveAt(index);
							}
							list.Add(quest);
							num++;
						}
						if (quest.QuestTags.Test_AnySet(QuestEventManager.treasureTag) && GameSparksCollector.CollectGamePlayData)
						{
							GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.QuestOfferedDistance, ((int)Vector3.Distance(quest.Position, this.position) / 50 * 50).ToString(), 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
						}
						if (num >= this.distanceIndices.Length)
						{
							break;
						}
					}
				}
				num2++;
			}
		}
		for (int k = 0; k < this.specialQuestList.Count; k++)
		{
			QuestEntry questEntry3 = this.specialQuestList[k];
			if ((questEntry3.StartStage == -1 || questEntry3.StartStage <= questFactionPoints) && (questEntry3.EndStage == -1 || questEntry3.EndStage >= questFactionPoints) && questEntry3.CheckRequirement(player))
			{
				list2.Add(questEntry3);
				if (questEntry3.QuestClass.UniqueKey == "" || !this.uniqueKeysUsed.Contains(questEntry3.QuestClass.UniqueKey))
				{
					QuestClass questClass2 = questEntry3.QuestClass;
					if ((int)(questClass2.DifficultyTier - 1) <= currentTier && !player.QuestJournal.FindCompletedQuest(questClass2.ID, questClass2.Repeatable ? ((int)base.NPCInfo.QuestFaction) : -1))
					{
						int l = 0;
						while (l < 100)
						{
							Quest quest2 = questClass2.CreateQuest();
							quest2.QuestGiverID = this.entityId;
							quest2.QuestFaction = base.NPCInfo.QuestFaction;
							quest2.SetPositionData(Quest.PositionDataTypes.QuestGiver, this.position);
							quest2.SetPositionData(Quest.PositionDataTypes.TraderPosition, (this.traderArea != null) ? this.traderArea.Position : this.position);
							quest2.SetupTags();
							if (!quest2.NeedsNPCSetPosition || quest2.SetupPosition(this, player, this.usedPOILocations, player.entityId))
							{
								list.Add(quest2);
								if (questClass2.UniqueKey != "")
								{
									this.uniqueKeysUsed.Add(questClass2.UniqueKey);
								}
								if (GameSparksCollector.CollectGamePlayData)
								{
									GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.QuestTraderToTraderDistance, ((int)Vector3.Distance(quest2.Position, this.position) / 50 * 50).ToString(), 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
									break;
								}
								break;
							}
							else
							{
								l++;
							}
						}
					}
				}
			}
		}
		list.Sort(delegate(Quest q0, Quest q1)
		{
			float sqrMagnitude = (q0.Position - player.position).sqrMagnitude;
			float sqrMagnitude2 = (q1.Position - player.position).sqrMagnitude;
			return sqrMagnitude.CompareTo(sqrMagnitude2);
		});
		return list;
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000FBEC2 File Offset: 0x000FA0C2
	public int GetQuestFactionPoints(EntityPlayer player)
	{
		return player.QuestJournal.GlobalFactionPoints;
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x000FBECF File Offset: 0x000FA0CF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityTargeted(EntityAlive target)
	{
		base.OnEntityTargeted(target);
		if (!this.isEntityRemote && base.GetSpawnerSource() != EnumSpawnerSource.Dynamic && target)
		{
			this.world.aiDirector.NotifyIntentToAttack(this, target);
		}
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000FBF04 File Offset: 0x000FA104
	public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		if (base.NPCInfo != null && base.NPCInfo.TraderID > 0)
		{
			return;
		}
		base.SetAttackTarget((EntityAlive)GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()), 600);
		base.ProcessDamageResponseLocal(_dmResponse);
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanDamageEntity(int _sourceEntityId)
	{
		return false;
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x000FBF59 File Offset: 0x000FA159
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale)
	{
		if (base.NPCInfo != null && base.NPCInfo.TraderID > 0)
		{
			return 0;
		}
		return base.DamageEntity(_damageSource, _strength, _criticalHit, _impulseScale);
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000FBF7E File Offset: 0x000FA17E
	public override void AwardKill(EntityAlive killer)
	{
		if (base.NPCInfo != null && base.NPCInfo.TraderID > 0)
		{
			return;
		}
		base.AwardKill(killer);
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x000FBF9E File Offset: 0x000FA19E
	public override Vector3 GetLookVector()
	{
		if (this.lookAtPosition.Equals(Vector3.zero))
		{
			return base.GetLookVector();
		}
		return Vector3.Normalize(this.lookAtPosition - this.getHeadPosition());
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x000FBFCF File Offset: 0x000FA1CF
	public override Ray GetLookRay()
	{
		return new Ray(this.position + new Vector3(0f, this.GetEyeHeight() * this.eyeHeightHackMod, 0f), this.GetLookVector());
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateSpeedForwardAndStrafe(Vector3 _dist, float _partialTicks)
	{
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x000FC004 File Offset: 0x000FA204
	public override void PlayVoiceSetEntry(string name, EntityPlayer player, bool ignoreTime = true, bool showReactionAnim = true)
	{
		if (!TraderInfo.TraderDialog)
		{
			return;
		}
		if (this.lastVoiceTime - Time.time < 0f || ignoreTime)
		{
			string voiceSet = base.NPCInfo.VoiceSet;
			string a = base.NPCInfo.CurrentStance.ToStringCached<NPCInfo.StanceTypes>();
			if (voiceSet == "" || a == "")
			{
				return;
			}
			string text = (voiceSet + "_" + name).ToLower();
			Manager.StopAllSequencesOnEntity((player == null) ? this : player);
			if (player == null)
			{
				this.PlayOneShot(text, false, false, false, null, 1f);
			}
			else if (player.isEntityRemote)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageAudioPlayInHead>().Setup(text, true), false, player.entityId, -1, -1, null, 192, false);
			}
			else
			{
				player.PlayOneShot(text, true, false, true, null, 1f);
			}
			if (showReactionAnim)
			{
				this.PlayAnimReaction(EntityTrader.AnimReaction.Neutral);
			}
			if (!ignoreTime)
			{
				this.lastVoiceTime = Time.time + 5f;
			}
		}
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000FC114 File Offset: 0x000FA314
	public void PlayAnimReaction(EntityTrader.AnimReaction reaction)
	{
		AvatarController avatarController = this.emodel.avatarController;
		if (avatarController)
		{
			avatarController.TriggerReaction((int)reaction);
		}
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000FC13C File Offset: 0x000FA33C
	public void SendAnimReaction(int reaction)
	{
		List<AnimParamData> list = new List<AnimParamData>();
		list.Add(new AnimParamData(AvatarController.reactionTypeHash, AnimParamData.ValueTypes.Int, reaction));
		list.Add(new AnimParamData(AvatarController.reactionTriggerHash, AnimParamData.ValueTypes.Trigger, true));
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entityId, list), false, -1, -1, this.entityId, null, 192, false);
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000FC1A6 File Offset: 0x000FA3A6
	public override bool IsSharedLock(ushort _channel)
	{
		if (_channel == 0)
		{
			return true;
		}
		if (_channel != 1)
		{
			throw new ArgumentOutOfRangeException("_channel", string.Format("Unsupported lock channel: {0}", _channel));
		}
		return false;
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000FC1CF File Offset: 0x000FA3CF
	public override bool CanLockOnServer(int _lockingPlayerID, ILockContext _context, ushort _channel)
	{
		return !this.IsDead() && (this.traderArea == null || !this.traderArea.IsClosed || GameUtils.IsPlaytesting());
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x000FC1F7 File Offset: 0x000FA3F7
	public override bool CanLockLocally(ILockContext _context, ushort _channel)
	{
		return !this.IsDead() && LocalPlayerUI.GetUIForPrimaryPlayer() != null;
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x000FC210 File Offset: 0x000FA410
	public override void OnLockedServer(bool _success, int _lockingPlayerID, ILockContext _context, ushort _channel)
	{
		if (!_success)
		{
			return;
		}
		if (_channel == 0)
		{
			this.activeQuests = QuestEventManager.Current.GetQuestList(GameManager.Instance.World, this.entityId, _lockingPlayerID);
			if (this.activeQuests == null)
			{
				this.SetupActiveQuestsForPlayer(this.world.GetEntity(_lockingPlayerID) as EntityPlayer, -1);
			}
			else if (_lockingPlayerID != this.world.GetPrimaryPlayerId())
			{
				NetPackageNPCQuestList.SendQuestPacketsToPlayer(this, _lockingPlayerID);
			}
		}
		if (_channel == 1)
		{
			EntityTrader.EntityTraderLockContext entityTraderLockContext = _context as EntityTrader.EntityTraderLockContext;
			if (entityTraderLockContext != null)
			{
				GameManager.Instance.traderManager.TraderInventoryRequested(this.TraderData, _lockingPlayerID);
				entityTraderLockContext.TraderData = this.TraderData.Clone();
			}
		}
	}

	// Token: 0x0600288E RID: 10382 RVA: 0x000FC2B4 File Offset: 0x000FA4B4
	public override void OnUnlockedServer(int _unlockingPlayerId, ushort _channel)
	{
		if (_channel == 1)
		{
			this.TraderData.SetModified(this);
		}
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x000FC2C8 File Offset: 0x000FA4C8
	public override void OnLockedLocal(bool _success, ILockContext _context, ushort _channel)
	{
		if (_channel == 0)
		{
			if (!_success)
			{
				return;
			}
			EntityTrader.EntityTraderLockContext entityTraderLockContext = _context as EntityTrader.EntityTraderLockContext;
			if (entityTraderLockContext == null)
			{
				Log.Warning("[EntityTrader] Missing or invalid lock context.");
				LockManager.Instance.UnlockRequestLocal();
				return;
			}
			LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
			this.SetNextTraderWindow(EntityTrader.TraderWindowState.Close);
			QuestEventManager.Current.NPCInteracted(this);
			QuestEventManager.Current.NPCMet(this);
			Quest nextCompletedQuest = uiforPrimaryPlayer.entityPlayer.QuestJournal.GetNextCompletedQuest(null, this.entityId);
			uiforPrimaryPlayer.xui.Trader.Trader = ((nextCompletedQuest != null) ? null : this);
			if (nextCompletedQuest != null)
			{
				uiforPrimaryPlayer.xui.Dialog.QuestTurnIn = nextCompletedQuest;
				this.SetNextTraderWindow(EntityTrader.TraderWindowState.QuestComplete);
			}
			else if (base.CommandIs(entityTraderLockContext.Command, "talk"))
			{
				uiforPrimaryPlayer.xui.Dialog.Respondent = this;
				this.SetNextTraderWindow(EntityTrader.TraderWindowState.Dialog);
				QuestEventManager.Current.NPCInteracted(this);
			}
			else if (base.CommandIs(entityTraderLockContext.Command, "trade"))
			{
				this.SetNextTraderWindow(EntityTrader.TraderWindowState.Trade);
			}
			else
			{
				if (!base.CommandIs(entityTraderLockContext.Command, "remove"))
				{
					Log.Warning("[EntityTrader] Unexpected lock command '" + entityTraderLockContext.Command + "'.");
					LockManager.Instance.UnlockRequestLocal();
					return;
				}
				EntityPlayerLocal entityPlayer = uiforPrimaryPlayer.entityPlayer;
				Waypoint waypoint = (entityPlayer != null) ? entityPlayer.Waypoints.GetLastKnownPositionWaypoint(this.entityId) : null;
				if (waypoint != null)
				{
					uiforPrimaryPlayer.entityPlayer.Waypoints.Collection.Remove(waypoint);
					NavObjectManager.Instance.UnRegisterNavObjectByPosition(waypoint.pos, "waypoint");
				}
				GameEventManager.Current.HandleAction("game_remove_entity", uiforPrimaryPlayer.entityPlayer, this, false, "", "", false, true, "", null);
			}
			this.TransitionToNextWindow();
		}
		if (_channel == 1)
		{
			LocalPlayerUI uiforPrimaryPlayer2 = LocalPlayerUI.GetUIForPrimaryPlayer();
			if (!_success)
			{
				GameManager.ShowTooltip(uiforPrimaryPlayer2.entityPlayer, Localization.Get("ttNoInteractPerson", false, null), string.Empty, "ui_denied", null, false, false, 0f);
				this.SetNextTraderWindow(EntityTrader.TraderWindowState.Close);
				this.TransitionToNextWindow();
				return;
			}
			EntityTrader.EntityTraderLockContext entityTraderLockContext2 = _context as EntityTrader.EntityTraderLockContext;
			if (entityTraderLockContext2 != null && entityTraderLockContext2.TraderData != null)
			{
				this.TraderData.CopyFrom(entityTraderLockContext2.TraderData);
			}
			uiforPrimaryPlayer2.xui.Trader.Trader = this;
			uiforPrimaryPlayer2.windowManager.CloseAllOpenModalWindows(null, false);
			uiforPrimaryPlayer2.windowManager.Open("trader", true);
		}
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000FC537 File Offset: 0x000FA737
	public void SetNextTraderWindow(EntityTrader.TraderWindowState _nextState)
	{
		this.nextWindow = _nextState;
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000FC540 File Offset: 0x000FA740
	[PublicizedFrom(EAccessModifier.Private)]
	public void TransitionToNextWindow()
	{
		LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
		EntityPlayerLocal entityPlayer = uiforPrimaryPlayer.entityPlayer;
		uiforPrimaryPlayer.entityPlayer.OverrideFOV = 30f;
		uiforPrimaryPlayer.xui.Dialog.Respondent = this;
		uiforPrimaryPlayer.xui.Trader.Trader = this;
		uiforPrimaryPlayer.xui.Dialog.KeepZoomOnClose = true;
		EntityTrader.TraderWindowState traderWindowState = this.nextWindow;
		this.nextWindow = EntityTrader.TraderWindowState.Close;
		switch (traderWindowState)
		{
		case EntityTrader.TraderWindowState.Dialog:
			XUiC_DialogWindowGroup.Open(uiforPrimaryPlayer.xui, new Action(this.TransitionToNextWindow));
			return;
		case EntityTrader.TraderWindowState.Trade:
			LockManager.Instance.UnlockRequestLocal();
			LockManager.Instance.LockRequestLocal(this, new EntityTrader.EntityTraderLockContext("trade", this.TraderData), 1);
			return;
		case EntityTrader.TraderWindowState.QuestComplete:
			this.PlayVoiceSetEntry("quest_complete", entityPlayer, true, true);
			XUiC_QuestTurnInWindowGroup.Open(uiforPrimaryPlayer.xui, new Action(this.TransitionToNextWindow));
			return;
		case EntityTrader.TraderWindowState.Close:
			uiforPrimaryPlayer.xui.Dialog.Respondent = null;
			uiforPrimaryPlayer.entityPlayer.OverrideFOV = -1f;
			uiforPrimaryPlayer.xui.Dialog.KeepZoomOnClose = false;
			uiforPrimaryPlayer.xui.Trader.Trader = null;
			LockManager.Instance.UnlockRequestLocal();
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x04001E57 RID: 7767
	public float eyeHeightHackMod = 1f;

	// Token: 0x04001E58 RID: 7768
	public bool ShowWornEquipment;

	// Token: 0x04001E5A RID: 7770
	public TraderArea traderArea;

	// Token: 0x04001E5B RID: 7771
	public Dictionary<EntityPlayer, float> GreetingDictionary = new Dictionary<EntityPlayer, float>();

	// Token: 0x04001E5C RID: 7772
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool firstTime = true;

	// Token: 0x04001E5D RID: 7773
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float updateTime;

	// Token: 0x04001E5E RID: 7774
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool warningPlayed;

	// Token: 0x04001E5F RID: 7775
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float traderTalkDelayTime = 90f;

	// Token: 0x04001E60 RID: 7776
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string CommandTalk = "talk";

	// Token: 0x04001E61 RID: 7777
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string CommandTrade = "trade";

	// Token: 0x04001E62 RID: 7778
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string CommandRemove = "remove";

	// Token: 0x04001E63 RID: 7779
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int preferredDistanceIndex;

	// Token: 0x04001E64 RID: 7780
	public List<QuestEntry> specialQuestList;

	// Token: 0x04001E65 RID: 7781
	public Dictionary<int, List<QuestEntry>> questDictionary = new Dictionary<int, List<QuestEntry>>();

	// Token: 0x04001E66 RID: 7782
	public List<Quest> activeQuests;

	// Token: 0x04001E67 RID: 7783
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector2> usedPOILocations = new List<Vector2>();

	// Token: 0x04001E68 RID: 7784
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> uniqueKeysUsed = new List<string>();

	// Token: 0x04001E69 RID: 7785
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public readonly int[] distanceIndices = new int[]
	{
		0,
		0,
		0,
		1,
		2,
		2,
		2
	};

	// Token: 0x04001E6A RID: 7786
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastVoiceTime;

	// Token: 0x04001E6B RID: 7787
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityTrader.TraderWindowState nextWindow = EntityTrader.TraderWindowState.Close;

	// Token: 0x020004DB RID: 1243
	public enum AnimReaction
	{
		// Token: 0x04001E6D RID: 7789
		Happy,
		// Token: 0x04001E6E RID: 7790
		Neutral,
		// Token: 0x04001E6F RID: 7791
		Angry
	}

	// Token: 0x020004DC RID: 1244
	[Preserve]
	public class EntityTraderLockContext : ILockContext
	{
		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06002894 RID: 10388 RVA: 0x000FC6FB File Offset: 0x000FA8FB
		// (set) Token: 0x06002895 RID: 10389 RVA: 0x000FC703 File Offset: 0x000FA903
		public string Command { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x06002896 RID: 10390 RVA: 0x0000640C File Offset: 0x0000460C
		public EntityTraderLockContext()
		{
		}

		// Token: 0x06002897 RID: 10391 RVA: 0x000FC70C File Offset: 0x000FA90C
		public EntityTraderLockContext(string _command, TraderData _traderData = null)
		{
			this.Command = _command;
			this.TraderData = ((_traderData != null) ? _traderData.Clone() : null);
		}

		// Token: 0x06002898 RID: 10392 RVA: 0x000FC72D File Offset: 0x000FA92D
		public void Read(PooledBinaryReader _br)
		{
			this.Command = _br.ReadString();
			if (_br.ReadBoolean())
			{
				if (this.TraderData == null)
				{
					this.TraderData = new TraderData();
				}
				this.TraderData.Read(_br);
				return;
			}
			this.TraderData = null;
		}

		// Token: 0x06002899 RID: 10393 RVA: 0x000FC76A File Offset: 0x000FA96A
		public void Write(PooledBinaryWriter _bw)
		{
			_bw.Write(this.Command);
			_bw.Write(this.TraderData != null);
			if (this.TraderData != null)
			{
				this.TraderData.Write(_bw);
			}
		}

		// Token: 0x04001E71 RID: 7793
		public TraderData TraderData;
	}

	// Token: 0x020004DD RID: 1245
	[PublicizedFrom(EAccessModifier.Private)]
	public enum LockChannel
	{
		// Token: 0x04001E73 RID: 7795
		Interaction,
		// Token: 0x04001E74 RID: 7796
		Trade
	}

	// Token: 0x020004DE RID: 1246
	public enum TraderWindowState
	{
		// Token: 0x04001E76 RID: 7798
		Dialog,
		// Token: 0x04001E77 RID: 7799
		Trade,
		// Token: 0x04001E78 RID: 7800
		QuestComplete,
		// Token: 0x04001E79 RID: 7801
		Close
	}
}
