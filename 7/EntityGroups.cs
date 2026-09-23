using System;
using System.Collections.Generic;

// Token: 0x02000ADD RID: 2781
public class EntityGroups
{
	// Token: 0x06005323 RID: 21283 RVA: 0x001FCB18 File Offset: 0x001FAD18
	public static int GetRandomEntityFromGroupMaxTier(string _sEntityGroupName, EntityClass.EntityTierTypes maxTier, ref int lastClassId, bool isEnemy = false, bool isAnimal = false, GameRandom random = null)
	{
		List<SEntityClassAndProb> list = EntityGroups.list[_sEntityGroupName];
		if (random == null)
		{
			random = GameManager.Instance.World.GetGameRandom();
		}
		int num = -1;
		if (EntityGroups.workingGroupList == null)
		{
			EntityGroups.workingGroupList = new List<SEntityClassAndProb>();
		}
		else
		{
			EntityGroups.workingGroupList.Clear();
		}
		foreach (SEntityClassAndProb sentityClassAndProb in list)
		{
			EntityClass entityClass = EntityClass.GetEntityClass(sentityClassAndProb.entityClassId);
			if (entityClass == null)
			{
				Log.Error(string.Format("EntityGroup GetRandomEnemyFromGroupMaxTier: unknown type ({0})", sentityClassAndProb.entityClassId));
				return -1;
			}
			EntityClass entityClassWithinMaxTier = EntityClass.GetEntityClassWithinMaxTier(entityClass, maxTier);
			if (entityClassWithinMaxTier != null)
			{
				bool flag = isEnemy && !entityClassWithinMaxTier.bIsEnemyEntity;
				bool flag2 = isAnimal && !entityClassWithinMaxTier.bIsAnimalEntity;
				if (!flag && !flag2)
				{
					EntityGroups.workingGroupList.Add(new SEntityClassAndProb
					{
						entityClassId = EntityClass.GetId(entityClassWithinMaxTier.entityClassName),
						prob = sentityClassAndProb.prob
					});
				}
			}
		}
		if (EntityGroups.workingGroupList.Count == 0)
		{
			return -1;
		}
		EntityGroups.NormalizeWorkingList(EntityGroups.workingGroupList);
		for (int i = 0; i < 3; i++)
		{
			num = EntityGroups.GetRandomFromGroupList(EntityGroups.workingGroupList, random);
			if (num != lastClassId)
			{
				lastClassId = num;
				break;
			}
		}
		return num;
	}

	// Token: 0x06005324 RID: 21284 RVA: 0x001FCC84 File Offset: 0x001FAE84
	[PublicizedFrom(EAccessModifier.Private)]
	public static void NormalizeWorkingList(List<SEntityClassAndProb> list)
	{
		float num = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			num += list[i].prob;
		}
		if (num <= 0f)
		{
			return;
		}
		for (int j = 0; j < list.Count; j++)
		{
			SEntityClassAndProb value = list[j];
			value.prob /= num;
			list[j] = value;
		}
	}

	// Token: 0x06005325 RID: 21285 RVA: 0x001FCCF0 File Offset: 0x001FAEF0
	public static int GetRandomFromGroup(string _sEntityGroupName, ref int lastClassId, GameRandom random = null)
	{
		List<SEntityClassAndProb> grpList = EntityGroups.list[_sEntityGroupName];
		if (random == null)
		{
			random = GameManager.Instance.World.GetGameRandom();
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			num = EntityGroups.GetRandomFromGroupList(grpList, random);
			if (num != lastClassId)
			{
				lastClassId = num;
				break;
			}
		}
		return num;
	}

	// Token: 0x06005326 RID: 21286 RVA: 0x001FCD40 File Offset: 0x001FAF40
	[PublicizedFrom(EAccessModifier.Private)]
	public static int GetRandomFromGroupList(List<SEntityClassAndProb> grpList, GameRandom random)
	{
		float randomFloat = random.RandomFloat;
		float num = 0f;
		for (int i = 0; i < grpList.Count; i++)
		{
			SEntityClassAndProb sentityClassAndProb = grpList[i];
			num += sentityClassAndProb.prob;
			if (randomFloat <= num && sentityClassAndProb.prob > 0f)
			{
				return sentityClassAndProb.entityClassId;
			}
		}
		return -1;
	}

	// Token: 0x06005327 RID: 21287 RVA: 0x001FCD98 File Offset: 0x001FAF98
	public static bool IsEnemyGroup(string _sEntityGroupName)
	{
		List<SEntityClassAndProb> list = EntityGroups.list[_sEntityGroupName];
		return list != null && list.Count >= 1 && EntityClass.list[list[0].entityClassId].bIsEnemyEntity;
	}

	// Token: 0x06005328 RID: 21288 RVA: 0x001FCDDC File Offset: 0x001FAFDC
	public static void Normalize(string _sEntityGroupName, float totalp)
	{
		List<SEntityClassAndProb> list = EntityGroups.list[_sEntityGroupName];
		for (int i = 0; i < list.Count; i++)
		{
			SEntityClassAndProb value = list[i];
			value.prob /= totalp;
			list[i] = value;
		}
	}

	// Token: 0x06005329 RID: 21289 RVA: 0x001FCE22 File Offset: 0x001FB022
	public static void Cleanup()
	{
		if (EntityGroups.list != null)
		{
			EntityGroups.list.Clear();
		}
	}

	// Token: 0x040040D2 RID: 16594
	public static DictionarySave<string, List<SEntityClassAndProb>> list = new DictionarySave<string, List<SEntityClassAndProb>>();

	// Token: 0x040040D3 RID: 16595
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<SEntityClassAndProb> workingGroupList;
}
