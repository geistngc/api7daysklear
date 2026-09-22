using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A49 RID: 2633
public class CharacterGazeController : MonoBehaviour
{
	// Token: 0x1700088C RID: 2188
	// (get) Token: 0x06004FC8 RID: 20424 RVA: 0x001E3C00 File Offset: 0x001E1E00
	public Transform LookAtTarget
	{
		get
		{
			if (this.lookAtTarget == null)
			{
				this.lookAtTarget = new GameObject("LookAtTarget").transform;
				this.lookAtTarget.parent = this.rootTransform;
				this.lookAtTarget.localPosition = this.lookAtTargetDefaultPosition;
				return this.lookAtTarget;
			}
			return this.lookAtTarget;
		}
	}

	// Token: 0x1700088D RID: 2189
	// (get) Token: 0x06004FC9 RID: 20425 RVA: 0x001E3C5F File Offset: 0x001E1E5F
	// (set) Token: 0x06004FCA RID: 20426 RVA: 0x001E3C67 File Offset: 0x001E1E67
	public Transform LookAtTransformOverride
	{
		get
		{
			return this.lookAtTransformOverride;
		}
		set
		{
			this.lookAtTransformOverride = value;
		}
	}

	// Token: 0x1700088E RID: 2190
	// (get) Token: 0x06004FCB RID: 20427 RVA: 0x001E3C70 File Offset: 0x001E1E70
	public GameRandom Random
	{
		get
		{
			if (this.random != null)
			{
				return this.random;
			}
			return this.random = GameRandomManager.Instance.CreateGameRandom();
		}
	}

	// Token: 0x06004FCC RID: 20428 RVA: 0x001E3CA0 File Offset: 0x001E1EA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.entityAlive = base.GetComponentInParent<EntityAlive>();
		this.eyeMaterial = new Material(this.eyeSkinnedMeshRenderer.material);
		if (this.eyeMaterial == null || this.eyeMaterial.shader.name != "Game/SDCS/Eye")
		{
			Debug.LogError("Eye Material is not valid");
			base.enabled = false;
			return;
		}
		this.eyeSkinnedMeshRenderer.material = this.eyeMaterial;
		this.gazeTimer = Time.realtimeSinceStartup + this.Random.RandomRange(0.25f, 2f);
	}

	// Token: 0x06004FCD RID: 20429 RVA: 0x001E3D3D File Offset: 0x001E1F3D
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		CharacterGazeController.instances.Add(this);
	}

	// Token: 0x06004FCE RID: 20430 RVA: 0x001E3D4A File Offset: 0x001E1F4A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (CharacterGazeController.instances.Contains(this))
		{
			CharacterGazeController.instances.Remove(this);
		}
	}

	// Token: 0x06004FCF RID: 20431 RVA: 0x001E3D65 File Offset: 0x001E1F65
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		if (CharacterGazeController.instances.Contains(this))
		{
			CharacterGazeController.instances.Remove(this);
		}
		if (this.lookAtTarget != null)
		{
			UnityEngine.Object.Destroy(this.lookAtTarget.gameObject);
		}
	}

	// Token: 0x06004FD0 RID: 20432 RVA: 0x001E3DA0 File Offset: 0x001E1FA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.entityAlive != null)
		{
			if (this.entityAlive.emodel.IsRagdollActive)
			{
				return;
			}
			this.isDead = this.entityAlive.IsDead();
		}
		this.UpdateLookAtTarget();
		this.UpdateHeadRotation();
		this.UpdateEyeGaze();
	}

	// Token: 0x06004FD1 RID: 20433 RVA: 0x001E3DF4 File Offset: 0x001E1FF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLookAtTarget()
	{
		if (this.lookAtTransformOverride != null)
		{
			this.lookAtCamera = true;
			this.LookAtTarget.position = this.lookAtTransformOverride.position;
			return;
		}
		CharacterGazeController characterGazeController = null;
		float num = float.PositiveInfinity;
		for (int i = CharacterGazeController.instances.Count - 1; i >= 0; i--)
		{
			CharacterGazeController characterGazeController2 = CharacterGazeController.instances[i];
			if (characterGazeController2 == null)
			{
				CharacterGazeController.instances.RemoveAt(i);
			}
			else if (characterGazeController2.enabled && characterGazeController2 != this)
			{
				Vector3 to = characterGazeController2.headTransform.position - this.headTransform.position;
				float num2 = Vector3.Angle(this.neckTransform.forward, to);
				float magnitude = to.magnitude;
				if (num2 < this.eyeLookAtTargetAngle && magnitude <= this.maxLookAtDistance && num2 < num)
				{
					characterGazeController = characterGazeController2;
					num = num2;
				}
			}
		}
		if (characterGazeController != null)
		{
			this.lookAtCamera = true;
			this.LookAtTarget.position = characterGazeController.headTransform.position;
		}
		else
		{
			bool flag = this.entityAlive is EntityPlayerLocal;
			EntityPlayerLocal entityPlayerLocal;
			if (GameManager.Instance != null && GameManager.Instance.World != null)
			{
				entityPlayerLocal = GameManager.Instance.World.GetPrimaryPlayer();
			}
			else
			{
				entityPlayerLocal = null;
			}
			Vector3 vector = this.rootTransform.TransformPoint(this.lookAtTargetDefaultPosition);
			if (entityPlayerLocal != null && entityPlayerLocal.cameraTransform != null)
			{
				if (flag)
				{
					if (entityPlayerLocal.bFirstPersonView)
					{
						vector = entityPlayerLocal.cameraTransform.position;
					}
					else if (entityPlayerLocal.IsCameraFacingCharacter())
					{
						vector = entityPlayerLocal.cameraTransform.position;
					}
					else
					{
						vector = entityPlayerLocal.cameraTransform.position + entityPlayerLocal.cameraTransform.forward * 10f;
					}
				}
				else if (entityPlayerLocal.bFirstPersonView)
				{
					vector = entityPlayerLocal.cameraTransform.position;
				}
				else
				{
					vector = entityPlayerLocal.getHeadPosition();
				}
			}
			else if (Camera.main != null)
			{
				vector = Camera.main.transform.position;
			}
			Vector3 to2 = vector - this.headTransform.position;
			float num3 = Vector3.Angle(this.neckTransform.forward, to2);
			float magnitude2 = to2.magnitude;
			this.lookAtCamera = false;
			if (num3 < this.headLookAtTargetAngle && magnitude2 <= this.maxLookAtDistance)
			{
				this.lookAtCamera = true;
			}
			else if (entityPlayerLocal != null && flag && !entityPlayerLocal.bFirstPersonView && !entityPlayerLocal.inventory.holdingItem.IsActionRunning(entityPlayerLocal.inventory.holdingItemData))
			{
				this.lookAtCamera = true;
			}
			if (this.lookAtCamera)
			{
				this.LookAtTarget.position = vector;
			}
			else
			{
				this.LookAtTarget.localPosition = this.lookAtTargetDefaultPosition;
			}
		}
		if (Time.realtimeSinceStartup > this.gazeTimer)
		{
			this.lookatOffsetIndex = this.Random.RandomRange(0, this.lookatOffsets.Count);
			this.gazeTimer = Time.realtimeSinceStartup + this.Random.RandomRange(0.25f, 2f);
		}
	}

	// Token: 0x06004FD2 RID: 20434 RVA: 0x001E411F File Offset: 0x001E231F
	public void SnapNextUpdate()
	{
		this.shouldSnapHeadNextUpdate = true;
		this.shouldSnapEyesNextUpdate = true;
	}

	// Token: 0x06004FD3 RID: 20435 RVA: 0x001E4130 File Offset: 0x001E2330
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHeadRotation()
	{
		Vector3 normalized = (this.LookAtTarget.position - this.headTransform.position).normalized;
		float num = Vector3.Angle(this.neckTransform.forward, normalized);
		Vector3 normalized2 = this.neckTransform.InverseTransformDirection(normalized).normalized;
		float num2 = Mathf.Atan2(normalized2.x, normalized2.z) * 57.29578f;
		float num3 = Mathf.Atan2(normalized2.y, new Vector2(normalized2.x, normalized2.z).magnitude) * 57.29578f;
		num2 *= 0.5f;
		float num4 = num3 * 0.75f;
		Quaternion rotation = Quaternion.AngleAxis(num2, this.neckTransform.up);
		Vector3 point = rotation * this.neckTransform.forward;
		Vector3 axis = rotation * this.neckTransform.right;
		Vector3 normalized3 = (Quaternion.AngleAxis(-num4, axis) * point).normalized;
		if (num < this.headLookAtTargetAngle && !this.isDead)
		{
			Quaternion quaternion = Quaternion.FromToRotation(this.neckTransform.forward, normalized3);
			quaternion *= Quaternion.Euler(0f, 0f, -(num2 / this.headLookAtTargetAngle) * 15f);
			if (this.shouldSnapHeadNextUpdate)
			{
				this.currentHeadRotation = quaternion;
				this.shouldSnapHeadNextUpdate = false;
			}
			else
			{
				this.currentHeadRotation = Quaternion.Slerp(this.currentHeadRotation, quaternion, this.headRotationSpeed * Time.deltaTime);
			}
		}
		else if (this.shouldSnapHeadNextUpdate)
		{
			this.currentHeadRotation = Quaternion.identity;
			this.shouldSnapHeadNextUpdate = false;
		}
		else
		{
			this.currentHeadRotation = Quaternion.Slerp(this.currentHeadRotation, Quaternion.identity, this.headRotationSpeed * Time.deltaTime);
		}
		this.headTransform.rotation = this.currentHeadRotation * this.neckTransform.rotation;
	}

	// Token: 0x06004FD4 RID: 20436 RVA: 0x001E430C File Offset: 0x001E250C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEyeGaze()
	{
		Vector3 localPosition = this.LookAtTarget.localPosition;
		if (!this.lookAtCamera)
		{
			this.LookAtTarget.position += this.lookatOffsets[this.lookatOffsetIndex];
		}
		Vector3 toDirection = this.LookAtTarget.position - this.headTransform.TransformPoint(this.leftEyeLocalPosition);
		Vector3 toDirection2 = this.LookAtTarget.position - this.headTransform.TransformPoint(this.rightEyeLocalPosition);
		Quaternion b = Quaternion.FromToRotation(this.leftEyeTransform.forward, toDirection);
		Quaternion b2 = Quaternion.FromToRotation(this.rightEyeTransform.forward, toDirection2);
		float num = Mathf.Sin(Time.time * this.twitchSpeed) * 0.5f + 0.5f;
		if (this.isDead)
		{
			b = Quaternion.identity;
			b2 = Quaternion.identity;
		}
		if (this.shouldSnapEyesNextUpdate)
		{
			this.currentLeftEyeRotation = b;
			this.currentRightEyeRotation = b2;
			this.shouldSnapEyesNextUpdate = false;
		}
		else
		{
			this.currentLeftEyeRotation = Quaternion.Slerp(this.currentLeftEyeRotation, b, this.eyeRotationSpeed * num * Time.deltaTime);
			this.currentRightEyeRotation = Quaternion.Slerp(this.currentRightEyeRotation, b2, this.eyeRotationSpeed * num * Time.deltaTime);
		}
		this.eyeMaterial.SetVector("_LeftEyeRotation", new Vector4(-this.currentLeftEyeRotation.x, this.currentLeftEyeRotation.y, this.currentLeftEyeRotation.z, this.currentLeftEyeRotation.w));
		this.eyeMaterial.SetVector("_RightEyeRotation", new Vector4(-this.currentRightEyeRotation.x, this.currentRightEyeRotation.y, this.currentRightEyeRotation.z, this.currentRightEyeRotation.w));
		this.eyeMaterial.SetVector("_LeftEyePosition", Vector3.Scale(this.leftEyeLocalPosition, this.leftEyeTransform.lossyScale));
		this.eyeMaterial.SetVector("_RightEyePosition", Vector3.Scale(this.rightEyeLocalPosition, this.rightEyeTransform.lossyScale));
		this.LookAtTarget.localPosition = localPosition;
	}

	// Token: 0x06004FD5 RID: 20437 RVA: 0x001E453A File Offset: 0x001E273A
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void Cleanup()
	{
		CharacterGazeController.instances.Clear();
	}

	// Token: 0x04003D38 RID: 15672
	public static List<CharacterGazeController> instances = new List<CharacterGazeController>();

	// Token: 0x04003D39 RID: 15673
	public Transform leftEyeTransform;

	// Token: 0x04003D3A RID: 15674
	public Transform rightEyeTransform;

	// Token: 0x04003D3B RID: 15675
	public Vector3 leftEyeLocalPosition;

	// Token: 0x04003D3C RID: 15676
	public Vector3 rightEyeLocalPosition;

	// Token: 0x04003D3D RID: 15677
	public Transform rootTransform;

	// Token: 0x04003D3E RID: 15678
	public Transform neckTransform;

	// Token: 0x04003D3F RID: 15679
	public Transform headTransform;

	// Token: 0x04003D40 RID: 15680
	[Range(0f, 100f)]
	public float eyeRotationSpeed = 30f;

	// Token: 0x04003D41 RID: 15681
	[Range(0f, 100f)]
	public float headRotationSpeed = 7f;

	// Token: 0x04003D42 RID: 15682
	[Range(0f, 50f)]
	public float twitchSpeed = 25f;

	// Token: 0x04003D43 RID: 15683
	public float eyeLookAtTargetAngle = 35f;

	// Token: 0x04003D44 RID: 15684
	public float headLookAtTargetAngle = 75f;

	// Token: 0x04003D45 RID: 15685
	[Range(0f, 20f)]
	public float maxLookAtDistance = 5f;

	// Token: 0x04003D46 RID: 15686
	public SkinnedMeshRenderer eyeSkinnedMeshRenderer;

	// Token: 0x04003D47 RID: 15687
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material eyeMaterial;

	// Token: 0x04003D48 RID: 15688
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform lookAtTransformOverride;

	// Token: 0x04003D49 RID: 15689
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lookAtCamera;

	// Token: 0x04003D4A RID: 15690
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform lookAtTarget;

	// Token: 0x04003D4B RID: 15691
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameRandom random;

	// Token: 0x04003D4C RID: 15692
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> lookatOffsets = new List<Vector3>
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(-2f, 0f, 0f),
		new Vector3(2f, 0f, 0f),
		new Vector3(0f, 0f, -1f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 0f, -2f),
		new Vector3(0f, 0f, 2f)
	};

	// Token: 0x04003D4D RID: 15693
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive entityAlive;

	// Token: 0x04003D4E RID: 15694
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentLeftEyeRotation;

	// Token: 0x04003D4F RID: 15695
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentRightEyeRotation;

	// Token: 0x04003D50 RID: 15696
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentHeadRotation;

	// Token: 0x04003D51 RID: 15697
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float gazeTimer;

	// Token: 0x04003D52 RID: 15698
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lookatOffsetIndex;

	// Token: 0x04003D53 RID: 15699
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shouldSnapHeadNextUpdate;

	// Token: 0x04003D54 RID: 15700
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shouldSnapEyesNextUpdate;

	// Token: 0x04003D55 RID: 15701
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDead;

	// Token: 0x04003D56 RID: 15702
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lookAtTargetDefaultPosition = new Vector3(0f, 1.7f, 10f);
}
