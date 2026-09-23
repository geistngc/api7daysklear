using System;
using UnityEngine;

// Token: 0x0200141A RID: 5146
public class GUIFPS : MonoBehaviour
{
	// Token: 0x170012F4 RID: 4852
	// (get) Token: 0x0600A19B RID: 41371 RVA: 0x003CC0FB File Offset: 0x003CA2FB
	// (set) Token: 0x0600A19C RID: 41372 RVA: 0x003CC103 File Offset: 0x003CA303
	public bool Enabled
	{
		get
		{
			return this.bEnabled;
		}
		set
		{
			if (this.bEnabled == value)
			{
				return;
			}
			this.bEnabled = value;
			if (!value && this.guiFpsGraphTexture != null && this.guiFpsGraphTexture.enabled)
			{
				this.guiFpsGraphTexture.enabled = false;
			}
		}
	}

	// Token: 0x170012F5 RID: 4853
	// (get) Token: 0x0600A19D RID: 41373 RVA: 0x003CC140 File Offset: 0x003CA340
	// (set) Token: 0x0600A19E RID: 41374 RVA: 0x003CC148 File Offset: 0x003CA348
	public bool ShowGraph
	{
		get
		{
			return this.bShowGraph;
		}
		set
		{
			if (this.bShowGraph == value)
			{
				return;
			}
			this.bShowGraph = value;
			if (value && this.guiFpsGraphTexture == null)
			{
				this.initFpsGraph();
			}
			if (this.guiFpsGraphTexture.enabled != this.bShowGraph)
			{
				this.guiFpsGraphTexture.enabled = this.bShowGraph;
			}
		}
	}

	// Token: 0x0600A19F RID: 41375 RVA: 0x003CC1A1 File Offset: 0x003CA3A1
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		this.windowManager = base.GetComponentInParent<GUIWindowManager>();
		GamePrefs.OnGamePrefChanged += this.OnGamePrefChanged;
	}

	// Token: 0x0600A1A0 RID: 41376 RVA: 0x003CC1C0 File Offset: 0x003CA3C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		GamePrefs.OnGamePrefChanged -= this.OnGamePrefChanged;
	}

	// Token: 0x0600A1A1 RID: 41377 RVA: 0x003CC1D3 File Offset: 0x003CA3D3
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGamePrefChanged(EnumGamePrefs _obj)
	{
		if (_obj == EnumGamePrefs.OptionsUiFpsScaling)
		{
			this.lastResolution = Vector2i.zero;
		}
	}

	// Token: 0x0600A1A2 RID: 41378 RVA: 0x003CC1E8 File Offset: 0x003CA3E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.fps.Update())
		{
			this.format = string.Format("{0:F1} FPS", this.fps.Counter);
		}
		if (!this.bEnabled)
		{
			return;
		}
		if (this.bShowGraph)
		{
			this.updateFPSGraph();
		}
	}

	// Token: 0x0600A1A3 RID: 41379 RVA: 0x003CC23C File Offset: 0x003CA43C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnGUI()
	{
		if (!this.Enabled || !this.windowManager.IsHUDEnabled())
		{
			return;
		}
		Vector2i vector2i = new Vector2i(Screen.width, Screen.height);
		if (this.lastResolution != vector2i)
		{
			float num = GamePrefs.GetFloat(EnumGamePrefs.OptionsUiFpsScaling) * 13f;
			this.lastResolution = vector2i;
			this.boxStyle = new GUIStyle(GUI.skin.box);
			int num2;
			if (vector2i.y > 1200)
			{
				num2 = Mathf.RoundToInt((float)vector2i.y / (1200f / num));
			}
			else
			{
				num2 = Mathf.RoundToInt(num);
			}
			this.boxStyle.fontSize = num2;
			this.boxAreaHeight = num2 + 10;
			this.boxAreaWidth = num2 * 7;
		}
		if (this.fps.Counter < 30f)
		{
			GUI.color = Color.yellow;
		}
		else if (this.fps.Counter < 10f)
		{
			GUI.color = Color.red;
		}
		else
		{
			GUI.color = Color.green;
		}
		GUI.Box(new Rect(14f, (float)(Screen.height / 2 + 40), (float)this.boxAreaWidth, (float)this.boxAreaHeight), this.format, this.boxStyle);
	}

	// Token: 0x0600A1A4 RID: 41380 RVA: 0x003CC36F File Offset: 0x003CA56F
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnApplicationQuit()
	{
		if (this.texture != null)
		{
			UnityEngine.Object.Destroy(this.texture);
		}
	}

	// Token: 0x0600A1A5 RID: 41381 RVA: 0x003CC38A File Offset: 0x003CA58A
	[PublicizedFrom(EAccessModifier.Private)]
	public void initFpsGraph()
	{
		this.texture = GUIFPS.createGUITexture();
		this.guiFpsGraphTexture = base.gameObject.AddMissingComponent<UITexture>();
		this.guiFpsGraphTexture.mainTexture = this.texture;
		this.guiFpsGraphTexture.enabled = false;
	}

	// Token: 0x0600A1A6 RID: 41382 RVA: 0x003CC3C8 File Offset: 0x003CA5C8
	[PublicizedFrom(EAccessModifier.Private)]
	public static Texture2D createGUITexture()
	{
		Texture2D texture2D = new Texture2D(1024, 256, TextureFormat.RGBA32, false);
		for (int i = 0; i < texture2D.height; i++)
		{
			for (int j = 0; j < texture2D.width; j++)
			{
				texture2D.SetPixel(j, i, default(Color));
			}
		}
		texture2D.filterMode = FilterMode.Point;
		return texture2D;
	}

	// Token: 0x0600A1A7 RID: 41383 RVA: 0x003CC424 File Offset: 0x003CA624
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateFPSGraph()
	{
		long totalMemory = GC.GetTotalMemory(false);
		if (totalMemory < this.lastTotalMemory)
		{
			this.gcSpikeCounter = 3;
		}
		this.lastTotalMemory = totalMemory;
		int height = this.texture.height;
		int num = (int)Math.Min((float)height, Time.deltaTime * 2500f);
		float num2 = 1f / Time.deltaTime;
		Color color;
		if (num2 > 20f)
		{
			if (num2 <= 40f)
			{
				color = new Color(1f, 1f, 0f, 0.5f);
			}
			else
			{
				color = new Color(0f, 1f, 0f, 0.5f);
			}
		}
		else if (num2 <= 10f)
		{
			color = new Color(1f, 0f, 0f, 0.5f);
		}
		else
		{
			color = new Color(1f, 0.5f, 0f, 0.5f);
		}
		Color color2 = color;
		int num3 = this.gcSpikeCounter;
		this.gcSpikeCounter = num3 - 1;
		if (num3 > 0)
		{
			color2 = Color.magenta;
		}
		for (int i = 0; i <= num; i++)
		{
			this.texture.SetPixel(this.curGraphXPos, i, color2);
		}
		for (int j = num + 1; j < height; j++)
		{
			this.texture.SetPixel(this.curGraphXPos, j, new Color(0f, 0f, 0f, 0f));
		}
		for (int k = 0; k < height; k++)
		{
			this.texture.SetPixel(this.curGraphXPos + 1, k, new Color(0f, 0f, 0f, 0f));
		}
		for (int l = 10; l <= 60; l += 10)
		{
			this.texture.SetPixel(this.curGraphXPos, (int)(2500f / (float)l), new Color(1f, 1f, 1f, 0.5f));
			this.texture.SetPixel(this.curGraphXPos, (int)(2500f / (float)l) - 1, new Color(1f, 1f, 1f, 0.5f));
		}
		this.texture.Apply(false);
		this.curGraphXPos++;
		this.curGraphXPos %= this.texture.width - 1;
	}

	// Token: 0x040079E7 RID: 31207
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bEnabled;

	// Token: 0x040079E8 RID: 31208
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public FPS fps = new FPS(0.5f);

	// Token: 0x040079E9 RID: 31209
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string format;

	// Token: 0x040079EA RID: 31210
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int BaseTextSize = 13;

	// Token: 0x040079EB RID: 31211
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bShowGraph;

	// Token: 0x040079EC RID: 31212
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D texture;

	// Token: 0x040079ED RID: 31213
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int curGraphXPos;

	// Token: 0x040079EE RID: 31214
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UITexture guiFpsGraphTexture;

	// Token: 0x040079EF RID: 31215
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public long lastTotalMemory;

	// Token: 0x040079F0 RID: 31216
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int gcSpikeCounter;

	// Token: 0x040079F1 RID: 31217
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBarHeight = 2500f;

	// Token: 0x040079F2 RID: 31218
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2i lastResolution;

	// Token: 0x040079F3 RID: 31219
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIStyle boxStyle;

	// Token: 0x040079F4 RID: 31220
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int boxAreaHeight;

	// Token: 0x040079F5 RID: 31221
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int boxAreaWidth;

	// Token: 0x040079F6 RID: 31222
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager windowManager;
}
