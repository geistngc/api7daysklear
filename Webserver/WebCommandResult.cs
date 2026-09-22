using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SpaceWizards.HttpListener;
using UnityEngine;
using Utf8Json;

namespace Webserver
{
	// Token: 0x02001AC9 RID: 6857
	public class WebCommandResult : IConsoleConnection
	{
		// Token: 0x0600CE92 RID: 52882 RVA: 0x004B6110 File Offset: 0x004B4310
		public WebCommandResult(string _command, string _parameters, WebCommandResult.ResultType _responseType, RequestContext _context)
		{
			this.context = _context;
			this.command = _command;
			this.parameters = _parameters;
			this.responseType = _responseType;
			WebConnection connection = _context.Connection;
			this.sourceName = (((connection != null) ? connection.Username : null) ?? ("Unauth-PermLevel-" + _context.PermissionLevel.ToString()));
		}

		// Token: 0x0600CE93 RID: 52883 RVA: 0x004B6178 File Offset: 0x004B4378
		public void SendLines(List<string> _output)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in _output)
			{
				stringBuilder.AppendLine(value);
			}
			string text = stringBuilder.ToString();
			try
			{
				if (this.responseType == WebCommandResult.ResultType.Raw)
				{
					WebUtils.WriteText(this.context.Response, text, HttpStatusCode.OK, null);
				}
				else
				{
					JsonWriter jsonWriter;
					WebUtils.PrepareEnvelopedResult(out jsonWriter);
					if (this.responseType == WebCommandResult.ResultType.ResultOnly)
					{
						jsonWriter.WriteRaw(WebCommandResult.jsonRawKey);
						jsonWriter.WriteString(text);
						jsonWriter.WriteEndObject();
					}
					else
					{
						jsonWriter.WriteRaw(WebCommandResult.jsonCommandKey);
						jsonWriter.WriteString(this.command);
						jsonWriter.WriteRaw(WebCommandResult.jsonParametersKey);
						jsonWriter.WriteString(this.parameters);
						jsonWriter.WriteRaw(WebCommandResult.jsonResultKey);
						jsonWriter.WriteString(text);
						jsonWriter.WriteEndObject();
					}
					WebUtils.SendEnvelopedResult(this.context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
				}
			}
			catch (IOException ex)
			{
				if (ex.InnerException is SocketException)
				{
					Log.Warning("[Web] Error in WebCommandResult.SendLines(): Remote host closed connection: " + ex.InnerException.Message);
				}
				else
				{
					Log.Warning(string.Format("[Web] Error (IO) in WebCommandResult.SendLines(): {0}", ex));
				}
			}
			catch (Exception arg)
			{
				Log.Warning(string.Format("[Web] Error in WebCommandResult.SendLines(): {0}", arg));
			}
			finally
			{
				RequestContext requestContext = this.context;
				if (requestContext != null)
				{
					SpaceWizards.HttpListener.HttpListenerResponse response = requestContext.Response;
					if (response != null)
					{
						response.Close();
					}
				}
			}
		}

		// Token: 0x0600CE94 RID: 52884 RVA: 0x000027FC File Offset: 0x000009FC
		public void SendLine(string _text)
		{
		}

		// Token: 0x0600CE95 RID: 52885 RVA: 0x000027FC File Offset: 0x000009FC
		public void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
		{
		}

		// Token: 0x0600CE96 RID: 52886 RVA: 0x000027FC File Offset: 0x000009FC
		public void EnableLogLevel(LogType _type, bool _enable)
		{
		}

		// Token: 0x0600CE97 RID: 52887 RVA: 0x004B6350 File Offset: 0x004B4550
		public string GetDescription()
		{
			return "WebCommandResult_for_" + this.command + "_by_" + this.sourceName;
		}

		// Token: 0x04009CCF RID: 40143
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string command;

		// Token: 0x04009CD0 RID: 40144
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string parameters;

		// Token: 0x04009CD1 RID: 40145
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string sourceName;

		// Token: 0x04009CD2 RID: 40146
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly RequestContext context;

		// Token: 0x04009CD3 RID: 40147
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WebCommandResult.ResultType responseType;

		// Token: 0x04009CD4 RID: 40148
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonRawKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("resultRaw");

		// Token: 0x04009CD5 RID: 40149
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonCommandKey = JsonWriter.GetEncodedPropertyNameWithBeginObject("command");

		// Token: 0x04009CD6 RID: 40150
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonParametersKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("parameters");

		// Token: 0x04009CD7 RID: 40151
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonResultKey = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("result");

		// Token: 0x02001ACA RID: 6858
		public enum ResultType
		{
			// Token: 0x04009CD9 RID: 40153
			Full,
			// Token: 0x04009CDA RID: 40154
			ResultOnly,
			// Token: 0x04009CDB RID: 40155
			Raw
		}
	}
}
