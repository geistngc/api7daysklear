using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000234 RID: 564
[Preserve]
public class ConsoleCmdListEntities : ConsoleCmdAbstract
{
	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x060010FD RID: 4349 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x060010FE RID: 4350 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x060010FF RID: 4351 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x0006C4CC File Offset: 0x0006A6CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"listents",
			"le"
		};
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x0006C4E4 File Offset: 0x0006A6E4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		int num = 0;
		for (int i = GameManager.Instance.World.Entities.list.Count - 1; i >= 0; i--)
		{
			Entity entity = GameManager.Instance.World.Entities.list[i];
			EntityAlive entityAlive = null;
			if (entity is EntityAlive)
			{
				entityAlive = (EntityAlive)entity;
			}
			SdtdConsole instance = SingletonMonoBehaviour<SdtdConsole>.Instance;
			string[] array = new string[17];
			int num2 = 0;
			int num3;
			num = (num3 = num + 1);
			array[num2] = num3.ToString();
			array[1] = ". id=";
			array[2] = entity.entityId.ToString();
			array[3] = ", ";
			array[4] = entity.ToString();
			array[5] = ", pos=";
			array[6] = entity.GetPosition().ToCultureInvariantString();
			array[7] = ", rot=";
			array[8] = entity.rotation.ToCultureInvariantString();
			array[9] = ", lifetime=";
			array[10] = ((entity.lifetime == float.MaxValue) ? "float.Max" : entity.lifetime.ToCultureInvariantString("0.0"));
			array[11] = ", remote=";
			array[12] = entity.isEntityRemote.ToString();
			array[13] = ", dead=";
			array[14] = entity.IsDead().ToString();
			array[15] = ", ";
			array[16] = ((entityAlive != null) ? ("health=" + entityAlive.Health.ToString()) : "");
			instance.Output(string.Concat(array));
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Total of " + GameManager.Instance.World.Entities.Count.ToString() + " in the game");
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x0006C696 File Offset: 0x0006A896
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "lists all entities";
	}
}
