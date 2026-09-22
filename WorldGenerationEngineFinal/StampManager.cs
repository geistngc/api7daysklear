using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200172B RID: 5931
	[BurstCompile(CompileSynchronously = true)]
	public class StampManager
	{
		// Token: 0x0600B875 RID: 47221 RVA: 0x004499B5 File Offset: 0x00447BB5
		public StampManager(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600B876 RID: 47222 RVA: 0x004499DC File Offset: 0x00447BDC
		public void ClearStamps()
		{
			foreach (RawStamp rawStamp in this.AllStamps.Values)
			{
				rawStamp.Clear();
			}
			this.AllStamps.Clear();
		}

		// Token: 0x0600B877 RID: 47223 RVA: 0x00449A3C File Offset: 0x00447C3C
		public void DrawStampGroup(StampGroup _group, ref NativeArray<float> _dest, int size)
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			for (int i = 0; i < _group.Stamps.Count; i++)
			{
				Stamp stamp = _group.Stamps[i];
				if (stamp != null)
				{
					this.DrawStamp(ref _dest, stamp);
					if (this.worldBuilder.IsCanceled)
					{
						break;
					}
				}
			}
			Log.Out("DrawStampGroup '{0}', count {1}, in {2}", new object[]
			{
				_group.Name,
				_group.Stamps.Count,
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
		}

		// Token: 0x0600B878 RID: 47224 RVA: 0x00449ACC File Offset: 0x00447CCC
		public void DrawStampGroup(StampGroup _group, Color32[] _image, int size, float _stampScale = 1f)
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			for (int i = 0; i < _group.Stamps.Count; i++)
			{
				Stamp stamp = _group.Stamps[i];
				if (stamp != null)
				{
					int x = (int)((float)stamp.transform.x * _stampScale);
					int y = (int)((float)stamp.transform.y * _stampScale);
					StampManager.DrawStamp(_image, ref stamp.stamp.data, x, y, size, size, stamp.imageWidth, stamp.imageHeight, stamp.alpha, stamp.scale * _stampScale, stamp.customColor, stamp.alphaCutoff, (float)stamp.transform.rotation, stamp.isWater);
					if (this.worldBuilder.IsCanceled)
					{
						break;
					}
				}
			}
			Log.Out("DrawStampGroup c32 '{0}', count {1}, in {2}", new object[]
			{
				_group.Name,
				_group.Stamps.Count,
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
		}

		// Token: 0x0600B879 RID: 47225 RVA: 0x00449BCC File Offset: 0x00447DCC
		public void DrawWaterStampGroup(StampGroup _group, ref NativeArray<float> _dest, int _destSize)
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			for (int i = 0; i < _group.Stamps.Count; i++)
			{
				Stamp stamp = _group.Stamps[i];
				if (stamp != null)
				{
					StampManager.DrawWaterStamp(stamp, ref _dest, _destSize);
					if (this.worldBuilder.IsCanceled)
					{
						break;
					}
				}
			}
			Log.Out("DrawWaterStampGroup '{0}', count {1}, in {2}", new object[]
			{
				_group.Name,
				_group.Stamps.Count,
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
		}

		// Token: 0x0600B87A RID: 47226 RVA: 0x00449C5C File Offset: 0x00447E5C
		[PublicizedFrom(EAccessModifier.Private)]
		public static float CalcRotatedValue(float x1, float y1, ref NativeArray<float> src, float sine, float cosine, int width, int height, bool isWater = false)
		{
			int num = width >> 1;
			int num2 = height >> 1;
			float num3 = cosine * (x1 - (float)num) + sine * (y1 - (float)num2) + (float)num;
			int num4 = (int)num3;
			if ((ulong)num4 >= (ulong)((long)width))
			{
				return 0f;
			}
			float num5 = -sine * (x1 - (float)num) + cosine * (y1 - (float)num2) + (float)num2;
			int num6 = (int)num5;
			if ((ulong)num6 >= (ulong)((long)height))
			{
				return 0f;
			}
			int num7 = num4 + num6 * width;
			float num8 = src[num7];
			float num9;
			if (num4 + 1 < width)
			{
				num9 = src[num7 + 1];
			}
			else
			{
				num9 = num8;
			}
			float num10;
			if (num6 + 1 < height)
			{
				num10 = src[num7 + width];
			}
			else
			{
				num10 = num8;
			}
			float num11;
			if (num4 + 1 < width && num6 + 1 < height)
			{
				num11 = src[num7 + width + 1];
			}
			else
			{
				num11 = num8;
			}
			if (isWater && (num8 > 0f || num10 > 0f || num9 > 0f || num11 > 0f))
			{
				return num8;
			}
			num3 -= (float)num4;
			num5 -= (float)num6;
			num8 += (num9 - num8) * num3;
			float num12 = num10 + (num11 - num10) * num3;
			return num8 + (num12 - num8) * num5;
		}

		// Token: 0x0600B87B RID: 47227 RVA: 0x00449D88 File Offset: 0x00447F88
		[PublicizedFrom(EAccessModifier.Private)]
		public static float CalcRotatedValue(ref StampManager.RotateParams _p, float x1, float y1, ref NativeArray<float> src)
		{
			float num = _p.cosine * (x1 - _p.halfWidth) + _p.sine * (y1 - _p.halfHeight) + _p.halfWidth;
			int num2 = (int)num;
			if ((ulong)num2 >= (ulong)((long)_p.width))
			{
				return 0f;
			}
			float num3 = -_p.sine * (x1 - _p.halfWidth) + _p.cosine * (y1 - _p.halfHeight) + _p.halfHeight;
			int num4 = (int)num3;
			if ((ulong)num4 >= (ulong)((long)_p.height))
			{
				return 0f;
			}
			int num5 = num2 + num4 * _p.width;
			float num6 = src[num5];
			float num7 = num6;
			if (num2 + 1 < _p.width)
			{
				num7 = src[num5 + 1];
			}
			float num8 = num6;
			if (num4 + 1 < _p.height)
			{
				num8 = src[num5 + _p.width];
			}
			float num9 = num6;
			if (num2 + 1 < _p.width && num4 + 1 < _p.height)
			{
				num9 = src[num5 + _p.width + 1];
			}
			if (_p.isWater && (num6 > 0f || num8 > 0f || num7 > 0f || num9 > 0f))
			{
				return num6;
			}
			num -= (float)num2;
			num3 -= (float)num4;
			float num10 = num6 + (num7 - num6) * num;
			float num11 = num8 + (num9 - num8) * num;
			return num10 + (num11 - num10) * num3;
		}

		// Token: 0x0600B87C RID: 47228 RVA: 0x00449EEC File Offset: 0x004480EC
		public void DrawStamp(ref NativeArray<float> _dest, Stamp stamp)
		{
			int x = stamp.transform.x;
			int y = stamp.transform.y;
			int worldSize = this.worldBuilder.WorldSize;
			float angle = (float)stamp.transform.rotation;
			StampManager.DrawStamp(ref _dest, ref stamp.stamp.data, x, y, worldSize, worldSize, stamp.alpha, stamp.additive, stamp.scale, ref stamp.customColor, stamp.alphaCutoff, angle);
		}

		// Token: 0x0600B87D RID: 47229 RVA: 0x00449F60 File Offset: 0x00448160
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void DrawStamp(ref NativeArray<float> _dest, ref RawStamp.Data _src, int _x, int _y, int _destWidth, int _destHeight, float _alphaScale, bool _additive, float _scale, ref Color32 _customColor, float _alphaCutoff, float _angle)
		{
			StampManager.DrawStamp_0000B825$BurstDirectCall.Invoke(ref _dest, ref _src, _x, _y, _destWidth, _destHeight, _alphaScale, _additive, _scale, ref _customColor, _alphaCutoff, _angle);
		}

		// Token: 0x0600B87E RID: 47230 RVA: 0x00449F88 File Offset: 0x00448188
		[PublicizedFrom(EAccessModifier.Private)]
		public static void DrawStamp(Color32[] _dest, ref RawStamp.Data _src, int _x, int _y, int _destWidth, int _destHeight, int _srcWidth, int _srcHeight, float _alpha, float _scale, Color32 _customColor, float _alphaCutoff = 0.1f, float _angle = 0f, bool isWater = false)
		{
			_x -= (int)((float)_srcWidth * _scale) / 2;
			_y -= (int)((float)_srcHeight * _scale) / 2;
			double num = (double)(0.017453292f * _angle);
			float sine = (float)Math.Sin(num);
			float cosine = (float)Math.Cos(num);
			int num2 = Mathf.FloorToInt((float)(((int)Mathf.Sqrt((float)(_srcWidth * _srcWidth + _srcHeight * _srcHeight)) - _srcWidth) / 2) * _scale);
			int num3 = Mathf.FloorToInt((float)_srcWidth * _scale + (float)num2);
			num2 = -num2;
			int num4 = num2;
			int num5 = _x + num2;
			if (num5 < 0)
			{
				num4 -= num5;
			}
			int num6 = num3;
			num5 = _x + num3;
			if (num5 >= _destWidth)
			{
				num6 -= num5 - _destWidth;
			}
			int num7 = num2;
			int num8 = _y + num2;
			if (num8 < 0)
			{
				num7 -= num8;
			}
			int num9 = num3;
			num8 = _y + num3;
			if (num8 >= _destHeight)
			{
				num9 -= num8 - _destHeight;
			}
			for (int i = num7; i < num9; i++)
			{
				int num10 = (_y + i) * _destWidth;
				float y = (float)i / _scale;
				for (int j = num4; j < num6; j++)
				{
					float num11 = _src.alphaConst;
					if (_src.alphaPixels.IsCreated)
					{
						num11 = StampManager.CalcRotatedValue((float)j / _scale, y, ref _src.alphaPixels, sine, cosine, _srcWidth, _srcHeight, isWater);
					}
					if (num11 > _alphaCutoff)
					{
						int num12 = _x + j + num10;
						_dest[num12] = _customColor;
					}
				}
			}
		}

		// Token: 0x0600B87F RID: 47231 RVA: 0x0044A0D4 File Offset: 0x004482D4
		public static void DrawWaterStamp(Stamp stamp, ref NativeArray<float> _dest, int _destSize)
		{
			if (!stamp.isWater)
			{
				throw new ArgumentException("DrawWaterStamp called with non-water stamp " + stamp.Name);
			}
			NativeArray<float> nativeArray = stamp.stamp.data.waterPixels;
			if (!nativeArray.IsCreated)
			{
				nativeArray = stamp.stamp.data.alphaPixels;
				if (!nativeArray.IsCreated)
				{
					return;
				}
			}
			int x = stamp.transform.x;
			int y = stamp.transform.y;
			float angle = (float)stamp.transform.rotation;
			StampManager.DrawWaterStamp(ref _dest, ref nativeArray, x, y, _destSize, _destSize, stamp.imageWidth, stamp.imageHeight, stamp.alpha, stamp.scale, (float)stamp.customColor.b, angle);
		}

		// Token: 0x0600B880 RID: 47232 RVA: 0x0044A18C File Offset: 0x0044838C
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void DrawWaterStamp(ref NativeArray<float> _dest, ref NativeArray<float> _src, int _x, int _y, int _destWidth, int _destHeight, int _srcWidth, int _srcHeight, float _alpha, float _scale, float _customColor, float _angle)
		{
			StampManager.DrawWaterStamp_0000B828$BurstDirectCall.Invoke(ref _dest, ref _src, _x, _y, _destWidth, _destHeight, _srcWidth, _srcHeight, _alpha, _scale, _customColor, _angle);
		}

		// Token: 0x0600B881 RID: 47233 RVA: 0x0044A1B4 File Offset: 0x004483B4
		public static void DrawBiomeStamp(Color32[] _dest, ref NativeArray<float> _src, int _x, int _y, int _destWidth, int _destHeight, int _srcWidth, int _srcHeight, float _scale, Color32 _destColor, float _alphaCutoff = 0.1f, float _angle = 0f)
		{
			_x -= (int)((float)_srcWidth * _scale) / 2;
			_y -= (int)((float)_srcHeight * _scale) / 2;
			double num = (double)(0.017453292f * _angle);
			float sine = (float)Math.Sin(num);
			float cosine = (float)Math.Cos(num);
			int num2 = Mathf.FloorToInt((float)(((int)Mathf.Sqrt((float)(_srcWidth * _srcWidth + _srcHeight * _srcHeight)) - _srcWidth) / 2) * _scale);
			int num3 = Mathf.FloorToInt((float)_srcWidth * _scale + (float)num2);
			num2 = -num2;
			int num4 = num2;
			int num5 = _x + num2;
			if (num5 < 0)
			{
				num4 -= num5;
			}
			int num6 = num3;
			num5 = _x + num3;
			if (num5 >= _destWidth)
			{
				num6 -= num5 - _destWidth;
			}
			int num7 = num2;
			int num8 = _y + num2;
			if (num8 < 0)
			{
				num7 -= num8;
			}
			int num9 = num3;
			num8 = _y + num3;
			if (num8 >= _destHeight)
			{
				num9 -= num8 - _destHeight;
			}
			for (int i = num7; i < num9; i++)
			{
				int num10 = (_y + i) * _destWidth;
				float y = (float)i / _scale;
				for (int j = num4; j < num6; j++)
				{
					int num11 = _x + j + num10;
					if (StampManager.CalcRotatedValue((float)j / _scale, y, ref _src, sine, cosine, _srcWidth, _srcHeight, false) > _alphaCutoff)
					{
						_dest[num11] = _destColor;
					}
				}
			}
		}

		// Token: 0x0600B882 RID: 47234 RVA: 0x0044A2DF File Offset: 0x004484DF
		public bool TryGetStamp(string terrainTypeName, string comboTypeName, out RawStamp tmp)
		{
			return this.TryGetStamp(comboTypeName, out tmp) || this.TryGetStamp(terrainTypeName, out tmp);
		}

		// Token: 0x0600B883 RID: 47235 RVA: 0x0044A2F8 File Offset: 0x004484F8
		public bool TryGetStamp(string _baseName, out RawStamp _output)
		{
			return this.TryGetStamp(_baseName, out _output, Rand.Instance);
		}

		// Token: 0x0600B884 RID: 47236 RVA: 0x0044A308 File Offset: 0x00448508
		public bool TryGetStamp(string _baseName, out RawStamp _output, Rand _rand)
		{
			List<RawStamp> obj = this.tempGetStampList;
			bool result;
			lock (obj)
			{
				foreach (KeyValuePair<string, RawStamp> keyValuePair in this.AllStamps)
				{
					if (keyValuePair.Key.StartsWith(_baseName))
					{
						this.tempGetStampList.Add(keyValuePair.Value);
					}
				}
				if (this.tempGetStampList.Count == 0)
				{
					_output = null;
					result = false;
				}
				else
				{
					if (this.tempGetStampList.Count == 1)
					{
						_output = this.tempGetStampList[0];
					}
					else
					{
						_output = this.tempGetStampList[_rand.Range(this.tempGetStampList.Count)];
					}
					this.tempGetStampList.Clear();
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600B885 RID: 47237 RVA: 0x0044A400 File Offset: 0x00448600
		public RawStamp GetStamp(string _baseName, Rand _rand = null)
		{
			if (_rand == null)
			{
				_rand = Rand.Instance;
			}
			RawStamp result;
			if (!this.TryGetStamp(_baseName, out result, _rand))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600B886 RID: 47238 RVA: 0x0044A426 File Offset: 0x00448626
		public IEnumerator LoadStamps()
		{
			if (this.AllStamps.Count > 0)
			{
				yield break;
			}
			MicroStopwatch ms = new MicroStopwatch(true);
			this.AllStamps.Clear();
			List<PathAbstractions.AbstractedLocation> stampMaps = PathAbstractions.RwgStampsSearchPaths.GetAvailablePathsList(null, true, null);
			MicroStopwatch msRestart = new MicroStopwatch(true);
			int num;
			for (int i = 0; i < stampMaps.Count; i = num + 1)
			{
				string extension = stampMaps[i].Extension;
				if (!(extension != ".exr") || !(extension != ".raw"))
				{
					this.LoadStamp(stampMaps[i]);
					if (msRestart.ElapsedMilliseconds > 500L)
					{
						yield return null;
						msRestart.ResetAndRestart();
					}
				}
				num = i;
			}
			for (int i = 0; i < stampMaps.Count; i = num + 1)
			{
				this.LoadStamp(stampMaps[i]);
				if (msRestart.ElapsedMilliseconds > 500L)
				{
					yield return null;
					msRestart.ResetAndRestart();
				}
				num = i;
			}
			Log.Out("LoadStamps in {0}", new object[]
			{
				(float)ms.ElapsedMilliseconds * 0.001f
			});
			yield break;
		}

		// Token: 0x0600B887 RID: 47239 RVA: 0x0044A438 File Offset: 0x00448638
		[PublicizedFrom(EAccessModifier.Private)]
		public void LoadStamp(PathAbstractions.AbstractedLocation _path)
		{
			string fileNameNoExtension = _path.FileNameNoExtension;
			if (this.AllStamps.ContainsKey(fileNameNoExtension))
			{
				return;
			}
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			Color[] array;
			if (_path.Extension == ".raw")
			{
				array = Utils.LoadRawStampFileArray(_path.FullPath);
			}
			else
			{
				if (_path.Extension == ".exr")
				{
					return;
				}
				Texture2D texture2D = TextureUtils.LoadTexture(_path.FullPath, FilterMode.Point, false, false, null);
				array = texture2D.GetPixels();
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(texture2D);
				}
			}
			if (array == null)
			{
				Log.Error("LoadStamp {0} failed", new object[]
				{
					_path.FullPath
				});
				return;
			}
			NativeArray<float> heightPixels = default(NativeArray<float>);
			float num = array[0].r;
			for (int i = 1; i < array.Length; i++)
			{
				if (num != array[i].r)
				{
					heightPixels = new NativeArray<float>(array.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					for (int j = 0; j < array.Length; j++)
					{
						heightPixels[j] = array[j].r * 255f;
					}
					break;
				}
			}
			num *= 255f;
			NativeArray<float> alphaPixels = default(NativeArray<float>);
			float a = array[0].a;
			for (int k = 1; k < array.Length; k++)
			{
				if (a != array[k].a)
				{
					alphaPixels = new NativeArray<float>(array.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					for (int l = 0; l < array.Length; l++)
					{
						alphaPixels[l] = array[l].a;
					}
					break;
				}
			}
			NativeArray<float> waterPixels = default(NativeArray<float>);
			if (!fileNameNoExtension.Contains("rwg_tile"))
			{
				for (int m = 0; m < array.Length; m++)
				{
					if (array[m].b != 0f)
					{
						waterPixels = new NativeArray<float>(array.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
						for (int n = 0; n < array.Length; n++)
						{
							waterPixels[n] = array[n].b * 255f;
						}
						break;
					}
				}
			}
			int num2 = (int)Mathf.Sqrt((float)array.Length);
			RawStamp rawStamp = new RawStamp
			{
				name = fileNameNoExtension
			};
			rawStamp.data.heightConst = num;
			rawStamp.data.heightPixels = heightPixels;
			rawStamp.data.alphaConst = a;
			rawStamp.data.alphaPixels = alphaPixels;
			rawStamp.data.waterPixels = waterPixels;
			rawStamp.data.height = num2;
			rawStamp.data.width = num2;
			this.AllStamps[fileNameNoExtension] = rawStamp;
			if (alphaPixels.IsCreated)
			{
				if (fileNameNoExtension.StartsWith("mountains_"))
				{
					RawStamp.SmoothAlpha(ref rawStamp.data, 5);
				}
				if (fileNameNoExtension.StartsWith("desert_mountains_"))
				{
					RawStamp.SmoothAlpha(ref rawStamp.data, 3);
				}
			}
			Log.Out("LoadStamp {0}, size {1}, height {2} ({3}), alpha {4} ({5}), water {6}, in {7}", new object[]
			{
				_path.FullPath,
				num2,
				heightPixels.IsCreated,
				num,
				alphaPixels.IsCreated,
				a,
				waterPixels.IsCreated,
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
		}

		// Token: 0x0600B888 RID: 47240 RVA: 0x0044A784 File Offset: 0x00448984
		[PublicizedFrom(EAccessModifier.Private)]
		public static void SaveEXR(string cleanname, Color[] colors, bool _replace = true)
		{
			string text = GameIO.GetGameDir("Data/Stamps") + "/" + cleanname + ".exr";
			if (!_replace && File.Exists(text))
			{
				return;
			}
			int num = (int)Mathf.Sqrt((float)colors.Length);
			Texture2D texture2D = new Texture2D(num, num, TextureFormat.RGBAFloat, false);
			texture2D.SetPixels(colors);
			File.WriteAllBytes(text, texture2D.EncodeToEXR(Texture2D.EXRFlags.None));
			Log.Out("SaveEXR {0}", new object[]
			{
				text
			});
		}

		// Token: 0x0600B889 RID: 47241 RVA: 0x0044A7F4 File Offset: 0x004489F4
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DrawStamp$BurstManaged(ref NativeArray<float> _dest, ref RawStamp.Data _src, int _x, int _y, int _destWidth, int _destHeight, float _alphaScale, bool _additive, float _scale, ref Color32 _customColor, float _alphaCutoff, float _angle)
		{
			_x -= (int)((float)_src.width * _scale) / 2;
			_y -= (int)((float)_src.height * _scale) / 2;
			double num = (double)(0.017453292f * _angle);
			float sine = (float)Math.Sin(num);
			float cosine = (float)Math.Cos(num);
			int num2 = (int)Math.Floor((double)((float)(((int)Math.Sqrt((double)(_src.width * _src.width + _src.height * _src.height)) - _src.width) / 2) * _scale));
			int num3 = (int)Math.Floor((double)((float)_src.width * _scale + (float)num2));
			num2 = -num2;
			int num4 = num2;
			int num5 = _x + num2;
			if (num5 < 0)
			{
				num4 -= num5;
			}
			int num6 = num3;
			num5 = _x + num3;
			if (num5 >= _destWidth)
			{
				num6 -= num5 - _destWidth;
			}
			int num7 = num2;
			int num8 = _y + num2;
			if (num8 < 0)
			{
				num7 -= num8;
			}
			int num9 = num3;
			num8 = _y + num3;
			if (num8 >= _destHeight)
			{
				num9 -= num8 - _destHeight;
			}
			StampManager.RotateParams rotateParams;
			rotateParams.sine = sine;
			rotateParams.cosine = cosine;
			rotateParams.width = _src.width;
			rotateParams.height = _src.height;
			rotateParams.halfWidth = (float)(_src.width / 2);
			rotateParams.halfHeight = (float)(_src.height / 2);
			rotateParams.isWater = false;
			for (int i = num7; i < num9; i++)
			{
				int num10 = (_y + i) * _destWidth;
				float y = (float)i / _scale;
				for (int j = num4; j < num6; j++)
				{
					float x = (float)j / _scale;
					float num11 = _src.alphaConst;
					if (_src.alphaPixels.IsCreated)
					{
						num11 = StampManager.CalcRotatedValue(ref rotateParams, x, y, ref _src.alphaPixels);
					}
					if (num11 >= 1E-05f)
					{
						int index = _x + j + num10;
						if (_customColor.a > 0)
						{
							if (num11 > _alphaCutoff)
							{
								_dest[index] = (float)_customColor.r;
							}
						}
						else
						{
							float num12 = _src.heightConst;
							if (_src.heightPixels.IsCreated)
							{
								num12 = StampManager.CalcRotatedValue(ref rotateParams, x, y, ref _src.heightPixels);
							}
							float num13 = num11 * _alphaScale;
							float num14 = _dest[index];
							if (_additive)
							{
								num14 += num12 * num13;
							}
							else
							{
								num14 += (num12 - num14) * num13;
							}
							_dest[index] = num14;
						}
					}
				}
			}
		}

		// Token: 0x0600B88A RID: 47242 RVA: 0x0044AA3C File Offset: 0x00448C3C
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DrawWaterStamp$BurstManaged(ref NativeArray<float> _dest, ref NativeArray<float> _src, int _x, int _y, int _destWidth, int _destHeight, int _srcWidth, int _srcHeight, float _alpha, float _scale, float _customColor, float _angle)
		{
			_x -= (int)((float)_srcWidth * _scale) / 2;
			_y -= (int)((float)_srcHeight * _scale) / 2;
			double num = (double)(0.017453292f * _angle);
			float sine = (float)Math.Sin(num);
			float cosine = (float)Math.Cos(num);
			int num2 = (int)Math.Floor((double)((float)(((int)Math.Sqrt((double)(_srcWidth * _srcWidth + _srcHeight * _srcHeight)) - _srcWidth) / 2) * _scale));
			int num3 = (int)Math.Floor((double)((float)_srcWidth * _scale + (float)num2));
			num2 = -num2;
			int num4 = num2;
			int num5 = _x + num2;
			if (num5 < 0)
			{
				num4 -= num5;
			}
			int num6 = num3;
			num5 = _x + num3;
			if (num5 >= _destWidth)
			{
				num6 -= num5 - _destWidth;
			}
			int num7 = num2;
			int num8 = _y + num2;
			if (num8 < 0)
			{
				num7 -= num8;
			}
			int num9 = num3;
			num8 = _y + num3;
			if (num8 >= _destHeight)
			{
				num9 -= num8 - _destHeight;
			}
			StampManager.RotateParams rotateParams;
			rotateParams.sine = sine;
			rotateParams.cosine = cosine;
			rotateParams.width = _srcWidth;
			rotateParams.height = _srcHeight;
			rotateParams.halfWidth = (float)(_srcWidth / 2);
			rotateParams.halfHeight = (float)(_srcHeight / 2);
			rotateParams.isWater = true;
			for (int i = num7; i < num9; i++)
			{
				int num10 = (_y + i) * _destWidth;
				float y = (float)i / _scale;
				for (int j = num4; j < num6; j++)
				{
					int index = _x + j + num10;
					if (_dest[index] <= 0f && StampManager.CalcRotatedValue(ref rotateParams, (float)j / _scale, y, ref _src) >= 1E-05f)
					{
						_dest[index] = _customColor;
					}
				}
			}
		}

		// Token: 0x04008A23 RID: 35363
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cAlphaCutoff = 1E-05f;

		// Token: 0x04008A24 RID: 35364
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04008A25 RID: 35365
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, RawStamp> AllStamps = new Dictionary<string, RawStamp>();

		// Token: 0x04008A26 RID: 35366
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<RawStamp> tempGetStampList = new List<RawStamp>();

		// Token: 0x0200172C RID: 5932
		[PublicizedFrom(EAccessModifier.Private)]
		public struct RotateParams
		{
			// Token: 0x04008A27 RID: 35367
			public float sine;

			// Token: 0x04008A28 RID: 35368
			public float cosine;

			// Token: 0x04008A29 RID: 35369
			public int width;

			// Token: 0x04008A2A RID: 35370
			public int height;

			// Token: 0x04008A2B RID: 35371
			public float halfWidth;

			// Token: 0x04008A2C RID: 35372
			public float halfHeight;

			// Token: 0x04008A2D RID: 35373
			public bool isWater;
		}

		// Token: 0x0200172E RID: 5934
		// (Invoke) Token: 0x0600B892 RID: 47250
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate void DrawStamp_0000B825$PostfixBurstDelegate(ref NativeArray<float> _dest, ref RawStamp.Data _src, int _x, int _y, int _destWidth, int _destHeight, float _alphaScale, bool _additive, float _scale, ref Color32 _customColor, float _alphaCutoff, float _angle);

		// Token: 0x0200172F RID: 5935
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class DrawStamp_0000B825$BurstDirectCall
		{
			// Token: 0x0600B895 RID: 47253 RVA: 0x0044ADAE File Offset: 0x00448FAE
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (StampManager.DrawStamp_0000B825$BurstDirectCall.Pointer == 0)
				{
					StampManager.DrawStamp_0000B825$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(StampManager.DrawStamp_0000B825$BurstDirectCall.DeferredCompilation, methodof(StampManager.DrawStamp$BurstManaged(NativeArray<float>*, RawStamp.Data*, int, int, int, int, float, bool, float, Color32*, float, float)).MethodHandle, typeof(StampManager.DrawStamp_0000B825$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = StampManager.DrawStamp_0000B825$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B896 RID: 47254 RVA: 0x0044ADDC File Offset: 0x00448FDC
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				StampManager.DrawStamp_0000B825$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B897 RID: 47255 RVA: 0x0044ADF4 File Offset: 0x00448FF4
			public unsafe static void Constructor()
			{
				StampManager.DrawStamp_0000B825$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(StampManager.DrawStamp(NativeArray<float>*, RawStamp.Data*, int, int, int, int, float, bool, float, Color32*, float, float)).MethodHandle);
			}

			// Token: 0x0600B898 RID: 47256 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B899 RID: 47257 RVA: 0x0044AE05 File Offset: 0x00449005
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static DrawStamp_0000B825$BurstDirectCall()
			{
				StampManager.DrawStamp_0000B825$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B89A RID: 47258 RVA: 0x0044AE0C File Offset: 0x0044900C
			public static void Invoke(ref NativeArray<float> _dest, ref RawStamp.Data _src, int _x, int _y, int _destWidth, int _destHeight, float _alphaScale, bool _additive, float _scale, ref Color32 _customColor, float _alphaCutoff, float _angle)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = StampManager.DrawStamp_0000B825$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Single>&,WorldGenerationEngineFinal.RawStamp/Data&,System.Int32,System.Int32,System.Int32,System.Int32,System.Single,System.Boolean,System.Single,UnityEngine.Color32&,System.Single,System.Single), ref _dest, ref _src, _x, _y, _destWidth, _destHeight, _alphaScale, _additive, _scale, ref _customColor, _alphaCutoff, _angle, functionPointer);
						return;
					}
				}
				StampManager.DrawStamp$BurstManaged(ref _dest, ref _src, _x, _y, _destWidth, _destHeight, _alphaScale, _additive, _scale, ref _customColor, _alphaCutoff, _angle);
			}

			// Token: 0x04008A35 RID: 35381
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x04008A36 RID: 35382
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}

		// Token: 0x02001730 RID: 5936
		// (Invoke) Token: 0x0600B89C RID: 47260
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate void DrawWaterStamp_0000B828$PostfixBurstDelegate(ref NativeArray<float> _dest, ref NativeArray<float> _src, int _x, int _y, int _destWidth, int _destHeight, int _srcWidth, int _srcHeight, float _alpha, float _scale, float _customColor, float _angle);

		// Token: 0x02001731 RID: 5937
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class DrawWaterStamp_0000B828$BurstDirectCall
		{
			// Token: 0x0600B89F RID: 47263 RVA: 0x0044AE63 File Offset: 0x00449063
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (StampManager.DrawWaterStamp_0000B828$BurstDirectCall.Pointer == 0)
				{
					StampManager.DrawWaterStamp_0000B828$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(StampManager.DrawWaterStamp_0000B828$BurstDirectCall.DeferredCompilation, methodof(StampManager.DrawWaterStamp$BurstManaged(NativeArray<float>*, NativeArray<float>*, int, int, int, int, int, int, float, float, float, float)).MethodHandle, typeof(StampManager.DrawWaterStamp_0000B828$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = StampManager.DrawWaterStamp_0000B828$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B8A0 RID: 47264 RVA: 0x0044AE90 File Offset: 0x00449090
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				StampManager.DrawWaterStamp_0000B828$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B8A1 RID: 47265 RVA: 0x0044AEA8 File Offset: 0x004490A8
			public unsafe static void Constructor()
			{
				StampManager.DrawWaterStamp_0000B828$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(StampManager.DrawWaterStamp(NativeArray<float>*, NativeArray<float>*, int, int, int, int, int, int, float, float, float, float)).MethodHandle);
			}

			// Token: 0x0600B8A2 RID: 47266 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B8A3 RID: 47267 RVA: 0x0044AEB9 File Offset: 0x004490B9
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static DrawWaterStamp_0000B828$BurstDirectCall()
			{
				StampManager.DrawWaterStamp_0000B828$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B8A4 RID: 47268 RVA: 0x0044AEC0 File Offset: 0x004490C0
			public static void Invoke(ref NativeArray<float> _dest, ref NativeArray<float> _src, int _x, int _y, int _destWidth, int _destHeight, int _srcWidth, int _srcHeight, float _alpha, float _scale, float _customColor, float _angle)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = StampManager.DrawWaterStamp_0000B828$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Single>&,Unity.Collections.NativeArray`1<System.Single>&,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Single,System.Single,System.Single,System.Single), ref _dest, ref _src, _x, _y, _destWidth, _destHeight, _srcWidth, _srcHeight, _alpha, _scale, _customColor, _angle, functionPointer);
						return;
					}
				}
				StampManager.DrawWaterStamp$BurstManaged(ref _dest, ref _src, _x, _y, _destWidth, _destHeight, _srcWidth, _srcHeight, _alpha, _scale, _customColor, _angle);
			}

			// Token: 0x04008A37 RID: 35383
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x04008A38 RID: 35384
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}
	}
}
