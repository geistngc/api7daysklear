using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001975 RID: 6517
	[Preserve]
	public class ActionBeltTooltip : ActionBaseClientAction
	{
		// Token: 0x0600C84F RID: 51279 RVA: 0x0049A070 File Offset: 0x00498270
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				if (this.soundName != "")
				{
					GameManager.ShowTooltip(entityPlayerLocal, this.text, false, false, 0f);
					return;
				}
				GameManager.ShowTooltip(entityPlayerLocal, this.text, "", this.soundName, null, false, false, 0f);
			}
		}

		// Token: 0x0600C850 RID: 51280 RVA: 0x0049A0CC File Offset: 0x004982CC
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionBeltTooltip.PropText))
			{
				this.text = properties.Values[ActionBeltTooltip.PropText];
			}
			if (properties.Values.ContainsKey(ActionBeltTooltip.PropTextKey))
			{
				this.textKey = properties.Values[ActionBeltTooltip.PropTextKey];
				this.text = Localization.Get(this.textKey, false, null);
			}
			properties.ParseString(ActionBeltTooltip.PropSound, ref this.soundName);
		}

		// Token: 0x0600C851 RID: 51281 RVA: 0x0049A154 File Offset: 0x00498354
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBeltTooltip
			{
				targetGroup = this.targetGroup,
				textKey = this.textKey,
				text = this.text,
				soundName = this.soundName
			};
		}

		// Token: 0x0400977C RID: 38780
		[PublicizedFrom(EAccessModifier.Protected)]
		public string textKey = "Sequence Complete";

		// Token: 0x0400977D RID: 38781
		[PublicizedFrom(EAccessModifier.Protected)]
		public string text = "";

		// Token: 0x0400977E RID: 38782
		[PublicizedFrom(EAccessModifier.Protected)]
		public string soundName = "";

		// Token: 0x0400977F RID: 38783
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropText = "text";

		// Token: 0x04009780 RID: 38784
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTextKey = "text_key";

		// Token: 0x04009781 RID: 38785
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSound = "sound";
	}
}
