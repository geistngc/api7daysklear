using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000256 RID: 598
[Preserve]
public class ConsoleCmdPermissionsAllowed : ConsoleCmdAbstract
{
	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x060011CD RID: 4557 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x060011CE RID: 4558 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0006FBC7 File Offset: 0x0006DDC7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"permissionsallowed",
			"pallowed",
			"pa"
		};
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x0006FBE7 File Offset: 0x0006DDE7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Apply a mask to permissions for testing purposes (respects the existing conditions though).";
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0006FBF0 File Offset: 0x0006DDF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		string str = string.Join<EUserPerms>("|", EnumUtils.Values<EUserPerms>());
		return string.Join('\n', new string[]
		{
			"pa i[nfo] - Prints info about the current permissions.",
			"pa g[rant] <" + str + "> - Adds the given permissions to the current debug permissions mask (still respects existing permissions though).",
			"pa rev[oke] <" + str + "> - Removes the given permissions from the current debug permissions mask.",
			"pa res[olve] <" + str + "> <true|false> - Attempt to resolve the specified permissions. True allows prompting the user for input, otherwise it is a silent resolution."
		});
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0006FC5C File Offset: 0x0006DE5C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		string[] array = _params.ToArray();
		if (_params.Count > 0)
		{
			string text = _params[0].ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			ConsoleCmdPermissionsAllowed.ExecuteSubCommand executeSubCommand;
			if (num <= 2935168099U)
			{
				if (num <= 804546073U)
				{
					if (num != 263456517U)
					{
						if (num != 804546073U)
						{
							goto IL_152;
						}
						if (!(text == "res"))
						{
							goto IL_152;
						}
					}
					else
					{
						if (!(text == "info"))
						{
							goto IL_152;
						}
						goto IL_124;
					}
				}
				else if (num != 854878930U)
				{
					if (num != 2935168099U)
					{
						goto IL_152;
					}
					if (!(text == "resolve"))
					{
						goto IL_152;
					}
				}
				else
				{
					if (!(text == "rev"))
					{
						goto IL_152;
					}
					goto IL_13B;
				}
				executeSubCommand = new ConsoleCmdPermissionsAllowed.ExecuteSubCommand(this.ExecuteResolve);
				goto IL_154;
			}
			if (num <= 3691399731U)
			{
				if (num != 2975164269U)
				{
					if (num != 3691399731U)
					{
						goto IL_152;
					}
					if (!(text == "revoke"))
					{
						goto IL_152;
					}
					goto IL_13B;
				}
				else if (!(text == "grant"))
				{
					goto IL_152;
				}
			}
			else if (num != 3792446982U)
			{
				if (num != 3960223172U)
				{
					goto IL_152;
				}
				if (!(text == "i"))
				{
					goto IL_152;
				}
				goto IL_124;
			}
			else if (!(text == "g"))
			{
				goto IL_152;
			}
			executeSubCommand = ConsoleCmdPermissionsAllowed.ExecuteGrant;
			goto IL_154;
			IL_124:
			executeSubCommand = new ConsoleCmdPermissionsAllowed.ExecuteSubCommand(ConsoleCmdPermissionsAllowed.ExecuteInfo);
			goto IL_154;
			IL_13B:
			executeSubCommand = ConsoleCmdPermissionsAllowed.ExecuteRevoke;
			goto IL_154;
			IL_152:
			executeSubCommand = null;
			IL_154:
			ConsoleCmdPermissionsAllowed.ExecuteSubCommand executeSubCommand2 = executeSubCommand;
			if (executeSubCommand2 == null)
			{
				Log.Warning("Unknown sub-command: " + _params[0]);
			}
			else if (executeSubCommand2(RuntimeHelpers.GetSubArray<string>(array, Range.StartAt(1))))
			{
				return;
			}
		}
		Log.Warning(this.GetHelp());
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0006FE04 File Offset: 0x0006E004
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ExecuteInfo(ReadOnlySpan<string> parameters)
	{
		EUserPerms permissions = PlatformManager.NativePlatform.User.Permissions;
		IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
		EUserPerms? euserPerms;
		if (crossplatformPlatform == null)
		{
			euserPerms = null;
		}
		else
		{
			IUserClient user = crossplatformPlatform.User;
			euserPerms = ((user != null) ? new EUserPerms?(user.Permissions) : null);
		}
		EUserPerms? euserPerms2 = euserPerms;
		StringBuilder stringBuilder = new StringBuilder(string.Format("User Native: {0}\nUser Cross: {1}", permissions, ((euserPerms2 != null) ? euserPerms2.GetValueOrDefault().ToString() : null) ?? "N/A"));
		foreach (object obj in Enum.GetValues(typeof(PermissionsManager.PermissionSources)))
		{
			PermissionsManager.PermissionSources permissionSources = (PermissionsManager.PermissionSources)obj;
			stringBuilder.Append(string.Format("\n{0}: {1}", permissionSources, PermissionsManager.GetPermissions(permissionSources)));
		}
		Log.Out(stringBuilder.ToString());
		return true;
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x0006FF1C File Offset: 0x0006E11C
	[PublicizedFrom(EAccessModifier.Private)]
	public static EUserPerms MaskModifierGrant(EUserPerms previous, EUserPerms input)
	{
		return previous | input;
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x0006FF21 File Offset: 0x0006E121
	[PublicizedFrom(EAccessModifier.Private)]
	public static EUserPerms MaskModifierRevoke(EUserPerms previous, EUserPerms input)
	{
		return previous & ~input;
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x0006FF27 File Offset: 0x0006E127
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConsoleCmdPermissionsAllowed.ExecuteSubCommand CreateExecuteMaskModifier(ConsoleCmdPermissionsAllowed.MaskModifier modifier)
	{
		ConsoleCmdPermissionsAllowed.<>c__DisplayClass16_0 CS$<>8__locals1 = new ConsoleCmdPermissionsAllowed.<>c__DisplayClass16_0();
		CS$<>8__locals1.modifier = modifier;
		return new ConsoleCmdPermissionsAllowed.ExecuteSubCommand(CS$<>8__locals1.<CreateExecuteMaskModifier>g__ExecuteMaskModifier|0);
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0006FF40 File Offset: 0x0006E140
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe bool ExecuteResolve(ReadOnlySpan<string> parameters)
	{
		if (parameters.Length != 2)
		{
			Log.Warning("Expected a two arguments.");
			return false;
		}
		EUserPerms permissionsToResolve;
		if (!ConsoleCmdPermissionsAllowed.TryParsePermission(*parameters[0], out permissionsToResolve))
		{
			return false;
		}
		bool shouldPrompt;
		if (!ConsoleCmdPermissionsAllowed.TryParseBoolean(*parameters[1], out shouldPrompt))
		{
			return false;
		}
		ThreadManager.StartCoroutine(this.ResolveCoroutine(permissionsToResolve, shouldPrompt));
		return true;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x0006FF9A File Offset: 0x0006E19A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ResolveCoroutine(EUserPerms permissionsToResolve, bool shouldPrompt)
	{
		if (this.m_resolveCoroutineRunning)
		{
			Log.Warning("[Perms Resolve] Resolving in progress already.");
			yield break;
		}
		yield return null;
		try
		{
			this.m_resolveCoroutineRunning = true;
			Log.Out(string.Format("[Perms Resolve] Resolving Permissions '{0}'.", permissionsToResolve));
			yield return PermissionsManager.ResolvePermissions(permissionsToResolve, shouldPrompt, null);
		}
		finally
		{
			this.m_resolveCoroutineRunning = false;
		}
		yield break;
		yield break;
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x0006FFB8 File Offset: 0x0006E1B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool TryParseBoolean(string input, out bool result)
	{
		string text = input.ToLowerInvariant();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1303515621U)
		{
			if (num <= 873244444U)
			{
				if (num != 184981848U)
				{
					if (num != 873244444U)
					{
						goto IL_102;
					}
					if (!(text == "1"))
					{
						goto IL_102;
					}
				}
				else
				{
					if (!(text == "false"))
					{
						goto IL_102;
					}
					goto IL_FD;
				}
			}
			else if (num != 890022063U)
			{
				if (num != 1303515621U)
				{
					goto IL_102;
				}
				if (!(text == "true"))
				{
					goto IL_102;
				}
			}
			else
			{
				if (!(text == "0"))
				{
					goto IL_102;
				}
				goto IL_FD;
			}
		}
		else if (num <= 2872740362U)
		{
			if (num != 1630810064U)
			{
				if (num != 2872740362U)
				{
					goto IL_102;
				}
				if (!(text == "off"))
				{
					goto IL_102;
				}
				goto IL_FD;
			}
			else if (!(text == "on"))
			{
				goto IL_102;
			}
		}
		else if (num != 3809224601U)
		{
			if (num != 4044111267U)
			{
				goto IL_102;
			}
			if (!(text == "t"))
			{
				goto IL_102;
			}
		}
		else
		{
			if (!(text == "f"))
			{
				goto IL_102;
			}
			goto IL_FD;
		}
		result = true;
		return true;
		IL_FD:
		result = false;
		return true;
		IL_102:
		Log.Warning("Expected true/false instead of: " + input);
		result = false;
		return false;
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x000700DC File Offset: 0x0006E2DC
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool TryParsePermission(string input, out EUserPerms result)
	{
		if (EnumUtils.TryParse<EUserPerms>(input, out result, true))
		{
			return true;
		}
		EUserPerms? euserPerms = null;
		foreach (ValueTuple<EUserPerms, string> valueTuple in EnumUtils.Values<EUserPerms>().Zip(EnumUtils.Names<EUserPerms>(), (EUserPerms perm, string name) => new ValueTuple<EUserPerms, string>(perm, name)))
		{
			EUserPerms item = valueTuple.Item1;
			if (valueTuple.Item2.StartsWith(input, StringComparison.OrdinalIgnoreCase))
			{
				if (euserPerms != null)
				{
					Log.Warning(string.Format("Input '{0}' is ambiguous between '{1}' and '{2}'.", input, euserPerms.Value, item));
					result = (EUserPerms)0;
					return false;
				}
				euserPerms = new EUserPerms?(item);
			}
		}
		if (euserPerms == null)
		{
			Log.Warning("Input '" + input + "' did not match any permissions.");
			result = (EUserPerms)0;
			return false;
		}
		result = euserPerms.Value;
		return true;
	}

	// Token: 0x04000CFA RID: 3322
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_resolveCoroutineRunning;

	// Token: 0x04000CFB RID: 3323
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ConsoleCmdPermissionsAllowed.ExecuteSubCommand ExecuteGrant = ConsoleCmdPermissionsAllowed.CreateExecuteMaskModifier(new ConsoleCmdPermissionsAllowed.MaskModifier(ConsoleCmdPermissionsAllowed.MaskModifierGrant));

	// Token: 0x04000CFC RID: 3324
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ConsoleCmdPermissionsAllowed.ExecuteSubCommand ExecuteRevoke = ConsoleCmdPermissionsAllowed.CreateExecuteMaskModifier(new ConsoleCmdPermissionsAllowed.MaskModifier(ConsoleCmdPermissionsAllowed.MaskModifierRevoke));

	// Token: 0x02000257 RID: 599
	// (Invoke) Token: 0x060011DE RID: 4574
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate bool ExecuteSubCommand(ReadOnlySpan<string> parameters);

	// Token: 0x02000258 RID: 600
	// (Invoke) Token: 0x060011E2 RID: 4578
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate EUserPerms MaskModifier(EUserPerms previous, EUserPerms input);
}
