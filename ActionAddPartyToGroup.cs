using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001962 RID: 6498
	[Preserve]
	public class ActionAddPartyToGroup : BaseAction
	{
		// Token: 0x0600C7F4 RID: 51188 RVA: 0x00497DB8 File Offset: 0x00495FB8
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			EntityPlayer entityPlayer = base.Owner.Target as EntityPlayer;
			if (entityPlayer != null)
			{
				List<Entity> list = new List<Entity>();
				if (entityPlayer.Party != null)
				{
					list.AddRange(entityPlayer.Party.MemberList);
				}
				else
				{
					list.Add(base.Owner.Target);
				}
				if (this.excludeTarget)
				{
					list.Remove(base.Owner.Target);
				}
				if (this.excludeTwitchActive)
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						EntityPlayer entityPlayer2 = list[i] as EntityPlayer;
						if (entityPlayer2 != null && entityPlayer2.TwitchEnabled && entityPlayer2 != base.Owner.Target)
						{
							list.RemoveAt(i);
						}
					}
				}
				base.Owner.AddEntitiesToGroup(this.groupName, list, this.twitchNegative);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C7F5 RID: 51189 RVA: 0x00497E90 File Offset: 0x00496090
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddPartyToGroup.PropGroupName, ref this.groupName);
			properties.ParseBool(ActionAddPartyToGroup.PropTwitchNegative, ref this.twitchNegative);
			properties.ParseBool(ActionAddPartyToGroup.PropExcludeTarget, ref this.excludeTarget);
			properties.ParseBool(ActionAddPartyToGroup.PropExcludeTwitchActive, ref this.excludeTwitchActive);
		}

		// Token: 0x0600C7F6 RID: 51190 RVA: 0x00497EE8 File Offset: 0x004960E8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddPartyToGroup
			{
				groupName = this.groupName,
				twitchNegative = this.twitchNegative,
				excludeTarget = this.excludeTarget,
				excludeTwitchActive = this.excludeTwitchActive
			};
		}

		// Token: 0x040096FF RID: 38655
		[PublicizedFrom(EAccessModifier.Protected)]
		public string groupName = "";

		// Token: 0x04009700 RID: 38656
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool twitchNegative = true;

		// Token: 0x04009701 RID: 38657
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool excludeTarget;

		// Token: 0x04009702 RID: 38658
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool excludeTwitchActive;

		// Token: 0x04009703 RID: 38659
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGroupName = "group_name";

		// Token: 0x04009704 RID: 38660
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTwitchNegative = "twitch_negative";

		// Token: 0x04009705 RID: 38661
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropExcludeTarget = "exclude_target";

		// Token: 0x04009706 RID: 38662
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropExcludeTwitchActive = "exclude_twitch_active";
	}
}
