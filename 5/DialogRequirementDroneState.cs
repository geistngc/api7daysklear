using System;
using UnityEngine.Scripting;

// Token: 0x020002F7 RID: 759
[Preserve]
public class DialogRequirementDroneState : BaseDialogRequirement
{
	// Token: 0x17000272 RID: 626
	// (get) Token: 0x060015AE RID: 5550 RVA: 0x00081531 File Offset: 0x0007F731
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.DroneState;
		}
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x00081534 File Offset: 0x0007F734
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		EntityDrone entityDrone = talkingTo as EntityDrone;
		if (entityDrone)
		{
			EntityDrone.Orders orders;
			if (Enum.TryParse<EntityDrone.Orders>(base.Value, out orders))
			{
				return entityDrone.OrderState == orders;
			}
			EntityDrone.AllyHealMode allyHealMode;
			if (Enum.TryParse<EntityDrone.AllyHealMode>(base.Value, out allyHealMode) && entityDrone.IsHealModAttached)
			{
				return entityDrone.HealAllyMode == allyHealMode;
			}
			if (entityDrone.IsFlashlightAttached)
			{
				if (base.Value.Equals("LightOff"))
				{
					return !entityDrone.IsFlashlightOn;
				}
				if (base.Value.Equals("LightOn"))
				{
					return entityDrone.IsFlashlightOn;
				}
			}
			bool flag = entityDrone.TargetCanBeHealed(player);
			if (base.Value.Equals("Heal") && flag)
			{
				return flag;
			}
		}
		return false;
	}
}
