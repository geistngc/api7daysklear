using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004D7 RID: 1239
[Preserve]
public class EntitySupplyCrate : EntityAlive
{
	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x0600283E RID: 10302 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsValidAimAssistSnapTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x000FA449 File Offset: 0x000F8649
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.hasAI = false;
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x000FA458 File Offset: 0x000F8658
	public override void PostInit()
	{
		base.PostInit();
		this.ValidateResources();
		base.gameObject.layer = 21;
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
		if (this.wasOnGround)
		{
			this.StopSmokeAndLights();
			if (this.parachuteT)
			{
				this.parachuteT.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x000FA4C7 File Offset: 0x000F86C7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitLocalActivationCommands(Action<EntityActivationCommand> _addCallback)
	{
		_addCallback(new EntityActivationCommand("search", "search", null, null));
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x000FA4E0 File Offset: 0x000F86E0
	public override bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_commandName, "search"))
		{
			return this.bag != null && !this.IsDead();
		}
		return base.AllowActivationCommand(_commandName, _playerFocusing);
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x000FA50C File Offset: 0x000F870C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityActivated(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_command.commandId, "search"))
		{
			LockManager.Instance.LockRequestLocal(this, new Entity.EntityLockContext(_command.commandId.ToString(), this.bag), 0);
		}
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x000FA548 File Offset: 0x000F8748
	public override string GetActivationText()
	{
		if (this.bag == null)
		{
			return string.Empty;
		}
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
		PlayerActionsLocal playerInput = entityPlayerLocal2.playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string arg2 = Localization.Get(this.LocalizedEntityName, false, null);
		if (!this.bag.Touched)
		{
			return string.Format(Localization.Get("lootTooltipNew", false, null), arg, arg2);
		}
		if (this.bag.IsEmpty())
		{
			return string.Format(Localization.Get("lootTooltipEmpty", false, null), arg, arg2);
		}
		return string.Format(Localization.Get("lootTooltipTouched", false, null), arg, arg2);
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x000FA624 File Offset: 0x000F8824
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		if (GameStats.GetBool(EnumGameStats.AirDropMarker))
		{
			NavObjectManager.Instance.UnRegisterNavObjectByEntityID(this.entityId);
			if (EntityClass.list[this.entityClass].NavObject != "")
			{
				this.NavObject = NavObjectManager.Instance.RegisterNavObject(EntityClass.list[this.entityClass].NavObject, this, "", false);
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				Vector3 position = this.NavObject.GetPosition() + Origin.position;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNavObject>().Setup(this.NavObject.NavObjectClass.NavObjectClassName, this.NavObject.DisplayName, position, true, this.NavObject.usingLocalizationId, this.entityId), false, -1, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x000FA711 File Offset: 0x000F8911
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.startRotY = this.rotation.y;
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x000FA72A File Offset: 0x000F892A
	public override void OnEntityUnload()
	{
		base.OnEntityUnload();
		if (this.unloadReason == EnumRemoveEntityReason.Killed && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			GameManager.Instance.World.aiDirector.GetComponent<AIDirectorAirDropComponent>().RemoveSupplyCrate(this.entityId);
		}
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000FA766 File Offset: 0x000F8966
	public override EnumMapObjectType GetMapObjectType()
	{
		return EnumMapObjectType.SupplyDrop;
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000027FC File Offset: 0x000009FC
	public override void SetMotionMultiplier(float _motionMultiplier)
	{
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x000FA76A File Offset: 0x000F896A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fallHitGround(float _v, Vector3 _fallMotion)
	{
		base.fallHitGround(Mathf.Min(_v, 5f), new Vector3(_fallMotion.x, Mathf.Max(-0.75f, _fallMotion.y), _fallMotion.z));
	}

	// Token: 0x0600284B RID: 10315 RVA: 0x000FA7A0 File Offset: 0x000F89A0
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
		base.MoveEntityHeaded(_direction, _isDirAbsolute);
		if (this.AttachedToEntity != null)
		{
			return;
		}
		if (((EModelSupplyCrate)this.emodel).parachute.gameObject.activeSelf && !base.IsInWater())
		{
			this.motion.y = this.motion.y + base.ScalePhysicsAddConstant(this.world.Gravity * 0.95f);
		}
	}

	// Token: 0x0600284C RID: 10316 RVA: 0x000FA80E File Offset: 0x000F8A0E
	public bool RequiresChunkObserver()
	{
		return !this.onGround || this.isSmokeOn;
	}

	// Token: 0x0600284D RID: 10317 RVA: 0x000FA820 File Offset: 0x000F8A20
	[PublicizedFrom(EAccessModifier.Private)]
	public void ValidateResources()
	{
		if (!this.crateT)
		{
			this.crateT = base.transform.FindInChilds("SupplyCrateEntityPrefab", false);
		}
		if (!this.parachuteT)
		{
			this.parachuteT = base.transform.FindInChilds("parachute_supplies", false);
		}
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x000FA878 File Offset: 0x000F8A78
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (this.showParachuteInTicks > 0)
		{
			this.showParachuteInTicks--;
		}
		if (this.closeParachuteInTicks > 0)
		{
			this.closeParachuteInTicks--;
		}
		if (!this.onGround && this.wasOnGround)
		{
			this.showParachuteInTicks = 10;
		}
		if (this.onGround && !this.wasOnGround)
		{
			this.closeParachuteInTicks = 10;
		}
		if ((this.onGround || base.IsInWater()) && this.closeParachuteInTicks <= 0)
		{
			((EModelSupplyCrate)this.emodel).parachute.gameObject.SetActive(false);
		}
		if (this.onGround && !this.wasOnGround)
		{
			float lightBrightness = this.world.GetLightBrightness(base.GetBlockPosition());
			GameManager.Instance.SpawnParticleEffectClient(new ParticleEffect("supply_crate_impact", base.GetPosition(), Quaternion.identity, lightBrightness, Color.white), this.entityId, false, false);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				AIDirectorAirDropComponent component = GameManager.Instance.World.aiDirector.GetComponent<AIDirectorAirDropComponent>();
				component.SetSupplyCratePosition(this.entityId, World.worldToBlockPos(this.position));
				component.RefreshCrates(-1);
			}
		}
		this.wasOnGround = this.onGround;
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x000FA9B8 File Offset: 0x000F8BB8
	public override bool CanUpdateEntity()
	{
		return this.isEntityRemote || base.CanUpdateEntity();
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000FA9CC File Offset: 0x000F8BCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		float time = Time.time;
		if (!GameManager.IsDedicatedServer)
		{
			if (this.wasOnGround && this.isSmokeOn)
			{
				if (this.smokeTimeOnGround == 0f)
				{
					this.smokeTimer = time;
				}
				this.smokeTimeOnGround = time - this.smokeTimer + 0.0001f;
				if (time > this.smokeTimer + this.smokeTimeAfterLanding)
				{
					this.StopSmokeAndLights();
				}
			}
			this.ValidateResources();
		}
		if (!this.onGround)
		{
			Vector3 vector;
			vector.x = Mathf.Sin(time) * 8f - 4f;
			vector.y = Mathf.Sin(time + 0.3f) * 8f - 4f + this.startRotY;
			vector.z = 0f;
			this.ModelTransform.localEulerAngles = vector;
			this.SetRotation(vector);
		}
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000FAAA8 File Offset: 0x000F8CA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void StopSmokeAndLights()
	{
		this.isSmokeOn = false;
		Transform modelTransform = this.emodel.GetModelTransform();
		List<Transform> list = new List<Transform>();
		GameUtils.FindTagInChilds(modelTransform, "SupplySmoke", list);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			ParticleSystem[] componentsInChildren = list[i].GetComponentsInChildren<ParticleSystem>();
			for (int j = componentsInChildren.Length - 1; j >= 0; j--)
			{
				componentsInChildren[j].main.loop = false;
			}
		}
		list.Clear();
		GameUtils.FindTagInChilds(modelTransform, "SupplyLit", list);
		for (int k = 0; k < list.Count; k++)
		{
			list[k].gameObject.SetActive(false);
		}
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool canDespawn()
	{
		return false;
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsSavedToFile()
	{
		return true;
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanCollideWithBlocks()
	{
		return false;
	}

	// Token: 0x06002856 RID: 10326 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x000FAB5C File Offset: 0x000F8D5C
	public override void OnEntityDeath()
	{
		base.OnEntityDeath();
		GameManager.Instance.World.ObjectOnMapRemove(EnumMapObjectType.SupplyDrop, this.entityId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityMapMarkerRemove>().Setup(EnumMapObjectType.SupplyDrop, this.entityId), false, -1, -1, -1, null, 192, false);
			base.DropBagServer();
		}
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanBePushed()
	{
		return false;
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanCollideWith(Entity _other)
	{
		return false;
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x000FABC7 File Offset: 0x000F8DC7
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		if (_version > 11)
		{
			this.wasOnGround = _br.ReadBoolean();
			this.closeParachuteInTicks = _br.ReadInt32();
			this.showParachuteInTicks = _br.ReadInt32();
		}
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000FABFA File Offset: 0x000F8DFA
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write(this.wasOnGround);
		_bw.Write(this.closeParachuteInTicks);
		_bw.Write(this.showParachuteInTicks);
	}

	// Token: 0x04001E47 RID: 7751
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startRotY;

	// Token: 0x04001E48 RID: 7752
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public new bool wasOnGround;

	// Token: 0x04001E49 RID: 7753
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int showParachuteInTicks;

	// Token: 0x04001E4A RID: 7754
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int closeParachuteInTicks;

	// Token: 0x04001E4B RID: 7755
	public bool isSmokeOn = true;

	// Token: 0x04001E4C RID: 7756
	public float smokeTimeAfterLanding = 240f;

	// Token: 0x04001E4D RID: 7757
	public float smokeTimeOnGround;

	// Token: 0x04001E4E RID: 7758
	public float smokeTimer;

	// Token: 0x04001E4F RID: 7759
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform crateT;

	// Token: 0x04001E50 RID: 7760
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform parachuteT;
}
