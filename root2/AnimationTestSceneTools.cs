using System;
using UnityEngine;

// Token: 0x02001534 RID: 5428
public class AnimationTestSceneTools : MonoBehaviour
{
	// Token: 0x0600AA63 RID: 43619 RVA: 0x003FACAE File Offset: 0x003F8EAE
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.weaponPrefabCount = this.weaponPrefabs.Length;
	}

	// Token: 0x0600AA64 RID: 43620 RVA: 0x003FACCC File Offset: 0x003F8ECC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			if (this.isCrouching)
			{
				this.isCrouching = false;
			}
			else
			{
				this.isCrouching = true;
			}
			this.anim.SetBool("IsCrouching", this.isCrouching);
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			this.locomotionState++;
			if (this.locomotionState >= this.locomotionSpeeds.Length)
			{
				this.locomotionState = 0;
			}
			this.forwardGoal = this.locomotionSpeeds[this.locomotionState];
		}
		if (Input.GetKey(KeyCode.A))
		{
			base.transform.Rotate(0f, this.turnRate * Time.deltaTime * -1f, 0f);
		}
		if (Input.GetKey(KeyCode.D))
		{
			base.transform.Rotate(0f, this.turnRate * Time.deltaTime, 0f);
		}
		if (Input.GetKeyUp(KeyCode.W))
		{
			this.weaponPrefabIndex++;
			this.anim.SetTrigger("ItemHasChangedTrigger");
			if (this.weaponPrefabIndex >= this.weaponPrefabCount)
			{
				this.weaponPrefabIndex = 0;
			}
			this.attachWeapon(this.weaponPrefabIndex);
		}
		if (Input.GetKeyUp(KeyCode.S))
		{
			this.weaponPrefabIndex--;
			this.anim.SetTrigger("ItemHasChangedTrigger");
			if (this.weaponPrefabIndex < 0)
			{
				this.weaponPrefabIndex = this.weaponPrefabCount;
			}
			this.attachWeapon(this.weaponPrefabIndex);
		}
		if (Input.GetKeyUp(KeyCode.R))
		{
			this.anim.SetTrigger("Reload");
		}
		if (Input.GetMouseButtonDown(0))
		{
			this.anim.SetTrigger("WeaponFire");
		}
		if (Input.GetMouseButtonUp(0))
		{
			this.anim.ResetTrigger("WeaponFire");
		}
		if (Input.GetMouseButtonDown(1))
		{
			this.anim.SetTrigger("IsAiming");
		}
		if (Input.GetMouseButtonUp(1))
		{
			this.anim.ResetTrigger("IsAiming");
		}
		if (Input.GetKeyUp(KeyCode.Q))
		{
			this.anim.SetTrigger("PowerAttack");
		}
		if (Input.GetKeyUp(KeyCode.E))
		{
			this.anim.SetTrigger("UseItem");
		}
		this.updateYLook();
		this.forward = Mathf.Lerp(this.forward, this.forwardGoal, 0.01f);
		this.anim.SetFloat("Forward", this.forward);
	}

	// Token: 0x0600AA65 RID: 43621 RVA: 0x003FAF24 File Offset: 0x003F9124
	[PublicizedFrom(EAccessModifier.Private)]
	public void attachWeapon(int weaponPrefabIndex)
	{
		this.removeAllWeapons();
		if (this.weaponPrefabs[weaponPrefabIndex] != null)
		{
			this.newWeapon = UnityEngine.Object.Instantiate<GameObject>(this.weaponPrefabs[weaponPrefabIndex]);
			this.newWeapon.transform.parent = this.weaponJoint.transform;
			this.newWeapon.transform.localPosition = Vector3.zero;
			this.newWeapon.transform.localEulerAngles = Vector3.zero;
		}
		Debug.Log(weaponPrefabIndex);
		this.anim.SetInteger("WeaponHoldType", this.weaponHoldTypes[weaponPrefabIndex]);
	}

	// Token: 0x0600AA66 RID: 43622 RVA: 0x003FAFC4 File Offset: 0x003F91C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeAllWeapons()
	{
		this.weaponJointChildrenCount = this.weaponJoint.childCount;
		if (this.weaponJointChildrenCount > 0)
		{
			for (int i = 0; i < this.weaponJointChildrenCount; i++)
			{
				this.existingChild = this.weaponJoint.GetChild(i).gameObject;
				UnityEngine.Object.Destroy(this.existingChild);
			}
		}
	}

	// Token: 0x0600AA67 RID: 43623 RVA: 0x003FB020 File Offset: 0x003F9220
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateYLook()
	{
		Vector3 mousePosition = Input.mousePosition;
		this.mousePosXRatio = mousePosition.x / (float)Screen.width;
		this.mousePosYRatio = mousePosition.y / (float)Screen.height;
		this.mousePosX = (this.mousePosXRatio - 0.5f) * 2f;
		this.mousePosY = (this.mousePosYRatio - 0.5f) * -2f;
		this.anim.SetFloat("YLook", this.mousePosY);
	}

	// Token: 0x04007EB8 RID: 32440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator anim;

	// Token: 0x04007EB9 RID: 32441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int layerIndex;

	// Token: 0x04007EBA RID: 32442
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float oneHandMeleeTargetWeight;

	// Token: 0x04007EBB RID: 32443
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float oneHandPistolTargetWeight;

	// Token: 0x04007EBC RID: 32444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int maxLayers;

	// Token: 0x04007EBD RID: 32445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float layerWeight;

	// Token: 0x04007EBE RID: 32446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float turnRate = 200f;

	// Token: 0x04007EBF RID: 32447
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int totalModels;

	// Token: 0x04007EC0 RID: 32448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentModel = 1;

	// Token: 0x04007EC1 RID: 32449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int weaponPrefabIndex;

	// Token: 0x04007EC2 RID: 32450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int weaponJointChildrenCount;

	// Token: 0x04007EC3 RID: 32451
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currOneHandMeleeWeight;

	// Token: 0x04007EC4 RID: 32452
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currOneHandPistolWeight;

	// Token: 0x04007EC5 RID: 32453
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCrouching;

	// Token: 0x04007EC6 RID: 32454
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int weaponPrefabCount;

	// Token: 0x04007EC7 RID: 32455
	public GameObject[] weaponPrefabs;

	// Token: 0x04007EC8 RID: 32456
	public int[] weaponHoldTypes;

	// Token: 0x04007EC9 RID: 32457
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] locomotionSpeeds = new float[]
	{
		0f,
		2.08f,
		4.2f
	};

	// Token: 0x04007ECA RID: 32458
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int locomotionState;

	// Token: 0x04007ECB RID: 32459
	public Transform weaponJoint;

	// Token: 0x04007ECC RID: 32460
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject existingChild;

	// Token: 0x04007ECD RID: 32461
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject newWeapon;

	// Token: 0x04007ECE RID: 32462
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mousePosXRatio;

	// Token: 0x04007ECF RID: 32463
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mousePosYRatio;

	// Token: 0x04007ED0 RID: 32464
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mousePosX;

	// Token: 0x04007ED1 RID: 32465
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mousePosY;

	// Token: 0x04007ED2 RID: 32466
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float forward;

	// Token: 0x04007ED3 RID: 32467
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float forwardGoal;

	// Token: 0x04007ED4 RID: 32468
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float strafe;

	// Token: 0x04007ED5 RID: 32469
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float YLook;

	// Token: 0x04007ED6 RID: 32470
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float horizontalMax = 4.2f;

	// Token: 0x04007ED7 RID: 32471
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float verticalMax = 4.2f;
}
