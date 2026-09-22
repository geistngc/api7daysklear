using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019C6 RID: 6598
	[Preserve]
	public class ActionShowWindow : ActionBaseClientAction
	{
		// Token: 0x0600C9B3 RID: 51635 RVA: 0x004A29E0 File Offset: 0x004A0BE0
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.PlayerUI.windowManager.Open(this.window, true);
			}
		}

		// Token: 0x0600C9B4 RID: 51636 RVA: 0x004A2A0E File Offset: 0x004A0C0E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionShowWindow.PropWindow, ref this.window);
		}

		// Token: 0x0600C9B5 RID: 51637 RVA: 0x004A2A28 File Offset: 0x004A0C28
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionShowWindow
			{
				targetGroup = this.targetGroup,
				window = this.window
			};
		}

		// Token: 0x04009945 RID: 39237
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PropWindow = "window";

		// Token: 0x04009946 RID: 39238
		[PublicizedFrom(EAccessModifier.Private)]
		public string window = "";
	}
}
