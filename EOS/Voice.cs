using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.RTC;
using Epic.OnlineServices.RTCAudio;
using UnityEngine;

namespace Platform.EOS
{
	// Token: 0x02001D33 RID: 7475
	public class Voice : IPartyVoice
	{
		// Token: 0x17001B9C RID: 7068
		// (get) Token: 0x0600DD64 RID: 56676 RVA: 0x004F5BF7 File Offset: 0x004F3DF7
		public UserIdentifierEos localUserIdentifier
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				IPlatform platform = this.owner;
				object obj;
				if (platform == null)
				{
					obj = null;
				}
				else
				{
					IUserClient user = platform.User;
					obj = ((user != null) ? user.PlatformUserId : null);
				}
				return (UserIdentifierEos)obj;
			}
		}

		// Token: 0x17001B9D RID: 7069
		// (get) Token: 0x0600DD65 RID: 56677 RVA: 0x004F5C1C File Offset: 0x004F3E1C
		public ProductUserId localProductUserId
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				UserIdentifierEos localUserIdentifier = this.localUserIdentifier;
				if (localUserIdentifier == null)
				{
					return null;
				}
				return localUserIdentifier.ProductUserId;
			}
		}

		// Token: 0x14000144 RID: 324
		// (add) Token: 0x0600DD66 RID: 56678 RVA: 0x004F5C30 File Offset: 0x004F3E30
		// (remove) Token: 0x0600DD67 RID: 56679 RVA: 0x004F5C90 File Offset: 0x004F3E90
		public event Action Initialized
		{
			add
			{
				object obj = this.initializedDelegateLock;
				lock (obj)
				{
					this.initializedDelegates = (Action)Delegate.Combine(this.initializedDelegates, value);
					if (this.Status == EPartyVoiceStatus.Ok)
					{
						value();
					}
				}
			}
			remove
			{
				object obj = this.initializedDelegateLock;
				lock (obj)
				{
					this.initializedDelegates = (Action)Delegate.Remove(this.initializedDelegates, value);
				}
			}
		}

		// Token: 0x17001B9E RID: 7070
		// (get) Token: 0x0600DD68 RID: 56680 RVA: 0x004F5CE4 File Offset: 0x004F3EE4
		// (set) Token: 0x0600DD69 RID: 56681 RVA: 0x004F5CEC File Offset: 0x004F3EEC
		public EPartyVoiceStatus Status { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EPartyVoiceStatus.Uninitialized;

		// Token: 0x17001B9F RID: 7071
		// (get) Token: 0x0600DD6A RID: 56682 RVA: 0x004F5CF5 File Offset: 0x004F3EF5
		public bool InLobby
		{
			get
			{
				return this.lobbyId != null && this.roomEntered;
			}
		}

		// Token: 0x17001BA0 RID: 7072
		// (get) Token: 0x0600DD6B RID: 56683 RVA: 0x004F5D07 File Offset: 0x004F3F07
		public bool InLobbyOrProgress
		{
			get
			{
				return this.InLobby || this.createInProgress || this.joinInProgress;
			}
		}

		// Token: 0x0600DD6C RID: 56684 RVA: 0x004F5D21 File Offset: 0x004F3F21
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.api = (Api)this.owner.Api;
			this.api.ClientApiInitialized += this.OnClientApiInitialized;
		}

		// Token: 0x0600DD6D RID: 56685 RVA: 0x004F5D58 File Offset: 0x004F3F58
		public void Destroy()
		{
			if (this.Status == EPartyVoiceStatus.Ok)
			{
				this.OnPartyVoiceUninitialize();
			}
			this.Status = EPartyVoiceStatus.Uninitialized;
			this.lobbyInterface = null;
			this.audioInterface = null;
			this.rtcInterface = null;
			this.api.ClientApiInitialized -= this.OnClientApiInitialized;
			this.api = null;
			this.owner = null;
		}

		// Token: 0x0600DD6E RID: 56686 RVA: 0x004F5DB4 File Offset: 0x004F3FB4
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientApiInitialized()
		{
			PlatformInterface platformInterface = this.api.PlatformInterface;
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.rtcInterface = ((platformInterface != null) ? platformInterface.GetRTCInterface() : null);
				RTCInterface rtcinterface = this.rtcInterface;
				this.audioInterface = ((rtcinterface != null) ? rtcinterface.GetAudioInterface() : null);
				this.lobbyInterface = ((platformInterface != null) ? platformInterface.GetLobbyInterface() : null);
			}
			lockObject = this.initializedDelegateLock;
			lock (lockObject)
			{
				if (this.rtcInterface != null && this.audioInterface != null && this.lobbyInterface != null)
				{
					this.Status = EPartyVoiceStatus.Ok;
					this.OnPartyVoiceInitialized();
					Log.Out("[EOS-Voice] Successfully initialized.");
					Action action = this.initializedDelegates;
					if (action != null)
					{
						action();
					}
				}
				else
				{
					this.Status = EPartyVoiceStatus.PermanentError;
					Log.Warning("[EOS-Voice] Failed to initialize.");
				}
			}
		}

		// Token: 0x0600DD6F RID: 56687 RVA: 0x004F5EC4 File Offset: 0x004F40C4
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPartyVoiceInitialized()
		{
			this.AddNotifications();
		}

		// Token: 0x0600DD70 RID: 56688 RVA: 0x004F5ECC File Offset: 0x004F40CC
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPartyVoiceUninitialize()
		{
			this.RemoveNotifications();
		}

		// Token: 0x0600DD71 RID: 56689 RVA: 0x004F5ED4 File Offset: 0x004F40D4
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddNotifications()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not add notifications because voice is currently not ready.");
				return;
			}
			this.AddAudioDevicesNotifications();
		}

		// Token: 0x0600DD72 RID: 56690 RVA: 0x004F5EEF File Offset: 0x004F40EF
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveNotifications()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not remove notifications because voice is currently not ready.");
				return;
			}
			this.RemoveAudioDevicesNotifications();
		}

		// Token: 0x0600DD73 RID: 56691 RVA: 0x004F5F0C File Offset: 0x004F410C
		public void CreateLobby(Action<string> _lobbyCreatedCallback)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not create lobby because voice is currently not valid.");
				return;
			}
			if (this.lobbyId != null)
			{
				Log.Error("[EOS-Voice] Can not create lobby while already in another");
				Log.Error(StackTraceUtility.ExtractStackTrace());
				return;
			}
			this.createInProgress = true;
			Log.Out("[EOS-Voice] Creating lobby");
			EosHelpers.AssertMainThread("Voice.Create");
			CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
			{
				LocalUserId = this.localProductUserId,
				MaxLobbyMembers = 8U,
				PermissionLevel = LobbyPermissionLevel.Joinviapresence,
				PresenceEnabled = false,
				AllowInvites = false,
				EnableRTCRoom = true,
				BucketId = "PartyVoice",
				LocalRTCOptions = new LocalRTCOptions?(new LocalRTCOptions
				{
					LocalAudioDeviceInputStartsMuted = true
				})
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.CreateLobby(ref createLobbyOptions, _lobbyCreatedCallback, delegate(ref CreateLobbyCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Create lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						this.createInProgress = false;
						Action<string> action = (Action<string>)_callbackData.ClientData;
						if (action == null)
						{
							return;
						}
						action(null);
						return;
					}
					else
					{
						this.lobbyEntered(_callbackData.LobbyId);
						Action<string> action2 = (Action<string>)_callbackData.ClientData;
						if (action2 == null)
						{
							return;
						}
						action2(this.lobbyId);
						return;
					}
				});
			}
		}

		// Token: 0x0600DD74 RID: 56692 RVA: 0x004F601C File Offset: 0x004F421C
		public void JoinLobby(string _lobbyId)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not join lobby because voice is currently not ready.");
				return;
			}
			if (string.IsNullOrEmpty(_lobbyId))
			{
				Log.Error("[EOS-Voice] Can not join lobby, missing id");
				return;
			}
			if (this.lobbyId != null)
			{
				if (this.lobbyId != _lobbyId)
				{
					Log.Error("[EOS-Voice] Can not join lobby while already in another");
				}
				return;
			}
			Log.Out("[EOS-Voice] Joining lobby");
			this.joinInProgress = true;
			ThreadManager.StartCoroutine(this.tryJoinLobbyCo(_lobbyId));
		}

		// Token: 0x0600DD75 RID: 56693 RVA: 0x004F6090 File Offset: 0x004F4290
		public void LeaveLobby()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not leave lobby because voice is currently not ready.");
				return;
			}
			if (this.lobbyId == null)
			{
				return;
			}
			if (this.leaveInProgress)
			{
				return;
			}
			this.leaveInProgress = true;
			Log.Out("[EOS-Voice] Leaving lobby");
			EosHelpers.AssertMainThread("Voice.Leave");
			LeaveLobbyOptions leaveLobbyOptions = new LeaveLobbyOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.LeaveLobby(ref leaveLobbyOptions, null, delegate(ref LeaveLobbyCallbackInfo _callbackData)
				{
					this.lobbyLeft();
					this.leaveInProgress = false;
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Leave lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
					}
				});
			}
		}

		// Token: 0x0600DD76 RID: 56694 RVA: 0x004F614C File Offset: 0x004F434C
		public void PromoteLeader(PlatformUserIdentifierAbs _newLeaderIdentifier)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not promote leader because voice is currently not ready.");
				return;
			}
			if (!this.IsLobbyOwner())
			{
				return;
			}
			UserIdentifierEos userIdentifierEos = _newLeaderIdentifier as UserIdentifierEos;
			if (userIdentifierEos == null)
			{
				Log.Error(string.Format("[EOS-Voice] New leader user identifier is not an EOS identifier: {0}", _newLeaderIdentifier));
				return;
			}
			Log.Out("[EOS-Voice] Promoting lobby owner");
			EosHelpers.AssertMainThread("Voice.Prom");
			PromoteMemberOptions promoteMemberOptions = new PromoteMemberOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId,
				TargetUserId = userIdentifierEos.ProductUserId
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.PromoteMember(ref promoteMemberOptions, null, delegate(ref PromoteMemberCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Promoting leader failed: " + _callbackData.ResultCode.ToStringCached<Result>());
					}
				});
			}
		}

		// Token: 0x0600DD77 RID: 56695 RVA: 0x004F6238 File Offset: 0x004F4438
		public bool IsLobbyOwner()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not check if lobby owner because voice is currently not ready.");
				return false;
			}
			if (this.lobbyId == null)
			{
				return false;
			}
			EosHelpers.AssertMainThread("Voice.Own");
			CopyLobbyDetailsHandleOptions copyLobbyDetailsHandleOptions = new CopyLobbyDetailsHandleOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId
			};
			object lockObject = AntiCheatCommon.LockObject;
			LobbyDetails lobbyDetails;
			Result result;
			lock (lockObject)
			{
				result = this.lobbyInterface.CopyLobbyDetailsHandle(ref copyLobbyDetailsHandleOptions, out lobbyDetails);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-Voice] Getting local lobby details failed: " + result.ToStringCached<Result>());
				return false;
			}
			LobbyDetailsGetLobbyOwnerOptions lobbyDetailsGetLobbyOwnerOptions = default(LobbyDetailsGetLobbyOwnerOptions);
			lockObject = AntiCheatCommon.LockObject;
			ProductUserId lobbyOwner;
			lock (lockObject)
			{
				lobbyOwner = lobbyDetails.GetLobbyOwner(ref lobbyDetailsGetLobbyOwnerOptions);
			}
			bool result2 = lobbyOwner == this.localProductUserId;
			lobbyDetails.Release();
			return result2;
		}

		// Token: 0x14000145 RID: 325
		// (add) Token: 0x0600DD78 RID: 56696 RVA: 0x004F6344 File Offset: 0x004F4544
		// (remove) Token: 0x0600DD79 RID: 56697 RVA: 0x004F637C File Offset: 0x004F457C
		public event Action<IPartyVoice.EVoiceChannelAction> OnLocalPlayerStateChanged;

		// Token: 0x14000146 RID: 326
		// (add) Token: 0x0600DD7A RID: 56698 RVA: 0x004F63B4 File Offset: 0x004F45B4
		// (remove) Token: 0x0600DD7B RID: 56699 RVA: 0x004F63EC File Offset: 0x004F45EC
		public event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceChannelAction> OnRemotePlayerStateChanged;

		// Token: 0x14000147 RID: 327
		// (add) Token: 0x0600DD7C RID: 56700 RVA: 0x004F6424 File Offset: 0x004F4624
		// (remove) Token: 0x0600DD7D RID: 56701 RVA: 0x004F645C File Offset: 0x004F465C
		public event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> OnRemotePlayerVoiceStateChanged;

		// Token: 0x0600DD7E RID: 56702 RVA: 0x004F6494 File Offset: 0x004F4694
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddAudioDevicesNotifications()
		{
			if (this.audioDevicesChangedNotificationId == 0UL)
			{
				this.RefreshAudioDevices();
				AddNotifyAudioDevicesChangedOptions addNotifyAudioDevicesChangedOptions = default(AddNotifyAudioDevicesChangedOptions);
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioDevicesChangedNotificationId = this.audioInterface.AddNotifyAudioDevicesChanged(ref addNotifyAudioDevicesChangedOptions, null, new OnAudioDevicesChangedCallback(this.<AddAudioDevicesNotifications>g__OnAudioDevicesChanged|60_0));
				}
			}
		}

		// Token: 0x0600DD7F RID: 56703 RVA: 0x004F6504 File Offset: 0x004F4704
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveAudioDevicesNotifications()
		{
			if (this.audioDevicesChangedNotificationId != 0UL)
			{
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					RTCAudioInterface rtcaudioInterface = this.audioInterface;
					if (rtcaudioInterface != null)
					{
						rtcaudioInterface.RemoveNotifyAudioDevicesChanged(this.audioDevicesChangedNotificationId);
					}
				}
				this.audioDevicesChangedNotificationId = 0UL;
			}
		}

		// Token: 0x0600DD80 RID: 56704 RVA: 0x004F6564 File Offset: 0x004F4764
		[PublicizedFrom(EAccessModifier.Private)]
		public void RefreshAudioDevices()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not refresh audio devices because voice is currently not ready.");
				return;
			}
			QueryInputDevicesInformationOptions queryInputDevicesInformationOptions = default(QueryInputDevicesInformationOptions);
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.QueryInputDevicesInformation(ref queryInputDevicesInformationOptions, null, new OnQueryInputDevicesInformationCallback(this.OnQueryInputDevicesInformation));
			}
			QueryOutputDevicesInformationOptions queryOutputDevicesInformationOptions = default(QueryOutputDevicesInformationOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.QueryOutputDevicesInformation(ref queryOutputDevicesInformationOptions, null, new OnQueryOutputDevicesInformationCallback(this.OnQueryOutputDevicesInformation));
			}
		}

		// Token: 0x0600DD81 RID: 56705 RVA: 0x004F661C File Offset: 0x004F481C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnQueryInputDevicesInformation(ref OnQueryInputDevicesInformationCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Log.Error("[EOS-Voice] Query Input Devices Information Failed: " + data.ResultCode.ToStringCached<Result>());
				return;
			}
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Warning(string.Format("[EOS-Voice] can not query input devices information. Voice status: {0}", this.Status));
				return;
			}
			GetInputDevicesCountOptions getInputDevicesCountOptions = default(GetInputDevicesCountOptions);
			object lockObject = AntiCheatCommon.LockObject;
			uint inputDevicesCount;
			lock (lockObject)
			{
				inputDevicesCount = this.audioInterface.GetInputDevicesCount(ref getInputDevicesCountOptions);
			}
			IList<IPartyVoice.VoiceAudioDevice> list = new List<IPartyVoice.VoiceAudioDevice>();
			for (uint num = 0U; num < inputDevicesCount; num += 1U)
			{
				CopyInputDeviceInformationByIndexOptions copyInputDeviceInformationByIndexOptions = new CopyInputDeviceInformationByIndexOptions
				{
					DeviceIndex = num
				};
				lockObject = AntiCheatCommon.LockObject;
				InputDeviceInformation? inputDeviceInformation;
				Result result;
				lock (lockObject)
				{
					result = this.audioInterface.CopyInputDeviceInformationByIndex(ref copyInputDeviceInformationByIndexOptions, out inputDeviceInformation);
				}
				try
				{
					if (result != Result.Success || inputDeviceInformation == null)
					{
						Log.Warning(string.Format("[EOS-Voice] Could not query input device {0}: {1}", num, result.ToStringCached<Result>()));
					}
					else
					{
						list.Add(new Voice.EosAudioDevice(inputDeviceInformation.Value));
					}
				}
				finally
				{
					bool flag2 = inputDeviceInformation != null;
				}
			}
			this.inputDevices = list;
		}

		// Token: 0x0600DD82 RID: 56706 RVA: 0x004F677C File Offset: 0x004F497C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnQueryOutputDevicesInformation(ref OnQueryOutputDevicesInformationCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Log.Error("[EOS-Voice] Query Output Devices Information Failed: " + data.ResultCode.ToStringCached<Result>());
				return;
			}
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Warning(string.Format("[EOS-Voice] can not query output devices information. Voice status: {0}", this.Status));
				return;
			}
			IList<IPartyVoice.VoiceAudioDevice> list = new List<IPartyVoice.VoiceAudioDevice>();
			GetOutputDevicesCountOptions getOutputDevicesCountOptions = default(GetOutputDevicesCountOptions);
			object lockObject = AntiCheatCommon.LockObject;
			uint outputDevicesCount;
			lock (lockObject)
			{
				outputDevicesCount = this.audioInterface.GetOutputDevicesCount(ref getOutputDevicesCountOptions);
			}
			for (uint num = 0U; num < outputDevicesCount; num += 1U)
			{
				CopyOutputDeviceInformationByIndexOptions copyOutputDeviceInformationByIndexOptions = new CopyOutputDeviceInformationByIndexOptions
				{
					DeviceIndex = num
				};
				lockObject = AntiCheatCommon.LockObject;
				OutputDeviceInformation? outputDeviceInformation;
				Result result;
				lock (lockObject)
				{
					result = this.audioInterface.CopyOutputDeviceInformationByIndex(ref copyOutputDeviceInformationByIndexOptions, out outputDeviceInformation);
				}
				try
				{
					if (result != Result.Success || outputDeviceInformation == null)
					{
						Log.Warning(string.Format("[EOS-Voice] Could not query output device {0}: {1}", num, result.ToStringCached<Result>()));
					}
					else
					{
						list.Add(new Voice.EosAudioDevice(outputDeviceInformation.Value));
					}
				}
				finally
				{
					bool flag2 = outputDeviceInformation != null;
				}
			}
			this.outputDevices = list;
		}

		// Token: 0x0600DD83 RID: 56707 RVA: 0x004F68DC File Offset: 0x004F4ADC
		[return: TupleElementNames(new string[]
		{
			"inputDevices",
			"outputDevices"
		})]
		public ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>> GetDevicesList()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not get voice devices because voice is currently not ready.");
				return new ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>>(Array.Empty<IPartyVoice.VoiceAudioDevice>(), Array.Empty<IPartyVoice.VoiceAudioDevice>());
			}
			EosHelpers.AssertMainThread("Voice.GetDev");
			return new ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>>(this.inputDevices, this.outputDevices);
		}

		// Token: 0x0600DD84 RID: 56708 RVA: 0x004F691C File Offset: 0x004F4B1C
		public void SetInputDevice(string _device)
		{
			Voice.<>c__DisplayClass66_0 CS$<>8__locals1 = new Voice.<>c__DisplayClass66_0();
			CS$<>8__locals1._device = _device;
			CS$<>8__locals1.<>4__this = this;
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not set input device because voice is currently not ready.");
				return;
			}
			EosHelpers.AssertMainThread("Voice.SetIn");
			SetInputDeviceSettingsOptions setInputDeviceSettingsOptions = new SetInputDeviceSettingsOptions
			{
				LocalUserId = this.localProductUserId,
				RealDeviceId = CS$<>8__locals1._device,
				PlatformAEC = true
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.SetInputDeviceSettings(ref setInputDeviceSettingsOptions, null, new OnSetInputDeviceSettingsCallback(CS$<>8__locals1.<SetInputDevice>g__OnSetInputDeviceSettings|0));
			}
		}

		// Token: 0x0600DD85 RID: 56709 RVA: 0x004F69D4 File Offset: 0x004F4BD4
		public void SetOutputDevice(string _device)
		{
			Voice.<>c__DisplayClass67_0 CS$<>8__locals1 = new Voice.<>c__DisplayClass67_0();
			CS$<>8__locals1._device = _device;
			CS$<>8__locals1.<>4__this = this;
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not set output device because voice is currently not ready.");
				return;
			}
			EosHelpers.AssertMainThread("Voice.SetOut");
			SetOutputDeviceSettingsOptions setOutputDeviceSettingsOptions = new SetOutputDeviceSettingsOptions
			{
				LocalUserId = this.localProductUserId,
				RealDeviceId = CS$<>8__locals1._device
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.SetOutputDeviceSettings(ref setOutputDeviceSettingsOptions, null, new OnSetOutputDeviceSettingsCallback(CS$<>8__locals1.<SetOutputDevice>g__OnSetOutputDeviceSettings|0));
			}
		}

		// Token: 0x17001BA1 RID: 7073
		// (get) Token: 0x0600DD86 RID: 56710 RVA: 0x004F6A84 File Offset: 0x004F4C84
		// (set) Token: 0x0600DD87 RID: 56711 RVA: 0x004F6A8C File Offset: 0x004F4C8C
		public bool MuteSelf
		{
			get
			{
				return this.muteSelf;
			}
			set
			{
				if (this.Status != EPartyVoiceStatus.Ok)
				{
					Log.Error("[EOS-Voice] Can not mute self because voice is currently not ready.");
					return;
				}
				if (value == this.muteSelf)
				{
					return;
				}
				this.muteSelf = value;
				if (this.roomName == null)
				{
					return;
				}
				EosHelpers.AssertMainThread("Voice.Mute");
				UpdateSendingOptions updateSendingOptions = new UpdateSendingOptions
				{
					LocalUserId = this.localProductUserId,
					RoomName = this.roomName,
					AudioStatus = (value ? RTCAudioStatus.Disabled : RTCAudioStatus.Enabled)
				};
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioInterface.UpdateSending(ref updateSendingOptions, null, delegate(ref UpdateSendingCallbackInfo _callbackData)
					{
						if (_callbackData.ResultCode != Result.Success)
						{
							Log.Error("[EOS-Voice] Toggling voice sending state failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						}
					});
				}
			}
		}

		// Token: 0x17001BA2 RID: 7074
		// (get) Token: 0x0600DD88 RID: 56712 RVA: 0x004F6B60 File Offset: 0x004F4D60
		// (set) Token: 0x0600DD89 RID: 56713 RVA: 0x004F6B68 File Offset: 0x004F4D68
		public bool MuteOthers
		{
			get
			{
				return this.muteOthers;
			}
			set
			{
				if (this.Status != EPartyVoiceStatus.Ok)
				{
					Log.Error("[EOS-Voice] Can not mute others because voice is currently not ready.");
					return;
				}
				if (value == this.muteOthers)
				{
					return;
				}
				this.muteOthers = value;
				if (this.roomName == null)
				{
					return;
				}
				EosHelpers.AssertMainThread("Voice.MuteOth");
				UpdateReceivingOptions updateReceivingOptions = new UpdateReceivingOptions
				{
					LocalUserId = this.localProductUserId,
					RoomName = this.roomName,
					AudioEnabled = !value
				};
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioInterface.UpdateReceiving(ref updateReceivingOptions, null, delegate(ref UpdateReceivingCallbackInfo _callbackData)
					{
						if (_callbackData.ResultCode != Result.Success)
						{
							Log.Error("[EOS-Voice] Toggling voice receiving state failed: " + _callbackData.ResultCode.ToStringCached<Result>());
							return;
						}
						Log.Out(string.Format("[EOS-Voice] Mute all state changed to: {0}", value));
					});
				}
			}
		}

		// Token: 0x17001BA3 RID: 7075
		// (get) Token: 0x0600DD8A RID: 56714 RVA: 0x004F6C44 File Offset: 0x004F4E44
		// (set) Token: 0x0600DD8B RID: 56715 RVA: 0x004F6C4C File Offset: 0x004F4E4C
		public float OutputVolume
		{
			get
			{
				return this.outputVolume;
			}
			set
			{
				if (this.Status != EPartyVoiceStatus.Ok)
				{
					Log.Error("[EOS-Voice] Can not set output volume because voice is currently not ready.");
					return;
				}
				if ((double)value > (double)this.outputVolume - 0.01 && (double)value < (double)this.outputVolume + 0.01)
				{
					return;
				}
				this.outputVolume = value;
				this.SetRoomReceivingVolume(this.outputVolume);
			}
		}

		// Token: 0x0600DD8C RID: 56716 RVA: 0x004F6CAC File Offset: 0x004F4EAC
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetRoomReceivingVolume(float platformVolume)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not set room receiving volume because voice is currently not ready.");
				return;
			}
			if (this.roomName == null)
			{
				return;
			}
			EosHelpers.AssertMainThread("Voice.Vol");
			UpdateReceivingVolumeOptions updateReceivingVolumeOptions = new UpdateReceivingVolumeOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName,
				Volume = platformVolume * 50f
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.UpdateReceivingVolume(ref updateReceivingVolumeOptions, null, new OnUpdateReceivingVolumeCallback(Voice.<SetRoomReceivingVolume>g__OnUpdateReceivingVolume|80_0));
			}
		}

		// Token: 0x0600DD8D RID: 56717 RVA: 0x004F6D5C File Offset: 0x004F4F5C
		public void BlockUser(PlatformUserIdentifierAbs _userIdentifier, bool _block)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not block user because voice is currently not ready.");
				return;
			}
			if (this.roomName == null)
			{
				return;
			}
			if (this.muteOthers)
			{
				return;
			}
			UserIdentifierEos userIdentifierEos = _userIdentifier as UserIdentifierEos;
			if (userIdentifierEos == null)
			{
				Log.Error(string.Format("[EOS-Voice] Block user identifier is not an EOS identifier: {0}", _userIdentifier));
				return;
			}
			Log.Out(string.Format("[EOS-Voice] Blocking user: {0} = {1}", userIdentifierEos, _block));
			EosHelpers.AssertMainThread("Voice.Block");
			BlockParticipantOptions blockParticipantOptions = new BlockParticipantOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName,
				ParticipantId = userIdentifierEos.ProductUserId,
				Blocked = _block
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.rtcInterface.BlockParticipant(ref blockParticipantOptions, null, delegate(ref BlockParticipantCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Blocking user failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						return;
					}
					if (_block)
					{
						this.blockedUsers.Add(_userIdentifier);
						Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged = this.OnRemotePlayerVoiceStateChanged;
						if (onRemotePlayerVoiceStateChanged == null)
						{
							return;
						}
						onRemotePlayerVoiceStateChanged(_userIdentifier, IPartyVoice.EVoiceMemberState.Muted);
						return;
					}
					else
					{
						this.blockedUsers.Remove(_userIdentifier);
						Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged2 = this.OnRemotePlayerVoiceStateChanged;
						if (onRemotePlayerVoiceStateChanged2 == null)
						{
							return;
						}
						onRemotePlayerVoiceStateChanged2(_userIdentifier, IPartyVoice.EVoiceMemberState.Normal);
						return;
					}
				});
			}
		}

		// Token: 0x0600DD8E RID: 56718 RVA: 0x004F6E84 File Offset: 0x004F5084
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator tryJoinLobbyCo(string _lobbyId)
		{
			Voice.<>c__DisplayClass83_0 CS$<>8__locals1 = new Voice.<>c__DisplayClass83_0();
			CS$<>8__locals1.<>4__this = this;
			EosHelpers.AssertMainThread("Voice.Join");
			int attempts = 0;
			CS$<>8__locals1.lobbyIdFound = null;
			CS$<>8__locals1.lobbyDetails = null;
			object lockObject;
			while (attempts < 5)
			{
				Voice.<>c__DisplayClass83_1 CS$<>8__locals2 = new Voice.<>c__DisplayClass83_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				int num = attempts;
				attempts = num + 1;
				Log.Out(string.Format("[EOS-Voice] Trying to find lobby for id {0}, attempt {1}", _lobbyId, attempts));
				CreateLobbySearchOptions createLobbySearchOptions = new CreateLobbySearchOptions
				{
					MaxResults = 10U
				};
				lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.lobbyInterface.CreateLobbySearch(ref createLobbySearchOptions, out CS$<>8__locals2.lobbySearch);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-Voice] Create lobby search failed: " + result.ToStringCached<Result>());
					this.joinInProgress = false;
					yield break;
				}
				LobbySearchSetLobbyIdOptions lobbySearchSetLobbyIdOptions = new LobbySearchSetLobbyIdOptions
				{
					LobbyId = _lobbyId
				};
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					result = CS$<>8__locals2.lobbySearch.SetLobbyId(ref lobbySearchSetLobbyIdOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-Voice] Set lobby search lobbyid failed: " + result.ToStringCached<Result>());
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						CS$<>8__locals2.lobbySearch.Release();
					}
					this.joinInProgress = false;
					yield break;
				}
				CS$<>8__locals2.findDone = false;
				CS$<>8__locals2.findError = false;
				CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
				LobbySearchFindOptions lobbySearchFindOptions = new LobbySearchFindOptions
				{
					LocalUserId = this.localProductUserId
				};
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					CS$<>8__locals2.lobbySearch.Find(ref lobbySearchFindOptions, null, delegate(ref LobbySearchFindCallbackInfo _callbackData)
					{
						object lockObject2;
						if (_callbackData.ResultCode == Result.NotFound)
						{
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							return;
						}
						if (_callbackData.ResultCode != Result.Success)
						{
							Log.Error("[EOS-Voice] Find lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							CS$<>8__locals2.findError = true;
							return;
						}
						LobbySearchGetSearchResultCountOptions lobbySearchGetSearchResultCountOptions = default(LobbySearchGetSearchResultCountOptions);
						lockObject2 = AntiCheatCommon.LockObject;
						uint searchResultCount;
						lock (lockObject2)
						{
							searchResultCount = CS$<>8__locals2.lobbySearch.GetSearchResultCount(ref lobbySearchGetSearchResultCountOptions);
						}
						if (searchResultCount != 1U)
						{
							Log.Error(string.Format("[EOS-Voice] Find lobby returned unexpected number of results ({0})", searchResultCount));
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							return;
						}
						LobbySearchCopySearchResultByIndexOptions lobbySearchCopySearchResultByIndexOptions = new LobbySearchCopySearchResultByIndexOptions
						{
							LobbyIndex = 0U
						};
						lockObject2 = AntiCheatCommon.LockObject;
						Result result2;
						lock (lockObject2)
						{
							result2 = CS$<>8__locals2.lobbySearch.CopySearchResultByIndex(ref lobbySearchCopySearchResultByIndexOptions, out CS$<>8__locals2.CS$<>8__locals1.lobbyDetails);
						}
						if (result2 != Result.Success)
						{
							Log.Error("[EOS-Voice] Get lobby details failed: " + result2.ToStringCached<Result>());
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
							CS$<>8__locals2.findDone = true;
							CS$<>8__locals2.findError = true;
							return;
						}
						LobbyDetailsCopyInfoOptions lobbyDetailsCopyInfoOptions = default(LobbyDetailsCopyInfoOptions);
						lockObject2 = AntiCheatCommon.LockObject;
						LobbyDetailsInfo? lobbyDetailsInfo;
						lock (lockObject2)
						{
							result2 = CS$<>8__locals2.CS$<>8__locals1.lobbyDetails.CopyInfo(ref lobbyDetailsCopyInfoOptions, out lobbyDetailsInfo);
						}
						if (result2 != Result.Success)
						{
							Log.Error("[EOS-Voice] Get lobby details info failed: " + result2.ToStringCached<Result>());
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.CS$<>8__locals1.lobbyDetails.Release();
							}
							CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							CS$<>8__locals2.findError = true;
							return;
						}
						CS$<>8__locals2.CS$<>8__locals1.lobbyIdFound = lobbyDetailsInfo.Value.LobbyId;
						Log.Out("[EOS-Voice] Found lobby: " + CS$<>8__locals2.CS$<>8__locals1.lobbyIdFound);
						CS$<>8__locals2.findDone = true;
						lockObject2 = AntiCheatCommon.LockObject;
						lock (lockObject2)
						{
							CS$<>8__locals2.lobbySearch.Release();
						}
					});
					goto IL_278;
				}
				goto IL_261;
				IL_278:
				if (CS$<>8__locals2.findDone)
				{
					if (CS$<>8__locals2.findError)
					{
						CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
						Log.Error("[EOS-Voice] Failed joining voice lobby");
						this.joinInProgress = false;
						yield break;
					}
					if (CS$<>8__locals2.CS$<>8__locals1.lobbyIdFound == null)
					{
						yield return new WaitForSeconds(0.5f);
						CS$<>8__locals2 = null;
						continue;
					}
					break;
				}
				IL_261:
				yield return null;
				goto IL_278;
			}
			if (CS$<>8__locals1.lobbyDetails == null)
			{
				Log.Error("[EOS-Voice] Did not find lobby");
				this.joinInProgress = false;
				yield break;
			}
			Log.Out(string.Format("[EOS-Voice] Found lobby on {0} attempt", attempts));
			JoinLobbyOptions joinLobbyOptions = new JoinLobbyOptions
			{
				LocalUserId = this.localProductUserId,
				PresenceEnabled = false,
				LobbyDetailsHandle = CS$<>8__locals1.lobbyDetails,
				LocalRTCOptions = new LocalRTCOptions?(new LocalRTCOptions
				{
					LocalAudioDeviceInputStartsMuted = true
				})
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.JoinLobby(ref joinLobbyOptions, null, delegate(ref JoinLobbyCallbackInfo _callbackData)
				{
					object lockObject2;
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Join lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						lockObject2 = AntiCheatCommon.LockObject;
						lock (lockObject2)
						{
							CS$<>8__locals1.lobbyDetails.Release();
						}
						CS$<>8__locals1.<>4__this.joinInProgress = false;
						return;
					}
					CS$<>8__locals1.<>4__this.lobbyEntered(CS$<>8__locals1.lobbyIdFound);
					lockObject2 = AntiCheatCommon.LockObject;
					lock (lockObject2)
					{
						CS$<>8__locals1.lobbyDetails.Release();
					}
				});
				yield break;
			}
			yield break;
		}

		// Token: 0x0600DD8F RID: 56719 RVA: 0x004F6E9C File Offset: 0x004F509C
		[PublicizedFrom(EAccessModifier.Private)]
		public void lobbyEntered(string _lobbyId)
		{
			this.blockedUsers.Clear();
			this.lobbyId = _lobbyId;
			GetRTCRoomNameOptions getRTCRoomNameOptions = new GetRTCRoomNameOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId
			};
			object lockObject = AntiCheatCommon.LockObject;
			Utf8String other;
			Result rtcroomName;
			lock (lockObject)
			{
				rtcroomName = this.lobbyInterface.GetRTCRoomName(ref getRTCRoomNameOptions, out other);
			}
			if (rtcroomName != Result.Success)
			{
				Log.Error("[EOS-Voice] Getting local lobby room name failed: " + rtcroomName.ToStringCached<Result>());
			}
			this.roomName = other;
			AddNotifyParticipantStatusChangedOptions addNotifyParticipantStatusChangedOptions = new AddNotifyParticipantStatusChangedOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.participantStatusChangedHandle = this.rtcInterface.AddNotifyParticipantStatusChanged(ref addNotifyParticipantStatusChangedOptions, null, new OnParticipantStatusChangedCallback(this.participantStatusChanged));
			}
			AddNotifyParticipantUpdatedOptions addNotifyParticipantUpdatedOptions = new AddNotifyParticipantUpdatedOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.participantUpdatedHandle = this.audioInterface.AddNotifyParticipantUpdated(ref addNotifyParticipantUpdatedOptions, null, new OnParticipantUpdatedCallback(this.participantVoiceChanged));
			}
			this.SetRoomReceivingVolume(this.outputVolume);
		}

		// Token: 0x0600DD90 RID: 56720 RVA: 0x004F7048 File Offset: 0x004F5248
		[PublicizedFrom(EAccessModifier.Private)]
		public void participantVoiceChanged(ref ParticipantUpdatedCallbackInfo _data)
		{
			if (Api.DebugLevel == Api.EDebugLevel.Verbose)
			{
				Log.Out(string.Format("[EOS-Voice] Participant update: {0}, speaking={1}, audio={2}", _data.ParticipantId, _data.Speaking, _data.AudioStatus));
			}
			UserIdentifierEos userIdentifierEos = new UserIdentifierEos(_data.ParticipantId);
			if (userIdentifierEos.Equals(this.localUserIdentifier))
			{
				return;
			}
			if (!this.blockedUsers.Contains(userIdentifierEos))
			{
				Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged = this.OnRemotePlayerVoiceStateChanged;
				if (onRemotePlayerVoiceStateChanged != null)
				{
					PlatformUserIdentifierAbs arg = userIdentifierEos;
					IPartyVoice.EVoiceMemberState arg2;
					switch (_data.AudioStatus)
					{
					case RTCAudioStatus.Unsupported:
						arg2 = IPartyVoice.EVoiceMemberState.Disabled;
						break;
					case RTCAudioStatus.Enabled:
						arg2 = IPartyVoice.EVoiceMemberState.VoiceActive;
						break;
					case RTCAudioStatus.Disabled:
						arg2 = IPartyVoice.EVoiceMemberState.Normal;
						break;
					case RTCAudioStatus.AdminDisabled:
						arg2 = IPartyVoice.EVoiceMemberState.Muted;
						break;
					case RTCAudioStatus.NotListeningDisabled:
						arg2 = IPartyVoice.EVoiceMemberState.Muted;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					onRemotePlayerVoiceStateChanged(arg, arg2);
				}
				return;
			}
			Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged2 = this.OnRemotePlayerVoiceStateChanged;
			if (onRemotePlayerVoiceStateChanged2 == null)
			{
				return;
			}
			onRemotePlayerVoiceStateChanged2(userIdentifierEos, IPartyVoice.EVoiceMemberState.Muted);
		}

		// Token: 0x0600DD91 RID: 56721 RVA: 0x004F7118 File Offset: 0x004F5318
		[PublicizedFrom(EAccessModifier.Private)]
		public void participantStatusChanged(ref ParticipantStatusChangedCallbackInfo _data)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				return;
			}
			if (Api.DebugLevel == Api.EDebugLevel.Verbose)
			{
				Log.Out(string.Format("[EOS-Voice] Participant state changed: {0}, {1}", _data.ParticipantId, _data.ParticipantStatus));
			}
			UserIdentifierEos userIdentifierEos = new UserIdentifierEos(_data.ParticipantId);
			if (userIdentifierEos.Equals(this.localUserIdentifier))
			{
				this.roomEntered = (_data.ParticipantStatus == RTCParticipantStatus.Joined);
				if (this.roomEntered)
				{
					this.createInProgress = false;
					this.joinInProgress = false;
					this.muteSelf = true;
					this.muteOthers = false;
				}
				Action<IPartyVoice.EVoiceChannelAction> onLocalPlayerStateChanged = this.OnLocalPlayerStateChanged;
				if (onLocalPlayerStateChanged == null)
				{
					return;
				}
				onLocalPlayerStateChanged((_data.ParticipantStatus == RTCParticipantStatus.Joined) ? IPartyVoice.EVoiceChannelAction.Joined : IPartyVoice.EVoiceChannelAction.Left);
				return;
			}
			else
			{
				Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceChannelAction> onRemotePlayerStateChanged = this.OnRemotePlayerStateChanged;
				if (onRemotePlayerStateChanged == null)
				{
					return;
				}
				onRemotePlayerStateChanged(userIdentifierEos, (_data.ParticipantStatus == RTCParticipantStatus.Joined) ? IPartyVoice.EVoiceChannelAction.Joined : IPartyVoice.EVoiceChannelAction.Left);
				return;
			}
		}

		// Token: 0x0600DD92 RID: 56722 RVA: 0x004F71E0 File Offset: 0x004F53E0
		[PublicizedFrom(EAccessModifier.Private)]
		public void lobbyLeft()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				return;
			}
			this.lobbyId = null;
			this.roomName = null;
			this.muteSelf = true;
			this.muteOthers = false;
			this.blockedUsers.Clear();
			if (this.participantStatusChangedHandle != 0UL)
			{
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.rtcInterface.RemoveNotifyParticipantStatusChanged(this.participantStatusChangedHandle);
				}
			}
			if (this.participantUpdatedHandle != 0UL)
			{
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioInterface.RemoveNotifyParticipantUpdated(this.participantUpdatedHandle);
				}
			}
			this.participantStatusChangedHandle = 0UL;
			this.participantUpdatedHandle = 0UL;
		}

		// Token: 0x0600DD96 RID: 56726 RVA: 0x004F738E File Offset: 0x004F558E
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <AddAudioDevicesNotifications>g__OnAudioDevicesChanged|60_0(ref AudioDevicesChangedCallbackInfo data)
		{
			this.RefreshAudioDevices();
		}

		// Token: 0x0600DD97 RID: 56727 RVA: 0x004F7396 File Offset: 0x004F5596
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <SetRoomReceivingVolume>g__OnUpdateReceivingVolume|80_0(ref UpdateReceivingVolumeCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Log.Error(string.Format("[EOS-Voice] Setting voice output volume for room '{0}' failed: {1}", data.RoomName, data.ResultCode.ToStringCached<Result>()));
			}
		}

		// Token: 0x0400A784 RID: 42884
		[PublicizedFrom(EAccessModifier.Private)]
		public const int voiceLobbyConnectAttempts = 5;

		// Token: 0x0400A785 RID: 42885
		[PublicizedFrom(EAccessModifier.Private)]
		public const float voiceLobbyConnectAttemptInterval = 0.5f;

		// Token: 0x0400A786 RID: 42886
		[PublicizedFrom(EAccessModifier.Private)]
		public const float platformVolumeToEosRtcVolume = 50f;

		// Token: 0x0400A787 RID: 42887
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A788 RID: 42888
		[PublicizedFrom(EAccessModifier.Private)]
		public Api api;

		// Token: 0x0400A789 RID: 42889
		[PublicizedFrom(EAccessModifier.Private)]
		public RTCInterface rtcInterface;

		// Token: 0x0400A78A RID: 42890
		[PublicizedFrom(EAccessModifier.Private)]
		public RTCAudioInterface audioInterface;

		// Token: 0x0400A78B RID: 42891
		[PublicizedFrom(EAccessModifier.Private)]
		public LobbyInterface lobbyInterface;

		// Token: 0x0400A78C RID: 42892
		[PublicizedFrom(EAccessModifier.Private)]
		public string lobbyId;

		// Token: 0x0400A78D RID: 42893
		[PublicizedFrom(EAccessModifier.Private)]
		public bool createInProgress;

		// Token: 0x0400A78E RID: 42894
		[PublicizedFrom(EAccessModifier.Private)]
		public bool joinInProgress;

		// Token: 0x0400A78F RID: 42895
		[PublicizedFrom(EAccessModifier.Private)]
		public bool leaveInProgress;

		// Token: 0x0400A790 RID: 42896
		[PublicizedFrom(EAccessModifier.Private)]
		public string roomName;

		// Token: 0x0400A791 RID: 42897
		[PublicizedFrom(EAccessModifier.Private)]
		public bool roomEntered;

		// Token: 0x0400A792 RID: 42898
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong participantStatusChangedHandle;

		// Token: 0x0400A793 RID: 42899
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong participantUpdatedHandle;

		// Token: 0x0400A794 RID: 42900
		[PublicizedFrom(EAccessModifier.Private)]
		public Action initializedDelegates;

		// Token: 0x0400A795 RID: 42901
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object initializedDelegateLock = new object();

		// Token: 0x0400A79A RID: 42906
		[PublicizedFrom(EAccessModifier.Private)]
		public string activeInputDeviceId;

		// Token: 0x0400A79B RID: 42907
		[PublicizedFrom(EAccessModifier.Private)]
		public string activeOutputDeviceId;

		// Token: 0x0400A79C RID: 42908
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong audioDevicesChangedNotificationId;

		// Token: 0x0400A79D RID: 42909
		[PublicizedFrom(EAccessModifier.Private)]
		public IList<IPartyVoice.VoiceAudioDevice> inputDevices;

		// Token: 0x0400A79E RID: 42910
		[PublicizedFrom(EAccessModifier.Private)]
		public IList<IPartyVoice.VoiceAudioDevice> outputDevices;

		// Token: 0x0400A79F RID: 42911
		[PublicizedFrom(EAccessModifier.Private)]
		public bool muteSelf;

		// Token: 0x0400A7A0 RID: 42912
		[PublicizedFrom(EAccessModifier.Private)]
		public bool muteOthers;

		// Token: 0x0400A7A1 RID: 42913
		[PublicizedFrom(EAccessModifier.Private)]
		public float outputVolume = 1f;

		// Token: 0x0400A7A2 RID: 42914
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<PlatformUserIdentifierAbs> blockedUsers = new HashSet<PlatformUserIdentifierAbs>();

		// Token: 0x02001D34 RID: 7476
		public class EosAudioDevice : IPartyVoice.VoiceAudioDevice
		{
			// Token: 0x0600DD98 RID: 56728 RVA: 0x004F73C0 File Offset: 0x004F55C0
			public EosAudioDevice(InputDeviceInformation _device) : base(false, _device.DefaultDevice)
			{
				this.Id = _device.DeviceId;
				this.Name = _device.DeviceName;
				this.LogDevice("Input");
			}

			// Token: 0x0600DD99 RID: 56729 RVA: 0x004F73FF File Offset: 0x004F55FF
			public EosAudioDevice(OutputDeviceInformation _device) : base(true, _device.DefaultDevice)
			{
				this.Id = _device.DeviceId;
				this.Name = _device.DeviceName;
				this.LogDevice("Output");
			}

			// Token: 0x0600DD9A RID: 56730 RVA: 0x004F7440 File Offset: 0x004F5640
			[PublicizedFrom(EAccessModifier.Private)]
			public void LogDevice(string _inOutString)
			{
				if (GameUtils.GetLaunchArgument("debugeos") != null)
				{
					Log.Out(string.Format("[EOS-Voice] {0} device: Id={1}, Name={2}, Default={3}", new object[]
					{
						_inOutString,
						this.Id,
						this.Name,
						this.IsDefault
					}));
				}
			}

			// Token: 0x0600DD9B RID: 56731 RVA: 0x004F7492 File Offset: 0x004F5692
			public override string ToString()
			{
				if (!this.IsDefault)
				{
					return this.Name;
				}
				return "(Default) " + this.Name;
			}

			// Token: 0x17001BA4 RID: 7076
			// (get) Token: 0x0600DD9C RID: 56732 RVA: 0x004F74B3 File Offset: 0x004F56B3
			public override string Identifier
			{
				get
				{
					return this.Id;
				}
			}

			// Token: 0x0400A7A3 RID: 42915
			public readonly string Id;

			// Token: 0x0400A7A4 RID: 42916
			public readonly string Name;
		}
	}
}
