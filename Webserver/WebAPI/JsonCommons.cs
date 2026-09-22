using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Utf8Json;

namespace Webserver.WebAPI
{
	// Token: 0x02001AD2 RID: 6866
	public static class JsonCommons
	{
		// Token: 0x0600CEC8 RID: 52936 RVA: 0x004B6D14 File Offset: 0x004B4F14
		public static void WriteVector3I(ref JsonWriter _writer, Vector3i _position)
		{
			_writer.WriteRaw(JsonCommons.jsonKeyPositionX);
			_writer.WriteInt32(_position.x);
			_writer.WriteRaw(JsonCommons.jsonKeyPositionY);
			_writer.WriteInt32(_position.y);
			_writer.WriteRaw(JsonCommons.jsonKeyPositionZ);
			_writer.WriteInt32(_position.z);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CEC9 RID: 52937 RVA: 0x004B6D6C File Offset: 0x004B4F6C
		public static void WriteVector3(ref JsonWriter _writer, Vector3 _position)
		{
			_writer.WriteRaw(JsonCommons.jsonKeyPositionX);
			_writer.WriteSingle(_position.x);
			_writer.WriteRaw(JsonCommons.jsonKeyPositionY);
			_writer.WriteSingle(_position.y);
			_writer.WriteRaw(JsonCommons.jsonKeyPositionZ);
			_writer.WriteSingle(_position.z);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CECA RID: 52938 RVA: 0x004B6DC4 File Offset: 0x004B4FC4
		public static void WriteVector2I(ref JsonWriter _writer, Vector2i _position)
		{
			_writer.WriteRaw(JsonCommons.jsonKeyPositionX);
			_writer.WriteInt32(_position.x);
			_writer.WriteRaw(JsonCommons.jsonKeyPositionY);
			_writer.WriteInt32(_position.y);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CECB RID: 52939 RVA: 0x004B6DFA File Offset: 0x004B4FFA
		public static void WriteVector2(ref JsonWriter _writer, Vector2 _position)
		{
			_writer.WriteRaw(JsonCommons.jsonKeyPositionX);
			_writer.WriteSingle(_position.x);
			_writer.WriteRaw(JsonCommons.jsonKeyPositionY);
			_writer.WriteSingle(_position.y);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CECC RID: 52940 RVA: 0x004B6E30 File Offset: 0x004B5030
		public static void WriteGameTimeObject(ref JsonWriter _writer, int _days, int _hours, int _minutes)
		{
			_writer.WriteRaw(JsonCommons.jsonKeyDays);
			_writer.WriteInt32(_days);
			_writer.WriteRaw(JsonCommons.jsonKeyHours);
			_writer.WriteInt32(_hours);
			_writer.WriteRaw(JsonCommons.jsonKeyMinutes);
			_writer.WriteInt32(_minutes);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CECD RID: 52941 RVA: 0x004B6E70 File Offset: 0x004B5070
		public static void WritePlatformUserIdentifier(ref JsonWriter _writer, PlatformUserIdentifierAbs _userIdentifier)
		{
			if (_userIdentifier == null)
			{
				_writer.WriteNull();
				return;
			}
			_writer.WriteRaw(JsonCommons.jsonKeyCombinedString);
			_writer.WriteString(_userIdentifier.CombinedString);
			_writer.WriteRaw(JsonCommons.jsonKeyPlatformId);
			_writer.WriteString(_userIdentifier.PlatformIdentifierString);
			_writer.WriteRaw(JsonCommons.jsonKeyUserId);
			_writer.WriteString(_userIdentifier.ReadablePlatformUserIdentifier);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CECE RID: 52942 RVA: 0x004B6ED4 File Offset: 0x004B50D4
		public static bool TryReadPlatformUserIdentifier(IDictionary<string, object> _jsonInput, out PlatformUserIdentifierAbs _userIdentifier)
		{
			string combinedString;
			if (JsonCommons.TryGetJsonField(_jsonInput, "combinedString", out combinedString))
			{
				_userIdentifier = PlatformUserIdentifierAbs.FromCombinedString(combinedString, false);
				if (_userIdentifier != null)
				{
					return true;
				}
			}
			string platformName;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "platformId", out platformName))
			{
				_userIdentifier = null;
				return false;
			}
			string userId;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "userId", out userId))
			{
				_userIdentifier = null;
				return false;
			}
			_userIdentifier = PlatformUserIdentifierAbs.FromPlatformAndId(platformName, userId, false);
			return _userIdentifier != null;
		}

		// Token: 0x0600CECF RID: 52943 RVA: 0x004B6F36 File Offset: 0x004B5136
		public static void WriteDateTime(ref JsonWriter _writer, DateTime _dateTime)
		{
			_writer.WriteString(_dateTime.ToString("o"));
		}

		// Token: 0x0600CED0 RID: 52944 RVA: 0x004B6F4C File Offset: 0x004B514C
		public static bool TryReadDateTime(IDictionary<string, object> _jsonInput, string _fieldName, out DateTime _result)
		{
			_result = default(DateTime);
			string s;
			return JsonCommons.TryGetJsonField(_jsonInput, _fieldName, out s) && DateTime.TryParse(s, null, DateTimeStyles.RoundtripKind, out _result);
		}

		// Token: 0x0600CED1 RID: 52945 RVA: 0x004B6F7C File Offset: 0x004B517C
		public static bool TryGetJsonField(IDictionary<string, object> _jsonObject, string _fieldName, out int _value)
		{
			_value = 0;
			object obj;
			if (!_jsonObject.TryGetValue(_fieldName, out obj))
			{
				return false;
			}
			if (obj is double)
			{
				double num = (double)obj;
				bool result;
				try
				{
					_value = (int)num;
					result = true;
				}
				catch (Exception)
				{
					result = false;
				}
				return result;
			}
			return false;
		}

		// Token: 0x0600CED2 RID: 52946 RVA: 0x004B6FCC File Offset: 0x004B51CC
		public static bool TryGetJsonField(IDictionary<string, object> _jsonObject, string _fieldName, out double _value)
		{
			_value = 0.0;
			object obj;
			if (!_jsonObject.TryGetValue(_fieldName, out obj))
			{
				return false;
			}
			if (obj is double)
			{
				double num = (double)obj;
				bool result;
				try
				{
					_value = num;
					result = true;
				}
				catch (Exception)
				{
					result = false;
				}
				return result;
			}
			return false;
		}

		// Token: 0x0600CED3 RID: 52947 RVA: 0x004B7024 File Offset: 0x004B5224
		public static bool TryGetJsonField(IDictionary<string, object> _jsonObject, string _fieldName, out string _value)
		{
			_value = null;
			object obj;
			if (!_jsonObject.TryGetValue(_fieldName, out obj))
			{
				return false;
			}
			string text = obj as string;
			if (text == null)
			{
				return false;
			}
			bool result;
			try
			{
				_value = text;
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600CED4 RID: 52948 RVA: 0x004B706C File Offset: 0x004B526C
		public static bool TryGetJsonField(IDictionary<string, object> _jsonObject, string _fieldName, out IDictionary<string, object> _value)
		{
			_value = null;
			object obj;
			if (!_jsonObject.TryGetValue(_fieldName, out obj))
			{
				return false;
			}
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary == null)
			{
				return false;
			}
			bool result;
			try
			{
				_value = dictionary;
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04009D22 RID: 40226
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPositionX = JsonWriter.GetEncodedPropertyNameWithBeginObject("x");

		// Token: 0x04009D23 RID: 40227
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPositionY = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("y");

		// Token: 0x04009D24 RID: 40228
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPositionZ = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("z");

		// Token: 0x04009D25 RID: 40229
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyDays = JsonWriter.GetEncodedPropertyNameWithBeginObject("days");

		// Token: 0x04009D26 RID: 40230
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyHours = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hours");

		// Token: 0x04009D27 RID: 40231
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyMinutes = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("minutes");

		// Token: 0x04009D28 RID: 40232
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyCombinedString = JsonWriter.GetEncodedPropertyNameWithBeginObject("combinedString");

		// Token: 0x04009D29 RID: 40233
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPlatformId = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("platformId");

		// Token: 0x04009D2A RID: 40234
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyUserId = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("userId");
	}
}
