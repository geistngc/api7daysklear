using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000453 RID: 1107
[Preserve]
public class EAISetAsTargetIfHurt : EAITarget
{
	// Token: 0x060021CA RID: 8650 RVA: 0x000CC46B File Offset: 0x000CA66B
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity, 0f, false);
		this.MutexBits = 1;
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000CC484 File Offset: 0x000CA684
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		this.targetClasses = new List<EAISetAsTargetIfHurt.TargetClass>();
		string text;
		if (data.TryGetValue("class", out text))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				EAISetAsTargetIfHurt.TargetClass item = default(EAISetAsTargetIfHurt.TargetClass);
				item.type = EntityFactory.GetEntityType(array[i]);
				this.targetClasses.Add(item);
			}
		}
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x000CC4F0 File Offset: 0x000CA6F0
	public override bool CanExecute()
	{
		EntityAlive revengeTarget = this.theEntity.GetRevengeTarget();
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (revengeTarget && revengeTarget != attackTarget && revengeTarget.entityType != this.theEntity.entityType)
		{
			if (this.targetClasses != null)
			{
				bool flag = false;
				Type type = revengeTarget.GetType();
				for (int i = 0; i < this.targetClasses.Count; i++)
				{
					EAISetAsTargetIfHurt.TargetClass targetClass = this.targetClasses[i];
					if (targetClass.type != null && targetClass.type.IsAssignableFrom(type))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			if (attackTarget != null && attackTarget.IsAlive() && base.RandomFloat < 0.66f)
			{
				this.theEntity.SetRevengeTarget(null);
				return false;
			}
			if (base.check(revengeTarget))
			{
				return true;
			}
			Vector3 vector = this.theEntity.position - revengeTarget.position;
			float searchRadius = EntityClass.list[this.theEntity.entityClass].SearchRadius;
			vector = revengeTarget.position + vector.normalized * (searchRadius * 0.35f);
			Vector2 vector2 = this.manager.random.RandomInsideUnitCircle * searchRadius;
			vector.x += vector2.x;
			vector.z += vector2.y;
			Vector3i vector3i = World.worldToBlockPos(vector);
			int height = (int)this.theEntity.world.GetHeight(vector3i.x, vector3i.z);
			if (height > 0)
			{
				vector.y = (float)height;
			}
			int ticks = this.theEntity.CalcInvestigateTicks(1200, revengeTarget);
			this.theEntity.SetInvestigatePosition(vector, ticks, true);
			this.theEntity.SetRevengeTarget(null);
		}
		return false;
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000CC6D4 File Offset: 0x000CA8D4
	public override void Start()
	{
		this.theEntity.SetAttackTarget(this.theEntity.GetRevengeTarget(), 400);
		this.viewAngleSave = this.theEntity.GetMaxViewAngle();
		this.theEntity.SetMaxViewAngle(270f);
		this.viewAngleRestoreCounter = 100;
		base.Start();
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000CC72B File Offset: 0x000CA92B
	public override void Update()
	{
		if (this.viewAngleRestoreCounter > 0)
		{
			this.viewAngleRestoreCounter--;
			if (this.viewAngleRestoreCounter == 0)
			{
				this.restoreViewAngle();
			}
		}
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000CC752 File Offset: 0x000CA952
	public override bool Continue()
	{
		return (!(this.theEntity.GetRevengeTarget() != null) || !(this.theEntity.GetAttackTarget() != this.theEntity.GetRevengeTarget())) && base.Continue();
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x000CC78C File Offset: 0x000CA98C
	public override void Reset()
	{
		base.Reset();
		this.restoreViewAngle();
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x000CC79A File Offset: 0x000CA99A
	[PublicizedFrom(EAccessModifier.Private)]
	public void restoreViewAngle()
	{
		if (this.viewAngleSave > 0f)
		{
			this.theEntity.SetMaxViewAngle(this.viewAngleSave);
			this.viewAngleSave = 0f;
			this.viewAngleRestoreCounter = 0;
		}
	}

	// Token: 0x04001756 RID: 5974
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAISetAsTargetIfHurt.TargetClass> targetClasses;

	// Token: 0x04001757 RID: 5975
	[PublicizedFrom(EAccessModifier.Private)]
	public float viewAngleSave;

	// Token: 0x04001758 RID: 5976
	[PublicizedFrom(EAccessModifier.Private)]
	public int viewAngleRestoreCounter;

	// Token: 0x02000454 RID: 1108
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TargetClass
	{
		// Token: 0x04001759 RID: 5977
		public Type type;
	}
}
