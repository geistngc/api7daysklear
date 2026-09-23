using System;
using UnityEngine.Scripting;

// Token: 0x02000122 RID: 290
[Preserve]
public class BlockInfo : Block
{
	// Token: 0x060007B2 RID: 1970 RVA: 0x00036B40 File Offset: 0x00034D40
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey(BlockInfo.PropInfoType))
		{
			this.InformationType = Enum.Parse<BlockInfo.InformationTypes>(base.Properties.Values[BlockInfo.PropInfoType]);
		}
		base.Properties.ParseFloat(BlockInfo.PropTakeDelay, ref this.TakeDelay);
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00036BA0 File Offset: 0x00034DA0
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return this.TakeDelay > 0f;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00036BB0 File Offset: 0x00034DB0
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		this.cmds[0].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00036BFC File Offset: 0x00034DFC
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		string result;
		switch (this.InformationType)
		{
		case BlockInfo.InformationTypes.Time:
			result = string.Format(Localization.Get("showClockTime", false, null), GameUtils.WorldTimeToHourMinutesString(GameManager.Instance.World.worldTime));
			break;
		case BlockInfo.InformationTypes.Day:
			result = string.Format(Localization.Get("showCalendarDate", false, null), GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime));
			break;
		case BlockInfo.InformationTypes.TimeAndDay:
			result = string.Format(Localization.Get("showTimeAndDay", false, null), GameUtils.WorldTimeToHourMinutesString(GameManager.Instance.World.worldTime), GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime));
			break;
		default:
			result = "";
			break;
		}
		return result;
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x00036CC4 File Offset: 0x00034EC4
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "take")
		{
			base.takeItemWithTimer(_blockPos, _blockValue, _player, this.TakeDelay);
			return true;
		}
		return false;
	}

	// Token: 0x0400090C RID: 2316
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTakeDelay = "TakeDelay";

	// Token: 0x0400090D RID: 2317
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropInfoType = "InfoType";

	// Token: 0x0400090E RID: 2318
	public BlockInfo.InformationTypes InformationType;

	// Token: 0x0400090F RID: 2319
	[PublicizedFrom(EAccessModifier.Protected)]
	public float TakeDelay = 2f;

	// Token: 0x04000910 RID: 2320
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("take", "hand", false, false, null)
	};

	// Token: 0x02000123 RID: 291
	public enum InformationTypes
	{
		// Token: 0x04000912 RID: 2322
		Time,
		// Token: 0x04000913 RID: 2323
		Day,
		// Token: 0x04000914 RID: 2324
		TimeAndDay
	}
}
