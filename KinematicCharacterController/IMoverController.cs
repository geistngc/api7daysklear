using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D52 RID: 7506
	public interface IMoverController
	{
		// Token: 0x0600DDE2 RID: 56802
		void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime);
	}
}
