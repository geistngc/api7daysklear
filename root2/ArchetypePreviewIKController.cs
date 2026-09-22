using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class ArchetypePreviewIKController : MonoBehaviour
{
	// Token: 0x060003A9 RID: 937 RVA: 0x0001AF04 File Offset: 0x00019104
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.animator = base.GetComponent<Animator>();
		Transform transform = base.transform.FindInChilds("LeftFoot", false);
		Transform transform2 = base.transform.FindInChilds("RightFoot", false);
		this.leftFootPos = transform.position;
		this.leftFootRot = transform.rotation;
		this.rightFootPos = transform2.position;
		this.rightFootRot = transform2.rotation;
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0001AF74 File Offset: 0x00019174
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnAnimatorIK()
	{
		if (this.animator)
		{
			this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
			this.animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
			this.animator.SetIKPosition(AvatarIKGoal.LeftFoot, this.leftFootPos);
			this.animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Euler(this.leftFootRot.eulerAngles.x + this.FootRotationModifier.x, this.leftFootRot.eulerAngles.y - this.FootRotationModifier.y, this.leftFootRot.eulerAngles.z + this.FootRotationModifier.z));
			this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
			this.animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
			this.animator.SetIKPosition(AvatarIKGoal.RightFoot, this.rightFootPos);
			this.animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.Euler(this.rightFootRot.eulerAngles.x + this.FootRotationModifier.x, this.rightFootRot.eulerAngles.y + this.FootRotationModifier.y, this.rightFootRot.eulerAngles.z + this.FootRotationModifier.z));
		}
	}

	// Token: 0x04000442 RID: 1090
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Animator animator;

	// Token: 0x04000443 RID: 1091
	public bool ikActive;

	// Token: 0x04000444 RID: 1092
	public Transform rightHandObj;

	// Token: 0x04000445 RID: 1093
	public Transform lookObj;

	// Token: 0x04000446 RID: 1094
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 leftFootPos;

	// Token: 0x04000447 RID: 1095
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 rightFootPos;

	// Token: 0x04000448 RID: 1096
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion leftFootRot;

	// Token: 0x04000449 RID: 1097
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion rightFootRot;

	// Token: 0x0400044A RID: 1098
	public Vector3 FootRotationModifier = new Vector3(-62f, -198f, -93.5f);
}
