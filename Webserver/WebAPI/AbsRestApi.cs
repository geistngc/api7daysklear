using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine.Profiling;
using Utf8Json;
using Webserver.Permissions;

namespace Webserver.WebAPI
{
	// Token: 0x02001ACF RID: 6863
	public abstract class AbsRestApi : AbsWebAPI
	{
		// Token: 0x0600CEB2 RID: 52914 RVA: 0x004B68F1 File Offset: 0x004B4AF1
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsRestApi(string _name = null) : this(null, _name)
		{
		}

		// Token: 0x0600CEB3 RID: 52915 RVA: 0x004B68FB File Offset: 0x004B4AFB
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsRestApi(Web _parentWeb, string _name = null) : base(_parentWeb, _name)
		{
		}

		// Token: 0x0600CEB4 RID: 52916 RVA: 0x004B6905 File Offset: 0x004B4B05
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void RegisterPermissions()
		{
			AdminWebModules.Instance.AddKnownModule(new AdminWebModules.WebModule(this.CachedApiModuleName, this.DefaultPermissionLevel(), this.DefaultMethodPermissionLevels(), true));
		}

		// Token: 0x0600CEB5 RID: 52917 RVA: 0x004B692C File Offset: 0x004B4B2C
		public sealed override void HandleRequest(RequestContext _context)
		{
			IDictionary<string, object> dictionary = null;
			byte[] array = null;
			if (_context.Request.HasEntityBody)
			{
				Stream inputStream = _context.Request.InputStream;
				array = new byte[_context.Request.ContentLength64];
				inputStream.Read(array, 0, (int)_context.Request.ContentLength64);
				try
				{
					dictionary = JsonSerializer.Deserialize<IDictionary<string, object>>(array);
				}
				catch (Exception exception)
				{
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, null, "INVALID_BODY", exception);
					return;
				}
			}
			try
			{
				switch (_context.Method)
				{
				case ERequestMethod.GET:
					if (dictionary != null)
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, array, "GET_WITH_BODY", null);
					}
					else
					{
						this.HandleRestGet(_context);
					}
					break;
				case ERequestMethod.POST:
					if (!this.AllowPostWithId && !string.IsNullOrEmpty(_context.RequestPath))
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, array, "POST_WITH_ID", null);
					}
					else if (dictionary == null)
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, null, "POST_WITHOUT_BODY", null);
					}
					else
					{
						this.HandleRestPost(_context, dictionary, array);
					}
					break;
				case ERequestMethod.PUT:
					if (string.IsNullOrEmpty(_context.RequestPath))
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, array, "PUT_WITHOUT_ID", null);
					}
					else if (dictionary == null)
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, null, "PUT_WITHOUT_BODY", null);
					}
					else
					{
						this.HandleRestPut(_context, dictionary, array);
					}
					break;
				case ERequestMethod.DELETE:
					if (string.IsNullOrEmpty(_context.RequestPath))
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, array, "DELETE_WITHOUT_ID", null);
					}
					else if (dictionary != null)
					{
						AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, null, "DELETE_WITH_BODY", null);
					}
					else
					{
						this.HandleRestDelete(_context);
					}
					break;
				default:
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.BadRequest, null, "INVALID_METHOD", null);
					break;
				}
			}
			catch (Exception exception2)
			{
				try
				{
					AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.InternalServerError, array, "ERROR_PROCESSING", exception2);
				}
				catch (Exception e)
				{
					Log.Error("[Web] In AbsRestApi.HandleRequest(): Handler " + this.Name + " threw an exception while trying to send a previous exception to the client:");
					Log.Exception(e);
				}
			}
		}

		// Token: 0x0600CEB6 RID: 52918 RVA: 0x004B6B64 File Offset: 0x004B4D64
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleRestGet(RequestContext _context)
		{
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.MethodNotAllowed, null, "Unsupported", null);
		}

		// Token: 0x0600CEB7 RID: 52919 RVA: 0x004B6B78 File Offset: 0x004B4D78
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleRestPost(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.MethodNotAllowed, _jsonInputData, "Unsupported", null);
		}

		// Token: 0x0600CEB8 RID: 52920 RVA: 0x004B6B78 File Offset: 0x004B4D78
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleRestPut(RequestContext _context, IDictionary<string, object> _jsonInput, byte[] _jsonInputData)
		{
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.MethodNotAllowed, _jsonInputData, "Unsupported", null);
		}

		// Token: 0x0600CEB9 RID: 52921 RVA: 0x004B6B64 File Offset: 0x004B4D64
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleRestDelete(RequestContext _context)
		{
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.MethodNotAllowed, null, "Unsupported", null);
		}

		// Token: 0x0600CEBA RID: 52922 RVA: 0x004B6B8C File Offset: 0x004B4D8C
		public override bool Authorized(RequestContext _context)
		{
			AdminWebModules.WebModule module = AdminWebModules.Instance.GetModule(this.CachedApiModuleName);
			if (module.LevelPerMethod == null)
			{
				return module.LevelGlobal >= _context.PermissionLevel;
			}
			int num = module.LevelPerMethod[(int)_context.Method];
			if (num == -2147483647)
			{
				return false;
			}
			if (num != -2147483648)
			{
				return num >= _context.PermissionLevel;
			}
			return module.LevelGlobal >= _context.PermissionLevel;
		}

		// Token: 0x17001979 RID: 6521
		// (get) Token: 0x0600CEBB RID: 52923 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool AllowPostWithId
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return false;
			}
		}

		// Token: 0x0600CEBC RID: 52924 RVA: 0x004B6C02 File Offset: 0x004B4E02
		public virtual int[] DefaultMethodPermissionLevels()
		{
			return new int[]
			{
				-2147483647,
				int.MinValue,
				int.MinValue,
				int.MinValue,
				int.MinValue
			};
		}

		// Token: 0x0600CEBD RID: 52925 RVA: 0x004B6C15 File Offset: 0x004B4E15
		[PublicizedFrom(EAccessModifier.Protected)]
		public static void PrepareEnvelopedResult(out JsonWriter _writer)
		{
			WebUtils.PrepareEnvelopedResult(out _writer);
		}

		// Token: 0x0600CEBE RID: 52926 RVA: 0x004B6C1D File Offset: 0x004B4E1D
		[PublicizedFrom(EAccessModifier.Protected)]
		public static void SendEnvelopedResult(RequestContext _context, ref JsonWriter _writer, HttpStatusCode _statusCode = HttpStatusCode.OK, byte[] _jsonInputData = null, string _errorCode = null, Exception _exception = null)
		{
			WebUtils.SendEnvelopedResult(_context, ref _writer, _statusCode, _jsonInputData, _errorCode, _exception);
		}

		// Token: 0x0600CEBF RID: 52927 RVA: 0x004B6C2C File Offset: 0x004B4E2C
		[PublicizedFrom(EAccessModifier.Protected)]
		public static void SendEmptyResponse(RequestContext _context, HttpStatusCode _statusCode = HttpStatusCode.OK, byte[] _jsonInputData = null, string _errorCode = null, Exception _exception = null)
		{
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(WebUtils.JsonEmptyData);
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, _statusCode, _jsonInputData, _errorCode, _exception);
		}

		// Token: 0x0600CEC0 RID: 52928 RVA: 0x004B6C59 File Offset: 0x004B4E59
		[PublicizedFrom(EAccessModifier.Protected)]
		public static void SendEmptyResponse(RequestContext _context, HttpStatusCode _statusCode, byte[] _jsonInputData, EApiErrorCode _errorCode, Exception _exception = null)
		{
			AbsRestApi.SendEmptyResponse(_context, _statusCode, _jsonInputData, _errorCode.ToStringCached<EApiErrorCode>(), _exception);
		}

		// Token: 0x04009CFC RID: 40188
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly CustomSampler jsonDeserializeSampler = CustomSampler.Create("JSON_Deserialize", false);
	}
}
