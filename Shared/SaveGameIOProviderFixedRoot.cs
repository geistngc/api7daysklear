using System;
using System.IO;

namespace Platform.Shared
{
	// Token: 0x02001CB3 RID: 7347
	public class SaveGameIOProviderFixedRoot : SaveGameIOProvider
	{
		// Token: 0x0600D9F2 RID: 55794 RVA: 0x004E55DC File Offset: 0x004E37DC
		public SaveGameIOProviderFixedRoot(string rootPath)
		{
			this.m_rootPath = GameIO.GetNormalizedPath(rootPath);
			Directory.CreateDirectory(this.m_rootPath);
		}

		// Token: 0x0600D9F3 RID: 55795 RVA: 0x004E55FC File Offset: 0x004E37FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string GetPath(SaveDataManagedPath path)
		{
			if (this.m_rootPath != null)
			{
				return GameIO.GetNormalizedPath(Path.Combine(this.m_rootPath, path.PathRelativeToRoot));
			}
			return path.GetOriginalPath();
		}

		// Token: 0x0400A57D RID: 42365
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly string m_rootPath;
	}
}
