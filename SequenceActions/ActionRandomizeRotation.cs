using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019A1 RID: 6561
	[Preserve]
	public class ActionRandomizeRotation : ActionBaseClientAction
	{
		// Token: 0x0600C8FE RID: 51454 RVA: 0x0049F428 File Offset: 0x0049D628
		public override void OnClientPerform(Entity target)
		{
			if (target != null)
			{
				Entity attachedToEntity = target.AttachedToEntity;
				float num = (float)GameEventManager.Current.Random.RandomRange(45, 315);
				if (attachedToEntity)
				{
					Transform physicsTransform = attachedToEntity.PhysicsTransform;
					Quaternion quaternion = physicsTransform.rotation;
					quaternion = Quaternion.AngleAxis(num, physicsTransform.up) * quaternion;
					attachedToEntity.SetRotation(quaternion.eulerAngles);
					physicsTransform.rotation = quaternion;
					EntityVehicle entityVehicle = attachedToEntity as EntityVehicle;
					if (entityVehicle)
					{
						entityVehicle.CameraChangeRotation(num);
						return;
					}
				}
				else
				{
					Vector3 rotation = target.rotation;
					rotation.y += num;
					target.SetRotation(rotation);
				}
			}
		}

		// Token: 0x0600C8FF RID: 51455 RVA: 0x0049F4D3 File Offset: 0x0049D6D3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRandomizeRotation
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
