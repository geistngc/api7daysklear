using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001DE RID: 478
[Preserve]
public class ConsoleCmdAccDecay : ConsoleCmdAbstract
{
	// Token: 0x06000EC9 RID: 3785 RVA: 0x0006041E File Offset: 0x0005E61E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"AccDecay",
			"SetAccDecay",
			"SetAccuracyDecay",
			"sad"
		};
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000ECA RID: 3786 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x00060446 File Offset: 0x0005E646
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Accuracy Decay for guns, show/hide/reset/<Decimal value>";
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0006044D File Offset: 0x0005E64D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "reset - apply the default settings for this command\n<Decimal Value> - Sets the decay constant for accuracy calculations using ItemActionRanged";
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x00060454 File Offset: 0x0005E654
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("ItemActionRanged.DecayConstant: {0}", ItemActionRanged.AccuracyUpdateDecayConstant));
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("ItemActionRanged.LogOldAccuracy: {0}", ItemActionRanged.LogOldAccuracy));
			return;
		}
		float num;
		if (float.TryParse(_params[0], out num))
		{
			ItemActionRanged.AccuracyUpdateDecayConstant = num;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Set ItemActionRanged.DecayConstant to {0} and reset old accuracy for checking", num));
			return;
		}
		if (_params[0].ToLower() == "reset")
		{
			ItemActionRanged.LogOldAccuracy = false;
			ItemActionRanged.AccuracyUpdateDecayConstant = 9.1f;
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("ItemActionRanged.DecayConstant: {0}", ItemActionRanged.AccuracyUpdateDecayConstant));
	}
}
