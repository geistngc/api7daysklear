using System;
using System.Net;
using System.Net.Sockets;

namespace Platform.LAN
{
	// Token: 0x02001CDB RID: 7387
	public class UdpClientSendHandler
	{
		// Token: 0x0600DB3B RID: 56123 RVA: 0x004E7FF3 File Offset: 0x004E61F3
		public UdpClientSendHandler(UdpClient _udpClient)
		{
			this.udpClient = _udpClient;
		}

		// Token: 0x0600DB3C RID: 56124 RVA: 0x004E8004 File Offset: 0x004E6204
		public bool BeginSend(byte[] _message, int _length, IPEndPoint _endPoint)
		{
			try
			{
				this.isComplete = false;
				this.udpClient.BeginSend(_message, _length, _endPoint, new AsyncCallback(UdpClientSendHandler.CompleteSendAsync), this);
				return true;
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN send handler unable to start send. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.isComplete = true;
			return false;
		}

		// Token: 0x0600DB3D RID: 56125 RVA: 0x004E8088 File Offset: 0x004E6288
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CompleteSendAsync(IAsyncResult _result)
		{
			((UdpClientSendHandler)_result.AsyncState).CompleteSend(_result);
		}

		// Token: 0x0600DB3E RID: 56126 RVA: 0x004E809C File Offset: 0x004E629C
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteSend(IAsyncResult _result)
		{
			try
			{
				this.udpClient.EndSend(_result);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN send handler unable to complete send. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.isComplete = true;
		}

		// Token: 0x0400A602 RID: 42498
		public readonly UdpClient udpClient;

		// Token: 0x0400A603 RID: 42499
		public bool isComplete;
	}
}
