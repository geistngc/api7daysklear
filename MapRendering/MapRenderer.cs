using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using Utf8Json;
using Webserver.FileCache;

namespace MapRendering
{
	// Token: 0x02001AB2 RID: 6834
	public class MapRenderer
	{
		// Token: 0x17001968 RID: 6504
		// (get) Token: 0x0600CE26 RID: 52774 RVA: 0x004B121B File Offset: 0x004AF41B
		public static bool Enabled
		{
			get
			{
				return SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GamePrefs.GetBool(EnumGamePrefs.EnableMapRendering);
			}
		}

		// Token: 0x0600CE27 RID: 52775 RVA: 0x004B1238 File Offset: 0x004AF438
		[PublicizedFrom(EAccessModifier.Private)]
		public MapRenderer()
		{
			Constants.MapDirectory = GameIO.GetSaveGameDir() + "/map";
			if (!this.LoadMapInfo())
			{
				this.WriteMapInfo();
			}
			this.cache.SetZoomCount(Constants.Zoomlevels);
			this.zoomLevelBuffers = new MapRenderBlockBuffer[Constants.Zoomlevels];
			for (int i = 0; i < Constants.Zoomlevels; i++)
			{
				this.zoomLevelBuffers[i] = new MapRenderBlockBuffer(i, this.cache);
			}
			this.renderCoroutineRef = ThreadManager.StartCoroutine(this.renderCoroutine());
		}

		// Token: 0x17001969 RID: 6505
		// (get) Token: 0x0600CE28 RID: 52776 RVA: 0x004B1319 File Offset: 0x004AF519
		public static bool HasInstance
		{
			get
			{
				return MapRenderer.instance != null;
			}
		}

		// Token: 0x1700196A RID: 6506
		// (get) Token: 0x0600CE29 RID: 52777 RVA: 0x004B1323 File Offset: 0x004AF523
		public static MapRenderer Instance
		{
			get
			{
				MapRenderer result;
				if ((result = MapRenderer.instance) == null)
				{
					result = (MapRenderer.instance = new MapRenderer());
				}
				return result;
			}
		}

		// Token: 0x0600CE2A RID: 52778 RVA: 0x004B1339 File Offset: 0x004AF539
		public static AbstractCache GetTileCache()
		{
			return MapRenderer.Instance.cache;
		}

		// Token: 0x0600CE2B RID: 52779 RVA: 0x004B1348 File Offset: 0x004AF548
		public static void Shutdown()
		{
			if (MapRenderer.instance == null)
			{
				return;
			}
			MapRenderer.instance.shutdown = true;
			if (MapRenderer.instance.renderCoroutineRef != null)
			{
				ThreadManager.StopCoroutine(MapRenderer.instance.renderCoroutineRef);
				MapRenderer.instance.renderCoroutineRef = null;
			}
			MapRenderer.instance = null;
		}

		// Token: 0x0600CE2C RID: 52780 RVA: 0x004B1394 File Offset: 0x004AF594
		public static void RenderSingleChunk(Chunk _chunk)
		{
			if (!MapRenderer.renderingEnabled || MapRenderer.instance == null)
			{
				return;
			}
			ThreadPool.UnsafeQueueUserWorkItem(delegate(object _o)
			{
				try
				{
					if (!MapRenderer.instance.renderingFullMap)
					{
						object obj = MapRenderer.lockObject;
						lock (obj)
						{
							Chunk chunk = (Chunk)_o;
							Vector3i worldPos = chunk.GetWorldPos();
							Vector2i key = new Vector2i(worldPos.x / 16, worldPos.z / 16);
							ushort[] mapColors = chunk.GetMapColors();
							if (mapColors != null)
							{
								Color32[] array = new Color32[256];
								for (int i = 0; i < mapColors.Length; i++)
								{
									array[i] = MapRenderer.shortColorToColor32(mapColors[i]);
								}
								MapRenderer.instance.dirtyChunks[key] = array;
							}
						}
					}
				}
				catch (Exception arg)
				{
					Log.Out(string.Format("Exception in MapRendering.RenderSingleChunk(): {0}", arg));
				}
			}, _chunk);
		}

		// Token: 0x0600CE2D RID: 52781 RVA: 0x004B13CC File Offset: 0x004AF5CC
		public void RenderFullMap()
		{
			MicroStopwatch microStopwatch = new MicroStopwatch();
			string saveGameRegionDir = GameIO.GetSaveGameRegionDir();
			RegionFileManager regionFileManager = new RegionFileManager(saveGameRegionDir, saveGameRegionDir, 0, false);
			Texture2D texture2D = null;
			Vector2i vector2i;
			Vector2i vector2i2;
			Vector2i vector2i3;
			Vector2i vector2i4;
			int num;
			int num2;
			int num3;
			int num4;
			this.getWorldExtent(regionFileManager, out vector2i, out vector2i2, out vector2i3, out vector2i4, out num, out num2, out num3, out num4);
			Log.Out(string.Format("RenderMap: min: {0}, max: {1}, minPos: {2}, maxPos: {3}, w/h: {4}/{5}, wP/hP: {6}/{7}", new object[]
			{
				vector2i.ToString(),
				vector2i2.ToString(),
				vector2i3.ToString(),
				vector2i4.ToString(),
				num,
				num2,
				num3,
				num4
			}));
			object obj = MapRenderer.lockObject;
			lock (obj)
			{
				for (int i = 0; i < Constants.Zoomlevels; i++)
				{
					this.zoomLevelBuffers[i].ResetBlock();
				}
				if (Directory.Exists(Constants.MapDirectory))
				{
					Directory.Delete(Constants.MapDirectory, true);
				}
				this.WriteMapInfo();
				this.renderingFullMap = true;
				if (num3 <= 8192 && num4 <= 8192)
				{
					texture2D = new Texture2D(num3, num4);
				}
				Vector2i vector2i5 = default(Vector2i);
				Vector2i vector2i6 = default(Vector2i);
				vector2i5.x = 0;
				while (vector2i5.x < num3)
				{
					vector2i5.y = 0;
					while (vector2i5.y < num4)
					{
						vector2i6.x = vector2i5.x / 16 + vector2i.x;
						vector2i6.y = vector2i5.y / 16 + vector2i.y;
						try
						{
							long key = WorldChunkCache.MakeChunkKey(vector2i6.x, vector2i6.y);
							if (regionFileManager.ContainsChunkSync(key))
							{
								ushort[] mapColors = regionFileManager.GetChunkSync(key).GetMapColors();
								if (mapColors != null)
								{
									Color32[] array = new Color32[256];
									for (int j = 0; j < mapColors.Length; j++)
									{
										array[j] = MapRenderer.shortColorToColor32(mapColors[j]);
									}
									this.dirtyChunks[vector2i6] = array;
									if (texture2D != null)
									{
										texture2D.SetPixels32(vector2i5.x, vector2i5.y, 16, 16, array);
									}
								}
							}
						}
						catch (Exception arg)
						{
							Log.Out(string.Format("Exception: {0}", arg));
						}
						vector2i5.y += 16;
					}
					while (this.dirtyChunks.Count > 0)
					{
						this.RenderDirtyChunks();
					}
					Log.Out(string.Format("RenderMap: {0}/{1} ({2}%)", vector2i5.x, num3, (int)((float)vector2i5.x / (float)num3 * 100f)));
					vector2i5.x += 16;
				}
			}
			regionFileManager.Cleanup();
			if (texture2D != null)
			{
				byte[] bytes = texture2D.EncodeToPNG();
				File.WriteAllBytes(Constants.MapDirectory + "/map.png", bytes);
				UnityEngine.Object.Destroy(texture2D);
			}
			this.renderingFullMap = false;
			Log.Out(string.Format("Generating map took: {0} ms", microStopwatch.ElapsedMilliseconds));
			Log.Out(string.Format("World extent: {0} - {1}", vector2i3, vector2i4));
		}

		// Token: 0x0600CE2E RID: 52782 RVA: 0x004B1738 File Offset: 0x004AF938
		[PublicizedFrom(EAccessModifier.Private)]
		public void SaveAllBlockMaps()
		{
			for (int i = 0; i < Constants.Zoomlevels; i++)
			{
				this.zoomLevelBuffers[i].SaveBlock();
			}
		}

		// Token: 0x0600CE2F RID: 52783 RVA: 0x004B1762 File Offset: 0x004AF962
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator renderCoroutine()
		{
			while (!this.shutdown)
			{
				object obj = MapRenderer.lockObject;
				lock (obj)
				{
					if (this.dirtyChunks.Count > 0 && this.renderTimeout >= 1.7014117E+38f)
					{
						this.renderTimeout = Time.time + 0.5f;
					}
					if (Time.time > this.renderTimeout || this.dirtyChunks.Count > 200)
					{
						this.RenderDirtyChunks();
					}
				}
				yield return this.coroutineDelay;
			}
			yield break;
		}

		// Token: 0x0600CE30 RID: 52784 RVA: 0x004B1774 File Offset: 0x004AF974
		[PublicizedFrom(EAccessModifier.Private)]
		public void RenderDirtyChunks()
		{
			this.msw.ResetAndRestart();
			if (this.dirtyChunks.Count <= 0)
			{
				return;
			}
			this.chunksToRender.Clear();
			this.chunksRendered.Clear();
			this.dirtyChunks.CopyKeysTo(this.chunksToRender);
			Vector2i vector2i = this.chunksToRender[0];
			this.chunksRendered.Add(vector2i);
			Vector2i vector2i2;
			Vector2i vector2i3;
			this.getBlockNumber(vector2i, out vector2i2, out vector2i3, Constants.MAP_BLOCK_TO_CHUNK_DIV, 16);
			this.zoomLevelBuffers[Constants.Zoomlevels - 1].LoadBlock(vector2i2);
			foreach (Vector2i vector2i4 in this.chunksToRender)
			{
				Vector2i vector2i5;
				Vector2i offset;
				this.getBlockNumber(vector2i4, out vector2i5, out offset, Constants.MAP_BLOCK_TO_CHUNK_DIV, 16);
				if (vector2i5.Equals(vector2i2))
				{
					this.chunksRendered.Add(vector2i4);
					if (this.dirtyChunks[vector2i4].Length != 256)
					{
						Log.Error(string.Format("Rendering chunk has incorrect data size of {0} instead of {1}", this.dirtyChunks[vector2i4].Length, 256));
					}
					this.zoomLevelBuffers[Constants.Zoomlevels - 1].SetPart(offset, 16, this.dirtyChunks[vector2i4]);
				}
			}
			foreach (Vector2i key in this.chunksRendered)
			{
				this.dirtyChunks.Remove(key);
			}
			this.RenderZoomLevel(vector2i2);
			this.SaveAllBlockMaps();
		}

		// Token: 0x0600CE31 RID: 52785 RVA: 0x004B1930 File Offset: 0x004AFB30
		[PublicizedFrom(EAccessModifier.Private)]
		public void RenderZoomLevel(Vector2i _innerBlock)
		{
			int i = Constants.Zoomlevels - 1;
			while (i > 0)
			{
				Vector2i vector2i;
				Vector2i offset;
				this.getBlockNumber(_innerBlock, out vector2i, out offset, 2, Constants.MapBlockSize / 2);
				this.zoomLevelBuffers[i - 1].LoadBlock(vector2i);
				if ((this.zoomLevelBuffers[i].FormatSelf == TextureFormat.ARGB32 || this.zoomLevelBuffers[i].FormatSelf == TextureFormat.RGBA32) && this.zoomLevelBuffers[i].FormatSelf == this.zoomLevelBuffers[i - 1].FormatSelf)
				{
					this.zoomLevelBuffers[i - 1].SetPartNative(offset, Constants.MapBlockSize / 2, this.zoomLevelBuffers[i].GetHalfScaledNative());
				}
				else
				{
					this.zoomLevelBuffers[i - 1].SetPart(offset, Constants.MapBlockSize / 2, this.zoomLevelBuffers[i].GetHalfScaled());
				}
				i--;
				_innerBlock = vector2i;
			}
		}

		// Token: 0x0600CE32 RID: 52786 RVA: 0x004B1A04 File Offset: 0x004AFC04
		[PublicizedFrom(EAccessModifier.Private)]
		public void getBlockNumber(Vector2i _innerPos, out Vector2i _block, out Vector2i _blockOffset, int _scaleFactor, int _offsetSize)
		{
			_block = default(Vector2i);
			_blockOffset = default(Vector2i);
			_block.x = (_innerPos.x + 16777216) / _scaleFactor - 16777216 / _scaleFactor;
			_block.y = (_innerPos.y + 16777216) / _scaleFactor - 16777216 / _scaleFactor;
			_blockOffset.x = (_innerPos.x + 16777216) % _scaleFactor * _offsetSize;
			_blockOffset.y = (_innerPos.y + 16777216) % _scaleFactor * _offsetSize;
		}

		// Token: 0x0600CE33 RID: 52787 RVA: 0x004B1A8C File Offset: 0x004AFC8C
		[PublicizedFrom(EAccessModifier.Private)]
		public void WriteMapInfo()
		{
			JsonWriter jsonWriter = default(JsonWriter);
			jsonWriter.WriteBeginObject();
			jsonWriter.WritePropertyName("blockSize");
			jsonWriter.WriteInt32(Constants.MapBlockSize);
			jsonWriter.WriteValueSeparator();
			jsonWriter.WritePropertyName("maxZoom");
			jsonWriter.WriteInt32(Constants.Zoomlevels - 1);
			jsonWriter.WriteEndObject();
			Directory.CreateDirectory(Constants.MapDirectory);
			File.WriteAllBytes(Constants.MapDirectory + "/mapinfo.json", jsonWriter.ToUtf8ByteArray());
		}

		// Token: 0x0600CE34 RID: 52788 RVA: 0x004B1B10 File Offset: 0x004AFD10
		[PublicizedFrom(EAccessModifier.Private)]
		public bool LoadMapInfo()
		{
			if (!File.Exists(Constants.MapDirectory + "/mapinfo.json"))
			{
				return false;
			}
			string json = File.ReadAllText(Constants.MapDirectory + "/mapinfo.json", Encoding.UTF8);
			try
			{
				IDictionary<string, object> dictionary = JsonSerializer.Deserialize<IDictionary<string, object>>(json);
				object obj;
				if (dictionary.TryGetValue("blockSize", out obj) && obj is double)
				{
					double num = (double)obj;
					Constants.MapBlockSize = (int)num;
				}
				if (dictionary.TryGetValue("maxZoom", out obj) && obj is double)
				{
					double num2 = (double)obj;
					Constants.Zoomlevels = (int)num2 + 1;
				}
				return true;
			}
			catch (Exception arg)
			{
				Log.Out(string.Format("Exception in LoadMapInfo: {0}", arg));
			}
			return false;
		}

		// Token: 0x0600CE35 RID: 52789 RVA: 0x004B1BD0 File Offset: 0x004AFDD0
		[PublicizedFrom(EAccessModifier.Private)]
		public void getWorldExtent(RegionFileManager _rfm, out Vector2i _minChunk, out Vector2i _maxChunk, out Vector2i _minPos, out Vector2i _maxPos, out int _widthChunks, out int _heightChunks, out int _widthPix, out int _heightPix)
		{
			_minChunk = default(Vector2i);
			_maxChunk = default(Vector2i);
			_minPos = default(Vector2i);
			_maxPos = default(Vector2i);
			long[] allChunkKeys = _rfm.GetAllChunkKeys();
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MinValue;
			int num4 = int.MinValue;
			foreach (long key in allChunkKeys)
			{
				int num5 = WorldChunkCache.extractX(key);
				int num6 = WorldChunkCache.extractZ(key);
				if (num5 < num)
				{
					num = num5;
				}
				if (num5 > num3)
				{
					num3 = num5;
				}
				if (num6 < num2)
				{
					num2 = num6;
				}
				if (num6 > num4)
				{
					num4 = num6;
				}
			}
			_minChunk.x = num;
			_minChunk.y = num2;
			_maxChunk.x = num3;
			_maxChunk.y = num4;
			_minPos.x = num * 16;
			_minPos.y = num2 * 16;
			_maxPos.x = num3 * 16;
			_maxPos.y = num4 * 16;
			_widthChunks = num3 - num + 1;
			_heightChunks = num4 - num2 + 1;
			_widthPix = _widthChunks * 16;
			_heightPix = _heightChunks * 16;
		}

		// Token: 0x0600CE36 RID: 52790 RVA: 0x004B1CCC File Offset: 0x004AFECC
		[PublicizedFrom(EAccessModifier.Private)]
		public static Color32 shortColorToColor32(ushort _col)
		{
			byte r = (byte)(256 * (_col >> 10 & 31) / 32);
			byte g = (byte)(256 * (_col >> 5 & 31) / 32);
			byte b = (byte)(256 * (_col & 31) / 32);
			return new Color32(r, g, b, byte.MaxValue);
		}

		// Token: 0x04009C74 RID: 40052
		[PublicizedFrom(EAccessModifier.Private)]
		public static MapRenderer instance;

		// Token: 0x04009C75 RID: 40053
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly object lockObject = new object();

		// Token: 0x04009C76 RID: 40054
		public static bool renderingEnabled = true;

		// Token: 0x04009C77 RID: 40055
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MapTileCache cache = new MapTileCache(Constants.MapBlockSize);

		// Token: 0x04009C78 RID: 40056
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<Vector2i, Color32[]> dirtyChunks = new Dictionary<Vector2i, Color32[]>();

		// Token: 0x04009C79 RID: 40057
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MicroStopwatch msw = new MicroStopwatch();

		// Token: 0x04009C7A RID: 40058
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MapRenderBlockBuffer[] zoomLevelBuffers;

		// Token: 0x04009C7B RID: 40059
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine renderCoroutineRef;

		// Token: 0x04009C7C RID: 40060
		[PublicizedFrom(EAccessModifier.Private)]
		public bool renderingFullMap;

		// Token: 0x04009C7D RID: 40061
		[PublicizedFrom(EAccessModifier.Private)]
		public float renderTimeout = float.MaxValue;

		// Token: 0x04009C7E RID: 40062
		[PublicizedFrom(EAccessModifier.Private)]
		public bool shutdown;

		// Token: 0x04009C7F RID: 40063
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WaitForSeconds coroutineDelay = new WaitForSeconds(0.2f);

		// Token: 0x04009C80 RID: 40064
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<Vector2i> chunksToRender = new List<Vector2i>();

		// Token: 0x04009C81 RID: 40065
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<Vector2i> chunksRendered = new List<Vector2i>();
	}
}
