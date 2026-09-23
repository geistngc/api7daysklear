using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200121B RID: 4635
public class GUILocalizedLabel : MonoBehaviour
{
	// Token: 0x060093FC RID: 37884 RVA: 0x0037FEC4 File Offset: 0x0037E0C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		Text component = base.GetComponent<Text>();
		if (component)
		{
			component.text = Localization.Get(this.localizationKey, false, null);
		}
	}

	// Token: 0x04006EE7 RID: 28391
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizationKey;
}
