using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Audio;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004E3 RID: 1251
[UnityEngine.Scripting.Preserve]
public class EntityVehicle : EntityAlive, ILockable
{
	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x060028C7 RID: 10439 RVA: 0x0002F184 File Offset: 0x0002D384
	public override Entity.EnumPositionUpdateMovementType positionUpdateMovementType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return Entity.EnumPositionUpdateMovementType.Instant;
		}
	}

	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x060028C8 RID: 10440 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsValidAimAssistSnapTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x060028C9 RID: 10441 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsValidAimAssistSlowdownTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x000FD7DC File Offset: 0x000FB9DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.isLocked = false;
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x000FD7EC File Offset: 0x000FB9EC
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		EntityClass entityClass = EntityClass.list[this.entityClass];
		this.vehicle = new Vehicle(entityClass.entityClassName, this);
		base.transform.tag = "E_Vehicle";
		Vector2i size = LootContainer.GetLootContainer(this.GetLootList(), true).size;
		this.bag.SetSlots(ItemStack.CreateArray(size.x * size.y));
		Transform physicsTransform = this.PhysicsTransform;
		this.vehicleRB = physicsTransform.GetComponent<Rigidbody>();
		if (this.vehicleRB)
		{
			this.vehicleRB.useGravity = false;
			if (this.vehicleRB.automaticCenterOfMass)
			{
				this.vehicleRB.centerOfMass = new Vector3(0f, 0.1f, 0f);
			}
			this.vehicleRB.sleepThreshold = this.vehicleRB.mass * 0.01f * 0.01f * 0.5f;
			physicsTransform.gameObject.AddComponent<CollisionCallForward>().Entity = this;
			physicsTransform.gameObject.layer = 21;
			Utils.SetTagsIfNoneRecursively(physicsTransform, "E_Vehicle");
			this.SetupDevices();
			this.SetVehicleDriven();
			if (!this.isEntityRemote)
			{
				this.isTryToFall = true;
			}
		}
		this.alertEnabled = false;
		GameManager.Instance.StartCoroutine(this.ApplyCollisionsCoroutine());
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddCharacterController()
	{
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x000FD944 File Offset: 0x000FBB44
	public override void PostInit()
	{
		this.LogVehicle("PostInit {0}, {1} (chunk {2}), rbPos {3}", new object[]
		{
			this,
			this.position,
			World.toChunkXZ(this.position),
			this.vehicleRB.position + Origin.position
		});
		base.transform.rotation = this.qrotation;
		if (this.vehicleRB)
		{
			this.PhysicsResetAndSleep();
			this.PhysicsTransform.rotation = this.qrotation;
			this.SetVehicleDriven();
		}
		this.HandleNavObject();
		this.UpdateContainerSize(true);
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x000FD9EC File Offset: 0x000FBBEC
	public bool CanRemoveInventoryMod()
	{
		Vector2i size = LootContainer.GetLootContainer(this.GetLootList(), true).size;
		int num = 0;
		ItemStack[] slots = this.bag.GetSlots();
		foreach (ItemStack itemStack in slots)
		{
			if (itemStack != null && !itemStack.IsEmpty())
			{
				num++;
			}
		}
		return num <= slots.Length - size.x;
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x000FDA54 File Offset: 0x000FBC54
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i getStorageSize()
	{
		Vector2i size = LootContainer.GetLootContainer(this.GetLootList(), true).size;
		size.y += this.storageModCount;
		return size;
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x000FDA85 File Offset: 0x000FBC85
	public void UpdateStorageModCount(int _storageModCount)
	{
		this.storageModCount = _storageModCount;
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x000FDA90 File Offset: 0x000FBC90
	public void UpdateContainerSize(bool forceUpdate = false)
	{
		Vector2i storageSize = this.getStorageSize();
		int num = storageSize.x * storageSize.y;
		int num2 = this.bag.GetSlots().Length;
		if (forceUpdate || num != num2)
		{
			if (num != num2)
			{
				ItemStack[] slots = this.bag.GetSlots();
				ItemStack[] array = new ItemStack[num];
				List<ItemStack> list = new List<ItemStack>();
				for (int i = 0; i < num2; i++)
				{
					ItemStack itemStack = slots[i];
					if (!itemStack.IsEmpty())
					{
						list.Add(itemStack.Clone());
					}
				}
				int count = list.Count;
				if (num >= count)
				{
					int i;
					for (i = 0; i < count; i++)
					{
						array[i] = list[i];
					}
					while (i < num)
					{
						array[i] = ItemStack.Empty;
						i++;
					}
				}
				else
				{
					int i;
					for (i = 0; i < num; i++)
					{
						array[i] = list[i];
					}
					ItemStack[] array2 = new ItemStack[count - num];
					int num3 = 0;
					while (i < count)
					{
						array2[num3++] = list[i];
						i++;
					}
					this.dropLoot(array2, 1.5f);
				}
				this.bag.SetSlots(array);
			}
			if (!this.isReadingFromRemote)
			{
				this.SetBagModified();
			}
		}
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x000FDBD4 File Offset: 0x000FBDD4
	public override void InitInventory()
	{
		this.inventory = new EntityVehicle.VehicleInventory(GameManager.Instance, this);
	}

	// Token: 0x060028D3 RID: 10451 RVA: 0x000FDBE7 File Offset: 0x000FBDE7
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupDevices()
	{
		this.SetupMotors();
		this.SetupForces();
		this.SetupWheels();
	}

	// Token: 0x060028D4 RID: 10452 RVA: 0x000FDBFC File Offset: 0x000FBDFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupForces()
	{
		DynamicProperties properties = this.vehicle.Properties;
		if (properties == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		DynamicProperties dynamicProperties;
		while (num2 < 99 && properties.Classes.TryGetValue("force" + num2.ToString(), out dynamicProperties))
		{
			num++;
			num2++;
		}
		this.forces = new EntityVehicle.Force[num];
		for (int i = 0; i < this.forces.Length; i++)
		{
			EntityVehicle.Force force = new EntityVehicle.Force();
			this.forces[i] = force;
			DynamicProperties dynamicProperties2 = properties.Classes["force" + i.ToString()];
			force.ceiling.x = 9999f;
			force.ceiling.y = 9999f;
			dynamicProperties2.ParseVec("ceiling", ref force.ceiling);
			force.ceiling.y = 1f / Utils.FastMax(0.5f, force.ceiling.y - force.ceiling.x);
			force.force = Vector3.forward;
			dynamicProperties2.ParseVec("force", ref force.force);
			force.trigger = EntityVehicle.Force.Trigger.On;
			dynamicProperties2.ParseEnum<EntityVehicle.Force.Trigger>("trigger", ref force.trigger);
			force.type = EntityVehicle.Force.Type.Relative;
			dynamicProperties2.ParseEnum<EntityVehicle.Force.Type>("type", ref force.type);
		}
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x000FDD60 File Offset: 0x000FBF60
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupMotors()
	{
		DynamicProperties properties = this.vehicle.Properties;
		if (properties == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		DynamicProperties dynamicProperties;
		while (num2 < 99 && properties.Classes.TryGetValue("motor" + num2.ToString(), out dynamicProperties))
		{
			num++;
			num2++;
		}
		this.motors = new EntityVehicle.Motor[num];
		Transform meshTransform = this.vehicle.GetMeshTransform();
		for (int i = 0; i < this.motors.Length; i++)
		{
			EntityVehicle.Motor motor = new EntityVehicle.Motor();
			this.motors[i] = motor;
			DynamicProperties dynamicProperties2 = properties.Classes["motor" + i.ToString()];
			string @string = dynamicProperties2.GetString("engine");
			if (@string.Length > 0)
			{
				motor.engine = (this.vehicle.FindPart(@string) as VPEngine);
			}
			motor.engineOffPer = 0f;
			dynamicProperties2.ParseFloat("engineOffPer", ref motor.engineOffPer);
			motor.turbo = 1f;
			dynamicProperties2.ParseFloat("turbo", ref motor.turbo);
			motor.rpmAccelMin = 1f;
			motor.rpmAccelMax = 1f;
			dynamicProperties2.ParseVec("rpmAccel_min_max", ref motor.rpmAccelMin, ref motor.rpmAccelMax);
			motor.rpmDrag = 1f;
			dynamicProperties2.ParseFloat("rpmDrag", ref motor.rpmDrag);
			motor.rpmMax = 1f;
			dynamicProperties2.ParseFloat("rpmMax", ref motor.rpmMax);
			if (motor.rpmMax == 0f)
			{
				motor.rpmMax = 0.001f;
			}
			motor.trigger = EntityVehicle.Motor.Trigger.On;
			dynamicProperties2.ParseEnum<EntityVehicle.Motor.Trigger>("trigger", ref motor.trigger);
			string string2 = dynamicProperties2.GetString("transform");
			if (string2.Length > 0)
			{
				motor.transform = meshTransform.Find(string2);
			}
			float num3 = 0f;
			dynamicProperties2.ParseFloat("axis", ref num3);
			motor.axis = (int)num3;
		}
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x000FDF64 File Offset: 0x000FC164
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupWheels()
	{
		DynamicProperties properties = this.vehicle.Properties;
		if (properties == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		DynamicProperties dynamicProperties;
		while (num2 < 99 && properties.Classes.TryGetValue("wheel" + num2.ToString(), out dynamicProperties))
		{
			num++;
			num2++;
		}
		this.wheels = new EntityVehicle.Wheel[num];
		Transform physicsTransform = this.PhysicsTransform;
		Transform meshTransform = this.vehicle.GetMeshTransform();
		for (int i = 0; i < this.wheels.Length; i++)
		{
			EntityVehicle.Wheel wheel = new EntityVehicle.Wheel();
			this.wheels[i] = wheel;
			Transform transform = physicsTransform.Find("Wheel" + i.ToString());
			wheel.wheelC = transform.GetComponent<WheelCollider>();
			wheel.forwardFriction = wheel.wheelC.forwardFriction;
			wheel.forwardStiffnessBase = wheel.forwardFriction.stiffness;
			wheel.sideFriction = wheel.wheelC.sidewaysFriction;
			wheel.sideStiffnessBase = wheel.sideFriction.stiffness;
			DynamicProperties dynamicProperties2 = properties.Classes["wheel" + i.ToString()];
			wheel.motorTorqueScale = 1f;
			wheel.brakeTorqueScale = 1f;
			dynamicProperties2.ParseVec("torqueScale_motor_brake", ref wheel.motorTorqueScale, ref wheel.brakeTorqueScale);
			wheel.bounceSound = "vwheel_bounce";
			dynamicProperties2.ParseString("bounceSound", ref wheel.bounceSound);
			wheel.slideSound = "vwheel_slide";
			dynamicProperties2.ParseString("slideSound", ref wheel.slideSound);
			string @string = dynamicProperties2.GetString("steerTransform");
			if (@string.Length > 0)
			{
				wheel.steerT = meshTransform.Find(@string);
				if (wheel.steerT)
				{
					wheel.steerBaseRot = wheel.steerT.localRotation;
				}
			}
			string string2 = dynamicProperties2.GetString("tireTransform");
			if (string2.Length > 0)
			{
				wheel.tireT = meshTransform.Find(string2);
			}
			wheel.isSteerParentOfTire = (wheel.steerT != wheel.tireT);
			if (dynamicProperties2.GetString("tireSuspensionPercent").Length > 0)
			{
				wheel.tireSuspensionPercent = 1f;
			}
		}
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x000FE1A7 File Offset: 0x000FC3A7
	public override void OnXMLChanged()
	{
		this.vehicle.OnXMLChanged();
		this.SetupDevices();
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000FE1BA File Offset: 0x000FC3BA
	public new void FixedUpdate()
	{
		this.PhysicsFixedUpdate();
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000FE1C4 File Offset: 0x000FC3C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsResetAndSleep()
	{
		Rigidbody rigidbody = this.vehicleRB;
		Transform physicsTransform = this.PhysicsTransform;
		Vector3 position = this.position - Origin.position;
		physicsTransform.position = position;
		rigidbody.position = position;
		Quaternion rotation = this.ModelTransform.rotation;
		physicsTransform.rotation = rotation;
		rigidbody.rotation = rotation;
		if (!this.vehicleRB.isKinematic)
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.Sleep();
		}
		this.SetWheelsForces(0f, 1f, 0f, 1f);
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x000FE25C File Offset: 0x000FC45C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsFixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		Rigidbody rigidbody = this.vehicleRB;
		Transform physicsTransform = this.PhysicsTransform;
		rigidbody.AddForce(new Vector3(0f, -9.81f * rigidbody.mass, 0f), ForceMode.Force);
		this.wheelMotor = 0f;
		this.wheelBrakes = 0f;
		if (this.isEntityRemote)
		{
			this.vehicleRB.isKinematic = true;
			Vector3 position = Vector3.Lerp(physicsTransform.position, this.position - Origin.position, 0.5f);
			physicsTransform.position = position;
			physicsTransform.rotation = Quaternion.Slerp(physicsTransform.rotation, this.ModelTransform.rotation, 0.3f);
			if (this.incomingRemoteData.Flags > 0)
			{
				this.lastRemoteData = this.currentRemoteData;
				this.currentRemoteData = this.incomingRemoteData;
				this.incomingRemoteData.Flags = 0;
				this.syncPlayTime = 0f;
				this.vehicle.CurrentIsAccel = ((this.currentRemoteData.Flags & 2) > 0);
				this.vehicle.CurrentIsBreak = ((this.currentRemoteData.Flags & 4) > 0);
			}
			if (this.syncPlayTime >= 0f)
			{
				float num = this.syncPlayTime / 0.5f;
				this.syncPlayTime += deltaTime;
				if (num >= 1f)
				{
					num = 1f;
					this.syncPlayTime = -1f;
				}
				float num2 = Mathf.Lerp(this.lastRemoteData.SteeringPercent, this.currentRemoteData.SteeringPercent, num);
				this.vehicle.CurrentSteeringPercent = num2;
				float currentMotorTorquePercent = Mathf.Lerp(this.lastRemoteData.MotorTorquePercent, this.currentRemoteData.MotorTorquePercent, num);
				this.vehicle.CurrentMotorTorquePercent = currentMotorTorquePercent;
				Vector3 vector = Vector3.Lerp(this.lastRemoteData.Velocity, this.currentRemoteData.Velocity, num);
				this.vehicle.CurrentVelocity = vector;
				this.vehicle.CurrentForwardVelocity = Vector3.Dot(vector, physicsTransform.forward);
				this.wheelDir = num2 * this.vehicle.SteerAngleMax;
				this.FixedUpdateMotors();
				this.vehicle.UpdateSimulation();
				int num3 = this.wheels.Length;
				if (num3 > 0 && this.lastRemoteData.parts != null)
				{
					int num4 = 0;
					for (int i = 0; i < num3; i++)
					{
						EntityVehicle.Wheel wheel = this.wheels[i];
						Transform steerT = wheel.steerT;
						if (steerT && wheel.isSteerParentOfTire)
						{
							Quaternion localRotation = Quaternion.Lerp(this.lastRemoteData.parts[num4].rot, this.currentRemoteData.parts[num4].rot, num);
							steerT.localRotation = localRotation;
							num4++;
						}
						Transform tireT = wheel.tireT;
						if (tireT)
						{
							Vector3 localPosition = Vector3.Lerp(this.lastRemoteData.parts[num4].pos, this.currentRemoteData.parts[num4].pos, num);
							tireT.localPosition = localPosition;
							Quaternion localRotation2 = Quaternion.Lerp(this.lastRemoteData.parts[num4].rot, this.currentRemoteData.parts[num4].rot, num);
							tireT.localRotation = localRotation2;
							num4++;
						}
					}
				}
			}
			return;
		}
		this.CheckForOutOfWorld();
		if (!this.RBActive)
		{
			this.PhysicsResetAndSleep();
			this.vehicleRB.isKinematic = true;
			return;
		}
		this.vehicleRB.isKinematic = false;
		if (!this.hasDriver)
		{
			Vector3 vector2 = rigidbody.velocity;
			vector2.x *= 0.98f;
			vector2.z *= 0.98f;
			if (this.GetWheelsOnGround() > 0)
			{
				this.RBNoDriverGndTime += deltaTime;
				float f = this.RBNoDriverGndTime / 8f;
				float num5 = Utils.FastLerp(0.6f, 1f, (0.5f - physicsTransform.up.y) / 0.5f);
				num5 = Utils.FastLerp(1f, num5, Mathf.Pow(f, 3f));
				vector2.x *= num5;
				vector2.z *= num5;
			}
			if (this.collisionGrazeCount >= 2)
			{
				float num6 = vector2.magnitude * 1.4f;
				if (num6 < 1f)
				{
					float num7 = Utils.FastLerpUnclamped((float)Utils.FastMin(3, this.collisionGrazeCount) * 0.29f, 0f, num6);
					vector2 *= 1f - num7;
					rigidbody.angularVelocity *= 1f - num7 * 0.65f;
				}
			}
			vector2.y *= this.vehicle.AirDragVelScale;
			rigidbody.velocity = vector2;
			if (vector2.sqrMagnitude < 0.010000001f && rigidbody.angularVelocity.sqrMagnitude < 0.0049f)
			{
				this.RBNoDriverSleepTime += deltaTime;
				if (this.RBNoDriverSleepTime >= 3f)
				{
					this.RBActive = false;
					this.RBNoDriverSleepTime = 0f;
				}
			}
			else
			{
				this.RBNoDriverSleepTime = 0f;
			}
			this.collisionGrazeCount = 0;
		}
		Vector3 vector3 = this.vehicleRB.velocity;
		float num8 = this.vehicle.MotorTorqueForward;
		float num9 = this.vehicle.VelocityMaxForward;
		this.vehicle.IsTurbo = false;
		if (this.movementInput != null)
		{
			if (this.movementInput.moveForward < 0f)
			{
				num8 = this.vehicle.MotorTorqueBackward;
				num9 = this.vehicle.VelocityMaxBackward;
			}
			if (this.movementInput.running && this.vehicle.CanTurbo && this.movementInput.moveForward != 0f)
			{
				this.vehicle.IsTurbo = true;
				num8 = this.vehicle.MotorTorqueTurboForward;
				num9 = this.vehicle.VelocityMaxTurboForward;
				if (this.movementInput.moveForward < 0f)
				{
					num8 = this.vehicle.MotorTorqueTurboBackward;
					num9 = this.vehicle.VelocityMaxTurboBackward;
				}
			}
		}
		num8 *= this.vehicle.EffectMotorTorquePer;
		num9 *= this.vehicle.EffectVelocityMaxPer;
		float num10 = (num9 > this.velocityMax) ? 2.5f : 1.5f;
		num9 = Mathf.MoveTowards(this.velocityMax, num9, num10 * deltaTime);
		this.velocityMax = num9;
		if (this.CalcWaterDepth(this.vehicle.WaterDragY) > 0f)
		{
			this.timeInWater += deltaTime;
			if (this.vehicle.WaterDragVelScale != 1f)
			{
				vector3 *= this.vehicle.WaterDragVelScale;
			}
			if (this.vehicle.WaterDragVelMaxScale != 1f)
			{
				num9 = Mathf.Lerp(num9, num9 * this.vehicle.WaterDragVelMaxScale, this.timeInWater * 0.5f);
			}
		}
		else
		{
			this.timeInWater = 0f;
		}
		float num11 = Mathf.Sqrt(vector3.x * vector3.x + vector3.z * vector3.z);
		if (num11 > num9)
		{
			float num12 = num9 / num11;
			vector3.x *= num12;
			vector3.z *= num12;
			this.vehicleRB.velocity = vector3;
		}
		float magnitude = vector3.magnitude;
		if (this.vehicle.WaterLiftForce > 0f)
		{
			float num13 = this.CalcWaterDepth(this.vehicle.WaterLiftY);
			if (num13 > 0f)
			{
				float y = Mathf.Lerp(this.vehicle.WaterLiftForce * 0.05f, this.vehicle.WaterLiftForce, num13 / (this.vehicle.WaterLiftDepth + 0.001f));
				this.vehicleRB.AddForce(new Vector3(0f, y, 0f), ForceMode.VelocityChange);
			}
		}
		float num14 = -this.lastRBVel.y;
		if (num14 > 8f && (magnitude < num14 * 0.45f || Vector3.Dot(this.lastRBVel.normalized, vector3.normalized) < 0.2f))
		{
			int num15 = (int)((num14 - 8f) * 4f + 0.999f);
			this.ApplyDamage(num15 * 10);
			this.ApplyCollisionDamageToAttached(num15);
		}
		this.lastRBPos = this.vehicleRB.position;
		this.lastRBRot = this.vehicleRB.rotation;
		this.lastRBVel = vector3;
		this.lastRBAngVel = this.vehicleRB.angularVelocity;
		float num16 = Vector3.Dot(vector3, physicsTransform.forward);
		this.vehicle.CurrentForwardVelocity = num16;
		float frictionPercent = 1f;
		if (this.hasDriver && this.wheels.Length < 4 && base.GetAttachedPlayerLocal().isPlayerInStorm)
		{
			frictionPercent = 0.75f;
			float num17 = 0.04f;
			float y2 = 0.01f;
			this.vehicleRB.AddForce(new Vector3(num17 * 0.707f, y2, num17 * 0.707f), ForceMode.VelocityChange);
		}
		this.motorTorque = 0f;
		this.brakeTorque = 0f;
		if (this.wheels.Length != 0)
		{
			if (this.movementInput != null)
			{
				float num18 = Mathf.Pow(magnitude * 0.1f, 2f);
				float num19 = Mathf.Clamp(1f - num18, 0.15f, 1f);
				this.wheelMotor = this.movementInput.moveForward;
				float steerAngleMax = this.vehicle.SteerAngleMax;
				float num20 = this.vehicle.SteerRate * num19 * deltaTime;
				if (EntityVehicle.isTurnTowardsLook)
				{
					float num21 = 0f;
					if (!Input.GetMouseButton(1))
					{
						vp_FPCamera vp_FPCamera = base.GetAttachedPlayerLocal().vp_FPCamera;
						Vector3 forward = base.transform.forward;
						forward.y = 0f;
						Vector3 forward2 = vp_FPCamera.Forward;
						forward2.y = 0f;
						num21 = Vector3.SignedAngle(forward, forward2, Vector3.up);
						if (num16 < -0.02f)
						{
							if (Mathf.Abs(num21) > 90f)
							{
								num21 += 180f;
								if (num21 > 180f)
								{
									num21 -= 360f;
								}
							}
							num21 = -num21;
						}
					}
					float num22 = num20 * 1.2f;
					if ((this.wheelDir < 0f && this.wheelDir < num21) || (this.wheelDir > 0f && this.wheelDir > num21))
					{
						num22 *= 3f;
					}
					this.wheelDir = Mathf.MoveTowards(this.wheelDir, num21, num22);
					this.wheelDir = Mathf.Clamp(this.wheelDir, -steerAngleMax, steerAngleMax);
				}
				else if (this.movementInput.lastInputController)
				{
					this.wheelDir = Mathf.MoveTowards(this.wheelDir, this.movementInput.moveStrafe * steerAngleMax, num20 * 1.5f);
				}
				else
				{
					float moveStrafe = this.movementInput.moveStrafe;
					float num23 = 0f;
					if (moveStrafe < 0f)
					{
						if (this.wheelDir > 0f)
						{
							num23 -= num20 * num18;
						}
						num23 -= num20;
					}
					if (moveStrafe > 0f)
					{
						if (this.wheelDir < 0f)
						{
							num23 += num20 * num18;
						}
						num23 += num20;
					}
					this.wheelDir += num23;
					this.wheelDir = Mathf.Clamp(this.wheelDir, -steerAngleMax, steerAngleMax);
					if (moveStrafe == 0f)
					{
						this.wheelDir = Mathf.MoveTowards(this.wheelDir, 0f, this.vehicle.SteerCenteringRate * deltaTime);
					}
				}
				if (this.wheelMotor != 0f)
				{
					if (this.wheelMotor > 0f)
					{
						if (num16 < -0.5f)
						{
							this.wheelBrakes = 1f;
						}
					}
					else if (num16 > 0.5f)
					{
						this.wheelBrakes = 1f;
					}
					if (!this.movementInput.running)
					{
						this.wheelMotor *= 0.5f;
					}
				}
				if (this.movementInput.jump)
				{
					this.wheelBrakes = 2f;
				}
				if (this.canHop)
				{
					if (this.movementInput.down && this.GetWheelsOnGround() > 0)
					{
						this.canHop = false;
						Vector3 force = Vector3.Slerp(Vector3.up, physicsTransform.up, 0.5f) * this.vehicle.HopForce.x;
						this.vehicleRB.AddForceAtPosition(force, this.vehicleRB.position + physicsTransform.forward * this.vehicle.HopForce.y, ForceMode.VelocityChange);
					}
				}
				else if (!this.movementInput.down)
				{
					this.canHop = true;
				}
			}
			if (this.wheelMotor != 0f)
			{
				if (this.vehicle.HasEnginePart())
				{
					if (this.IsEngineRunning)
					{
						this.motorTorque = this.wheelMotor * num8;
					}
					else
					{
						this.motorTorque = this.wheelMotor * 50f;
					}
				}
				else if (this.vehicle.GetHealth() > 0)
				{
					this.motorTorque = this.wheelMotor * num8;
				}
				else
				{
					this.motorTorque = this.wheelMotor * 10f;
					if (this.rand.RandomFloat < 0.2f)
					{
						this.vehicleRB.AddRelativeForce(0.15f * this.rand.RandomOnUnitSphere, ForceMode.VelocityChange);
					}
					this.wheelDir = Mathf.Clamp(this.wheelDir + (this.rand.RandomFloat * 2f - 1f) * 5f, -this.vehicle.SteerAngleMax, this.vehicle.SteerAngleMax);
				}
				if (magnitude < 0.15f && this.wheelBrakes == 0f && Utils.FastAbs(physicsTransform.up.y) > 0.34f)
				{
					Vector3 force2 = Quaternion.Euler(0f, this.wheelDir, 0f) * (this.vehicle.UnstickForce * Mathf.Sign(this.wheelMotor) * Vector3.forward);
					this.vehicleRB.AddRelativeForce(force2, ForceMode.VelocityChange);
				}
			}
			this.brakeTorque = this.wheelBrakes * this.vehicle.BrakeTorque;
			this.SetWheelsForces(this.motorTorque, num8, this.brakeTorque, frictionPercent);
			this.UpdateWheelsCollision();
			this.UpdateWheelsSteering();
		}
		this.vehicleRB.velocity *= this.vehicle.AirDragVelScale;
		this.vehicleRB.angularVelocity *= this.vehicle.AirDragAngVelScale;
		this.PhysicsInputMove();
		this.FixedUpdateMotors();
		this.FixedUpdateForces();
		if (this.hasDriver || base.GetFirstAttached())
		{
			if (this.vehicle.TiltUpForce > 0f)
			{
				Vector3 right = physicsTransform.right;
				Mathf.Abs(right.y);
				float num24 = Mathf.Asin(right.y) * 57.29578f;
				float num25 = this.wheelDir / this.vehicle.SteerAngleMax;
				num25 *= 2f;
				num25 = Mathf.LerpUnclamped(0f, num25, Mathf.Pow(magnitude * 0.1f, 2f));
				float tiltAngleMax = this.vehicle.TiltAngleMax;
				num25 = Mathf.Clamp(num25 * tiltAngleMax, -tiltAngleMax, tiltAngleMax);
				float f2 = num24 + num25;
				float num26 = Mathf.Abs(f2);
				if (num26 > this.vehicle.TiltThreshold)
				{
					float num27 = (num26 - this.vehicle.TiltThreshold) * Mathf.Sign(f2) * 0.01f * -this.vehicle.TiltUpForce;
					num27 = Mathf.Clamp(num27, -4f, 4f);
					this.vehicleRB.AddRelativeTorque(0f, 0f, num27, ForceMode.VelocityChange);
				}
				if (num26 < this.vehicle.TiltDampenThreshold)
				{
					Vector3 angularVelocity = this.vehicleRB.angularVelocity;
					float magnitude2 = angularVelocity.magnitude;
					if (magnitude2 > 0f)
					{
						Vector3 rhs = angularVelocity * (1f / magnitude2);
						float num28 = Mathf.Abs(Vector3.Dot(base.transform.forward, rhs));
						this.vehicleRB.angularVelocity -= angularVelocity * (0.02f + this.vehicle.TiltDampening * num28);
					}
				}
			}
			if (this.vehicle.UpForce > 0f)
			{
				Vector3 up = physicsTransform.up;
				float num29 = Mathf.Abs(Mathf.Acos(up.y) * 57.29578f) - this.vehicle.UpAngleMax;
				if (num29 > 0f)
				{
					float num30 = num29 / 90f;
					Vector3 torque = Vector3.Cross(up, Vector3.up) * (num30 * num30 * this.vehicle.UpForce);
					this.vehicleRB.AddRelativeTorque(torque, ForceMode.VelocityChange);
				}
			}
		}
		Vector3 position2 = physicsTransform.position;
		this.SetPosition(position2 + Origin.position, false);
		this.qrotation = physicsTransform.rotation;
		this.rotation = this.qrotation.eulerAngles;
		this.ModelTransform.rotation = this.qrotation;
		this.vehicle.CurrentIsAccel = (this.motorTorque != 0f && this.brakeTorque == 0f);
		this.vehicle.CurrentIsBreak = (this.brakeTorque != 0f);
		this.vehicle.CurrentSteeringPercent = this.wheelDir / this.vehicle.SteerAngleMax;
		this.vehicle.CurrentVelocity = this.vehicleRB.velocity;
		this.vehicle.UpdateSimulation();
		if (!this.isEntityRemote)
		{
			this.syncHighRateTime += deltaTime;
			if (this.syncHighRateTime >= 0.5f)
			{
				this.SendSyncData(32768);
				this.syncHighRateTime = 0f;
			}
			this.syncLowRateTime += deltaTime;
			if (this.syncLowRateTime >= 2f)
			{
				this.SendSyncData(16384);
				this.syncLowRateTime = 0f;
			}
		}
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void PhysicsInputMove()
	{
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x000FF478 File Offset: 0x000FD678
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdateForces()
	{
		if (this.movementInput == null)
		{
			return;
		}
		float num = 1f;
		for (int i = 0; i < this.forces.Length; i++)
		{
			EntityVehicle.Force force = this.forces[i];
			float num2 = 1f;
			switch (force.trigger)
			{
			case EntityVehicle.Force.Trigger.Off:
				num2 = 0f;
				break;
			case EntityVehicle.Force.Trigger.InputForward:
				num2 = this.movementInput.moveForward;
				break;
			case EntityVehicle.Force.Trigger.InputStrafe:
				num2 = this.movementInput.moveStrafe;
				break;
			case EntityVehicle.Force.Trigger.InputUp:
				num2 = (float)(this.movementInput.jump ? 1 : 0);
				break;
			case EntityVehicle.Force.Trigger.InputDown:
				num2 = (float)(this.movementInput.down ? 1 : 0);
				break;
			case EntityVehicle.Force.Trigger.Motor0:
			case EntityVehicle.Force.Trigger.Motor1:
			case EntityVehicle.Force.Trigger.Motor2:
			case EntityVehicle.Force.Trigger.Motor3:
			case EntityVehicle.Force.Trigger.Motor4:
			case EntityVehicle.Force.Trigger.Motor5:
			case EntityVehicle.Force.Trigger.Motor6:
			case EntityVehicle.Force.Trigger.Motor7:
			{
				EntityVehicle.Motor motor = this.motors[force.trigger - EntityVehicle.Force.Trigger.Motor0];
				num2 = motor.rpm / motor.rpmMax;
				break;
			}
			}
			if (num2 != 0f)
			{
				num2 *= num;
				float num3 = this.position.y - force.ceiling.x;
				if (num3 > 0f)
				{
					num2 *= Utils.FastMax(0f, 1f - num3 * force.ceiling.y);
				}
				EntityVehicle.Force.Type type = force.type;
				if (type != EntityVehicle.Force.Type.Relative)
				{
					if (type == EntityVehicle.Force.Type.RelativeTorque)
					{
						this.vehicleRB.AddRelativeTorque(force.force * num2, ForceMode.VelocityChange);
					}
				}
				else
				{
					this.vehicleRB.AddRelativeForce(force.force * num2, ForceMode.VelocityChange);
				}
			}
		}
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x000FF60C File Offset: 0x000FD80C
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdateMotors()
	{
		for (int i = 0; i < this.motors.Length; i++)
		{
			EntityVehicle.Motor motor = this.motors[i];
			motor.rpm *= motor.rpmDrag;
			float num = 0f;
			switch (motor.trigger)
			{
			case EntityVehicle.Motor.Trigger.On:
				num = 1f;
				break;
			case EntityVehicle.Motor.Trigger.InputForward:
				if (this.movementInput != null)
				{
					num = this.movementInput.moveForward;
				}
				break;
			case EntityVehicle.Motor.Trigger.InputStrafe:
				if (this.movementInput != null)
				{
					num = this.movementInput.moveStrafe;
				}
				break;
			case EntityVehicle.Motor.Trigger.InputUp:
				if (this.movementInput != null && this.movementInput.jump)
				{
					num = 1f;
				}
				break;
			case EntityVehicle.Motor.Trigger.InputDown:
				if (this.movementInput != null && this.movementInput.down)
				{
					num = 1f;
				}
				break;
			case EntityVehicle.Motor.Trigger.Vel:
				num = this.vehicle.CurrentForwardVelocity / (this.vehicle.VelocityMaxForward + 0.001f);
				if (num < 0.01f)
				{
					num = 0f;
				}
				break;
			}
			if (num != 0f)
			{
				float num2 = 1f;
				if (this.movementInput != null && this.movementInput.running)
				{
					num2 = motor.turbo;
				}
				if (motor.engine != null && !motor.engine.isRunning)
				{
					num *= motor.engineOffPer;
					num2 = 1f;
				}
				num *= num2;
				switch (motor.type)
				{
				case EntityVehicle.Motor.Type.Spin:
					if (this.hasDriver)
					{
						float num3 = Mathf.Lerp(motor.rpmAccelMin, motor.rpmAccelMax, num);
						motor.rpm += num3;
						motor.rpm = Mathf.Min(motor.rpm, motor.rpmMax * num2);
					}
					break;
				}
			}
		}
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x000FF7E0 File Offset: 0x000FD9E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateMotors()
	{
		for (int i = 0; i < this.motors.Length; i++)
		{
			EntityVehicle.Motor motor = this.motors[i];
			Transform transform = motor.transform;
			if (transform)
			{
				Vector3 localEulerAngles = transform.localEulerAngles;
				ref Vector3 ptr = ref localEulerAngles;
				int axis = motor.axis;
				ptr[axis] += motor.rpm * 360f * Time.deltaTime;
				transform.localEulerAngles = localEulerAngles;
			}
		}
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x000FF858 File Offset: 0x000FDA58
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetWheelsOnGround()
	{
		int num = 0;
		int num2 = this.wheels.Length;
		for (int i = 0; i < num2; i++)
		{
			if (this.wheels[i].isGrounded)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x000FF890 File Offset: 0x000FDA90
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetWheelsForces(float motorTorque, float motorTorqueBase, float brakeTorque, float _frictionPercent)
	{
		this.vehicle.CurrentMotorTorquePercent = motorTorque / motorTorqueBase;
		float num = (_frictionPercent == 1f) ? 1f : (_frictionPercent * 0.33f);
		int num2 = this.wheels.Length;
		for (int i = 0; i < num2; i++)
		{
			EntityVehicle.Wheel wheel = this.wheels[i];
			wheel.wheelC.motorTorque = motorTorque * wheel.motorTorqueScale;
			wheel.wheelC.brakeTorque = brakeTorque * wheel.brakeTorqueScale;
			wheel.forwardFriction.stiffness = wheel.forwardStiffnessBase * _frictionPercent;
			wheel.wheelC.forwardFriction = wheel.forwardFriction;
			wheel.sideFriction.stiffness = wheel.sideStiffnessBase * num;
			wheel.wheelC.sidewaysFriction = wheel.sideFriction;
		}
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000FF954 File Offset: 0x000FDB54
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateWheelsCollision()
	{
		float wheelPtlScale = this.vehicle.WheelPtlScale;
		for (int i = 0; i < this.wheels.Length; i++)
		{
			EntityVehicle.Wheel wheel = this.wheels[i];
			wheel.isGrounded = false;
			WheelHit wheelHit;
			if (wheel.wheelC.GetGroundHit(out wheelHit))
			{
				float mass = wheel.wheelC.mass;
				if (wheelHit.normal.y >= 0f)
				{
					wheel.isGrounded = true;
				}
				if (wheelHit.force > 260f * mass)
				{
					this.PlayOneShot(wheel.bounceSound, false, false, false, null, 1f);
				}
				float forwardSlip = wheelHit.forwardSlip;
				if (forwardSlip <= -0.9f || forwardSlip >= 0.995f)
				{
					wheel.slideTime += Time.deltaTime;
				}
				else if (Utils.FastAbs(wheelHit.sidewaysSlip) >= 0.19f)
				{
					wheel.slideTime += Time.deltaTime;
				}
				else
				{
					wheel.slideTime = 0f;
				}
				if (wheel.slideTime > 0.2f)
				{
					wheel.slideTime = 0f;
					this.PlayOneShot(wheel.slideSound, false, false, false, null, 1f);
				}
				if (wheelPtlScale > 0f && Utils.FastAbs(forwardSlip) >= 0.5f)
				{
					wheel.ptlTime += Time.deltaTime;
					if (wheel.ptlTime > 0.05f)
					{
						wheel.ptlTime = 0f;
						float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(wheelHit.point)) * 0.5f;
						ParticleEffect pe = new ParticleEffect("tiresmoke", Vector3.zero, lightValue, new Color(1f, 1f, 1f, 1f), null, wheel.wheelC.transform, false, 1f, "");
						Transform transform = GameManager.Instance.SpawnParticleEffectClientForceCreation(pe, -1, false);
						if (transform)
						{
							transform.position = wheelHit.point;
							transform.localScale = new Vector3(wheelPtlScale, wheelPtlScale, wheelPtlScale);
						}
					}
				}
			}
		}
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000FFB68 File Offset: 0x000FDD68
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateWheelsSteering()
	{
		this.wheels[0].wheelC.steerAngle = this.wheelDir;
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000FFB82 File Offset: 0x000FDD82
	public Vector3 GetRBVelocity()
	{
		return this.lastRBVel;
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000FFB8C File Offset: 0x000FDD8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && base.GetFirstAttached())
		{
			this.world.entityDistributer.SendFullUpdateNextTick(this);
		}
		if (this.vehicleRB && this.RBActive)
		{
			Quaternion rhs = Quaternion.Euler(0f, this.wheelDir, 0f);
			for (int i = 0; i < this.wheels.Length; i++)
			{
				EntityVehicle.Wheel wheel = this.wheels[i];
				wheel.tireSpinSpeed = Utils.FastLerpUnclamped(wheel.tireSpinSpeed, wheel.wheelC.rotationSpeed, 0.3f);
				wheel.tireSpin += Utils.FastClamp(wheel.tireSpinSpeed * Time.deltaTime, -13f, 13f);
				Vector3 vector;
				Quaternion quaternion;
				wheel.wheelC.GetWorldPose(out vector, out quaternion);
				if (wheel.steerT)
				{
					quaternion = Quaternion.Euler(wheel.tireSpin, 0f, 0f);
					Quaternion quaternion2 = wheel.steerBaseRot * rhs;
					if (!wheel.isSteerParentOfTire)
					{
						quaternion2 *= quaternion;
					}
					wheel.steerT.localRotation = quaternion2;
				}
				if (wheel.tireT)
				{
					if (wheel.tireSuspensionPercent > 0f)
					{
						vector = wheel.tireT.parent.InverseTransformPoint(vector);
						Vector3 localPosition = wheel.tireT.localPosition;
						localPosition.y = vector.y;
						wheel.tireT.localPosition = localPosition;
					}
					if (wheel.steerT)
					{
						if (wheel.isSteerParentOfTire)
						{
							wheel.tireT.localRotation = quaternion;
						}
					}
					else
					{
						wheel.tireT.localRotation = Quaternion.Euler(wheel.tireSpin, 0f, 0f);
					}
				}
			}
		}
		this.UpdateAttachment();
		if (this.RBActive || this.syncPlayTime >= 0f)
		{
			this.UpdateMotors();
		}
		this.vehicle.Update(Time.deltaTime);
		if ((Time.frameCount & 1) == 0)
		{
			this.hitEffectCount = 1;
		}
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000FFDB4 File Offset: 0x000FDFB4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		if (this.isEntityRemote)
		{
			float t = Time.deltaTime * 10f;
			Transform modelTransform = this.ModelTransform;
			Vector3 position = Vector3.Lerp(modelTransform.position, this.position - Origin.position, t);
			Quaternion rotation = Quaternion.Slerp(modelTransform.rotation, this.qrotation, t);
			modelTransform.SetPositionAndRotation(position, rotation);
		}
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000FFE14 File Offset: 0x000FE014
	public void CameraChangeRotation(float _newRotation)
	{
		if (EntityVehicle.isTurnTowardsLook)
		{
			EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
			if (attachedPlayerLocal)
			{
				attachedPlayerLocal.vp_FPCamera.Yaw += _newRotation;
			}
		}
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000FFE4C File Offset: 0x000FE04C
	public override void OriginChanged(Vector3 _deltaPos)
	{
		base.OriginChanged(_deltaPos);
		Vector3 position = this.position - Origin.position;
		this.ModelTransform.position = position;
		this.PhysicsTransform.position = position;
		if (this.vehicleRB)
		{
			this.vehicleRB.position = position;
		}
		this.cameraPos += _deltaPos;
		this.cameraStartPos += _deltaPos;
		EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
		if (attachedPlayerLocal)
		{
			attachedPlayerLocal.vp_FPCamera.DrivingPosition += _deltaPos;
		}
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x000FFEEC File Offset: 0x000FE0EC
	public override void SetPosition(Vector3 _pos, bool _bUpdatePhysics = true)
	{
		base.SetPosition(_pos, _bUpdatePhysics);
		if (!this.isEntityRemote)
		{
			this.ModelTransform.position = _pos - Origin.position;
		}
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x000FFF14 File Offset: 0x000FE114
	public override void SetRotation(Vector3 _rot)
	{
		base.SetRotation(_rot);
		if (!this.isEntityRemote)
		{
			this.ModelTransform.rotation = this.qrotation;
		}
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000FFF36 File Offset: 0x000FE136
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetCenterPosition()
	{
		return this.position + this.ModelTransform.up * 0.8f;
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsQRotationUsed()
	{
		return true;
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	public override float GetHeight()
	{
		return 1f;
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x000FFF58 File Offset: 0x000FE158
	public void AddRelativeForce(Vector3 forceVec, ForceMode mode = ForceMode.VelocityChange)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.RBActive)
		{
			this.RBActive = true;
			this.vehicleRB.isKinematic = false;
		}
		this.vehicleRB.AddRelativeForce(forceVec, mode);
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x000FFF8B File Offset: 0x000FE18B
	public void AddForce(Vector3 forceVec, ForceMode mode = ForceMode.VelocityChange)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.RBActive)
		{
			this.RBActive = true;
			this.vehicleRB.isKinematic = false;
		}
		this.vehicleRB.AddForce(forceVec, mode);
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x000FFFBE File Offset: 0x000FE1BE
	public override Vector3 GetVelocityPerSecond()
	{
		if (this.isEntityRemote)
		{
			return this.vehicle.CurrentVelocity;
		}
		return this.vehicleRB.velocity;
	}

	// Token: 0x060028F0 RID: 10480 RVA: 0x000FFFE0 File Offset: 0x000FE1E0
	public void VelocityFlip()
	{
		if (this.isEntityRemote)
		{
			this.vehicle.CurrentVelocity = new Vector3(this.vehicle.CurrentVelocity.x * -1f, this.vehicle.CurrentVelocity.y, this.vehicle.CurrentVelocity.z * -1f);
			return;
		}
		this.vehicleRB.velocity = new Vector3(this.vehicleRB.velocity.x * -1f, this.vehicleRB.velocity.y, this.vehicleRB.velocity.z * -1f);
	}

	// Token: 0x060028F1 RID: 10481 RVA: 0x00100090 File Offset: 0x000FE290
	public Vector3 GetCameraOffset(float deltaTime)
	{
		EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
		Vector3 zero = Vector3.zero;
		Vector3 vector;
		if (!this.isEntityRemote)
		{
			vector = this.PhysicsTransform.position + Origin.position;
			this.SetPosition(vector, false);
			vector -= Origin.position;
			this.qrotation = this.PhysicsTransform.rotation;
			this.rotation = this.qrotation.eulerAngles;
			this.ModelTransform.rotation = this.qrotation;
		}
		else
		{
			vector = this.ModelTransform.position;
		}
		if (attachedPlayerLocal)
		{
			vp_FPCamera vp_FPCamera = attachedPlayerLocal.vp_FPCamera;
			if (!EntityVehicle.isTurnTowardsLook)
			{
				this.cameraAngleTarget = Vector2.SignedAngle(this.cameraStartVec, new Vector2(base.transform.forward.x, base.transform.forward.z));
				float num = this.cameraAngle;
				float num2 = Mathf.Abs(Mathf.DeltaAngle(this.cameraAngle, this.cameraAngleTarget));
				this.cameraAngle = Mathf.MoveTowardsAngle(this.cameraAngle, this.cameraAngleTarget, num2 * 0.3f);
				num -= this.cameraAngle;
				vp_FPCamera.yaw3P += num;
			}
			float magnitude = this.vehicleRB.velocity.magnitude;
			float num3 = Mathf.Lerp(this.vehicle.CameraDistance.x, this.vehicle.CameraDistance.y, magnitude / this.vehicle.VelocityMaxForward) * EntityVehicle.cameraDistScale - this.cameraDist;
			if (num3 < 0f)
			{
				this.cameraOutTime += deltaTime;
				if (this.cameraOutTime > 1f)
				{
					num3 *= 0.03f;
					this.cameraDist += num3;
				}
			}
			else if (num3 > 0f)
			{
				this.cameraOutTime = 0f;
				num3 *= 0.22f;
				this.cameraDist += num3;
			}
			zero = new Vector3(0f, 0f, Mathf.Abs(this.cameraDist));
			vector.y += 1.8f;
			this.cameraPos.x = vector.x;
			this.cameraPos.z = vector.z;
			this.cameraPos.y = vector.y;
			if (this.cameraStartBlend < 1f)
			{
				this.cameraStartBlend = Mathf.Min(this.cameraStartBlend + deltaTime, 1f);
			}
			vp_FPCamera.DrivingPosition = Vector3.Lerp(attachedPlayerLocal.vp_FPController.SmoothPosition, this.cameraPos, this.cameraStartBlend);
		}
		return zero;
	}

	// Token: 0x060028F2 RID: 10482 RVA: 0x00100334 File Offset: 0x000FE534
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnterVehicle(EntityAlive _entity)
	{
		EntityPlayerLocal entityPlayerLocal = _entity as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			entityPlayerLocal.MountEvent.Invoke(true);
		}
		int slot = -1;
		_entity.StartAttachToEntity(this, slot);
		if (this.NavObject != null)
		{
			this.NavObject.IsActive = !(_entity is EntityPlayerLocal);
		}
		if (entityPlayerLocal != null)
		{
			entityPlayerLocal.Waypoints.UpdateEntityVehicleWayPoint(this, false);
		}
	}

	// Token: 0x060028F3 RID: 10483 RVA: 0x0010039C File Offset: 0x000FE59C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetVehicleDriven()
	{
		if (base.AttachedMainEntity != null && !base.AttachedMainEntity.isEntityRemote)
		{
			Utils.SetLayerRecursively(this.vehicleRB.gameObject, 21);
			this.RBActive = true;
			this.vehicleRB.isKinematic = false;
			this.vehicleRB.WakeUp();
			if (this.world.IsRemote())
			{
				this.vehicleRB.velocity = this.vehicle.CurrentVelocity;
			}
			this.lastRBVel = Vector3.zero;
			EntityPlayerLocal entityPlayerLocal = base.AttachedMainEntity as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.Waypoints.SetWaypointHiddenOnMap(this.entityId, true);
				return;
			}
		}
		else
		{
			Utils.SetLayerRecursively(this.vehicleRB.gameObject, 21);
			if (this.isEntityRemote)
			{
				this.RBActive = false;
			}
		}
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x0010046C File Offset: 0x000FE66C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateAttachment()
	{
		Entity attachedMainEntity = base.AttachedMainEntity;
		if (this.hasDriver && attachedMainEntity == null)
		{
			this.DriverRemoved();
		}
		if (attachedMainEntity != null && attachedMainEntity.IsDead())
		{
			((EntityAlive)attachedMainEntity).RemoveIKTargets();
			attachedMainEntity.Detach();
			this.DriverRemoved();
		}
		for (int i = this.delayedAttachments.Count - 1; i >= 0; i--)
		{
			EntityVehicle.DelayedAttach delayedAttach = this.delayedAttachments[i];
			Entity entity = GameManager.Instance.World.GetEntity(delayedAttach.entityId);
			if (entity)
			{
				if (!base.IsAttached(entity))
				{
					entity.AttachToEntity(this, delayedAttach.slot);
				}
				this.delayedAttachments.RemoveAt(i);
			}
		}
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x00100528 File Offset: 0x000FE728
	[PublicizedFrom(EAccessModifier.Private)]
	public void DriverRemoved()
	{
		EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
		if (attachedPlayerLocal != null)
		{
			attachedPlayerLocal.MountEvent.Invoke(false);
		}
		this.hasDriver = false;
		this.vehicle.SetColors();
		this.vehicle.FireEvent(Vehicle.Event.Stop);
		this.isInteractionLocked = false;
		this.RBNoDriverGndTime = 0f;
		this.RBNoDriverSleepTime = 0f;
		this.collisionGrazeCount = 0;
		if (this.GetWheelsOnGround() > 0 && !this.vehicleRB.isKinematic)
		{
			this.vehicleRB.velocity *= 0.5f;
		}
		if (this.NavObject != null)
		{
			this.NavObject.IsActive = true;
		}
		this.SendSyncData(49152);
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x001005E4 File Offset: 0x000FE7E4
	public override int AttachEntityToSelf(Entity _entity, int slot = -1)
	{
		slot = base.AttachEntityToSelf(_entity, slot);
		if (slot >= 0)
		{
			EntityAlive entityAlive = (EntityAlive)_entity;
			int seatPose = this.vehicle.GetSeatPose(slot);
			entityAlive.SetVehiclePoseMode(seatPose);
			entityAlive.transform.gameObject.layer = 24;
			entityAlive.m_characterController.Enable(false);
			entityAlive.SetIKTargets(this.vehicle.GetIKTargets(slot));
			this.isInteractionLocked = (base.GetAttachFreeCount() == 0);
			if (this.nativeCollider)
			{
				this.nativeCollider.enabled = !this.isInteractionLocked;
			}
			if (slot == 0)
			{
				this.hasDriver = true;
				this.vehicle.SetColors();
				this.vehicle.FireEvent(Vehicle.Event.Start);
			}
			this.SetVehicleDriven();
			this.vehicle.TriggerUpdateEffects();
			if (!_entity.isEntityRemote && GameManager.Instance.World != null)
			{
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entity as EntityPlayerLocal);
				if (uiforPlayer != null && uiforPlayer.playerInput != null)
				{
					PlayerActionsVehicle vehicleActions = uiforPlayer.playerInput.VehicleActions;
					uiforPlayer.ActionSetManager.Insert(vehicleActions, 1, null);
					this.movementInput = new MovementInput();
					this.CameraInit();
				}
			}
		}
		return slot;
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x0010070C File Offset: 0x000FE90C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void DetachEntity(Entity _entity)
	{
		for (int i = this.delayedAttachments.Count - 1; i >= 0; i--)
		{
			if (this.delayedAttachments[i].entityId == _entity.entityId)
			{
				this.delayedAttachments.RemoveAt(i);
			}
		}
		int num = base.FindAttachSlot(_entity);
		if (num < 0)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)_entity;
		entityAlive.SetVehiclePoseMode(-1);
		entityAlive.RemoveIKTargets();
		int modelLayer = entityAlive.GetModelLayer();
		entityAlive.SetModelLayer(modelLayer, true, null);
		if (entityAlive is EntityPlayerLocal)
		{
			entityAlive.transform.gameObject.layer = 20;
		}
		else if (ConsoleCmdCCPhysics.EnableCCPhysicsChanges && entityAlive is EntityPlayer)
		{
			entityAlive.transform.gameObject.layer = 3;
		}
		entityAlive.ModelTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		entityAlive.m_characterController.Enable(true);
		if (!_entity.isEntityRemote && GameManager.Instance.World != null)
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entity as EntityPlayerLocal);
			if (uiforPlayer != null)
			{
				PlayerActionsVehicle vehicleActions = uiforPlayer.playerInput.VehicleActions;
				uiforPlayer.ActionSetManager.Remove(vehicleActions, 1, null);
			}
			this.movementInput = null;
		}
		if (num == 0)
		{
			this.DriverRemoved();
		}
		bool isEntityRemote = this.isEntityRemote;
		base.DetachEntity(_entity);
		this.isInteractionLocked = (base.GetAttachFreeCount() == 0);
		if (this.nativeCollider)
		{
			this.nativeCollider.enabled = !this.isInteractionLocked;
		}
		this.SetVehicleDriven();
		this.vehicle.TriggerUpdateEffects();
		if (isEntityRemote && !this.isEntityRemote)
		{
			this.RBActive = true;
			this.RBNoDriverSleepTime = 0f;
			this.vehicleRB.isKinematic = false;
			this.vehicleRB.velocity = this.vehicle.CurrentVelocity;
		}
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x001008C9 File Offset: 0x000FEAC9
	public override int AttachToEntity(Entity _entity, int slot = -1)
	{
		return -1;
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x001008CC File Offset: 0x000FEACC
	public override AttachedToEntitySlotInfo GetAttachedToInfo(int _slotIdx)
	{
		AttachedToEntitySlotInfo attachedToEntitySlotInfo = new AttachedToEntitySlotInfo();
		attachedToEntitySlotInfo.bKeep3rdPersonModelVisible = true;
		attachedToEntitySlotInfo.bReplaceLocalInventory = true;
		attachedToEntitySlotInfo.pitchRestriction = new Vector2(-30f, 30f);
		attachedToEntitySlotInfo.yawRestriction = new Vector2(-90f, 90f);
		attachedToEntitySlotInfo.enterParentTransform = base.transform;
		attachedToEntitySlotInfo.enterPosition = new Vector3(0f, 0f, -0.201f);
		attachedToEntitySlotInfo.enterRotation = Vector3.zero;
		DynamicProperties propertiesForClass = this.vehicle.GetPropertiesForClass("seat" + _slotIdx.ToString());
		if (propertiesForClass != null)
		{
			propertiesForClass.ParseVec("position", ref attachedToEntitySlotInfo.enterPosition);
			propertiesForClass.ParseVec("rotation", ref attachedToEntitySlotInfo.enterRotation);
			string @string = propertiesForClass.GetString("exit");
			if (@string.Length > 0)
			{
				char[] separator = new char[]
				{
					'~'
				};
				string[] array = @string.Split(separator);
				for (int i = 0; i < array.Length; i++)
				{
					Vector3 vector = StringParsers.ParseVector3(array[i], 0, -1);
					vector.y += 0.02f;
					AttachedToEntitySlotExit item;
					item.position = base.GetPosition() + base.transform.TransformDirection(vector);
					float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
					item.rotation = new Vector3(0f, num + 180f + this.rotation.y, 0f);
					attachedToEntitySlotInfo.exits.Add(item);
				}
			}
		}
		else
		{
			AttachedToEntitySlotExit item2 = default(AttachedToEntitySlotExit);
			item2.position = base.GetPosition() + -2f * base.transform.right;
			item2.rotation = new Vector3(0f, this.rotation.y + 90f, 0f);
			attachedToEntitySlotInfo.exits.Add(item2);
		}
		return attachedToEntitySlotInfo;
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x00100AC8 File Offset: 0x000FECC8
	public Vector3 GetExitVelocity()
	{
		Vector3 a = this.GetVelocityPerSecond();
		if (this.GetWheelsOnGround() > 0)
		{
			a *= 0.5f;
		}
		return a * 0.7f;
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x00100B00 File Offset: 0x000FED00
	public void CameraInit()
	{
		Transform transform = base.transform;
		Vector3 forward = transform.forward;
		this.cameraStartVec.x = forward.x;
		this.cameraStartVec.y = forward.z;
		this.cameraPos = transform.position;
		this.cameraPos.y = this.cameraPos.y + 1.8f;
		EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
		if (attachedPlayerLocal)
		{
			vp_FPCamera vp_FPCamera = attachedPlayerLocal.vp_FPCamera;
			this.cameraStartPos = vp_FPCamera.transform.position;
			this.cameraStartBlend = 0.5f;
			vp_FPCamera.m_Current3rdPersonBlend = 1f;
			this.cameraDist = Mathf.Min(3f, (this.cameraPos - this.cameraStartPos).magnitude);
			this.cameraPos.y = attachedPlayerLocal.vp_FPCamera.transform.position.y;
			vp_FPCamera.Position3rdPersonOffset = new Vector3(0f, 1.8f, this.cameraDist);
			vp_FPCamera.DrivingPosition = attachedPlayerLocal.vp_FPController.SmoothPosition;
		}
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x00100C14 File Offset: 0x000FEE14
	public override void OnCollisionForward(Transform t, Collision collision, bool isStay)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.RBActive)
		{
			if (this.vehicleRB.velocity.magnitude > 0.01f && this.vehicleRB.angularVelocity.magnitude > 0.05f)
			{
				this.RBActive = true;
			}
			if (this.vehicleRB.isKinematic && (!collision.rigidbody || collision.rigidbody.velocity.magnitude > 0.05f))
			{
				this.RBActive = true;
			}
		}
		Entity entity = null;
		int layer = collision.gameObject.layer;
		if (layer != 16)
		{
			ColliderHitCallForward component = collision.gameObject.GetComponent<ColliderHitCallForward>();
			if (component)
			{
				entity = component.Entity;
			}
			if (!entity)
			{
				entity = this.FindEntity(collision.transform.parent);
			}
			if (!entity)
			{
				Rigidbody rigidbody = collision.rigidbody;
				if (rigidbody)
				{
					entity = this.FindEntity(rigidbody.transform);
				}
			}
		}
		if (entity && entity.IsSpawned())
		{
			if (collision.impulse.sqrMagnitude > 4f)
			{
				Vector3 vector = -collision.relativeVelocity;
				if (layer != 19)
				{
					vector *= 0.4f;
				}
				float num = vector.magnitude + 0.0001f;
				Vector3 vector2 = vector * (1f / num);
				EnumBodyPartHit enumBodyPartHit = EnumBodyPartHit.Torso;
				bool flag = false;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				int contactCount = collision.contactCount;
				for (int i = 0; i < contactCount; i++)
				{
					ContactPoint contact = collision.GetContact(i);
					vector3 += contact.point;
					vector4 += contact.normal;
					flag |= contact.thisCollider.CompareTag("E_VehicleStrong");
					string tag = contact.otherCollider.tag;
					enumBodyPartHit |= DamageSource.TagToBodyPart(tag);
				}
				vector3 *= 1f / (float)contactCount;
				vector3 += Origin.position;
				vector4 = Vector3.Normalize(vector4);
				float num2 = -Vector3.Dot(vector2, vector4);
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (num > 1f)
				{
					float num3 = Vector3.Dot(entity.motion.normalized, vector2);
					if (num3 > 0.2f)
					{
						float num4 = entity.motion.magnitude * 20f;
						num -= num4 * num3;
					}
				}
				float num5 = num * num2;
				float num6 = this.vehicleRB.mass * 0.2f;
				num6 += 20f;
				float massKg = EntityClass.list[entity.entityClass].MassKg;
				float num7 = num6 / massKg;
				float num8 = Utils.FastClamp(num7, 0.25f, 1.6f);
				float num9 = num5 * num8;
				float num10 = Utils.FastClamp(num7, 1f, 1.5f);
				float num11 = num5 / num10;
				if (massKg < 2f)
				{
					num2 = 0f;
					num9 = 0f;
					num11 = 0f;
				}
				EntityPlayer entityPlayer = entity as EntityPlayer;
				if (entityPlayer && (float)entityPlayer.SpawnedTicks <= 80f)
				{
					num9 = 0f;
					num11 = 0f;
				}
				bool flag2 = this.world.IsWorldEvent(World.WorldEvent.BloodMoon);
				bool flag3 = num7 >= 2f && !flag2 && (this.lastRBVel.sqrMagnitude > 10.240001f || num9 > 2.1f);
				vector *= num6 * 0.008f;
				vector.y = Utils.FastMin(50f, vector.y + vector.magnitude * 3f);
				if (num9 > 2.1f)
				{
					int entityId = this.entityId;
					Entity firstAttached = base.GetFirstAttached();
					if (firstAttached)
					{
						entityId = firstAttached.entityId;
					}
					DamageSourceEntity damageSourceEntity = new DamageSourceEntity(EnumDamageSource.External, EnumDamageTypes.Crushing, entityId, vector);
					damageSourceEntity.bodyParts = enumBodyPartHit;
					damageSourceEntity.DismemberChance = 1.2f;
					damageSourceEntity.bIgnorePartyShare = true;
					damageSourceEntity.KillXPScale = 0.5f;
					damageSourceEntity.AttackingItem = this.vehicle.itemValue;
					float num12 = 1f + (num9 - 2.1f) * 12f;
					if (entityPlayer)
					{
						num12 = Utils.FastMin(num12, 10f);
					}
					if (flag)
					{
						num12 *= this.vehicle.EffectEntityDamagePer;
					}
					bool flag4 = entity.IsAlive();
					entity.DamageEntity(damageSourceEntity, (int)num12, false, 1f);
					if ((entity.entityFlags & (EntityFlags.Player | EntityFlags.Zombie | EntityFlags.Animal | EntityFlags.Bandit)) > EntityFlags.None && num12 > 70f)
					{
						this.SpawnParticle("blood_vehicle", entity.entityId, 0.22f);
						if (num12 > 200f)
						{
							this.SpawnParticle("blood_vehicle", entity.entityId, 0.35f);
						}
					}
					float num13 = 1f;
					if (flag2)
					{
						this.velocityMax *= 0.7f;
						num13 *= 15f;
					}
					EntityPlayer entityPlayer2 = firstAttached as EntityPlayer;
					if (entityPlayer2)
					{
						entityPlayer2.MinEventContext.Other = (entity as EntityAlive);
						entityPlayer2.FireEvent(MinEventTypes.onSelfVehicleAttackedOther, true);
					}
					if (flag4 && entity.IsDead())
					{
						flag3 = false;
					}
					else if (num9 >= num13)
					{
						float num14 = num9 * 0.09f;
						if (num9 < 8f && num14 > 0.9f)
						{
							num14 = 0.9f;
						}
						if (this.rand.RandomFloat < num14)
						{
							flag3 = true;
						}
					}
				}
				if (entity.emodel.IsRagdollOn)
				{
					num11 *= 0.3f;
				}
				if (flag3)
				{
					entity.emodel.DoRagdoll(EModelBase.RagdollMode.Default, 2.5f, enumBodyPartHit, vector, vector3, false);
				}
				if (num11 > 2.1f)
				{
					float num15 = 1f + (num11 - 2.1f) * 28f;
					num15 *= this.vehicle.EffectSelfDamagePer;
					if (flag)
					{
						num15 *= this.vehicle.EffectStrongSelfDamagePer;
					}
					float num16 = (this.Health > 1) ? 1f : 0.1f;
					this.damageAccumulator += num15 * num16;
					this.ApplyAccumulatedDamage();
				}
				if (num > 0.1f && num2 > 0.2f)
				{
					this.velocityMax *= Mathf.LerpUnclamped(1f, 0.4f + num10 * 0.39666668f, num2);
					return;
				}
			}
		}
		else
		{
			Vector3 a = this.lastRBVel;
			float magnitude = a.magnitude;
			float num17 = Utils.FastMax(0f, magnitude - 1.5f) * this.vehicleRB.mass * 0.058333334f;
			if (isStay)
			{
				num17 *= 0.2f;
			}
			if (num17 < 2f)
			{
				this.collisionGrazeCount++;
				return;
			}
			this.collisionBlockDamage = num17;
			this.collisionVelNorm = a * (1f / magnitude);
			this.collisionIgnoreCount = 0;
			int contactCount2 = collision.contactCount;
			for (int j = 0; j < contactCount2; j++)
			{
				ContactPoint contact2 = collision.GetContact(j);
				Ray ray = new Ray(contact2.point + Origin.position + contact2.normal * 0.004f, -contact2.normal);
				bool flag5 = Voxel.Raycast(this.world, ray, 0.03f, -555520029, 69, 0f);
				if (!flag5)
				{
					ray.origin += contact2.normal * -contact2.separation;
					ray.direction = -contact2.normal + this.collisionVelNorm;
					flag5 = Voxel.Raycast(this.world, ray, 0.03f, -555520029, 69, 0f);
				}
				if (flag5 && GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag))
				{
					bool flag6 = false;
					for (int k = 0; k < this.collisionHits.Count; k++)
					{
						if (this.collisionHits[k].hit.blockPos == Voxel.voxelRayHitInfo.hit.blockPos)
						{
							flag6 = true;
							break;
						}
					}
					if (!flag6)
					{
						this.contactPoints.Add(contact2);
						this.collisionHits.Add(Voxel.voxelRayHitInfo.Clone());
					}
				}
				else
				{
					this.collisionIgnoreCount++;
				}
			}
		}
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x0010146A File Offset: 0x000FF66A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ApplyCollisionsCoroutine()
	{
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		for (;;)
		{
			yield return wait;
			int count = this.contactPoints.Count;
			if (count > 0)
			{
				float num = (this.Health > 1) ? 1f : 0.1f;
				int entityId = this.entityId;
				ItemActionAttack.EnumAttackMode attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvesting;
				if (this.hitEffectCount <= 0)
				{
					attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvestingOrEffects;
				}
				float num2 = 1f / ((float)count + 0.001f);
				float num3 = this.collisionBlockDamage;
				num3 *= num2;
				for (int i = 0; i < count; i++)
				{
					ContactPoint contactPoint = this.contactPoints[i];
					WorldRayHitInfo worldRayHitInfo = this.collisionHits[i];
					float num4 = -Vector3.Dot(contactPoint.normal, this.collisionVelNorm);
					num4 = Mathf.Pow(num4 * 1.01f, 3f);
					num4 = Utils.FastClamp(num4, 0.01f, 1f);
					float num5 = 0f;
					float num6 = 2.5f;
					bool flag = contactPoint.thisCollider.CompareTag("E_VehicleStrong");
					bool flag2 = worldRayHitInfo.tag == "T_Mesh";
					if (flag2)
					{
						if (contactPoint.normal.y < 0.85f)
						{
							num5 = 0.7f + 4f * this.rand.RandomFloat * num4;
							num6 = 0.1f;
						}
					}
					else
					{
						num5 = num3 * num4;
						if (flag)
						{
							num5 *= this.vehicle.EffectBlockDamagePer;
						}
						float vehicleHitScale = worldRayHitInfo.hit.blockValue.Block.VehicleHitScale;
						num5 *= vehicleHitScale;
						num6 /= vehicleHitScale;
						if (num5 < 5f)
						{
							num5 = 0f;
						}
					}
					if (num5 >= 1f)
					{
						List<string> buffActions = null;
						ItemActionAttack.AttackHitInfo attackHitInfo = new ItemActionAttack.AttackHitInfo();
						attackHitInfo.hardnessScale = 1f;
						if (flag2 || !worldRayHitInfo.hit.blockValue.Block.shape.IsTerrain())
						{
							ItemActionAttack.Hit(worldRayHitInfo, entityId, EnumDamageTypes.Bashing, num5, num5, 1f, 1f, 0f, 0.05f, "metal", null, buffActions, attackHitInfo, 1, 0, 0f, null, null, attackMode, null, -1, null, false, false, true, null);
							int num7 = this.hitEffectCount - 1;
							this.hitEffectCount = num7;
							if (num7 <= 0)
							{
								attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvestingOrEffects;
							}
						}
						if (!attackHitInfo.bBlockHit)
						{
							ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
							if (chunkCache != null)
							{
								Vector3i vector3i = Vector3i.FromVector3Rounded(contactPoint.point + Origin.position);
								for (int j = 0; j >= -1; j--)
								{
									worldRayHitInfo.hit.blockPos.y = vector3i.y + j;
									for (int k = 0; k >= -1; k--)
									{
										worldRayHitInfo.hit.blockPos.z = vector3i.z + k;
										for (int l = 0; l >= -1; l--)
										{
											worldRayHitInfo.hit.blockPos.x = vector3i.x + l;
											if (!chunkCache.GetBlock(worldRayHitInfo.hit.blockPos).Block.shape.IsTerrain())
											{
												ItemActionAttack.Hit(worldRayHitInfo, entityId, EnumDamageTypes.Bashing, num5, num5, 1f, 1f, 0f, 0.05f, "metal", null, buffActions, attackHitInfo, 1, 0, 0f, null, null, attackMode, null, -1, null, false, false, true, null);
												int num7 = this.hitEffectCount - 1;
												this.hitEffectCount = num7;
												if (num7 <= 0)
												{
													attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvestingOrEffects;
												}
												if (attackHitInfo.bBlockHit)
												{
													j = -999;
													k = -999;
													break;
												}
											}
										}
									}
								}
							}
						}
						if (attackHitInfo.bKilled && attackHitInfo.bBlockHit)
						{
							BlockModelTree blockModelTree = attackHitInfo.blockBeingDamaged.Block as BlockModelTree;
							if (blockModelTree != null && blockModelTree.isMultiBlock && blockModelTree.multiBlockPos.dim.y >= 12)
							{
								this.velocityMax *= 0.3f;
								this.vehicleRB.AddRelativeForce(Vector3.up * 2.5f, ForceMode.VelocityChange);
								this.vehicleRB.AddRelativeForce(this.collisionVelNorm * 2f, ForceMode.VelocityChange);
							}
						}
						if ((attackHitInfo.bKilled || !attackHitInfo.bBlockHit) && attackHitInfo.hardnessScale > 0f)
						{
							this.collisionIgnoreCount++;
						}
						num5 = Utils.FastMin(num5, (float)attackHitInfo.damageGiven);
					}
					float num8 = num5 * num6;
					num8 *= this.vehicle.EffectSelfDamagePer;
					if (flag)
					{
						num8 *= this.vehicle.EffectStrongSelfDamagePer;
					}
					this.damageAccumulator += num8 * num;
					if (num8 > 50f)
					{
						this.SpawnParticle("blockdestroy_metal", worldRayHitInfo.hit.pos);
					}
				}
				this.ApplyAccumulatedDamage();
				int num9 = this.collisionIgnoreCount - count;
				if (num9 >= 0)
				{
					this.PhysicsRevertCollisionMotion(num9);
				}
				this.contactPoints.Clear();
				this.collisionHits.Clear();
			}
		}
		yield break;
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x0010147C File Offset: 0x000FF67C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyAccumulatedDamage()
	{
		if (this.damageAccumulator >= 1f)
		{
			int num = (int)this.damageAccumulator;
			this.damageAccumulator -= (float)num;
			this.ApplyDamage(num);
		}
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x001014B4 File Offset: 0x000FF6B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnParticle(string _particleName, Vector3 _pos)
	{
		Vector3i blockPos = World.worldToBlockPos(_pos);
		float lightBrightness = this.world.GetLightBrightness(blockPos);
		this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(_particleName, _pos, lightBrightness, Color.white, null, null, false, 1f, ""), this.entityId, false, false);
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x00101508 File Offset: 0x000FF708
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnParticle(string _particleName, int _entityId, float _offsetY)
	{
		Vector3 pos = new Vector3(0f, _offsetY, 0f);
		this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(_particleName, pos, 1f, Color.white, null, _entityId, ParticleEffect.Attachment.Pelvis), this.entityId, false, false);
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x00101554 File Offset: 0x000FF754
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsRevertCollisionMotion(int _ignoreExcess)
	{
		if (_ignoreExcess == 0)
		{
			float num = Time.fixedDeltaTime * 0.5f;
			float num2 = this.lastRBVel.x * num;
			float num3 = this.lastRBVel.z * num;
			if (num2 < -0.0001f || num2 > 0.0001f || num3 < -0.0001f || num3 > 0.0001f)
			{
				this.lastRBPos.x = this.lastRBPos.x + num2;
				this.lastRBPos.z = this.lastRBPos.z + num3;
				this.vehicleRB.position = this.lastRBPos;
			}
		}
		Vector3 velocity = this.vehicleRB.velocity;
		velocity.x = this.lastRBVel.x * 0.9f;
		velocity.z = this.lastRBVel.z * 0.9f;
		velocity.y = this.lastRBVel.y * 0.6f + velocity.y * 0.4f;
		this.vehicleRB.velocity = velocity;
		this.vehicleRB.angularVelocity = this.lastRBAngVel;
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x0010165C File Offset: 0x000FF85C
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawRayHandle(Vector3 pos, Vector3 dir, Color color, float duration = 0f)
	{
		Vector3 normalized = Vector3.Cross(Vector3.up, dir).normalized;
		Debug.DrawRay(pos, normalized * 0.005f, Color.blue, duration);
		Debug.DrawRay(pos, dir, color, duration);
	}

	// Token: 0x06002903 RID: 10499 RVA: 0x001016A0 File Offset: 0x000FF8A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawBlocks(WorldRayHitInfo hitInfo)
	{
		ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
		if (chunkCache == null)
		{
			return;
		}
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					Vector3i blockPos = hitInfo.hit.blockPos;
					blockPos.x += j;
					blockPos.y += i;
					blockPos.z += k;
					Vector3 start = blockPos.ToVector3() - Origin.position;
					BlockValue block = chunkCache.GetBlock(blockPos);
					Color color = Color.black;
					if (!block.isair)
					{
						if (block.Block.shape.IsTerrain())
						{
							color = Color.yellow;
						}
						else
						{
							color = Color.white;
						}
					}
					Debug.DrawRay(start, Vector3.up, color);
					Debug.DrawRay(start, Vector3.right, color);
					Debug.DrawRay(start, Vector3.forward, color);
				}
			}
		}
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x0010179C File Offset: 0x000FF99C
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity FindEntity(Transform t)
	{
		Entity componentInChildren = t.GetComponentInChildren<Entity>();
		if (componentInChildren)
		{
			return componentInChildren;
		}
		return t.GetComponentInParent<Entity>();
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void entityCollision(Vector3 _motion)
	{
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x001017C4 File Offset: 0x000FF9C4
	public static EntityVehicle FindCollisionEntity(Transform t)
	{
		EntityVehicle entityVehicle = t.GetComponent<EntityVehicle>();
		if (!entityVehicle)
		{
			CollisionCallForward componentInParent = t.GetComponentInParent<CollisionCallForward>();
			if (componentInParent)
			{
				entityVehicle = (componentInParent.Entity as EntityVehicle);
			}
		}
		return entityVehicle;
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x001017FC File Offset: 0x000FF9FC
	public override float GetBlockDamageScale(bool isTerrain)
	{
		EntityAlive entityAlive = base.AttachedMainEntity as EntityAlive;
		if (entityAlive)
		{
			return entityAlive.GetBlockDamageScale(isTerrain);
		}
		return 1f;
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void switchModelView(EnumEntityModelView modelView)
	{
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x000027FC File Offset: 0x000009FC
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x0010182C File Offset: 0x000FFA2C
	public override void MoveByAttachedEntity(EntityPlayerLocal _player)
	{
		if (this.movementInput == null)
		{
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_player);
		if (uiforPlayer == null || uiforPlayer.playerInput == null)
		{
			return;
		}
		PlayerActionsVehicle vehicleActions = uiforPlayer.playerInput.VehicleActions;
		MovementInput movementInput = _player.movementInput;
		if (_player == base.AttachedMainEntity)
		{
			this.movementInput.moveForward = (_player.MoveController.isAutorun ? 1f : vehicleActions.Move.Y);
			this.movementInput.moveStrafe = vehicleActions.Move.X;
			this.movementInput.down = vehicleActions.Hop.IsPressed;
			this.movementInput.jump = vehicleActions.Brake.IsPressed;
			if (EffectManager.GetValue(PassiveEffects.FlipControls, null, 0f, _player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
			{
				this.movementInput.moveForward *= -1f;
				this.movementInput.moveStrafe *= -1f;
			}
			this.movementInput.running = _player.movementInput.running;
			this.movementInput.lastInputController = movementInput.lastInputController;
			if (vehicleActions.ToggleTurnMode.WasPressed && !uiforPlayer.windowManager.IsModalWindowOpen())
			{
				EntityVehicle.isTurnTowardsLook = !EntityVehicle.isTurnTowardsLook;
			}
		}
		MovementInput movementInput2 = movementInput;
		movementInput2.rotation.x = movementInput2.rotation.x * this.vehicle.CameraTurnRate.x;
		MovementInput movementInput3 = movementInput;
		movementInput3.rotation.y = movementInput3.rotation.y * this.vehicle.CameraTurnRate.y;
		float num = vehicleActions.Scroll.Value;
		if (vehicleActions.LastInputType == BindingSourceType.DeviceBindingSource)
		{
			num *= 0.25f;
		}
		if (num != 0f)
		{
			EntityVehicle.cameraDistScale += num * -0.5f;
			EntityVehicle.cameraDistScale = Utils.FastClamp(EntityVehicle.cameraDistScale, 0.3f, 1.2f);
			this.cameraOutTime = 999f;
		}
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x00101A2C File Offset: 0x000FFC2C
	public bool HasHeadlight()
	{
		VPHeadlight vpheadlight = this.vehicle.FindPart("headlight") as VPHeadlight;
		return vpheadlight != null && (vpheadlight.GetTransform() || vpheadlight.modInstalled);
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x00101A6A File Offset: 0x000FFC6A
	public void ToggleHeadlight()
	{
		this.IsHeadlightOn = !this.IsHeadlightOn;
	}

	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x0600290D RID: 10509 RVA: 0x00101A7C File Offset: 0x000FFC7C
	// (set) Token: 0x0600290E RID: 10510 RVA: 0x00101AAA File Offset: 0x000FFCAA
	public bool IsHeadlightOn
	{
		get
		{
			VPHeadlight vpheadlight = this.vehicle.FindPart("headlight") as VPHeadlight;
			return vpheadlight != null && vpheadlight.IsOn();
		}
		set
		{
			this.vehicle.FireEvent(VehiclePart.Event.LightsOn, null, (float)(value ? 1 : 0));
		}
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x00101AC4 File Offset: 0x000FFCC4
	public override float GetLightLevel()
	{
		VPHeadlight vpheadlight = this.vehicle.FindPart("headlight") as VPHeadlight;
		if (vpheadlight == null)
		{
			return 0f;
		}
		return vpheadlight.GetLightLevel();
	}

	// Token: 0x06002910 RID: 10512 RVA: 0x00101AF8 File Offset: 0x000FFCF8
	public void UseHorn(EntityPlayerLocal player)
	{
		string hornSoundName = this.vehicle.GetHornSoundName();
		if (hornSoundName.Length > 0)
		{
			this.PlayOneShot(hornSoundName, false, false, false, null, 1f);
		}
		float time = Time.time;
		if (time - this.lastHonkEventTime > 1f)
		{
			if (this.HornActivation != null)
			{
				this.HornActivation.Activate();
			}
			string hornEventName = this.vehicle.GetHornEventName();
			if (hornEventName != "")
			{
				GameEventManager.Current.HandleAction(hornEventName, null, player, false, this.position, "", "", false, true, "", null, null);
				this.lastHonkEventTime = time;
			}
		}
	}

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06002911 RID: 10513 RVA: 0x00101BA5 File Offset: 0x000FFDA5
	public bool HasDriver
	{
		get
		{
			return this.hasDriver;
		}
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x00101BB0 File Offset: 0x000FFDB0
	[PublicizedFrom(EAccessModifier.Private)]
	public float CalcWaterDepth(float offsetY)
	{
		Vector3 position = this.position;
		position.y += offsetY;
		Vector3i vector3i = World.worldToBlockPos(position);
		if (this.world.IsWater(vector3i))
		{
			for (int i = 0; i < 5; i++)
			{
				vector3i.y++;
				if (!this.world.IsWater(vector3i))
				{
					break;
				}
			}
			return (float)vector3i.y - position.y;
		}
		return 0f;
	}

	// Token: 0x1700048F RID: 1167
	// (set) Token: 0x06002913 RID: 10515 RVA: 0x00101C1F File Offset: 0x000FFE1F
	public override int Health
	{
		set
		{
			base.Stats.Health.Value = (float)value;
			if (this.vehicle != null)
			{
				this.vehicle.FireEvent(Vehicle.Event.HealthChanged);
			}
		}
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x00101C48 File Offset: 0x000FFE48
	[PublicizedFrom(EAccessModifier.Protected)]
	public override DamageResponse damageEntityLocal(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		DamageResponse damageResponse = new DamageResponse
		{
			Source = _damageSource,
			Strength = _strength,
			Critical = _criticalHit,
			HitDirection = Utils.EnumHitDirection.None,
			MovementState = this.MovementState,
			Random = this.rand.RandomFloat,
			ImpulseScale = impulseScale
		};
		this.ProcessDamageResponseLocal(damageResponse);
		return damageResponse;
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x00101CB0 File Offset: 0x000FFEB0
	public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		DamageSource source = _dmResponse.Source;
		if (source.damageType == EnumDamageTypes.Disease || source.damageType == EnumDamageTypes.Suffocation)
		{
			return;
		}
		this.UpdateInteractionUI();
		int strength = _dmResponse.Strength;
		if (base.AttachedMainEntity && !this.isEntityRemote && this.world.IsWorldEvent(World.WorldEvent.BloodMoon))
		{
			this.velocityMax *= 0.6f;
			this.vehicleRB.AddRelativeForce(_dmResponse.Source.getDirection() * 6f, ForceMode.VelocityChange);
		}
		if (this.attachedEntities != null && _dmResponse.Source.GetSource() == EnumDamageSource.External)
		{
			int strength2 = Utils.FastRoundToInt((float)_dmResponse.Strength * this.vehicle.GetPlayerDamagePercent());
			DamageSource damageSource = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Bashing);
			EntityAlive entityAlive = this.world.GetEntity(source.getEntityId()) as EntityAlive;
			foreach (Entity entity in this.attachedEntities)
			{
				if (entity != null)
				{
					EntityAlive entityAlive2 = entity as EntityAlive;
					if (entityAlive == null || entityAlive2 == null || entityAlive2.FriendlyFireCheck(entityAlive))
					{
						entity.DamageEntity(damageSource, strength2, false, 1f);
					}
				}
			}
		}
		this.ApplyDamage(strength);
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x00101DFC File Offset: 0x000FFFFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyDamage(int damage)
	{
		int num = this.Health;
		if (num <= 0)
		{
			return;
		}
		bool flag = damage >= 99999;
		if (num == 1 || flag)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.explodeHealth -= (float)damage;
				if (this.explodeHealth <= 0f && (flag || this.rand.RandomFloat < 0.2f))
				{
					this.DropItemsAsBackpack();
					this.Kill();
					GameManager.Instance.ExplosionServer(base.GetPosition(), World.worldToBlockPos(base.GetPosition()), base.transform.rotation, EntityClass.list[this.entityClass].explosionData, this.entityId, 0f, false, null);
					return;
				}
			}
		}
		else
		{
			num -= damage;
			if (num <= 1)
			{
				num = 1;
				this.explodeHealth = (float)this.vehicle.GetMaxHealth() * 0.03f;
			}
			this.Health = num;
		}
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x00101EF0 File Offset: 0x001000F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyCollisionDamageToAttached(int damage)
	{
		DamageSource damageSource = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.VehicleInside);
		int attachMaxCount = base.GetAttachMaxCount();
		for (int i = 0; i < attachMaxCount; i++)
		{
			Entity attached = base.GetAttached(i);
			if (attached)
			{
				attached.DamageEntity(damageSource, damage, false, 1f);
			}
		}
	}

	// Token: 0x06002918 RID: 10520 RVA: 0x00101F38 File Offset: 0x00100138
	public override bool HasImmunity(BuffClass _buffClass)
	{
		return _buffClass.DamageType != EnumDamageTypes.Heat;
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x00101F48 File Offset: 0x00100148
	public bool IsLockedForLocalPlayer(EntityAlive _entityFocusing)
	{
		bool flag = this.LocalPlayerIsOwner();
		return this.isLocked && !flag && this.hasLock() && !this.isAllowedUser(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x00101F84 File Offset: 0x00100184
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitLocalActivationCommands(Action<EntityActivationCommand> _addCallback)
	{
		_addCallback(new EntityActivationCommand("drive", "drive", null, null));
		_addCallback(new EntityActivationCommand("ride", "drive", null, null));
		_addCallback(new EntityActivationCommand("service", "service", null, null));
		_addCallback(new EntityActivationCommand("repair", "wrench", null, null));
		_addCallback(new EntityActivationCommand("lock", "lock", null, null));
		_addCallback(new EntityActivationCommand("unlock", "unlock", null, null));
		_addCallback(new EntityActivationCommand("storage", "loot_sack", null, null));
		_addCallback(new EntityActivationCommand("keypad", "keypad", null, null));
		_addCallback(new EntityActivationCommand("refuel", "gas", null, null));
		_addCallback(new EntityActivationCommand("take", "hand", null, null));
		_addCallback(new EntityActivationCommand("horn", "horn", null, null));
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x0010208E File Offset: 0x0010028E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ReorderActivationCommands(List<EntityActivationCommand> _commands)
	{
		if (this.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
		{
			Entity.MoveActivationCommandAfter(_commands, "storage", "horn");
		}
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x001020B0 File Offset: 0x001002B0
	public override bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		if (this.IsDead())
		{
			return false;
		}
		bool flag = this.LocalPlayerIsOwner();
		bool flag2 = !this.isLocked || flag || !this.hasLock() || this.isAllowedUser(PlatformManager.InternalLocalUserIdentifier);
		bool flag3 = base.CanAttach(_playerFocusing) && this.isDriveable();
		bool flag4 = base.IsDriven();
		if (base.CommandIs(_commandName, "drive"))
		{
			return !flag4 && flag3 && flag2;
		}
		if (base.CommandIs(_commandName, "ride"))
		{
			return flag4 && flag3 && flag2;
		}
		if (base.CommandIs(_commandName, "service"))
		{
			return flag2;
		}
		if (base.CommandIs(_commandName, "repair"))
		{
			return this.vehicle.GetRepairAmountNeeded() > 0;
		}
		if (base.CommandIs(_commandName, "lock"))
		{
			return this.hasLock() && !this.isLocked && !flag4 && flag;
		}
		if (base.CommandIs(_commandName, "unlock"))
		{
			return this.hasLock() && this.isLocked && !flag4 && flag;
		}
		if (base.CommandIs(_commandName, "keypad"))
		{
			return flag || (this.hasLock() && this.isLocked && !this.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier) && this.HasPassword());
		}
		if (base.CommandIs(_commandName, "refuel"))
		{
			return this.hasGasCan(_playerFocusing) && this.needsFuel() && EntityVehicle.VehicleFuelUsageModifier > 0f && flag2;
		}
		if (base.CommandIs(_commandName, "take"))
		{
			return !this.hasDriver && flag2 && flag;
		}
		if (base.CommandIs(_commandName, "horn"))
		{
			return this.vehicle.HasHorn() && flag2;
		}
		if (base.CommandIs(_commandName, "storage"))
		{
			return this.bag != null;
		}
		return base.AllowActivationCommand(_commandName, _playerFocusing);
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x00102278 File Offset: 0x00100478
	public override string GetActivationText()
	{
		GameManager instance = GameManager.Instance;
		EntityPlayerLocal entityPlayerLocal;
		if (instance == null)
		{
			entityPlayerLocal = null;
		}
		else
		{
			World world = instance.World;
			entityPlayerLocal = ((world != null) ? world.GetPrimaryPlayer() : null);
		}
		EntityPlayerLocal entityPlayerLocal2 = entityPlayerLocal;
		if (entityPlayerLocal2 == null)
		{
			return string.Empty;
		}
		string arg = entityPlayerLocal2.playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + entityPlayerLocal2.playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string text = string.Format(Localization.Get("tooltipInteract", false, null), arg, Localization.Get(this.LocalizedEntityName, false, null));
		if (this.IsLockedForLocalPlayer(entityPlayerLocal2))
		{
			text = Localization.Get("ttLocked", false, null) + "\n" + text;
		}
		return text;
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x00102328 File Offset: 0x00100528
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityActivated(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_command.commandId, "storage") && this.isLocked && !this.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
		{
			Manager.Play(this, "locked", 1f, false);
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_playerFocusing);
		if (_playerFocusing.inventory.IsHoldingItemActionRunning() || uiforPlayer.xui.IsUsingItemActionEntryUse)
		{
			return;
		}
		if (base.CommandIs(_command.commandId, "drive") || base.CommandIs(_command.commandId, "ride"))
		{
			if (uiforPlayer != null && uiforPlayer.windowManager.IsWindowOpen("windowpaging"))
			{
				return;
			}
			if (base.CanAttach(_playerFocusing) && _playerFocusing.AttachedToEntity == null && this.isDriveable() && (!this.isLocked || !this.hasLock() || this.LocalPlayerIsOwner() || this.isAllowedUser(PlatformManager.InternalLocalUserIdentifier)))
			{
				if (EffectManager.GetValue(PassiveEffects.NoVehicle, null, 0f, _playerFocusing, null, base.EntityClass.Tags, true, true, true, true, true, 1, true, false) > 0f)
				{
					Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
					return;
				}
				Vector3 vector = this.position - Origin.position;
				vector.y += 0.5f;
				Vector3 up = Vector3.up;
				bool flag = false;
				for (int i = 0; i < 8; i++)
				{
					Vector3 a = Quaternion.AngleAxis((float)(i * 45), up) * base.transform.forward;
					if (Physics.Raycast(vector + a * 0.25f, up, 1.3f, 65536))
					{
						flag = true;
						Vector3 vector2 = _playerFocusing.position - Origin.position;
						vector2.y += 1.1f;
						vector2 = (vector2 - vector).normalized * this.vehicleRB.mass * 0.005f;
						this.AddForce(vector2, ForceMode.VelocityChange);
						break;
					}
				}
				if (!flag)
				{
					this.EnterVehicle(_playerFocusing);
				}
			}
		}
		if (base.CommandIs(_command.commandId, "repair") && XUiM_Vehicle.RepairVehicle(uiforPlayer.xui, this.vehicle))
		{
			this.PlayOneShot("crafting/craft_repair_item", true, false, false, null, 1f);
			this.SendSyncData(4);
		}
		if (base.CommandIs(_command.commandId, "lock"))
		{
			this.vehicle.SetLocked(true, _playerFocusing);
			this.PlayOneShot("misc/locking", true, false, false, null, 1f);
			this.SendSyncData(2);
		}
		if (base.CommandIs(_command.commandId, "unlock"))
		{
			this.vehicle.SetLocked(false, _playerFocusing);
			this.PlayOneShot("misc/unlocking", true, false, false, null, 1f);
			this.SendSyncData(2);
		}
		if (base.CommandIs(_command.commandId, "keypad"))
		{
			this.PlayOneShot("misc/password_type", true, false, false, null, 1f);
			XUiC_KeypadWindow.Open(uiforPlayer, this, null, null);
		}
		if (base.CommandIs(_command.commandId, "refuel") && this.AddFuelFromInventory(_playerFocusing))
		{
			this.SendSyncData(4);
		}
		if (base.CommandIs(_command.commandId, "take"))
		{
			if (!this.bag.IsEmpty())
			{
				GameManager.ShowTooltip(_playerFocusing, Localization.Get("ttEmptyVehicleBeforePickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
				return;
			}
			if (!this.hasDriver)
			{
				ItemStack itemStack = new ItemStack(this.vehicle.GetUpdatedItemValue(), 1);
				if (_playerFocusing.inventory.CanTakeItem(itemStack) || _playerFocusing.bag.CanTakeItem(itemStack))
				{
					base.Collect(_playerFocusing.entityId);
				}
				else
				{
					GameManager.ShowTooltip(_playerFocusing, Localization.Get("xuiInventoryFullForPickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
				}
			}
		}
		if (base.CommandIs(_command.commandId, "horn"))
		{
			this.UseHorn(_playerFocusing);
		}
		if (base.CommandIs(_command.commandId, "service") || base.CommandIs(_command.commandId, "storage"))
		{
			LockManager.Instance.LockRequestLocal(this, new Entity.EntityLockContext(_command.commandId.ToString(), this.bag), 0);
		}
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x001027A8 File Offset: 0x001009A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartInteraction(ReadOnlySpan<char> _commandName)
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (!primaryPlayer)
		{
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(primaryPlayer);
		GUIWindowManager windowManager = uiforPlayer.windowManager;
		uiforPlayer.xui.Vehicle.CurrentVehicle = this;
		if (base.CommandIs(_commandName, "service"))
		{
			((XUiC_VehicleWindowGroup)((XUiWindowGroup)windowManager.GetWindow("vehicle")).Controller).CurrentVehicleEntity = this;
			windowManager.Open("vehicle", true);
			Manager.BroadcastPlayByLocalPlayer(this.position, "UseActions/service_vehicle");
			return;
		}
		if (base.CommandIs(_commandName, "storage"))
		{
			XUiC_BagStorageWindowGroup.Open(uiforPlayer.xui, this, this.bag, LootContainer.GetLootContainer(this.GetLootList(), true), Localization.Get("xuiStorage", false, null), new Action(this.SetBagModified), new Action(this.StopUIInteraction), new Func<bool>(this.CheckUIInteraction), false);
			return;
		}
		this.StopInteraction(0);
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x001028A0 File Offset: 0x00100AA0
	public bool CheckUIInteraction()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (!primaryPlayer)
		{
			return false;
		}
		float distanceSq = base.GetDistanceSq(primaryPlayer);
		float num = Constants.cPlayerInteractDistance * Constants.cPlayerInteractDistance;
		return distanceSq <= num;
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x001028E0 File Offset: 0x00100AE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateInteractionUI()
	{
		if (GameManager.Instance.World == null)
		{
			return;
		}
		for (int i = 0; i < LocalPlayerUI.PlayerUIs.Count; i++)
		{
			LocalPlayerUI localPlayerUI = LocalPlayerUI.PlayerUIs[i];
			if (localPlayerUI != null && localPlayerUI.xui != null && localPlayerUI.windowManager.IsWindowOpen("vehicle"))
			{
				XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)localPlayerUI.windowManager.GetWindow("vehicle");
				if (xuiWindowGroup != null && xuiWindowGroup.Controller != null)
				{
					xuiWindowGroup.Controller.RefreshBindingsSelfAndChildren();
				}
			}
		}
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x00102970 File Offset: 0x00100B70
	public void StopUIInteraction()
	{
		this.StopInteraction(14);
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x0010297A File Offset: 0x00100B7A
	[PublicizedFrom(EAccessModifier.Private)]
	public void StopInteraction(ushort syncFlags = 0)
	{
		LocalPlayerUI.GetUIForPrimaryPlayer().xui.Vehicle.CurrentVehicle = null;
		if (syncFlags != 0)
		{
			this.SendSyncData(syncFlags);
		}
		LockManager.Instance.UnlockRequestLocal();
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsSharedLock(ushort _channel)
	{
		return false;
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000E4A5F File Offset: 0x000E2C5F
	public override void OnLockedServer(bool _success, int _lockingPlayerID, ILockContext _context, ushort _channel)
	{
		base.OnLockedServer(_success, _lockingPlayerID, _context, _channel);
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x000E4A6C File Offset: 0x000E2C6C
	public override void OnUnlockedServer(int _unlockingPlayerId, ushort _channel)
	{
		base.OnUnlockedServer(_unlockingPlayerId, _channel);
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x001029A8 File Offset: 0x00100BA8
	public override void OnLockedLocal(bool _success, ILockContext _context, ushort _channel)
	{
		if (!_success)
		{
			GameManager.ShowTooltip(GameManager.Instance.World.GetPrimaryPlayer(), Localization.Get("ttVehicleInUse", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		Entity.EntityLockContext entityLockContext = _context as Entity.EntityLockContext;
		if (entityLockContext == null)
		{
			Log.Warning("[EntityVehicle] Missing or invalid lock context.");
			LockManager.Instance.UnlockRequestLocal();
			return;
		}
		if (entityLockContext.Bag != null)
		{
			this.bag = entityLockContext.Bag.Clone();
		}
		this.StartInteraction(entityLockContext.Command);
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000EB674 File Offset: 0x000E9874
	public override void OnCollectServer(int _playerId)
	{
		this.world.RemoveEntity(this.entityId, EnumRemoveEntityReason.Killed);
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x00102A34 File Offset: 0x00100C34
	public override void OnCollectLocal(int _playerId)
	{
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_playerId) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		ItemStack itemStack = new ItemStack(this.vehicle.GetUpdatedItemValue(), 1);
		if (!uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
		{
			GameManager.Instance.ItemDropServer(itemStack, entityPlayerLocal.GetPosition(), Vector3.zero, _playerId, 60f, false);
		}
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x00102A9C File Offset: 0x00100C9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void DropItemsAsBackpack()
	{
		List<ItemStack> list = new List<ItemStack>();
		foreach (ItemStack itemStack in this.bag.GetSlots())
		{
			if (!itemStack.IsEmpty())
			{
				list.Add(itemStack);
			}
		}
		ItemValue updatedItemValue = this.vehicle.GetUpdatedItemValue();
		for (int j = 0; j < updatedItemValue.CosmeticMods.Length; j++)
		{
			ItemValue itemValue = updatedItemValue.CosmeticMods[j];
			if (itemValue != null && !itemValue.IsEmpty())
			{
				list.Add(new ItemStack(itemValue, 1));
			}
		}
		for (int k = 0; k < updatedItemValue.Modifications.Length; k++)
		{
			ItemValue itemValue2 = updatedItemValue.Modifications[k];
			if (itemValue2 != null && !itemValue2.IsEmpty())
			{
				list.Add(new ItemStack(itemValue2, 1));
			}
		}
		this.dropLoot(list.ToArray(), 0.9f);
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x00102B78 File Offset: 0x00100D78
	[PublicizedFrom(EAccessModifier.Private)]
	public void dropLoot(ItemStack[] items, float height)
	{
		Vector3 position = this.position;
		position.y += height;
		GameManager.Instance.DropContentInLootContainerServer(-1, "DroppedVehicleContainer", position, items, false, new Vector3?(new Vector3(0f, 1f, 0f)));
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x00102BC4 File Offset: 0x00100DC4
	public void AddMaxFuel()
	{
		this.vehicle.AddFuel(this.vehicle.GetMaxFuelLevel());
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x00102BDC File Offset: 0x00100DDC
	public bool AddFuelFromInventory(EntityAlive entity)
	{
		if (this.vehicle.GetFuelPercent() < 1f)
		{
			float maxFuelLevel = this.vehicle.GetMaxFuelLevel();
			float fuelLevel = this.vehicle.GetFuelLevel();
			float f = Mathf.Min(2500f, (maxFuelLevel - fuelLevel) * 25f);
			float num = this.takeFuel(entity, Mathf.CeilToInt(f));
			this.vehicle.AddFuel(num / 25f);
			this.PlayOneShot("useactions/gas_refill", false, false, false, null, 1f);
			return true;
		}
		return false;
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x00102C5E File Offset: 0x00100E5E
	public int GetFuelCount()
	{
		return Mathf.FloorToInt(this.vehicle.GetFuelLevel() * 25f);
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x00102C78 File Offset: 0x00100E78
	[PublicizedFrom(EAccessModifier.Private)]
	public float takeFuel(EntityAlive _entityFocusing, int count)
	{
		EntityPlayer entityPlayer = _entityFocusing as EntityPlayer;
		if (!entityPlayer)
		{
			return 0f;
		}
		string fuelItem = this.GetVehicle().GetFuelItem();
		if (fuelItem == "")
		{
			return 0f;
		}
		ItemValue item = ItemClass.GetItem(fuelItem, false);
		int num = entityPlayer.inventory.DecItem(item, count, false, null);
		if (num == 0)
		{
			num = entityPlayer.bag.DecItem(item, count, false, null);
			if (num == 0)
			{
				return 0f;
			}
		}
		float num2 = (float)num;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entityFocusing as EntityPlayerLocal);
		if (null != uiforPlayer)
		{
			ItemStack @is = new ItemStack(item, num);
			uiforPlayer.xui.CollectedItemList.RemoveItemStack(@is);
		}
		else
		{
			Log.Warning("EntityVehicle::takeFuel - Failed to remove item stack from player's collected item list.");
		}
		return num2;
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isEntityStatic()
	{
		return true;
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanBePushed()
	{
		return false;
	}

	// Token: 0x06002932 RID: 10546 RVA: 0x00102D2E File Offset: 0x00100F2E
	public Vehicle GetVehicle()
	{
		return this.vehicle;
	}

	// Token: 0x06002933 RID: 10547 RVA: 0x00102D36 File Offset: 0x00100F36
	public void SetBagModified()
	{
		this.SendSyncData(8);
	}

	// Token: 0x06002934 RID: 10548 RVA: 0x00102D40 File Offset: 0x00100F40
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendSyncData(ushort syncFlags)
	{
		NetPackageVehicleDataSync package = NetPackageManager.GetPackage<NetPackageVehicleDataSync>().Setup(this, GameManager.Instance.World.GetPrimaryPlayerId(), syncFlags);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x00102DA0 File Offset: 0x00100FA0
	public ushort GetSyncFlagsReplicated(ushort syncFlags)
	{
		return syncFlags & 49159;
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x00102DAC File Offset: 0x00100FAC
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		if (_version < 26)
		{
			Log.Warning("Vehicle: Ignoring old data v{0}", new object[]
			{
				_version
			});
			return;
		}
		ushort syncFlags = _br.ReadUInt16();
		this.ReadSyncData(_br, syncFlags, 0);
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x00102DF0 File Offset: 0x00100FF0
	public void ReadSyncData(BinaryReader _br, ushort syncFlags, int senderId)
	{
		this.isReadingFromRemote = (senderId != GameManager.Instance.World.GetPrimaryPlayerId());
		byte b = _br.ReadByte();
		if ((syncFlags & 32768) > 0)
		{
			this.incomingRemoteData.Flags = _br.ReadInt32();
			this.incomingRemoteData.Flags = (this.incomingRemoteData.Flags | 1);
			this.incomingRemoteData.MotorTorquePercent = (float)_br.ReadInt16() * 0.0001f;
			this.incomingRemoteData.SteeringPercent = (float)_br.ReadInt16() * 0.0001f;
			this.incomingRemoteData.Velocity = StreamUtils.ReadVector3(_br);
			List<EntityVehicle.RemoteData.Part> list = new List<EntityVehicle.RemoteData.Part>(4);
			this.incomingRemoteData.parts = list;
			for (;;)
			{
				byte b2 = _br.ReadByte();
				if (b2 == 0)
				{
					break;
				}
				EntityVehicle.RemoteData.Part item;
				if (b2 == 2)
				{
					item.pos = StreamUtils.ReadVector3(_br);
				}
				else
				{
					item.pos = Vector3.zero;
				}
				item.rot = StreamUtils.ReadQuaterion(_br);
				list.Add(item);
			}
		}
		if ((syncFlags & 16384) > 0)
		{
			this.IsHeadlightOn = _br.ReadBoolean();
		}
		if ((syncFlags & 1) > 0)
		{
			this.delayedAttachments.Clear();
			int num = (int)_br.ReadByte();
			for (int i = 0; i < num; i++)
			{
				int num2 = _br.ReadInt32();
				if (num2 != -1)
				{
					EntityVehicle.DelayedAttach item2;
					item2.entityId = num2;
					item2.slot = i;
					this.delayedAttachments.Add(item2);
				}
				else
				{
					Entity attached = base.GetAttached(i);
					if (attached)
					{
						attached.Detach();
					}
				}
			}
		}
		if ((syncFlags & 2) > 0)
		{
			byte b3 = _br.ReadByte();
			this.isInteractionLocked = ((b3 & 1) > 0);
			this.isLocked = ((b3 & 2) > 0);
			this.vehicle.OwnerId = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			if (b > 1)
			{
				this.vehicle.PasswordHash = _br.ReadString();
			}
			else
			{
				this.vehicle.PasswordHash = _br.ReadInt32().ToString();
			}
			this.vehicle.AllowedUsers.Clear();
			int num3 = (int)_br.ReadByte();
			for (int j = 0; j < num3; j++)
			{
				this.vehicle.AllowedUsers.Add(PlatformUserIdentifierAbs.FromStream(_br, false, false));
			}
		}
		if ((syncFlags & 4) > 0)
		{
			int num4 = (int)_br.ReadByte();
			ItemStack[] array = new ItemStack[num4];
			for (int k = 0; k < num4; k++)
			{
				ItemStack itemStack = new ItemStack();
				itemStack.Read(_br);
				array[k] = itemStack;
			}
			this.vehicle.LoadItems(array);
		}
		if ((syncFlags & 8) > 0)
		{
			if (b >= 3)
			{
				this.bag = Bag.Read(_br);
			}
			else
			{
				int num5 = (int)_br.ReadByte();
				ItemStack[] array2 = new ItemStack[num5];
				for (int l = 0; l < num5; l++)
				{
					ItemStack itemStack2 = new ItemStack();
					array2[l] = itemStack2.Read(_br);
				}
				this.bag.SetSlots(array2);
				if (b >= 1)
				{
					if (_br.ReadBoolean())
					{
						this.bag.LockedSlots = new PackedBoolArray(0);
						this.bag.LockedSlots.Read(_br);
					}
					else
					{
						this.bag.LockedSlots = new PackedBoolArray(this.bag.SlotCount);
					}
				}
			}
		}
		this.isReadingFromRemote = false;
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x00103118 File Offset: 0x00101318
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		ushort num = _bNetworkWrite ? 16399 : 16398;
		_bw.Write(num);
		this.WriteSyncData(_bw, num);
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x0010314C File Offset: 0x0010134C
	public void WriteSyncData(BinaryWriter _bw, ushort syncFlags)
	{
		_bw.Write(3);
		if ((syncFlags & 32768) > 0)
		{
			int num = 0;
			if (this.vehicle.CurrentIsAccel)
			{
				num |= 2;
			}
			if (this.vehicle.CurrentIsBreak)
			{
				num |= 4;
			}
			_bw.Write(num);
			_bw.Write((short)(this.vehicle.CurrentMotorTorquePercent * 10000f));
			_bw.Write((short)(this.vehicle.CurrentSteeringPercent * 10000f));
			StreamUtils.Write(_bw, this.vehicle.CurrentVelocity);
			int num2 = this.wheels.Length;
			for (int i = 0; i < num2; i++)
			{
				EntityVehicle.Wheel wheel = this.wheels[i];
				if (wheel.steerT && wheel.isSteerParentOfTire)
				{
					_bw.Write(1);
					StreamUtils.Write(_bw, wheel.steerT.localRotation);
				}
				if (wheel.tireT)
				{
					_bw.Write(2);
					StreamUtils.Write(_bw, wheel.tireT.localPosition);
					StreamUtils.Write(_bw, wheel.tireT.localRotation);
				}
			}
			_bw.Write(0);
		}
		if ((syncFlags & 16384) > 0)
		{
			_bw.Write(this.IsHeadlightOn);
		}
		if ((syncFlags & 1) > 0)
		{
			int attachMaxCount = base.GetAttachMaxCount();
			_bw.Write((byte)attachMaxCount);
			for (int j = 0; j < attachMaxCount; j++)
			{
				Entity attached = base.GetAttached(j);
				_bw.Write(attached ? attached.entityId : -1);
			}
		}
		if ((syncFlags & 2) > 0)
		{
			byte b = 0;
			if (this.isInteractionLocked)
			{
				b |= 1;
			}
			if (this.isLocked)
			{
				b |= 2;
			}
			_bw.Write(b);
			this.vehicle.OwnerId.ToStream(_bw, false);
			_bw.Write(this.vehicle.PasswordHash);
			_bw.Write((byte)this.vehicle.AllowedUsers.Count);
			for (int k = 0; k < this.vehicle.AllowedUsers.Count; k++)
			{
				this.vehicle.AllowedUsers[k].ToStream(_bw, false);
			}
		}
		if ((syncFlags & 4) > 0)
		{
			_bw.Write(1);
			this.vehicle.GetItems()[0].Write(_bw);
		}
		if ((syncFlags & 8) > 0)
		{
			this.bag.Write(_bw);
		}
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x00103398 File Offset: 0x00101598
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isDriveable()
	{
		return this.vehicle.IsDriveable();
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x001033A5 File Offset: 0x001015A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasStorage()
	{
		return this.vehicle.HasStorage();
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x001033B2 File Offset: 0x001015B2
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasHandlebars()
	{
		return this.vehicle.HasSteering();
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool HasChassis()
	{
		return true;
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x001033BF File Offset: 0x001015BF
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool needsFuel()
	{
		return this.vehicle.HasEnginePart() && this.vehicle.GetFuelPercent() < 1f;
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x001033E4 File Offset: 0x001015E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasGasCan(EntityAlive _ea)
	{
		string fuelItem = this.GetVehicle().GetFuelItem();
		if (fuelItem == "")
		{
			return false;
		}
		ItemValue item = ItemClass.GetItem(fuelItem, false);
		int num = 0;
		ItemStack[] slots = _ea.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].itemValue.type == item.type)
			{
				num++;
			}
		}
		for (int j = 0; j < _ea.inventory.PUBLIC_SLOTS; j++)
		{
			if (_ea.inventory.GetItem(j).itemValue.type == item.type)
			{
				num++;
			}
		}
		return num > 0;
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasLock()
	{
		return true;
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x0010348F File Offset: 0x0010168F
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isAllowedUser(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.vehicle.AllowedUsers.Contains(_userIdentifier);
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void PlayStepSound(string stepSound, float _volume)
	{
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTasks()
	{
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool canDespawn()
	{
		return false;
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x000E4BB2 File Offset: 0x000E2DB2
	public string GetHashForPassword(string _password)
	{
		return Utils.HashString(_password);
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x001034A4 File Offset: 0x001016A4
	public bool SetPasswordHash(string _passwordHash, PlatformUserIdentifierAbs _userIdentifier)
	{
		if (this.LocalPlayerIsOwner() && _passwordHash != null)
		{
			if (_passwordHash != this.vehicle.PasswordHash)
			{
				this.vehicle.PasswordHash = _passwordHash;
				this.vehicle.AllowedUsers.Clear();
				if (this.vehicle.OwnerId == null)
				{
					this.SetOwner(_userIdentifier);
					this.isLocked = true;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x0010350C File Offset: 0x0010170C
	public bool CheckPasswordHash(string _passwordHash, PlatformUserIdentifierAbs _userIdentifier)
	{
		if (this.LocalPlayerIsOwner() || !this.HasPassword())
		{
			this.SendSyncData(2);
			return true;
		}
		if (_passwordHash == this.vehicle.PasswordHash)
		{
			this.vehicle.AllowedUsers.Add(_userIdentifier);
			this.SendSyncData(2);
			return true;
		}
		return false;
	}

	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x06002949 RID: 10569 RVA: 0x000E4AFC File Offset: 0x000E2CFC
	// (set) Token: 0x0600294A RID: 10570 RVA: 0x000027FC File Offset: 0x000009FC
	public int EntityId
	{
		get
		{
			return this.entityId;
		}
		set
		{
		}
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x00103560 File Offset: 0x00101760
	public bool IsLocked()
	{
		return this.isLocked;
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x00103568 File Offset: 0x00101768
	public void SetLocked(bool _isLocked)
	{
		this.isLocked = _isLocked;
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x00103571 File Offset: 0x00101771
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.vehicle.OwnerId;
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x0010357E File Offset: 0x0010177E
	public override void OnAddedToWorld()
	{
		this.bSpawned = true;
		this.HandleNavObject();
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x0010358D File Offset: 0x0010178D
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.vehicle.OwnerId = _userIdentifier;
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x0010359B File Offset: 0x0010179B
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.LocalPlayerIsOwner() || this.vehicle.AllowedUsers.Contains(_userIdentifier);
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000E4B59 File Offset: 0x000E2D59
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return new List<PlatformUserIdentifierAbs>();
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x001035B8 File Offset: 0x001017B8
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x001035C5 File Offset: 0x001017C5
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.vehicle.OwnerId == null || this.vehicle.OwnerId.Equals(_userIdentifier);
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x001035E7 File Offset: 0x001017E7
	public bool HasPassword()
	{
		return !string.IsNullOrEmpty(this.vehicle.PasswordHash);
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x001035FC File Offset: 0x001017FC
	public string GetPasswordHash()
	{
		return this.vehicle.PasswordHash;
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x0010360C File Offset: 0x0010180C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckForOutOfWorld()
	{
		if (this.bDead)
		{
			return;
		}
		Vector3 vector = this.position;
		if (this.world.AdjustBoundsForPlayers(ref vector, 0.2f))
		{
			if (!this.vehicleRB.isKinematic)
			{
				Vector3 velocity = this.vehicleRB.velocity;
				velocity.x *= -0.5f;
				velocity.z *= -0.5f;
				this.vehicleRB.velocity = velocity;
			}
			vector.y = this.vehicleRB.position.y + Origin.position.y;
			this.SetPosition(vector, true);
			EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
			if (attachedPlayerLocal)
			{
				GameManager.ShowTooltip(attachedPlayerLocal, Localization.Get("ttWorldEnd", false, null), false, false, 0f);
			}
			return;
		}
		Vector3 centerPosition = this.GetCenterPosition();
		Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos((int)centerPosition.x, (int)centerPosition.y, (int)centerPosition.z);
		if (chunk == null || !chunk.IsCollisionMeshGenerated || !chunk.IsDisplayed)
		{
			if (!this.vehicleRB.isKinematic)
			{
				this.vehicleRB.velocity = Vector3.zero;
				this.vehicleRB.angularVelocity = Vector3.zero;
			}
			if (!this.hasDriver)
			{
				this.RBActive = false;
				this.isTryToFall = true;
			}
			return;
		}
		Entity firstAttached = base.GetFirstAttached();
		if (firstAttached && !firstAttached.IsSpawned())
		{
			return;
		}
		if (this.RBActive && !this.IsTerrainBelow(centerPosition))
		{
			int num = this.worldTerrainFailCount + 1;
			this.worldTerrainFailCount = num;
			if (num <= 6)
			{
				if (this.worldTerrainFailCount == 2)
				{
					chunk.NeedsRegeneration = true;
					this.LogVehicle("{0}, {1}, center {2}, rbPos {3}, in ground. Chunk regen {4}", new object[]
					{
						base.transform.parent.name,
						vector.ToCultureInvariantString(),
						centerPosition.ToCultureInvariantString(),
						(this.vehicleRB.position + Origin.position).ToCultureInvariantString(),
						chunk
					});
				}
			}
			else if (this.hasWorldValidPos)
			{
				Vector3 vector2 = this.worldValidPos - vector;
				if (vector2.y < 0f)
				{
					vector2.y = 0f;
				}
				float sqrMagnitude = vector2.sqrMagnitude;
				vector2 = vector2.normalized;
				if (sqrMagnitude > 0.122499995f)
				{
					vector = this.worldValidPos + vector2 * 0.1f;
					this.SetPosition(vector, true);
				}
				if (!this.vehicleRB.isKinematic)
				{
					Vector3 vector3 = this.vehicleRB.velocity;
					if (Vector3.Dot(vector3, vector2) < 0f)
					{
						vector3 *= -0.5f;
					}
					vector3.y = 1f + this.rand.RandomFloat * 2f;
					vector3 += vector2 * 3f;
					this.vehicleRB.velocity = vector3;
					this.vehicleRB.angularVelocity = Vector3.zero;
				}
				this.LogVehicle("{0}, {1}, center {2} in ground. back {3}", new object[]
				{
					base.transform.parent.name,
					vector.ToCultureInvariantString(),
					centerPosition.ToCultureInvariantString(),
					this.worldValidPos.ToCultureInvariantString()
				});
				this.worldValidPos.x = this.worldValidPos.x + (this.rand.RandomFloat - 0.5f) * 2f * 0.05f;
				this.worldValidPos.z = this.worldValidPos.z + (this.rand.RandomFloat - 0.5f) * 2f * 0.05f;
				this.worldValidPos.y = this.worldValidPos.y + 0.001f;
				this.worldValidDelay -= Time.deltaTime;
				if (this.worldValidDelay <= 0f)
				{
					this.worldValidDelay = 1f;
					this.worldValidPos.y = this.worldValidPos.y + 1.2f;
				}
			}
			else
			{
				Vector3 pos = centerPosition;
				pos.y = 257f;
				bool flag = this.IsTerrainBelow(pos);
				if (flag)
				{
					vector.y += 3f;
					this.SetPosition(vector, true);
				}
				this.LogVehicle("{0}, {1}, center {2} (vel {3}, {4}) {5}", new object[]
				{
					base.transform.parent.name,
					vector.ToCultureInvariantString(),
					centerPosition.ToCultureInvariantString(),
					this.vehicleRB.velocity,
					this.vehicleRB.angularVelocity,
					flag ? " in ground. up" : " out of world"
				});
				if (!this.vehicleRB.isKinematic)
				{
					this.vehicleRB.velocity *= 0.5f;
					this.vehicleRB.angularVelocity *= 0.5f;
				}
			}
		}
		else
		{
			this.worldTerrainFailCount = 0;
			if (this.hasWorldValidPos)
			{
				if ((this.worldValidPos - vector).sqrMagnitude > 4f)
				{
					this.worldValidPos = vector;
				}
			}
			else
			{
				this.hasWorldValidPos = true;
				this.worldValidPos = vector;
			}
		}
		if (this.isTryToFall)
		{
			this.isTryToFall = false;
			this.RBActive = true;
			this.vehicleRB.WakeUp();
		}
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x00103B50 File Offset: 0x00101D50
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsTerrainBelow(Vector3 pos)
	{
		Ray ray = new Ray(pos - Origin.position, Vector3.down);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 3.4028235E+38f, 1073807360))
		{
			return true;
		}
		Utils.DrawCircleLinesHorzontal(ray.origin, 0.25f, new Color(1f, 1f, 0f), new Color(1f, 0f, 0f), 8, 5f);
		Utils.DrawLine(ray.origin, new Vector3(ray.origin.x, 0f - Origin.position.y, ray.origin.z), new Color(1f, 1f, 0f), new Color(1f, 0f, 0f), 5, 5f);
		ray.origin += new Vector3(0.02f, 0.5f, 0.03f);
		return Physics.SphereCast(ray, 0.1f, out raycastHit, float.MaxValue, 1073807360);
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsDeadIfOutOfWorld()
	{
		return false;
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x00103C74 File Offset: 0x00101E74
	public override void CheckPosition()
	{
		base.CheckPosition();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.Spawned && !this.hasDriver)
		{
			Vector3i vector3i;
			Vector3i vector3i2;
			this.world.GetWorldExtent(out vector3i, out vector3i2);
			if (this.position.y < (float)vector3i.y)
			{
				Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos(new Vector3i((int)this.position.x, (int)this.position.y, (int)this.position.z));
				if (chunk != null && chunk.IsCollisionMeshGenerated && chunk.IsDisplayed)
				{
					this.TeleportToWithinBounds(vector3i.ToVector3(), vector3i2.ToVector3());
				}
			}
		}
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x00103D38 File Offset: 0x00101F38
	[PublicizedFrom(EAccessModifier.Private)]
	public void TeleportToWithinBounds(Vector3 _min, Vector3 _max)
	{
		_min.x += 66f;
		_min.z += 66f;
		_max.x -= 66f;
		_max.z -= 66f;
		Vector3 position = this.position;
		if (position.x < _min.x)
		{
			position.x = _min.x;
		}
		else if (position.x > _max.x)
		{
			position.x = _max.x;
		}
		if (position.z < _min.z)
		{
			position.z = _min.z;
		}
		else if (position.z > _max.z)
		{
			position.z = _max.z;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(new Vector3(position.x, 999f, position.z) - Origin.position, Vector3.down), out raycastHit, 3.4028235E+38f, 1076428800))
		{
			position.y = raycastHit.point.y + Origin.position.y + 1f;
			this.SetPosition(position, true);
			Log.Out("Vehicle out of world. Teleporting to " + position.ToCultureInvariantString());
		}
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x00103E7C File Offset: 0x0010207C
	public void Kill()
	{
		LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
		if (this.LocalPlayerIsOwner() && uiforPrimaryPlayer != null)
		{
			XUiC_BagStorageWindowGroup childByType = uiforPrimaryPlayer.xui.FindWindowGroupByName(XUiC_BagStorageWindowGroup.ID).GetChildByType<XUiC_BagStorageWindowGroup>();
			if (childByType != null && childByType.Entity == this)
			{
				uiforPrimaryPlayer.windowManager.Close(childByType.WindowGroup, false);
			}
		}
		int attachMaxCount = base.GetAttachMaxCount();
		for (int i = 0; i < attachMaxCount; i++)
		{
			Entity attached = base.GetAttached(i);
			if (attached != null)
			{
				attached.Detach();
			}
		}
		this.timeStayAfterDeath = 0;
		this.SetDead();
		this.MarkToUnload();
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x00103F1C File Offset: 0x0010211C
	public override void OnEntityUnload()
	{
		if (this.vehicleRB)
		{
			this.position = this.vehicleRB.position + Origin.position;
			this.rotation = this.vehicleRB.rotation.eulerAngles;
		}
		base.OnEntityUnload();
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x00103F70 File Offset: 0x00102170
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		if (EntityClass.list[this.entityClass].NavObject != "")
		{
			if (this.LocalPlayerIsOwner())
			{
				this.NavObject = NavObjectManager.Instance.RegisterNavObject(EntityClass.list[this.entityClass].NavObject, this.vehicle.GetMeshTransform(), "", false);
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer != null)
				{
					primaryPlayer.Waypoints.UpdateEntityVehicleWayPoint(this, false);
					return;
				}
			}
			else if (this.NavObject != null)
			{
				NavObjectManager.Instance.UnRegisterNavObject(this.NavObject);
				this.NavObject = null;
			}
		}
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x00104025 File Offset: 0x00102225
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogVehicle(string format, params object[] args)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || base.GetAttachedPlayerLocal())
		{
			format = string.Format("{0} Vehicle {1}", GameManager.frameCount, format);
			Log.Out(format, args);
		}
	}

	// Token: 0x04001EA4 RID: 7844
	public static readonly FastTags<TagGroup.Global> StorageModifierTags = FastTags<TagGroup.Global>.Parse("storage");

	// Token: 0x04001EA5 RID: 7845
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockScale = 0.058333334f;

	// Token: 0x04001EA6 RID: 7846
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockVelReduction = 1.5f;

	// Token: 0x04001EA7 RID: 7847
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockMin = 5f;

	// Token: 0x04001EA8 RID: 7848
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockSelfPer = 2.5f;

	// Token: 0x04001EA9 RID: 7849
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageTerrainSelfPer = 0.1f;

	// Token: 0x04001EAA RID: 7850
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageEntityScale = 12f;

	// Token: 0x04001EAB RID: 7851
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageEntitySelfScale = 28f;

	// Token: 0x04001EAC RID: 7852
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cKillEntityXPPer = 0.5f;

	// Token: 0x04001EAD RID: 7853
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cExitVelScale = 0.5f;

	// Token: 0x04001EAE RID: 7854
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSleepTime = 3f;

	// Token: 0x04001EAF RID: 7855
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cGravity = -9.81f;

	// Token: 0x04001EB0 RID: 7856
	public bool IsEngineRunning;

	// Token: 0x04001EB1 RID: 7857
	public bool isLocked;

	// Token: 0x04001EB2 RID: 7858
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInteractionLocked;

	// Token: 0x04001EB3 RID: 7859
	public Vehicle vehicle;

	// Token: 0x04001EB4 RID: 7860
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isTryToFall;

	// Token: 0x04001EB5 RID: 7861
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasDriver;

	// Token: 0x04001EB6 RID: 7862
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeInWater;

	// Token: 0x04001EB7 RID: 7863
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isReadingFromRemote;

	// Token: 0x04001EB8 RID: 7864
	public static float VehicleFuelUsageModifier = 1f;

	// Token: 0x04001EB9 RID: 7865
	public static float VehicleEntityDamageModifier = 1f;

	// Token: 0x04001EBA RID: 7866
	public static float VehicleBlockDamageModifier = 1f;

	// Token: 0x04001EBB RID: 7867
	public static float VehicleSelfDamageModifier = 1f;

	// Token: 0x04001EBC RID: 7868
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public MovementInput movementInput;

	// Token: 0x04001EBD RID: 7869
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool isTurnTowardsLook = true;

	// Token: 0x04001EBE RID: 7870
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityVehicle.RemoteData incomingRemoteData;

	// Token: 0x04001EBF RID: 7871
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityVehicle.RemoteData currentRemoteData;

	// Token: 0x04001EC0 RID: 7872
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityVehicle.RemoteData lastRemoteData;

	// Token: 0x04001EC1 RID: 7873
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSyncHighRateDuration = 0.5f;

	// Token: 0x04001EC2 RID: 7874
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float syncHighRateTime;

	// Token: 0x04001EC3 RID: 7875
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float syncPlayTime = -1f;

	// Token: 0x04001EC4 RID: 7876
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float syncLowRateTime;

	// Token: 0x04001EC5 RID: 7877
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSyncLowRateDuration = 2f;

	// Token: 0x04001EC6 RID: 7878
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody vehicleRB;

	// Token: 0x04001EC7 RID: 7879
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool RBActive;

	// Token: 0x04001EC8 RID: 7880
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float RBNoDriverGndTime;

	// Token: 0x04001EC9 RID: 7881
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float RBNoDriverSleepTime;

	// Token: 0x04001ECA RID: 7882
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastRBPos;

	// Token: 0x04001ECB RID: 7883
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion lastRBRot;

	// Token: 0x04001ECC RID: 7884
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastRBVel;

	// Token: 0x04001ECD RID: 7885
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastRBAngVel;

	// Token: 0x04001ECE RID: 7886
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float velocityMax;

	// Token: 0x04001ECF RID: 7887
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float damageAccumulator;

	// Token: 0x04001ED0 RID: 7888
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int hitEffectCount;

	// Token: 0x04001ED1 RID: 7889
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float explodeHealth;

	// Token: 0x04001ED2 RID: 7890
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canHop;

	// Token: 0x04001ED3 RID: 7891
	public const float cVehicleCameraOffset = 1.8f;

	// Token: 0x04001ED4 RID: 7892
	public const float cVehicleCameraChaseSpeed = 7f;

	// Token: 0x04001ED5 RID: 7893
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 cameraStartPos;

	// Token: 0x04001ED6 RID: 7894
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraStartBlend;

	// Token: 0x04001ED7 RID: 7895
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 cameraStartVec;

	// Token: 0x04001ED8 RID: 7896
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 cameraPos;

	// Token: 0x04001ED9 RID: 7897
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraDist;

	// Token: 0x04001EDA RID: 7898
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float cameraDistScale = 1f;

	// Token: 0x04001EDB RID: 7899
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraAngle;

	// Token: 0x04001EDC RID: 7900
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraAngleTarget;

	// Token: 0x04001EDD RID: 7901
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraOutTime;

	// Token: 0x04001EDE RID: 7902
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraVelY;

	// Token: 0x04001EDF RID: 7903
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastHonkEventTime;

	// Token: 0x04001EE0 RID: 7904
	public TraderDoorController HornActivation;

	// Token: 0x04001EE1 RID: 7905
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVehicle.Force[] forces = new EntityVehicle.Force[0];

	// Token: 0x04001EE2 RID: 7906
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVehicle.Motor[] motors = new EntityVehicle.Motor[0];

	// Token: 0x04001EE3 RID: 7907
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float motorTorque;

	// Token: 0x04001EE4 RID: 7908
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float brakeTorque;

	// Token: 0x04001EE5 RID: 7909
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wheelDir;

	// Token: 0x04001EE6 RID: 7910
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wheelMotor;

	// Token: 0x04001EE7 RID: 7911
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wheelBrakes;

	// Token: 0x04001EE8 RID: 7912
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVehicle.Wheel[] wheels = new EntityVehicle.Wheel[0];

	// Token: 0x04001EE9 RID: 7913
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int storageModCount;

	// Token: 0x04001EEA RID: 7914
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<EntityVehicle.DelayedAttach> delayedAttachments = new List<EntityVehicle.DelayedAttach>();

	// Token: 0x04001EEB RID: 7915
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float collisionBlockDamage;

	// Token: 0x04001EEC RID: 7916
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 collisionVelNorm;

	// Token: 0x04001EED RID: 7917
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int collisionIgnoreCount;

	// Token: 0x04001EEE RID: 7918
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<ContactPoint> contactPoints = new List<ContactPoint>();

	// Token: 0x04001EEF RID: 7919
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<WorldRayHitInfo> collisionHits = new List<WorldRayHitInfo>();

	// Token: 0x04001EF0 RID: 7920
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int collisionGrazeCount;

	// Token: 0x04001EF1 RID: 7921
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cFuelItemScale = 25f;

	// Token: 0x04001EF2 RID: 7922
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncVersion = 3;

	// Token: 0x04001EF3 RID: 7923
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncAttachment = 1;

	// Token: 0x04001EF4 RID: 7924
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncInteractAndSecurity = 2;

	// Token: 0x04001EF5 RID: 7925
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncItem = 4;

	// Token: 0x04001EF6 RID: 7926
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncStorage = 8;

	// Token: 0x04001EF7 RID: 7927
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncLowRate = 16384;

	// Token: 0x04001EF8 RID: 7928
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncHighRate = 32768;

	// Token: 0x04001EF9 RID: 7929
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncAllNonRates = 15;

	// Token: 0x04001EFA RID: 7930
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncLowRateAndNonRates = 16399;

	// Token: 0x04001EFB RID: 7931
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncReplicate = 49159;

	// Token: 0x04001EFC RID: 7932
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncSave = 16398;

	// Token: 0x04001EFD RID: 7933
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncInteractAndSecurityFInteracting = 1;

	// Token: 0x04001EFE RID: 7934
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncInteractAndSecurityFLocked = 2;

	// Token: 0x04001EFF RID: 7935
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cWorldPad = 66;

	// Token: 0x04001F00 RID: 7936
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasWorldValidPos;

	// Token: 0x04001F01 RID: 7937
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 worldValidPos;

	// Token: 0x04001F02 RID: 7938
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float worldValidDelay;

	// Token: 0x04001F03 RID: 7939
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int worldTerrainFailCount;

	// Token: 0x020004E4 RID: 1252
	[PublicizedFrom(EAccessModifier.Private)]
	public struct RemoteData
	{
		// Token: 0x04001F04 RID: 7940
		public const int cFHasData = 1;

		// Token: 0x04001F05 RID: 7941
		public const int cFAccel = 2;

		// Token: 0x04001F06 RID: 7942
		public const int cFBreak = 4;

		// Token: 0x04001F07 RID: 7943
		public int Flags;

		// Token: 0x04001F08 RID: 7944
		public float MotorTorquePercent;

		// Token: 0x04001F09 RID: 7945
		public float SteeringPercent;

		// Token: 0x04001F0A RID: 7946
		public Vector3 Velocity;

		// Token: 0x04001F0B RID: 7947
		public List<EntityVehicle.RemoteData.Part> parts;

		// Token: 0x020004E5 RID: 1253
		public struct Part
		{
			// Token: 0x04001F0C RID: 7948
			public Vector3 pos;

			// Token: 0x04001F0D RID: 7949
			public Quaternion rot;
		}
	}

	// Token: 0x020004E6 RID: 1254
	[PublicizedFrom(EAccessModifier.Protected)]
	public class Force
	{
		// Token: 0x04001F0E RID: 7950
		public Vector2 ceiling;

		// Token: 0x04001F0F RID: 7951
		public Vector3 force;

		// Token: 0x04001F10 RID: 7952
		public EntityVehicle.Force.Trigger trigger;

		// Token: 0x04001F11 RID: 7953
		public EntityVehicle.Force.Type type;

		// Token: 0x020004E7 RID: 1255
		public enum Trigger
		{
			// Token: 0x04001F13 RID: 7955
			Off,
			// Token: 0x04001F14 RID: 7956
			On,
			// Token: 0x04001F15 RID: 7957
			InputForward,
			// Token: 0x04001F16 RID: 7958
			InputStrafe,
			// Token: 0x04001F17 RID: 7959
			InputUp,
			// Token: 0x04001F18 RID: 7960
			InputDown,
			// Token: 0x04001F19 RID: 7961
			Motor0,
			// Token: 0x04001F1A RID: 7962
			Motor1,
			// Token: 0x04001F1B RID: 7963
			Motor2,
			// Token: 0x04001F1C RID: 7964
			Motor3,
			// Token: 0x04001F1D RID: 7965
			Motor4,
			// Token: 0x04001F1E RID: 7966
			Motor5,
			// Token: 0x04001F1F RID: 7967
			Motor6,
			// Token: 0x04001F20 RID: 7968
			Motor7
		}

		// Token: 0x020004E8 RID: 1256
		public enum Type
		{
			// Token: 0x04001F22 RID: 7970
			Relative,
			// Token: 0x04001F23 RID: 7971
			RelativeTorque
		}
	}

	// Token: 0x020004E9 RID: 1257
	[PublicizedFrom(EAccessModifier.Protected)]
	public class Motor
	{
		// Token: 0x04001F24 RID: 7972
		public VPEngine engine;

		// Token: 0x04001F25 RID: 7973
		public float engineOffPer;

		// Token: 0x04001F26 RID: 7974
		public float turbo;

		// Token: 0x04001F27 RID: 7975
		public float rpm;

		// Token: 0x04001F28 RID: 7976
		public float rpmAccelMin;

		// Token: 0x04001F29 RID: 7977
		public float rpmAccelMax;

		// Token: 0x04001F2A RID: 7978
		public float rpmDrag;

		// Token: 0x04001F2B RID: 7979
		public float rpmMax;

		// Token: 0x04001F2C RID: 7980
		public EntityVehicle.Motor.Trigger trigger;

		// Token: 0x04001F2D RID: 7981
		public EntityVehicle.Motor.Type type;

		// Token: 0x04001F2E RID: 7982
		public Transform transform;

		// Token: 0x04001F2F RID: 7983
		public int axis;

		// Token: 0x020004EA RID: 1258
		public enum Trigger
		{
			// Token: 0x04001F31 RID: 7985
			Off,
			// Token: 0x04001F32 RID: 7986
			On,
			// Token: 0x04001F33 RID: 7987
			InputForward,
			// Token: 0x04001F34 RID: 7988
			InputStrafe,
			// Token: 0x04001F35 RID: 7989
			InputUp,
			// Token: 0x04001F36 RID: 7990
			InputDown,
			// Token: 0x04001F37 RID: 7991
			Vel
		}

		// Token: 0x020004EB RID: 1259
		public enum Type
		{
			// Token: 0x04001F39 RID: 7993
			Spin,
			// Token: 0x04001F3A RID: 7994
			Relative,
			// Token: 0x04001F3B RID: 7995
			RelativeTorque
		}
	}

	// Token: 0x020004EC RID: 1260
	[PublicizedFrom(EAccessModifier.Protected)]
	public class Wheel
	{
		// Token: 0x04001F3C RID: 7996
		public float motorTorqueScale;

		// Token: 0x04001F3D RID: 7997
		public float brakeTorqueScale;

		// Token: 0x04001F3E RID: 7998
		public string bounceSound;

		// Token: 0x04001F3F RID: 7999
		public string slideSound;

		// Token: 0x04001F40 RID: 8000
		public bool isSteerParentOfTire;

		// Token: 0x04001F41 RID: 8001
		public Transform steerT;

		// Token: 0x04001F42 RID: 8002
		public Quaternion steerBaseRot;

		// Token: 0x04001F43 RID: 8003
		public Transform tireT;

		// Token: 0x04001F44 RID: 8004
		public float tireSpinSpeed;

		// Token: 0x04001F45 RID: 8005
		public float tireSpin;

		// Token: 0x04001F46 RID: 8006
		public float tireSuspensionPercent;

		// Token: 0x04001F47 RID: 8007
		public WheelCollider wheelC;

		// Token: 0x04001F48 RID: 8008
		public WheelFrictionCurve forwardFriction;

		// Token: 0x04001F49 RID: 8009
		public float forwardStiffnessBase;

		// Token: 0x04001F4A RID: 8010
		public WheelFrictionCurve sideFriction;

		// Token: 0x04001F4B RID: 8011
		public float sideStiffnessBase;

		// Token: 0x04001F4C RID: 8012
		public float slideTime;

		// Token: 0x04001F4D RID: 8013
		public float ptlTime;

		// Token: 0x04001F4E RID: 8014
		public bool isGrounded;
	}

	// Token: 0x020004ED RID: 1261
	public class VehicleInventory : Inventory
	{
		// Token: 0x06002964 RID: 10596 RVA: 0x00104118 File Offset: 0x00102318
		public VehicleInventory(IGameManager _gameManager, EntityAlive _entity) : base(_gameManager, _entity)
		{
			this.cSlotCount = base.PUBLIC_SLOTS + 1;
			this.SetupSlots();
		}

		// Token: 0x06002965 RID: 10597 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Execute(int _actionIdx, bool _bReleased, PlayerActionsLocal _playerActions = null)
		{
		}

		// Token: 0x06002966 RID: 10598 RVA: 0x00104136 File Offset: 0x00102336
		public void SetupSlots()
		{
			this.slots = new ItemInventoryData[this.cSlotCount];
			this.models = new Transform[this.cSlotCount];
			this.m_HoldingItemIdx = 0;
			base.Clear();
		}

		// Token: 0x06002967 RID: 10599 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void updateHoldingItem()
		{
		}

		// Token: 0x04001F4F RID: 8015
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int cSlotCount;
	}

	// Token: 0x020004EE RID: 1262
	public struct DelayedAttach
	{
		// Token: 0x04001F50 RID: 8016
		public int entityId;

		// Token: 0x04001F51 RID: 8017
		public int slot;
	}
}
