using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020002A1 RID: 673
[Preserve]
public class ConsoleCmdSpectrum : ConsoleCmdAbstract
{
	// Token: 0x06001390 RID: 5008 RVA: 0x00077A92 File Offset: 0x00075C92
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spectrum"
		};
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x00077AA2 File Offset: 0x00075CA2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Force a particular lighting spectrum.";
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x00077AA9 File Offset: 0x00075CA9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "spectrum <Auto, Biome, BloodMoon, Foggy, Rainy, Stormy, Snowy>\n";
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x06001393 RID: 5011 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x00077AB0 File Offset: 0x00075CB0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (_params.Count == 0)
		{
			if (WeatherManager.forcedSpectrum != SpectrumWeatherType.None)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("forced " + WeatherManager.forcedSpectrum.ToString());
				return;
			}
			if (WeatherManager.Instance == null)
			{
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(WeatherManager.Instance.GetSpectrumInfo());
			return;
		}
		else
		{
			if (_params.Count != 1)
			{
				return;
			}
			if (_params[0].EqualsCaseInsensitive("Snowy"))
			{
				WeatherManager.SetForceSpectrum(SpectrumWeatherType.Snowy);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("Rainy"))
			{
				WeatherManager.SetForceSpectrum(SpectrumWeatherType.Rainy);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("Stormy"))
			{
				WeatherManager.SetForceSpectrum(SpectrumWeatherType.Stormy);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("Foggy"))
			{
				WeatherManager.SetForceSpectrum(SpectrumWeatherType.Foggy);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("BloodMoon"))
			{
				WeatherManager.SetForceSpectrum(SpectrumWeatherType.BloodMoon);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("Biome"))
			{
				WeatherManager.SetForceSpectrum(SpectrumWeatherType.Biome);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("Auto"))
			{
				WeatherManager.SetForceSpectrum(SpectrumWeatherType.None);
				return;
			}
			return;
		}
	}
}
