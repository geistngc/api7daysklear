using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NCalc;
using Unity.Profiling;

// Token: 0x020010E5 RID: 4325
public class BindingInfoNcalc : IBindingInstance, IBindingNcalc
{
	// Token: 0x06008995 RID: 35221 RVA: 0x00348104 File Offset: 0x00346304
	public static bool IsFullNcalcBinding(string _input)
	{
		if (!BindingInfoNcalc.fullBinding.IsMatch(_input))
		{
			return false;
		}
		int num = 0;
		for (int i = 2; i < _input.Length - 1; i++)
		{
			if (_input[i] == '{')
			{
				num++;
			}
			else if (_input[i] == '}')
			{
				num--;
			}
			if (num < 0)
			{
				return false;
			}
		}
		return num <= 0;
	}

	// Token: 0x06008996 RID: 35222 RVA: 0x00348164 File Offset: 0x00346364
	public BindingInfoNcalc(IXUiElement _targetViewOrTween, string _attribute, string _sourceString)
	{
		this.targetViewOrTween = _targetViewOrTween;
		this.attributeName = _attribute;
		this.sourceString = _sourceString;
		this.pmRefreshValueCompleteSourceText = new ProfilerMarker(_sourceString);
		int num = 1;
		if (_sourceString[1] == '%' || _sourceString[1] == '#')
		{
			num++;
		}
		string input = _sourceString.Substring(num, _sourceString.Length - num - 1);
		Match match = BindingInfoNcalc.singleIdentifierExpression.Match(input);
		if (match.Success)
		{
			string value = match.Groups[1].Value;
			this.singleIdentifierBinding = true;
			if (!this.FindParameter(value, out this.currentValue))
			{
				if (XUiFromXml.DebugXuiLoading != XUiFromXml.DebugLevel.Off)
				{
					Log.Warning("[XUi] Binding name '" + value + "' not found! Hierarchy: " + this.xuiHierarchy());
				}
				return;
			}
		}
		else
		{
			this.expression = new Expression(input, EvaluateOptions.NoCache | EvaluateOptions.UseDoubleForAbsFunction | EvaluateOptions.ReuseInstances)
			{
				Parameters = this.expressionParamDict
			};
			this.expression.EvaluateFunction += this.nCalcEvaluateFunction;
			this.expression.EvaluateParameter += this.nCalcEvaluateParameter;
			this.IsInitializing = 1;
			this.currentValue = this.evaluateExpression();
			this.IsInitializing = 0;
		}
		if (this.currentValue == null)
		{
			this.expression = null;
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Failed evaluating binding expression, result is null. Binding ",
				this.attributeName,
				"=\"",
				this.sourceString,
				"\", view hierarchy: ",
				this.xuiHierarchy()
			}));
			return;
		}
		this.<.ctor>g__BindToController|16_0();
		Type type = this.currentValue.GetType();
		ParsingMethodCache.ParsingMethodData parsingMethodData;
		if (ParsingMethodCache.Instance.TryGetParsingDelegate(this.targetViewOrTween, this.attributeName, out parsingMethodData))
		{
			if (parsingMethodData.TryGetDelegateForSourceType(type, out this.targetParsingDelegate))
			{
				this.targetParsingElement = this.targetViewOrTween;
				return;
			}
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Failed mapping binding to parser on view, no conversion possible. Binding ",
				this.attributeName,
				"=\"",
				this.sourceString,
				"\", binding result type ",
				type.FullName,
				", parser native type ",
				parsingMethodData.NativeParseType.FullName,
				", view hierarchy: ",
				this.xuiHierarchy()
			}));
			this.expression = null;
			return;
		}
		else
		{
			if (this.targetViewOrTween is XUiTweenAbs)
			{
				Log.Error("[XUi] Failed mapping binding to parser on XUiTween, no parse found for attribute name '" + this.attributeName + "', view hierarchy: " + this.xuiHierarchy());
				this.expression = null;
				return;
			}
			ParsingMethodCache.ParsingMethodData parsingMethodData2;
			if (!ParsingMethodCache.Instance.TryGetParsingDelegate(this.targetViewOrTween.Controller, this.attributeName, out parsingMethodData2))
			{
				this.targetParsingElement = this.targetViewOrTween.Controller;
				return;
			}
			if (parsingMethodData2.TryGetDelegateForSourceType(type, out this.targetParsingDelegate))
			{
				this.targetParsingElement = this.targetViewOrTween.Controller;
				return;
			}
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Failed mapping binding to parser on controller, no conversion possible. Binding ",
				this.attributeName,
				"=\"",
				this.sourceString,
				"\", binding result type ",
				type.FullName,
				", parser native type ",
				parsingMethodData2.NativeParseType.FullName,
				", view hierarchy: ",
				this.xuiHierarchy()
			}));
			this.expression = null;
			return;
		}
	}

	// Token: 0x06008997 RID: 35223 RVA: 0x00348510 File Offset: 0x00346710
	public void RefreshValue()
	{
		if ((this.singleIdentifierBinding && this.variables.Count == 0) || (this.expression == null && !this.singleIdentifierBinding))
		{
			return;
		}
		using (this.pmRefreshValueComplete.Auto())
		{
			using (this.pmRefreshValueCompleteSourceText.Auto())
			{
				bool flag = this.usesIndeterministicFunctions;
				using (this.pmRefreshBindings.Auto())
				{
					for (int i = 0; i < this.variables.Count; i++)
					{
						flag |= this.variables[i].RefreshValue();
					}
				}
				if (this.singleIdentifierBinding)
				{
					this.currentValue = this.variables[0].CurrentValue;
				}
				else
				{
					using (this.pmNcalcEval.Auto())
					{
						if (flag)
						{
							this.currentValue = this.evaluateExpression();
						}
					}
				}
				try
				{
					object obj = this.currentValue;
					string text = obj as string;
					if (text != null && text.Contains("{cvar("))
					{
						obj = BindingsManager.ReplaceCVars(text);
					}
					if (obj == null)
					{
						Log.Error(string.Concat(new string[]
						{
							"[XUi] Binding returned null value: ",
							this.attributeName,
							"=\"",
							this.sourceString,
							"\", view hierarchy: ",
							this.xuiHierarchy(),
							":"
						}));
					}
					else
					{
						using (this.pmApplyAttribute.Auto())
						{
							if (this.targetParsingDelegate == null)
							{
								((XUiController)this.targetParsingElement).CustomAttributes[this.attributeName] = obj;
							}
							else
							{
								this.targetParsingDelegate(this.targetParsingElement, obj);
							}
						}
					}
				}
				catch (Exception e)
				{
					string format = "[XUi] Exception parsing result of binding. Binding {0}=\"{1}\", binding result: '{2}' (type: {3}), view hierarchy: {4}:";
					object[] array = new object[5];
					array[0] = this.attributeName;
					array[1] = this.sourceString;
					array[2] = this.currentValue;
					int num = 3;
					object obj2 = this.currentValue;
					array[num] = ((obj2 != null) ? obj2.GetType().FullName : null);
					array[4] = this.xuiHierarchy();
					Log.Error(string.Format(format, array));
					Log.Exception(e);
				}
			}
		}
	}

	// Token: 0x06008998 RID: 35224 RVA: 0x003487F4 File Offset: 0x003469F4
	[PublicizedFrom(EAccessModifier.Private)]
	public object evaluateExpression()
	{
		object result;
		try
		{
			result = this.expression.Evaluate();
		}
		catch (Exception e)
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Binding expression can not be evaluated. Binding ",
				this.attributeName,
				"=\"",
				this.sourceString,
				"\" --- hierarchy: ",
				this.xuiHierarchy()
			}));
			Log.Exception(e);
			result = null;
		}
		return result;
	}

	// Token: 0x06008999 RID: 35225 RVA: 0x00348870 File Offset: 0x00346A70
	[PublicizedFrom(EAccessModifier.Private)]
	public void nCalcEvaluateParameter(string _name, ParameterArgs _args)
	{
		object result;
		if (this.FindParameter(_name, out result))
		{
			_args.Result = result;
		}
	}

	// Token: 0x0600899A RID: 35226 RVA: 0x0034888F File Offset: 0x00346A8F
	[PublicizedFrom(EAccessModifier.Private)]
	public void nCalcEvaluateFunction(string _name, FunctionArgs _args, bool _ignoreCase)
	{
		BindingNcalcFunctions.EvaluateFunc(this, _name, _args);
	}

	// Token: 0x0600899B RID: 35227 RVA: 0x0034889C File Offset: 0x00346A9C
	public bool FindParameter(string _name, out object _value)
	{
		if (this.expressionParamDict.TryGetValue(_name, out _value))
		{
			return true;
		}
		for (XUiController xuiController = this.targetViewOrTween.Controller; xuiController != null; xuiController = xuiController.Parent)
		{
			if (xuiController.GetType() != typeof(XUiController))
			{
				object obj = null;
				BindingInfoNcalc.VariableStateAbs variableStateAbs = null;
				XuiBindingDelegate xuiBindingDelegate;
				if (BindingMethodCache.Instance.TryGetBindingDelegate(xuiController, _name, out xuiBindingDelegate))
				{
					obj = xuiBindingDelegate(xuiController);
					variableStateAbs = new BindingInfoNcalc.VariableStateParamBinding(_name, xuiController, obj, xuiBindingDelegate, this.expressionParamDict);
				}
				else
				{
					string text = "";
					if (xuiController.GetBindingValue(ref text, _name))
					{
						variableStateAbs = new BindingInfoNcalc.VariableStateLegacyBinding(_name, xuiController, text, this.expressionParamDict);
						obj = text;
					}
				}
				if (variableStateAbs != null)
				{
					xuiController.Bindings.AddBinding(this);
					this.boundToControllers++;
					this.variables.Add(variableStateAbs);
					_value = obj;
					return true;
				}
			}
		}
		if (XUiFromXml.DebugXuiLoading == XUiFromXml.DebugLevel.Verbose)
		{
			Log.Warning("[XUi] Binding name '" + _name + "' not found! Hierarchy: " + this.xuiHierarchy());
		}
		return false;
	}

	// Token: 0x0600899C RID: 35228 RVA: 0x00348997 File Offset: 0x00346B97
	[PublicizedFrom(EAccessModifier.Private)]
	public string xuiHierarchy()
	{
		return this.targetViewOrTween.GetXuiHierarchy();
	}

	// Token: 0x17000FF3 RID: 4083
	// (get) Token: 0x0600899D RID: 35229 RVA: 0x003489A4 File Offset: 0x00346BA4
	public string SourceText
	{
		get
		{
			return this.sourceString;
		}
	}

	// Token: 0x17000FF4 RID: 4084
	// (get) Token: 0x0600899E RID: 35230 RVA: 0x003489AC File Offset: 0x00346BAC
	public IXUiElement TargetElement
	{
		get
		{
			return this.targetViewOrTween;
		}
	}

	// Token: 0x17000FF5 RID: 4085
	// (get) Token: 0x0600899F RID: 35231 RVA: 0x003489B4 File Offset: 0x00346BB4
	public bool IsInitializing { get; }

	// Token: 0x060089A0 RID: 35232 RVA: 0x003489BC File Offset: 0x00346BBC
	public void SetIndeterministic()
	{
		this.usesIndeterministicFunctions = true;
	}

	// Token: 0x060089A1 RID: 35233 RVA: 0x003489C5 File Offset: 0x00346BC5
	public bool RegisterVariable(BindingInfoNcalc.VariableStateAbs _var)
	{
		this.variables.Add(_var);
		return true;
	}

	// Token: 0x060089A3 RID: 35235 RVA: 0x00348A00 File Offset: 0x00346C00
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <.ctor>g__BindToController|16_0()
	{
		if (!this.usesIndeterministicFunctions && this.boundToControllers != 0)
		{
			return;
		}
		for (XUiController xuiController = this.targetViewOrTween.Controller; xuiController != null; xuiController = xuiController.Parent)
		{
			if (xuiController.GetType() != typeof(XUiController))
			{
				xuiController.Bindings.AddBinding(this);
				return;
			}
		}
	}

	// Token: 0x04006666 RID: 26214
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IXUiElement targetViewOrTween;

	// Token: 0x04006667 RID: 26215
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string attributeName;

	// Token: 0x04006668 RID: 26216
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string sourceString;

	// Token: 0x04006669 RID: 26217
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IXUiElement targetParsingElement;

	// Token: 0x0400666A RID: 26218
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XuiParsingDelegate targetParsingDelegate;

	// Token: 0x0400666B RID: 26219
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<BindingInfoNcalc.VariableStateAbs> variables = new List<BindingInfoNcalc.VariableStateAbs>();

	// Token: 0x0400666C RID: 26220
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, object> expressionParamDict = new Dictionary<string, object>(StringComparer.Ordinal);

	// Token: 0x0400666D RID: 26221
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Expression expression;

	// Token: 0x0400666E RID: 26222
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool singleIdentifierBinding;

	// Token: 0x0400666F RID: 26223
	[PublicizedFrom(EAccessModifier.Private)]
	public bool usesIndeterministicFunctions;

	// Token: 0x04006670 RID: 26224
	[PublicizedFrom(EAccessModifier.Private)]
	public int boundToControllers;

	// Token: 0x04006671 RID: 26225
	[PublicizedFrom(EAccessModifier.Private)]
	public object currentValue;

	// Token: 0x04006672 RID: 26226
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex fullBinding = new Regex("^\\{%.*\\}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	// Token: 0x04006673 RID: 26227
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex singleIdentifierExpression = new Regex("^\\s*([a-zA-Z]\\w*)\\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	// Token: 0x04006674 RID: 26228
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ProfilerMarker pmCtor = new ProfilerMarker("BindingInfoNcalc.ctor");

	// Token: 0x04006675 RID: 26229
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ProfilerMarker pmRefreshValueComplete = new ProfilerMarker("BindingInfoNcalc.RefreshValue");

	// Token: 0x04006676 RID: 26230
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ProfilerMarker pmRefreshValueCompleteSourceText;

	// Token: 0x04006677 RID: 26231
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ProfilerMarker pmRefreshBindings = new ProfilerMarker("Refresh Bindings");

	// Token: 0x04006678 RID: 26232
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ProfilerMarker pmNcalcEval = new ProfilerMarker("NCalc.Evaluate");

	// Token: 0x04006679 RID: 26233
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ProfilerMarker pmApplyAttribute = new ProfilerMarker("Apply Attribute");

	// Token: 0x020010E6 RID: 4326
	public abstract class VariableStateAbs
	{
		// Token: 0x17000FF6 RID: 4086
		// (get) Token: 0x060089A4 RID: 35236
		public abstract object CurrentValue { get; }

		// Token: 0x060089A5 RID: 35237
		public abstract bool RefreshValue();

		// Token: 0x060089A6 RID: 35238 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public VariableStateAbs()
		{
		}
	}

	// Token: 0x020010E7 RID: 4327
	public abstract class VariableStateBinding : BindingInfoNcalc.VariableStateAbs
	{
		// Token: 0x060089A7 RID: 35239 RVA: 0x00348A5A File Offset: 0x00346C5A
		[PublicizedFrom(EAccessModifier.Protected)]
		public VariableStateBinding(string _name, XUiController _controller, Dictionary<string, object> _expressionParamDict)
		{
			this.BindingName = _name;
			this.Controller = _controller;
			this.ExpressionParamDict = _expressionParamDict;
		}

		// Token: 0x0400667B RID: 26235
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly XUiController Controller;

		// Token: 0x0400667C RID: 26236
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly string BindingName;

		// Token: 0x0400667D RID: 26237
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly Dictionary<string, object> ExpressionParamDict;
	}

	// Token: 0x020010E8 RID: 4328
	public class VariableStateParamBinding : BindingInfoNcalc.VariableStateBinding
	{
		// Token: 0x060089A8 RID: 35240 RVA: 0x00348A77 File Offset: 0x00346C77
		public VariableStateParamBinding(string _name, XUiController _controller, object _initialValue, XuiBindingDelegate _bindingDelegate, Dictionary<string, object> _expressionParamDict) : base(_name, _controller, _expressionParamDict)
		{
			this.currentValue = _initialValue;
			this.bindingDelegate = _bindingDelegate;
			this.ExpressionParamDict[this.BindingName] = _initialValue;
		}

		// Token: 0x060089A9 RID: 35241 RVA: 0x00348AA4 File Offset: 0x00346CA4
		public override bool RefreshValue()
		{
			object obj = this.bindingDelegate(this.Controller);
			if (obj == null)
			{
				Log.Warning(string.Concat(new string[]
				{
					"[XUi] Binding '",
					this.BindingName,
					"' on controller '",
					this.Controller.GetType().FullName,
					"' returned null, should always return appropriate non-null value of same data type. Hierarchy: ",
					this.Controller.GetXuiHierarchy()
				}));
				return false;
			}
			if (obj.Equals(this.CurrentValue))
			{
				return false;
			}
			this.currentValue = obj;
			this.ExpressionParamDict[this.BindingName] = obj;
			return true;
		}

		// Token: 0x060089AA RID: 35242 RVA: 0x00348B44 File Offset: 0x00346D44
		public override string ToString()
		{
			return string.Format("ParamBinding: {0}='{1}'", this.BindingName, this.currentValue);
		}

		// Token: 0x17000FF7 RID: 4087
		// (get) Token: 0x060089AB RID: 35243 RVA: 0x00348B5C File Offset: 0x00346D5C
		public override object CurrentValue
		{
			get
			{
				return this.currentValue;
			}
		}

		// Token: 0x0400667E RID: 26238
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XuiBindingDelegate bindingDelegate;

		// Token: 0x0400667F RID: 26239
		[PublicizedFrom(EAccessModifier.Private)]
		public object currentValue;
	}

	// Token: 0x020010E9 RID: 4329
	public class VariableStateLegacyBinding : BindingInfoNcalc.VariableStateBinding
	{
		// Token: 0x060089AC RID: 35244 RVA: 0x00348B64 File Offset: 0x00346D64
		public VariableStateLegacyBinding(string _name, XUiController _controller, string _initialValue, Dictionary<string, object> _expressionParamDict) : base(_name, _controller, _expressionParamDict)
		{
			this.currentValue = _initialValue;
			this.ExpressionParamDict[this.BindingName] = _initialValue;
		}

		// Token: 0x060089AD RID: 35245 RVA: 0x00348B8C File Offset: 0x00346D8C
		public override bool RefreshValue()
		{
			string text = "";
			if (!this.Controller.GetBindingValue(ref text, this.BindingName))
			{
				Log.Error(string.Concat(new string[]
				{
					"[XUi] Refreshing binding failed: Controller's GetBindingValue no longer returns true! (Binding: ",
					this.BindingName,
					", hierarchy: ",
					this.Controller.GetXuiHierarchy(),
					")"
				}));
				return false;
			}
			if (string.Equals(this.currentValue, text, StringComparison.Ordinal))
			{
				return false;
			}
			this.currentValue = text;
			this.ExpressionParamDict[this.BindingName] = text;
			return true;
		}

		// Token: 0x060089AE RID: 35246 RVA: 0x00348C20 File Offset: 0x00346E20
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"LegacyBinding: ",
				this.BindingName,
				"='",
				this.currentValue,
				"'"
			});
		}

		// Token: 0x17000FF8 RID: 4088
		// (get) Token: 0x060089AF RID: 35247 RVA: 0x00348C57 File Offset: 0x00346E57
		public override object CurrentValue
		{
			get
			{
				return this.currentValue;
			}
		}

		// Token: 0x04006680 RID: 26240
		[PublicizedFrom(EAccessModifier.Private)]
		public string currentValue;
	}

	// Token: 0x020010EA RID: 4330
	public class VariableStateCVar : BindingInfoNcalc.VariableStateAbs
	{
		// Token: 0x060089B0 RID: 35248 RVA: 0x00348C5F File Offset: 0x00346E5F
		public VariableStateCVar(string _name)
		{
			this.cVarName = _name;
			this.currentValue = 0f;
		}

		// Token: 0x060089B1 RID: 35249 RVA: 0x00348C7C File Offset: 0x00346E7C
		public override bool RefreshValue()
		{
			GameManager instance = GameManager.Instance;
			if (instance == null)
			{
				return false;
			}
			World world = instance.World;
			EntityPlayer entityPlayer = (world != null) ? world.GetPrimaryPlayer() : null;
			if (entityPlayer == null)
			{
				return false;
			}
			float cvar = entityPlayer.GetCVar(this.cVarName);
			if (cvar.Equals(this.currentValue))
			{
				return false;
			}
			this.currentValue = cvar;
			return true;
		}

		// Token: 0x060089B2 RID: 35250 RVA: 0x00348CDE File Offset: 0x00346EDE
		public override string ToString()
		{
			return string.Format("cVar: {0}={1}", this.cVarName, this.currentValue);
		}

		// Token: 0x17000FF9 RID: 4089
		// (get) Token: 0x060089B3 RID: 35251 RVA: 0x00348CFB File Offset: 0x00346EFB
		public override object CurrentValue
		{
			get
			{
				return this.currentValue;
			}
		}

		// Token: 0x04006681 RID: 26241
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string cVarName;

		// Token: 0x04006682 RID: 26242
		[PublicizedFrom(EAccessModifier.Private)]
		public float currentValue;
	}

	// Token: 0x020010EB RID: 4331
	public abstract class VariableStateSimpleLookupAbs : BindingInfoNcalc.VariableStateAbs
	{
		// Token: 0x060089B4 RID: 35252
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract object getCurrentValue();

		// Token: 0x17000FFA RID: 4090
		// (get) Token: 0x060089B5 RID: 35253
		public abstract string VarType { [PublicizedFrom(EAccessModifier.Protected)] get; }

		// Token: 0x17000FFB RID: 4091
		// (get) Token: 0x060089B6 RID: 35254
		public abstract string VarName { [PublicizedFrom(EAccessModifier.Protected)] get; }

		// Token: 0x060089B7 RID: 35255 RVA: 0x00348D08 File Offset: 0x00346F08
		public override bool RefreshValue()
		{
			object obj = this.getCurrentValue();
			if (obj == null)
			{
				Log.Warning(string.Concat(new string[]
				{
					"[XUi] ",
					this.VarType,
					" '",
					this.VarName,
					"' returned null."
				}));
				return false;
			}
			if (obj.Equals(this.CurrentValue))
			{
				return false;
			}
			this.currentValue = obj;
			return true;
		}

		// Token: 0x060089B8 RID: 35256 RVA: 0x00348D73 File Offset: 0x00346F73
		public override string ToString()
		{
			return string.Format("{0}: {1}={2}", this.VarType, this.VarName, this.currentValue);
		}

		// Token: 0x17000FFC RID: 4092
		// (get) Token: 0x060089B9 RID: 35257 RVA: 0x00348D91 File Offset: 0x00346F91
		public override object CurrentValue
		{
			get
			{
				return this.currentValue;
			}
		}

		// Token: 0x060089BA RID: 35258 RVA: 0x00348D99 File Offset: 0x00346F99
		[PublicizedFrom(EAccessModifier.Protected)]
		public VariableStateSimpleLookupAbs()
		{
		}

		// Token: 0x04006683 RID: 26243
		[PublicizedFrom(EAccessModifier.Protected)]
		public object currentValue;
	}

	// Token: 0x020010EC RID: 4332
	public class VariableStateGamePref : BindingInfoNcalc.VariableStateSimpleLookupAbs
	{
		// Token: 0x060089BB RID: 35259 RVA: 0x00348DA1 File Offset: 0x00346FA1
		public VariableStateGamePref(EnumGamePrefs _pref)
		{
			this.pref = _pref;
			this.currentValue = this.getCurrentValue();
		}

		// Token: 0x060089BC RID: 35260 RVA: 0x00348DBC File Offset: 0x00346FBC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object getCurrentValue()
		{
			return GamePrefs.GetObject(this.pref);
		}

		// Token: 0x17000FFD RID: 4093
		// (get) Token: 0x060089BD RID: 35261 RVA: 0x00348DC9 File Offset: 0x00346FC9
		public override string VarType
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "GamePref";
			}
		}

		// Token: 0x17000FFE RID: 4094
		// (get) Token: 0x060089BE RID: 35262 RVA: 0x00348DD0 File Offset: 0x00346FD0
		public override string VarName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return this.pref.ToStringCached<EnumGamePrefs>();
			}
		}

		// Token: 0x04006684 RID: 26244
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EnumGamePrefs pref;
	}

	// Token: 0x020010ED RID: 4333
	public class VariableStateGameStat : BindingInfoNcalc.VariableStateSimpleLookupAbs
	{
		// Token: 0x060089BF RID: 35263 RVA: 0x00348DDD File Offset: 0x00346FDD
		public VariableStateGameStat(EnumGameStats _stat)
		{
			this.stat = _stat;
			this.currentValue = this.getCurrentValue();
		}

		// Token: 0x060089C0 RID: 35264 RVA: 0x00348DF8 File Offset: 0x00346FF8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object getCurrentValue()
		{
			return GameStats.GetObject(this.stat);
		}

		// Token: 0x17000FFF RID: 4095
		// (get) Token: 0x060089C1 RID: 35265 RVA: 0x00348E05 File Offset: 0x00347005
		public override string VarType
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "GameStat";
			}
		}

		// Token: 0x17001000 RID: 4096
		// (get) Token: 0x060089C2 RID: 35266 RVA: 0x00348E0C File Offset: 0x0034700C
		public override string VarName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return this.stat.ToStringCached<EnumGameStats>();
			}
		}

		// Token: 0x04006685 RID: 26245
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EnumGameStats stat;
	}

	// Token: 0x020010EE RID: 4334
	public class VariableStateGameInfoInt : BindingInfoNcalc.VariableStateSimpleLookupAbs
	{
		// Token: 0x060089C3 RID: 35267 RVA: 0x00348E19 File Offset: 0x00347019
		public VariableStateGameInfoInt(GameInfoInt _name)
		{
			this.name = _name;
			this.currentValue = this.getCurrentValue();
		}

		// Token: 0x060089C4 RID: 35268 RVA: 0x00348E34 File Offset: 0x00347034
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object getCurrentValue()
		{
			return (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo).GetValue(this.name);
		}

		// Token: 0x17001001 RID: 4097
		// (get) Token: 0x060089C5 RID: 35269 RVA: 0x00348E68 File Offset: 0x00347068
		public override string VarType
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "GameInfoInt";
			}
		}

		// Token: 0x17001002 RID: 4098
		// (get) Token: 0x060089C6 RID: 35270 RVA: 0x00348E6F File Offset: 0x0034706F
		public override string VarName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return this.name.ToStringCached<GameInfoInt>();
			}
		}

		// Token: 0x04006686 RID: 26246
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly GameInfoInt name;
	}

	// Token: 0x020010EF RID: 4335
	public class VariableStateGameInfoBool : BindingInfoNcalc.VariableStateSimpleLookupAbs
	{
		// Token: 0x060089C7 RID: 35271 RVA: 0x00348E7C File Offset: 0x0034707C
		public VariableStateGameInfoBool(GameInfoBool _name)
		{
			this.name = _name;
			this.currentValue = this.getCurrentValue();
		}

		// Token: 0x060089C8 RID: 35272 RVA: 0x00348E97 File Offset: 0x00347097
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object getCurrentValue()
		{
			return (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo).GetValue(this.name);
		}

		// Token: 0x17001003 RID: 4099
		// (get) Token: 0x060089C9 RID: 35273 RVA: 0x00348ECB File Offset: 0x003470CB
		public override string VarType
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "GameInfoBool";
			}
		}

		// Token: 0x17001004 RID: 4100
		// (get) Token: 0x060089CA RID: 35274 RVA: 0x00348ED2 File Offset: 0x003470D2
		public override string VarName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return this.name.ToStringCached<GameInfoBool>();
			}
		}

		// Token: 0x04006687 RID: 26247
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly GameInfoBool name;
	}

	// Token: 0x020010F0 RID: 4336
	public class VariableStateGameInfoString : BindingInfoNcalc.VariableStateSimpleLookupAbs
	{
		// Token: 0x060089CB RID: 35275 RVA: 0x00348EDF File Offset: 0x003470DF
		public VariableStateGameInfoString(GameInfoString _name)
		{
			this.name = _name;
			this.currentValue = this.getCurrentValue();
		}

		// Token: 0x060089CC RID: 35276 RVA: 0x00348EFA File Offset: 0x003470FA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object getCurrentValue()
		{
			return (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo).GetValue(this.name);
		}

		// Token: 0x17001005 RID: 4101
		// (get) Token: 0x060089CD RID: 35277 RVA: 0x00348F29 File Offset: 0x00347129
		public override string VarType
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "GameInfoString";
			}
		}

		// Token: 0x17001006 RID: 4102
		// (get) Token: 0x060089CE RID: 35278 RVA: 0x00348F30 File Offset: 0x00347130
		public override string VarName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return this.name.ToStringCached<GameInfoString>();
			}
		}

		// Token: 0x04006688 RID: 26248
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly GameInfoString name;
	}
}
