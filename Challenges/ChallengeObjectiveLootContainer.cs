using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200190D RID: 6413
	[Preserve]
	public class ChallengeObjectiveLootContainer : BaseChallengeObjective
	{
		// Token: 0x1700187A RID: 6266
		// (get) Token: 0x0600C603 RID: 50691 RVA: 0x004906EA File Offset: 0x0048E8EA
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.LootContainer;
			}
		}

		// Token: 0x1700187B RID: 6267
		// (get) Token: 0x0600C604 RID: 50692 RVA: 0x004906EE File Offset: 0x0048E8EE
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("ObjectiveLootContainer_keyword", false, null);
			}
		}

		// Token: 0x0600C605 RID: 50693 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C606 RID: 50694 RVA: 0x004906FC File Offset: 0x0048E8FC
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.ContainerOpened -= this.Current_ContainerOpened;
			QuestEventManager.Current.ContainerOpened += this.Current_ContainerOpened;
		}

		// Token: 0x0600C607 RID: 50695 RVA: 0x0049072C File Offset: 0x0048E92C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_ContainerOpened(Vector3i containerLocation, ITileEntityLootable tileEntity)
		{
			if (containerLocation == this.lastLocation)
			{
				return;
			}
			this.lastLocation = containerLocation;
			if (tileEntity.bWasTouched)
			{
				return;
			}
			if (this.CheckBaseRequirements())
			{
				return;
			}
			int num = base.Current;
			base.Current = num + 1;
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600C608 RID: 50696 RVA: 0x00490779 File Offset: 0x0048E979
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.ContainerOpened -= this.Current_ContainerOpened;
		}

		// Token: 0x0600C609 RID: 50697 RVA: 0x00490791 File Offset: 0x0048E991
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveLootContainer();
		}

		// Token: 0x040095BF RID: 38335
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3i lastLocation = Vector3i.zero;
	}
}
