using System;
using System.Collections.Generic;
using System.IO;
using BhvrAnalyticsServices.Interfaces;
using Services;
using Services.Analytics;
using Services.Analytics.Events;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020018EF RID: 6383
	[Preserve]
	public class Challenge
	{
		// Token: 0x17001841 RID: 6209
		// (get) Token: 0x0600C4BD RID: 50365 RVA: 0x0048AD32 File Offset: 0x00488F32
		// (set) Token: 0x0600C4BE RID: 50366 RVA: 0x0048AD3A File Offset: 0x00488F3A
		public byte CurrentFileVersion { get; set; }

		// Token: 0x17001842 RID: 6210
		// (get) Token: 0x0600C4BF RID: 50367 RVA: 0x0048AD43 File Offset: 0x00488F43
		public bool IsActive
		{
			get
			{
				return this.ChallengeState == Challenge.ChallengeStates.Active;
			}
		}

		// Token: 0x14000117 RID: 279
		// (add) Token: 0x0600C4C0 RID: 50368 RVA: 0x0048AD50 File Offset: 0x00488F50
		// (remove) Token: 0x0600C4C1 RID: 50369 RVA: 0x0048AD88 File Offset: 0x00488F88
		public event ChallengeStateChanged OnChallengeStateChanged;

		// Token: 0x17001843 RID: 6211
		// (get) Token: 0x0600C4C2 RID: 50370 RVA: 0x0048ADBD File Offset: 0x00488FBD
		// (set) Token: 0x0600C4C3 RID: 50371 RVA: 0x0048ADC5 File Offset: 0x00488FC5
		public bool NeedsPreRequisites
		{
			get
			{
				return this.needsPrerequisites;
			}
			set
			{
				this.needsPrerequisites = value;
				if (this.OnChallengeStateChanged != null)
				{
					this.OnChallengeStateChanged(this);
				}
			}
		}

		// Token: 0x0600C4C4 RID: 50372 RVA: 0x0048ADE2 File Offset: 0x00488FE2
		public void SetRequirementGroup(BaseRequirementObjectiveGroup requirementObjectiveGroup)
		{
			requirementObjectiveGroup.Owner = this;
			this.RequirementObjectiveGroup = requirementObjectiveGroup;
		}

		// Token: 0x17001844 RID: 6212
		// (get) Token: 0x0600C4C5 RID: 50373 RVA: 0x0048ADF2 File Offset: 0x00488FF2
		public bool ReadyToComplete
		{
			get
			{
				return this.ChallengeState == Challenge.ChallengeStates.Completed || (this.ChallengeClass.RedeemAlways && this.ChallengeState == Challenge.ChallengeStates.Active);
			}
		}

		// Token: 0x17001845 RID: 6213
		// (get) Token: 0x0600C4C6 RID: 50374 RVA: 0x0048AE18 File Offset: 0x00489018
		public float FillAmount
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < this.ObjectiveList.Count; i++)
				{
					num += this.ObjectiveList[i].FillAmount;
				}
				return num / (float)this.ObjectiveList.Count;
			}
		}

		// Token: 0x17001846 RID: 6214
		// (get) Token: 0x0600C4C7 RID: 50375 RVA: 0x0048AE63 File Offset: 0x00489063
		public int ActiveObjectives
		{
			get
			{
				if (!this.NeedsPreRequisites)
				{
					return this.ObjectiveList.Count;
				}
				return this.RequirementObjectiveGroup.Count;
			}
		}

		// Token: 0x17001847 RID: 6215
		// (get) Token: 0x0600C4C8 RID: 50376 RVA: 0x0048AE84 File Offset: 0x00489084
		public bool NeedsUIUpdate
		{
			get
			{
				return this.ChallengeState == Challenge.ChallengeStates.Active && this.ChallengeClass.NeedsConstantUIUpdate;
			}
		}

		// Token: 0x0600C4C9 RID: 50377 RVA: 0x0048AE9C File Offset: 0x0048909C
		public virtual void Read(BinaryReader _br)
		{
			this.CurrentFileVersion = _br.ReadByte();
			string key = _br.ReadString();
			this.ChallengeState = (Challenge.ChallengeStates)_br.ReadByte();
			if (this.CurrentFileVersion >= 2)
			{
				this.AutoCompleted = _br.ReadBoolean();
			}
			byte currentVersion = _br.ReadByte();
			int num = _br.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				ChallengeObjectiveType type = (ChallengeObjectiveType)_br.ReadByte();
				this.ObjectiveList.Add(BaseChallengeObjective.ReadObjective(currentVersion, type, _br));
			}
			if (ChallengeClass.s_Challenges.ContainsKey(key))
			{
				this.ChallengeClass = ChallengeClass.s_Challenges[key];
				this.ChallengeGroup = this.ChallengeClass.ChallengeGroup;
			}
		}

		// Token: 0x0600C4CA RID: 50378 RVA: 0x0048AF44 File Offset: 0x00489144
		public ChallengeObjectiveChallengeComplete GetChallengeCompleteObjective()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				ChallengeObjectiveChallengeComplete challengeObjectiveChallengeComplete = this.ObjectiveList[i] as ChallengeObjectiveChallengeComplete;
				if (challengeObjectiveChallengeComplete != null)
				{
					return challengeObjectiveChallengeComplete;
				}
			}
			return null;
		}

		// Token: 0x0600C4CB RID: 50379 RVA: 0x0048AF7F File Offset: 0x0048917F
		[PublicizedFrom(EAccessModifier.Internal)]
		public Recipe GetRecipeFromRequirements()
		{
			if (this.RequirementObjectiveGroup != null)
			{
				return this.RequirementObjectiveGroup.GetItemRecipe();
			}
			return null;
		}

		// Token: 0x0600C4CC RID: 50380 RVA: 0x0048AF98 File Offset: 0x00489198
		public virtual void Write(BinaryWriter _bw)
		{
			_bw.Write(Challenge.FileVersion);
			_bw.Write(this.ChallengeClass.Name);
			_bw.Write((byte)this.ChallengeState);
			_bw.Write(this.AutoCompleted);
			_bw.Write(BaseChallengeObjective.FileVersion);
			_bw.Write(this.ObjectiveList.Count);
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].WriteObjective(_bw);
			}
		}

		// Token: 0x0600C4CD RID: 50381 RVA: 0x0048B01D File Offset: 0x0048921D
		public bool ResetToChallengeClass()
		{
			return this.ChallengeClass != null && this.ChallengeClass.ResetObjectives(this);
		}

		// Token: 0x0600C4CE RID: 50382 RVA: 0x0048B035 File Offset: 0x00489235
		public List<BaseChallengeObjective> GetObjectiveList()
		{
			if (!this.NeedsPreRequisites)
			{
				return this.ObjectiveList;
			}
			return this.RequirementObjectiveGroup.CurrentObjectiveList;
		}

		// Token: 0x0600C4CF RID: 50383 RVA: 0x0048B054 File Offset: 0x00489254
		public List<Recipe> CraftedRecipes()
		{
			List<Recipe> list = null;
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				if (!this.ObjectiveList[i].Complete)
				{
					Recipe[] recipeItems = this.ObjectiveList[i].GetRecipeItems();
					if (recipeItems != null)
					{
						if (list == null)
						{
							list = new List<Recipe>();
						}
						list.AddRange(recipeItems);
					}
				}
			}
			return list;
		}

		// Token: 0x0600C4D0 RID: 50384 RVA: 0x0048B0B4 File Offset: 0x004892B4
		public void StartChallenge()
		{
			if (!this.ChallengeClass.HandleResourceRequirement())
			{
				for (int i = 0; i < this.ObjectiveList.Count; i++)
				{
					this.ObjectiveList[i].HandleAutoComplete();
				}
				this.HandleComplete(false, true);
				if (this.ChallengeState == Challenge.ChallengeStates.Completed)
				{
					this.AutoCompleted = true;
					this.ChallengeState = Challenge.ChallengeStates.Redeemed;
				}
			}
			if (this.IsActive)
			{
				for (int j = 0; j < this.ObjectiveList.Count; j++)
				{
					this.ObjectiveList[j].HandleAddHooks();
				}
			}
		}

		// Token: 0x0600C4D1 RID: 50385 RVA: 0x0048B144 File Offset: 0x00489344
		public void EndChallenge(bool isCompleted)
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].HandleRemoveHooks();
			}
			if (this.RequirementObjectiveGroup != null)
			{
				this.RequirementObjectiveGroup.HandleRemoveHooks();
			}
			if (isCompleted)
			{
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Player);
				if (uiforPlayer.xui.QuestTracker.TrackedChallenge == this)
				{
					uiforPlayer.xui.QuestTracker.TrackedChallenge = this.Owner.GetNextChallenge(this);
				}
			}
		}

		// Token: 0x0600C4D2 RID: 50386 RVA: 0x0048B1D0 File Offset: 0x004893D0
		public void HandleComplete(bool showTooltip = true, bool forceComplete = false)
		{
			bool flag = false;
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				if (!this.ObjectiveList[i].Complete)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (this.IsTracked)
				{
					this.CheckPrerequisites();
				}
				return;
			}
			if (this.ChallengeState == Challenge.ChallengeStates.Active)
			{
				this.ChallengeState = Challenge.ChallengeStates.Completed;
			}
			this.EndChallenge(true);
			QuestEventManager.Current.ChallengeCompleted(this.ChallengeClass, false);
			if (this.ChallengeClass.ChallengeGroup.IsVisible(this.Owner.Player) && showTooltip)
			{
				GameManager.ShowTooltip(this.Owner.Player, string.Format(Localization.Get("challengeMessageComplete", false, null), this.ChallengeClass.Title), "", "ui_challenge_complete", null, false, false, 0f);
			}
			ChallengeCompletedEventData challengeCompletedEventData = new ChallengeCompletedEventData();
			challengeCompletedEventData.ServerId = Helper.GetServerId();
			string saveId;
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				GameManager instance = GameManager.Instance;
				if (instance == null)
				{
					saveId = null;
				}
				else
				{
					World world = instance.World;
					saveId = ((world != null) ? world.Guid : null);
				}
			}
			else
			{
				saveId = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
			}
			challengeCompletedEventData.SaveId = saveId;
			challengeCompletedEventData.ChallengeCompletedTimeStamp = DateTime.UtcNow.ToString("O");
			challengeCompletedEventData.ElapsedTime = (uint)(this.Owner.Player.totalTimePlayed * 60f);
			challengeCompletedEventData.PersonalGameStage = this.Owner.Player.gameStage;
			challengeCompletedEventData.PlayerLevel = this.Owner.Player.Progression.Level;
			challengeCompletedEventData.ChallengeName = this.ChallengeClass.Name;
			challengeCompletedEventData.ChallengeGroup = this.ChallengeGroup.Name;
			challengeCompletedEventData.ChallengeCategory = this.ChallengeGroup.Category;
			challengeCompletedEventData.IsForcedCompletion = forceComplete;
			ChallengeCompletedEventData analyticsEventData = challengeCompletedEventData;
			ServiceProvider.Instance.Get<IAnalyticsService>().LogEvent(analyticsEventData);
		}

		// Token: 0x0600C4D3 RID: 50387 RVA: 0x0048B3A0 File Offset: 0x004895A0
		public void Redeem()
		{
			GameEventManager.Current.HandleAction(this.ChallengeClass.RewardEvent, null, this.Owner.Player, false, "", "", false, true, "", null);
			this.Owner.HandleChallengeRedeemed(this);
			this.Owner.HandleChallengeGroupComplete(this.ChallengeGroup);
			ChallengeClaimedEventData challengeClaimedEventData = new ChallengeClaimedEventData();
			challengeClaimedEventData.ServerId = Helper.GetServerId();
			string saveId;
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				GameManager instance = GameManager.Instance;
				if (instance == null)
				{
					saveId = null;
				}
				else
				{
					World world = instance.World;
					saveId = ((world != null) ? world.Guid : null);
				}
			}
			else
			{
				saveId = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
			}
			challengeClaimedEventData.SaveId = saveId;
			challengeClaimedEventData.ChallengeClaimedTimeStamp = DateTime.UtcNow.ToString("O");
			challengeClaimedEventData.ElapsedTime = (uint)(this.Owner.Player.totalTimePlayed * 60f);
			challengeClaimedEventData.PersonalGameStage = this.Owner.Player.gameStage;
			challengeClaimedEventData.PlayerLevel = this.Owner.Player.Progression.Level;
			challengeClaimedEventData.ChallengeName = this.ChallengeClass.Name;
			challengeClaimedEventData.ChallengeGroup = this.ChallengeGroup.Name;
			challengeClaimedEventData.IsChallengeGroupComplete = this.ChallengeGroup.IsComplete;
			challengeClaimedEventData.ChallengeCategory = this.ChallengeGroup.Category;
			challengeClaimedEventData.RewardEvent = this.ChallengeClass.RewardEvent;
			ChallengeClaimedEventData analyticsEventData = challengeClaimedEventData;
			ServiceProvider.Instance.Get<IAnalyticsService>().LogEvent(analyticsEventData);
		}

		// Token: 0x0600C4D4 RID: 50388 RVA: 0x0048B518 File Offset: 0x00489718
		public void HandleTrackingStarted()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				if (!this.ObjectiveList[i].Complete)
				{
					this.ObjectiveList[i].HandleTrackingStarted();
				}
			}
		}

		// Token: 0x0600C4D5 RID: 50389 RVA: 0x0048B560 File Offset: 0x00489760
		public void HandleTrackingEnded()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].HandleTrackingEnded();
			}
		}

		// Token: 0x0600C4D6 RID: 50390 RVA: 0x0048B594 File Offset: 0x00489794
		public ChallengeTrackingHandler GetTrackingHelper()
		{
			if (this.TrackingHandler == null)
			{
				this.TrackingHandler = new ChallengeTrackingHandler
				{
					Owner = this,
					LocalPlayer = this.Owner.Player
				};
			}
			return this.TrackingHandler;
		}

		// Token: 0x0600C4D7 RID: 50391 RVA: 0x0048B5C7 File Offset: 0x004897C7
		public void AddTrackingEntry(TrackingEntry entry)
		{
			if (this.TrackingHandler == null)
			{
				this.TrackingHandler = new ChallengeTrackingHandler
				{
					Owner = this,
					LocalPlayer = this.Owner.Player
				};
			}
			this.TrackingHandler.AddTrackingEntry(entry);
		}

		// Token: 0x0600C4D8 RID: 50392 RVA: 0x0048B600 File Offset: 0x00489800
		public void RemoveTrackingEntry(TrackingEntry entry)
		{
			if (this.TrackingHandler == null)
			{
				return;
			}
			this.TrackingHandler.RemoveTrackingEntry(entry);
		}

		// Token: 0x0600C4D9 RID: 50393 RVA: 0x0048B618 File Offset: 0x00489818
		public Challenge Clone()
		{
			Challenge challenge = new Challenge();
			challenge.ChallengeClass = this.ChallengeClass;
			challenge.ChallengeState = this.ChallengeState;
			challenge.IsTracked = this.IsTracked;
			challenge.AutoCompleted = this.AutoCompleted;
			if (this.RequirementObjectiveGroup != null)
			{
				BaseRequirementObjectiveGroup requirementObjectiveGroup = this.RequirementObjectiveGroup;
				BaseRequirementObjectiveGroup baseRequirementObjectiveGroup = requirementObjectiveGroup.Clone();
				baseRequirementObjectiveGroup.Owner = this;
				baseRequirementObjectiveGroup.ClonePhases(requirementObjectiveGroup);
				challenge.RequirementObjectiveGroup = baseRequirementObjectiveGroup;
			}
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i].Clone();
				baseChallengeObjective.CopyValues(this.ObjectiveList[i], this.ChallengeClass.ObjectiveList[i]);
				baseChallengeObjective.Owner = challenge;
				challenge.ObjectiveList.Add(baseChallengeObjective);
			}
			return challenge;
		}

		// Token: 0x0600C4DA RID: 50394 RVA: 0x0048B6E9 File Offset: 0x004898E9
		public void RemovePrerequisiteHooks()
		{
			if (this.RequirementObjectiveGroup != null)
			{
				this.RequirementObjectiveGroup.HandleRemoveHooks();
			}
		}

		// Token: 0x0600C4DB RID: 50395 RVA: 0x0048B700 File Offset: 0x00489900
		public void CheckPrerequisites()
		{
			bool flag = false;
			if (this.RequirementObjectiveGroup != null && this.RequirementObjectiveGroup.HasPrerequisiteCondition())
			{
				if (this.RequirementObjectiveGroup.HandleCheckStatus())
				{
					flag = true;
					this.UIDirty = true;
				}
				this.RequirementObjectiveGroup.UpdateStatus();
			}
			if (this.NeedsPreRequisites != flag)
			{
				this.NeedsPreRequisites = flag;
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Player);
				if (uiforPlayer.xui.QuestTracker.TrackedChallenge == this)
				{
					uiforPlayer.xui.QuestTracker.HandleTrackedChallengeChanged();
				}
			}
		}

		// Token: 0x0600C4DC RID: 50396 RVA: 0x0048B789 File Offset: 0x00489989
		public void AddPrerequisiteHooks()
		{
			if (this.RequirementObjectiveGroup != null)
			{
				this.RequirementObjectiveGroup.HandleAddHooks();
			}
			this.CheckPrerequisites();
		}

		// Token: 0x0600C4DD RID: 50397 RVA: 0x0048B7A4 File Offset: 0x004899A4
		public BaseChallengeObjective GetNavObjective()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				if (baseChallengeObjective.NavType != ChallengeClass.UINavTypes.None)
				{
					return baseChallengeObjective;
				}
			}
			return null;
		}

		// Token: 0x0600C4DE RID: 50398 RVA: 0x0048B7E0 File Offset: 0x004899E0
		public void CompleteChallenge(bool forceRedeem = false, bool giveReward = true, bool forceComplete = false)
		{
			if (!this.IsActive && (!forceRedeem || this.ChallengeState != Challenge.ChallengeStates.Completed))
			{
				return;
			}
			foreach (BaseChallengeObjective baseChallengeObjective in this.ObjectiveList)
			{
				baseChallengeObjective.CompleteObjective(false);
			}
			this.HandleComplete(false, forceComplete);
			if (!forceRedeem)
			{
				return;
			}
			this.ChallengeState = Challenge.ChallengeStates.Redeemed;
			if (giveReward)
			{
				this.Redeem();
				QuestEventManager.Current.ChallengeCompleted(this.ChallengeClass, true);
			}
		}

		// Token: 0x040094FB RID: 38139
		public static byte FileVersion = 2;

		// Token: 0x040094FD RID: 38141
		public Challenge.ChallengeStates ChallengeState;

		// Token: 0x040094FE RID: 38142
		public bool IsTracked;

		// Token: 0x040094FF RID: 38143
		public bool UIDirty;

		// Token: 0x04009500 RID: 38144
		public bool AutoCompleted;

		// Token: 0x04009501 RID: 38145
		public ChallengeClass ChallengeClass;

		// Token: 0x04009502 RID: 38146
		public ChallengeGroup ChallengeGroup;

		// Token: 0x04009503 RID: 38147
		public BaseRequirementObjectiveGroup RequirementObjectiveGroup;

		// Token: 0x04009504 RID: 38148
		public List<BaseChallengeObjective> ObjectiveList = new List<BaseChallengeObjective>();

		// Token: 0x04009506 RID: 38150
		public ChallengeTrackingHandler TrackingHandler;

		// Token: 0x04009507 RID: 38151
		[PublicizedFrom(EAccessModifier.Private)]
		public bool needsPrerequisites;

		// Token: 0x04009508 RID: 38152
		public ChallengeJournal Owner;

		// Token: 0x020018F0 RID: 6384
		public enum ChallengeStates : byte
		{
			// Token: 0x0400950A RID: 38154
			Active,
			// Token: 0x0400950B RID: 38155
			Completed,
			// Token: 0x0400950C RID: 38156
			Redeemed
		}
	}
}
