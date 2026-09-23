using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;

// Token: 0x0200119A RID: 4506
public class EntitlementManager
{
	// Token: 0x17001140 RID: 4416
	// (get) Token: 0x06009006 RID: 36870 RVA: 0x00362F1C File Offset: 0x0036111C
	public static EntitlementManager Instance
	{
		get
		{
			if (EntitlementManager._instance == null)
			{
				EntitlementManager._instance = new EntitlementManager();
				if (PlatformManager.MultiPlatform.EntitlementValidators != null)
				{
					PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _)
					{
						object obj = EntitlementManager.lockObj;
						lock (obj)
						{
							EntitlementManager._instance.entitlementValidators = PlatformManager.MultiPlatform.EntitlementValidators;
						}
					};
				}
			}
			return EntitlementManager._instance;
		}
	}

	// Token: 0x06009007 RID: 36871 RVA: 0x00362F7C File Offset: 0x0036117C
	public bool HasEntitlement(object _addressableKey)
	{
		EntitlementSetEnum entitlementSet = this.GetEntitlementSet(_addressableKey);
		return this.HasEntitlement(entitlementSet);
	}

	// Token: 0x06009008 RID: 36872 RVA: 0x00362F98 File Offset: 0x00361198
	public bool HasEntitlement(EntitlementSetEnum _set)
	{
		ValueTuple<bool, bool> valueTuple = this.CheckOverride(_set);
		bool item = valueTuple.Item1;
		bool item2 = valueTuple.Item2;
		if (item)
		{
			return item2;
		}
		if (_set == EntitlementSetEnum.None)
		{
			return true;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HasEntitlement(_set))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06009009 RID: 36873 RVA: 0x00363038 File Offset: 0x00361238
	public bool IsAvailableOnPlatform(object _addressableKey)
	{
		EntitlementSetEnum setForAsset = this.GetSetForAsset(_addressableKey);
		return this.IsAvailableOnPlatform(setForAsset);
	}

	// Token: 0x0600900A RID: 36874 RVA: 0x00363054 File Offset: 0x00361254
	public bool IsAvailableOnPlatform(EntitlementSetEnum _set)
	{
		ValueTuple<bool, bool> valueTuple = this.CheckOverride(_set);
		bool item = valueTuple.Item1;
		bool item2 = valueTuple.Item2;
		if (item)
		{
			return item2;
		}
		if (_set == EntitlementSetEnum.None)
		{
			return true;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsAvailableOnPlatform(_set))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600900B RID: 36875 RVA: 0x003630F4 File Offset: 0x003612F4
	public unsafe EntitlementSetEnum GetSetForAsset(object addressableKey)
	{
		string text = addressableKey as string;
		if (text == null)
		{
			EntitlementSetEnum result;
			if (EntitlementAddressablesMaps.AddressablesKeyMap.TryGetValue(addressableKey, out result))
			{
				return result;
			}
			return EntitlementSetEnum.None;
		}
		else
		{
			if (string.IsNullOrEmpty(text))
			{
				return EntitlementSetEnum.None;
			}
			StringSpan key = text.ToLowerInvariant().Trim();
			if (key.Length >= 2 && *key[0] == 64 && *key[1] == 58)
			{
				key = key.Slice(2);
			}
			while (key.Length > 0)
			{
				EntitlementSetEnum result2;
				if (EntitlementAddressablesMaps.AddressablesStringMap.TryGetValue(key, out result2))
				{
					return result2;
				}
				int num = key.LastIndexOf('/');
				if (num == -1)
				{
					break;
				}
				key = key.Slice(0, num);
			}
			return EntitlementSetEnum.None;
		}
	}

	// Token: 0x0600900C RID: 36876 RVA: 0x003631A4 File Offset: 0x003613A4
	public bool IsEntitlementPurchasable(object _addressableKey)
	{
		EntitlementSetEnum entitlementSet = this.GetEntitlementSet(_addressableKey);
		return this.IsEntitlementPurchasable(entitlementSet);
	}

	// Token: 0x0600900D RID: 36877 RVA: 0x003631C0 File Offset: 0x003613C0
	public bool IsEntitlementPurchasable(EntitlementSetEnum _set)
	{
		if (_set == EntitlementSetEnum.None)
		{
			return true;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsEntitlementPurchasable(_set))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600900E RID: 36878 RVA: 0x00363240 File Offset: 0x00361440
	public unsafe EntitlementSetEnum GetEntitlementSet(object addressableKey)
	{
		string text = addressableKey as string;
		if (text == null)
		{
			EntitlementSetEnum result;
			if (EntitlementAddressablesMaps.AddressablesKeyMap.TryGetValue(addressableKey, out result))
			{
				return result;
			}
			return EntitlementSetEnum.None;
		}
		else
		{
			if (string.IsNullOrEmpty(text))
			{
				return EntitlementSetEnum.None;
			}
			StringSpan key = text.ToLowerInvariant().Trim();
			if (key.Length >= 2 && *key[0] == 64 && *key[1] == 58)
			{
				key = key.Slice(2);
			}
			while (key.Length > 0)
			{
				EntitlementSetEnum result2;
				if (EntitlementAddressablesMaps.AddressablesStringMap.TryGetValue(key, out result2))
				{
					return result2;
				}
				int num = key.LastIndexOf('/');
				if (num == -1)
				{
					break;
				}
				key = key.Slice(0, num);
			}
			return EntitlementSetEnum.None;
		}
	}

	// Token: 0x0600900F RID: 36879 RVA: 0x003632F0 File Offset: 0x003614F0
	public void OpenStore(EntitlementSetEnum _set, Action<EntitlementSetEnum> _onPurchased)
	{
		if (_set == EntitlementSetEnum.None)
		{
			return;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.OpenStore(_set, _onPurchased))
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06009010 RID: 36880 RVA: 0x0036336C File Offset: 0x0036156C
	public string GetEntitlementSetId(EntitlementSetEnum entitlementSet)
	{
		if (entitlementSet == EntitlementSetEnum.None)
		{
			return null;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			foreach (IEntitlementValidator entitlementValidator in this.entitlementValidators)
			{
				string entitlementSetId = entitlementValidator.GetEntitlementSetId(entitlementSet);
				if (entitlementSetId != null)
				{
					return entitlementSetId;
				}
			}
		}
		return null;
	}

	// Token: 0x06009011 RID: 36881 RVA: 0x003633F0 File Offset: 0x003615F0
	public DateTime? GetAcquiredDate(EntitlementSetEnum entitlementSet)
	{
		if (entitlementSet == EntitlementSetEnum.None)
		{
			DateTime? result = null;
			return result;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			foreach (IEntitlementValidator entitlementValidator in this.entitlementValidators)
			{
				DateTime? result = entitlementValidator.GetAcquiredDate(entitlementSet);
				if (result != null)
				{
					DateTime valueOrDefault = result.GetValueOrDefault();
					return new DateTime?(valueOrDefault);
				}
			}
		}
		return null;
	}

	// Token: 0x06009012 RID: 36882 RVA: 0x00363498 File Offset: 0x00361698
	[return: TupleElementNames(new string[]
	{
		"hasOverride",
		"overrideValue"
	})]
	public ValueTuple<bool, bool> CheckOverride(EntitlementSetEnum _set)
	{
		return new ValueTuple<bool, bool>(false, false);
	}

	// Token: 0x04006A9F RID: 27295
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object lockObj = new object();

	// Token: 0x04006AA0 RID: 27296
	[PublicizedFrom(EAccessModifier.Private)]
	public static EntitlementManager _instance;

	// Token: 0x04006AA1 RID: 27297
	[PublicizedFrom(EAccessModifier.Private)]
	public IList<IEntitlementValidator> entitlementValidators = new List<IEntitlementValidator>();
}
