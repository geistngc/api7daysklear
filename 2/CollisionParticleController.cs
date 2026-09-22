using System;
using UnityEngine;

// Token: 0x020008C3 RID: 2243
public class CollisionParticleController
{
	// Token: 0x060040F8 RID: 16632 RVA: 0x001968D1 File Offset: 0x00194AD1
	public void Init(int _entityId, string _colliderSurfaceCategory, string _collisionSurfaceCategory, int _layerMask)
	{
		this.entityId = _entityId;
		this.particleEffectName = string.Format("impact_{0}_on_{1}", _colliderSurfaceCategory, _collisionSurfaceCategory);
		this.soundName = string.Format("{0}hit{1}", _colliderSurfaceCategory, _collisionSurfaceCategory);
		this.layerMask = _layerMask;
		this.Reset();
	}

	// Token: 0x060040F9 RID: 16633 RVA: 0x0019690C File Offset: 0x00194B0C
	public void CheckCollision(Vector3 worldPos, Vector3 direction, float distance, int originEntityId = -1)
	{
		if (this.hasHit)
		{
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(worldPos - Origin.position, direction), out raycastHit, distance, this.layerMask))
		{
			Vector3 vector = raycastHit.point + Origin.position;
			float lightBrightness = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(vector));
			GameManager.Instance.SpawnParticleEffectServer(new ParticleEffect(this.particleEffectName, vector, Quaternion.FromToRotation(Vector3.up, raycastHit.normal), lightBrightness, Color.white, this.soundName, null, 1f, ""), (originEntityId == -1) ? this.entityId : originEntityId, false, false);
			this.hasHit = true;
		}
	}

	// Token: 0x060040FA RID: 16634 RVA: 0x001969C1 File Offset: 0x00194BC1
	public void Reset()
	{
		this.hasHit = false;
	}

	// Token: 0x040034C5 RID: 13509
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasHit;

	// Token: 0x040034C6 RID: 13510
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x040034C7 RID: 13511
	[PublicizedFrom(EAccessModifier.Private)]
	public string particleEffectName;

	// Token: 0x040034C8 RID: 13512
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundName;

	// Token: 0x040034C9 RID: 13513
	[PublicizedFrom(EAccessModifier.Private)]
	public int layerMask;
}
