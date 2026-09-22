using System;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class AvatarRootMotion : MonoBehaviour
{
	// Token: 0x0600051F RID: 1311 RVA: 0x00023332 File Offset: 0x00021532
	public void Init(AvatarController _mainController, Animator _root)
	{
		this.mainController = _mainController;
		this.root = _root;
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x00023342 File Offset: 0x00021542
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnAnimatorMove()
	{
		if (this.mainController != null && this.root != null)
		{
			this.mainController.NotifyAnimatorMove(this.root);
		}
	}

	// Token: 0x04000580 RID: 1408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AvatarController mainController;

	// Token: 0x04000581 RID: 1409
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator root;
}
