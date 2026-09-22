using System;
using UnityEngine.Scripting;

// Token: 0x0200041F RID: 1055
[Preserve]
public class AIDirectorZombieState : IMemoryPoolableObject
{
	// Token: 0x0600209F RID: 8351 RVA: 0x000C4B67 File Offset: 0x000C2D67
	public AIDirectorZombieState Construct(EntityEnemy zombie)
	{
		this.m_zombie = zombie;
		return this;
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x000C4B71 File Offset: 0x000C2D71
	public void Reset()
	{
		this.m_zombie = null;
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x060020A2 RID: 8354 RVA: 0x000C4B7A File Offset: 0x000C2D7A
	public EntityEnemy Zombie
	{
		get
		{
			return this.m_zombie;
		}
	}

	// Token: 0x04001647 RID: 5703
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityEnemy m_zombie;
}
