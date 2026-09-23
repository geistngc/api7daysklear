using System;
using Platform;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200121A RID: 4634
public class GUIButtonPrompt : MonoBehaviour
{
	// Token: 0x060093F8 RID: 37880 RVA: 0x0037FE7B File Offset: 0x0037E07B
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.image = base.GetComponent<Image>();
	}

	// Token: 0x060093F9 RID: 37881 RVA: 0x0037FE89 File Offset: 0x0037E089
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.RefreshIcon();
	}

	// Token: 0x060093FA RID: 37882 RVA: 0x0037FE94 File Offset: 0x0037E094
	public void RefreshIcon()
	{
		PlayerInputManager.InputStyle inputStyle = PlayerInputManager.InputStyleFromSelectedIconStyle();
		this.image.sprite = ((inputStyle == PlayerInputManager.InputStyle.PS4) ? this.PSSprite : this.XBSprite);
	}

	// Token: 0x04006EE4 RID: 28388
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Sprite XBSprite;

	// Token: 0x04006EE5 RID: 28389
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Sprite PSSprite;

	// Token: 0x04006EE6 RID: 28390
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Image image;
}
