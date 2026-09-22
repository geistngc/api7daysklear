using System;

// Token: 0x020010F4 RID: 4340
public class BindingItemCvar : BindingItem
{
	// Token: 0x060089D4 RID: 35284 RVA: 0x003490AC File Offset: 0x003472AC
	public BindingItemCvar(BindingInfo _parent, string _sourceText) : base(_sourceText)
	{
		this.fieldName = this.fieldName.Replace("cvar(", "").Replace(")", "");
		if (this.fieldName.IndexOf(':') >= 0)
		{
			string[] array = this.fieldName.Split(':', StringSplitOptions.None);
			this.fieldName = array[0];
			this.format = array[1];
		}
		for (XUiController xuiController = _parent.View.Controller; xuiController != null; xuiController = xuiController.Parent)
		{
			if (xuiController.GetType() != typeof(XUiController))
			{
				xuiController.Bindings.AddBinding(_parent);
				return;
			}
		}
	}

	// Token: 0x060089D5 RID: 35285 RVA: 0x00349158 File Offset: 0x00347358
	public override string GetValue()
	{
		return XUiM_Player.GetPlayer().GetCVar(this.fieldName).ToString(this.format);
	}

	// Token: 0x0400668E RID: 26254
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string format;
}
