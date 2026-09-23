using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000452 RID: 1106
[Preserve]
public class EAIRunawayWhenHurt : EAIRunAway
{
	// Token: 0x060021C3 RID: 8643 RVA: 0x000CC2D5 File Offset: 0x000CA4D5
	public EAIRunawayWhenHurt()
	{
		this.MutexBits = 1;
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x000CC2F0 File Offset: 0x000CA4F0
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		string input;
		if (data.TryGetValue("runChance", out input))
		{
			this.lowHealthPercent = 0f;
			if (StringParsers.ParseFloat(input, 0, -1, NumberStyles.Any) >= base.RandomFloat)
			{
				base.GetData(data, "healthPer", ref this.lowHealthPercent);
				if (data.TryGetValue("healthPerMax", out input))
				{
					float num = StringParsers.ParseFloat(input, 0, -1, NumberStyles.Any);
					this.lowHealthPercent += base.RandomFloat * (num - this.lowHealthPercent);
				}
			}
		}
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x000CC380 File Offset: 0x000CA580
	public override bool CanExecute()
	{
		this.enemy = this.theEntity.GetRevengeTarget();
		return this.enemy && (this.lowHealthPercent >= 1f || (float)this.theEntity.Health / (float)this.theEntity.GetMaxHealth() < this.lowHealthPercent) && base.CanExecute();
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x000CC3E3 File Offset: 0x000CA5E3
	public override bool Continue()
	{
		return base.Continue();
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x000CC3EB File Offset: 0x000CA5EB
	public override void Update()
	{
		base.Update();
		this.theEntity.navigator.setMoveSpeed(this.theEntity.IsInWater() ? this.theEntity.GetMoveSpeed() : this.theEntity.GetMoveSpeedPanic());
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x000CC428 File Offset: 0x000CA628
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 GetFleeFromPos()
	{
		if (this.enemy)
		{
			return this.enemy.position;
		}
		return this.theEntity.position;
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000CC44E File Offset: 0x000CA64E
	public override string ToString()
	{
		return string.Format("{0}, per {1}", base.ToString(), this.lowHealthPercent);
	}

	// Token: 0x04001755 RID: 5973
	[PublicizedFrom(EAccessModifier.Private)]
	public float lowHealthPercent = 1f;
}
