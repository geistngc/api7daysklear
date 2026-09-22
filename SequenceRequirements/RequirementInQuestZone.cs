using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001939 RID: 6457
	[Preserve]
	public class RequirementInQuestZone : BaseRequirement
	{
		// Token: 0x0600C732 RID: 50994 RVA: 0x0049448C File Offset: 0x0049268C
		public override bool CanPerform(Entity target)
		{
			World world = GameManager.Instance.World;
			Vector3 position = target.position;
			position.y = position.z;
			if (QuestEventManager.Current.QuestBounds.Contains(position))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C733 RID: 50995 RVA: 0x004944DA File Offset: 0x004926DA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInQuestZone();
		}
	}
}
