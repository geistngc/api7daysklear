using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200028F RID: 655
[Preserve]
public class ConsoleCmdShowClouds : ConsoleCmdAbstract
{
	// Token: 0x06001329 RID: 4905 RVA: 0x0007617B File Offset: 0x0007437B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showClouds"
		};
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x0007618B File Offset: 0x0007438B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Artist command to show one layer of clouds.";
	}

	// Token: 0x0600132B RID: 4907 RVA: 0x00076192 File Offset: 0x00074392
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "type \"showClouds myCloudTexture\" where \"myCloudTexture\" is the name of the texture you want to see.\ntype \"showClouds\" to turn off this view.\nNote: cloud textures MUST be locasted at ./resources/textures/environment/spectrums/default\n";
	}

	// Token: 0x0600132C RID: 4908 RVA: 0x00076199 File Offset: 0x00074399
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 0)
		{
			Resources.Load("Textures/Environment/Spectrums/default/" + _params[0], typeof(Texture));
		}
	}
}
