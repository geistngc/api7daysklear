using System;

// Token: 0x0200097B RID: 2427
public abstract class BaseObjectiveModifier
{
	// Token: 0x1700077A RID: 1914
	// (get) Token: 0x060046F6 RID: 18166 RVA: 0x001BFCD7 File Offset: 0x001BDED7
	// (set) Token: 0x060046F7 RID: 18167 RVA: 0x001BFCDF File Offset: 0x001BDEDF
	public BaseObjective OwnerObjective { get; set; }

	// Token: 0x060046F8 RID: 18168 RVA: 0x0000640C File Offset: 0x0000460C
	public BaseObjectiveModifier()
	{
	}

	// Token: 0x060046F9 RID: 18169 RVA: 0x001BFCE8 File Offset: 0x001BDEE8
	public void HandleAddHooks()
	{
		this.AddHooks();
	}

	// Token: 0x060046FA RID: 18170 RVA: 0x001BFCF0 File Offset: 0x001BDEF0
	public void HandleRemoveHooks()
	{
		this.RemoveHooks();
	}

	// Token: 0x060046FB RID: 18171 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void AddHooks()
	{
	}

	// Token: 0x060046FC RID: 18172 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void RemoveHooks()
	{
	}

	// Token: 0x060046FD RID: 18173 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual BaseObjectiveModifier Clone()
	{
		return null;
	}

	// Token: 0x060046FE RID: 18174 RVA: 0x001BFCF8 File Offset: 0x001BDEF8
	public virtual void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		this.OwnerObjective.OwnerQuestClass.HandleVariablesForProperties(properties);
	}

	// Token: 0x040038FF RID: 14591
	public DynamicProperties Properties;
}
