using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001921 RID: 6433
	[Preserve]
	public class RequirementObjectiveGroupWindowOpen : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600C6AD RID: 50861 RVA: 0x00493017 File Offset: 0x00491217
		public RequirementObjectiveGroupWindowOpen(string windowOpen)
		{
			this.WindowOpen = windowOpen;
		}

		// Token: 0x0600C6AE RID: 50862 RVA: 0x00493034 File Offset: 0x00491234
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			ChallengeObjectiveWindowOpen challengeObjectiveWindowOpen = new ChallengeObjectiveWindowOpen();
			challengeObjectiveWindowOpen.WindowName = this.WindowOpen;
			challengeObjectiveWindowOpen.Parent = this;
			challengeObjectiveWindowOpen.Owner = this.Owner;
			challengeObjectiveWindowOpen.IsRequirement = true;
			challengeObjectiveWindowOpen.Init();
			requirementGroupPhase.AddChallengeObjective(challengeObjectiveWindowOpen);
			this.PhaseList.Add(requirementGroupPhase);
		}

		// Token: 0x0600C6AF RID: 50863 RVA: 0x004930A0 File Offset: 0x004912A0
		public override bool HasPrerequisiteCondition()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			GUIWindow window = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player).windowManager.GetWindow(this.WindowOpen);
			return window != null && !window.isShowing;
		}

		// Token: 0x0600C6B0 RID: 50864 RVA: 0x004930F2 File Offset: 0x004912F2
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupWindowOpen(this.WindowOpen);
		}

		// Token: 0x040095FA RID: 38394
		public string WindowOpen = "";
	}
}
