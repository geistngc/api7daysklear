using System;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.Permissions;

namespace Webserver.WebAPI.APIs.WorldState
{
	// Token: 0x02001ADC RID: 6876
	[Preserve]
	public class Player : AbsRestApi
	{
		// Token: 0x0600CEFE RID: 52990 RVA: 0x004B8344 File Offset: 0x004B6544
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			bool allowViewAll = PermissionUtils.CanViewAllPlayers(_context.PermissionLevel);
			WebConnection connection = _context.Connection;
			PlatformUserIdentifierAbs requesterNativeUserId = (connection != null) ? connection.UserId : null;
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(Player.jsonPlayersKey);
			jsonWriter.WriteBeginArray();
			int num = 0;
			if (string.IsNullOrEmpty(requestPath))
			{
				for (int i = 0; i < SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List.Count; i++)
				{
					ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List[i];
					this.writePlayerJson(ref jsonWriter, ref num, clientInfo.PlatformId, allowViewAll, requesterNativeUserId);
				}
			}
			else
			{
				int entityId;
				ClientInfo clientInfo2;
				if (!int.TryParse(requestPath, out entityId) || (clientInfo2 = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId)) == null)
				{
					jsonWriter.WriteEndArray();
					jsonWriter.WriteEndObject();
					AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.NotFound, null, null, null);
					return;
				}
				this.writePlayerJson(ref jsonWriter, ref num, clientInfo2.PlatformId, allowViewAll, requesterNativeUserId);
			}
			jsonWriter.WriteEndArray();
			jsonWriter.WriteEndObject();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CEFF RID: 52991 RVA: 0x004B8460 File Offset: 0x004B6660
		[PublicizedFrom(EAccessModifier.Private)]
		public void writePlayerJson(ref JsonWriter _writer, ref int _written, PlatformUserIdentifierAbs _nativeUserId, bool _allowViewAll, PlatformUserIdentifierAbs _requesterNativeUserId)
		{
			if (!_allowViewAll && (_requesterNativeUserId == null || !_requesterNativeUserId.Equals(_nativeUserId)))
			{
				return;
			}
			ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForUserId(_nativeUserId);
			if (clientInfo == null)
			{
				Log.Warning("[Web] Player.GET: ClientInfo null");
				return;
			}
			int entityId = clientInfo.entityId;
			EntityPlayer entityPlayer;
			GameManager.Instance.World.Players.dict.TryGetValue(entityId, out entityPlayer);
			if (entityPlayer == null)
			{
				Log.Warning("[Web] Player.GET: EntityPlayer null");
				return;
			}
			if (_written > 0)
			{
				_writer.WriteValueSeparator();
			}
			_written++;
			bool flag = true;
			_writer.WriteRaw(Player.jsonEntityIdKey);
			_writer.WriteInt32(entityId);
			_writer.WriteRaw(Player.jsonNameKey);
			_writer.WriteString(clientInfo.playerName);
			_writer.WriteRaw(Player.jsonPlatformIdKey);
			JsonCommons.WritePlatformUserIdentifier(ref _writer, _nativeUserId);
			_writer.WriteRaw(Player.jsonCrossplatformIdKey);
			JsonCommons.WritePlatformUserIdentifier(ref _writer, clientInfo.CrossplatformId);
			_writer.WriteRaw(Player.jsonTotalPlayTimeKey);
			_writer.WriteNull();
			_writer.WriteRaw(Player.jsonLastOnlineKey);
			_writer.WriteNull();
			_writer.WriteRaw(Player.jsonOnlineKey);
			_writer.WriteBoolean(flag);
			_writer.WriteRaw(Player.jsonIpKey);
			if (flag)
			{
				_writer.WriteString(clientInfo.ip);
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteRaw(Player.jsonPingKey);
			if (flag)
			{
				_writer.WriteInt32(clientInfo.ping);
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteRaw(Player.jsonPositionKey);
			if (flag)
			{
				JsonCommons.WriteVector3(ref _writer, entityPlayer.GetPosition());
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteRaw(Player.jsonLevelKey);
			_writer.WriteInt32(entityPlayer.Progression.Level);
			_writer.WriteRaw(Player.jsonHealthKey);
			_writer.WriteInt32(entityPlayer.Health);
			_writer.WriteRaw(Player.jsonStaminaKey);
			_writer.WriteSingle(entityPlayer.Stamina);
			_writer.WriteRaw(Player.jsonScoreKey);
			_writer.WriteInt32(entityPlayer.Score);
			_writer.WriteRaw(Player.jsonDeathsKey);
			_writer.WriteInt32(entityPlayer.Died);
			_writer.WriteRaw(Player.jsonKillsKey);
			_writer.WriteRaw(Player.jsonKillsZombiesKey);
			_writer.WriteInt32(entityPlayer.KilledZombies);
			_writer.WriteRaw(Player.jsonKillsPlayersKey);
			_writer.WriteInt32(entityPlayer.KilledPlayers);
			_writer.WriteEndObject();
			_writer.WriteRaw(Player.jsonBannedKey);
			DateTime dateTime;
			string value;
			bool flag2 = GameManager.Instance.adminTools.Blacklist.IsBanned(_nativeUserId, out dateTime, out value);
			if (!flag2 && clientInfo.CrossplatformId != null)
			{
				flag2 = GameManager.Instance.adminTools.Blacklist.IsBanned(clientInfo.CrossplatformId, out dateTime, out value);
			}
			_writer.WriteRaw(Player.jsonBanActiveKey);
			_writer.WriteBoolean(flag2);
			_writer.WriteRaw(Player.jsonBanReasonKey);
			if (flag2)
			{
				_writer.WriteString(value);
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteRaw(Player.jsonBanUntilKey);
			if (flag2)
			{
				JsonCommons.WriteDateTime(ref _writer, dateTime);
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteEndObject();
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF00 RID: 52992 RVA: 0x004B7F39 File Offset: 0x004B6139
		public override int DefaultPermissionLevel()
		{
			return 2000;
		}

		// Token: 0x0600CF01 RID: 52993 RVA: 0x004B25BC File Offset: 0x004B07BC
		public Player() : base(null)
		{
		}

		// Token: 0x04009D4E RID: 40270
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonPlayersKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("players");

		// Token: 0x04009D4F RID: 40271
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonEntityIdKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("entityId");

		// Token: 0x04009D50 RID: 40272
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonNameKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name");

		// Token: 0x04009D51 RID: 40273
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonPlatformIdKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("platformId");

		// Token: 0x04009D52 RID: 40274
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonCrossplatformIdKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("crossplatformId");

		// Token: 0x04009D53 RID: 40275
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonTotalPlayTimeKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("totalPlayTimeSeconds");

		// Token: 0x04009D54 RID: 40276
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonLastOnlineKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lastOnline");

		// Token: 0x04009D55 RID: 40277
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOnlineKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("online");

		// Token: 0x04009D56 RID: 40278
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonIpKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ip");

		// Token: 0x04009D57 RID: 40279
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonPingKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ping");

		// Token: 0x04009D58 RID: 40280
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonPositionKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("position");

		// Token: 0x04009D59 RID: 40281
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonLevelKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("level");

		// Token: 0x04009D5A RID: 40282
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonHealthKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("health");

		// Token: 0x04009D5B RID: 40283
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonStaminaKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("stamina");

		// Token: 0x04009D5C RID: 40284
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonScoreKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("score");

		// Token: 0x04009D5D RID: 40285
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonDeathsKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("deaths");

		// Token: 0x04009D5E RID: 40286
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKillsKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("kills");

		// Token: 0x04009D5F RID: 40287
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKillsZombiesKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("zombies");

		// Token: 0x04009D60 RID: 40288
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKillsPlayersKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("players");

		// Token: 0x04009D61 RID: 40289
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonBannedKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("banned");

		// Token: 0x04009D62 RID: 40290
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonBanActiveKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("banActive");

		// Token: 0x04009D63 RID: 40291
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonBanReasonKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("reason");

		// Token: 0x04009D64 RID: 40292
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonBanUntilKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("until");
	}
}
