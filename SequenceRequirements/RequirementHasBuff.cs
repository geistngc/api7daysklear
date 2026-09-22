using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001930 RID: 6448
	[Preserve]
	public class RequirementHasBuff : BaseRequirement
	{
		// Token: 0x0600C705 RID: 50949 RVA: 0x00493E51 File Offset: 0x00492051
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			this.BuffList = this.BuffName.Split(',', StringSplitOptions.None);
		}

		// Token: 0x0600C706 RID: 50950 RVA: 0x00493E68 File Offset: 0x00492068
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				for (int i = 0; i < this.BuffList.Length; i++)
				{
					if (!this.CheckBuff(entityAlive, this.BuffList[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600C707 RID: 50951 RVA: 0x00493EA8 File Offset: 0x004920A8
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool CheckBuff(EntityAlive player, string buffName)
		{
			if (player.Buffs.HasBuff(buffName))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C708 RID: 50952 RVA: 0x00493EC8 File Offset: 0x004920C8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(RequirementHasBuff.PropBuffName))
			{
				this.BuffName = properties.Values[RequirementHasBuff.PropBuffName];
			}
		}

		// Token: 0x0600C709 RID: 50953 RVA: 0x00493EF9 File Offset: 0x004920F9
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasBuff
			{
				BuffName = this.BuffName,
				BuffList = this.BuffList,
				Invert = this.Invert
			};
		}

		// Token: 0x0400963E RID: 38462
		[PublicizedFrom(EAccessModifier.Protected)]
		public string BuffName = "";

		// Token: 0x0400963F RID: 38463
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] BuffList;

		// Token: 0x04009640 RID: 38464
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBuffName = "buff_name";
	}
}
