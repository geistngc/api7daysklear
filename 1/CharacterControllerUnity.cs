using System;
using UnityEngine;

// Token: 0x02000502 RID: 1282
public class CharacterControllerUnity : CharacterControllerAbstract
{
	// Token: 0x06002A08 RID: 10760 RVA: 0x001083CF File Offset: 0x001065CF
	public CharacterControllerUnity(CharacterController _cc)
	{
		this.cc = _cc;
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x001083DE File Offset: 0x001065DE
	public override void Enable(bool isEnabled)
	{
		this.cc.enabled = isEnabled;
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x001083EC File Offset: 0x001065EC
	public override void SetStepOffset(float _stepOffset)
	{
		this.cc.stepOffset = _stepOffset;
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x001083FA File Offset: 0x001065FA
	public override float GetStepOffset()
	{
		return this.cc.stepOffset;
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x00108407 File Offset: 0x00106607
	public override void SetSize(Vector3 _center, float _height, float _radius)
	{
		this.cc.center = _center;
		this.cc.height = _height;
		this.cc.radius = _radius;
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x0010842D File Offset: 0x0010662D
	public override void SetCenter(Vector3 _center)
	{
		this.cc.center = _center;
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x0010843B File Offset: 0x0010663B
	public override Vector3 GetCenter()
	{
		return this.cc.center;
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x00108448 File Offset: 0x00106648
	public override void SetRadius(float _radius)
	{
		this.cc.radius = _radius;
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x00108456 File Offset: 0x00106656
	public override float GetRadius()
	{
		return this.cc.radius;
	}

	// Token: 0x06002A11 RID: 10769 RVA: 0x00108463 File Offset: 0x00106663
	public override void SetSkinWidth(float _width)
	{
		this.cc.skinWidth = _width;
	}

	// Token: 0x06002A12 RID: 10770 RVA: 0x00108471 File Offset: 0x00106671
	public override float GetSkinWidth()
	{
		return this.cc.skinWidth;
	}

	// Token: 0x06002A13 RID: 10771 RVA: 0x0010847E File Offset: 0x0010667E
	public override void SetHeight(float _height)
	{
		this.cc.height = _height;
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x0010848C File Offset: 0x0010668C
	public override float GetHeight()
	{
		return this.cc.height;
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x00108499 File Offset: 0x00106699
	public override bool IsGrounded()
	{
		return this.cc.isGrounded;
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x001084A6 File Offset: 0x001066A6
	public override CollisionFlags Move(Vector3 _dir)
	{
		return this.cc.Move(_dir);
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000027FC File Offset: 0x000009FC
	public override void Rotate(Quaternion _dir)
	{
	}

	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x06002A18 RID: 10776 RVA: 0x001084B4 File Offset: 0x001066B4
	// (set) Token: 0x06002A19 RID: 10777 RVA: 0x001084C1 File Offset: 0x001066C1
	public override bool enableOverlapRecovery
	{
		get
		{
			return this.cc.enableOverlapRecovery;
		}
		set
		{
			this.cc.enableOverlapRecovery = value;
		}
	}

	// Token: 0x04001FEE RID: 8174
	[PublicizedFrom(EAccessModifier.Private)]
	public CharacterController cc;
}
