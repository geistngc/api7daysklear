using System;
using System.IO;

namespace Platform.Shared
{
	// Token: 0x02001CB1 RID: 7345
	public static class PlatformIdCache
	{
		// Token: 0x17001B0E RID: 6926
		// (get) Token: 0x0600D9E0 RID: 55776 RVA: 0x004E53B5 File Offset: 0x004E35B5
		public static string IdFilePath
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return Path.Combine(GameIO.GetUserGameDataDir(), "PlatformIdCache.txt");
			}
		}

		// Token: 0x0600D9E1 RID: 55777 RVA: 0x004E53C8 File Offset: 0x004E35C8
		public static bool TryGetCachedId<T>(out T _platformUserIdentifier) where T : PlatformUserIdentifierAbs
		{
			string idFilePath = PlatformIdCache.IdFilePath;
			if (SdFile.Exists(idFilePath))
			{
				using (Stream stream = SdFile.OpenRead(idFilePath))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						string text = streamReader.ReadLine();
						if (text == null)
						{
							Log.Out("[PlatformIdCache] no cached user id");
							_platformUserIdentifier = default(T);
							return false;
						}
						PlatformUserIdentifierAbs platformUserIdentifierAbs = PlatformUserIdentifierAbs.FromCombinedString(text, true);
						if (!(platformUserIdentifierAbs is T))
						{
							Log.Error(string.Format("[PlatformIdCache] cannot retrieved cached id {0} as {1}", text, typeof(T)));
							_platformUserIdentifier = default(T);
							return false;
						}
						_platformUserIdentifier = (T)((object)platformUserIdentifierAbs);
						return true;
					}
				}
			}
			Log.Out("[PlatformIdCache] no id cache file at " + idFilePath);
			_platformUserIdentifier = default(T);
			return false;
		}

		// Token: 0x0600D9E2 RID: 55778 RVA: 0x004E54AC File Offset: 0x004E36AC
		public static void SetCachedId(PlatformUserIdentifierAbs _platformUserIdentifier)
		{
			using (Stream stream = SdFile.OpenWrite(PlatformIdCache.IdFilePath))
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					streamWriter.WriteLine(_platformUserIdentifier.CombinedString);
				}
			}
		}

		// Token: 0x0400A57A RID: 42362
		[PublicizedFrom(EAccessModifier.Private)]
		public const string idCacheFile = "PlatformIdCache.txt";
	}
}
