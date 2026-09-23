using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Platform;

// Token: 0x02000C56 RID: 3158
public static class GeneratedTextManager
{
	// Token: 0x06006022 RID: 24610 RVA: 0x00261818 File Offset: 0x0025FA18
	public static string GetDisplayTextImmediately(AuthoredText _authoredText, bool _checkBlockState, GeneratedTextManager.TextFilteringMode _filteringMode = GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode _bbSupportMode = GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes)
	{
		GeneratedTextManager.AuthoredTextDetails orCreateFilterDetails = GeneratedTextManager.GetOrCreateFilterDetails(_authoredText);
		if (string.IsNullOrEmpty((orCreateFilterDetails != null) ? orCreateFilterDetails.BaseText : null) || GameManager.IsDedicatedServer)
		{
			if (orCreateFilterDetails == null)
			{
				return null;
			}
			return orCreateFilterDetails.GetDisplayText(false, _bbSupportMode);
		}
		else
		{
			if (_checkBlockState && _authoredText.Author != null && !PlatformManager.MultiPlatform.User.PlatformUserId.Equals(_authoredText.Author))
			{
				PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(_authoredText.Author);
				if (playerData != null && playerData.PlatformData.Blocked[EBlockType.TextChat].IsBlocked())
				{
					return "";
				}
			}
			if (PlatformManager.MultiPlatform.TextCensor == null || GeneratedTextManager.ShouldSkipFiltering(_authoredText.Author, _filteringMode))
			{
				return orCreateFilterDetails.GetDisplayText(false, _bbSupportMode);
			}
			if (orCreateFilterDetails.IsFiltered())
			{
				return orCreateFilterDetails.GetDisplayText(true, _bbSupportMode);
			}
			return "{...}";
		}
	}

	// Token: 0x06006023 RID: 24611 RVA: 0x002618EC File Offset: 0x0025FAEC
	public static void GetDisplayText(AuthoredText _authoredText, Action<string> _textReadyCallback, bool _runCallbackIfReadyNow, bool _checkBlockState, GeneratedTextManager.TextFilteringMode _filteringMode = GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode _bbSupportMode = GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes)
	{
		GeneratedTextManager.AuthoredTextDetails orCreateFilterDetails = GeneratedTextManager.GetOrCreateFilterDetails(_authoredText);
		if (string.IsNullOrEmpty((orCreateFilterDetails != null) ? orCreateFilterDetails.BaseText : null) || GameManager.IsDedicatedServer)
		{
			if (_runCallbackIfReadyNow && _textReadyCallback != null)
			{
				_textReadyCallback((orCreateFilterDetails != null) ? orCreateFilterDetails.GetDisplayText(false, _bbSupportMode) : null);
			}
			return;
		}
		if (_checkBlockState && _authoredText.Author != null && !PlatformManager.MultiPlatform.User.PlatformUserId.Equals(_authoredText.Author))
		{
			PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(_authoredText.Author);
			if (playerData != null && playerData.PlatformData.Blocked[EBlockType.TextChat].IsBlocked())
			{
				if (_runCallbackIfReadyNow && _textReadyCallback != null)
				{
					_textReadyCallback("");
				}
				return;
			}
		}
		if (PlatformManager.MultiPlatform.TextCensor == null || GeneratedTextManager.ShouldSkipFiltering(_authoredText.Author, _filteringMode))
		{
			if (_runCallbackIfReadyNow && _textReadyCallback != null)
			{
				_textReadyCallback(orCreateFilterDetails.GetDisplayText(false, _bbSupportMode));
			}
			return;
		}
		if (orCreateFilterDetails.IsFiltered())
		{
			if (_runCallbackIfReadyNow && _textReadyCallback != null)
			{
				_textReadyCallback(orCreateFilterDetails.GetDisplayText(true, _bbSupportMode));
			}
			return;
		}
		if (_filteringMode == GeneratedTextManager.TextFilteringMode.FilterWithSafeString && _textReadyCallback != null)
		{
			_textReadyCallback("{...}");
		}
		object obj = GeneratedTextManager.lockObj;
		bool flag2;
		lock (obj)
		{
			flag2 = GeneratedTextManager.pendingFilterCallbacks.ContainsKey(_authoredText);
			if (!flag2)
			{
				GeneratedTextManager.pendingFilterCallbacks.Add(_authoredText, null);
			}
			if (_textReadyCallback != null)
			{
				if (GeneratedTextManager.pendingFilterCallbacks[_authoredText] == null)
				{
					GeneratedTextManager.pendingFilterCallbacks[_authoredText] = new List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>>();
				}
				GeneratedTextManager.pendingFilterCallbacks[_authoredText].Add(new ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>(_bbSupportMode, _textReadyCallback));
			}
		}
		if (!flag2)
		{
			string textToFilter = GeneratedTextManager.GetTextToFilter(orCreateFilterDetails.BaseText, _bbSupportMode);
			PlatformManager.MultiPlatform.TextCensor.CensorProfanity(textToFilter, _authoredText.Author, delegate(CensoredTextResult _censorResult)
			{
				GeneratedTextManager.FilterTextCallback(_authoredText, _censorResult, _bbSupportMode);
			});
		}
	}

	// Token: 0x06006024 RID: 24612 RVA: 0x00261B1C File Offset: 0x0025FD1C
	public static void GetDisplayText(string _text, PlatformUserIdentifierAbs _author, Action<string> _textReadyCallback, bool _checkBlockState, GeneratedTextManager.TextFilteringMode _filteringMode = GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode _bbSupportMode = GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes)
	{
		if (_textReadyCallback == null)
		{
			Log.Warning("Could not get display text \"" + _text + "\", no callback action provided");
		}
		if (string.IsNullOrEmpty(_text) || GameManager.IsDedicatedServer)
		{
			_textReadyCallback((_bbSupportMode == GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes) ? Utils.EscapeBbCodes(_text, false, false) : _text);
			return;
		}
		if (_checkBlockState && _author != null && !PlatformManager.MultiPlatform.User.PlatformUserId.Equals(_author))
		{
			PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(_author);
			if (playerData != null && playerData.PlatformData.Blocked[EBlockType.TextChat].IsBlocked())
			{
				if (_textReadyCallback != null)
				{
					_textReadyCallback("");
				}
				return;
			}
		}
		if (PlatformManager.MultiPlatform.TextCensor == null || GeneratedTextManager.ShouldSkipFiltering(_author, _filteringMode))
		{
			_textReadyCallback((_bbSupportMode == GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes) ? Utils.EscapeBbCodes(_text, false, false) : _text);
			return;
		}
		if (_filteringMode == GeneratedTextManager.TextFilteringMode.FilterWithSafeString)
		{
			_textReadyCallback("{...}");
		}
		object obj = GeneratedTextManager.lockObj;
		lock (obj)
		{
			if (!GeneratedTextManager.pendingFilterCallbacksStrings.ContainsKey(_text))
			{
				GeneratedTextManager.pendingFilterCallbacksStrings.Add(_text, new List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>>());
			}
			GeneratedTextManager.pendingFilterCallbacksStrings[_text].Add(new ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>(_bbSupportMode, _textReadyCallback));
		}
		string textToFilter = GeneratedTextManager.GetTextToFilter(_text, _bbSupportMode);
		PlatformManager.MultiPlatform.TextCensor.CensorProfanity(textToFilter, _author, delegate(CensoredTextResult _censorResult)
		{
			GeneratedTextManager.FilterTextCallbackStrings(_text, _censorResult);
		});
	}

	// Token: 0x06006025 RID: 24613 RVA: 0x00261CC4 File Offset: 0x0025FEC4
	public static void PrefilterText(AuthoredText _authoredText, GeneratedTextManager.TextFilteringMode _filteringMode = GeneratedTextManager.TextFilteringMode.Filter)
	{
		GeneratedTextManager.GetDisplayText(_authoredText, null, false, false, _filteringMode, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
	}

	// Token: 0x06006026 RID: 24614 RVA: 0x00261CD4 File Offset: 0x0025FED4
	public static bool IsFiltered(AuthoredText _authoredText)
	{
		GeneratedTextManager.AuthoredTextDetails authoredTextDetails;
		return _authoredText != null && (PlatformManager.MultiPlatform.TextCensor == null || string.IsNullOrEmpty(_authoredText.Text) || GameManager.IsDedicatedServer || (GeneratedTextManager.authoredTextReferences.TryGetValue(_authoredText, out authoredTextDetails) && authoredTextDetails.IsFiltered()));
	}

	// Token: 0x06006027 RID: 24615 RVA: 0x00261D20 File Offset: 0x0025FF20
	public static bool IsFiltering(AuthoredText _authoredText)
	{
		object obj = GeneratedTextManager.lockObj;
		bool result;
		lock (obj)
		{
			result = (_authoredText != null && GeneratedTextManager.pendingFilterCallbacks.ContainsKey(_authoredText));
		}
		return result;
	}

	// Token: 0x06006028 RID: 24616 RVA: 0x00261D6C File Offset: 0x0025FF6C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ShouldSkipFiltering(PlatformUserIdentifierAbs _author, GeneratedTextManager.TextFilteringMode _mode)
	{
		switch (_mode)
		{
		case GeneratedTextManager.TextFilteringMode.None:
			return true;
		case GeneratedTextManager.TextFilteringMode.Filter:
		case GeneratedTextManager.TextFilteringMode.FilterWithSafeString:
			return false;
		case GeneratedTextManager.TextFilteringMode.FilterOtherPlatforms:
		{
			EPlatformIdentifier eplatformIdentifier;
			return PlatformUserManager.TryGetNativePlatform(_author, out eplatformIdentifier) && eplatformIdentifier == PlatformManager.NativePlatform.PlatformIdentifier;
		}
		default:
			throw new NotImplementedException(string.Format("Cannot determine if filtering should be skipped for filtering mode {0}", _mode));
		}
	}

	// Token: 0x06006029 RID: 24617 RVA: 0x00261DC4 File Offset: 0x0025FFC4
	[PublicizedFrom(EAccessModifier.Private)]
	public static GeneratedTextManager.AuthoredTextDetails GetOrCreateFilterDetails(AuthoredText _authoredText)
	{
		if (_authoredText == null)
		{
			return null;
		}
		GeneratedTextManager.AuthoredTextDetails authoredTextDetails;
		if (GeneratedTextManager.authoredTextReferences.TryGetValue(_authoredText, out authoredTextDetails))
		{
			authoredTextDetails.SetText(_authoredText.Text);
			return authoredTextDetails;
		}
		authoredTextDetails = new GeneratedTextManager.AuthoredTextDetails(_authoredText.Text);
		GeneratedTextManager.authoredTextReferences.Add(_authoredText, authoredTextDetails);
		return authoredTextDetails;
	}

	// Token: 0x0600602A RID: 24618 RVA: 0x00261E0C File Offset: 0x0026000C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FilterTextCallback(AuthoredText _authoredText, CensoredTextResult _censorResult, GeneratedTextManager.BbCodeSupportMode _originalBBSupport)
	{
		object obj = GeneratedTextManager.lockObj;
		List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>> list;
		lock (obj)
		{
			if (!GeneratedTextManager.pendingFilterCallbacks.TryGetValue(_authoredText, out list))
			{
				Log.Error("Invalid callback information during text filtering.");
				return;
			}
		}
		GeneratedTextManager.AuthoredTextDetails authoredTextDetails;
		if (!GeneratedTextManager.authoredTextReferences.TryGetValue(_authoredText, out authoredTextDetails))
		{
			Log.Error("Authored Text filter details not found.");
			return;
		}
		if (GeneratedTextManager.GetTextToFilter(authoredTextDetails.BaseText, _originalBBSupport) != _censorResult.OriginalText)
		{
			Log.Warning("Text has changed during filtering process, displayed texts may be outdated.");
		}
		if (_censorResult.Success)
		{
			authoredTextDetails.SetFilteredText(_censorResult.CensoredText);
		}
		else if (_authoredText.Author.Equals(PlatformManager.MultiPlatform.User.PlatformUserId))
		{
			authoredTextDetails.SetFilteredText(authoredTextDetails.BaseText);
		}
		else
		{
			authoredTextDetails.SetFilteredText("{...}");
		}
		obj = GeneratedTextManager.lockObj;
		lock (obj)
		{
			GeneratedTextManager.pendingFilterCallbacks.Remove(_authoredText);
		}
		if (list != null)
		{
			foreach (ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>> valueTuple in list)
			{
				GeneratedTextManager.BbCodeSupportMode item = valueTuple.Item1;
				Action<string> item2 = valueTuple.Item2;
				if (item2 != null)
				{
					item2(authoredTextDetails.GetDisplayText(true, item));
				}
			}
		}
	}

	// Token: 0x0600602B RID: 24619 RVA: 0x00261F7C File Offset: 0x0026017C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FilterTextCallbackStrings(string _text, CensoredTextResult _censorResult)
	{
		object obj = GeneratedTextManager.lockObj;
		List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>> list;
		lock (obj)
		{
			if (!GeneratedTextManager.pendingFilterCallbacksStrings.TryGetValue(_text, out list))
			{
				Log.Error("Invalid callback information during text filtering.");
				return;
			}
			GeneratedTextManager.pendingFilterCallbacksStrings.Remove(_text);
		}
		if (list != null)
		{
			foreach (ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>> valueTuple in list)
			{
				GeneratedTextManager.BbCodeSupportMode item = valueTuple.Item1;
				Action<string> item2 = valueTuple.Item2;
				string text;
				if (item != GeneratedTextManager.BbCodeSupportMode.Supported)
				{
					if (item != GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes)
					{
						text = _censorResult.CensoredText;
					}
					else
					{
						text = Utils.EscapeBbCodes(_censorResult.CensoredText, false, false);
					}
				}
				else
				{
					text = GeneratedTextManager.ReconstructFilteredTextWithBbCodes(_text, _censorResult.CensoredText);
				}
				string obj2 = text;
				if (item2 != null)
				{
					item2(obj2);
				}
			}
		}
	}

	// Token: 0x0600602C RID: 24620 RVA: 0x00262074 File Offset: 0x00260274
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetTextToFilter(string _baseText, GeneratedTextManager.BbCodeSupportMode _bbSupport)
	{
		if (_baseText == null)
		{
			return null;
		}
		if (_bbSupport != GeneratedTextManager.BbCodeSupportMode.Supported)
		{
			return _baseText;
		}
		return Utils.GetVisibileTextWithBbCodes(_baseText);
	}

	// Token: 0x0600602D RID: 24621 RVA: 0x00262088 File Offset: 0x00260288
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ReconstructFilteredTextWithBbCodes(string originalText, string filteredText)
	{
		int num = 0;
		int num2 = 0;
		StringBuilder stringBuilder = new StringBuilder();
		while (num < originalText.Length && num2 < filteredText.Length)
		{
			ValueTuple<int, int, bool> valueTuple = Utils.FindNextBbCode(originalText, num, false);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			bool item3 = valueTuple.Item3;
			if (item == -1)
			{
				break;
			}
			int num3 = item - num;
			stringBuilder.Append(filteredText, num2, num3);
			num2 += num3;
			stringBuilder.Append(originalText, item, item2);
			num = item + item2;
			if (item3)
			{
				num2 += item2 - 4;
			}
		}
		if (num2 < filteredText.Length)
		{
			stringBuilder.Append(filteredText, num2, filteredText.Length - num2);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04004B6A RID: 19306
	public const string SafeString = "{...}";

	// Token: 0x04004B6B RID: 19307
	public const string BlockedString = "";

	// Token: 0x04004B6C RID: 19308
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object lockObj = new object();

	// Token: 0x04004B6D RID: 19309
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConditionalWeakTable<AuthoredText, GeneratedTextManager.AuthoredTextDetails> authoredTextReferences = new ConditionalWeakTable<AuthoredText, GeneratedTextManager.AuthoredTextDetails>();

	// Token: 0x04004B6E RID: 19310
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<AuthoredText, List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>>> pendingFilterCallbacks = new Dictionary<AuthoredText, List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>>>();

	// Token: 0x04004B6F RID: 19311
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>>> pendingFilterCallbacksStrings = new Dictionary<string, List<ValueTuple<GeneratedTextManager.BbCodeSupportMode, Action<string>>>>();

	// Token: 0x02000C57 RID: 3159
	public enum TextFilteringMode
	{
		// Token: 0x04004B71 RID: 19313
		None,
		// Token: 0x04004B72 RID: 19314
		Filter,
		// Token: 0x04004B73 RID: 19315
		FilterOtherPlatforms,
		// Token: 0x04004B74 RID: 19316
		FilterWithSafeString
	}

	// Token: 0x02000C58 RID: 3160
	public enum BbCodeSupportMode
	{
		// Token: 0x04004B76 RID: 19318
		NotSupported,
		// Token: 0x04004B77 RID: 19319
		Supported,
		// Token: 0x04004B78 RID: 19320
		SupportedAndAddEscapes
	}

	// Token: 0x02000C59 RID: 3161
	[PublicizedFrom(EAccessModifier.Private)]
	public class AuthoredTextDetails
	{
		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x0600602F RID: 24623 RVA: 0x0026214F File Offset: 0x0026034F
		public string BaseText
		{
			get
			{
				return this.baseText;
			}
		}

		// Token: 0x06006030 RID: 24624 RVA: 0x00262157 File Offset: 0x00260357
		public AuthoredTextDetails(string _baseText)
		{
			this.SetText(_baseText);
		}

		// Token: 0x06006031 RID: 24625 RVA: 0x00262166 File Offset: 0x00260366
		public bool IsFiltered()
		{
			return this.filteredTextBase != null;
		}

		// Token: 0x06006032 RID: 24626 RVA: 0x00262171 File Offset: 0x00260371
		public void SetText(string _baseText)
		{
			if (_baseText != this.baseText)
			{
				this.baseText = _baseText;
				this.baseTextEscaped = null;
				this.filteredTextBase = null;
				this.filteredTextBBSupported = null;
				this.filteredTextBBEscaped = null;
			}
		}

		// Token: 0x06006033 RID: 24627 RVA: 0x002621A4 File Offset: 0x002603A4
		public void SetFilteredText(string _filteredText)
		{
			this.filteredTextBase = _filteredText;
		}

		// Token: 0x06006034 RID: 24628 RVA: 0x002621B0 File Offset: 0x002603B0
		public string GetDisplayText(bool _filtered, GeneratedTextManager.BbCodeSupportMode _bbSupportMode)
		{
			if (_filtered)
			{
				switch (_bbSupportMode)
				{
				case GeneratedTextManager.BbCodeSupportMode.NotSupported:
					return this.filteredTextBase;
				case GeneratedTextManager.BbCodeSupportMode.Supported:
					if (this.filteredTextBBSupported == null && this.filteredTextBase != null)
					{
						this.filteredTextBBSupported = GeneratedTextManager.ReconstructFilteredTextWithBbCodes(this.baseText, this.filteredTextBase);
					}
					return this.filteredTextBBSupported;
				case GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes:
					if (this.filteredTextBBEscaped == null && this.filteredTextBase != null)
					{
						this.filteredTextBBEscaped = Utils.EscapeBbCodes(this.filteredTextBase, false, false);
					}
					return this.filteredTextBBEscaped;
				default:
					return null;
				}
			}
			else
			{
				if (_bbSupportMode <= GeneratedTextManager.BbCodeSupportMode.Supported)
				{
					return this.baseText;
				}
				if (_bbSupportMode != GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes)
				{
					return this.baseText;
				}
				if (this.baseTextEscaped == null && this.baseText != null)
				{
					this.baseTextEscaped = Utils.EscapeBbCodes(this.baseText, false, false);
				}
				return this.baseTextEscaped;
			}
		}

		// Token: 0x04004B79 RID: 19321
		[PublicizedFrom(EAccessModifier.Private)]
		public string baseText;

		// Token: 0x04004B7A RID: 19322
		[PublicizedFrom(EAccessModifier.Private)]
		public string baseTextEscaped;

		// Token: 0x04004B7B RID: 19323
		[PublicizedFrom(EAccessModifier.Private)]
		public string filteredTextBase;

		// Token: 0x04004B7C RID: 19324
		[PublicizedFrom(EAccessModifier.Private)]
		public string filteredTextBBSupported;

		// Token: 0x04004B7D RID: 19325
		[PublicizedFrom(EAccessModifier.Private)]
		public string filteredTextBBEscaped;
	}
}
