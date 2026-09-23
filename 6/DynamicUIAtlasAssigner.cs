using System;
using UnityEngine;

// Token: 0x02001285 RID: 4741
public class DynamicUIAtlasAssigner : MonoBehaviour
{
	// Token: 0x060096BE RID: 38590 RVA: 0x0038BEC8 File Offset: 0x0038A0C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		GameObject gameObject = GameObject.Find(this.AtlasPathInScene);
		if (gameObject == null)
		{
			Log.Warning("Could not assign atlas: Atlas object not found");
			UnityEngine.Object.Destroy(this);
			return;
		}
		this.atlas = gameObject.GetComponent<DynamicUIAtlas>();
		if (this.atlas == null)
		{
			Log.Warning("Could not assign atlas: Atlas component not found");
			UnityEngine.Object.Destroy(this);
			return;
		}
		this.atlas.AtlasUpdatedEv += this.AtlasUpdateCallback;
		this.sprites = base.GetComponents<UISprite>();
		foreach (UISprite uisprite in this.sprites)
		{
			uisprite.atlas = this.atlas;
			if (!string.IsNullOrEmpty(this.OptionalSpriteName))
			{
				uisprite.spriteName = this.OptionalSpriteName;
			}
		}
	}

	// Token: 0x060096BF RID: 38591 RVA: 0x0038BF87 File Offset: 0x0038A187
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnDestroy()
	{
		if (this.atlas != null)
		{
			this.atlas.AtlasUpdatedEv -= this.AtlasUpdateCallback;
		}
	}

	// Token: 0x060096C0 RID: 38592 RVA: 0x0038BFB0 File Offset: 0x0038A1B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AtlasUpdateCallback()
	{
		if (!string.IsNullOrEmpty(this.OptionalSpriteName))
		{
			foreach (UISprite uisprite in this.sprites)
			{
				uisprite.spriteName = null;
				uisprite.spriteName = this.OptionalSpriteName;
			}
		}
	}

	// Token: 0x040070EC RID: 28908
	public string AtlasPathInScene;

	// Token: 0x040070ED RID: 28909
	public string OptionalSpriteName;

	// Token: 0x040070EE RID: 28910
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DynamicUIAtlas atlas;

	// Token: 0x040070EF RID: 28911
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UISprite[] sprites;
}
