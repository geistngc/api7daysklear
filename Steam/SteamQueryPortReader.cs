using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C95 RID: 7317
	public class SteamQueryPortReader
	{
		// Token: 0x14000136 RID: 310
		// (add) Token: 0x0600D8E7 RID: 55527 RVA: 0x004E11C0 File Offset: 0x004DF3C0
		// (remove) Token: 0x0600D8E8 RID: 55528 RVA: 0x004E11F8 File Offset: 0x004DF3F8
		[method: PublicizedFrom(EAccessModifier.Private)]
		public event GameServerDetailsCallback GameServerDetailsEvent;

		// Token: 0x0600D8E9 RID: 55529 RVA: 0x004E1230 File Offset: 0x004DF430
		public void Init(IPlatform _owner)
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			if (this.matchmakingRulesResponse != null)
			{
				return;
			}
			this.matchmakingRulesResponse = new ISteamMatchmakingRulesResponse(new ISteamMatchmakingRulesResponse.RulesResponded(this.RulesResponded), new ISteamMatchmakingRulesResponse.RulesFailedToRespond(this.RulesFailedToRespond), new ISteamMatchmakingRulesResponse.RulesRefreshComplete(this.RulesRefreshComplete));
		}

		// Token: 0x0600D8EA RID: 55530 RVA: 0x004E127D File Offset: 0x004DF47D
		public void Disconnect()
		{
			if (this.rulesRequestHandle != HServerQuery.Invalid)
			{
				SteamMatchmakingServers.CancelServerQuery(this.rulesRequestHandle);
				this.rulesRequestHandle = HServerQuery.Invalid;
			}
			this.GameServerDetailsEvent = null;
		}

		// Token: 0x0600D8EB RID: 55531 RVA: 0x004E12AE File Offset: 0x004DF4AE
		public void RegisterGameServerCallbacks(GameServerDetailsCallback _details)
		{
			this.GameServerDetailsEvent = _details;
		}

		// Token: 0x0600D8EC RID: 55532 RVA: 0x004E12B7 File Offset: 0x004DF4B7
		[PublicizedFrom(EAccessModifier.Private)]
		public void RunGameServerDetailsEvent(GameServerInfo _info, bool _success)
		{
			GameServerDetailsCallback gameServerDetailsEvent = this.GameServerDetailsEvent;
			if (gameServerDetailsEvent == null)
			{
				return;
			}
			gameServerDetailsEvent(_info, _success);
		}

		// Token: 0x0600D8ED RID: 55533 RVA: 0x004E12CC File Offset: 0x004DF4CC
		public void GetGameServerInfo(GameServerInfo _gameInfo)
		{
			if (_gameInfo.IsLobby)
			{
				this.RunGameServerDetailsEvent(_gameInfo, true);
				return;
			}
			if (_gameInfo.IsNoResponse)
			{
				this.RunGameServerDetailsEvent(_gameInfo, true);
			}
			string text = _gameInfo.GetValue(GameInfoString.IP);
			long num;
			if (!long.TryParse(text.Replace(".", ""), out num))
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(text);
					if (hostEntry.AddressList.Length == 0)
					{
						Log.Out("Steamworks.NET] No valid IP for server found");
						this.RunGameServerDetailsEvent(_gameInfo, false);
						return;
					}
					text = hostEntry.AddressList[0].ToString();
				}
				catch (SocketException ex)
				{
					string str = "Steamworks.NET] No such hostname: \"";
					string str2 = text;
					string str3 = "\": ";
					SocketException ex2 = ex;
					Log.Out(str + str2 + str3 + ((ex2 != null) ? ex2.ToString() : null));
					this.RunGameServerDetailsEvent(_gameInfo, false);
					return;
				}
			}
			SteamQueryPortReader.RulesRequest item = new SteamQueryPortReader.RulesRequest
			{
				GameInfo = _gameInfo,
				Ip = NetworkUtils.ToInt(text),
				Port = (ushort)_gameInfo.GetValue(GameInfoInt.Port)
			};
			this.rulesRequests.Enqueue(item);
			if (this.rulesRequestHandle == HServerQuery.Invalid)
			{
				this.StartNextRulesRequest();
			}
		}

		// Token: 0x0600D8EE RID: 55534 RVA: 0x004E13DC File Offset: 0x004DF5DC
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartNextRulesRequest()
		{
			this.currentRulesRequest = null;
			this.rulesRequestHandle = HServerQuery.Invalid;
			if (this.rulesRequests.Count > 0)
			{
				this.currentRulesRequest = this.rulesRequests.Dequeue();
				this.currentRulesRequest.GameInfoClone = new GameServerInfo(this.currentRulesRequest.GameInfo);
				this.rulesRequestHandle = SteamMatchmakingServers.ServerRules(this.currentRulesRequest.Ip, this.currentRulesRequest.Port, this.matchmakingRulesResponse);
			}
		}

		// Token: 0x0600D8EF RID: 55535 RVA: 0x004E145C File Offset: 0x004DF65C
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesFailedToRespond()
		{
			this.RunGameServerDetailsEvent(this.currentRulesRequest.GameInfo, false);
			this.StartNextRulesRequest();
		}

		// Token: 0x0600D8F0 RID: 55536 RVA: 0x004E1478 File Offset: 0x004DF678
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesRefreshComplete()
		{
			if (!this.currentRulesRequest.DataErrors && this.currentRulesRequest.GameInfoClone.GetValue(GameInfoString.GameName).Length > 0)
			{
				this.currentRulesRequest.GameInfo.Merge(this.currentRulesRequest.GameInfoClone, this.currentRulesRequest.GameInfo.IsLAN ? EServerRelationType.LAN : EServerRelationType.Internet);
				this.RunGameServerDetailsEvent(this.currentRulesRequest.GameInfo, true);
			}
			else
			{
				if (this.currentRulesRequest.DataErrors)
				{
					this.currentRulesRequest.GameInfo.SetValue(GameInfoString.ServerDescription, Localization.Get("xuiServerBrowserFailedRetrievingData", false, null));
				}
				this.RunGameServerDetailsEvent(this.currentRulesRequest.GameInfo, false);
			}
			this.StartNextRulesRequest();
		}

		// Token: 0x0600D8F1 RID: 55537 RVA: 0x004E1534 File Offset: 0x004DF734
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesResponded(string _rule, string _value)
		{
			SteamQueryPortReader.RulesRequest rulesRequest = this.currentRulesRequest;
			if (rulesRequest.DataErrors)
			{
				return;
			}
			if (_rule.EqualsCaseInsensitive("gameinfo") || _rule.EqualsCaseInsensitive("ping"))
			{
				return;
			}
			if (rulesRequest.GameInfoClone.IsLAN && _rule.EqualsCaseInsensitive("ip"))
			{
				return;
			}
			if (!rulesRequest.GameInfoClone.ParseAny(_rule, _value))
			{
				rulesRequest.DataErrors = true;
			}
		}

		// Token: 0x0400A4E2 RID: 42210
		[PublicizedFrom(EAccessModifier.Private)]
		public ISteamMatchmakingRulesResponse matchmakingRulesResponse;

		// Token: 0x0400A4E3 RID: 42211
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Queue<SteamQueryPortReader.RulesRequest> rulesRequests = new Queue<SteamQueryPortReader.RulesRequest>();

		// Token: 0x0400A4E4 RID: 42212
		[PublicizedFrom(EAccessModifier.Private)]
		public SteamQueryPortReader.RulesRequest currentRulesRequest;

		// Token: 0x0400A4E5 RID: 42213
		[PublicizedFrom(EAccessModifier.Private)]
		public HServerQuery rulesRequestHandle = HServerQuery.Invalid;

		// Token: 0x02001C96 RID: 7318
		[PublicizedFrom(EAccessModifier.Private)]
		public class RulesRequest
		{
			// Token: 0x0400A4E6 RID: 42214
			public uint Ip;

			// Token: 0x0400A4E7 RID: 42215
			public ushort Port;

			// Token: 0x0400A4E8 RID: 42216
			public GameServerInfo GameInfo;

			// Token: 0x0400A4E9 RID: 42217
			public GameServerInfo GameInfoClone;

			// Token: 0x0400A4EA RID: 42218
			public bool DataErrors;
		}
	}
}
