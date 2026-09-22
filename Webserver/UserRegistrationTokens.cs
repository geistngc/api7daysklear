using System;
using System.Collections.Generic;

namespace Webserver
{
	// Token: 0x02001AC5 RID: 6853
	public static class UserRegistrationTokens
	{
		// Token: 0x0600CE7F RID: 52863 RVA: 0x004B56E8 File Offset: 0x004B38E8
		public static bool TryValidate(string _token, out UserRegistrationTokens.RegistrationData _data)
		{
			return UserRegistrationTokens.activeTokens.TryGetValue(_token, out _data) && _data.ExpiryTime > DateTime.Now;
		}

		// Token: 0x0600CE80 RID: 52864 RVA: 0x004B570C File Offset: 0x004B390C
		public static string CreateToken(string _playerName, PlatformUserIdentifierAbs _platformUserId, PlatformUserIdentifierAbs _crossPlatformUserId)
		{
			string text = Utils.GenerateGuid();
			DateTime currentTime = DateTime.Now;
			UserRegistrationTokens.activeTokens.RemoveAll(delegate(UserRegistrationTokens.RegistrationData _data)
			{
				if (!(_data.ExpiryTime < currentTime) && !_platformUserId.Equals(_data.PlatformUserId))
				{
					PlatformUserIdentifierAbs crossPlatformUserId = _crossPlatformUserId;
					return crossPlatformUserId != null && crossPlatformUserId.Equals(_data.CrossPlatformUserId);
				}
				return true;
			});
			UserRegistrationTokens.RegistrationData value = new UserRegistrationTokens.RegistrationData(_playerName, _platformUserId, _crossPlatformUserId);
			UserRegistrationTokens.activeTokens[text] = value;
			return text;
		}

		// Token: 0x04009CB8 RID: 40120
		[PublicizedFrom(EAccessModifier.Private)]
		public const float tokenExpirationMinutes = 3f;

		// Token: 0x04009CB9 RID: 40121
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<string, UserRegistrationTokens.RegistrationData> activeTokens = new Dictionary<string, UserRegistrationTokens.RegistrationData>();

		// Token: 0x02001AC6 RID: 6854
		public class RegistrationData
		{
			// Token: 0x0600CE82 RID: 52866 RVA: 0x004B5780 File Offset: 0x004B3980
			public RegistrationData(string _playerName, PlatformUserIdentifierAbs _platformUserId, PlatformUserIdentifierAbs _crossPlatformUserId)
			{
				this.PlayerName = _playerName;
				this.ExpiryTime = DateTime.Now + TimeSpan.FromMinutes(3.0);
				this.PlatformUserId = _platformUserId;
				this.CrossPlatformUserId = _crossPlatformUserId;
			}

			// Token: 0x04009CBA RID: 40122
			public readonly string PlayerName;

			// Token: 0x04009CBB RID: 40123
			public readonly DateTime ExpiryTime;

			// Token: 0x04009CBC RID: 40124
			public readonly PlatformUserIdentifierAbs PlatformUserId;

			// Token: 0x04009CBD RID: 40125
			public readonly PlatformUserIdentifierAbs CrossPlatformUserId;
		}
	}
}
