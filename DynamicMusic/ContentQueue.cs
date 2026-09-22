using System;
using System.Collections.Generic;
using MusicUtils.Enums;
using UniLinq;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A46 RID: 6726
	[Preserve]
	public class ContentQueue
	{
		// Token: 0x170018ED RID: 6381
		// (get) Token: 0x0600CBF7 RID: 52215 RVA: 0x0002003D File Offset: 0x0001E23D
		[Preserve]
		public bool IsReady
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600CBF8 RID: 52216 RVA: 0x004AAE28 File Offset: 0x004A9028
		public ContentQueue(SectionType _section, LayerType _layer)
		{
			this.section = _section;
			this.layer = _layer;
			this.count = (from e in Content.AllContent.OfType<LayeredContent>()
			where e.Section == this.section && e.Layer == this.layer
			select e).Count<LayeredContent>();
		}

		// Token: 0x0600CBF9 RID: 52217 RVA: 0x004AAE7C File Offset: 0x004A907C
		public LayeredContent Next()
		{
			if (this.queue.Count < this.count / 2)
			{
				(from e in Content.AllContent.OfType<LayeredContent>()
				where e.Section == this.section && e.Layer == this.layer && !this.queue.Contains(e)
				orderby ContentQueue.rng.RandomInt
				select e).ToList<LayeredContent>().ForEach(new Action<LayeredContent>(this.queue.Enqueue));
			}
			return this.queue.Dequeue();
		}

		// Token: 0x0600CBFA RID: 52218 RVA: 0x004AAF03 File Offset: 0x004A9103
		public void Clear()
		{
			this.queue.Clear();
		}

		// Token: 0x04009B4E RID: 39758
		[PublicizedFrom(EAccessModifier.Private)]
		public static GameRandom rng = GameRandomManager.Instance.CreateGameRandom();

		// Token: 0x04009B4F RID: 39759
		[PublicizedFrom(EAccessModifier.Private)]
		public SectionType section;

		// Token: 0x04009B50 RID: 39760
		[PublicizedFrom(EAccessModifier.Private)]
		public LayerType layer;

		// Token: 0x04009B51 RID: 39761
		[PublicizedFrom(EAccessModifier.Private)]
		public int count;

		// Token: 0x04009B52 RID: 39762
		[PublicizedFrom(EAccessModifier.Private)]
		public Queue<LayeredContent> queue = new Queue<LayeredContent>();
	}
}
