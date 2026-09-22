using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020002C0 RID: 704
[Preserve]
public class ConsoleCmdWeatherSurvival : ConsoleCmdAbstract
{
	// Token: 0x0600144F RID: 5199 RVA: 0x0007B651 File Offset: 0x00079851
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"weathersurvival"
		};
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x0007B661 File Offset: 0x00079861
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Enables/disables weather survival";
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x0007B668 File Offset: 0x00079868
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count > 0)
		{
			if (_params.Count > 1 || (_params[0] != "on" && _params[0] != "off"))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: weathersurvival [on/off]");
				return;
			}
			EntityStats.WeatherSurvivalEnabled = (_params[0] == "on");
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Weather survival is " + (EntityStats.WeatherSurvivalEnabled ? "on" : "off"));
	}
}
