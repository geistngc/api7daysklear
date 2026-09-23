using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003F2 RID: 1010
[Preserve]
public struct EntityActivationCommand
{
	// Token: 0x06001E95 RID: 7829 RVA: 0x000B99C8 File Offset: 0x000B7BC8
	public EntityActivationCommand(string _commandId, string _icon, string _eventName = null, string _customCommandText = null)
	{
		this.commandId = _commandId;
		this.icon = _icon;
		this.eventName = _eventName;
		this.iconColor = Color.white;
		this.activateTime = -1f;
		this.enabled = true;
		this.commandText = this.commandId;
		if (!string.IsNullOrEmpty(_customCommandText))
		{
			this.commandText = _customCommandText;
		}
		this.commandText = Localization.Get("entitycommand_" + this.commandText, false, null);
	}

	// Token: 0x0400147A RID: 5242
	public string commandId;

	// Token: 0x0400147B RID: 5243
	public string icon;

	// Token: 0x0400147C RID: 5244
	public Color iconColor;

	// Token: 0x0400147D RID: 5245
	public bool enabled;

	// Token: 0x0400147E RID: 5246
	public string eventName;

	// Token: 0x0400147F RID: 5247
	public float activateTime;

	// Token: 0x04001480 RID: 5248
	public string commandText;
}
