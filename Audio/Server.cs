using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	// Token: 0x02001B28 RID: 6952
	public class Server : IDisposable
	{
		// Token: 0x0600D0A0 RID: 53408 RVA: 0x004C4200 File Offset: 0x004C2400
		public void Play(Entity playOnEntity, string soundGroupName, float _occlusion, bool signalOnly = false, float volumeScale = 1f)
		{
			if (GameManager.IsDedicatedServer && playOnEntity != null)
			{
				Manager.ConvertName(ref soundGroupName, playOnEntity);
				Manager.SignalAI(playOnEntity, playOnEntity.GetPosition(), soundGroupName, volumeScale);
			}
			if (!signalOnly)
			{
				foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
				{
					if (Manager.IgnoresDistanceCheck(soundGroupName) || Entity.CheckDistance(playOnEntity, keyValuePair.Value.entityId))
					{
						keyValuePair.Value.Play(playOnEntity.entityId, soundGroupName, _occlusion, 1f);
					}
				}
			}
		}

		// Token: 0x0600D0A1 RID: 53409 RVA: 0x004C42AC File Offset: 0x004C24AC
		public void Play(Vector3 position, string soundGroupName, float _occlusion, int entityId = -1, float volumeScale = 1f)
		{
			if (GameManager.IsDedicatedServer)
			{
				Manager.ConvertName(ref soundGroupName, null);
				Manager.SignalAI(null, position, soundGroupName, volumeScale);
			}
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				if (Manager.IgnoresDistanceCheck(soundGroupName) || Entity.CheckDistance(position, keyValuePair.Value.entityId))
				{
					keyValuePair.Value.Play(position, soundGroupName, _occlusion, entityId, volumeScale);
				}
			}
		}

		// Token: 0x0600D0A2 RID: 53410 RVA: 0x004C4340 File Offset: 0x004C2540
		public void Stop(int playOnEntityId, string soundGroupName)
		{
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				keyValuePair.Value.Stop(playOnEntityId, soundGroupName);
			}
		}

		// Token: 0x0600D0A3 RID: 53411 RVA: 0x004C439C File Offset: 0x004C259C
		public void Stop(Vector3 position, string soundGroupName)
		{
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				keyValuePair.Value.Stop(position, soundGroupName);
			}
		}

		// Token: 0x0600D0A4 RID: 53412 RVA: 0x004C43F8 File Offset: 0x004C25F8
		public void AttachLocalPlayer(EntityPlayerLocal localPlayer)
		{
			this.m_localPlayer = localPlayer;
		}

		// Token: 0x0600D0A5 RID: 53413 RVA: 0x004C4404 File Offset: 0x004C2604
		public void EntityAddedToWorld(Entity entity, World world)
		{
			if (entity is EntityPlayer && (this.m_localPlayer == null || entity.entityId != this.m_localPlayer.entityId))
			{
				Client value;
				if (this.m_players.TryGetValue(entity.entityId, out value))
				{
					Log.Warning("[AudioLog] AudioManagerServer: consistency error, client id '" + entity.entityId.ToString() + "' already exists, but is being added again!");
					return;
				}
				value = new Client(entity.entityId);
				this.m_players[entity.entityId] = value;
			}
		}

		// Token: 0x0600D0A6 RID: 53414 RVA: 0x004C4490 File Offset: 0x004C2690
		public void EntityRemovedFromWorld(Entity entity, World world)
		{
			Client client;
			if (this.m_players.TryGetValue(entity.entityId, out client))
			{
				this.m_players.Remove(entity.entityId);
				client.Dispose();
			}
		}

		// Token: 0x0600D0A7 RID: 53415 RVA: 0x004C44CC File Offset: 0x004C26CC
		public void Dispose()
		{
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				keyValuePair.Value.Dispose();
			}
			this.m_players = null;
			this.m_localPlayer = null;
		}

		// Token: 0x04009F42 RID: 40770
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal m_localPlayer;

		// Token: 0x04009F43 RID: 40771
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, Client> m_players = new Dictionary<int, Client>();
	}
}
