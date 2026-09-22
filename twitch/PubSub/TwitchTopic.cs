using System;

namespace Twitch.PubSub
{
	// Token: 0x02001894 RID: 6292
	public class TwitchTopic
	{
		// Token: 0x0600C1F7 RID: 49655 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchTopic()
		{
		}

		// Token: 0x170017D5 RID: 6101
		// (get) Token: 0x0600C1F8 RID: 49656 RVA: 0x0047D52E File Offset: 0x0047B72E
		// (set) Token: 0x0600C1F9 RID: 49657 RVA: 0x0047D536 File Offset: 0x0047B736
		public string TopicString { get; set; }

		// Token: 0x0600C1FA RID: 49658 RVA: 0x0047D53F File Offset: 0x0047B73F
		public static TwitchTopic ChannelPoints(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("channel-points-channel-v1.{0}", channelId)
			};
		}

		// Token: 0x0600C1FB RID: 49659 RVA: 0x0047D557 File Offset: 0x0047B757
		public static TwitchTopic Bits(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("channel-bits-events-v2.{0}", channelId)
			};
		}

		// Token: 0x0600C1FC RID: 49660 RVA: 0x0047D56F File Offset: 0x0047B76F
		public static TwitchTopic Subscription(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("channel-subscribe-events-v1.{0}", channelId)
			};
		}

		// Token: 0x0600C1FD RID: 49661 RVA: 0x0047D587 File Offset: 0x0047B787
		public static TwitchTopic HypeTrain(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("hype-train-events-v1.{0}", channelId)
			};
		}

		// Token: 0x0600C1FE RID: 49662 RVA: 0x0047D59F File Offset: 0x0047B79F
		public static TwitchTopic CreatorGoal(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("creator-goals-events-v1.{0}", channelId)
			};
		}
	}
}
