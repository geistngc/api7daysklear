using System;
using System.Net;

namespace Platform
{
	// Token: 0x02001BCB RID: 7115
	public static class NetworkUtils
	{
		// Token: 0x0600D3CA RID: 54218 RVA: 0x004CB24A File Offset: 0x004C944A
		public static uint ToInt(string _addr)
		{
			return (uint)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(IPAddress.Parse(_addr).GetAddressBytes(), 0));
		}

		// Token: 0x0600D3CB RID: 54219 RVA: 0x004CB264 File Offset: 0x004C9464
		public static string ToAddr(uint _address)
		{
			string[] array = new IPAddress((long)((ulong)_address)).ToString().Split('.', StringSplitOptions.None);
			return string.Concat(new string[]
			{
				array[3],
				".",
				array[2],
				".",
				array[1],
				".",
				array[0]
			});
		}

		// Token: 0x0600D3CC RID: 54220 RVA: 0x004CB2C4 File Offset: 0x004C94C4
		public static string BuildGameTags(GameServerInfo _game)
		{
			string result;
			using (PooledMemoryStream pooledMemoryStream = MemoryPools.poolMS.AllocSync(true))
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(true))
				{
					pooledBinaryWriter.SetBaseStream(pooledMemoryStream);
					foreach (GameInfoInt key in GameServerInfo.IntInfosInGameTags)
					{
						pooledBinaryWriter.Write7BitEncodedSignedInt(_game.GetValue(key));
					}
					byte b = 0;
					int j;
					for (j = 0; j < GameServerInfo.BoolInfosInGameTags.Length; j++)
					{
						b |= (byte)((_game.GetValue(GameServerInfo.BoolInfosInGameTags[j]) ? 1 : 0) << (j % 8 & 31));
						if (j % 8 == 7)
						{
							pooledBinaryWriter.Write(b);
							b = 0;
						}
					}
					if (j % 8 != 0)
					{
						pooledBinaryWriter.Write(b);
					}
					result = Convert.ToBase64String(pooledMemoryStream.GetBuffer(), 0, (int)pooledMemoryStream.Length);
				}
			}
			return result;
		}

		// Token: 0x0600D3CD RID: 54221 RVA: 0x004CB3B8 File Offset: 0x004C95B8
		public static bool ParseGameTags(string _tags, GameServerInfo _gameInfo)
		{
			return _tags.IndexOf(';') < 0 && NetworkUtils.ParseGameTags2(_tags, _gameInfo);
		}

		// Token: 0x0600D3CE RID: 54222 RVA: 0x004CB3D0 File Offset: 0x004C95D0
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool ParseGameTags2(string _tags, GameServerInfo _gameInfo)
		{
			byte[] array;
			try
			{
				array = Convert.FromBase64String(_tags);
			}
			catch (Exception)
			{
				Log.Warning(string.Concat(new string[]
				{
					"Parsing gametags for server ",
					_gameInfo.GetValue(GameInfoString.IP),
					":",
					_gameInfo.GetValue(GameInfoInt.Port).ToString(),
					" failed: \"",
					_tags,
					"\""
				}));
				return false;
			}
			bool result;
			using (PooledMemoryStream pooledMemoryStream = MemoryPools.poolMS.AllocSync(true))
			{
				pooledMemoryStream.Write(array, 0, array.Length);
				pooledMemoryStream.Position = 0L;
				using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
				{
					pooledBinaryReader.SetBaseStream(pooledMemoryStream);
					try
					{
						foreach (GameInfoInt key in GameServerInfo.IntInfosInGameTags)
						{
							int value = pooledBinaryReader.Read7BitEncodedSignedInt();
							_gameInfo.SetValue(key, value);
						}
						for (int j = 0; j < GameServerInfo.BoolInfosInGameTags.Length; j += 8)
						{
							byte b = pooledBinaryReader.ReadByte();
							int num = 0;
							while (j + num < GameServerInfo.BoolInfosInGameTags.Length && num < 8)
							{
								GameInfoBool key2 = GameServerInfo.BoolInfosInGameTags[j + num];
								bool value2 = ((int)b & 1 << num) != 0;
								_gameInfo.SetValue(key2, value2);
								num++;
							}
						}
					}
					catch (Exception)
					{
						return false;
					}
					result = true;
				}
			}
			return result;
		}
	}
}
