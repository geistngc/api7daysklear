using System;

// Token: 0x0200145C RID: 5212
public class DictionaryChangedEventArgs<TKey, TValue> : EventArgs
{
	// Token: 0x1700132D RID: 4909
	// (get) Token: 0x0600A376 RID: 41846 RVA: 0x003D3F93 File Offset: 0x003D2193
	public TKey Key { get; }

	// Token: 0x1700132E RID: 4910
	// (get) Token: 0x0600A377 RID: 41847 RVA: 0x003D3F9B File Offset: 0x003D219B
	public TValue Value { get; }

	// Token: 0x1700132F RID: 4911
	// (get) Token: 0x0600A378 RID: 41848 RVA: 0x003D3FA3 File Offset: 0x003D21A3
	public ObservableDictionary<TKey, TValue>.EChangeType Action { get; }

	// Token: 0x0600A379 RID: 41849 RVA: 0x003D3FAB File Offset: 0x003D21AB
	public DictionaryChangedEventArgs(TKey key, TValue value, ObservableDictionary<TKey, TValue>.EChangeType action)
	{
		this.Key = key;
		this.Value = value;
		this.Action = action;
	}
}
