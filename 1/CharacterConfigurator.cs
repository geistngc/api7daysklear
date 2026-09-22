using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Platform;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class CharacterConfigurator : MonoBehaviour
{
	// Token: 0x0600002E RID: 46 RVA: 0x00005356 File Offset: 0x00003556
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		LoadManager.Init();
		PlatformManager.Init();
		this.PrepareHairColorArray();
		this.PrepareEyeColorArray();
		this.PopulateGearLookups();
		this.CreateCharacter();
	}

	// Token: 0x0600002F RID: 47 RVA: 0x0000537C File Offset: 0x0000357C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrepareHairColorArray()
	{
		string[] files = Directory.GetFiles("Assets/AssetBundles/Player/Common/HairColorSwatches", "*.asset");
		this.hairColors = new string[files.Length];
		for (int i = 0; i < files.Length; i++)
		{
			this.hairColors[i] = Path.GetFileNameWithoutExtension(files[i]);
		}
	}

	// Token: 0x06000030 RID: 48 RVA: 0x000053C8 File Offset: 0x000035C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrepareEyeColorArray()
	{
		string[] files = Directory.GetFiles("Assets/AssetBundles/Player/Common/Eyes/Materials", "*.mat");
		this.eyeColors = new string[files.Length];
		for (int i = 0; i < files.Length; i++)
		{
			this.eyeColors[i] = Path.GetFileNameWithoutExtension(files[i]);
		}
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00005411 File Offset: 0x00003611
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetItemsXmlPath()
	{
		return GameIO.GetGameDir("Data/Config") + "/items.xml";
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00005427 File Offset: 0x00003627
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnsureGearCatalogLoaded()
	{
		if (this.gearCatalog == null)
		{
			this.PopulateGearLookups();
		}
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00005438 File Offset: 0x00003638
	[PublicizedFrom(EAccessModifier.Private)]
	public void PopulateGearLookups()
	{
		this.allGearKeys.Clear();
		this.gearCatalog = new CharacterConfigurator.SDCSGearXmlCatalog();
		string itemsXmlPath = this.GetItemsXmlPath();
		if (!File.Exists(itemsXmlPath))
		{
			Debug.LogWarning("CharacterConfigurator could not find items.xml at: " + itemsXmlPath);
			return;
		}
		try
		{
			string xmlText = File.ReadAllText(itemsXmlPath);
			this.gearCatalog = CharacterConfigurator.SDCSGearXmlCatalog.BuildFromXml(xmlText);
			this.allGearKeys = this.gearCatalog.GetOrderedKeys();
		}
		catch (Exception ex)
		{
			string str = "CharacterConfigurator failed to read SDCS gear data from items.xml: ";
			Exception ex2 = ex;
			Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
			this.gearCatalog = new CharacterConfigurator.SDCSGearXmlCatalog();
			this.allGearKeys.Clear();
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000054E8 File Offset: 0x000036E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateCharacter()
	{
		if (this.characterInstance != null)
		{
			UnityEngine.Object.DestroyImmediate(this.characterInstance);
		}
		this.characterInstance = UnityEngine.Object.Instantiate<GameObject>(this.baseRig, base.transform);
		this.boneCatalog = new SDCSUtils.TransformCatalog(this.characterInstance.transform);
		this.UpdateCharacter();
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00005544 File Offset: 0x00003744
	public void UpdateCharacter()
	{
		if (this.characterInstance == null)
		{
			return;
		}
		this.EnsureGearCatalogLoaded();
		CharacterConstructUtils.CreateViz(this.CreateCurrentArchetype(), ref this.characterInstance, ref this.boneCatalog);
		this.characterInstance.name = "Character";
		this.characterInstance.transform.localPosition = Vector3.zero;
		this.characterInstance.transform.localRotation = Quaternion.Euler(0f, this.rotationValue, 0f);
	}

	// Token: 0x06000036 RID: 54 RVA: 0x000055C8 File Offset: 0x000037C8
	[PublicizedFrom(EAccessModifier.Private)]
	public Archetype CreateCurrentArchetype()
	{
		this.EnsureGearCatalogLoaded();
		Archetype archetype = new Archetype("", this.selectedSexIndex == 0, true);
		archetype.Sex = this.sexes[this.selectedSexIndex];
		archetype.Race = this.races[this.selectedRaceIndex];
		archetype.Variant = this.variants[this.selectedVariantIndex];
		archetype.EyeColorName = this.eyeColors[this.selectedEyeColorIndex];
		if (this.selectedHairIndex >= 0 && this.selectedHairIndex < this.hairs.Length)
		{
			archetype.Hair = this.hairs[this.selectedHairIndex];
			archetype.HairColor = this.hairColors[this.selectedHairColorIndex];
		}
		else
		{
			archetype.Hair = "";
		}
		if (this.selectedSexIndex == 0 && this.selectedFacialHairIndex >= 0 && this.selectedFacialHairIndex < this.facialHairIndexes.Length)
		{
			int num = this.facialHairIndexes[this.selectedFacialHairIndex];
			archetype.MustacheName = num.ToString();
			archetype.ChopsName = num.ToString();
			archetype.BeardName = num.ToString();
		}
		if (this.IsValidGearIndex(this.selectedFeetGearIndex) || this.IsValidGearIndex(this.selectedHandsGearIndex) || this.IsValidGearIndex(this.selectedBodyGearIndex) || this.IsValidGearIndex(this.selectedHeadGearIndex))
		{
			archetype.Equipment = new List<SDCSUtils.SlotData>();
			string[] array = new string[]
			{
				this.IsValidGearIndex(this.selectedHeadGearIndex) ? this.allGearKeys[this.selectedHeadGearIndex] : string.Empty,
				this.IsValidGearIndex(this.selectedBodyGearIndex) ? this.allGearKeys[this.selectedBodyGearIndex] : string.Empty,
				this.IsValidGearIndex(this.selectedHandsGearIndex) ? this.allGearKeys[this.selectedHandsGearIndex] : string.Empty,
				this.IsValidGearIndex(this.selectedFeetGearIndex) ? this.allGearKeys[this.selectedFeetGearIndex] : string.Empty
			};
			for (int i = 0; i < CharacterConfigurator.baseParts.Length; i++)
			{
				string text = array[i];
				string partName = CharacterConfigurator.baseParts[i];
				CharacterConfigurator.SDCSGearXmlCatalog.Entry entry;
				if (!string.IsNullOrEmpty(text) && this.gearCatalog.TryGetPart(text, partName, out entry))
				{
					SDCSUtils.SlotData slotData = new SDCSUtils.SlotData();
					slotData.PartName = entry.PartName;
					slotData.PrefabName = entry.PrefabName;
					slotData.BaseToTurnOff = entry.BaseToTurnOff;
					slotData.HairMaskType = SDCSUtils.SlotData.HairMaskTypes.Full;
					slotData.FacialHairMaskType = SDCSUtils.SlotData.HairMaskTypes.Full;
					if (slotData.PartName == "head")
					{
						slotData.HairMaskType = this.ParseHairMaskType(entry.HairMaskType);
						slotData.FacialHairMaskType = this.ParseHairMaskType(entry.FacialHairMaskType);
						if (slotData.HairMaskType == SDCSUtils.SlotData.HairMaskTypes.None)
						{
							archetype.Hair = "";
						}
						if (slotData.FacialHairMaskType == SDCSUtils.SlotData.HairMaskTypes.None)
						{
							archetype.MustacheName = "";
							archetype.ChopsName = "";
							archetype.BeardName = "";
						}
					}
					if (slotData.BaseToTurnOff == "head")
					{
						archetype.Hair = "";
						archetype.MustacheName = "";
						archetype.ChopsName = "";
						archetype.BeardName = "";
					}
					archetype.Equipment.Add(slotData);
				}
			}
		}
		return archetype;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00005923 File Offset: 0x00003B23
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsValidGearIndex(int gearIndex)
	{
		return gearIndex >= 0 && gearIndex < this.allGearKeys.Count;
	}

	// Token: 0x06000038 RID: 56 RVA: 0x0000593C File Offset: 0x00003B3C
	[PublicizedFrom(EAccessModifier.Private)]
	public SDCSUtils.SlotData.HairMaskTypes ParseHairMaskType(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return SDCSUtils.SlotData.HairMaskTypes.Full;
		}
		SDCSUtils.SlotData.HairMaskTypes result;
		if (Enum.TryParse<SDCSUtils.SlotData.HairMaskTypes>(value, true, out result))
		{
			return result;
		}
		Debug.LogWarning("Unknown SDCS HairMaskType '" + value + "'. Falling back to None.");
		return SDCSUtils.SlotData.HairMaskTypes.None;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00005976 File Offset: 0x00003B76
	public string[] GetSexTypes()
	{
		return this.sexes;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x0000597E File Offset: 0x00003B7E
	public string[] GetRaceTypes()
	{
		return this.races;
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00005988 File Offset: 0x00003B88
	public string[] GetVariantTypes()
	{
		string[] array = new string[this.variants.Length];
		for (int i = 0; i < this.variants.Length; i++)
		{
			array[i] = this.variants[i].ToString();
		}
		return array;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000059CC File Offset: 0x00003BCC
	public string[] GetHairTypes()
	{
		string[] array = new string[this.hairs.Length + 1];
		array[0] = "None";
		for (int i = 0; i < this.hairs.Length; i++)
		{
			array[i + 1] = this.hairs[i];
		}
		return array;
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00005A12 File Offset: 0x00003C12
	public string[] GetHairColors()
	{
		this.PrepareHairColorArray();
		return this.hairColors;
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00005A20 File Offset: 0x00003C20
	public string[] GetEyeColors()
	{
		this.PrepareEyeColorArray();
		return this.eyeColors;
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00005A30 File Offset: 0x00003C30
	public string[] GetGearTypes()
	{
		this.EnsureGearCatalogLoaded();
		string[] array = new string[this.allGearKeys.Count + 1];
		array[0] = "None";
		for (int i = 0; i < this.allGearKeys.Count; i++)
		{
			array[i + 1] = this.allGearKeys[i];
		}
		return array;
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00005A88 File Offset: 0x00003C88
	public string[] GetFacialHairTypes()
	{
		string[] array = new string[this.facialHairIndexes.Length + 1];
		array[0] = "None";
		for (int i = 0; i < this.facialHairIndexes.Length; i++)
		{
			array[i + 1] = "Style " + this.facialHairIndexes[i].ToString();
		}
		return array;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00005AE1 File Offset: 0x00003CE1
	public void SetSex(int sexIndex)
	{
		if (sexIndex >= 0 && sexIndex < this.sexes.Length)
		{
			this.selectedSexIndex = sexIndex;
			if (sexIndex == 1)
			{
				this.selectedFacialHairIndex = -1;
			}
			this.UpdateCharacter();
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00005B0A File Offset: 0x00003D0A
	public void SetRace(int raceIndex)
	{
		if (raceIndex >= 0 && raceIndex < this.races.Length)
		{
			this.selectedRaceIndex = raceIndex;
			this.UpdateCharacter();
		}
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00005B28 File Offset: 0x00003D28
	public void SetVariant(int variantIndex)
	{
		if (variantIndex >= 0 && variantIndex < this.variants.Length)
		{
			this.selectedVariantIndex = variantIndex;
			this.UpdateCharacter();
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00005B46 File Offset: 0x00003D46
	public void SetHair(int hairIndex)
	{
		this.selectedHairIndex = hairIndex - 1;
		this.UpdateCharacter();
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00005B57 File Offset: 0x00003D57
	public void SetHairColor(int colorIndex)
	{
		if (colorIndex >= 0 && colorIndex < this.hairColors.Length)
		{
			this.selectedHairColorIndex = colorIndex;
			this.UpdateCharacter();
		}
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00005B75 File Offset: 0x00003D75
	public void SetEyeColor(int colorIndex)
	{
		if (colorIndex >= 0 && colorIndex < this.eyeColors.Length)
		{
			this.selectedEyeColorIndex = colorIndex;
			this.UpdateCharacter();
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00005B94 File Offset: 0x00003D94
	public void SetGear(int gearIndex)
	{
		this.selectedGearIndex = gearIndex - 1;
		this.selectedFeetGearIndex = (this.selectedHandsGearIndex = (this.selectedBodyGearIndex = (this.selectedHeadGearIndex = this.selectedGearIndex)));
		this.UpdateCharacter();
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00005BD7 File Offset: 0x00003DD7
	public void SetFeetGear(int gearIndex)
	{
		this.selectedFeetGearIndex = gearIndex - 1;
		this.UpdateCharacter();
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00005BE8 File Offset: 0x00003DE8
	public void SetHandsGear(int gearIndex)
	{
		this.selectedHandsGearIndex = gearIndex - 1;
		this.UpdateCharacter();
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00005BF9 File Offset: 0x00003DF9
	public void SetBodyGear(int gearIndex)
	{
		this.selectedBodyGearIndex = gearIndex - 1;
		this.UpdateCharacter();
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00005C0A File Offset: 0x00003E0A
	public void SetHeadGear(int gearIndex)
	{
		this.selectedHeadGearIndex = gearIndex - 1;
		this.UpdateCharacter();
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00005C1B File Offset: 0x00003E1B
	public void SetFacialHair(int facialHairIndex)
	{
		if (this.selectedSexIndex == 0)
		{
			this.selectedFacialHairIndex = facialHairIndex - 1;
			this.UpdateCharacter();
		}
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00005C34 File Offset: 0x00003E34
	public void ToggleJiggle()
	{
		this.isJiggling = !this.isJiggling;
		if (this.isJiggling)
		{
			base.StartCoroutine(this.JiggleCharacter());
		}
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00005C5A File Offset: 0x00003E5A
	public IEnumerator JiggleCharacter()
	{
		float angle = 20f;
		float speed = 6f;
		while (this.isJiggling)
		{
			float num = angle * Mathf.SmoothStep(-1f, 1f, Mathf.Sin(Time.time * speed) * 0.5f + 0.5f);
			this.characterInstance.transform.localRotation = Quaternion.Euler(0f, this.rotationValue + num, 0f);
			yield return null;
		}
		this.characterInstance.transform.localRotation = Quaternion.Euler(0f, this.rotationValue, 0f);
		yield break;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00005C69 File Offset: 0x00003E69
	public bool IsJiggleEnabled()
	{
		return this.isJiggling;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00005C71 File Offset: 0x00003E71
	public void RotateCharacter(float value)
	{
		this.rotationValue = -value;
		this.characterInstance.transform.localRotation = Quaternion.Euler(0f, this.rotationValue, 0f);
	}

	// Token: 0x0400007C RID: 124
	public GameObject baseRig;

	// Token: 0x0400007D RID: 125
	[Header("Character Configuration")]
	public int selectedSexIndex;

	// Token: 0x0400007E RID: 126
	public int selectedRaceIndex;

	// Token: 0x0400007F RID: 127
	public int selectedVariantIndex;

	// Token: 0x04000080 RID: 128
	public int selectedHairIndex;

	// Token: 0x04000081 RID: 129
	public int selectedGearIndex = -1;

	// Token: 0x04000082 RID: 130
	public int selectedHeadGearIndex = -1;

	// Token: 0x04000083 RID: 131
	public int selectedBodyGearIndex = -1;

	// Token: 0x04000084 RID: 132
	public int selectedHandsGearIndex = -1;

	// Token: 0x04000085 RID: 133
	public int selectedFeetGearIndex = -1;

	// Token: 0x04000086 RID: 134
	public int selectedFacialHairIndex = -1;

	// Token: 0x04000087 RID: 135
	public int selectedHairColorIndex = 2;

	// Token: 0x04000088 RID: 136
	public int selectedEyeColorIndex;

	// Token: 0x04000089 RID: 137
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject characterInstance;

	// Token: 0x0400008A RID: 138
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SDCSUtils.TransformCatalog boneCatalog;

	// Token: 0x0400008B RID: 139
	public bool isJiggling;

	// Token: 0x0400008C RID: 140
	public float rotationValue = 180f;

	// Token: 0x0400008D RID: 141
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] sexes = new string[]
	{
		"Male",
		"Female"
	};

	// Token: 0x0400008E RID: 142
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] races = new string[]
	{
		"White",
		"Black",
		"Asian",
		"Native"
	};

	// Token: 0x0400008F RID: 143
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] variants = new int[]
	{
		1,
		2,
		3,
		4
	};

	// Token: 0x04000090 RID: 144
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] hairColors;

	// Token: 0x04000091 RID: 145
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] eyeColors;

	// Token: 0x04000092 RID: 146
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] hairs = new string[]
	{
		"buzzcut",
		"comb_over",
		"slicked_back",
		"slicked_back_long",
		"pixie_cut",
		"ponytail",
		"midpart_karen_messy",
		"midpart_long",
		"midpart_mid",
		"midpart_short",
		"midpart_shoulder",
		"sidepart_short",
		"sidepart_mid",
		"sidepart_long",
		"cornrows",
		"mohawk",
		"flattop_fro",
		"small_fro",
		"dreads",
		"afro_curly"
	};

	// Token: 0x04000093 RID: 147
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] facialHairIndexes = new int[]
	{
		1,
		2,
		3,
		4,
		5
	};

	// Token: 0x04000094 RID: 148
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> allGearKeys = new List<string>();

	// Token: 0x04000095 RID: 149
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CharacterConfigurator.SDCSGearXmlCatalog gearCatalog;

	// Token: 0x04000096 RID: 150
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string[] baseParts = new string[]
	{
		"head",
		"body",
		"hands",
		"feet"
	};

	// Token: 0x02000011 RID: 17
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class SDCSGearXmlCatalog
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00005E58 File Offset: 0x00004058
		public static CharacterConfigurator.SDCSGearXmlCatalog BuildFromXml(string xmlText)
		{
			CharacterConfigurator.SDCSGearXmlCatalog sdcsgearXmlCatalog = new CharacterConfigurator.SDCSGearXmlCatalog();
			if (string.IsNullOrEmpty(xmlText))
			{
				return sdcsgearXmlCatalog;
			}
			foreach (XElement xelement in XDocument.Parse(xmlText, LoadOptions.PreserveWhitespace).Descendants("item"))
			{
				XElement xelement2 = xelement.Elements("property").FirstOrDefault((XElement p) => CharacterConfigurator.SDCSGearXmlCatalog.EqualsIgnoreCase(CharacterConfigurator.SDCSGearXmlCatalog.Attr(p, "class"), "SDCS"));
				if (xelement2 != null)
				{
					string text = CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement2, "Prefab");
					if (!string.IsNullOrEmpty(text))
					{
						string equipSlot = CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement, "EquipSlot");
						string text2 = CharacterConfigurator.SDCSGearXmlCatalog.NormalizePart(CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement2, "TransformName"), equipSlot);
						if (!string.IsNullOrEmpty(text2))
						{
							string itemName = CharacterConfigurator.SDCSGearXmlCatalog.Attr(xelement, "name");
							string text3 = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeGearKey(CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement, "ArmorGroup"), CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement, "DisplayType"), itemName);
							if (!string.IsNullOrEmpty(text3))
							{
								string excludes = CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement2, "Excludes");
								sdcsgearXmlCatalog.Add(new CharacterConfigurator.SDCSGearXmlCatalog.Entry
								{
									ItemName = itemName,
									GearKey = text3,
									PartName = text2,
									EquipSlot = equipSlot,
									PrefabName = text,
									BaseToTurnOff = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeBaseToTurnOff(excludes, text2),
									HairMaskType = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeOptional(CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement2, "HairMaskType")),
									FacialHairMaskType = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeOptional(CharacterConfigurator.SDCSGearXmlCatalog.DirectPropertyValue(xelement2, "FacialHairMaskType"))
								});
							}
						}
					}
				}
			}
			return sdcsgearXmlCatalog;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00006014 File Offset: 0x00004214
		public List<string> GetOrderedKeys()
		{
			return new List<string>(this.orderedKeys);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00006024 File Offset: 0x00004224
		public bool TryGetPart(string gearKey, string partName, out CharacterConfigurator.SDCSGearXmlCatalog.Entry entry)
		{
			entry = null;
			CharacterConfigurator.SDCSGearXmlCatalog.GearSet gearSet;
			return !string.IsNullOrEmpty(gearKey) && !string.IsNullOrEmpty(partName) && this.sets.TryGetValue(gearKey, out gearSet) && gearSet.Parts.TryGetValue(partName, out entry);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00006064 File Offset: 0x00004264
		[PublicizedFrom(EAccessModifier.Private)]
		public void Add(CharacterConfigurator.SDCSGearXmlCatalog.Entry entry)
		{
			if (entry == null || string.IsNullOrEmpty(entry.GearKey) || string.IsNullOrEmpty(entry.PartName))
			{
				return;
			}
			CharacterConfigurator.SDCSGearXmlCatalog.GearSet gearSet;
			if (!this.sets.TryGetValue(entry.GearKey, out gearSet))
			{
				gearSet = new CharacterConfigurator.SDCSGearXmlCatalog.GearSet();
				gearSet.Key = entry.GearKey;
				this.sets.Add(entry.GearKey, gearSet);
				this.orderedKeys.Add(entry.GearKey);
			}
			gearSet.Parts[entry.PartName] = entry;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000060EC File Offset: 0x000042EC
		[PublicizedFrom(EAccessModifier.Private)]
		public static string Attr(XElement e, string name)
		{
			XAttribute xattribute = (e == null) ? null : e.Attribute(name);
			if (xattribute != null)
			{
				return xattribute.Value;
			}
			return null;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006118 File Offset: 0x00004318
		[PublicizedFrom(EAccessModifier.Private)]
		public static string DirectPropertyValue(XElement parent, string name)
		{
			if (parent == null)
			{
				return null;
			}
			XElement xelement = parent.Elements("property").FirstOrDefault((XElement p) => CharacterConfigurator.SDCSGearXmlCatalog.EqualsIgnoreCase(CharacterConfigurator.SDCSGearXmlCatalog.Attr(p, "name"), name));
			XAttribute xattribute = (xelement == null) ? null : xelement.Attribute("value");
			if (xattribute != null)
			{
				return xattribute.Value;
			}
			return null;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000617C File Offset: 0x0000437C
		[PublicizedFrom(EAccessModifier.Private)]
		public static string NormalizeGearKey(string armorGroup, string displayType, string itemName)
		{
			if (!string.IsNullOrEmpty(armorGroup))
			{
				return CharacterConfigurator.SDCSGearXmlCatalog.StripPrefix(armorGroup.Trim(), "group");
			}
			return CharacterConfigurator.SDCSGearXmlCatalog.NormalizeOptional(CharacterConfigurator.SDCSGearXmlCatalog.StripSuffix(CharacterConfigurator.SDCSGearXmlCatalog.StripSuffix(CharacterConfigurator.SDCSGearXmlCatalog.StripSuffix(CharacterConfigurator.SDCSGearXmlCatalog.StripSuffix(CharacterConfigurator.SDCSGearXmlCatalog.StripPrefix(((!string.IsNullOrEmpty(displayType)) ? displayType : itemName) ?? string.Empty, "armor"), "Helmet"), "Outfit"), "Gloves"), "Boots"));
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000061F0 File Offset: 0x000043F0
		[PublicizedFrom(EAccessModifier.Private)]
		public static string NormalizePart(string transformName, string equipSlot)
		{
			string text = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeOptional(transformName);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.ToLowerInvariant();
				if (text == "head" || text == "body" || text == "hands" || text == "feet")
				{
					return text;
				}
			}
			string text2 = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeOptional(equipSlot);
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			string a = text2.ToLowerInvariant();
			if (a == "head")
			{
				return "head";
			}
			if (a == "chest")
			{
				return "body";
			}
			if (a == "hands")
			{
				return "hands";
			}
			if (!(a == "feet"))
			{
				return null;
			}
			return "feet";
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000062B0 File Offset: 0x000044B0
		[PublicizedFrom(EAccessModifier.Private)]
		public static string NormalizeBaseToTurnOff(string excludes, string preferredPart)
		{
			excludes = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeOptional(excludes);
			if (string.IsNullOrEmpty(excludes))
			{
				return null;
			}
			string[] array = excludes.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = CharacterConfigurator.SDCSGearXmlCatalog.NormalizePartName(array[i]);
				if (CharacterConfigurator.SDCSGearXmlCatalog.EqualsIgnoreCase(text, preferredPart))
				{
					return text;
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				string text2 = CharacterConfigurator.SDCSGearXmlCatalog.NormalizePartName(array[j]);
				if (!string.IsNullOrEmpty(text2))
				{
					return text2;
				}
			}
			return null;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00006320 File Offset: 0x00004520
		[PublicizedFrom(EAccessModifier.Private)]
		public static string NormalizePartName(string value)
		{
			value = CharacterConfigurator.SDCSGearXmlCatalog.NormalizeOptional(value);
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			value = value.ToLowerInvariant();
			if (value == "head" || value == "body" || value == "hands" || value == "feet")
			{
				return value;
			}
			return null;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000637E File Offset: 0x0000457E
		[PublicizedFrom(EAccessModifier.Private)]
		public static string NormalizeOptional(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}
			value = value.Trim();
			return value;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00006393 File Offset: 0x00004593
		[PublicizedFrom(EAccessModifier.Private)]
		public static string StripPrefix(string value, string prefix)
		{
			if (!string.IsNullOrEmpty(value) && value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			{
				return value.Substring(prefix.Length);
			}
			return value;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000063B5 File Offset: 0x000045B5
		[PublicizedFrom(EAccessModifier.Private)]
		public static string StripSuffix(string value, string suffix)
		{
			if (!string.IsNullOrEmpty(value) && value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
			{
				return value.Substring(0, value.Length - suffix.Length);
			}
			return value;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000063DF File Offset: 0x000045DF
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool EqualsIgnoreCase(string a, string b)
		{
			return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x04000097 RID: 151
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, CharacterConfigurator.SDCSGearXmlCatalog.GearSet> sets = new Dictionary<string, CharacterConfigurator.SDCSGearXmlCatalog.GearSet>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000098 RID: 152
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<string> orderedKeys = new List<string>();

		// Token: 0x02000012 RID: 18
		public sealed class Entry
		{
			// Token: 0x04000099 RID: 153
			public string ItemName;

			// Token: 0x0400009A RID: 154
			public string GearKey;

			// Token: 0x0400009B RID: 155
			public string PartName;

			// Token: 0x0400009C RID: 156
			public string EquipSlot;

			// Token: 0x0400009D RID: 157
			public string PrefabName;

			// Token: 0x0400009E RID: 158
			public string BaseToTurnOff;

			// Token: 0x0400009F RID: 159
			public string HairMaskType;

			// Token: 0x040000A0 RID: 160
			public string FacialHairMaskType;
		}

		// Token: 0x02000013 RID: 19
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class GearSet
		{
			// Token: 0x040000A1 RID: 161
			public string Key;

			// Token: 0x040000A2 RID: 162
			public readonly Dictionary<string, CharacterConfigurator.SDCSGearXmlCatalog.Entry> Parts = new Dictionary<string, CharacterConfigurator.SDCSGearXmlCatalog.Entry>(StringComparer.OrdinalIgnoreCase);
		}
	}
}
