using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Platform.LAN
{
	// Token: 0x02001CD2 RID: 7378
	public class LANMasterServerAnnouncer : IMasterServerAnnouncer
	{
		// Token: 0x17001B5C RID: 7004
		// (get) Token: 0x0600DB09 RID: 56073 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool GameServerInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600DB0A RID: 56074 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600DB0B RID: 56075 RVA: 0x0004E558 File Offset: 0x0004C758
		public string GetServerPorts()
		{
			return string.Empty;
		}

		// Token: 0x0600DB0C RID: 56076 RVA: 0x004E776C File Offset: 0x004E596C
		public void AdvertiseServer(Action _onServerRegistered)
		{
			int num = 11000;
			IPAddress multicastGroupIp = LANServerSearchConfig.MulticastGroupIp;
			try
			{
				this.udpClient = new UdpClient(num);
				this.udpClient.JoinMulticastGroup(multicastGroupIp);
				this.receiveHandler = new UdpClientReceiveHandler(this.udpClient);
				this.shouldAnnounce = true;
				this.replyCoroutine = ThreadManager.StartCoroutine(this.LANServerListReplyTask());
				Log.Out(string.Format("[{0}] listening on {1} and multicast group {2}", "LANMasterServerAnnouncer", num, multicastGroupIp));
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("[{0}] could not start LAN server search listening on port {1} and multicast group {2}. ErrorCode: {3}, Message: {4}", new object[]
				{
					"LANMasterServerAnnouncer",
					num,
					multicastGroupIp,
					ex.ErrorCode,
					ex.Message
				}));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			_onServerRegistered();
		}

		// Token: 0x0600DB0D RID: 56077 RVA: 0x004E7850 File Offset: 0x004E5A50
		public IEnumerator LANServerListReplyTask()
		{
			while (this.shouldAnnounce)
			{
				if (!this.receiveHandler.BeginReceive())
				{
					Log.Error("[LANMasterServerAnnouncer] could not start receive");
					yield break;
				}
				while (!this.receiveHandler.isComplete)
				{
					yield return null;
				}
				IPEndPoint remoteEP = this.receiveHandler.remoteEP;
				byte[] message = this.receiveHandler.message;
				int length = this.receiveHandler.length;
				if (remoteEP != null && message != null && length == 0)
				{
					this.SendReply(remoteEP);
				}
			}
			yield break;
		}

		// Token: 0x0600DB0E RID: 56078 RVA: 0x004E7860 File Offset: 0x004E5A60
		[PublicizedFrom(EAccessModifier.Private)]
		public void SendReply(IPEndPoint _remoteEP)
		{
			try
			{
				int value = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoInt.Port);
				int size = 0;
				StreamUtils.Write(LANMasterServerAnnouncer.replyBuffer, value, ref size);
				this.udpClient.Client.SendTo(LANMasterServerAnnouncer.replyBuffer, 0, size, SocketFlags.None, _remoteEP);
			}
			catch (Exception e)
			{
				Log.Error(string.Format("[{0}] could not send reply to {1}", "LANMasterServerAnnouncer", _remoteEP));
				Log.Exception(e);
			}
		}

		// Token: 0x0600DB0F RID: 56079 RVA: 0x000027FC File Offset: 0x000009FC
		public void Update()
		{
		}

		// Token: 0x0600DB10 RID: 56080 RVA: 0x004E78D8 File Offset: 0x004E5AD8
		public void StopServer()
		{
			this.shouldAnnounce = false;
			if (this.replyCoroutine != null)
			{
				ThreadManager.StopCoroutine(this.replyCoroutine);
			}
			UdpClient udpClient = this.udpClient;
			if (udpClient != null)
			{
				udpClient.Dispose();
			}
			this.udpClient = null;
			this.receiveHandler = null;
		}

		// Token: 0x0400A5DB RID: 42459
		[PublicizedFrom(EAccessModifier.Private)]
		public bool shouldAnnounce = true;

		// Token: 0x0400A5DC RID: 42460
		[PublicizedFrom(EAccessModifier.Private)]
		public UdpClient udpClient;

		// Token: 0x0400A5DD RID: 42461
		[PublicizedFrom(EAccessModifier.Private)]
		public UdpClientReceiveHandler receiveHandler;

		// Token: 0x0400A5DE RID: 42462
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine replyCoroutine;

		// Token: 0x0400A5DF RID: 42463
		[PublicizedFrom(EAccessModifier.Private)]
		public static byte[] emptyMessage = new byte[0];

		// Token: 0x0400A5E0 RID: 42464
		[PublicizedFrom(EAccessModifier.Private)]
		public static byte[] replyBuffer = new byte[4];
	}
}
