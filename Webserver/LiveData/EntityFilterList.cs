using System;
using System.Collections.Generic;

namespace Webserver.LiveData
{
	// Token: 0x02001B0A RID: 6922
	public abstract class EntityFilterList<T> where T : Entity
	{
		// Token: 0x0600D006 RID: 53254 RVA: 0x004BE530 File Offset: 0x004BC730
		public void Get(List<T> _list)
		{
			_list.Clear();
			try
			{
				List<Entity> list = GameManager.Instance.World.Entities.list;
				for (int i = 0; i < list.Count; i++)
				{
					Entity e = list[i];
					T t = this.predicate(e);
					if (t != null)
					{
						_list.Add(t);
					}
				}
			}
			catch (Exception e2)
			{
				Log.Exception(e2);
			}
		}

		// Token: 0x0600D007 RID: 53255 RVA: 0x004BE5A8 File Offset: 0x004BC7A8
		public int GetCount()
		{
			int num = 0;
			try
			{
				List<Entity> list = GameManager.Instance.World.Entities.list;
				for (int i = 0; i < list.Count; i++)
				{
					Entity e = list[i];
					if (this.predicate(e) != null)
					{
						num++;
					}
				}
			}
			catch (Exception e2)
			{
				Log.Exception(e2);
			}
			return num;
		}

		// Token: 0x0600D008 RID: 53256
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract T predicate(Entity _e);

		// Token: 0x0600D009 RID: 53257 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public EntityFilterList()
		{
		}
	}
}
