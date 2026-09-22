using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Platform
{
	// Token: 0x02001BD3 RID: 7123
	public class PlatformConfiguration
	{
		// Token: 0x17001A38 RID: 6712
		// (get) Token: 0x0600D3F4 RID: 54260 RVA: 0x004CBC05 File Offset: 0x004C9E05
		// (set) Token: 0x0600D3F5 RID: 54261 RVA: 0x004CBC2D File Offset: 0x004C9E2D
		public EPlatformIdentifier NativePlatform
		{
			get
			{
				if (this.nativePlatform != EPlatformIdentifier.Count)
				{
					return this.nativePlatform;
				}
				Log.Warning(string.Format("[Platform] Platform config file has no valid entry for platform, defaulting to {0}", EPlatformIdentifier.Local));
				return EPlatformIdentifier.Local;
			}
			set
			{
				this.nativePlatform = value;
			}
		}

		// Token: 0x17001A39 RID: 6713
		// (get) Token: 0x0600D3F6 RID: 54262 RVA: 0x004CBC36 File Offset: 0x004C9E36
		// (set) Token: 0x0600D3F7 RID: 54263 RVA: 0x004CBC5E File Offset: 0x004C9E5E
		public EPlatformIdentifier CrossPlatform
		{
			get
			{
				if (this.crossPlatform != EPlatformIdentifier.Count)
				{
					return this.crossPlatform;
				}
				Log.Warning(string.Format("[Platform] Platform config file has no valid entry for cross platform, defaulting to {0}", EPlatformIdentifier.None));
				return EPlatformIdentifier.None;
			}
			set
			{
				this.crossPlatform = value;
			}
		}

		// Token: 0x0600D3F8 RID: 54264 RVA: 0x004CBC68 File Offset: 0x004C9E68
		public bool ParsePlatform(string _platformGroup, string _value)
		{
			if (string.IsNullOrEmpty(_platformGroup))
			{
				return false;
			}
			if (string.IsNullOrEmpty(_value))
			{
				return false;
			}
			_value = _value.Trim();
			if (_platformGroup == "platform")
			{
				EPlatformIdentifier eplatformIdentifier;
				if (!PlatformManager.TryPlatformIdentifierFromString(_value, out eplatformIdentifier))
				{
					Log.Warning("[Platform] Can not parse platform name '" + _value + "'");
				}
				else
				{
					this.nativePlatform = eplatformIdentifier;
				}
				return true;
			}
			if (_platformGroup == "crossplatform")
			{
				EPlatformIdentifier eplatformIdentifier;
				if (!PlatformManager.TryPlatformIdentifierFromString(_value, out eplatformIdentifier))
				{
					Log.Warning("[Platform] Can not parse cross platform name '" + _value + "'");
				}
				else
				{
					this.crossPlatform = eplatformIdentifier;
				}
				return true;
			}
			if (!(_platformGroup == "serverplatforms"))
			{
				Log.Warning("[Platform] Unsupported platform group specifier '" + _platformGroup + "'");
				return false;
			}
			this.ServerPlatforms.Clear();
			string[] array = _value.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (!string.IsNullOrEmpty(text))
				{
					EPlatformIdentifier eplatformIdentifier;
					if (!PlatformManager.TryPlatformIdentifierFromString(text, out eplatformIdentifier))
					{
						Log.Warning("[Platform] Can not parse server platform name '" + text + "'");
					}
					else if (eplatformIdentifier == EPlatformIdentifier.Count || eplatformIdentifier == EPlatformIdentifier.None)
					{
						Log.Warning("[Platform] Unsupported platform for server operations '" + text + "'");
					}
					else
					{
						this.ServerPlatforms.Add(eplatformIdentifier);
					}
				}
			}
			return true;
		}

		// Token: 0x0600D3F9 RID: 54265 RVA: 0x004CBDA8 File Offset: 0x004C9FA8
		public string WriteString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("platform=");
			stringBuilder.AppendLine(PlatformManager.PlatformStringFromEnum(this.NativePlatform));
			stringBuilder.Append("crossplatform=");
			stringBuilder.AppendLine(PlatformManager.PlatformStringFromEnum(this.CrossPlatform));
			stringBuilder.Append("serverplatforms=");
			foreach (EPlatformIdentifier platformIdentifier in this.ServerPlatforms)
			{
				stringBuilder.Append(PlatformManager.PlatformStringFromEnum(platformIdentifier));
				stringBuilder.Append(",");
			}
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x0600D3FA RID: 54266 RVA: 0x004CBE68 File Offset: 0x004CA068
		public void WriteFile(string _configFilename = null)
		{
			if (_configFilename == null)
			{
				_configFilename = GameIO.GetApplicationPath() + "/platform.cfg";
			}
			string contents = this.WriteString();
			File.WriteAllText(_configFilename, contents);
		}

		// Token: 0x0600D3FB RID: 54267 RVA: 0x004CBE98 File Offset: 0x004CA098
		public static bool ReadString(ref PlatformConfiguration _result, string _config)
		{
			if (_result == null)
			{
				_result = new PlatformConfiguration();
			}
			using (StringReader stringReader = new StringReader(_config))
			{
				PlatformConfiguration.Parse(ref _result, stringReader);
			}
			return true;
		}

		// Token: 0x0600D3FC RID: 54268 RVA: 0x004CBEDC File Offset: 0x004CA0DC
		public static bool ReadFile(ref PlatformConfiguration _result, string _configFilename = null)
		{
			if (_result == null)
			{
				_result = new PlatformConfiguration();
			}
			if (_configFilename == null)
			{
				_configFilename = GameIO.GetApplicationPath() + "/platform.cfg";
			}
			if (!File.Exists(_configFilename))
			{
				return false;
			}
			using (StreamReader streamReader = File.OpenText(_configFilename))
			{
				PlatformConfiguration.Parse(ref _result, streamReader);
			}
			return true;
		}

		// Token: 0x0600D3FD RID: 54269 RVA: 0x004CBF40 File Offset: 0x004CA140
		[PublicizedFrom(EAccessModifier.Private)]
		public static void Parse(ref PlatformConfiguration _result, TextReader _stream)
		{
			while (_stream.Peek() >= 0)
			{
				string[] array = _stream.ReadLine().Split('=', StringSplitOptions.None);
				if (array.Length == 2)
				{
					_result.ParsePlatform(array[0], array[1]);
				}
			}
		}

		// Token: 0x0400A19F RID: 41375
		[PublicizedFrom(EAccessModifier.Private)]
		public const EPlatformIdentifier defaultNativePlatform = EPlatformIdentifier.Local;

		// Token: 0x0400A1A0 RID: 41376
		[PublicizedFrom(EAccessModifier.Private)]
		public const EPlatformIdentifier defaultCrossPlatform = EPlatformIdentifier.None;

		// Token: 0x0400A1A1 RID: 41377
		[PublicizedFrom(EAccessModifier.Private)]
		public EPlatformIdentifier nativePlatform = EPlatformIdentifier.Count;

		// Token: 0x0400A1A2 RID: 41378
		[PublicizedFrom(EAccessModifier.Private)]
		public EPlatformIdentifier crossPlatform = EPlatformIdentifier.Count;

		// Token: 0x0400A1A3 RID: 41379
		public readonly List<EPlatformIdentifier> ServerPlatforms = new List<EPlatformIdentifier>();
	}
}
