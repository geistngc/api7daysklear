using System;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceLoops
{
	// Token: 0x0200194F RID: 6479
	[Preserve]
	public class LoopFor : BaseLoop
	{
		// Token: 0x0600C79F RID: 51103 RVA: 0x0049553C File Offset: 0x0049373C
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.loopCount == -1)
			{
				this.loopCount = GameEventManager.GetIntValue(base.Owner.Target as EntityAlive, this.loopCountText, 1);
			}
			if (base.HandleActions() == BaseAction.ActionCompleteStates.Complete)
			{
				this.currentLoop++;
				this.CurrentPhase = 0;
				for (int i = 0; i < this.Actions.Count; i++)
				{
					this.Actions[i].Reset();
				}
				if (this.currentLoop >= this.loopCount)
				{
					this.IsComplete = true;
					return BaseAction.ActionCompleteStates.Complete;
				}
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600C7A0 RID: 51104 RVA: 0x004955D1 File Offset: 0x004937D1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			base.OnReset();
			this.loopCount = -1;
			this.currentLoop = 0;
		}

		// Token: 0x0600C7A1 RID: 51105 RVA: 0x004955E7 File Offset: 0x004937E7
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(LoopFor.PropLoopCount, ref this.loopCountText);
		}

		// Token: 0x0600C7A2 RID: 51106 RVA: 0x00495601 File Offset: 0x00493801
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new LoopFor
			{
				loopCountText = this.loopCountText
			};
		}

		// Token: 0x04009688 RID: 38536
		[PublicizedFrom(EAccessModifier.Private)]
		public int loopCount = -1;

		// Token: 0x04009689 RID: 38537
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentLoop;

		// Token: 0x0400968A RID: 38538
		public string loopCountText;

		// Token: 0x0400968B RID: 38539
		public static string PropLoopCount = "loop_count";
	}
}
