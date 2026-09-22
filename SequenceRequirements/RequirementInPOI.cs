using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001938 RID: 6456
	[Preserve]
	public class RequirementInPOI : BaseRequirement
	{
		// Token: 0x0600C72D RID: 50989 RVA: 0x00494308 File Offset: 0x00492508
		public override bool CanPerform(Entity target)
		{
			bool flag = true;
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				if (entityPlayer.prefab != null)
				{
					Prefab prefab = entityPlayer.prefab.prefab;
					if (this.poiTags != "" && !prefab.Tags.Test_AnySet(FastTags<TagGroup.Poi>.Parse(this.poiTags)))
					{
						flag = false;
					}
					if (this.poiName != "" && !this.poiName.ContainsCaseInsensitive(prefab.PrefabName) && !this.poiName.ContainsCaseInsensitive(prefab.LocalizedName))
					{
						flag = false;
					}
					if (this.poiTier != -1 && (int)prefab.DifficultyTier != this.poiTier)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			if (!this.Invert)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0600C72E RID: 50990 RVA: 0x004943D3 File Offset: 0x004925D3
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseInt(RequirementInPOI.PropPOITier, ref this.poiTier);
			properties.ParseString(RequirementInPOI.PropPOITags, ref this.poiTags);
			properties.ParseString(RequirementInPOI.PropPOINames, ref this.poiName);
		}

		// Token: 0x0600C72F RID: 50991 RVA: 0x0049440F File Offset: 0x0049260F
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInPOI
			{
				Invert = this.Invert,
				poiTier = this.poiTier,
				poiTags = this.poiTags,
				poiName = this.poiName
			};
		}

		// Token: 0x0400964C RID: 38476
		[PublicizedFrom(EAccessModifier.Private)]
		public string poiName = "";

		// Token: 0x0400964D RID: 38477
		[PublicizedFrom(EAccessModifier.Private)]
		public string poiTags = "";

		// Token: 0x0400964E RID: 38478
		[PublicizedFrom(EAccessModifier.Private)]
		public int poiTier = -1;

		// Token: 0x0400964F RID: 38479
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPOITier = "tier";

		// Token: 0x04009650 RID: 38480
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPOITags = "tags";

		// Token: 0x04009651 RID: 38481
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPOINames = "name";
	}
}
