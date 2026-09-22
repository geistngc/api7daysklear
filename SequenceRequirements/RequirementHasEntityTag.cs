using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001932 RID: 6450
	[Preserve]
	public class RequirementHasEntityTag : BaseRequirement
	{
		// Token: 0x0600C711 RID: 50961 RVA: 0x00493FF4 File Offset: 0x004921F4
		public override bool CanPerform(Entity target)
		{
			FastTags<TagGroup.Global> tags = (this.tag == "") ? FastTags<TagGroup.Global>.none : FastTags<TagGroup.Global>.Parse(this.tag);
			if (!(target is EntityAlive))
			{
				return false;
			}
			if (target.HasAnyTags(tags))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C712 RID: 50962 RVA: 0x00494049 File Offset: 0x00492249
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementHasEntityTag.PropTag, ref this.tag);
		}

		// Token: 0x0600C713 RID: 50963 RVA: 0x00494063 File Offset: 0x00492263
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasEntityTag
			{
				tag = this.tag,
				Invert = this.Invert
			};
		}

		// Token: 0x04009643 RID: 38467
		[PublicizedFrom(EAccessModifier.Protected)]
		public string tag = "";

		// Token: 0x04009644 RID: 38468
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTag = "entity_tags";
	}
}
