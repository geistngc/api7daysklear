using System;
using System.Net;
using JetBrains.Annotations;
using SandboxOptions;
using UnityEngine.Scripting;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Webserver.WebAPI.APIs.ServerState
{
	// Token: 0x02001AE0 RID: 6880
	[Preserve]
	public class SandboxSettings : AbsRestApi
	{
		// Token: 0x0600CF18 RID: 53016 RVA: 0x004B8D5D File Offset: 0x004B6F5D
		public SandboxSettings() : base("SandboxSettings")
		{
		}

		// Token: 0x0600CF19 RID: 53017 RVA: 0x004B8D6C File Offset: 0x004B6F6C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string code = _context.QueryParameters["code"] ?? GameStats.GetString(EnumGameStats.SandboxCode);
			bool onlyChanged;
			if (_context.QueryParameters["onlyChanged"] == null || !bool.TryParse(_context.QueryParameters["onlyChanged"], out onlyChanged))
			{
				onlyChanged = false;
			}
			bool detailed;
			if (_context.QueryParameters["detailed"] == null || !bool.TryParse(_context.QueryParameters["detailed"], out detailed))
			{
				detailed = false;
			}
			SandboxOptionPreset sandboxOptionPreset = new SandboxOptionPreset();
			if (!SandboxOptionManager.Current.LoadOptionsFromCode(code, sandboxOptionPreset))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.InternalServerError, null, EApiErrorCode.NO_SANDBOX_DATA, null);
				return;
			}
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(SandboxSettings.jsonCodeKey);
			jsonWriter.WriteString(sandboxOptionPreset.SandboxCode);
			jsonWriter.WriteRaw(SandboxSettings.jsonOptionsKey);
			jsonWriter.WriteBeginArray();
			this.logSandbox(ref jsonWriter, sandboxOptionPreset, onlyChanged, detailed);
			jsonWriter.WriteEndArray();
			jsonWriter.WriteEndObject();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CF1A RID: 53018 RVA: 0x004B8E70 File Offset: 0x004B7070
		[PublicizedFrom(EAccessModifier.Private)]
		public void logSandbox(ref JsonWriter _writer, SandboxOptionPreset _preset, bool _onlyChanged, bool _detailed)
		{
			JsonSerializer.SetDefaultResolver(StandardResolver.CamelCase);
			bool flag = false;
			for (int i = 0; i < 165; i++)
			{
				SandboxOptions sandboxOptions = (SandboxOptions)i;
				BaseSandboxOption baseSandboxOption;
				if (SandboxOptionManager.Current.SandboxOptionsDict.TryGetValue(sandboxOptions, out baseSandboxOption))
				{
					int defaultIndex = baseSandboxOption.GetDefaultIndex();
					int index;
					if (!_preset.PresetValues.TryGetValue(sandboxOptions, out index))
					{
						if (_onlyChanged)
						{
							goto IL_1B8;
						}
						index = defaultIndex;
					}
					string valueTextFromIndex = baseSandboxOption.GetValueTextFromIndex(index, null);
					string valueTextFromIndex2 = baseSandboxOption.GetValueTextFromIndex(defaultIndex, null);
					if (flag)
					{
						_writer.WriteValueSeparator();
					}
					flag = true;
					_writer.WriteRaw(SandboxSettings.jsonOptInternalKey);
					_writer.WriteString(baseSandboxOption.Option.ToStringCached<SandboxOptions>());
					_writer.WriteRaw(SandboxSettings.jsonOptTypeKey);
					_writer.WriteString(baseSandboxOption.OptionType.ToStringCached<BaseSandboxOption.OptionTypes>());
					_writer.WriteRaw(SandboxSettings.jsonOptActiveValueKey);
					this.logValue(ref _writer, index, baseSandboxOption.GetValue(), valueTextFromIndex);
					_writer.WriteRaw(SandboxSettings.jsonOptDefaultValueKey);
					this.logValue(ref _writer, defaultIndex, baseSandboxOption.GetDefaultValue(), valueTextFromIndex2);
					if (_detailed)
					{
						_writer.WriteRaw(SandboxSettings.jsonOptValueSetKey);
						_writer.WriteBeginArray();
						SandboxOptionValueSet valueSet = baseSandboxOption.GetValueSet();
						for (int j = 0; j < valueSet.GetValueCount(); j++)
						{
							if (j > 0)
							{
								_writer.WriteValueSeparator();
							}
							object value;
							if (!valueSet.GetValue(j, out value))
							{
								Log.Error(string.Format("[Web] In {0}: Could not get value set element at index {1} for option {2}", "SandboxSettings", j, sandboxOptions.ToStringCached<SandboxOptions>()));
							}
							else
							{
								this.logValue(ref _writer, j, value, valueSet.GetDisplayAtIndex(j, null));
							}
						}
						_writer.WriteEndArray();
						SandboxSettings.SandboxOptionInfo value2;
						value2.CategoryName = baseSandboxOption.CategoryName;
						value2.InternalName = baseSandboxOption.OptionName;
						value2.LocalizedName = baseSandboxOption.OptionNameText;
						value2.Description = baseSandboxOption.DescriptionText;
						_writer.WriteRaw(SandboxSettings.jsonOptDetailsKey);
						JsonSerializer.Serialize<SandboxSettings.SandboxOptionInfo>(ref _writer, value2);
					}
					_writer.WriteEndObject();
				}
				IL_1B8:;
			}
		}

		// Token: 0x0600CF1B RID: 53019 RVA: 0x004B9044 File Offset: 0x004B7244
		[PublicizedFrom(EAccessModifier.Private)]
		public void logValue(ref JsonWriter _writer, int _index, object _value, string _localizedValue)
		{
			SandboxSettings.SandboxOptionValue value;
			value.Index = _index;
			value.Value = _value;
			value.LocalizedName = _localizedValue;
			JsonSerializer.Serialize<SandboxSettings.SandboxOptionValue>(ref _writer, value);
		}

		// Token: 0x04009D6B RID: 40299
		[PublicizedFrom(EAccessModifier.Private)]
		public const string QueryParamCode = "code";

		// Token: 0x04009D6C RID: 40300
		[PublicizedFrom(EAccessModifier.Private)]
		public const string QueryParamOnlyChanged = "onlyChanged";

		// Token: 0x04009D6D RID: 40301
		[PublicizedFrom(EAccessModifier.Private)]
		public const string QueryParamDetailed = "detailed";

		// Token: 0x04009D6E RID: 40302
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonCodeKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("code");

		// Token: 0x04009D6F RID: 40303
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOptionsKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("options");

		// Token: 0x04009D70 RID: 40304
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOptInternalKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("key");

		// Token: 0x04009D71 RID: 40305
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOptDetailsKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("details");

		// Token: 0x04009D72 RID: 40306
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOptTypeKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("type");

		// Token: 0x04009D73 RID: 40307
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOptActiveValueKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("activeValue");

		// Token: 0x04009D74 RID: 40308
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOptDefaultValueKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("defaultValue");

		// Token: 0x04009D75 RID: 40309
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOptValueSetKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("valueSet");

		// Token: 0x02001AE1 RID: 6881
		[PublicizedFrom(EAccessModifier.Private)]
		public struct SandboxOptionInfo
		{
			// Token: 0x04009D76 RID: 40310
			[UsedImplicitly]
			public string CategoryName;

			// Token: 0x04009D77 RID: 40311
			[UsedImplicitly]
			public string InternalName;

			// Token: 0x04009D78 RID: 40312
			[UsedImplicitly]
			public string LocalizedName;

			// Token: 0x04009D79 RID: 40313
			[UsedImplicitly]
			public string Description;
		}

		// Token: 0x02001AE2 RID: 6882
		[PublicizedFrom(EAccessModifier.Private)]
		public struct SandboxOptionValue
		{
			// Token: 0x04009D7A RID: 40314
			[UsedImplicitly]
			public int Index;

			// Token: 0x04009D7B RID: 40315
			[UsedImplicitly]
			public object Value;

			// Token: 0x04009D7C RID: 40316
			[UsedImplicitly]
			public string LocalizedName;
		}
	}
}
