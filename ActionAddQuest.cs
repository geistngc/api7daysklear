using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001964 RID: 6500
	[Preserve]
	public class ActionAddQuest : ActionBaseClientAction
	{
		// Token: 0x0600C7FE RID: 51198 RVA: 0x004980EC File Offset: 0x004962EC
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				Quest q = QuestClass.CreateQuest(this.QuestID);
				entityPlayer.QuestJournal.AddQuest(q, this.Notify);
			}
		}

		// Token: 0x0600C7FF RID: 51199 RVA: 0x00498121 File Offset: 0x00496321
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddQuest.PropQuestID, ref this.QuestID);
			properties.ParseBool(ActionAddQuest.PropNotify, ref this.Notify);
		}

		// Token: 0x0600C800 RID: 51200 RVA: 0x0049814C File Offset: 0x0049634C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddQuest
			{
				targetGroup = this.targetGroup,
				QuestID = this.QuestID,
				Notify = this.Notify
			};
		}

		// Token: 0x0400970D RID: 38669
		public string QuestID;

		// Token: 0x0400970E RID: 38670
		public bool Notify = true;

		// Token: 0x0400970F RID: 38671
		public static string PropQuestID = "quest";

		// Token: 0x04009710 RID: 38672
		public static string PropNotify = "notify";
	}
}
