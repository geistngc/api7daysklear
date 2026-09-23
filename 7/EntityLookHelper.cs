using System;

// Token: 0x02000507 RID: 1287
public class EntityLookHelper
{
	// Token: 0x06002A28 RID: 10792 RVA: 0x0010862F File Offset: 0x0010682F
	public EntityLookHelper(EntityAlive _e)
	{
		this.entity = _e;
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x00108640 File Offset: 0x00106840
	public void onUpdateLook()
	{
		if (this.entity.rotation.x > 1f)
		{
			EntityAlive entityAlive = this.entity;
			entityAlive.rotation.x = entityAlive.rotation.x - 1f;
			return;
		}
		if (this.entity.rotation.x < -1f)
		{
			EntityAlive entityAlive2 = this.entity;
			entityAlive2.rotation.x = entityAlive2.rotation.x + 1f;
		}
	}

	// Token: 0x04001FF5 RID: 8181
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entity;
}
