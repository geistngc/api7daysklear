using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x02001B70 RID: 7024
	public interface IPartyVoice
	{
		// Token: 0x1400011E RID: 286
		// (add) Token: 0x0600D22D RID: 53805
		// (remove) Token: 0x0600D22E RID: 53806
		event Action Initialized;

		// Token: 0x170019DF RID: 6623
		// (get) Token: 0x0600D22F RID: 53807
		EPartyVoiceStatus Status { get; }

		// Token: 0x170019E0 RID: 6624
		// (get) Token: 0x0600D230 RID: 53808
		bool InLobby { get; }

		// Token: 0x170019E1 RID: 6625
		// (get) Token: 0x0600D231 RID: 53809
		bool InLobbyOrProgress { get; }

		// Token: 0x0600D232 RID: 53810
		void Init(IPlatform _owner);

		// Token: 0x0600D233 RID: 53811
		void Destroy();

		// Token: 0x0600D234 RID: 53812
		void CreateLobby(Action<string> _lobbyCreatedCallback);

		// Token: 0x0600D235 RID: 53813
		void JoinLobby(string _lobbyId);

		// Token: 0x0600D236 RID: 53814
		void LeaveLobby();

		// Token: 0x0600D237 RID: 53815
		void PromoteLeader(PlatformUserIdentifierAbs _newLeaderIdentifier);

		// Token: 0x0600D238 RID: 53816
		bool IsLobbyOwner();

		// Token: 0x1400011F RID: 287
		// (add) Token: 0x0600D239 RID: 53817
		// (remove) Token: 0x0600D23A RID: 53818
		event Action<IPartyVoice.EVoiceChannelAction> OnLocalPlayerStateChanged;

		// Token: 0x14000120 RID: 288
		// (add) Token: 0x0600D23B RID: 53819
		// (remove) Token: 0x0600D23C RID: 53820
		event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceChannelAction> OnRemotePlayerStateChanged;

		// Token: 0x14000121 RID: 289
		// (add) Token: 0x0600D23D RID: 53821
		// (remove) Token: 0x0600D23E RID: 53822
		event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> OnRemotePlayerVoiceStateChanged;

		// Token: 0x0600D23F RID: 53823
		[return: TupleElementNames(new string[]
		{
			"inputDevices",
			"outputDevices"
		})]
		ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>> GetDevicesList();

		// Token: 0x0600D240 RID: 53824
		void SetInputDevice(string _device);

		// Token: 0x0600D241 RID: 53825
		void SetOutputDevice(string _device);

		// Token: 0x170019E2 RID: 6626
		// (get) Token: 0x0600D242 RID: 53826
		// (set) Token: 0x0600D243 RID: 53827
		bool MuteSelf { get; set; }

		// Token: 0x170019E3 RID: 6627
		// (get) Token: 0x0600D244 RID: 53828
		// (set) Token: 0x0600D245 RID: 53829
		bool MuteOthers { get; set; }

		// Token: 0x170019E4 RID: 6628
		// (get) Token: 0x0600D246 RID: 53830
		// (set) Token: 0x0600D247 RID: 53831
		float OutputVolume { get; set; }

		// Token: 0x0600D248 RID: 53832
		void BlockUser(PlatformUserIdentifierAbs _userIdentifier, bool _block);

		// Token: 0x02001B71 RID: 7025
		public enum EVoiceMemberState
		{
			// Token: 0x0400A0EF RID: 41199
			Disabled,
			// Token: 0x0400A0F0 RID: 41200
			Normal,
			// Token: 0x0400A0F1 RID: 41201
			Muted,
			// Token: 0x0400A0F2 RID: 41202
			VoiceActive
		}

		// Token: 0x02001B72 RID: 7026
		public enum EVoiceChannelAction
		{
			// Token: 0x0400A0F4 RID: 41204
			Joined,
			// Token: 0x0400A0F5 RID: 41205
			Left
		}

		// Token: 0x02001B73 RID: 7027
		public abstract class VoiceAudioDevice
		{
			// Token: 0x0600D249 RID: 53833 RVA: 0x004C9F9D File Offset: 0x004C819D
			public VoiceAudioDevice(bool _isOutput, bool _isDefault)
			{
				this.IsOutput = _isOutput;
				this.IsDefault = _isDefault;
			}

			// Token: 0x170019E5 RID: 6629
			// (get) Token: 0x0600D24A RID: 53834
			public abstract string Identifier { get; }

			// Token: 0x0400A0F6 RID: 41206
			public readonly bool IsOutput;

			// Token: 0x0400A0F7 RID: 41207
			public readonly bool IsDefault;
		}

		// Token: 0x02001B74 RID: 7028
		public class VoiceAudioDeviceNotFound : IPartyVoice.VoiceAudioDevice
		{
			// Token: 0x0600D24B RID: 53835 RVA: 0x004C9FB3 File Offset: 0x004C81B3
			public VoiceAudioDeviceNotFound() : base(false, false)
			{
			}

			// Token: 0x0600D24C RID: 53836 RVA: 0x004C9FBD File Offset: 0x004C81BD
			public override string ToString()
			{
				return Localization.Get("noAudioDeviceFound", false, null);
			}

			// Token: 0x170019E6 RID: 6630
			// (get) Token: 0x0600D24D RID: 53837 RVA: 0x00032163 File Offset: 0x00030363
			public override string Identifier
			{
				get
				{
					return "";
				}
			}
		}

		// Token: 0x02001B75 RID: 7029
		public class VoiceAudioDeviceDefault : IPartyVoice.VoiceAudioDevice
		{
			// Token: 0x0600D24E RID: 53838 RVA: 0x004C9FB3 File Offset: 0x004C81B3
			public VoiceAudioDeviceDefault() : base(false, false)
			{
			}

			// Token: 0x0600D24F RID: 53839 RVA: 0x004C9FCB File Offset: 0x004C81CB
			public override string ToString()
			{
				return Localization.Get("defaultAudioDevice", false, null);
			}

			// Token: 0x170019E7 RID: 6631
			// (get) Token: 0x0600D250 RID: 53840 RVA: 0x00032163 File Offset: 0x00030363
			public override string Identifier
			{
				get
				{
					return "";
				}
			}
		}
	}
}
