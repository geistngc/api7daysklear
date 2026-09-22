using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001C1E RID: 7198
	public static class XblHelpers
	{
		// Token: 0x1400012F RID: 303
		// (add) Token: 0x0600D5CD RID: 54733 RVA: 0x004D2BC0 File Offset: 0x004D0DC0
		// (remove) Token: 0x0600D5CE RID: 54734 RVA: 0x004D2BF4 File Offset: 0x004D0DF4
		public static event XblHelpers.ErrorDelegate OnError;

		// Token: 0x0600D5CF RID: 54735 RVA: 0x004D2C28 File Offset: 0x004D0E28
		public static bool Succeeded(int _hresult, string _operationFriendlyName, bool _logToConsole = true, bool _printSuccess = false)
		{
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(_hresult))
			{
				if (_printSuccess && _logToConsole)
				{
					Log.Out("[XBL] Success: " + _operationFriendlyName);
				}
				return true;
			}
			string text;
			if (!XblHelpers.hresultToFriendlyErrorLookup.TryGetValue(_hresult, out text))
			{
				text = _operationFriendlyName + " failed. Error code: " + XblHelpers.GetHRName(_hresult);
			}
			if (_logToConsole)
			{
				Log.Error(string.Format("[XBL] Error: 0x{0:X8} - {1}", _hresult, text));
			}
			XblHelpers.ErrorDelegate onError = XblHelpers.OnError;
			if (onError != null)
			{
				onError(_hresult, _operationFriendlyName, text);
			}
			return false;
		}

		// Token: 0x0600D5D0 RID: 54736 RVA: 0x004D2CA4 File Offset: 0x004D0EA4
		[PublicizedFrom(EAccessModifier.Private)]
		static XblHelpers()
		{
			XblHelpers.hresultToFriendlyErrorLookup[-2143330041] = "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, have you published it in Partner Center?";
			XblHelpers.hresultToFriendlyErrorLookup[-2015035361] = "Missing Game Config";
		}

		// Token: 0x0600D5D1 RID: 54737 RVA: 0x004D2D64 File Offset: 0x004D0F64
		public static string GetHRName(int hr)
		{
			string result;
			if (!XblHelpers.s_hrToName.TryGetValue(hr, out result))
			{
				return "UNKNOWN";
			}
			return result;
		}

		// Token: 0x0600D5D2 RID: 54738 RVA: 0x004D2D88 File Offset: 0x004D0F88
		public static void LogHR(int hr, string identifier, bool failWarn = false)
		{
			bool flag = Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr);
			string text = flag ? "SUCCEEDED" : "FAILED";
			string txt = string.Format("[HResult] {0} (0x{1:X8} = {2}) {3}", new object[]
			{
				text,
				hr,
				XblHelpers.GetHRName(hr),
				identifier
			});
			if (flag)
			{
				Log.Out(txt);
				return;
			}
			if (failWarn)
			{
				Log.Warning(txt);
				return;
			}
			Log.Error(txt);
		}

		// Token: 0x0400A2DE RID: 41694
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<int, string> hresultToFriendlyErrorLookup = new Dictionary<int, string>();

		// Token: 0x0400A2DF RID: 41695
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly IReadOnlyDictionary<int, string> s_hrToName = (from f in new Type[]
		{
			typeof(Unity.XGamingRuntime.Interop.HR),
			typeof(Unity.XGamingRuntime.HR),
			typeof(HREx)
		}.SelectMany((Type t) => t.GetFields(BindingFlags.Static | BindingFlags.Public))
		where f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(int)
		select f).ToDictionary((FieldInfo f) => (int)f.GetValue(null), (FieldInfo f) => f.Name);

		// Token: 0x02001C1F RID: 7199
		// (Invoke) Token: 0x0600D5D4 RID: 54740
		public delegate void ErrorDelegate(int _hresult, string _operationFriendlyName, string _errorMessage);
	}
}
