using System;
using System.Collections.Generic;

// Token: 0x02000ADE RID: 2782
public class EntityGroupSpawnState
{
	// Token: 0x0600532C RID: 21292 RVA: 0x001FCE44 File Offset: 0x001FB044
	public EntityGroupSpawnState(string _sEntityGroupName)
	{
		List<SEntityClassAndProb> list = EntityGroups.list[_sEntityGroupName];
		for (int i = 0; i < list.Count; i++)
		{
			this.state.Add(new EntityGroupSpawnState.State(list[i]));
		}
	}

	// Token: 0x0600532D RID: 21293 RVA: 0x001FCE98 File Offset: 0x001FB098
	public int GetRandomFromGroup()
	{
		float randomFloat = GameManager.Instance.World.GetGameRandom().RandomFloat;
		float num = 0f;
		for (int i = 0; i < this.state.Count; i++)
		{
			EntityGroupSpawnState.State state = this.state[i];
			num += state.prob;
			if (randomFloat <= num && state.prob > 0f)
			{
				return state.entityClassId;
			}
		}
		return -1;
	}

	// Token: 0x0600532E RID: 21294 RVA: 0x001FCF08 File Offset: 0x001FB108
	public void DidSpawn(int _classId)
	{
		for (int i = 0; i < this.state.Count; i++)
		{
			EntityGroupSpawnState.State state = this.state[i];
			if (state.entityClassId == _classId)
			{
				state.numSpawned++;
			}
			this.state[i] = state;
		}
	}

	// Token: 0x040040D4 RID: 16596
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityGroupSpawnState.State> state = new List<EntityGroupSpawnState.State>();

	// Token: 0x02000ADF RID: 2783
	[PublicizedFrom(EAccessModifier.Private)]
	public struct State
	{
		// Token: 0x0600532F RID: 21295 RVA: 0x001FCF5A File Offset: 0x001FB15A
		public State(SEntityClassAndProb _src)
		{
			this.entityClassId = _src.entityClassId;
			this.prob = _src.prob;
			this.numSpawned = 0;
		}

		// Token: 0x040040D5 RID: 16597
		public readonly int entityClassId;

		// Token: 0x040040D6 RID: 16598
		public readonly float prob;

		// Token: 0x040040D7 RID: 16599
		public int numSpawned;
	}
}
