using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200023F RID: 575
[Preserve]
public class ConsoleCmdLoot : ConsoleCmdAbstract
{
	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x0600114C RID: 4428 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x0600114D RID: 4429 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x0600114E RID: 4430 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x0006DC2F File Offset: 0x0006BE2F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"loot"
		};
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0006DC3F File Offset: 0x0006BE3F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Loot commands";
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x0006DC46 File Offset: 0x0006BE46
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Loot commands:\ncontainer [name] <count> <stage> <abundance> - list loot from named container for count times";
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0006DC50 File Offset: 0x0006BE50
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string text = _params[0].ToLower();
		if (text == "c" || text == "container")
		{
			if (_params.Count >= 2)
			{
				if (!LootContainer.IsLoaded())
				{
					WorldStaticData.InitSync(true, false, false, null);
				}
				int count = 1;
				if (_params.Count >= 3)
				{
					int.TryParse(_params[2], out count);
				}
				int stage = 1;
				if (_params.Count >= 4)
				{
					int.TryParse(_params[3], out stage);
				}
				float abundance = 1f;
				if (_params.Count >= 5)
				{
					float.TryParse(_params[4], out abundance);
				}
				this.ContainerList(_params[1], count, stage, abundance);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown command " + text);
		}
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0006DD34 File Offset: 0x0006BF34
	[PublicizedFrom(EAccessModifier.Private)]
	public void ContainerList(string _name, int _count, int _stage, float _abundance)
	{
		LootContainer lootContainer = LootContainer.GetLootContainer(_name, false);
		if (lootContainer != null)
		{
			GameRandom gameRandom = new GameRandom();
			gameRandom.SetSeed(0);
			int num = 0;
			List<ItemStack> list = new List<ItemStack>();
			for (int i = 0; i < _count; i++)
			{
				int num2 = this.CountItems(list);
				int num3 = 999999;
				LootContainer.SpawnLootItemsFromList(gameRandom, lootContainer.itemsToSpawn, 1, _abundance, list, ref num3, (float)_stage, 0f, lootContainer.lootQualityTemplate, null, FastTags<TagGroup.Global>.none, false, false, true, lootContainer.ignoreLootAbundance, null, false);
				if (num2 == this.CountItems(list))
				{
					num++;
				}
			}
			list.Sort(delegate(ItemStack a, ItemStack b)
			{
				int num4 = b.count.CompareTo(a.count);
				if (num4 == 0)
				{
					num4 = a.itemValue.ItemClass.Name.CompareTo(b.itemValue.ItemClass.Name);
					if (num4 == 0)
					{
						num4 = a.itemValue.Quality.CompareTo(b.itemValue.Quality);
					}
				}
				return num4;
			});
			for (int j = list.Count - 1; j > 0; j--)
			{
				ItemStack itemStack = list[j];
				ItemStack itemStack2 = list[j - 1];
				if (itemStack.itemValue.type == itemStack2.itemValue.type && itemStack.itemValue.Quality == itemStack2.itemValue.Quality)
				{
					itemStack2.count += itemStack.count;
					list.RemoveAt(j);
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				ItemStack itemStack3 = list[k];
				this.Print("#{0} {1}, q{2}, count {3}", new object[]
				{
					k,
					itemStack3.itemValue.ItemClass.GetItemName(),
					itemStack3.itemValue.Quality,
					itemStack3.count
				});
			}
			this.Print("Loot Container {0}, unique items {1}, empties {2}", new object[]
			{
				lootContainer.Name,
				list.Count,
				num
			});
			return;
		}
		this.Print("Unknown container " + _name, Array.Empty<object>());
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0006DF1C File Offset: 0x0006C11C
	[PublicizedFrom(EAccessModifier.Private)]
	public int CountItems(List<ItemStack> _list)
	{
		int num = 0;
		for (int i = 0; i < _list.Count; i++)
		{
			ItemStack itemStack = _list[i];
			num += itemStack.count;
		}
		return num;
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0006DF50 File Offset: 0x0006C150
	[PublicizedFrom(EAccessModifier.Private)]
	public void Print(string _s, params object[] _values)
	{
		string line = string.Format(_s, _values);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(line);
	}
}
