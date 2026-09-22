using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001802 RID: 6146
	public class TwitchActionPreset
	{
		// Token: 0x0600BE3A RID: 48698 RVA: 0x0046880A File Offset: 0x00466A0A
		public void AddCooldownModifier(TwitchActionCooldownModifier modifier)
		{
			if (this.ActionCooldownModifiers == null)
			{
				this.ActionCooldownModifiers = new List<TwitchActionCooldownModifier>();
			}
			this.ActionCooldownModifiers.Add(modifier);
		}

		// Token: 0x0600BE3B RID: 48699 RVA: 0x0046882C File Offset: 0x00466A2C
		public void HandleCooldowns()
		{
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				twitchAction.Cooldown = twitchAction.OriginalCooldown;
				if (twitchAction.IsInPreset(this) && this.ActionCooldownModifiers != null)
				{
					for (int i = 0; i < this.ActionCooldownModifiers.Count; i++)
					{
						TwitchActionCooldownModifier twitchActionCooldownModifier = this.ActionCooldownModifiers[i];
						if (twitchActionCooldownModifier.ActionName == twitchAction.Name || twitchActionCooldownModifier.CategoryName == twitchAction.MainCategory.Name)
						{
							switch (twitchActionCooldownModifier.Modifier)
							{
							case PassiveEffect.ValueModifierTypes.base_set:
								twitchAction.Cooldown = twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.base_add:
								twitchAction.Cooldown += twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.base_subtract:
								twitchAction.Cooldown -= twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.perc_set:
								twitchAction.Cooldown *= twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.perc_add:
								twitchAction.Cooldown += twitchAction.Cooldown * twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.perc_subtract:
								twitchAction.Cooldown -= twitchAction.Cooldown * twitchActionCooldownModifier.Value;
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x04008F69 RID: 36713
		public string Name;

		// Token: 0x04008F6A RID: 36714
		public bool IsEnabled = true;

		// Token: 0x04008F6B RID: 36715
		public bool IsDefault;

		// Token: 0x04008F6C RID: 36716
		public bool IsEmpty;

		// Token: 0x04008F6D RID: 36717
		public string Title;

		// Token: 0x04008F6E RID: 36718
		public string Description;

		// Token: 0x04008F6F RID: 36719
		public bool AllowPointGeneration = true;

		// Token: 0x04008F70 RID: 36720
		public bool UseHelperReward = true;

		// Token: 0x04008F71 RID: 36721
		public bool ShowNewCommands = true;

		// Token: 0x04008F72 RID: 36722
		public List<TwitchActionCooldownModifier> ActionCooldownModifiers;

		// Token: 0x04008F73 RID: 36723
		public List<string> AddedActions = new List<string>();

		// Token: 0x04008F74 RID: 36724
		public List<string> RemovedActions = new List<string>();
	}
}
