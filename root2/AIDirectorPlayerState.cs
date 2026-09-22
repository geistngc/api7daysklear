using System;
using UnityEngine.Scripting;

// Token: 0x0200041A RID: 1050
[Preserve]
public class AIDirectorPlayerState : IMemoryPoolableObject
{
	// Token: 0x0600206F RID: 8303 RVA: 0x000C43DC File Offset: 0x000C25DC
	public AIDirectorPlayerState Construct(EntityPlayer _player)
	{
		this.Player = _player;
		this.m_dead = false;
		return this;
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000C43ED File Offset: 0x000C25ED
	public void Reset()
	{
		this.Player = null;
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x06002072 RID: 8306 RVA: 0x000C43F6 File Offset: 0x000C25F6
	// (set) Token: 0x06002073 RID: 8307 RVA: 0x000C43FE File Offset: 0x000C25FE
	public AIDirectorPlayerInventory Inventory
	{
		get
		{
			return this.m_inventory;
		}
		set
		{
			this.m_inventory = value;
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x06002074 RID: 8308 RVA: 0x000C4407 File Offset: 0x000C2607
	// (set) Token: 0x06002075 RID: 8309 RVA: 0x000C440F File Offset: 0x000C260F
	public bool Dead
	{
		get
		{
			return this.m_dead;
		}
		set
		{
			this.m_dead = value;
		}
	}

	// Token: 0x0400162B RID: 5675
	public const float kCheckUndergroundTime = 5f;

	// Token: 0x0400162C RID: 5676
	public const int kNumBlocksUnderground = 10;

	// Token: 0x0400162D RID: 5677
	public EntityPlayer Player;

	// Token: 0x0400162E RID: 5678
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorPlayerInventory m_inventory;

	// Token: 0x0400162F RID: 5679
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_dead;
}
