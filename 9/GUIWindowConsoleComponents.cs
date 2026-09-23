using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02001221 RID: 4641
public class GUIWindowConsoleComponents : MonoBehaviour
{
	// Token: 0x06009433 RID: 37939 RVA: 0x00380CBD File Offset: 0x0037EEBD
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.buttonPrompts = new List<GUIButtonPrompt>(base.GetComponentsInChildren<GUIButtonPrompt>());
	}

	// Token: 0x06009434 RID: 37940 RVA: 0x00380CD0 File Offset: 0x0037EED0
	public void RefreshButtonPrompts()
	{
		foreach (GUIButtonPrompt guibuttonPrompt in this.buttonPrompts)
		{
			guibuttonPrompt.RefreshIcon();
		}
	}

	// Token: 0x04006F0B RID: 28427
	public ScrollRect scrollRect;

	// Token: 0x04006F0C RID: 28428
	public Transform contentRect;

	// Token: 0x04006F0D RID: 28429
	public InputField commandField;

	// Token: 0x04006F0E RID: 28430
	public Button closeButton;

	// Token: 0x04006F0F RID: 28431
	public Button openLogsButton;

	// Token: 0x04006F10 RID: 28432
	public GameObject controllerPrompts;

	// Token: 0x04006F11 RID: 28433
	public GameObject consoleLinePrefab;

	// Token: 0x04006F12 RID: 28434
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GUIButtonPrompt> buttonPrompts;
}
