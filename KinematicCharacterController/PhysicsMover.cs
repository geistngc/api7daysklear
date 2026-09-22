using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D5F RID: 7519
	[RequireComponent(typeof(Rigidbody))]
	public class PhysicsMover : MonoBehaviour
	{
		// Token: 0x17001BB9 RID: 7097
		// (get) Token: 0x0600DE3C RID: 56892 RVA: 0x004FC638 File Offset: 0x004FA838
		// (set) Token: 0x0600DE3D RID: 56893 RVA: 0x004FC640 File Offset: 0x004FA840
		public int IndexInCharacterSystem { get; set; }

		// Token: 0x17001BBA RID: 7098
		// (get) Token: 0x0600DE3E RID: 56894 RVA: 0x004FC649 File Offset: 0x004FA849
		// (set) Token: 0x0600DE3F RID: 56895 RVA: 0x004FC651 File Offset: 0x004FA851
		public Vector3 InitialTickPosition { get; set; }

		// Token: 0x17001BBB RID: 7099
		// (get) Token: 0x0600DE40 RID: 56896 RVA: 0x004FC65A File Offset: 0x004FA85A
		// (set) Token: 0x0600DE41 RID: 56897 RVA: 0x004FC662 File Offset: 0x004FA862
		public Quaternion InitialTickRotation { get; set; }

		// Token: 0x17001BBC RID: 7100
		// (get) Token: 0x0600DE42 RID: 56898 RVA: 0x004FC66B File Offset: 0x004FA86B
		// (set) Token: 0x0600DE43 RID: 56899 RVA: 0x004FC673 File Offset: 0x004FA873
		public Transform Transform { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001BBD RID: 7101
		// (get) Token: 0x0600DE44 RID: 56900 RVA: 0x004FC67C File Offset: 0x004FA87C
		// (set) Token: 0x0600DE45 RID: 56901 RVA: 0x004FC684 File Offset: 0x004FA884
		public Vector3 InitialSimulationPosition { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001BBE RID: 7102
		// (get) Token: 0x0600DE46 RID: 56902 RVA: 0x004FC68D File Offset: 0x004FA88D
		// (set) Token: 0x0600DE47 RID: 56903 RVA: 0x004FC695 File Offset: 0x004FA895
		public Quaternion InitialSimulationRotation { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001BBF RID: 7103
		// (get) Token: 0x0600DE48 RID: 56904 RVA: 0x004FC69E File Offset: 0x004FA89E
		// (set) Token: 0x0600DE49 RID: 56905 RVA: 0x004FC6A6 File Offset: 0x004FA8A6
		public Vector3 TransientPosition
		{
			get
			{
				return this._internalTransientPosition;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._internalTransientPosition = value;
			}
		}

		// Token: 0x17001BC0 RID: 7104
		// (get) Token: 0x0600DE4A RID: 56906 RVA: 0x004FC6AF File Offset: 0x004FA8AF
		// (set) Token: 0x0600DE4B RID: 56907 RVA: 0x004FC6B7 File Offset: 0x004FA8B7
		public Quaternion TransientRotation
		{
			get
			{
				return this._internalTransientRotation;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._internalTransientRotation = value;
			}
		}

		// Token: 0x0600DE4C RID: 56908 RVA: 0x004FC6C0 File Offset: 0x004FA8C0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Reset()
		{
			this.ValidateData();
		}

		// Token: 0x0600DE4D RID: 56909 RVA: 0x004FC6C0 File Offset: 0x004FA8C0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnValidate()
		{
			this.ValidateData();
		}

		// Token: 0x0600DE4E RID: 56910 RVA: 0x004FC6C8 File Offset: 0x004FA8C8
		public void ValidateData()
		{
			this.Rigidbody = base.gameObject.GetComponent<Rigidbody>();
			this.Rigidbody.centerOfMass = Vector3.zero;
			this.Rigidbody.useGravity = false;
			this.Rigidbody.drag = 0f;
			this.Rigidbody.angularDrag = 0f;
			this.Rigidbody.maxAngularVelocity = float.PositiveInfinity;
			this.Rigidbody.maxDepenetrationVelocity = float.PositiveInfinity;
			this.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			this.Rigidbody.isKinematic = true;
			this.Rigidbody.constraints = RigidbodyConstraints.None;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}

		// Token: 0x0600DE4F RID: 56911 RVA: 0x004FC772 File Offset: 0x004FA972
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			KinematicCharacterSystem.EnsureCreation();
			KinematicCharacterSystem.RegisterPhysicsMover(this);
		}

		// Token: 0x0600DE50 RID: 56912 RVA: 0x004FC77F File Offset: 0x004FA97F
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			KinematicCharacterSystem.UnregisterPhysicsMover(this);
		}

		// Token: 0x0600DE51 RID: 56913 RVA: 0x004FC788 File Offset: 0x004FA988
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			this.Transform = base.transform;
			this.ValidateData();
			this.TransientPosition = this.Rigidbody.position;
			this.TransientRotation = this.Rigidbody.rotation;
			this.InitialSimulationPosition = this.Rigidbody.position;
			this.InitialSimulationRotation = this.Rigidbody.rotation;
		}

		// Token: 0x0600DE52 RID: 56914 RVA: 0x004FC7EB File Offset: 0x004FA9EB
		public void SetPosition(Vector3 position)
		{
			this.Transform.position = position;
			this.Rigidbody.position = position;
			this.InitialSimulationPosition = position;
			this.TransientPosition = position;
		}

		// Token: 0x0600DE53 RID: 56915 RVA: 0x004FC813 File Offset: 0x004FAA13
		public void SetRotation(Quaternion rotation)
		{
			this.Transform.rotation = rotation;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationRotation = rotation;
			this.TransientRotation = rotation;
		}

		// Token: 0x0600DE54 RID: 56916 RVA: 0x004FC83C File Offset: 0x004FAA3C
		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			this.Transform.SetPositionAndRotation(position, rotation);
			this.Rigidbody.position = position;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationPosition = position;
			this.InitialSimulationRotation = rotation;
			this.TransientPosition = position;
			this.TransientRotation = rotation;
		}

		// Token: 0x0600DE55 RID: 56917 RVA: 0x004FC88C File Offset: 0x004FAA8C
		public PhysicsMoverState GetState()
		{
			return new PhysicsMoverState
			{
				Position = this.TransientPosition,
				Rotation = this.TransientRotation,
				Velocity = this.Rigidbody.velocity,
				AngularVelocity = this.Rigidbody.velocity
			};
		}

		// Token: 0x0600DE56 RID: 56918 RVA: 0x004FC8E0 File Offset: 0x004FAAE0
		public void ApplyState(PhysicsMoverState state)
		{
			this.SetPositionAndRotation(state.Position, state.Rotation);
			this.Rigidbody.velocity = state.Velocity;
			this.Rigidbody.angularVelocity = state.AngularVelocity;
		}

		// Token: 0x0600DE57 RID: 56919 RVA: 0x004FC918 File Offset: 0x004FAB18
		public void VelocityUpdate(float deltaTime)
		{
			this.InitialSimulationPosition = this.TransientPosition;
			this.InitialSimulationRotation = this.TransientRotation;
			this.MoverController.UpdateMovement(out this._internalTransientPosition, out this._internalTransientRotation, deltaTime);
			if (deltaTime > 0f)
			{
				this.Rigidbody.velocity = (this.TransientPosition - this.InitialSimulationPosition) / deltaTime;
				Quaternion quaternion = this.TransientRotation * Quaternion.Inverse(this.InitialSimulationRotation);
				this.Rigidbody.angularVelocity = 0.017453292f * quaternion.eulerAngles / deltaTime;
			}
		}

		// Token: 0x0400A8E9 RID: 43241
		[ReadOnly]
		public Rigidbody Rigidbody;

		// Token: 0x0400A8EA RID: 43242
		[NonSerialized]
		public IMoverController MoverController;

		// Token: 0x0400A8F1 RID: 43249
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _internalTransientPosition;

		// Token: 0x0400A8F2 RID: 43250
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion _internalTransientRotation;
	}
}
