using System;

namespace Platform
{
	// Token: 0x02001BBA RID: 7098
	public abstract class UserDataRoamingAbs : IUserDataRoaming
	{
		// Token: 0x0600D38C RID: 54156 RVA: 0x004CB0D3 File Offset: 0x004C92D3
		public void Init(IPlatform platform)
		{
			this.platform = platform;
		}

		// Token: 0x17001A29 RID: 6697
		// (get) Token: 0x0600D38D RID: 54157 RVA: 0x004CB0DC File Offset: 0x004C92DC
		public bool IsSupported
		{
			get
			{
				return this.platform.SaveGameProvider != null;
			}
		}

		// Token: 0x17001A2A RID: 6698
		// (get) Token: 0x0600D38E RID: 54158
		public abstract SaveRoamingMode SaveRoamingMode { get; }

		// Token: 0x17001A2B RID: 6699
		// (get) Token: 0x0600D38F RID: 54159 RVA: 0x004CB0EC File Offset: 0x004C92EC
		public bool SaveRoamingEnabled
		{
			get
			{
				return this.IsSupported && (this.SaveRoamingMode == SaveRoamingMode.Optional || this.SaveRoamingMode == SaveRoamingMode.Forced);
			}
		}

		// Token: 0x17001A2C RID: 6700
		// (get) Token: 0x0600D390 RID: 54160 RVA: 0x004CB10C File Offset: 0x004C930C
		public bool IsRoamingOptional
		{
			get
			{
				return this.SaveRoamingMode == SaveRoamingMode.Optional;
			}
		}

		// Token: 0x17001A2D RID: 6701
		// (get) Token: 0x0600D391 RID: 54161
		public abstract UserDataStorageType DefaultSaveStorage { get; }

		// Token: 0x0600D392 RID: 54162 RVA: 0x004CB117 File Offset: 0x004C9317
		public void ValidateRoamingMode()
		{
			this.ValidateStoragePref(EnumGamePrefs.GameSaveStorageType);
			this.ValidateStoragePref(EnumGamePrefs.UserWorldStorageType);
		}

		// Token: 0x0600D393 RID: 54163 RVA: 0x004CB130 File Offset: 0x004C9330
		[PublicizedFrom(EAccessModifier.Private)]
		public void ValidateStoragePref(EnumGamePrefs pref)
		{
			UserDataStorageType @int = (UserDataStorageType)GamePrefs.GetInt(pref);
			UserDataStorageType userDataStorageType = @int;
			SaveRoamingMode saveRoamingMode = this.SaveRoamingMode;
			if (saveRoamingMode != SaveRoamingMode.None)
			{
				if (saveRoamingMode == SaveRoamingMode.Forced)
				{
					userDataStorageType = UserDataStorageType.Roaming;
				}
			}
			else
			{
				userDataStorageType = UserDataStorageType.DeviceLocal;
			}
			if (@int != userDataStorageType)
			{
				Log.Out(string.Format("UserDataRoaming invalid storage pref {0} configured for {1}. Platform roaming mode is {2}. Changing to {3}", new object[]
				{
					@int,
					pref.ToStringCached<EnumGamePrefs>(),
					this.SaveRoamingMode,
					userDataStorageType
				}));
				GamePrefs.Set(pref, (int)userDataStorageType);
			}
		}

		// Token: 0x0600D394 RID: 54164 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public UserDataRoamingAbs()
		{
		}

		// Token: 0x0400A172 RID: 41330
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatform platform;
	}
}
