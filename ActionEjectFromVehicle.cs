using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001980 RID: 6528
	[Preserve]
	public class ActionEjectFromVehicle : ActionBaseTargetAction
	{
		// Token: 0x0600C883 RID: 51331 RVA: 0x0049B6C0 File Offset: 0x004998C0
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null && entityPlayer.AttachedToEntity != null)
			{
				if (entityPlayer.isEntityRemote)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityPlayer.entityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
				}
				else
				{
					(entityPlayer as EntityPlayerLocal).PlayerUI.windowManager.CloseAllOpenModalWindows(null, false);
				}
				entityPlayer.SendDetach();
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C884 RID: 51332 RVA: 0x0049B746 File Offset: 0x00499946
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionEjectFromVehicle
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
