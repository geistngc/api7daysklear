using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x0200178F RID: 6031
	[Preserve]
	public class UAIPackage
	{
		// Token: 0x170016A2 RID: 5794
		// (get) Token: 0x0600BAAD RID: 47789 RVA: 0x0045A377 File Offset: 0x00458577
		// (set) Token: 0x0600BAAE RID: 47790 RVA: 0x0045A37F File Offset: 0x0045857F
		public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170016A3 RID: 5795
		// (get) Token: 0x0600BAAF RID: 47791 RVA: 0x0045A388 File Offset: 0x00458588
		// (set) Token: 0x0600BAB0 RID: 47792 RVA: 0x0045A390 File Offset: 0x00458590
		public float Weight { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600BAB1 RID: 47793 RVA: 0x0045A399 File Offset: 0x00458599
		public UAIPackage(string _name = "", float _weight = 1f)
		{
			this.Name = _name;
			this.Weight = _weight;
			this.actionList = new List<UAIAction>();
		}

		// Token: 0x0600BAB2 RID: 47794 RVA: 0x0045A3BC File Offset: 0x004585BC
		public float DecideAction(Context _context, out UAIAction _chosenAction, out object _chosenTarget)
		{
			float num = 0f;
			_chosenAction = null;
			_chosenTarget = null;
			for (int i = 0; i < this.actionList.Count; i++)
			{
				int num2 = 0;
				int num3 = 0;
				while (num3 < _context.ConsiderationData.EntityTargets.Count && num2 <= UAIBase.MaxEntitiesToConsider)
				{
					float score = this.actionList[i].GetScore(_context, _context.ConsiderationData.EntityTargets[num3], 0f);
					if (score > num)
					{
						num = score;
						_chosenAction = this.actionList[i];
						_chosenTarget = _context.ConsiderationData.EntityTargets[num3];
					}
					num2++;
					num3++;
				}
				int num4 = 0;
				while (num4 < _context.ConsiderationData.WaypointTargets.Count && num4 <= UAIBase.MaxWaypointsToConsider)
				{
					float score2 = this.actionList[i].GetScore(_context, _context.ConsiderationData.WaypointTargets[num4], 0f);
					if (score2 > num)
					{
						num = score2;
						_chosenAction = this.actionList[i];
						_chosenTarget = _context.ConsiderationData.WaypointTargets[num4];
					}
					num4++;
				}
			}
			return num;
		}

		// Token: 0x0600BAB3 RID: 47795 RVA: 0x0045A4F8 File Offset: 0x004586F8
		public List<UAIAction> GetActions()
		{
			return this.actionList;
		}

		// Token: 0x0600BAB4 RID: 47796 RVA: 0x0045A500 File Offset: 0x00458700
		public void AddAction(UAIAction _action)
		{
			this.actionList.Add(_action);
		}

		// Token: 0x04008C3C RID: 35900
		[PublicizedFrom(EAccessModifier.Private)]
		public List<UAIAction> actionList;
	}
}
