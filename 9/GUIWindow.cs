using System;
using UnityEngine;

// Token: 0x0200121C RID: 4636
public abstract class GUIWindow
{
	// Token: 0x060093FE RID: 37886 RVA: 0x0037FEF3 File Offset: 0x0037E0F3
	[PublicizedFrom(EAccessModifier.Protected)]
	public GUIWindow(string _id)
	{
		this.Id = _id;
	}

	// Token: 0x060093FF RID: 37887 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnGUI()
	{
	}

	// Token: 0x06009400 RID: 37888 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Update()
	{
	}

	// Token: 0x06009401 RID: 37889 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnOpen()
	{
	}

	// Token: 0x06009402 RID: 37890 RVA: 0x0037FF0D File Offset: 0x0037E10D
	public virtual void OnClose()
	{
		Action onWindowClose = this.OnWindowClose;
		if (onWindowClose != null)
		{
			onWindowClose();
		}
		this.OnWindowClose = null;
	}

	// Token: 0x06009403 RID: 37891 RVA: 0x0037FF27 File Offset: 0x0037E127
	public virtual PlayerActionsBase GetActionSet()
	{
		return this.playerUI.playerInput.GUIActions;
	}

	// Token: 0x06009404 RID: 37892 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool HasActionSet()
	{
		return true;
	}

	// Token: 0x06009405 RID: 37893 RVA: 0x0037FF3C File Offset: 0x0037E13C
	public override bool Equals(object _obj)
	{
		GUIWindow guiwindow = _obj as GUIWindow;
		return guiwindow != null && guiwindow.Id.Equals(this.Id);
	}

	// Token: 0x06009406 RID: 37894 RVA: 0x0037FF66 File Offset: 0x0037E166
	public override int GetHashCode()
	{
		return this.Id.GetHashCode();
	}

	// Token: 0x06009407 RID: 37895 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Cleanup()
	{
	}

	// Token: 0x06009408 RID: 37896 RVA: 0x0037FF74 File Offset: 0x0037E174
	public static Matrix4x4 UiScaleMatrix(out float _targetScale, out float _actualScale, float _x = 0f, float _y = 0f, float _clampMin = 0.4f, float _clampMax = 2f)
	{
		_targetScale = (float)Screen.height / 1080f;
		_targetScale *= GameOptionsManager.GetActiveUiScale();
		_actualScale = Utils.FastClamp(_targetScale, _clampMin, _clampMax);
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = Matrix4x4.TRS(new Vector3(_x, _y), Quaternion.identity, new Vector3(_actualScale, _actualScale, 1f));
		return matrix;
	}

	// Token: 0x04006EE8 RID: 28392
	public readonly string Id;

	// Token: 0x04006EE9 RID: 28393
	public bool bActionSetEnabled;

	// Token: 0x04006EEA RID: 28394
	public bool isShowing;

	// Token: 0x04006EEB RID: 28395
	public bool isModal;

	// Token: 0x04006EEC RID: 28396
	public bool alwaysUsesMouseCursor;

	// Token: 0x04006EED RID: 28397
	public bool isEscClosable;

	// Token: 0x04006EEE RID: 28398
	public bool isInputActive;

	// Token: 0x04006EEF RID: 28399
	public GUIWindowManager windowManager;

	// Token: 0x04006EF0 RID: 28400
	public LocalPlayerUI playerUI;

	// Token: 0x04006EF1 RID: 28401
	public string openWindowOnEsc = string.Empty;

	// Token: 0x04006EF2 RID: 28402
	public Action OnWindowClose;
}
