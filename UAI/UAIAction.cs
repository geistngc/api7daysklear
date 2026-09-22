using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x02001787 RID: 6023
	[Preserve]
	public class UAIAction
	{
		// Token: 0x1700169E RID: 5790
		// (get) Token: 0x0600BA8F RID: 47759 RVA: 0x00459C74 File Offset: 0x00457E74
		// (set) Token: 0x0600BA90 RID: 47760 RVA: 0x00459C7C File Offset: 0x00457E7C
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700169F RID: 5791
		// (get) Token: 0x0600BA91 RID: 47761 RVA: 0x00459C85 File Offset: 0x00457E85
		// (set) Token: 0x0600BA92 RID: 47762 RVA: 0x00459C8D File Offset: 0x00457E8D
		public float Weight { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600BA93 RID: 47763 RVA: 0x00459C96 File Offset: 0x00457E96
		public UAIAction(string _name, float _weight)
		{
			this.Name = _name;
			this.Weight = _weight;
			this.considerations = new List<UAIConsiderationBase>();
			this.tasks = new List<UAITaskBase>();
		}

		// Token: 0x0600BA94 RID: 47764 RVA: 0x00459CC4 File Offset: 0x00457EC4
		public float GetScore(Context _context, object _target, float min = 0f)
		{
			float num = 1f;
			if (this.considerations.Count == 0)
			{
				return num * this.Weight;
			}
			if (this.tasks.Count == 0)
			{
				return 0f;
			}
			for (int i = 0; i < this.considerations.Count; i++)
			{
				if (0f > num || num < min)
				{
					return 0f;
				}
				num *= this.considerations[i].ComputeResponseCurve(this.considerations[i].GetScore(_context, _target));
			}
			return (num + (1f - num) * (float)(1 - 1 / this.considerations.Count) * num) * this.Weight;
		}

		// Token: 0x0600BA95 RID: 47765 RVA: 0x00459D72 File Offset: 0x00457F72
		public void AddConsideration(UAIConsiderationBase _c)
		{
			this.considerations.Add(_c);
		}

		// Token: 0x0600BA96 RID: 47766 RVA: 0x00459D80 File Offset: 0x00457F80
		public void AddTask(UAITaskBase _t)
		{
			this.tasks.Add(_t);
		}

		// Token: 0x0600BA97 RID: 47767 RVA: 0x00459D8E File Offset: 0x00457F8E
		public List<UAIConsiderationBase> GetConsiderations()
		{
			return this.considerations;
		}

		// Token: 0x0600BA98 RID: 47768 RVA: 0x00459D96 File Offset: 0x00457F96
		public List<UAITaskBase> GetTasks()
		{
			return this.tasks;
		}

		// Token: 0x04008C21 RID: 35873
		[PublicizedFrom(EAccessModifier.Private)]
		public List<UAITaskBase> tasks;

		// Token: 0x04008C22 RID: 35874
		[PublicizedFrom(EAccessModifier.Private)]
		public List<UAIConsiderationBase> considerations;
	}
}
