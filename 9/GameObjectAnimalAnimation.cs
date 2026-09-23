using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000E3 RID: 227
[Preserve]
public class GameObjectAnimalAnimation : AvatarController
{
	// Token: 0x060005C3 RID: 1475 RVA: 0x000297E8 File Offset: 0x000279E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.parentT = EModelBase.FindModel(base.transform);
		for (int i = this.parentT.childCount - 1; i >= 0; i--)
		{
			this.figureT = this.parentT.GetChild(i);
			if (this.figureT.gameObject.activeSelf)
			{
				break;
			}
		}
		this.anim = this.figureT.GetComponent<Animation>();
		if (this.anim["Idle1"])
		{
			this.anim.Play("Idle1");
		}
		this.attack1AS = this.anim["Attack1"];
		this.attack2AS = this.anim["Attack2"];
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x000298AD File Offset: 0x00027AAD
	public void SetAlwaysWalk(bool _b)
	{
		this.bAlwaysWalk = _b;
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x000298B6 File Offset: 0x00027AB6
	public override bool IsAnimationAttackPlaying()
	{
		return (this.attack1AS != null && this.attack1AS.enabled) || (this.attack2AS != null && this.attack2AS.enabled);
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x000298F0 File Offset: 0x00027AF0
	public override void StartAnimationAttack()
	{
		this.state = GameObjectAnimalAnimation.State.Attack;
		if (this.attack1AS != null)
		{
			if (this.attack2AS != null)
			{
				if (this.entity.rand.RandomFloat > 0.5f)
				{
					this.anim.Play("Attack1");
					return;
				}
				this.anim.Play("Attack2");
				return;
			}
			else
			{
				this.anim.Play("Attack1");
			}
		}
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x0002996C File Offset: 0x00027B6C
	public override void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
		if (this.isDead)
		{
			return;
		}
		this.state = GameObjectAnimalAnimation.State.Pain;
		if (this.anim["Pain"])
		{
			this.anim.Play("Pain");
		}
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x000299A8 File Offset: 0x00027BA8
	public override void StartAnimationJumping()
	{
		if (!this.entity.IsSwimming() && this.anim["Jump"] != null)
		{
			this.state = GameObjectAnimalAnimation.State.Jump;
			this.anim.CrossFade("Jump", 0.2f);
		}
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x000299F8 File Offset: 0x00027BF8
	public override void SetVisible(bool _b)
	{
		if (this.m_bVisible != _b || !this.visInit)
		{
			this.m_bVisible = _b;
			this.visInit = true;
			Transform transform = this.parentT;
			if (transform)
			{
				Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = _b;
				}
			}
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00029A54 File Offset: 0x00027C54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		if (!this.m_bVisible)
		{
			return;
		}
		if (this.entity == null)
		{
			return;
		}
		if (this.entity.IsDead())
		{
			if (!this.isDead)
			{
				this.isDead = true;
				this.anim.Stop();
				if (this.anim["Death"])
				{
					this.anim.CrossFade("Death", 0.5f);
				}
			}
			return;
		}
		if (!this.entity.Jumping && (this.attack1AS == null || !this.attack1AS.enabled) && (this.attack2AS == null || !this.attack2AS.enabled) && (this.anim["Death"] == null || !this.anim["Death"].enabled) && (this.anim["Pain"] == null || !this.anim["Pain"].enabled))
		{
			float num = this.lastAbsMotion;
			float num2 = Mathf.Abs(this.entity.position.x - this.entity.lastTickPos[0].x) * 6f;
			float num3 = Mathf.Abs(this.entity.position.z - this.entity.lastTickPos[0].z) * 6f;
			if (!this.entity.isEntityRemote)
			{
				if (Mathf.Abs(num2 - this.lastAbsMotionX) > 0.01f || Mathf.Abs(num3 - this.lastAbsMotionZ) > 0.01f)
				{
					num = Mathf.Sqrt(num2 * num2 + num3 * num3);
					this.lastAbsMotionX = num2;
					this.lastAbsMotionZ = num3;
					this.lastAbsMotion = num;
				}
			}
			else if (num2 > this.lastAbsMotionX || num3 > this.lastAbsMotionZ)
			{
				num = Mathf.Sqrt(num2 * num2 + num3 * num3);
				this.lastAbsMotionX = num2;
				this.lastAbsMotionZ = num3;
				this.lastAbsMotion = num;
			}
			else
			{
				this.lastAbsMotionX *= 0.9f;
				this.lastAbsMotionZ *= 0.9f;
				this.lastAbsMotion *= 0.9f;
			}
			if (this.bAlwaysWalk || num > 0.15f)
			{
				if (this.entity.IsSwimming() && this.anim["Swim"] != null)
				{
					this.state = GameObjectAnimalAnimation.State.Swim;
					if (!this.anim["Swim"].enabled)
					{
						this.anim.Play("Swim");
					}
					this.anim["Swim"].speed = Mathf.Clamp01(num * 2f);
					return;
				}
				if (num >= 1f)
				{
					if (this.state != GameObjectAnimalAnimation.State.Run)
					{
						this.state = GameObjectAnimalAnimation.State.Run;
						AnimationState animationState = this.anim["Run"];
						if (!animationState.enabled)
						{
							this.anim.CrossFade("Run", 0.5f);
						}
						animationState.speed = Utils.FastMin(num, 1.5f);
					}
				}
				else if (this.state != GameObjectAnimalAnimation.State.Run)
				{
					this.state = GameObjectAnimalAnimation.State.Walk;
					AnimationState animationState2 = this.anim["Walk"];
					if (!animationState2.enabled)
					{
						this.anim.CrossFade("Walk", 0.5f);
					}
					animationState2.speed = num * 2f;
				}
				if (this.stepSoundCounter <= 0f)
				{
					this.stepSoundCounter = 0.3f;
					return;
				}
			}
			else
			{
				this.state = GameObjectAnimalAnimation.State.Idle;
				if (this.anim["Idle2"] != null)
				{
					if (!this.anim["Idle1"].enabled && !this.anim["Idle2"].enabled)
					{
						if (this.entity.rand.RandomFloat > 0.5f)
						{
							this.anim.CrossFade("Idle1", 0.5f);
							return;
						}
						this.anim.CrossFade("Idle2", 0.5f);
						return;
					}
				}
				else if (!this.anim["Idle1"].enabled)
				{
					this.anim.CrossFade("Idle1", 0.5f);
				}
			}
		}
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00029EC5 File Offset: 0x000280C5
	public override Transform GetActiveModelRoot()
	{
		return this.figureT;
	}

	// Token: 0x04000660 RID: 1632
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimIdle1 = "Idle1";

	// Token: 0x04000661 RID: 1633
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimIdle2 = "Idle2";

	// Token: 0x04000662 RID: 1634
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimAttack1 = "Attack1";

	// Token: 0x04000663 RID: 1635
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimAttack2 = "Attack2";

	// Token: 0x04000664 RID: 1636
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimPain = "Pain";

	// Token: 0x04000665 RID: 1637
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimJump = "Jump";

	// Token: 0x04000666 RID: 1638
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimDeath = "Death";

	// Token: 0x04000667 RID: 1639
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimRun = "Run";

	// Token: 0x04000668 RID: 1640
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimWalk = "Walk";

	// Token: 0x04000669 RID: 1641
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimSwim = "Swim";

	// Token: 0x0400066A RID: 1642
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform parentT;

	// Token: 0x0400066B RID: 1643
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform figureT;

	// Token: 0x0400066C RID: 1644
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public new Animation anim;

	// Token: 0x0400066D RID: 1645
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AnimationState attack1AS;

	// Token: 0x0400066E RID: 1646
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AnimationState attack2AS;

	// Token: 0x0400066F RID: 1647
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool visInit;

	// Token: 0x04000670 RID: 1648
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_bVisible;

	// Token: 0x04000671 RID: 1649
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDead;

	// Token: 0x04000672 RID: 1650
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bAlwaysWalk;

	// Token: 0x04000673 RID: 1651
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAbsMotionX;

	// Token: 0x04000674 RID: 1652
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAbsMotionZ;

	// Token: 0x04000675 RID: 1653
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAbsMotion;

	// Token: 0x04000676 RID: 1654
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stepSoundCounter;

	// Token: 0x04000677 RID: 1655
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObjectAnimalAnimation.State state;

	// Token: 0x020000E4 RID: 228
	[PublicizedFrom(EAccessModifier.Private)]
	public enum State
	{
		// Token: 0x04000679 RID: 1657
		None,
		// Token: 0x0400067A RID: 1658
		Attack,
		// Token: 0x0400067B RID: 1659
		Idle,
		// Token: 0x0400067C RID: 1660
		Jump,
		// Token: 0x0400067D RID: 1661
		Pain,
		// Token: 0x0400067E RID: 1662
		Run,
		// Token: 0x0400067F RID: 1663
		Swim,
		// Token: 0x04000680 RID: 1664
		Walk
	}
}
