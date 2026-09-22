using System;
using System.Collections.Generic;
using UniLinq;

// Token: 0x02000A44 RID: 2628
public class Archetype
{
	// Token: 0x1700087E RID: 2174
	// (get) Token: 0x06004F77 RID: 20343 RVA: 0x001E2AE7 File Offset: 0x001E0CE7
	// (set) Token: 0x06004F78 RID: 20344 RVA: 0x001E2AFC File Offset: 0x001E0CFC
	public string Sex
	{
		get
		{
			if (!this.IsMale)
			{
				return "Female";
			}
			return "Male";
		}
		set
		{
			this.IsMale = (value.ToLower() == "male");
		}
	}

	// Token: 0x1700087F RID: 2175
	// (get) Token: 0x06004F79 RID: 20345 RVA: 0x001E2B14 File Offset: 0x001E0D14
	public bool ShowInList
	{
		get
		{
			return this.Name != "BaseMale" && this.Name != "BaseFemale";
		}
	}

	// Token: 0x06004F7A RID: 20346 RVA: 0x001E2B3C File Offset: 0x001E0D3C
	public Archetype(string _name, bool _isMale, bool _canCustomize)
	{
		this.Name = _name;
		this.IsMale = _isMale;
		this.CanCustomize = _canCustomize;
	}

	// Token: 0x06004F7B RID: 20347 RVA: 0x001E2BA6 File Offset: 0x001E0DA6
	public static void InitializeStatics()
	{
		Archetype.s_Archetypes = new CaseInsensitiveStringDictionary<Archetype>();
	}

	// Token: 0x06004F7C RID: 20348 RVA: 0x001E2BB4 File Offset: 0x001E0DB4
	public static void SetArchetype(Archetype archetype)
	{
		if (Archetype.s_Archetypes == null)
		{
			return;
		}
		if (Archetype.s_Archetypes.ContainsKey(archetype.Name))
		{
			Archetype.s_Archetypes[archetype.Name] = archetype;
			return;
		}
		Archetype.s_Archetypes[archetype.Name] = archetype;
		if (!archetype.CanCustomize)
		{
			ProfileSDF.SaveArchetype(archetype.Name, archetype.IsMale);
		}
	}

	// Token: 0x06004F7D RID: 20349 RVA: 0x001E2C17 File Offset: 0x001E0E17
	public static Archetype GetArchetype(string name)
	{
		if (Archetype.s_Archetypes == null)
		{
			return null;
		}
		if (!Archetype.s_Archetypes.ContainsKey(name))
		{
			return null;
		}
		return Archetype.s_Archetypes[name];
	}

	// Token: 0x06004F7E RID: 20350 RVA: 0x001E2C3C File Offset: 0x001E0E3C
	public void AddEquipmentSlot(SDCSUtils.SlotData slotData)
	{
		if (this.Equipment == null)
		{
			this.Equipment = new List<SDCSUtils.SlotData>();
		}
		this.Equipment.Add(slotData);
	}

	// Token: 0x06004F7F RID: 20351 RVA: 0x001E2C60 File Offset: 0x001E0E60
	public Archetype Clone()
	{
		return new Archetype(this.Name, this.IsMale, this.CanCustomize)
		{
			CanCustomize = this.CanCustomize,
			IsMale = this.IsMale,
			Race = this.Race,
			Variant = this.Variant,
			Hair = this.Hair,
			HairColor = this.HairColor,
			MustacheName = this.MustacheName,
			ChopsName = this.ChopsName,
			BeardName = this.BeardName,
			EyeColorName = this.EyeColorName
		};
	}

	// Token: 0x06004F80 RID: 20352 RVA: 0x001E2CFC File Offset: 0x001E0EFC
	public static void SaveArchetypesToFile()
	{
		SDCSArchetypesFromXml.Save("archetypes", Archetype.s_Archetypes.Values.ToList<Archetype>());
	}

	// Token: 0x04003D1C RID: 15644
	public static Dictionary<string, Archetype> s_Archetypes;

	// Token: 0x04003D1D RID: 15645
	public string Name;

	// Token: 0x04003D1E RID: 15646
	public string Race;

	// Token: 0x04003D1F RID: 15647
	public int Variant;

	// Token: 0x04003D20 RID: 15648
	public string Hair = "";

	// Token: 0x04003D21 RID: 15649
	public string HairColor = "";

	// Token: 0x04003D22 RID: 15650
	public string MustacheName = "";

	// Token: 0x04003D23 RID: 15651
	public string ChopsName = "";

	// Token: 0x04003D24 RID: 15652
	public string BeardName = "";

	// Token: 0x04003D25 RID: 15653
	public string EyeColorName = "Blue01";

	// Token: 0x04003D26 RID: 15654
	public bool IsMale;

	// Token: 0x04003D27 RID: 15655
	public bool CanCustomize;

	// Token: 0x04003D28 RID: 15656
	public List<SDCSUtils.SlotData> Equipment;
}
