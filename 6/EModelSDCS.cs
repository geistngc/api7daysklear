using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000518 RID: 1304
[Preserve]
public class EModelSDCS : EModelPlayer
{
	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x06002ADF RID: 10975 RVA: 0x0010FCD7 File Offset: 0x0010DED7
	public bool isMale
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.archetype == null || this.archetype.Sex == "Male";
		}
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x06002AE0 RID: 10976 RVA: 0x0010FCF8 File Offset: 0x0010DEF8
	public override Transform NeckTransform
	{
		get
		{
			if (!base.IsFPV)
			{
				return this.boneCatalog["Neck"];
			}
			return this.boneCatalogFP["Neck"];
		}
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x0010FD23 File Offset: 0x0010DF23
	public void Awake()
	{
		this.playerEntity = base.transform.GetComponent<EntityPlayerLocal>();
		if (this.playerEntity == null)
		{
			this.playerEntity = base.transform.GetComponent<EntityPlayer>();
		}
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x0010FD58 File Offset: 0x0010DF58
	public override void Init(World _world, Entity _entity, EModelInstanceAssets _assets)
	{
		this.entity = _entity;
		this.assets = _assets;
		this.entityClass = EntityClass.list[this.entity.entityClass];
		this.archetype = this.playerEntity.playerProfile.CreateTempArchetype();
		this.ragdollChance = this.entityClass.RagdollOnDeathChance;
		this.bHasRagdoll = this.entityClass.HasRagdoll;
		this.modelTransformParent = EModelBase.FindModel(base.transform);
		base.IsFPV = (this.entity is EntityPlayerLocal);
		this.createModel(_world, this.entityClass);
		this.createAvatarController(EntityClass.list[this.entity.entityClass]);
		XUiM_PlayerEquipment.HandleRefreshEquipment += this.XUiM_PlayerEquipment_HandleRefreshEquipment;
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0010FE24 File Offset: 0x0010E024
	[PublicizedFrom(EAccessModifier.Private)]
	public new void OnDestroy()
	{
		XUiM_PlayerEquipment.HandleRefreshEquipment -= this.XUiM_PlayerEquipment_HandleRefreshEquipment;
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x0010FE37 File Offset: 0x0010E037
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiM_PlayerEquipment_HandleRefreshEquipment(XUiM_PlayerEquipment playerEquipment)
	{
		if (playerEquipment.Equipment == this.playerEntity.equipment)
		{
			this.UpdateEquipment();
		}
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x0010FE54 File Offset: 0x0010E054
	public void UpdateEquipment()
	{
		Animator animator = this.avatarController.GetAnimator();
		if (animator && (base.IsFPV || animator.enabled))
		{
			this.GenerateMeshes();
		}
	}

	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x06002AE6 RID: 10982 RVA: 0x0010FE8C File Offset: 0x0010E08C
	public Transform HeadTransformFP
	{
		get
		{
			if (this.headTransFP == null && this.boneCatalogFP.ContainsKey("Head"))
			{
				this.headTransFP = this.boneCatalogFP["Head"];
			}
			return this.headTransFP;
		}
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x0010FECC File Offset: 0x0010E0CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (base.IsFPV && this.HeadTransformFP != null)
		{
			foreach (Material material in this.ClipMaterialsFP)
			{
				material.SetVector("_ClipCenter", this.HeadTransformFP.position);
			}
		}
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x0010FF50 File Offset: 0x0010E150
	public override void SwitchModelAndView(bool _bFPV, bool _isMale)
	{
		base.IsFPV = _bFPV;
		this.playerEntity.IsMale = this.isMale;
		this.GenerateMeshes();
		base.SwitchModelAndView(base.IsFPV, _isMale);
		this.meshTransform = this.modelTransform.FindInChildren("Spine1");
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x0010FFA0 File Offset: 0x0010E1A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createModel(World _world, EntityClass _ec)
	{
		if (this.modelTransformParent == null)
		{
			return;
		}
		if (this.playerModelTransform != null)
		{
			UnityEngine.Object.Destroy(this.playerModelTransform.gameObject);
		}
		this.playerModelTransform = this.GenerateMeshes();
		this.playerModelTransform.name = (this.modelName = "player_" + this.archetype.Sex + "Ragdoll");
		this.playerModelTransform.tag = "E_BP_Body";
		this.playerModelTransform.SetParent(this.modelTransformParent, false);
		this.playerModelTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.playerModelTransform.gameObject.GetOrAddComponent<AnimationEventBridge>();
		this.updateLightScript = this.playerModelTransform.gameObject.GetOrAddComponent<UpdateLightOnPlayers>();
		this.updateLightScript.IsDynamicObject = true;
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (entityAlive != null)
		{
			entityAlive.ReassignEquipmentTransforms();
		}
		this.baseRig.transform.FindInChilds("Origin", false).tag = "E_BP_BipedRoot";
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x001100B4 File Offset: 0x0010E2B4
	public Transform GenerateMeshes()
	{
		SDCSUtils.CreateVizTP(this.archetype, ref this.baseRig, ref this.boneCatalog, this.playerEntity, base.IsFPV);
		if (this.playerEntity as EntityPlayerLocal != null)
		{
			SDCSUtils.CreateVizFP(this.archetype, ref this.baseRigFP, ref this.boneCatalogFP, this.playerEntity, base.IsFPV);
		}
		base.ClothSimInit();
		return this.baseRig.transform;
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x0011012B File Offset: 0x0010E32B
	public override Transform GetHeadTransform()
	{
		if (this.headT == null && this.boneCatalog.ContainsKey("Head"))
		{
			this.headT = this.boneCatalog["Head"];
		}
		return this.headT;
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x00110169 File Offset: 0x0010E369
	public override Transform GetPelvisTransform()
	{
		if (this.bipedPelvisTransform == null && this.boneCatalog.ContainsKey("Hips"))
		{
			this.bipedPelvisTransform = this.boneCatalog["Hips"];
		}
		return this.bipedPelvisTransform;
	}

	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x06002AED RID: 10989 RVA: 0x001101A7 File Offset: 0x0010E3A7
	public Archetype Archetype
	{
		get
		{
			return this.archetype;
		}
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x001101AF File Offset: 0x0010E3AF
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetRace(string value)
	{
		this.archetype.Race = value;
		if (SDCSUtils.BasePartsExist(this.archetype))
		{
			this.SwitchModelAndView(base.IsFPV, this.isMale);
		}
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x001101DC File Offset: 0x0010E3DC
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetVariant(int value)
	{
		this.archetype.Variant = (int)((byte)value);
		if (SDCSUtils.BasePartsExist(this.archetype))
		{
			this.SwitchModelAndView(base.IsFPV, this.isMale);
		}
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x0011020A File Offset: 0x0010E40A
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetSex(bool value)
	{
		this.archetype.IsMale = value;
		if (SDCSUtils.BasePartsExist(this.archetype))
		{
			this.SwitchModelAndView(base.IsFPV, this.isMale);
		}
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x00110238 File Offset: 0x0010E438
	public override void SetVisible(bool _bVisible, bool _isKeepColliders = false)
	{
		bool visible = base.visible;
		if (_bVisible != visible)
		{
			SDCSUtils.SetVisible(this.baseRig, _bVisible);
		}
		base.SetVisible(_bVisible, _isKeepColliders);
	}

	// Token: 0x040020C3 RID: 8387
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityClass entityClass;

	// Token: 0x040020C4 RID: 8388
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayer playerEntity;

	// Token: 0x040020C5 RID: 8389
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject baseRig;

	// Token: 0x040020C6 RID: 8390
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SDCSUtils.TransformCatalog boneCatalog;

	// Token: 0x040020C7 RID: 8391
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject baseRigFP;

	// Token: 0x040020C8 RID: 8392
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SDCSUtils.TransformCatalog boneCatalogFP;

	// Token: 0x040020C9 RID: 8393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform playerModelTransform;

	// Token: 0x040020CA RID: 8394
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform headT;

	// Token: 0x040020CB RID: 8395
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform headTransFP;

	// Token: 0x040020CC RID: 8396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UpdateLightOnPlayers updateLightScript;

	// Token: 0x040020CD RID: 8397
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Archetype archetype;

	// Token: 0x040020CE RID: 8398
	public SDCSUtils.SlotData.HairMaskTypes HairMaskType;

	// Token: 0x040020CF RID: 8399
	public SDCSUtils.SlotData.HairMaskTypes FacialHairMaskType;

	// Token: 0x040020D0 RID: 8400
	public List<Material> ClipMaterialsFP = new List<Material>();
}
