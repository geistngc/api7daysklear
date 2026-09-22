using System;
using System.Collections.Generic;

namespace Challenges
{
	// Token: 0x020018F9 RID: 6393
	public class ChallengeGroupEntry
	{
		// Token: 0x0600C50C RID: 50444 RVA: 0x0048C8F9 File Offset: 0x0048AAF9
		public ChallengeGroupEntry(ChallengeGroup group)
		{
			this.ChallengeGroup = group;
		}

		// Token: 0x0600C50D RID: 50445 RVA: 0x0048C90F File Offset: 0x0048AB0F
		public void CreateChallenges(EntityPlayer player)
		{
			this.ResetChallenges(player);
			if (this.ChallengeGroup.DayReset != -1)
			{
				this.LastUpdateDay = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime) + this.ChallengeGroup.DayReset;
			}
		}

		// Token: 0x0600C50E RID: 50446 RVA: 0x0048C94C File Offset: 0x0048AB4C
		public void Update(int day, EntityPlayer player)
		{
			if (this.ChallengeGroup.DayReset == -1)
			{
				return;
			}
			if (this.LastUpdateDay <= day)
			{
				this.ResetChallenges(player);
				this.LastUpdateDay = day + this.ChallengeGroup.DayReset;
			}
		}

		// Token: 0x0600C50F RID: 50447 RVA: 0x0048C980 File Offset: 0x0048AB80
		public void AddAnyMissingChallenges(EntityPlayer player)
		{
			ChallengeJournal challengeJournal = player.challengeJournal;
			if (!this.ChallengeGroup.IsRandom)
			{
				int activeChallengeCount = this.ChallengeGroup.ActiveChallengeCount;
				int num = 0;
				while (num < this.ChallengeGroup.ChallengeClasses.Count && num < activeChallengeCount)
				{
					if (!challengeJournal.ChallengeDictionary.ContainsKey(this.ChallengeGroup.ChallengeClasses[num].Name))
					{
						Challenge challenge = this.ChallengeGroup.ChallengeClasses[num].CreateChallenge(challengeJournal);
						challenge.ChallengeGroup = this.ChallengeGroup;
						challengeJournal.AddChallenge(challenge);
					}
					num++;
				}
			}
		}

		// Token: 0x0600C510 RID: 50448 RVA: 0x0048CA1C File Offset: 0x0048AC1C
		public void ResetChallenges(EntityPlayer player)
		{
			ChallengeJournal challengeJournal = player.challengeJournal;
			if (this.ChallengeGroup.IsRandom)
			{
				challengeJournal.RemoveChallengesForGroup(this.ChallengeGroup);
				int activeChallengeCount = this.ChallengeGroup.ActiveChallengeCount;
				List<ChallengeClass> challengeClassesForCreate = this.ChallengeGroup.GetChallengeClassesForCreate();
				for (int i = 0; i < challengeClassesForCreate.Count; i++)
				{
					if (i >= activeChallengeCount)
					{
						return;
					}
					Challenge challenge = challengeClassesForCreate[i].CreateChallenge(challengeJournal);
					challenge.ChallengeGroup = this.ChallengeGroup;
					challengeJournal.AddChallenge(challenge);
					challenge.StartChallenge();
					if (challenge.IsTracked)
					{
						LocalPlayerUI.GetUIForPrimaryPlayer().xui.QuestTracker.TrackedChallenge = challenge;
					}
				}
			}
			else
			{
				int activeChallengeCount2 = this.ChallengeGroup.ActiveChallengeCount;
				this.ChallengeGroup.IsVisible(player);
				int num = 0;
				while (num < this.ChallengeGroup.ChallengeClasses.Count && num < activeChallengeCount2)
				{
					Challenge challenge2 = this.ChallengeGroup.ChallengeClasses[num].CreateChallenge(challengeJournal);
					challenge2.ChallengeGroup = this.ChallengeGroup;
					if (challengeJournal.Challenges.Count == 0)
					{
						challenge2.IsTracked = true;
					}
					challengeJournal.AddChallenge(challenge2);
					num++;
				}
			}
		}

		// Token: 0x0400954D RID: 38221
		public ChallengeGroup ChallengeGroup;

		// Token: 0x0400954E RID: 38222
		public int LastUpdateDay = -1;
	}
}
