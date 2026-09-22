using System;
using UnityEngine;

// Token: 0x02000542 RID: 1346
public class DamageSourceEntity : DamageSource
{
	// Token: 0x06002C44 RID: 11332 RVA: 0x00117CFE File Offset: 0x00115EFE
	public DamageSourceEntity(EnumDamageSource _damageSource, EnumDamageTypes _damageType, int _damageSourceEntityId) : base(_damageSource, _damageType)
	{
		this.ownerEntityId = _damageSourceEntityId;
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x00117D0F File Offset: 0x00115F0F
	public DamageSourceEntity(EnumDamageSource _damageSource, EnumDamageTypes _damageType, int _damageSourceEntityId, Vector3 _direction) : base(_damageSource, _damageType, _direction)
	{
		this.ownerEntityId = _damageSourceEntityId;
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x00117D22 File Offset: 0x00115F22
	public DamageSourceEntity(EnumDamageSource _damageSource, EnumDamageTypes _damageType, int _damageSourceEntityId, Vector3 _direction, string _hitTransformName, Vector3 _hitTransformPosition, Vector2 _uvHit) : this(_damageSource, _damageType, _damageSourceEntityId, _direction)
	{
		this.hitTransformName = _hitTransformName;
		this.hitTransformPosition = _hitTransformPosition;
		this.uvHit = _uvHit;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x00117D47 File Offset: 0x00115F47
	public override Vector3 getHitTransformPosition()
	{
		return this.hitTransformPosition;
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x00117D4F File Offset: 0x00115F4F
	public override string getHitTransformName()
	{
		return this.hitTransformName;
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x00117D57 File Offset: 0x00115F57
	public override Vector2 getUVHit()
	{
		return this.uvHit;
	}

	// Token: 0x040021E6 RID: 8678
	public Vector2 uvHit;

	// Token: 0x040021E7 RID: 8679
	public string hitTransformName;

	// Token: 0x040021E8 RID: 8680
	public Vector3 hitTransformPosition;
}
