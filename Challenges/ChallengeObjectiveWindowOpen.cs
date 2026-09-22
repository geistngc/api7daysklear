using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001919 RID: 6425
	[Preserve]
	public class ChallengeObjectiveWindowOpen : BaseChallengeObjective
	{
		// Token: 0x17001896 RID: 6294
		// (get) Token: 0x0600C66F RID: 50799 RVA: 0x000EDFFC File Offset: 0x000EC1FC
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.WindowOpen;
			}
		}

		// Token: 0x17001897 RID: 6295
		// (get) Token: 0x0600C670 RID: 50800 RVA: 0x00491B1B File Offset: 0x0048FD1B
		public override string DescriptionText
		{
			get
			{
				return string.Format(Localization.Get("ObjectiveOpenWindow_keyword", false, null), string.Format("[DECEA3]{0}[-]", Localization.Get("xui" + this.WindowName, false, null)));
			}
		}

		// Token: 0x0600C671 RID: 50801 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C672 RID: 50802 RVA: 0x00491B4F File Offset: 0x0048FD4F
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.WindowChanged -= this.Current_WindowChanged;
			QuestEventManager.Current.WindowChanged += this.Current_WindowChanged;
		}

		// Token: 0x0600C673 RID: 50803 RVA: 0x00491B7D File Offset: 0x0048FD7D
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_WindowChanged(string _windowName)
		{
			if (_windowName == "windowpaging")
			{
				return;
			}
			if (this.CheckBaseRequirements())
			{
				return;
			}
			this.currentOpenWindow = _windowName;
			this.HandleUpdatingCurrent();
			this.CheckObjectiveComplete(true);
			this.Parent.CheckPrerequisites();
		}

		// Token: 0x0600C674 RID: 50804 RVA: 0x00491BB6 File Offset: 0x0048FDB6
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.WindowChanged -= this.Current_WindowChanged;
		}

		// Token: 0x0600C675 RID: 50805 RVA: 0x00491BCE File Offset: 0x0048FDCE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleUpdatingCurrent()
		{
			base.HandleUpdatingCurrent();
			if (this.Owner != null)
			{
				base.Current = ((this.currentOpenWindow == this.WindowName) ? 1 : 0);
			}
		}

		// Token: 0x0600C676 RID: 50806 RVA: 0x00491BFB File Offset: 0x0048FDFB
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveWindowOpen
			{
				WindowName = this.WindowName
			};
		}

		// Token: 0x0600C677 RID: 50807 RVA: 0x0048D363 File Offset: 0x0048B563
		public override void CompleteObjective(bool handleComplete = true)
		{
			base.Current = this.MaxCount;
			base.Complete = true;
			if (handleComplete)
			{
				this.Owner.HandleComplete(true, false);
			}
		}

		// Token: 0x040095E6 RID: 38374
		public string WindowName = "";

		// Token: 0x040095E7 RID: 38375
		[PublicizedFrom(EAccessModifier.Private)]
		public string currentOpenWindow = "";

		// Token: 0x040095E8 RID: 38376
		public RequirementObjectiveGroupWindowOpen Parent;
	}
}
