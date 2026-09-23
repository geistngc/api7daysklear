using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000448 RID: 1096
[Preserve]
public class EAILook : EAIBase
{
	// Token: 0x06002173 RID: 8563 RVA: 0x000C98A9 File Offset: 0x000C7AA9
	public EAILook()
	{
		this.MutexBits = 1;
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000C98B8 File Offset: 0x000C7AB8
	public override bool CanExecute()
	{
		return this.manager.lookTime > 0f && !this.theEntity.Jumping;
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000C98DC File Offset: 0x000C7ADC
	public override void Start()
	{
		this.waitTicks = (int)(this.manager.lookTime * 20f);
		this.manager.lookTime = 0f;
		this.theEntity.GetEntitySenses().Clear();
		this.lookAtTicks = 0;
		this.turnTicks = 0;
		this.theEntity.moveHelper.Stop();
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x000C9940 File Offset: 0x000C7B40
	public override bool Continue()
	{
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		int num;
		if (this.theEntity.IsAlert)
		{
			this.waitTicks--;
			this.lookAtTicks -= 2;
			num = this.turnTicks - 1;
			this.turnTicks = num;
			if (num <= 0)
			{
				this.turnTicks = 14;
				this.theEntity.SeekYaw(this.theEntity.rotation.y + (base.RandomFloat * 120f - 60f), 0f, 35f);
			}
		}
		num = this.waitTicks - 1;
		this.waitTicks = num;
		if (num <= 0)
		{
			return false;
		}
		num = this.lookAtTicks - 1;
		this.lookAtTicks = num;
		if (num <= 0)
		{
			this.lookAtTicks = 40;
			Vector3 headPosition = this.theEntity.getHeadPosition();
			Vector3 vector = this.theEntity.GetForwardVector() * 20f;
			vector = Quaternion.Euler(base.RandomFloat * 60f - 30f, base.RandomFloat * 120f - 60f, 0f) * vector;
			this.theEntity.SetLookPosition(headPosition + vector);
		}
		return true;
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x000C9A7B File Offset: 0x000C7C7B
	public override void Reset()
	{
		this.theEntity.SetLookPosition(Vector3.zero);
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x000C9A8D File Offset: 0x000C7C8D
	public override string ToString()
	{
		return string.Format("{0}, wait {1}", base.ToString(), ((float)this.waitTicks / 20f).ToCultureInvariantString());
	}

	// Token: 0x04001702 RID: 5890
	[PublicizedFrom(EAccessModifier.Private)]
	public int waitTicks;

	// Token: 0x04001703 RID: 5891
	[PublicizedFrom(EAccessModifier.Private)]
	public int lookAtTicks;

	// Token: 0x04001704 RID: 5892
	[PublicizedFrom(EAccessModifier.Private)]
	public int turnTicks;
}
