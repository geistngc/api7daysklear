using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020002A8 RID: 680
[Preserve]
public class ConsoleCmdStarve : ConsoleCmdAbstract
{
	// Token: 0x17000220 RID: 544
	// (get) Token: 0x060013B3 RID: 5043 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x000785EB File Offset: 0x000767EB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"starve",
			"hungry",
			"food"
		};
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x0007860C File Offset: 0x0007680C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			float num = 0f;
			if (_params.Count > 0)
			{
				num = StringParsers.ParseFloat(_params[0], 0, -1, NumberStyles.Any);
			}
			primaryPlayer.Stats.Food.Value = (float)Mathf.CeilToInt(num / 100f * primaryPlayer.Stats.Food.ModifiedMax);
		}
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x00078683 File Offset: 0x00076883
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Makes the player starve (optionally specify the amount of food you want to have in percent).";
	}
}
