using System;
using System.Collections.Generic;

namespace Platform.Shared
{
	// Token: 0x02001CB8 RID: 7352
	public static class SaveGameProviderHelper
	{
		// Token: 0x0600DA23 RID: 55843 RVA: 0x004E5E23 File Offset: 0x004E4023
		public static IEnumerable<SaveDataManagedPath> GetManagedPathsFromBaseAndSubPaths(SaveDataManagedPath path, string basePath, IEnumerable<string> subPaths)
		{
			foreach (string subPath in subPaths)
			{
				yield return SaveGameProviderHelper.GetManagedPathFromBaseAndSubPath(path, basePath, subPath);
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600DA24 RID: 55844 RVA: 0x004E5E44 File Offset: 0x004E4044
		public static SaveDataManagedPath GetManagedPathFromBaseAndSubPath(SaveDataManagedPath path, string basePath, string subPath)
		{
			ReadOnlySpan<char> span = subPath.AsSpan(basePath.Length).TrimStart("\\/");
			return path.GetChildPath(span);
		}
	}
}
