using System;
using System.Collections.Generic;
using NCalc;
using Unity.Profiling;

// Token: 0x020010F5 RID: 4341
public class BindingItemNcalc : BindingItem, IBindingNcalc
{
	// Token: 0x060089D6 RID: 35286 RVA: 0x00349184 File Offset: 0x00347384
	public BindingItemNcalc(BindingInfo _parent, string _sourceText) : base(_sourceText)
	{
		if (this.fieldName[0] == '#')
		{
			this.fieldName = this.fieldName.Substring(1);
		}
		this.Parent = _parent;
		this.IsInitializing = 1;
		this.expression = new Expression(this.fieldName, EvaluateOptions.IgnoreCase | EvaluateOptions.NoCache | EvaluateOptions.UseDoubleForAbsFunction | EvaluateOptions.ReuseInstances)
		{
			Parameters = this.expressionParamDict
		};
		this.expression.EvaluateFunction += this.nCalcEvaluateFunction;
		this.expression.EvaluateParameter += this.nCalcEvaluateParameter;
		this.currentValue = this.evaluateExpression();
		this.IsInitializing = 0;
		if (this.usesIndeterministicFunctions || this.bindings.Count == 0)
		{
			for (XUiController xuiController = this.Parent.View.Controller; xuiController != null; xuiController = xuiController.Parent)
			{
				if (xuiController.GetType() != typeof(XUiController))
				{
					xuiController.Bindings.AddBinding(_parent);
					return;
				}
			}
		}
	}

	// Token: 0x060089D7 RID: 35287 RVA: 0x003492B4 File Offset: 0x003474B4
	public override string GetValue()
	{
		bool flag = this.usesIndeterministicFunctions;
		foreach (KeyValuePair<string, BindingItemNcalc.BindingState> keyValuePair in this.bindings)
		{
			if (keyValuePair.Value.RefreshValue())
			{
				this.expressionParamDict[keyValuePair.Key] = keyValuePair.Value.CurrentValue;
				flag = true;
			}
		}
		if (!flag)
		{
			return this.currentValue;
		}
		using (this.pmNcalcEval.Auto())
		{
			this.currentValue = this.evaluateExpression();
		}
		return this.currentValue;
	}

	// Token: 0x060089D8 RID: 35288 RVA: 0x00349380 File Offset: 0x00347580
	[PublicizedFrom(EAccessModifier.Private)]
	public string evaluateExpression()
	{
		string result = null;
		try
		{
			object obj = this.expression.Evaluate();
			string text;
			if (obj != null)
			{
				if (obj is decimal)
				{
					decimal value = (decimal)obj;
					text = value.ToCultureInvariantString("0.########");
				}
				else if (obj is float)
				{
					float value2 = (float)obj;
					text = value2.ToCultureInvariantString();
				}
				else if (obj is double)
				{
					double value3 = (double)obj;
					text = value3.ToCultureInvariantString();
				}
				else
				{
					text = obj.ToString();
				}
			}
			else
			{
				text = "";
			}
			result = text;
		}
		catch (ArgumentException e)
		{
			Log.Error("[XUi] Binding expression can not be evaluated: " + this.SourceText + " --- hierarchy: " + this.Parent.View.GetXuiHierarchy());
			Log.Exception(e);
		}
		catch (Exception e2)
		{
			Log.Error("[XUi] Binding expression can not be evaluated: " + this.SourceText + " --- hierarchy: " + this.Parent.View.GetXuiHierarchy());
			Log.Exception(e2);
			result = "";
		}
		return result;
	}

	// Token: 0x060089D9 RID: 35289 RVA: 0x00349494 File Offset: 0x00347694
	[PublicizedFrom(EAccessModifier.Private)]
	public void nCalcEvaluateParameter(string _name, ParameterArgs _args)
	{
		object result;
		if (this.FindParameter(_name, out result))
		{
			_args.Result = result;
		}
	}

	// Token: 0x060089DA RID: 35290 RVA: 0x003494B4 File Offset: 0x003476B4
	public bool FindParameter(string _name, out object _value)
	{
		BindingItemNcalc.BindingState bindingState;
		if (this.bindings.TryGetValue(_name, out bindingState))
		{
			_value = bindingState.CurrentValue;
			return true;
		}
		for (XUiController xuiController = this.Parent.View.Controller; xuiController != null; xuiController = xuiController.Parent)
		{
			if (xuiController.GetType() != typeof(XUiController))
			{
				string text = "";
				if (xuiController.GetBindingValue(ref text, _name))
				{
					xuiController.Bindings.AddBinding(this.Parent);
					bindingState = new BindingItemNcalc.BindingState(_name, xuiController, text, null);
					this.bindings[_name] = bindingState;
					this.expressionParamDict[_name] = text;
					_value = text;
					return true;
				}
				XuiBindingDelegate xuiBindingDelegate;
				if (BindingMethodCache.Instance.TryGetBindingDelegate(xuiController, _name, out xuiBindingDelegate))
				{
					xuiController.Bindings.AddBinding(this.Parent);
					text = xuiBindingDelegate(xuiController).ToString();
					bindingState = new BindingItemNcalc.BindingState(_name, xuiController, text, xuiBindingDelegate);
					this.bindings[_name] = bindingState;
					this.expressionParamDict[_name] = text;
					_value = xuiBindingDelegate(xuiController);
					return true;
				}
			}
		}
		_value = null;
		return false;
	}

	// Token: 0x060089DB RID: 35291 RVA: 0x0034888F File Offset: 0x00346A8F
	[PublicizedFrom(EAccessModifier.Private)]
	public void nCalcEvaluateFunction(string _name, FunctionArgs _args, bool _ignoreCase)
	{
		BindingNcalcFunctions.EvaluateFunc(this, _name, _args);
	}

	// Token: 0x17001007 RID: 4103
	// (get) Token: 0x060089DC RID: 35292 RVA: 0x003495C4 File Offset: 0x003477C4
	public new string SourceText
	{
		get
		{
			return this.SourceText;
		}
	}

	// Token: 0x17001008 RID: 4104
	// (get) Token: 0x060089DD RID: 35293 RVA: 0x003495CC File Offset: 0x003477CC
	public IXUiElement TargetElement
	{
		get
		{
			return this.Parent.View;
		}
	}

	// Token: 0x17001009 RID: 4105
	// (get) Token: 0x060089DE RID: 35294 RVA: 0x003495D9 File Offset: 0x003477D9
	public bool IsInitializing { get; }

	// Token: 0x060089DF RID: 35295 RVA: 0x003495E1 File Offset: 0x003477E1
	public void SetIndeterministic()
	{
		this.usesIndeterministicFunctions = true;
	}

	// Token: 0x060089E0 RID: 35296 RVA: 0x00010E62 File Offset: 0x0000F062
	public bool RegisterVariable(BindingInfoNcalc.VariableStateAbs _var)
	{
		return false;
	}

	// Token: 0x0400668F RID: 26255
	public readonly BindingInfo Parent;

	// Token: 0x04006690 RID: 26256
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, BindingItemNcalc.BindingState> bindings = new Dictionary<string, BindingItemNcalc.BindingState>(StringComparer.Ordinal);

	// Token: 0x04006691 RID: 26257
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, object> expressionParamDict = new Dictionary<string, object>(StringComparer.Ordinal);

	// Token: 0x04006692 RID: 26258
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Expression expression;

	// Token: 0x04006693 RID: 26259
	[PublicizedFrom(EAccessModifier.Private)]
	public bool usesIndeterministicFunctions;

	// Token: 0x04006694 RID: 26260
	[PublicizedFrom(EAccessModifier.Private)]
	public string currentValue;

	// Token: 0x04006695 RID: 26261
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ProfilerMarker pmNcalcEval = new ProfilerMarker("NCalc.Evaluate");

	// Token: 0x020010F6 RID: 4342
	[PublicizedFrom(EAccessModifier.Private)]
	public class BindingState
	{
		// Token: 0x060089E1 RID: 35297 RVA: 0x003495EA File Offset: 0x003477EA
		public BindingState(string _bindingName, XUiController _controller, string _initialValue, XuiBindingDelegate _bindingDelegate)
		{
			this.bindingName = _bindingName;
			this.controller = _controller;
			this.CurrentValue = _initialValue;
			this.bindingDelegate = _bindingDelegate;
		}

		// Token: 0x060089E2 RID: 35298 RVA: 0x00349610 File Offset: 0x00347810
		public bool RefreshValue()
		{
			string text = "";
			if (this.bindingDelegate == null)
			{
				if (!this.controller.GetBindingValue(ref text, this.bindingName))
				{
					Log.Error(string.Concat(new string[]
					{
						"[XUi] Refreshing binding failed: Controller's GetBindingValue no longer returns true! (Binding: ",
						this.bindingName,
						", hierarchy: ",
						this.controller.GetXuiHierarchy(),
						")"
					}));
					return false;
				}
			}
			else
			{
				object obj = this.bindingDelegate(this.controller);
				if (obj == null)
				{
					Log.Warning("[XUi] Binding '" + this.bindingName + "' returned null, should always return appropriate non-null value of same data type. Hierarchy: " + this.controller.GetXuiHierarchy());
					obj = "";
				}
				text = obj.ToString();
			}
			if (string.Equals(this.CurrentValue, text, StringComparison.Ordinal))
			{
				return false;
			}
			this.CurrentValue = text;
			return true;
		}

		// Token: 0x060089E3 RID: 35299 RVA: 0x003496E4 File Offset: 0x003478E4
		public override string ToString()
		{
			return "Binding:" + this.bindingName + "=" + this.CurrentValue;
		}

		// Token: 0x04006697 RID: 26263
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string bindingName;

		// Token: 0x04006698 RID: 26264
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUiController controller;

		// Token: 0x04006699 RID: 26265
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XuiBindingDelegate bindingDelegate;

		// Token: 0x0400669A RID: 26266
		public string CurrentValue;
	}
}
