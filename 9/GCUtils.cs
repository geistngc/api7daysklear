using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02001413 RID: 5139
public static class GCUtils
{
	// Token: 0x0600A173 RID: 41331 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PreUnload()
	{
	}

	// Token: 0x0600A174 RID: 41332 RVA: 0x003CB587 File Offset: 0x003C9787
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PostUnload()
	{
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}

	// Token: 0x0600A175 RID: 41333 RVA: 0x003CB593 File Offset: 0x003C9793
	public static void UnloadAndCollectStart()
	{
		ThreadManager.StartCoroutine(GCUtils.UnloadAndCollectCo());
	}

	// Token: 0x0600A176 RID: 41334 RVA: 0x003CB5A0 File Offset: 0x003C97A0
	public static IEnumerator UnloadAndCollectCo()
	{
		Interlocked.Increment(ref GCUtils.m_working);
		try
		{
			Task preUnload = Task.Run(new Action(GCUtils.PreUnload));
			yield return Resources.UnloadUnusedAssets();
			while (!preUnload.IsCompleted)
			{
				yield return null;
			}
			Task postUnload = Task.Run(new Action(GCUtils.PostUnload));
			while (!postUnload.IsCompleted)
			{
				yield return null;
			}
			preUnload = null;
			postUnload = null;
		}
		finally
		{
			Interlocked.Decrement(ref GCUtils.m_working);
		}
		yield break;
		yield break;
	}

	// Token: 0x0600A177 RID: 41335 RVA: 0x003CB5A8 File Offset: 0x003C97A8
	public static IEnumerator WaitForIdle()
	{
		while (GCUtils.m_working > 0)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x040079D4 RID: 31188
	[PublicizedFrom(EAccessModifier.Private)]
	public static int m_working;
}
