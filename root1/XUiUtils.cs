using System;
using System.Collections.Generic;
using System.Text;
using InControl;
using Platform;
using UnityEngine;

// Token: 0x02001182 RID: 4482
public static class XUiUtils
{
	// Token: 0x06008FAE RID: 36782 RVA: 0x00360508 File Offset: 0x0035E708
	public static string GetBindingXuiMarkupString(this PlayerAction _action, XUiUtils.EmptyBindingStyle _emptyStyle = XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle _displayStyle = XUiUtils.DisplayStyle.Plain, string _customDisplayStyle = null)
	{
		if (_action == null)
		{
			return "";
		}
		string name = ((PlayerActionsBase)_action.Owner).Name;
		string name2 = _action.Name;
		bool flag = _emptyStyle > XUiUtils.EmptyBindingStyle.EmptyString;
		bool flag2 = !string.IsNullOrEmpty(_customDisplayStyle);
		bool flag3 = _displayStyle > XUiUtils.DisplayStyle.Plain || flag2;
		string text = (flag || flag3) ? (":" + _emptyStyle.ToStringCached<XUiUtils.EmptyBindingStyle>()) : "";
		string text2 = flag3 ? (":" + (flag2 ? _customDisplayStyle : _displayStyle.ToStringCached<XUiUtils.DisplayStyle>())) : "";
		return string.Concat(new string[]
		{
			"[action:",
			name,
			":",
			name2,
			text,
			text2,
			"]"
		});
	}

	// Token: 0x06008FAF RID: 36783 RVA: 0x003605C0 File Offset: 0x0035E7C0
	public static bool ParseActionsMarkup(XUi _xui, string _input, out string _parsed, string _defaultCustomFormat = null, XUiUtils.ForceLabelInputStyle _forceInputStyle = XUiUtils.ForceLabelInputStyle.Off)
	{
		bool result = false;
		int num;
		while ((num = _input.IndexOf("[action:", StringComparison.OrdinalIgnoreCase)) >= 0)
		{
			int num2 = num + "[action:".Length;
			int num3 = _input.IndexOf(':', num2);
			int num4 = _input.IndexOf(':', num3 + 1);
			int num5 = _input.IndexOf(':', num4 + 1);
			int i = 0;
			int num6 = num2;
			while (i >= 0)
			{
				int num7 = _input.IndexOf('[', num6);
				int num8 = _input.IndexOf(']', num6);
				bool flag = num7 >= 0;
				bool flag2 = num8 >= 0;
				if (flag && num7 < num8)
				{
					i++;
					num6 = num7 + 1;
				}
				else
				{
					if (!flag2)
					{
						break;
					}
					i--;
					num6 = num8 + 1;
				}
			}
			if (i >= 0)
			{
				Log.Warning("[XUi] Could not parse action descriptor in label text, no closing bracket found");
			}
			else
			{
				int num9 = num6 - 1;
				bool flag3 = num4 >= 0 && num4 < num9;
				bool flag4 = flag3 && num5 >= 0 && num5 < num9;
				if (num9 < 0)
				{
					Log.Warning("[XUi] Could not parse action descriptor in label text, no closing bracket found");
				}
				else if (num3 < 0 || num3 > num9)
				{
					Log.Warning("[XUi] Could not parse action descriptor in label text, no separator between action set name and action found");
				}
				else
				{
					int num10 = flag3 ? (num4 - 1) : (num9 - 1);
					int num11 = flag4 ? (num5 - 1) : (num9 - 1);
					string text = _input.Substring(num2, num3 - num2);
					string text2 = _input.Substring(num3 + 1, num10 - num3);
					string text3 = flag3 ? _input.Substring(num4 + 1, num11 - num4) : null;
					string text4 = flag4 ? _input.Substring(num5 + 1, num9 - num5 - 1) : null;
					PlayerActionsBase actionSetForName = PlatformManager.NativePlatform.Input.GetActionSetForName(text);
					if (actionSetForName == null)
					{
						Log.Warning("[XUi] Could not parse action descriptor in label text, action set \"" + text + "\" not found");
					}
					else
					{
						PlayerAction playerActionByName = actionSetForName.GetPlayerActionByName(text2);
						if (playerActionByName != null)
						{
							XUiUtils.EmptyBindingStyle emptyStyle = XUiUtils.EmptyBindingStyle.EmptyString;
							if (flag3 && text3.Length > 0 && !EnumUtils.TryParse<XUiUtils.EmptyBindingStyle>(text3, out emptyStyle, true))
							{
								Log.Warning("[XUi] Could not parse action descriptor empty style, \"" + text3 + "\" unknown");
							}
							bool isCustomDisplayStyle = false;
							XUiUtils.DisplayStyle displayStyle = XUiUtils.DisplayStyle.Plain;
							if (flag4 && !EnumUtils.TryParse<XUiUtils.DisplayStyle>(text4, out displayStyle, true))
							{
								if (text4.Length < 1)
								{
									Log.Warning("[XUi] Could not parse action descriptor display type, \"" + text4 + "\" unknown");
								}
								else if (text4.IndexOf("###", StringComparison.Ordinal) < 0)
								{
									Log.Warning("[XUi] Could not parse action descriptor display type, \"" + text4 + "\" assumed to be a custom format, missing the '#' placeholder");
								}
								else
								{
									isCustomDisplayStyle = true;
								}
							}
							if (!flag4 && !string.IsNullOrEmpty(_defaultCustomFormat))
							{
								isCustomDisplayStyle = true;
								text4 = _defaultCustomFormat;
							}
							string bindingString = playerActionByName.GetBindingString(_forceInputStyle != XUiUtils.ForceLabelInputStyle.Keyboard && (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard || _forceInputStyle == XUiUtils.ForceLabelInputStyle.Controller), XUiUtils.GetInputStyleFromForcedStyle(_forceInputStyle), emptyStyle, displayStyle, isCustomDisplayStyle, text4);
							_input = _input.Remove(num, num9 - num + 1);
							_input = _input.Insert(num, bindingString);
							result = true;
							continue;
						}
						Log.Warning("[XUi] Could not parse action descriptor in label text, action \"" + text2 + "\" not found");
					}
				}
			}
			IL_337:
			while ((num = _input.IndexOf("[button:", StringComparison.OrdinalIgnoreCase)) >= 0)
			{
				int num12 = num + "[button:".Length;
				int num13 = _input.IndexOf(']', num);
				if (num13 < 0)
				{
					Log.Warning("[XUi] Could not parse button descriptor in label text, no closing bracket found");
					break;
				}
				string value = InControlExtensions.TryLocalizeButtonName(_input.Substring(num12, num13 - num12));
				_input = _input.Remove(num, num13 - num + 1);
				_input = _input.Insert(num, value);
				result = true;
			}
			_parsed = _input;
			return result;
		}
		goto IL_337;
	}

	// Token: 0x06008FB0 RID: 36784 RVA: 0x00360919 File Offset: 0x0035EB19
	[PublicizedFrom(EAccessModifier.Private)]
	public static PlayerInputManager.InputStyle GetInputStyleFromForcedStyle(XUiUtils.ForceLabelInputStyle _forceStyle)
	{
		if (_forceStyle == XUiUtils.ForceLabelInputStyle.Keyboard)
		{
			return PlayerInputManager.InputStyle.Keyboard;
		}
		if (_forceStyle != XUiUtils.ForceLabelInputStyle.Controller)
		{
			return PlatformManager.NativePlatform.Input.CurrentInputStyle;
		}
		return PlatformManager.NativePlatform.Input.CurrentControllerInputStyle;
	}

	// Token: 0x06008FB1 RID: 36785 RVA: 0x00360948 File Offset: 0x0035EB48
	public static string GetXuiHierarchy(XUiController _current)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XUiUtils.getXuiHierarchyRec(_current, stringBuilder);
		return stringBuilder.ToString();
	}

	// Token: 0x06008FB2 RID: 36786 RVA: 0x00360968 File Offset: 0x0035EB68
	[PublicizedFrom(EAccessModifier.Private)]
	public static void getXuiHierarchyRec(XUiController _current, StringBuilder _sb)
	{
		if (_current.Parent != null)
		{
			XUiUtils.getXuiHierarchyRec(_current.Parent, _sb);
			_sb.Append(" -> ");
		}
		string text;
		string id;
		if (_current.ViewComponent != null)
		{
			text = _current.ViewComponent.GetType().Name.Replace("XUiV_", "");
			id = _current.ViewComponent.ID;
		}
		else
		{
			text = "windowgroup";
			id = _current.WindowGroup.Id;
		}
		if (text.EqualsCaseInsensitive(id))
		{
			_sb.Append(id);
			return;
		}
		_sb.Append(text);
		_sb.Append(" (");
		_sb.Append(id);
		_sb.Append(")");
	}

	// Token: 0x06008FB3 RID: 36787 RVA: 0x00360A18 File Offset: 0x0035EC18
	public static string ToXuiColorString(this Color32 _color)
	{
		return string.Format("{0},{1},{2},{3}", new object[]
		{
			_color.r,
			_color.g,
			_color.b,
			_color.a
		});
	}

	// Token: 0x06008FB4 RID: 36788 RVA: 0x00360A70 File Offset: 0x0035EC70
	public static XUiView FindHierarchyClosestView(XUiController _startController, string _name)
	{
		XUiController controller = _startController.WindowGroup.Controller;
		XUiController parent = _startController.Parent;
		XUiController childById;
		for (;;)
		{
			childById = parent.GetChildById(_name);
			if (childById != null)
			{
				break;
			}
			parent = parent.Parent;
			if (parent == controller)
			{
				goto Block_2;
			}
		}
		return childById.ViewComponent;
		Block_2:
		return null;
	}

	// Token: 0x06008FB5 RID: 36789 RVA: 0x00360AB0 File Offset: 0x0035ECB0
	public static bool HotkeysAllowedFor(XUiView _view, bool _ignoreViewLock = false)
	{
		XUi xui = _view.xui;
		CursorControllerAbs cursorController = xui.playerUI.CursorController;
		GUIWindowManager windowManager = xui.playerUI.windowManager;
		return _view.IsVisible && !GUIWindowConsole.IsOpen() && windowManager.IsKeyShortcutsAllowed() && (_ignoreViewLock || cursorController.IsWithinNavigationLockView(_view)) && !XUiC_FullScreenCollider.IsBlocked(_view);
	}

	// Token: 0x06008FB6 RID: 36790 RVA: 0x00360B10 File Offset: 0x0035ED10
	public static void ApplyFillPaddedSprite(UISprite _uiSprite, string _origSpriteName)
	{
		INGUIAtlas atlas = _uiSprite.atlas;
		if (atlas == null)
		{
			return;
		}
		XUiUtils.modifyAtlas(_uiSprite, atlas, _origSpriteName);
	}

	// Token: 0x06008FB7 RID: 36791 RVA: 0x00360B30 File Offset: 0x0035ED30
	[PublicizedFrom(EAccessModifier.Private)]
	public static void modifyAtlas(UISprite _uiSprite, INGUIAtlas _atlas, string _origSpriteName)
	{
		if (_atlas.replacement != null)
		{
			XUiUtils.modifyAtlas(_uiSprite, _atlas.replacement, _origSpriteName);
			return;
		}
		string filledSpriteName = _origSpriteName + "##filled";
		List<UISpriteData> spriteList = _atlas.spriteList;
		if (spriteList.FindIndex((UISpriteData _data) => _data.name == filledSpriteName) >= 0)
		{
			_uiSprite.spriteName = filledSpriteName;
			return;
		}
		int num = spriteList.FindIndex((UISpriteData _data) => _data.name == _origSpriteName);
		if (num < 0)
		{
			return;
		}
		UISpriteData uispriteData = new UISpriteData();
		uispriteData.CopyFrom(spriteList[num]);
		uispriteData.name = filledSpriteName;
		uispriteData.x++;
		uispriteData.y++;
		uispriteData.width -= 2;
		uispriteData.height -= 2;
		spriteList.Add(uispriteData);
		XUiUtils.setAtlasSpriteList(_atlas, spriteList);
		_uiSprite.spriteName = filledSpriteName;
	}

	// Token: 0x06008FB8 RID: 36792 RVA: 0x00360C30 File Offset: 0x0035EE30
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setAtlasSpriteList(INGUIAtlas _atlas, List<UISpriteData> _list)
	{
		UIAtlas uiatlas = _atlas as UIAtlas;
		if (uiatlas != null)
		{
			uiatlas.spriteList = _list;
			uiatlas.MarkSpriteListAsChanged();
			return;
		}
		NGUIAtlas nguiatlas = _atlas as NGUIAtlas;
		if (nguiatlas != null)
		{
			nguiatlas.spriteList = _list;
			return;
		}
		throw new Exception();
	}

	// Token: 0x02001183 RID: 4483
	public enum EmptyBindingStyle
	{
		// Token: 0x04006925 RID: 26917
		EmptyString,
		// Token: 0x04006926 RID: 26918
		NullString,
		// Token: 0x04006927 RID: 26919
		LocalizedUnbound,
		// Token: 0x04006928 RID: 26920
		LocalizedNone
	}

	// Token: 0x02001184 RID: 4484
	public enum DisplayStyle
	{
		// Token: 0x0400692A RID: 26922
		Plain,
		// Token: 0x0400692B RID: 26923
		KeyboardWithAngleBrackets,
		// Token: 0x0400692C RID: 26924
		KeyboardWithParentheses,
		// Token: 0x0400692D RID: 26925
		KeyboardWithBrackets
	}

	// Token: 0x02001185 RID: 4485
	public enum ForceLabelInputStyle
	{
		// Token: 0x0400692F RID: 26927
		Off,
		// Token: 0x04006930 RID: 26928
		Keyboard,
		// Token: 0x04006931 RID: 26929
		Controller
	}
}
