using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Platform
{
	// Token: 0x02001B3B RID: 6971
	public class BlockedPlayerList : IRemotePlayerStorageObject
	{
		// Token: 0x170019A1 RID: 6561
		// (get) Token: 0x0600D0FE RID: 53502 RVA: 0x004C7684 File Offset: 0x004C5884
		public static BlockedPlayerList Instance
		{
			get
			{
				if (BlockedPlayerList.instance == null)
				{
					IPlatform multiPlatform = PlatformManager.MultiPlatform;
					if (((multiPlatform != null) ? multiPlatform.RemotePlayerFileStorage : null) != null)
					{
						BlockedPlayerList.instance = new BlockedPlayerList();
						PlayerInteractions.Instance.OnNewPlayerInteraction += BlockedPlayerList.instance.OnPlayerInteraction;
					}
				}
				return BlockedPlayerList.instance;
			}
		}

		// Token: 0x0600D0FF RID: 53503 RVA: 0x004C76D4 File Offset: 0x004C58D4
		public void Update()
		{
			IPlatform multiPlatform = PlatformManager.MultiPlatform;
			if (((multiPlatform != null) ? multiPlatform.RemotePlayerFileStorage : null) == null)
			{
				return;
			}
			if ((this.writeRequestTime != null && DateTime.UtcNow - this.writeRequestTime >= BlockedPlayerList.WriteRequestDelay) || DateTime.UtcNow - this.lastWriteTime >= BlockedPlayerList.WriteThreshold)
			{
				this.WriteToStorage();
				this.lastWriteTime = DateTime.UtcNow;
				this.writeRequestTime = null;
			}
		}

		// Token: 0x0600D100 RID: 53504 RVA: 0x004C7794 File Offset: 0x004C5994
		public void UpdatePlayersSeenInWorld(World _world)
		{
			if (((_world != null) ? _world.Players : null) == null)
			{
				return;
			}
			foreach (EntityPlayer entityPlayer in _world.Players.list)
			{
				PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(entityPlayer.entityId);
				this.AddOrUpdatePlayer(playerDataFromEntityID.PlayerData, DateTime.UtcNow, null, false);
			}
		}

		// Token: 0x0600D101 RID: 53505 RVA: 0x004C7828 File Offset: 0x004C5A28
		[PublicizedFrom(EAccessModifier.Private)]
		public BlockedPlayerList.ListEntry AddOrUpdatePlayer(PlayerData _playerData, DateTime _timeStamp, bool? _blocked = null, bool _ignoreLimit = false)
		{
			if (_playerData == null || _playerData.PrimaryId.Equals(PlatformManager.MultiPlatform.User.PlatformUserId))
			{
				return null;
			}
			DateTime t = DateTime.UtcNow.AddHours(-168.0);
			bool? flag = _blocked;
			bool flag2 = false;
			if ((flag.GetValueOrDefault() == flag2 & flag != null) && t >= _timeStamp)
			{
				return null;
			}
			object obj = this.bplLock;
			BlockedPlayerList.ListEntry result;
			lock (obj)
			{
				BlockedPlayerList.ListEntry valueOrDefault = this.playerStates.dict.GetValueOrDefault(_playerData.PrimaryId);
				if (!_ignoreLimit)
				{
					flag = _blocked;
					bool flag3 = true;
					if ((flag.GetValueOrDefault() == flag3 & flag != null) && (valueOrDefault == null || !valueOrDefault.Blocked) && this.EntryCount(true, false) >= 500)
					{
						return null;
					}
				}
				BlockedPlayerList.ListEntry listEntry;
				if (_blocked == null && valueOrDefault != null)
				{
					listEntry = new BlockedPlayerList.ListEntry(_playerData, _timeStamp, valueOrDefault.Blocked);
				}
				else
				{
					listEntry = new BlockedPlayerList.ListEntry(_playerData, _timeStamp, _blocked.GetValueOrDefault());
				}
				this.playerStates.Set(_playerData.PrimaryId, listEntry);
				result = listEntry;
			}
			return result;
		}

		// Token: 0x0600D102 RID: 53506 RVA: 0x004C7964 File Offset: 0x004C5B64
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPlayerInteraction(PlayerInteraction _interaction)
		{
			BlockedPlayerList.ListEntry listEntry = this.AddOrUpdatePlayer(_interaction.PlayerData, DateTime.UtcNow, null, false);
			if (listEntry != null)
			{
				this.MarkForWrite();
				listEntry.SetResolvedOnce();
			}
		}

		// Token: 0x0600D103 RID: 53507 RVA: 0x004C799C File Offset: 0x004C5B9C
		public int EntryCount(bool _blocked, bool _resolveRequired)
		{
			return this.playerStates.list.Count((BlockedPlayerList.ListEntry entry) => entry.Blocked == _blocked && (!_resolveRequired || entry.ResolvedOnce));
		}

		// Token: 0x0600D104 RID: 53508 RVA: 0x004C79D9 File Offset: 0x004C5BD9
		public IEnumerable<BlockedPlayerList.ListEntry> GetEntriesOrdered(bool _blocked, bool _resolveRequired)
		{
			object obj = this.bplLock;
			lock (obj)
			{
				this.SortPlayerStates();
				int num;
				for (int i = 0; i < this.playerStates.list.Count; i = num + 1)
				{
					BlockedPlayerList.ListEntry listEntry = this.playerStates.list[i];
					if (listEntry.Blocked == _blocked && (!_resolveRequired || listEntry.ResolvedOnce))
					{
						yield return listEntry;
					}
					num = i;
				}
			}
			obj = null;
			yield break;
			yield break;
		}

		// Token: 0x0600D105 RID: 53509 RVA: 0x004C79F8 File Offset: 0x004C5BF8
		public BlockedPlayerList.ListEntry GetPlayerStateInfo(PlatformUserIdentifierAbs _primaryId)
		{
			object obj = this.bplLock;
			BlockedPlayerList.ListEntry result;
			lock (obj)
			{
				BlockedPlayerList.ListEntry listEntry;
				if (this.playerStates.dict.TryGetValue(_primaryId, out listEntry))
				{
					result = listEntry;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600D106 RID: 53510 RVA: 0x004C7A50 File Offset: 0x004C5C50
		[PublicizedFrom(EAccessModifier.Private)]
		public void SortPlayerStates()
		{
			this.playerStates.list.Sort((BlockedPlayerList.ListEntry p1, BlockedPlayerList.ListEntry p2) => p2.LastSeen.CompareTo(p1.LastSeen));
		}

		// Token: 0x0600D107 RID: 53511 RVA: 0x004C7A81 File Offset: 0x004C5C81
		public IEnumerator ReadStorageAndResolve()
		{
			BlockedPlayerList.<>c__DisplayClass25_0 CS$<>8__locals1 = new BlockedPlayerList.<>c__DisplayClass25_0();
			CS$<>8__locals1.<>4__this = this;
			this.readStorageState = ERoutineState.Running;
			object obj = this.bplLock;
			lock (obj)
			{
				BlockedPlayerList blockedPlayerList = IRemotePlayerFileStorage.ReadCachedObject<BlockedPlayerList>(PlatformManager.MultiPlatform.User, "BlockedPlayerList");
				if (blockedPlayerList != null)
				{
					this.playerStates = blockedPlayerList.playerStates;
				}
			}
			CS$<>8__locals1.callbackComplete = false;
			IRemotePlayerFileStorage remotePlayerFileStorage = PlatformManager.MultiPlatform.RemotePlayerFileStorage;
			if (remotePlayerFileStorage != null)
			{
				remotePlayerFileStorage.ReadRemoteObject<BlockedPlayerList>("BlockedPlayerList", true, new IRemotePlayerFileStorage.FileReadObjectCompleteCallback<BlockedPlayerList>(CS$<>8__locals1.<ReadStorageAndResolve>g__ReadRPFSCallback|0));
				while (!CS$<>8__locals1.callbackComplete)
				{
					yield return null;
				}
			}
			if (this.playerStates.Count > 0)
			{
				yield return this.ResolveUserDetails();
			}
			this.readStorageState = ERoutineState.Succeeded;
			yield break;
		}

		// Token: 0x0600D108 RID: 53512 RVA: 0x004C7A90 File Offset: 0x004C5C90
		public void ReadInto(BinaryReader _reader)
		{
			object obj = this.bplLock;
			lock (obj)
			{
				this.playerStates.Clear();
				_reader.ReadInt32();
				int num = _reader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					BlockedPlayerList.ListEntry listEntry = BlockedPlayerList.ListEntry.Read(_reader);
					this.AddOrUpdatePlayer(listEntry.PlayerData, listEntry.LastSeen, new bool?(listEntry.Blocked), false);
				}
			}
		}

		// Token: 0x0600D109 RID: 53513 RVA: 0x004C7B1C File Offset: 0x004C5D1C
		public void MarkForWrite()
		{
			if (this.writeRequestTime == null)
			{
				this.writeRequestTime = new DateTime?(DateTime.UtcNow);
			}
		}

		// Token: 0x0600D10A RID: 53514 RVA: 0x004C7B3C File Offset: 0x004C5D3C
		[PublicizedFrom(EAccessModifier.Private)]
		public void WriteToStorage()
		{
			if (this.writeToStorageState == ERoutineState.Running)
			{
				Log.Warning("[BlockedPlayerList] Tried to write to storage while another write is already in progress.");
				return;
			}
			if (this.readStorageResult != IRemotePlayerFileStorage.CallbackResult.Success && this.readStorageResult != IRemotePlayerFileStorage.CallbackResult.MalformedData && this.readStorageResult != IRemotePlayerFileStorage.CallbackResult.FileNotFound)
			{
				Log.Out("[BlockedPlayerList] Error when processing remote list. Saving to local cache only.");
				if (!IRemotePlayerFileStorage.WriteCachedObject(PlatformManager.MultiPlatform.User, "BlockedPlayerList", this))
				{
					Log.Warning("[BlockedPlayerList] Failed to write to local cache.");
				}
				return;
			}
			if (this.readStorageResult == IRemotePlayerFileStorage.CallbackResult.MalformedData)
			{
				Log.Out("[BlockedPlayerList] Previous remote list was malformed so it will be overwritten.");
			}
			this.writeToStorageState = ERoutineState.Running;
			PlatformManager.MultiPlatform.RemotePlayerFileStorage.WriteRemoteObject("BlockedPlayerList", this, true, new IRemotePlayerFileStorage.FileWriteCompleteCallback(this.WriteRPFSCallback));
		}

		// Token: 0x0600D10B RID: 53515 RVA: 0x004C7BE4 File Offset: 0x004C5DE4
		[PublicizedFrom(EAccessModifier.Private)]
		public void WriteRPFSCallback(IRemotePlayerFileStorage.CallbackResult _result)
		{
			this.writeToStorageState = ERoutineState.NotStarted;
			if (_result != IRemotePlayerFileStorage.CallbackResult.Success)
			{
				Log.Warning("[BlockedPlayerList] Recent Player List failed to write to remote storage.");
			}
		}

		// Token: 0x0600D10C RID: 53516 RVA: 0x004C7BFC File Offset: 0x004C5DFC
		public void WriteFrom(BinaryWriter _writer)
		{
			object obj = this.bplLock;
			lock (obj)
			{
				_writer.Write(1);
				this.SortPlayerStates();
				int num = this.EntryCount(true, false);
				int num2 = Math.Min(this.EntryCount(false, false), 100);
				_writer.Write(num + num2);
				for (int i = 0; i < num + num2; i++)
				{
					this.playerStates.list[i].Write(_writer);
				}
			}
		}

		// Token: 0x0600D10D RID: 53517 RVA: 0x004C7C90 File Offset: 0x004C5E90
		public bool PendingResolve()
		{
			return this.readStorageState != ERoutineState.Succeeded || this.resolveState == ERoutineState.Running;
		}

		// Token: 0x0600D10E RID: 53518 RVA: 0x004C7CA6 File Offset: 0x004C5EA6
		public IEnumerator ResolveUserDetails()
		{
			while (this.resolveState == ERoutineState.Running)
			{
				yield return null;
			}
			try
			{
				this.resolveState = ERoutineState.Running;
				List<IPlatformUserData> dataList = new List<IPlatformUserData>();
				object obj = this.bplLock;
				lock (obj)
				{
					foreach (BlockedPlayerList.ListEntry listEntry in this.playerStates.list)
					{
						listEntry.PlayerData.PlatformData.RequestUserDetailsUpdate();
						dataList.Add(listEntry.PlayerData.PlatformData);
					}
				}
				yield return PlatformUserManager.ResolveUsersDetailsCoroutine(dataList);
				foreach (IPlatformUserData platformUserData in dataList)
				{
					AuthoredText playerName = this.playerStates.dict[platformUserData.PrimaryId].PlayerData.PlayerName;
					if (platformUserData.Name != null && platformUserData.Name != playerName.Text)
					{
						playerName.Update(platformUserData.Name, playerName.Author);
						GeneratedTextManager.PrefilterText(playerName, GeneratedTextManager.TextFilteringMode.Filter);
					}
					this.playerStates.dict[platformUserData.PrimaryId].SetResolvedOnce();
				}
				this.resolveState = ERoutineState.Succeeded;
				dataList = null;
			}
			finally
			{
				if (this.resolveState != ERoutineState.Succeeded)
				{
					this.resolveState = ERoutineState.Failed;
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x0400A03A RID: 41018
		[PublicizedFrom(EAccessModifier.Private)]
		public static BlockedPlayerList instance;

		// Token: 0x0400A03B RID: 41019
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan WriteThreshold = TimeSpan.FromMinutes(10.0);

		// Token: 0x0400A03C RID: 41020
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan WriteRequestDelay = TimeSpan.FromSeconds(5.0);

		// Token: 0x0400A03D RID: 41021
		public const int MaxBlockedPlayerEntries = 500;

		// Token: 0x0400A03E RID: 41022
		public const int MaxRecentPlayerEntries = 100;

		// Token: 0x0400A03F RID: 41023
		[PublicizedFrom(EAccessModifier.Private)]
		public const int Version = 1;

		// Token: 0x0400A040 RID: 41024
		[PublicizedFrom(EAccessModifier.Private)]
		public const string FilePath = "BlockedPlayerList";

		// Token: 0x0400A041 RID: 41025
		[PublicizedFrom(EAccessModifier.Private)]
		public const int TimeoutHours = 168;

		// Token: 0x0400A042 RID: 41026
		[PublicizedFrom(EAccessModifier.Private)]
		public object bplLock = new object();

		// Token: 0x0400A043 RID: 41027
		[PublicizedFrom(EAccessModifier.Private)]
		public DictionaryList<PlatformUserIdentifierAbs, BlockedPlayerList.ListEntry> playerStates = new DictionaryList<PlatformUserIdentifierAbs, BlockedPlayerList.ListEntry>();

		// Token: 0x0400A044 RID: 41028
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime lastWriteTime = DateTime.UtcNow;

		// Token: 0x0400A045 RID: 41029
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime? writeRequestTime;

		// Token: 0x0400A046 RID: 41030
		[PublicizedFrom(EAccessModifier.Private)]
		public ERoutineState readStorageState;

		// Token: 0x0400A047 RID: 41031
		[PublicizedFrom(EAccessModifier.Private)]
		public IRemotePlayerFileStorage.CallbackResult readStorageResult = IRemotePlayerFileStorage.CallbackResult.Other;

		// Token: 0x0400A048 RID: 41032
		[PublicizedFrom(EAccessModifier.Private)]
		public ERoutineState writeToStorageState;

		// Token: 0x0400A049 RID: 41033
		[PublicizedFrom(EAccessModifier.Private)]
		public ERoutineState resolveState;

		// Token: 0x02001B3C RID: 6972
		public class ListEntry
		{
			// Token: 0x170019A2 RID: 6562
			// (get) Token: 0x0600D111 RID: 53521 RVA: 0x004C7D0D File Offset: 0x004C5F0D
			// (set) Token: 0x0600D112 RID: 53522 RVA: 0x004C7D15 File Offset: 0x004C5F15
			public bool ResolvedOnce { get; [PublicizedFrom(EAccessModifier.Private)] set; }

			// Token: 0x170019A3 RID: 6563
			// (get) Token: 0x0600D113 RID: 53523 RVA: 0x004C7D1E File Offset: 0x004C5F1E
			// (set) Token: 0x0600D114 RID: 53524 RVA: 0x004C7D26 File Offset: 0x004C5F26
			public bool Blocked { get; [PublicizedFrom(EAccessModifier.Private)] set; }

			// Token: 0x0600D115 RID: 53525 RVA: 0x004C7D2F File Offset: 0x004C5F2F
			public ListEntry(PlayerData _playerData, DateTime _lastSeen, bool _blockState)
			{
				this.PlayerData = _playerData;
				this.LastSeen = _lastSeen;
				this.Blocked = _blockState;
			}

			// Token: 0x0600D116 RID: 53526 RVA: 0x004C7D4C File Offset: 0x004C5F4C
			public static BlockedPlayerList.ListEntry Read(BinaryReader _reader)
			{
				PlayerData playerData = PlayerData.Read(_reader);
				DateTime utcDateTime = DateTimeOffset.FromUnixTimeSeconds(_reader.ReadInt64()).UtcDateTime;
				bool blockState = _reader.ReadBoolean();
				return new BlockedPlayerList.ListEntry(playerData, utcDateTime, blockState);
			}

			// Token: 0x0600D117 RID: 53527 RVA: 0x004C7D84 File Offset: 0x004C5F84
			public void Write(BinaryWriter _writer)
			{
				this.PlayerData.Write(_writer);
				long value = new DateTimeOffset(this.LastSeen).ToUnixTimeSeconds();
				_writer.Write(value);
				_writer.Write(this.Blocked);
			}

			// Token: 0x0600D118 RID: 53528 RVA: 0x004C7DC4 File Offset: 0x004C5FC4
			public void SetResolvedOnce()
			{
				this.ResolvedOnce = true;
			}

			// Token: 0x0600D119 RID: 53529 RVA: 0x004C7DD0 File Offset: 0x004C5FD0
			public ValueTuple<bool, string> SetBlockState(bool _blockState)
			{
				if (this.Blocked == _blockState)
				{
					return new ValueTuple<bool, string>(false, null);
				}
				if (PlatformManager.NativePlatform.User.CanShowProfile(this.PlayerData.NativeId))
				{
					this.Blocked = false;
					Log.Warning(string.Format("[BlockedPlayerList] Cannot change block state of native user {0} through the block list", this.PlayerData.NativeId));
					return new ValueTuple<bool, string>(false, null);
				}
				if (_blockState && BlockedPlayerList.Instance.EntryCount(true, false) >= 500)
				{
					return new ValueTuple<bool, string>(false, Localization.Get("xuiBlockedPlayersCantAddMessage", false, null));
				}
				this.PlayerData.PlatformData.MarkBlockedStateChanged();
				BlockedPlayerList.Instance.MarkForWrite();
				this.Blocked = _blockState;
				return new ValueTuple<bool, string>(true, null);
			}

			// Token: 0x0400A04A RID: 41034
			public readonly PlayerData PlayerData;

			// Token: 0x0400A04B RID: 41035
			public readonly DateTime LastSeen;
		}
	}
}
