using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Scripting;
using Webserver.Permissions;

namespace Webserver.Commands
{
	// Token: 0x02001B12 RID: 6930
	[Preserve]
	public class WebTokens : ConsoleCmdAbstract
	{
		// Token: 0x0600D029 RID: 53289 RVA: 0x004BEDF8 File Offset: 0x004BCFF8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string[] getCommands()
		{
			return new string[]
			{
				"webtokens"
			};
		}

		// Token: 0x0600D02A RID: 53290 RVA: 0x004BEE08 File Offset: 0x004BD008
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getDescription()
		{
			return "Manage web tokens";
		}

		// Token: 0x0600D02B RID: 53291 RVA: 0x004BEE0F File Offset: 0x004BD00F
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getHelp()
		{
			return "Set/get webtoken permission levels. A level of 0 is maximum permission.\nUsage:\n   webtokens add <tokenname> <tokensecret> <level>\n   webtokens remove <tokenname>\n   webtokens list";
		}

		// Token: 0x0600D02C RID: 53292 RVA: 0x004BEE18 File Offset: 0x004BD018
		public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
		{
			if (_params.Count < 1)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No sub command given.");
				return;
			}
			if (_params[0].EqualsCaseInsensitive("add"))
			{
				this.ExecuteAdd(_params);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("remove"))
			{
				this.ExecuteRemove(_params);
				return;
			}
			if (_params[0].EqualsCaseInsensitive("list"))
			{
				this.ExecuteList();
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid sub command \"" + _params[0] + "\".");
		}

		// Token: 0x0600D02D RID: 53293 RVA: 0x004BEEB0 File Offset: 0x004BD0B0
		[PublicizedFrom(EAccessModifier.Private)]
		public void ExecuteAdd(List<string> _params)
		{
			if (_params.Count != 4)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Wrong number of arguments, expected 4, found {0}.", _params.Count));
				return;
			}
			if (string.IsNullOrEmpty(_params[1]))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 'tokenname' is empty.");
				return;
			}
			if (!WebTokens.validNameTokenMatcher.IsMatch(_params[1]))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 'tokenname' may only contain characters (A-Z, a-z), digits (0-9) and underscores (_).");
				return;
			}
			if (string.IsNullOrEmpty(_params[2]))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 'tokensecret' is empty.");
				return;
			}
			if (!WebTokens.validNameTokenMatcher.IsMatch(_params[2]))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 'tokensecret' may only contain characters (A-Z, a-z), digits (0-9) and underscores (_).");
				return;
			}
			int num;
			if (!int.TryParse(_params[3], out num))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 'level' is not a valid integer.");
				return;
			}
			AdminApiTokens.Instance.AddToken(_params[1], _params[2], num);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Web API token with name={0} and secret={1} added with permission level of {2}.", _params[1], _params[2], num));
		}

		// Token: 0x0600D02E RID: 53294 RVA: 0x004BEFCC File Offset: 0x004BD1CC
		[PublicizedFrom(EAccessModifier.Private)]
		public void ExecuteRemove(List<string> _params)
		{
			if (_params.Count != 2)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Wrong number of arguments, expected 2, found {0}.", _params.Count));
				return;
			}
			if (string.IsNullOrEmpty(_params[1]))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 'tokenname' is empty.");
				return;
			}
			if (!WebTokens.validNameTokenMatcher.IsMatch(_params[1]))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 'tokenname' may only contain characters (A-Z, a-z), digits (0-9) and underscores (_).");
				return;
			}
			AdminApiTokens.Instance.RemoveToken(_params[1]);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(_params[1] + " removed from web API token permissions list.");
		}

		// Token: 0x0600D02F RID: 53295 RVA: 0x004BF070 File Offset: 0x004BD270
		[PublicizedFrom(EAccessModifier.Private)]
		public void ExecuteList()
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Defined web API token permissions:");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("  Level: Name / Secret");
			foreach (KeyValuePair<string, AdminApiTokens.ApiToken> keyValuePair in AdminApiTokens.Instance.GetTokens())
			{
				string text;
				AdminApiTokens.ApiToken apiToken;
				keyValuePair.Deconstruct(out text, out apiToken);
				AdminApiTokens.ApiToken apiToken2 = apiToken;
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("  {0,5}: {1} / {2}", apiToken2.PermissionLevel, apiToken2.Name, apiToken2.Secret));
			}
		}

		// Token: 0x04009E4D RID: 40525
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex validNameTokenMatcher = new Regex("^\\w+$");
	}
}
