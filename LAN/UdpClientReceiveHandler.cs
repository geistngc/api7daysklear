using System;
using System.Net;
using System.Net.Sockets;

namespace Platform.LAN
{
	// Token: 0x02001CDC RID: 7388
	public class UdpClientReceiveHandler
	{
		// Token: 0x0600DB3F RID: 56127 RVA: 0x004E8114 File Offset: 0x004E6314
		public UdpClientReceiveHandler(UdpClient _udpClient)
		{
			this.udpClient = _udpClient;
		}

		// Token: 0x0600DB40 RID: 56128 RVA: 0x004E8124 File Offset: 0x004E6324
		public bool BeginReceive()
		{
			this.remoteEP = null;
			this.message = null;
			try
			{
				this.isComplete = false;
				this.udpClient.BeginReceive(new AsyncCallback(UdpClientReceiveHandler.CompleteReceiveAsync), this);
				return true;
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN receive handler unable to start receive. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.isComplete = true;
			return false;
		}

		// Token: 0x0600DB41 RID: 56129 RVA: 0x004E81B4 File Offset: 0x004E63B4
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CompleteReceiveAsync(IAsyncResult _result)
		{
			((UdpClientReceiveHandler)_result.AsyncState).CompleteReceive(_result);
		}

		// Token: 0x0600DB42 RID: 56130 RVA: 0x004E81C8 File Offset: 0x004E63C8
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteReceive(IAsyncResult _result)
		{
			try
			{
				this.message = this.udpClient.EndReceive(_result, ref this.remoteEP);
				this.length = this.message.Length;
				this.isComplete = true;
				return;
			}
			catch (ObjectDisposedException)
			{
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN receive handler unable to complete receive. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.remoteEP = null;
			this.message = null;
			this.length = 0;
			this.isComplete = true;
		}

		// Token: 0x0400A604 RID: 42500
		public readonly UdpClient udpClient;

		// Token: 0x0400A605 RID: 42501
		public IPEndPoint remoteEP;

		// Token: 0x0400A606 RID: 42502
		public byte[] message;

		// Token: 0x0400A607 RID: 42503
		public int length;

		// Token: 0x0400A608 RID: 42504
		public bool isComplete;
	}
}
