using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001976 RID: 6518
	[Preserve]
	public class ActionCallGameEvent : BaseAction
	{
		// Token: 0x0600C854 RID: 51284 RVA: 0x0049A1D4 File Offset: 0x004983D4
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			GameEventActionSequence ownerSeq = (base.Owner.OwnerSequence != null) ? base.Owner.OwnerSequence : base.Owner;
			if (this.targetGroup != "")
			{
				List<Entity> entityGroup = base.Owner.GetEntityGroup(this.targetGroup);
				if (entityGroup != null)
				{
					for (int i = 0; i < entityGroup.Count; i++)
					{
						GameEventManager.Current.HandleAction(this.gameEventNames[GameEventManager.Current.Random.RandomRange(0, this.gameEventNames.Count)], base.Owner.Requester, entityGroup[i], base.Owner.TwitchActivated, base.Owner.ExtraData, "", false, true, "", ownerSeq);
					}
				}
			}
			else
			{
				GameEventManager.Current.HandleAction(this.gameEventNames[GameEventManager.Current.Random.RandomRange(0, this.gameEventNames.Count)], base.Owner.Requester, base.Owner.Target, base.Owner.TwitchActivated, base.Owner.ExtraData, "", false, true, "", ownerSeq);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C855 RID: 51285 RVA: 0x0049A314 File Offset: 0x00498514
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionCallGameEvent.PropGameEventNames))
			{
				this.gameEventNames.AddRange(properties.Values[ActionCallGameEvent.PropGameEventNames].Split(',', StringSplitOptions.None));
			}
			properties.ParseString(ActionCallGameEvent.PropTargetGroup, ref this.targetGroup);
		}

		// Token: 0x0600C856 RID: 51286 RVA: 0x0049A36E File Offset: 0x0049856E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionCallGameEvent
			{
				gameEventNames = this.gameEventNames,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x04009782 RID: 38786
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<string> gameEventNames = new List<string>();

		// Token: 0x04009783 RID: 38787
		[PublicizedFrom(EAccessModifier.Protected)]
		public string targetGroup = "";

		// Token: 0x04009784 RID: 38788
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGameEventNames = "game_events";

		// Token: 0x04009785 RID: 38789
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetGroup = "target_group";
	}
}
