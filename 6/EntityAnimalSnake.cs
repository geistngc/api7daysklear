using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000485 RID: 1157
[Preserve]
public class EntityAnimalSnake : EntityEnemyAnimal
{
	// Token: 0x060023F4 RID: 9204 RVA: 0x000DA148 File Offset: 0x000D8348
	public override Vector3 GetAttackTargetHitPosition()
	{
		Vector3 position = this.attackTarget.position;
		position.y += 0.5f;
		return position;
	}
}
