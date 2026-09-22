using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200197E RID: 6526
	[Preserve]
	public class ActionDropHeldItem : ActionBaseItemAction
	{
		// Token: 0x0600C876 RID: 51318 RVA: 0x0049B21C File Offset: 0x0049941C
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			return entityPlayer != null && !((entityPlayer.saveInventory != null) ? entityPlayer.saveInventory : entityPlayer.inventory).holdingItemStack.IsEmpty();
		}

		// Token: 0x0600C877 RID: 51319 RVA: 0x0049B25C File Offset: 0x0049945C
		public override void OnClientPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				Inventory inventory = (entityAlive.saveInventory != null) ? entityAlive.saveInventory : entityAlive.inventory;
				if (inventory.holdingItem != entityAlive.inventory.GetBareHandItem())
				{
					Vector3 dropPosition = entityAlive.GetDropPosition();
					ItemValue holdingItemItemValue = inventory.holdingItemItemValue;
					int count = inventory.holdingItemStack.count;
					GameManager.Instance.DropContentInLootContainerServer(entityAlive.entityId, "DroppedLootContainerTwitch", dropPosition, new ItemStack[]
					{
						inventory.holdingItemStack.Clone()
					}, false, null);
					entityAlive.AddUIHarvestingItem(new ItemStack(holdingItemItemValue, -count), false);
					Manager.BroadcastPlay(entityAlive, this.DropSound, false, 1f);
					inventory.DecHoldingItem(count);
				}
			}
		}

		// Token: 0x0600C878 RID: 51320 RVA: 0x0049B321 File Offset: 0x00499521
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionDropHeldItem.PropDropSound, ref this.DropSound);
		}

		// Token: 0x0600C879 RID: 51321 RVA: 0x0049B33B File Offset: 0x0049953B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionDropHeldItem
			{
				targetGroup = this.targetGroup,
				DropSound = this.DropSound
			};
		}

		// Token: 0x040097AF RID: 38831
		public string DropSound = "";

		// Token: 0x040097B0 RID: 38832
		public static string PropDropSound = "drop_sound";
	}
}
