using System;
using System.Collections.Generic;
using System.Globalization;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000224 RID: 548
[Preserve]
public class ConsoleCmdGiveQuest : ConsoleCmdAbstract
{
	// Token: 0x0600109C RID: 4252 RVA: 0x000699B1 File Offset: 0x00067BB1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"givequest"
		};
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x0600109D RID: 4253 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x0600109E RID: 4254 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x000699C1 File Offset: 0x00067BC1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Gives a quest to the player or add to quest tier";
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x000699C8 File Offset: 0x00067BC8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "givequest commands:\ncomplete - complete all quests\ntieradd <points> - add the points to your quest tier (default is 10)\n<quest name> - gives you the quest";
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x000699D0 File Offset: 0x00067BD0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.IsDedicatedServer)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Cannot execute givequest on dedicated server, please execute as a client");
		}
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.getHelp());
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Available quests:");
			foreach (KeyValuePair<string, QuestClass> keyValuePair in QuestClass.s_Quests)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   " + keyValuePair.Key);
			}
			return;
		}
		QuestJournal questJournal = XUiM_Player.GetPlayer().QuestJournal;
		if (_params[0].EqualsCaseInsensitive("tieradd"))
		{
			int difficultyTier = 10;
			if (_params.Count >= 2)
			{
				difficultyTier = StringParsers.ParseSInt32(_params[1], 0, -1, NumberStyles.Integer);
			}
			questJournal.AddQuestFactionPoint(1, difficultyTier);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Quest points {0}, tier {1}", new object[]
			{
				questJournal.GetQuestFactionPoints(1),
				questJournal.GetCurrentFactionTier(1, 0, false)
			});
			return;
		}
		if (_params[0].EqualsCaseInsensitive("complete"))
		{
			if (questJournal.quests.Count > 0)
			{
				for (int i = 0; i < questJournal.quests.Count; i++)
				{
					Quest quest = questJournal.quests[i];
					if (quest.CurrentState == Quest.QuestState.NotStarted || quest.CurrentState == Quest.QuestState.InProgress)
					{
						for (int j = 0; j < quest.Objectives.Count; j++)
						{
							BaseObjective baseObjective = quest.Objectives[j];
							if (baseObjective.ObjectiveState != BaseObjective.ObjectiveStates.Complete && !(baseObjective is ObjectiveReturnToNPC))
							{
								baseObjective.ChangeStatus(true);
							}
						}
						questJournal.AddPOIToTraderData((int)quest.QuestClass.DifficultyTier, quest.PositionData[Quest.PositionDataTypes.TraderPosition], quest.PositionData[Quest.PositionDataTypes.POIPosition]);
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Quest completed {0}", new object[]
						{
							quest.GetPOIName()
						});
					}
				}
			}
			return;
		}
		if (!QuestClass.s_Quests.ContainsKey(_params[0]))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Quest '{0}' does not exist!", new object[]
			{
				_params[0]
			});
			return;
		}
		string text = _params[0];
		foreach (KeyValuePair<string, QuestClass> keyValuePair2 in QuestClass.s_Quests)
		{
			if (keyValuePair2.Key.EqualsCaseInsensitive(text))
			{
				text = keyValuePair2.Key;
				break;
			}
		}
		Quest quest2 = QuestClass.CreateQuest(text);
		if (quest2 == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Quest '{0}' does not exist!", new object[]
			{
				_params[0]
			});
			return;
		}
		questJournal.AddQuest(quest2, true);
	}
}
