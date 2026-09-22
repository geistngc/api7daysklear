using System;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001A00 RID: 6656
	[Preserve]
	public class ActionTwitchAddPoints : ActionBaseClientAction
	{
		// Token: 0x0600CACB RID: 51915 RVA: 0x004A7808 File Offset: 0x004A5A08
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null && base.Owner.Requester == entityPlayerLocal)
			{
				this.amount = GameEventManager.GetIntValue(entityPlayerLocal, this.amountText, 0);
				if (this.recipientType == ActionTwitchAddPoints.RecipientTypes.All)
				{
					switch (this.pointType)
					{
					case TwitchAction.PointTypes.PP:
						TwitchManager.Current.ViewerData.AddPointsAll(this.amount, 0, true);
						break;
					case TwitchAction.PointTypes.SP:
						TwitchManager.Current.ViewerData.AddPointsAll(0, this.amount, true);
						break;
					case TwitchAction.PointTypes.Bits:
						Debug.LogWarning("TwitchAddPoints: Cannot add Bit Credit to all.");
						break;
					}
					TwitchManager.Current.SendChannelMessage(Localization.Get(this.awardText, false, null), true);
					return;
				}
				this.viewer = ((this.recipientType == ActionTwitchAddPoints.RecipientTypes.Requester) ? base.Owner.ExtraData : TwitchManager.Current.ViewerData.GetRandomActiveViewer());
				if (this.viewer == "")
				{
					return;
				}
				switch (this.pointType)
				{
				case TwitchAction.PointTypes.PP:
					TwitchManager.Current.ViewerData.AddPoints(this.viewer, this.amount, false, false);
					break;
				case TwitchAction.PointTypes.SP:
					TwitchManager.Current.ViewerData.AddPoints(this.viewer, this.amount, true, false);
					break;
				case TwitchAction.PointTypes.Bits:
					TwitchManager.Current.ViewerData.AddCredit(this.viewer, this.amount, false);
					break;
				}
				if (this.awardText != "")
				{
					TwitchManager.Current.SendChannelMessage(base.GetTextWithElements(Localization.Get(this.awardText, false, null)), true);
				}
			}
		}

		// Token: 0x0600CACC RID: 51916 RVA: 0x004A79A7 File Offset: 0x004A5BA7
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string ParseTextElement(string element)
		{
			if (element == "amount")
			{
				return this.amount.ToString();
			}
			if (!(element == "viewer"))
			{
				return base.ParseTextElement(element);
			}
			return this.viewer;
		}

		// Token: 0x0600CACD RID: 51917 RVA: 0x004A79E0 File Offset: 0x004A5BE0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionTwitchAddPoints.PropAmount, ref this.amountText);
			properties.ParseEnum<TwitchAction.PointTypes>(ActionTwitchAddPoints.PropPointType, ref this.pointType);
			properties.ParseEnum<ActionTwitchAddPoints.RecipientTypes>(ActionTwitchAddPoints.PropRecipientType, ref this.recipientType);
			properties.ParseBool(ActionTwitchAddPoints.PropRequesterOnly, ref this.requesterOnly);
			properties.ParseString(ActionTwitchAddPoints.PropAwardText, ref this.awardText);
		}

		// Token: 0x0600CACE RID: 51918 RVA: 0x004A7A4C File Offset: 0x004A5C4C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchAddPoints
			{
				amountText = this.amountText,
				pointType = this.pointType,
				recipientType = this.recipientType,
				requesterOnly = this.requesterOnly,
				awardText = this.awardText
			};
		}

		// Token: 0x04009A44 RID: 39492
		[PublicizedFrom(EAccessModifier.Protected)]
		public string amountText;

		// Token: 0x04009A45 RID: 39493
		[PublicizedFrom(EAccessModifier.Protected)]
		public string viewer;

		// Token: 0x04009A46 RID: 39494
		[PublicizedFrom(EAccessModifier.Protected)]
		public int amount;

		// Token: 0x04009A47 RID: 39495
		[PublicizedFrom(EAccessModifier.Protected)]
		public TwitchAction.PointTypes pointType;

		// Token: 0x04009A48 RID: 39496
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool requesterOnly = true;

		// Token: 0x04009A49 RID: 39497
		[PublicizedFrom(EAccessModifier.Protected)]
		public string awardText = "";

		// Token: 0x04009A4A RID: 39498
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionTwitchAddPoints.RecipientTypes recipientType;

		// Token: 0x04009A4B RID: 39499
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAmount = "amount";

		// Token: 0x04009A4C RID: 39500
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPointType = "point_type";

		// Token: 0x04009A4D RID: 39501
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRecipientType = "recipient_type";

		// Token: 0x04009A4E RID: 39502
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRequesterOnly = "requester_only";

		// Token: 0x04009A4F RID: 39503
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAwardText = "award_text";

		// Token: 0x02001A01 RID: 6657
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum RecipientTypes
		{
			// Token: 0x04009A51 RID: 39505
			Requester,
			// Token: 0x04009A52 RID: 39506
			All,
			// Token: 0x04009A53 RID: 39507
			Random
		}
	}
}
