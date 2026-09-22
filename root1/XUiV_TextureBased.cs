using System;
using UnityEngine;

// Token: 0x02001165 RID: 4453
public class XUiV_TextureBased : XUiV_ImageBased
{
	// Token: 0x170010F6 RID: 4342
	// (get) Token: 0x06008E43 RID: 36419 RVA: 0x00359FCA File Offset: 0x003581CA
	// (set) Token: 0x06008E44 RID: 36420 RVA: 0x00359FD2 File Offset: 0x003581D2
	public virtual Texture Texture
	{
		get
		{
			return this.texture;
		}
		set
		{
			this.texture = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010F7 RID: 4343
	// (get) Token: 0x06008E45 RID: 36421 RVA: 0x00359FE1 File Offset: 0x003581E1
	// (set) Token: 0x06008E46 RID: 36422 RVA: 0x00359FE9 File Offset: 0x003581E9
	public Material Material
	{
		get
		{
			return this.material;
		}
		set
		{
			this.material = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010F8 RID: 4344
	// (get) Token: 0x06008E47 RID: 36423 RVA: 0x00359FF8 File Offset: 0x003581F8
	// (set) Token: 0x06008E48 RID: 36424 RVA: 0x0035A005 File Offset: 0x00358205
	public UIDrawCall.OnRenderCallback OnRenderTexture
	{
		get
		{
			return this.uiTexture.onRender;
		}
		set
		{
			this.uiTexture.onRender = value;
		}
	}

	// Token: 0x170010F9 RID: 4345
	// (get) Token: 0x06008E49 RID: 36425 RVA: 0x0035A013 File Offset: 0x00358213
	// (set) Token: 0x06008E4A RID: 36426 RVA: 0x0035A01B File Offset: 0x0035821B
	public Rect UVRect
	{
		get
		{
			return this.uvRect;
		}
		set
		{
			this.uvRect = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010FA RID: 4346
	// (get) Token: 0x06008E4B RID: 36427 RVA: 0x0035A02A File Offset: 0x0035822A
	// (set) Token: 0x06008E4C RID: 36428 RVA: 0x0035A032 File Offset: 0x00358232
	public Vector4 Border
	{
		get
		{
			return this.border;
		}
		set
		{
			this.border = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010FB RID: 4347
	// (get) Token: 0x06008E4D RID: 36429 RVA: 0x0035A041 File Offset: 0x00358241
	// (set) Token: 0x06008E4E RID: 36430 RVA: 0x0035A049 File Offset: 0x00358249
	[XuiXmlAttribute("color", false)]
	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010FC RID: 4348
	// (get) Token: 0x06008E4F RID: 36431 RVA: 0x0035A058 File Offset: 0x00358258
	// (set) Token: 0x06008E50 RID: 36432 RVA: 0x0035A060 File Offset: 0x00358260
	public UIBasicSprite.FillDirection FillDirection
	{
		get
		{
			return this.fillDirection;
		}
		set
		{
			this.fillDirection = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010FD RID: 4349
	// (get) Token: 0x06008E51 RID: 36433 RVA: 0x0035A06F File Offset: 0x0035826F
	// (set) Token: 0x06008E52 RID: 36434 RVA: 0x0035A077 File Offset: 0x00358277
	[XuiXmlAttribute("sourceaspectratiorespectpivot", false)]
	public bool SourceAspectRatioRespectPivot
	{
		get
		{
			return this.sourceAspectRatioRespectPivot;
		}
		set
		{
			if (this.sourceAspectRatioRespectPivot == value)
			{
				return;
			}
			this.sourceAspectRatioRespectPivot = value;
			base.SetDirty();
		}
	}

	// Token: 0x06008E53 RID: 36435 RVA: 0x0035A090 File Offset: 0x00358290
	public XUiV_TextureBased(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008E54 RID: 36436 RVA: 0x0035A0CF File Offset: 0x003582CF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		base.createComponents(_go);
		_go.AddComponent<UITexture>();
	}

	// Token: 0x06008E55 RID: 36437 RVA: 0x0035A0E0 File Offset: 0x003582E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.widget = (this.uiTexture = this.uiTransform.gameObject.GetComponent<UITexture>());
	}

	// Token: 0x06008E56 RID: 36438 RVA: 0x0035A114 File Offset: 0x00358314
	public void CreateMaterial(string _shaderName = "Unlit/Transparent Colored Emissive TextureArray")
	{
		Shader shader = GlobalAssets.FindShader(_shaderName);
		if (shader == null)
		{
			Log.Error("Could not find shader " + _shaderName);
			return;
		}
		this.Material = new Material(shader);
		this.isCreatedMaterial = true;
	}

	// Token: 0x06008E57 RID: 36439 RVA: 0x0035A155 File Offset: 0x00358355
	public override void Cleanup()
	{
		base.Cleanup();
		if (!this.isCreatedMaterial)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.material);
		this.material = null;
		this.isCreatedMaterial = false;
	}

	// Token: 0x06008E58 RID: 36440 RVA: 0x00359581 File Offset: 0x00357781
	public override void InitView()
	{
		base.InitView();
		this.updateData();
	}

	// Token: 0x06008E59 RID: 36441 RVA: 0x0035A180 File Offset: 0x00358380
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		this.uiTexture.enabled = (this.texture != null);
		this.uiTexture.mainTexture = this.texture;
		this.uiTexture.color = base.opacityModColor(this.color);
		this.uiTexture.keepAspectRatio = this.keepAspectRatio;
		this.uiTexture.aspectRatio = this.aspectRatio;
		this.uiTexture.fixedAspect = this.keepSourceAspectRatio;
		this.uiTexture.fixedAspectRespectPivot = this.sourceAspectRatioRespectPivot;
		this.uiTexture.SetDimensions(this.size.x, this.size.y);
		this.uiTexture.type = this.type;
		this.uiTexture.border = this.border;
		this.uiTexture.uvRect = this.uvRect;
		this.uiTexture.flip = this.flip;
		this.uiTexture.centerType = (this.fillCenter ? UIBasicSprite.AdvancedType.Sliced : UIBasicSprite.AdvancedType.Invisible);
		this.uiTexture.fillDirection = this.fillDirection;
		this.uiTexture.material = this.material;
		base.updateData();
	}

	// Token: 0x06008E5A RID: 36442 RVA: 0x0035A2B4 File Offset: 0x003584B4
	public void SetTextureDirty()
	{
		this.uiTexture.mainTexture = null;
		base.SetDirty();
	}

	// Token: 0x06008E5B RID: 36443 RVA: 0x0035A2C8 File Offset: 0x003584C8
	public virtual void UnloadTexture()
	{
		if (this.Texture == null)
		{
			return;
		}
		this.uiTexture.mainTexture = null;
		this.texture = null;
		base.SetDirty();
	}

	// Token: 0x06008E5C RID: 36444 RVA: 0x0035A2F2 File Offset: 0x003584F2
	[XuiXmlAttribute("material", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeMaterial(string _value)
	{
		this.xui.LoadData<Material>(_value, delegate(Material _o)
		{
			this.material = new Material(_o);
		});
	}

	// Token: 0x06008E5D RID: 36445 RVA: 0x0035A30C File Offset: 0x0035850C
	[XuiXmlAttribute("rect_offset", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeRectOffset(Vector2 _value)
	{
		Rect uvrect = this.UVRect;
		uvrect.x = _value.x;
		uvrect.y = _value.y;
		this.UVRect = uvrect;
	}

	// Token: 0x06008E5E RID: 36446 RVA: 0x0035A344 File Offset: 0x00358544
	[XuiXmlAttribute("rect_size", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeRectSize(Vector2 _value)
	{
		Rect uvrect = this.UVRect;
		uvrect.width = _value.x;
		uvrect.height = _value.y;
		this.UVRect = uvrect;
	}

	// Token: 0x0400685A RID: 26714
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITexture uiTexture;

	// Token: 0x0400685B RID: 26715
	[PublicizedFrom(EAccessModifier.Private)]
	public Texture texture;

	// Token: 0x0400685C RID: 26716
	[PublicizedFrom(EAccessModifier.Private)]
	public Material material;

	// Token: 0x0400685D RID: 26717
	[PublicizedFrom(EAccessModifier.Private)]
	public Rect uvRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x0400685E RID: 26718
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector4 border = Vector4.zero;

	// Token: 0x0400685F RID: 26719
	[PublicizedFrom(EAccessModifier.Private)]
	public Color color = Color.white;

	// Token: 0x04006860 RID: 26720
	[PublicizedFrom(EAccessModifier.Private)]
	public UIBasicSprite.FillDirection fillDirection;

	// Token: 0x04006861 RID: 26721
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isCreatedMaterial;

	// Token: 0x04006862 RID: 26722
	[PublicizedFrom(EAccessModifier.Private)]
	public bool sourceAspectRatioRespectPivot;
}
