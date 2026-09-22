using System;
using System.Collections.Generic;
using Audio;
using GameEvent.SequenceActions;
using UnityEngine;

namespace GameEvent.GameEventHelpers
{
	// Token: 0x02001955 RID: 6485
	public class HomerunData
	{
		// Token: 0x1700189C RID: 6300
		// (get) Token: 0x0600C7B3 RID: 51123 RVA: 0x00495C2C File Offset: 0x00493E2C
		// (set) Token: 0x0600C7B4 RID: 51124 RVA: 0x00495C34 File Offset: 0x00493E34
		public int Score
		{
			get
			{
				return this.score;
			}
			set
			{
				this.score = value;
				this.currentScoreIndex = this.GetRewardIndex(this.currentScoreIndex, value);
			}
		}

		// Token: 0x0600C7B5 RID: 51125 RVA: 0x00495C50 File Offset: 0x00493E50
		public HomerunData(EntityPlayer player, float gameTime, string goalEntityNames, List<int> rewardLevels, List<string> rewardEvents, HomerunManager manager, Action completeCallback)
		{
			this.Player = player;
			this.Owner = manager;
			this.rewardLevels = rewardLevels;
			this.rewardEvents = rewardEvents;
			this.CompleteCallback = completeCallback;
			if (player.IsInParty())
			{
				this.BuffedPlayers = new List<EntityPlayer>();
				for (int i = 0; i < player.Party.MemberList.Count; i++)
				{
					EntityPlayer entityPlayer = player.Party.MemberList[i];
					if (!entityPlayer.Buffs.HasBuff("twitch_buffHomeRun"))
					{
						entityPlayer.Buffs.AddBuff("twitch_buffHomeRun", -1, true, false, -1f);
					}
					if (player != entityPlayer)
					{
						this.BuffedPlayers.Add(entityPlayer);
					}
				}
			}
			else if (!player.Buffs.HasBuff("twitch_buffHomeRun"))
			{
				player.Buffs.AddBuff("twitch_buffHomeRun", -1, true, false, -1f);
			}
			this.gr = GameEventManager.Current.Random;
			this.timeRemaining = gameTime;
			this.SetupEntityIDs(goalEntityNames);
			this.world = GameManager.Instance.World;
		}

		// Token: 0x0600C7B6 RID: 51126 RVA: 0x00495DA4 File Offset: 0x00493FA4
		public void SetupEntityIDs(string entityNames)
		{
			string[] array = entityNames.Split(',', StringSplitOptions.None);
			this.entityIDs.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
				{
					if (keyValuePair.Value.entityClassName == array[i])
					{
						this.entityIDs.Add(keyValuePair.Key);
						if (this.entityIDs.Count == array.Length)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600C7B7 RID: 51127 RVA: 0x00495E54 File Offset: 0x00494054
		public bool Update(float deltaTime)
		{
			for (int i = this.ScoreDisplays.Count - 1; i >= 0; i--)
			{
				if (!this.ScoreDisplays[i].Update(deltaTime))
				{
					this.ScoreDisplays.RemoveAt(i);
				}
			}
			if (this.Player.IsDead())
			{
				return false;
			}
			if (this.BuffedPlayers != null)
			{
				for (int j = this.BuffedPlayers.Count - 1; j >= 0; j--)
				{
					if (this.BuffedPlayers[j].IsDead())
					{
						this.BuffedPlayers.RemoveAt(j);
					}
				}
			}
			if (this.timeRemaining > 10f && this.timeRemaining - deltaTime < 10f)
			{
				if (!this.Player.Buffs.HasBuff("twitch_buffHomeRunEnding"))
				{
					this.Player.Buffs.AddBuff("twitch_buffHomeRunEnding", -1, true, false, -1f);
				}
				if (this.BuffedPlayers != null)
				{
					for (int k = 0; k < this.BuffedPlayers.Count; k++)
					{
						if (!this.BuffedPlayers[k].Buffs.HasBuff("twitch_buffHomeRunEnding"))
						{
							this.BuffedPlayers[k].Buffs.AddBuff("twitch_buffHomeRunEnding", -1, true, false, -1f);
						}
					}
				}
			}
			this.timeRemaining -= deltaTime;
			if (this.timeRemaining > 0f)
			{
				if (this.GoalControllers.Count < this.ExpectedCount)
				{
					this.createTime -= deltaTime;
					if (this.createTime <= 0f)
					{
						Vector3 zero = Vector3.zero;
						if (ActionBaseSpawn.FindValidPosition(out zero, this.Player, 6f, 12f, true, 1f, true))
						{
							EntityHomerunGoal entityHomerunGoal = EntityFactory.CreateEntity(this.entityIDs[this.gr.RandomRange(this.entityIDs.Count)], zero, Vector3.zero, this.Player.entityId, "") as EntityHomerunGoal;
							entityHomerunGoal.SetSpawnerSource(EnumSpawnerSource.Dynamic);
							GameManager.Instance.World.SpawnEntityInWorld(entityHomerunGoal);
							entityHomerunGoal.StartPosition = zero;
							entityHomerunGoal.position = zero;
							entityHomerunGoal.direction = (EntityHomerunGoal.Direction)this.gr.RandomRange(5);
							Manager.BroadcastPlayByLocalPlayer(entityHomerunGoal.position, "twitch_balloon_spawn");
							entityHomerunGoal.Owner = this;
							this.GoalControllers.Add(entityHomerunGoal);
							this.createTime = 1f;
						}
					}
				}
				for (int l = this.GoalControllers.Count - 1; l >= 0; l--)
				{
					EntityHomerunGoal entityHomerunGoal2 = this.GoalControllers[l];
					if (this.GoalControllers[l].ReadyForDelete)
					{
						Manager.BroadcastPlayByLocalPlayer(entityHomerunGoal2.position, "twitch_balloon_despawn");
						this.world.RemoveEntity(entityHomerunGoal2.entityId, EnumRemoveEntityReason.Killed);
						this.GoalControllers.RemoveAt(l);
					}
				}
				return true;
			}
			int num = -1;
			for (int m = this.rewardLevels.Count - 1; m >= 0; m--)
			{
				if (this.Score > this.rewardLevels[m])
				{
					num = m;
					break;
				}
			}
			if (num >= 0)
			{
				string text = string.Format(Localization.Get("ttTwitchHomerunScore", false, null), Utils.ColorToHex(QualityInfo.GetTierColor(this.currentScoreIndex)), this.Score);
				GameManager.ShowTooltipMP(this.Player, text, "");
				GameEventManager.Current.HandleAction(this.rewardEvents[num], this.Player, this.Player, false, "", "", false, true, "", null);
				if (this.BuffedPlayers != null)
				{
					for (int n = 0; n < this.BuffedPlayers.Count; n++)
					{
						GameManager.ShowTooltipMP(this.BuffedPlayers[n], text, "");
						GameEventManager.Current.HandleAction(this.rewardEvents[num], this.Player, this.BuffedPlayers[n], false, "", "", false, true, "", null);
					}
				}
			}
			else
			{
				string text2 = Localization.Get("ttTwitchHomerunFailed", false, null);
				GameManager.ShowTooltipMP(this.Player, text2, "");
				if (this.BuffedPlayers != null)
				{
					for (int num2 = 0; num2 < this.BuffedPlayers.Count; num2++)
					{
						GameManager.ShowTooltipMP(this.BuffedPlayers[num2], text2, "");
					}
				}
			}
			return false;
		}

		// Token: 0x0600C7B8 RID: 51128 RVA: 0x004962D4 File Offset: 0x004944D4
		public void Cleanup()
		{
			for (int i = this.ScoreDisplays.Count - 1; i >= 0; i--)
			{
				this.ScoreDisplays[i].Cleanup();
			}
			this.ScoreDisplays.Clear();
			for (int j = 0; j < this.GoalControllers.Count; j++)
			{
				if (this.GoalControllers[j] != null)
				{
					this.world.RemoveEntity(this.GoalControllers[j].entityId, EnumRemoveEntityReason.Killed);
				}
			}
			if (this.Player != null)
			{
				this.Player.Buffs.RemoveBuff("twitch_buffHomeRun", -1, true);
			}
			if (this.BuffedPlayers != null)
			{
				for (int k = 0; k < this.BuffedPlayers.Count; k++)
				{
					this.BuffedPlayers[k].Buffs.RemoveBuff("twitch_buffHomeRun", -1, true);
				}
			}
			this.GoalControllers.Clear();
		}

		// Token: 0x0600C7B9 RID: 51129 RVA: 0x004963C8 File Offset: 0x004945C8
		public void AddScoreDisplay(Vector3 position)
		{
			Color tierColor = QualityInfo.GetTierColor(this.currentScoreIndex);
			if (this.Player.isEntityRemote)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNavObject>().Setup("twitch_score", this.Score.ToString(), position, true, tierColor, false), false, this.Player.entityId, -1, -1, null, 192, false);
			}
			if (this.BuffedPlayers != null)
			{
				for (int i = 0; i < this.BuffedPlayers.Count; i++)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNavObject>().Setup("twitch_score", this.Score.ToString(), position, true, tierColor, false), false, this.BuffedPlayers[i].entityId, -1, -1, null, 192, false);
				}
			}
			this.ScoreDisplays.Add(new HomerunData.ScoreDisplay(this.Score, position, tierColor)
			{
				Owner = this
			});
		}

		// Token: 0x0600C7BA RID: 51130 RVA: 0x004964C4 File Offset: 0x004946C4
		public void RemoveScoreDisplay(Vector3 position)
		{
			if (this.Player.isEntityRemote)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNavObject>().Setup("twitch_score", "", position, false, false, -1), false, this.Player.entityId, -1, -1, null, 192, false);
			}
			if (this.BuffedPlayers != null)
			{
				for (int i = 0; i < this.BuffedPlayers.Count; i++)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNavObject>().Setup("twitch_score", "", position, false, false, -1), false, this.BuffedPlayers[i].entityId, -1, -1, null, 192, false);
				}
			}
		}

		// Token: 0x0600C7BB RID: 51131 RVA: 0x00496580 File Offset: 0x00494780
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetRewardIndex(int currentIndex, int newScore)
		{
			int num = currentIndex + 1;
			while (num < this.rewardLevels.Count && newScore >= this.rewardLevels[num - 1])
			{
				currentIndex = num;
				num++;
			}
			return currentIndex;
		}

		// Token: 0x040096A1 RID: 38561
		public List<EntityHomerunGoal> GoalControllers = new List<EntityHomerunGoal>();

		// Token: 0x040096A2 RID: 38562
		public EntityPlayer Player;

		// Token: 0x040096A3 RID: 38563
		public List<EntityPlayer> BuffedPlayers;

		// Token: 0x040096A4 RID: 38564
		public HomerunManager Owner;

		// Token: 0x040096A5 RID: 38565
		[PublicizedFrom(EAccessModifier.Private)]
		public List<int> rewardLevels;

		// Token: 0x040096A6 RID: 38566
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> rewardEvents;

		// Token: 0x040096A7 RID: 38567
		[PublicizedFrom(EAccessModifier.Private)]
		public List<int> entityIDs = new List<int>();

		// Token: 0x040096A8 RID: 38568
		public float timeRemaining = 120f;

		// Token: 0x040096A9 RID: 38569
		public int ExpectedCount = 3;

		// Token: 0x040096AA RID: 38570
		public Action CompleteCallback;

		// Token: 0x040096AB RID: 38571
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentScoreIndex;

		// Token: 0x040096AC RID: 38572
		[PublicizedFrom(EAccessModifier.Private)]
		public int score;

		// Token: 0x040096AD RID: 38573
		public List<HomerunData.ScoreDisplay> ScoreDisplays = new List<HomerunData.ScoreDisplay>();

		// Token: 0x040096AE RID: 38574
		[PublicizedFrom(EAccessModifier.Private)]
		public World world;

		// Token: 0x040096AF RID: 38575
		[PublicizedFrom(EAccessModifier.Private)]
		public float createTime = 1f;

		// Token: 0x040096B0 RID: 38576
		[PublicizedFrom(EAccessModifier.Private)]
		public GameRandom gr;

		// Token: 0x02001956 RID: 6486
		public class ScoreDisplay
		{
			// Token: 0x0600C7BC RID: 51132 RVA: 0x004965BC File Offset: 0x004947BC
			public ScoreDisplay(int score, Vector3 position, Color color)
			{
				this.NavObject = NavObjectManager.Instance.RegisterNavObject("twitch_score", position, "", false, -1, null);
				this.NavObject.IsActive = true;
				this.NavObject.name = score.ToString();
				this.NavObject.UseOverrideFontColor = true;
				this.NavObject.OverrideColor = color;
			}

			// Token: 0x0600C7BD RID: 51133 RVA: 0x0049662E File Offset: 0x0049482E
			public bool Update(float deltaTime)
			{
				this.TimeRemaining -= deltaTime;
				if (this.TimeRemaining <= 0f)
				{
					this.RemoveNavObject();
					return false;
				}
				return true;
			}

			// Token: 0x0600C7BE RID: 51134 RVA: 0x00496654 File Offset: 0x00494854
			public void Cleanup()
			{
				if (this.NavObject != null)
				{
					this.RemoveNavObject();
				}
			}

			// Token: 0x0600C7BF RID: 51135 RVA: 0x00496664 File Offset: 0x00494864
			[PublicizedFrom(EAccessModifier.Private)]
			public void RemoveNavObject()
			{
				this.Owner.RemoveScoreDisplay(this.NavObject.TrackedPosition);
				NavObjectManager.Instance.UnRegisterNavObject(this.NavObject);
				this.NavObject = null;
			}

			// Token: 0x040096B1 RID: 38577
			public int Score;

			// Token: 0x040096B2 RID: 38578
			public NavObject NavObject;

			// Token: 0x040096B3 RID: 38579
			public float TimeRemaining = 3f;

			// Token: 0x040096B4 RID: 38580
			public HomerunData Owner;
		}
	}
}
