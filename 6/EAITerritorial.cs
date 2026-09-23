using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200045C RID: 1116
[Preserve]
public class EAITerritorial : EAIBase
{
	// Token: 0x060021FD RID: 8701 RVA: 0x000C98A9 File Offset: 0x000C7AA9
	public EAITerritorial()
	{
		this.MutexBits = 1;
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x000C95A0 File Offset: 0x000C77A0
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x000CDAA0 File Offset: 0x000CBCA0
	public override bool CanExecute()
	{
		if (this.theEntity.isWithinHomeDistanceCurrentPosition())
		{
			return false;
		}
		ChunkCoordinates homePosition = this.theEntity.getHomePosition();
		Vector3 vector = RandomPositionGenerator.CalcTowards(this.theEntity, 5, 15, 7, homePosition.position.ToVector3());
		if (vector.Equals(Vector3.zero))
		{
			return false;
		}
		this.movePos = vector;
		return true;
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x000CDAFB File Offset: 0x000CBCFB
	public override bool Continue()
	{
		return !this.theEntity.getNavigator().noPathAndNotPlanningOne();
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x000CDB10 File Offset: 0x000CBD10
	public override void Start()
	{
		this.theEntity.FindPath(this.movePos, this.theEntity.GetMoveSpeed(), false, this);
	}

	// Token: 0x0400177C RID: 6012
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 movePos;
}
