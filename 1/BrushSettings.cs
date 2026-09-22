using System;
using UnityEngine;

// Token: 0x02001339 RID: 4921
[Serializable]
public class BrushSettings
{
	// Token: 0x17001270 RID: 4720
	// (get) Token: 0x06009B68 RID: 39784 RVA: 0x003AB111 File Offset: 0x003A9311
	// (set) Token: 0x06009B69 RID: 39785 RVA: 0x003AB119 File Offset: 0x003A9319
	public string MainTexName { get; set; } = "_MainTex";

	// Token: 0x17001271 RID: 4721
	// (get) Token: 0x06009B6A RID: 39786 RVA: 0x003AB122 File Offset: 0x003A9322
	// (set) Token: 0x06009B6B RID: 39787 RVA: 0x003AB12A File Offset: 0x003A932A
	public string DirectionMapName { get; set; } = "_DirectionMap";

	// Token: 0x17001272 RID: 4722
	// (get) Token: 0x06009B6C RID: 39788 RVA: 0x003AB133 File Offset: 0x003A9333
	// (set) Token: 0x06009B6D RID: 39789 RVA: 0x003AB13B File Offset: 0x003A933B
	public string RMOLMapName { get; set; } = "_RMOL";

	// Token: 0x17001273 RID: 4723
	// (get) Token: 0x06009B6E RID: 39790 RVA: 0x003AB144 File Offset: 0x003A9344
	// (set) Token: 0x06009B6F RID: 39791 RVA: 0x003AB14C File Offset: 0x003A934C
	public float Size
	{
		get
		{
			return this.size;
		}
		set
		{
			this.UpdateBrushPreview();
			this.size = Mathf.Max(1f, value);
		}
	}

	// Token: 0x17001274 RID: 4724
	// (get) Token: 0x06009B70 RID: 39792 RVA: 0x003AB165 File Offset: 0x003A9365
	// (set) Token: 0x06009B71 RID: 39793 RVA: 0x003AB16D File Offset: 0x003A936D
	public float Strength
	{
		get
		{
			return this.strength;
		}
		set
		{
			this.isDirty = true;
			this.strength = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17001275 RID: 4725
	// (get) Token: 0x06009B72 RID: 39794 RVA: 0x003AB182 File Offset: 0x003A9382
	// (set) Token: 0x06009B73 RID: 39795 RVA: 0x003AB18A File Offset: 0x003A938A
	public float Falloff
	{
		get
		{
			return this.falloff;
		}
		set
		{
			this.isDirty = true;
			this.falloff = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17001276 RID: 4726
	// (get) Token: 0x06009B74 RID: 39796 RVA: 0x003AB19F File Offset: 0x003A939F
	// (set) Token: 0x06009B75 RID: 39797 RVA: 0x003AB1A7 File Offset: 0x003A93A7
	public float Matting
	{
		get
		{
			return this.matting;
		}
		set
		{
			this.matting = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17001277 RID: 4727
	// (get) Token: 0x06009B76 RID: 39798 RVA: 0x003AB1B5 File Offset: 0x003A93B5
	// (set) Token: 0x06009B77 RID: 39799 RVA: 0x003AB1BD File Offset: 0x003A93BD
	public float Length
	{
		get
		{
			return this.length;
		}
		set
		{
			this.length = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17001278 RID: 4728
	// (get) Token: 0x06009B78 RID: 39800 RVA: 0x003AB1CB File Offset: 0x003A93CB
	// (set) Token: 0x06009B79 RID: 39801 RVA: 0x003AB1D3 File Offset: 0x003A93D3
	public float Roughness
	{
		get
		{
			return this.roughness;
		}
		set
		{
			this.roughness = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17001279 RID: 4729
	// (get) Token: 0x06009B7A RID: 39802 RVA: 0x003AB1E1 File Offset: 0x003A93E1
	// (set) Token: 0x06009B7B RID: 39803 RVA: 0x003AB1E9 File Offset: 0x003A93E9
	public float Metallic
	{
		get
		{
			return this.metallic;
		}
		set
		{
			this.metallic = Mathf.Clamp01(value);
		}
	}

	// Token: 0x1700127A RID: 4730
	// (get) Token: 0x06009B7C RID: 39804 RVA: 0x003AB1F7 File Offset: 0x003A93F7
	// (set) Token: 0x06009B7D RID: 39805 RVA: 0x003AB1FF File Offset: 0x003A93FF
	public float Occlusion
	{
		get
		{
			return this.occlusion;
		}
		set
		{
			this.occlusion = Mathf.Clamp01(value);
		}
	}

	// Token: 0x1700127B RID: 4731
	// (get) Token: 0x06009B7E RID: 39806 RVA: 0x003AB20D File Offset: 0x003A940D
	// (set) Token: 0x06009B7F RID: 39807 RVA: 0x003AB215 File Offset: 0x003A9415
	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
		}
	}

	// Token: 0x1700127C RID: 4732
	// (get) Token: 0x06009B80 RID: 39808 RVA: 0x003AB21E File Offset: 0x003A941E
	// (set) Token: 0x06009B81 RID: 39809 RVA: 0x003AB22C File Offset: 0x003A942C
	public Texture2D BrushPreview
	{
		get
		{
			this.UpdateBrushPreview();
			return this.brushPreview;
		}
		set
		{
			this.brushPreview = value;
		}
	}

	// Token: 0x06009B82 RID: 39810 RVA: 0x003AB238 File Offset: 0x003A9438
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateBrushPreview()
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		if (this.brushPreview == null)
		{
			this.brushPreview = new Texture2D(100, 100);
		}
		Color[] array = new Color[this.brushPreview.width * this.brushPreview.height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Color.clear;
		}
		int num = Mathf.FloorToInt(this.size * 0.5f);
		for (int j = 0; j < 100; j++)
		{
			for (int k = 0; k < 100; k++)
			{
				Vector2 v = new Vector2((float)j, (float)k) - new Vector2(50f, 50f);
				float num2 = Vector3.Dot(v, v);
				if (num2 <= (float)(num * num))
				{
					float num3 = Mathf.Pow(Mathf.Clamp01(1f - MathF.Sqrt(num2) / (float)num), this.falloff * 4f) * this.strength;
					array[k * this.brushPreview.width + j] = new Color(num3, num3, num3, 1f);
				}
				else
				{
					array[k * this.brushPreview.width + j] = Color.black;
				}
			}
		}
		this.brushPreview.SetPixels(array);
		this.brushPreview.Apply();
	}

	// Token: 0x04007528 RID: 29992
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D brushPreview;

	// Token: 0x04007529 RID: 29993
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float size = 100f;

	// Token: 0x0400752A RID: 29994
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float strength = 0.5f;

	// Token: 0x0400752B RID: 29995
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float falloff = 0.5f;

	// Token: 0x0400752C RID: 29996
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float matting = 1f;

	// Token: 0x0400752D RID: 29997
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float length = 1f;

	// Token: 0x0400752E RID: 29998
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float roughness = 0.5f;

	// Token: 0x0400752F RID: 29999
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float metallic = 0.5f;

	// Token: 0x04007530 RID: 30000
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float occlusion = 1f;

	// Token: 0x04007531 RID: 30001
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color color = Color.white;

	// Token: 0x04007532 RID: 30002
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDirty = true;
}
