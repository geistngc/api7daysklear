using System;
using System.Collections.Generic;
using InControl;
using Platform;

// Token: 0x020011D5 RID: 4565
public static class GameOptionsControls
{
	// Token: 0x1700116F RID: 4463
	// (get) Token: 0x0600921A RID: 37402 RVA: 0x00371379 File Offset: 0x0036F579
	public static IEnumerable<ValueTuple<PlayerActionsBase, string>> ActionSetPrefs
	{
		get
		{
			foreach (PlayerActionsBase playerActionsBase in PlatformManager.NativePlatform.Input.ActionSets)
			{
				yield return new ValueTuple<PlayerActionsBase, string>(playerActionsBase, "ActionSet_" + playerActionsBase.Name);
			}
			IEnumerator<PlayerActionsBase> enumerator = null;
			yield break;
			yield break;
		}
	}

	// Token: 0x0600921B RID: 37403 RVA: 0x00371384 File Offset: 0x0036F584
	public static void Save()
	{
		foreach (ValueTuple<PlayerActionsBase, string> valueTuple in GameOptionsControls.ActionSetPrefs)
		{
			PlayerActionsBase item = valueTuple.Item1;
			SdPlayerPrefs.SetString(valueTuple.Item2, item.Save());
		}
		try
		{
			SdPlayerPrefs.SetString("Controls", GameOptionsControls.Export());
		}
		catch (Exception)
		{
		}
		SdPlayerPrefs.SetInt("ActionSetsSaved", 0);
		GameOptionsControls.ApplyAllowControllerOption();
	}

	// Token: 0x0600921C RID: 37404 RVA: 0x00371410 File Offset: 0x0036F610
	public static void Load()
	{
		bool flag = SdPlayerPrefs.HasKey("ActionSetsSaved");
		if (!flag && SdPlayerPrefs.HasKey("Controls"))
		{
			string @string = SdPlayerPrefs.GetString("Controls", string.Empty);
			PlatformManager.NativePlatform.Input.LoadActionSetsFromStrings(@string.Split(';', StringSplitOptions.None));
			GameOptionsControls.Save();
			GameOptionsControls.ApplyAllowControllerOption();
			GameOptionsControls.RestoreNonBindableControllerActionsToDefaults();
			Log.Out("Legacy controls data converted");
			return;
		}
		if (flag)
		{
			foreach (ValueTuple<PlayerActionsBase, string> valueTuple in GameOptionsControls.ActionSetPrefs)
			{
				PlayerActionsBase item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				if (!string.IsNullOrEmpty(SdPlayerPrefs.GetString(item2, string.Empty)))
				{
					item.Load(SdPlayerPrefs.GetString(item2));
				}
				else
				{
					Log.Warning("Loading controls: No data for action set " + item.Name);
				}
			}
		}
		GameOptionsControls.ApplyAllowControllerOption();
		GameOptionsControls.RestoreNonBindableControllerActionsToDefaults();
	}

	// Token: 0x0600921D RID: 37405 RVA: 0x00371504 File Offset: 0x0036F704
	public static string Export()
	{
		string[] array = new string[PlatformManager.NativePlatform.Input.ActionSets.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = PlatformManager.NativePlatform.Input.ActionSets[i].Save();
		}
		return string.Join(";", array);
	}

	// Token: 0x0600921E RID: 37406 RVA: 0x00371561 File Offset: 0x0036F761
	public static void Import(string importString)
	{
		PlatformManager.NativePlatform.Input.LoadActionSetsFromStrings(importString.Split(';', StringSplitOptions.None));
		GameOptionsControls.ApplyAllowControllerOption();
		GameOptionsControls.RestoreNonBindableControllerActionsToDefaults();
	}

	// Token: 0x0600921F RID: 37407 RVA: 0x00371588 File Offset: 0x0036F788
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplyAllowControllerOption()
	{
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.OptionsAllowController);
		for (int i = 0; i < PlatformManager.NativePlatform.Input.ActionSets.Count; i++)
		{
			PlatformManager.NativePlatform.Input.ActionSets[i].Device = (@bool ? null : InputDevice.Null);
		}
	}

	// Token: 0x06009220 RID: 37408 RVA: 0x003715E4 File Offset: 0x0036F7E4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void RestoreNonBindableControllerActionsToDefaults()
	{
		PlatformManager.NativePlatform.Input.GetActionSetForName("gui").ResetControllerBindings();
		PlatformManager.NativePlatform.Input.GetActionSetForName("permanent").ResetControllerBindings();
	}

	// Token: 0x04006BF8 RID: 27640
	public const string cActionSetSavePrefix = "ActionSet_";

	// Token: 0x04006BF9 RID: 27641
	public const string ActionSetSaveModeFlagPref = "ActionSetsSaved";

	// Token: 0x04006BFA RID: 27642
	public const string LegacyControlsPref = "Controls";
}
