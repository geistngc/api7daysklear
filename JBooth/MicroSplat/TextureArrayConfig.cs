using System;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x02001D3F RID: 7487
	[CreateAssetMenu(menuName = "MicroSplat/Texture Array Config", order = 1)]
	[ExecuteInEditMode]
	public class TextureArrayConfig : ScriptableObject
	{
		// Token: 0x0600DDBA RID: 56762 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsScatter()
		{
			return false;
		}

		// Token: 0x0600DDBB RID: 56763 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsDecal()
		{
			return false;
		}

		// Token: 0x0600DDBC RID: 56764 RVA: 0x004F806C File Offset: 0x004F626C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			TextureArrayConfig.sAllConfigs.Add(this);
		}

		// Token: 0x0600DDBD RID: 56765 RVA: 0x004F8079 File Offset: 0x004F6279
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDestroy()
		{
			TextureArrayConfig.sAllConfigs.Remove(this);
		}

		// Token: 0x0600DDBE RID: 56766 RVA: 0x004F8088 File Offset: 0x004F6288
		public static TextureArrayConfig FindConfig(Texture2DArray diffuse)
		{
			for (int i = 0; i < TextureArrayConfig.sAllConfigs.Count; i++)
			{
				if (TextureArrayConfig.sAllConfigs[i].diffuseArray == diffuse)
				{
					return TextureArrayConfig.sAllConfigs[i];
				}
			}
			return null;
		}

		// Token: 0x0400A7BF RID: 42943
		public bool diffuseIsLinear;

		// Token: 0x0400A7C0 RID: 42944
		[HideInInspector]
		public bool antiTileArray;

		// Token: 0x0400A7C1 RID: 42945
		[HideInInspector]
		public bool emisMetalArray;

		// Token: 0x0400A7C2 RID: 42946
		public bool traxArray;

		// Token: 0x0400A7C3 RID: 42947
		[HideInInspector]
		public TextureArrayConfig.TextureMode textureMode = TextureArrayConfig.TextureMode.PBR;

		// Token: 0x0400A7C4 RID: 42948
		[HideInInspector]
		public TextureArrayConfig.ClusterMode clusterMode;

		// Token: 0x0400A7C5 RID: 42949
		[HideInInspector]
		public TextureArrayConfig.PackingMode packingMode;

		// Token: 0x0400A7C6 RID: 42950
		[HideInInspector]
		public TextureArrayConfig.PBRWorkflow pbrWorkflow;

		// Token: 0x0400A7C7 RID: 42951
		[HideInInspector]
		public int hash;

		// Token: 0x0400A7C8 RID: 42952
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static List<TextureArrayConfig> sAllConfigs = new List<TextureArrayConfig>();

		// Token: 0x0400A7C9 RID: 42953
		[HideInInspector]
		public Texture2DArray splatArray;

		// Token: 0x0400A7CA RID: 42954
		[HideInInspector]
		public Texture2DArray diffuseArray;

		// Token: 0x0400A7CB RID: 42955
		[HideInInspector]
		public Texture2DArray normalSAOArray;

		// Token: 0x0400A7CC RID: 42956
		[HideInInspector]
		public Texture2DArray smoothAOArray;

		// Token: 0x0400A7CD RID: 42957
		[HideInInspector]
		public Texture2DArray specularArray;

		// Token: 0x0400A7CE RID: 42958
		[HideInInspector]
		public Texture2DArray diffuseArray2;

		// Token: 0x0400A7CF RID: 42959
		[HideInInspector]
		public Texture2DArray normalSAOArray2;

		// Token: 0x0400A7D0 RID: 42960
		[HideInInspector]
		public Texture2DArray smoothAOArray2;

		// Token: 0x0400A7D1 RID: 42961
		[HideInInspector]
		public Texture2DArray specularArray2;

		// Token: 0x0400A7D2 RID: 42962
		[HideInInspector]
		public Texture2DArray diffuseArray3;

		// Token: 0x0400A7D3 RID: 42963
		[HideInInspector]
		public Texture2DArray normalSAOArray3;

		// Token: 0x0400A7D4 RID: 42964
		[HideInInspector]
		public Texture2DArray smoothAOArray3;

		// Token: 0x0400A7D5 RID: 42965
		[HideInInspector]
		public Texture2DArray specularArray3;

		// Token: 0x0400A7D6 RID: 42966
		[HideInInspector]
		public Texture2DArray emisArray;

		// Token: 0x0400A7D7 RID: 42967
		[HideInInspector]
		public Texture2DArray emisArray2;

		// Token: 0x0400A7D8 RID: 42968
		[HideInInspector]
		public Texture2DArray emisArray3;

		// Token: 0x0400A7D9 RID: 42969
		public TextureArrayConfig.TextureArrayGroup defaultTextureSettings = new TextureArrayConfig.TextureArrayGroup();

		// Token: 0x0400A7DA RID: 42970
		public List<TextureArrayConfig.PlatformTextureOverride> platformOverrides = new List<TextureArrayConfig.PlatformTextureOverride>();

		// Token: 0x0400A7DB RID: 42971
		public TextureArrayConfig.SourceTextureSize sourceTextureSize;

		// Token: 0x0400A7DC RID: 42972
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelHeight = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x0400A7DD RID: 42973
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelSmoothness = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x0400A7DE RID: 42974
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelAO = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x0400A7DF RID: 42975
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x0400A7E0 RID: 42976
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures2 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x0400A7E1 RID: 42977
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures3 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x02001D40 RID: 7488
		public enum AllTextureChannel
		{
			// Token: 0x0400A7E3 RID: 42979
			R,
			// Token: 0x0400A7E4 RID: 42980
			G,
			// Token: 0x0400A7E5 RID: 42981
			B,
			// Token: 0x0400A7E6 RID: 42982
			A,
			// Token: 0x0400A7E7 RID: 42983
			Custom
		}

		// Token: 0x02001D41 RID: 7489
		public enum TextureChannel
		{
			// Token: 0x0400A7E9 RID: 42985
			R,
			// Token: 0x0400A7EA RID: 42986
			G,
			// Token: 0x0400A7EB RID: 42987
			B,
			// Token: 0x0400A7EC RID: 42988
			A
		}

		// Token: 0x02001D42 RID: 7490
		public enum Compression
		{
			// Token: 0x0400A7EE RID: 42990
			AutomaticCompressed,
			// Token: 0x0400A7EF RID: 42991
			ForceDXT,
			// Token: 0x0400A7F0 RID: 42992
			ForcePVR,
			// Token: 0x0400A7F1 RID: 42993
			ForceETC2,
			// Token: 0x0400A7F2 RID: 42994
			ForceASTC,
			// Token: 0x0400A7F3 RID: 42995
			ForceCrunch,
			// Token: 0x0400A7F4 RID: 42996
			Uncompressed
		}

		// Token: 0x02001D43 RID: 7491
		public enum TextureSize
		{
			// Token: 0x0400A7F6 RID: 42998
			k4096 = 4096,
			// Token: 0x0400A7F7 RID: 42999
			k2048 = 2048,
			// Token: 0x0400A7F8 RID: 43000
			k1024 = 1024,
			// Token: 0x0400A7F9 RID: 43001
			k512 = 512,
			// Token: 0x0400A7FA RID: 43002
			k256 = 256,
			// Token: 0x0400A7FB RID: 43003
			k128 = 128,
			// Token: 0x0400A7FC RID: 43004
			k64 = 64,
			// Token: 0x0400A7FD RID: 43005
			k32 = 32
		}

		// Token: 0x02001D44 RID: 7492
		[Serializable]
		public class TextureArraySettings
		{
			// Token: 0x0600DDC1 RID: 56769 RVA: 0x004F8142 File Offset: 0x004F6342
			public TextureArraySettings(TextureArrayConfig.TextureSize s, TextureArrayConfig.Compression c, FilterMode f, int a = 1)
			{
				this.textureSize = s;
				this.compression = c;
				this.filterMode = f;
				this.Aniso = a;
			}

			// Token: 0x0400A7FE RID: 43006
			public TextureArrayConfig.TextureSize textureSize;

			// Token: 0x0400A7FF RID: 43007
			public TextureArrayConfig.Compression compression;

			// Token: 0x0400A800 RID: 43008
			public FilterMode filterMode;

			// Token: 0x0400A801 RID: 43009
			[Range(0f, 16f)]
			public int Aniso = 1;
		}

		// Token: 0x02001D45 RID: 7493
		public enum PBRWorkflow
		{
			// Token: 0x0400A803 RID: 43011
			Metallic,
			// Token: 0x0400A804 RID: 43012
			Specular
		}

		// Token: 0x02001D46 RID: 7494
		public enum PackingMode
		{
			// Token: 0x0400A806 RID: 43014
			Fastest,
			// Token: 0x0400A807 RID: 43015
			Quality
		}

		// Token: 0x02001D47 RID: 7495
		public enum SourceTextureSize
		{
			// Token: 0x0400A809 RID: 43017
			Unchanged,
			// Token: 0x0400A80A RID: 43018
			k32 = 32,
			// Token: 0x0400A80B RID: 43019
			k256 = 256
		}

		// Token: 0x02001D48 RID: 7496
		public enum TextureMode
		{
			// Token: 0x0400A80D RID: 43021
			Basic,
			// Token: 0x0400A80E RID: 43022
			PBR
		}

		// Token: 0x02001D49 RID: 7497
		public enum ClusterMode
		{
			// Token: 0x0400A810 RID: 43024
			None,
			// Token: 0x0400A811 RID: 43025
			TwoVariations,
			// Token: 0x0400A812 RID: 43026
			ThreeVariations
		}

		// Token: 0x02001D4A RID: 7498
		[Serializable]
		public class TextureArrayGroup
		{
			// Token: 0x0400A813 RID: 43027
			public TextureArrayConfig.TextureArraySettings diffuseSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400A814 RID: 43028
			public TextureArrayConfig.TextureArraySettings normalSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Trilinear, 1);

			// Token: 0x0400A815 RID: 43029
			public TextureArrayConfig.TextureArraySettings smoothSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400A816 RID: 43030
			public TextureArrayConfig.TextureArraySettings antiTileSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400A817 RID: 43031
			public TextureArrayConfig.TextureArraySettings emissiveSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400A818 RID: 43032
			public TextureArrayConfig.TextureArraySettings specularSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400A819 RID: 43033
			public TextureArrayConfig.TextureArraySettings traxDiffuseSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400A81A RID: 43034
			public TextureArrayConfig.TextureArraySettings traxNormalSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400A81B RID: 43035
			public TextureArrayConfig.TextureArraySettings decalSplatSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
		}

		// Token: 0x02001D4B RID: 7499
		[Serializable]
		public class PlatformTextureOverride
		{
			// Token: 0x0400A81C RID: 43036
			public TextureArrayConfig.TextureArrayGroup settings = new TextureArrayConfig.TextureArrayGroup();
		}

		// Token: 0x02001D4C RID: 7500
		[Serializable]
		public class TextureEntry
		{
			// Token: 0x0600DDC4 RID: 56772 RVA: 0x004F8244 File Offset: 0x004F6444
			public void Reset()
			{
				this.diffuse = null;
				this.height = null;
				this.normal = null;
				this.smoothness = null;
				this.specular = null;
				this.ao = null;
				this.isRoughness = false;
				this.detailNoise = null;
				this.distanceNoise = null;
				this.metal = null;
				this.emis = null;
				this.heightChannel = TextureArrayConfig.TextureChannel.G;
				this.smoothnessChannel = TextureArrayConfig.TextureChannel.G;
				this.aoChannel = TextureArrayConfig.TextureChannel.G;
				this.distanceChannel = TextureArrayConfig.TextureChannel.G;
				this.detailChannel = TextureArrayConfig.TextureChannel.G;
				this.traxDiffuse = null;
				this.traxNormal = null;
				this.traxHeight = null;
				this.traxSmoothness = null;
				this.traxAO = null;
				this.traxHeightChannel = TextureArrayConfig.TextureChannel.G;
				this.traxSmoothnessChannel = TextureArrayConfig.TextureChannel.G;
				this.traxAOChannel = TextureArrayConfig.TextureChannel.G;
				this.splat = null;
			}

			// Token: 0x0600DDC5 RID: 56773 RVA: 0x004F8300 File Offset: 0x004F6500
			public bool HasTextures(TextureArrayConfig.PBRWorkflow wf)
			{
				if (wf == TextureArrayConfig.PBRWorkflow.Specular)
				{
					return this.diffuse != null || this.height != null || this.normal != null || this.smoothness != null || this.specular != null || this.ao != null;
				}
				return this.diffuse != null || this.height != null || this.normal != null || this.smoothness != null || this.metal != null || this.ao != null;
			}

			// Token: 0x0400A81D RID: 43037
			public Texture2D diffuse;

			// Token: 0x0400A81E RID: 43038
			public Texture2D height;

			// Token: 0x0400A81F RID: 43039
			public TextureArrayConfig.TextureChannel heightChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A820 RID: 43040
			public Texture2D normal;

			// Token: 0x0400A821 RID: 43041
			public Texture2D smoothness;

			// Token: 0x0400A822 RID: 43042
			public TextureArrayConfig.TextureChannel smoothnessChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A823 RID: 43043
			public bool isRoughness;

			// Token: 0x0400A824 RID: 43044
			public Texture2D ao;

			// Token: 0x0400A825 RID: 43045
			public TextureArrayConfig.TextureChannel aoChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A826 RID: 43046
			public Texture2D emis;

			// Token: 0x0400A827 RID: 43047
			public Texture2D metal;

			// Token: 0x0400A828 RID: 43048
			public TextureArrayConfig.TextureChannel metalChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A829 RID: 43049
			public Texture2D specular;

			// Token: 0x0400A82A RID: 43050
			public Texture2D noiseNormal;

			// Token: 0x0400A82B RID: 43051
			public Texture2D detailNoise;

			// Token: 0x0400A82C RID: 43052
			public TextureArrayConfig.TextureChannel detailChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A82D RID: 43053
			public Texture2D distanceNoise;

			// Token: 0x0400A82E RID: 43054
			public TextureArrayConfig.TextureChannel distanceChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A82F RID: 43055
			public Texture2D traxDiffuse;

			// Token: 0x0400A830 RID: 43056
			public Texture2D traxHeight;

			// Token: 0x0400A831 RID: 43057
			public TextureArrayConfig.TextureChannel traxHeightChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A832 RID: 43058
			public Texture2D traxNormal;

			// Token: 0x0400A833 RID: 43059
			public Texture2D traxSmoothness;

			// Token: 0x0400A834 RID: 43060
			public TextureArrayConfig.TextureChannel traxSmoothnessChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A835 RID: 43061
			public bool traxIsRoughness;

			// Token: 0x0400A836 RID: 43062
			public Texture2D traxAO;

			// Token: 0x0400A837 RID: 43063
			public TextureArrayConfig.TextureChannel traxAOChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400A838 RID: 43064
			public Texture2D splat;
		}
	}
}
