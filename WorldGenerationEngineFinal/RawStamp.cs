using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001724 RID: 5924
	[BurstCompile(CompileSynchronously = true)]
	public class RawStamp
	{
		// Token: 0x0600B84F RID: 47183 RVA: 0x004491B7 File Offset: 0x004473B7
		[BurstCompile(CompileSynchronously = true)]
		public static void SmoothAlpha(ref RawStamp.Data d, int _boxSize)
		{
			RawStamp.SmoothAlpha_0000B802$BurstDirectCall.Invoke(ref d, _boxSize);
		}

		// Token: 0x0600B850 RID: 47184 RVA: 0x004491C0 File Offset: 0x004473C0
		public void BoxAlpha()
		{
			for (int i = 0; i < this.height; i += 4)
			{
				for (int j = 0; j < this.width; j += 4)
				{
					int num = j + i * this.width;
					double num2 = 0.0;
					for (int k = 0; k < 4; k++)
					{
						for (int l = 0; l < 4; l++)
						{
							num2 += (double)this.data.alphaPixels[num + l + k * this.width];
						}
					}
					num2 /= 16.0;
					for (int m = 0; m < 4; m++)
					{
						for (int n = 0; n < 4; n++)
						{
							this.data.alphaPixels[num + n + m * this.width] = (float)num2;
						}
					}
				}
			}
		}

		// Token: 0x0600B851 RID: 47185 RVA: 0x0044929E File Offset: 0x0044749E
		public void Clear()
		{
			this.data.heightPixels.Dispose();
			this.data.alphaPixels.Dispose();
			this.data.waterPixels.Dispose();
		}

		// Token: 0x17001665 RID: 5733
		// (get) Token: 0x0600B852 RID: 47186 RVA: 0x004492D0 File Offset: 0x004474D0
		public int width
		{
			get
			{
				return this.data.width;
			}
		}

		// Token: 0x17001666 RID: 5734
		// (get) Token: 0x0600B853 RID: 47187 RVA: 0x004492DD File Offset: 0x004474DD
		public int height
		{
			get
			{
				return this.data.height;
			}
		}

		// Token: 0x0600B855 RID: 47189 RVA: 0x004492EC File Offset: 0x004474EC
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SmoothAlpha$BurstManaged(ref RawStamp.Data d, int _boxSize)
		{
			NativeArray<float> alphaPixels = new NativeArray<float>(d.alphaPixels.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			for (int i = 0; i < d.height; i++)
			{
				for (int j = 0; j < d.width; j++)
				{
					float num = 0f;
					int num2 = 0;
					for (int k = -1; k < _boxSize; k++)
					{
						int num3 = i + k;
						if (num3 < d.height)
						{
							for (int l = -1; l < _boxSize; l++)
							{
								int num4 = j + l;
								if (num4 < d.width)
								{
									num += d.alphaPixels[num4 + num3 * d.width];
									num2++;
								}
							}
						}
					}
					num /= (float)num2;
					alphaPixels[j + i * d.width] = num;
				}
			}
			d.alphaPixels.Dispose();
			d.alphaPixels = alphaPixels;
		}

		// Token: 0x04008A08 RID: 35336
		public string name;

		// Token: 0x04008A09 RID: 35337
		public RawStamp.Data data;

		// Token: 0x02001725 RID: 5925
		public struct Data
		{
			// Token: 0x04008A0A RID: 35338
			public float heightConst;

			// Token: 0x04008A0B RID: 35339
			public NativeArray<float> heightPixels;

			// Token: 0x04008A0C RID: 35340
			public float alphaConst;

			// Token: 0x04008A0D RID: 35341
			public NativeArray<float> alphaPixels;

			// Token: 0x04008A0E RID: 35342
			public NativeArray<float> waterPixels;

			// Token: 0x04008A0F RID: 35343
			public int width;

			// Token: 0x04008A10 RID: 35344
			public int height;
		}

		// Token: 0x02001726 RID: 5926
		// (Invoke) Token: 0x0600B857 RID: 47191
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate void SmoothAlpha_0000B802$PostfixBurstDelegate(ref RawStamp.Data d, int _boxSize);

		// Token: 0x02001727 RID: 5927
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class SmoothAlpha_0000B802$BurstDirectCall
		{
			// Token: 0x0600B85A RID: 47194 RVA: 0x004493CE File Offset: 0x004475CE
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (RawStamp.SmoothAlpha_0000B802$BurstDirectCall.Pointer == 0)
				{
					RawStamp.SmoothAlpha_0000B802$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(RawStamp.SmoothAlpha_0000B802$BurstDirectCall.DeferredCompilation, methodof(RawStamp.SmoothAlpha$BurstManaged(RawStamp.Data*, int)).MethodHandle, typeof(RawStamp.SmoothAlpha_0000B802$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = RawStamp.SmoothAlpha_0000B802$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B85B RID: 47195 RVA: 0x004493FC File Offset: 0x004475FC
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				RawStamp.SmoothAlpha_0000B802$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B85C RID: 47196 RVA: 0x00449414 File Offset: 0x00447614
			public unsafe static void Constructor()
			{
				RawStamp.SmoothAlpha_0000B802$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(RawStamp.SmoothAlpha(RawStamp.Data*, int)).MethodHandle);
			}

			// Token: 0x0600B85D RID: 47197 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B85E RID: 47198 RVA: 0x00449425 File Offset: 0x00447625
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static SmoothAlpha_0000B802$BurstDirectCall()
			{
				RawStamp.SmoothAlpha_0000B802$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B85F RID: 47199 RVA: 0x0044942C File Offset: 0x0044762C
			public static void Invoke(ref RawStamp.Data d, int _boxSize)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = RawStamp.SmoothAlpha_0000B802$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(WorldGenerationEngineFinal.RawStamp/Data&,System.Int32), ref d, _boxSize, functionPointer);
						return;
					}
				}
				RawStamp.SmoothAlpha$BurstManaged(ref d, _boxSize);
			}

			// Token: 0x04008A11 RID: 35345
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x04008A12 RID: 35346
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}
	}
}
