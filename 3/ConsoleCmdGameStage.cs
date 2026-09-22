using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200021A RID: 538
[Preserve]
public class ConsoleCmdGameStage : ConsoleCmdAbstract
{
	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06001053 RID: 4179 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06001054 RID: 4180 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06001055 RID: 4181 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x00067769 File Offset: 0x00065969
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"gamestage"
		};
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x00067779 File Offset: 0x00065979
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Shows the gamestage of the local player";
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x00067780 File Offset: 0x00065980
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			string text = string.Empty;
			float num = 0f;
			float num2 = 0f;
			BiomeDefinition biomeStandingOn = primaryPlayer.biomeStandingOn;
			if (biomeStandingOn != null)
			{
				text = biomeStandingOn.m_sBiomeName;
			}
			if (primaryPlayer.QuestJournal.ActiveQuest != null)
			{
				num = primaryPlayer.QuestJournal.ActiveQuest.QuestClass.GameStageMod;
				num2 = primaryPlayer.QuestJournal.ActiveQuest.QuestClass.GameStageBonus;
			}
			Log.Out("\nPlayer Game Stage = (((Player Level * (1 + Biome Modifier + Quest Modifier) + Days Alive) + Biome Bonus + Quest Bonus) * Difficulty Bonus) * Global Game Stage Modifier\"\nPlayer Game Stage {0} = ((({2} * (1 + {3} + {4}) + {5}) + {6} + {7}) * {8}) * {9}\nPlayer Game Stage {0}\nPlayer Level {2}\nBiome Modifier {3}\nQuest Modifier {4}\nDays Alive {5}\nBiome Bonus {6}\nQuest Bonus {7}\nDifficulty Bonus {8}\nGlobal Game Stage Modifier {9}\nPlayer Loot Stage {1}\nBiome {10}", new object[]
			{
				primaryPlayer.gameStage,
				primaryPlayer.GetLootStage(0f, 0f),
				primaryPlayer.Progression.Level,
				primaryPlayer.biomeStandingOn.GameStageMod * EntityPlayer.BiomeGameStageModifier,
				num,
				(long)((primaryPlayer.world.worldTime - primaryPlayer.gameStageBornAtWorldTime) / 24000UL),
				primaryPlayer.biomeStandingOn.GameStageBonus,
				num2,
				GameStageDefinition.DifficultyBonus,
				EntityPlayer.GlobalGameStageModifier,
				text
			});
		}
	}
}
