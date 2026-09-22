using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001931 RID: 6449
	[Preserve]
	public class RequirementHasBuffByTag : BaseRequirement
	{
		// Token: 0x0600C70C RID: 50956 RVA: 0x00493F44 File Offset: 0x00492144
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null && entityAlive.Buffs.HasBuffByTag(this.buffTags))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C70D RID: 50957 RVA: 0x00493F7E File Offset: 0x0049217E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(RequirementHasBuffByTag.PropBuffTags))
			{
				this.buffTags = FastTags<TagGroup.Global>.Parse(properties.Values[RequirementHasBuffByTag.PropBuffTags]);
			}
		}

		// Token: 0x0600C70E RID: 50958 RVA: 0x00493FB4 File Offset: 0x004921B4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasBuffByTag
			{
				buffTags = this.buffTags,
				Invert = this.Invert
			};
		}

		// Token: 0x04009641 RID: 38465
		[PublicizedFrom(EAccessModifier.Protected)]
		public FastTags<TagGroup.Global> buffTags = FastTags<TagGroup.Global>.none;

		// Token: 0x04009642 RID: 38466
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBuffTags = "buff_tags";
	}
}
