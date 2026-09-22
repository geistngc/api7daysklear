using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
[RequireComponent(typeof(UIInput))]
[AddComponentMenu("NGUI/Examples/Chat Input")]
public class ChatInput : MonoBehaviour
{
	// Token: 0x06000196 RID: 406 RVA: 0x0000FDE0 File Offset: 0x0000DFE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mInput = base.GetComponent<UIInput>();
		this.mInput.label.maxLineCount = 1;
		if (this.fillWithDummyData && this.textList != null)
		{
			for (int i = 0; i < 30; i++)
			{
				this.textList.Add(((i % 2 == 0) ? "[FFFFFF]" : "[AAAAAA]") + "This is an example paragraph for the text list, testing line " + i.ToString() + "[-]");
			}
		}
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000FE60 File Offset: 0x0000E060
	public void OnSubmit()
	{
		if (this.textList != null)
		{
			string text = NGUIText.StripSymbols(this.mInput.value);
			if (!string.IsNullOrEmpty(text))
			{
				this.textList.Add(text);
				this.mInput.value = "";
				this.mInput.isSelected = false;
			}
		}
	}

	// Token: 0x04000247 RID: 583
	public UITextList textList;

	// Token: 0x04000248 RID: 584
	public bool fillWithDummyData;

	// Token: 0x04000249 RID: 585
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIInput mInput;
}
