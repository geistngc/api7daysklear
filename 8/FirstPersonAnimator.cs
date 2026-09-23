using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class FirstPersonAnimator : BodyAnimator
{
	// Token: 0x0600058A RID: 1418 RVA: 0x00027760 File Offset: 0x00025960
	public FirstPersonAnimator(EntityAlive _entity, AvatarCharacterController.AnimationStates _animStates, Transform _bodyTransform, BodyAnimator.EnumState _defaultState)
	{
		BodyAnimator.BodyParts bodyParts = new BodyAnimator.BodyParts(_bodyTransform, _bodyTransform.FindInChilds((_entity.emodel is EModelSDCS) ? "RightWeapon" : "Gunjoint", false));
		base.initBodyAnimator(_entity, bodyParts, _defaultState);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x000277A4 File Offset: 0x000259A4
	public override void SetDrunk(float _numBeers)
	{
		Animator animator = base.Animator;
		if (animator)
		{
			animator.SetFloat("drunk", _numBeers);
		}
	}
}
