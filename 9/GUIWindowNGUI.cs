using System;

// Token: 0x02001224 RID: 4644
public class GUIWindowNGUI : GUIWindow
{
	// Token: 0x0600945E RID: 37982 RVA: 0x003819B3 File Offset: 0x0037FBB3
	public GUIWindowNGUI(EnumNGUIWindow _nguiEnum) : base(_nguiEnum.ToStringCached<EnumNGUIWindow>())
	{
		this.nguiEnum = _nguiEnum;
	}

	// Token: 0x0600945F RID: 37983 RVA: 0x003819C8 File Offset: 0x0037FBC8
	public override void OnOpen()
	{
		this.playerUI.nguiWindowManager.Show(this.nguiEnum, true);
	}

	// Token: 0x06009460 RID: 37984 RVA: 0x003819E1 File Offset: 0x0037FBE1
	public override void OnClose()
	{
		base.OnClose();
		this.playerUI.nguiWindowManager.Show(this.nguiEnum, false);
	}

	// Token: 0x04006F27 RID: 28455
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumNGUIWindow nguiEnum;
}
