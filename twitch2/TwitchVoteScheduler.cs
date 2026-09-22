using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200186D RID: 6253
	public class TwitchVoteScheduler
	{
		// Token: 0x170017A1 RID: 6049
		// (get) Token: 0x0600C10D RID: 49421 RVA: 0x0047A0C4 File Offset: 0x004782C4
		public static TwitchVoteScheduler Current
		{
			get
			{
				if (TwitchVoteScheduler.instance == null)
				{
					TwitchVoteScheduler.instance = new TwitchVoteScheduler();
				}
				return TwitchVoteScheduler.instance;
			}
		}

		// Token: 0x170017A2 RID: 6050
		// (get) Token: 0x0600C10E RID: 49422 RVA: 0x0047A0DC File Offset: 0x004782DC
		public static bool HasInstance
		{
			get
			{
				return TwitchVoteScheduler.instance != null;
			}
		}

		// Token: 0x0600C10F RID: 49423 RVA: 0x0047A0E6 File Offset: 0x004782E6
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchVoteScheduler()
		{
		}

		// Token: 0x0600C110 RID: 49424 RVA: 0x0047A0F9 File Offset: 0x004782F9
		public void Cleanup()
		{
			this.ClearParticipants();
			TwitchVoteScheduler.instance = null;
		}

		// Token: 0x0600C111 RID: 49425 RVA: 0x0047A107 File Offset: 0x00478307
		public void AddParticipant(int entityID)
		{
			if (!this.votingParticipants.Contains(entityID))
			{
				this.votingParticipants.Add(entityID);
			}
		}

		// Token: 0x0600C112 RID: 49426 RVA: 0x0047A123 File Offset: 0x00478323
		public void ClearParticipants()
		{
			this.votingParticipants.Clear();
		}

		// Token: 0x0600C113 RID: 49427 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init()
		{
		}

		// Token: 0x0600C114 RID: 49428 RVA: 0x0047A130 File Offset: 0x00478330
		public void Update(float deltaTime)
		{
			if (GameManager.Instance.World == null || GameManager.Instance.World.Players == null || GameManager.Instance.World.Players.Count == 0)
			{
				return;
			}
			if (this.nextVoteTime > 0f)
			{
				this.nextVoteTime -= deltaTime;
			}
			if (this.votingParticipants.Count == 0)
			{
				return;
			}
			if (this.nextVoteTime <= 0f)
			{
				if (GameManager.Instance.World.GetPrimaryPlayerId() == this.votingParticipants[0])
				{
					TwitchManager.Current.VotingManager.RequestApprovedToStart();
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTwitchVoteScheduling>().Setup(), false, this.votingParticipants[0], -1, -1, null, 192, false);
				}
				this.votingParticipants.RemoveAt(0);
				this.nextVoteTime = 3f;
			}
		}

		// Token: 0x04009229 RID: 37417
		[PublicizedFrom(EAccessModifier.Private)]
		public static TwitchVoteScheduler instance;

		// Token: 0x0400922A RID: 37418
		[PublicizedFrom(EAccessModifier.Private)]
		public List<int> votingParticipants = new List<int>();

		// Token: 0x0400922B RID: 37419
		[PublicizedFrom(EAccessModifier.Private)]
		public float nextVoteTime;
	}
}
