using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SpaceWizards.HttpListener;
using Webserver.UrlHandlers;

namespace Webserver.SSE
{
	// Token: 0x02001B01 RID: 6913
	public class SseClient
	{
		// Token: 0x0600CFCC RID: 53196 RVA: 0x004BD154 File Offset: 0x004BB354
		public SseClient(SseHandler _parent, RequestContext _context)
		{
			this.parent = _parent;
			this.response = _context.Response;
			this.RemoteEndpoint = _context.Request.RemoteEndPoint;
			this.response.SendChunked = true;
			this.response.AddHeader("Content-Type", "text/event-stream");
			this.response.OutputStream.Flush();
		}

		// Token: 0x0600CFCD RID: 53197 RVA: 0x004BD1C8 File Offset: 0x004BB3C8
		public ESseClientWriteResult Write(byte[] _bytes, int _bytesToSend)
		{
			SpaceWizards.HttpListener.HttpListenerResponse httpListenerResponse = this.response;
			ESseClientWriteResult result;
			try
			{
				if (!httpListenerResponse.OutputStream.CanWrite)
				{
					this.parent.ClientClosed(this);
					httpListenerResponse.Close();
					result = ESseClientWriteResult.Closed;
				}
				else
				{
					httpListenerResponse.OutputStream.Write(_bytes, 0, _bytesToSend);
					httpListenerResponse.OutputStream.Flush();
					this.lastMessageSent = DateTime.Now;
					result = ESseClientWriteResult.Ok;
				}
			}
			catch (IOException ex)
			{
				this.parent.ClientClosed(this);
				SocketException ex2 = ex.InnerException as SocketException;
				if (ex2 != null)
				{
					if (ex2.SocketErrorCode == SocketError.ConnectionAborted || ex2.SocketErrorCode == SocketError.Shutdown)
					{
						result = ESseClientWriteResult.Closed;
					}
					else
					{
						Log.Error("[Web] [SSE] SocketError (" + ex2.SocketErrorCode.ToStringCached<SocketError>() + ") while trying to write", new object[]
						{
							true
						});
						result = ESseClientWriteResult.Error;
					}
				}
				else
				{
					Log.Error("[Web] [SSE] IOException while trying to write:", new object[]
					{
						true
					});
					Log.Exception(ex);
					result = ESseClientWriteResult.Error;
				}
			}
			catch (Exception e)
			{
				this.parent.ClientClosed(this);
				httpListenerResponse.Close();
				Log.Error("[Web] [SSE] Exception while trying to write:", new object[]
				{
					true
				});
				Log.Exception(e);
				result = ESseClientWriteResult.Error;
			}
			return result;
		}

		// Token: 0x0600CFCE RID: 53198 RVA: 0x004BD314 File Offset: 0x004BB514
		public void HandleKeepAlive()
		{
			DateTime now = DateTime.Now;
			if ((now - this.lastMessageSent).TotalSeconds < 10.0)
			{
				return;
			}
			this.Write(SseClient.keepAliveData, SseClient.keepAliveData.Length);
			this.lastMessageSent = now;
		}

		// Token: 0x04009E2A RID: 40490
		[PublicizedFrom(EAccessModifier.Private)]
		public const int keepAliveIntervalSeconds = 10;

		// Token: 0x04009E2B RID: 40491
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] keepAliveData = Encoding.UTF8.GetBytes(": KeepAlive\n\n");

		// Token: 0x04009E2C RID: 40492
		public readonly IPEndPoint RemoteEndpoint;

		// Token: 0x04009E2D RID: 40493
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly SseHandler parent;

		// Token: 0x04009E2E RID: 40494
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly SpaceWizards.HttpListener.HttpListenerResponse response;

		// Token: 0x04009E2F RID: 40495
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime lastMessageSent = DateTime.Now;
	}
}
