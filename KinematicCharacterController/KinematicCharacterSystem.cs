using System;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D5D RID: 7517
	[DefaultExecutionOrder(-100)]
	public class KinematicCharacterSystem : MonoBehaviour
	{
		// Token: 0x0600DE2A RID: 56874 RVA: 0x004FC19C File Offset: 0x004FA39C
		public static void EnsureCreation()
		{
			if (KinematicCharacterSystem._instance == null)
			{
				GameObject gameObject = new GameObject("KinematicCharacterSystem");
				KinematicCharacterSystem._instance = gameObject.AddComponent<KinematicCharacterSystem>();
				gameObject.hideFlags = HideFlags.NotEditable;
				KinematicCharacterSystem._instance.hideFlags = HideFlags.NotEditable;
			}
		}

		// Token: 0x0600DE2B RID: 56875 RVA: 0x004FC1D1 File Offset: 0x004FA3D1
		public static KinematicCharacterSystem GetInstance()
		{
			return KinematicCharacterSystem._instance;
		}

		// Token: 0x0600DE2C RID: 56876 RVA: 0x004FC1D8 File Offset: 0x004FA3D8
		public static void SetCharacterMotorsCapacity(int capacity)
		{
			if (capacity < KinematicCharacterSystem.CharacterMotors.Count)
			{
				capacity = KinematicCharacterSystem.CharacterMotors.Count;
			}
			KinematicCharacterSystem.CharacterMotors.Capacity = capacity;
		}

		// Token: 0x0600DE2D RID: 56877 RVA: 0x004FC1FE File Offset: 0x004FA3FE
		public static void RegisterCharacterMotor(KinematicCharacterMotor motor)
		{
			KinematicCharacterSystem.CharacterMotors.Add(motor);
		}

		// Token: 0x0600DE2E RID: 56878 RVA: 0x004FC20B File Offset: 0x004FA40B
		public static void UnregisterCharacterMotor(KinematicCharacterMotor motor)
		{
			KinematicCharacterSystem.CharacterMotors.Remove(motor);
		}

		// Token: 0x0600DE2F RID: 56879 RVA: 0x004FC219 File Offset: 0x004FA419
		public static void SetPhysicsMoversCapacity(int capacity)
		{
			if (capacity < KinematicCharacterSystem.PhysicsMovers.Count)
			{
				capacity = KinematicCharacterSystem.PhysicsMovers.Count;
			}
			KinematicCharacterSystem.PhysicsMovers.Capacity = capacity;
		}

		// Token: 0x0600DE30 RID: 56880 RVA: 0x004FC23F File Offset: 0x004FA43F
		public static void RegisterPhysicsMover(PhysicsMover mover)
		{
			KinematicCharacterSystem.PhysicsMovers.Add(mover);
			mover.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}

		// Token: 0x0600DE31 RID: 56881 RVA: 0x004FC258 File Offset: 0x004FA458
		public static void UnregisterPhysicsMover(PhysicsMover mover)
		{
			KinematicCharacterSystem.PhysicsMovers.Remove(mover);
		}

		// Token: 0x0600DE32 RID: 56882 RVA: 0x00148373 File Offset: 0x00146573
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x0600DE33 RID: 56883 RVA: 0x004FC266 File Offset: 0x004FA466
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			KinematicCharacterSystem._instance = this;
		}

		// Token: 0x0600DE34 RID: 56884 RVA: 0x004FC270 File Offset: 0x004FA470
		[PublicizedFrom(EAccessModifier.Private)]
		public void FixedUpdate()
		{
			if (KinematicCharacterSystem.AutoSimulation)
			{
				float deltaTime = Time.deltaTime;
				if (KinematicCharacterSystem.Interpolate)
				{
					KinematicCharacterSystem.PreSimulationInterpolationUpdate(deltaTime);
				}
				KinematicCharacterSystem.Simulate(deltaTime, KinematicCharacterSystem.CharacterMotors, KinematicCharacterSystem.CharacterMotors.Count, KinematicCharacterSystem.PhysicsMovers, KinematicCharacterSystem.PhysicsMovers.Count);
				if (KinematicCharacterSystem.Interpolate)
				{
					KinematicCharacterSystem.PostSimulationInterpolationUpdate(deltaTime);
				}
			}
		}

		// Token: 0x0600DE35 RID: 56885 RVA: 0x004FC2C8 File Offset: 0x004FA4C8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			if (KinematicCharacterSystem.Interpolate)
			{
				KinematicCharacterSystem.CustomInterpolationUpdate();
			}
		}

		// Token: 0x0600DE36 RID: 56886 RVA: 0x004FC2D8 File Offset: 0x004FA4D8
		public static void PreSimulationInterpolationUpdate(float deltaTime)
		{
			for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
			{
				KinematicCharacterMotor kinematicCharacterMotor = KinematicCharacterSystem.CharacterMotors[i];
				kinematicCharacterMotor.InitialTickPosition = kinematicCharacterMotor.TransientPosition;
				kinematicCharacterMotor.InitialTickRotation = kinematicCharacterMotor.TransientRotation;
				kinematicCharacterMotor.Transform.SetPositionAndRotation(kinematicCharacterMotor.TransientPosition, kinematicCharacterMotor.TransientRotation);
			}
			for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
			{
				PhysicsMover physicsMover = KinematicCharacterSystem.PhysicsMovers[j];
				physicsMover.InitialTickPosition = physicsMover.TransientPosition;
				physicsMover.InitialTickRotation = physicsMover.TransientRotation;
				physicsMover.Transform.SetPositionAndRotation(physicsMover.TransientPosition, physicsMover.TransientRotation);
				physicsMover.Rigidbody.position = physicsMover.TransientPosition;
				physicsMover.Rigidbody.rotation = physicsMover.TransientRotation;
			}
		}

		// Token: 0x0600DE37 RID: 56887 RVA: 0x004FC3A8 File Offset: 0x004FA5A8
		public static void Simulate(float deltaTime, List<KinematicCharacterMotor> motors, int characterMotorsCount, List<PhysicsMover> movers, int physicsMoversCount)
		{
			for (int i = 0; i < physicsMoversCount; i++)
			{
				movers[i].VelocityUpdate(deltaTime);
			}
			for (int j = 0; j < characterMotorsCount; j++)
			{
				motors[j].UpdatePhase1(deltaTime);
			}
			for (int k = 0; k < physicsMoversCount; k++)
			{
				PhysicsMover physicsMover = movers[k];
				physicsMover.Transform.SetPositionAndRotation(physicsMover.TransientPosition, physicsMover.TransientRotation);
				physicsMover.Rigidbody.position = physicsMover.TransientPosition;
				physicsMover.Rigidbody.rotation = physicsMover.TransientRotation;
			}
			for (int l = 0; l < characterMotorsCount; l++)
			{
				KinematicCharacterMotor kinematicCharacterMotor = motors[l];
				kinematicCharacterMotor.UpdatePhase2(deltaTime);
				kinematicCharacterMotor.Transform.SetPositionAndRotation(kinematicCharacterMotor.TransientPosition, kinematicCharacterMotor.TransientRotation);
			}
			Physics.SyncTransforms();
		}

		// Token: 0x0600DE38 RID: 56888 RVA: 0x004FC478 File Offset: 0x004FA678
		public static void PostSimulationInterpolationUpdate(float deltaTime)
		{
			KinematicCharacterSystem._lastCustomInterpolationStartTime = Time.time;
			KinematicCharacterSystem._lastCustomInterpolationDeltaTime = deltaTime;
			for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
			{
				KinematicCharacterMotor kinematicCharacterMotor = KinematicCharacterSystem.CharacterMotors[i];
				kinematicCharacterMotor.Transform.SetPositionAndRotation(kinematicCharacterMotor.InitialTickPosition, kinematicCharacterMotor.InitialTickRotation);
			}
			for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
			{
				PhysicsMover physicsMover = KinematicCharacterSystem.PhysicsMovers[j];
				physicsMover.Rigidbody.position = physicsMover.InitialTickPosition;
				physicsMover.Rigidbody.rotation = physicsMover.InitialTickRotation;
				physicsMover.Rigidbody.MovePosition(physicsMover.TransientPosition);
				physicsMover.Rigidbody.MoveRotation(physicsMover.TransientRotation);
			}
		}

		// Token: 0x0600DE39 RID: 56889 RVA: 0x004FC534 File Offset: 0x004FA734
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CustomInterpolationUpdate()
		{
			float t = Mathf.Clamp01((Time.time - KinematicCharacterSystem._lastCustomInterpolationStartTime) / KinematicCharacterSystem._lastCustomInterpolationDeltaTime);
			for (int i = 0; i < KinematicCharacterSystem.CharacterMotors.Count; i++)
			{
				KinematicCharacterMotor kinematicCharacterMotor = KinematicCharacterSystem.CharacterMotors[i];
				kinematicCharacterMotor.Transform.SetPositionAndRotation(Vector3.Lerp(kinematicCharacterMotor.InitialTickPosition, kinematicCharacterMotor.TransientPosition, t), Quaternion.Slerp(kinematicCharacterMotor.InitialTickRotation, kinematicCharacterMotor.TransientRotation, t));
			}
			for (int j = 0; j < KinematicCharacterSystem.PhysicsMovers.Count; j++)
			{
				PhysicsMover physicsMover = KinematicCharacterSystem.PhysicsMovers[j];
				physicsMover.Transform.SetPositionAndRotation(Vector3.Lerp(physicsMover.InitialTickPosition, physicsMover.TransientPosition, t), Quaternion.Slerp(physicsMover.InitialTickRotation, physicsMover.TransientRotation, t));
			}
		}

		// Token: 0x0400A8DC RID: 43228
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static KinematicCharacterSystem _instance;

		// Token: 0x0400A8DD RID: 43229
		public static List<KinematicCharacterMotor> CharacterMotors = new List<KinematicCharacterMotor>(100);

		// Token: 0x0400A8DE RID: 43230
		public static List<PhysicsMover> PhysicsMovers = new List<PhysicsMover>(100);

		// Token: 0x0400A8DF RID: 43231
		public static bool AutoSimulation = true;

		// Token: 0x0400A8E0 RID: 43232
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static float _lastCustomInterpolationStartTime = -1f;

		// Token: 0x0400A8E1 RID: 43233
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static float _lastCustomInterpolationDeltaTime = -1f;

		// Token: 0x0400A8E2 RID: 43234
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int CharacterMotorsBaseCapacity = 100;

		// Token: 0x0400A8E3 RID: 43235
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int PhysicsMoversBaseCapacity = 100;

		// Token: 0x0400A8E4 RID: 43236
		public static bool Interpolate = true;
	}
}
