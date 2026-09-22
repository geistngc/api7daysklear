using System;

namespace Webserver.LiveData
{
	// Token: 0x02001B09 RID: 6921
	public class Animals : EntityFilterList<EntityAnimal>
	{
		// Token: 0x0600D003 RID: 53251 RVA: 0x004BE4F8 File Offset: 0x004BC6F8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override EntityAnimal predicate(Entity _e)
		{
			EntityAnimal entityAnimal = _e as EntityAnimal;
			if (entityAnimal != null && entityAnimal.IsAlive())
			{
				return entityAnimal;
			}
			return null;
		}

		// Token: 0x04009E49 RID: 40521
		public static readonly Animals Instance = new Animals();
	}
}
