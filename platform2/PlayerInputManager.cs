using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InControl;
using UnityEngine;

namespace Platform
{
	// Token: 0x02001BF3 RID: 7155
	public class PlayerInputManager
	{
		// Token: 0x0600D4B5 RID: 54453 RVA: 0x004CE9B0 File Offset: 0x004CCBB0
		public PlayerInputManager()
		{
			Log.Out("Starting PlayerInputManager...");
			MouseBindingSource.ScaleX = (MouseBindingSource.ScaleY = (MouseBindingSource.ScaleZ = 0.2f));
			GameObject gameObject = GameObject.Find("Input");
			if (gameObject == null)
			{
				gameObject = new GameObject("Input");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			InControlManager inControlManager = gameObject.GetComponent<InControlManager>();
			if (inControlManager != null)
			{
				Log.Error("InControl already instantiated");
				return;
			}
			bool flag = GameUtils.GetLaunchArgument("noxinput") == null;
			bool enableNativeInput = GameUtils.GetLaunchArgument("disablenativeinput") == null;
			if (GameManager.IsDedicatedServer)
			{
				flag = false;
				enableNativeInput = false;
			}
			gameObject.SetActive(false);
			inControlManager = gameObject.AddComponent<InControlManager>();
			inControlManager.logDebugInfo = false;
			inControlManager.suspendInBackground = true;
			inControlManager.nativeInputPreventSleep = true;
			inControlManager.enableNativeInput = enableNativeInput;
			inControlManager.enableXInput = flag;
			inControlManager.nativeInputEnableXInput = flag;
			InputManager.AddCustomDeviceManagers += delegate(ref bool enableUnityInput)
			{
			};
			InControl.Logger.OnLogMessage += delegate(LogMessage _message)
			{
				switch (_message.Type)
				{
				case LogMessageType.Info:
					Log.Out(_message.Text);
					return;
				case LogMessageType.Warning:
					Log.Warning(_message.Text);
					return;
				case LogMessageType.Error:
					Log.Error(_message.Text);
					return;
				default:
					return;
				}
			};
			InControl.Logger.LogInfo(string.Concat(new string[]
			{
				"InControl (version ",
				InputManager.Version.ToString(),
				", native module = ",
				inControlManager.enableNativeInput.ToString(),
				", XInput = ",
				inControlManager.enableXInput.ToString(),
				")"
			}));
			gameObject.SetActive(true);
			PlayerActionsGlobal.Init();
			if (!Submission.Enabled)
			{
				this.actionSets.Add(PlayerActionsGlobal.Instance);
			}
			this.PrimaryPlayer = new PlayerActionsLocal();
			this.actionSets.Add(this.PrimaryPlayer);
			this.actionSets.Add(this.PrimaryPlayer.VehicleActions);
			this.actionSets.Add(this.PrimaryPlayer.GUIActions);
			this.actionSets.Add(this.PrimaryPlayer.PermanentActions);
			this.ActionSets = new ReadOnlyCollection<PlayerActionsBase>(this.actionSets);
			for (int i = 0; i < this.actionSets.Count; i++)
			{
				PlayerActionSet actionSet = this.actionSets[i];
				actionSet.OnLastInputTypeChanged += delegate(BindingSourceType _type)
				{
					if (_type == BindingSourceType.DeviceBindingSource)
					{
						this.newInputDevice = (actionSet.Device ?? InputManager.ActiveDevice);
						return;
					}
					this.newInputDevice = InputDevice.Null;
				};
			}
			if (!GameManager.IsDedicatedServer)
			{
				this.ActionSetManager.Push(this.PrimaryPlayer);
			}
			this.CurrentInputStyle = this.defaultInputStyle;
		}

		// Token: 0x0600D4B6 RID: 54454 RVA: 0x004CEC6C File Offset: 0x004CCE6C
		public void Update()
		{
			BindingSourceType bindingSourceType;
			this.newInputDevice = this.LastActiveInputDevice(out bindingSourceType);
			if (!this.firstInputDetected && bindingSourceType == BindingSourceType.None)
			{
				return;
			}
			if (this.lastInputDevice == this.newInputDevice)
			{
				if (bindingSourceType == this.lastBindingSource)
				{
					return;
				}
				if (bindingSourceType == BindingSourceType.KeyBindingSource && this.lastBindingSource == BindingSourceType.MouseBindingSource)
				{
					return;
				}
				if (bindingSourceType == BindingSourceType.MouseBindingSource && this.lastBindingSource == BindingSourceType.KeyBindingSource)
				{
					return;
				}
			}
			if (!this.firstInputDetected)
			{
				this.firstInputDetected = true;
			}
			this.lastInputDevice = this.newInputDevice;
			this.lastBindingSource = bindingSourceType;
			PlayerInputManager.InputStyle currentInputStyle = this.CurrentInputStyle;
			if (bindingSourceType == BindingSourceType.KeyBindingSource || bindingSourceType == BindingSourceType.MouseBindingSource)
			{
				this.lastInputDeviceName = null;
				this.CurrentInputStyle = PlayerInputManager.InputStyle.Keyboard;
			}
			else if (this.lastInputDevice.Name == "None" || bindingSourceType == BindingSourceType.None)
			{
				this.lastInputDeviceName = null;
				this.CurrentInputStyle = this.defaultInputStyle;
			}
			else
			{
				string name = this.lastInputDevice.Name;
				if (name != this.lastInputDeviceName)
				{
					this.lastInputDeviceName = name;
					this.CurrentInputStyle = ((this.lastInputDevice.DeviceStyle == InputDeviceStyle.PlayStation2 || this.lastInputDevice.DeviceStyle == InputDeviceStyle.PlayStation3 || this.lastInputDevice.DeviceStyle == InputDeviceStyle.PlayStation4 || this.lastInputDevice.DeviceStyle == InputDeviceStyle.PlayStation5) ? PlayerInputManager.InputStyle.PS4 : PlayerInputManager.InputStyle.XB1);
				}
			}
			if (currentInputStyle != this.CurrentInputStyle && currentInputStyle != PlayerInputManager.InputStyle.Undefined)
			{
				float unscaledTime = Time.unscaledTime;
				this.inputStylesUsedMinutes[(int)currentInputStyle] += (unscaledTime - this.lastInputStyleSwitchTime) / 60f;
				this.lastInputStyleSwitchTime = unscaledTime;
			}
			Action<PlayerInputManager.InputStyle> onLastInputStyleChanged = this.OnLastInputStyleChanged;
			if (onLastInputStyleChanged == null)
			{
				return;
			}
			onLastInputStyleChanged(this.CurrentInputStyle);
		}

		// Token: 0x0600D4B7 RID: 54455 RVA: 0x004CEDEA File Offset: 0x004CCFEA
		public void ForceInputStyleChange()
		{
			Action<PlayerInputManager.InputStyle> onLastInputStyleChanged = this.OnLastInputStyleChanged;
			if (onLastInputStyleChanged == null)
			{
				return;
			}
			onLastInputStyleChanged(this.CurrentInputStyle);
		}

		// Token: 0x1400012A RID: 298
		// (add) Token: 0x0600D4B8 RID: 54456 RVA: 0x004CEE04 File Offset: 0x004CD004
		// (remove) Token: 0x0600D4B9 RID: 54457 RVA: 0x004CEE3C File Offset: 0x004CD03C
		public event Action<PlayerInputManager.InputStyle> OnLastInputStyleChanged;

		// Token: 0x17001A63 RID: 6755
		// (get) Token: 0x0600D4BA RID: 54458 RVA: 0x004CEE71 File Offset: 0x004CD071
		// (set) Token: 0x0600D4BB RID: 54459 RVA: 0x004CEE79 File Offset: 0x004CD079
		public PlayerInputManager.InputStyle CurrentInputStyle
		{
			get
			{
				return this._currentInputStyle;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				if (value == PlayerInputManager.InputStyle.PS4 || value == PlayerInputManager.InputStyle.XB1)
				{
					this.CurrentControllerInputStyle = value;
				}
				this._currentInputStyle = value;
			}
		}

		// Token: 0x17001A64 RID: 6756
		// (get) Token: 0x0600D4BC RID: 54460 RVA: 0x004CEE91 File Offset: 0x004CD091
		// (set) Token: 0x0600D4BD RID: 54461 RVA: 0x004CEEAF File Offset: 0x004CD0AF
		public PlayerInputManager.InputStyle CurrentControllerInputStyle
		{
			get
			{
				if (DeviceFlag.PS5.IsCurrent())
				{
					return PlayerInputManager.InputStyle.PS4;
				}
				if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent())
				{
					return PlayerInputManager.InputStyle.XB1;
				}
				return this._currentControllerInputStyle;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._currentControllerInputStyle = value;
			}
		}

		// Token: 0x17001A65 RID: 6757
		// (get) Token: 0x0600D4BE RID: 54462 RVA: 0x004CEEB8 File Offset: 0x004CD0B8
		public ReadOnlyCollection<PlayerActionsBase> ActionSets { get; }

		// Token: 0x0600D4BF RID: 54463 RVA: 0x004CEEC0 File Offset: 0x004CD0C0
		[PublicizedFrom(EAccessModifier.Private)]
		public InputDevice LastActiveInputDevice(out BindingSourceType lastBindingSource)
		{
			ulong num = 0UL;
			PlayerActionSet playerActionSet = null;
			lastBindingSource = BindingSourceType.None;
			for (int i = 0; i < this.ActionSets.Count; i++)
			{
				PlayerActionSet playerActionSet2 = this.ActionSets[i];
				if (playerActionSet2.Enabled && playerActionSet2.LastInputTypeChangedTick > num)
				{
					playerActionSet = playerActionSet2;
					num = playerActionSet2.LastInputTypeChangedTick;
				}
			}
			if (playerActionSet != null)
			{
				lastBindingSource = playerActionSet.LastInputType;
				if (playerActionSet.LastInputType == BindingSourceType.DeviceBindingSource)
				{
					return playerActionSet.Device ?? InputManager.ActiveDevice;
				}
			}
			return InputDevice.Null;
		}

		// Token: 0x0600D4C0 RID: 54464 RVA: 0x004CEF3C File Offset: 0x004CD13C
		public void ResetInputStyleUsage()
		{
			for (int i = 0; i < this.inputStylesUsedMinutes.Length; i++)
			{
				this.inputStylesUsedMinutes[i] = 0f;
				this.lastInputStyleSwitchTime = Time.unscaledTime;
			}
		}

		// Token: 0x0600D4C1 RID: 54465 RVA: 0x004CEF74 File Offset: 0x004CD174
		public PlayerInputManager.InputStyle MostUsedInputStyle()
		{
			if (this.CurrentInputStyle != PlayerInputManager.InputStyle.Undefined)
			{
				float unscaledTime = Time.unscaledTime;
				this.inputStylesUsedMinutes[(int)this.CurrentInputStyle] += (unscaledTime - this.lastInputStyleSwitchTime) / 60f;
				this.lastInputStyleSwitchTime = unscaledTime;
			}
			PlayerInputManager.InputStyle result = PlayerInputManager.InputStyle.Count;
			float num = -1f;
			for (int i = 0; i < this.inputStylesUsedMinutes.Length; i++)
			{
				if (this.inputStylesUsedMinutes[i] > num)
				{
					num = this.inputStylesUsedMinutes[i];
					result = (PlayerInputManager.InputStyle)i;
				}
			}
			return result;
		}

		// Token: 0x0600D4C2 RID: 54466 RVA: 0x004CEFEC File Offset: 0x004CD1EC
		public PlayerActionsBase GetActionSetForName(string _name)
		{
			foreach (PlayerActionsBase playerActionsBase in this.ActionSets)
			{
				if (playerActionsBase.Name.EqualsCaseInsensitive(_name))
				{
					return playerActionsBase;
				}
			}
			return null;
		}

		// Token: 0x17001A66 RID: 6758
		// (get) Token: 0x0600D4C3 RID: 54467 RVA: 0x004CF048 File Offset: 0x004CD248
		public PlayerActionsLocal PrimaryPlayer { get; }

		// Token: 0x0600D4C4 RID: 54468 RVA: 0x004CF050 File Offset: 0x004CD250
		public void LoadActionSetsFromStrings(IList<string> actionSets)
		{
			if (this.ActionSets.Count != actionSets.Count)
			{
				Log.Warning(string.Format("Loading ActionSets from string array with incorrect length. Expected: {0}. Actual: {1}.", this.ActionSets.Count, (actionSets != null) ? new int?(actionSets.Count) : null));
				return;
			}
			for (int i = 0; i < this.ActionSets.Count; i++)
			{
				this.ActionSets[i].Load(actionSets[i]);
			}
		}

		// Token: 0x0600D4C5 RID: 54469 RVA: 0x004CF0DC File Offset: 0x004CD2DC
		public static PlayerInputManager.InputStyle InputStyleFromSelectedIconStyle()
		{
			PlayerInputManager.ControllerIconStyle @int = (PlayerInputManager.ControllerIconStyle)GamePrefs.GetInt(EnumGamePrefs.OptionsControllerIconStyle);
			if (@int == PlayerInputManager.ControllerIconStyle.Xbox)
			{
				return PlayerInputManager.InputStyle.XB1;
			}
			if (@int == PlayerInputManager.ControllerIconStyle.Playstation)
			{
				return PlayerInputManager.InputStyle.PS4;
			}
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			PlayerInputManager.InputStyle? inputStyle;
			if (nativePlatform == null)
			{
				inputStyle = null;
			}
			else
			{
				PlayerInputManager input = nativePlatform.Input;
				inputStyle = ((input != null) ? new PlayerInputManager.InputStyle?(input.CurrentControllerInputStyle) : null);
			}
			PlayerInputManager.InputStyle? inputStyle2 = inputStyle;
			if (inputStyle2 == null)
			{
				return PlayerInputManager.InputStyle.XB1;
			}
			return inputStyle2.GetValueOrDefault();
		}

		// Token: 0x0400A20D RID: 41485
		[PublicizedFrom(EAccessModifier.Private)]
		public PlayerInputManager.InputStyle defaultInputStyle = PlayerInputManager.InputStyle.Keyboard;

		// Token: 0x0400A20E RID: 41486
		[PublicizedFrom(EAccessModifier.Private)]
		public bool firstInputDetected;

		// Token: 0x0400A20F RID: 41487
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float[] inputStylesUsedMinutes = new float[4];

		// Token: 0x0400A210 RID: 41488
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastInputStyleSwitchTime;

		// Token: 0x0400A212 RID: 41490
		[PublicizedFrom(EAccessModifier.Private)]
		public string lastInputDeviceName;

		// Token: 0x0400A213 RID: 41491
		[PublicizedFrom(EAccessModifier.Private)]
		public InputDevice lastInputDevice;

		// Token: 0x0400A214 RID: 41492
		[PublicizedFrom(EAccessModifier.Private)]
		public InputDevice newInputDevice;

		// Token: 0x0400A215 RID: 41493
		[PublicizedFrom(EAccessModifier.Private)]
		public BindingSourceType lastBindingSource;

		// Token: 0x0400A216 RID: 41494
		[PublicizedFrom(EAccessModifier.Private)]
		public PlayerInputManager.InputStyle _currentInputStyle;

		// Token: 0x0400A217 RID: 41495
		[PublicizedFrom(EAccessModifier.Private)]
		public PlayerInputManager.InputStyle _currentControllerInputStyle = PlayerInputManager.InputStyle.XB1;

		// Token: 0x0400A219 RID: 41497
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<PlayerActionsBase> actionSets = new List<PlayerActionsBase>();

		// Token: 0x0400A21B RID: 41499
		public readonly ActionSetManager ActionSetManager = new ActionSetManager();

		// Token: 0x02001BF4 RID: 7156
		public enum InputStyle
		{
			// Token: 0x0400A21D RID: 41501
			Undefined,
			// Token: 0x0400A21E RID: 41502
			Keyboard,
			// Token: 0x0400A21F RID: 41503
			PS4,
			// Token: 0x0400A220 RID: 41504
			XB1,
			// Token: 0x0400A221 RID: 41505
			Count
		}

		// Token: 0x02001BF5 RID: 7157
		public enum ControllerIconStyle
		{
			// Token: 0x0400A223 RID: 41507
			Automatic,
			// Token: 0x0400A224 RID: 41508
			Xbox,
			// Token: 0x0400A225 RID: 41509
			Playstation
		}
	}
}
