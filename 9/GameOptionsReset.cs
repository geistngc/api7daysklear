using System;
using System.Collections.Generic;

// Token: 0x020011DB RID: 4571
public static class GameOptionsReset
{
	// Token: 0x06009258 RID: 37464 RVA: 0x00373318 File Offset: 0x00371518
	public static void Init()
	{
		GameOptionsReset.Audio = new GameOptionsReset.AudioGroup();
		GameOptionsReset.Graphics = new GameOptionsReset.GraphicsGroup();
		GameOptionsReset.Controls = new GameOptionsReset.ControlsGroup();
		GameOptionsReset.Controller = new GameOptionsReset.ControllerGroup();
		GameOptionsReset.Bindings = new GameOptionsReset.BindingsGroup();
		GameOptionsReset.groups = new List<GameOptionsReset.IGroup>
		{
			GameOptionsReset.Audio,
			GameOptionsReset.Graphics,
			GameOptionsReset.Controls,
			GameOptionsReset.Controller,
			GameOptionsReset.Bindings
		};
	}

	// Token: 0x06009259 RID: 37465 RVA: 0x00373398 File Offset: 0x00371598
	public static string GetGroupId(EnumGamePrefs enumGamePref)
	{
		return GameOptionsReset.GetGroupId(enumGamePref.ToStringCached<EnumGamePrefs>());
	}

	// Token: 0x0600925A RID: 37466 RVA: 0x003733A8 File Offset: 0x003715A8
	public static string GetGroupId(string prefName)
	{
		foreach (GameOptionsReset.IGroup group in GameOptionsReset.groups)
		{
			if (group.HasPref(prefName))
			{
				return group.VersionId;
			}
		}
		return GameOptionsReset.defaultGroupId;
	}

	// Token: 0x0600925B RID: 37467 RVA: 0x0037340C File Offset: 0x0037160C
	[PublicizedFrom(EAccessModifier.Private)]
	public static PrefType EnumTypeToPrefType(GamePrefs.EnumType enumType)
	{
		switch (enumType)
		{
		case GamePrefs.EnumType.Int:
			return PrefType.Int;
		case GamePrefs.EnumType.Float:
			return PrefType.Float;
		case GamePrefs.EnumType.String:
			return PrefType.String;
		case GamePrefs.EnumType.Bool:
			return PrefType.Int;
		case GamePrefs.EnumType.Binary:
			return PrefType.String;
		default:
			throw new Exception(string.Format("Unmapped {0} for {1}: {2}", "PrefType", "EnumType", enumType));
		}
	}

	// Token: 0x0600925C RID: 37468 RVA: 0x00373460 File Offset: 0x00371660
	public static bool TryGetPrefType(EnumGamePrefs enumGamePref, out PrefType prefType)
	{
		GamePrefs.EnumType? prefType2 = GamePrefs.GetPrefType(enumGamePref);
		if (prefType2 == null)
		{
			Log.Error(string.Format("Unknown enum type for {0}", enumGamePref));
			prefType = PrefType.Float;
			return false;
		}
		prefType = GameOptionsReset.EnumTypeToPrefType(prefType2.Value);
		return true;
	}

	// Token: 0x0600925D RID: 37469 RVA: 0x003734A6 File Offset: 0x003716A6
	public static bool NeedsResetGame()
	{
		return 13 != GamePrefs.GetInt(EnumGamePrefs.LastGameResetRevision);
	}

	// Token: 0x0600925E RID: 37470 RVA: 0x003734B8 File Offset: 0x003716B8
	public static void ResetGame(PrefVersionStore versionedPrefs)
	{
		GamePrefs.PropertyDecl[] propertyList = GamePrefs.GetPropertyList();
		for (int i = 0; i < propertyList.Length; i++)
		{
			GameOptionsReset.ResetPref(propertyList[i].name, versionedPrefs);
		}
		GameOptionsReset.Graphics.Reset(versionedPrefs);
		GameOptionsReset.Controls.Reset(versionedPrefs);
		GameOptionsReset.Bindings.Reset(versionedPrefs);
		GamePrefs.Set(EnumGamePrefs.LastGameResetRevision, 13);
	}

	// Token: 0x0600925F RID: 37471 RVA: 0x00373518 File Offset: 0x00371718
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ResetPref(EnumGamePrefs enumGamePref, PrefVersionStore versionedPrefs)
	{
		object @default;
		if (versionedPrefs == null || !versionedPrefs.TryGetGamePref(enumGamePref, out @default))
		{
			@default = GamePrefs.GetDefault(enumGamePref);
		}
		GamePrefs.SetObject(enumGamePref, @default);
	}

	// Token: 0x04006C1C RID: 27676
	public static GameOptionsReset.AudioGroup Audio;

	// Token: 0x04006C1D RID: 27677
	public static GameOptionsReset.GraphicsGroup Graphics;

	// Token: 0x04006C1E RID: 27678
	public static GameOptionsReset.ControlsGroup Controls;

	// Token: 0x04006C1F RID: 27679
	public static GameOptionsReset.ControllerGroup Controller;

	// Token: 0x04006C20 RID: 27680
	public static GameOptionsReset.BindingsGroup Bindings;

	// Token: 0x04006C21 RID: 27681
	public static List<GameOptionsReset.IGroup> groups;

	// Token: 0x04006C22 RID: 27682
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string defaultGroupId = string.Format("DefaultGroup_{0}", 13);

	// Token: 0x020011DC RID: 4572
	public interface IGroup
	{
		// Token: 0x17001174 RID: 4468
		// (get) Token: 0x06009261 RID: 37473
		string VersionId { get; }

		// Token: 0x06009262 RID: 37474
		bool HasPref(string prefName);

		// Token: 0x06009263 RID: 37475
		bool NeedsReset();

		// Token: 0x06009264 RID: 37476
		void Reset(PrefVersionStore versionedPrefs);
	}

	// Token: 0x020011DD RID: 4573
	public abstract class EnumGamePrefGroup : GameOptionsReset.IGroup
	{
		// Token: 0x17001175 RID: 4469
		// (get) Token: 0x06009265 RID: 37477
		public abstract string VersionId { get; }

		// Token: 0x06009266 RID: 37478 RVA: 0x0037355C File Offset: 0x0037175C
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumGamePrefGroup(params EnumGamePrefs[] enumGamePrefs)
		{
			foreach (EnumGamePrefs enumGamePref in enumGamePrefs)
			{
				this.AddEnumGamePref(enumGamePref);
			}
		}

		// Token: 0x06009267 RID: 37479 RVA: 0x003735A0 File Offset: 0x003717A0
		[PublicizedFrom(EAccessModifier.Protected)]
		public void AddEnumGamePref(EnumGamePrefs enumGamePref)
		{
			if (!this.EnumPrefs.Add(enumGamePref))
			{
				return;
			}
			this.PrefNames.Add(enumGamePref.ToStringCached<EnumGamePrefs>());
		}

		// Token: 0x06009268 RID: 37480 RVA: 0x003735C3 File Offset: 0x003717C3
		public virtual bool HasPref(string prefName)
		{
			return this.PrefNames.Contains(prefName);
		}

		// Token: 0x06009269 RID: 37481 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool NeedsReset()
		{
			return false;
		}

		// Token: 0x0600926A RID: 37482 RVA: 0x003735D4 File Offset: 0x003717D4
		public virtual void Reset(PrefVersionStore versionedPrefs)
		{
			foreach (EnumGamePrefs enumGamePref in this.EnumPrefs)
			{
				GameOptionsReset.ResetPref(enumGamePref, versionedPrefs);
			}
		}

		// Token: 0x04006C23 RID: 27683
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<EnumGamePrefs> EnumPrefs = new HashSet<EnumGamePrefs>();

		// Token: 0x04006C24 RID: 27684
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<string> PrefNames = new HashSet<string>();
	}

	// Token: 0x020011DE RID: 4574
	public class AudioGroup : GameOptionsReset.EnumGamePrefGroup
	{
		// Token: 0x17001176 RID: 4470
		// (get) Token: 0x0600926B RID: 37483 RVA: 0x00373628 File Offset: 0x00371828
		public override string VersionId
		{
			get
			{
				return string.Format("Audio_{0}_0", 13);
			}
		}

		// Token: 0x0600926C RID: 37484 RVA: 0x0037363B File Offset: 0x0037183B
		public AudioGroup() : base(new EnumGamePrefs[]
		{
			EnumGamePrefs.OptionsAmbientVolumeLevel,
			EnumGamePrefs.OptionsMusicVolumeLevel,
			EnumGamePrefs.OptionsMenuMusicVolumeLevel,
			EnumGamePrefs.OptionsDynamicMusicEnabled,
			EnumGamePrefs.OptionsDynamicMusicDailyTime,
			EnumGamePrefs.OptionsMicVolumeLevel,
			EnumGamePrefs.OptionsVoiceVolumeLevel,
			EnumGamePrefs.OptionsOverallAudioVolumeLevel,
			EnumGamePrefs.OptionsVoiceChatEnabled,
			EnumGamePrefs.OptionsAudioOcclusion,
			EnumGamePrefs.OptionsSubtitlesEnabled
		})
		{
		}

		// Token: 0x0600926D RID: 37485 RVA: 0x00373655 File Offset: 0x00371855
		public override void Reset(PrefVersionStore versionedPrefs)
		{
			Log.Out("Resetting game options AudioGroup");
			base.Reset(versionedPrefs);
		}
	}

	// Token: 0x020011DF RID: 4575
	public class GraphicsGroup : GameOptionsReset.EnumGamePrefGroup
	{
		// Token: 0x17001177 RID: 4471
		// (get) Token: 0x0600926E RID: 37486 RVA: 0x00373668 File Offset: 0x00371868
		public override string VersionId
		{
			get
			{
				return string.Format("Graphics_{0}_{1}", 13, 4);
			}
		}

		// Token: 0x0600926F RID: 37487 RVA: 0x00373684 File Offset: 0x00371884
		public GraphicsGroup() : base(new EnumGamePrefs[]
		{
			EnumGamePrefs.OptionsGfxResolution,
			EnumGamePrefs.OptionsGfxVsync,
			EnumGamePrefs.OptionsGfxDynamicMode,
			EnumGamePrefs.OptionsGfxUpscalerMode,
			EnumGamePrefs.OptionsGfxFSRPreset,
			EnumGamePrefs.OptionsGfxDynamicScale,
			EnumGamePrefs.OptionsGfxBrightness,
			EnumGamePrefs.OptionsGfxSignQuality,
			EnumGamePrefs.DynamicMeshEnabled,
			EnumGamePrefs.DynamicMeshDistance,
			EnumGamePrefs.NoGraphicsMode
		})
		{
			base.AddEnumGamePref(EnumGamePrefs.OptionsGfxQualityPreset);
			foreach (EnumGamePrefs enumGamePref in GameOptionsManager.QualityPresets.Keys)
			{
				base.AddEnumGamePref(enumGamePref);
			}
		}

		// Token: 0x06009270 RID: 37488 RVA: 0x00373700 File Offset: 0x00371900
		public override bool NeedsReset()
		{
			return 4 != GamePrefs.GetInt(EnumGamePrefs.OptionsGfxResetRevision);
		}

		// Token: 0x06009271 RID: 37489 RVA: 0x00373710 File Offset: 0x00371910
		public override void Reset(PrefVersionStore versionedPrefs)
		{
			Log.Out("Resetting game options GraphicsGroup");
			base.Reset(versionedPrefs);
			GameOptionsPlatforms.GfxPreset value = GameOptionsPlatforms.CalcDefaultGfxPreset();
			GamePrefs.Set(EnumGamePrefs.OptionsGfxQualityPreset, (int)value);
			GameOptionsManager.SetGraphicsQuality();
			GamePrefs.Set(EnumGamePrefs.OptionsGfxResetRevision, 4);
		}
	}

	// Token: 0x020011E0 RID: 4576
	public class ControlsGroup : GameOptionsReset.EnumGamePrefGroup
	{
		// Token: 0x17001178 RID: 4472
		// (get) Token: 0x06009272 RID: 37490 RVA: 0x0037374C File Offset: 0x0037194C
		public override string VersionId
		{
			get
			{
				return string.Format("Controls_{0}_{1}", 13, 7);
			}
		}

		// Token: 0x06009273 RID: 37491 RVA: 0x00373765 File Offset: 0x00371965
		public ControlsGroup() : base(new EnumGamePrefs[]
		{
			EnumGamePrefs.OptionsLookSensitivity,
			EnumGamePrefs.OptionsZoomSensitivity,
			EnumGamePrefs.OptionsZoomAccel,
			EnumGamePrefs.OptionsInvertMouse,
			EnumGamePrefs.OptionsVehicleLookSensitivity,
			EnumGamePrefs.OptionsControlsSprintLock
		})
		{
		}

		// Token: 0x06009274 RID: 37492 RVA: 0x0037377E File Offset: 0x0037197E
		public override bool NeedsReset()
		{
			return 7 != GamePrefs.GetInt(EnumGamePrefs.OptionsControlsResetRevision);
		}

		// Token: 0x06009275 RID: 37493 RVA: 0x00373790 File Offset: 0x00371990
		public override void Reset(PrefVersionStore versionedPrefs)
		{
			Log.Out("Resetting game options ControlsGroup");
			base.Reset(versionedPrefs);
			GamePrefs.Set(EnumGamePrefs.OptionsControlsResetRevision, 7);
		}
	}

	// Token: 0x020011E1 RID: 4577
	public class ControllerGroup : GameOptionsReset.EnumGamePrefGroup
	{
		// Token: 0x17001179 RID: 4473
		// (get) Token: 0x06009276 RID: 37494 RVA: 0x003737AE File Offset: 0x003719AE
		public override string VersionId
		{
			get
			{
				return string.Format("Controller_{0}_0", 13);
			}
		}

		// Token: 0x06009277 RID: 37495 RVA: 0x003737C1 File Offset: 0x003719C1
		public ControllerGroup() : base(new EnumGamePrefs[]
		{
			EnumGamePrefs.OptionsAllowController,
			EnumGamePrefs.OptionsControllerVibration,
			EnumGamePrefs.OptionsInterfaceSensitivity,
			EnumGamePrefs.OptionsControllerSensitivityX,
			EnumGamePrefs.OptionsControllerSensitivityY,
			EnumGamePrefs.OptionsControllerLookInvert,
			EnumGamePrefs.OptionsControllerJoystickLayout,
			EnumGamePrefs.OptionsControllerLookAcceleration,
			EnumGamePrefs.OptionsControllerZoomSensitivity,
			EnumGamePrefs.OptionsControllerLookAxisDeadzone,
			EnumGamePrefs.OptionsControllerMoveAxisDeadzone,
			EnumGamePrefs.OptionsControllerCursorSnap,
			EnumGamePrefs.OptionsControllerCursorHoverSensitivity,
			EnumGamePrefs.OptionsControllerVehicleSensitivity,
			EnumGamePrefs.OptionsControllerAimAssists,
			EnumGamePrefs.OptionsControllerWeaponAiming,
			EnumGamePrefs.OptionsControlsSprintLock,
			EnumGamePrefs.OptionsControllerTriggerEffects,
			EnumGamePrefs.OptionsControllerIconStyle
		})
		{
		}

		// Token: 0x06009278 RID: 37496 RVA: 0x003737DB File Offset: 0x003719DB
		public override void Reset(PrefVersionStore versionedPrefs)
		{
			Log.Out("Resetting game options ControllerGroup");
			base.Reset(versionedPrefs);
		}
	}

	// Token: 0x020011E2 RID: 4578
	public class BindingsGroup : GameOptionsReset.IGroup
	{
		// Token: 0x1700117A RID: 4474
		// (get) Token: 0x06009279 RID: 37497 RVA: 0x003737EE File Offset: 0x003719EE
		public string VersionId
		{
			get
			{
				return string.Format("Bindings_{0}_{1}", 13, 1);
			}
		}

		// Token: 0x0600927A RID: 37498 RVA: 0x00373808 File Offset: 0x00371A08
		public bool HasPref(string prefName)
		{
			if (prefName.Equals("Controls"))
			{
				return true;
			}
			if (prefName.Equals("ActionSetsSaved"))
			{
				return true;
			}
			if (!prefName.StartsWith("ActionSet_"))
			{
				return false;
			}
			foreach (ValueTuple<PlayerActionsBase, string> valueTuple in GameOptionsControls.ActionSetPrefs)
			{
				string item = valueTuple.Item2;
				if (prefName.Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600927B RID: 37499 RVA: 0x00373890 File Offset: 0x00371A90
		public bool NeedsReset()
		{
			return 1 != GamePrefs.GetInt(EnumGamePrefs.OptionsBindingsResetRevision);
		}

		// Token: 0x0600927C RID: 37500 RVA: 0x003738A4 File Offset: 0x00371AA4
		public void Reset(PrefVersionStore versionedPrefs)
		{
			Log.Out("Resetting game options BindingsGroup");
			foreach (ValueTuple<PlayerActionsBase, string> valueTuple in GameOptionsControls.ActionSetPrefs)
			{
				PlayerActionsBase item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				string text;
				if (versionedPrefs != null && versionedPrefs.TryGetString(item2, out text) && !string.IsNullOrEmpty(text))
				{
					item.Load(text);
				}
				else
				{
					item.Reset();
				}
			}
			GameOptionsControls.Save();
			GamePrefs.Set(EnumGamePrefs.OptionsBindingsResetRevision, 1);
		}
	}
}
