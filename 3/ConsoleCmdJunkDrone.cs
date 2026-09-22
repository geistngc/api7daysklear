using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200022A RID: 554
[Preserve]
public class ConsoleCmdJunkDrone : ConsoleCmdAbstract
{
	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x060010C7 RID: 4295 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x0006AB54 File Offset: 0x00068D54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"junkDrone",
			"jd"
		};
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x0006AB6C File Offset: 0x00068D6C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Local player junk drone commands.";
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x0006AB73 File Offset: 0x00068D73
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return this.buildHelpLog();
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x0006AB7C File Offset: 0x00068D7C
	[PublicizedFrom(EAccessModifier.Private)]
	public string buildHelpLog()
	{
		string text = "JunkDrone help:" + Environment.NewLine;
		text = text + "[Client Commands]" + Environment.NewLine;
		text = text + "jd, log - logs out local player owned drones" + Environment.NewLine;
		text = text + "unstuck - triggers teleport to player" + Environment.NewLine;
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			text = text + "debuglog - toggles extended data logging" + Environment.NewLine;
			text = text + "clear - Clears local player drone type owned entities" + Environment.NewLine;
			text = text + "debugcam, dcam - toggles debug camera" + Environment.NewLine;
			text = text + "friendlyfire, ff - toggles friendly fire" + Environment.NewLine;
		}
		return text;
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x0006AC1C File Offset: 0x00068E1C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameManager instance = GameManager.Instance;
		if (!instance)
		{
			return;
		}
		World world = instance.World;
		if (world == null)
		{
			return;
		}
		EntityPlayer entityPlayer = (_senderInfo.RemoteClientInfo == null) ? world.GetPrimaryPlayer() : (world.GetEntity(_senderInfo.RemoteClientInfo.entityId) as EntityPlayer);
		if (!entityPlayer)
		{
			return;
		}
		if (_params.Count == 0)
		{
			Log.Out(this.logPlayerOwnedDrones(entityPlayer));
			return;
		}
		string text = _params[0];
		if (text.Equals("debuglog"))
		{
			DroneManager.DebugLogEnabled = !DroneManager.DebugLogEnabled;
			Log.Out("Drone debug log enabled: " + DroneManager.DebugLogEnabled.ToString());
			return;
		}
		if (text.Equals("help"))
		{
			Log.Out(this.buildHelpLog());
			return;
		}
		if (text.Equals("log"))
		{
			Log.Out(this.logPlayerOwnedDrones(entityPlayer));
			return;
		}
		if (text.Equals("unstuck"))
		{
			List<OwnedEntityData> drones = this.getDrones(entityPlayer);
			if (drones.Count > 0)
			{
				EntityDrone entityDrone = GameManager.Instance.World.GetEntity(drones[0].Id) as EntityDrone;
				if (entityDrone != null)
				{
					entityDrone.DebugTeleportUnstuck();
					Log.Out("drone unstuck complete");
					return;
				}
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					DroneManager instance2 = DroneManager.Instance;
					entityDrone = ((instance2 != null) ? instance2.LoadDrone(drones[0].Id, GameManager.Instance.World) : null);
					if (entityDrone)
					{
						entityDrone.DebugTeleportUnstuck();
						return;
					}
				}
				Log.Warning("Client drone unstuck failed. Try server side unstuck using command \"jds unstuck\"");
			}
			return;
		}
		if (text.Equals("clear"))
		{
			this.clearDronesForPlayer(entityPlayer);
			Log.Out("JunkDrone data cleared for {0}.", new object[]
			{
				entityPlayer.EntityName
			});
			return;
		}
		if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			return;
		}
		if (text.Equals("debugcam") || text.Equals("dcam"))
		{
			this.toggleDebugCam(entityPlayer);
			return;
		}
		if (text.Equals("friendlyfire") || text.Equals("ff"))
		{
			List<OwnedEntityData> drones2 = this.getDrones(entityPlayer);
			if (drones2.Count > 0)
			{
				EntityDrone entityDrone2 = GameManager.Instance.World.GetEntity(drones2[0].Id) as EntityDrone;
				entityDrone2.DebugToggleFriendlyFire();
				Log.Out("JunkDrone friendlyfire {0}", new object[]
				{
					entityDrone2.DebugFrendlyFireEnabled
				});
			}
			return;
		}
		if (text.Equals("debugrecon") || text.Equals("drc"))
		{
			List<OwnedEntityData> drones3 = this.getDrones(entityPlayer);
			if (drones3.Count > 0)
			{
				EntityDrone entityDrone3 = GameManager.Instance.World.GetEntity(drones3[0].Id) as EntityDrone;
				entityDrone3.DebugToggleDebugCamera();
				Log.Out("JunkDrone debugcam {0}", new object[]
				{
					entityDrone3.IsDebugCameraEnabled
				});
			}
			return;
		}
		if (text.Equals("debug"))
		{
			EntityDrone.DebugModeEnabled = !EntityDrone.DebugModeEnabled;
			List<OwnedEntityData> drones4 = this.getDrones(entityPlayer);
			if (drones4.Count > 0)
			{
				(GameManager.Instance.World.GetEntity(drones4[0].Id) as EntityDrone).SetDebugCameraEnabled(EntityDrone.DebugModeEnabled);
			}
			Log.Out("drone debug mode enabled: " + EntityDrone.DebugModeEnabled.ToString());
			return;
		}
		if (text.Equals("teir"))
		{
			this.toggleDebugCam(entityPlayer);
			this.toggleEnemiesInRange(entityPlayer);
			return;
		}
		if (text.Equals("eir"))
		{
			this.toggleEnemiesInRange(entityPlayer);
			return;
		}
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x0006AF99 File Offset: 0x00069199
	[PublicizedFrom(EAccessModifier.Private)]
	public List<OwnedEntityData> getDrones(EntityPlayer player)
	{
		if (player)
		{
			return player.GetOwnedEntities(EntityClass.junkDroneClass);
		}
		return null;
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x0006AFB0 File Offset: 0x000691B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void toggleDebugCam(EntityPlayer player)
	{
		List<OwnedEntityData> drones = this.getDrones(player);
		if (drones.Count > 0)
		{
			EntityDrone entityDrone = GameManager.Instance.World.GetEntity(drones[0].Id) as EntityDrone;
			entityDrone.DebugToggleDebugCamera();
			Log.Out("JunkDrone debugcam {0}", new object[]
			{
				entityDrone.IsDebugCameraEnabled
			});
		}
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x0006B014 File Offset: 0x00069214
	[PublicizedFrom(EAccessModifier.Private)]
	public string logPlayerOwnedDrones(EntityPlayer player)
	{
		string empty = string.Empty;
		int num = 0;
		List<OwnedEntityData> drones = this.getDrones(player);
		string text = string.Empty;
		for (int i = 0; i < drones.Count; i++)
		{
			OwnedEntityData ownedEntityData = drones[i];
			if (ownedEntityData != null && EntityClass.list[ownedEntityData.ClassId].entityClassName.ContainsCaseInsensitive("entityJunkDrone"))
			{
				text = text + string.Format("entityId: {0}, classId: {1}, lastKnownPosition: {2}", ownedEntityData.Id, EntityClass.GetEntityClassName(ownedEntityData.ClassId), ownedEntityData.hasLastKnownPosition ? ownedEntityData.LastKnownPosition.ToString() : "none") + Environment.NewLine;
				num++;
			}
		}
		return empty + string.Format("[{0} - count({1})]" + Environment.NewLine + "{2}", player.EntityName, num, text);
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x0006B104 File Offset: 0x00069304
	[PublicizedFrom(EAccessModifier.Private)]
	public void clearDronesForPlayer(EntityPlayer player)
	{
		List<OwnedEntityData> drones = this.getDrones(player);
		for (int i = 0; i < drones.Count; i++)
		{
			player.RemoveOwnedEntity(drones[i]);
		}
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x0006B138 File Offset: 0x00069338
	[PublicizedFrom(EAccessModifier.Private)]
	public void toggleEnemiesInRange(EntityPlayer player)
	{
		List<OwnedEntityData> drones = this.getDrones(player);
		if (drones.Count > 0)
		{
			EntityDrone entityDrone = GameManager.Instance.World.GetEntity(drones[0].Id) as EntityDrone;
			entityDrone.DebugEnemiesInRange = !entityDrone.DebugEnemiesInRange;
			Log.Out("JunkDrone DebugEnemiesInRange {0}", new object[]
			{
				entityDrone.DebugEnemiesInRange
			});
		}
	}
}
