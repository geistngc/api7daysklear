using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001977 RID: 6519
	[Preserve]
	public class ActionClearGroup : BaseAction
	{
		// Token: 0x0600C859 RID: 51289 RVA: 0x0049A3C1 File Offset: 0x004985C1
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			base.Owner.ClearEntityGroup(this.groupName);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C85A RID: 51290 RVA: 0x0049A3D5 File Offset: 0x004985D5
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionClearGroup.PropGroupName, ref this.groupName);
		}

		// Token: 0x0600C85B RID: 51291 RVA: 0x0049A3EF File Offset: 0x004985EF
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionClearGroup
			{
				groupName = this.groupName
			};
		}

		// Token: 0x04009786 RID: 38790
		[PublicizedFrom(EAccessModifier.Protected)]
		public string groupName = "";

		// Token: 0x04009787 RID: 38791
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGroupName = "group_name";
	}
}
