using System;
using System.Collections.Generic;
using InControl;
using Platform;
using UnityEngine;

namespace GUI_2
{
	// Token: 0x020016C2 RID: 5826
	public static class UIUtils
	{
		// Token: 0x1700162E RID: 5678
		// (get) Token: 0x0600B60F RID: 46607 RVA: 0x0043D189 File Offset: 0x0043B389
		public static INGUIAtlas IconAtlas
		{
			get
			{
				return UIUtils.symbolAtlas;
			}
		}

		// Token: 0x0600B610 RID: 46608 RVA: 0x0043D190 File Offset: 0x0043B390
		public static string GetSpriteName(UIUtils.ButtonIcon _icon)
		{
			if (PlayerInputManager.InputStyleFromSelectedIconStyle() != PlayerInputManager.InputStyle.PS4)
			{
				return UIUtils.buttonIconMapXB[_icon];
			}
			return UIUtils.buttonIconMapPS[_icon];
		}

		// Token: 0x0600B611 RID: 46609 RVA: 0x0043D1B4 File Offset: 0x0043B3B4
		public static UIUtils.ButtonIcon GetButtonIconForAction(PlayerAction _action)
		{
			if (_action == null)
			{
				return UIUtils.ButtonIcon.None;
			}
			DeviceBindingSource deviceBindingSource = _action.GetBindingOfType(true) as DeviceBindingSource;
			if (deviceBindingSource == null)
			{
				if (UIUtils.loggedMissingBindingSources.Add(_action))
				{
					Log.Warning("UIUtils: No device binding source could be found for PlayerAction {0}", new object[]
					{
						_action.Name
					});
				}
				return UIUtils.ButtonIcon.None;
			}
			UIUtils.ButtonIcon result;
			if (UIUtils.iconControlMap.TryGetValue(deviceBindingSource.Control, out result))
			{
				return result;
			}
			Log.Warning("UIUtils: Could not assign a ButtonIcon for device control {0}", new object[]
			{
				deviceBindingSource.Control.ToString()
			});
			return UIUtils.ButtonIcon.None;
		}

		// Token: 0x0600B612 RID: 46610 RVA: 0x0043D245 File Offset: 0x0043B445
		public static void LoadAtlas()
		{
			UIUtils.symbolAtlas = Resources.Load<UIAtlas>("GUI/Prefabs/SymbolAtlas");
		}

		// Token: 0x0400887D RID: 34941
		[PublicizedFrom(EAccessModifier.Private)]
		public static INGUIAtlas symbolAtlas;

		// Token: 0x0400887E RID: 34942
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<UIUtils.ButtonIcon, string> buttonIconMapPS = new EnumDictionary<UIUtils.ButtonIcon, string>
		{
			{
				UIUtils.ButtonIcon.FaceButtonSouth,
				"PS5_Button_Action1"
			},
			{
				UIUtils.ButtonIcon.FaceButtonNorth,
				"PS5_Button_Action4"
			},
			{
				UIUtils.ButtonIcon.FaceButtonEast,
				"PS5_Button_Action2"
			},
			{
				UIUtils.ButtonIcon.FaceButtonWest,
				"PS5_Button_Action3"
			},
			{
				UIUtils.ButtonIcon.ConfirmButton,
				"PS5_Button_Action1"
			},
			{
				UIUtils.ButtonIcon.CancelButton,
				"PS5_Button_Action2"
			},
			{
				UIUtils.ButtonIcon.LeftBumper,
				"PS5_Button_LeftBumper"
			},
			{
				UIUtils.ButtonIcon.LeftTrigger,
				"PS5_Button_LeftTrigger"
			},
			{
				UIUtils.ButtonIcon.RightBumper,
				"PS5_Button_RightBumper"
			},
			{
				UIUtils.ButtonIcon.RightTrigger,
				"PS5_Button_RightTrigger"
			},
			{
				UIUtils.ButtonIcon.LeftStick,
				"PS5_Button_LeftStick"
			},
			{
				UIUtils.ButtonIcon.LeftStickUpDown,
				"PS5_Button_LeftStickUpDown"
			},
			{
				UIUtils.ButtonIcon.LeftStickLeftRight,
				"PS5_Button_LeftStickLeftRight"
			},
			{
				UIUtils.ButtonIcon.LeftStickButton,
				"PS5_Button_LeftStickButton"
			},
			{
				UIUtils.ButtonIcon.LeftStickLeft,
				"PS5_Button_LeftStickLeft"
			},
			{
				UIUtils.ButtonIcon.LeftStickRight,
				"PS5_Button_LeftStickRight"
			},
			{
				UIUtils.ButtonIcon.LeftStickDown,
				"PS5_Button_LeftStickDown"
			},
			{
				UIUtils.ButtonIcon.LeftStickUp,
				"PS5_Button_LeftStickUp"
			},
			{
				UIUtils.ButtonIcon.RightStick,
				"PS5_Button_RightStick"
			},
			{
				UIUtils.ButtonIcon.RightStickUpDown,
				"PS5_Button_RightStickUpDown"
			},
			{
				UIUtils.ButtonIcon.RightStickLeftRight,
				"PS5_Button_RightStickLeftRight"
			},
			{
				UIUtils.ButtonIcon.RightStickButton,
				"PS5_Button_RightStickButton"
			},
			{
				UIUtils.ButtonIcon.RightStickLeft,
				"PS5_Button_RightStickLeft"
			},
			{
				UIUtils.ButtonIcon.RightStickRight,
				"PS5_Button_RightStickRight"
			},
			{
				UIUtils.ButtonIcon.RightStickDown,
				"PS5_Button_RightStickDown"
			},
			{
				UIUtils.ButtonIcon.RightStickUp,
				"PS5_Button_RightStickUp"
			},
			{
				UIUtils.ButtonIcon.DPadLeft,
				"PS5_Button_DPadLeft"
			},
			{
				UIUtils.ButtonIcon.DPadRight,
				"PS5_Button_DPadRight"
			},
			{
				UIUtils.ButtonIcon.DPadUp,
				"PS5_Button_DPadUp"
			},
			{
				UIUtils.ButtonIcon.DPadDown,
				"PS5_Button_DPadDown"
			},
			{
				UIUtils.ButtonIcon.StartButton,
				"PS5_Button_Start"
			},
			{
				UIUtils.ButtonIcon.BackButton,
				"PS5_Button_Back"
			},
			{
				UIUtils.ButtonIcon.None,
				""
			}
		};

		// Token: 0x0400887F RID: 34943
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<UIUtils.ButtonIcon, string> buttonIconMapXB = new EnumDictionary<UIUtils.ButtonIcon, string>
		{
			{
				UIUtils.ButtonIcon.FaceButtonSouth,
				"XB_Button_Action1"
			},
			{
				UIUtils.ButtonIcon.FaceButtonNorth,
				"XB_Button_Action4"
			},
			{
				UIUtils.ButtonIcon.FaceButtonEast,
				"XB_Button_Action2"
			},
			{
				UIUtils.ButtonIcon.FaceButtonWest,
				"XB_Button_Action3"
			},
			{
				UIUtils.ButtonIcon.ConfirmButton,
				"XB_Button_Action1"
			},
			{
				UIUtils.ButtonIcon.CancelButton,
				"XB_Button_Action2"
			},
			{
				UIUtils.ButtonIcon.LeftBumper,
				"XB_Button_LeftBumper"
			},
			{
				UIUtils.ButtonIcon.LeftTrigger,
				"XB_Button_LeftTrigger"
			},
			{
				UIUtils.ButtonIcon.RightBumper,
				"XB_Button_RightBumper"
			},
			{
				UIUtils.ButtonIcon.RightTrigger,
				"XB_Button_RightTrigger"
			},
			{
				UIUtils.ButtonIcon.LeftStick,
				"XB_Button_LeftStick"
			},
			{
				UIUtils.ButtonIcon.LeftStickUpDown,
				"XB_Button_LeftStickUpDown"
			},
			{
				UIUtils.ButtonIcon.LeftStickLeftRight,
				"XB_Button_LeftStickLeftRight"
			},
			{
				UIUtils.ButtonIcon.LeftStickButton,
				"XB_Button_LeftStickButton"
			},
			{
				UIUtils.ButtonIcon.LeftStickLeft,
				"XB_Button_LeftStickLeft"
			},
			{
				UIUtils.ButtonIcon.LeftStickRight,
				"XB_Button_LeftStickRight"
			},
			{
				UIUtils.ButtonIcon.LeftStickDown,
				"XB_Button_LeftStickDown"
			},
			{
				UIUtils.ButtonIcon.LeftStickUp,
				"XB_Button_LeftStickUp"
			},
			{
				UIUtils.ButtonIcon.RightStick,
				"XB_Button_RightStick"
			},
			{
				UIUtils.ButtonIcon.RightStickUpDown,
				"XB_Button_RightStickUpDown"
			},
			{
				UIUtils.ButtonIcon.RightStickLeftRight,
				"XB_Button_RightStickLeftRight"
			},
			{
				UIUtils.ButtonIcon.RightStickButton,
				"XB_Button_RightStickButton"
			},
			{
				UIUtils.ButtonIcon.RightStickLeft,
				"XB_Button_RightStickLeft"
			},
			{
				UIUtils.ButtonIcon.RightStickRight,
				"XB_Button_RightStickRight"
			},
			{
				UIUtils.ButtonIcon.RightStickDown,
				"XB_Button_RightStickDown"
			},
			{
				UIUtils.ButtonIcon.RightStickUp,
				"XB_Button_RightStickUp"
			},
			{
				UIUtils.ButtonIcon.DPadLeft,
				"XB_Button_DPadLeft"
			},
			{
				UIUtils.ButtonIcon.DPadRight,
				"XB_Button_DPadRight"
			},
			{
				UIUtils.ButtonIcon.DPadUp,
				"XB_Button_DPadUp"
			},
			{
				UIUtils.ButtonIcon.DPadDown,
				"XB_Button_DPadDown"
			},
			{
				UIUtils.ButtonIcon.StartButton,
				"XB_Button_Menu"
			},
			{
				UIUtils.ButtonIcon.BackButton,
				"XB_Button_Back"
			},
			{
				UIUtils.ButtonIcon.None,
				""
			}
		};

		// Token: 0x04008880 RID: 34944
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<InputControlType, UIUtils.ButtonIcon> iconControlMap = new EnumDictionary<InputControlType, UIUtils.ButtonIcon>
		{
			{
				InputControlType.Action1,
				UIUtils.ButtonIcon.FaceButtonSouth
			},
			{
				InputControlType.Action2,
				UIUtils.ButtonIcon.FaceButtonEast
			},
			{
				InputControlType.Action3,
				UIUtils.ButtonIcon.FaceButtonWest
			},
			{
				InputControlType.Action4,
				UIUtils.ButtonIcon.FaceButtonNorth
			},
			{
				InputControlType.LeftBumper,
				UIUtils.ButtonIcon.LeftBumper
			},
			{
				InputControlType.RightBumper,
				UIUtils.ButtonIcon.RightBumper
			},
			{
				InputControlType.LeftTrigger,
				UIUtils.ButtonIcon.LeftTrigger
			},
			{
				InputControlType.RightTrigger,
				UIUtils.ButtonIcon.RightTrigger
			},
			{
				InputControlType.LeftStickButton,
				UIUtils.ButtonIcon.LeftStickButton
			},
			{
				InputControlType.LeftStickX,
				UIUtils.ButtonIcon.LeftStickLeftRight
			},
			{
				InputControlType.LeftStickY,
				UIUtils.ButtonIcon.LeftStickUpDown
			},
			{
				InputControlType.LeftStickLeft,
				UIUtils.ButtonIcon.LeftStickLeft
			},
			{
				InputControlType.LeftStickRight,
				UIUtils.ButtonIcon.LeftStickRight
			},
			{
				InputControlType.LeftStickDown,
				UIUtils.ButtonIcon.LeftStickDown
			},
			{
				InputControlType.LeftStickUp,
				UIUtils.ButtonIcon.LeftStickUp
			},
			{
				InputControlType.RightStickButton,
				UIUtils.ButtonIcon.RightStickButton
			},
			{
				InputControlType.RightStickX,
				UIUtils.ButtonIcon.RightStickLeftRight
			},
			{
				InputControlType.RightStickY,
				UIUtils.ButtonIcon.RightStickUpDown
			},
			{
				InputControlType.RightStickLeft,
				UIUtils.ButtonIcon.RightStickLeft
			},
			{
				InputControlType.RightStickRight,
				UIUtils.ButtonIcon.RightStickRight
			},
			{
				InputControlType.RightStickDown,
				UIUtils.ButtonIcon.RightStickDown
			},
			{
				InputControlType.RightStickUp,
				UIUtils.ButtonIcon.RightStickUp
			},
			{
				InputControlType.DPadUp,
				UIUtils.ButtonIcon.DPadUp
			},
			{
				InputControlType.DPadDown,
				UIUtils.ButtonIcon.DPadDown
			},
			{
				InputControlType.DPadLeft,
				UIUtils.ButtonIcon.DPadLeft
			},
			{
				InputControlType.DPadRight,
				UIUtils.ButtonIcon.DPadRight
			},
			{
				InputControlType.Start,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Menu,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Options,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Plus,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Select,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.View,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.TouchPadButton,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.Minus,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.None,
				UIUtils.ButtonIcon.None
			}
		};

		// Token: 0x04008881 RID: 34945
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly HashSet<PlayerAction> loggedMissingBindingSources = new HashSet<PlayerAction>();

		// Token: 0x020016C3 RID: 5827
		public enum ButtonIcon
		{
			// Token: 0x04008883 RID: 34947
			FaceButtonSouth,
			// Token: 0x04008884 RID: 34948
			FaceButtonNorth,
			// Token: 0x04008885 RID: 34949
			FaceButtonEast,
			// Token: 0x04008886 RID: 34950
			FaceButtonWest,
			// Token: 0x04008887 RID: 34951
			ConfirmButton,
			// Token: 0x04008888 RID: 34952
			CancelButton,
			// Token: 0x04008889 RID: 34953
			LeftBumper,
			// Token: 0x0400888A RID: 34954
			RightBumper,
			// Token: 0x0400888B RID: 34955
			LeftTrigger,
			// Token: 0x0400888C RID: 34956
			RightTrigger,
			// Token: 0x0400888D RID: 34957
			LeftStick,
			// Token: 0x0400888E RID: 34958
			LeftStickUpDown,
			// Token: 0x0400888F RID: 34959
			LeftStickLeftRight,
			// Token: 0x04008890 RID: 34960
			LeftStickButton,
			// Token: 0x04008891 RID: 34961
			LeftStickLeft,
			// Token: 0x04008892 RID: 34962
			LeftStickRight,
			// Token: 0x04008893 RID: 34963
			LeftStickUp,
			// Token: 0x04008894 RID: 34964
			LeftStickDown,
			// Token: 0x04008895 RID: 34965
			RightStick,
			// Token: 0x04008896 RID: 34966
			RightStickUpDown,
			// Token: 0x04008897 RID: 34967
			RightStickLeftRight,
			// Token: 0x04008898 RID: 34968
			RightStickButton,
			// Token: 0x04008899 RID: 34969
			RightStickLeft,
			// Token: 0x0400889A RID: 34970
			RightStickRight,
			// Token: 0x0400889B RID: 34971
			RightStickUp,
			// Token: 0x0400889C RID: 34972
			RightStickDown,
			// Token: 0x0400889D RID: 34973
			DPadLeft,
			// Token: 0x0400889E RID: 34974
			DPadRight,
			// Token: 0x0400889F RID: 34975
			DPadUp,
			// Token: 0x040088A0 RID: 34976
			DPadDown,
			// Token: 0x040088A1 RID: 34977
			StartButton,
			// Token: 0x040088A2 RID: 34978
			BackButton,
			// Token: 0x040088A3 RID: 34979
			None,
			// Token: 0x040088A4 RID: 34980
			Count
		}
	}
}
