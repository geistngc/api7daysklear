using System;
using System.IO;
using UnityEngine;
using Webserver.FileCache;

namespace MapRendering
{
	// Token: 0x02001AB7 RID: 6839
	public class MapTileCache : AbstractCache
	{
		// Token: 0x0600CE48 RID: 52808 RVA: 0x004B2010 File Offset: 0x004B0210
		public MapTileCache(int _tileSize)
		{
			Texture2D texture2D = new Texture2D(_tileSize, _tileSize);
			Color color = new Color(0f, 0f, 0f, 0f);
			for (int i = 0; i < _tileSize; i++)
			{
				for (int j = 0; j < _tileSize; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
			this.transparentTile = texture2D.EncodeToPNG();
			UnityEngine.Object.Destroy(texture2D);
		}

		// Token: 0x0600CE49 RID: 52809 RVA: 0x004B207C File Offset: 0x004B027C
		public void SetZoomCount(int _count)
		{
			this.cache = new MapTileCache.CurrentZoomFile[_count];
			for (int i = 0; i < this.cache.Length; i++)
			{
				this.cache[i] = new MapTileCache.CurrentZoomFile();
			}
		}

		// Token: 0x0600CE4A RID: 52810 RVA: 0x004B20B8 File Offset: 0x004B02B8
		public byte[] LoadTile(int _zoomlevel, string _filename)
		{
			try
			{
				MapTileCache.CurrentZoomFile[] obj = this.cache;
				lock (obj)
				{
					MapTileCache.CurrentZoomFile currentZoomFile = this.cache[_zoomlevel];
					if (currentZoomFile.filename != null && currentZoomFile.filename.Equals(_filename))
					{
						return currentZoomFile.pngData;
					}
					currentZoomFile.filename = _filename;
					if (!File.Exists(_filename))
					{
						currentZoomFile.pngData = null;
						return null;
					}
					currentZoomFile.pngData = MapTileCache.ReadAllBytes(_filename);
					return currentZoomFile.pngData;
				}
			}
			catch (Exception arg)
			{
				Log.Warning(string.Format("Error in MapTileCache.LoadTile: {0}", arg));
			}
			return null;
		}

		// Token: 0x0600CE4B RID: 52811 RVA: 0x004B216C File Offset: 0x004B036C
		public void SaveTile(int _zoomlevel, byte[] _contentPng)
		{
			try
			{
				MapTileCache.CurrentZoomFile[] obj = this.cache;
				lock (obj)
				{
					MapTileCache.CurrentZoomFile currentZoomFile = this.cache[_zoomlevel];
					string filename = currentZoomFile.filename;
					if (!string.IsNullOrEmpty(filename))
					{
						currentZoomFile.pngData = _contentPng;
						using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096))
						{
							stream.Write(_contentPng, 0, _contentPng.Length);
						}
					}
				}
			}
			catch (Exception arg)
			{
				Log.Warning(string.Format("Error in MapTileCache.SaveTile: {0}", arg));
			}
		}

		// Token: 0x0600CE4C RID: 52812 RVA: 0x004B2220 File Offset: 0x004B0420
		public void ResetTile(int _zoomlevel)
		{
			try
			{
				MapTileCache.CurrentZoomFile[] obj = this.cache;
				lock (obj)
				{
					this.cache[_zoomlevel].filename = null;
					this.cache[_zoomlevel].pngData = null;
				}
			}
			catch (Exception arg)
			{
				Log.Warning(string.Format("Error in MapTileCache.ResetTile: {0}", arg));
			}
		}

		// Token: 0x0600CE4D RID: 52813 RVA: 0x004B2298 File Offset: 0x004B0498
		public override byte[] GetFileContent(string _filename)
		{
			try
			{
				MapTileCache.CurrentZoomFile[] obj = this.cache;
				lock (obj)
				{
					foreach (MapTileCache.CurrentZoomFile currentZoomFile in this.cache)
					{
						if (currentZoomFile.filename != null && currentZoomFile.filename.Equals(_filename))
						{
							return currentZoomFile.pngData;
						}
					}
					return (!File.Exists(_filename)) ? this.transparentTile : MapTileCache.ReadAllBytes(_filename);
				}
			}
			catch (Exception arg)
			{
				Log.Warning(string.Format("Error in MapTileCache.GetFileContent: {0}", arg));
			}
			return null;
		}

		// Token: 0x0600CE4E RID: 52814 RVA: 0x004B234C File Offset: 0x004B054C
		public override ValueTuple<int, int> Invalidate()
		{
			return new ValueTuple<int, int>(0, 0);
		}

		// Token: 0x0600CE4F RID: 52815 RVA: 0x004B2358 File Offset: 0x004B0558
		[PublicizedFrom(EAccessModifier.Private)]
		public static byte[] ReadAllBytes(string _path)
		{
			byte[] result;
			using (FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
			{
				int num = 0;
				int i = (int)fileStream.Length;
				byte[] array = new byte[i];
				while (i > 0)
				{
					int num2 = fileStream.Read(array, num, i);
					if (num2 == 0)
					{
						throw new IOException("Unexpected end of stream");
					}
					num += num2;
					i -= num2;
				}
				result = array;
			}
			return result;
		}

		// Token: 0x04009C8A RID: 40074
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly byte[] transparentTile;

		// Token: 0x04009C8B RID: 40075
		[PublicizedFrom(EAccessModifier.Private)]
		public MapTileCache.CurrentZoomFile[] cache;

		// Token: 0x02001AB8 RID: 6840
		[PublicizedFrom(EAccessModifier.Private)]
		public class CurrentZoomFile
		{
			// Token: 0x04009C8C RID: 40076
			public string filename;

			// Token: 0x04009C8D RID: 40077
			public byte[] pngData;
		}
	}
}
