using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Audio;
using Challenges;
using Newtonsoft.Json.Linq;
using Platform;
using Twitch.PubSub;
using UniLinq;
using UnityEngine;

namespace Twitch
{
	// Token: 0x0200184C RID: 6220
	public class TwitchManager
	{
		// Token: 0x17001780 RID: 6016
		// (get) Token: 0x0600BFB1 RID: 49073 RVA: 0x0046DD5B File Offset: 0x0046BF5B
		public static TwitchManager Current
		{
			get
			{
				if (TwitchManager.instance == null)
				{
					TwitchManager.instance = new TwitchManager();
				}
				return TwitchManager.instance;
			}
		}

		// Token: 0x17001781 RID: 6017
		// (get) Token: 0x0600BFB2 RID: 49074 RVA: 0x0046DD73 File Offset: 0x0046BF73
		public static bool HasInstance
		{
			get
			{
				return TwitchManager.instance != null;
			}
		}

		// Token: 0x0600BFB3 RID: 49075 RVA: 0x0046DD80 File Offset: 0x0046BF80
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchManager()
		{
			this.ViewerData = new TwitchViewerData(this);
			this.VotingManager = new TwitchVotingManager(this);
			this.HighestGameStage = -1;
			this.UseProgression = true;
		}

		// Token: 0x0600BFB4 RID: 49076 RVA: 0x0046E059 File Offset: 0x0046C259
		public void Cleanup()
		{
			this.Disconnect();
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.InitState == TwitchManager.InitStates.Ready)
			{
				this.SaveViewerData();
			}
			TwitchManager.instance = null;
		}

		// Token: 0x17001782 RID: 6018
		// (get) Token: 0x0600BFB5 RID: 49077 RVA: 0x0046E082 File Offset: 0x0046C282
		// (set) Token: 0x0600BFB6 RID: 49078 RVA: 0x0046E08A File Offset: 0x0046C28A
		public byte CurrentFileVersion { get; set; }

		// Token: 0x17001783 RID: 6019
		// (get) Token: 0x0600BFB7 RID: 49079 RVA: 0x0046E093 File Offset: 0x0046C293
		// (set) Token: 0x0600BFB8 RID: 49080 RVA: 0x0046E09B File Offset: 0x0046C29B
		public byte CurrentMainFileVersion { get; set; }

		// Token: 0x17001784 RID: 6020
		// (get) Token: 0x0600BFB9 RID: 49081 RVA: 0x0046E0A4 File Offset: 0x0046C2A4
		// (set) Token: 0x0600BFBA RID: 49082 RVA: 0x0046E0AC File Offset: 0x0046C2AC
		public bool OverrideProgession
		{
			get
			{
				return this.overrideProgression;
			}
			set
			{
				if (this.overrideProgression != value)
				{
					this.overrideProgression = value;
					if (this.InitState == TwitchManager.InitStates.Ready)
					{
						this.resetCommandsNeeded = true;
					}
				}
			}
		}

		// Token: 0x17001785 RID: 6021
		// (get) Token: 0x0600BFBB RID: 49083 RVA: 0x0046E0CE File Offset: 0x0046C2CE
		// (set) Token: 0x0600BFBC RID: 49084 RVA: 0x0046E0D6 File Offset: 0x0046C2D6
		public bool UseProgression { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001786 RID: 6022
		// (get) Token: 0x0600BFBD RID: 49085 RVA: 0x0046E0DF File Offset: 0x0046C2DF
		// (set) Token: 0x0600BFBE RID: 49086 RVA: 0x0046E0E7 File Offset: 0x0046C2E7
		public int HighestGameStage { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001787 RID: 6023
		// (get) Token: 0x0600BFBF RID: 49087 RVA: 0x0046E0F0 File Offset: 0x0046C2F0
		public static bool BossHordeActive
		{
			get
			{
				return TwitchManager.HasInstance && TwitchManager.Current.IsBossHordeActive;
			}
		}

		// Token: 0x17001788 RID: 6024
		// (get) Token: 0x0600BFC0 RID: 49088 RVA: 0x0046E105 File Offset: 0x0046C305
		// (set) Token: 0x0600BFC1 RID: 49089 RVA: 0x0046E10D File Offset: 0x0046C30D
		public TwitchManager.IntegrationSettings IntegrationSetting
		{
			get
			{
				return this.integrationSetting;
			}
			set
			{
				if (this.integrationSetting != value)
				{
					this.integrationSetting = value;
					this.IntegrationTypeChanged();
				}
			}
		}

		// Token: 0x17001789 RID: 6025
		// (get) Token: 0x0600BFC2 RID: 49090 RVA: 0x0046E125 File Offset: 0x0046C325
		// (set) Token: 0x0600BFC3 RID: 49091 RVA: 0x0046E12D File Offset: 0x0046C32D
		public float ActionCooldownModifier
		{
			get
			{
				return this.actionCooldownModifier;
			}
			set
			{
				if (value != this.actionCooldownModifier)
				{
					this.actionCooldownModifier = value;
					this.UpdateActionCooldowns(value);
				}
			}
		}

		// Token: 0x1700178A RID: 6026
		// (get) Token: 0x0600BFC4 RID: 49092 RVA: 0x0046E146 File Offset: 0x0046C346
		public bool AllowActions
		{
			get
			{
				return !this.CurrentActionPreset.IsEmpty;
			}
		}

		// Token: 0x1700178B RID: 6027
		// (get) Token: 0x0600BFC5 RID: 49093 RVA: 0x0046E156 File Offset: 0x0046C356
		public bool AllowEvents
		{
			get
			{
				return !this.CurrentEventPreset.IsEmpty;
			}
		}

		// Token: 0x1700178C RID: 6028
		// (get) Token: 0x0600BFC6 RID: 49094 RVA: 0x0046E166 File Offset: 0x0046C366
		public bool OnCooldown
		{
			get
			{
				return this.CooldownTime > 0f || this.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Always;
			}
		}

		// Token: 0x1700178D RID: 6029
		// (get) Token: 0x0600BFC7 RID: 49095 RVA: 0x0046E185 File Offset: 0x0046C385
		// (set) Token: 0x0600BFC8 RID: 49096 RVA: 0x0046E18D File Offset: 0x0046C38D
		public float BitPriceMultiplier
		{
			get
			{
				return this.bitPriceMultiplier;
			}
			set
			{
				if (this.bitPriceMultiplier != value)
				{
					this.bitPriceMultiplier = value;
					this.ResetPrices();
				}
			}
		}

		// Token: 0x1700178E RID: 6030
		// (get) Token: 0x0600BFC9 RID: 49097 RVA: 0x0046E1A5 File Offset: 0x0046C3A5
		// (set) Token: 0x0600BFCA RID: 49098 RVA: 0x0046E1AD File Offset: 0x0046C3AD
		public TwitchVotingManager VotingManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700178F RID: 6031
		// (get) Token: 0x0600BFCB RID: 49099 RVA: 0x0046E1B6 File Offset: 0x0046C3B6
		// (set) Token: 0x0600BFCC RID: 49100 RVA: 0x0046E1BE File Offset: 0x0046C3BE
		public TwitchViewerData ViewerData { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001790 RID: 6032
		// (get) Token: 0x0600BFCD RID: 49101 RVA: 0x0046E1C7 File Offset: 0x0046C3C7
		// (set) Token: 0x0600BFCE RID: 49102 RVA: 0x0046E1CF File Offset: 0x0046C3CF
		public string BroadcasterType
		{
			get
			{
				return this.broadcasterType;
			}
			set
			{
				this.broadcasterType = value;
				this.UIDirty = true;
				if (this.CommandsChanged != null)
				{
					this.CommandsChanged();
				}
			}
		}

		// Token: 0x17001791 RID: 6033
		// (get) Token: 0x0600BFCF RID: 49103 RVA: 0x0046E1F2 File Offset: 0x0046C3F2
		// (set) Token: 0x0600BFD0 RID: 49104 RVA: 0x0046E1FC File Offset: 0x0046C3FC
		public TwitchManager.InitStates InitState
		{
			get
			{
				return this.initState;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				if (this.initState != value)
				{
					TwitchManager.InitStates oldState = this.initState;
					this.initState = value;
					if (this.ConnectionStateChanged != null)
					{
						this.ConnectionStateChanged(oldState, this.initState);
					}
				}
			}
		}

		// Token: 0x17001792 RID: 6034
		// (get) Token: 0x0600BFD1 RID: 49105 RVA: 0x0046E23C File Offset: 0x0046C43C
		public string StateText
		{
			get
			{
				switch (this.initState)
				{
				case TwitchManager.InitStates.Setup:
				case TwitchManager.InitStates.WaitingForOAuth:
				case TwitchManager.InitStates.Authenticating:
				case TwitchManager.InitStates.Authenticated:
					return Localization.Get("xuiTwitchStatus_Connecting", false, null);
				case TwitchManager.InitStates.WaitingForPermission:
					return Localization.Get("xuiTwitchStatus_RequestPermission", false, null);
				case TwitchManager.InitStates.PermissionDenied:
					return Localization.Get("xuiTwitchStatus_PermissionDenied", false, null);
				case TwitchManager.InitStates.Ready:
					return string.Format(Localization.Get("xuiTwitchStatus_Connected", false, null), this.Authentication.userName);
				case TwitchManager.InitStates.ExtensionNotInstalled:
					return Localization.Get("xuiTwitchStatus_ExtensionDenied", false, null);
				case TwitchManager.InitStates.Failed:
					return Localization.Get("xuiTwitchStatus_ConnectionFailed", false, null);
				}
				return "";
			}
		}

		// Token: 0x17001793 RID: 6035
		// (get) Token: 0x0600BFD2 RID: 49106 RVA: 0x0046E2E7 File Offset: 0x0046C4E7
		public bool IsReady
		{
			get
			{
				return this.initState == TwitchManager.InitStates.Ready;
			}
		}

		// Token: 0x17001794 RID: 6036
		// (get) Token: 0x0600BFD3 RID: 49107 RVA: 0x0046E2F2 File Offset: 0x0046C4F2
		public bool IsVoting
		{
			get
			{
				return this.initState == TwitchManager.InitStates.Ready && this.VotingManager.VotingIsActive;
			}
		}

		// Token: 0x17001795 RID: 6037
		// (get) Token: 0x0600BFD4 RID: 49108 RVA: 0x0046E30A File Offset: 0x0046C50A
		public bool IsBossHordeActive
		{
			get
			{
				return this.initState == TwitchManager.InitStates.Ready && (this.VotingManager.CurrentVoteState == TwitchVotingManager.VoteStateTypes.EventActive || this.VotingManager.CurrentVoteState == TwitchVotingManager.VoteStateTypes.WaitingForActive);
			}
		}

		// Token: 0x17001796 RID: 6038
		// (get) Token: 0x0600BFD5 RID: 49109 RVA: 0x0046E335 File Offset: 0x0046C535
		public bool ReadyForVote
		{
			get
			{
				return this.actionSpawnLiveList.Count == 0;
			}
		}

		// Token: 0x17001797 RID: 6039
		// (get) Token: 0x0600BFD6 RID: 49110 RVA: 0x0046E345 File Offset: 0x0046C545
		// (set) Token: 0x0600BFD7 RID: 49111 RVA: 0x0046E34D File Offset: 0x0046C54D
		public bool IsSafe { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1400010E RID: 270
		// (add) Token: 0x0600BFD8 RID: 49112 RVA: 0x0046E358 File Offset: 0x0046C558
		// (remove) Token: 0x0600BFD9 RID: 49113 RVA: 0x0046E390 File Offset: 0x0046C590
		public event OnTwitchConnectionStateChange ConnectionStateChanged;

		// Token: 0x1400010F RID: 271
		// (add) Token: 0x0600BFDA RID: 49114 RVA: 0x0046E3C8 File Offset: 0x0046C5C8
		// (remove) Token: 0x0600BFDB RID: 49115 RVA: 0x0046E400 File Offset: 0x0046C600
		public event OnCommandsChanged CommandsChanged;

		// Token: 0x14000110 RID: 272
		// (add) Token: 0x0600BFDC RID: 49116 RVA: 0x0046E438 File Offset: 0x0046C638
		// (remove) Token: 0x0600BFDD RID: 49117 RVA: 0x0046E470 File Offset: 0x0046C670
		public event OnHistoryAdded ActionHistoryAdded;

		// Token: 0x14000111 RID: 273
		// (add) Token: 0x0600BFDE RID: 49118 RVA: 0x0046E4A8 File Offset: 0x0046C6A8
		// (remove) Token: 0x0600BFDF RID: 49119 RVA: 0x0046E4E0 File Offset: 0x0046C6E0
		public event OnHistoryAdded VoteHistoryAdded;

		// Token: 0x14000112 RID: 274
		// (add) Token: 0x0600BFE0 RID: 49120 RVA: 0x0046E518 File Offset: 0x0046C718
		// (remove) Token: 0x0600BFE1 RID: 49121 RVA: 0x0046E550 File Offset: 0x0046C750
		public event OnHistoryAdded EventHistoryAdded;

		// Token: 0x17001798 RID: 6040
		// (get) Token: 0x0600BFE2 RID: 49122 RVA: 0x0046E585 File Offset: 0x0046C785
		public bool HasCustomEvents
		{
			get
			{
				return this.EventPresets.Count > 0;
			}
		}

		// Token: 0x0600BFE3 RID: 49123 RVA: 0x0046E598 File Offset: 0x0046C798
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupLocalization()
		{
			this.chatOutput_ActivatedAction = Localization.Get("TwitchChat_ActivatedAction", false, null);
			this.chatOutput_ActivatedBitAction = Localization.Get("TwitchChat_ActivatedBitAction", false, null);
			this.chatOutput_BitCredits = Localization.Get("TwitchChat_BitCredits", false, null);
			this.chatOutput_BitEvent = Localization.Get("TwitchChat_BitEvent", false, null);
			this.chatOutput_BitPotBalance = Localization.Get("TwitchChat_BitPotBalance", false, null);
			this.chatOutput_ChannelPointEvent = Localization.Get("TwitchChat_ChannelPointEvent", false, null);
			this.chatOutput_CharityEvent = Localization.Get("TwitchChat_CharityEvent", false, null);
			this.chatOutput_Commands = Localization.Get("TwitchChat_Commands", false, null);
			this.chatOutput_CooldownComplete = Localization.Get("TwitchChat_CooldownComplete", false, null);
			this.chatOutput_CooldownStarted = Localization.Get("TwitchChat_CooldownStarted", false, null);
			this.chatOutput_CooldownTime = Localization.Get("TwitchChat_CooldownTime", false, null);
			this.chatOutput_CreatorGoalEvent = Localization.Get("TwitchChat_CreatorGoalEvent", false, null);
			this.chatOutput_DonateBits = Localization.Get("TwitchChat_DonateBits", false, null);
			this.chatOutput_DonateCharity = Localization.Get("TwitchChat_DonateCharity", false, null);
			this.chatOutput_Gamestage = Localization.Get("TwitchChat_Gamestage", false, null);
			this.chatOutput_GiftSubEvent = Localization.Get("TwitchChat_GiftedSubEvent", false, null);
			this.chatOutput_GiftSubs = Localization.Get("TwitchChat_GiftedSubs", false, null);
			this.chatOutput_HypeTrainEvent = Localization.Get("TwitchChat_HypeTrainEvent", false, null);
			this.chatOutput_KilledParty = Localization.Get("TwitchChat_KilledParty", false, null);
			this.chatOutput_KilledStreamer = Localization.Get("TwitchChat_KilledStreamer", false, null);
			this.chatOutput_KilledByBits = Localization.Get("TwitchChat_KilledByBits", false, null);
			this.chatOutput_KilledByHypeTrain = Localization.Get("TwitchChat_KilledByHypeTrain", false, null);
			this.chatOutput_KilledByVote = Localization.Get("TwitchChat_KilledByVote", false, null);
			this.chatOutput_NewActions = Localization.Get("TwitchChat_NewActions", false, null);
			this.chatOutput_PimpPotBalance = Localization.Get("TwitchChat_PimpPotBalance", false, null);
			this.chatOutput_PointsWithSpecial = Localization.Get("TwitchChat_PointsWithSpecial", false, null);
			this.chatOutput_PointsWithoutSpecial = Localization.Get("TwitchChat_PointsWithoutSpecial", false, null);
			this.chatOutput_QueuedBitAction = Localization.Get("TwitchChat_QueuedBitAction", false, null);
			this.chatOutput_RaidEvent = Localization.Get("TwitchChat_RaidEvent", false, null);
			this.chatOutput_RaidPoints = Localization.Get("TwitchChat_RaidPoints", false, null);
			this.chatOutput_SubEvent = Localization.Get("TwitchChat_SubEvent", false, null);
			this.chatOutput_Subscribed = Localization.Get("TwitchChat_Subscribed", false, null);
			this.ingameOutput_ActivatedAction = Localization.Get("TwitchInGame_ActivatedAction", false, null);
			this.ingameOutput_BitRespawns = Localization.Get("TwitchInGame_BitRespawns", false, null);
			this.ingameOutput_DonateBits = Localization.Get("TwitchInGame_DonateBits", false, null);
			this.ingameOutput_DonateCharity = Localization.Get("TwitchInGame_DonateCharity", false, null);
			this.ingameOutput_GiftSubs = Localization.Get("TwitchInGame_GiftedSubs", false, null);
			this.ingameOutput_KilledParty = Localization.Get("TwitchInGame_KilledParty", false, null);
			this.ingameOutput_KilledStreamer = Localization.Get("TwitchInGame_KilledStreamer", false, null);
			this.ingameOutput_KilledByBits = Localization.Get("TwitchInGame_KilledByBits", false, null);
			this.ingameOutput_KilledByHypeTrain = Localization.Get("TwitchInGame_KilledByHypeTrain", false, null);
			this.ingameOutput_KilledByVote = Localization.Get("TwitchInGame_KilledByVote", false, null);
			this.ingameOutput_RaidPoints = Localization.Get("TwitchInGame_RaidPoints", false, null);
			this.ingameOutput_RefundedAction = Localization.Get("TwitchInGame_RefundedAction", false, null);
			this.ingameOutput_Subscribed = Localization.Get("TwitchInGame_Subscribed", false, null);
			this.ingameDeathScreen_Message = Localization.Get("TwitchDeathMessage", false, null);
			this.ingameBitsDeathScreen_Message = Localization.Get("TwitchBitsDeathMessage", false, null);
			this.ingameHypeTrainDeathScreen_Message = Localization.Get("TwitchHypeTrainDeathMessage", false, null);
			this.ingameVoteDeathScreen_Message = Localization.Get("TwitchVoteDeathMessage", false, null);
			this.subPointDisplay = Localization.Get("xuiOptionsTwitchSubPointDisplay", false, null);
			this.ViewerData.SetupLocalization();
			this.VotingManager.SetupLocalization();
			TwitchManager.LeaderboardStats.SetupLocalization();
		}

		// Token: 0x0600BFE4 RID: 49124 RVA: 0x0046E94C File Offset: 0x0046CB4C
		public void SetupClient(string twitchChannel, string password)
		{
			this.ircClient = new TwitchIRCClient("irc.twitch.tv", 6667, twitchChannel, password);
			if (this.EventSub == null)
			{
				this.EventSub = new EventSubClient(this.Authentication.userID, this.Authentication.oauth.Substring(6), TwitchAuthentication.client_id);
			}
		}

		// Token: 0x0600BFE5 RID: 49125 RVA: 0x0046E9A4 File Offset: 0x0046CBA4
		public void IntegrationTypeChanged()
		{
			if (this.IsReady && this.extensionManager == null)
			{
				this.extensionManager = new ExtensionManager();
				this.extensionManager.Init();
			}
		}

		// Token: 0x0600BFE6 RID: 49126 RVA: 0x0046E9CC File Offset: 0x0046CBCC
		public void CleanupData()
		{
			BaseTwitchCommand.ClearCommandPermissionOverrides();
			this.VotingManager.CleanupData();
			this.CooldownPresets.Clear();
			this.tipTitleList.Clear();
			this.tipDescriptionList.Clear();
			this.ActionPresets.Clear();
			this.VotePresets.Clear();
			this.CurrentActionPreset = null;
			this.CurrentVotePreset = null;
			this.CleanupEventData();
		}

		// Token: 0x0600BFE7 RID: 49127 RVA: 0x0046EA34 File Offset: 0x0046CC34
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveChannelPointRedeems()
		{
			if (this.CurrentEventPreset != null)
			{
				this.CurrentEventPreset.RemoveChannelPointRedemptions(null);
			}
		}

		// Token: 0x0600BFE8 RID: 49128 RVA: 0x0046EA4A File Offset: 0x0046CC4A
		public void CleanupEventData()
		{
			this.RemoveChannelPointRedeems();
			this.CurrentEventPreset = null;
			this.EventPresets.Clear();
		}

		// Token: 0x0600BFE9 RID: 49129 RVA: 0x0046EA64 File Offset: 0x0046CC64
		[PublicizedFrom(EAccessModifier.Private)]
		public void Disconnect()
		{
			this.RemoveChannelPointRedeems();
			if (this.EventSub != null)
			{
				this.EventSub.Disconnect();
				this.EventSub.Cleanup();
				this.EventSub = null;
			}
			if (this.ircClient != null)
			{
				this.ircClient.Disconnect();
				this.ircClient = null;
			}
			if (this.extensionManager != null)
			{
				this.extensionManager.Cleanup();
				this.extensionManager = null;
			}
			this.Authentication = null;
			if (this.LocalPlayer != null && this.LocalPlayer.PlayerUI != null && this.LocalPlayer.PlayerUI.windowManager.IsWindowOpen("twitch"))
			{
				this.LocalPlayer.PlayerUI.windowManager.Close("twitch");
			}
		}

		// Token: 0x0600BFEA RID: 49130 RVA: 0x0046EB2E File Offset: 0x0046CD2E
		public void AddRandomGroup(string name, int randomCount)
		{
			if (!this.randomGroups.ContainsKey(name))
			{
				this.randomGroups.Add(name, new TwitchRandomActionGroup
				{
					Name = name,
					RandomCount = randomCount
				});
			}
		}

		// Token: 0x0600BFEB RID: 49131 RVA: 0x0046EB60 File Offset: 0x0046CD60
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetDailyCommands(int currentDay, int lastGameStage = -1)
		{
			this.randomKeys.Clear();
			foreach (string key in this.AvailableCommands.Keys)
			{
				TwitchAction twitchAction = this.AvailableCommands[key];
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					if (twitchAction.RandomDaily)
					{
						if (!this.randomKeys.ContainsKey(twitchAction.RandomGroup))
						{
							this.randomKeys.Add(twitchAction.RandomGroup, new List<TwitchAction>());
						}
						this.randomKeys[twitchAction.RandomGroup].Add(twitchAction);
					}
					else if (twitchAction.SingleDayUse)
					{
						twitchAction.AllowedDay = currentDay;
					}
				}
			}
			foreach (string key2 in this.randomKeys.Keys)
			{
				int num = 1;
				if (this.randomGroups.ContainsKey(key2))
				{
					num = this.randomGroups[key2].RandomCount;
				}
				List<TwitchAction> list = this.randomKeys[key2];
				if (lastGameStage != -1)
				{
					bool flag = false;
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].StartGameStage > lastGameStage)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					int index2 = UnityEngine.Random.Range(0, list.Count);
					TwitchAction value = list[index];
					list[index] = list[index2];
					list[index2] = value;
				}
				for (int k = 0; k < list.Count; k++)
				{
					list[k].AllowedDay = ((k < num) ? currentDay : -1);
				}
			}
			if (this.CommandsChanged != null)
			{
				this.CommandsChanged();
			}
		}

		// Token: 0x0600BFEC RID: 49132 RVA: 0x0046EDA0 File Offset: 0x0046CFA0
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupTwitchCommands()
		{
			this.TwitchCommandList.Clear();
			this.TwitchCommandList.Add(new TwitchCommandAddBitCredit());
			this.TwitchCommandList.Add(new TwitchCommandAddPoints());
			this.TwitchCommandList.Add(new TwitchCommandAddSpecialPoints());
			this.TwitchCommandList.Add(new TwitchCommandCheckCredit());
			this.TwitchCommandList.Add(new TwitchCommandCheckPoints());
			this.TwitchCommandList.Add(new TwitchCommandCommands());
			this.TwitchCommandList.Add(new TwitchCommandDebug());
			this.TwitchCommandList.Add(new TwitchCommandDisableCommand());
			this.TwitchCommandList.Add(new TwitchCommandGamestage());
			this.TwitchCommandList.Add(new TwitchCommandPauseCommand());
			this.TwitchCommandList.Add(new TwitchCommandUnpauseCommand());
			this.TwitchCommandList.Add(new TwitchCommandRemoveViewer());
			this.TwitchCommandList.Add(new TwitchCommandResetCooldowns());
			this.TwitchCommandList.Add(new TwitchCommandSetBitPot());
			this.TwitchCommandList.Add(new TwitchCommandSetCooldown());
			this.TwitchCommandList.Add(new TwitchCommandSetPot());
			this.TwitchCommandList.Add(new TwitchCommandTeleportBackpack());
			if (this.CurrentEventPreset.HasBitEvents && this.AllowBitEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemBits());
			}
			if (this.CurrentEventPreset.HasSubEvents && this.AllowSubEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemSub());
			}
			if (this.CurrentEventPreset.HasGiftSubEvents && this.AllowGiftSubEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemGiftSub());
			}
			if (this.CurrentEventPreset.HasRaidEvents && this.AllowRaidEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemRaid());
			}
			if (this.CurrentEventPreset.HasCharityEvents && this.AllowCharityEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemCharity());
			}
			if (this.CurrentEventPreset.HasHypeTrainEvents && this.AllowHypeTrainEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemHypeTrain());
			}
			if (this.CurrentEventPreset.HasCreatorGoalEvents && this.AllowCreatorGoalEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemCreatorGoal());
			}
			this.TwitchCommandList.Add(new TwitchCommandUseProgression());
		}

		// Token: 0x0600BFED RID: 49133 RVA: 0x0046EFDC File Offset: 0x0046D1DC
		public void StartTwitchIntegration()
		{
			this.updateTime = 60f;
			try
			{
				if (this.Authentication == null)
				{
					this.Authentication = new TwitchAuthentication();
				}
				this.Authentication.StopListener();
				this.Authentication.GetToken();
			}
			catch (Exception ex)
			{
				Log.Out("Twitch integration failed to start with message " + ex.Message);
				this.updateTime = 5f;
			}
			this.InitialCooldownSet = false;
			this.InitState = TwitchManager.InitStates.WaitingForOAuth;
		}

		// Token: 0x0600BFEE RID: 49134 RVA: 0x0046F060 File Offset: 0x0046D260
		public void StopTwitchIntegration(TwitchManager.InitStates initState = TwitchManager.InitStates.None)
		{
			this.resetClientAttempts = 0;
			this.Disconnect();
			this.TwitchDisconnectPartyUpdate();
			this.ClearEventHandlers();
			if (this.LocalPlayer != null)
			{
				this.LocalPlayer.TwitchEnabled = false;
				this.LocalPlayer.TwitchActionsEnabled = EntityPlayer.TwitchActionsStates.Enabled;
			}
			if (this.Authentication != null)
			{
				this.Authentication.StopListener();
			}
			this.InitState = initState;
		}

		// Token: 0x0600BFEF RID: 49135 RVA: 0x0046F0C6 File Offset: 0x0046D2C6
		public void WaitForOAuth()
		{
			this.updateTime = 10f;
			this.InitState = TwitchManager.InitStates.WaitingForOAuth;
		}

		// Token: 0x0600BFF0 RID: 49136 RVA: 0x0046F0DA File Offset: 0x0046D2DA
		public void WaitForPermission()
		{
			this.updateTime = 10f;
			this.InitState = TwitchManager.InitStates.WaitingForPermission;
		}

		// Token: 0x0600BFF1 RID: 49137 RVA: 0x0046F0EE File Offset: 0x0046D2EE
		public void DeniedPermission()
		{
			this.InitState = TwitchManager.InitStates.PermissionDenied;
		}

		// Token: 0x0600BFF2 RID: 49138 RVA: 0x0046F0F8 File Offset: 0x0046D2F8
		[PublicizedFrom(EAccessModifier.Private)]
		public void ClearEventHandlers()
		{
			GameEventManager gameEventManager = GameEventManager.Current;
			gameEventManager.GameEntitySpawned -= this.Current_GameEntitySpawned;
			gameEventManager.GameEntityDespawned -= this.Current_GameEntityDespawned;
			gameEventManager.GameEntityKilled -= this.Current_GameEntityKilled;
			gameEventManager.GameBlocksAdded -= this.Current_GameBlocksAdded;
			gameEventManager.GameBlocksRemoved -= this.Current_GameBlocksRemoved;
			gameEventManager.GameBlockRemoved -= this.Current_GameBlockRemoved;
			gameEventManager.GameEventApproved -= this.Current_GameEventApproved;
			gameEventManager.TwitchPartyGameEventApproved -= this.Current_TwitchPartyGameEventApproved;
			gameEventManager.TwitchRefundNeeded -= this.Current_TwitchRefundNeeded;
			gameEventManager.GameEventDenied -= this.Current_GameEventDenied;
			gameEventManager.GameEventCompleted -= this.Current_GameEventCompleted;
			if (this.LocalPlayer != null)
			{
				this.LocalPlayer.PartyLeave -= this.LocalPlayer_PartyLeave;
				this.LocalPlayer.PartyJoined -= this.LocalPlayer_PartyJoined;
				this.LocalPlayer.PartyChanged -= this.LocalPlayer_PartyChanged;
				if (this.LocalPlayer.Party != null)
				{
					this.LocalPlayer.Party.PartyMemberAdded -= this.Party_PartyMemberAdded;
					this.LocalPlayer.Party.PartyMemberRemoved -= this.Party_PartyMemberRemoved;
				}
			}
		}

		// Token: 0x0600BFF3 RID: 49139 RVA: 0x0046F26C File Offset: 0x0046D46C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_TwitchPartyGameEventApproved(string gameEventID, int targetEntityID, string extraData, string tag)
		{
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				if (twitchAction.IsInPreset(this.CurrentActionPreset) && twitchAction.EventName == gameEventID)
				{
					this.AddCooldownForAction(twitchAction);
					break;
				}
			}
		}

		// Token: 0x0600BFF4 RID: 49140 RVA: 0x0046F2E4 File Offset: 0x0046D4E4
		public void Update(float deltaTime)
		{
			GameManager gameManager = GameManager.Instance;
			if (gameManager.World == null || gameManager.World.Players == null || gameManager.World.Players.Count == 0)
			{
				return;
			}
			this.CurrentUnityTime = Time.time;
			switch (this.InitState)
			{
			case TwitchManager.InitStates.Setup:
			{
				GameEventManager gameEventManager = GameEventManager.Current;
				gameEventManager.GameEventAccessApproved -= this.Current_GameEventAccessApproved;
				gameEventManager.GameEventAccessApproved += this.Current_GameEventAccessApproved;
				this.SetupLocalization();
				if (!this.isLoaded)
				{
					this.isLoaded = true;
					this.LoadViewerData();
				}
				this.LoadMainViewerData();
				this.InitState = TwitchManager.InitStates.None;
				break;
			}
			case TwitchManager.InitStates.WaitingForPermission:
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					this.StopTwitchIntegration(TwitchManager.InitStates.None);
					Log.Warning("Twitch: login failed in " + this.InitState.ToString() + " state");
					this.InitState = TwitchManager.InitStates.Failed;
					return;
				}
				break;
			case TwitchManager.InitStates.WaitingForOAuth:
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					this.StopTwitchIntegration(TwitchManager.InitStates.None);
					Log.Warning("Twitch: Login failed in " + this.InitState.ToString() + " state");
					this.InitState = TwitchManager.InitStates.Failed;
					return;
				}
				if (this.Authentication.oauth != "" && this.Authentication.userName != "" && this.Authentication.userID != "")
				{
					this.SetupClient(this.Authentication.userName, this.Authentication.oauth);
					this.EventSub.OnEventReceived -= this.EventSubMessageReceived;
					this.EventSub.OnEventReceived += this.EventSubMessageReceived;
					this.EventSub.Connect();
					this.updateTime = 3f;
					Log.Out("retrieved oauth. Waiting for IRC to post auth...");
					this.InitState = TwitchManager.InitStates.Authenticating;
				}
				break;
			case TwitchManager.InitStates.Authenticating:
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					if (this.Authentication.oauth != "" && this.Authentication.userName != "" && this.Authentication.userID != "")
					{
						int num = this.resetClientAttempts;
						this.resetClientAttempts = num + 1;
						if (num < 5)
						{
							this.SetupClient(this.Authentication.userName, this.Authentication.oauth);
							this.updateTime = 2.5f;
							Log.Out("attempting to reset client...");
							break;
						}
					}
					Log.Warning("Twitch: Login failed in " + this.InitState.ToString() + " state");
					this.StopTwitchIntegration(TwitchManager.InitStates.Failed);
					this.InitState = TwitchManager.InitStates.Failed;
					return;
				}
				break;
			case TwitchManager.InitStates.Authenticated:
			{
				this.ClearEventHandlers();
				GameEventManager gameEventManager2 = GameEventManager.Current;
				gameEventManager2.GameEntitySpawned += this.Current_GameEntitySpawned;
				gameEventManager2.GameEntityDespawned += this.Current_GameEntityDespawned;
				gameEventManager2.GameEntityKilled += this.Current_GameEntityKilled;
				gameEventManager2.GameBlocksAdded += this.Current_GameBlocksAdded;
				gameEventManager2.GameBlockRemoved += this.Current_GameBlockRemoved;
				gameEventManager2.GameBlocksRemoved += this.Current_GameBlocksRemoved;
				gameEventManager2.GameEventApproved += this.Current_GameEventApproved;
				gameEventManager2.TwitchPartyGameEventApproved += this.Current_TwitchPartyGameEventApproved;
				gameEventManager2.TwitchRefundNeeded += this.Current_TwitchRefundNeeded;
				gameEventManager2.GameEventDenied += this.Current_GameEventDenied;
				gameEventManager2.GameEventCompleted += this.Current_GameEventCompleted;
				this.world = gameManager.World;
				if (this.extensionManager == null)
				{
					this.extensionManager = new ExtensionManager();
					this.extensionManager.Init();
				}
				this.InitState = TwitchManager.InitStates.Ready;
				QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.Enabled, "");
				break;
			}
			case TwitchManager.InitStates.CheckingForExtension:
				if (!this.AllowActions)
				{
					this.InitState = TwitchManager.InitStates.Authenticated;
				}
				else if (!this.checkingExtensionInstalled)
				{
					this.checkingExtensionInstalled = true;
					ExtensionManager.CheckExtensionInstalled(delegate(bool IsInstalled)
					{
						this.checkingExtensionInstalled = false;
						if (!IsInstalled)
						{
							XUiC_MessageBoxWindowGroup.ShowOk(this.LocalPlayerXUi, Localization.Get("xuiTwitchPopup_ExtensionNeededHeader", false, null), Localization.Get("xuiTwitchPopup_ExtensionNeeded", false, null), "", null, true, true, false);
							Application.OpenURL("https://dashboard.twitch.tv/extensions/k6ji189bf7i4ge8il4iczzw7kpgmjt");
						}
						this.InitState = TwitchManager.InitStates.Authenticated;
					});
				}
				break;
			case TwitchManager.InitStates.Ready:
				if (this.LocalPlayer == null)
				{
					this.SetupTwitchCommands();
					this.LocalPlayer = (XUiM_Player.GetPlayer() as EntityPlayerLocal);
					this.RefreshPartyInfo();
					this.HighestGameStage = this.LocalPlayer.unModifiedGameStage;
					this.ActionMessages.Clear();
					this.GetCooldownMax();
					this.SetCooldown((float)this.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.Startup, false, false);
					this.LocalPlayer.PartyLeave += this.LocalPlayer_PartyLeave;
					this.LocalPlayer.PartyJoined += this.LocalPlayer_PartyJoined;
					this.LocalPlayer.PartyChanged += this.LocalPlayer_PartyChanged;
					if (this.LocalPlayer.Party != null)
					{
						this.LocalPlayer.Party.PartyMemberAdded += this.Party_PartyMemberAdded;
						this.LocalPlayer.Party.PartyMemberRemoved += this.Party_PartyMemberRemoved;
					}
					if (!this.InitialCooldownSet)
					{
						if (this.CurrentCooldownPreset.StartCooldownTime > 0)
						{
							this.SetCooldown(100000f, TwitchManager.CooldownTypes.Startup, false, true);
						}
						if (this.CurrentCooldownPreset.CooldownType != CooldownPreset.CooldownTypes.Fill)
						{
							this.SetCooldown(0f, TwitchManager.CooldownTypes.None, false, false);
						}
						this.CurrentActionPreset.HandleCooldowns();
						this.InitialCooldownSet = true;
					}
				}
				this.LocalPlayer.TwitchEnabled = true;
				this.LocalPlayerInLandClaim = GameManager.Instance.World.GetLandClaimOwnerInParty(this.LocalPlayer, this.LocalPlayer.persistentPlayerData);
				if (!this.ircClient.IsConnected)
				{
					Log.Out("Reached 'Ready' but waiting for IRC to post auth message...");
					this.ircClient.Reconnect();
					this.InitState = TwitchManager.InitStates.Authenticating;
					this.updateTime = 30f;
				}
				TwitchManager.LeaderboardStats.UpdateStats(deltaTime);
				if (this.resetCommandsNeeded)
				{
					this.ResetCommands();
				}
				if (!XUi.InGameMenuOpen && this.AllowActions && this.ExtensionCheckTime < 0f)
				{
					this.ExtensionCheckTime = 30f;
					ExtensionManager.CheckExtensionInstalled(delegate(bool IsInstalled)
					{
						if (IsInstalled)
						{
							this.extensionActiveCheckFailures = 0;
							return;
						}
						if (this.extensionActiveCheckFailures < 3)
						{
							this.extensionActiveCheckFailures++;
							return;
						}
						this.extensionActiveCheckFailures = 0;
						LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.LocalPlayer);
						this.ircClient.SendChannelMessage(Localization.Get("TwitchChat_ExtensionNotInstalled", false, null), false);
						XUiC_ChatOutput.AddMessage(uiforPlayer.xui, EnumGameMessages.PlainTextLocal, Localization.Get("TwitchChat_ExtensionNotInstalled", false, null), EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.None, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
						this.StopTwitchIntegration(TwitchManager.InitStates.None);
						this.InitState = TwitchManager.InitStates.ExtensionNotInstalled;
						Application.OpenURL("https://dashboard.twitch.tv/extensions/k6ji189bf7i4ge8il4iczzw7kpgmjt");
					});
				}
				this.ExtensionCheckTime -= deltaTime;
				if (this.LocalPlayer.Buffs.HasBuff("twitch_extensionneeded"))
				{
					this.updateTime -= deltaTime;
					if (this.updateTime <= 0f)
					{
						ExtensionManager.CheckExtensionInstalled(delegate(bool IsInstalled)
						{
							if (IsInstalled)
							{
								this.LocalPlayer.Buffs.RemoveBuff("twitch_extensionneeded", -1, true);
							}
						});
						this.updateTime = 5f;
					}
				}
				break;
			}
			if (this.extensionManager != null)
			{
				if (this.extensionManager.HasCommand())
				{
					ExtensionAction command = this.extensionManager.GetCommand();
					int userId = int.Parse(command.username);
					string command2 = command.command;
					bool isRerun = false;
					int creditUsed = command.creditUsed;
					ExtensionBitAction extensionBitAction = command as ExtensionBitAction;
					this.HandleExtensionMessage(userId, command2, isRerun, creditUsed, (extensionBitAction != null) ? extensionBitAction.cost : 0);
				}
				this.extensionManager.Update();
			}
			if (this.ircClient != null)
			{
				this.ircClient.Update(deltaTime);
				if (this.ircClient.AvailableMessage())
				{
					this.HandleMessage(this.ircClient.ReadMessage());
				}
				this.ViewerData.Update(deltaTime);
				if (this.LocalPlayer == null)
				{
					return;
				}
				bool flag = false;
				for (int i = this.LiveActionEntries.Count - 1; i >= 0; i--)
				{
					if (this.LiveActionEntries[i].ReadyForRemove)
					{
						this.LiveActionEntries.RemoveAt(i);
					}
					else if (this.LiveActionEntries[i].Action.CooldownBlocked)
					{
						flag = true;
					}
				}
				for (int j = this.actionSpawnLiveList.Count - 1; j >= 0; j--)
				{
					if (this.actionSpawnLiveList[j].SpawnedEntity == null)
					{
						this.actionSpawnLiveList.RemoveAt(j);
					}
				}
				for (int k = this.LiveEvents.Count - 1; k >= 0; k--)
				{
					if (this.LiveEvents[k].ReadyForRemove)
					{
						this.LiveEvents.RemoveAt(k);
					}
				}
				if (this.LocalPlayer.IsAlive() && this.CooldownTime > 0f && this.TwitchActive)
				{
					if (this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting && this.actionSpawnLiveList.Count == 0 && !flag)
					{
						this.SetCooldown(this.CooldownTime, TwitchManager.CooldownTypes.MaxReached, false, true);
					}
					if (this.CooldownType == TwitchManager.CooldownTypes.MaxReached || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.Startup || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldownExit)
					{
						float cooldownTime = this.CooldownTime;
						this.CooldownTime -= Time.deltaTime;
						if (cooldownTime >= 15f && this.CooldownTime < 15f && this.CooldownTime > 0f && this.CooldownType != TwitchManager.CooldownTypes.SafeCooldownExit)
						{
							this.LocalPlayer.TwitchActionsEnabled = EntityPlayer.TwitchActionsStates.TempDisabledEnding;
						}
					}
					if (this.CooldownTime <= 0f)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.SafeCooldownExit)
						{
							this.HandleEndCooldownStateChanging();
						}
						else
						{
							this.HandleEndCooldown();
						}
						this.VotingManager.VoteStartDelayTimeRemaining = 10f;
					}
				}
				for (int l = this.liveList.Count - 1; l >= 0; l--)
				{
					if (this.liveList[l].SpawnedEntity == null)
					{
						this.liveList[l].SpawnedEntity = this.world.GetEntity(this.liveList[l].SpawnedEntityID);
						this.liveList[l].SpawnedEntity == null;
					}
				}
				for (int m = this.recentlyDeadList.Count - 1; m >= 0; m--)
				{
					this.recentlyDeadList[m].TimeRemaining -= deltaTime;
					if (this.recentlyDeadList[m].TimeRemaining <= 0f)
					{
						this.recentlyDeadList.RemoveAt(m);
					}
				}
				for (int n = this.liveBlockList.Count - 1; n >= 0; n--)
				{
					TwitchSpawnedBlocksEntry twitchSpawnedBlocksEntry = this.liveBlockList[n];
					if (twitchSpawnedBlocksEntry.TimeRemaining > 0f)
					{
						twitchSpawnedBlocksEntry.TimeRemaining -= deltaTime;
						if (twitchSpawnedBlocksEntry.TimeRemaining <= 0f)
						{
							this.liveBlockList.RemoveAt(n);
						}
					}
				}
				int num2 = GameUtils.WorldTimeToDays(this.world.worldTime);
				if (num2 != this.lastGameDay)
				{
					this.SetupAvailableCommands();
					this.ResetDailyCommands(num2, -1);
					this.HandleCooldownActionLocking();
					this.lastGameDay = num2;
				}
				if (this.CooldownType != TwitchManager.CooldownTypes.Startup && this.TwitchActive && !gameManager.IsPaused() && this.InitState == TwitchManager.InitStates.Ready)
				{
					this.VotingManager.Update(deltaTime);
				}
				this.HandleEventQueue();
				if (this.LocalPlayer.IsAlive() && this.CooldownType != TwitchManager.CooldownTypes.Time)
				{
					int num3 = 0;
					while (num3 < this.QueuedActionEntries.Count)
					{
						if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
						{
							if (!this.QueuedActionEntries[num3].IsSent)
							{
								SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageGameEventRequest>().Setup(this.QueuedActionEntries[num3].Action.EventName, this.QueuedActionEntries[num3].Target.entityId, true, Vector3.zero, null, this.QueuedActionEntries[num3].UserName, "action", this.AllowCrateSharing, true, null), false);
								this.QueuedActionEntries[num3].IsSent = true;
								break;
							}
							num3++;
						}
						else
						{
							if (GameEventManager.Current.HandleAction(this.QueuedActionEntries[num3].Action.EventName, this.LocalPlayer, this.QueuedActionEntries[num3].Target, true, this.QueuedActionEntries[num3].UserName, "action", this.AllowCrateSharing, true, "", null))
							{
								if (this.LocalPlayer.Party != null)
								{
									for (int num4 = 0; num4 < this.LocalPlayer.Party.MemberList.Count; num4++)
									{
										EntityPlayer entityPlayer = this.LocalPlayer.Party.MemberList[num4];
										if (entityPlayer != this.LocalPlayer && entityPlayer.TwitchEnabled)
										{
											SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.QueuedActionEntries[num3].Action.EventName, this.QueuedActionEntries[num3].Target.entityId, this.QueuedActionEntries[num3].UserName, "action", NetPackageGameEventResponse.ResponseTypes.TwitchPartyActionApproved, -1, -1, false, ""), false, entityPlayer.entityId, -1, -1, null, 192, false);
										}
									}
								}
								GameEventManager.Current.HandleGameEventApproved(this.QueuedActionEntries[num3].Action.EventName, this.QueuedActionEntries[num3].Target.entityId, this.QueuedActionEntries[num3].UserName, "action");
								break;
							}
							TwitchActionEntry twitchActionEntry = this.QueuedActionEntries[num3];
							ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
							this.AddActionHistory(twitchActionEntry, viewerEntry, TwitchActionHistoryEntry.EntryStates.Reimbursed);
							this.ShowReimburseMessage(twitchActionEntry, viewerEntry);
							this.ViewerData.ReimburseAction(twitchActionEntry);
							this.QueuedActionEntries.RemoveAt(num3);
							break;
						}
					}
				}
				this.saveTime -= Time.deltaTime;
				if (this.saveTime <= 0f && (this.dataSaveThreadInfo == null || this.dataSaveThreadInfo.HasTerminated()))
				{
					this.saveTime = 30f;
					if (this.HasDataChanges)
					{
						this.SaveViewerData();
						this.HasDataChanges = false;
					}
				}
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					this.updateTime = 2f;
					if (this.UseProgression && !this.OverrideProgession && this.commandsAvailable == -1)
					{
						this.commandsAvailable = this.GetCommandCount();
					}
					this.RefreshCommands(true);
					int @int = GameStats.GetInt(EnumGameStats.BloodMoonDay);
					if (this.nextBMDay != @int)
					{
						this.nextBMDay = @int;
						if (num2 != this.currentBMDayEnd)
						{
							this.currentBMDayEnd = this.nextBMDay + 1;
						}
						this.SetupBloodMoonData();
					}
					this.RefreshVoteLockedLevel();
					this.IsSafe = this.LocalPlayer.TwitchSafe;
					this.isBMActive = false;
					if (this.CooldownType != TwitchManager.CooldownTypes.Time && !this.IsVoting)
					{
						if (this.UseActionsDuringBloodmoon != 1)
						{
							if (this.WithinBloodMoonPeriod())
							{
								this.isBMActive = true;
								if (this.CooldownType != TwitchManager.CooldownTypes.BloodMoonDisabled && this.CooldownType != TwitchManager.CooldownTypes.BloodMoonCooldown)
								{
									this.SetCooldown(5f, (this.UseActionsDuringBloodmoon == 0) ? TwitchManager.CooldownTypes.BloodMoonDisabled : TwitchManager.CooldownTypes.BloodMoonCooldown, false, true);
								}
							}
							else if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown)
							{
								this.SetCooldown(5f, TwitchManager.CooldownTypes.Time, false, true);
								this.currentBMDayEnd = this.nextBMDay + 1;
								this.VotingManager.VoteStartDelayTimeRemaining += 35f;
							}
						}
						else if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown)
						{
							this.SetCooldown(5f, TwitchManager.CooldownTypes.Time, false, true);
						}
						if (this.AllowActions && this.CooldownType != TwitchManager.CooldownTypes.BloodMoonDisabled && this.CooldownType != TwitchManager.CooldownTypes.BloodMoonCooldown)
						{
							if (this.UseActionsDuringQuests != 1 && this.CooldownType != TwitchManager.CooldownTypes.Time)
							{
								if (QuestEventManager.Current.QuestBounds.width != 0f)
								{
									if (this.UseActionsDuringQuests == 0 && this.CooldownType != TwitchManager.CooldownTypes.QuestDisabled)
									{
										this.SetCooldown(5f, TwitchManager.CooldownTypes.QuestDisabled, false, false);
									}
									else if (this.UseActionsDuringQuests == 2 && this.CooldownType != TwitchManager.CooldownTypes.QuestCooldown)
									{
										this.SetCooldown(5f, TwitchManager.CooldownTypes.QuestCooldown, false, false);
									}
								}
								else if (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
								{
									this.SetCooldown(60f, TwitchManager.CooldownTypes.Time, false, true);
									this.CurrentCooldownFill = 0f;
								}
							}
							else if (this.UseActionsDuringQuests == 1 && (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled))
							{
								this.SetCooldown(60f, TwitchManager.CooldownTypes.Time, false, true);
							}
							if (this.CooldownType != TwitchManager.CooldownTypes.Time && this.CooldownType != TwitchManager.CooldownTypes.Startup && this.CooldownType != TwitchManager.CooldownTypes.MaxReached && this.CooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && this.CooldownType != TwitchManager.CooldownTypes.QuestCooldown && this.CooldownType != TwitchManager.CooldownTypes.QuestDisabled)
							{
								if (this.CooldownType != TwitchManager.CooldownTypes.SafeCooldown && this.LocalPlayer.TwitchSafe)
								{
									this.SetCooldown(5f, TwitchManager.CooldownTypes.SafeCooldown, false, false);
								}
								else if (this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown && !this.LocalPlayer.TwitchSafe)
								{
									this.SetCooldown(5f, TwitchManager.CooldownTypes.SafeCooldownExit, false, true);
								}
							}
						}
						else if (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							this.SetCooldown(60f, TwitchManager.CooldownTypes.Time, false, true);
						}
					}
					for (int num5 = this.RespawnEntries.Count - 1; num5 >= 0; num5--)
					{
						TwitchRespawnEntry twitchRespawnEntry = this.RespawnEntries[num5];
						if (twitchRespawnEntry.CanRespawn(this))
						{
							EntityPlayer target = twitchRespawnEntry.Target;
							if (!this.PartyInfo.ContainsKey(target) || this.PartyInfo[target].Cooldown <= 0f)
							{
								if (target.Buffs.HasBuff("twitch_pausedspawns"))
								{
									target.Buffs.RemoveBuff("twitch_pausedspawns", -1, true);
								}
								target.PlayOneShot("twitch_unpause", false, false, false, null, 1f);
								this.QueuedActionEntries.Add(twitchRespawnEntry.RespawnAction());
							}
						}
					}
				}
				if (this.lastAlive && this.LocalPlayer.IsDead())
				{
					this.CurrentCooldownFill = 0f;
					if (this.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Fill)
					{
						this.SetCooldown((float)this.CurrentCooldownPreset.AfterDeathCooldownTime, TwitchManager.CooldownTypes.Time, false, true);
					}
					this.KillAllSpawnsForPlayer(this.LocalPlayer);
				}
				if (this.lastAlive && this.LocalPlayer.IsAlive())
				{
					TwitchManager.DeathText = "";
				}
				if (!this.lastAlive && this.LocalPlayer.IsAlive())
				{
					this.respawnEventNeeded = true;
				}
				if (this.respawnEventNeeded && this.CheckCanRespawnEvent(this.LocalPlayer))
				{
					if (this.OnPlayerRespawnEvent != "")
					{
						GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, this.LocalPlayer, false, "", "", false, true, "", null);
					}
					this.respawnEventNeeded = false;
				}
				this.lastAlive = this.LocalPlayer.IsAlive();
				this.twitchPlayerDeathsThisFrame.Clear();
				this.UpdatePartyInfo(deltaTime);
			}
			this.HandleInGameChatQueue();
		}

		// Token: 0x0600BFF5 RID: 49141 RVA: 0x004705F6 File Offset: 0x0046E7F6
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleEndCooldown()
		{
			this.CurrentCooldownFill = 0f;
			Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_cooldown_ended");
			this.ircClient.SendChannelMessage(this.chatOutput_CooldownComplete, true);
			this.HandleEndCooldownStateChanging();
		}

		// Token: 0x0600BFF6 RID: 49142 RVA: 0x00470630 File Offset: 0x0046E830
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleEndCooldownStateChanging()
		{
			this.CooldownType = TwitchManager.CooldownTypes.None;
			if (this.LocalPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.TempDisabled || this.LocalPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.TempDisabledEnding)
			{
				this.LocalPlayer.TwitchActionsEnabled = EntityPlayer.TwitchActionsStates.Enabled;
			}
			if (this.ConnectionStateChanged != null)
			{
				this.ConnectionStateChanged(this.initState, this.initState);
			}
			this.HandleCooldownActionLocking();
		}

		// Token: 0x0600BFF7 RID: 49143 RVA: 0x00470691 File Offset: 0x0046E891
		public void AddToInGameChatQueue(string msg, string sound = null)
		{
			this.inGameChatQueue.Add(new TwitchMessageEntry(msg, sound));
		}

		// Token: 0x0600BFF8 RID: 49144 RVA: 0x004706A8 File Offset: 0x0046E8A8
		public void HandleInGameChatQueue()
		{
			if (this.inGameChatQueue.Count > 0 && this.LocalPlayer != null && this.LocalPlayer.IsAlive())
			{
				TwitchMessageEntry twitchMessageEntry = this.inGameChatQueue[0];
				XUiC_ChatOutput.AddMessage(this.LocalPlayerXUi, EnumGameMessages.PlainTextLocal, twitchMessageEntry.Message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.Server, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
				if (twitchMessageEntry.Sound != null)
				{
					this.LocalPlayer.PlayOneShot(twitchMessageEntry.Sound, false, false, false, null, 1f);
				}
				this.inGameChatQueue.RemoveAt(0);
			}
		}

		// Token: 0x0600BFF9 RID: 49145 RVA: 0x00470733 File Offset: 0x0046E933
		public void RefreshVoteLockedLevel()
		{
			this.VoteLockedLevel = this.LocalPlayer.HasTwitchVoteLockMember();
		}

		// Token: 0x0600BFFA RID: 49146 RVA: 0x00470748 File Offset: 0x0046E948
		public void SetupBloodMoonData()
		{
			ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			this.BMCooldownStart = item - this.CurrentCooldownPreset.BMStartOffset;
			this.BMCooldownEnd = item2 + this.CurrentCooldownPreset.BMEndOffset;
		}

		// Token: 0x0600BFFB RID: 49147 RVA: 0x00470794 File Offset: 0x0046E994
		public bool WithinBloodMoonPeriod()
		{
			ulong worldTime = this.world.worldTime;
			int num = GameUtils.WorldTimeToDays(worldTime);
			int num2 = GameUtils.WorldTimeToHours(worldTime);
			if (num == this.nextBMDay)
			{
				if (num2 >= this.BMCooldownStart)
				{
					return true;
				}
			}
			else if (num > 1 && num == this.currentBMDayEnd && num2 < this.BMCooldownEnd)
			{
				return true;
			}
			return false;
		}

		// Token: 0x0600BFFC RID: 49148 RVA: 0x004707E6 File Offset: 0x0046E9E6
		[PublicizedFrom(EAccessModifier.Private)]
		public void Party_PartyMemberAdded(EntityPlayer player)
		{
			this.GetCooldownMax();
			if (!this.PartyInfo.ContainsKey(player))
			{
				this.PartyInfo.Add(player, new TwitchManager.TwitchPartyMemberInfo());
				ExtensionManager extensionManager = this.extensionManager;
				if (extensionManager == null)
				{
					return;
				}
				extensionManager.OnPartyChanged();
			}
		}

		// Token: 0x0600BFFD RID: 49149 RVA: 0x0047081D File Offset: 0x0046EA1D
		[PublicizedFrom(EAccessModifier.Private)]
		public void Party_PartyMemberRemoved(EntityPlayer player)
		{
			this.GetCooldownMax();
			if (this.PartyInfo.ContainsKey(player))
			{
				this.PartyInfo.Remove(player);
				ExtensionManager extensionManager = this.extensionManager;
				if (extensionManager == null)
				{
					return;
				}
				extensionManager.OnPartyChanged();
			}
		}

		// Token: 0x0600BFFE RID: 49150 RVA: 0x00470850 File Offset: 0x0046EA50
		[PublicizedFrom(EAccessModifier.Protected)]
		public void RefreshPartyInfo()
		{
			if (this.LocalPlayer == null)
			{
				return;
			}
			if (this.LocalPlayer.Party == null)
			{
				this.PartyInfo.Clear();
				return;
			}
			this.PartyInfo.Clear();
			for (int i = 0; i < this.LocalPlayer.Party.MemberList.Count; i++)
			{
				EntityPlayer entityPlayer = this.LocalPlayer.Party.MemberList[i];
				if (!(entityPlayer == this.LocalPlayer) && !this.PartyInfo.ContainsKey(entityPlayer))
				{
					this.PartyInfo.Add(entityPlayer, new TwitchManager.TwitchPartyMemberInfo());
				}
			}
		}

		// Token: 0x0600BFFF RID: 49151 RVA: 0x004708F4 File Offset: 0x0046EAF4
		[PublicizedFrom(EAccessModifier.Protected)]
		public void UpdatePartyInfo(float deltaTime)
		{
			bool flag = false;
			foreach (EntityPlayer entityPlayer in this.PartyInfo.Keys)
			{
				TwitchManager.TwitchPartyMemberInfo twitchPartyMemberInfo = this.PartyInfo[entityPlayer];
				if (!twitchPartyMemberInfo.LastAlive && entityPlayer.IsAlive() && this.PartyRespawnEvent != "")
				{
					GameEventManager.Current.HandleAction(this.PartyRespawnEvent, this.LocalPlayer, entityPlayer, false, "", "", false, true, "", null);
				}
				if (twitchPartyMemberInfo.LastOptedOut != (entityPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.Disabled))
				{
					twitchPartyMemberInfo.LastOptedOut = (entityPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.Disabled);
					flag = true;
				}
				if (twitchPartyMemberInfo.Cooldown > 0f)
				{
					twitchPartyMemberInfo.Cooldown -= deltaTime;
				}
				if (twitchPartyMemberInfo.LastAlive && !entityPlayer.IsAlive())
				{
					this.KillAllSpawnsForPlayer(entityPlayer);
					twitchPartyMemberInfo.Cooldown = 60f;
				}
				if (!twitchPartyMemberInfo.LastAlive && entityPlayer.IsAlive())
				{
					twitchPartyMemberInfo.NeedsRespawnEvent = true;
				}
				if (twitchPartyMemberInfo.NeedsRespawnEvent && this.CheckCanRespawnEvent(entityPlayer) && twitchPartyMemberInfo.Cooldown <= 0f)
				{
					if (this.OnPlayerRespawnEvent != "")
					{
						GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, entityPlayer, false, "", "", false, true, "", null);
					}
					twitchPartyMemberInfo.NeedsRespawnEvent = false;
				}
				twitchPartyMemberInfo.LastAlive = entityPlayer.IsAlive();
			}
			if (flag)
			{
				this.GetCooldownMax();
			}
		}

		// Token: 0x0600C000 RID: 49152 RVA: 0x00470AA0 File Offset: 0x0046ECA0
		[PublicizedFrom(EAccessModifier.Protected)]
		public void TwitchDisconnectPartyUpdate()
		{
			this.RespawnEntries.Clear();
			if (this.OnPlayerRespawnEvent != "")
			{
				GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, this.LocalPlayer, false, "", "", false, true, "", null);
			}
			if (this.LocalPlayer != null && this.LocalPlayer.Buffs.HasBuff("twitch_pausedspawns"))
			{
				this.LocalPlayer.Buffs.RemoveBuff("twitch_pausedspawns", -1, true);
			}
			foreach (EntityPlayer entityPlayer in this.PartyInfo.Keys)
			{
				TwitchManager.TwitchPartyMemberInfo twitchPartyMemberInfo = this.PartyInfo[entityPlayer];
				if (twitchPartyMemberInfo.NeedsRespawnEvent)
				{
					if (this.OnPlayerRespawnEvent != "")
					{
						GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, entityPlayer, false, "", "", false, true, "", null);
					}
					twitchPartyMemberInfo.NeedsRespawnEvent = false;
				}
				if (entityPlayer.Buffs.HasBuff("twitch_pausedspawns"))
				{
					entityPlayer.Buffs.RemoveBuff("twitch_pausedspawns", -1, true);
				}
			}
		}

		// Token: 0x0600C001 RID: 49153 RVA: 0x00470C00 File Offset: 0x0046EE00
		public bool CheckCanRespawnEvent(EntityPlayer player)
		{
			return player != null && player.IsAlive() && player.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.Enabled && !player.TwitchSafe;
		}

		// Token: 0x0600C002 RID: 49154 RVA: 0x00470C28 File Offset: 0x0046EE28
		[PublicizedFrom(EAccessModifier.Protected)]
		public void KillAllSpawnsForPlayer(EntityPlayer player)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = this.RespawnEntries.Count - 1; i >= 0; i--)
			{
				TwitchRespawnEntry twitchRespawnEntry = this.RespawnEntries[i];
				if (twitchRespawnEntry != null && twitchRespawnEntry.Target == player)
				{
					twitchRespawnEntry.NeedsRespawn = (twitchRespawnEntry.SpawnedEntities.Count > twitchRespawnEntry.Action.RespawnThreshold);
					if (twitchRespawnEntry.NeedsRespawn)
					{
						if (flag)
						{
							stringBuilder.Append(", ");
						}
						flag = true;
						stringBuilder.Append(twitchRespawnEntry.Action.Command);
					}
					else
					{
						Debug.LogWarning(string.Format("Respawn Entry removed '{0}' because count {1} was less than {2}", twitchRespawnEntry.Action.Command, twitchRespawnEntry.SpawnedEntities.Count, twitchRespawnEntry.Action.RespawnThreshold));
						this.RespawnEntries.RemoveAt(i);
					}
				}
			}
			if (flag)
			{
				if (!player.Buffs.HasBuff("twitch_pausedspawns"))
				{
					player.Buffs.AddBuff("twitch_pausedspawns", -1, true, false, -1f);
				}
				string text = stringBuilder.ToString();
				string msg = string.Format(this.ingameOutput_BitRespawns, text);
				this.AddToInGameChatQueue(msg, "twitch_pause");
				Debug.LogWarning(string.Format("Respawns Found for {0}: {1}", player.EntityName, text));
			}
			else
			{
				Debug.LogWarning("No Respawns Found!");
				if (player.Buffs.HasBuff("twitch_pausedspawns"))
				{
					player.Buffs.RemoveBuff("twitch_pausedspawns", -1, true);
				}
			}
			if (this.OnPlayerDeathEvent != "")
			{
				GameEventManager.Current.HandleAction(this.OnPlayerDeathEvent, this.LocalPlayer, player, false, "", "", false, true, "", null);
			}
			for (int j = this.LiveActionEntries.Count - 1; j >= 0; j--)
			{
				if (this.LiveActionEntries[j] != null && this.LiveActionEntries[j].Target == player)
				{
					TwitchActionEntry twitchActionEntry = this.LiveActionEntries[j];
					twitchActionEntry.ReadyForRemove = true;
					if (twitchActionEntry.HistoryEntry != null)
					{
						twitchActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Despawned;
					}
				}
			}
		}

		// Token: 0x0600C003 RID: 49155 RVA: 0x00470E58 File Offset: 0x0046F058
		[PublicizedFrom(EAccessModifier.Private)]
		public void LocalPlayer_PartyChanged(Party _affectedParty, EntityPlayer _player)
		{
			if (this.extensionManager != null)
			{
				this.extensionManager.OnPartyChanged();
			}
			this.GetCooldownMax();
		}

		// Token: 0x0600C004 RID: 49156 RVA: 0x00470E58 File Offset: 0x0046F058
		[PublicizedFrom(EAccessModifier.Private)]
		public void LocalPlayer_PartyLeave(Party _affectedParty, EntityPlayer _player)
		{
			if (this.extensionManager != null)
			{
				this.extensionManager.OnPartyChanged();
			}
			this.GetCooldownMax();
		}

		// Token: 0x0600C005 RID: 49157 RVA: 0x00470E74 File Offset: 0x0046F074
		[PublicizedFrom(EAccessModifier.Private)]
		public void LocalPlayer_PartyJoined(Party _affectedParty, EntityPlayer _player)
		{
			if (this.LocalPlayer.Party != null)
			{
				this.LocalPlayer.Party.PartyMemberAdded += this.Party_PartyMemberAdded;
				this.LocalPlayer.Party.PartyMemberRemoved += this.Party_PartyMemberRemoved;
			}
			if (this.extensionManager != null)
			{
				this.extensionManager.OnPartyChanged();
			}
			this.GetCooldownMax();
			this.RefreshPartyInfo();
		}

		// Token: 0x0600C006 RID: 49158 RVA: 0x00470EE8 File Offset: 0x0046F0E8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntityKilled(int entityID)
		{
			for (int i = this.liveList.Count - 1; i >= 0; i--)
			{
				TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = this.liveList[i];
				if (twitchSpawnedEntityEntry.SpawnedEntityID == entityID)
				{
					if (twitchSpawnedEntityEntry.RespawnEntry != null)
					{
						TwitchRespawnEntry respawnEntry = twitchSpawnedEntityEntry.RespawnEntry;
						if (respawnEntry.RemoveSpawnedEntry(entityID, true) && respawnEntry.ReadyForRemove)
						{
							this.RespawnEntries.Remove(respawnEntry);
						}
					}
					this.actionSpawnLiveList.Remove(twitchSpawnedEntityEntry);
					this.recentlyDeadList.Add(new TwitchRecentlyRemovedEntityEntry(twitchSpawnedEntityEntry));
					this.liveList.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0600C007 RID: 49159 RVA: 0x00470F7C File Offset: 0x0046F17C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntityDespawned(int entityID)
		{
			for (int i = this.liveList.Count - 1; i >= 0; i--)
			{
				if (this.liveList[i].SpawnedEntityID == entityID)
				{
					TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = this.liveList[i];
					if (twitchSpawnedEntityEntry.Action != null && twitchSpawnedEntityEntry.Action.UserName != null)
					{
						this.ViewerData.AddPoints(twitchSpawnedEntityEntry.Action.UserName, (int)((float)twitchSpawnedEntityEntry.Action.ActionCost * 0.25f), false, false);
						if (twitchSpawnedEntityEntry.Action.HistoryEntry != null)
						{
							twitchSpawnedEntityEntry.Action.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Despawned;
						}
						if (twitchSpawnedEntityEntry.RespawnEntry != null)
						{
							TwitchRespawnEntry respawnEntry = twitchSpawnedEntityEntry.RespawnEntry;
							if (respawnEntry.RemoveSpawnedEntry(entityID, false) && respawnEntry.RespawnsLeft == 0)
							{
								this.RespawnEntries.Remove(respawnEntry);
							}
						}
					}
					else if (twitchSpawnedEntityEntry.Event != null && twitchSpawnedEntityEntry.Event.HistoryEntry != null)
					{
						twitchSpawnedEntityEntry.Event.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Despawned;
					}
					this.actionSpawnLiveList.Remove(twitchSpawnedEntityEntry);
					this.recentlyDeadList.Add(new TwitchRecentlyRemovedEntityEntry(twitchSpawnedEntityEntry));
					this.liveList.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0600C008 RID: 49160 RVA: 0x004710AD File Offset: 0x0046F2AD
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventAccessApproved()
		{
			if (this.InitState == TwitchManager.InitStates.None || this.InitState == TwitchManager.InitStates.WaitingForPermission)
			{
				this.StartTwitchIntegration();
			}
		}

		// Token: 0x0600C009 RID: 49161 RVA: 0x004710C8 File Offset: 0x0046F2C8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventApproved(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.QueuedActionEntries.Count; i++)
				{
					if (this.QueuedActionEntries[i].Action.EventName == gameEventID && this.QueuedActionEntries[i].Target.entityId == targetEntityID && this.QueuedActionEntries[i].UserName == viewerName)
					{
						this.ConfirmAction(this.QueuedActionEntries[i]);
						this.LiveActionEntries.Add(this.QueuedActionEntries[i]);
						this.QueuedActionEntries.RemoveAt(i);
						return;
					}
				}
				return;
			}
			if (tag == "event")
			{
				for (int j = 0; j < this.EventQueue.Count; j++)
				{
					if (this.EventQueue[j].UserName == viewerName && this.EventQueue[j].Event.EventName == gameEventID)
					{
						this.LiveEvents.Add(this.EventQueue[j]);
						this.EventQueue.RemoveAt(j);
						return;
					}
				}
			}
		}

		// Token: 0x0600C00A RID: 49162 RVA: 0x00471208 File Offset: 0x0046F408
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventDenied(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.QueuedActionEntries.Count; i++)
				{
					if (this.QueuedActionEntries[i].Action.EventName == gameEventID && this.QueuedActionEntries[i].Target.entityId == targetEntityID && this.QueuedActionEntries[i].UserName == viewerName)
					{
						this.ViewerData.ReimburseAction(this.QueuedActionEntries[i]);
						this.QueuedActionEntries.RemoveAt(i);
						return;
					}
				}
			}
		}

		// Token: 0x0600C00B RID: 49163 RVA: 0x004712B4 File Offset: 0x0046F4B4
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_TwitchRefundNeeded(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.LiveActionEntries.Count; i++)
				{
					TwitchActionEntry twitchActionEntry = this.LiveActionEntries[i];
					if (twitchActionEntry.Action.EventName == gameEventID && twitchActionEntry.Target.entityId == targetEntityID && twitchActionEntry.UserName == viewerName)
					{
						this.ViewerData.ReimburseAction(this.LiveActionEntries[i]);
						this.ShowReimburseMessage(twitchActionEntry, null);
						Debug.LogWarning(string.Format("TwitchAction {0} refunded for {1}.", twitchActionEntry.Action.Name, viewerName));
						if (twitchActionEntry.HistoryEntry != null)
						{
							twitchActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Reimbursed;
						}
						if (twitchActionEntry.Action.tempCooldown > 30f)
						{
							twitchActionEntry.Action.SetCooldown(this.CurrentUnityTime, 30f);
						}
						this.LiveActionEntries.RemoveAt(i);
						return;
					}
				}
				return;
			}
			if (tag == "event")
			{
				int j = 0;
				while (j < this.LiveEvents.Count)
				{
					TwitchEventActionEntry twitchEventActionEntry = this.LiveEvents[j];
					if (twitchEventActionEntry.Event.EventName == gameEventID && twitchEventActionEntry.UserName == viewerName)
					{
						Debug.LogWarning(string.Format("Twitch Debug: Live Event: Refunded {0} for {1}.", twitchEventActionEntry.Event.EventTitle, viewerName));
						if (twitchEventActionEntry.HistoryEntry != null)
						{
							twitchEventActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Reimbursed;
							return;
						}
						break;
					}
					else
					{
						j++;
					}
				}
			}
		}

		// Token: 0x0600C00C RID: 49164 RVA: 0x0047143C File Offset: 0x0046F63C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventCompleted(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.LiveActionEntries.Count; i++)
				{
					TwitchActionEntry twitchActionEntry = this.LiveActionEntries[i];
					if (!twitchActionEntry.ReadyForRemove && twitchActionEntry.Action.EventName == gameEventID && twitchActionEntry.Target.entityId == targetEntityID && twitchActionEntry.UserName == viewerName)
					{
						twitchActionEntry.ReadyForRemove = true;
						if (twitchActionEntry.HistoryEntry != null)
						{
							twitchActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Completed;
						}
						return;
					}
				}
				return;
			}
			if (tag == "event")
			{
				for (int j = 0; j < this.LiveEvents.Count; j++)
				{
					TwitchEventActionEntry twitchEventActionEntry = this.LiveEvents[j];
					if (!twitchEventActionEntry.ReadyForRemove && twitchEventActionEntry.UserName == viewerName && twitchEventActionEntry.Event.EventName == gameEventID)
					{
						if (twitchEventActionEntry.HistoryEntry != null)
						{
							twitchEventActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Completed;
						}
						twitchEventActionEntry.ReadyForRemove = true;
						return;
					}
				}
			}
		}

		// Token: 0x0600C00D RID: 49165 RVA: 0x00471544 File Offset: 0x0046F744
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntitySpawned(string gameEventID, int entityID, string tag)
		{
			if (tag == "action")
			{
				int i = 0;
				while (i < this.LiveActionEntries.Count)
				{
					if (!this.LiveActionEntries[i].ReadyForRemove && this.LiveActionEntries[i].Action.EventName == gameEventID)
					{
						Entity entity = null;
						if (!this.LiveActionEntries[i].Action.AddsToCooldown)
						{
							return;
						}
						TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = new TwitchSpawnedEntityEntry
						{
							Action = this.LiveActionEntries[i],
							SpawnedEntityID = entityID
						};
						this.liveList.Add(twitchSpawnedEntityEntry);
						if (entity == null)
						{
							entity = this.world.GetEntity(entityID);
						}
						twitchSpawnedEntityEntry.SpawnedEntity = entity;
						if (twitchSpawnedEntityEntry.Action.Action.RespawnCountType != TwitchAction.RespawnCountTypes.None)
						{
							TwitchRespawnEntry respawnEntry = this.GetRespawnEntry(twitchSpawnedEntityEntry.Action.UserName, twitchSpawnedEntityEntry.Action.Target, twitchSpawnedEntityEntry.Action.Action);
							respawnEntry.SpawnedEntities.Add(entityID);
							twitchSpawnedEntityEntry.RespawnEntry = respawnEntry;
						}
						this.actionSpawnLiveList.Add(twitchSpawnedEntityEntry);
						return;
					}
					else
					{
						i++;
					}
				}
				return;
			}
			if (tag == "event")
			{
				int j = 0;
				while (j < this.LiveEvents.Count)
				{
					if (!this.LiveEvents[j].ReadyForRemove && this.LiveEvents[j].Event.EventName == gameEventID)
					{
						Entity entity2 = null;
						if (entity2 == null)
						{
							entity2 = this.world.GetEntity(entityID);
						}
						if (!(entity2 is EntityAlive))
						{
							return;
						}
						TwitchSpawnedEntityEntry twitchSpawnedEntityEntry2 = new TwitchSpawnedEntityEntry
						{
							Event = this.LiveEvents[j],
							SpawnedEntityID = entityID
						};
						this.liveList.Add(twitchSpawnedEntityEntry2);
						twitchSpawnedEntityEntry2.SpawnedEntity = entity2;
						this.actionSpawnLiveList.Add(twitchSpawnedEntityEntry2);
						return;
					}
					else
					{
						j++;
					}
				}
				return;
			}
			if (!(tag == "vote") || this.VotingManager.CurrentEvent == null || !(this.VotingManager.CurrentEvent.VoteClass.GameEvent == gameEventID))
			{
				return;
			}
			Entity entity3 = null;
			if (entity3 == null)
			{
				entity3 = this.world.GetEntity(entityID);
			}
			if (!(entity3 is EntityAlive))
			{
				return;
			}
			TwitchSpawnedEntityEntry twitchSpawnedEntityEntry3 = new TwitchSpawnedEntityEntry
			{
				Vote = this.VotingManager.CurrentEvent,
				SpawnedEntityID = entityID
			};
			this.liveList.Add(twitchSpawnedEntityEntry3);
			twitchSpawnedEntityEntry3.SpawnedEntity = entity3;
			this.VotingManager.CurrentEvent.ActiveSpawns.Add(entityID);
			this.actionSpawnLiveList.Add(twitchSpawnedEntityEntry3);
		}

		// Token: 0x0600C00E RID: 49166 RVA: 0x00471800 File Offset: 0x0046FA00
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameBlocksAdded(string gameEventID, int blockGroupID, List<Vector3i> blockList, string tag)
		{
			if (tag == "action")
			{
				int i = 0;
				while (i < this.LiveActionEntries.Count)
				{
					if (!this.LiveActionEntries[i].ReadyForRemove && this.LiveActionEntries[i].Action.EventName == gameEventID)
					{
						if (!this.LiveActionEntries[i].Action.AddsToCooldown)
						{
							return;
						}
						TwitchSpawnedBlocksEntry twitchSpawnedBlocksEntry = new TwitchSpawnedBlocksEntry
						{
							BlockGroupID = blockGroupID,
							Action = this.LiveActionEntries[i],
							blocks = blockList.ToList<Vector3i>()
						};
						this.liveBlockList.Add(twitchSpawnedBlocksEntry);
						if (twitchSpawnedBlocksEntry.Action.Action.RespawnCountType != TwitchAction.RespawnCountTypes.None)
						{
							TwitchRespawnEntry respawnEntry = this.GetRespawnEntry(twitchSpawnedBlocksEntry.Action.UserName, twitchSpawnedBlocksEntry.Action.Target, twitchSpawnedBlocksEntry.Action.Action);
							respawnEntry.SpawnedBlocks.AddRange(blockList);
							twitchSpawnedBlocksEntry.RespawnEntry = respawnEntry;
						}
						return;
					}
					else
					{
						i++;
					}
				}
				return;
			}
			if (tag == "event")
			{
				for (int j = 0; j < this.LiveEvents.Count; j++)
				{
					if (!this.LiveEvents[j].ReadyForRemove && this.LiveEvents[j].Event.EventName == gameEventID)
					{
						TwitchSpawnedBlocksEntry item = new TwitchSpawnedBlocksEntry
						{
							BlockGroupID = blockGroupID,
							Event = this.LiveEvents[j],
							blocks = blockList.ToList<Vector3i>()
						};
						this.liveBlockList.Add(item);
						return;
					}
				}
				return;
			}
			if (tag == "vote" && this.VotingManager.CurrentEvent != null && this.VotingManager.CurrentEvent.VoteClass.GameEvent == gameEventID)
			{
				TwitchSpawnedBlocksEntry item2 = new TwitchSpawnedBlocksEntry
				{
					BlockGroupID = blockGroupID,
					Vote = this.VotingManager.CurrentEvent,
					blocks = blockList.ToList<Vector3i>()
				};
				this.liveBlockList.Add(item2);
				return;
			}
		}

		// Token: 0x0600C00F RID: 49167 RVA: 0x00471A14 File Offset: 0x0046FC14
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameBlocksRemoved(int blockGroupID, bool isDespawn)
		{
			for (int i = 0; i < this.liveBlockList.Count; i++)
			{
				if (this.liveBlockList[i].BlockGroupID == blockGroupID)
				{
					if (this.liveBlockList[i].RespawnEntry != null)
					{
						TwitchRespawnEntry respawnEntry = this.liveBlockList[i].RespawnEntry;
						if (respawnEntry.RemoveAllSpawnedBlock(!isDespawn) && respawnEntry.ReadyForRemove)
						{
							this.RespawnEntries.Remove(respawnEntry);
						}
					}
					this.liveBlockList.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0600C010 RID: 49168 RVA: 0x00471AA0 File Offset: 0x0046FCA0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameBlockRemoved(Vector3i blockRemoved)
		{
			for (int i = 0; i < this.liveBlockList.Count; i++)
			{
				if (this.liveBlockList[i].RemoveBlock(blockRemoved))
				{
					this.liveBlockList[i].TimeRemaining = 5f;
					return;
				}
			}
		}

		// Token: 0x0600C011 RID: 49169 RVA: 0x00471AF0 File Offset: 0x0046FCF0
		public void HandleConsoleAction(List<string> consoleParams)
		{
			for (int i = 0; i < this.TwitchCommandList.Count; i++)
			{
				for (int j = 0; j < this.TwitchCommandList[i].CommandText.Length; j++)
				{
					if (consoleParams[0].StartsWith(this.TwitchCommandList[i].CommandText[j]))
					{
						this.TwitchCommandList[i].ExecuteConsole(consoleParams);
						return;
					}
				}
			}
		}

		// Token: 0x0600C012 RID: 49170 RVA: 0x00471B68 File Offset: 0x0046FD68
		public bool IsActionAvailable(string actionName)
		{
			if (!this.VotingManager.VotingIsActive)
			{
				if (actionName[0] != '#')
				{
					actionName = "#" + actionName.ToLower();
				}
				if (this.twitchActive && this.VoteLockedLevel != TwitchVoteLockTypes.ActionsLocked && this.AllowActions && this.AvailableCommands.ContainsKey(actionName))
				{
					TwitchAction twitchAction = this.AvailableCommands[actionName];
					if (!twitchAction.IgnoreCooldown)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							return false;
						}
						if ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && twitchAction.WaitingBlocked)
						{
							return false;
						}
					}
					if (this.UseProgression && !this.OverrideProgession && twitchAction.StartGameStage != -1 && twitchAction.StartGameStage > this.HighestGameStage)
					{
						return false;
					}
					if (!twitchAction.CanUse)
					{
						return false;
					}
					if ((twitchAction.IgnoreCooldown || !twitchAction.CooldownBlocked || !this.OnCooldown) && twitchAction.IsReady(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600C013 RID: 49171 RVA: 0x00471C7C File Offset: 0x0046FE7C
		public void HandleExtensionMessage(int userId, string message, bool isRerun, int creditUsed, int bitsUsed)
		{
			string text;
			if (!this.ViewerData.IdToUsername.TryGetValue(userId, out text))
			{
				return;
			}
			bool flag = false;
			if (creditUsed < 0)
			{
				creditUsed = 0;
			}
			string[] array = message.Split(' ', StringSplitOptions.None);
			string text2 = array[0];
			TwitchAction twitchAction = null;
			if (this.AvailableCommands.ContainsKey(text2))
			{
				twitchAction = this.AvailableCommands[text2];
			}
			else
			{
				foreach (TwitchAction twitchAction2 in TwitchActionManager.TwitchActions.Values)
				{
					if (twitchAction2.IsInPreset(this.CurrentActionPreset) && twitchAction2.Command == text2)
					{
						twitchAction = twitchAction2;
						break;
					}
				}
			}
			if (twitchAction == null)
			{
				return;
			}
			bool flag2 = twitchAction.PointType == TwitchAction.PointTypes.Bits;
			if (!this.VotingManager.VotingIsActive && this.twitchActive && (flag2 || (this.VoteLockedLevel != TwitchVoteLockTypes.ActionsLocked && this.AllowActions)))
			{
				if (!isRerun && !flag2)
				{
					if (!twitchAction.IgnoreCooldown)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							return;
						}
						if ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && twitchAction.WaitingBlocked)
						{
							return;
						}
					}
					if (this.UseProgression && !this.OverrideProgession && twitchAction.StartGameStage != -1 && twitchAction.StartGameStage > this.HighestGameStage)
					{
						return;
					}
					if (!twitchAction.CanUse)
					{
						return;
					}
				}
				if (flag2 || isRerun || twitchAction.IgnoreCooldown || !twitchAction.CooldownBlocked || !this.OnCooldown || ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && !twitchAction.WaitingBlocked))
				{
					TwitchActionEntry twitchActionEntry = null;
					if ((isRerun || twitchAction.IsReady(this)) && this.ViewerData.HandleInitialActionEntrySetup(text, twitchAction, isRerun, flag2, bitsUsed, out twitchActionEntry))
					{
						twitchActionEntry.UserName = text;
						twitchActionEntry.ChannelNotify = (twitchAction.PointType == TwitchAction.PointTypes.Bits);
						twitchActionEntry.IsBitAction = flag2;
						twitchActionEntry.IsReRun = isRerun;
						twitchActionEntry.Action = twitchAction;
						EntityPlayer entityPlayer = this.LocalPlayer;
						if (array.Length > 1 && this.LocalPlayer.Party != null)
						{
							string b = message.Substring(array[0].Length + 1).ToLower();
							for (int i = 0; i < this.LocalPlayer.Party.MemberList.Count; i++)
							{
								if (this.LocalPlayer.Party.MemberList[i].EntityName.ToLower() == b)
								{
									entityPlayer = this.LocalPlayer.Party.MemberList[i];
									break;
								}
							}
							if (entityPlayer != this.LocalPlayer && entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
							{
								if (flag2)
								{
									ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
									twitchActionEntry.Target = entityPlayer;
									this.AddActionHistory(twitchActionEntry, viewerEntry, TwitchActionHistoryEntry.EntryStates.Reimbursed);
									this.ShowReimburseMessage(twitchActionEntry, viewerEntry);
								}
								this.ViewerData.ReimburseAction(twitchActionEntry);
								return;
							}
							if (this.PartyInfo.ContainsKey(entityPlayer) && this.PartyInfo[entityPlayer].Cooldown > 0f)
							{
								if (flag2)
								{
									ViewerEntry viewerEntry2 = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
									twitchActionEntry.Target = entityPlayer;
									this.AddActionHistory(twitchActionEntry, viewerEntry2, TwitchActionHistoryEntry.EntryStates.Reimbursed);
									this.ShowReimburseMessage(twitchActionEntry, viewerEntry2);
								}
								this.ViewerData.ReimburseAction(twitchActionEntry);
								return;
							}
						}
						twitchActionEntry.Target = entityPlayer;
						if (flag2)
						{
							int currentCost = twitchAction.CurrentCost;
							int creditsUsed = twitchActionEntry.CreditsUsed;
							int num = Utils.FastMax(0, currentCost - creditsUsed);
							if (bitsUsed < num)
							{
								Debug.LogWarning(string.Concat(new string[]
								{
									"[Bits] Insufficient bits for '",
									text2,
									"' by ",
									text,
									". ",
									string.Format("Cost={0} CreditsSpent={1} BitsRequired={2} BitsUsed={3}. ", new object[]
									{
										currentCost,
										creditsUsed,
										num,
										bitsUsed
									}),
									"Rejecting + reimbursing credits + resyncing."
								}));
								if (creditsUsed > 0)
								{
									this.ViewerData.ReimburseAction(text, creditsUsed, twitchAction);
								}
								ViewerEntry viewerEntry3 = this.ViewerData.GetViewerEntry(text);
								if (viewerEntry3 != null)
								{
									this.PushBalanceToExtensionQueue(viewerEntry3.UserID.ToString(), viewerEntry3.BitCredits);
								}
								return;
							}
							if (creditsUsed != creditUsed)
							{
								Debug.LogWarning(string.Concat(new string[]
								{
									"[Bits] Credit split mismatch for '",
									text2,
									"' by ",
									text,
									". ",
									string.Format("ClientCreditsSpent={0} ExtCreditUsed={1}. Proceeding (client-authoritative).", creditsUsed, creditUsed)
								}));
							}
							int num2 = bitsUsed - num;
							if (num2 > 0)
							{
								this.ViewerData.AddCredit(text, num2, false);
							}
						}
						if (twitchAction.ModifiedCooldown > 0f)
						{
							twitchAction.tempCooldownSet = this.CurrentUnityTime;
							twitchAction.tempCooldown = 1f;
						}
						if (twitchActionEntry.CreditsUsed > 0)
						{
							ViewerEntry viewerEntry4 = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
							this.PushBalanceToExtensionQueue(viewerEntry4.UserID.ToString(), viewerEntry4.BitCredits);
						}
						this.QueuedActionEntries.Add(twitchActionEntry);
						flag = true;
					}
				}
			}
			if (!flag && flag2)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"[Bits] Bit action '",
					text2,
					"' did not start for ",
					text,
					". Rejecting + resyncing balance (no crediting). ",
					string.Format("ExtCreditUsed={0} BitsUsed={1} Cost={2}", creditUsed, bitsUsed, twitchAction.CurrentCost)
				}));
				ViewerEntry viewerEntry5 = this.ViewerData.GetViewerEntry(text);
				if (viewerEntry5 != null)
				{
					this.PushBalanceToExtensionQueue(viewerEntry5.UserID.ToString(), viewerEntry5.BitCredits);
				}
				return;
			}
		}

		// Token: 0x0600C014 RID: 49172 RVA: 0x00472268 File Offset: 0x00470468
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleMessage(TwitchIRCClient.TwitchChatMessage message)
		{
			switch (message.MessageType)
			{
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Invalid:
				return;
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Message:
			{
				if (this.InitState != TwitchManager.InitStates.Ready)
				{
					return;
				}
				string text = message.Message.ToLower();
				ViewerEntry entry = this.ViewerData.UpdateViewerEntry(message.UserID, message.UserName, message.UserNameColor, message.isSub);
				if (message.isBroadcaster)
				{
					if (text.StartsWith("#cooldowninfo"))
					{
						this.ircClient.SendChannelMessage(string.Format("[7DTD]: Cooldown is at {0}/{1}.", this.CurrentCooldownFill, this.CurrentCooldownPreset.CooldownFillMax), true);
					}
					else if (text.StartsWith("#reset "))
					{
						this.actionSpawnLiveList.Clear();
						this.LiveActionEntries.Clear();
						this.ircClient.SendChannelMessage("[7DTD]: Action Live list Cleared!", true);
					}
				}
				for (int i = 0; i < this.TwitchCommandList.Count; i++)
				{
					for (int j = 0; j < this.TwitchCommandList[i].CommandTextList.Count; j++)
					{
						if (text.StartsWith(this.TwitchCommandList[i].CommandTextList[j]) && this.TwitchCommandList[i].CheckAllowed(message))
						{
							this.TwitchCommandList[i].Execute(entry, message);
							break;
						}
					}
				}
				this.VotingManager.HandleMessage(message);
				if (!this.VotingManager.VotingIsActive)
				{
					if (this.IntegrationSetting == TwitchManager.IntegrationSettings.ExtensionOnly)
					{
						return;
					}
					string[] array = text.Split(' ', StringSplitOptions.None);
					string key = array[0];
					if (this.AlternateCommands.ContainsKey(key))
					{
						key = this.AlternateCommands[key];
					}
					if (this.twitchActive && this.VoteLockedLevel != TwitchVoteLockTypes.ActionsLocked && this.AllowActions && this.AvailableCommands.ContainsKey(key))
					{
						TwitchAction twitchAction = this.AvailableCommands[key];
						if (twitchAction.PointType == TwitchAction.PointTypes.Bits)
						{
							return;
						}
						if (!twitchAction.IgnoreCooldown)
						{
							if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.Startup || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
							{
								return;
							}
							if ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && twitchAction.WaitingBlocked)
							{
								return;
							}
						}
						if (this.UseProgression && !this.OverrideProgession && twitchAction.StartGameStage != -1 && twitchAction.StartGameStage > this.HighestGameStage)
						{
							return;
						}
						if (!twitchAction.CanUse || !twitchAction.CheckUsable(message))
						{
							return;
						}
						if (twitchAction.IgnoreCooldown || !twitchAction.CooldownBlocked || !this.OnCooldown || ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && !twitchAction.WaitingBlocked))
						{
							TwitchActionEntry twitchActionEntry = null;
							if (twitchAction.IsReady(this) && this.ViewerData.HandleInitialActionEntrySetup(message.UserName, twitchAction, false, false, out twitchActionEntry))
							{
								twitchActionEntry.UserName = message.UserName;
								EntityPlayer entityPlayer = this.LocalPlayer;
								if (array.Length > 1 && this.LocalPlayer.Party != null)
								{
									int index = -1;
									if (StringParsers.TryParseSInt32(array[1], out index))
									{
										entityPlayer = this.LocalPlayer.Party.GetMemberAtIndex(index, this.LocalPlayer);
										if (entityPlayer == null)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
										if (entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
									}
									else
									{
										string b = text.Substring(array[0].Length + 1);
										bool flag = false;
										for (int k = 0; k < this.LocalPlayer.Party.MemberList.Count; k++)
										{
											if (this.LocalPlayer.Party.MemberList[k].EntityName.ToLower() == b)
											{
												entityPlayer = this.LocalPlayer.Party.MemberList[k];
												flag = true;
												break;
											}
										}
										if (!flag)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
										if (entityPlayer != this.LocalPlayer && entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
										if (this.PartyInfo.ContainsKey(entityPlayer) && this.PartyInfo[entityPlayer].Cooldown > 0f)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
									}
								}
								if (twitchAction.StreamerOnly && entityPlayer != this.LocalPlayer)
								{
									this.ViewerData.ReimburseAction(twitchActionEntry);
									return;
								}
								twitchActionEntry.Target = entityPlayer;
								twitchActionEntry.Action = twitchAction;
								this.QueuedActionEntries.Add(twitchActionEntry);
								return;
							}
						}
					}
				}
				break;
			}
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Output:
				break;
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Authenticated:
				if (this.InitState != TwitchManager.InitStates.Ready)
				{
					List<string> list = new List<string>();
					list.Add("CAP REQ :twitch.tv/membership");
					list.Add("CAP REQ :twitch.tv/tags");
					list.Add("CAP REQ :twitch.tv/commands");
					this.ircClient.SendIrcMessages(list, false);
					this.InitState = TwitchManager.InitStates.CheckingForExtension;
					TwitchAuthentication.bFirstLogin = false;
					return;
				}
				break;
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Raid:
			{
				int viewerAmount = StringParsers.ParseSInt32(message.Message, 0, -1, NumberStyles.Integer);
				this.HandleRaid(message.UserName.ToLower(), message.UserID, viewerAmount);
				return;
			}
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Charity:
			{
				int charityAmount = StringParsers.ParseSInt32(message.Message, 0, -1, NumberStyles.Integer);
				this.HandleCharity(message.UserName.ToLower(), message.UserID, charityAmount);
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x0600C015 RID: 49173 RVA: 0x004727E0 File Offset: 0x004709E0
		public void DisplayDebug(string message)
		{
			Debug.LogWarning("Called: " + message);
			Debug.LogWarning(string.Format("[7DTD]: Spawns Alive: {0}  Blocks Alive: {1}  ActionLiveList: {2}.", this.actionSpawnLiveList.Count, this.liveBlockList.Count, this.LiveActionEntries.Count));
			for (int i = 0; i < this.actionSpawnLiveList.Count; i++)
			{
				if (this.actionSpawnLiveList[i].SpawnedEntity != null)
				{
					Debug.LogWarning(string.Format("Spawn Alive: {0}", this.actionSpawnLiveList[i].SpawnedEntity.name));
				}
			}
			for (int j = 0; j < this.LiveActionEntries.Count; j++)
			{
				Debug.LogWarning(string.Format("Action: {0} Target: {1} Viewer: {2}", this.LiveActionEntries[j].Action.Name, this.LiveActionEntries[j].Target.EntityName, this.LiveActionEntries[j].UserName));
			}
			for (int k = 0; k < this.EventQueue.Count; k++)
			{
				Debug.LogWarning(string.Format("Event: {0} User: {1} Sent: {2}", this.EventQueue[k].Event.EventTitle, this.EventQueue[k].UserName, this.EventQueue[k].IsSent));
			}
			this.ircClient.SendChannelMessage("[7DTD]: Debug Complete!", true);
		}

		// Token: 0x0600C016 RID: 49174 RVA: 0x00472968 File Offset: 0x00470B68
		public void AddToPot(int amount)
		{
			this.RewardPot += amount;
			if (this.RewardPot < 0)
			{
				this.RewardPot = 0;
			}
			if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
			{
				TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
			}
		}

		// Token: 0x0600C017 RID: 49175 RVA: 0x004729B8 File Offset: 0x00470BB8
		public void AddToBitPot(int amount)
		{
			this.BitPot += amount;
			if (this.BitPot < 0)
			{
				this.BitPot = 0;
			}
			if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
			{
				TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
			}
		}

		// Token: 0x0600C018 RID: 49176 RVA: 0x00472A08 File Offset: 0x00470C08
		public void SetPot(int newPot)
		{
			if (newPot < 0)
			{
				newPot = 0;
			}
			this.RewardPot = newPot;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_PimpPotBalance, this.RewardPot), true);
			if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
			{
				TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
			}
		}

		// Token: 0x0600C019 RID: 49177 RVA: 0x00472A68 File Offset: 0x00470C68
		public void SetBitPot(int newPot)
		{
			if (newPot < 0)
			{
				newPot = 0;
			}
			this.BitPot = newPot;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_BitPotBalance, this.BitPot), true);
			if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
			{
				TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
			}
		}

		// Token: 0x0600C01A RID: 49178 RVA: 0x00472AC8 File Offset: 0x00470CC8
		public void SetCooldown(float newCooldownTime, TwitchManager.CooldownTypes newCooldownType, bool displayToChannel = false, bool playCooldownSound = true)
		{
			if (this.LocalPlayer == null)
			{
				return;
			}
			if (this.CooldownType == newCooldownType && this.CooldownTime == newCooldownTime)
			{
				return;
			}
			if (newCooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && newCooldownType != TwitchManager.CooldownTypes.SafeCooldown && newCooldownType != TwitchManager.CooldownTypes.SafeCooldownExit && newCooldownType != TwitchManager.CooldownTypes.None)
			{
				this.LocalPlayer.HandleTwitchActionsTempEnabled((newCooldownTime > 15f) ? EntityPlayer.TwitchActionsStates.TempDisabled : EntityPlayer.TwitchActionsStates.TempDisabledEnding);
			}
			else if (newCooldownType == TwitchManager.CooldownTypes.None)
			{
				this.LocalPlayer.HandleTwitchActionsTempEnabled(EntityPlayer.TwitchActionsStates.Enabled);
			}
			this.CooldownType = newCooldownType;
			this.CooldownTime = newCooldownTime;
			if (displayToChannel)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_CooldownTime, newCooldownTime), true);
			}
			this.HandleCooldownActionLocking();
			if (playCooldownSound && this.LocalPlayer != null && newCooldownType != TwitchManager.CooldownTypes.None)
			{
				Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_cooldown_started");
			}
		}

		// Token: 0x0600C01B RID: 49179 RVA: 0x00472B90 File Offset: 0x00470D90
		public bool ForceEndCooldown(bool playEndSound = true)
		{
			if (this.IsReady && (this.CooldownType == TwitchManager.CooldownTypes.MaxReached || this.CooldownType == TwitchManager.CooldownTypes.Time))
			{
				this.SetCooldown(0f, TwitchManager.CooldownTypes.None, false, true);
				this.CurrentCooldownFill = 0f;
				if (playEndSound)
				{
					Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_end_cooldown");
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600C01C RID: 49180 RVA: 0x00472BEC File Offset: 0x00470DEC
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConfirmAction(TwitchActionEntry entry)
		{
			TwitchAction action = entry.Action;
			if (!entry.IsRespawn)
			{
				action.SetQueued();
			}
			if (!entry.IsRespawn)
			{
				if (!entry.IsBitAction && this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
				{
					int num = (int)EffectManager.GetValue(PassiveEffects.TwitchAddPimpPot, null, (float)action.ModifiedCost * this.ActionPotPercentage, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					if (num > 0)
					{
						this.RewardPot += num;
						if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
						{
							TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
						}
					}
				}
				if (entry.IsBitAction && entry.BitsUsed > 0)
				{
					int num2 = (int)((float)(entry.BitsUsed - entry.CreditsUsed) * this.BitPotPercentage);
					if (num2 > 0)
					{
						this.BitPot += num2;
						if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
						{
							TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
						}
					}
				}
			}
			this.AddCooldownForAction(action);
			if (!entry.IsRespawn)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(entry.UserName);
				if (EffectManager.GetValue(PassiveEffects.DisableGameEventNotify, null, 0f, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 0f)
				{
					if (action.DelayNotify)
					{
						GameManager.Instance.StartCoroutine(this.onDelayedActionNotify(entry, viewerEntry));
					}
					else
					{
						this.DisplayActionNotification(entry, viewerEntry);
					}
				}
				this.AddActionHistory(entry, viewerEntry, TwitchActionHistoryEntry.EntryStates.Waiting);
			}
		}

		// Token: 0x0600C01D RID: 49181 RVA: 0x00472D70 File Offset: 0x00470F70
		public void ShowReimburseMessage(string userName, int bitsUsed, TwitchAction action, ViewerEntry viewerEntry = null)
		{
			if (bitsUsed > 0)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				if (viewerEntry == null)
				{
					return;
				}
				string msg = string.Format(this.ingameOutput_RefundedAction, new object[]
				{
					viewerEntry.UserColor,
					userName,
					bitsUsed,
					action.Command
				});
				Debug.LogWarning(string.Format("{0} has been refunded {1} bits for {2}", userName, bitsUsed, action.Command));
				this.AddToInGameChatQueue(msg, "twitch_refund");
			}
		}

		// Token: 0x0600C01E RID: 49182 RVA: 0x00472DF2 File Offset: 0x00470FF2
		public void ShowReimburseMessage(TwitchActionEntry entry, ViewerEntry viewerEntry = null)
		{
			this.ShowReimburseMessage(entry.UserName, entry.BitsUsed, entry.Action, viewerEntry);
		}

		// Token: 0x0600C01F RID: 49183 RVA: 0x00472E10 File Offset: 0x00471010
		public TwitchActionHistoryEntry AddActionHistory(TwitchActionEntry entry, ViewerEntry viewerEntry, TwitchActionHistoryEntry.EntryStates startState = TwitchActionHistoryEntry.EntryStates.Waiting)
		{
			if (entry.IsReRun)
			{
				return null;
			}
			if (entry.HistoryEntry == null)
			{
				TwitchActionHistoryEntry twitchActionHistoryEntry = entry.SetupHistoryEntry(viewerEntry);
				twitchActionHistoryEntry.EntryState = startState;
				entry.HistoryEntry = twitchActionHistoryEntry;
				this.ActionHistory.Insert(0, twitchActionHistoryEntry);
				if (this.ActionHistory.Count > 500)
				{
					this.ActionHistory.RemoveAt(this.ActionHistory.Count - 1);
				}
				if (this.ActionHistoryAdded != null)
				{
					this.ActionHistoryAdded();
				}
				return twitchActionHistoryEntry;
			}
			entry.HistoryEntry.EntryState = startState;
			return entry.HistoryEntry;
		}

		// Token: 0x0600C020 RID: 49184 RVA: 0x00472EA4 File Offset: 0x004710A4
		public void AddVoteHistory(TwitchVote vote)
		{
			TwitchActionHistoryEntry twitchActionHistoryEntry = new TwitchActionHistoryEntry("Vote", "FFFFFF", null, vote, null);
			this.VoteHistory.Insert(0, twitchActionHistoryEntry);
			twitchActionHistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Completed;
			if (this.VoteHistory.Count > 500)
			{
				this.VoteHistory.RemoveAt(this.VoteHistory.Count - 1);
			}
			if (this.VoteHistoryAdded != null)
			{
				this.VoteHistoryAdded();
			}
		}

		// Token: 0x0600C021 RID: 49185 RVA: 0x00472F18 File Offset: 0x00471118
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddCooldownForAction(TwitchAction action)
		{
			if (action.AddsToCooldown)
			{
				int num = (int)EffectManager.GetValue(PassiveEffects.TwitchAddCooldown, null, (float)action.CooldownAddAmount, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				if (num > 0)
				{
					this.AddCooldownAmount(num);
				}
			}
		}

		// Token: 0x0600C022 RID: 49186 RVA: 0x00472F64 File Offset: 0x00471164
		public void AddCooldownAmount(int amount)
		{
			if (this.CurrentCooldownPreset == null)
			{
				this.GetCooldownMax();
			}
			if (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled)
			{
				return;
			}
			if (this.IsVoting)
			{
				return;
			}
			if (this.CurrentCooldownPreset != null && this.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Fill)
			{
				if (this.CurrentCooldownFill < this.CurrentCooldownPreset.CooldownFillMax)
				{
					this.CurrentCooldownFill += (float)amount;
					if (this.CurrentCooldownFill >= this.CurrentCooldownPreset.CooldownFillMax)
					{
						this.SetCooldown((float)this.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.MaxReachedWaiting, false, true);
						if (this.ircClient != null)
						{
							this.ircClient.SendChannelMessage(this.chatOutput_CooldownStarted, true);
						}
					}
				}
				else if (this.CooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && this.CooldownType != TwitchManager.CooldownTypes.MaxReached)
				{
					this.SetCooldown((float)this.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.MaxReachedWaiting, false, true);
					if (this.ircClient != null)
					{
						this.ircClient.SendChannelMessage(this.chatOutput_CooldownStarted, true);
					}
				}
			}
			this.UIDirty = true;
		}

		// Token: 0x0600C023 RID: 49187 RVA: 0x00473077 File Offset: 0x00471277
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator onDelayedActionNotify(TwitchActionEntry entry, ViewerEntry viewerEntry)
		{
			yield return new WaitForSeconds(5f);
			if (this.ircClient != null)
			{
				this.DisplayActionNotification(entry, viewerEntry);
			}
			yield break;
		}

		// Token: 0x0600C024 RID: 49188 RVA: 0x00473094 File Offset: 0x00471294
		[PublicizedFrom(EAccessModifier.Private)]
		public void DisplayActionNotification(TwitchActionEntry entry, ViewerEntry viewerEntry)
		{
			if (entry.HistoryEntry != null && entry.HistoryEntry.EntryState == TwitchActionHistoryEntry.EntryStates.Reimbursed)
			{
				return;
			}
			TwitchAction action = entry.Action;
			if (entry.ChannelNotify && action.TwitchNotify)
			{
				string message;
				if (action.PointType == TwitchAction.PointTypes.Bits)
				{
					message = string.Format(this.chatOutput_ActivatedBitAction, new object[]
					{
						entry.UserName,
						action.Command,
						viewerEntry.CombinedPoints,
						entry.Target.EntityName,
						entry.Action.CurrentCost
					});
					if (action.PlayBitSound)
					{
						Manager.PlayInsidePlayerHead(action.IsPositive ? "twitch_donation" : "twitch_donation_bad", this.LocalPlayer.entityId, 0f, false, false);
					}
				}
				else
				{
					message = string.Format(this.chatOutput_ActivatedAction, new object[]
					{
						entry.UserName,
						action.Command,
						viewerEntry.CombinedPoints,
						entry.Target.EntityName
					});
				}
				this.ircClient.SendChannelMessage(message, true);
			}
			string text = string.Format(this.ingameOutput_ActivatedAction, new object[]
			{
				viewerEntry.UserColor,
				entry.UserName,
				action.Command,
				entry.Target.EntityName
			});
			this.SendServerChatMessage(text);
			this.AddToInGameChatQueue(text, null);
		}

		// Token: 0x0600C025 RID: 49189 RVA: 0x00473208 File Offset: 0x00471408
		[PublicizedFrom(EAccessModifier.Private)]
		public void SendServerChatMessage(string serverMsg)
		{
			if (this.LocalPlayer.IsInParty())
			{
				List<int> memberIdList = this.LocalPlayer.Party.GetMemberIdList(this.LocalPlayer);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					if (memberIdList == null)
					{
						return;
					}
					using (List<int>.Enumerator enumerator = memberIdList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int entityId = enumerator.Current;
							ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
							if (clientInfo != null)
							{
								clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageSimpleChat>().Setup(serverMsg));
							}
						}
						return;
					}
				}
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSimpleChat>().Setup(serverMsg, memberIdList), false);
			}
		}

		// Token: 0x0600C026 RID: 49190 RVA: 0x004732C4 File Offset: 0x004714C4
		[PublicizedFrom(EAccessModifier.Private)]
		public void RefreshCommands(bool displayMessage)
		{
			if (this.LocalPlayer.unModifiedGameStage != this.HighestGameStage)
			{
				int highestGameStage = this.HighestGameStage;
				this.HighestGameStage = this.LocalPlayer.unModifiedGameStage;
				this.GetCooldownMax();
				if (this.UseProgression && !this.OverrideProgession)
				{
					this.ActionMessages.Clear();
					this.SetupAvailableCommandsWithOutput(highestGameStage, displayMessage);
					this.ResetDailyCommands(this.lastGameDay, highestGameStage);
					this.HandleCooldownActionLocking();
					this.commandsAvailable = this.AvailableCommands.Count;
				}
			}
		}

		// Token: 0x0600C027 RID: 49191 RVA: 0x00473349 File Offset: 0x00471549
		public void ToggleTwitchActive()
		{
			this.twitchActive = !this.twitchActive;
			this.ActionMessages.Clear();
			this.HandleCooldownActionLocking();
		}

		// Token: 0x0600C028 RID: 49192 RVA: 0x0047336B File Offset: 0x0047156B
		public void SetTwitchActive(bool newActive)
		{
			if (this.twitchActive != newActive)
			{
				this.twitchActive = newActive;
				this.ActionMessages.Clear();
				this.HandleCooldownActionLocking();
			}
		}

		// Token: 0x0600C029 RID: 49193 RVA: 0x00473390 File Offset: 0x00471590
		public void ResetPrices()
		{
			bool flag = false;
			using (Dictionary<string, TwitchAction>.ValueCollection.Enumerator enumerator = TwitchActionManager.TwitchActions.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.UpdateCost(this.BitPriceMultiplier))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.resetCommandsNeeded = true;
			}
		}

		// Token: 0x0600C02A RID: 49194 RVA: 0x004733FC File Offset: 0x004715FC
		public void ResetPricesToDefault()
		{
			bool flag = false;
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				twitchAction.ResetToDefaultCost();
				if (twitchAction.UpdateCost(this.BitPriceMultiplier))
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.resetCommandsNeeded = true;
			}
		}

		// Token: 0x0600C02B RID: 49195 RVA: 0x0047346C File Offset: 0x0047166C
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetCommands()
		{
			this.ActionMessages.Clear();
			this.SetupAvailableCommands();
			if (this.UseProgression && !this.OverrideProgession)
			{
				this.ResetDailyCommands(this.lastGameDay, -1);
			}
			this.HandleCooldownActionLocking();
			this.resetCommandsNeeded = false;
		}

		// Token: 0x0600C02C RID: 49196 RVA: 0x004734A9 File Offset: 0x004716A9
		public void SetUseProgression(bool useProgression)
		{
			if (this.UseProgression != useProgression)
			{
				this.UseProgression = useProgression;
				if (this.InitState == TwitchManager.InitStates.Ready)
				{
					this.resetCommandsNeeded = true;
				}
			}
		}

		// Token: 0x0600C02D RID: 49197 RVA: 0x004734CC File Offset: 0x004716CC
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetCommandCount()
		{
			int num = 0;
			if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
			{
				return 0;
			}
			foreach (string key in TwitchActionManager.TwitchActions.Keys)
			{
				TwitchAction twitchAction = TwitchActionManager.TwitchActions[key];
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					if (this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting)
					{
						if (twitchAction.WaitingBlocked)
						{
							continue;
						}
					}
					else if (twitchAction.CooldownBlocked && this.CooldownTime > 0f)
					{
						continue;
					}
					int startGameStage = twitchAction.StartGameStage;
					if (startGameStage == -1 || startGameStage <= this.HighestGameStage)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600C02E RID: 49198 RVA: 0x0047359C File Offset: 0x0047179C
		public TwitchRespawnEntry GetRespawnEntry(string username, EntityPlayer target, TwitchAction action)
		{
			for (int i = 0; i < this.RespawnEntries.Count; i++)
			{
				if (this.RespawnEntries[i].CheckRespawn(username, target, action))
				{
					return this.RespawnEntries[i];
				}
			}
			TwitchRespawnEntry twitchRespawnEntry = new TwitchRespawnEntry(username, GameEventManager.Current.Random.RandomRange(action.MinRespawnCount, action.MaxRespawnCount), target, action);
			this.RespawnEntries.Add(twitchRespawnEntry);
			return twitchRespawnEntry;
		}

		// Token: 0x0600C02F RID: 49199 RVA: 0x00473614 File Offset: 0x00471814
		public void CheckKiller(EntityPlayer player, EntityAlive killer, Vector3i pos)
		{
			if (this.LocalPlayer == null)
			{
				return;
			}
			if (this.twitchPlayerDeathsThisFrame.Contains(player))
			{
				return;
			}
			if (player == this.LocalPlayer && this.VotingManager != null && this.VotingManager.VotingEnabled && this.VotingManager.CurrentEvent != null)
			{
				this.HandleVoteKill(null);
				this.VotingManager.ResetVoteOnDeath();
				this.twitchPlayerDeathsThisFrame.Add(player);
				return;
			}
			bool flag;
			if (player == this.LocalPlayer)
			{
				flag = true;
			}
			else
			{
				if (this.LocalPlayer.Party == null || !this.LocalPlayer.Party.ContainsMember(player))
				{
					return;
				}
				flag = false;
			}
			TwitchActionEntry twitchActionEntry = null;
			TwitchEventActionEntry twitchEventActionEntry = null;
			TwitchVoteEntry twitchVoteEntry = null;
			if (killer == null)
			{
				int i = this.liveBlockList.Count - 1;
				while (i >= 0)
				{
					if (this.liveBlockList[i].CheckPos(pos))
					{
						TwitchSpawnedBlocksEntry twitchSpawnedBlocksEntry = this.liveBlockList[i];
						twitchActionEntry = twitchSpawnedBlocksEntry.Action;
						twitchEventActionEntry = twitchSpawnedBlocksEntry.Event;
						twitchVoteEntry = twitchSpawnedBlocksEntry.Vote;
						if (twitchSpawnedBlocksEntry.RespawnEntry != null && (flag || twitchActionEntry.Target == player))
						{
							this.RespawnEntries.Remove(twitchSpawnedBlocksEntry.RespawnEntry);
							break;
						}
						break;
					}
					else
					{
						i--;
					}
				}
			}
			else
			{
				int j = this.liveList.Count - 1;
				while (j >= 0)
				{
					if (this.liveList[j].SpawnedEntity == killer)
					{
						TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = this.liveList[j];
						twitchActionEntry = twitchSpawnedEntityEntry.Action;
						twitchEventActionEntry = twitchSpawnedEntityEntry.Event;
						twitchVoteEntry = twitchSpawnedEntityEntry.Vote;
						if (twitchSpawnedEntityEntry.RespawnEntry != null && (flag || twitchSpawnedEntityEntry.Action.Target == player))
						{
							this.RespawnEntries.Remove(twitchSpawnedEntityEntry.RespawnEntry);
							break;
						}
						break;
					}
					else
					{
						j--;
					}
				}
			}
			if (twitchActionEntry == null && twitchEventActionEntry == null && twitchVoteEntry == null)
			{
				for (int k = this.recentlyDeadList.Count - 1; k >= 0; k--)
				{
					if (this.recentlyDeadList[k].SpawnedEntity == killer)
					{
						twitchActionEntry = this.recentlyDeadList[k].Action;
						twitchEventActionEntry = this.recentlyDeadList[k].Event;
						twitchVoteEntry = this.recentlyDeadList[k].Vote;
						break;
					}
				}
			}
			if (twitchActionEntry != null || twitchEventActionEntry != null)
			{
				string text = (twitchActionEntry != null) ? twitchActionEntry.UserName : twitchEventActionEntry.UserName;
				string text2 = (twitchActionEntry != null) ? twitchActionEntry.Action.Command : twitchEventActionEntry.Event.EventTitle;
				if (twitchEventActionEntry != null && (twitchEventActionEntry.Event.EventType == BaseTwitchEventEntry.EventTypes.HypeTrain || twitchEventActionEntry.Event.EventType == BaseTwitchEventEntry.EventTypes.CreatorGoal))
				{
					if (flag)
					{
						int rewardAmount;
						bool flag2;
						if (twitchEventActionEntry.Event.EventType == BaseTwitchEventEntry.EventTypes.HypeTrain)
						{
							TwitchHypeTrainEventEntry twitchHypeTrainEventEntry = (TwitchHypeTrainEventEntry)twitchEventActionEntry.Event;
							rewardAmount = twitchHypeTrainEventEntry.RewardAmount;
							flag2 = (twitchHypeTrainEventEntry.RewardType > TwitchAction.PointTypes.PP);
						}
						else
						{
							TwitchCreatorGoalEventEntry twitchCreatorGoalEventEntry = (TwitchCreatorGoalEventEntry)twitchEventActionEntry.Event;
							rewardAmount = twitchCreatorGoalEventEntry.RewardAmount;
							flag2 = (twitchCreatorGoalEventEntry.RewardType > TwitchAction.PointTypes.PP);
						}
						this.ViewerData.AddPointsAll((!flag2) ? rewardAmount : 0, flag2 ? rewardAmount : 0, false);
						string arg = flag2 ? Localization.Get("TwitchPoints_SP", false, null) : Localization.Get("TwitchPoints_PP", false, null);
						string text3 = string.Format(this.ingameOutput_KilledByHypeTrain, rewardAmount, arg, this.LocalPlayer.EntityName);
						GameManager.ShowTooltip(this.LocalPlayer, text3, false, false, 0f);
						GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text3), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
						this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledByHypeTrain, rewardAmount, arg, this.LocalPlayer.EntityName), true);
						TwitchManager.DeathText = string.Format(this.ingameHypeTrainDeathScreen_Message, rewardAmount, arg);
					}
					this.twitchPlayerDeathsThisFrame.Add(player);
					return;
				}
				if (flag)
				{
					ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(text);
					if ((twitchActionEntry != null && twitchActionEntry.IsBitAction) || (twitchEventActionEntry != null && twitchEventActionEntry.Event.RewardsBitPot))
					{
						if (this.BitPot > 0)
						{
							viewerEntry.BitCredits += this.BitPot;
						}
						string text4 = (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP) ? Localization.Get("TwitchPoints_SP", false, null) : Localization.Get("TwitchPoints_PP", false, null);
						if (this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
						{
							if (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP)
							{
								viewerEntry.SpecialPoints += (float)this.RewardPot;
							}
							else
							{
								viewerEntry.StandardPoints += (float)this.RewardPot;
							}
						}
						this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledByBits, new object[]
						{
							text,
							viewerEntry.BitCredits,
							this.BitPot,
							player.EntityName,
							this.RewardPot,
							text4
						}), true);
						string text5 = string.Format(this.ingameOutput_KilledByBits, new object[]
						{
							viewerEntry.UserColor,
							text,
							this.BitPot,
							player.EntityName,
							this.RewardPot,
							text4
						});
						GameManager.ShowTooltip(this.LocalPlayer, text5, false, false, 0f);
						GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text5), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
						TwitchManager.DeathText = string.Format(this.ingameBitsDeathScreen_Message, new object[]
						{
							viewerEntry.UserColor,
							text,
							text2,
							this.BitPot,
							this.RewardPot,
							text4
						});
						QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.BitPot, "");
						this.BitPot = 0;
						this.RewardPot = TwitchManager.PimpPotDefault;
					}
					else
					{
						string text6 = (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP) ? Localization.Get("TwitchPoints_SP", false, null) : Localization.Get("TwitchPoints_PP", false, null);
						if (this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
						{
							if (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP)
							{
								viewerEntry.SpecialPoints += (float)this.RewardPot;
							}
							else
							{
								viewerEntry.StandardPoints += (float)this.RewardPot;
							}
							this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledStreamer, new object[]
							{
								text,
								viewerEntry.CombinedPoints,
								this.RewardPot,
								text6,
								player.EntityName
							}), true);
							string text7 = string.Format(this.ingameOutput_KilledStreamer, new object[]
							{
								viewerEntry.UserColor,
								text,
								this.RewardPot,
								text6,
								player.EntityName
							});
							GameManager.ShowTooltip(TwitchManager.Current.LocalPlayer, text7, false, false, 0f);
							GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text7), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
							QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.PimpPot, "");
						}
						TwitchManager.DeathText = string.Format(this.ingameDeathScreen_Message, new object[]
						{
							viewerEntry.UserColor,
							text,
							text2,
							this.RewardPot,
							text6
						});
						this.RewardPot = TwitchManager.PimpPotDefault;
					}
					TwitchManager.LeaderboardStats.CheckTopKiller(TwitchManager.LeaderboardStats.AddKill(text, viewerEntry.UserColor));
					this.AddKillToLeaderboard(text, viewerEntry.UserColor);
					this.twitchPlayerDeathsThisFrame.Add(player);
				}
				else
				{
					if (this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
					{
						ViewerEntry viewerEntry2 = this.ViewerData.GetViewerEntry(text);
						int num = Mathf.Min(this.PartyKillRewardMax, this.RewardPot);
						viewerEntry2.StandardPoints += (float)num;
						this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledParty, new object[]
						{
							text,
							viewerEntry2.CombinedPoints,
							num,
							player.EntityName
						}), true);
						string text8 = string.Format(this.ingameOutput_KilledParty, new object[]
						{
							viewerEntry2.UserColor,
							text,
							num,
							player.EntityName
						});
						GameManager.ShowTooltip(TwitchManager.Current.LocalPlayer, text8, false, false, 0f);
						GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text8), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
					}
					this.twitchPlayerDeathsThisFrame.Add(player);
				}
			}
			else if (twitchVoteEntry != null && flag && this.VotingManager != null && !twitchVoteEntry.Complete)
			{
				this.HandleVoteKill(twitchVoteEntry);
			}
			if (flag)
			{
				this.VotingManager.ResetVoteOnDeath();
			}
		}

		// Token: 0x0600C030 RID: 49200 RVA: 0x00473F08 File Offset: 0x00472108
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleVoteKill(TwitchVoteEntry voteEntry)
		{
			List<string> list = this.VotingManager.HandleKiller(voteEntry);
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.ViewerData.GetViewerEntry(list[i]).StandardPoints += (float)this.VotingManager.ViewerDefeatReward;
				}
				string text = string.Format(this.ingameOutput_KilledByVote, this.VotingManager.ViewerDefeatReward, this.LocalPlayer.EntityName);
				GameManager.ShowTooltip(TwitchManager.Current.LocalPlayer, text, false, false, 0f);
				GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledByVote, this.VotingManager.ViewerDefeatReward, this.LocalPlayer.EntityName), true);
				TwitchManager.DeathText = string.Format(this.ingameVoteDeathScreen_Message, this.VotingManager.ViewerDefeatReward);
				list.Clear();
			}
			if (voteEntry != null)
			{
				voteEntry.Complete = true;
			}
		}

		// Token: 0x0600C031 RID: 49201 RVA: 0x00474030 File Offset: 0x00472230
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddKillToLeaderboard(string username, string usercolor)
		{
			bool flag = false;
			for (int i = 0; i < this.Leaderboard.Count; i++)
			{
				if (this.Leaderboard[i].UserName == username)
				{
					this.Leaderboard[i].Kills++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.Leaderboard.Add(new TwitchLeaderboardEntry(username, usercolor, 1));
			}
		}

		// Token: 0x0600C032 RID: 49202 RVA: 0x004740A1 File Offset: 0x004722A1
		public void ClearLeaderboard()
		{
			this.Leaderboard.Clear();
		}

		// Token: 0x0600C033 RID: 49203 RVA: 0x004740AE File Offset: 0x004722AE
		public void AddCooldownPreset(CooldownPreset preset)
		{
			if (this.CooldownPresets == null)
			{
				this.CooldownPresets = new List<CooldownPreset>();
			}
			if (preset.IsDefault)
			{
				this.CooldownPresetIndex = this.CooldownPresets.Count;
			}
			this.CooldownPresets.Add(preset);
		}

		// Token: 0x0600C034 RID: 49204 RVA: 0x004740E8 File Offset: 0x004722E8
		public void SetCooldownPreset(int index)
		{
			if (this.InitState == TwitchManager.InitStates.Ready)
			{
				bool flag = this.CurrentCooldownPreset.CooldownType != this.CooldownPresets[index].CooldownType;
				this.CooldownPresetIndex = index;
				this.GetCooldownMax();
				if (flag)
				{
					this.resetCommandsNeeded = true;
					return;
				}
			}
			else
			{
				this.CooldownPresetIndex = index;
			}
		}

		// Token: 0x0600C035 RID: 49205 RVA: 0x00474140 File Offset: 0x00472340
		public void SetToDefaultCooldown()
		{
			for (int i = 0; i < this.CooldownPresets.Count; i++)
			{
				if (this.CooldownPresets[i].IsDefault)
				{
					this.SetCooldownPreset(i);
					return;
				}
			}
		}

		// Token: 0x0600C036 RID: 49206 RVA: 0x0047417E File Offset: 0x0047237E
		public void GetCooldownMax()
		{
			this.CurrentCooldownPreset = this.CooldownPresets[this.CooldownPresetIndex];
			this.CurrentCooldownPreset.SetupCooldownInfo(this.HighestGameStage, this.LocalPlayer);
			this.SetupBloodMoonData();
		}

		// Token: 0x0600C037 RID: 49207 RVA: 0x004741B4 File Offset: 0x004723B4
		public void AddTwitchActionPreset(TwitchActionPreset preset)
		{
			if (this.ActionPresets == null)
			{
				this.ActionPresets = new List<TwitchActionPreset>();
			}
			this.ActionPresets.Add(preset);
			if (preset.IsDefault)
			{
				this.ActionPresetIndex = this.ActionPresets.Count - 1;
				this.CurrentActionPreset = this.ActionPresets[this.ActionPresetIndex];
			}
		}

		// Token: 0x0600C038 RID: 49208 RVA: 0x00474214 File Offset: 0x00472414
		public void SetTwitchActionPreset(int index)
		{
			if (this.ActionPresetIndex == index)
			{
				return;
			}
			this.ActionPresets[this.ActionPresetIndex].AddedActions.Clear();
			this.ActionPresets[this.ActionPresetIndex].RemovedActions.Clear();
			this.ActionPresetIndex = index;
			this.CurrentActionPreset = this.ActionPresets[this.ActionPresetIndex];
			this.CurrentActionPreset.HandleCooldowns();
			if (this.InitState == TwitchManager.InitStates.Ready)
			{
				this.resetCommandsNeeded = true;
			}
		}

		// Token: 0x0600C039 RID: 49209 RVA: 0x0047429C File Offset: 0x0047249C
		public void SetToDefaultActionPreset()
		{
			for (int i = 0; i < this.ActionPresets.Count; i++)
			{
				if (this.ActionPresets[i].IsDefault)
				{
					this.SetTwitchActionPreset(i);
					return;
				}
			}
		}

		// Token: 0x0600C03A RID: 49210 RVA: 0x004742DC File Offset: 0x004724DC
		public void AddTwitchVotePreset(TwitchVotePreset preset)
		{
			if (this.VotePresets == null)
			{
				this.VotePresets = new List<TwitchVotePreset>();
			}
			this.VotePresets.Add(preset);
			if (preset.IsDefault)
			{
				this.VotePresetIndex = this.VotePresets.Count - 1;
				this.CurrentVotePreset = this.VotePresets[this.VotePresetIndex];
			}
		}

		// Token: 0x0600C03B RID: 49211 RVA: 0x0047433C File Offset: 0x0047253C
		public void SetTwitchVotePreset(int index)
		{
			if (this.VotePresetIndex == index)
			{
				return;
			}
			this.VotePresetIndex = index;
			this.CurrentVotePreset = this.VotePresets[this.VotePresetIndex];
			if (this.CurrentVotePreset.IsEmpty)
			{
				this.VotingManager.ForceEndVote();
			}
			this.SetupAvailableCommands();
		}

		// Token: 0x0600C03C RID: 49212 RVA: 0x00474390 File Offset: 0x00472590
		public void SetToDefaultVotePreset()
		{
			for (int i = 0; i < this.VotePresets.Count; i++)
			{
				if (this.VotePresets[i].IsDefault)
				{
					this.SetTwitchVotePreset(i);
					return;
				}
			}
		}

		// Token: 0x0600C03D RID: 49213 RVA: 0x004743D0 File Offset: 0x004725D0
		public void AddTwitchEventPreset(TwitchEventPreset preset)
		{
			if (this.EventPresets == null)
			{
				this.EventPresets = new List<TwitchEventPreset>();
			}
			this.EventPresets.Add(preset);
			if (preset.IsDefault)
			{
				this.EventPresetIndex = this.EventPresets.Count - 1;
				this.CurrentEventPreset = this.EventPresets[this.EventPresetIndex];
			}
		}

		// Token: 0x0600C03E RID: 49214 RVA: 0x00474430 File Offset: 0x00472630
		public void SetTwitchEventPreset(int index, bool oldAllowChannelPointRedeems)
		{
			TwitchEventPreset currentEventPreset = this.CurrentEventPreset;
			this.EventPresetIndex = index;
			this.CurrentEventPreset = this.EventPresets[this.EventPresetIndex];
			if (currentEventPreset != null)
			{
				currentEventPreset.RemoveChannelPointRedemptions(this.AllowChannelPointRedemptions ? this.CurrentEventPreset : null);
			}
			this.SetupTwitchCommands();
			if (this.AllowChannelPointRedemptions && this.CurrentEventPreset != null)
			{
				this.CurrentEventPreset.AddChannelPointRedemptions();
			}
		}

		// Token: 0x0600C03F RID: 49215 RVA: 0x004744A0 File Offset: 0x004726A0
		public TwitchEventPreset GetEventPreset(string name)
		{
			for (int i = 0; i < this.EventPresets.Count; i++)
			{
				if (this.EventPresets[i].Name.EqualsCaseInsensitive(name))
				{
					return this.EventPresets[i];
				}
			}
			return null;
		}

		// Token: 0x0600C040 RID: 49216 RVA: 0x004744EC File Offset: 0x004726EC
		public void SetToDefaultEventPreset()
		{
			for (int i = 0; i < this.EventPresets.Count; i++)
			{
				if (this.EventPresets[i].IsDefault)
				{
					this.SetTwitchEventPreset(i, this.AllowChannelPointRedemptions);
					return;
				}
			}
		}

		// Token: 0x0600C041 RID: 49217 RVA: 0x00474530 File Offset: 0x00472730
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSub_OnSubscriptionRedeemed(SubscriptionEventBase e)
		{
			if (e.UserName == null)
			{
				return;
			}
			string text = e.UserName.ToLower();
			TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.GetSubTier(e.Tier);
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(text);
			viewerEntry.UserID = StringParsers.ParseSInt32(e.UserId, 0, -1, NumberStyles.Integer);
			int num = this.ViewerData.GetSubTierPoints(subTier) * this.SubPointModifier;
			if (num > 0)
			{
				viewerEntry.SpecialPoints += (float)num;
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_Subscribed, new object[]
				{
					text,
					viewerEntry.CombinedPoints,
					this.GetTierName(subTier),
					num
				}), true);
				string msg = string.Format(this.ingameOutput_Subscribed, text, this.GetTierName(subTier), num);
				this.AddToInGameChatQueue(msg, null);
			}
			SubscriptionMessageEvent subscriptionMessageEvent = e as SubscriptionMessageEvent;
			if (subscriptionMessageEvent != null)
			{
				this.HandleSubEvent(text, subscriptionMessageEvent.CumulativeMonths, subTier);
				return;
			}
			this.HandleSubEvent(text, 1, subTier);
		}

		// Token: 0x0600C042 RID: 49218 RVA: 0x00474634 File Offset: 0x00472834
		public void EventSub_OnSubGifted(SubscriptionGiftEvent e)
		{
			if (e.UserName != null && !e.IsAnonymous)
			{
				this.ViewerData.AddGiftSubEntry(e.UserName.ToLower(), StringParsers.ParseSInt32(e.UserId, 0, -1, NumberStyles.Integer), TwitchSubEventEntry.GetSubTier(e.Tier), e.Total);
			}
		}

		// Token: 0x0600C043 RID: 49219 RVA: 0x00474686 File Offset: 0x00472886
		public string GetTierName(TwitchSubEventEntry.SubTierTypes tier)
		{
			switch (tier)
			{
			case TwitchSubEventEntry.SubTierTypes.Prime:
				return "Prime";
			case TwitchSubEventEntry.SubTierTypes.Tier1:
				return "1";
			case TwitchSubEventEntry.SubTierTypes.Tier2:
				return "2";
			case TwitchSubEventEntry.SubTierTypes.Tier3:
				return "3";
			default:
				return "1";
			}
		}

		// Token: 0x0600C044 RID: 49220 RVA: 0x004746C0 File Offset: 0x004728C0
		public string GetSubTierRewards(int subModifier)
		{
			if (subModifier == 0)
			{
				return Localization.Get("xuiLightPropShadowsNone", false, null);
			}
			return string.Format(this.subPointDisplay, this.ViewerData.GetSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier1) * subModifier, this.ViewerData.GetSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier2) * subModifier, this.ViewerData.GetSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier3) * subModifier);
		}

		// Token: 0x0600C045 RID: 49221 RVA: 0x00474724 File Offset: 0x00472924
		public string GetGiftSubTierRewards(int subModifier)
		{
			if (subModifier == 0)
			{
				return Localization.Get("xuiLightPropShadowsNone", false, null);
			}
			return string.Format(this.subPointDisplay, this.ViewerData.GetGiftSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier1) * subModifier, this.ViewerData.GetGiftSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier2) * subModifier, this.ViewerData.GetGiftSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier3) * subModifier);
		}

		// Token: 0x0600C046 RID: 49222 RVA: 0x00474788 File Offset: 0x00472988
		public void HandleSubEvent(string username, int months, TwitchSubEventEntry.SubTierTypes tier)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchSubEventEntry twitchSubEventEntry = this.CurrentEventPreset.HandleSubEvent(months, tier);
			if (twitchSubEventEntry != null)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(username);
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = username;
				twitchEventActionEntry.Event = twitchSubEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(username, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_SubEvent, twitchSubEventEntry.EventTitle, username, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600C047 RID: 49223 RVA: 0x00474814 File Offset: 0x00472A14
		public void HandleGiftSubEvent(string username, int giftCounts, TwitchSubEventEntry.SubTierTypes tier)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchSubEventEntry twitchSubEventEntry = this.CurrentEventPreset.HandleGiftSubEvent(giftCounts, tier);
			if (twitchSubEventEntry != null)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(username);
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = username;
				twitchEventActionEntry.Event = twitchSubEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(username, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_GiftSubEvent, twitchSubEventEntry.EventTitle, username, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600C048 RID: 49224 RVA: 0x004748A0 File Offset: 0x00472AA0
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSubMessageReceived(JObject payload)
		{
			JToken jtoken = payload["subscription"];
			string text;
			if (jtoken == null)
			{
				text = null;
			}
			else
			{
				JToken jtoken2 = jtoken["type"];
				text = ((jtoken2 != null) ? jtoken2.ToString() : null);
			}
			string text2 = text ?? "unknown";
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num <= 765010691U)
			{
				if (num <= 303371570U)
				{
					if (num != 62361312U)
					{
						if (num == 303371570U)
						{
							if (text2 == "channel.subscribe")
							{
								SubscriptionEvent subscriptionEvent = payload["event"].ToObject<SubscriptionEvent>();
								if (subscriptionEvent != null && !subscriptionEvent.IsGift)
								{
									this.EventSub_OnSubscriptionRedeemed(subscriptionEvent);
									return;
								}
								return;
							}
						}
					}
					else if (text2 == "channel.hype_train.end")
					{
						this.EndHypeTrain();
						return;
					}
				}
				else if (num != 336644660U)
				{
					if (num == 765010691U)
					{
						if (text2 == "channel.bits.use")
						{
							BitsUsedEvent bitsUsedEvent = payload["event"].ToObject<BitsUsedEvent>();
							if (bitsUsedEvent != null)
							{
								this.EventSub_OnBitsRedeemed(bitsUsedEvent);
								return;
							}
							return;
						}
					}
				}
				else if (text2 == "channel.hype_train.begin")
				{
					if (this.HypeTrainLevel == 0)
					{
						this.StartHypeTrain();
						return;
					}
					return;
				}
			}
			else if (num <= 2096046674U)
			{
				if (num != 1906819046U)
				{
					if (num == 2096046674U)
					{
						if (text2 == "channel.raid")
						{
							RaidEvent raidEvent = payload["event"].ToObject<RaidEvent>();
							int userID;
							if (int.TryParse(raidEvent.RaiderID, out userID))
							{
								this.HandleRaid(raidEvent.RaiderUserName.ToLower(), userID, raidEvent.viewerCount);
								return;
							}
							Log.Warning("Failed to run Raid Event because RaiderID could not be parsed as an int");
							return;
						}
					}
				}
				else if (text2 == "channel.subscription.message")
				{
					SubscriptionMessageEvent subscriptionMessageEvent = payload["event"].ToObject<SubscriptionMessageEvent>();
					if (subscriptionMessageEvent != null)
					{
						this.EventSub_OnSubscriptionRedeemed(subscriptionMessageEvent);
						return;
					}
					return;
				}
			}
			else if (num != 2529875513U)
			{
				if (num != 3677527194U)
				{
					if (num == 3760536684U)
					{
						if (text2 == "channel.hype_train.progress")
						{
							HypeTrainProgressEvent hypeTrainProgressEvent = payload["event"].ToObject<HypeTrainProgressEvent>();
							if (this.HypeTrainLevel < hypeTrainProgressEvent.Level)
							{
								this.IncrementHypeTrainLevel();
								return;
							}
							return;
						}
					}
				}
				else if (text2 == "channel.channel_points_custom_reward_redemption.add")
				{
					ChannelPointsRedemptionEvent channelPointsRedemptionEvent = payload["event"].ToObject<ChannelPointsRedemptionEvent>();
					if (channelPointsRedemptionEvent != null)
					{
						this.EventSub_OnChannelPointsRedeemed(channelPointsRedemptionEvent);
						return;
					}
					return;
				}
			}
			else if (text2 == "channel.subscription.gift")
			{
				SubscriptionGiftEvent e = payload["event"].ToObject<SubscriptionGiftEvent>();
				this.EventSub_OnSubGifted(e);
				return;
			}
			Log.Warning("Unhandled event: " + text2);
		}

		// Token: 0x0600C049 RID: 49225 RVA: 0x00474B74 File Offset: 0x00472D74
		[PublicizedFrom(EAccessModifier.Private)]
		public void PubSub_OnBitsRedeemed(object sender, PubSubBitRedemptionMessage.BitRedemptionData e)
		{
			if (e.user_name == null)
			{
				return;
			}
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(e.user_name);
			int num = e.bits_used * this.BitPointModifier;
			viewerEntry.UserID = StringParsers.ParseSInt32(e.user_id, 0, -1, NumberStyles.Integer);
			viewerEntry.SpecialPoints += (float)num;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_DonateBits, new object[]
			{
				e.user_name,
				viewerEntry.CombinedPoints,
				e.bits_used,
				num
			}), true);
			string msg = string.Format(this.ingameOutput_DonateBits, e.user_name, e.bits_used, num);
			this.AddToInGameChatQueue(msg, null);
			this.HandleBitRedeem(e.user_name, e.bits_used, viewerEntry);
		}

		// Token: 0x0600C04A RID: 49226 RVA: 0x00474C58 File Offset: 0x00472E58
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSub_OnBitsRedeemed(BitsUsedEvent e)
		{
			if (e.UserName == null)
			{
				return;
			}
			string text = e.UserName.ToLower();
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(text);
			int num = e.Bits * this.BitPointModifier;
			viewerEntry.UserID = StringParsers.ParseSInt32(e.UserId, 0, -1, NumberStyles.Integer);
			viewerEntry.SpecialPoints += (float)num;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_DonateBits, new object[]
			{
				text,
				viewerEntry.CombinedPoints,
				e.Bits,
				num
			}), true);
			string msg = string.Format(this.ingameOutput_DonateBits, text, e.Bits, num);
			this.AddToInGameChatQueue(msg, null);
			this.HandleBitRedeem(text, e.Bits, viewerEntry);
		}

		// Token: 0x0600C04B RID: 49227 RVA: 0x00474D34 File Offset: 0x00472F34
		public void HandleBitRedeem(string userName, int bitAmount, ViewerEntry viewerEntry = null)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleBitRedeem(bitAmount);
			if (twitchEventEntry != null)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_BitEvent, twitchEventEntry.EventTitle, userName, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600C04C RID: 49228 RVA: 0x00474DC1 File Offset: 0x00472FC1
		[PublicizedFrom(EAccessModifier.Private)]
		public void PubSub_OnChannelPointsRedeemed(object sender, PubSubChannelPointMessage.ChannelRedemptionData e)
		{
			this.HandleChannelPointsRedeem(e.redemption.reward.title, e.redemption.user.display_name.ToLower());
		}

		// Token: 0x0600C04D RID: 49229 RVA: 0x00474DEE File Offset: 0x00472FEE
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSub_OnChannelPointsRedeemed(ChannelPointsRedemptionEvent e)
		{
			this.HandleChannelPointsRedeem(e.Reward.Title, e.UserLogin.ToLower());
		}

		// Token: 0x0600C04E RID: 49230 RVA: 0x00474E0C File Offset: 0x0047300C
		public void HandleChannelPointsRedeem(string title, string userName)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchChannelPointEventEntry twitchChannelPointEventEntry = this.CurrentEventPreset.HandleChannelPointsRedeem(title);
			if (twitchChannelPointEventEntry != null)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(userName);
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchChannelPointEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_ChannelPointEvent, twitchChannelPointEventEntry.EventTitle, userName, viewerEntry.CombinedPoints), true);
				QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.ChannelPointRedeems, twitchChannelPointEventEntry.EventName);
			}
		}

		// Token: 0x0600C04F RID: 49231 RVA: 0x00474EA8 File Offset: 0x004730A8
		public void HandleRaid(string userName, int userID, int viewerAmount)
		{
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(userName);
			if (viewerAmount >= this.RaidViewerMinimum && this.RaidPointAdd > 0)
			{
				viewerEntry.UserID = userID;
				viewerEntry.SpecialPoints += (float)this.RaidPointAdd;
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_RaidPoints, new object[]
				{
					userName,
					viewerEntry.CombinedPoints,
					viewerAmount,
					this.RaidPointAdd
				}), true);
				string msg = string.Format(this.ingameOutput_RaidPoints, userName, viewerAmount, this.RaidPointAdd);
				this.AddToInGameChatQueue(msg, null);
			}
			this.HandleRaidRedeem(userName, viewerAmount, viewerEntry);
		}

		// Token: 0x0600C050 RID: 49232 RVA: 0x00474F6C File Offset: 0x0047316C
		public void HandleRaidRedeem(string userName, int viewerAmount, ViewerEntry viewerEntry = null)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleRaid(viewerAmount);
			if (twitchEventEntry != null)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_RaidEvent, new object[]
				{
					twitchEventEntry.EventTitle,
					userName,
					viewerEntry.CombinedPoints,
					viewerAmount
				}), true);
			}
		}

		// Token: 0x0600C051 RID: 49233 RVA: 0x00475014 File Offset: 0x00473214
		public void HandleCharity(string userName, int userID, int charityAmount)
		{
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(userName);
			int num = charityAmount * this.BitPointModifier;
			viewerEntry.UserID = userID;
			viewerEntry.SpecialPoints += (float)num;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_DonateCharity, new object[]
			{
				userName,
				viewerEntry.CombinedPoints,
				charityAmount,
				num
			}), true);
			string msg = string.Format(this.ingameOutput_DonateCharity, userName, charityAmount, num);
			this.AddToInGameChatQueue(msg, null);
			this.HandleCharityRedeem(userName, charityAmount, viewerEntry);
		}

		// Token: 0x0600C052 RID: 49234 RVA: 0x004750B8 File Offset: 0x004732B8
		public void HandleCharityRedeem(string userName, int charityAmount, ViewerEntry viewerEntry = null)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleCharityRedeem(charityAmount);
			if (twitchEventEntry != null)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_CharityEvent, twitchEventEntry.EventTitle, userName, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600C053 RID: 49235 RVA: 0x00475145 File Offset: 0x00473345
		public void StartHypeTrain()
		{
			this.HypeTrainLevel = 1;
			this.HandleHypeTrainRedeem(this.HypeTrainLevel);
		}

		// Token: 0x0600C054 RID: 49236 RVA: 0x0047515A File Offset: 0x0047335A
		public void IncrementHypeTrainLevel()
		{
			this.HypeTrainLevel++;
			this.HandleHypeTrainRedeem(this.HypeTrainLevel);
		}

		// Token: 0x0600C055 RID: 49237 RVA: 0x00475176 File Offset: 0x00473376
		public void EndHypeTrain()
		{
			this.HypeTrainLevel = 0;
		}

		// Token: 0x0600C056 RID: 49238 RVA: 0x00475180 File Offset: 0x00473380
		public void HandleHypeTrainRedeem(int hypeTrainLevel)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleHypeTrainRedeem(hypeTrainLevel);
			if (twitchEventEntry != null)
			{
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = " ";
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(twitchEventActionEntry.UserName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_HypeTrainEvent, twitchEventEntry.EventTitle, hypeTrainLevel), true);
			}
		}

		// Token: 0x0600C057 RID: 49239 RVA: 0x004751FF File Offset: 0x004733FF
		[PublicizedFrom(EAccessModifier.Private)]
		public void PubSub_OnGoalAchieved(object sender, PubSubGoalMessage.Goal e)
		{
			this.HandleCreatorGoalRedeem(e.contributionType.ToLower());
		}

		// Token: 0x0600C058 RID: 49240 RVA: 0x00475214 File Offset: 0x00473414
		public void HandleCreatorGoalRedeem(string goalType)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchCreatorGoalEventEntry twitchCreatorGoalEventEntry = this.CurrentEventPreset.HandleCreatorGoalEvent(goalType);
			if (twitchCreatorGoalEventEntry != null)
			{
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = " ";
				twitchEventActionEntry.Event = twitchCreatorGoalEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(twitchEventActionEntry.UserName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_CreatorGoalEvent, twitchCreatorGoalEventEntry.EventTitle), true);
			}
		}

		// Token: 0x0600C059 RID: 49241 RVA: 0x00475290 File Offset: 0x00473490
		public void HandleEventQueue()
		{
			if (!this.twitchActive)
			{
				return;
			}
			for (int i = 0; i < this.EventQueue.Count; i++)
			{
				TwitchEventActionEntry twitchEventActionEntry = this.EventQueue[i];
				if (!twitchEventActionEntry.IsSent && twitchEventActionEntry.HandleEvent(this))
				{
					Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_custom_event");
					if (!twitchEventActionEntry.IsRetry)
					{
						TwitchActionHistoryEntry twitchActionHistoryEntry = new TwitchActionHistoryEntry(twitchEventActionEntry.UserName, "FFFFFF", null, null, twitchEventActionEntry);
						twitchActionHistoryEntry.EventEntry = twitchEventActionEntry;
						twitchEventActionEntry.HistoryEntry = twitchActionHistoryEntry;
						this.EventHistory.Insert(0, twitchActionHistoryEntry);
						if (this.EventHistory.Count > 500)
						{
							this.EventHistory.RemoveAt(this.EventHistory.Count - 1);
						}
						if (this.EventHistoryAdded != null)
						{
							this.EventHistoryAdded();
						}
					}
					return;
				}
			}
		}

		// Token: 0x17001799 RID: 6041
		// (get) Token: 0x0600C05A RID: 49242 RVA: 0x00475370 File Offset: 0x00473570
		public bool TwitchActive
		{
			get
			{
				return this.twitchActive;
			}
		}

		// Token: 0x0600C05B RID: 49243 RVA: 0x00475378 File Offset: 0x00473578
		public void SaveExportViewerData(string _filePath)
		{
			this.ViewerData.WriteExport(_filePath);
		}

		// Token: 0x0600C05C RID: 49244 RVA: 0x00475388 File Offset: 0x00473588
		public void LoadExportViewerData(string _filePath)
		{
			using (StreamReader streamReader = File.OpenText(_filePath))
			{
				this.ViewerData.LoadExport(streamReader);
			}
		}

		// Token: 0x0600C05D RID: 49245 RVA: 0x004753C4 File Offset: 0x004735C4
		[PublicizedFrom(EAccessModifier.Private)]
		public int saveViewerDataThreaded(ThreadManager.ThreadInfo _threadInfo)
		{
			PooledExpandableMemoryStream[] array = (PooledExpandableMemoryStream[])_threadInfo.parameter;
			string arg = SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient ? GameIO.GetSaveGameLocalDir() : GameIO.GetSaveGameDir();
			string text = string.Format("{0}/{1}", arg, "twitch.dat");
			if (SdFile.Exists(text))
			{
				SdFile.Copy(text, string.Format("{0}/{1}", arg, "twitch.dat.bak"), true);
			}
			array[0].Position = 0L;
			StreamUtils.WriteStreamToFile(array[0], text);
			MemoryPools.poolMemoryStream.FreeSync(array[0]);
			string arg2 = GameIO.GetUserGameDataDir() + "/Twitch/" + TwitchManager.MainFileVersion.ToString();
			text = string.Format("{0}/{1}", arg2, "twitch_main.dat");
			if (SdFile.Exists(text))
			{
				SdFile.Copy(text, string.Format("{0}/{1}", arg2, "twitch_main.dat.bak"), true);
			}
			array[1].Position = 0L;
			StreamUtils.WriteStreamToFile(array[1], text);
			MemoryPools.poolMemoryStream.FreeSync(array[1]);
			return -1;
		}

		// Token: 0x0600C05E RID: 49246 RVA: 0x004754B4 File Offset: 0x004736B4
		[PublicizedFrom(EAccessModifier.Private)]
		public void LoadViewerData()
		{
			string arg = SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient ? GameIO.GetSaveGameLocalDir() : GameIO.GetSaveGameDir();
			string path = string.Format("{0}/{1}", arg, "twitch.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.Read(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", arg, "twitch.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.Read(pooledBinaryReader2);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600C05F RID: 49247 RVA: 0x004755D0 File Offset: 0x004737D0
		[PublicizedFrom(EAccessModifier.Private)]
		public bool LoadSpecialViewerData(UserDataStorageType _userDataStorage)
		{
			string path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(_userDataStorage), "twitch_special.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.ReadSpecial(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(_userDataStorage), "twitch_special.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.ReadSpecial(pooledBinaryReader2);
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600C060 RID: 49248 RVA: 0x004756DC File Offset: 0x004738DC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool LoadMainViewerData(UserDataStorageType _userDataStorage)
		{
			string path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(_userDataStorage), "twitch_main.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.ReadMain(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(_userDataStorage), "twitch_main.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.ReadMain(pooledBinaryReader2);
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600C061 RID: 49249 RVA: 0x004757E8 File Offset: 0x004739E8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool LoadLatestMainViewerData(UserDataStorageType _userDataStorage)
		{
			for (int i = (int)TwitchManager.MainFileVersion; i >= 2; i--)
			{
				if (this.LoadLatestMainViewerData(_userDataStorage, i))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600C062 RID: 49250 RVA: 0x00475814 File Offset: 0x00473A14
		[PublicizedFrom(EAccessModifier.Private)]
		public bool LoadLatestMainViewerData(UserDataStorageType _userDataStorage, int version)
		{
			string text = GameIO.GetUserGameDataDir(_userDataStorage) + "/Twitch/" + version.ToString();
			if (!SdDirectory.Exists(text))
			{
				SdDirectory.CreateDirectory(text);
			}
			string path = string.Format("{0}/{1}", text, "twitch_main.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.ReadMain(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", text, "twitch_main.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.ReadMain(pooledBinaryReader2);
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600C063 RID: 49251 RVA: 0x00475940 File Offset: 0x00473B40
		[PublicizedFrom(EAccessModifier.Private)]
		public void LoadMainViewerData()
		{
			UserDataStorageType defaultSaveStorage = PlatformManager.MultiPlatform.UserDataRoaming.DefaultSaveStorage;
			if (!this.<LoadMainViewerData>g__LoadViewerDataFromStorage|386_0(defaultSaveStorage))
			{
				UserDataStorageType userDataStorageType = UserDataStorageType.DeviceLocal;
				if (defaultSaveStorage != userDataStorageType && this.<LoadMainViewerData>g__LoadViewerDataFromStorage|386_0(userDataStorageType))
				{
					Log.Out(string.Format("Twitch main viewer data found in {0}", userDataStorageType));
				}
			}
		}

		// Token: 0x0600C064 RID: 49252 RVA: 0x0047598C File Offset: 0x00473B8C
		public void SaveViewerData()
		{
			if (this.dataSaveThreadInfo == null || !ThreadManager.ActiveThreads.ContainsKey("viewerDataSave"))
			{
				PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
					this.Write(pooledBinaryWriter);
				}
				PooledExpandableMemoryStream pooledExpandableMemoryStream2 = MemoryPools.poolMemoryStream.AllocSync(true);
				using (PooledBinaryWriter pooledBinaryWriter2 = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter2.SetBaseStream(pooledExpandableMemoryStream2);
					this.WriteMain(pooledBinaryWriter2);
				}
				this.dataSaveThreadInfo = ThreadManager.StartThread("viewerDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.saveViewerDataThreaded), null, new PooledExpandableMemoryStream[]
				{
					pooledExpandableMemoryStream,
					pooledExpandableMemoryStream2
				}, null, false, true);
			}
		}

		// Token: 0x0600C065 RID: 49253 RVA: 0x00475A64 File Offset: 0x00473C64
		public void Write(BinaryWriter bw)
		{
			bw.Write(TwitchManager.FileVersion);
			bw.Write(this.UseProgression);
			this.ViewerData.Write(bw);
			bw.Write(this.Leaderboard.Count);
			for (int i = 0; i < this.Leaderboard.Count; i++)
			{
				TwitchLeaderboardEntry twitchLeaderboardEntry = this.Leaderboard[i];
				bw.Write(twitchLeaderboardEntry.UserName);
				bw.Write(twitchLeaderboardEntry.Kills);
				bw.Write(twitchLeaderboardEntry.UserColor);
			}
			bw.Write(this.UseActionsDuringBloodmoon);
			bw.Write(this.RewardPot);
			bw.Write(this.ViewerData.PointRate);
			bw.Write(this.CooldownPresetIndex);
			bw.Write((byte)this.PimpPotType);
			bw.Write(this.AllowCrateSharing);
			bw.Write(this.BitPointModifier);
			bw.Write(this.RaidPointAdd);
			bw.Write(this.RaidViewerMinimum);
			bw.Write(this.SubPointModifier);
			bw.Write(this.GiftSubPointModifier);
			bw.Write((byte)this.VotingManager.MaxDailyVotes);
			bw.Write(this.VotingManager.VoteTime);
			bw.Write((byte)this.VotingManager.CurrentVoteDayTimeRange);
			bw.Write(this.VotingManager.ViewerDefeatReward);
			bw.Write(this.VotingManager.AllowVotesDuringBloodmoon);
			bw.Write(this.ViewerData.ActionSpamDelay);
			bw.Write(this.ViewerData.StartingPoints);
			bw.Write(this.UseActionsDuringQuests);
			bw.Write(this.VotingManager.AllowVotesDuringQuests);
			bw.Write(this.VotingManager.AllowVotesInSafeZone);
			bw.Write(this.changedEnabledVoteList.Count);
			for (int j = 0; j < this.changedEnabledVoteList.Count; j++)
			{
				bw.Write(this.changedEnabledVoteList[j]);
				bw.Write(TwitchActionManager.TwitchVotes[this.changedEnabledVoteList[j]].Enabled);
			}
			bw.Write((byte)this.integrationSetting);
			bw.Write(this.EventPresetIndex);
			bw.Write(this.ActionPresetIndex);
			bw.Write(this.VotePresetIndex);
			bw.Write(this.AllowBitEvents);
			bw.Write(this.AllowSubEvents);
			bw.Write(this.AllowGiftSubEvents);
			bw.Write(this.AllowCharityEvents);
			bw.Write(this.AllowRaidEvents);
			bw.Write(this.AllowHypeTrainEvents);
			bw.Write(this.AllowChannelPointRedemptions);
			bw.Write(TwitchManager.LeaderboardStats.GoodRewardTime);
			bw.Write(TwitchManager.LeaderboardStats.GoodRewardAmount);
			int num = 0;
			for (int k = 0; k < this.ActionPresets.Count; k++)
			{
				TwitchActionPreset twitchActionPreset = this.ActionPresets[k];
				if (twitchActionPreset.AddedActions.Count > 0 || twitchActionPreset.RemovedActions.Count > 0)
				{
					num++;
				}
			}
			bw.Write(num);
			for (int l = 0; l < this.ActionPresets.Count; l++)
			{
				TwitchActionPreset twitchActionPreset2 = this.ActionPresets[l];
				if (twitchActionPreset2.AddedActions.Count > 0 || twitchActionPreset2.RemovedActions.Count > 0)
				{
					bw.Write(twitchActionPreset2.Name);
					bw.Write(twitchActionPreset2.AddedActions.Count);
					for (int m = 0; m < twitchActionPreset2.AddedActions.Count; m++)
					{
						bw.Write(twitchActionPreset2.AddedActions[m]);
					}
					bw.Write(twitchActionPreset2.RemovedActions.Count);
					for (int n = 0; n < twitchActionPreset2.RemovedActions.Count; n++)
					{
						bw.Write(twitchActionPreset2.RemovedActions[n]);
					}
				}
			}
			bw.Write(this.bitPriceMultiplier);
			bw.Write(this.AllowCreatorGoalEvents);
			bw.Write(this.changedActionList.Count);
			for (int num2 = 0; num2 < this.changedActionList.Count; num2++)
			{
				bw.Write(this.changedActionList[num2]);
				bw.Write(TwitchActionManager.TwitchActions[this.changedActionList[num2]].ModifiedCost);
			}
			bw.Write(this.BitPot);
			bw.Write(this.BitPotPercentage);
		}

		// Token: 0x0600C066 RID: 49254 RVA: 0x00475EE0 File Offset: 0x004740E0
		public void HandleChangedPropertyList()
		{
			this.changedActionList.Clear();
			this.changedEnabledVoteList.Clear();
			foreach (string text in TwitchActionManager.TwitchActions.Keys)
			{
				TwitchAction twitchAction = TwitchActionManager.TwitchActions[text];
				if (twitchAction.DefaultCost != twitchAction.ModifiedCost)
				{
					this.changedActionList.Add(text);
				}
			}
			foreach (string text2 in TwitchActionManager.TwitchVotes.Keys)
			{
				TwitchVote twitchVote = TwitchActionManager.TwitchVotes[text2];
				if (twitchVote.Enabled != twitchVote.OriginalEnabled)
				{
					this.changedEnabledVoteList.Add(text2);
				}
			}
		}

		// Token: 0x0600C067 RID: 49255 RVA: 0x00475FD8 File Offset: 0x004741D8
		public void WriteSpecial(BinaryWriter bw)
		{
			this.ViewerData.WriteSpecial(bw);
		}

		// Token: 0x0600C068 RID: 49256 RVA: 0x00475FE6 File Offset: 0x004741E6
		public void WriteMain(BinaryWriter bw)
		{
			bw.Write(TwitchManager.MainFileVersion);
			bw.Write(this.HasViewedSettings);
			this.ViewerData.WriteSpecial(bw);
		}

		// Token: 0x0600C069 RID: 49257 RVA: 0x0047600C File Offset: 0x0047420C
		public void Read(BinaryReader br)
		{
			this.CurrentFileVersion = br.ReadByte();
			if (this.CurrentFileVersion > 1)
			{
				this.UseProgression = br.ReadBoolean();
			}
			this.ViewerData.Read(br, this.CurrentFileVersion);
			if (this.CurrentFileVersion > 3)
			{
				int num = br.ReadInt32();
				this.Leaderboard.Clear();
				for (int i = 0; i < num; i++)
				{
					string username;
					int kills;
					string usercolor;
					if (this.CurrentFileVersion > 10)
					{
						username = br.ReadString();
						kills = br.ReadInt32();
						usercolor = br.ReadString();
					}
					else
					{
						username = br.ReadString();
						usercolor = br.ReadString();
						kills = br.ReadInt32();
					}
					this.Leaderboard.Add(new TwitchLeaderboardEntry(username, usercolor, kills));
				}
			}
			if (this.CurrentFileVersion > 4)
			{
				this.UseActionsDuringBloodmoon = br.ReadInt32();
			}
			if (this.CurrentFileVersion > 5)
			{
				this.RewardPot = br.ReadInt32();
				if (this.RewardPot <= 0)
				{
					this.RewardPot = 0;
				}
				if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
				{
					TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
				}
			}
			if (this.CurrentFileVersion > 6)
			{
				this.ViewerData.PointRate = br.ReadSingle();
				this.CooldownPresetIndex = br.ReadInt32();
				if (this.CurrentFileVersion <= 18)
				{
					this.ActionCooldownModifier = br.ReadSingle();
				}
				this.PimpPotType = (TwitchManager.PimpPotSettings)br.ReadByte();
				this.AllowCrateSharing = br.ReadBoolean();
			}
			if (this.CurrentFileVersion > 7)
			{
				if (this.CurrentFileVersion <= 17)
				{
					br.ReadBoolean();
					br.ReadBoolean();
					br.ReadBoolean();
				}
				this.BitPointModifier = br.ReadInt32();
				this.RaidPointAdd = br.ReadInt32();
				this.RaidViewerMinimum = br.ReadInt32();
				this.SubPointModifier = br.ReadInt32();
				this.GiftSubPointModifier = br.ReadInt32();
				if (this.CurrentFileVersion <= 20)
				{
					int num2 = br.ReadInt32();
					this.changedActionList.Clear();
					for (int j = 0; j < num2; j++)
					{
						string text = br.ReadString();
						bool enabled = br.ReadBoolean();
						if (TwitchActionManager.TwitchActions.ContainsKey(text))
						{
							this.changedActionList.Add(text);
							TwitchActionManager.TwitchActions[text].Enabled = enabled;
						}
					}
				}
			}
			if (this.CurrentFileVersion > 8)
			{
				this.VotingManager.MaxDailyVotes = (int)br.ReadByte();
				this.VotingManager.VoteTime = br.ReadSingle();
				this.VotingManager.CurrentVoteDayTimeRange = (int)br.ReadByte();
				this.VotingManager.ViewerDefeatReward = br.ReadInt32();
				this.VotingManager.AllowVotesDuringBloodmoon = br.ReadBoolean();
			}
			if (this.CurrentFileVersion > 9)
			{
				this.ViewerData.ActionSpamDelay = br.ReadSingle();
			}
			if (this.CurrentFileVersion > 10)
			{
				this.ViewerData.StartingPoints = br.ReadInt32();
			}
			if (this.CurrentFileVersion > 11)
			{
				this.UseActionsDuringQuests = br.ReadInt32();
				this.VotingManager.AllowVotesDuringQuests = br.ReadBoolean();
			}
			if (this.CurrentFileVersion > 12)
			{
				this.VotingManager.AllowVotesInSafeZone = br.ReadBoolean();
				if (this.CurrentFileVersion <= 17)
				{
					br.ReadByte();
				}
			}
			if (this.CurrentFileVersion > 13)
			{
				int num3 = br.ReadInt32();
				this.changedEnabledVoteList.Clear();
				for (int k = 0; k < num3; k++)
				{
					string text2 = br.ReadString();
					this.changedEnabledVoteList.Add(text2);
					TwitchActionManager.TwitchVotes[text2].Enabled = br.ReadBoolean();
				}
			}
			if (this.CurrentFileVersion >= 16)
			{
				byte b = br.ReadByte();
				if (b > 1)
				{
					b = 1;
				}
				this.IntegrationSetting = (TwitchManager.IntegrationSettings)b;
			}
			if (this.CurrentFileVersion >= 17)
			{
				this.EventPresetIndex = br.ReadInt32();
				this.CurrentEventPreset = this.EventPresets[this.EventPresetIndex];
			}
			if (this.CurrentFileVersion >= 18)
			{
				this.ActionPresetIndex = br.ReadInt32();
				this.CurrentActionPreset = this.ActionPresets[this.ActionPresetIndex];
				this.VotePresetIndex = br.ReadInt32();
				this.CurrentVotePreset = this.VotePresets[this.VotePresetIndex];
				this.AllowBitEvents = br.ReadBoolean();
				this.AllowSubEvents = br.ReadBoolean();
				this.AllowGiftSubEvents = br.ReadBoolean();
				this.AllowCharityEvents = br.ReadBoolean();
				this.AllowRaidEvents = br.ReadBoolean();
				this.AllowHypeTrainEvents = br.ReadBoolean();
				this.AllowChannelPointRedemptions = br.ReadBoolean();
			}
			if (this.CurrentFileVersion >= 20)
			{
				TwitchManager.LeaderboardStats.GoodRewardTime = br.ReadInt32();
				TwitchManager.LeaderboardStats.GoodRewardAmount = br.ReadInt32();
			}
			if (this.CurrentFileVersion >= 21)
			{
				int num4 = br.ReadInt32();
				for (int l = 0; l < num4; l++)
				{
					string b2 = br.ReadString();
					TwitchActionPreset twitchActionPreset = null;
					for (int m = 0; m < this.ActionPresets.Count; m++)
					{
						if (this.ActionPresets[m].Name == b2)
						{
							twitchActionPreset = this.ActionPresets[m];
							break;
						}
					}
					int num5 = br.ReadInt32();
					if (twitchActionPreset != null)
					{
						twitchActionPreset.AddedActions.Clear();
						twitchActionPreset.RemovedActions.Clear();
					}
					for (int n = 0; n < num5; n++)
					{
						if (twitchActionPreset != null)
						{
							twitchActionPreset.AddedActions.Add(br.ReadString());
						}
					}
					num5 = br.ReadInt32();
					for (int num6 = 0; num6 < num5; num6++)
					{
						if (twitchActionPreset != null)
						{
							twitchActionPreset.RemovedActions.Add(br.ReadString());
						}
					}
				}
			}
			if (this.CurrentFileVersion >= 22)
			{
				this.BitPriceMultiplier = br.ReadSingle();
			}
			if (this.CurrentFileVersion >= 23)
			{
				this.AllowCreatorGoalEvents = br.ReadBoolean();
			}
			if (this.CurrentFileVersion >= 24)
			{
				int num7 = br.ReadInt32();
				this.changedActionList.Clear();
				for (int num8 = 0; num8 < num7; num8++)
				{
					string text3 = br.ReadString();
					int modifiedCost = br.ReadInt32();
					if (TwitchActionManager.TwitchActions.ContainsKey(text3))
					{
						this.changedActionList.Add(text3);
						TwitchActionManager.TwitchActions[text3].ModifiedCost = modifiedCost;
					}
				}
			}
			if (this.CurrentFileVersion >= 25)
			{
				this.BitPot = br.ReadInt32();
				if (this.BitPot <= 0)
				{
					this.BitPot = 0;
				}
				if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
				{
					TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
				}
			}
			if (this.CurrentFileVersion >= 26)
			{
				this.BitPotPercentage = br.ReadSingle();
			}
		}

		// Token: 0x0600C06A RID: 49258 RVA: 0x0047669F File Offset: 0x0047489F
		public void ReadSpecial(BinaryReader br)
		{
			this.ViewerData.ReadSpecial(br, 1);
		}

		// Token: 0x0600C06B RID: 49259 RVA: 0x004766AE File Offset: 0x004748AE
		public void ReadMain(BinaryReader br)
		{
			this.CurrentMainFileVersion = br.ReadByte();
			this.HasViewedSettings = br.ReadBoolean();
			this.ViewerData.ReadSpecial(br, this.CurrentMainFileVersion);
		}

		// Token: 0x0600C06C RID: 49260 RVA: 0x004766DC File Offset: 0x004748DC
		public void SendChannelPointOutputMessage(string name, ViewerEntry entry)
		{
			if (entry.SpecialPoints == 0f)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_PointsWithoutSpecial, name, entry.CombinedPoints), true);
				return;
			}
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_PointsWithSpecial, name, entry.CombinedPoints, entry.SpecialPoints), true);
		}

		// Token: 0x0600C06D RID: 49261 RVA: 0x00476748 File Offset: 0x00474948
		public void SendChannelCreditOutputMessage(string name, ViewerEntry entry)
		{
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_BitCredits, name, entry.BitCredits), true);
		}

		// Token: 0x0600C06E RID: 49262 RVA: 0x0047676D File Offset: 0x0047496D
		public void SendChannelPointOutputMessage(string name)
		{
			this.SendChannelPointOutputMessage(name, this.ViewerData.GetViewerEntry(name));
		}

		// Token: 0x0600C06F RID: 49263 RVA: 0x00476782 File Offset: 0x00474982
		public void SendChannelCreditOutputMessage(string name)
		{
			this.SendChannelCreditOutputMessage(name, this.ViewerData.GetViewerEntry(name));
		}

		// Token: 0x0600C070 RID: 49264 RVA: 0x00476797 File Offset: 0x00474997
		public void SendChannelMessage(string message, bool useQueue = true)
		{
			this.ircClient.SendChannelMessage(message, useQueue);
		}

		// Token: 0x0600C071 RID: 49265 RVA: 0x004767A8 File Offset: 0x004749A8
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateActionCooldowns(float modifier)
		{
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					twitchAction.UpdateModifiedCooldown(modifier);
				}
			}
		}

		// Token: 0x0600C072 RID: 49266 RVA: 0x00476810 File Offset: 0x00474A10
		public void SetupAvailableCommands()
		{
			this.AvailableCommands.Clear();
			this.AlternateCommands.Clear();
			TwitchAction[] array = (from a in TwitchActionManager.TwitchActions.Values
			where a.CanUse && a.IsInPreset(this.CurrentActionPreset)
			orderby a.Command
			orderby a.PointType
			select a).ToArray<TwitchAction>();
			List<string> list = null;
			for (int i = 0; i < array.Count<TwitchAction>(); i++)
			{
				TwitchAction twitchAction = array[i];
				if (this.UseProgression && !this.OverrideProgession)
				{
					int startGameStage = twitchAction.StartGameStage;
					if (startGameStage == -1 || startGameStage <= this.HighestGameStage)
					{
						if (twitchAction.Replaces != "")
						{
							string item = twitchAction.Replaces;
							if (this.AlternateCommands.ContainsKey(twitchAction.Replaces))
							{
								item = this.AlternateCommands[twitchAction.Replaces];
							}
							if (list == null)
							{
								list = new List<string>();
							}
							list.Add(item);
						}
						if (this.AvailableCommands.ContainsKey(twitchAction.Command))
						{
							this.AvailableCommands[twitchAction.Command] = twitchAction;
						}
						else
						{
							this.AvailableCommands.Add(twitchAction.Command, twitchAction);
						}
						if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
						{
							this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
						}
					}
				}
				else
				{
					if (twitchAction.RandomDaily)
					{
						twitchAction.AllowedDay = this.lastGameDay;
					}
					if (twitchAction.Replaces != "")
					{
						string item2 = twitchAction.Replaces;
						if (this.AlternateCommands.ContainsKey(twitchAction.Replaces))
						{
							item2 = this.AlternateCommands[twitchAction.Replaces];
						}
						if (list == null)
						{
							list = new List<string>();
						}
						list.Add(item2);
					}
					if (this.AvailableCommands.ContainsKey(twitchAction.Command))
					{
						this.AvailableCommands[twitchAction.Command] = twitchAction;
					}
					else
					{
						this.AvailableCommands.Add(twitchAction.Command, twitchAction);
					}
					if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
					{
						this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
					}
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					string key = list[j];
					if (this.AvailableCommands.ContainsKey(key))
					{
						this.AvailableCommands.Remove(key);
					}
				}
			}
		}

		// Token: 0x0600C073 RID: 49267 RVA: 0x00476AAC File Offset: 0x00474CAC
		public void SetupAvailableCommandsWithOutput(int lastGameStage, bool displayMessage)
		{
			this.AvailableCommands.Clear();
			this.AlternateCommands.Clear();
			StringBuilder stringBuilder = null;
			TwitchAction[] array = (from a in TwitchActionManager.TwitchActions.Values
			where a.CanUse && a.IsInPreset(this.CurrentActionPreset)
			orderby a.Command
			orderby a.PointType
			select a).ToArray<TwitchAction>();
			List<string> list = null;
			for (int i = 0; i < array.Count<TwitchAction>(); i++)
			{
				TwitchAction twitchAction = array[i];
				if (this.UseProgression && !this.OverrideProgession)
				{
					int startGameStage = twitchAction.StartGameStage;
					if (startGameStage == -1 || startGameStage <= this.HighestGameStage)
					{
						if (twitchAction.Replaces != "")
						{
							string item = twitchAction.Replaces;
							if (this.AlternateCommands.ContainsKey(twitchAction.Replaces))
							{
								item = this.AlternateCommands[twitchAction.Replaces];
							}
							if (list == null)
							{
								list = new List<string>();
							}
							list.Add(item);
						}
						if (this.AvailableCommands.ContainsKey(twitchAction.Command))
						{
							this.AvailableCommands[twitchAction.Command] = twitchAction;
							if (startGameStage > lastGameStage)
							{
								if (stringBuilder == null)
								{
									stringBuilder = new StringBuilder();
									stringBuilder.Append("*" + twitchAction.Command);
								}
								else
								{
									stringBuilder.Append(", " + twitchAction.Command);
								}
							}
						}
						else
						{
							this.AvailableCommands.Add(twitchAction.Command, twitchAction);
							if (startGameStage > lastGameStage)
							{
								if (stringBuilder == null)
								{
									stringBuilder = new StringBuilder();
									stringBuilder.Append(twitchAction.Command);
								}
								else
								{
									stringBuilder.Append(", " + twitchAction.Command);
								}
							}
						}
						if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
						{
							this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
						}
					}
				}
				else
				{
					if (twitchAction.RandomDaily)
					{
						twitchAction.AllowedDay = this.lastGameDay;
					}
					if (this.AvailableCommands.ContainsKey(twitchAction.Command))
					{
						this.AvailableCommands[twitchAction.Command] = twitchAction;
					}
					else
					{
						this.AvailableCommands.Add(twitchAction.Command, twitchAction);
					}
					if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
					{
						this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
					}
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					string key = list[j];
					if (this.AvailableCommands.ContainsKey(key))
					{
						this.AvailableCommands.Remove(key);
					}
				}
			}
			if (displayMessage && stringBuilder != null && this.AllowActions && this.CurrentActionPreset.ShowNewCommands)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_NewActions, stringBuilder), true);
				Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_new_commands");
			}
		}

		// Token: 0x0600C074 RID: 49268 RVA: 0x00476DD4 File Offset: 0x00474FD4
		public void HandleCooldownActionLocking()
		{
			foreach (string key in this.AvailableCommands.Keys)
			{
				TwitchAction twitchAction = this.AvailableCommands[key];
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					if (this.OnCooldown)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time)
						{
							twitchAction.OnCooldown = true;
						}
						else if (this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown)
						{
							twitchAction.OnCooldown = twitchAction.WaitingBlocked;
						}
						else
						{
							twitchAction.OnCooldown = twitchAction.CooldownBlocked;
						}
					}
					else
					{
						twitchAction.OnCooldown = false;
					}
				}
			}
			if (this.CommandsChanged != null)
			{
				this.CommandsChanged();
			}
		}

		// Token: 0x0600C075 RID: 49269 RVA: 0x00476EB0 File Offset: 0x004750B0
		public void PushBalanceToExtensionQueue(string userID, int creditBalance)
		{
			if (this.extensionManager != null)
			{
				this.extensionManager.PushUserBalance(new ValueTuple<string, int>(userID, creditBalance));
			}
		}

		// Token: 0x0600C076 RID: 49270 RVA: 0x00476ECC File Offset: 0x004750CC
		public void DisplayActions()
		{
			int count = this.ActionMessages.Count;
			if (this.CurrentUnityTime > this.nextDisplayCommandsTime)
			{
				this.nextDisplayCommandsTime = this.CurrentUnityTime + 15f;
				this.ircClient.SendChannelMessages(this.ActionMessages, true);
			}
		}

		// Token: 0x0600C077 RID: 49271 RVA: 0x00476F0C File Offset: 0x0047510C
		public void DisplayCommands(bool isBroadcaster, bool isMod, bool isVIP, bool isSub)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.TwitchCommandList.Count; i++)
			{
				if (!(this.TwitchCommandList[i] is TwitchCommandCommands))
				{
					bool flag = false;
					switch (BaseTwitchCommand.GetPermission(this.TwitchCommandList[i]))
					{
					case BaseTwitchCommand.PermissionLevels.Everyone:
						flag = true;
						break;
					case BaseTwitchCommand.PermissionLevels.VIP:
						flag = isVIP;
						break;
					case BaseTwitchCommand.PermissionLevels.Sub:
						flag = isSub;
						break;
					case BaseTwitchCommand.PermissionLevels.Mod:
						flag = isMod;
						break;
					case BaseTwitchCommand.PermissionLevels.Broadcaster:
						flag = isBroadcaster;
						break;
					}
					if (flag)
					{
						for (int j = 0; j < this.TwitchCommandList[i].LocalizedCommandNames.Length; j++)
						{
							if (stringBuilder.Length != 0)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(this.TwitchCommandList[i].LocalizedCommandNames[j]);
						}
					}
				}
			}
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_Commands, stringBuilder.ToString()), true);
		}

		// Token: 0x0600C078 RID: 49272 RVA: 0x00477008 File Offset: 0x00475208
		public void AddTip(string tipname)
		{
			string text = Localization.Get(tipname, false, null);
			string item = Localization.Get(tipname + "Desc", false, null);
			if (text != "")
			{
				this.tipTitleList.Add(text);
				this.tipDescriptionList.Add(item);
			}
		}

		// Token: 0x0600C079 RID: 49273 RVA: 0x00477056 File Offset: 0x00475256
		public void DisplayGameStage()
		{
			if (this.LocalPlayer != null)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_Gamestage, this.LocalPlayer.unModifiedGameStage), true);
			}
		}

		// Token: 0x0600C07A RID: 49274 RVA: 0x0047708D File Offset: 0x0047528D
		public bool CheckIfTwitchKill(EntityPlayer player)
		{
			return this.twitchPlayerDeathsThisFrame.Contains(player);
		}

		// Token: 0x0600C07B RID: 49275 RVA: 0x0047709C File Offset: 0x0047529C
		public bool LiveListContains(int entityID)
		{
			for (int i = 0; i < this.liveList.Count; i++)
			{
				if (this.liveList[i].SpawnedEntityID == entityID)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600C080 RID: 49280 RVA: 0x0047720E File Offset: 0x0047540E
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public bool <LoadMainViewerData>g__LoadViewerDataFromStorage|386_0(UserDataStorageType storageType)
		{
			return this.LoadLatestMainViewerData(storageType) || this.LoadMainViewerData(storageType) || this.LoadSpecialViewerData(storageType);
		}

		// Token: 0x04009072 RID: 36978
		[PublicizedFrom(EAccessModifier.Private)]
		public static TwitchManager instance = null;

		// Token: 0x04009073 RID: 36979
		[PublicizedFrom(EAccessModifier.Private)]
		public const float SAVE_TIME_SEC = 30f;

		// Token: 0x04009074 RID: 36980
		public float saveTime;

		// Token: 0x04009075 RID: 36981
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreadManager.ThreadInfo dataSaveThreadInfo;

		// Token: 0x04009076 RID: 36982
		public static byte FileVersion = 26;

		// Token: 0x04009078 RID: 36984
		public static byte MainFileVersion = 3;

		// Token: 0x0400907A RID: 36986
		public TwitchIRCClient ircClient;

		// Token: 0x0400907B RID: 36987
		public ExtensionManager extensionManager;

		// Token: 0x0400907C RID: 36988
		[PublicizedFrom(EAccessModifier.Private)]
		public int resetClientAttempts;

		// Token: 0x0400907D RID: 36989
		[PublicizedFrom(EAccessModifier.Private)]
		public bool overrideProgression;

		// Token: 0x04009080 RID: 36992
		[PublicizedFrom(EAccessModifier.Private)]
		public int commandsAvailable = -1;

		// Token: 0x04009081 RID: 36993
		public Dictionary<string, TwitchAction> AvailableCommands = new Dictionary<string, TwitchAction>();

		// Token: 0x04009082 RID: 36994
		public Dictionary<string, string> AlternateCommands = new Dictionary<string, string>();

		// Token: 0x04009083 RID: 36995
		public TwitchManager.PimpPotSettings PimpPotType = TwitchManager.PimpPotSettings.EnabledSP;

		// Token: 0x04009084 RID: 36996
		public static int PimpPotDefault = 500;

		// Token: 0x04009085 RID: 36997
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchManager.IntegrationSettings integrationSetting = TwitchManager.IntegrationSettings.Both;

		// Token: 0x04009086 RID: 36998
		[PublicizedFrom(EAccessModifier.Private)]
		public float actionCooldownModifier = 1f;

		// Token: 0x04009087 RID: 36999
		[PublicizedFrom(EAccessModifier.Private)]
		public const int HistoryItemMax = 500;

		// Token: 0x04009088 RID: 37000
		public float ActionPotPercentage = 0.15f;

		// Token: 0x04009089 RID: 37001
		public float BitPotPercentage = 0.25f;

		// Token: 0x0400908A RID: 37002
		public int RewardPot = TwitchManager.PimpPotDefault;

		// Token: 0x0400908B RID: 37003
		public int BitPot;

		// Token: 0x0400908C RID: 37004
		public int PartyKillRewardMax = 250;

		// Token: 0x0400908D RID: 37005
		public EntityPlayerLocal LocalPlayer;

		// Token: 0x0400908E RID: 37006
		public bool LocalPlayerInLandClaim;

		// Token: 0x0400908F RID: 37007
		public TwitchManager.CooldownTypes CooldownType = TwitchManager.CooldownTypes.Startup;

		// Token: 0x04009090 RID: 37008
		public float CooldownTime = 300f;

		// Token: 0x04009091 RID: 37009
		public float CurrentCooldownFill;

		// Token: 0x04009092 RID: 37010
		public float CooldownFillMax = 50f;

		// Token: 0x04009093 RID: 37011
		public int NextCooldownTime = 180;

		// Token: 0x04009094 RID: 37012
		public bool AllowCrateSharing;

		// Token: 0x04009095 RID: 37013
		public bool AllowBitEvents = true;

		// Token: 0x04009096 RID: 37014
		public bool AllowSubEvents = true;

		// Token: 0x04009097 RID: 37015
		public bool AllowGiftSubEvents = true;

		// Token: 0x04009098 RID: 37016
		public bool AllowCharityEvents = true;

		// Token: 0x04009099 RID: 37017
		public bool AllowRaidEvents = true;

		// Token: 0x0400909A RID: 37018
		public bool AllowHypeTrainEvents = true;

		// Token: 0x0400909B RID: 37019
		public bool AllowCreatorGoalEvents = true;

		// Token: 0x0400909C RID: 37020
		public bool AllowChannelPointRedemptions = true;

		// Token: 0x0400909D RID: 37021
		public List<CooldownPreset> CooldownPresets = new List<CooldownPreset>();

		// Token: 0x0400909E RID: 37022
		public int CooldownPresetIndex;

		// Token: 0x0400909F RID: 37023
		public CooldownPreset CurrentCooldownPreset;

		// Token: 0x040090A0 RID: 37024
		public List<TwitchActionPreset> ActionPresets = new List<TwitchActionPreset>();

		// Token: 0x040090A1 RID: 37025
		public List<TwitchVotePreset> VotePresets = new List<TwitchVotePreset>();

		// Token: 0x040090A2 RID: 37026
		public List<TwitchEventPreset> EventPresets = new List<TwitchEventPreset>();

		// Token: 0x040090A3 RID: 37027
		public int ActionPresetIndex;

		// Token: 0x040090A4 RID: 37028
		public int VotePresetIndex;

		// Token: 0x040090A5 RID: 37029
		public int EventPresetIndex;

		// Token: 0x040090A6 RID: 37030
		public TwitchActionPreset CurrentActionPreset;

		// Token: 0x040090A7 RID: 37031
		public TwitchVotePreset CurrentVotePreset;

		// Token: 0x040090A8 RID: 37032
		public TwitchEventPreset CurrentEventPreset;

		// Token: 0x040090A9 RID: 37033
		public bool UIDirty;

		// Token: 0x040090AA RID: 37034
		[PublicizedFrom(EAccessModifier.Private)]
		public float updateTime = 1f;

		// Token: 0x040090AB RID: 37035
		public float ExtensionCheckTime;

		// Token: 0x040090AC RID: 37036
		[PublicizedFrom(EAccessModifier.Private)]
		public World world;

		// Token: 0x040090AD RID: 37037
		public int lastGameDay = -1;

		// Token: 0x040090AE RID: 37038
		public int currentBMDayEnd = -1;

		// Token: 0x040090AF RID: 37039
		public int nextBMDay = -1;

		// Token: 0x040090B0 RID: 37040
		public int BMCooldownStart;

		// Token: 0x040090B1 RID: 37041
		public int BMCooldownEnd;

		// Token: 0x040090B2 RID: 37042
		public int BitPointModifier = 1;

		// Token: 0x040090B3 RID: 37043
		public int SubPointModifier = 1;

		// Token: 0x040090B4 RID: 37044
		public int GiftSubPointModifier = 2;

		// Token: 0x040090B5 RID: 37045
		public int RaidPointAdd = 1000;

		// Token: 0x040090B6 RID: 37046
		public int RaidViewerMinimum = 10;

		// Token: 0x040090B7 RID: 37047
		public int HypeTrainLevel;

		// Token: 0x040090B8 RID: 37048
		[PublicizedFrom(EAccessModifier.Private)]
		public float bitPriceMultiplier = 1f;

		// Token: 0x040090B9 RID: 37049
		public static TwitchLeaderboardStats LeaderboardStats = new TwitchLeaderboardStats();

		// Token: 0x040090BA RID: 37050
		public bool isBMActive;

		// Token: 0x040090BB RID: 37051
		public TwitchVoteLockTypes VoteLockedLevel;

		// Token: 0x040090BC RID: 37052
		public List<TwitchActionHistoryEntry> ActionHistory = new List<TwitchActionHistoryEntry>();

		// Token: 0x040090BD RID: 37053
		public List<TwitchActionHistoryEntry> VoteHistory = new List<TwitchActionHistoryEntry>();

		// Token: 0x040090BE RID: 37054
		public List<TwitchActionHistoryEntry> EventHistory = new List<TwitchActionHistoryEntry>();

		// Token: 0x040090BF RID: 37055
		public List<TwitchLeaderboardEntry> Leaderboard = new List<TwitchLeaderboardEntry>();

		// Token: 0x040090C0 RID: 37056
		public List<TwitchRespawnEntry> RespawnEntries = new List<TwitchRespawnEntry>();

		// Token: 0x040090C1 RID: 37057
		public int UseActionsDuringBloodmoon = 2;

		// Token: 0x040090C2 RID: 37058
		public int UseActionsDuringQuests = 2;

		// Token: 0x040090C3 RID: 37059
		public bool InitialCooldownSet;

		// Token: 0x040090C6 RID: 37062
		public List<EntityPlayer> twitchPlayerDeathsThisFrame = new List<EntityPlayer>();

		// Token: 0x040090C7 RID: 37063
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchManager.InitStates initState;

		// Token: 0x040090C8 RID: 37064
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isLoaded;

		// Token: 0x040090C9 RID: 37065
		public XUi LocalPlayerXUi;

		// Token: 0x040090CA RID: 37066
		public float CurrentUnityTime;

		// Token: 0x040090CB RID: 37067
		[PublicizedFrom(EAccessModifier.Private)]
		public bool resetCommandsNeeded;

		// Token: 0x040090CC RID: 37068
		[PublicizedFrom(EAccessModifier.Private)]
		public bool respawnEventNeeded;

		// Token: 0x040090CD RID: 37069
		[PublicizedFrom(EAccessModifier.Private)]
		public bool checkingExtensionInstalled;

		// Token: 0x040090CE RID: 37070
		[PublicizedFrom(EAccessModifier.Private)]
		public string broadcasterType = "";

		// Token: 0x040090CF RID: 37071
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchMessageEntry> inGameChatQueue = new List<TwitchMessageEntry>();

		// Token: 0x040090D6 RID: 37078
		public TwitchAuthentication Authentication;

		// Token: 0x040090D7 RID: 37079
		public EventSubClient EventSub;

		// Token: 0x040090D8 RID: 37080
		public bool HasViewedSettings;

		// Token: 0x040090D9 RID: 37081
		public string DeniedCrateEvent = "";

		// Token: 0x040090DA RID: 37082
		public string StealingCrateEvent = "";

		// Token: 0x040090DB RID: 37083
		public string PartyRespawnEvent = "";

		// Token: 0x040090DC RID: 37084
		public string OnPlayerDeathEvent = "";

		// Token: 0x040090DD RID: 37085
		public string OnPlayerRespawnEvent = "";

		// Token: 0x040090DE RID: 37086
		public List<string> tipTitleList = new List<string>();

		// Token: 0x040090DF RID: 37087
		public List<string> tipDescriptionList = new List<string>();

		// Token: 0x040090E0 RID: 37088
		[PublicizedFrom(EAccessModifier.Private)]
		public int extensionActiveCheckFailures;

		// Token: 0x040090E1 RID: 37089
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ActivatedAction;

		// Token: 0x040090E2 RID: 37090
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ActivatedBitAction;

		// Token: 0x040090E3 RID: 37091
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_BitCredits;

		// Token: 0x040090E4 RID: 37092
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_BitEvent;

		// Token: 0x040090E5 RID: 37093
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_BitPotBalance;

		// Token: 0x040090E6 RID: 37094
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ChannelPointEvent;

		// Token: 0x040090E7 RID: 37095
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CharityEvent;

		// Token: 0x040090E8 RID: 37096
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CooldownComplete;

		// Token: 0x040090E9 RID: 37097
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CooldownStarted;

		// Token: 0x040090EA RID: 37098
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CooldownTime;

		// Token: 0x040090EB RID: 37099
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_Commands;

		// Token: 0x040090EC RID: 37100
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CreatorGoalEvent;

		// Token: 0x040090ED RID: 37101
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_DonateBits;

		// Token: 0x040090EE RID: 37102
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_DonateCharity;

		// Token: 0x040090EF RID: 37103
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_Gamestage;

		// Token: 0x040090F0 RID: 37104
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_GiftSubEvent;

		// Token: 0x040090F1 RID: 37105
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_GiftSubs;

		// Token: 0x040090F2 RID: 37106
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_HypeTrainEvent;

		// Token: 0x040090F3 RID: 37107
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledParty;

		// Token: 0x040090F4 RID: 37108
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledStreamer;

		// Token: 0x040090F5 RID: 37109
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledByBits;

		// Token: 0x040090F6 RID: 37110
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledByHypeTrain;

		// Token: 0x040090F7 RID: 37111
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledByVote;

		// Token: 0x040090F8 RID: 37112
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_NewActions;

		// Token: 0x040090F9 RID: 37113
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_PimpPotBalance;

		// Token: 0x040090FA RID: 37114
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_PointsWithSpecial;

		// Token: 0x040090FB RID: 37115
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_PointsWithoutSpecial;

		// Token: 0x040090FC RID: 37116
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_QueuedBitAction;

		// Token: 0x040090FD RID: 37117
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_RaidEvent;

		// Token: 0x040090FE RID: 37118
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_RaidPoints;

		// Token: 0x040090FF RID: 37119
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_SubEvent;

		// Token: 0x04009100 RID: 37120
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_Subscribed;

		// Token: 0x04009101 RID: 37121
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_ActivatedAction;

		// Token: 0x04009102 RID: 37122
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_BitRespawns;

		// Token: 0x04009103 RID: 37123
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_DonateBits;

		// Token: 0x04009104 RID: 37124
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_DonateCharity;

		// Token: 0x04009105 RID: 37125
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_GiftSubs;

		// Token: 0x04009106 RID: 37126
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledParty;

		// Token: 0x04009107 RID: 37127
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledStreamer;

		// Token: 0x04009108 RID: 37128
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledByBits;

		// Token: 0x04009109 RID: 37129
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledByHypeTrain;

		// Token: 0x0400910A RID: 37130
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledByVote;

		// Token: 0x0400910B RID: 37131
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_RaidPoints;

		// Token: 0x0400910C RID: 37132
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_RefundedAction;

		// Token: 0x0400910D RID: 37133
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_Subscribed;

		// Token: 0x0400910E RID: 37134
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameDeathScreen_Message;

		// Token: 0x0400910F RID: 37135
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameBitsDeathScreen_Message;

		// Token: 0x04009110 RID: 37136
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameHypeTrainDeathScreen_Message;

		// Token: 0x04009111 RID: 37137
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameVoteDeathScreen_Message;

		// Token: 0x04009112 RID: 37138
		[PublicizedFrom(EAccessModifier.Private)]
		public string subPointDisplay;

		// Token: 0x04009113 RID: 37139
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, TwitchRandomActionGroup> randomGroups = new Dictionary<string, TwitchRandomActionGroup>();

		// Token: 0x04009114 RID: 37140
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, List<TwitchAction>> randomKeys = new Dictionary<string, List<TwitchAction>>();

		// Token: 0x04009115 RID: 37141
		public List<BaseTwitchCommand> TwitchCommandList = new List<BaseTwitchCommand>();

		// Token: 0x04009116 RID: 37142
		[PublicizedFrom(EAccessModifier.Protected)]
		public Dictionary<EntityPlayer, TwitchManager.TwitchPartyMemberInfo> PartyInfo = new Dictionary<EntityPlayer, TwitchManager.TwitchPartyMemberInfo>();

		// Token: 0x04009117 RID: 37143
		[PublicizedFrom(EAccessModifier.Private)]
		public bool lastAlive;

		// Token: 0x04009118 RID: 37144
		public static string DeathText = "";

		// Token: 0x04009119 RID: 37145
		[PublicizedFrom(EAccessModifier.Private)]
		public bool twitchActive = true;

		// Token: 0x0400911A RID: 37146
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchActionEntry> QueuedActionEntries = new List<TwitchActionEntry>();

		// Token: 0x0400911B RID: 37147
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchActionEntry> LiveActionEntries = new List<TwitchActionEntry>();

		// Token: 0x0400911C RID: 37148
		public List<TwitchEventActionEntry> EventQueue = new List<TwitchEventActionEntry>();

		// Token: 0x0400911D RID: 37149
		public List<TwitchEventActionEntry> LiveEvents = new List<TwitchEventActionEntry>();

		// Token: 0x0400911E RID: 37150
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchSpawnedEntityEntry> liveList = new List<TwitchSpawnedEntityEntry>();

		// Token: 0x0400911F RID: 37151
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchSpawnedBlocksEntry> liveBlockList = new List<TwitchSpawnedBlocksEntry>();

		// Token: 0x04009120 RID: 37152
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchRecentlyRemovedEntityEntry> recentlyDeadList = new List<TwitchRecentlyRemovedEntityEntry>();

		// Token: 0x04009121 RID: 37153
		public List<TwitchSpawnedEntityEntry> actionSpawnLiveList = new List<TwitchSpawnedEntityEntry>();

		// Token: 0x04009122 RID: 37154
		public bool HasDataChanges;

		// Token: 0x04009123 RID: 37155
		public List<string> changedActionList = new List<string>();

		// Token: 0x04009124 RID: 37156
		public List<string> changedEnabledVoteList = new List<string>();

		// Token: 0x04009125 RID: 37157
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> ActionMessages = new List<string>();

		// Token: 0x04009126 RID: 37158
		[PublicizedFrom(EAccessModifier.Private)]
		public float nextDisplayCommandsTime;

		// Token: 0x0200184D RID: 6221
		public enum PimpPotSettings
		{
			// Token: 0x04009128 RID: 37160
			Disabled,
			// Token: 0x04009129 RID: 37161
			EnabledSP,
			// Token: 0x0400912A RID: 37162
			EnabledPP
		}

		// Token: 0x0200184E RID: 6222
		public enum IntegrationSettings
		{
			// Token: 0x0400912C RID: 37164
			ExtensionOnly,
			// Token: 0x0400912D RID: 37165
			Both
		}

		// Token: 0x0200184F RID: 6223
		public enum CooldownTypes
		{
			// Token: 0x0400912F RID: 37167
			None,
			// Token: 0x04009130 RID: 37168
			Startup,
			// Token: 0x04009131 RID: 37169
			Time,
			// Token: 0x04009132 RID: 37170
			MaxReached,
			// Token: 0x04009133 RID: 37171
			MaxReachedWaiting,
			// Token: 0x04009134 RID: 37172
			BloodMoonDisabled,
			// Token: 0x04009135 RID: 37173
			BloodMoonCooldown,
			// Token: 0x04009136 RID: 37174
			QuestDisabled,
			// Token: 0x04009137 RID: 37175
			QuestCooldown,
			// Token: 0x04009138 RID: 37176
			SafeCooldown,
			// Token: 0x04009139 RID: 37177
			SafeCooldownExit
		}

		// Token: 0x02001850 RID: 6224
		public enum InitStates
		{
			// Token: 0x0400913B RID: 37179
			Setup,
			// Token: 0x0400913C RID: 37180
			None,
			// Token: 0x0400913D RID: 37181
			WaitingForPermission,
			// Token: 0x0400913E RID: 37182
			PermissionDenied,
			// Token: 0x0400913F RID: 37183
			WaitingForOAuth,
			// Token: 0x04009140 RID: 37184
			Authenticating,
			// Token: 0x04009141 RID: 37185
			Authenticated,
			// Token: 0x04009142 RID: 37186
			CheckingForExtension,
			// Token: 0x04009143 RID: 37187
			Ready,
			// Token: 0x04009144 RID: 37188
			ExtensionNotInstalled,
			// Token: 0x04009145 RID: 37189
			Failed
		}

		// Token: 0x02001851 RID: 6225
		public class TwitchPartyMemberInfo
		{
			// Token: 0x04009146 RID: 37190
			public bool LastOptedOut;

			// Token: 0x04009147 RID: 37191
			public bool LastAlive = true;

			// Token: 0x04009148 RID: 37192
			public float Cooldown;

			// Token: 0x04009149 RID: 37193
			public bool NeedsRespawnEvent;
		}
	}
}
