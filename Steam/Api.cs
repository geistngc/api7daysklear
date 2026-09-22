using System;
using System.Text;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C83 RID: 7299
	public class Api : IPlatformApi
	{
		// Token: 0x17001AD3 RID: 6867
		// (get) Token: 0x0600D85E RID: 55390 RVA: 0x004DF8D2 File Offset: 0x004DDAD2
		// (set) Token: 0x0600D85F RID: 55391 RVA: 0x004DF8DA File Offset: 0x004DDADA
		public EApiStatus ClientApiStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EApiStatus.Uninitialized;

		// Token: 0x14000135 RID: 309
		// (add) Token: 0x0600D860 RID: 55392 RVA: 0x004DF8E4 File Offset: 0x004DDAE4
		// (remove) Token: 0x0600D861 RID: 55393 RVA: 0x004DF940 File Offset: 0x004DDB40
		public event Action ClientApiInitialized
		{
			add
			{
				lock (this)
				{
					this.clientApiInitialized = (Action)Delegate.Combine(this.clientApiInitialized, value);
					if (this.ClientApiStatus == EApiStatus.Ok)
					{
						value();
					}
				}
			}
			remove
			{
				lock (this)
				{
					this.clientApiInitialized = (Action)Delegate.Remove(this.clientApiInitialized, value);
				}
			}
		}

		// Token: 0x0600D862 RID: 55394 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600D863 RID: 55395 RVA: 0x004DF98C File Offset: 0x004DDB8C
		public bool InitClientApis()
		{
			if (!Packsize.Test())
			{
				Log.Out("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
				this.ClientApiStatus = EApiStatus.PermanentError;
				return false;
			}
			if (!DllCheck.Test())
			{
				Log.Out("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
				this.ClientApiStatus = EApiStatus.PermanentError;
				return false;
			}
			try
			{
				if (!SteamAPI.Init())
				{
					Log.Out("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
					this.ClientApiStatus = EApiStatus.TemporaryError;
					return false;
				}
			}
			catch (DllNotFoundException ex)
			{
				string str = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
				DllNotFoundException ex2 = ex;
				Log.Out(str + ((ex2 != null) ? ex2.ToString() : null));
				this.ClientApiStatus = EApiStatus.PermanentError;
				return false;
			}
			Log.Out("[Steamworks.NET] SteamAPI_Init() ok");
			SteamClient.SetWarningMessageHook(new SteamAPIWarningMessageHook_t(this.ExceptionThrown));
			SteamUtils.SetOverlayNotificationPosition(ENotificationPosition.k_EPositionTopRight);
			this.ClientApiStatus = EApiStatus.Ok;
			Action action = this.clientApiInitialized;
			if (action != null)
			{
				action();
			}
			return true;
		}

		// Token: 0x0600D864 RID: 55396 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool InitServerApis()
		{
			return true;
		}

		// Token: 0x0600D865 RID: 55397 RVA: 0x004DFA60 File Offset: 0x004DDC60
		public void ServerApiLoaded()
		{
			if (this.ClientApiStatus != EApiStatus.Ok)
			{
				Action action = this.clientApiInitialized;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x0600D866 RID: 55398 RVA: 0x004DFA7C File Offset: 0x004DDC7C
		public void Update()
		{
			if (this.ClientApiStatus == EApiStatus.Ok)
			{
				this.tickDurationStopwatch.Restart();
				SteamAPI.RunCallbacks();
				long num = this.tickDurationStopwatch.ElapsedMicroseconds / 1000L;
				if (num > 25L)
				{
					Log.Warning(string.Format("[Steam] Tick took exceptionally long: {0} ms", num));
				}
			}
		}

		// Token: 0x0600D867 RID: 55399 RVA: 0x004DFACF File Offset: 0x004DDCCF
		public void Destroy()
		{
			if (this.ClientApiStatus == EApiStatus.Ok)
			{
				SteamAPI.Shutdown();
			}
		}

		// Token: 0x0600D868 RID: 55400 RVA: 0x004DFADE File Offset: 0x004DDCDE
		[PublicizedFrom(EAccessModifier.Private)]
		public void ExceptionThrown(int _severity, StringBuilder _message)
		{
			Log.Error("[Steamworks.NET] " + ((_severity == 0) ? "Info: " : "Warning: ") + ": " + ((_message != null) ? _message.ToString() : null));
		}

		// Token: 0x0600D869 RID: 55401 RVA: 0x00040FA0 File Offset: 0x0003F1A0
		public float GetScreenBoundsValueFromSystem()
		{
			return 1f;
		}

		// Token: 0x0400A4B0 RID: 42160
		[PublicizedFrom(EAccessModifier.Private)]
		public Action clientApiInitialized;

		// Token: 0x0400A4B1 RID: 42161
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MicroStopwatch tickDurationStopwatch = new MicroStopwatch(false);
	}
}
