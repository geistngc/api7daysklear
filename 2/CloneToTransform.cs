using System;
using UnityEngine;

// Token: 0x02001333 RID: 4915
public class CloneToTransform : MonoBehaviour
{
	// Token: 0x06009B26 RID: 39718 RVA: 0x003A9EBA File Offset: 0x003A80BA
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.m_transform = base.transform;
	}

	// Token: 0x06009B27 RID: 39719 RVA: 0x003A9EC8 File Offset: 0x003A80C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.m_parentEntity = base.GetComponentInParent<Entity>();
		if (this.m_parentEntity != null)
		{
			this.m_hasParentEntity = true;
		}
		if (this.m_parentEntity is EntityPlayerLocal)
		{
			this.m_hasParentEntityLocal = true;
		}
	}

	// Token: 0x06009B28 RID: 39720 RVA: 0x003A9EFF File Offset: 0x003A80FF
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		this.DestroyClone();
	}

	// Token: 0x06009B29 RID: 39721 RVA: 0x003A9F07 File Offset: 0x003A8107
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyClone()
	{
		if (!this.m_clone)
		{
			return;
		}
		this.m_cloneTransform = null;
		UnityEngine.Object.Destroy(this.m_clone);
		this.m_clone = null;
	}

	// Token: 0x06009B2A RID: 39722 RVA: 0x003A9F30 File Offset: 0x003A8130
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		if (this.m_clone)
		{
			this.m_clone.SetActive(true);
		}
	}

	// Token: 0x06009B2B RID: 39723 RVA: 0x003A9F4B File Offset: 0x003A814B
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (this.m_clone)
		{
			this.m_clone.SetActive(false);
		}
	}

	// Token: 0x06009B2C RID: 39724 RVA: 0x003A9F68 File Offset: 0x003A8168
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (!this.m_storage)
		{
			this.m_storage = UnityEngine.Object.Instantiate<GameObject>(base.gameObject, this.m_transform, true);
			this.m_storageTransform = this.m_storage.transform;
			this.m_storageTransform.name = this.m_transform.name + "(CloneToTransform)";
			this.m_storage.SetActive(false);
			CloneToTransform obj;
			if (this.m_storage.TryGetComponent<CloneToTransform>(out obj))
			{
				UnityEngine.Object.Destroy(obj);
			}
			foreach (object obj2 in this.m_transform)
			{
				Transform transform = (Transform)obj2;
				if (!(transform == this.m_storageTransform))
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			foreach (Component component in base.GetComponents<Component>())
			{
				if (!(component == this) && !(component is Transform))
				{
					UnityEngine.Object.Destroy(component);
				}
			}
		}
		Transform transform2 = null;
		if ((this.m_hasParentEntityLocal || !this.m_hasParentEntity) && Camera.main)
		{
			transform2 = Camera.main.transform;
		}
		else if (this.m_parentEntity != null && this.m_parentEntity.emodel != null)
		{
			transform2 = this.m_parentEntity.emodel.GetModelTransformParent();
		}
		if (!transform2)
		{
			this.DestroyClone();
			this.m_lastCloneTarget = null;
			return;
		}
		if (!this.m_clone)
		{
			this.m_lastCloneTarget = transform2;
			this.m_clone = UnityEngine.Object.Instantiate<GameObject>(this.m_storage, transform2, true);
			this.m_cloneTransform = this.m_clone.transform;
			this.m_cloneTransform.name = this.m_transform.name + "(clone)";
			this.m_clone.SetActive(true);
			CloneToTransform obj3;
			if (this.m_clone.TryGetComponent<CloneToTransform>(out obj3))
			{
				UnityEngine.Object.Destroy(obj3);
			}
		}
		if (this.m_lastCloneTarget != transform2)
		{
			this.m_lastCloneTarget = transform2;
			this.m_cloneTransform.parent = transform2;
			this.m_lastLocalPosition = default(Vector3);
			this.m_lastLocalRotation = default(Quaternion);
		}
		this.CheckTransform();
	}

	// Token: 0x06009B2D RID: 39725 RVA: 0x003AA1B8 File Offset: 0x003A83B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckTransform()
	{
		if (this.m_lastLocalPosition == this.m_transform.localPosition && this.m_lastLocalRotation == this.m_transform.localRotation)
		{
			return;
		}
		this.m_lastLocalPosition = this.m_transform.localPosition;
		this.m_lastLocalRotation = this.m_transform.localRotation;
		this.m_cloneTransform.SetPositionAndRotation(this.m_transform.position, this.m_transform.rotation);
	}

	// Token: 0x040074FD RID: 29949
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_transform;

	// Token: 0x040074FE RID: 29950
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject m_storage;

	// Token: 0x040074FF RID: 29951
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_storageTransform;

	// Token: 0x04007500 RID: 29952
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject m_clone;

	// Token: 0x04007501 RID: 29953
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_cloneTransform;

	// Token: 0x04007502 RID: 29954
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_lastCloneTarget;

	// Token: 0x04007503 RID: 29955
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_lastLocalPosition;

	// Token: 0x04007504 RID: 29956
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion m_lastLocalRotation;

	// Token: 0x04007505 RID: 29957
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_hasParentEntity;

	// Token: 0x04007506 RID: 29958
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_hasParentEntityLocal;

	// Token: 0x04007507 RID: 29959
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity m_parentEntity;
}
