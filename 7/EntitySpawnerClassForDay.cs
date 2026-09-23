using System;
using System.Collections.Generic;

// Token: 0x02000AE7 RID: 2791
public class EntitySpawnerClassForDay
{
	// Token: 0x0600534E RID: 21326 RVA: 0x001FDC20 File Offset: 0x001FBE20
	public void AddForDay(int _day, EntitySpawnerClass _class)
	{
		if (_day != 0 && this.days.Count == 0)
		{
			this.days.Add(_class);
		}
		while (this.days.Count <= _day)
		{
			this.days.Add(null);
		}
		this.days[_day] = _class;
	}

	// Token: 0x0600534F RID: 21327 RVA: 0x001FDC74 File Offset: 0x001FBE74
	public EntitySpawnerClass Day(int _day)
	{
		if (this.days.Count == 0)
		{
			return null;
		}
		if (this.bWrapDays && _day > 0 && _day >= this.days.Count)
		{
			if (this.days.Count > 1)
			{
				_day %= this.days.Count - 1;
				if (_day == 0)
				{
					_day = this.days.Count - 1;
				}
			}
			else
			{
				_day = 1;
			}
			if (_day == 0)
			{
				_day++;
			}
		}
		else if (this.bClampDays && _day >= this.days.Count && this.days.Count > 0)
		{
			_day = this.days.Count - 1;
		}
		if (_day >= this.days.Count || this.days[_day] == null)
		{
			return this.days[0];
		}
		return this.days[_day];
	}

	// Token: 0x06005350 RID: 21328 RVA: 0x001FDD52 File Offset: 0x001FBF52
	public int Count()
	{
		return this.days.Count;
	}

	// Token: 0x040040F4 RID: 16628
	public bool bDynamicSpawner;

	// Token: 0x040040F5 RID: 16629
	public bool bWrapDays;

	// Token: 0x040040F6 RID: 16630
	public bool bClampDays;

	// Token: 0x040040F7 RID: 16631
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntitySpawnerClass> days = new List<EntitySpawnerClass>();
}
