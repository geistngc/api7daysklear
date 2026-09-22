using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001978 RID: 6520
	[Preserve]
	public class ActionCloseWindow : ActionBaseClientAction
	{
		// Token: 0x0600C85E RID: 51294 RVA: 0x0049A424 File Offset: 0x00498624
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				if (this.windowName == "")
				{
					entityPlayerLocal.PlayerUI.windowManager.CloseAllOpenModalWindows(null, false);
					return;
				}
				entityPlayerLocal.PlayerUI.windowManager.Close(this.windowName);
			}
		}

		// Token: 0x0600C85F RID: 51295 RVA: 0x0049A477 File Offset: 0x00498677
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionCloseWindow.PropWindow, ref this.windowName);
		}

		// Token: 0x0600C860 RID: 51296 RVA: 0x0049A491 File Offset: 0x00498691
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionCloseWindow
			{
				targetGroup = this.targetGroup,
				windowName = this.windowName
			};
		}

		// Token: 0x04009788 RID: 38792
		[PublicizedFrom(EAccessModifier.Protected)]
		public string windowName = "";

		// Token: 0x04009789 RID: 38793
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropWindow = "window";
	}
}
