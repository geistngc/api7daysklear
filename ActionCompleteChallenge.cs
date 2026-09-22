using System;
using Challenges;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001979 RID: 6521
	[Preserve]
	public class ActionCompleteChallenge : ActionBaseClientAction
	{
		// Token: 0x0600C863 RID: 51299 RVA: 0x0049A4D0 File Offset: 0x004986D0
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				string[] array = this.ChallengeID.ToLower().Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					Challenge challenge = entityPlayerLocal.challengeJournal.ChallengeDictionary[array[i]];
					if (challenge != null)
					{
						challenge.CompleteChallenge(this.ForceRedeem, true, false);
					}
				}
				entityPlayerLocal.PlayerUI.xui.QuestTracker.TrackedChallenge = null;
			}
		}

		// Token: 0x0600C864 RID: 51300 RVA: 0x0049A544 File Offset: 0x00498744
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionCompleteChallenge.PropChallengeID, ref this.ChallengeID);
			properties.ParseBool(ActionCompleteChallenge.PropForceRedeem, ref this.ForceRedeem);
		}

		// Token: 0x0600C865 RID: 51301 RVA: 0x0049A56F File Offset: 0x0049876F
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionCompleteChallenge
			{
				ChallengeID = this.ChallengeID,
				ForceRedeem = this.ForceRedeem
			};
		}

		// Token: 0x0400978A RID: 38794
		public string ChallengeID = "";

		// Token: 0x0400978B RID: 38795
		public bool ForceRedeem;

		// Token: 0x0400978C RID: 38796
		public static string PropChallengeID = "challenges";

		// Token: 0x0400978D RID: 38797
		public static string PropForceRedeem = "force_redeem";
	}
}
