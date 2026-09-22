using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001944 RID: 6468
	[Preserve]
	public class RequirementOnQuest : BaseRequirement
	{
		// Token: 0x0600C758 RID: 51032 RVA: 0x00494A48 File Offset: 0x00492C48
		public override bool CanPerform(Entity target)
		{
			bool flag = false;
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer == null)
			{
				return false;
			}
			if (this.QuestID == "")
			{
				if (entityPlayer.QuestJournal.ActiveQuest != null || entityPlayer.QuestJournal.FindActiveQuest() != null)
				{
					flag = true;
				}
			}
			else if (entityPlayer.QuestJournal.FindActiveQuest(this.QuestID, -1) != null)
			{
				flag = true;
			}
			if (!this.Invert)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0600C759 RID: 51033 RVA: 0x00494AB7 File Offset: 0x00492CB7
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementOnQuest.PropQuest, ref this.QuestID);
		}

		// Token: 0x0600C75A RID: 51034 RVA: 0x00494AD1 File Offset: 0x00492CD1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementOnQuest
			{
				Invert = this.Invert,
				QuestID = this.QuestID
			};
		}

		// Token: 0x0400965F RID: 38495
		[PublicizedFrom(EAccessModifier.Private)]
		public string QuestID = "";

		// Token: 0x04009660 RID: 38496
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropQuest = "quest";
	}
}
