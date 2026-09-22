using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001974 RID: 6516
	[Preserve]
	public class ActionBaseTeleport : ActionBaseTargetAction
	{
		// Token: 0x0600C84A RID: 51274 RVA: 0x00499FFC File Offset: 0x004981FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public void TeleportEntity(Entity entity, Vector3 position)
		{
			float floatValue = GameEventManager.GetFloatValue(entity as EntityAlive, this.teleportDelayText, 0.1f);
			GameManager.Instance.StartCoroutine(base.TeleportEntity(entity, position, floatValue));
		}

		// Token: 0x0600C84B RID: 51275 RVA: 0x0049A034 File Offset: 0x00498234
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionBaseTeleport.PropTeleportDelay, ref this.teleportDelayText);
		}

		// Token: 0x0600C84C RID: 51276 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return null;
		}

		// Token: 0x0400977A RID: 38778
		[PublicizedFrom(EAccessModifier.Protected)]
		public string teleportDelayText = "";

		// Token: 0x0400977B RID: 38779
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTeleportDelay = "teleport_delay";
	}
}
