using System;
using UnityEngine;

// Token: 0x0200133A RID: 4922
public class FurCombPainter
{
	// Token: 0x06009B84 RID: 39812 RVA: 0x003AB44C File Offset: 0x003A964C
	public void Paint(Texture2D texture, Vector2 uv, Vector3 tangentDirection, BrushSettings brushSettings, FurCombPainter.PaintMode paintMode)
	{
		if (this.texturePaintComputeShader == null)
		{
			this.texturePaintComputeShader = Resources.Load<ComputeShader>("Shaders/TexturePaintCompute");
		}
		if (this.texturePaintComputeShader == null)
		{
			Debug.LogError("TexturePaintCompute.compute not found in Resources folder");
			return;
		}
		int kernelIndex = this.texturePaintComputeShader.FindKernel("ChannelWrite");
		int num = Mathf.FloorToInt(uv.x * (float)texture.width);
		int num2 = Mathf.FloorToInt(uv.y * (float)texture.height);
		RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
		renderTexture.enableRandomWrite = true;
		renderTexture.useMipMap = (texture.mipmapCount > 1);
		renderTexture.Create();
		Graphics.CopyTexture(texture, renderTexture);
		Vector3 vector = tangentDirection * 0.5f + Vector3.one * 0.5f;
		this.texturePaintComputeShader.SetInts("PaintPosition", new int[]
		{
			num,
			num2
		});
		this.texturePaintComputeShader.SetInt("BrushRadius", (int)brushSettings.Size);
		this.texturePaintComputeShader.SetFloat("BrushStrength", brushSettings.Strength);
		this.texturePaintComputeShader.SetFloat("BrushFalloff", brushSettings.Falloff);
		this.texturePaintComputeShader.SetVector("BrushColor", brushSettings.Color);
		switch (paintMode)
		{
		case FurCombPainter.PaintMode.Albedo:
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(brushSettings.Color.r, brushSettings.Color.g, brushSettings.Color.b, brushSettings.Color.a));
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(1f, 1f, 1f, 0f));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		case FurCombPainter.PaintMode.Direction:
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(vector.x, vector.y, 0f, 0f));
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(1f, 1f, 0f, 0f));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		case FurCombPainter.PaintMode.Matting:
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(0f, 0f, 0f, 1f));
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(0f, 0f, 0f, brushSettings.Matting));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		case FurCombPainter.PaintMode.DirectionAndMatting:
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(1f, 1f, 0f, 1f));
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(vector.x, vector.y, 0f, brushSettings.Matting));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		case FurCombPainter.PaintMode.Length:
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(0f, 0f, 1f, 0f));
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(0f, 0f, brushSettings.Length, 0f));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		case FurCombPainter.PaintMode.Roughness:
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(1f, 0f, 0f, 0f));
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(brushSettings.Roughness, 0f, 0f, 0f));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		case FurCombPainter.PaintMode.Metallic:
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(0f, 1f, 0f, 0f));
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(0f, brushSettings.Metallic, 0f, 0f));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		case FurCombPainter.PaintMode.Occlusion:
			this.texturePaintComputeShader.SetVector("ChannelMask", new Vector4(0f, 0f, 1f, 0f));
			this.texturePaintComputeShader.SetVector("BrushValue", new Vector4(0f, 0f, brushSettings.Occlusion, 0f));
			this.texturePaintComputeShader.SetTexture(kernelIndex, "Result", renderTexture);
			this.texturePaintComputeShader.Dispatch(kernelIndex, texture.width / 8, texture.height / 8, 1);
			break;
		}
		RenderTexture.active = renderTexture;
		texture.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
		texture.Apply();
		RenderTexture.active = null;
		renderTexture.Release();
	}

	// Token: 0x04007536 RID: 30006
	public ComputeShader texturePaintComputeShader;

	// Token: 0x0200133B RID: 4923
	public enum PaintMode
	{
		// Token: 0x04007538 RID: 30008
		Albedo,
		// Token: 0x04007539 RID: 30009
		Direction,
		// Token: 0x0400753A RID: 30010
		Matting,
		// Token: 0x0400753B RID: 30011
		DirectionAndMatting,
		// Token: 0x0400753C RID: 30012
		Length,
		// Token: 0x0400753D RID: 30013
		Roughness,
		// Token: 0x0400753E RID: 30014
		Metallic,
		// Token: 0x0400753F RID: 30015
		Occlusion
	}
}
