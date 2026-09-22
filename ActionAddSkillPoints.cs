using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001966 RID: 6502
	[Preserve]
	public class ActionAddSkillPoints : ActionBaseClientAction
	{
		// Token: 0x0600C808 RID: 51208 RVA: 0x004982F0 File Offset: 0x004964F0
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				int intValue = GameEventManager.GetIntValue(entityPlayer, this.skillPointsText, 1);
				if (intValue <= 0)
				{
					return;
				}
				entityPlayer.Progression.SkillPoints += intValue;
			}
		}

		// Token: 0x0600C809 RID: 51209 RVA: 0x0049832D File Offset: 0x0049652D
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddSkillPoints.PropSkillPoints, ref this.skillPointsText);
		}

		// Token: 0x0600C80A RID: 51210 RVA: 0x00498347 File Offset: 0x00496547
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddSkillPoints
			{
				skillPointsText = this.skillPointsText
			};
		}

		// Token: 0x04009717 RID: 38679
		[PublicizedFrom(EAccessModifier.Protected)]
		public string skillPointsText = "";

		// Token: 0x04009718 RID: 38680
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSkillPoints = "skill_points";
	}
}
