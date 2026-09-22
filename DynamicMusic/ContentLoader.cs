using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicMusic
{
	// Token: 0x02001A4B RID: 6731
	public class ContentLoader
	{
		// Token: 0x170018F1 RID: 6385
		// (get) Token: 0x0600CC0F RID: 52239 RVA: 0x004AB0CE File Offset: 0x004A92CE
		public static ContentLoader Instance
		{
			get
			{
				if (ContentLoader.instance == null)
				{
					ContentLoader.instance = new ContentLoader();
				}
				return ContentLoader.instance;
			}
		}

		// Token: 0x0600CC10 RID: 52240 RVA: 0x004AB0E6 File Offset: 0x004A92E6
		public void Start()
		{
			this.LoadQueue = new Queue<IEnumerator>();
			this.Loader = GameManager.Instance.StartCoroutine(this.Load());
		}

		// Token: 0x0600CC11 RID: 52241 RVA: 0x004AB109 File Offset: 0x004A9309
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator Load()
		{
			for (;;)
			{
				yield return new WaitUntil(() => this.LoadQueue.Count > 0);
				IEnumerator enumerator = this.LoadQueue.Dequeue();
				yield return enumerator;
			}
			yield break;
		}

		// Token: 0x0600CC12 RID: 52242 RVA: 0x004AB118 File Offset: 0x004A9318
		public void Cleanup()
		{
			if (this.Loader != null)
			{
				GameManager.Instance.StopCoroutine(this.Loader);
			}
			if (this.LoadQueue != null)
			{
				this.LoadQueue.Clear();
			}
			this.Loader = null;
			this.LoadQueue = null;
		}

		// Token: 0x04009B5C RID: 39772
		public static ContentLoader instance;

		// Token: 0x04009B5D RID: 39773
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine Loader;

		// Token: 0x04009B5E RID: 39774
		public Queue<IEnumerator> LoadQueue;
	}
}
