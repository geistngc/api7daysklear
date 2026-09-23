using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000516 RID: 1302
[Preserve]
public class EModelNpc : EModelBase
{
	// Token: 0x06002AD9 RID: 10969 RVA: 0x0010FA9C File Offset: 0x0010DC9C
	public override void Init(World _world, Entity _entity, EModelInstanceAssets _assets)
	{
		this.entity = _entity;
		this.assets = _assets;
		EntityClass entityClass = EntityClass.list[this.entity.entityClass];
		this.ragdollChance = entityClass.RagdollOnDeathChance;
		this.bHasRagdoll = entityClass.HasRagdoll;
		this.modelTransformParent = EModelBase.FindModel(base.transform);
		this.createModel(_world, entityClass);
		this.setupColliders(this.entity.transform);
		bool bIsMale = entityClass.bIsMale;
		if (GameManager.IsDedicatedServer && !this.entity.RootMotion)
		{
			this.avatarController = base.transform.gameObject.AddComponent<AvatarControllerDummy>();
			Animator[] componentsInChildren = base.transform.GetComponentsInChildren<Animator>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			if (this.modelTransformParent != null)
			{
				this.SwitchModelAndView(false, bIsMale);
			}
		}
		else
		{
			this.createAvatarController(entityClass);
			if (this.modelTransformParent != null)
			{
				this.SwitchModelAndView(false, bIsMale);
			}
			if (GameManager.IsDedicatedServer && this.avatarController != null && this.entity.RootMotion)
			{
				this.avatarController.SetVisible(true);
			}
		}
		base.LookAtInit();
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x0010FBCC File Offset: 0x0010DDCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void setupColliders(Transform bodyRoot)
	{
		Transform transform = bodyRoot.FindInChilds("Position", false);
		if (transform == null)
		{
			transform = bodyRoot.FindInChilds("Origin", false);
		}
		transform.tag = "E_BP_BipedRoot";
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x0010FC08 File Offset: 0x0010DE08
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createAvatarController(EntityClass _ec)
	{
		Type type = Type.GetType(_ec.Properties.GetValue(EntityClass.PropAvatarController));
		this.avatarController = (base.transform.gameObject.AddComponent(type) as AvatarController);
		(this.entity as EntityAlive).ReassignEquipmentTransforms();
	}
}
