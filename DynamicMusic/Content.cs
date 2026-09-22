using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A2B RID: 6699
	[Preserve]
	public abstract class Content
	{
		// Token: 0x0600CB42 RID: 52034 RVA: 0x004A8D1B File Offset: 0x004A6F1B
		public Content()
		{
			Content.AllContent.Add(this);
		}

		// Token: 0x170018C9 RID: 6345
		// (get) Token: 0x0600CB43 RID: 52035 RVA: 0x004A8D2E File Offset: 0x004A6F2E
		// (set) Token: 0x0600CB44 RID: 52036 RVA: 0x004A8D36 File Offset: 0x004A6F36
		public string Name { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170018CA RID: 6346
		// (get) Token: 0x0600CB45 RID: 52037 RVA: 0x004A8D3F File Offset: 0x004A6F3F
		// (set) Token: 0x0600CB46 RID: 52038 RVA: 0x004A8D47 File Offset: 0x004A6F47
		public SectionType Section { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170018CB RID: 6347
		// (get) Token: 0x0600CB47 RID: 52039 RVA: 0x004A8D50 File Offset: 0x004A6F50
		// (set) Token: 0x0600CB48 RID: 52040 RVA: 0x004A8D58 File Offset: 0x004A6F58
		public virtual bool IsLoaded { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600CB49 RID: 52041
		[Preserve]
		public abstract IEnumerator Load();

		// Token: 0x0600CB4A RID: 52042
		[Preserve]
		public abstract void Unload();

		// Token: 0x0600CB4B RID: 52043 RVA: 0x004A8D61 File Offset: 0x004A6F61
		[Preserve]
		public static Content CreateWrapper(string _type)
		{
			return (Content)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("DynamicMusic.", _type));
		}

		// Token: 0x0600CB4C RID: 52044 RVA: 0x004A8D78 File Offset: 0x004A6F78
		public virtual void ParseFromXml(XElement _xmlNode)
		{
			this.Name = _xmlNode.GetAttribute("name");
		}

		// Token: 0x04009AF8 RID: 39672
		public static Dictionary<SectionType, int> SamplesFor = new Dictionary<SectionType, int>();

		// Token: 0x04009AF9 RID: 39673
		public static Dictionary<SectionType, string> SourcePathFor = new Dictionary<SectionType, string>();

		// Token: 0x04009AFA RID: 39674
		[PublicizedFrom(EAccessModifier.Protected)]
		public static GameRandom rng = GameRandomManager.Instance.CreateGameRandom();

		// Token: 0x04009AFB RID: 39675
		public static List<Content> AllContent = new List<Content>();
	}
}
