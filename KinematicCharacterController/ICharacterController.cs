using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D51 RID: 7505
	public interface ICharacterController
	{
		// Token: 0x0600DDD6 RID: 56790
		void UpdateRotation(ref Quaternion currentRotation, float deltaTime);

		// Token: 0x0600DDD7 RID: 56791
		void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);

		// Token: 0x0600DDD8 RID: 56792
		void BeforeCharacterUpdate(float deltaTime);

		// Token: 0x0600DDD9 RID: 56793
		void PostGroundingUpdate(float deltaTime);

		// Token: 0x0600DDDA RID: 56794
		void AfterCharacterUpdate(float deltaTime);

		// Token: 0x0600DDDB RID: 56795
		bool IsColliderValidForCollisions(Collider coll);

		// Token: 0x0600DDDC RID: 56796
		void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600DDDD RID: 56797
		void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600DDDE RID: 56798
		void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600DDDF RID: 56799
		void OnDiscreteCollisionDetected(Collider hitCollider);

		// Token: 0x0600DDE0 RID: 56800
		bool OnCollisionOverlap(int nbOverlaps, Collider[] _internalProbedColliders);

		// Token: 0x0600DDE1 RID: 56801
		float GetCollisionOverlapScale(Transform overlappedTransform);
	}
}
