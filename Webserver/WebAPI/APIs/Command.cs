using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;

namespace Webserver.WebAPI.APIs
{
	// Token: 0x02001AD6 RID: 6870
	[Preserve]
	public class Command : AbsRestApi
	{
		// Token: 0x0600CEE7 RID: 52967 RVA: 0x004B78EC File Offset: 0x004B5AEC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			int permissionLevel = _context.PermissionLevel;
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(Command.jsonCommandsKey);
			jsonWriter.WriteBeginArray();
			if (string.IsNullOrEmpty(requestPath))
			{
				IList<IConsoleCommand> commands = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommands();
				for (int i = 0; i < commands.Count; i++)
				{
					IConsoleCommand command = commands[i];
					if (i > 0)
					{
						jsonWriter.WriteValueSeparator();
					}
					this.writeCommandJson(ref jsonWriter, command, permissionLevel);
				}
			}
			else
			{
				IConsoleCommand command2 = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(requestPath, false);
				if (command2 == null)
				{
					jsonWriter.WriteEndArray();
					jsonWriter.WriteEndObject();
					AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.NotFound, null, null, null);
					return;
				}
				this.writeCommandJson(ref jsonWriter, command2, permissionLevel);
			}
			jsonWriter.WriteEndArray();
			jsonWriter.WriteEndObject();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CEE8 RID: 52968 RVA: 0x004B79C8 File Offset: 0x004B5BC8
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeCommandJson(ref JsonWriter _writer, IConsoleCommand _command, int _userPermissionLevel)
		{
			_writer.WriteRaw(Command.jsonOverloadsKey);
			_writer.WriteBeginArray();
			string text = string.Empty;
			string[] commands = _command.GetCommands();
			for (int i = 0; i < commands.Length; i++)
			{
				string text2 = commands[i];
				if (i > 0)
				{
					_writer.WriteValueSeparator();
				}
				_writer.WriteString(text2);
				if (text2.Length > text.Length)
				{
					text = text2;
				}
			}
			_writer.WriteEndArray();
			_writer.WriteRaw(Command.jsonCommandKey);
			_writer.WriteString(text);
			_writer.WriteRaw(Command.jsonDescriptionKey);
			_writer.WriteString(_command.GetDescription());
			_writer.WriteRaw(Command.jsonHelpKey);
			_writer.WriteString(_command.GetHelp());
			int commandPermissionLevel = GameManager.Instance.adminTools.Commands.GetCommandPermissionLevel(_command.GetCommands());
			_writer.WriteRaw(Command.jsonAllowedKey);
			_writer.WriteBoolean(_userPermissionLevel <= commandPermissionLevel);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CEE9 RID: 52969 RVA: 0x004B7AAC File Offset: 0x004B5CAC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestPost(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			string text;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "command", out text))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_COMMAND, null);
				return;
			}
			WebCommandResult.ResultType responseType = WebCommandResult.ResultType.Full;
			string a;
			if (JsonCommons.TryGetJsonField(_jsonInput, "format", out a))
			{
				if (a.EqualsCaseInsensitive("raw"))
				{
					responseType = WebCommandResult.ResultType.Raw;
				}
				else if (a.EqualsCaseInsensitive("simple"))
				{
					responseType = WebCommandResult.ResultType.ResultOnly;
				}
			}
			int num = text.IndexOf(' ');
			string text2 = (num > 0) ? text.Substring(0, num) : text;
			string parameters = (num > 0) ? text.Substring(text2.Length + 1) : "";
			IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(text2, true);
			if (command == null)
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.NotFound, _jsonInputData, EApiErrorCode.UNKNOWN_COMMAND, null);
				return;
			}
			int commandPermissionLevel = GameManager.Instance.adminTools.Commands.GetCommandPermissionLevel(command.GetCommands());
			if (_context.PermissionLevel > commandPermissionLevel)
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Forbidden, _jsonInputData, EApiErrorCode.NO_PERMISSION, null);
				return;
			}
			_context.Response.SendChunked = true;
			WebCommandResult sender = new WebCommandResult(text2, parameters, responseType, _context);
			SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteAsync(text, sender);
		}

		// Token: 0x0600CEEA RID: 52970 RVA: 0x000617F2 File Offset: 0x0005F9F2
		public override int DefaultPermissionLevel()
		{
			return 1000;
		}

		// Token: 0x0600CEEB RID: 52971 RVA: 0x004B7BBC File Offset: 0x004B5DBC
		public override int[] DefaultMethodPermissionLevels()
		{
			return new int[]
			{
				-2147483647,
				2000,
				int.MinValue,
				-2147483647,
				-2147483647
			};
		}

		// Token: 0x0600CEEC RID: 52972 RVA: 0x004B25BC File Offset: 0x004B07BC
		public Command() : base(null)
		{
		}

		// Token: 0x04009D32 RID: 40242
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonCommandsKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("commands");

		// Token: 0x04009D33 RID: 40243
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonOverloadsKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("overloads");

		// Token: 0x04009D34 RID: 40244
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonCommandKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("command");

		// Token: 0x04009D35 RID: 40245
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonDescriptionKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description");

		// Token: 0x04009D36 RID: 40246
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonHelpKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("help");

		// Token: 0x04009D37 RID: 40247
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonAllowedKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("allowed");
	}
}
