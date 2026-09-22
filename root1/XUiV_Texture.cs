using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02001163 RID: 4451
public class XUiV_Texture : XUiV_TextureBased
{
	// Token: 0x170010F0 RID: 4336
	// (set) Token: 0x06008E30 RID: 36400 RVA: 0x00359B6E File Offset: 0x00357D6E
	public override Texture Texture
	{
		set
		{
			if (this.AutoUnload && base.Texture != null)
			{
				this.UnloadTexture();
			}
			base.Texture = value;
			if (value == null)
			{
				this.isExternalTexture = false;
			}
		}
	}

	// Token: 0x170010F1 RID: 4337
	// (get) Token: 0x06008E31 RID: 36401 RVA: 0x00359BA3 File Offset: 0x00357DA3
	// (set) Token: 0x06008E32 RID: 36402 RVA: 0x00359BAB File Offset: 0x00357DAB
	[XuiXmlAttribute("textures", false)]
	public string[] TextureUris
	{
		get
		{
			return this.textureUris;
		}
		set
		{
			if (value == null || value.Length == 0)
			{
				this.textureUris = Array.Empty<string>();
				this.Texture = null;
				base.SetDirty();
				return;
			}
			this.textureUris = value;
			this.loadTexture(0);
		}
	}

	// Token: 0x170010F2 RID: 4338
	// (get) Token: 0x06008E33 RID: 36403 RVA: 0x00359BDB File Offset: 0x00357DDB
	// (set) Token: 0x06008E34 RID: 36404 RVA: 0x00359BE5 File Offset: 0x00357DE5
	[XuiXmlAttribute("texture", false)]
	public string Path
	{
		get
		{
			return this.textureUris[0];
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				this.TextureUris = null;
				return;
			}
			if (this.TextureUris.Length == 1 && this.TextureUris[0] == value)
			{
				return;
			}
			this.TextureUris = new string[]
			{
				value
			};
		}
	}

	// Token: 0x170010F3 RID: 4339
	// (get) Token: 0x06008E35 RID: 36405 RVA: 0x00359C23 File Offset: 0x00357E23
	// (set) Token: 0x06008E36 RID: 36406 RVA: 0x00359C2B File Offset: 0x00357E2B
	[XuiXmlAttribute("autounload", false)]
	public bool AutoUnload
	{
		get
		{
			return this.autoUnload;
		}
		set
		{
			if (this.autoUnload == value)
			{
				return;
			}
			this.autoUnload = value;
		}
	}

	// Token: 0x06008E37 RID: 36407 RVA: 0x00359C3E File Offset: 0x00357E3E
	public XUiV_Texture(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008E38 RID: 36408 RVA: 0x00359C54 File Offset: 0x00357E54
	[PublicizedFrom(EAccessModifier.Private)]
	public void loadTexture(int _index = 0)
	{
		if (_index >= this.TextureUris.Length)
		{
			return;
		}
		string text = this.TextureUris[_index];
		try
		{
			string str;
			if (ModManager.TryPatchModPathString(text, out str))
			{
				this.<loadTexture>g__FetchWwwTexture|15_0(_index, "file://" + str);
			}
			else if (text[0] == '@' && text[1] != ':')
			{
				string text2 = text.Substring(1);
				if (text2.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
				{
					string text3 = text2.Substring(5);
					if (text3[0] != '/' && text3[0] != '\\')
					{
						text2 = new Uri(((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXServer) ? (Application.dataPath + "/../../") : (Application.dataPath + "/../")) + text3).AbsoluteUri;
					}
				}
				this.<loadTexture>g__FetchWwwTexture|15_0(_index, text2);
			}
			else
			{
				this.xui.LoadData<Texture>(text, delegate(Texture _o)
				{
					this.Texture = _o;
					this.isExternalTexture = false;
				});
			}
		}
		catch (Exception e)
		{
			Log.Error("[XUi] Could not load texture: " + text + ", on " + base.GetXuiHierarchy());
			Log.Exception(e);
		}
		base.SetDirty();
	}

	// Token: 0x06008E39 RID: 36409 RVA: 0x00359D88 File Offset: 0x00357F88
	public override void UnloadTexture()
	{
		Texture texture = this.Texture;
		base.UnloadTexture();
		this.TextureUris = null;
		if (texture == null)
		{
			return;
		}
		if (!this.isExternalTexture)
		{
			Resources.UnloadAsset(texture);
			return;
		}
		UnityEngine.Object.DestroyImmediate(texture);
	}

	// Token: 0x06008E3B RID: 36411 RVA: 0x00359DD8 File Offset: 0x00357FD8
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <loadTexture>g__FetchWwwTexture|15_0(int _currentIndex, string _fetchUri)
	{
		_fetchUri = _fetchUri.Replace("#", "%23").Replace("+", "%2B");
		UnityWebRequest texture = UnityWebRequestTexture.GetTexture(_fetchUri);
		texture.SendWebRequest();
		ThreadManager.StartCoroutine(this.<loadTexture>g__WaitForWwwData|15_1(_currentIndex, texture, _fetchUri));
	}

	// Token: 0x06008E3C RID: 36412 RVA: 0x00359E23 File Offset: 0x00358023
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator <loadTexture>g__WaitForWwwData|15_1(int _currentIndex, UnityWebRequest _www, string _fetchUri)
	{
		while (!_www.isDone)
		{
			yield return null;
		}
		if (_www.result != UnityWebRequest.Result.Success)
		{
			if (this.TextureUris.Length == 1)
			{
				Log.Warning(string.Concat(new string[]
				{
					"[XUi] Retrieving texture file from '",
					_fetchUri,
					"' failed (",
					_www.error,
					")."
				}));
			}
			else if (_currentIndex + 1 < this.TextureUris.Length)
			{
				Log.Warning(string.Format("[XUi] Retrieving texture file {0} from '{1}' failed: ({2}). Trying next URI.", _currentIndex + 1, _fetchUri, _www.error));
				this.loadTexture(_currentIndex + 1);
			}
			else
			{
				Log.Warning(string.Format("[XUi] Retrieving texture file {0} from '{1}' failed: ({2}). No URIs successful.", _currentIndex + 1, _fetchUri, _www.error));
			}
			yield break;
		}
		Texture2D texture = ((DownloadHandlerTexture)_www.downloadHandler).texture;
		this.Texture = TextureUtils.CloneTexture(texture, false, false, true);
		UnityEngine.Object.DestroyImmediate(texture);
		_www.Dispose();
		this.isExternalTexture = true;
		yield break;
	}

	// Token: 0x04006851 RID: 26705
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] textureUris = Array.Empty<string>();

	// Token: 0x04006852 RID: 26706
	[PublicizedFrom(EAccessModifier.Private)]
	public bool autoUnload;

	// Token: 0x04006853 RID: 26707
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isExternalTexture;
}
