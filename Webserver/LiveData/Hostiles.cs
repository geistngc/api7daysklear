using System;

namespace Webserver.LiveData
{
	// Token: 0x02001B0B RID: 6923
	public class Hostiles : EntityFilterList<EntityEnemy>
	{
		// Token: 0x0600D00A RID: 53258 RVA: 0x004BE618 File Offset: 0x004BC818
		[PublicizedFrom(EAccessModifier.Protected)]
		public override EntityEnemy predicate(Entity _e)
		{
			EntityEnemy entityEnemy = _e as EntityEnemy;
			if (entityEnemy != null && entityEnemy.IsAlive())
			{
				return entityEnemy;
			}
			return null;
		}

		// Token: 0x04009E4A RID: 40522
		public static readonly Hostiles Instance = new Hostiles();
	}
}
