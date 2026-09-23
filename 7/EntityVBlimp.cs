using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004E2 RID: 1250
[Preserve]
public class EntityVBlimp : EntityDriveable
{
	// Token: 0x060028C2 RID: 10434 RVA: 0x000FD65F File Offset: 0x000FB85F
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		this.vehicleRB.useGravity = false;
	}

	// Token: 0x060028C3 RID: 10435 RVA: 0x000FD678 File Offset: 0x000FB878
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void PhysicsInputMove()
	{
		this.vehicleRB.velocity *= 0.996f;
		this.vehicleRB.velocity += new Vector3(0f, -0.001f, 0f);
		this.vehicleRB.angularVelocity *= 0.98f;
		if (this.movementInput != null)
		{
			float num = 2f;
			if (this.movementInput.running)
			{
				num *= 6f;
			}
			this.wheelMotor = this.movementInput.moveForward;
			this.vehicleRB.AddRelativeForce(0f, 0f, this.wheelMotor * num * 0.05f, ForceMode.VelocityChange);
			float num2;
			if (this.movementInput.lastInputController)
			{
				num2 = this.movementInput.moveStrafe * num;
			}
			else
			{
				num2 = this.movementInput.moveStrafe * num;
			}
			this.vehicleRB.AddRelativeTorque(0f, num2 * 0.01f, 0f, ForceMode.VelocityChange);
			if (this.movementInput.jump)
			{
				this.vehicleRB.AddRelativeForce(0f, 0.02f * num, 0f, ForceMode.VelocityChange);
			}
			if (this.movementInput.down)
			{
				this.vehicleRB.AddRelativeForce(0f, -0.02f * num, 0f, ForceMode.VelocityChange);
			}
		}
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetWheelsForces(float motorTorque, float motorTorqueBase, float brakeTorque, float _friction)
	{
	}

	// Token: 0x060028C5 RID: 10437 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateWheelsSteering()
	{
	}
}
