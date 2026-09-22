using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Webserver.UrlHandlers;

namespace Webserver.SSE
{
	// Token: 0x02001AFE RID: 6910
	public abstract class AbsEvent
	{
		// Token: 0x0600CFC2 RID: 53186 RVA: 0x004BCE54 File Offset: 0x004BB054
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsEvent(SseHandler _parent, bool _reuseEncodingBuffer = true, string _name = null)
		{
			this.Name = (_name ?? base.GetType().Name);
			this.parent = _parent;
			if (_reuseEncodingBuffer)
			{
				this.encodingBuffer = new byte[1048576];
			}
		}

		// Token: 0x0600CFC3 RID: 53187 RVA: 0x004BCEB8 File Offset: 0x004BB0B8
		public void AddListener(SseClient _client)
		{
			this.totalOpened++;
			this.currentlyOpen++;
			this.openClients.Add(_client);
			this.logConnectionState("Connection opened", _client);
		}

		// Token: 0x0600CFC4 RID: 53188 RVA: 0x004BCEEE File Offset: 0x004BB0EE
		[PublicizedFrom(EAccessModifier.Protected)]
		public void SendData(string _eventName, string _data)
		{
			this.sendQueue.Enqueue(new ValueTuple<string, string>(_eventName, _data));
			this.parent.SignalSendQueue();
		}

		// Token: 0x0600CFC5 RID: 53189 RVA: 0x004BCF10 File Offset: 0x004BB110
		public void ProcessSendQueue()
		{
			while (this.sendQueue.HasData())
			{
				ValueTuple<string, string> valueTuple = this.sendQueue.Dequeue();
				string item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				this.stringBuilder.Append("event: ");
				this.stringBuilder.AppendLine(item);
				this.stringBuilder.Append("data: ");
				this.stringBuilder.AppendLine(item2);
				this.stringBuilder.AppendLine("");
				string text = this.stringBuilder.ToString();
				this.stringBuilder.Clear();
				byte[] bytes;
				int bytesToSend;
				if (this.encodingBuffer != null)
				{
					bytes = this.encodingBuffer;
					try
					{
						bytesToSend = Encoding.UTF8.GetBytes(text, 0, text.Length, bytes, 0);
						goto IL_DB;
					}
					catch (ArgumentException e)
					{
						Log.Error("[Web] [SSE] '" + this.Name + "': Exception while encoding data for output, most likely exceeding buffer size:");
						Log.Exception(e);
						break;
					}
					goto IL_CA;
				}
				goto IL_CA;
				IL_DB:
				this.sendBufToListeners(bytes, bytesToSend);
				continue;
				IL_CA:
				bytes = Encoding.UTF8.GetBytes(text);
				bytesToSend = bytes.Length;
				goto IL_DB;
			}
		}

		// Token: 0x0600CFC6 RID: 53190 RVA: 0x004BD024 File Offset: 0x004BB224
		[PublicizedFrom(EAccessModifier.Private)]
		public void sendBufToListeners(byte[] _bytes, int _bytesToSend)
		{
			for (int i = this.openClients.Count - 1; i >= 0; i--)
			{
				this.openClients[i].Write(_bytes, _bytesToSend);
			}
		}

		// Token: 0x0600CFC7 RID: 53191 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int DefaultPermissionLevel()
		{
			return 0;
		}

		// Token: 0x0600CFC8 RID: 53192 RVA: 0x004BD060 File Offset: 0x004BB260
		[PublicizedFrom(EAccessModifier.Private)]
		public void logConnectionState(string _message, SseClient _client)
		{
			Log.Out(string.Format("[Web] [SSE] '{0}': {1} from {2} (Left open: {3}, total opened: {4}, closed: {5})", new object[]
			{
				this.Name,
				_message,
				_client.RemoteEndpoint,
				this.currentlyOpen,
				this.totalOpened,
				this.totalClosed
			}));
		}

		// Token: 0x0600CFC9 RID: 53193 RVA: 0x004BD0C2 File Offset: 0x004BB2C2
		public void ClientClosed(SseClient _client)
		{
			if (this.openClients.Remove(_client))
			{
				this.currentlyOpen--;
				this.totalClosed++;
				this.logConnectionState("Closed connection", _client);
			}
		}

		// Token: 0x04009E1C RID: 40476
		[PublicizedFrom(EAccessModifier.Private)]
		public const int encodingBufferSize = 1048576;

		// Token: 0x04009E1D RID: 40477
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly SseHandler parent;

		// Token: 0x04009E1E RID: 40478
		public readonly string Name;

		// Token: 0x04009E1F RID: 40479
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly byte[] encodingBuffer;

		// Token: 0x04009E20 RID: 40480
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04009E21 RID: 40481
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<SseClient> openClients = new List<SseClient>();

		// Token: 0x04009E22 RID: 40482
		[TupleElementNames(new string[]
		{
			"_eventName",
			"_data"
		})]
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BlockingQueue<ValueTuple<string, string>> sendQueue = new BlockingQueue<ValueTuple<string, string>>();

		// Token: 0x04009E23 RID: 40483
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentlyOpen;

		// Token: 0x04009E24 RID: 40484
		[PublicizedFrom(EAccessModifier.Private)]
		public int totalOpened;

		// Token: 0x04009E25 RID: 40485
		[PublicizedFrom(EAccessModifier.Private)]
		public int totalClosed;
	}
}
