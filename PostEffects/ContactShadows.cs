using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostEffects
{
	// Token: 0x02001D66 RID: 7526
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public sealed class ContactShadows : MonoBehaviour
	{
		// Token: 0x17001BC1 RID: 7105
		// (get) Token: 0x0600DE7E RID: 56958 RVA: 0x004FD8DB File Offset: 0x004FBADB
		// (set) Token: 0x0600DE7F RID: 56959 RVA: 0x004FD8E3 File Offset: 0x004FBAE3
		public Light Light
		{
			get
			{
				return this._light;
			}
			set
			{
				this._light = value;
			}
		}

		// Token: 0x0600DE80 RID: 56960 RVA: 0x004FD8EC File Offset: 0x004FBAEC
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDestroy()
		{
			if (this._material != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this._material);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this._material);
				}
			}
			if (this._prevMaskRT1 != null)
			{
				RenderTexture.ReleaseTemporary(this._prevMaskRT1);
			}
			if (this._prevMaskRT2 != null)
			{
				RenderTexture.ReleaseTemporary(this._prevMaskRT2);
			}
			if (this._command1 != null)
			{
				this._command1.Release();
			}
			if (this._command2 != null)
			{
				this._command2.Release();
			}
		}

		// Token: 0x0600DE81 RID: 56961 RVA: 0x004FD97E File Offset: 0x004FBB7E
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPreCull()
		{
			this.UpdateTempObjects();
			if (this._light != null)
			{
				this.BuildCommandBuffer();
				this._light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this._command1);
				this._light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this._command2);
			}
		}

		// Token: 0x0600DE82 RID: 56962 RVA: 0x004FD9C0 File Offset: 0x004FBBC0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPreRender()
		{
			if (this._light != null)
			{
				this._light.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, this._command1);
				this._light.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, this._command2);
				this._command1.Clear();
				this._command2.Clear();
			}
		}

		// Token: 0x0600DE83 RID: 56963 RVA: 0x004FDA15 File Offset: 0x004FBC15
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}

		// Token: 0x0600DE84 RID: 56964 RVA: 0x004FDA2C File Offset: 0x004FBC2C
		[PublicizedFrom(EAccessModifier.Private)]
		public static Matrix4x4 CalculateVPMatrix()
		{
			Camera current = Camera.current;
			Matrix4x4 nonJitteredProjectionMatrix = current.nonJitteredProjectionMatrix;
			Matrix4x4 worldToCameraMatrix = current.worldToCameraMatrix;
			return GL.GetGPUProjectionMatrix(nonJitteredProjectionMatrix, true) * worldToCameraMatrix;
		}

		// Token: 0x0600DE85 RID: 56965 RVA: 0x004FDA58 File Offset: 0x004FBC58
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2Int GetScreenSize()
		{
			Camera current = Camera.current;
			int num = this._downsample ? 2 : 1;
			return new Vector2Int(current.pixelWidth / num, current.pixelHeight / num);
		}

		// Token: 0x0600DE86 RID: 56966 RVA: 0x004FDA90 File Offset: 0x004FBC90
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateTempObjects()
		{
			if (this._prevMaskRT2 != null)
			{
				RenderTexture.ReleaseTemporary(this._prevMaskRT2);
				this._prevMaskRT2 = null;
			}
			if (this._light == null)
			{
				return;
			}
			if (this._material == null)
			{
				this._material = new Material(this._shader);
				this._material.hideFlags = HideFlags.DontSave;
			}
			if (this._command1 == null)
			{
				this._command1 = new CommandBuffer();
				this._command2 = new CommandBuffer();
				this._command1.name = "Contact Shadow Ray Tracing";
				this._command2.name = "Contact Shadow Temporal Filter";
			}
			else
			{
				this._command1.Clear();
				this._command2.Clear();
			}
			this._material.SetFloat("_RejectionDepth", this._rejectionDepth);
			this._material.SetInt("_SampleCount", this._sampleCount);
			float value = Mathf.Pow(1f - this._temporalFilter, 2f);
			this._material.SetFloat("_Convergence", value);
			this._material.SetVector("_LightVector", base.transform.InverseTransformDirection(-this._light.transform.forward) * this._light.shadowBias / ((float)this._sampleCount - 1.5f));
			Texture2D texture = this._noiseTextures.GetTexture();
			Vector2 v = this.GetScreenSize() / (float)texture.width;
			this._material.SetVector("_NoiseScale", v);
			this._material.SetTexture("_NoiseTex", texture);
			this._material.SetMatrix("_Reprojection", this._previousVP * base.transform.localToWorldMatrix);
			this._previousVP = ContactShadows.CalculateVPMatrix();
		}

		// Token: 0x0600DE87 RID: 56967 RVA: 0x004FDC78 File Offset: 0x004FBE78
		[PublicizedFrom(EAccessModifier.Private)]
		public void BuildCommandBuffer()
		{
			Vector2Int screenSize = this.GetScreenSize();
			RenderTextureFormat format = RenderTextureFormat.R8;
			RenderTexture temporary = RenderTexture.GetTemporary(screenSize.x, screenSize.y, 0, format);
			if (this._temporalFilter == 0f)
			{
				this._command1.SetGlobalTexture(Shader.PropertyToID("_ShadowMask"), BuiltinRenderTextureType.CurrentActive);
				this._command1.SetRenderTarget(temporary);
				this._command1.DrawProcedural(Matrix4x4.identity, this._material, 0, MeshTopology.Triangles, 3);
			}
			else
			{
				int nameID = Shader.PropertyToID("_UnfilteredMask");
				this._command1.SetGlobalTexture(Shader.PropertyToID("_ShadowMask"), BuiltinRenderTextureType.CurrentActive);
				this._command1.GetTemporaryRT(nameID, screenSize.x, screenSize.y, 0, FilterMode.Point, format);
				this._command1.SetRenderTarget(nameID);
				this._command1.DrawProcedural(Matrix4x4.identity, this._material, 0, MeshTopology.Triangles, 3);
				this._command1.SetGlobalTexture(Shader.PropertyToID("_PrevMask"), this._prevMaskRT1);
				this._command1.SetRenderTarget(temporary);
				this._command1.DrawProcedural(Matrix4x4.identity, this._material, 1 + (Time.frameCount & 1), MeshTopology.Triangles, 3);
			}
			if (this._downsample)
			{
				this._command2.SetGlobalTexture(Shader.PropertyToID("_TempMask"), temporary);
				this._command2.DrawProcedural(Matrix4x4.identity, this._material, 3, MeshTopology.Triangles, 3);
			}
			else
			{
				this._command2.Blit(temporary, BuiltinRenderTextureType.CurrentActive);
			}
			this._prevMaskRT2 = this._prevMaskRT1;
			this._prevMaskRT1 = temporary;
		}

		// Token: 0x0400A924 RID: 43300
		[SerializeField]
		[PublicizedFrom(EAccessModifier.Private)]
		public Light _light;

		// Token: 0x0400A925 RID: 43301
		[SerializeField]
		[Range(0f, 5f)]
		[PublicizedFrom(EAccessModifier.Private)]
		public float _rejectionDepth = 0.05f;

		// Token: 0x0400A926 RID: 43302
		[SerializeField]
		[Range(4f, 32f)]
		[PublicizedFrom(EAccessModifier.Private)]
		public int _sampleCount = 16;

		// Token: 0x0400A927 RID: 43303
		[SerializeField]
		[Range(0f, 1f)]
		[PublicizedFrom(EAccessModifier.Private)]
		public float _temporalFilter = 0.5f;

		// Token: 0x0400A928 RID: 43304
		[SerializeField]
		[PublicizedFrom(EAccessModifier.Private)]
		public bool _downsample = true;

		// Token: 0x0400A929 RID: 43305
		[SerializeField]
		[HideInInspector]
		[PublicizedFrom(EAccessModifier.Private)]
		public Shader _shader;

		// Token: 0x0400A92A RID: 43306
		[SerializeField]
		[HideInInspector]
		[PublicizedFrom(EAccessModifier.Private)]
		public NoiseTextureSet _noiseTextures;

		// Token: 0x0400A92B RID: 43307
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Material _material;

		// Token: 0x0400A92C RID: 43308
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public RenderTexture _prevMaskRT1;

		// Token: 0x0400A92D RID: 43309
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public RenderTexture _prevMaskRT2;

		// Token: 0x0400A92E RID: 43310
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public CommandBuffer _command1;

		// Token: 0x0400A92F RID: 43311
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public CommandBuffer _command2;

		// Token: 0x0400A930 RID: 43312
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Matrix4x4 _previousVP = Matrix4x4.identity;
	}
}
