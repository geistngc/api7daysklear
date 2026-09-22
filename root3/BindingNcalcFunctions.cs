using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NCalc;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020010FE RID: 4350
[XuiBindingNcalcFunction]
[Preserve]
public static class BindingNcalcFunctions
{
	// Token: 0x060089FF RID: 35327 RVA: 0x00349F04 File Offset: 0x00348104
	[XuiBindingNcalcFunction("<ERROR>", 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void localization(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		_args.Result = Localization.Get(_evaluatedArguments[0].ToString(), false, null);
	}

	// Token: 0x06008A00 RID: 35328 RVA: 0x00349F1C File Offset: 0x0034811C
	[XuiBindingNcalcFunction(1f, 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void cvar(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		string text = _evaluatedArguments[0].ToString();
		if (!_bindingInstance.RegisterVariable(new BindingInfoNcalc.VariableStateCVar(text)))
		{
			_bindingInstance.SetIndeterministic();
		}
		if (GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			return;
		}
		_args.Result = primaryPlayer.GetCVar(text);
	}

	// Token: 0x06008A01 RID: 35329 RVA: 0x00349F8C File Offset: 0x0034818C
	[XuiBindingNcalcFunction(0, 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void gamepref(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		string text = _evaluatedArguments[0].ToString();
		if (string.IsNullOrEmpty(text))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a non-empty string.", _evaluatedArguments[0]), null, "gamepref");
			return;
		}
		EnumGamePrefs enumGamePrefs;
		if (!EnumUtils.TryParseIgnoreCase<EnumGamePrefs>(text, out enumGamePrefs))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a valid GamePref.", _evaluatedArguments[0]), null, "gamepref");
			return;
		}
		if (!_bindingInstance.RegisterVariable(new BindingInfoNcalc.VariableStateGamePref(enumGamePrefs)))
		{
			_bindingInstance.SetIndeterministic();
		}
		_args.Result = GamePrefs.GetObject(enumGamePrefs);
	}

	// Token: 0x06008A02 RID: 35330 RVA: 0x0034A008 File Offset: 0x00348208
	[XuiBindingNcalcFunction(0, 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void gamestat(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		string text = _evaluatedArguments[0].ToString();
		if (string.IsNullOrEmpty(text))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a non-empty string.", _evaluatedArguments[0]), null, "gamestat");
			return;
		}
		EnumGameStats enumGameStats;
		if (!EnumUtils.TryParseIgnoreCase<EnumGameStats>(text, out enumGameStats))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a valid GameStat.", _evaluatedArguments[0]), null, "gamestat");
			return;
		}
		if (!_bindingInstance.RegisterVariable(new BindingInfoNcalc.VariableStateGameStat(enumGameStats)))
		{
			_bindingInstance.SetIndeterministic();
		}
		_args.Result = GameStats.GetObject(enumGameStats);
	}

	// Token: 0x06008A03 RID: 35331 RVA: 0x0034A084 File Offset: 0x00348284
	[XuiBindingNcalcFunction(0, 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void serverinfoint(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		string text = _evaluatedArguments[0].ToString();
		if (string.IsNullOrEmpty(text))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a non-empty string.", _evaluatedArguments[0]), null, "serverinfoint");
			return;
		}
		GameInfoInt gameInfoInt;
		if (!EnumUtils.TryParseIgnoreCase<GameInfoInt>(text, out gameInfoInt))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a valid GameInfoInt.", _evaluatedArguments[0]), null, "serverinfoint");
			return;
		}
		if (!_bindingInstance.RegisterVariable(new BindingInfoNcalc.VariableStateGameInfoInt(gameInfoInt)))
		{
			_bindingInstance.SetIndeterministic();
		}
		GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
		if (gameServerInfo == null)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, "No game loaded.", null, "serverinfoint");
			return;
		}
		_args.Result = gameServerInfo.GetValue(gameInfoInt);
	}

	// Token: 0x06008A04 RID: 35332 RVA: 0x0034A140 File Offset: 0x00348340
	[XuiBindingNcalcFunction(false, 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void serverinfobool(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		string text = _evaluatedArguments[0].ToString();
		if (string.IsNullOrEmpty(text))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a non-empty string.", _evaluatedArguments[0]), null, "serverinfobool");
			return;
		}
		GameInfoBool gameInfoBool;
		if (!EnumUtils.TryParseIgnoreCase<GameInfoBool>(text, out gameInfoBool))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a valid GameInfoBool.", _evaluatedArguments[0]), null, "serverinfobool");
			return;
		}
		if (!_bindingInstance.RegisterVariable(new BindingInfoNcalc.VariableStateGameInfoBool(gameInfoBool)))
		{
			_bindingInstance.SetIndeterministic();
		}
		GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
		if (gameServerInfo == null)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, "No game loaded.", null, "serverinfobool");
			return;
		}
		_args.Result = gameServerInfo.GetValue(gameInfoBool);
	}

	// Token: 0x06008A05 RID: 35333 RVA: 0x0034A1FC File Offset: 0x003483FC
	[XuiBindingNcalcFunction("", 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void serverinfostring(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		string text = _evaluatedArguments[0].ToString();
		if (string.IsNullOrEmpty(text))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a non-empty string.", _evaluatedArguments[0]), null, "serverinfostring");
			return;
		}
		GameInfoString gameInfoString;
		if (!EnumUtils.TryParseIgnoreCase<GameInfoString>(text, out gameInfoString))
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' is not a valid GameInfoString.", _evaluatedArguments[0]), null, "serverinfostring");
			return;
		}
		if (!_bindingInstance.RegisterVariable(new BindingInfoNcalc.VariableStateGameInfoString(gameInfoString)))
		{
			_bindingInstance.SetIndeterministic();
		}
		GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
		if (gameServerInfo == null)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, "No game loaded.", null, "serverinfostring");
			return;
		}
		_args.Result = gameServerInfo.GetValue(gameInfoString);
	}

	// Token: 0x06008A06 RID: 35334 RVA: 0x0034A2B4 File Offset: 0x003484B4
	[XuiBindingNcalcFunction(null, 2, "dictvalue")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void dictValue(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		_bindingInstance.SetIndeterministic();
		IDictionary dictionary = _evaluatedArguments[0] as IDictionary;
		if (dictionary == null)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument 0 '{0}' does not evaluate to a dictionary.", _evaluatedArguments[0]), null, "dictValue");
			return;
		}
		object obj = _evaluatedArguments[1];
		if (obj == null)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, "Argument 1 is null.", null, "dictValue");
			return;
		}
		if (!dictionary.Contains(obj))
		{
			_args.Result = "";
			return;
		}
		_args.Result = dictionary[obj];
	}

	// Token: 0x06008A07 RID: 35335 RVA: 0x0034A328 File Offset: 0x00348528
	[XuiBindingNcalcFunction("", -1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void format(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		if (_args.Parameters.Length < 1)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Invalid number of arguments ({0}, expected at least 1).", _args.Parameters.Length), null, "format");
			return;
		}
		object obj = _args.Parameters[0].Evaluate();
		if (obj == null)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, "Can not evaluate argument.", null, "format");
			return;
		}
		object[] objectArray = BindingNcalcFunctions.GetObjectArray(_args.Parameters.Length - 1);
		for (int i = 1; i < _args.Parameters.Length; i++)
		{
			objectArray[i - 1] = _args.Parameters[i].Evaluate();
		}
		string result = string.Format(obj.ToString(), objectArray);
		BindingNcalcFunctions.ReturnObjectArray(objectArray);
		_args.Result = result;
	}

	// Token: 0x06008A08 RID: 35336 RVA: 0x0034A3D6 File Offset: 0x003485D6
	[XuiBindingNcalcFunction(0, 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void length(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		_args.Result = _evaluatedArguments[0].ToString().Length;
	}

	// Token: 0x06008A09 RID: 35337 RVA: 0x0034A3F0 File Offset: 0x003485F0
	[XuiBindingNcalcFunction(0, 1, "int")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void toInt(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = Convert.ToInt32(_evaluatedArguments[0]);
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to an integer.", _evaluatedArguments[0]), e, "toInt");
		}
	}

	// Token: 0x06008A0A RID: 35338 RVA: 0x0034A440 File Offset: 0x00348640
	[XuiBindingNcalcFunction(0, 1, "roundtoint")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void roundToInt(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		float f;
		if (!BindingNcalcFunctions.tryArgAsFloat(_bindingInstance, _evaluatedArguments, 0, "v", out f, "roundToInt"))
		{
			return;
		}
		_args.Result = Mathf.RoundToInt(f);
	}

	// Token: 0x06008A0B RID: 35339 RVA: 0x0034A478 File Offset: 0x00348678
	[XuiBindingNcalcFunction(0.0, 1, "float")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void toFloat(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = Convert.ToDouble(_evaluatedArguments[0]);
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a number.", _evaluatedArguments[0]), e, "toFloat");
		}
	}

	// Token: 0x06008A0C RID: 35340 RVA: 0x0034A4C8 File Offset: 0x003486C8
	[XuiBindingNcalcFunction("", 1, "str")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void toString(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		object obj = _evaluatedArguments[0];
		_args.Result = (((obj != null) ? obj.ToString() : null) ?? "");
	}

	// Token: 0x06008A0D RID: 35341 RVA: 0x0034A4E8 File Offset: 0x003486E8
	[XuiBindingNcalcFunction("", 2, "itos")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void intToString(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		int num;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 0, "i", out num, "intToString"))
		{
			return;
		}
		_args.Result = num.ToString(_evaluatedArguments[1].ToString());
	}

	// Token: 0x06008A0E RID: 35342 RVA: 0x0034A524 File Offset: 0x00348724
	[XuiBindingNcalcFunction("", 2, "ftos")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void floatToString(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		float num;
		if (!BindingNcalcFunctions.tryArgAsFloat(_bindingInstance, _evaluatedArguments, 0, "f", out num, "floatToString"))
		{
			return;
		}
		_args.Result = num.ToString(_evaluatedArguments[1].ToString());
	}

	// Token: 0x06008A0F RID: 35343 RVA: 0x0034A560 File Offset: 0x00348760
	[XuiBindingNcalcFunction(false, 1, "bound")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void isBound(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06008A10 RID: 35344 RVA: 0x0034A572 File Offset: 0x00348772
	[XuiBindingNcalcFunction("", 1, null)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void always(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		_bindingInstance.SetIndeterministic();
		_args.Result = _evaluatedArguments[0];
	}

	// Token: 0x06008A11 RID: 35345 RVA: 0x0034A584 File Offset: 0x00348784
	[XuiBindingNcalcFunction(false, 1, "defined")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void isDefined(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		object obj;
		_args.Result = (_bindingInstance.FindParameter(_evaluatedArguments[0].ToString(), out obj) ? BindingNcalcFunctions.boxedTrue : BindingNcalcFunctions.boxedFalse);
	}

	// Token: 0x06008A12 RID: 35346 RVA: 0x0034A5B8 File Offset: 0x003487B8
	[XuiBindingNcalcFunction(0, 2, "c_i")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void color32_i(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		int num;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 1, "i", out num, "color32_i"))
		{
			return;
		}
		if (num < 0 || num > 3)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' outside allowed range (0-3).", _evaluatedArguments[1]), null, "color32_i");
		}
		try
		{
			_args.Result = ((Color32)_evaluatedArguments[0])[num];
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Color.", _evaluatedArguments[0]), e, "color32_i");
		}
	}

	// Token: 0x06008A13 RID: 35347 RVA: 0x0034A648 File Offset: 0x00348848
	[XuiBindingNcalcFunction(0, 1, "c_r")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void color32_r(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Color32)_evaluatedArguments[0]).r;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Color.", _evaluatedArguments[0]), e, "color32_r");
		}
	}

	// Token: 0x06008A14 RID: 35348 RVA: 0x0034A69C File Offset: 0x0034889C
	[XuiBindingNcalcFunction(0, 1, "c_g")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void color32_g(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Color32)_evaluatedArguments[0]).g;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Color.", _evaluatedArguments[0]), e, "color32_g");
		}
	}

	// Token: 0x06008A15 RID: 35349 RVA: 0x0034A6F0 File Offset: 0x003488F0
	[XuiBindingNcalcFunction(0, 1, "c_b")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void color32_b(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Color32)_evaluatedArguments[0]).b;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Color.", _evaluatedArguments[0]), e, "color32_b");
		}
	}

	// Token: 0x06008A16 RID: 35350 RVA: 0x0034A744 File Offset: 0x00348944
	[XuiBindingNcalcFunction(0, 1, "c_a")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void color32_a(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Color32)_evaluatedArguments[0]).a;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Color.", _evaluatedArguments[0]), e, "color32_a");
		}
	}

	// Token: 0x06008A17 RID: 35351 RVA: 0x0034A798 File Offset: 0x00348998
	[XuiBindingNcalcFunction(null, 4, "color")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void color32(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		byte r;
		if (!BindingNcalcFunctions.tryArgAsByteClamped(_bindingInstance, _evaluatedArguments, 0, "r", out r, "color32"))
		{
			return;
		}
		byte g;
		if (!BindingNcalcFunctions.tryArgAsByteClamped(_bindingInstance, _evaluatedArguments, 1, "g", out g, "color32"))
		{
			return;
		}
		byte b;
		if (!BindingNcalcFunctions.tryArgAsByteClamped(_bindingInstance, _evaluatedArguments, 2, "b", out b, "color32"))
		{
			return;
		}
		byte a;
		if (!BindingNcalcFunctions.tryArgAsByteClamped(_bindingInstance, _evaluatedArguments, 3, "a", out a, "color32"))
		{
			return;
		}
		_args.Result = new Color32(r, g, b, a);
	}

	// Token: 0x06008A18 RID: 35352 RVA: 0x0034A818 File Offset: 0x00348A18
	[XuiBindingNcalcFunction(0, 2, "v2i_i")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void v2i_i(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		int num;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 1, "i", out num, "v2i_i"))
		{
			return;
		}
		if (num < 0 || num > 1)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' outside allowed range (0-1).", _evaluatedArguments[1]), null, "v2i_i");
		}
		try
		{
			_args.Result = ((Vector2i)_evaluatedArguments[0])[num];
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Vector2i.", _evaluatedArguments[0]), e, "v2i_i");
		}
	}

	// Token: 0x06008A19 RID: 35353 RVA: 0x0034A8A8 File Offset: 0x00348AA8
	[XuiBindingNcalcFunction(0, 1, "v2i_x")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void v2i_x(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Vector2i)_evaluatedArguments[0]).x;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Vector2i.", _evaluatedArguments[0]), e, "v2i_x");
		}
	}

	// Token: 0x06008A1A RID: 35354 RVA: 0x0034A8FC File Offset: 0x00348AFC
	[XuiBindingNcalcFunction(0, 1, "v2i_y")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void v2i_y(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Vector2i)_evaluatedArguments[0]).y;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Vector2i.", _evaluatedArguments[0]), e, "v2i_y");
		}
	}

	// Token: 0x06008A1B RID: 35355 RVA: 0x0034A950 File Offset: 0x00348B50
	[XuiBindingNcalcFunction(null, 2, "v2i")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void vector2i(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		int x;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 0, "x", out x, "vector2i"))
		{
			return;
		}
		int y;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 1, "y", out y, "vector2i"))
		{
			return;
		}
		_args.Result = new Vector2i(x, y);
	}

	// Token: 0x06008A1C RID: 35356 RVA: 0x0034A9A0 File Offset: 0x00348BA0
	[XuiBindingNcalcFunction(0, 2, "v3i_i")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void v3i_i(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		int num;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 1, "i", out num, "v3i_i"))
		{
			return;
		}
		if (num < 0 || num > 2)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' outside allowed range (0-2).", _evaluatedArguments[1]), null, "v3i_i");
		}
		try
		{
			_args.Result = ((Vector2i)_evaluatedArguments[0])[num];
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Vector3i.", _evaluatedArguments[0]), e, "v3i_i");
		}
	}

	// Token: 0x06008A1D RID: 35357 RVA: 0x0034AA30 File Offset: 0x00348C30
	[XuiBindingNcalcFunction(0, 1, "v3i_x")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void v3i_x(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Vector3i)_evaluatedArguments[0]).x;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Vector3i.", _evaluatedArguments[0]), e, "v3i_x");
		}
	}

	// Token: 0x06008A1E RID: 35358 RVA: 0x0034AA84 File Offset: 0x00348C84
	[XuiBindingNcalcFunction(0, 1, "v3i_y")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void v3i_y(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Vector3i)_evaluatedArguments[0]).y;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Vector3i.", _evaluatedArguments[0]), e, "v3i_y");
		}
	}

	// Token: 0x06008A1F RID: 35359 RVA: 0x0034AAD8 File Offset: 0x00348CD8
	[XuiBindingNcalcFunction(0, 1, "v3i_z")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void v3i_z(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		try
		{
			_args.Result = ((Vector3i)_evaluatedArguments[0]).z;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument '{0}' does not evaluate to a Vector3i.", _evaluatedArguments[0]), e, "v3i_z");
		}
	}

	// Token: 0x06008A20 RID: 35360 RVA: 0x0034AB2C File Offset: 0x00348D2C
	[XuiBindingNcalcFunction(null, 3, "v3i")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void vector3i(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		int x;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 0, "x", out x, "vector3i"))
		{
			return;
		}
		int y;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 1, "y", out y, "vector3i"))
		{
			return;
		}
		int z;
		if (!BindingNcalcFunctions.tryArgAsInt(_bindingInstance, _evaluatedArguments, 2, "z", out z, "vector3i"))
		{
			return;
		}
		_args.Result = new Vector3i(x, y, z);
	}

	// Token: 0x06008A21 RID: 35361 RVA: 0x0034AB94 File Offset: 0x00348D94
	[XuiBindingNcalcFunction(null, 2, "v2")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void vector2(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		float x;
		if (!BindingNcalcFunctions.tryArgAsFloat(_bindingInstance, _evaluatedArguments, 0, "x", out x, "vector2"))
		{
			return;
		}
		float y;
		if (!BindingNcalcFunctions.tryArgAsFloat(_bindingInstance, _evaluatedArguments, 1, "y", out y, "vector2"))
		{
			return;
		}
		_args.Result = new Vector2(x, y);
	}

	// Token: 0x06008A22 RID: 35362 RVA: 0x0034ABE4 File Offset: 0x00348DE4
	[XuiBindingNcalcFunction(null, 3, "v3")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void vector3(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments)
	{
		float x;
		if (!BindingNcalcFunctions.tryArgAsFloat(_bindingInstance, _evaluatedArguments, 0, "x", out x, "vector3"))
		{
			return;
		}
		float y;
		if (!BindingNcalcFunctions.tryArgAsFloat(_bindingInstance, _evaluatedArguments, 1, "y", out y, "vector3"))
		{
			return;
		}
		float z;
		if (!BindingNcalcFunctions.tryArgAsFloat(_bindingInstance, _evaluatedArguments, 2, "z", out z, "vector3"))
		{
			return;
		}
		_args.Result = new Vector3(x, y, z);
	}

	// Token: 0x06008A23 RID: 35363 RVA: 0x0034AC4C File Offset: 0x00348E4C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool tryArgAsByteClamped(IBindingNcalc _bindingInstance, object[] _evaluatedArguments, int _argIndex, string _argName, out byte _value, [CallerMemberName] string _funcName = null)
	{
		object obj = _evaluatedArguments[_argIndex];
		bool result;
		try
		{
			_value = (byte)Mathf.Clamp((int)obj, 0, 255);
			result = true;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument {0}='{1}' (type {2}) does not evaluate to an integer.", _argName, obj, obj.GetType().FullName), e, _funcName);
			_value = 0;
			result = false;
		}
		return result;
	}

	// Token: 0x06008A24 RID: 35364 RVA: 0x0034ACB0 File Offset: 0x00348EB0
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool tryArgAsInt(IBindingNcalc _bindingInstance, object[] _evaluatedArguments, int _argIndex, string _argName, out int _value, [CallerMemberName] string _funcName = null)
	{
		object obj = _evaluatedArguments[_argIndex];
		bool result;
		try
		{
			_value = Convert.ToInt32(obj);
			result = true;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument {0}='{1}' (type {2}) does not evaluate to an integer.", _argName, obj, obj.GetType().FullName), e, _funcName);
			_value = 0;
			result = false;
		}
		return result;
	}

	// Token: 0x06008A25 RID: 35365 RVA: 0x0034AD08 File Offset: 0x00348F08
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool tryArgAsFloat(IBindingNcalc _bindingInstance, object[] _evaluatedArguments, int _argIndex, string _argName, out float _value, [CallerMemberName] string _funcName = null)
	{
		object obj = _evaluatedArguments[_argIndex];
		bool result;
		try
		{
			_value = Convert.ToSingle(obj);
			result = true;
		}
		catch (Exception e)
		{
			BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Argument {0}='{1}' (type {2}) does not evaluate to a number.", _argName, obj, obj.GetType().FullName), e, _funcName);
			_value = 0f;
			result = false;
		}
		return result;
	}

	// Token: 0x06008A26 RID: 35366 RVA: 0x0034AD64 File Offset: 0x00348F64
	public static void LogFunctionError(IBindingNcalc _bindingInstance, string _message, Exception _e = null, [CallerMemberName] string _funcName = null)
	{
		Log.Error(string.Concat(new string[]
		{
			"[XUi] Binding expression calling function '",
			_funcName,
			"': ",
			_message,
			" Binding expression: '",
			_bindingInstance.SourceText,
			"' --- hierarchy: ",
			_bindingInstance.TargetElement.GetXuiHierarchy()
		}));
		if (_e != null)
		{
			Log.Exception(_e);
		}
	}

	// Token: 0x06008A27 RID: 35367 RVA: 0x0034ADCC File Offset: 0x00348FCC
	public static void EvaluateFunc(IBindingNcalc _bindingInstance, string _name, FunctionArgs _args)
	{
		BindingNcalcFunctions.FunctionDefinition functionDefinition;
		if (!BindingNcalcFunctions.ncalcFunctions.TryGetValue(_name, out functionDefinition))
		{
			return;
		}
		object[] array = null;
		_args.Result = null;
		if (functionDefinition.ExpectedArgumentCount >= 0)
		{
			if (_args.Parameters.Length != functionDefinition.ExpectedArgumentCount)
			{
				BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Invalid number of arguments ({0}, expected {1}).", _args.Parameters.Length, functionDefinition.ExpectedArgumentCount), null, _name);
				if (functionDefinition.ErrorResult != null)
				{
					_args.Result = functionDefinition.ErrorResult;
				}
				return;
			}
			if (_args.Parameters.Length != 0)
			{
				array = BindingNcalcFunctions.GetObjectArray(_args.Parameters.Length);
				for (int i = 0; i < _args.Parameters.Length; i++)
				{
					object obj = _args.Parameters[i].Evaluate();
					if (obj == null)
					{
						BindingNcalcFunctions.LogFunctionError(_bindingInstance, string.Format("Can not evaluate argument at index {0}.", i), null, _name);
						if (functionDefinition.ErrorResult != null)
						{
							_args.Result = functionDefinition.ErrorResult;
						}
						return;
					}
					array[i] = obj;
				}
			}
		}
		functionDefinition.Delegate(_bindingInstance, _args, array);
		if (_args.Result == null && functionDefinition.ErrorResult != null)
		{
			_args.Result = functionDefinition.ErrorResult;
		}
		BindingNcalcFunctions.ReturnObjectArray(array);
	}

	// Token: 0x06008A28 RID: 35368 RVA: 0x0034AEE9 File Offset: 0x003490E9
	[PublicizedFrom(EAccessModifier.Private)]
	public static object[] GetObjectArray(int _length)
	{
		if (_length > 20)
		{
			return new object[_length];
		}
		if (BindingNcalcFunctions.objectArrays.Count == 0)
		{
			return new object[_length];
		}
		return BindingNcalcFunctions.objectArrays.Pop();
	}

	// Token: 0x06008A29 RID: 35369 RVA: 0x0034AF14 File Offset: 0x00349114
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ReturnObjectArray(object[] _arr)
	{
		if (_arr != null && _arr.Length == 20)
		{
			BindingNcalcFunctions.objectArrays.Push(_arr);
		}
	}

	// Token: 0x06008A2A RID: 35370 RVA: 0x0034AF2C File Offset: 0x0034912C
	public static void RegisterNcalcFunctions()
	{
		if (BindingNcalcFunctions.ncalcFunctions.Count > 0)
		{
			return;
		}
		for (int i = 0; i < 10; i++)
		{
			BindingNcalcFunctions.objectArrays.Push(new object[20]);
		}
		ReflectionHelpers.FindTypesWithAttribute<XuiBindingNcalcFunctionAttribute>(new Action<Type, bool, XuiBindingNcalcFunctionAttribute>(BindingNcalcFunctions.<RegisterNcalcFunctions>g__TypeFoundCallback|50_0), true);
	}

	// Token: 0x06008A2C RID: 35372 RVA: 0x0034AFA3 File Offset: 0x003491A3
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <RegisterNcalcFunctions>g__TypeFoundCallback|50_0(Type _type, bool _hasMultiple, XuiBindingNcalcFunctionAttribute _classAttribute)
	{
		ReflectionHelpers.GetMethodsWithAttribute<XuiBindingNcalcFunctionAttribute>(_type, new Action<MethodInfo, bool, XuiBindingNcalcFunctionAttribute>(BindingNcalcFunctions.<RegisterNcalcFunctions>g__MethodFoundCallback|50_1), true, true, true);
	}

	// Token: 0x06008A2D RID: 35373 RVA: 0x0034AFBC File Offset: 0x003491BC
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <RegisterNcalcFunctions>g__MethodFoundCallback|50_1(MethodInfo _method, bool _hasMultiple, XuiBindingNcalcFunctionAttribute _methodAttribute)
	{
		if (!ReflectionHelpers.MethodCompatibleWithDelegate<BindingNcalcFunctions.CustomNcalcFunctionDelegate>(_method, true))
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Binding NCalc function method ",
				_method.DeclaringType.FullName,
				".",
				_method.Name,
				" not compatible with delegate signature"
			}));
			return;
		}
		string text = _methodAttribute.FunctionName ?? _method.Name.ToLowerInvariant();
		BindingNcalcFunctions.CustomNcalcFunctionDelegate @delegate = (BindingNcalcFunctions.CustomNcalcFunctionDelegate)_method.CreateDelegate(typeof(BindingNcalcFunctions.CustomNcalcFunctionDelegate));
		if (BindingNcalcFunctions.ncalcFunctions.ContainsKey(text))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] Binding NCalc function method ",
				_method.DeclaringType.FullName,
				".",
				_method.Name,
				" overriding previously defined method for NCalc function '",
				text,
				"'"
			}));
		}
		BindingNcalcFunctions.ncalcFunctions[text] = new BindingNcalcFunctions.FunctionDefinition(@delegate, _methodAttribute.ExpectedArgumentCount, _methodAttribute.ErrorResult);
	}

	// Token: 0x040066AA RID: 26282
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object boxedFalse = false;

	// Token: 0x040066AB RID: 26283
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object boxedTrue = true;

	// Token: 0x040066AC RID: 26284
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ObjectArrayLengthCached = 20;

	// Token: 0x040066AD RID: 26285
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Stack<object[]> objectArrays = new Stack<object[]>();

	// Token: 0x040066AE RID: 26286
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, BindingNcalcFunctions.FunctionDefinition> ncalcFunctions = new Dictionary<string, BindingNcalcFunctions.FunctionDefinition>();

	// Token: 0x020010FF RID: 4351
	// (Invoke) Token: 0x06008A2F RID: 35375
	public delegate void CustomNcalcFunctionDelegate(IBindingNcalc _bindingInstance, FunctionArgs _args, object[] _evaluatedArguments);

	// Token: 0x02001100 RID: 4352
	[PublicizedFrom(EAccessModifier.Private)]
	public class FunctionDefinition
	{
		// Token: 0x06008A32 RID: 35378 RVA: 0x0034B0B3 File Offset: 0x003492B3
		public FunctionDefinition(BindingNcalcFunctions.CustomNcalcFunctionDelegate _delegate, int _expectedArgumentCount, object _errorResult)
		{
			this.Delegate = _delegate;
			this.ExpectedArgumentCount = _expectedArgumentCount;
			this.ErrorResult = _errorResult;
		}

		// Token: 0x040066AF RID: 26287
		public readonly BindingNcalcFunctions.CustomNcalcFunctionDelegate Delegate;

		// Token: 0x040066B0 RID: 26288
		public readonly int ExpectedArgumentCount;

		// Token: 0x040066B1 RID: 26289
		public readonly object ErrorResult;
	}
}
