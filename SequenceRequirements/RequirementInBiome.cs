using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001937 RID: 6455
	[Preserve]
	public class RequirementInBiome : BaseRequirement
	{
		// Token: 0x0600C728 RID: 50984 RVA: 0x00494264 File Offset: 0x00492464
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive == null)
			{
				return false;
			}
			bool flag = this.biomeList.ContainsCaseInsensitive(entityAlive.biomeStandingOn.m_sBiomeName);
			if (!this.Invert)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0600C729 RID: 50985 RVA: 0x004942A2 File Offset: 0x004924A2
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementInBiome.PropBiome, ref this.biomes);
			if (!string.IsNullOrEmpty(this.biomes))
			{
				this.biomeList = this.biomes.Split(',', StringSplitOptions.None);
			}
		}

		// Token: 0x0600C72A RID: 50986 RVA: 0x004942DD File Offset: 0x004924DD
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInBiome
			{
				Invert = this.Invert,
				biomeList = this.biomeList
			};
		}

		// Token: 0x04009649 RID: 38473
		[PublicizedFrom(EAccessModifier.Private)]
		public string biomes;

		// Token: 0x0400964A RID: 38474
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] biomeList;

		// Token: 0x0400964B RID: 38475
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBiome = "biomes";
	}
}
