using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019C5 RID: 6597
	[Preserve]
	public class ActionShowMessageWindow : ActionBaseClientAction
	{
		// Token: 0x0600C9AE RID: 51630 RVA: 0x004A2917 File Offset: 0x004A0B17
		public override void OnClientPerform(Entity target)
		{
			if (target is EntityPlayerLocal)
			{
				XUiC_MessageBoxWindowGroup.ShowOk(LocalPlayerUI.GetUIForPrimaryPlayer().xui, Localization.Get(this.title, false, null), Localization.Get(this.message, false, null), "ui_game_symbol_pen", null, true, true, false);
			}
		}

		// Token: 0x0600C9AF RID: 51631 RVA: 0x004A2953 File Offset: 0x004A0B53
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionShowMessageWindow.PropMessage, ref this.message);
			properties.ParseString(ActionShowMessageWindow.PropTitle, ref this.title);
		}

		// Token: 0x0600C9B0 RID: 51632 RVA: 0x004A297E File Offset: 0x004A0B7E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionShowMessageWindow
			{
				targetGroup = this.targetGroup,
				message = this.message,
				title = this.title
			};
		}

		// Token: 0x04009941 RID: 39233
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PropMessage = "message";

		// Token: 0x04009942 RID: 39234
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PropTitle = "title";

		// Token: 0x04009943 RID: 39235
		[PublicizedFrom(EAccessModifier.Private)]
		public string message = "";

		// Token: 0x04009944 RID: 39236
		[PublicizedFrom(EAccessModifier.Private)]
		public string title = "";
	}
}
