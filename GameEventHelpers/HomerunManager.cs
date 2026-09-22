using System;
using System.Collections.Generic;

namespace GameEvent.GameEventHelpers
{
	// Token: 0x02001954 RID: 6484
	public class HomerunManager
	{
		// Token: 0x0600C7AE RID: 51118 RVA: 0x00495B10 File Offset: 0x00493D10
		public void Cleanup()
		{
			for (int i = 0; i < this.HomerunDataList.Count; i++)
			{
				this.HomerunDataList.list[i].Cleanup();
			}
			this.HomerunDataList.Clear();
		}

		// Token: 0x0600C7AF RID: 51119 RVA: 0x00495B54 File Offset: 0x00493D54
		public void Update(float deltaTime)
		{
			for (int i = this.HomerunDataList.Count - 1; i >= 0; i--)
			{
				if (!this.HomerunDataList.list[i].Update(deltaTime))
				{
					HomerunData homerunData = this.HomerunDataList.list[i];
					homerunData.CompleteCallback();
					this.HomerunDataList.Remove(homerunData.Player);
					homerunData.Cleanup();
				}
			}
		}

		// Token: 0x0600C7B0 RID: 51120 RVA: 0x00495BC8 File Offset: 0x00493DC8
		public void AddPlayerToHomerun(EntityPlayer player, List<int> rewardLevels, List<string> rewardEvents, float gameTime, Action completeCallback)
		{
			if (!this.HomerunDataList.dict.ContainsKey(player))
			{
				this.HomerunDataList.Add(player, new HomerunData(player, gameTime, "twitch_homerungoal_red,twitch_homerungoal_blue,twitch_homerungoal_green", rewardLevels, rewardEvents, this, completeCallback));
			}
		}

		// Token: 0x0600C7B1 RID: 51121 RVA: 0x00495C06 File Offset: 0x00493E06
		public bool HasHomerunActive(EntityPlayer player)
		{
			return this.HomerunDataList.dict.ContainsKey(player);
		}

		// Token: 0x040096A0 RID: 38560
		public DictionaryList<EntityPlayer, HomerunData> HomerunDataList = new DictionaryList<EntityPlayer, HomerunData>();
	}
}
