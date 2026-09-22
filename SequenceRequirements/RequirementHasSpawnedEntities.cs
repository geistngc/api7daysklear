using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001936 RID: 6454
	[Preserve]
	public class RequirementHasSpawnedEntities : BaseRequirement
	{
		// Token: 0x0600C722 RID: 50978 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C723 RID: 50979 RVA: 0x00494150 File Offset: 0x00492350
		public override bool CanPerform(Entity target)
		{
			FastTags<TagGroup.Global> tags = FastTags<TagGroup.Global>.Parse(this.tag);
			for (int i = 0; i < GameEventManager.Current.spawnEntries.Count; i++)
			{
				GameEventManager.SpawnEntry spawnEntry = GameEventManager.Current.spawnEntries[i];
				if (spawnEntry.SpawnedEntity.HasAnyTags(tags) && (!this.targetOnly || spawnEntry.SpawnedEntity.spawnById == target.entityId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600C724 RID: 50980 RVA: 0x004941C0 File Offset: 0x004923C0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(RequirementHasSpawnedEntities.PropTag))
			{
				this.tag = properties.Values[RequirementHasSpawnedEntities.PropTag];
			}
			if (properties.Values.ContainsKey(RequirementHasSpawnedEntities.PropTargetOnly))
			{
				this.targetOnly = StringParsers.ParseBool(properties.Values[RequirementHasSpawnedEntities.PropTargetOnly], 0, -1, true);
			}
		}

		// Token: 0x0600C725 RID: 50981 RVA: 0x0049422C File Offset: 0x0049242C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasSpawnedEntities
			{
				tag = this.tag,
				targetOnly = this.targetOnly
			};
		}

		// Token: 0x04009645 RID: 38469
		[PublicizedFrom(EAccessModifier.Protected)]
		public string tag;

		// Token: 0x04009646 RID: 38470
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool targetOnly;

		// Token: 0x04009647 RID: 38471
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTag = "entity_tags";

		// Token: 0x04009648 RID: 38472
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetOnly = "target_only";
	}
}
