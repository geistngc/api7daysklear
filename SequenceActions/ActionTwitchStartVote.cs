using System;
using Twitch;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001A06 RID: 6662
	[Preserve]
	public class ActionTwitchStartVote : BaseAction
	{
		// Token: 0x0600CAE6 RID: 51942 RVA: 0x004A7DA0 File Offset: 0x004A5FA0
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.voteType == "")
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			TwitchManager twitchManager = TwitchManager.Current;
			if (!twitchManager.TwitchActive || !twitchManager.VotingManager.VotingEnabled)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			twitchManager.VotingManager.QueueVote(this.voteType);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600CAE7 RID: 51943 RVA: 0x004A7DF0 File Offset: 0x004A5FF0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionTwitchStartVote.PropVoteType, ref this.voteType);
		}

		// Token: 0x0600CAE8 RID: 51944 RVA: 0x004A7E0A File Offset: 0x004A600A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchStartVote
			{
				voteType = this.voteType
			};
		}

		// Token: 0x04009A61 RID: 39521
		[PublicizedFrom(EAccessModifier.Protected)]
		public string voteType = "";

		// Token: 0x04009A62 RID: 39522
		public static string PropVoteType = "vote_type";
	}
}
