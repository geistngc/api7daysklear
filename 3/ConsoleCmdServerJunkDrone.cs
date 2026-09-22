using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200022B RID: 555
[Preserve]
public class ConsoleCmdServerJunkDrone : ConsoleCmdAbstract
{
	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x060010D3 RID: 4307 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsExecuteOnClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x0006B1A4 File Offset: 0x000693A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"jds"
		};
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x0006B1B4 File Offset: 0x000693B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Server junk drone commands.";
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x0006B1BB File Offset: 0x000693BB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return this.buildHelpLog();
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0006B1C4 File Offset: 0x000693C4
	[PublicizedFrom(EAccessModifier.Private)]
	public string buildHelpLog()
	{
		return "JunkDrone help:" + Environment.NewLine + "[Server/Host Commands]" + Environment.NewLine + "jds, log man - logs out drone manager data" + Environment.NewLine + "clear - clears drone data for player (ex. clear [PlayerName])" + Environment.NewLine + "unstuck [playerId] - triggers teleport to player" + Environment.NewLine;
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0006B21C File Offset: 0x0006941C
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
		if (_params.Count == 0)
		{
			Log.Out(this.logDroneManager("Manager Drones"));
			return;
		}
		string text = _params[0];
		if (text.Equals("help"))
		{
			Log.Out(this.buildHelpLog());
			return;
		}
		if (text.Equals("save"))
		{
			DroneManager.Instance.TriggerSave();
			return;
		}
		if (text.Equals("log"))
		{
			if (_params.Count > 1)
			{
				string a = _params[1];
				if (a.ContainsCaseInsensitive("man"))
				{
					Log.Out(this.logDroneManager("Manager Drones"));
				}
				if (a.ContainsCaseInsensitive("active"))
				{
					Log.Out("Active Drones" + Environment.NewLine + DroneManager.Instance.LogActiveDrones() + Environment.NewLine);
				}
				if (a.ContainsCaseInsensitive("unloaded"))
				{
					Log.Out("Unloaded Drones" + Environment.NewLine + DroneManager.Instance.LogUnloadedDrones() + Environment.NewLine);
				}
			}
			return;
		}
		if (text.Equals("clearall"))
		{
			if (_senderInfo.RemoteClientInfo == null)
			{
				foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in GameManager.Instance.GetPersistentPlayerList().Players)
				{
					this.removeDronesForPlayer(keyValuePair.Value.EntityId);
				}
				Log.Out("JunkDrone data cleared for all players.");
				return;
			}
			Log.Out("This command can only be run from the host or server.");
			return;
		}
		else
		{
			if (text.Equals("clear") && _params.Count > 1)
			{
				string b = _params[1];
				List<EntityPlayer> list = world.Players.list;
				for (int i = 0; i < list.Count; i++)
				{
					EntityPlayer entityPlayer = list[i];
					if (entityPlayer.EntityName.ContainsCaseInsensitive(b))
					{
						this.removeDronesForPlayer(entityPlayer);
						Log.Out("JunkDrone data cleared for {0}.", new object[]
						{
							entityPlayer.EntityName
						});
						return;
					}
				}
				return;
			}
			if (text.Equals("unstuck"))
			{
				EntityPlayer entityPlayer2 = (_senderInfo.RemoteClientInfo == null) ? world.GetPrimaryPlayer() : (world.GetEntity(_senderInfo.RemoteClientInfo.entityId) as EntityPlayer);
				if (entityPlayer2)
				{
					List<OwnedEntityData> drones = this.getDrones(entityPlayer2);
					if (drones.Count > 0)
					{
						EntityDrone entityDrone = GameManager.Instance.World.GetEntity(drones[0].Id) as EntityDrone;
						if (entityDrone == null)
						{
							entityDrone = DroneManager.Instance.LoadDrone(drones[0].Id, GameManager.Instance.World);
						}
						if (entityDrone != null)
						{
							entityDrone.DebugTeleportUnstuck();
							Log.Out("drone unstuck complete");
							return;
						}
						Log.Warning("Server unstuck drone failed");
					}
				}
				return;
			}
			if (text.Equals("teleport") || text.Equals("tele"))
			{
				EntityPlayer entityPlayer3 = (_senderInfo.RemoteClientInfo == null) ? world.GetPrimaryPlayer() : (world.GetEntity(_senderInfo.RemoteClientInfo.entityId) as EntityPlayer);
				if (entityPlayer3)
				{
					int num = -1;
					if (int.TryParse(_params[1], out num))
					{
						EntityDrone activeDronesWithId = DroneManager.Instance.GetActiveDronesWithId(num);
						if (activeDronesWithId)
						{
							activeDronesWithId.DebugTeleportTo(entityPlayer3.position);
							Log.Out("drone debug teleport complete for {0}", new object[]
							{
								num
							});
						}
					}
				}
				return;
			}
			if (text.Equals("remove"))
			{
				int entityId = -1;
				if (int.TryParse(_params[1], out entityId))
				{
					DroneManager.Instance.RemoveActiveDrone(entityId);
				}
				return;
			}
			if (text.Equals("unassign"))
			{
				string playerName = _params[1];
				int num2 = -1;
				if (int.TryParse(_params[2], out num2))
				{
					EntityPlayer entityPlayer4 = GameManager.Instance.World.Players.list.Find((EntityPlayer p) => p.EntityName.ContainsCaseInsensitive(playerName));
					if (entityPlayer4)
					{
						EntityDrone entityDrone2 = GameManager.Instance.World.GetEntity(num2) as EntityDrone;
						if (entityDrone2)
						{
							entityDrone2.belongsPlayerId = -1;
							entityDrone2.Owner = null;
							entityPlayer4.RemoveOwnedEntity(entityDrone2);
							entityDrone2.OwnerID = null;
							Log.Out("unassigned drone {0} from player {1}", new object[]
							{
								num2,
								entityPlayer4.EntityName
							});
						}
					}
				}
				return;
			}
			if (text.Equals("assign"))
			{
				string playerName = _params[1];
				int num3 = -1;
				if (!int.TryParse(_params[2], out num3))
				{
					Log.Out("assign drone parse failed");
					return;
				}
				EntityPlayer entityPlayer5 = GameManager.Instance.World.Players.list.Find((EntityPlayer p) => p.EntityName.ContainsCaseInsensitive(playerName));
				if (!entityPlayer5)
				{
					Log.Out("unknown player name: " + playerName);
					return;
				}
				bool flag = EntityDrone.IsValidForLocalPlayer();
				if (!flag)
				{
					Log.Out("please pick up, or clear the currently deployed drone");
					return;
				}
				if (DroneManager.Instance.AssignUnloadedDrone(entityPlayer5, num3))
				{
					Log.Out("assigned unloaded drone {0} to player {1}", new object[]
					{
						num3,
						entityPlayer5.EntityName
					});
					return;
				}
				if (!flag)
				{
					Log.Out("assign drone failed");
					return;
				}
				EntityDrone entityDrone3 = GameManager.Instance.World.GetEntity(num3) as EntityDrone;
				if (entityDrone3)
				{
					entityDrone3.belongsPlayerId = entityPlayer5.entityId;
					entityDrone3.Owner = entityPlayer5;
					entityPlayer5.AddOwnedEntity(entityDrone3);
					entityDrone3.OwnerID = PlatformManager.InternalLocalUserIdentifier;
					Log.Warning(entityDrone3.OwnerID.ReadablePlatformUserIdentifier.ToString());
					Log.Out("assigned drone {0} to player {1}", new object[]
					{
						num3,
						entityPlayer5.EntityName
					});
					return;
				}
				Log.Out("assign drone failed, id is not a drone");
				return;
			}
			else
			{
				if (text.Equals("eir"))
				{
					string playerName = _params[1];
					int entityId2 = -1;
					if (int.TryParse(_params[2], out entityId2) && GameManager.Instance.World.Players.list.Find((EntityPlayer p) => p.EntityName.ContainsCaseInsensitive(playerName)))
					{
						EntityDrone entityDrone4 = GameManager.Instance.World.GetEntity(entityId2) as EntityDrone;
						if (entityDrone4)
						{
							entityDrone4.DebugEnemiesInRange = !entityDrone4.DebugEnemiesInRange;
							Log.Out("JunkDrone DebugEnemiesInRange {0}", new object[]
							{
								entityDrone4.DebugEnemiesInRange
							});
							return;
						}
					}
					return;
				}
				return;
			}
		}
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x0006AF99 File Offset: 0x00069199
	[PublicizedFrom(EAccessModifier.Private)]
	public List<OwnedEntityData> getDrones(EntityPlayer player)
	{
		if (player)
		{
			return player.GetOwnedEntities(EntityClass.junkDroneClass);
		}
		return null;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x0006B8EC File Offset: 0x00069AEC
	[PublicizedFrom(EAccessModifier.Private)]
	public string logDroneManager(string header)
	{
		string text = header + Environment.NewLine;
		List<EntityCreationData> allDronesECD = DroneManager.Instance.GetAllDronesECD();
		for (int i = 0; i < allDronesECD.Count; i++)
		{
			EntityCreationData entityCreationData = allDronesECD[i];
			text = text + string.Format("#{0}, id {1}, {2}, {3}, chunk {4}, ownerId {5}", new object[]
			{
				i,
				entityCreationData.id,
				EntityClass.GetEntityClassName(entityCreationData.entityClass),
				entityCreationData.pos.ToCultureInvariantString(),
				World.toChunkXZ(entityCreationData.pos),
				entityCreationData.belongsPlayerId
			}) + Environment.NewLine;
		}
		return text;
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x0006B9A0 File Offset: 0x00069BA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeDronesForPlayer(EntityPlayer player)
	{
		List<OwnedEntityData> drones = this.getDrones(player);
		for (int i = 0; i < drones.Count; i++)
		{
			player.RemoveOwnedEntity(drones[i]);
		}
		DroneManager.Instance.ClearAllDronesForPlayer(player);
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x0006B9E0 File Offset: 0x00069BE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeDronesForPlayer(int entityId)
	{
		EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(entityId) as EntityPlayer;
		if (entityPlayer)
		{
			List<OwnedEntityData> drones = this.getDrones(entityPlayer);
			for (int i = 0; i < drones.Count; i++)
			{
				entityPlayer.RemoveOwnedEntity(drones[i]);
			}
		}
		DroneManager.Instance.ClearAllDronesForPlayer(entityId);
	}
}
