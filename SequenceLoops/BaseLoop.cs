using System;
using System.Collections.Generic;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceLoops
{
	// Token: 0x0200194E RID: 6478
	[Preserve]
	public class BaseLoop : BaseAction
	{
		// Token: 0x0600C798 RID: 51096 RVA: 0x004951B4 File Offset: 0x004933B4
		public override void SetActionKeyData(int _actionIndex, BaseAction _parent, string prefix = "")
		{
			base.SetActionKeyData(_actionIndex, _parent, prefix);
			for (int i = 0; i < this.Actions.Count; i++)
			{
				this.Actions[i].SetActionKeyData(i, this, "");
			}
		}

		// Token: 0x0600C799 RID: 51097 RVA: 0x004951F8 File Offset: 0x004933F8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			base.OnInit();
			List<int> list = new List<int>();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				if (!list.Contains(this.Actions[i].Phase))
				{
					list.Add(this.Actions[i].Phase);
				}
			}
			list.Sort();
			if (list.Count > 0)
			{
				this.PhaseMax = list[list.Count - 1] + 1;
				return;
			}
			this.PhaseMax = 0;
		}

		// Token: 0x0600C79A RID: 51098 RVA: 0x00495284 File Offset: 0x00493484
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseAction.ActionCompleteStates HandleActions()
		{
			bool flag = false;
			int num = this.CurrentPhase;
			for (int i = 0; i < this.Actions.Count; i++)
			{
				if (this.Actions[i].Phase == this.CurrentPhase && !this.Actions[i].IsComplete)
				{
					BaseAction.ActionCompleteStates actionCompleteStates = BaseAction.ActionCompleteStates.InComplete;
					flag = true;
					if (!this.Actions[i].IsComplete)
					{
						this.Actions[i].Owner = base.Owner;
						actionCompleteStates = this.Actions[i].PerformAction();
					}
					if (actionCompleteStates == BaseAction.ActionCompleteStates.Complete || (actionCompleteStates == BaseAction.ActionCompleteStates.InCompleteRefund && this.Actions[i].IgnoreRefund))
					{
						this.Actions[i].IsComplete = true;
						if (this.Actions[i].PhaseOnComplete != -1)
						{
							num = this.Actions[i].PhaseOnComplete;
						}
					}
					else if (actionCompleteStates == BaseAction.ActionCompleteStates.RequirementsNotMet)
					{
						this.Actions[i].IsComplete = true;
						if (this.Actions[i].PhaseOnDenied != -1)
						{
							num = this.Actions[i].PhaseOnDenied;
						}
					}
					else if (base.Owner.AllowRefunds && actionCompleteStates == BaseAction.ActionCompleteStates.InCompleteRefund)
					{
						return BaseAction.ActionCompleteStates.InCompleteRefund;
					}
				}
			}
			if (!flag)
			{
				this.CurrentPhase++;
			}
			else if (this.CurrentPhase != num)
			{
				this.CurrentPhase = num;
				for (int j = 0; j < this.Actions.Count; j++)
				{
					if (this.Actions[j].Phase >= this.CurrentPhase)
					{
						this.Actions[j].Reset();
					}
				}
			}
			if (this.CurrentPhase >= this.PhaseMax)
			{
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600C79B RID: 51099 RVA: 0x00495448 File Offset: 0x00493648
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			base.OnReset();
			this.CurrentPhase = 0;
			for (int i = 0; i < this.Actions.Count; i++)
			{
				this.Actions[i].Reset();
			}
		}

		// Token: 0x0600C79C RID: 51100 RVA: 0x0049548C File Offset: 0x0049368C
		public override void HandleTemplateInit(GameEventActionSequence seq)
		{
			base.HandleTemplateInit(seq);
			for (int i = 0; i < this.Actions.Count; i++)
			{
				this.Actions[i].HandleTemplateInit(seq);
			}
		}

		// Token: 0x0600C79D RID: 51101 RVA: 0x004954C8 File Offset: 0x004936C8
		public override BaseAction Clone()
		{
			BaseLoop baseLoop = (BaseLoop)base.Clone();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				baseLoop.Actions.Add(this.Actions[i].Clone());
			}
			baseLoop.PhaseMax = this.PhaseMax;
			return baseLoop;
		}

		// Token: 0x04009685 RID: 38533
		public List<BaseAction> Actions = new List<BaseAction>();

		// Token: 0x04009686 RID: 38534
		public int PhaseMax = 1;

		// Token: 0x04009687 RID: 38535
		public int CurrentPhase;
	}
}
