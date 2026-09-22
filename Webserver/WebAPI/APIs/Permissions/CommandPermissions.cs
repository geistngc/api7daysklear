using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;

namespace Webserver.WebAPI.APIs.Permissions
{
	// Token: 0x02001AE6 RID: 6886
	[Preserve]
	public class CommandPermissions : AbsRestApi
	{
		// Token: 0x1700197C RID: 6524
		// (get) Token: 0x0600CF2E RID: 53038 RVA: 0x004B956A File Offset: 0x004B776A
		public static AdminCommands CommandsInstance
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return GameManager.Instance.adminTools.Commands;
			}
		}

		// Token: 0x0600CF2F RID: 53039 RVA: 0x004B957C File Offset: 0x004B777C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			if (string.IsNullOrEmpty(requestPath))
			{
				jsonWriter.WriteBeginArray();
				bool flag = true;
				foreach (IConsoleCommand consoleCommand in SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommands())
				{
					if (!flag)
					{
						jsonWriter.WriteValueSeparator();
					}
					flag = false;
					AdminCommands.CommandPermission adminToolsCommandPermission = CommandPermissions.CommandsInstance.GetAdminToolsCommandPermission(consoleCommand.GetCommands());
					bool isDefault = adminToolsCommandPermission.PermissionLevel == consoleCommand.DefaultPermissionLevel;
					if (adminToolsCommandPermission.Command == "")
					{
						adminToolsCommandPermission = new AdminCommands.CommandPermission(consoleCommand.GetCommands()[0], adminToolsCommandPermission.PermissionLevel);
					}
					this.writeCommandJson(ref jsonWriter, adminToolsCommandPermission, isDefault);
				}
				jsonWriter.WriteEndArray();
				AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
				return;
			}
			jsonWriter.WriteRaw(WebUtils.JsonEmptyData);
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.BadRequest, null, null, null);
		}

		// Token: 0x0600CF30 RID: 53040 RVA: 0x004B9680 File Offset: 0x004B7880
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeCommandJson(ref JsonWriter _writer, AdminCommands.CommandPermission _commandPermission, bool _isDefault)
		{
			_writer.WriteRaw(CommandPermissions.jsonKeyCommand);
			_writer.WriteString(_commandPermission.Command);
			_writer.WriteRaw(CommandPermissions.jsonKeyPermissionLevel);
			_writer.WriteInt32(_commandPermission.PermissionLevel);
			_writer.WriteRaw(CommandPermissions.jsonKeyIsDefault);
			_writer.WriteBoolean(_isDefault);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF31 RID: 53041 RVA: 0x004B96D4 File Offset: 0x004B78D4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestPost(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			string requestPath = _context.RequestPath;
			if (string.IsNullOrEmpty(requestPath))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_COMMAND, null);
				return;
			}
			IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(requestPath, false);
			if (command == null)
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.NotFound, _jsonInputData, EApiErrorCode.UNKNOWN_COMMAND, null);
				return;
			}
			int permissionLevel;
			if (!JsonCommons.TryGetJsonField(_jsonInput, "permissionLevel", out permissionLevel))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_OR_INVALID_PERMISSION_LEVEL, null);
				return;
			}
			CommandPermissions.CommandsInstance.AddCommand(command.GetCommands()[0], permissionLevel, true);
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Created, null, null, null);
		}

		// Token: 0x0600CF32 RID: 53042 RVA: 0x004B9760 File Offset: 0x004B7960
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestDelete(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(requestPath, false);
			if (command == null || !CommandPermissions.CommandsInstance.IsPermissionDefined(command.GetCommands()))
			{
				AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.NotFound, null, null, null);
				return;
			}
			AbsRestApi.SendEmptyResponse(_context, CommandPermissions.CommandsInstance.RemoveCommand(command.GetCommands()) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound, null, null, null);
		}

		// Token: 0x1700197D RID: 6525
		// (get) Token: 0x0600CF33 RID: 53043 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool AllowPostWithId
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return true;
			}
		}

		// Token: 0x0600CF34 RID: 53044 RVA: 0x004B9519 File Offset: 0x004B7719
		public override int[] DefaultMethodPermissionLevels()
		{
			return new int[]
			{
				-2147483647,
				int.MinValue,
				int.MinValue,
				-2147483647,
				int.MinValue
			};
		}

		// Token: 0x0600CF35 RID: 53045 RVA: 0x004B25BC File Offset: 0x004B07BC
		public CommandPermissions() : base(null)
		{
		}

		// Token: 0x04009D89 RID: 40329
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyCommand = "command";

		// Token: 0x04009D8A RID: 40330
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyPermissionLevel = "permissionLevel";

		// Token: 0x04009D8B RID: 40331
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyIsDefault = "default";

		// Token: 0x04009D8C RID: 40332
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyCommand = JsonWriter.GetEncodedPropertyNameWithBeginObject("command");

		// Token: 0x04009D8D RID: 40333
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPermissionLevel = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("permissionLevel");

		// Token: 0x04009D8E RID: 40334
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyIsDefault = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("default");
	}
}
