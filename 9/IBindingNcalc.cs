using System;

// Token: 0x02001101 RID: 4353
public interface IBindingNcalc
{
	// Token: 0x1700100B RID: 4107
	// (get) Token: 0x06008A33 RID: 35379
	string SourceText { get; }

	// Token: 0x1700100C RID: 4108
	// (get) Token: 0x06008A34 RID: 35380
	IXUiElement TargetElement { get; }

	// Token: 0x1700100D RID: 4109
	// (get) Token: 0x06008A35 RID: 35381
	bool IsInitializing { get; }

	// Token: 0x06008A36 RID: 35382
	void SetIndeterministic();

	// Token: 0x06008A37 RID: 35383
	bool FindParameter(string _name, out object _value);

	// Token: 0x06008A38 RID: 35384
	bool RegisterVariable(BindingInfoNcalc.VariableStateAbs _var);
}
