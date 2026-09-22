using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

// Token: 0x020012CC RID: 4812
public abstract class CursorControllerAbs : MonoBehaviour, IGamePrefsChangedListener
{
	// Token: 0x1700120B RID: 4619
	// (get) Token: 0x0600987F RID: 39039 RVA: 0x0039F0DF File Offset: 0x0039D2DF
	public static bool PrefabReady
	{
		get
		{
			return CursorControllerAbs.softCursorPrefab != null;
		}
	}

	// Token: 0x1700120C RID: 4620
	// (get) Token: 0x06009880 RID: 39040 RVA: 0x0039F0EC File Offset: 0x0039D2EC
	// (set) Token: 0x06009881 RID: 39041 RVA: 0x0039F0F4 File Offset: 0x0039D2F4
	public virtual XUiView HoverTarget
	{
		get
		{
			return this.hoverTarget;
		}
		set
		{
			this.hoverTarget = value;
			this.bHasHoverTarget = (value != null);
		}
	}

	// Token: 0x1700120D RID: 4621
	// (get) Token: 0x06009882 RID: 39042 RVA: 0x0039F107 File Offset: 0x0039D307
	// (set) Token: 0x06009883 RID: 39043 RVA: 0x0039F10F File Offset: 0x0039D30F
	public XUiView navigationTarget { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x1700120E RID: 4622
	// (get) Token: 0x06009884 RID: 39044 RVA: 0x0039F118 File Offset: 0x0039D318
	// (set) Token: 0x06009885 RID: 39045 RVA: 0x0039F120 File Offset: 0x0039D320
	public XUiView navigationTargetLater { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x1700120F RID: 4623
	// (get) Token: 0x06009886 RID: 39046 RVA: 0x0039F129 File Offset: 0x0039D329
	// (set) Token: 0x06009887 RID: 39047 RVA: 0x0039F131 File Offset: 0x0039D331
	public XUiView lockNavigationToView { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17001210 RID: 4624
	// (get) Token: 0x06009888 RID: 39048 RVA: 0x0039F13A File Offset: 0x0039D33A
	// (set) Token: 0x06009889 RID: 39049 RVA: 0x0039F142 File Offset: 0x0039D342
	public bool Locked
	{
		get
		{
			return this._locked;
		}
		set
		{
			this._locked = value;
		}
	}

	// Token: 0x17001211 RID: 4625
	// (get) Token: 0x0600988A RID: 39050 RVA: 0x0039F14B File Offset: 0x0039D34B
	// (set) Token: 0x0600988B RID: 39051 RVA: 0x0039F153 File Offset: 0x0039D353
	public bool VirtualCursorHidden
	{
		get
		{
			return this._virtualCursorHidden;
		}
		set
		{
			if (value != this._virtualCursorHidden)
			{
				this._virtualCursorHidden = value;
				this.OnVirtualCursorVisibleChanged();
			}
		}
	}

	// Token: 0x17001212 RID: 4626
	// (get) Token: 0x0600988C RID: 39052 RVA: 0x0039F16B File Offset: 0x0039D36B
	public XUiView CurrentTarget
	{
		get
		{
			if (this.CursorModeActive)
			{
				return this.hoverTarget;
			}
			return this.navigationTarget;
		}
	}

	// Token: 0x17001213 RID: 4627
	// (get) Token: 0x0600988D RID: 39053 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool CursorModeActive
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600988E RID: 39054
	public abstract Vector2 GetScreenPosition();

	// Token: 0x0600988F RID: 39055
	public abstract Vector2 GetLocalScreenPosition();

	// Token: 0x06009890 RID: 39056
	public abstract void SetScreenPosition(Vector2 _newPosition);

	// Token: 0x06009891 RID: 39057
	public abstract void SetScreenPosition(float _x, float _y);

	// Token: 0x06009892 RID: 39058
	public abstract void SetNavigationTarget(XUiView _view);

	// Token: 0x06009893 RID: 39059
	public abstract void SetNavigationTargetLater(XUiView _view);

	// Token: 0x06009894 RID: 39060
	public abstract void ResetNavigationTarget();

	// Token: 0x06009895 RID: 39061
	public abstract void ResetToCenter();

	// Token: 0x06009896 RID: 39062
	public abstract void SetNavigationLockView(XUiView _view, XUiView _viewToSelect = null);

	// Token: 0x06009897 RID: 39063
	public abstract bool IsWithinNavigationLockView(XUiView _view);

	// Token: 0x06009898 RID: 39064
	public abstract void RefreshSelection();

	// Token: 0x06009899 RID: 39065
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void OnVirtualCursorVisibleChanged();

	// Token: 0x0600989A RID: 39066 RVA: 0x0039F182 File Offset: 0x0039D382
	public void SetGUIActions(PlayerActionsGUI _guiActions)
	{
		this.guiActions = _guiActions;
	}

	// Token: 0x0600989B RID: 39067 RVA: 0x0039F18B File Offset: 0x0039D38B
	public void SetWindowManager(GUIWindowManager _windowManager)
	{
		this.windowManager = _windowManager;
	}

	// Token: 0x0600989C RID: 39068 RVA: 0x0039F194 File Offset: 0x0039D394
	public static void UpdateGamePrefs()
	{
		CursorControllerAbs.bSnapCursor = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerCursorSnap);
		CursorControllerAbs.regularSpeed = GamePrefs.GetFloat(EnumGamePrefs.OptionsInterfaceSensitivity);
		CursorControllerAbs.hoverSpeed = GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerCursorHoverSensitivity);
	}

	// Token: 0x0600989D RID: 39069 RVA: 0x0039F1C3 File Offset: 0x0039D3C3
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AwakeBase()
	{
		GamePrefs.AddChangeListener(this);
		GameOptionsManager.ResolutionChanged += this.OnResolutionChanged;
		CursorControllerAbs.UpdateGamePrefs();
		CursorControllerAbs.softCursors.Add(this);
	}

	// Token: 0x0600989E RID: 39070 RVA: 0x0039F1EC File Offset: 0x0039D3EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DestroyBase()
	{
		GamePrefs.RemoveChangeListener(this);
		GameOptionsManager.ResolutionChanged -= this.OnResolutionChanged;
		CursorControllerAbs.softCursors.Remove(this);
	}

	// Token: 0x0600989F RID: 39071 RVA: 0x0039F214 File Offset: 0x0039D414
	[PublicizedFrom(EAccessModifier.Protected)]
	public void InitCursorBounds()
	{
		this.cursorWorldBounds = new Bounds(this.cursor.worldCenter, Vector3.zero);
		Vector3[] worldCorners = this.cursor.worldCorners;
		for (int i = 0; i < worldCorners.Length; i++)
		{
			this.cursorWorldBounds.Encapsulate(worldCorners[i]);
		}
		Bounds bounds = new Bounds(this.uiCamera.cachedCamera.WorldToScreenPoint(this.cursorWorldBounds.min), Vector3.zero);
		bounds.Encapsulate(this.uiCamera.cachedCamera.WorldToScreenPoint(this.cursorWorldBounds.max));
		this.cursorBuffer = bounds.extents;
	}

	// Token: 0x060098A0 RID: 39072
	public abstract void UpdateMoveSpeed();

	// Token: 0x060098A1 RID: 39073 RVA: 0x0039F2BE File Offset: 0x0039D4BE
	public void UpdateBounds(string _boundsName, Bounds _bounds)
	{
		_bounds.Expand(this.cursorBuffer);
		this.activeBounds[_boundsName] = _bounds;
		this.RefreshBounds();
	}

	// Token: 0x060098A2 RID: 39074 RVA: 0x0039F2E0 File Offset: 0x0039D4E0
	public void RemoveBounds(string _boundsName)
	{
		this.activeBounds.Remove(_boundsName);
		this.RefreshBounds();
	}

	// Token: 0x060098A3 RID: 39075 RVA: 0x0039F2F8 File Offset: 0x0039D4F8
	public void RefreshBounds()
	{
		this.currentBounds.size = Vector3.zero;
		if (this.activeBounds.Count > 0)
		{
			bool flag = true;
			using (Dictionary<string, Bounds>.Enumerator enumerator = this.activeBounds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, Bounds> keyValuePair = enumerator.Current;
					if (flag)
					{
						this.currentBounds.center = keyValuePair.Value.center;
						flag = false;
					}
					this.currentBounds.Encapsulate(keyValuePair.Value);
				}
				return;
			}
		}
		this.currentBounds.center = new Vector3(0f, 0f);
		this.currentBounds.Encapsulate(new Vector3((float)this.uiCamera.cachedCamera.pixelWidth, (float)this.uiCamera.cachedCamera.pixelHeight));
	}

	// Token: 0x060098A4 RID: 39076 RVA: 0x0039F3E4 File Offset: 0x0039D5E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnResolutionChanged(int _width, int _height)
	{
		base.StartCoroutine(this.RefreshBoundsNextFrame());
	}

	// Token: 0x060098A5 RID: 39077 RVA: 0x0039F3F3 File Offset: 0x0039D5F3
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
		if (_enum == EnumGamePrefs.OptionsInterfaceSensitivity)
		{
			this.UpdateMoveSpeed();
			return;
		}
		if (_enum == EnumGamePrefs.OptionsControllerCursorSnap)
		{
			CursorControllerAbs.bSnapCursor = GamePrefs.GetBool(_enum);
			return;
		}
		if (_enum != EnumGamePrefs.OptionsControllerCursorHoverSensitivity)
		{
			return;
		}
		CursorControllerAbs.hoverSpeed = GamePrefs.GetFloat(_enum);
	}

	// Token: 0x060098A6 RID: 39078 RVA: 0x0039F42C File Offset: 0x0039D62C
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator RefreshBoundsNextFrame()
	{
		yield return null;
		this.RefreshBounds();
		yield break;
	}

	// Token: 0x060098A7 RID: 39079
	public abstract void SetCursorHidden(bool _hidden);

	// Token: 0x060098A8 RID: 39080
	public abstract bool GetCursorHidden();

	// Token: 0x060098A9 RID: 39081 RVA: 0x0039F43C File Offset: 0x0039D63C
	public bool GetMouseButtonDown(UICamera.MouseButton _mouseButton)
	{
		if (this.guiActions == null)
		{
			return false;
		}
		if (GUIWindowConsole.IsOpen())
		{
			return false;
		}
		switch (_mouseButton)
		{
		case UICamera.MouseButton.LeftButton:
			return this.guiActions.Submit.WasPressed || this.guiActions.LeftClick.WasPressed;
		case UICamera.MouseButton.RightButton:
			return this.guiActions.Inspect.WasPressed || this.guiActions.RightClick.WasPressed;
		case UICamera.MouseButton.MiddleButton:
			return this.guiActions.MiddleClick.WasPressed;
		default:
			return false;
		}
	}

	// Token: 0x060098AA RID: 39082 RVA: 0x0039F4CC File Offset: 0x0039D6CC
	public bool GetMouseButton(UICamera.MouseButton _mouseButton)
	{
		if (this.guiActions == null)
		{
			return false;
		}
		if (GUIWindowConsole.IsOpen())
		{
			return false;
		}
		switch (_mouseButton)
		{
		case UICamera.MouseButton.LeftButton:
			return this.guiActions.Submit.IsPressed || this.guiActions.LeftClick.IsPressed;
		case UICamera.MouseButton.RightButton:
			return this.guiActions.Inspect.IsPressed || this.guiActions.RightClick.IsPressed;
		case UICamera.MouseButton.MiddleButton:
			return this.guiActions.MiddleClick.IsPressed;
		default:
			return false;
		}
	}

	// Token: 0x060098AB RID: 39083 RVA: 0x0039F55C File Offset: 0x0039D75C
	public bool GetMouseButtonUp(UICamera.MouseButton _mouseButton)
	{
		if (this.guiActions == null)
		{
			return false;
		}
		switch (_mouseButton)
		{
		case UICamera.MouseButton.LeftButton:
			return this.guiActions.Submit.WasReleased || this.guiActions.LeftClick.WasReleased;
		case UICamera.MouseButton.RightButton:
			return this.guiActions.Inspect.WasReleased || this.guiActions.RightClick.WasReleased;
		case UICamera.MouseButton.MiddleButton:
			return this.guiActions.MiddleClick.WasReleased;
		default:
			return false;
		}
	}

	// Token: 0x060098AC RID: 39084 RVA: 0x0039F5E4 File Offset: 0x0039D7E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DebugDrawBound(Bounds _bound)
	{
		Vector3 vector = _bound.max;
		Vector3 vector2 = _bound.min;
		Vector3 vector3 = new Vector3(vector2.x, vector.y, vector.z);
		Vector3 vector4 = new Vector3(vector.x, vector2.y, vector2.z);
		vector = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector);
		vector3 = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector3);
		vector4 = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector4);
		vector2 = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector2);
		Debug.DrawLine(vector, vector3);
		Debug.DrawLine(vector4, vector2);
		Debug.DrawLine(vector, vector4);
		Debug.DrawLine(vector3, vector2);
	}

	// Token: 0x060098AD RID: 39085 RVA: 0x0039F697 File Offset: 0x0039D897
	public static void SetCursor(CursorControllerAbs.ECursorType _cursorType)
	{
		SoftCursor.SetCursor(_cursorType);
	}

	// Token: 0x060098AE RID: 39086 RVA: 0x0039F69F File Offset: 0x0039D89F
	public static void LoadStaticData(LoadManager.LoadGroup _loadGroup)
	{
		LoadManager.LoadAssetFromResources<GameObject>(CursorControllerAbs.softCursorPrefabPath, delegate(GameObject _asset)
		{
			CursorControllerAbs.softCursorPrefab = _asset;
		}, null, false, true);
	}

	// Token: 0x060098AF RID: 39087 RVA: 0x0039F6D0 File Offset: 0x0039D8D0
	public static CursorControllerAbs AddSoftCursor(UICamera _camera, PlayerActionsGUI _guiActions, GUIWindowManager _windowManager)
	{
		GameObject gameObject = _camera.gameObject.AddChild(CursorControllerAbs.softCursorPrefab);
		SoftCursor component = gameObject.GetComponent<SoftCursor>();
		component.SetGUIActions(_guiActions);
		component.SetWindowManager(_windowManager);
		_camera.cancelKey0 = KeyCode.None;
		_camera.submitKey1 = KeyCode.None;
		_camera.cancelKey1 = KeyCode.None;
		UICamera.GetMousePosition = new UICamera.GetMousePositionFunc(component.GetScreenPosition);
		UICamera.GetMouseButton = new UICamera.GetMouseButtonFunc(component.GetMouseButton);
		UICamera.GetMouseButtonDown = new UICamera.GetMouseButtonFunc(component.GetMouseButtonDown);
		UICamera.GetMouseButtonUp = new UICamera.GetMouseButtonFunc(component.GetMouseButtonUp);
		gameObject.SetActive(true);
		return component;
	}

	// Token: 0x060098B0 RID: 39088 RVA: 0x0039F763 File Offset: 0x0039D963
	public void PlayPagingSound()
	{
		Manager.PlayXUiSound(this.pagingSound, 1f);
	}

	// Token: 0x060098B1 RID: 39089 RVA: 0x0039F775 File Offset: 0x0039D975
	[PublicizedFrom(EAccessModifier.Protected)]
	public CursorControllerAbs()
	{
	}

	// Token: 0x040072B2 RID: 29362
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string softCursorPrefabPath = "Prefabs/SoftCursor";

	// Token: 0x040072B3 RID: 29363
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameObject softCursorPrefab;

	// Token: 0x040072B4 RID: 29364
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static List<CursorControllerAbs> softCursors = new List<CursorControllerAbs>();

	// Token: 0x040072B5 RID: 29365
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public UICamera uiCamera;

	// Token: 0x040072B6 RID: 29366
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public PlayerActionsGUI guiActions;

	// Token: 0x040072B7 RID: 29367
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x040072B8 RID: 29368
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public UISprite cursor;

	// Token: 0x040072B9 RID: 29369
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Bounds cursorWorldBounds;

	// Token: 0x040072BA RID: 29370
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 cursorBuffer;

	// Token: 0x040072BB RID: 29371
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<string, Bounds> activeBounds = new Dictionary<string, Bounds>();

	// Token: 0x040072BC RID: 29372
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Bounds currentBounds;

	// Token: 0x040072BD RID: 29373
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioClip cursorSelectSound;

	// Token: 0x040072BE RID: 29374
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioClip pagingSound;

	// Token: 0x040072BF RID: 29375
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public XUiView hoverTarget;

	// Token: 0x040072C0 RID: 29376
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bHasHoverTarget;

	// Token: 0x040072C1 RID: 29377
	public static bool bSnapCursor;

	// Token: 0x040072C2 RID: 29378
	public static float regularSpeed = 1f;

	// Token: 0x040072C3 RID: 29379
	public static float hoverSpeed = 1f;

	// Token: 0x040072C7 RID: 29383
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public AnimationCurve accelerationCurve;

	// Token: 0x040072C8 RID: 29384
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool _locked;

	// Token: 0x040072C9 RID: 29385
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool _virtualCursorHidden;

	// Token: 0x040072CA RID: 29386
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static bool FreeCursorEnabled = true;

	// Token: 0x020012CD RID: 4813
	public enum InputType
	{
		// Token: 0x040072CC RID: 29388
		Controller,
		// Token: 0x040072CD RID: 29389
		Mouse,
		// Token: 0x040072CE RID: 29390
		Both
	}

	// Token: 0x020012CE RID: 4814
	public enum ECursorType
	{
		// Token: 0x040072D0 RID: 29392
		None,
		// Token: 0x040072D1 RID: 29393
		Default,
		// Token: 0x040072D2 RID: 29394
		Map,
		// Token: 0x040072D3 RID: 29395
		Count
	}
}
