using System;
using System.Globalization;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x02001786 RID: 6022
	[Preserve]
	public class UAITaskWander : UAITaskBase
	{
		// Token: 0x0600BA8B RID: 47755 RVA: 0x00459BFC File Offset: 0x00457DFC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void initializeParameters()
		{
			base.initializeParameters();
			if (this.Parameters.ContainsKey("max_distance"))
			{
				this.maxWanderDistance = StringParsers.ParseFloat(this.Parameters["max_distance"], 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600BA8C RID: 47756 RVA: 0x00459C38 File Offset: 0x00457E38
		public override void Start(Context _context)
		{
			base.Start(_context);
			int num = 10;
			_context.Self.FindPath(RandomPositionGenerator.CalcAround(_context.Self, num, num), _context.Self.GetMoveSpeed(), false, null);
		}

		// Token: 0x0600BA8D RID: 47757 RVA: 0x00459BDA File Offset: 0x00457DDA
		public override void Update(Context _context)
		{
			base.Update(_context);
			if (_context.Self.getNavigator().noPathAndNotPlanningOne())
			{
				this.Stop(_context);
			}
		}

		// Token: 0x04008C1E RID: 35870
		public float maxWanderDistance;
	}
}
