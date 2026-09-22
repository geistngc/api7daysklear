using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200004B RID: 75
[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
	// Token: 0x06000199 RID: 409 RVA: 0x0000FEBC File Offset: 0x0000E0BC
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator Start()
	{
		UnityWebRequest www = UnityWebRequest.Get(this.url);
		yield return www.SendWebRequest();
		this.mTex = DownloadHandlerTexture.GetContent(www);
		if (this.mTex != null)
		{
			UITexture component = base.GetComponent<UITexture>();
			component.mainTexture = this.mTex;
			if (this.pixelPerfect)
			{
				component.MakePixelPerfect();
			}
		}
		www.Dispose();
		yield break;
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000FECB File Offset: 0x0000E0CB
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		if (this.mTex != null)
		{
			UnityEngine.Object.Destroy(this.mTex);
		}
	}

	// Token: 0x0400024A RID: 586
	public string url = "http://www.yourwebsite.com/logo.png";

	// Token: 0x0400024B RID: 587
	public bool pixelPerfect = true;

	// Token: 0x0400024C RID: 588
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D mTex;
}
