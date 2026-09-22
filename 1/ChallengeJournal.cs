using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Audio;
using Challenges;
using UniLinq;
using UnityEngine;

// Token: 0x02000608 RID: 1544
public class ChallengeJournal
{
	// Token: 0x0600324A RID: 12874 RVA: 0x00148664 File Offset: 0x00146864
	public void Read(BinaryReader _br)
	{
		this.SetupData();
		if (_br.ReadByte() == 1)
		{
			return;
		}
		int num = _br.ReadInt32();
		this.Challenges.Clear();
		this.ChallengeDictionary.Clear();
		this.CompleteChallengesForMinEvents.Clear();
		for (int i = 0; i < num; i++)
		{
			Challenge challenge = new Challenge();
			challenge.Owner = this;
			challenge.Read(_br);
			if (challenge.ResetToChallengeClass())
			{
				if (challenge.ChallengeState == Challenge.ChallengeStates.Redeemed && this.eventList.ContainsKey(challenge.ChallengeClass.Name))
				{
					this.CompleteChallengesForMinEvents.Add(challenge);
				}
				this.ChallengeDictionary.Add(challenge.ChallengeClass.Name, challenge);
			}
		}
		this.Challenges = (from c in this.ChallengeDictionary.Values
		orderby c.ChallengeClass.OrderIndex
		select c).ToList<Challenge>();
		if (this.ChallengeGroups.Count == 0 && !GameManager.Instance.World.IsEditor() && !GameUtils.IsWorldEditor() && !GameUtils.IsPlaytesting())
		{
			foreach (ChallengeGroup group in ChallengeGroup.s_ChallengeGroups.Values)
			{
				ChallengeGroupEntry item = new ChallengeGroupEntry(group);
				this.ChallengeGroups.Add(item);
			}
		}
		num = _br.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			string b = _br.ReadString();
			int lastUpdateDay = _br.ReadInt32();
			for (int k = 0; k < this.ChallengeGroups.Count; k++)
			{
				if (this.ChallengeGroups[k].ChallengeGroup.Name == b)
				{
					this.ChallengeGroups[k].LastUpdateDay = lastUpdateDay;
				}
			}
		}
		string text = _br.ReadString();
		if (text != "" && this.ChallengeDictionary.ContainsKey(text))
		{
			this.ChallengeDictionary[text].IsTracked = true;
		}
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x0014887C File Offset: 0x00146A7C
	public void Write(BinaryWriter _bw)
	{
		string value = "";
		_bw.Write(2);
		_bw.Write(this.Challenges.Count);
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			Challenge challenge = this.Challenges[i];
			challenge.Write(_bw);
			if (challenge.IsTracked)
			{
				value = challenge.ChallengeClass.Name;
			}
		}
		int num = 0;
		for (int j = 0; j < this.ChallengeGroups.Count; j++)
		{
			if (this.ChallengeGroups[j].LastUpdateDay != -1)
			{
				num++;
			}
		}
		_bw.Write(num);
		for (int k = 0; k < this.ChallengeGroups.Count; k++)
		{
			if (this.ChallengeGroups[k].LastUpdateDay != -1)
			{
				_bw.Write(this.ChallengeGroups[k].ChallengeGroup.Name);
				_bw.Write(this.ChallengeGroups[k].LastUpdateDay);
			}
		}
		_bw.Write(value);
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x0014898C File Offset: 0x00146B8C
	public ChallengeJournal Clone()
	{
		ChallengeJournal challengeJournal = new ChallengeJournal();
		challengeJournal.Player = this.Player;
		for (int i = 0; i < this.ChallengeGroups.Count; i++)
		{
			challengeJournal.ChallengeGroups.Add(this.ChallengeGroups[i]);
		}
		for (int j = 0; j < this.Challenges.Count; j++)
		{
			Challenge challenge = this.Challenges[j].Clone();
			challengeJournal.ChallengeDictionary.Add(challenge.ChallengeClass.Name, challenge);
			challengeJournal.Challenges.Add(challenge);
		}
		return challengeJournal;
	}

	// Token: 0x0600324D RID: 12877 RVA: 0x00148A24 File Offset: 0x00146C24
	public void Update(World world)
	{
		int num = GameUtils.WorldTimeToDays(world.worldTime);
		if (this.lastDay < num)
		{
			for (int i = 0; i < this.ChallengeGroups.Count; i++)
			{
				this.ChallengeGroups[i].Update(num, this.Player);
			}
			this.lastDay = num;
		}
		if (Time.time - this.lastUpdateTime >= 1f)
		{
			this.FireEvent(MinEventTypes.onSelfChallengeCompleteUpdate, this.Player.MinEventContext);
			this.lastUpdateTime = Time.time;
		}
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x00148AAC File Offset: 0x00146CAC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupData()
	{
		this.eventList.Clear();
		foreach (ChallengeClass challengeClass in ChallengeClass.s_Challenges.Values)
		{
			if (challengeClass.HasEventsOrPassives())
			{
				this.eventList.Add(challengeClass.Name, challengeClass);
			}
		}
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x00148B24 File Offset: 0x00146D24
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.eventList == null)
		{
			return;
		}
		for (int i = 0; i < this.CompleteChallengesForMinEvents.Count; i++)
		{
			Challenge challenge = this.CompleteChallengesForMinEvents[i];
			ChallengeClass challengeClass = challenge.ChallengeClass;
			_params.Challenge = challenge;
			challengeClass.FireEvent(_eventType, _params);
		}
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x00148B74 File Offset: 0x00146D74
	public void ModifyValue(PassiveEffects _effect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> _tags)
	{
		for (int i = 0; i < this.CompleteChallengesForMinEvents.Count; i++)
		{
			Challenge challenge = this.CompleteChallengesForMinEvents[i];
			if (challenge != null)
			{
				ChallengeClass challengeClass = challenge.ChallengeClass;
				if (challengeClass != null)
				{
					MinEffectController effects = challengeClass.Effects;
					if (effects != null)
					{
						HashSet<PassiveEffects> passivesIndex = effects.PassivesIndex;
						if (passivesIndex != null && passivesIndex.Contains(_effect))
						{
							challengeClass.ModifyValue(this.Player, _effect, ref _base_val, ref _perc_val, _tags);
						}
					}
				}
			}
		}
		for (int j = 0; j < this.CompleteChallengeGroupsForMinEvents.Count; j++)
		{
			ChallengeGroup challengeGroup = this.CompleteChallengeGroupsForMinEvents[j];
			if (challengeGroup != null)
			{
				MinEffectController effects2 = challengeGroup.Effects;
				if (effects2 != null)
				{
					HashSet<PassiveEffects> passivesIndex2 = effects2.PassivesIndex;
					if (passivesIndex2 != null && passivesIndex2.Contains(_effect))
					{
						challengeGroup.ModifyValue(this.Player, _effect, ref _base_val, ref _perc_val, _tags);
					}
				}
			}
		}
	}

	// Token: 0x06003251 RID: 12881 RVA: 0x00148C48 File Offset: 0x00146E48
	public void StartChallenges(EntityPlayerLocal player)
	{
		if (this.Player == null)
		{
			this.Player = player;
		}
		if (this.Player == null)
		{
			return;
		}
		if (this.ChallengeGroups.Count == 0)
		{
			using (Dictionary<string, ChallengeGroup>.ValueCollection.Enumerator enumerator = ChallengeGroup.s_ChallengeGroups.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ChallengeGroup challengeGroup = enumerator.Current;
					ChallengeGroupEntry challengeGroupEntry = new ChallengeGroupEntry(challengeGroup);
					this.ChallengeGroups.Add(challengeGroupEntry);
					challengeGroup.IsComplete = true;
					challengeGroupEntry.CreateChallenges(this.Player);
				}
				goto IL_FE;
			}
		}
		int num = 0;
		foreach (ChallengeGroup challengeGroup2 in ChallengeGroup.s_ChallengeGroups.Values)
		{
			challengeGroup2.IsComplete = true;
			if (num < this.ChallengeGroups.Count)
			{
				ChallengeGroupEntry challengeGroupEntry2 = this.ChallengeGroups[num];
				if (challengeGroupEntry2.ChallengeGroup == challengeGroup2)
				{
					challengeGroupEntry2.AddAnyMissingChallenges(this.Player);
				}
			}
			num++;
		}
		IL_FE:
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			Challenge challenge = this.Challenges[i];
			if ((ChallengeJournal.IntroChallengesEnabled || !challenge.ChallengeClass.ChallengeGroup.IsIntro) && challenge.ChallengeClass.ChallengeGroup.IsActive)
			{
				challenge.StartChallenge();
				if (challenge.ChallengeState != Challenge.ChallengeStates.Redeemed)
				{
					challenge.ChallengeGroup.IsComplete = false;
				}
				if (challenge.IsTracked)
				{
					LocalPlayerUI.GetUIForPrimaryPlayer().xui.QuestTracker.TrackedChallenge = challenge;
				}
			}
		}
		foreach (ChallengeGroup challengeGroup3 in ChallengeGroup.s_ChallengeGroups.Values)
		{
			if (challengeGroup3.IsComplete)
			{
				this.CompleteChallengeGroupsForMinEvents.Add(challengeGroup3);
			}
		}
	}

	// Token: 0x06003252 RID: 12882 RVA: 0x00148E5C File Offset: 0x0014705C
	public void EndChallenges()
	{
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			this.Challenges[i].EndChallenge(false);
		}
	}

	// Token: 0x06003253 RID: 12883 RVA: 0x00148E91 File Offset: 0x00147091
	public void AddChallenge(Challenge challenge)
	{
		this.ChallengeDictionary.Add(challenge.ChallengeClass.Name, challenge);
		this.Challenges.Add(challenge);
	}

	// Token: 0x06003254 RID: 12884 RVA: 0x00148EB8 File Offset: 0x001470B8
	public void RemoveChallengesForGroup(ChallengeGroup challengeGroup)
	{
		for (int i = this.Challenges.Count - 1; i >= 0; i--)
		{
			Challenge challenge = this.Challenges[i];
			if (challenge.ChallengeGroup == challengeGroup)
			{
				challenge.EndChallenge(false);
				this.ChallengeDictionary.Remove(challenge.ChallengeClass.Name);
				this.Challenges.RemoveAt(i);
			}
		}
	}

	// Token: 0x06003255 RID: 12885 RVA: 0x00148F20 File Offset: 0x00147120
	public void ResetChallenges()
	{
		if (!GameManager.Instance.World.IsEditor() && !GameUtils.IsWorldEditor() && !GameUtils.IsPlaytesting())
		{
			this.Player.Buffs.RemoveCustomVar("StarterQuest");
			this.EndChallenges();
			this.ChallengeDictionary.Clear();
			this.Challenges.Clear();
			this.ChallengeGroups.Clear();
			this.CompleteChallengesForMinEvents.Clear();
			this.CompleteChallengeGroupsForMinEvents.Clear();
			this.StartChallenges(this.Player);
		}
	}

	// Token: 0x06003256 RID: 12886 RVA: 0x00148FAA File Offset: 0x001471AA
	public void HandleChallengeRedeemed(Challenge challenge)
	{
		if (this.eventList.ContainsKey(challenge.ChallengeClass.Name))
		{
			this.CompleteChallengesForMinEvents.Add(challenge);
		}
	}

	// Token: 0x06003257 RID: 12887 RVA: 0x00148FD0 File Offset: 0x001471D0
	[PublicizedFrom(EAccessModifier.Internal)]
	public void HandleChallengeGroupComplete(ChallengeGroup group)
	{
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			Challenge challenge = this.Challenges[i];
			if (challenge.ChallengeGroup == group && challenge.ChallengeState != Challenge.ChallengeStates.Redeemed)
			{
				group.IsComplete = false;
				Manager.PlayInsidePlayerHead("ui_challenge_redeem", -1, 0f, false, false);
				return;
			}
		}
		if (group.RewardEvent != null)
		{
			GameEventManager.Current.HandleAction(group.RewardEvent, null, this.Player, false, "", "", false, true, "", null);
		}
		Manager.PlayInsidePlayerHead("ui_challenge_complete_row", -1, 0f, false, false);
		group.IsComplete = true;
		if (!this.CompleteChallengeGroupsForMinEvents.Contains(group))
		{
			this.CompleteChallengeGroupsForMinEvents.Add(group);
		}
		GameManager.Instance.StartCoroutine(this.unhideRowLater(group));
	}

	// Token: 0x06003258 RID: 12888 RVA: 0x001490A4 File Offset: 0x001472A4
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator unhideRowLater(ChallengeGroup group)
	{
		yield return new WaitForSeconds(1f);
		bool flag = false;
		foreach (ChallengeGroup challengeGroup in ChallengeGroup.s_ChallengeGroups.Values)
		{
			if (challengeGroup.HiddenBy.EqualsCaseInsensitive(group.Name))
			{
				challengeGroup.UIDirty = true;
				flag = true;
			}
		}
		if (flag)
		{
			Manager.PlayInsidePlayerHead("ui_challenge_unhide_row", -1, 0f, false, false);
		}
		yield break;
	}

	// Token: 0x06003259 RID: 12889 RVA: 0x001490B4 File Offset: 0x001472B4
	[PublicizedFrom(EAccessModifier.Internal)]
	public Challenge GetNextChallenge(Challenge challenge)
	{
		Challenge challenge2 = null;
		string text = challenge.ChallengeGroup.ChallengeClasses[0].Name;
		if (text != "" && this.ChallengeDictionary.ContainsKey(text))
		{
			challenge2 = this.ChallengeDictionary[text];
		}
		while (challenge2 != null && !challenge2.IsActive)
		{
			text = challenge2.ChallengeClass.GetNextChallengeName();
			if (text != "" && this.ChallengeDictionary.ContainsKey(text))
			{
				challenge2 = this.ChallengeDictionary[text];
			}
			else
			{
				challenge2 = null;
			}
		}
		return challenge2;
	}

	// Token: 0x0600325A RID: 12890 RVA: 0x00149148 File Offset: 0x00147348
	[PublicizedFrom(EAccessModifier.Internal)]
	public Challenge GetNextRedeemableChallenge(Challenge challenge)
	{
		Challenge challenge2 = null;
		string text = challenge.ChallengeGroup.ChallengeClasses[0].Name;
		if (text != "" && this.ChallengeDictionary.ContainsKey(text))
		{
			challenge2 = this.ChallengeDictionary[text];
		}
		while (challenge2 != null && !challenge2.ReadyToComplete)
		{
			text = challenge2.ChallengeClass.GetNextChallengeName();
			if (text != "" && this.ChallengeDictionary.ContainsKey(text))
			{
				challenge2 = this.ChallengeDictionary[text];
			}
			else
			{
				challenge2 = null;
			}
		}
		return challenge2;
	}

	// Token: 0x0600325B RID: 12891 RVA: 0x001491DC File Offset: 0x001473DC
	public bool HasCompletedChallenges()
	{
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			if (this.Challenges[i].ReadyToComplete)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x00149218 File Offset: 0x00147418
	public void CompleteIntroChallenges()
	{
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			Challenge challenge = this.Challenges[i];
			if (challenge.ChallengeState != Challenge.ChallengeStates.Redeemed && challenge.ChallengeGroup.IsIntro)
			{
				if (challenge.ChallengeState == Challenge.ChallengeStates.Active)
				{
					challenge.CompleteChallenge(true, false, true);
				}
				else if (challenge.ChallengeState == Challenge.ChallengeStates.Completed)
				{
					challenge.ChallengeState = Challenge.ChallengeStates.Redeemed;
				}
			}
		}
	}

	// Token: 0x040027F6 RID: 10230
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCurrentSaveVersion = 2;

	// Token: 0x040027F7 RID: 10231
	public List<ChallengeGroupEntry> ChallengeGroups = new List<ChallengeGroupEntry>();

	// Token: 0x040027F8 RID: 10232
	public Dictionary<string, Challenge> ChallengeDictionary = new Dictionary<string, Challenge>();

	// Token: 0x040027F9 RID: 10233
	public List<Challenge> Challenges = new List<Challenge>();

	// Token: 0x040027FA RID: 10234
	public List<Challenge> CompleteChallengesForMinEvents = new List<Challenge>();

	// Token: 0x040027FB RID: 10235
	public List<ChallengeGroup> CompleteChallengeGroupsForMinEvents = new List<ChallengeGroup>();

	// Token: 0x040027FC RID: 10236
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, ChallengeClass> eventList = new Dictionary<string, ChallengeClass>();

	// Token: 0x040027FD RID: 10237
	public EntityPlayerLocal Player;

	// Token: 0x040027FE RID: 10238
	public static bool AllowChallenges = true;

	// Token: 0x040027FF RID: 10239
	public static bool IntroChallengesEnabled = true;

	// Token: 0x04002800 RID: 10240
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastDay;

	// Token: 0x04002801 RID: 10241
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastUpdateTime;
}
