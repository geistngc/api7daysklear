using System;
using System.Collections.Generic;
using Twitch;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019FD RID: 6653
	[Preserve]
	public class ActionTwitchAddActionCooldown : ActionBaseClientAction
	{
		// Token: 0x0600CAC2 RID: 51906 RVA: 0x004A7450 File Offset: 0x004A5650
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null && entityPlayer.TwitchEnabled && entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Disabled)
			{
				return this.searchType != ActionTwitchAddActionCooldown.SearchTypes.Name || !(this.twitchActions == "");
			}
			return base.CanPerform(target);
		}

		// Token: 0x0600CAC3 RID: 51907 RVA: 0x004A749C File Offset: 0x004A569C
		public override void OnClientPerform(Entity target)
		{
			TwitchManager twitchManager = TwitchManager.Current;
			if (!twitchManager.TwitchActive)
			{
				return;
			}
			float currentUnityTime = twitchManager.CurrentUnityTime;
			switch (this.searchType)
			{
			case ActionTwitchAddActionCooldown.SearchTypes.Name:
			{
				string[] array = this.twitchActions.Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					if (TwitchActionManager.TwitchActions.ContainsKey(array[i]))
					{
						TwitchAction twitchAction = TwitchActionManager.TwitchActions[array[i]];
						twitchAction.tempCooldown = this.cooldownTime;
						twitchAction.tempCooldownSet = currentUnityTime;
					}
				}
				return;
			}
			case ActionTwitchAddActionCooldown.SearchTypes.Positive:
				using (Dictionary<string, TwitchAction>.ValueCollection.Enumerator enumerator = twitchManager.AvailableCommands.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TwitchAction twitchAction2 = enumerator.Current;
						if (twitchAction2.IsPositive)
						{
							twitchAction2.tempCooldown = this.cooldownTime;
							twitchAction2.tempCooldownSet = currentUnityTime;
						}
					}
					return;
				}
				break;
			case ActionTwitchAddActionCooldown.SearchTypes.Negative:
				break;
			default:
				return;
			}
			foreach (TwitchAction twitchAction3 in twitchManager.AvailableCommands.Values)
			{
				if (!twitchAction3.IsPositive)
				{
					twitchAction3.tempCooldown = this.cooldownTime;
					twitchAction3.tempCooldownSet = currentUnityTime;
				}
			}
		}

		// Token: 0x0600CAC4 RID: 51908 RVA: 0x004A75F0 File Offset: 0x004A57F0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseFloat(ActionTwitchAddActionCooldown.PropTime, ref this.cooldownTime);
			properties.ParseString(ActionTwitchAddActionCooldown.PropTwitchActions, ref this.twitchActions);
			properties.ParseEnum<ActionTwitchAddActionCooldown.SearchTypes>(ActionTwitchAddActionCooldown.PropSearchType, ref this.searchType);
		}

		// Token: 0x0600CAC5 RID: 51909 RVA: 0x004A762C File Offset: 0x004A582C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchAddActionCooldown
			{
				twitchActions = this.twitchActions,
				cooldownTime = this.cooldownTime,
				searchType = this.searchType
			};
		}

		// Token: 0x04009A3A RID: 39482
		[PublicizedFrom(EAccessModifier.Protected)]
		public float cooldownTime = 5f;

		// Token: 0x04009A3B RID: 39483
		[PublicizedFrom(EAccessModifier.Protected)]
		public string twitchActions = "";

		// Token: 0x04009A3C RID: 39484
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionTwitchAddActionCooldown.SearchTypes searchType;

		// Token: 0x04009A3D RID: 39485
		public static string PropTime = "time";

		// Token: 0x04009A3E RID: 39486
		public static string PropTwitchActions = "action_name";

		// Token: 0x04009A3F RID: 39487
		public static string PropSearchType = "search_type";

		// Token: 0x020019FE RID: 6654
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum SearchTypes
		{
			// Token: 0x04009A41 RID: 39489
			Name,
			// Token: 0x04009A42 RID: 39490
			Positive,
			// Token: 0x04009A43 RID: 39491
			Negative
		}
	}
}
