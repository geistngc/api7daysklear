using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;

namespace Webserver.WebAPI.APIs.Permissions
{
	// Token: 0x02001AE9 RID: 6889
	[Preserve]
	public class UserPermissions : AbsRestApi
	{
		// Token: 0x1700197E RID: 6526
		// (get) Token: 0x0600CF3D RID: 53053 RVA: 0x004B9BFB File Offset: 0x004B7DFB
		public static AdminUsers UsersInstance
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return GameManager.Instance.adminTools.Users;
			}
		}

		// Token: 0x0600CF3E RID: 53054 RVA: 0x004B9C0C File Offset: 0x004B7E0C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			if (string.IsNullOrEmpty(requestPath))
			{
				jsonWriter.WriteRaw(UserPermissions.jsonKeyUsers);
				jsonWriter.WriteBeginArray();
				bool flag = true;
				foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminUsers.UserPermission> keyValuePair in UserPermissions.UsersInstance.GetUsers())
				{
					PlatformUserIdentifierAbs platformUserIdentifierAbs;
					AdminUsers.UserPermission userPermission;
					keyValuePair.Deconstruct(out platformUserIdentifierAbs, out userPermission);
					AdminUsers.UserPermission userPermission2 = userPermission;
					if (!flag)
					{
						jsonWriter.WriteValueSeparator();
					}
					flag = false;
					this.writeUserJson(ref jsonWriter, userPermission2);
				}
				jsonWriter.WriteEndArray();
				jsonWriter.WriteRaw(UserPermissions.jsonKeyGroups);
				jsonWriter.WriteBeginArray();
				flag = true;
				foreach (KeyValuePair<string, AdminUsers.GroupPermission> keyValuePair2 in UserPermissions.UsersInstance.GetGroups())
				{
					string text;
					AdminUsers.GroupPermission groupPermission;
					keyValuePair2.Deconstruct(out text, out groupPermission);
					AdminUsers.GroupPermission groupPermission2 = groupPermission;
					if (!flag)
					{
						jsonWriter.WriteValueSeparator();
					}
					flag = false;
					this.writeGroupJson(ref jsonWriter, groupPermission2);
				}
				jsonWriter.WriteEndArray();
				jsonWriter.WriteEndObject();
				AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
				return;
			}
			jsonWriter.WriteRaw(WebUtils.JsonEmptyData);
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.BadRequest, null, null, null);
		}

		// Token: 0x0600CF3F RID: 53055 RVA: 0x004B9D6C File Offset: 0x004B7F6C
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeUserJson(ref JsonWriter _writer, AdminUsers.UserPermission _userPermission)
		{
			_writer.WriteRaw(UserPermissions.jsonKeyName);
			_writer.WriteString(_userPermission.Name ?? "");
			_writer.WriteRaw(UserPermissions.jsonKeyUserId);
			JsonCommons.WritePlatformUserIdentifier(ref _writer, _userPermission.UserIdentifier);
			_writer.WriteRaw(UserPermissions.jsonKeyPermissionLevel);
			_writer.WriteInt32(_userPermission.PermissionLevel);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF40 RID: 53056 RVA: 0x004B9DD0 File Offset: 0x004B7FD0
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeGroupJson(ref JsonWriter _writer, AdminUsers.GroupPermission _groupPermission)
		{
			_writer.WriteRaw(UserPermissions.jsonKeyName);
			_writer.WriteString(_groupPermission.Name ?? "");
			_writer.WriteRaw(UserPermissions.jsonKeyGroupId);
			_writer.WriteString(_groupPermission.SteamIdGroup);
			_writer.WriteRaw(UserPermissions.jsonKeyPermissionLevelMods);
			_writer.WriteInt32(_groupPermission.PermissionLevelMods);
			_writer.WriteRaw(UserPermissions.jsonKeyPermissionLevelNormal);
			_writer.WriteInt32(_groupPermission.PermissionLevelNormal);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF41 RID: 53057 RVA: 0x004B9E48 File Offset: 0x004B8048
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestPost(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			string steamId;
			if (!PermissionsApiHelpers.TryParseId(_context, _jsonInputData, out platformUserIdentifierAbs, out steamId))
			{
				return;
			}
			if (platformUserIdentifierAbs != null)
			{
				int permissionLevel;
				if (!JsonCommons.TryGetJsonField(_jsonInput, "permissionLevel", out permissionLevel))
				{
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_OR_INVALID_PERMISSION_LEVEL, null);
					return;
				}
				string name;
				JsonCommons.TryGetJsonField(_jsonInput, "name", out name);
				UserPermissions.UsersInstance.AddUser(name, platformUserIdentifierAbs, permissionLevel);
			}
			else
			{
				int permissionLevelMod;
				if (!JsonCommons.TryGetJsonField(_jsonInput, "permissionLevelMods", out permissionLevelMod))
				{
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_OR_INVALID_PERMISSION_LEVEL_MODS, null);
					return;
				}
				int permissionLevelDefault;
				if (!JsonCommons.TryGetJsonField(_jsonInput, "permissionLevelNormal", out permissionLevelDefault))
				{
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, _jsonInputData, EApiErrorCode.NO_OR_INVALID_PERMISSION_LEVEL_NORMAL, null);
					return;
				}
				string name2;
				JsonCommons.TryGetJsonField(_jsonInput, "name", out name2);
				UserPermissions.UsersInstance.AddGroup(name2, steamId, permissionLevelDefault, permissionLevelMod);
			}
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.Created, null, null, null);
		}

		// Token: 0x0600CF42 RID: 53058 RVA: 0x004B9F0C File Offset: 0x004B810C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestDelete(RequestContext _context)
		{
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			string steamId;
			if (!PermissionsApiHelpers.TryParseId(_context, null, out platformUserIdentifierAbs, out steamId))
			{
				return;
			}
			AbsRestApi.SendEmptyResponse(_context, ((platformUserIdentifierAbs != null) ? UserPermissions.UsersInstance.RemoveUser(platformUserIdentifierAbs, true) : UserPermissions.UsersInstance.RemoveGroup(steamId)) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound, null, null, null);
		}

		// Token: 0x1700197F RID: 6527
		// (get) Token: 0x0600CF43 RID: 53059 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool AllowPostWithId
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return true;
			}
		}

		// Token: 0x0600CF44 RID: 53060 RVA: 0x004B9519 File Offset: 0x004B7719
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

		// Token: 0x0600CF45 RID: 53061 RVA: 0x004B25BC File Offset: 0x004B07BC
		public UserPermissions() : base(null)
		{
		}

		// Token: 0x04009D93 RID: 40339
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyName = "name";

		// Token: 0x04009D94 RID: 40340
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyUserId = "userId";

		// Token: 0x04009D95 RID: 40341
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyPermissionLevel = "permissionLevel";

		// Token: 0x04009D96 RID: 40342
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyGroupId = "groupId";

		// Token: 0x04009D97 RID: 40343
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyPermissionLevelMods = "permissionLevelMods";

		// Token: 0x04009D98 RID: 40344
		[PublicizedFrom(EAccessModifier.Private)]
		public const string propertyPermissionLevelNormal = "permissionLevelNormal";

		// Token: 0x04009D99 RID: 40345
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyUsers = JsonWriter.GetEncodedPropertyNameWithBeginObject("users");

		// Token: 0x04009D9A RID: 40346
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyGroups = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("groups");

		// Token: 0x04009D9B RID: 40347
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyName = JsonWriter.GetEncodedPropertyNameWithBeginObject("name");

		// Token: 0x04009D9C RID: 40348
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyUserId = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("userId");

		// Token: 0x04009D9D RID: 40349
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPermissionLevel = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("permissionLevel");

		// Token: 0x04009D9E RID: 40350
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyGroupId = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("groupId");

		// Token: 0x04009D9F RID: 40351
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPermissionLevelMods = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("permissionLevelMods");

		// Token: 0x04009DA0 RID: 40352
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPermissionLevelNormal = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("permissionLevelNormal");
	}
}
