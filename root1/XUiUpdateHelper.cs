using System;
using UnityEngine;

// Token: 0x02001180 RID: 4480
[PublicizedFrom(EAccessModifier.Internal)]
public class XUiUpdateHelper : MonoBehaviour
{
	// Token: 0x06008FA8 RID: 36776 RVA: 0x0036047E File Offset: 0x0035E67E
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		XUiUpdater.Update();
	}
}
