using System;
using UnityEngine;

// Token: 0x02000039 RID: 57
[AddComponentMenu("NGUI/Examples/Equip Items")]
public class EquipItems : MonoBehaviour
{
	// Token: 0x06000154 RID: 340 RVA: 0x0000E984 File Offset: 0x0000CB84
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		if (this.itemIDs != null && this.itemIDs.Length != 0)
		{
			InvEquipment invEquipment = base.GetComponent<InvEquipment>();
			if (invEquipment == null)
			{
				invEquipment = base.gameObject.AddComponent<InvEquipment>();
			}
			int maxExclusive = 12;
			int i = 0;
			int num = this.itemIDs.Length;
			while (i < num)
			{
				int num2 = this.itemIDs[i];
				InvBaseItem invBaseItem = InvDatabase.FindByID(num2);
				if (invBaseItem != null)
				{
					invEquipment.Equip(new InvGameItem(num2, invBaseItem)
					{
						quality = (InvGameItem.Quality)UnityEngine.Random.Range(0, maxExclusive),
						itemLevel = NGUITools.RandomRange(invBaseItem.minItemLevel, invBaseItem.maxItemLevel)
					});
				}
				else
				{
					Debug.LogWarning("Can't resolve the item ID of " + num2.ToString());
				}
				i++;
			}
		}
		UnityEngine.Object.Destroy(this);
	}

	// Token: 0x040001E8 RID: 488
	public int[] itemIDs;
}
