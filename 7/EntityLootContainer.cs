using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004BA RID: 1210
[Preserve]
public class EntityLootContainer : EntityItem
{
	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06002672 RID: 9842 RVA: 0x000EC136 File Offset: 0x000EA336
	public override string LocalizedEntityName
	{
		get
		{
			if (!string.IsNullOrEmpty(this.OverrideName))
			{
				return Localization.Get(this.OverrideName, false, null);
			}
			return base.LocalizedEntityName;
		}
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000EC15C File Offset: 0x000EA35C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		foreach (Collider collider in base.transform.GetComponentsInChildren<Collider>())
		{
			collider.gameObject.tag = "E_BP_Body";
			collider.enabled = true;
			collider.gameObject.layer = 13;
			collider.gameObject.AddMissingComponent<RootTransformRefEntity>().RootTransform = base.transform;
		}
		this.SetDead();
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000EC1CB File Offset: 0x000EA3CB
	public void SetContent(ItemStack[] _inventory)
	{
		this.SetContent(_inventory, 0);
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000EC1D8 File Offset: 0x000EA3D8
	public void SetContent(ItemStack[] _inventory, int _slotCountOverride)
	{
		int num = 0;
		if (_slotCountOverride > 0)
		{
			num = _slotCountOverride;
		}
		else
		{
			LootContainer lootContainer = LootContainer.GetLootContainer(this.GetLootList(), true);
			if (lootContainer != null)
			{
				Vector2i size = lootContainer.size;
				num = size.x * size.y;
			}
			num = Math.Max(num, (_inventory != null) ? _inventory.Length : 0);
		}
		if (num <= 0)
		{
			return;
		}
		ItemStack[] array = ItemStack.CreateArray(num);
		int num2 = 0;
		while (_inventory != null && num2 < _inventory.Length && num2 < array.Length)
		{
			array[num2] = _inventory[num2].Clone();
			num2++;
		}
		this.bag.SetSlots(array);
		this.bag.Touched = true;
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000EC274 File Offset: 0x000EA474
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		if (entityClass.Properties.Values.ContainsKey(EntityClass.PropTimeStayAfterDeath))
		{
			this.timeStayAfterDeath = (int)(StringParsers.ParseFloat(entityClass.Properties.Values[EntityClass.PropTimeStayAfterDeath], 0, -1, NumberStyles.Any) * 20f);
			return;
		}
		this.timeStayAfterDeath = 100;
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000EC2E8 File Offset: 0x000EA4E8
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		try
		{
			if (!(SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? LockManager.Instance.IsLockedServer(this, 0) : LockManager.Instance.IsLockedByLocalPlayer(this, 0)))
			{
				if (this.bag.Touched && this.bag.IsEmpty())
				{
					this.removeBackpack();
				}
				else if (this.deathUpdateTime >= this.timeStayAfterDeath - 1)
				{
					this.removeBackpack();
				}
				else
				{
					this.deathUpdateTime++;
				}
			}
		}
		finally
		{
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000DA858 File Offset: 0x000D8A58
	public override bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_commandName, "search"))
		{
			return this.bag != null;
		}
		return base.AllowActivationCommand(_commandName, _playerFocusing);
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000EC380 File Offset: 0x000EA580
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeBackpack()
	{
		this.deathUpdateTime = this.timeStayAfterDeath;
		this.bRemoved = true;
		this.MarkToUnload();
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x000EC39B File Offset: 0x000EA59B
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		if (_strength >= 99999)
		{
			this.removeBackpack();
		}
		return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000EC3B6 File Offset: 0x000EA5B6
	public override bool IsMarkedForUnload()
	{
		return base.IsMarkedForUnload() && this.bRemoved;
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createMesh()
	{
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000EC3C8 File Offset: 0x000EA5C8
	public override string GetLootList()
	{
		if (!(this.OverrideLootList != ""))
		{
			return this.lootList;
		}
		return this.OverrideLootList;
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x000EC3E9 File Offset: 0x000EA5E9
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write(this.OverrideLootList);
		_bw.Write(this.OverrideName);
		this.bag.Write(_bw);
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000EC418 File Offset: 0x000EA618
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		if (_version < 35)
		{
			int num = (int)_br.ReadUInt16();
			ItemStack[] array = ItemStack.CreateArray(num);
			for (int i = 0; i < num; i++)
			{
				array[i].Read(_br);
			}
			num = (int)_br.ReadUInt16();
			for (int j = 0; j < num; j++)
			{
				new ItemStack().Read(_br);
			}
			if (_version >= 30)
			{
				this.OverrideLootList = _br.ReadString();
				this.OverrideName = _br.ReadString();
			}
			LootContainer lootContainer = LootContainer.GetLootContainer(this.GetLootList(), true);
			int num2 = array.Length;
			if (lootContainer != null)
			{
				num2 = Math.Max(num2, lootContainer.size.x * lootContainer.size.y);
			}
			if (num2 > array.Length)
			{
				ItemStack[] array2 = ItemStack.CreateArray(num2);
				for (int k = 0; k < array.Length; k++)
				{
					array2[k] = array[k];
				}
				array = array2;
			}
			this.bag.SetSlots(array);
			return;
		}
		if (_version >= 30)
		{
			this.OverrideLootList = _br.ReadString();
			this.OverrideName = _br.ReadString();
		}
		if (_version >= 35)
		{
			this.bag.ReadInto(_br);
		}
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x000EC538 File Offset: 0x000EA738
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		if (EntityClass.list[this.entityClass].NavObject != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(EntityClass.list[this.entityClass].NavObject, this, "", false);
		}
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x000EC592 File Offset: 0x000EA792
	public override string ToString()
	{
		return string.Format("[type={0}, name={1}]", base.GetType().Name, (this.itemClass != null) ? this.itemClass.GetItemName() : "?");
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x000DAC10 File Offset: 0x000D8E10
	public override void OnUnlockedServer(int _unlockingPlayerId, ushort _channel)
	{
		if (this.bag.IsEmpty())
		{
			this.KillLootContainer();
		}
	}

	// Token: 0x04001C91 RID: 7313
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int deathUpdateTime;

	// Token: 0x04001C92 RID: 7314
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int timeStayAfterDeath;

	// Token: 0x04001C93 RID: 7315
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bRemoved;

	// Token: 0x04001C94 RID: 7316
	public string OverrideLootList = "";

	// Token: 0x04001C95 RID: 7317
	public string OverrideName = "";
}
