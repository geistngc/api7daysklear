using System;
using UnityEngine;

// Token: 0x02000103 RID: 259
public struct BlockActivationCommand
{
	// Token: 0x0600064A RID: 1610 RVA: 0x0002CC09 File Offset: 0x0002AE09
	public BlockActivationCommand(string _text, string _icon, bool _enabled, bool _highlighted = false, string _eventName = null)
	{
		this.text = _text;
		this.icon = _icon;
		this.enabled = _enabled;
		this.highlighted = _highlighted;
		this.eventName = _eventName;
		this.iconColor = Color.white;
		this.activateTime = -1f;
	}

	// Token: 0x040006FC RID: 1788
	public string text;

	// Token: 0x040006FD RID: 1789
	public string icon;

	// Token: 0x040006FE RID: 1790
	public Color iconColor;

	// Token: 0x040006FF RID: 1791
	public bool enabled;

	// Token: 0x04000700 RID: 1792
	public bool highlighted;

	// Token: 0x04000701 RID: 1793
	public string eventName;

	// Token: 0x04000702 RID: 1794
	public float activateTime;

	// Token: 0x04000703 RID: 1795
	public static readonly BlockActivationCommand[] Empty = Array.Empty<BlockActivationCommand>();
}
