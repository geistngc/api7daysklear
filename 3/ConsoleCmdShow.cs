using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200028B RID: 651
[Preserve]
public class ConsoleCmdShow : ConsoleCmdAbstract
{
	// Token: 0x17000205 RID: 517
	// (get) Token: 0x0600130F RID: 4879 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x06001310 RID: 4880 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x00075B40 File Offset: 0x00073D40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"show"
		};
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00075B50 File Offset: 0x00073D50
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Shows custom layers of rendering.";
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00075B58 File Offset: 0x00073D58
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Enable(ConsoleCmdShow.DebugView dView)
	{
		ConsoleCmdShow.Disable();
		ConsoleCmdShow.enabledKeyword = dView.key;
		Shader.EnableKeyword(ConsoleCmdShow.enabledKeyword);
		ConsoleCmdShow.savedShadowsOption = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowQuality);
		ConsoleCmdShow.savedSSAOOption = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxSSAO);
		ConsoleCmdShow.savedDOFOption = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxDOF);
		if (dView.disableShadows)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 0);
		}
		if (dView.disableSSAO)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, false);
		}
		if (dView.disableDOF)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
		}
		GameManager.Instance.ApplyAllOptions();
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00075BF0 File Offset: 0x00073DF0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Disable()
	{
		if (ConsoleCmdShow.enabledKeyword.Length < 1)
		{
			return;
		}
		Shader.DisableKeyword(ConsoleCmdShow.enabledKeyword);
		ConsoleCmdShow.enabledKeyword = "";
		GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, ConsoleCmdShow.savedShadowsOption);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, ConsoleCmdShow.savedSSAOOption);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, ConsoleCmdShow.savedDOFOption);
		GameManager.Instance.ApplyAllOptions();
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00075C56 File Offset: 0x00073E56
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool IsEnabled(string key)
	{
		return ConsoleCmdShow.enabledKeyword.Length >= 1 && ConsoleCmdShow.enabledKeyword.EqualsCaseInsensitive(key);
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00075C74 File Offset: 0x00073E74
	public static void Init()
	{
		for (int i = 0; i < ConsoleCmdShow.Commands.Length; i++)
		{
			Shader.DisableKeyword(ConsoleCmdShow.Commands[i].key);
		}
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00075CA4 File Offset: 0x00073EA4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Switch(ConsoleCmdShow.DebugView dView)
	{
		if (ConsoleCmdShow.IsEnabled(dView.key))
		{
			ConsoleCmdShow.Disable();
			return;
		}
		ConsoleCmdShow.Enable(dView);
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetDescription());
			return;
		}
		if (_params.Count == 1)
		{
			for (int i = 0; i < ConsoleCmdShow.Commands.Length; i++)
			{
				if (_params[0].EqualsCaseInsensitive(ConsoleCmdShow.Commands[i].cmd))
				{
					ConsoleCmdShow.Switch(ConsoleCmdShow.Commands[i]);
					return;
				}
			}
			if (_params[0].EqualsCaseInsensitive("none") || _params[0].EqualsCaseInsensitive("off"))
			{
				ConsoleCmdShow.Disable();
			}
			return;
		}
		if (_params.Count > 1)
		{
			StringParsers.ParseFloat(_params[1], 0, -1, NumberStyles.Any);
		}
		_params[0].EqualsCaseInsensitive("NA");
	}

	// Token: 0x04000D5E RID: 3422
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool DISABLE_SHADOWS = true;

	// Token: 0x04000D5F RID: 3423
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ENABLE_SHADOWS = false;

	// Token: 0x04000D60 RID: 3424
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool DISABLE_SSAO = true;

	// Token: 0x04000D61 RID: 3425
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ENABLE_SSAO = false;

	// Token: 0x04000D62 RID: 3426
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool DISABLE_DOF = true;

	// Token: 0x04000D63 RID: 3427
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ENABLE_DOF = false;

	// Token: 0x04000D64 RID: 3428
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConsoleCmdShow.DebugView[] Commands = new ConsoleCmdShow.DebugView[]
	{
		new ConsoleCmdShow.DebugView("blockAO", "SHOW_BLOCK_AO", ConsoleCmdShow.DISABLE_SHADOWS, ConsoleCmdShow.DISABLE_SSAO, ConsoleCmdShow.DISABLE_DOF),
		new ConsoleCmdShow.DebugView("occlusion", "SHOW_OCCLUSION", ConsoleCmdShow.DISABLE_SHADOWS, ConsoleCmdShow.DISABLE_SSAO, ConsoleCmdShow.DISABLE_DOF),
		new ConsoleCmdShow.DebugView("lighting", "SHOW_LIGHTING", ConsoleCmdShow.ENABLE_SHADOWS, ConsoleCmdShow.ENABLE_SSAO, ConsoleCmdShow.DISABLE_DOF),
		new ConsoleCmdShow.DebugView("normals", "SHOW_NORMALS", ConsoleCmdShow.DISABLE_SHADOWS, ConsoleCmdShow.DISABLE_SSAO, ConsoleCmdShow.DISABLE_DOF)
	};

	// Token: 0x04000D65 RID: 3429
	[PublicizedFrom(EAccessModifier.Private)]
	public static string enabledKeyword = "";

	// Token: 0x04000D66 RID: 3430
	[PublicizedFrom(EAccessModifier.Private)]
	public static int savedShadowsOption = -1;

	// Token: 0x04000D67 RID: 3431
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool savedSSAOOption = false;

	// Token: 0x04000D68 RID: 3432
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool savedDOFOption = false;

	// Token: 0x0200028C RID: 652
	public class DebugView
	{
		// Token: 0x0600131B RID: 4891 RVA: 0x00075E60 File Offset: 0x00074060
		public DebugView(string _cmd, string _key, bool _disableShadows, bool _disableSSAO, bool _disableDOF)
		{
			this.cmd = _cmd;
			this.key = _key;
			this.disableShadows = _disableShadows;
			this.disableSSAO = _disableSSAO;
			this.disableDOF = _disableDOF;
		}

		// Token: 0x04000D69 RID: 3433
		public string cmd;

		// Token: 0x04000D6A RID: 3434
		public string key;

		// Token: 0x04000D6B RID: 3435
		public bool disableShadows;

		// Token: 0x04000D6C RID: 3436
		public bool disableSSAO;

		// Token: 0x04000D6D RID: 3437
		public bool disableDOF;
	}
}
