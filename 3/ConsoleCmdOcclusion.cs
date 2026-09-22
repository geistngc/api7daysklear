using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200024E RID: 590
[Preserve]
public class ConsoleCmdOcclusion : ConsoleCmdAbstract
{
	// Token: 0x0600119D RID: 4509 RVA: 0x0006F222 File Offset: 0x0006D422
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"occlusion"
		};
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x0600119E RID: 4510 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x0600119F RID: 4511 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0006F232 File Offset: 0x0006D432
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Control OcclusionManager";
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x00032163 File Offset: 0x00030363
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0006F23C File Offset: 0x0006D43C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("occlusion off, partial, full, toggleVisible, togglePrints");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" off (set in main menu, not a map)");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   turns off gpu occlusion ");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" full (set in main menu, not a map)");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   hides occluded objects from both the camera and the sun");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" toggleVisible");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   toggles between forcing all meshes visible and normal gpu culling");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" view");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   toggles view of occlusion texture");
			return;
		}
		OcclusionManager instance = OcclusionManager.Instance;
		if (instance == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No occlusion manager!");
			return;
		}
		if (_params.Count == 1)
		{
			if (_params[0].EqualsCaseInsensitive("off"))
			{
				instance.EnableCulling(false);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Occlusion disabled");
				return;
			}
			if (_params[0].EqualsCaseInsensitive("full"))
			{
				instance.EnableCulling(true);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Occlusion enabled full");
				return;
			}
			if (_params[0].EqualsCaseInsensitive("toggleVisible"))
			{
				instance.forceAllVisible = !instance.forceAllVisible;
				if (instance.forceAllVisible)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("All meshes are forced to visible");
					return;
				}
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Normal GPU occlusion");
				return;
			}
			else if (_params[0].EqualsCaseInsensitive("view"))
			{
				instance.ToggleDebugView();
			}
		}
	}
}
