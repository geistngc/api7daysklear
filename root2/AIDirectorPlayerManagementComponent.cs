using System;
using UnityEngine.Scripting;

// Token: 0x02000419 RID: 1049
[Preserve]
public class AIDirectorPlayerManagementComponent : AIDirectorComponent
{
	// Token: 0x06002067 RID: 8295 RVA: 0x000C427C File Offset: 0x000C247C
	public override void Tick(double _dt)
	{
		base.Tick(_dt);
		this.TickPlayerStates(_dt);
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x000C428C File Offset: 0x000C248C
	public void AddPlayer(EntityPlayer _player)
	{
		if (!this.trackedPlayers.dict.ContainsKey(_player.entityId))
		{
			AIDirectorPlayerState aidirectorPlayerState = this.playerPool.Alloc(false);
			if (aidirectorPlayerState != null)
			{
				this.trackedPlayers.Add(_player.entityId, aidirectorPlayerState.Construct(_player));
			}
		}
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000C42DC File Offset: 0x000C24DC
	public void RemovePlayer(EntityPlayer _player)
	{
		AIDirectorPlayerState aidirectorPlayerState;
		if (this.trackedPlayers.dict.TryGetValue(_player.entityId, out aidirectorPlayerState))
		{
			this.trackedPlayers.Remove(_player.entityId);
			aidirectorPlayerState.Reset();
			this.playerPool.Free(aidirectorPlayerState);
		}
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000C4328 File Offset: 0x000C2528
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickPlayerStates(double _dt)
	{
		for (int i = 0; i < this.trackedPlayers.list.Count; i++)
		{
			AIDirectorPlayerState ps = this.trackedPlayers.list[i];
			this.TickPlayerState(ps, _dt);
		}
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000C436C File Offset: 0x000C256C
	public void UpdatePlayerInventory(int entityId, AIDirectorPlayerInventory inventory)
	{
		AIDirectorPlayerState aidirectorPlayerState;
		if (this.trackedPlayers.dict.TryGetValue(entityId, out aidirectorPlayerState))
		{
			aidirectorPlayerState.Inventory = inventory;
		}
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000C4395 File Offset: 0x000C2595
	public void UpdatePlayerInventory(EntityPlayerLocal player)
	{
		this.UpdatePlayerInventory(player.entityId, AIDirectorPlayerInventory.FromEntity(player));
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x000C43A9 File Offset: 0x000C25A9
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickPlayerState(AIDirectorPlayerState _ps, double _dt)
	{
		_ps.Dead = _ps.Player.IsDead();
	}

	// Token: 0x04001629 RID: 5673
	public DictionaryList<int, AIDirectorPlayerState> trackedPlayers = new DictionaryList<int, AIDirectorPlayerState>();

	// Token: 0x0400162A RID: 5674
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledObject<AIDirectorPlayerState> playerPool = new MemoryPooledObject<AIDirectorPlayerState>(32);
}
