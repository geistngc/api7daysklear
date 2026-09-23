using System;
using System.Collections;
using Platform;
using UnityEngine;

// Token: 0x020012D3 RID: 4819
public class FlexibleCursor : CursorControllerAbs
{
	// Token: 0x17001220 RID: 4640
	// (get) Token: 0x060098F5 RID: 39157 RVA: 0x0039F8EB File Offset: 0x0039DAEB
	public bool SoftcursorAllowed
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.guiActions != null && this.guiActions.Enabled && LocalPlayerUI.AnyModalWindowOpen();
		}
	}

	// Token: 0x060098F6 RID: 39158 RVA: 0x003A0A70 File Offset: 0x0039EC70
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.AwakeBase();
		this.speedMultiplier = this.speed;
		this.cursor = base.GetComponentInChildren<UISprite>();
		if (FlexibleCursor.defaultMouseCursor == null)
		{
			FlexibleCursor.emptyCursor = new Texture2D(32, 32, TextureFormat.ARGB32, false);
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 32; j++)
				{
					FlexibleCursor.emptyCursor.SetPixel(i, j, new Color(0f, 0f, 0f, 0.01f));
				}
			}
			FlexibleCursor.emptyCursor.Apply();
			FlexibleCursor.defaultMouseCursor = Resources.Load<Texture2D>(FlexibleCursor.defaultMouseCursorResource);
			FlexibleCursor.defaultControllerCursor = Resources.Load<Texture2D>(FlexibleCursor.defaultControllerCursorResource);
			FlexibleCursor.mapCursor = Resources.Load<Texture2D>(FlexibleCursor.mapCursorResource);
		}
		UISprite[] componentsInChildren = base.GetComponentsInChildren<UISprite>(true);
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			componentsInChildren[k].gameObject.SetActive(false);
		}
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged += this.OnLastInputStyleChanged;
		FlexibleCursor.SetCursor(FlexibleCursor.currentCursorType);
	}

	// Token: 0x060098F7 RID: 39159 RVA: 0x003A0B7A File Offset: 0x0039ED7A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLastInputStyleChanged(PlayerInputManager.InputStyle _style)
	{
		FlexibleCursor.SetCursor(FlexibleCursor.currentCursorType);
	}

	// Token: 0x060098F8 RID: 39160 RVA: 0x0039FB73 File Offset: 0x0039DD73
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.uiCamera = base.GetComponentInParent<UICamera>();
		this.UpdateMoveSpeed();
		base.InitCursorBounds();
	}

	// Token: 0x060098F9 RID: 39161 RVA: 0x003A0B86 File Offset: 0x0039ED86
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		IPlatform nativePlatform = PlatformManager.NativePlatform;
		if (((nativePlatform != null) ? nativePlatform.Input : null) != null)
		{
			PlatformManager.NativePlatform.Input.OnLastInputStyleChanged -= this.OnLastInputStyleChanged;
		}
		base.DestroyBase();
	}

	// Token: 0x060098FA RID: 39162 RVA: 0x003A0BBC File Offset: 0x0039EDBC
	public override void UpdateMoveSpeed()
	{
		CursorControllerAbs.regularSpeed = GamePrefs.GetFloat(EnumGamePrefs.OptionsInterfaceSensitivity);
		this.speed = 500f + 1000f * CursorControllerAbs.regularSpeed;
	}

	// Token: 0x060098FB RID: 39163 RVA: 0x003A0BE4 File Offset: 0x0039EDE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Application.isPlaying)
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (((nativePlatform != null) ? nativePlatform.Input : null) != null && PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard && this.SoftcursorAllowed)
			{
				this.HandleControllerInput();
			}
			this.LastFrameTime = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060098FC RID: 39164 RVA: 0x003A0C38 File Offset: 0x0039EE38
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleControllerInput()
	{
		if (this.guiActions == null)
		{
			return;
		}
		Vector2 vector = new Vector2(this.guiActions.Right.RawValue - this.guiActions.Left.RawValue, this.guiActions.Up.RawValue - this.guiActions.Down.RawValue);
		float magnitude = vector.magnitude;
		Vector3 vector2 = this.GetScreenPosition();
		Vector3 vector3 = vector2;
		if (this.bHasHoverTarget && (this.HoverTarget == null || !this.HoverTarget.ColliderEnabled || !this.HoverTarget.IsActiveInHierarchy))
		{
			this.HoverTarget = null;
		}
		float b = this.bHasHoverTarget ? CursorControllerAbs.hoverSpeed : 1f;
		this.currentAcceleration = Mathf.Clamp(this.currentAcceleration + magnitude * Time.unscaledDeltaTime, 0f, Mathf.Min(magnitude, b));
		this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, this.bHasHoverTarget ? CursorControllerAbs.hoverSpeed : 1f, Time.unscaledDeltaTime * (this.bHasHoverTarget ? 10f : 1f));
		float num = Time.unscaledDeltaTime * this.speed * this.speedMultiplier * this.accelerationCurve.Evaluate(this.currentAcceleration);
		vector.x *= num;
		vector.y *= num;
		vector3.x += vector.x;
		vector3.y += vector.y;
		if (CursorControllerAbs.bSnapCursor)
		{
			if (vector2 == vector3)
			{
				if (!this.snapped)
				{
					vector3 = this.SnapOs(vector3);
					this.snapped = true;
				}
			}
			else
			{
				this.snapped = false;
			}
		}
		vector3 = this.ConstrainCursorOs(vector3);
		this.SetScreenPosition(vector3.x, vector3.y);
	}

	// Token: 0x060098FD RID: 39165 RVA: 0x003A0E08 File Offset: 0x0039F008
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 SnapOs(Vector3 _newPos)
	{
		if (this.hoverTarget == null || !this.hoverTarget.IsActiveInHierarchy)
		{
			return _newPos;
		}
		if (this.cursorWorldBounds.extents.x > this.hoverTarget.ColliderBounds.extents.x - this.OffsetSnapBounds)
		{
			return this.uiCamera.cachedCamera.WorldToScreenPoint(this.hoverTarget.ColliderBounds.center);
		}
		Vector3 vector = this.hoverTarget.ColliderBounds.ClosestPoint(this.uiCamera.cachedCamera.ScreenToWorldPoint(_newPos));
		Vector3 b = Vector3.right * this.cursorWorldBounds.extents.x;
		Vector3 point = vector - b;
		Vector3 point2 = vector + b;
		if (!this.hoverTarget.ColliderBounds.Contains(point))
		{
			vector = this.hoverTarget.ColliderBounds.ClosestPoint(point) + b;
		}
		else if (!this.hoverTarget.ColliderBounds.Contains(point2))
		{
			vector = this.hoverTarget.ColliderBounds.ClosestPoint(point2) - b;
		}
		vector.y = this.hoverTarget.ColliderBounds.center.y;
		return this.uiCamera.cachedCamera.WorldToScreenPoint(vector);
	}

	// Token: 0x060098FE RID: 39166 RVA: 0x003A0F70 File Offset: 0x0039F170
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 ConstrainCursorOs(Vector3 _newPos)
	{
		Vector3 vector = this.ConstrainToBounds(_newPos);
		vector.x = Mathf.Clamp(vector.x, 5f, (float)(Screen.width - 5));
		vector.y = Mathf.Clamp(vector.y, 5f, (float)(Screen.height - 5));
		return vector;
	}

	// Token: 0x060098FF RID: 39167 RVA: 0x003A0FC4 File Offset: 0x0039F1C4
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 ConstrainToBounds(Vector3 _newPosition)
	{
		Vector3 point = _newPosition;
		point.z = this.currentBounds.center.z;
		return this.currentBounds.ClosestPoint(point);
	}

	// Token: 0x06009900 RID: 39168 RVA: 0x003A0FF8 File Offset: 0x0039F1F8
	public override Vector2 GetScreenPosition()
	{
		return MouseLib.GetLocalMousePosition();
	}

	// Token: 0x06009901 RID: 39169 RVA: 0x003A0FF8 File Offset: 0x0039F1F8
	public override Vector2 GetLocalScreenPosition()
	{
		return MouseLib.GetLocalMousePosition();
	}

	// Token: 0x06009902 RID: 39170 RVA: 0x003A0FFF File Offset: 0x0039F1FF
	public override void SetScreenPosition(Vector2 _newPosition)
	{
		MouseLib.SetCursorPosition((int)(_newPosition.x + 0.5f), (int)(_newPosition.y + 0.5f));
	}

	// Token: 0x06009903 RID: 39171 RVA: 0x003A0264 File Offset: 0x0039E464
	public override void SetScreenPosition(float _x, float _y)
	{
		this.SetScreenPosition(new Vector2(_x, _y));
	}

	// Token: 0x06009904 RID: 39172 RVA: 0x003A1020 File Offset: 0x0039F220
	public override void ResetToCenter()
	{
		this.SetScreenPosition((float)(Screen.width / 2), (float)(Screen.height / 2));
	}

	// Token: 0x06009905 RID: 39173 RVA: 0x000880CC File Offset: 0x000862CC
	public override void SetNavigationTarget(XUiView _view)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06009906 RID: 39174 RVA: 0x000880CC File Offset: 0x000862CC
	public override void SetNavigationTargetLater(XUiView _view)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06009907 RID: 39175 RVA: 0x000880CC File Offset: 0x000862CC
	public override void ResetNavigationTarget()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06009908 RID: 39176 RVA: 0x000880CC File Offset: 0x000862CC
	public override void SetNavigationLockView(XUiView _view, XUiView _viewToSelect = null)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06009909 RID: 39177 RVA: 0x000880CC File Offset: 0x000862CC
	public override bool IsWithinNavigationLockView(XUiView _view)
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600990A RID: 39178 RVA: 0x000880CC File Offset: 0x000862CC
	public override void RefreshSelection()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600990B RID: 39179 RVA: 0x003A1038 File Offset: 0x0039F238
	public override void SetCursorHidden(bool _hidden)
	{
		GameManager.Instance.SetCursorEnabledOverride(_hidden, false);
	}

	// Token: 0x0600990C RID: 39180 RVA: 0x003A1046 File Offset: 0x0039F246
	public override bool GetCursorHidden()
	{
		return GameManager.Instance.GetCursorEnabledOverride();
	}

	// Token: 0x0600990D RID: 39181 RVA: 0x000880CC File Offset: 0x000862CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnVirtualCursorVisibleChanged()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600990E RID: 39182 RVA: 0x003A1052 File Offset: 0x0039F252
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator ApplyCursorChangeLater()
	{
		while (!Cursor.visible)
		{
			yield return null;
		}
		Cursor.SetCursor(FlexibleCursor.currentCursorTexture, FlexibleCursor.currentCursorHotspot, CursorMode.Auto);
		FlexibleCursor.cursorUpdateCo = null;
		yield break;
	}

	// Token: 0x0600990F RID: 39183 RVA: 0x003A105A File Offset: 0x0039F25A
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetCursorTexture(Texture2D _tex, Vector2 _hotspot)
	{
		if (_tex != FlexibleCursor.currentCursorTexture)
		{
			FlexibleCursor.currentCursorTexture = _tex;
			FlexibleCursor.currentCursorHotspot = _hotspot;
			if (FlexibleCursor.cursorUpdateCo == null)
			{
				FlexibleCursor.cursorUpdateCo = ThreadManager.StartCoroutine(FlexibleCursor.ApplyCursorChangeLater());
			}
		}
	}

	// Token: 0x06009910 RID: 39184 RVA: 0x003A108C File Offset: 0x0039F28C
	public new static void SetCursor(CursorControllerAbs.ECursorType _cursorType)
	{
		FlexibleCursor.currentCursorType = _cursorType;
		switch (_cursorType)
		{
		case CursorControllerAbs.ECursorType.None:
			FlexibleCursor.SetCursorTexture(FlexibleCursor.emptyCursor, FlexibleCursor.mapCursorCenter);
			return;
		case CursorControllerAbs.ECursorType.Default:
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
			{
				FlexibleCursor.SetCursorTexture(FlexibleCursor.defaultMouseCursor, FlexibleCursor.defaultMouseCursorCenter);
				return;
			}
			FlexibleCursor.SetCursorTexture(FlexibleCursor.defaultControllerCursor, FlexibleCursor.defaultControllerCursorCenter);
			return;
		case CursorControllerAbs.ECursorType.Map:
			FlexibleCursor.SetCursorTexture(FlexibleCursor.mapCursor, FlexibleCursor.mapCursorCenter);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x04007301 RID: 29441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float BaseSpeed = 500f;

	// Token: 0x04007302 RID: 29442
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float SpeedModRange = 1000f;

	// Token: 0x04007303 RID: 29443
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string defaultMouseCursorResource = "@:Textures/UI/cursor01.tga";

	// Token: 0x04007304 RID: 29444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly Vector2 defaultMouseCursorCenter = Vector2.zero;

	// Token: 0x04007305 RID: 29445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string defaultControllerCursorResource = "@:Textures/UI/soft_cursor";

	// Token: 0x04007306 RID: 29446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly Vector2 defaultControllerCursorCenter = new Vector2(16f, 16f);

	// Token: 0x04007307 RID: 29447
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string mapCursorResource = "@:Textures/UI/map_cursor.tga";

	// Token: 0x04007308 RID: 29448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly Vector2 mapCursorCenter = new Vector2(16f, 16f);

	// Token: 0x04007309 RID: 29449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture2D emptyCursor;

	// Token: 0x0400730A RID: 29450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture2D defaultControllerCursor;

	// Token: 0x0400730B RID: 29451
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture2D defaultMouseCursor;

	// Token: 0x0400730C RID: 29452
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture2D mapCursor;

	// Token: 0x0400730D RID: 29453
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture2D currentCursorTexture;

	// Token: 0x0400730E RID: 29454
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Vector2 currentCursorHotspot;

	// Token: 0x0400730F RID: 29455
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static CursorControllerAbs.ECursorType currentCursorType = CursorControllerAbs.ECursorType.Default;

	// Token: 0x04007310 RID: 29456
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Coroutine cursorUpdateCo;

	// Token: 0x04007311 RID: 29457
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speed;

	// Token: 0x04007312 RID: 29458
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedMultiplier = 1f;

	// Token: 0x04007313 RID: 29459
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float LastFrameTime;

	// Token: 0x04007314 RID: 29460
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool snapped;

	// Token: 0x04007315 RID: 29461
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentAcceleration;

	// Token: 0x04007316 RID: 29462
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float OffsetSnapBounds = 0.1f;

	// Token: 0x04007317 RID: 29463
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public PlayerInputManager.InputStyle m_lastInputStyle = PlayerInputManager.InputStyle.Count;

	// Token: 0x04007318 RID: 29464
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int CONTROLLER_CURSOR_MOVEMENT_LIMIT = 5;
}
