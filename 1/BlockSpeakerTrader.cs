using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000151 RID: 337
[Preserve]
public class BlockSpeakerTrader : Block
{
	// Token: 0x06000950 RID: 2384 RVA: 0x00040D04 File Offset: 0x0003EF04
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("OpenSound"))
		{
			this.openSound = base.Properties.Values["OpenSound"];
		}
		if (base.Properties.Values.ContainsKey("CloseSound"))
		{
			this.closeSound = base.Properties.Values["CloseSound"];
		}
		if (base.Properties.Values.ContainsKey("WarningSound"))
		{
			this.warningSound = base.Properties.Values["WarningSound"];
		}
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00040DB0 File Offset: 0x0003EFB0
	public void PlayOpen(Vector3i _blockPos, EntityTrader _trader)
	{
		string text = this.openSound;
		if (string.IsNullOrEmpty(text))
		{
			text = ((_trader != null) ? (_trader.NPCInfo.VoiceSet + "_announce_open") : "");
		}
		if (text != "")
		{
			Manager.BroadcastPlay(_blockPos.ToVector3(), text, 0f);
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00040E14 File Offset: 0x0003F014
	public void PlayClose(Vector3i _blockPos, EntityTrader _trader)
	{
		string text = this.closeSound;
		if (string.IsNullOrEmpty(text))
		{
			text = ((_trader != null) ? (_trader.NPCInfo.VoiceSet + "_announce_closed") : "");
		}
		if (text != "")
		{
			Manager.BroadcastPlay(_blockPos.ToVector3(), text, 0f);
		}
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00040E78 File Offset: 0x0003F078
	public void PlayWarning(Vector3i _blockPos, EntityTrader _trader)
	{
		string text = this.warningSound;
		if (string.IsNullOrEmpty(text))
		{
			text = ((_trader != null) ? (_trader.NPCInfo.VoiceSet + "_announce_closing") : "");
		}
		if (text != "")
		{
			Manager.BroadcastPlay(_blockPos.ToVector3(), text, 0f);
		}
	}

	// Token: 0x040009B9 RID: 2489
	[PublicizedFrom(EAccessModifier.Private)]
	public string openSound;

	// Token: 0x040009BA RID: 2490
	[PublicizedFrom(EAccessModifier.Private)]
	public string closeSound;

	// Token: 0x040009BB RID: 2491
	[PublicizedFrom(EAccessModifier.Private)]
	public string warningSound;
}
