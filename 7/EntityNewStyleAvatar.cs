using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004BD RID: 1213
[Preserve]
public class EntityNewStyleAvatar : Entity
{
	// Token: 0x06002687 RID: 9863 RVA: 0x000EC5F4 File Offset: 0x000EA7F4
	public void EnableSubmesh(string submeshName, bool enable)
	{
		Transform transform = base.transform;
		Transform transform2 = transform.Find("Graphics/Model");
		if (transform2 == null)
		{
			transform2 = transform;
		}
		Transform transform3 = transform2.Find("base");
		if (transform3 != null)
		{
			int childCount = transform3.childCount;
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = transform3.GetChild(i).gameObject;
				if (gameObject.name == submeshName)
				{
					gameObject.SetActive(enable);
				}
			}
		}
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x000EC674 File Offset: 0x000EA874
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		Transform transform = base.transform;
		Transform transform2 = transform.Find("Graphics/Model");
		if (transform2 == null)
		{
			transform2 = transform;
		}
		Transform transform3 = null;
		if (transform2 != null)
		{
			transform3 = DataLoader.LoadAsset<Transform>("@:Entities/Player/Male/maleTestPrefab.prefab", false);
			if (transform3 != null)
			{
				transform3 = UnityEngine.Object.Instantiate<Transform>(transform3, transform2);
				transform3.name = "base";
			}
		}
		if (transform3)
		{
			int childCount = transform3.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform3.GetChild(i);
				Renderer component = child.GetComponent<Renderer>();
				if (!(component == null) && component.sharedMaterials != null)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
		base.gameObject.AddComponent<NewAvatarRootMotion>();
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000EC739 File Offset: 0x000EA939
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000EC741 File Offset: 0x000EA941
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
	}

	// Token: 0x04001C96 RID: 7318
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, EntityNewStyleAvatar.BodySlot> m_entitySlots = new Dictionary<string, EntityNewStyleAvatar.BodySlot>();

	// Token: 0x020004BE RID: 1214
	[PublicizedFrom(EAccessModifier.Protected)]
	public class StringTags
	{
		// Token: 0x0600268B RID: 9867 RVA: 0x000EC749 File Offset: 0x000EA949
		public void AddTag(string tag)
		{
			this.tags.Add(tag);
		}

		// Token: 0x0600268C RID: 9868 RVA: 0x000EC758 File Offset: 0x000EA958
		public bool HasTag(string tag)
		{
			return this.tags.Contains(tag);
		}

		// Token: 0x04001C97 RID: 7319
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSet<string> tags;
	}

	// Token: 0x020004BF RID: 1215
	[PublicizedFrom(EAccessModifier.Protected)]
	public class BodySlot
	{
		// Token: 0x04001C98 RID: 7320
		[PublicizedFrom(EAccessModifier.Private)]
		public string submeshName;

		// Token: 0x04001C99 RID: 7321
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityNewStyleAvatar.StringTags tags;
	}
}
