using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200001D RID: 29
public class CharacterConstructUIController : MonoBehaviour
{
	// Token: 0x060000C1 RID: 193 RVA: 0x000090CC File Offset: 0x000072CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		if (this.characterConstruct != null)
		{
			this.InitializeToggles();
			this.InitializeDropdowns();
		}
		else
		{
			Debug.LogError("CharacterConstruct reference is missing! Please assign it in the inspector.");
		}
		this.SetupToggleListeners();
		this.SetupDropdownListeners();
		this.UpdateHatGearControlsVisibility(this.showHatHairToggle.isOn);
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x0000911C File Offset: 0x0000731C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitializeToggles()
	{
		this.showCharactersToggle.isOn = this.characterConstruct.ShowCharacters;
		this.showGearToggle.isOn = this.characterConstruct.ShowGear;
		this.showHairToggle.isOn = this.characterConstruct.ShowHair;
		this.showHatHairToggle.isOn = this.characterConstruct.ShowHatHair;
		this.showFacialHairToggle.isOn = this.characterConstruct.ShowFacialHair;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00009198 File Offset: 0x00007398
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitializeDropdowns()
	{
		if (this.raceDropdown != null)
		{
			this.raceDropdown.ClearOptions();
			foreach (string text in this.characterConstruct.GetRaceTypes())
			{
				this.raceDropdown.options.Add(new TMP_Dropdown.OptionData(text));
			}
			this.raceDropdown.value = this.characterConstruct.selectedRaceIndex;
			this.raceDropdown.RefreshShownValue();
		}
		if (this.variantDropdown != null)
		{
			this.variantDropdown.ClearOptions();
			foreach (string text2 in this.characterConstruct.GetVariantTypes())
			{
				this.variantDropdown.options.Add(new TMP_Dropdown.OptionData(text2));
			}
			this.variantDropdown.value = this.characterConstruct.selectedVariantIndex;
			this.variantDropdown.RefreshShownValue();
		}
		if (this.hatGearDropdown != null)
		{
			this.hatGearDropdown.ClearOptions();
			foreach (string text3 in this.characterConstruct.GetGearTypes())
			{
				this.hatGearDropdown.options.Add(new TMP_Dropdown.OptionData(text3));
			}
			this.hatGearDropdown.value = this.characterConstruct.hatHairGearIndex;
			this.hatGearDropdown.RefreshShownValue();
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x000092F4 File Offset: 0x000074F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupToggleListeners()
	{
		this.showCharactersToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowCharactersToggled));
		this.showGearToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowGearToggled));
		this.showHairToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowHairToggled));
		this.showHatHairToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowHatHairToggled));
		this.showFacialHairToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowFacialHairToggled));
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00009390 File Offset: 0x00007590
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupDropdownListeners()
	{
		if (this.raceDropdown != null)
		{
			this.raceDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnRaceDropdownChanged));
		}
		if (this.variantDropdown != null)
		{
			this.variantDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnVariantDropdownChanged));
		}
		if (this.hatGearDropdown != null)
		{
			this.hatGearDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnHatGearDropdownChanged));
		}
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x0000941B File Offset: 0x0000761B
	public void OnShowCharactersToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowCharacters = isOn;
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00009437 File Offset: 0x00007637
	public void OnShowGearToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowGear = isOn;
		}
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00009453 File Offset: 0x00007653
	public void OnShowHairToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowHair = isOn;
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x0000946F File Offset: 0x0000766F
	public void OnShowHatHairToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowHatHair = isOn;
			this.UpdateHatGearControlsVisibility(isOn);
		}
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00009492 File Offset: 0x00007692
	public void OnShowFacialHairToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowFacialHair = isOn;
		}
	}

	// Token: 0x060000CB RID: 203 RVA: 0x000094AE File Offset: 0x000076AE
	public void OnRaceDropdownChanged(int index)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.selectedRaceIndex = index;
			this.characterConstruct.RespawnAllGroups();
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x000094D5 File Offset: 0x000076D5
	public void OnVariantDropdownChanged(int index)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.selectedVariantIndex = index;
			this.characterConstruct.RespawnAllGroups();
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x000094FC File Offset: 0x000076FC
	public void OnHatGearDropdownChanged(int index)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.hatHairGearIndex = index;
			this.characterConstruct.RespawnHatHairGroup();
		}
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00009523 File Offset: 0x00007723
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHatGearControlsVisibility(bool isHatHairVisible)
	{
		if (this.hatGearControlsPanel != null)
		{
			this.hatGearControlsPanel.SetActive(isHatHairVisible);
		}
	}

	// Token: 0x040000EA RID: 234
	[Header("References")]
	[Tooltip("Reference to the CharacterConstruct script")]
	public CharacterConstruct characterConstruct;

	// Token: 0x040000EB RID: 235
	[Header("UI Elements")]
	[Tooltip("Toggle button for showing characters")]
	public Toggle showCharactersToggle;

	// Token: 0x040000EC RID: 236
	[Tooltip("Toggle button for showing gear")]
	public Toggle showGearToggle;

	// Token: 0x040000ED RID: 237
	[Tooltip("Toggle button for showing hair")]
	public Toggle showHairToggle;

	// Token: 0x040000EE RID: 238
	[Tooltip("Toggle button for showing hat hair")]
	public Toggle showHatHairToggle;

	// Token: 0x040000EF RID: 239
	[Tooltip("Toggle button for showing facial hair")]
	public Toggle showFacialHairToggle;

	// Token: 0x040000F0 RID: 240
	[Header("Race and Variant Controls")]
	[Tooltip("Panel containing race and variant controls")]
	public GameObject raceVariantControlsPanel;

	// Token: 0x040000F1 RID: 241
	[Tooltip("Dropdown for selecting race")]
	public TMP_Dropdown raceDropdown;

	// Token: 0x040000F2 RID: 242
	[Tooltip("Dropdown for selecting variant")]
	public TMP_Dropdown variantDropdown;

	// Token: 0x040000F3 RID: 243
	[Header("Hat Hair Controls")]
	[Tooltip("Panel containing hat gear controls")]
	public GameObject hatGearControlsPanel;

	// Token: 0x040000F4 RID: 244
	[Tooltip("Dropdown for selecting hat gear type")]
	public TMP_Dropdown hatGearDropdown;

	// Token: 0x040000F5 RID: 245
	[Header("UI Panel")]
	[Tooltip("Panel that contains all UI controls")]
	public GameObject controlPanel;
}
