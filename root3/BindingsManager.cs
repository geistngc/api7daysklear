using System;
using System.Collections.Generic;
using Unity.Profiling;

// Token: 0x02001103 RID: 4355
public class BindingsManager
{
	// Token: 0x06008A3B RID: 35387 RVA: 0x0034B0F0 File Offset: 0x003492F0
	public void RefreshBindings()
	{
		using (this.pmControllerRefreshBindings.Auto())
		{
			for (int i = 0; i < this.bindingList.Count; i++)
			{
				this.bindingList[i].RefreshValue();
			}
		}
	}

	// Token: 0x06008A3C RID: 35388 RVA: 0x0034B150 File Offset: 0x00349350
	public void AddBinding(IBindingInstance _info)
	{
		if (!this.bindingList.Contains(_info))
		{
			this.bindingList.Add(_info);
		}
	}

	// Token: 0x06008A3D RID: 35389 RVA: 0x0034B16C File Offset: 0x0034936C
	public static IBindingInstance CreateBinding(XUiView _view, string _attribute, string _value)
	{
		IBindingInstance result;
		using (BindingsManager.pmControllerCreateBinding.Auto())
		{
			BindingNcalcFunctions.RegisterNcalcFunctions();
			IBindingInstance bindingInstance;
			if (BindingInfoNcalc.IsFullNcalcBinding(_value))
			{
				bindingInstance = new BindingInfoNcalc(_view, _attribute, _value);
				bindingInstance.RefreshValue();
			}
			else
			{
				bindingInstance = new BindingInfo(_view, _attribute, _value);
			}
			result = bindingInstance;
		}
		return result;
	}

	// Token: 0x06008A3E RID: 35390 RVA: 0x0034B1D0 File Offset: 0x003493D0
	public static string ReplaceCVars(string _fullText)
	{
		string result;
		using (BindingsManager.pmReplaceCvars.Auto())
		{
			for (int num = _fullText.IndexOf("{cvar(", StringComparison.Ordinal); num != -1; num = _fullText.IndexOf("{cvar(", num, StringComparison.Ordinal))
			{
				string text = _fullText.Substring(num, _fullText.IndexOf('}', num) + 1 - num);
				string format = "";
				int num2 = text.IndexOf('(') + 1;
				string text2 = text.Substring(num2, text.IndexOf(')') - num2);
				if (text2.IndexOf(':') >= 0)
				{
					string[] array = text2.Split(':', StringSplitOptions.None);
					text2 = array[0];
					format = array[1];
				}
				_fullText = _fullText.Replace(text, XUiM_Player.GetPlayer().GetCVar(text2).ToString(format));
			}
			result = _fullText;
		}
		return result;
	}

	// Token: 0x040066B5 RID: 26293
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<IBindingInstance> bindingList = new List<IBindingInstance>();

	// Token: 0x040066B6 RID: 26294
	[PublicizedFrom(EAccessModifier.Private)]
	public ProfilerMarker pmControllerRefreshBindings = new ProfilerMarker("XC.RefreshBindings");

	// Token: 0x040066B7 RID: 26295
	[PublicizedFrom(EAccessModifier.Private)]
	public static ProfilerMarker pmControllerCreateBinding = new ProfilerMarker("XC.CreateBinding");

	// Token: 0x040066B8 RID: 26296
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker pmReplaceCvars = new ProfilerMarker("ParseCVars");
}
