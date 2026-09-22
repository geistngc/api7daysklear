using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Platform.XBL
{
	// Token: 0x02001C0A RID: 7178
	public class TextCensor : ITextCensor
	{
		// Token: 0x0600D53D RID: 54589 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600D53E RID: 54590 RVA: 0x004D0C24 File Offset: 0x004CEE24
		public void Update()
		{
			if (this.fetchStarted)
			{
				return;
			}
			this.fetchStarted = true;
			if (PlatformManager.MultiPlatform.RemoteFileStorage != null)
			{
				ThreadManager.StartCoroutine(this.RetrieveBannedWords());
			}
		}

		// Token: 0x0600D53F RID: 54591 RVA: 0x004D0C4E File Offset: 0x004CEE4E
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator RetrieveBannedWords()
		{
			IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
			if (storage == null)
			{
				yield break;
			}
			while (!storage.IsReady)
			{
				yield return null;
			}
			storage.GetFile("BannedWordsXBL.txt", new IRemoteFileStorage.FileDownloadCompleteCallback(this.StorageProviderCallback));
			yield break;
		}

		// Token: 0x0600D540 RID: 54592 RVA: 0x004D0C60 File Offset: 0x004CEE60
		[PublicizedFrom(EAccessModifier.Private)]
		public void StorageProviderCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorDetails, byte[] _data)
		{
			if (_result != IRemoteFileStorage.EFileDownloadResult.Ok)
			{
				Log.Warning(string.Concat(new string[]
				{
					"Retrieving banned words list failed: ",
					_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
					" (",
					_errorDetails,
					")"
				}));
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream(_data))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8))
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						if (Regex.IsMatch(text, "^[a-zA-Z0-9]+$"))
						{
							this.bannedPatterns.Add("(?<![a-zA-Z])" + Regex.Escape(text) + "(?![a-zA-Z])");
						}
						else
						{
							this.bannedPatterns.Add(Regex.Escape(text));
						}
					}
				}
			}
			this.fetchComplete = true;
		}

		// Token: 0x0600D541 RID: 54593 RVA: 0x004D0D40 File Offset: 0x004CEF40
		public void CensorProfanity(string _input, PlatformUserIdentifierAbs _author, Action<CensoredTextResult> _callback)
		{
			if (string.IsNullOrEmpty(_input) || _input.Length == 0 || !this.fetchComplete)
			{
				_callback(new CensoredTextResult(true, _input, _input));
				return;
			}
			if (_author != null)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (((crossplatformPlatform != null) ? crossplatformPlatform.User : null) != null && PlatformManager.CrossplatformPlatform.User.PlatformUserId.Equals(_author))
				{
					_callback(new CensoredTextResult(true, _input, _input));
					return;
				}
				IPlatform nativePlatform = PlatformManager.NativePlatform;
				if (((nativePlatform != null) ? nativePlatform.User : null) != null && (PlatformManager.NativePlatform.User.PlatformUserId.Equals(_author) || PlatformManager.NativePlatform.User.IsFriend(_author)))
				{
					_callback(new CensoredTextResult(true, _input, _input));
					return;
				}
			}
			Task.Run(delegate()
			{
				char[] array = _input.ToCharArray();
				foreach (string pattern in this.bannedPatterns)
				{
					foreach (object obj in Regex.Matches(_input, pattern, RegexOptions.IgnoreCase))
					{
						Match match = (Match)obj;
						for (int i = match.Index; i < match.Index + match.Length; i++)
						{
							array[i] = '*';
						}
					}
				}
				_callback(new CensoredTextResult(true, _input, new string(array)));
			});
		}

		// Token: 0x0400A26D RID: 41581
		[PublicizedFrom(EAccessModifier.Private)]
		public const string isAlpNum = "^[a-zA-Z0-9]+$";

		// Token: 0x0400A26E RID: 41582
		[PublicizedFrom(EAccessModifier.Private)]
		public const string nonAlphabeticBefore = "(?<![a-zA-Z])";

		// Token: 0x0400A26F RID: 41583
		[PublicizedFrom(EAccessModifier.Private)]
		public const string nonAlphabeticAfter = "(?![a-zA-Z])";

		// Token: 0x0400A270 RID: 41584
		[PublicizedFrom(EAccessModifier.Private)]
		public bool fetchStarted;

		// Token: 0x0400A271 RID: 41585
		[PublicizedFrom(EAccessModifier.Private)]
		public bool fetchComplete;

		// Token: 0x0400A272 RID: 41586
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSet<string> bannedPatterns = new HashSet<string>();
	}
}
