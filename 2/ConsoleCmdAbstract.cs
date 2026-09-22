using System;
using System.Collections.Generic;
using Platform;

// Token: 0x020001DD RID: 477
public abstract class ConsoleCmdAbstract : IConsoleCommand
{
	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000EBB RID: 3771 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsExecuteOnClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000EBC RID: 3772 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual int DefaultPermissionLevel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000EBD RID: 3773 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000EBE RID: 3774 RVA: 0x0006037D File Offset: 0x0005E57D
	public virtual DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX;
		}
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0006037D File Offset: 0x0005E57D
	public virtual DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX;
		}
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0000640C File Offset: 0x0000460C
	public ConsoleCmdAbstract()
	{
	}

	// Token: 0x06000EC1 RID: 3777
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract string[] getCommands();

	// Token: 0x06000EC2 RID: 3778 RVA: 0x00060380 File Offset: 0x0005E580
	public virtual string[] GetCommands()
	{
		string[] result;
		if ((result = this.commandNamesCache) == null)
		{
			result = (this.commandNamesCache = this.getCommands());
		}
		return result;
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x000603A8 File Offset: 0x0005E5A8
	public virtual string PrimaryCommand
	{
		get
		{
			string result;
			if ((result = this.primaryCommand) == null)
			{
				result = (this.primaryCommand = this.GetCommands()[0]);
			}
			return result;
		}
	}

	// Token: 0x06000EC4 RID: 3780
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract string getDescription();

	// Token: 0x06000EC5 RID: 3781 RVA: 0x000603D0 File Offset: 0x0005E5D0
	public virtual string GetDescription()
	{
		string result;
		if ((result = this.commandDescriptionCache) == null)
		{
			result = (this.commandDescriptionCache = this.getDescription());
		}
		return result;
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string getHelp()
	{
		return null;
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x000603F8 File Offset: 0x0005E5F8
	public virtual string GetHelp()
	{
		string result;
		if ((result = this.commandHelpCache) == null)
		{
			result = (this.commandHelpCache = this.getHelp());
		}
		return result;
	}

	// Token: 0x06000EC8 RID: 3784
	public abstract void Execute(List<string> _params, CommandSenderInfo _senderInfo);

	// Token: 0x04000C85 RID: 3205
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] commandNamesCache;

	// Token: 0x04000C86 RID: 3206
	[PublicizedFrom(EAccessModifier.Private)]
	public string commandDescriptionCache;

	// Token: 0x04000C87 RID: 3207
	[PublicizedFrom(EAccessModifier.Private)]
	public string commandHelpCache;

	// Token: 0x04000C88 RID: 3208
	[PublicizedFrom(EAccessModifier.Private)]
	public string primaryCommand;
}
