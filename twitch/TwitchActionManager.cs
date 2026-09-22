using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x020017F6 RID: 6134
	public class TwitchActionManager
	{
		// Token: 0x17001712 RID: 5906
		// (get) Token: 0x0600BE06 RID: 48646 RVA: 0x00466FF2 File Offset: 0x004651F2
		public static TwitchActionManager Current
		{
			get
			{
				if (TwitchActionManager.instance == null)
				{
					TwitchActionManager.instance = new TwitchActionManager();
				}
				return TwitchActionManager.instance;
			}
		}

		// Token: 0x17001713 RID: 5907
		// (get) Token: 0x0600BE07 RID: 48647 RVA: 0x0046700A File Offset: 0x0046520A
		public static bool HasInstance
		{
			get
			{
				return TwitchActionManager.instance != null;
			}
		}

		// Token: 0x0600BE08 RID: 48648 RVA: 0x00467014 File Offset: 0x00465214
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchActionManager()
		{
		}

		// Token: 0x0600BE09 RID: 48649 RVA: 0x00467027 File Offset: 0x00465227
		public void Cleanup()
		{
			if (TwitchActionManager.TwitchActions != null)
			{
				TwitchActionManager.TwitchActions.Clear();
			}
			if (TwitchActionManager.TwitchVotes != null)
			{
				TwitchActionManager.TwitchVotes.Clear();
			}
			this.CategoryList.Clear();
			if (TwitchManager.HasInstance)
			{
				TwitchManager.Current.CleanupData();
			}
		}

		// Token: 0x0600BE0A RID: 48650 RVA: 0x00467067 File Offset: 0x00465267
		public void AddAction(TwitchAction action)
		{
			if (!TwitchActionManager.TwitchActions.ContainsKey(action.Name))
			{
				TwitchActionManager.TwitchActions.Add(action.Name, action);
			}
		}

		// Token: 0x0600BE0B RID: 48651 RVA: 0x0046708C File Offset: 0x0046528C
		public void AddVoteClass(TwitchVote vote)
		{
			TwitchActionManager.TwitchVotes.Add(vote.VoteName, vote);
		}

		// Token: 0x0600BE0C RID: 48652 RVA: 0x004670A0 File Offset: 0x004652A0
		public int GetCategoryIndex(string categoryName)
		{
			for (int i = 0; i < this.CategoryList.Count; i++)
			{
				if (categoryName.StartsWith(this.CategoryList[i].Name))
				{
					return i;
				}
			}
			return 9999;
		}

		// Token: 0x0600BE0D RID: 48653 RVA: 0x004670E4 File Offset: 0x004652E4
		public TwitchActionManager.ActionCategory GetCategory(string categoryName)
		{
			for (int i = 0; i < this.CategoryList.Count; i++)
			{
				if (this.CategoryList[i].Name == categoryName)
				{
					return this.CategoryList[i];
				}
			}
			return null;
		}

		// Token: 0x04008EC0 RID: 36544
		[PublicizedFrom(EAccessModifier.Private)]
		public static TwitchActionManager instance = null;

		// Token: 0x04008EC1 RID: 36545
		public List<TwitchActionManager.ActionCategory> CategoryList = new List<TwitchActionManager.ActionCategory>();

		// Token: 0x04008EC2 RID: 36546
		public static Dictionary<string, TwitchAction> TwitchActions = new Dictionary<string, TwitchAction>();

		// Token: 0x04008EC3 RID: 36547
		public static Dictionary<string, TwitchVote> TwitchVotes = new Dictionary<string, TwitchVote>();

		// Token: 0x020017F7 RID: 6135
		public class ActionCategory
		{
			// Token: 0x04008EC4 RID: 36548
			public string Name;

			// Token: 0x04008EC5 RID: 36549
			public string DisplayName;

			// Token: 0x04008EC6 RID: 36550
			public string Icon;

			// Token: 0x04008EC7 RID: 36551
			public bool ShowInCommandList = true;

			// Token: 0x04008EC8 RID: 36552
			public bool AlwaysShowInMenu;
		}
	}
}
