using System;
using System.Collections.Generic;

// Token: 0x02000446 RID: 1094
public class EAIItemTask : EAIBase
{
	// Token: 0x06002164 RID: 8548 RVA: 0x000C9597 File Offset: 0x000C7797
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000C95A0 File Offset: 0x000C77A0
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000C95A9 File Offset: 0x000C77A9
	public override bool CanExecute()
	{
		return !string.IsNullOrEmpty(this.ItemKey);
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000C95B9 File Offset: 0x000C77B9
	public override bool Continue()
	{
		return base.Continue();
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000C95C1 File Offset: 0x000C77C1
	public override void Update()
	{
		base.Update();
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000C95C9 File Offset: 0x000C77C9
	public override void Reset()
	{
		base.Reset();
	}

	// Token: 0x040016FB RID: 5883
	public string ItemKey;
}
