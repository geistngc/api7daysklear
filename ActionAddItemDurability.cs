using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001960 RID: 6496
	[Preserve]
	public class ActionAddItemDurability : ActionBaseItemAction
	{
		// Token: 0x0600C7E8 RID: 51176 RVA: 0x00497817 File Offset: 0x00495A17
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnClientActionStarted(EntityPlayer player)
		{
			base.OnClientActionStarted(player);
			this.amount = GameEventManager.GetFloatValue(player, this.amountText, 0.25f);
		}

		// Token: 0x0600C7E9 RID: 51177 RVA: 0x00497838 File Offset: 0x00495A38
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleItemStackChange(ref ItemStack stack, EntityPlayer player)
		{
			if (stack.itemValue.MaxUseTimes <= 0 || EffectManager.GetValue(PassiveEffects.DegradationPerUse, stack.itemValue, 1f, player, null, stack.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false) <= 0f)
			{
				return false;
			}
			if (this.itemTags != "" && !stack.itemValue.ItemClass.HasAnyTags(this.fastItemTags))
			{
				return false;
			}
			if (this.isNegative)
			{
				if (this.isPercent)
				{
					stack.itemValue.UseTimes += (float)stack.itemValue.MaxUseTimes * this.amount;
				}
				else
				{
					stack.itemValue.UseTimes += this.amount;
				}
			}
			else if (this.isPercent)
			{
				stack.itemValue.UseTimes -= (float)stack.itemValue.MaxUseTimes * this.amount;
			}
			else
			{
				stack.itemValue.UseTimes -= this.amount;
			}
			if (stack.itemValue.UseTimes < 0f)
			{
				stack.itemValue.UseTimes = 0f;
			}
			if (stack.itemValue.UseTimes > (float)stack.itemValue.MaxUseTimes)
			{
				stack.itemValue.UseTimes = (float)stack.itemValue.MaxUseTimes;
			}
			if (this.count != -1)
			{
				this.count--;
				if (this.count == 0)
				{
					this.isFinished = true;
				}
			}
			return true;
		}

		// Token: 0x0600C7EA RID: 51178 RVA: 0x004979DC File Offset: 0x00495BDC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleItemValueChange(ref ItemValue itemValue, EntityPlayer player)
		{
			if (itemValue.MaxUseTimes <= 0 || EffectManager.GetValue(PassiveEffects.DegradationPerUse, itemValue, 1f, player, null, itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false) <= 0f)
			{
				return false;
			}
			if (this.itemTags != "" && !itemValue.ItemClass.HasAnyTags(this.fastItemTags))
			{
				return false;
			}
			if (this.isNegative)
			{
				if (this.isPercent)
				{
					itemValue.UseTimes += (float)itemValue.MaxUseTimes * this.amount;
				}
				else
				{
					itemValue.UseTimes += this.amount;
				}
			}
			else if (this.isPercent)
			{
				itemValue.UseTimes -= (float)itemValue.MaxUseTimes * this.amount;
			}
			else
			{
				itemValue.UseTimes -= this.amount;
			}
			if (itemValue.UseTimes < 0f)
			{
				itemValue.UseTimes = 0f;
			}
			if (itemValue.UseTimes > (float)itemValue.MaxUseTimes)
			{
				itemValue.UseTimes = (float)itemValue.MaxUseTimes;
			}
			if (this.count != -1)
			{
				this.count--;
				if (this.count == 0)
				{
					this.isFinished = true;
				}
			}
			return true;
		}

		// Token: 0x0600C7EB RID: 51179 RVA: 0x00497B2E File Offset: 0x00495D2E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddItemDurability.PropAmount, ref this.amountText);
			properties.ParseBool(ActionAddItemDurability.PropIsPercent, ref this.isPercent);
			properties.ParseBool(ActionAddItemDurability.PropIsNegative, ref this.isNegative);
		}

		// Token: 0x0600C7EC RID: 51180 RVA: 0x00497B6A File Offset: 0x00495D6A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddItemDurability
			{
				isPercent = this.isPercent,
				isNegative = this.isNegative,
				amountText = this.amountText
			};
		}

		// Token: 0x040096F4 RID: 38644
		[PublicizedFrom(EAccessModifier.Protected)]
		public string amountText = "";

		// Token: 0x040096F5 RID: 38645
		[PublicizedFrom(EAccessModifier.Protected)]
		public float amount = 0.25f;

		// Token: 0x040096F6 RID: 38646
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isPercent = true;

		// Token: 0x040096F7 RID: 38647
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isNegative;

		// Token: 0x040096F8 RID: 38648
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAmount = "amount";

		// Token: 0x040096F9 RID: 38649
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIsPercent = "is_percent";

		// Token: 0x040096FA RID: 38650
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIsNegative = "is_negative";
	}
}
