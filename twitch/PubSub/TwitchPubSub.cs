using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Twitch.PubSub
{
	// Token: 0x0200188C RID: 6284
	public class TwitchPubSub
	{
		// Token: 0x0600C1D3 RID: 49619 RVA: 0x0047C554 File Offset: 0x0047A754
		public void Connect(string userID)
		{
			if (this.cts != null)
			{
				this.cts.Cancel();
			}
			this.cts = new CancellationTokenSource();
			Task.Run(() => this.StartAsync(new TwitchTopic[]
			{
				TwitchTopic.ChannelPoints(userID),
				TwitchTopic.Bits(userID),
				TwitchTopic.Subscription(userID),
				TwitchTopic.HypeTrain(userID),
				TwitchTopic.CreatorGoal(userID)
			}, this.cts.Token));
		}

		// Token: 0x0600C1D4 RID: 49620 RVA: 0x0047C5A3 File Offset: 0x0047A7A3
		public void Disconnect()
		{
			this.cts.Cancel();
			TwitchPubSub.reconnect = false;
		}

		// Token: 0x0600C1D5 RID: 49621 RVA: 0x0047C5B8 File Offset: 0x0047A7B8
		public Task StartAsync(TwitchTopic[] newTopics, CancellationToken token)
		{
			TwitchPubSub.<StartAsync>d__12 <StartAsync>d__;
			<StartAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartAsync>d__.<>4__this = this;
			<StartAsync>d__.newTopics = newTopics;
			<StartAsync>d__.token = token;
			<StartAsync>d__.<>1__state = -1;
			<StartAsync>d__.<>t__builder.Start<TwitchPubSub.<StartAsync>d__12>(ref <StartAsync>d__);
			return <StartAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600C1D6 RID: 49622 RVA: 0x0047C60C File Offset: 0x0047A80C
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleMessage(string receivedMessage, TwitchPubSub.MessageTypes msgType)
		{
			if (msgType != TwitchPubSub.MessageTypes.Standard)
			{
				if (msgType == TwitchPubSub.MessageTypes.HypeStart)
				{
					TwitchManager.Current.StartHypeTrain();
				}
				return;
			}
			JObject jobject = JObject.Parse(receivedMessage);
			string a = jobject["type"].Value<string>();
			if (a == "RESPONSE" && jobject["error"].Value<string>() != "")
			{
				return;
			}
			if (a == "RESPONSE")
			{
				return;
			}
			if (this.HandlePongMessage(receivedMessage))
			{
				return;
			}
			if (this.HandleReconnectMessage(receivedMessage))
			{
				return;
			}
			this.HandleRedemptionsMessages(receivedMessage);
		}

		// Token: 0x0600C1D7 RID: 49623 RVA: 0x0047C698 File Offset: 0x0047A898
		[PublicizedFrom(EAccessModifier.Private)]
		public Task StartListening(IEnumerable<TwitchTopic> topics)
		{
			TwitchPubSub.<StartListening>d__14 <StartListening>d__;
			<StartListening>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartListening>d__.<>4__this = this;
			<StartListening>d__.topics = topics;
			<StartListening>d__.<>1__state = -1;
			<StartListening>d__.<>t__builder.Start<TwitchPubSub.<StartListening>d__14>(ref <StartListening>d__);
			return <StartListening>d__.<>t__builder.Task;
		}

		// Token: 0x0600C1D8 RID: 49624 RVA: 0x0047C6E4 File Offset: 0x0047A8E4
		[PublicizedFrom(EAccessModifier.Private)]
		public void PingTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			string message = "{ \"type\": \"PING\" }";
			this.SendMessageOnSocket(message).GetAwaiter().GetResult();
			this.pongTimer = new System.Timers.Timer(TimeSpan.FromSeconds(10.0).TotalMilliseconds);
			this.pongTimer.Elapsed += this.PongTimer_Elapsed;
			this.pongTimer.Start();
			this.pingAcknowledged = false;
		}

		// Token: 0x0600C1D9 RID: 49625 RVA: 0x0047C755 File Offset: 0x0047A955
		[PublicizedFrom(EAccessModifier.Private)]
		public void PongTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!this.pingAcknowledged)
			{
				TwitchPubSub.reconnect = true;
				this.pongTimer.Dispose();
			}
		}

		// Token: 0x0600C1DA RID: 49626 RVA: 0x0047C770 File Offset: 0x0047A970
		[PublicizedFrom(EAccessModifier.Private)]
		public Task SendMessageOnSocket(string message)
		{
			if (this.socket.State != WebSocketState.Open)
			{
				return Task.CompletedTask;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(message);
			return this.socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
		}

		// Token: 0x14000113 RID: 275
		// (add) Token: 0x0600C1DB RID: 49627 RVA: 0x0047C7B8 File Offset: 0x0047A9B8
		// (remove) Token: 0x0600C1DC RID: 49628 RVA: 0x0047C7F0 File Offset: 0x0047A9F0
		public event EventHandler<PubSubBitRedemptionMessage.BitRedemptionData> OnBitsRedeemed;

		// Token: 0x14000114 RID: 276
		// (add) Token: 0x0600C1DD RID: 49629 RVA: 0x0047C828 File Offset: 0x0047AA28
		// (remove) Token: 0x0600C1DE RID: 49630 RVA: 0x0047C860 File Offset: 0x0047AA60
		public event EventHandler<PubSubSubscriptionRedemptionMessage> OnSubscriptionRedeemed;

		// Token: 0x14000115 RID: 277
		// (add) Token: 0x0600C1DF RID: 49631 RVA: 0x0047C898 File Offset: 0x0047AA98
		// (remove) Token: 0x0600C1E0 RID: 49632 RVA: 0x0047C8D0 File Offset: 0x0047AAD0
		public event EventHandler<PubSubChannelPointMessage.ChannelRedemptionData> OnChannelPointsRedeemed;

		// Token: 0x14000116 RID: 278
		// (add) Token: 0x0600C1E1 RID: 49633 RVA: 0x0047C908 File Offset: 0x0047AB08
		// (remove) Token: 0x0600C1E2 RID: 49634 RVA: 0x0047C940 File Offset: 0x0047AB40
		public event EventHandler<PubSubGoalMessage.Goal> OnGoalAchieved;

		// Token: 0x0600C1E3 RID: 49635 RVA: 0x0047C975 File Offset: 0x0047AB75
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HandlePongMessage(string message)
		{
			if (message.Contains("\"PONG\""))
			{
				this.pingAcknowledged = true;
				this.pongTimer.Stop();
				this.pongTimer.Dispose();
				return true;
			}
			return false;
		}

		// Token: 0x0600C1E4 RID: 49636 RVA: 0x0047C9A4 File Offset: 0x0047ABA4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HandleReconnectMessage(string message)
		{
			if (message.Contains("\"RECONNECT\""))
			{
				TwitchPubSub.reconnect = true;
				return true;
			}
			return false;
		}

		// Token: 0x0600C1E5 RID: 49637 RVA: 0x0047C9BC File Offset: 0x0047ABBC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HandleRedemptionsMessages(string message)
		{
			JObject jobject = JObject.Parse(message);
			if (jobject["type"].Value<string>() == "MESSAGE")
			{
				string text = jobject["data"]["topic"].Value<string>();
				if (text.StartsWith("channel-points-channel-v1"))
				{
					string message2 = jobject["data"]["message"].Value<string>();
					PubSubChannelPointMessage pubSubChannelPointMessage = null;
					try
					{
						pubSubChannelPointMessage = PubSubChannelPointMessage.Deserialize(message2);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.ToString());
						Debug.LogError(message2);
					}
					if (this.OnChannelPointsRedeemed != null && pubSubChannelPointMessage != null)
					{
						this.OnChannelPointsRedeemed(null, pubSubChannelPointMessage.data);
					}
					return true;
				}
				if (text.StartsWith("channel-bits-events"))
				{
					string message3 = jobject["data"]["message"].Value<string>();
					PubSubBitRedemptionMessage pubSubBitRedemptionMessage = null;
					try
					{
						pubSubBitRedemptionMessage = PubSubBitRedemptionMessage.Deserialize(message3);
					}
					catch (Exception ex2)
					{
						Debug.LogError(ex2.ToString());
						Debug.LogError(message3);
					}
					if (this.OnBitsRedeemed != null && pubSubBitRedemptionMessage != null)
					{
						this.OnBitsRedeemed(null, pubSubBitRedemptionMessage.data);
					}
					return true;
				}
				if (text.StartsWith("channel-subscribe-events"))
				{
					string message4 = jobject["data"]["message"].Value<string>();
					PubSubSubscriptionRedemptionMessage pubSubSubscriptionRedemptionMessage = null;
					try
					{
						pubSubSubscriptionRedemptionMessage = PubSubSubscriptionRedemptionMessage.Deserialize(message4);
					}
					catch (Exception ex3)
					{
						Debug.LogError(ex3.ToString());
						Debug.LogError(message4);
					}
					if (this.OnSubscriptionRedeemed != null && pubSubSubscriptionRedemptionMessage != null)
					{
						this.OnSubscriptionRedeemed(null, pubSubSubscriptionRedemptionMessage);
					}
					return true;
				}
				if (text.StartsWith("creator-goals-events"))
				{
					string message5 = jobject["data"]["message"].Value<string>();
					PubSubGoalMessage pubSubGoalMessage = null;
					try
					{
						pubSubGoalMessage = PubSubGoalMessage.Deserialize(message5);
					}
					catch (Exception ex4)
					{
						Debug.LogError(ex4.ToString());
						Debug.LogError(message5);
					}
					if (pubSubGoalMessage.type == "goal_achieved" && this.OnGoalAchieved != null && pubSubGoalMessage != null)
					{
						this.OnGoalAchieved(null, pubSubGoalMessage.data.goal);
					}
					return true;
				}
				if (text.StartsWith("hype-train-events-v1"))
				{
					try
					{
						string text2 = jobject["data"]["message"].ToString();
						Debug.LogWarning(text2);
						if (text2.Contains("hype-train-start"))
						{
							TwitchManager.Current.StartHypeTrain();
						}
						else if (text2.Contains("hype-train-level-up"))
						{
							TwitchManager.Current.IncrementHypeTrainLevel();
						}
						else if (text2.Contains("hype-train-end"))
						{
							TwitchManager.Current.EndHypeTrain();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogWarning("Hype Train Error: " + message);
						Debug.LogWarning("Hype Train Exception: " + ex5.ToString());
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600C1E6 RID: 49638 RVA: 0x0047CCB0 File Offset: 0x0047AEB0
		public void Cleanup()
		{
			if (this.pingTimer != null)
			{
				this.pingTimer.Dispose();
			}
			if (this.pongTimer != null)
			{
				this.pongTimer.Dispose();
			}
			if (this.socket != null)
			{
				this.socket.Dispose();
			}
		}

		// Token: 0x040092E7 RID: 37607
		[PublicizedFrom(EAccessModifier.Private)]
		public ClientWebSocket socket;

		// Token: 0x040092E8 RID: 37608
		[PublicizedFrom(EAccessModifier.Private)]
		public System.Timers.Timer pingTimer;

		// Token: 0x040092E9 RID: 37609
		[PublicizedFrom(EAccessModifier.Private)]
		public System.Timers.Timer pongTimer;

		// Token: 0x040092EA RID: 37610
		[PublicizedFrom(EAccessModifier.Private)]
		public System.Timers.Timer reconnectTimer = new System.Timers.Timer();

		// Token: 0x040092EB RID: 37611
		[PublicizedFrom(EAccessModifier.Private)]
		public bool pingAcknowledged;

		// Token: 0x040092EC RID: 37612
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool reconnect = false;

		// Token: 0x040092ED RID: 37613
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchTopic[] topics;

		// Token: 0x040092EE RID: 37614
		[PublicizedFrom(EAccessModifier.Private)]
		public CancellationTokenSource cts;

		// Token: 0x040092EF RID: 37615
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan[] _ReconnectTimeouts = new TimeSpan[]
		{
			TimeSpan.FromSeconds(1.0),
			TimeSpan.FromSeconds(5.0),
			TimeSpan.FromSeconds(10.0),
			TimeSpan.FromSeconds(30.0),
			TimeSpan.FromMinutes(1.0),
			TimeSpan.FromMinutes(5.0)
		};

		// Token: 0x0200188D RID: 6285
		[PublicizedFrom(EAccessModifier.Private)]
		public enum MessageTypes
		{
			// Token: 0x040092F5 RID: 37621
			Standard,
			// Token: 0x040092F6 RID: 37622
			HypeStart
		}
	}
}
