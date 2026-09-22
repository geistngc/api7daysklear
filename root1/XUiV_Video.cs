using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02001166 RID: 4454
public class XUiV_Video : XUiV_TextureBased
{
	// Token: 0x140000ED RID: 237
	// (add) Token: 0x06008E60 RID: 36448 RVA: 0x0035A388 File Offset: 0x00358588
	// (remove) Token: 0x06008E61 RID: 36449 RVA: 0x0035A3C0 File Offset: 0x003585C0
	public event XUiV_Video.VideoErrorDelegate VideoError;

	// Token: 0x140000EE RID: 238
	// (add) Token: 0x06008E62 RID: 36450 RVA: 0x0035A3F8 File Offset: 0x003585F8
	// (remove) Token: 0x06008E63 RID: 36451 RVA: 0x0035A430 File Offset: 0x00358630
	public event XUiV_Video.VideoFinishedDelegate VideoFinished;

	// Token: 0x170010FE RID: 4350
	// (get) Token: 0x06008E64 RID: 36452 RVA: 0x0035A465 File Offset: 0x00358665
	// (set) Token: 0x06008E65 RID: 36453 RVA: 0x0035A46D File Offset: 0x0035866D
	[XuiXmlAttribute("loop", false)]
	public bool Loop
	{
		get
		{
			return this.loop;
		}
		set
		{
			if (this.loop == value)
			{
				return;
			}
			this.loop = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010FF RID: 4351
	// (get) Token: 0x06008E66 RID: 36454 RVA: 0x0035A486 File Offset: 0x00358686
	// (set) Token: 0x06008E67 RID: 36455 RVA: 0x0035A48E File Offset: 0x0035868E
	[XuiXmlAttribute("uri", false)]
	public string VideoUri
	{
		get
		{
			return this.videoUri;
		}
		set
		{
			if (this.videoUri == value)
			{
				return;
			}
			this.videoUri = value;
			this.videoUriChanged = true;
			base.SetDirty();
		}
	}

	// Token: 0x17001100 RID: 4352
	// (get) Token: 0x06008E68 RID: 36456 RVA: 0x0035A4B3 File Offset: 0x003586B3
	// (set) Token: 0x06008E69 RID: 36457 RVA: 0x0035A4BB File Offset: 0x003586BB
	[XuiXmlAttribute("videoaspect", false)]
	public VideoAspectRatio VideoAspect
	{
		get
		{
			return this.videoAspect;
		}
		set
		{
			if (this.videoAspect == value)
			{
				return;
			}
			this.videoAspect = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001101 RID: 4353
	// (get) Token: 0x06008E6A RID: 36458 RVA: 0x0035A4D4 File Offset: 0x003586D4
	// (set) Token: 0x06008E6B RID: 36459 RVA: 0x0035A4E1 File Offset: 0x003586E1
	public bool Playing
	{
		get
		{
			return this.videoPlayer.isPlaying;
		}
		set
		{
			if (this.videoPlayer.isPlaying == value)
			{
				return;
			}
			if (value)
			{
				this.videoPlayer.Play();
				return;
			}
			this.videoPlayer.Stop();
		}
	}

	// Token: 0x17001102 RID: 4354
	// (get) Token: 0x06008E6C RID: 36460 RVA: 0x0035A50C File Offset: 0x0035870C
	// (set) Token: 0x06008E6D RID: 36461 RVA: 0x0035A514 File Offset: 0x00358714
	public EnumGamePrefs VolumeSetting
	{
		get
		{
			return this.volumeSetting;
		}
		set
		{
			if (this.volumeSetting == value)
			{
				return;
			}
			this.volumeSetting = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001103 RID: 4355
	// (get) Token: 0x06008E6E RID: 36462 RVA: 0x0035A52D File Offset: 0x0035872D
	// (set) Token: 0x06008E6F RID: 36463 RVA: 0x0035A53A File Offset: 0x0035873A
	public double CurrentTime
	{
		get
		{
			return this.videoPlayer.time;
		}
		set
		{
			this.videoPlayer.time = value;
		}
	}

	// Token: 0x06008E70 RID: 36464 RVA: 0x0035A548 File Offset: 0x00358748
	public XUiV_Video(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008E71 RID: 36465 RVA: 0x0035A560 File Offset: 0x00358760
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		base.createComponents(_go);
		_go.AddComponent<VideoPlayer>();
	}

	// Token: 0x06008E72 RID: 36466 RVA: 0x0035A570 File Offset: 0x00358770
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.videoPlayer = this.uiTransform.gameObject.GetComponent<VideoPlayer>();
	}

	// Token: 0x06008E73 RID: 36467 RVA: 0x0035A590 File Offset: 0x00358790
	public override void InitView()
	{
		base.InitView();
		this.videoPlayer.playOnAwake = false;
		this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
		this.videoPlayer.prepareCompleted += this.OnVideoPrepared;
		this.videoPlayer.loopPointReached += this.OnVideoFinished;
		this.videoPlayer.errorReceived += this.OnVideoErrorReceived;
		GamePrefs.OnGamePrefChanged += this.OnGamePrefChanged;
	}

	// Token: 0x06008E74 RID: 36468 RVA: 0x0035A611 File Offset: 0x00358811
	public override void Cleanup()
	{
		base.Cleanup();
		GamePrefs.OnGamePrefChanged -= this.OnGamePrefChanged;
		this.destroyRenderTexture();
	}

	// Token: 0x06008E75 RID: 36469 RVA: 0x0035A630 File Offset: 0x00358830
	public override void OnOpen()
	{
		base.OnOpen();
		ThreadManager.StartCoroutine(this.startVideoEndOfFrame());
	}

	// Token: 0x06008E76 RID: 36470 RVA: 0x0035A644 File Offset: 0x00358844
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator startVideoEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		this.startVideo();
		yield break;
	}

	// Token: 0x06008E77 RID: 36471 RVA: 0x0035A653 File Offset: 0x00358853
	public override void OnClose()
	{
		base.OnClose();
		this.destroyRenderTexture();
	}

	// Token: 0x06008E78 RID: 36472 RVA: 0x0035A661 File Offset: 0x00358861
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGamePrefChanged(EnumGamePrefs _pref)
	{
		if (_pref != this.VolumeSetting)
		{
			return;
		}
		this.updateVolumeLevel();
	}

	// Token: 0x06008E79 RID: 36473 RVA: 0x0035A674 File Offset: 0x00358874
	[PublicizedFrom(EAccessModifier.Private)]
	public void startVideo()
	{
		if (string.IsNullOrEmpty(this.VideoUri))
		{
			this.destroyRenderTexture();
			return;
		}
		this.updateRenderTexture();
		string text = null;
		string text2;
		if (ModManager.TryPatchModPathString(this.VideoUri, out text2))
		{
			text = text2;
		}
		else if (this.VideoUri[0] == '@' && this.VideoUri[1] != ':')
		{
			string text3 = this.VideoUri.Substring(1);
			if (text3.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
			{
				text = text3.Substring(5);
				if (text[0] != '/' && text[0] != '\\')
				{
					text = new Uri(((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXServer) ? (Application.dataPath + "/../../") : (Application.dataPath + "/../")) + text).AbsoluteUri;
				}
			}
			if (text == null)
			{
				this.videoPlayer.url = text3;
			}
		}
		else
		{
			text = Application.streamingAssetsPath + this.VideoUri;
		}
		if (text != null)
		{
			string extension = Path.GetExtension(text);
			if (string.IsNullOrEmpty(extension) || extension.Length > 5)
			{
				string text4;
				if (Application.platform == RuntimePlatform.PS5)
				{
					text4 = ".mp4";
				}
				else
				{
					text4 = ".webm";
				}
				string str = text4;
				if (Application.platform == RuntimePlatform.PS5)
				{
					text4 = ".webm";
				}
				else
				{
					text4 = ".mp4";
				}
				string str2 = text4;
				if (File.Exists(text + str))
				{
					text += str;
				}
				else if (File.Exists(text + str2))
				{
					text += str2;
				}
				else
				{
					text = null;
				}
			}
			this.videoPlayer.url = text;
		}
		this.videoPlayer.Prepare();
	}

	// Token: 0x06008E7A RID: 36474 RVA: 0x0035A81B File Offset: 0x00358A1B
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopVideo()
	{
		if (!this.videoPlayer.isPlaying)
		{
			return;
		}
		this.videoPlayer.Stop();
		this.videoPlayer.url = null;
	}

	// Token: 0x06008E7B RID: 36475 RVA: 0x0035A842 File Offset: 0x00358A42
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.updateRenderTexture();
	}

	// Token: 0x06008E7C RID: 36476 RVA: 0x0035A854 File Offset: 0x00358A54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		base.updateData();
		this.uiTexture.enabled = true;
		this.videoPlayer.aspectRatio = this.VideoAspect;
		this.videoPlayer.isLooping = this.Loop;
		if (this.videoUriChanged)
		{
			ThreadManager.StartCoroutine(this.videoUriChangedCo());
		}
	}

	// Token: 0x06008E7D RID: 36477 RVA: 0x0035A8A9 File Offset: 0x00358AA9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator videoUriChangedCo()
	{
		this.stopVideo();
		int i = 0;
		while ((long)i <= 2L)
		{
			yield return null;
			int num = i;
			i = num + 1;
		}
		this.startVideo();
		yield break;
	}

	// Token: 0x06008E7E RID: 36478 RVA: 0x0035A8B8 File Offset: 0x00358AB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updateVolumeLevel()
	{
		if (!this.videoPlayer.isPrepared || this.videoPlayer.audioTrackCount < 1)
		{
			return;
		}
		float @float = GamePrefs.GetFloat(this.VolumeSetting);
		this.videoPlayer.SetDirectAudioVolume(0, @float);
	}

	// Token: 0x06008E7F RID: 36479 RVA: 0x0035A8FC File Offset: 0x00358AFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateRenderTexture()
	{
		Vector3[] worldCorners = this.uiTexture.worldCorners;
		Vector3 vector = this.xui.playerUI.camera.WorldToScreenPoint(worldCorners[2]);
		Vector3 vector2 = this.xui.playerUI.camera.WorldToScreenPoint(worldCorners[0]);
		int num = Mathf.RoundToInt(vector.x - vector2.x);
		int num2 = Mathf.RoundToInt(vector.y - vector2.y);
		if (num < 2 || num2 < 2)
		{
			return;
		}
		if (this.rt != null && this.rt.width == num && this.rt.height == num2)
		{
			return;
		}
		this.destroyRenderTexture();
		this.rt = new RenderTexture(num, num2, 16);
		this.rt.Create();
		this.Texture = (this.videoPlayer.targetTexture = this.rt);
	}

	// Token: 0x06008E80 RID: 36480 RVA: 0x0035A9E4 File Offset: 0x00358BE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void destroyRenderTexture()
	{
		if (this.rt == null)
		{
			return;
		}
		this.Texture = (this.videoPlayer.targetTexture = null);
		this.rt.Release();
		UnityEngine.Object.Destroy(this.rt);
		this.rt = null;
	}

	// Token: 0x06008E81 RID: 36481 RVA: 0x0035AA32 File Offset: 0x00358C32
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVideoPrepared(VideoPlayer _source)
	{
		ThreadManager.StartCoroutine(this.playCo());
	}

	// Token: 0x06008E82 RID: 36482 RVA: 0x0035AA40 File Offset: 0x00358C40
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator playCo()
	{
		int i = 0;
		while ((long)i <= 2L)
		{
			yield return null;
			int num = i;
			i = num + 1;
		}
		this.videoPlayer.Play();
		this.updateVolumeLevel();
		yield break;
	}

	// Token: 0x06008E83 RID: 36483 RVA: 0x0035AA4F File Offset: 0x00358C4F
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVideoErrorReceived(VideoPlayer _source, string _message)
	{
		Log.Error("[XUi] Video player encountered an error. Skipping video. Message: " + _message);
		XUiV_Video.VideoErrorDelegate videoError = this.VideoError;
		if (videoError == null)
		{
			return;
		}
		videoError(this);
	}

	// Token: 0x06008E84 RID: 36484 RVA: 0x0035AA72 File Offset: 0x00358C72
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVideoFinished(VideoPlayer _source)
	{
		XUiV_Video.VideoFinishedDelegate videoFinished = this.VideoFinished;
		if (videoFinished == null)
		{
			return;
		}
		videoFinished(this);
	}

	// Token: 0x04006865 RID: 26725
	[PublicizedFrom(EAccessModifier.Private)]
	public VideoPlayer videoPlayer;

	// Token: 0x04006866 RID: 26726
	[PublicizedFrom(EAccessModifier.Private)]
	public RenderTexture rt;

	// Token: 0x04006867 RID: 26727
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint FrameDelay = 2U;

	// Token: 0x04006868 RID: 26728
	[PublicizedFrom(EAccessModifier.Private)]
	public bool loop;

	// Token: 0x04006869 RID: 26729
	[PublicizedFrom(EAccessModifier.Private)]
	public string videoUri;

	// Token: 0x0400686A RID: 26730
	[PublicizedFrom(EAccessModifier.Private)]
	public bool videoUriChanged;

	// Token: 0x0400686B RID: 26731
	[PublicizedFrom(EAccessModifier.Private)]
	public VideoAspectRatio videoAspect = VideoAspectRatio.FitInside;

	// Token: 0x0400686C RID: 26732
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumGamePrefs volumeSetting = EnumGamePrefs.OptionsMenuMusicVolumeLevel;

	// Token: 0x02001167 RID: 4455
	// (Invoke) Token: 0x06008E86 RID: 36486
	public delegate void VideoErrorDelegate(XUiV_Video _sender);

	// Token: 0x02001168 RID: 4456
	// (Invoke) Token: 0x06008E8A RID: 36490
	public delegate void VideoFinishedDelegate(XUiV_Video _sender);
}
