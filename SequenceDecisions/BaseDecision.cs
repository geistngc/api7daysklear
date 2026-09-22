using System;
using System.Collections.Generic;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceDecisions
{
	// Token: 0x02001957 RID: 6487
	[Preserve]
	public class BaseDecision : BaseAction
	{
		// Token: 0x1700189D RID: 6301
		// (get) Token: 0x0600C7C0 RID: 51136 RVA: 0x00010E62 File Offset: 0x0000F062
		public override bool UseRequirements
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600C7C1 RID: 51137 RVA: 0x00496694 File Offset: 0x00494894
		public override void SetActionKeyData(int _actionIndex, BaseAction _parent, string prefix = "")
		{
			base.SetActionKeyData(_actionIndex, _parent, prefix);
			for (int i = 0; i < this.Actions.Count; i++)
			{
				this.Actions[i].SetActionKeyData(i, this, "");
			}
		}

		// Token: 0x0600C7C2 RID: 51138 RVA: 0x004966D8 File Offset: 0x004948D8
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
				this.phaseMax = list[list.Count - 1] + 1;
				return;
			}
			this.phaseMax = 0;
		}

		// Token: 0x0600C7C3 RID: 51139 RVA: 0x00496764 File Offset: 0x00494964
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			base.OnReset();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				this.Actions[i].Reset();
			}
			this.currentPhase = 0;
		}

		// Token: 0x0600C7C4 RID: 51140 RVA: 0x004967A8 File Offset: 0x004949A8
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseAction.ActionCompleteStates HandleActions()
		{
			bool flag = false;
			int num = this.currentPhase;
			for (int i = 0; i < this.Actions.Count; i++)
			{
				if (this.Actions[i].Phase == this.currentPhase && !this.Actions[i].IsComplete)
				{
					flag = true;
					this.Actions[i].Owner = base.Owner;
					BaseAction.ActionCompleteStates actionCompleteStates = this.Actions[i].PerformAction();
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
				this.currentPhase++;
			}
			else if (this.currentPhase != num)
			{
				this.currentPhase = num;
				for (int j = 0; j < this.Actions.Count; j++)
				{
					if (this.Actions[j].Phase >= this.currentPhase)
					{
						this.Actions[j].Reset();
					}
				}
			}
			if (this.currentPhase >= this.phaseMax)
			{
				this.IsComplete = true;
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600C7C5 RID: 51141 RVA: 0x00496960 File Offset: 0x00494B60
		public override void HandleTemplateInit(GameEventActionSequence seq)
		{
			base.HandleTemplateInit(seq);
			for (int i = 0; i < this.Actions.Count; i++)
			{
				seq.HandleVariablesForProperties(this.Actions[i].Properties);
				this.Actions[i].ParseProperties(this.Actions[i].Properties);
			}
		}

		// Token: 0x0600C7C6 RID: 51142 RVA: 0x004969C4 File Offset: 0x00494BC4
		public override BaseAction Clone()
		{
			BaseDecision baseDecision = (BaseDecision)base.Clone();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				BaseAction baseAction = this.Actions[i].Clone();
				baseAction.Owner = base.Owner;
				baseDecision.Actions.Add(baseAction);
			}
			baseDecision.phaseMax = this.phaseMax;
			return baseDecision;
		}

		// Token: 0x040096B5 RID: 38581
		public List<BaseAction> Actions = new List<BaseAction>();

		// Token: 0x040096B6 RID: 38582
		[PublicizedFrom(EAccessModifier.Protected)]
		public int currentPhase;

		// Token: 0x040096B7 RID: 38583
		[PublicizedFrom(EAccessModifier.Protected)]
		public int phaseMax;
	}
}
