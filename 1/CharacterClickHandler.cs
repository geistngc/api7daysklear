using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class CharacterClickHandler : MonoBehaviour
{
	// Token: 0x060000BE RID: 190 RVA: 0x000090B9 File Offset: 0x000072B9
	public void HandleClick()
	{
		this.parentScript.OnCharacterClicked(base.gameObject);
	}

	// Token: 0x040000E8 RID: 232
	public CharacterConstruct parentScript;
}
