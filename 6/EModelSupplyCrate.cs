using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200051A RID: 1306
[Preserve]
public class EModelSupplyCrate : EModelBase
{
	// Token: 0x06002AF5 RID: 10997 RVA: 0x001102A2 File Offset: 0x0010E4A2
	public override void Init(World _world, Entity _entity, EModelInstanceAssets _assets)
	{
		base.Init(_world, _entity, _assets);
		this.parachute = base.transform.FindInChilds("parachute_supplies", false);
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x001102C4 File Offset: 0x0010E4C4
	public override void SetSkinTexture(string _texture)
	{
		base.SetSkinTexture(_texture);
		if (_texture == null || _texture.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.modelTransformParent.childCount; i++)
		{
			Transform child = this.modelTransformParent.GetChild(i);
			if (child != this.parachute)
			{
				child.GetComponent<Renderer>().material.mainTexture = DataLoader.LoadAsset<Texture2D>(DataLoader.IsInResources(_texture) ? ("Entities/" + _texture) : _texture, false);
				return;
			}
		}
	}

	// Token: 0x040020D1 RID: 8401
	public Transform parachute;
}
