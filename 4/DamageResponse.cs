using System;

// Token: 0x0200046E RID: 1134
public struct DamageResponse
{
	// Token: 0x06002226 RID: 8742 RVA: 0x000CE4D8 File Offset: 0x000CC6D8
	public static DamageResponse New(bool _fatal)
	{
		return new DamageResponse
		{
			HitBodyPart = EnumBodyPartHit.Torso,
			Random = GameManager.Instance.World.GetGameRandom().RandomFloat,
			Fatal = _fatal,
			PainHit = !_fatal,
			ImpulseScale = 1f,
			ArmorSlot = EquipmentSlots.Count,
			ArmorDamage = 0
		};
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x000CE544 File Offset: 0x000CC744
	public static DamageResponse New(DamageSource _source, bool _fatal)
	{
		return new DamageResponse
		{
			HitBodyPart = EnumBodyPartHit.Torso,
			Random = GameManager.Instance.World.GetGameRandom().RandomFloat,
			Fatal = _fatal,
			PainHit = !_fatal,
			Source = _source,
			ImpulseScale = 1f,
			ArmorSlot = EquipmentSlots.Count,
			ArmorDamage = 0
		};
	}

	// Token: 0x040017CD RID: 6093
	public DamageSource Source;

	// Token: 0x040017CE RID: 6094
	public int Strength;

	// Token: 0x040017CF RID: 6095
	public int ModStrength;

	// Token: 0x040017D0 RID: 6096
	public int MovementState;

	// Token: 0x040017D1 RID: 6097
	public Utils.EnumHitDirection HitDirection;

	// Token: 0x040017D2 RID: 6098
	public EnumBodyPartHit HitBodyPart;

	// Token: 0x040017D3 RID: 6099
	public bool PainHit;

	// Token: 0x040017D4 RID: 6100
	public bool Fatal;

	// Token: 0x040017D5 RID: 6101
	public bool Critical;

	// Token: 0x040017D6 RID: 6102
	public bool Dismember;

	// Token: 0x040017D7 RID: 6103
	public bool CrippleLegs;

	// Token: 0x040017D8 RID: 6104
	public bool TurnIntoCrawler;

	// Token: 0x040017D9 RID: 6105
	public float Random;

	// Token: 0x040017DA RID: 6106
	public float ImpulseScale;

	// Token: 0x040017DB RID: 6107
	public EnumEntityStunType Stun;

	// Token: 0x040017DC RID: 6108
	public float StunDuration;

	// Token: 0x040017DD RID: 6109
	public EquipmentSlots ArmorSlot;

	// Token: 0x040017DE RID: 6110
	public EquipmentSlotGroups ArmorSlotGroup;

	// Token: 0x040017DF RID: 6111
	public int ArmorDamage;
}
