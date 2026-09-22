using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000204 RID: 516
[Preserve]
public class ConsoleCmdScreenEffect : ConsoleCmdAbstract
{
	// Token: 0x06000FC6 RID: 4038 RVA: 0x0006616A File Offset: 0x0006436A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"ScreenEffect"
		};
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x0006617A File Offset: 0x0006437A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sets a screen effect";
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x00066181 File Offset: 0x00064381
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "ScreenEffect [name] [intensity] [fade time]\nScreenEffect clear\nScreenEffect reload";
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x00066188 File Offset: 0x00064388
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
		if (_params[0] == "clear")
		{
			for (int i = 0; i < localPlayers.Count; i++)
			{
				localPlayers[i].ScreenEffectManager.DisableScreenEffects();
			}
			return;
		}
		if (_params[0] == "reload")
		{
			for (int j = 0; j < localPlayers.Count; j++)
			{
				localPlayers[j].ScreenEffectManager.ResetEffects();
			}
			return;
		}
		float intensity = 0f;
		float fadeTime = 4f;
		if (_params.Count >= 2)
		{
			StringParsers.TryParseFloat(_params[1], out intensity);
		}
		if (_params.Count >= 3)
		{
			StringParsers.TryParseFloat(_params[2], out fadeTime);
		}
		for (int k = 0; k < localPlayers.Count; k++)
		{
			localPlayers[k].ScreenEffectManager.SetScreenEffect(_params[0], intensity, fadeTime);
		}
	}
}
