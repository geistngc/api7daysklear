using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D5C RID: 7516
	[RequireComponent(typeof(CapsuleCollider))]
	public class KinematicCharacterMotor : MonoBehaviour
	{
		// Token: 0x17001BA7 RID: 7079
		// (get) Token: 0x0600DDE6 RID: 56806 RVA: 0x004F95A9 File Offset: 0x004F77A9
		public Transform Transform
		{
			get
			{
				return this._transform;
			}
		}

		// Token: 0x17001BA8 RID: 7080
		// (get) Token: 0x0600DDE7 RID: 56807 RVA: 0x004F95B1 File Offset: 0x004F77B1
		public Vector3 TransientPosition
		{
			get
			{
				return this._transientPosition;
			}
		}

		// Token: 0x17001BA9 RID: 7081
		// (get) Token: 0x0600DDE8 RID: 56808 RVA: 0x004F95B9 File Offset: 0x004F77B9
		public Vector3 CharacterUp
		{
			get
			{
				return this._characterUp;
			}
		}

		// Token: 0x17001BAA RID: 7082
		// (get) Token: 0x0600DDE9 RID: 56809 RVA: 0x004F95C1 File Offset: 0x004F77C1
		public Vector3 CharacterForward
		{
			get
			{
				return this._characterForward;
			}
		}

		// Token: 0x17001BAB RID: 7083
		// (get) Token: 0x0600DDEA RID: 56810 RVA: 0x004F95C9 File Offset: 0x004F77C9
		public Vector3 CharacterRight
		{
			get
			{
				return this._characterRight;
			}
		}

		// Token: 0x17001BAC RID: 7084
		// (get) Token: 0x0600DDEB RID: 56811 RVA: 0x004F95D1 File Offset: 0x004F77D1
		public Vector3 InitialSimulationPosition
		{
			get
			{
				return this._initialSimulationPosition;
			}
		}

		// Token: 0x17001BAD RID: 7085
		// (get) Token: 0x0600DDEC RID: 56812 RVA: 0x004F95D9 File Offset: 0x004F77D9
		public Quaternion InitialSimulationRotation
		{
			get
			{
				return this._initialSimulationRotation;
			}
		}

		// Token: 0x17001BAE RID: 7086
		// (get) Token: 0x0600DDED RID: 56813 RVA: 0x004F95E1 File Offset: 0x004F77E1
		public Rigidbody AttachedRigidbody
		{
			get
			{
				return this._attachedRigidbody;
			}
		}

		// Token: 0x17001BAF RID: 7087
		// (get) Token: 0x0600DDEE RID: 56814 RVA: 0x004F95E9 File Offset: 0x004F77E9
		public Vector3 CharacterTransformToCapsuleCenter
		{
			get
			{
				return this._characterTransformToCapsuleCenter;
			}
		}

		// Token: 0x17001BB0 RID: 7088
		// (get) Token: 0x0600DDEF RID: 56815 RVA: 0x004F95F1 File Offset: 0x004F77F1
		public Vector3 CharacterTransformToCapsuleBottom
		{
			get
			{
				return this._characterTransformToCapsuleBottom;
			}
		}

		// Token: 0x17001BB1 RID: 7089
		// (get) Token: 0x0600DDF0 RID: 56816 RVA: 0x004F95F9 File Offset: 0x004F77F9
		public Vector3 CharacterTransformToCapsuleTop
		{
			get
			{
				return this._characterTransformToCapsuleTop;
			}
		}

		// Token: 0x17001BB2 RID: 7090
		// (get) Token: 0x0600DDF1 RID: 56817 RVA: 0x004F9601 File Offset: 0x004F7801
		public Vector3 CharacterTransformToCapsuleBottomHemi
		{
			get
			{
				return this._characterTransformToCapsuleBottomHemi;
			}
		}

		// Token: 0x17001BB3 RID: 7091
		// (get) Token: 0x0600DDF2 RID: 56818 RVA: 0x004F9609 File Offset: 0x004F7809
		public Vector3 CharacterTransformToCapsuleTopHemi
		{
			get
			{
				return this._characterTransformToCapsuleTopHemi;
			}
		}

		// Token: 0x17001BB4 RID: 7092
		// (get) Token: 0x0600DDF3 RID: 56819 RVA: 0x004F9611 File Offset: 0x004F7811
		public Vector3 AttachedRigidbodyVelocity
		{
			get
			{
				return this._attachedRigidbodyVelocity;
			}
		}

		// Token: 0x17001BB5 RID: 7093
		// (get) Token: 0x0600DDF4 RID: 56820 RVA: 0x004F9619 File Offset: 0x004F7819
		public int OverlapsCount
		{
			get
			{
				return this._overlapsCount;
			}
		}

		// Token: 0x17001BB6 RID: 7094
		// (get) Token: 0x0600DDF5 RID: 56821 RVA: 0x004F9621 File Offset: 0x004F7821
		public OverlapResult[] Overlaps
		{
			get
			{
				return this._overlaps;
			}
		}

		// Token: 0x17001BB7 RID: 7095
		// (get) Token: 0x0600DDF6 RID: 56822 RVA: 0x004F9629 File Offset: 0x004F7829
		// (set) Token: 0x0600DDF7 RID: 56823 RVA: 0x004F9634 File Offset: 0x004F7834
		public Quaternion TransientRotation
		{
			get
			{
				return this._transientRotation;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._transientRotation = value;
				this._characterUp = this._transientRotation * this._cachedWorldUp;
				this._characterForward = this._transientRotation * this._cachedWorldForward;
				this._characterRight = this._transientRotation * this._cachedWorldRight;
			}
		}

		// Token: 0x17001BB8 RID: 7096
		// (get) Token: 0x0600DDF8 RID: 56824 RVA: 0x004F968D File Offset: 0x004F788D
		public Vector3 Velocity
		{
			get
			{
				return this.BaseVelocity + this._attachedRigidbodyVelocity;
			}
		}

		// Token: 0x0600DDF9 RID: 56825 RVA: 0x004F96A0 File Offset: 0x004F78A0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			KinematicCharacterSystem.EnsureCreation();
			KinematicCharacterSystem.RegisterCharacterMotor(this);
		}

		// Token: 0x0600DDFA RID: 56826 RVA: 0x004F96AD File Offset: 0x004F78AD
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			KinematicCharacterSystem.UnregisterCharacterMotor(this);
		}

		// Token: 0x0600DDFB RID: 56827 RVA: 0x004F96B5 File Offset: 0x004F78B5
		[PublicizedFrom(EAccessModifier.Private)]
		public void Reset()
		{
			this.ValidateData();
		}

		// Token: 0x0600DDFC RID: 56828 RVA: 0x004F96B5 File Offset: 0x004F78B5
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnValidate()
		{
			this.ValidateData();
		}

		// Token: 0x0600DDFD RID: 56829 RVA: 0x004F96BD File Offset: 0x004F78BD
		[ContextMenu("Remove Component")]
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleRemoveComponent()
		{
			UnityEngine.Object component = base.gameObject.GetComponent<CapsuleCollider>();
			UnityEngine.Object.DestroyImmediate(this);
			UnityEngine.Object.DestroyImmediate(component);
		}

		// Token: 0x0600DDFE RID: 56830 RVA: 0x004F96D8 File Offset: 0x004F78D8
		public void ValidateData()
		{
			if (base.GetComponent<Rigidbody>())
			{
				base.GetComponent<Rigidbody>().hideFlags = HideFlags.None;
			}
			this.Capsule = base.GetComponent<CapsuleCollider>();
			this.CapsuleRadius = Mathf.Clamp(this.CapsuleRadius, 0f, this.CapsuleHeight * 0.5f);
			this.Capsule.isTrigger = false;
			this.Capsule.direction = 1;
			this.Capsule.sharedMaterial = this.CapsulePhysicsMaterial;
			this.SetCapsuleDimensions(this.CapsuleRadius, this.CapsuleHeight, this.CapsuleYOffset);
			this.MaxStepHeight = Mathf.Clamp(this.MaxStepHeight, 0f, float.PositiveInfinity);
			this.MinRequiredStepDepth = Mathf.Clamp(this.MinRequiredStepDepth, 0f, this.CapsuleRadius);
			this.MaxStableDistanceFromLedge = Mathf.Clamp(this.MaxStableDistanceFromLedge, 0f, this.CapsuleRadius);
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x0600DDFF RID: 56831 RVA: 0x004F97D0 File Offset: 0x004F79D0
		public void SetCapsuleCollisionsActivation(bool collisionsActive)
		{
			this.Capsule.isTrigger = !collisionsActive;
		}

		// Token: 0x0600DE00 RID: 56832 RVA: 0x004F97E1 File Offset: 0x004F79E1
		public void SetMovementCollisionsSolvingActivation(bool movementCollisionsSolvingActive)
		{
			this._solveMovementCollisions = movementCollisionsSolvingActive;
		}

		// Token: 0x0600DE01 RID: 56833 RVA: 0x004F97EA File Offset: 0x004F79EA
		public void SetGroundSolvingActivation(bool stabilitySolvingActive)
		{
			this._solveGrounding = stabilitySolvingActive;
		}

		// Token: 0x0600DE02 RID: 56834 RVA: 0x004F97F3 File Offset: 0x004F79F3
		public void SetPosition(Vector3 position, bool bypassInterpolation = true)
		{
			this._transform.position = position;
			this._initialSimulationPosition = position;
			this._transientPosition = position;
			if (bypassInterpolation)
			{
				this.InitialTickPosition = position;
			}
		}

		// Token: 0x0600DE03 RID: 56835 RVA: 0x004F9819 File Offset: 0x004F7A19
		public void SetRotation(Quaternion rotation, bool bypassInterpolation = true)
		{
			this._transform.rotation = rotation;
			this._initialSimulationRotation = rotation;
			this.TransientRotation = rotation;
			if (bypassInterpolation)
			{
				this.InitialTickRotation = rotation;
			}
		}

		// Token: 0x0600DE04 RID: 56836 RVA: 0x004F983F File Offset: 0x004F7A3F
		public void SetPositionAndRotation(Vector3 position, Quaternion rotation, bool bypassInterpolation = true)
		{
			this._transform.SetPositionAndRotation(position, rotation);
			this._initialSimulationPosition = position;
			this._initialSimulationRotation = rotation;
			this._transientPosition = position;
			this.TransientRotation = rotation;
			if (bypassInterpolation)
			{
				this.InitialTickPosition = position;
				this.InitialTickRotation = rotation;
			}
		}

		// Token: 0x0600DE05 RID: 56837 RVA: 0x004F987B File Offset: 0x004F7A7B
		public void MoveCharacter(Vector3 toPosition)
		{
			this._movePositionDirty = true;
			this._movePositionTarget = toPosition;
		}

		// Token: 0x0600DE06 RID: 56838 RVA: 0x004F988B File Offset: 0x004F7A8B
		public void RotateCharacter(Quaternion toRotation)
		{
			this._moveRotationDirty = true;
			this._moveRotationTarget = toRotation;
		}

		// Token: 0x0600DE07 RID: 56839 RVA: 0x004F989C File Offset: 0x004F7A9C
		public KinematicCharacterMotorState GetState()
		{
			KinematicCharacterMotorState result = default(KinematicCharacterMotorState);
			result.Position = this._transientPosition;
			result.Rotation = this._transientRotation;
			result.BaseVelocity = this.BaseVelocity;
			result.AttachedRigidbodyVelocity = this._attachedRigidbodyVelocity;
			result.MustUnground = this._mustUnground;
			result.MustUngroundTime = this._mustUngroundTimeCounter;
			result.LastMovementIterationFoundAnyGround = this.LastMovementIterationFoundAnyGround;
			result.GroundingStatus.CopyFrom(this.GroundingStatus);
			result.AttachedRigidbody = this._attachedRigidbody;
			return result;
		}

		// Token: 0x0600DE08 RID: 56840 RVA: 0x004F992C File Offset: 0x004F7B2C
		public void ApplyState(KinematicCharacterMotorState state, bool bypassInterpolation = true)
		{
			this.SetPositionAndRotation(state.Position, state.Rotation, bypassInterpolation);
			this.BaseVelocity = state.BaseVelocity;
			this._attachedRigidbodyVelocity = state.AttachedRigidbodyVelocity;
			this._mustUnground = state.MustUnground;
			this._mustUngroundTimeCounter = state.MustUngroundTime;
			this.LastMovementIterationFoundAnyGround = state.LastMovementIterationFoundAnyGround;
			this.GroundingStatus.CopyFrom(state.GroundingStatus);
			this._attachedRigidbody = state.AttachedRigidbody;
		}

		// Token: 0x0600DE09 RID: 56841 RVA: 0x004F99A8 File Offset: 0x004F7BA8
		public void SetCapsuleDimensions(float radius, float height, float yOffset)
		{
			this.CapsuleRadius = radius;
			this.CapsuleHeight = height;
			this.CapsuleYOffset = yOffset;
			this.Capsule.radius = this.CapsuleRadius;
			this.Capsule.height = Mathf.Clamp(this.CapsuleHeight, this.CapsuleRadius * 2f, this.CapsuleHeight);
			this.Capsule.center = new Vector3(0f, this.CapsuleYOffset, 0f);
			this._characterTransformToCapsuleCenter = this.Capsule.center;
			this._characterTransformToCapsuleBottom = this.Capsule.center + -this._cachedWorldUp * (this.Capsule.height * 0.5f);
			this._characterTransformToCapsuleTop = this.Capsule.center + this._cachedWorldUp * (this.Capsule.height * 0.5f);
			this._characterTransformToCapsuleBottomHemi = this.Capsule.center + -this._cachedWorldUp * (this.Capsule.height * 0.5f) + this._cachedWorldUp * this.Capsule.radius;
			this._characterTransformToCapsuleTopHemi = this.Capsule.center + this._cachedWorldUp * (this.Capsule.height * 0.5f) + -this._cachedWorldUp * this.Capsule.radius;
		}

		// Token: 0x0600DE0A RID: 56842 RVA: 0x004F9B44 File Offset: 0x004F7D44
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			this._transform = base.transform;
			this.ValidateData();
			this._transientPosition = this._transform.position;
			this.TransientRotation = this._transform.rotation;
			this.CollidableLayers = 0;
			for (int i = 0; i < 32; i++)
			{
				if (!Physics.GetIgnoreLayerCollision(base.gameObject.layer, i))
				{
					this.CollidableLayers |= 1 << i;
				}
			}
			this.SetCapsuleDimensions(this.CapsuleRadius, this.CapsuleHeight, this.CapsuleYOffset);
		}

		// Token: 0x0600DE0B RID: 56843 RVA: 0x004F9BE8 File Offset: 0x004F7DE8
		public void UpdatePhase1(float deltaTime)
		{
			if (float.IsNaN(this.BaseVelocity.x) || float.IsNaN(this.BaseVelocity.y) || float.IsNaN(this.BaseVelocity.z))
			{
				this.BaseVelocity = Vector3.zero;
			}
			if (float.IsNaN(this._attachedRigidbodyVelocity.x) || float.IsNaN(this._attachedRigidbodyVelocity.y) || float.IsNaN(this._attachedRigidbodyVelocity.z))
			{
				this._attachedRigidbodyVelocity = Vector3.zero;
			}
			this.CharacterController.BeforeCharacterUpdate(deltaTime);
			this._transientPosition = this._transform.position;
			this.TransientRotation = this._transform.rotation;
			this._initialSimulationPosition = this._transientPosition;
			this._initialSimulationRotation = this._transientRotation;
			this._rigidbodyProjectionHitCount = 0;
			this._overlapsCount = 0;
			this._lastSolvedOverlapNormalDirty = false;
			if (this._movePositionDirty)
			{
				if (this._solveMovementCollisions)
				{
					if (this.InternalCharacterMove(this._movePositionTarget - this._transientPosition, deltaTime, out this._internalResultingMovementMagnitude, out this._internalResultingMovementDirection) && this.InteractiveRigidbodyHandling)
					{
						Vector3 zero = Vector3.zero;
						this.ProcessVelocityForRigidbodyHits(ref zero, deltaTime);
					}
				}
				else
				{
					this._transientPosition = this._movePositionTarget;
				}
				this._movePositionDirty = false;
			}
			this.LastGroundingStatus.CopyFrom(this.GroundingStatus);
			this.GroundingStatus = default(CharacterGroundingReport);
			this.GroundingStatus.GroundNormal = this._characterUp;
			if (this._solveMovementCollisions)
			{
				Vector3 vector = this._cachedWorldUp;
				float num = 0f;
				int num2 = 0;
				bool flag = false;
				while (num2 < 3 && !flag)
				{
					int num3 = this.CharacterCollisionsOverlap(this._transientPosition, this._transientRotation, this._internalProbedColliders, 0f, false);
					if (num3 > 0)
					{
						if (!this.CharacterController.OnCollisionOverlap(num3, this._internalProbedColliders))
						{
							break;
						}
						int i = 0;
						while (i < num3)
						{
							Transform component = this._internalProbedColliders[i].GetComponent<Transform>();
							if (Physics.ComputePenetration(this.Capsule, this._transientPosition, this._transientRotation, this._internalProbedColliders[i], component.position, component.rotation, out vector, out num))
							{
								HitStabilityReport hitStabilityReport = new HitStabilityReport
								{
									IsStable = this.IsStableOnNormal(vector)
								};
								vector = this.GetObstructionNormal(vector, hitStabilityReport);
								num *= this.CharacterController.GetCollisionOverlapScale(component);
								Vector3 b = vector * (num + 0.001f);
								this._transientPosition += b;
								if (this._overlapsCount < this._overlaps.Length)
								{
									this._overlaps[this._overlapsCount] = new OverlapResult(vector, this._internalProbedColliders[i]);
									this._overlapsCount++;
									break;
								}
								break;
							}
							else
							{
								i++;
							}
						}
					}
					else
					{
						flag = true;
					}
					num2++;
				}
			}
			if (this._solveGrounding)
			{
				if (this.MustUnground())
				{
					this._transientPosition += this._characterUp * 0.0075f;
				}
				else
				{
					float num4 = 0.005f;
					if (!this.LastGroundingStatus.SnappingPrevented && (this.LastGroundingStatus.IsStableOnGround || this.LastMovementIterationFoundAnyGround))
					{
						if (this.StepHandling != StepHandlingMethod.None)
						{
							num4 = Mathf.Max(this.CapsuleRadius, this.MaxStepHeight);
						}
						else
						{
							num4 = this.CapsuleRadius;
						}
						num4 += this.GroundDetectionExtraDistance;
					}
					this.ProbeGround(ref this._transientPosition, this._transientRotation, num4, ref this.GroundingStatus);
				}
			}
			this.LastMovementIterationFoundAnyGround = false;
			if (this._mustUngroundTimeCounter > 0f)
			{
				this._mustUngroundTimeCounter -= deltaTime;
			}
			this._mustUnground = false;
			if (this._solveGrounding)
			{
				this.CharacterController.PostGroundingUpdate(deltaTime);
			}
			if (this.InteractiveRigidbodyHandling)
			{
				this._lastAttachedRigidbody = this._attachedRigidbody;
				if (this.AttachedRigidbodyOverride)
				{
					this._attachedRigidbody = this.AttachedRigidbodyOverride;
				}
				else if (this.GroundingStatus.IsStableOnGround && this.GroundingStatus.GroundCollider.attachedRigidbody)
				{
					Rigidbody interactiveRigidbody = this.GetInteractiveRigidbody(this.GroundingStatus.GroundCollider);
					if (interactiveRigidbody)
					{
						this._attachedRigidbody = interactiveRigidbody;
					}
				}
				else
				{
					this._attachedRigidbody = null;
				}
				Vector3 vector2 = Vector3.zero;
				if (this._attachedRigidbody)
				{
					vector2 = this.GetVelocityFromRigidbodyMovement(this._attachedRigidbody, this._transientPosition, deltaTime);
				}
				if (this.PreserveAttachedRigidbodyMomentum && this._lastAttachedRigidbody != null && this._attachedRigidbody != this._lastAttachedRigidbody)
				{
					this.BaseVelocity += this._attachedRigidbodyVelocity;
					this.BaseVelocity -= vector2;
				}
				this._attachedRigidbodyVelocity = this._cachedZeroVector;
				if (this._attachedRigidbody)
				{
					this._attachedRigidbodyVelocity = vector2;
					Vector3 normalized = Vector3.ProjectOnPlane(Quaternion.Euler(57.29578f * this._attachedRigidbody.angularVelocity * deltaTime) * this._characterForward, this._characterUp).normalized;
					this.TransientRotation = Quaternion.LookRotation(normalized, this._characterUp);
				}
				if (this.GroundingStatus.GroundCollider && this.GroundingStatus.GroundCollider.attachedRigidbody && this.GroundingStatus.GroundCollider.attachedRigidbody == this._attachedRigidbody && this._attachedRigidbody != null && this._lastAttachedRigidbody == null)
				{
					this.BaseVelocity -= Vector3.ProjectOnPlane(this._attachedRigidbodyVelocity, this._characterUp);
				}
				if (this._attachedRigidbodyVelocity.sqrMagnitude > 0f)
				{
					this._isMovingFromAttachedRigidbody = true;
					if (this._solveMovementCollisions)
					{
						if (this.InternalCharacterMove(this._attachedRigidbodyVelocity * deltaTime, deltaTime, out this._internalResultingMovementMagnitude, out this._internalResultingMovementDirection))
						{
							this._attachedRigidbodyVelocity = this._internalResultingMovementDirection * this._internalResultingMovementMagnitude / deltaTime;
						}
						else
						{
							this._attachedRigidbodyVelocity = Vector3.zero;
						}
					}
					else
					{
						this._transientPosition += this._attachedRigidbodyVelocity * deltaTime;
					}
					this._isMovingFromAttachedRigidbody = false;
				}
			}
		}

		// Token: 0x0600DE0C RID: 56844 RVA: 0x004FA23C File Offset: 0x004F843C
		public void UpdatePhase2(float deltaTime)
		{
			this.CharacterController.UpdateRotation(ref this._transientRotation, deltaTime);
			this.TransientRotation = this._transientRotation;
			if (this._moveRotationDirty)
			{
				this.TransientRotation = this._moveRotationTarget;
				this._moveRotationDirty = false;
			}
			if (this._solveMovementCollisions && this.InteractiveRigidbodyHandling)
			{
				if (this.InteractiveRigidbodyHandling && this._attachedRigidbody)
				{
					float radius = this.Capsule.radius;
					RaycastHit raycastHit;
					if (this.CharacterGroundSweep(this._transientPosition + this._characterUp * radius, this._transientRotation, -this._characterUp, radius, out raycastHit) && raycastHit.collider.attachedRigidbody == this._attachedRigidbody && this.IsStableOnNormal(raycastHit.normal))
					{
						float d = radius - raycastHit.distance;
						this._transientPosition = this._transientPosition + this._characterUp * d + this._characterUp * 0.001f;
					}
				}
				if (this.InteractiveRigidbodyHandling)
				{
					Vector3 vector = this._cachedWorldUp;
					float num = 0f;
					int num2 = 0;
					bool flag = false;
					while (num2 < 3 && !flag)
					{
						int num3 = this.CharacterCollisionsOverlap(this._transientPosition, this._transientRotation, this._internalProbedColliders, 0f, false);
						if (num3 > 0)
						{
							int i = 0;
							while (i < num3)
							{
								Transform component = this._internalProbedColliders[i].GetComponent<Transform>();
								if (Physics.ComputePenetration(this.Capsule, this._transientPosition, this._transientRotation, this._internalProbedColliders[i], component.position, component.rotation, out vector, out num))
								{
									HitStabilityReport hitStabilityReport = new HitStabilityReport
									{
										IsStable = this.IsStableOnNormal(vector)
									};
									vector = this.GetObstructionNormal(vector, hitStabilityReport);
									Vector3 b = vector * (num + 0.001f);
									this._transientPosition += b;
									if (this.InteractiveRigidbodyHandling)
									{
										Rigidbody interactiveRigidbody = this.GetInteractiveRigidbody(this._internalProbedColliders[i]);
										if (interactiveRigidbody != null)
										{
											HitStabilityReport hitStabilityReport2 = new HitStabilityReport
											{
												IsStable = this.IsStableOnNormal(vector)
											};
											if (hitStabilityReport2.IsStable)
											{
												this.LastMovementIterationFoundAnyGround = hitStabilityReport2.IsStable;
											}
											if (interactiveRigidbody != this._attachedRigidbody)
											{
												Vector3 point = this._transientPosition + this._transientRotation * this._characterTransformToCapsuleCenter;
												Vector3 transientPosition = this._transientPosition;
												MeshCollider meshCollider = this._internalProbedColliders[i] as MeshCollider;
												if (!meshCollider || meshCollider.convex)
												{
													Physics.ClosestPoint(point, this._internalProbedColliders[i], component.position, component.rotation);
												}
												this.StoreRigidbodyHit(interactiveRigidbody, this.Velocity, transientPosition, vector, hitStabilityReport2);
											}
										}
									}
									if (this._overlapsCount < this._overlaps.Length)
									{
										this._overlaps[this._overlapsCount] = new OverlapResult(vector, this._internalProbedColliders[i]);
										this._overlapsCount++;
										break;
									}
									break;
								}
								else
								{
									i++;
								}
							}
						}
						else
						{
							flag = true;
						}
						num2++;
					}
				}
			}
			this.CharacterController.UpdateVelocity(ref this.BaseVelocity, deltaTime);
			if (this.BaseVelocity.magnitude < 0.01f)
			{
				this.BaseVelocity = Vector3.zero;
			}
			if (this.BaseVelocity.sqrMagnitude > 0f)
			{
				if (this._solveMovementCollisions)
				{
					if (this.InternalCharacterMove(this.BaseVelocity * deltaTime, deltaTime, out this._internalResultingMovementMagnitude, out this._internalResultingMovementDirection))
					{
						this.BaseVelocity = this._internalResultingMovementDirection * this._internalResultingMovementMagnitude / deltaTime;
					}
					else
					{
						this.BaseVelocity = Vector3.zero;
					}
				}
				else
				{
					this._transientPosition += this.BaseVelocity * deltaTime;
				}
			}
			if (this.InteractiveRigidbodyHandling)
			{
				this.ProcessVelocityForRigidbodyHits(ref this.BaseVelocity, deltaTime);
			}
			if (this.HasPlanarConstraint)
			{
				this._transientPosition = this._initialSimulationPosition + Vector3.ProjectOnPlane(this._transientPosition - this._initialSimulationPosition, this.PlanarConstraintAxis.normalized);
			}
			if (this.DiscreteCollisionEvents)
			{
				int num4 = this.CharacterCollisionsOverlap(this._transientPosition, this._transientRotation, this._internalProbedColliders, 0.002f, false);
				for (int j = 0; j < num4; j++)
				{
					this.CharacterController.OnDiscreteCollisionDetected(this._internalProbedColliders[j]);
				}
			}
			this.CharacterController.AfterCharacterUpdate(deltaTime);
		}

		// Token: 0x0600DE0D RID: 56845 RVA: 0x004FA6D3 File Offset: 0x004F88D3
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsStableOnNormal(Vector3 normal)
		{
			return Vector3.Angle(this._characterUp, normal) <= this.MaxStableSlopeAngle;
		}

		// Token: 0x0600DE0E RID: 56846 RVA: 0x004FA6EC File Offset: 0x004F88EC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsStableWithSpecialCases(ref HitStabilityReport stabilityReport, Vector3 velocity)
		{
			if (this.LedgeAndDenivelationHandling)
			{
				if (stabilityReport.LedgeDetected && stabilityReport.IsMovingTowardsEmptySideOfLedge)
				{
					if (velocity.magnitude >= this.MaxVelocityForLedgeSnap)
					{
						return false;
					}
					if (stabilityReport.IsOnEmptySideOfLedge && stabilityReport.DistanceFromLedge > this.MaxStableDistanceFromLedge)
					{
						return false;
					}
				}
				if (this.LastGroundingStatus.FoundAnyGround && stabilityReport.InnerNormal.sqrMagnitude != 0f && stabilityReport.OuterNormal.sqrMagnitude != 0f)
				{
					if (Vector3.Angle(stabilityReport.InnerNormal, stabilityReport.OuterNormal) > this.MaxStableDenivelationAngle)
					{
						return false;
					}
					if (Vector3.Angle(this.LastGroundingStatus.InnerGroundNormal, stabilityReport.OuterNormal) > this.MaxStableDenivelationAngle)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600DE0F RID: 56847 RVA: 0x004FA7AC File Offset: 0x004F89AC
		public void ProbeGround(ref Vector3 probingPosition, Quaternion atRotation, float probingDistance, ref CharacterGroundingReport groundingReport)
		{
			if (probingDistance < 0.005f)
			{
				probingDistance = 0.005f;
			}
			int num = 0;
			RaycastHit raycastHit = default(RaycastHit);
			bool flag = false;
			Vector3 vector = probingPosition;
			Vector3 vector2 = atRotation * -this._cachedWorldUp;
			float num2 = probingDistance;
			while (num2 > 0f && num <= 2 && !flag)
			{
				if (this.CharacterGroundSweep(vector, atRotation, vector2, num2, out raycastHit))
				{
					Vector3 vector3 = vector + vector2 * raycastHit.distance;
					HitStabilityReport hitStabilityReport = default(HitStabilityReport);
					this.EvaluateHitStability(raycastHit.collider, raycastHit.normal, raycastHit.point, vector3, this._transientRotation, this.BaseVelocity, ref hitStabilityReport);
					groundingReport.FoundAnyGround = true;
					groundingReport.GroundNormal = raycastHit.normal;
					groundingReport.InnerGroundNormal = hitStabilityReport.InnerNormal;
					groundingReport.OuterGroundNormal = hitStabilityReport.OuterNormal;
					groundingReport.GroundCollider = raycastHit.collider;
					groundingReport.GroundPoint = raycastHit.point;
					groundingReport.SnappingPrevented = false;
					if (hitStabilityReport.IsStable)
					{
						groundingReport.SnappingPrevented = !this.IsStableWithSpecialCases(ref hitStabilityReport, this.BaseVelocity);
						groundingReport.IsStableOnGround = true;
						if (!groundingReport.SnappingPrevented)
						{
							vector3 += -vector2 * 0.001f;
							probingPosition = vector3;
						}
						this.CharacterController.OnGroundHit(raycastHit.collider, raycastHit.normal, raycastHit.point, ref hitStabilityReport);
						flag = true;
					}
					else
					{
						Vector3 b = vector2 * raycastHit.distance + atRotation * this._cachedWorldUp * Mathf.Max(0.001f, raycastHit.distance);
						vector += b;
						num2 = Mathf.Min(0.02f, Mathf.Max(num2 - b.magnitude, 0f));
						vector2 = Vector3.ProjectOnPlane(vector2, raycastHit.normal).normalized;
					}
				}
				else
				{
					flag = true;
				}
				num++;
			}
		}

		// Token: 0x0600DE10 RID: 56848 RVA: 0x004FA9B6 File Offset: 0x004F8BB6
		public void ForceUnground(float time = 0.1f)
		{
			this._mustUnground = true;
			this._mustUngroundTimeCounter = time;
		}

		// Token: 0x0600DE11 RID: 56849 RVA: 0x004FA9C6 File Offset: 0x004F8BC6
		public bool MustUnground()
		{
			return this._mustUnground || this._mustUngroundTimeCounter > 0f;
		}

		// Token: 0x0600DE12 RID: 56850 RVA: 0x004FA9E0 File Offset: 0x004F8BE0
		public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
		{
			Vector3 rhs = Vector3.Cross(direction, this._characterUp);
			return Vector3.Cross(surfaceNormal, rhs).normalized;
		}

		// Token: 0x0600DE13 RID: 56851 RVA: 0x004FAA0C File Offset: 0x004F8C0C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool InternalCharacterMove(Vector3 movement, float deltaTime, out float resultingMovementMagnitude, out Vector3 resultingMovementDirection)
		{
			this._rigidbodiesPushedCount = 0;
			bool result = true;
			float num = movement.magnitude;
			Vector3 normalized = movement.normalized;
			resultingMovementDirection = normalized;
			resultingMovementMagnitude = num;
			int num2 = 0;
			bool flag = true;
			Vector3 vector = this._transientPosition;
			Vector3 originalMoveDirection = normalized;
			Vector3 cachedZeroVector = this._cachedZeroVector;
			MovementSweepState movementSweepState = MovementSweepState.Initial;
			for (int i = 0; i < this._overlapsCount; i++)
			{
				Vector3 normal = this._overlaps[i].Normal;
				if (Vector3.Dot(normalized, normal) < 0f)
				{
					this.InternalHandleMovementProjection(this.IsStableOnNormal(normal) && !this.MustUnground(), normal, normal, originalMoveDirection, ref movementSweepState, ref cachedZeroVector, ref resultingMovementMagnitude, ref normalized, ref num);
				}
			}
			while (num > 0f && num2 <= 6 && flag)
			{
				RaycastHit raycastHit;
				if (this.CharacterCollisionsSweep(vector, this._transientRotation, normalized, num + 0.001f, out raycastHit, this._internalCharacterHits, 0f, false) > 0)
				{
					Vector3 vector2 = vector + normalized * raycastHit.distance + raycastHit.normal * 0.001f;
					Vector3 a = vector2 - vector;
					float magnitude = a.magnitude;
					Vector3 withCharacterVelocity = Vector3.zero;
					if (deltaTime > 0f)
					{
						withCharacterVelocity = a / deltaTime;
					}
					HitStabilityReport hitStabilityReport = default(HitStabilityReport);
					this.EvaluateHitStability(raycastHit.collider, raycastHit.normal, raycastHit.point, vector2, this._transientRotation, withCharacterVelocity, ref hitStabilityReport);
					bool flag2 = false;
					if (this._solveGrounding && this.StepHandling != StepHandlingMethod.None && hitStabilityReport.ValidStepDetected && Mathf.Abs(Vector3.Dot(raycastHit.normal, this._characterUp)) <= 0.01f)
					{
						Vector3 normalized2 = Vector3.ProjectOnPlane(-raycastHit.normal, this._characterUp).normalized;
						Vector3 vector3 = vector2 + normalized2 * 0.03f + this._characterUp * this.MaxStepHeight;
						RaycastHit raycastHit2;
						int num3 = this.CharacterCollisionsSweep(vector3, this._transientRotation, -this._characterUp, this.MaxStepHeight, out raycastHit2, this._internalCharacterHits, 0f, true);
						for (int j = 0; j < num3; j++)
						{
							if (this._internalCharacterHits[j].collider == hitStabilityReport.SteppedCollider)
							{
								vector = vector3 + -this._characterUp * (this._internalCharacterHits[j].distance - 0.001f);
								flag2 = true;
								num = Mathf.Max(num - magnitude, 0f);
								Vector3 vector4 = num * normalized;
								vector4 = Vector3.ProjectOnPlane(vector4, this.CharacterUp);
								num = vector4.magnitude;
								normalized = vector4.normalized;
								resultingMovementMagnitude = num;
								break;
							}
						}
					}
					if (!flag2)
					{
						Collider collider = raycastHit.collider;
						Vector3 point = raycastHit.point;
						Vector3 normal2 = raycastHit.normal;
						vector = vector2;
						num = Mathf.Max(num - magnitude, 0f);
						this.CharacterController.OnMovementHit(collider, normal2, point, ref hitStabilityReport);
						Vector3 obstructionNormal = this.GetObstructionNormal(normal2, hitStabilityReport);
						if (this.InteractiveRigidbodyHandling && collider.attachedRigidbody)
						{
							this.StoreRigidbodyHit(collider.attachedRigidbody, normalized * resultingMovementMagnitude / deltaTime, point, obstructionNormal, hitStabilityReport);
						}
						this.InternalHandleMovementProjection(hitStabilityReport.IsStable && !this.MustUnground(), normal2, obstructionNormal, originalMoveDirection, ref movementSweepState, ref cachedZeroVector, ref resultingMovementMagnitude, ref normalized, ref num);
					}
				}
				else
				{
					flag = false;
				}
				num2++;
				if (num2 > 6)
				{
					num = 0f;
					result = false;
				}
			}
			this._transientPosition = vector + normalized * num;
			resultingMovementDirection = normalized;
			return result;
		}

		// Token: 0x0600DE14 RID: 56852 RVA: 0x004FADDC File Offset: 0x004F8FDC
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 GetObstructionNormal(Vector3 hitNormal, HitStabilityReport hitStabilityReport)
		{
			Vector3 vector = hitNormal;
			if (this.GroundingStatus.IsStableOnGround && !this.MustUnground() && !hitStabilityReport.IsStable)
			{
				vector = Vector3.Cross(Vector3.Cross(this.GroundingStatus.GroundNormal, vector).normalized, this._characterUp).normalized;
			}
			if (vector.sqrMagnitude == 0f)
			{
				vector = hitNormal;
			}
			return vector;
		}

		// Token: 0x0600DE15 RID: 56853 RVA: 0x004FAE48 File Offset: 0x004F9048
		[PublicizedFrom(EAccessModifier.Private)]
		public void StoreRigidbodyHit(Rigidbody hitRigidbody, Vector3 hitVelocity, Vector3 hitPoint, Vector3 obstructionNormal, HitStabilityReport hitStabilityReport)
		{
			if (this._rigidbodyProjectionHitCount < this._internalRigidbodyProjectionHits.Length && !hitRigidbody.GetComponent<KinematicCharacterMotor>())
			{
				RigidbodyProjectionHit rigidbodyProjectionHit = default(RigidbodyProjectionHit);
				rigidbodyProjectionHit.Rigidbody = hitRigidbody;
				rigidbodyProjectionHit.HitPoint = hitPoint;
				rigidbodyProjectionHit.EffectiveHitNormal = obstructionNormal;
				rigidbodyProjectionHit.HitVelocity = hitVelocity;
				rigidbodyProjectionHit.StableOnHit = hitStabilityReport.IsStable;
				this._internalRigidbodyProjectionHits[this._rigidbodyProjectionHitCount] = rigidbodyProjectionHit;
				this._rigidbodyProjectionHitCount++;
			}
		}

		// Token: 0x0600DE16 RID: 56854 RVA: 0x004FAECC File Offset: 0x004F90CC
		[PublicizedFrom(EAccessModifier.Private)]
		public void InternalHandleMovementProjection(bool stableOnHit, Vector3 hitNormal, Vector3 obstructionNormal, Vector3 originalMoveDirection, ref MovementSweepState sweepState, ref Vector3 previousObstructionNormal, ref float resultingMovementMagnitude, ref Vector3 remainingMovementDirection, ref float remainingMovementMagnitude)
		{
			if (remainingMovementMagnitude <= 0f)
			{
				return;
			}
			Vector3 vector = originalMoveDirection * remainingMovementMagnitude;
			float num = remainingMovementMagnitude;
			if (stableOnHit)
			{
				this.LastMovementIterationFoundAnyGround = true;
			}
			if (sweepState == MovementSweepState.FoundBlockingCrease)
			{
				remainingMovementMagnitude = 0f;
				resultingMovementMagnitude = 0f;
				sweepState = MovementSweepState.FoundBlockingCorner;
			}
			else
			{
				this.HandleMovementProjection(ref vector, obstructionNormal, stableOnHit);
				remainingMovementMagnitude = vector.magnitude;
				remainingMovementDirection = vector.normalized;
				resultingMovementMagnitude = remainingMovementMagnitude / num * resultingMovementMagnitude;
				if (sweepState == MovementSweepState.Initial)
				{
					sweepState = MovementSweepState.AfterFirstHit;
				}
				else if (sweepState == MovementSweepState.AfterFirstHit && Vector3.Dot(previousObstructionNormal, remainingMovementDirection) < 0f)
				{
					Vector3 normalized = Vector3.Cross(previousObstructionNormal, obstructionNormal).normalized;
					vector = Vector3.Project(vector, normalized);
					remainingMovementMagnitude = vector.magnitude;
					remainingMovementDirection = vector.normalized;
					resultingMovementMagnitude = remainingMovementMagnitude / num * resultingMovementMagnitude;
					sweepState = MovementSweepState.FoundBlockingCrease;
				}
			}
			previousObstructionNormal = obstructionNormal;
		}

		// Token: 0x0600DE17 RID: 56855 RVA: 0x004FAFC4 File Offset: 0x004F91C4
		public virtual void HandleMovementProjection(ref Vector3 movement, Vector3 obstructionNormal, bool stableOnHit)
		{
			if (this.GroundingStatus.IsStableOnGround && !this.MustUnground())
			{
				if (stableOnHit)
				{
					movement = this.GetDirectionTangentToSurface(movement, obstructionNormal) * movement.magnitude;
					return;
				}
				Vector3 normalized = Vector3.Cross(Vector3.Cross(obstructionNormal, this.GroundingStatus.GroundNormal).normalized, obstructionNormal).normalized;
				movement = this.GetDirectionTangentToSurface(movement, normalized) * movement.magnitude;
				movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
				return;
			}
			else
			{
				if (stableOnHit)
				{
					movement = Vector3.ProjectOnPlane(movement, this.CharacterUp);
					movement = this.GetDirectionTangentToSurface(movement, obstructionNormal) * movement.magnitude;
					return;
				}
				movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
				return;
			}
		}

		// Token: 0x0600DE18 RID: 56856 RVA: 0x004FB0B0 File Offset: 0x004F92B0
		public virtual void HandleSimulatedRigidbodyInteraction(ref Vector3 processedVelocity, RigidbodyProjectionHit hit, float deltaTime)
		{
			float num = 0.2f;
			if (num > 0f && !hit.StableOnHit && !hit.Rigidbody.isKinematic)
			{
				float d = num / hit.Rigidbody.mass;
				Vector3 velocityFromRigidbodyMovement = this.GetVelocityFromRigidbodyMovement(hit.Rigidbody, hit.HitPoint, deltaTime);
				Vector3 a = Vector3.Project(hit.HitVelocity, hit.EffectiveHitNormal) - velocityFromRigidbodyMovement;
				hit.Rigidbody.AddForceAtPosition(d * a, hit.HitPoint, ForceMode.VelocityChange);
			}
			if (!hit.StableOnHit)
			{
				Vector3 a2 = Vector3.Project(this.GetVelocityFromRigidbodyMovement(hit.Rigidbody, hit.HitPoint, deltaTime), hit.EffectiveHitNormal);
				Vector3 b = Vector3.Project(processedVelocity, hit.EffectiveHitNormal);
				processedVelocity += a2 - b;
			}
		}

		// Token: 0x0600DE19 RID: 56857 RVA: 0x004FB188 File Offset: 0x004F9388
		[PublicizedFrom(EAccessModifier.Private)]
		public void ProcessVelocityForRigidbodyHits(ref Vector3 processedVelocity, float deltaTime)
		{
			for (int i = 0; i < this._rigidbodyProjectionHitCount; i++)
			{
				if (this._internalRigidbodyProjectionHits[i].Rigidbody)
				{
					bool flag = false;
					for (int j = 0; j < this._rigidbodiesPushedCount; j++)
					{
						if (this._rigidbodiesPushedThisMove[j] == this._internalRigidbodyProjectionHits[j].Rigidbody)
						{
							flag = true;
							break;
						}
					}
					if (!flag && this._internalRigidbodyProjectionHits[i].Rigidbody != this._attachedRigidbody && this._rigidbodiesPushedCount < this._rigidbodiesPushedThisMove.Length)
					{
						this._rigidbodiesPushedThisMove[this._rigidbodiesPushedCount] = this._internalRigidbodyProjectionHits[i].Rigidbody;
						this._rigidbodiesPushedCount++;
						if (this.RigidbodyInteractionType == RigidbodyInteractionType.SimulatedDynamic)
						{
							this.HandleSimulatedRigidbodyInteraction(ref processedVelocity, this._internalRigidbodyProjectionHits[i], deltaTime);
						}
					}
				}
			}
		}

		// Token: 0x0600DE1A RID: 56858 RVA: 0x004FB278 File Offset: 0x004F9478
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckIfColliderValidForCollisions(Collider coll)
		{
			return !(coll == this.Capsule) && this.InternalIsColliderValidForCollisions(coll);
		}

		// Token: 0x0600DE1B RID: 56859 RVA: 0x004FB298 File Offset: 0x004F9498
		[PublicizedFrom(EAccessModifier.Private)]
		public bool InternalIsColliderValidForCollisions(Collider coll)
		{
			Rigidbody attachedRigidbody = coll.attachedRigidbody;
			if (attachedRigidbody)
			{
				bool isKinematic = attachedRigidbody.isKinematic;
				if (this._isMovingFromAttachedRigidbody && (!isKinematic || attachedRigidbody == this._attachedRigidbody))
				{
					return false;
				}
				if (this.RigidbodyInteractionType == RigidbodyInteractionType.Kinematic && !isKinematic)
				{
					return false;
				}
			}
			return this.CharacterController.IsColliderValidForCollisions(coll);
		}

		// Token: 0x0600DE1C RID: 56860 RVA: 0x004FB2F8 File Offset: 0x004F94F8
		public void EvaluateHitStability(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, Vector3 withCharacterVelocity, ref HitStabilityReport stabilityReport)
		{
			if (!this._solveGrounding)
			{
				stabilityReport.IsStable = false;
				return;
			}
			Vector3 vector = atCharacterRotation * this._cachedWorldUp;
			Vector3 normalized = Vector3.ProjectOnPlane(hitNormal, vector).normalized;
			stabilityReport.IsStable = this.IsStableOnNormal(hitNormal);
			stabilityReport.InnerNormal = hitNormal;
			stabilityReport.OuterNormal = hitNormal;
			if (this.LedgeAndDenivelationHandling)
			{
				float num = 0.05f;
				if (this.StepHandling != StepHandlingMethod.None)
				{
					num = this.MaxStepHeight;
				}
				bool flag = false;
				bool flag2 = false;
				RaycastHit raycastHit;
				if (this.CharacterCollisionsRaycast(hitPoint + vector * 0.02f + normalized * 0.001f, -vector, num + 0.02f, out raycastHit, this._internalCharacterHits, false) > 0)
				{
					Vector3 normal = raycastHit.normal;
					stabilityReport.InnerNormal = normal;
					flag = this.IsStableOnNormal(normal);
				}
				RaycastHit raycastHit2;
				if (this.CharacterCollisionsRaycast(hitPoint + vector * 0.02f + -normalized * 0.001f, -vector, num + 0.02f, out raycastHit2, this._internalCharacterHits, false) > 0)
				{
					Vector3 normal2 = raycastHit2.normal;
					stabilityReport.OuterNormal = normal2;
					flag2 = this.IsStableOnNormal(normal2);
				}
				stabilityReport.LedgeDetected = (flag != flag2);
				if (stabilityReport.LedgeDetected)
				{
					stabilityReport.IsOnEmptySideOfLedge = (flag2 && !flag);
					stabilityReport.LedgeGroundNormal = (flag2 ? stabilityReport.OuterNormal : stabilityReport.InnerNormal);
					stabilityReport.LedgeRightDirection = Vector3.Cross(hitNormal, stabilityReport.OuterNormal).normalized;
					stabilityReport.LedgeFacingDirection = Vector3.Cross(stabilityReport.LedgeGroundNormal, stabilityReport.LedgeRightDirection).normalized;
					stabilityReport.DistanceFromLedge = Vector3.ProjectOnPlane(hitPoint - (atCharacterPosition + atCharacterRotation * this._characterTransformToCapsuleBottom), vector).magnitude;
					stabilityReport.IsMovingTowardsEmptySideOfLedge = (Vector3.Dot(withCharacterVelocity, Vector3.ProjectOnPlane(stabilityReport.LedgeFacingDirection, this.CharacterUp)) > 0f);
				}
				if (stabilityReport.IsStable)
				{
					stabilityReport.IsStable = this.IsStableWithSpecialCases(ref stabilityReport, withCharacterVelocity);
				}
			}
			if (this.StepHandling != StepHandlingMethod.None && !stabilityReport.IsStable)
			{
				Rigidbody attachedRigidbody = hitCollider.attachedRigidbody;
				if (!attachedRigidbody || attachedRigidbody.isKinematic)
				{
					this.DetectSteps(atCharacterPosition, atCharacterRotation, hitPoint, normalized, ref stabilityReport);
					if (stabilityReport.ValidStepDetected)
					{
						stabilityReport.IsStable = true;
					}
				}
			}
			this.CharacterController.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref stabilityReport);
		}

		// Token: 0x0600DE1D RID: 56861 RVA: 0x004FB58C File Offset: 0x004F978C
		[PublicizedFrom(EAccessModifier.Private)]
		public void DetectSteps(Vector3 characterPosition, Quaternion characterRotation, Vector3 hitPoint, Vector3 innerHitDirection, ref HitStabilityReport stabilityReport)
		{
			Vector3 vector = characterRotation * this._cachedWorldUp;
			Vector3 b = Vector3.Project(hitPoint - characterPosition, vector);
			Vector3 vector2 = hitPoint - b + vector * this.MaxStepHeight;
			RaycastHit raycastHit;
			int nbStepHits = this.CharacterCollisionsSweep(vector2, characterRotation, -vector, this.MaxStepHeight + 0.001f, out raycastHit, this._internalCharacterHits, 0f, true);
			Collider steppedCollider;
			if (this.CheckStepValidity(nbStepHits, characterPosition, characterRotation, innerHitDirection, vector2, out steppedCollider))
			{
				stabilityReport.ValidStepDetected = true;
				stabilityReport.SteppedCollider = steppedCollider;
			}
			if (this.StepHandling == StepHandlingMethod.Extra && !stabilityReport.ValidStepDetected)
			{
				vector2 = characterPosition + vector * this.MaxStepHeight + -innerHitDirection * this.MinRequiredStepDepth;
				nbStepHits = this.CharacterCollisionsSweep(vector2, characterRotation, -vector, this.MaxStepHeight - 0.001f, out raycastHit, this._internalCharacterHits, 0f, true);
				if (this.CheckStepValidity(nbStepHits, characterPosition, characterRotation, innerHitDirection, vector2, out steppedCollider))
				{
					stabilityReport.ValidStepDetected = true;
					stabilityReport.SteppedCollider = steppedCollider;
				}
			}
		}

		// Token: 0x0600DE1E RID: 56862 RVA: 0x004FB6A8 File Offset: 0x004F98A8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckStepValidity(int nbStepHits, Vector3 characterPosition, Quaternion characterRotation, Vector3 innerHitDirection, Vector3 stepCheckStartPos, out Collider hitCollider)
		{
			hitCollider = null;
			Vector3 vector = characterRotation * Vector3.up;
			bool flag = false;
			while (nbStepHits > 0 && !flag)
			{
				RaycastHit raycastHit = default(RaycastHit);
				float num = 0f;
				int num2 = 0;
				for (int i = 0; i < nbStepHits; i++)
				{
					float distance = this._internalCharacterHits[i].distance;
					if (distance > num)
					{
						num = distance;
						raycastHit = this._internalCharacterHits[i];
						num2 = i;
					}
				}
				Vector3 b = characterPosition + characterRotation * this._characterTransformToCapsuleBottom;
				float sqrMagnitude = Vector3.Project(raycastHit.point - b, vector).sqrMagnitude;
				Vector3 vector2 = stepCheckStartPos + -vector * (raycastHit.distance - 0.001f);
				RaycastHit raycastHit2;
				if (this.CharacterCollisionsOverlap(vector2, characterRotation, this._internalProbedColliders, 0f, false) <= 0 && this.CharacterCollisionsRaycast(raycastHit.point + vector * 0.02f + -innerHitDirection * 0.001f, -vector, this.MaxStepHeight + 0.02f, out raycastHit2, this._internalCharacterHits, true) > 0 && this.IsStableOnNormal(raycastHit2.normal) && this.CharacterCollisionsSweep(characterPosition, characterRotation, vector, this.MaxStepHeight - raycastHit.distance, out raycastHit2, this._internalCharacterHits, 0f, false) <= 0)
				{
					bool flag2 = false;
					RaycastHit raycastHit3;
					if (this.AllowSteppingWithoutStableGrounding)
					{
						flag2 = true;
					}
					else if (this.CharacterCollisionsRaycast(characterPosition + Vector3.Project(vector2 - characterPosition, vector), -vector, this.MaxStepHeight, out raycastHit3, this._internalCharacterHits, true) > 0 && this.IsStableOnNormal(raycastHit3.normal))
					{
						flag2 = true;
					}
					if (!flag2 && this.CharacterCollisionsRaycast(raycastHit.point + innerHitDirection * 0.001f, -vector, this.MaxStepHeight, out raycastHit3, this._internalCharacterHits, true) > 0 && this.IsStableOnNormal(raycastHit3.normal))
					{
						flag2 = true;
					}
					if (flag2)
					{
						hitCollider = raycastHit.collider;
						return true;
					}
				}
				if (!flag)
				{
					nbStepHits--;
					if (num2 < nbStepHits)
					{
						this._internalCharacterHits[num2] = this._internalCharacterHits[nbStepHits];
					}
				}
			}
			return false;
		}

		// Token: 0x0600DE1F RID: 56863 RVA: 0x004FB900 File Offset: 0x004F9B00
		public Vector3 GetVelocityFromRigidbodyMovement(Rigidbody interactiveRigidbody, Vector3 atPoint, float deltaTime)
		{
			if (deltaTime > 0f)
			{
				Vector3 vector = interactiveRigidbody.velocity;
				if (interactiveRigidbody.angularVelocity != Vector3.zero)
				{
					Vector3 vector2 = interactiveRigidbody.position + interactiveRigidbody.centerOfMass;
					Vector3 point = atPoint - vector2;
					Quaternion rotation = Quaternion.Euler(57.29578f * interactiveRigidbody.angularVelocity * deltaTime);
					Vector3 a = vector2 + rotation * point;
					vector += (a - atPoint) / deltaTime;
				}
				return vector;
			}
			return Vector3.zero;
		}

		// Token: 0x0600DE20 RID: 56864 RVA: 0x004FB990 File Offset: 0x004F9B90
		[PublicizedFrom(EAccessModifier.Private)]
		public Rigidbody GetInteractiveRigidbody(Collider onCollider)
		{
			Rigidbody attachedRigidbody = onCollider.attachedRigidbody;
			if (attachedRigidbody)
			{
				if (attachedRigidbody.gameObject.GetComponent<PhysicsMover>())
				{
					return attachedRigidbody;
				}
				if (!attachedRigidbody.isKinematic)
				{
					return attachedRigidbody;
				}
			}
			return null;
		}

		// Token: 0x0600DE21 RID: 56865 RVA: 0x004FB9CB File Offset: 0x004F9BCB
		public Vector3 GetVelocityForMovePosition(Vector3 fromPosition, Vector3 toPosition, float deltaTime)
		{
			if (deltaTime > 0f)
			{
				return (toPosition - fromPosition) / deltaTime;
			}
			return Vector3.zero;
		}

		// Token: 0x0600DE22 RID: 56866 RVA: 0x004FB9E8 File Offset: 0x004F9BE8
		[PublicizedFrom(EAccessModifier.Private)]
		public void RestrictVectorToPlane(ref Vector3 vector, Vector3 toPlane)
		{
			if (vector.x > 0f != toPlane.x > 0f)
			{
				vector.x = 0f;
			}
			if (vector.y > 0f != toPlane.y > 0f)
			{
				vector.y = 0f;
			}
			if (vector.z > 0f != toPlane.z > 0f)
			{
				vector.z = 0f;
			}
		}

		// Token: 0x0600DE23 RID: 56867 RVA: 0x004FBA6C File Offset: 0x004F9C6C
		public int CharacterCollisionsOverlap(Vector3 position, Quaternion rotation, Collider[] overlappedColliders, float inflate = 0f, bool acceptOnlyStableGroundLayer = false)
		{
			int layerMask = this.CollidableLayers;
			if (acceptOnlyStableGroundLayer)
			{
				layerMask = (this.CollidableLayers & this.StableGroundLayers);
			}
			Vector3 vector = position + rotation * this._characterTransformToCapsuleBottomHemi;
			Vector3 vector2 = position + rotation * this._characterTransformToCapsuleTopHemi;
			if (inflate != 0f)
			{
				vector += rotation * Vector3.down * inflate;
				vector2 += rotation * Vector3.up * inflate;
			}
			int num;
			for (int i = (num = Physics.OverlapCapsuleNonAlloc(vector, vector2, this.Capsule.radius + inflate, overlappedColliders, layerMask, QueryTriggerInteraction.Ignore)) - 1; i >= 0; i--)
			{
				if (!this.CheckIfColliderValidForCollisions(overlappedColliders[i]))
				{
					num--;
					if (i < num)
					{
						overlappedColliders[i] = overlappedColliders[num];
					}
				}
			}
			return num;
		}

		// Token: 0x0600DE24 RID: 56868 RVA: 0x004FBB4C File Offset: 0x004F9D4C
		public int CharacterOverlap(Vector3 position, Quaternion rotation, Collider[] overlappedColliders, LayerMask layers, QueryTriggerInteraction triggerInteraction, float inflate = 0f)
		{
			Vector3 vector = position + rotation * this._characterTransformToCapsuleBottomHemi;
			Vector3 vector2 = position + rotation * this._characterTransformToCapsuleTopHemi;
			if (inflate != 0f)
			{
				vector += rotation * Vector3.down * inflate;
				vector2 += rotation * Vector3.up * inflate;
			}
			int num;
			for (int i = (num = Physics.OverlapCapsuleNonAlloc(vector, vector2, this.Capsule.radius + inflate, overlappedColliders, layers, triggerInteraction)) - 1; i >= 0; i--)
			{
				if (overlappedColliders[i] == this.Capsule)
				{
					num--;
					if (i < num)
					{
						overlappedColliders[i] = overlappedColliders[num];
					}
				}
			}
			return num;
		}

		// Token: 0x0600DE25 RID: 56869 RVA: 0x004FBC0C File Offset: 0x004F9E0C
		public int CharacterCollisionsSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, out RaycastHit closestHit, RaycastHit[] hits, float inflate = 0f, bool acceptOnlyStableGroundLayer = false)
		{
			int layerMask = this.CollidableLayers;
			if (acceptOnlyStableGroundLayer)
			{
				layerMask = (this.CollidableLayers & this.StableGroundLayers);
			}
			Vector3 vector = position + rotation * this._characterTransformToCapsuleBottomHemi - direction * 0.002f;
			Vector3 vector2 = position + rotation * this._characterTransformToCapsuleTopHemi - direction * 0.002f;
			if (inflate != 0f)
			{
				vector += rotation * Vector3.down * inflate;
				vector2 += rotation * Vector3.up * inflate;
			}
			int num = Physics.CapsuleCastNonAlloc(vector, vector2, this.Capsule.radius + inflate, direction, hits, distance + 0.002f, layerMask, QueryTriggerInteraction.Ignore);
			closestHit = default(RaycastHit);
			float num2 = float.PositiveInfinity;
			int num3 = num;
			for (int i = num - 1; i >= 0; i--)
			{
				int num4 = i;
				hits[num4].distance = hits[num4].distance - 0.002f;
				RaycastHit raycastHit = hits[i];
				float distance2 = raycastHit.distance;
				if (distance2 <= 0f || !this.CheckIfColliderValidForCollisions(raycastHit.collider))
				{
					num3--;
					if (i < num3)
					{
						hits[i] = hits[num3];
					}
				}
				else if (distance2 < num2)
				{
					closestHit = raycastHit;
					num2 = distance2;
				}
			}
			return num3;
		}

		// Token: 0x0600DE26 RID: 56870 RVA: 0x004FBD80 File Offset: 0x004F9F80
		public int CharacterSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, out RaycastHit closestHit, RaycastHit[] hits, LayerMask layers, QueryTriggerInteraction triggerInteraction, float inflate = 0f)
		{
			closestHit = default(RaycastHit);
			Vector3 vector = position + rotation * this._characterTransformToCapsuleBottomHemi;
			Vector3 vector2 = position + rotation * this._characterTransformToCapsuleTopHemi;
			if (inflate != 0f)
			{
				vector += rotation * Vector3.down * inflate;
				vector2 += rotation * Vector3.up * inflate;
			}
			int num = Physics.CapsuleCastNonAlloc(vector, vector2, this.Capsule.radius + inflate, direction, hits, distance, layers, triggerInteraction);
			float num2 = float.PositiveInfinity;
			int num3 = num;
			for (int i = num - 1; i >= 0; i--)
			{
				RaycastHit raycastHit = hits[i];
				if (raycastHit.distance <= 0f || raycastHit.collider == this.Capsule)
				{
					num3--;
					if (i < num3)
					{
						hits[i] = hits[num3];
					}
				}
				else
				{
					float distance2 = raycastHit.distance;
					if (distance2 < num2)
					{
						closestHit = raycastHit;
						num2 = distance2;
					}
				}
			}
			return num3;
		}

		// Token: 0x0600DE27 RID: 56871 RVA: 0x004FBE98 File Offset: 0x004FA098
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CharacterGroundSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, out RaycastHit closestHit)
		{
			closestHit = default(RaycastHit);
			int num = Physics.CapsuleCastNonAlloc(position + rotation * this._characterTransformToCapsuleBottomHemi - direction * 0.1f, position + rotation * this._characterTransformToCapsuleTopHemi - direction * 0.1f, this.Capsule.radius, direction, this._internalCharacterHits, distance + 0.1f, this.CollidableLayers & this.StableGroundLayers, QueryTriggerInteraction.Ignore);
			bool result = false;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this._internalCharacterHits[i];
				float distance2 = raycastHit.distance;
				if (distance2 > 0f && this.CheckIfColliderValidForCollisions(raycastHit.collider) && distance2 < num2)
				{
					closestHit = raycastHit;
					closestHit.distance -= 0.1f;
					num2 = distance2;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600DE28 RID: 56872 RVA: 0x004FBF94 File Offset: 0x004FA194
		public int CharacterCollisionsRaycast(Vector3 position, Vector3 direction, float distance, out RaycastHit closestHit, RaycastHit[] hits, bool acceptOnlyStableGroundLayer = false)
		{
			int layerMask = this.CollidableLayers;
			if (acceptOnlyStableGroundLayer)
			{
				layerMask = (this.CollidableLayers & this.StableGroundLayers);
			}
			int num = Physics.RaycastNonAlloc(position, direction, hits, distance, layerMask, QueryTriggerInteraction.Ignore);
			closestHit = default(RaycastHit);
			float num2 = float.PositiveInfinity;
			int num3 = num;
			for (int i = num - 1; i >= 0; i--)
			{
				RaycastHit raycastHit = hits[i];
				float distance2 = raycastHit.distance;
				if (distance2 <= 0f || !this.CheckIfColliderValidForCollisions(raycastHit.collider))
				{
					num3--;
					if (i < num3)
					{
						hits[i] = hits[num3];
					}
				}
				else if (distance2 < num2)
				{
					closestHit = raycastHit;
					num2 = distance2;
				}
			}
			return num3;
		}

		// Token: 0x0400A87F RID: 43135
		[Header("Components")]
		[ReadOnly]
		public CapsuleCollider Capsule;

		// Token: 0x0400A880 RID: 43136
		[Header("Capsule Settings")]
		[SerializeField]
		[Tooltip("Radius of the Character Capsule")]
		[PublicizedFrom(EAccessModifier.Private)]
		public float CapsuleRadius = 0.5f;

		// Token: 0x0400A881 RID: 43137
		[SerializeField]
		[Tooltip("Height of the Character Capsule")]
		[PublicizedFrom(EAccessModifier.Private)]
		public float CapsuleHeight = 2f;

		// Token: 0x0400A882 RID: 43138
		[SerializeField]
		[Tooltip("Height of the Character Capsule")]
		[PublicizedFrom(EAccessModifier.Private)]
		public float CapsuleYOffset = 1f;

		// Token: 0x0400A883 RID: 43139
		[SerializeField]
		[Tooltip("Physics material of the Character Capsule (Does not affect character movement. Only affects things colliding with it)")]
		[PublicizedFrom(EAccessModifier.Private)]
		public PhysicMaterial CapsulePhysicsMaterial;

		// Token: 0x0400A884 RID: 43140
		[Header("Misc settings")]
		[Tooltip("Increases the range of ground detection, to allow snapping to ground at very high speeds")]
		public float GroundDetectionExtraDistance;

		// Token: 0x0400A885 RID: 43141
		[Range(0f, 89f)]
		[Tooltip("Maximum slope angle on which the character can be stable")]
		public float MaxStableSlopeAngle = 60f;

		// Token: 0x0400A886 RID: 43142
		[Tooltip("Which layers can the character be considered stable on")]
		public LayerMask StableGroundLayers = -1;

		// Token: 0x0400A887 RID: 43143
		[Tooltip("Notifies the Character Controller when discrete collisions are detected")]
		public bool DiscreteCollisionEvents;

		// Token: 0x0400A888 RID: 43144
		[Header("Step settings")]
		[Tooltip("Handles properly detecting grounding status on steps, but has a performance cost.")]
		public StepHandlingMethod StepHandling = StepHandlingMethod.Standard;

		// Token: 0x0400A889 RID: 43145
		[Tooltip("Maximum height of a step which the character can climb")]
		public float MaxStepHeight = 0.5f;

		// Token: 0x0400A88A RID: 43146
		[Tooltip("Can the character step up obstacles even if it is not currently stable?")]
		public bool AllowSteppingWithoutStableGrounding;

		// Token: 0x0400A88B RID: 43147
		[Tooltip("Minimum length of a step that the character can step on (used in Extra stepping method). Use this to let the character step on steps that are smaller that its radius")]
		public float MinRequiredStepDepth = 0.1f;

		// Token: 0x0400A88C RID: 43148
		[Header("Ledge settings")]
		[Tooltip("Handles properly detecting ledge information and grounding status, but has a performance cost.")]
		public bool LedgeAndDenivelationHandling = true;

		// Token: 0x0400A88D RID: 43149
		[Tooltip("The distance from the capsule central axis at which the character can stand on a ledge and still be stable")]
		public float MaxStableDistanceFromLedge = 0.5f;

		// Token: 0x0400A88E RID: 43150
		[Tooltip("Prevents snapping to ground on ledges beyond a certain velocity")]
		public float MaxVelocityForLedgeSnap;

		// Token: 0x0400A88F RID: 43151
		[Tooltip("The maximun downward slope angle change that the character can be subjected to and still be snapping to the ground")]
		[Range(1f, 180f)]
		public float MaxStableDenivelationAngle = 180f;

		// Token: 0x0400A890 RID: 43152
		[Header("Rigidbody interaction settings")]
		[Tooltip("Handles properly being pushed by and standing on PhysicsMovers or dynamic rigidbodies. Also handles pushing dynamic rigidbodies")]
		public bool InteractiveRigidbodyHandling = true;

		// Token: 0x0400A891 RID: 43153
		[Tooltip("How the character interacts with non-kinematic rigidbodies. \"Kinematic\" mode means the character pushes the rigidbodies with infinite force (as a kinematic body would). \"SimulatedDynamic\" pushes the rigidbodies with a simulated mass value.")]
		public RigidbodyInteractionType RigidbodyInteractionType;

		// Token: 0x0400A892 RID: 43154
		[Tooltip("Determines if the character preserves moving platform velocities when de-grounding from them")]
		public bool PreserveAttachedRigidbodyMomentum = true;

		// Token: 0x0400A893 RID: 43155
		[Header("Constraints settings")]
		[Tooltip("Determines if the character's movement uses the planar constraint")]
		public bool HasPlanarConstraint;

		// Token: 0x0400A894 RID: 43156
		[Tooltip("Defines the plane that the character's movement is constrained on, if HasMovementConstraintPlane is active")]
		public Vector3 PlanarConstraintAxis = Vector3.forward;

		// Token: 0x0400A895 RID: 43157
		[NonSerialized]
		public CharacterGroundingReport GroundingStatus;

		// Token: 0x0400A896 RID: 43158
		[NonSerialized]
		public CharacterTransientGroundingReport LastGroundingStatus;

		// Token: 0x0400A897 RID: 43159
		[NonSerialized]
		public LayerMask CollidableLayers = -1;

		// Token: 0x0400A898 RID: 43160
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Transform _transform;

		// Token: 0x0400A899 RID: 43161
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _transientPosition;

		// Token: 0x0400A89A RID: 43162
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterUp;

		// Token: 0x0400A89B RID: 43163
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterForward;

		// Token: 0x0400A89C RID: 43164
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterRight;

		// Token: 0x0400A89D RID: 43165
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _initialSimulationPosition;

		// Token: 0x0400A89E RID: 43166
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion _initialSimulationRotation;

		// Token: 0x0400A89F RID: 43167
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Rigidbody _attachedRigidbody;

		// Token: 0x0400A8A0 RID: 43168
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterTransformToCapsuleCenter;

		// Token: 0x0400A8A1 RID: 43169
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterTransformToCapsuleBottom;

		// Token: 0x0400A8A2 RID: 43170
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterTransformToCapsuleTop;

		// Token: 0x0400A8A3 RID: 43171
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterTransformToCapsuleBottomHemi;

		// Token: 0x0400A8A4 RID: 43172
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _characterTransformToCapsuleTopHemi;

		// Token: 0x0400A8A5 RID: 43173
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _attachedRigidbodyVelocity;

		// Token: 0x0400A8A6 RID: 43174
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _overlapsCount;

		// Token: 0x0400A8A7 RID: 43175
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public OverlapResult[] _overlaps = new OverlapResult[16];

		// Token: 0x0400A8A8 RID: 43176
		[NonSerialized]
		public ICharacterController CharacterController;

		// Token: 0x0400A8A9 RID: 43177
		[NonSerialized]
		public bool LastMovementIterationFoundAnyGround;

		// Token: 0x0400A8AA RID: 43178
		[NonSerialized]
		public int IndexInCharacterSystem;

		// Token: 0x0400A8AB RID: 43179
		[NonSerialized]
		public Vector3 InitialTickPosition;

		// Token: 0x0400A8AC RID: 43180
		[NonSerialized]
		public Quaternion InitialTickRotation;

		// Token: 0x0400A8AD RID: 43181
		[NonSerialized]
		public Rigidbody AttachedRigidbodyOverride;

		// Token: 0x0400A8AE RID: 43182
		[NonSerialized]
		public Vector3 BaseVelocity;

		// Token: 0x0400A8AF RID: 43183
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public RaycastHit[] _internalCharacterHits = new RaycastHit[16];

		// Token: 0x0400A8B0 RID: 43184
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Collider[] _internalProbedColliders = new Collider[16];

		// Token: 0x0400A8B1 RID: 43185
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Rigidbody[] _rigidbodiesPushedThisMove = new Rigidbody[16];

		// Token: 0x0400A8B2 RID: 43186
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public RigidbodyProjectionHit[] _internalRigidbodyProjectionHits = new RigidbodyProjectionHit[6];

		// Token: 0x0400A8B3 RID: 43187
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Rigidbody _lastAttachedRigidbody;

		// Token: 0x0400A8B4 RID: 43188
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _solveMovementCollisions = true;

		// Token: 0x0400A8B5 RID: 43189
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _solveGrounding = true;

		// Token: 0x0400A8B6 RID: 43190
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _movePositionDirty;

		// Token: 0x0400A8B7 RID: 43191
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _movePositionTarget = Vector3.zero;

		// Token: 0x0400A8B8 RID: 43192
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _moveRotationDirty;

		// Token: 0x0400A8B9 RID: 43193
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion _moveRotationTarget = Quaternion.identity;

		// Token: 0x0400A8BA RID: 43194
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _lastSolvedOverlapNormalDirty;

		// Token: 0x0400A8BB RID: 43195
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _lastSolvedOverlapNormal = Vector3.forward;

		// Token: 0x0400A8BC RID: 43196
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _rigidbodiesPushedCount;

		// Token: 0x0400A8BD RID: 43197
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _rigidbodyProjectionHitCount;

		// Token: 0x0400A8BE RID: 43198
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public float _internalResultingMovementMagnitude;

		// Token: 0x0400A8BF RID: 43199
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _internalResultingMovementDirection = Vector3.zero;

		// Token: 0x0400A8C0 RID: 43200
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _isMovingFromAttachedRigidbody;

		// Token: 0x0400A8C1 RID: 43201
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _mustUnground;

		// Token: 0x0400A8C2 RID: 43202
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public float _mustUngroundTimeCounter;

		// Token: 0x0400A8C3 RID: 43203
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _cachedWorldUp = Vector3.up;

		// Token: 0x0400A8C4 RID: 43204
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _cachedWorldForward = Vector3.forward;

		// Token: 0x0400A8C5 RID: 43205
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _cachedWorldRight = Vector3.right;

		// Token: 0x0400A8C6 RID: 43206
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _cachedZeroVector = Vector3.zero;

		// Token: 0x0400A8C7 RID: 43207
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion _transientRotation;

		// Token: 0x0400A8C8 RID: 43208
		public const int MaxHitsBudget = 16;

		// Token: 0x0400A8C9 RID: 43209
		public const int MaxCollisionBudget = 16;

		// Token: 0x0400A8CA RID: 43210
		public const int MaxGroundingSweepIterations = 2;

		// Token: 0x0400A8CB RID: 43211
		public const int MaxMovementSweepIterations = 6;

		// Token: 0x0400A8CC RID: 43212
		public const int MaxSteppingSweepIterations = 3;

		// Token: 0x0400A8CD RID: 43213
		public const int MaxRigidbodyOverlapsCount = 16;

		// Token: 0x0400A8CE RID: 43214
		public const int MaxDiscreteCollisionIterations = 3;

		// Token: 0x0400A8CF RID: 43215
		public const float CollisionOffset = 0.001f;

		// Token: 0x0400A8D0 RID: 43216
		public const float GroundProbeReboundDistance = 0.02f;

		// Token: 0x0400A8D1 RID: 43217
		public const float MinimumGroundProbingDistance = 0.005f;

		// Token: 0x0400A8D2 RID: 43218
		public const float GroundProbingBackstepDistance = 0.1f;

		// Token: 0x0400A8D3 RID: 43219
		public const float SweepProbingBackstepDistance = 0.002f;

		// Token: 0x0400A8D4 RID: 43220
		public const float SecondaryProbesVertical = 0.02f;

		// Token: 0x0400A8D5 RID: 43221
		public const float SecondaryProbesHorizontal = 0.001f;

		// Token: 0x0400A8D6 RID: 43222
		public const float MinVelocityMagnitude = 0.01f;

		// Token: 0x0400A8D7 RID: 43223
		public const float SteppingForwardDistance = 0.03f;

		// Token: 0x0400A8D8 RID: 43224
		public const float MinDistanceForLedge = 0.05f;

		// Token: 0x0400A8D9 RID: 43225
		public const float CorrelationForVerticalObstruction = 0.01f;

		// Token: 0x0400A8DA RID: 43226
		public const float ExtraSteppingForwardDistance = 0.01f;

		// Token: 0x0400A8DB RID: 43227
		public const float ExtraStepHeightPadding = 0.01f;
	}
}
