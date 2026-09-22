using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Platform.XBL.Save.MasterFileTable.Latest;

namespace Platform.XBL.Save
{
	// Token: 0x02001C3D RID: 7229
	public class BlobCache
	{
		// Token: 0x0600D662 RID: 54882 RVA: 0x004D523D File Offset: 0x004D343D
		[Conditional("DEBUG_SAVE_DATA_MANAGER")]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogTrace(string text)
		{
			Log.Out("[XBL: BlobCache] " + text);
		}

		// Token: 0x0600D663 RID: 54883 RVA: 0x004D523D File Offset: 0x004D343D
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogInfo(string text)
		{
			Log.Out("[XBL: BlobCache] " + text);
		}

		// Token: 0x0600D664 RID: 54884 RVA: 0x004D524F File Offset: 0x004D344F
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogWarning(string text)
		{
			Log.Warning("[XBL: BlobCache] " + text);
		}

		// Token: 0x0600D665 RID: 54885 RVA: 0x004D5261 File Offset: 0x004D3461
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogError(string text)
		{
			Log.Error("[XBL: BlobCache] " + text);
		}

		// Token: 0x0600D666 RID: 54886 RVA: 0x004D5274 File Offset: 0x004D3474
		public BlobCache(string containerName, IEnumerable<BlobRef> expectedBlobRefs)
		{
			if (!LaunchPrefs.GameCoreBlobCache.Value)
			{
				return;
			}
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			this.m_root = GameIO.GetNormalizedPath(Path.Combine(GameIO.GetNormalizedPath(GameIO.GetDeviceLocalUserGameDataDir()) + "BlobCache", containerName));
			Directory.CreateDirectory(this.m_root);
			this.m_blobRefs = expectedBlobRefs.ToDictionary((BlobRef blobRef) => blobRef.Id, (BlobRef blobRef) => blobRef);
			this.m_inUse = new ConcurrentDictionary<ulong, byte>();
			HashSet<ulong> cachedIds = new HashSet<ulong>();
			foreach (string path in Directory.EnumerateDirectories(this.m_root))
			{
				Directory.Delete(path, true);
			}
			HashAlgorithm hashAlgorithm = BlobRef.GetHashAlgorithm();
			foreach (FileInfo fileInfo in new DirectoryInfo(this.m_root).EnumerateFiles())
			{
				string name = fileInfo.Name;
				ulong num;
				BlobRef blobRef2;
				if (!SaveContainer.TryConvertToId(name, out num))
				{
					BlobCache.LogWarning("Invalid cached blob name '" + name + "'.");
				}
				else if (!this.m_blobRefs.TryGetValue(num, out blobRef2))
				{
					BlobCache.LogWarning("Extraneous cached blob '" + name + "'.");
					fileInfo.Delete();
				}
				else if (fileInfo.Length != (long)((ulong)blobRef2.Length))
				{
					BlobCache.LogWarning(string.Format("Length mismatch for blob '{0}'. {1} != {2}", name, fileInfo.Length, blobRef2.Length));
					this.m_blobRefs.Remove(blobRef2.Id);
					fileInfo.Delete();
				}
				else
				{
					byte[] array = null;
					using (FileStream fileStream = fileInfo.OpenRead())
					{
						if (fileStream.Length == (long)((ulong)blobRef2.Length))
						{
							array = hashAlgorithm.ComputeHash(fileStream);
						}
					}
					if (array == null)
					{
						BlobCache.LogWarning(string.Format("Length mismatch for blob '{0}'. {1} != {2}", name, fileInfo.Length, blobRef2.Length));
						this.m_blobRefs.Remove(blobRef2.Id);
						fileInfo.Delete();
					}
					else if (!blobRef2.Hash.Span.SequenceEqual(array))
					{
						BlobCache.LogWarning(string.Concat(new string[]
						{
							"Hash mismatch for '",
							name,
							"'. ",
							array.ToHexString(),
							" != ",
							blobRef2.Hash.ToHexString()
						}));
						this.m_blobRefs.Remove(blobRef2.Id);
						fileInfo.Delete();
					}
					else
					{
						cachedIds.Add(num);
					}
				}
			}
			this.m_blobRefs.RemoveAll((ulong blobId) => !cachedIds.Contains(blobId));
			BlobCache.LogInfo(string.Format("Cached validated in {0:F3} ms.", microStopwatch.Elapsed.TotalMilliseconds));
		}

		// Token: 0x0600D667 RID: 54887 RVA: 0x004D5608 File Offset: 0x004D3808
		public bool Contains(BlobRef requestedRef)
		{
			if (!LaunchPrefs.GameCoreBlobCache.Value)
			{
				return false;
			}
			object blobRefsLock = this.m_blobRefsLock;
			BlobRef right;
			lock (blobRefsLock)
			{
				if (!this.m_blobRefs.TryGetValue(requestedRef.Id, out right))
				{
					return false;
				}
			}
			return !(requestedRef != right);
		}

		// Token: 0x0600D668 RID: 54888 RVA: 0x004D5678 File Offset: 0x004D3878
		public bool TryGet(BlobRef requestedRef, out RefCountedBuffer buffer)
		{
			if (!LaunchPrefs.GameCoreBlobCache.Value)
			{
				buffer = null;
				return false;
			}
			object blobRefsLock = this.m_blobRefsLock;
			BlobRef blobRef;
			lock (blobRefsLock)
			{
				if (!this.m_blobRefs.TryGetValue(requestedRef.Id, out blobRef))
				{
					buffer = null;
					return false;
				}
			}
			if (requestedRef != blobRef)
			{
				buffer = null;
				return false;
			}
			ulong id = blobRef.Id;
			string path;
			bool result;
			using (this.GetCachedPath(id, out path))
			{
				if (!File.Exists(path))
				{
					BlobCache.LogWarning(string.Format("Cache MISS Expected {0} to exist in the cache?", blobRef));
					this.Invalidate(id);
					buffer = null;
					result = false;
				}
				else
				{
					buffer = null;
					try
					{
						using (FileStream fileStream = File.OpenRead(path))
						{
							if (fileStream.Length != (long)((ulong)blobRef.Length))
							{
								BlobCache.LogWarning(string.Format("Cache MISS Unexpected length of {0} for {1}.", fileStream.Length, blobRef));
								this.Invalidate(id);
								buffer = null;
								result = false;
							}
							else
							{
								buffer = RefCountedBuffer.CreatePooled((int)fileStream.Length);
								int num = 0;
								while ((long)num < fileStream.Length)
								{
									Stream stream = fileStream;
									Span<byte> span = buffer.Span;
									int num2 = num;
									int num3 = stream.Read(span.Slice(num2, span.Length - num2));
									if (num3 <= 0)
									{
										throw new IOException(string.Format("Expected {0} bytes but only read {1}.", fileStream.Length, num));
									}
									num += num3;
								}
								result = true;
							}
						}
					}
					catch (Exception arg)
					{
						BlobCache.LogError(string.Format("Cache MISS Failed to read {0}: {1}", blobRef, arg));
						this.Invalidate(id);
						RefCountedBuffer refCountedBuffer = buffer;
						if (refCountedBuffer != null)
						{
							refCountedBuffer.Dispose();
						}
						buffer = null;
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x0600D669 RID: 54889 RVA: 0x004D5898 File Offset: 0x004D3A98
		public BlobRef Set(ulong blobId, RefCountedBuffer buffer)
		{
			BlobRef blobRef = new BlobRef
			{
				Id = blobId,
				Length = (uint)buffer.Length,
				Hash = BlobRef.CalculateHash(buffer)
			};
			if (!LaunchPrefs.GameCoreBlobCache.Value)
			{
				return blobRef;
			}
			object blobRefsLock = this.m_blobRefsLock;
			lock (blobRefsLock)
			{
				BlobRef blobRef2;
				if (this.m_blobRefs.TryGetValue(blobId, out blobRef2) && blobRef == blobRef2)
				{
					return blobRef2;
				}
			}
			string path;
			BlobRef result;
			using (this.GetCachedPath(blobId, out path))
			{
				try
				{
					using (FileStream fileStream = File.Create(path))
					{
						fileStream.Write(buffer.Span);
					}
				}
				catch (Exception arg)
				{
					BlobCache.LogError(string.Format("Cache FAIL {0}: {1}", blobId, arg));
					this.Invalidate(blobId);
					return blobRef;
				}
				blobRefsLock = this.m_blobRefsLock;
				lock (blobRefsLock)
				{
					this.m_blobRefs[blobId] = blobRef;
				}
				result = blobRef;
			}
			return result;
		}

		// Token: 0x0600D66A RID: 54890 RVA: 0x004D59F4 File Offset: 0x004D3BF4
		public void Invalidate(ulong blobId)
		{
			if (!LaunchPrefs.GameCoreBlobCache.Value)
			{
				return;
			}
			object blobRefsLock = this.m_blobRefsLock;
			lock (blobRefsLock)
			{
				if (!this.m_blobRefs.Remove(blobId))
				{
					return;
				}
			}
			try
			{
				string path;
				using (this.GetCachedPath(blobId, out path))
				{
					File.Delete(path);
				}
			}
			catch (Exception arg)
			{
				BlobCache.LogError(string.Format("Failed to invalidate '{0}': {1}", blobId, arg));
			}
		}

		// Token: 0x0600D66B RID: 54891 RVA: 0x004D5AA4 File Offset: 0x004D3CA4
		[PublicizedFrom(EAccessModifier.Private)]
		public BlobCache.FileScope GetCachedPath(ulong blobId, out string filePath)
		{
			BlobCache.FileScope fileScope = new BlobCache.FileScope(this, blobId);
			filePath = null;
			BlobCache.FileScope result;
			try
			{
				filePath = GameIO.GetNormalizedPath(Path.Join(this.m_root, SaveContainer.IdToString(blobId)));
				result = fileScope;
			}
			finally
			{
				if (filePath == null)
				{
					fileScope.Dispose();
				}
			}
			return result;
		}

		// Token: 0x0400A390 RID: 41872
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string m_root;

		// Token: 0x0400A391 RID: 41873
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object m_blobRefsLock = new object();

		// Token: 0x0400A392 RID: 41874
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<ulong, BlobRef> m_blobRefs;

		// Token: 0x0400A393 RID: 41875
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ConcurrentDictionary<ulong, byte> m_inUse;

		// Token: 0x02001C3E RID: 7230
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly struct FileScope : IDisposable
		{
			// Token: 0x0600D66C RID: 54892 RVA: 0x004D5B00 File Offset: 0x004D3D00
			public FileScope(BlobCache parent, ulong blobId)
			{
				this.m_parent = parent;
				this.m_blobId = blobId;
				while (!this.m_parent.m_inUse.TryAdd(this.m_blobId, 0))
				{
					Thread.Sleep(0);
				}
			}

			// Token: 0x0600D66D RID: 54893 RVA: 0x004D5B34 File Offset: 0x004D3D34
			public void Dispose()
			{
				byte b;
				if (!this.m_parent.m_inUse.TryRemove(this.m_blobId, out b))
				{
					BlobCache.LogError(string.Format("Expected to release '{0}'.", this.m_blobId));
				}
			}

			// Token: 0x0400A394 RID: 41876
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly BlobCache m_parent;

			// Token: 0x0400A395 RID: 41877
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly ulong m_blobId;
		}
	}
}
