using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.DuckType.Jiggle
{
	// Token: 0x02001D63 RID: 7523
	public static class JiggleScheduler
	{
		// Token: 0x0600DE6F RID: 56943 RVA: 0x004FD54C File Offset: 0x004FB74C
		public static void Register(Jiggle jiggleBone)
		{
			JiggleScheduler.s_Records[jiggleBone] = JiggleScheduler.GetHierarchyDepth(jiggleBone.transform);
			JiggleScheduler.isDirty = true;
		}

		// Token: 0x0600DE70 RID: 56944 RVA: 0x004FD56A File Offset: 0x004FB76A
		public static void Deregister(Jiggle jiggleBone)
		{
			JiggleScheduler.s_Records.Remove(jiggleBone);
			JiggleScheduler.isDirty = true;
		}

		// Token: 0x0600DE71 RID: 56945 RVA: 0x004FD580 File Offset: 0x004FB780
		public static void Update(Jiggle jiggle)
		{
			if (JiggleScheduler.isDirty)
			{
				JiggleScheduler.isDirty = false;
				JiggleScheduler.UpdateOrderedRecords();
			}
			if (jiggle == JiggleScheduler.m_UpdateTriggerJiggle)
			{
				foreach (Jiggle jiggle2 in JiggleScheduler.s_OrderedRecords)
				{
					if (jiggle2.enabled && !jiggle2.UpdateWithPhysics)
					{
						jiggle2.ScheduledUpdate(Time.deltaTime);
					}
				}
			}
		}

		// Token: 0x0600DE72 RID: 56946 RVA: 0x004FD608 File Offset: 0x004FB808
		public static void FixedUpdate(Jiggle jiggle)
		{
			if (jiggle == JiggleScheduler.m_UpdateTriggerJiggle)
			{
				foreach (Jiggle jiggle2 in JiggleScheduler.s_OrderedRecords)
				{
					if (jiggle2.enabled && jiggle2.UpdateWithPhysics)
					{
						jiggle2.ScheduledUpdate(Time.fixedDeltaTime);
					}
				}
			}
		}

		// Token: 0x0600DE73 RID: 56947 RVA: 0x004FD67C File Offset: 0x004FB87C
		[PublicizedFrom(EAccessModifier.Private)]
		public static void UpdateOrderedRecords()
		{
			JiggleScheduler.s_OrderedRecords = (from x in JiggleScheduler.s_Records
			orderby x.Value
			select x.Key).ToList<Jiggle>();
			JiggleScheduler.m_UpdateTriggerJiggle = JiggleScheduler.s_OrderedRecords.FirstOrDefault<Jiggle>();
		}

		// Token: 0x0600DE74 RID: 56948 RVA: 0x004FD6EF File Offset: 0x004FB8EF
		[PublicizedFrom(EAccessModifier.Private)]
		public static int GetHierarchyDepth(Transform t)
		{
			if (!(t == null))
			{
				return JiggleScheduler.GetHierarchyDepth(t.parent) + 1;
			}
			return -1;
		}

		// Token: 0x0400A913 RID: 43283
		[PublicizedFrom(EAccessModifier.Private)]
		public static Dictionary<Jiggle, int> s_Records = new Dictionary<Jiggle, int>();

		// Token: 0x0400A914 RID: 43284
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<Jiggle> s_OrderedRecords = new List<Jiggle>();

		// Token: 0x0400A915 RID: 43285
		[PublicizedFrom(EAccessModifier.Private)]
		public static Jiggle m_UpdateTriggerJiggle = null;

		// Token: 0x0400A916 RID: 43286
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool isDirty;
	}
}
