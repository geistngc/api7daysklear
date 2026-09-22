using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200190B RID: 6411
	[Preserve]
	public class ChallengeObjectiveKill : BaseChallengeObjective
	{
		// Token: 0x17001876 RID: 6262
		// (get) Token: 0x0600C5F1 RID: 50673 RVA: 0x000814EA File Offset: 0x0007F6EA
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Kill;
			}
		}

		// Token: 0x17001877 RID: 6263
		// (get) Token: 0x0600C5F2 RID: 50674 RVA: 0x00490084 File Offset: 0x0048E284
		public override string DescriptionText
		{
			get
			{
				if (this.Biome == "")
				{
					return Localization.Get("challengeObjectiveKill", false, null) + " " + Localization.Get(this.entityIDs, false, null) + ":";
				}
				return string.Format(Localization.Get("challengeObjectiveKillIn", false, null), Localization.Get(this.entityIDs, false, null), Localization.Get("biome_" + this.Biome, false, null));
			}
		}

		// Token: 0x0600C5F3 RID: 50675 RVA: 0x00490104 File Offset: 0x0048E304
		public override void Init()
		{
			if (this.entityIDs != null)
			{
				string[] array = this.entityIDs.Split(',', StringSplitOptions.None);
				if (array.Length > 1)
				{
					this.entityIDs = array[0];
					this.entityNames = new string[array.Length - 1];
					for (int i = 1; i < array.Length; i++)
					{
						this.entityNames[i - 1] = array[i];
					}
				}
			}
		}

		// Token: 0x0600C5F4 RID: 50676 RVA: 0x00490164 File Offset: 0x0048E364
		public override void HandleAddHooks()
		{
			if (!EntityFactory.EnemySpawnMode && this.isEnemy)
			{
				base.Current = this.MaxCount;
				base.Complete = true;
				this.Owner.HandleComplete(false, true);
				if (this.Owner.ChallengeState == Challenge.ChallengeStates.Completed)
				{
					this.Owner.AutoCompleted = true;
					this.Owner.ChallengeState = Challenge.ChallengeStates.Redeemed;
					return;
				}
			}
			else
			{
				QuestEventManager.Current.EntityKill -= this.Current_EntityKill;
				QuestEventManager.Current.EntityKill += this.Current_EntityKill;
			}
		}

		// Token: 0x0600C5F5 RID: 50677 RVA: 0x004901F3 File Offset: 0x0048E3F3
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.EntityKill -= this.Current_EntityKill;
		}

		// Token: 0x0600C5F6 RID: 50678 RVA: 0x0049020C File Offset: 0x0048E40C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_EntityKill(EntityAlive killedBy, EntityAlive killedEntity)
		{
			string entityClassName = killedEntity.EntityClass.entityClassName;
			bool flag = false;
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.entityIDs == null || entityClassName.EqualsCaseInsensitive(this.entityIDs))
			{
				flag = true;
			}
			if (!flag && this.entityNames != null)
			{
				for (int i = 0; i < this.entityNames.Length; i++)
				{
					if (this.entityNames[i].EqualsCaseInsensitive(entityClassName))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				int num = base.Current;
				base.Current = num + 1;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C5F7 RID: 50679 RVA: 0x00490295 File Offset: 0x0048E495
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			e.ParseAttribute("entity_names", ref this.entityIDs);
			e.ParseAttribute("is_enemy", ref this.isEnemy);
		}

		// Token: 0x0600C5F8 RID: 50680 RVA: 0x004902CC File Offset: 0x0048E4CC
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveKill
			{
				entityIDs = this.entityIDs,
				entityNames = this.entityNames,
				isEnemy = this.isEnemy,
				Biome = this.Biome
			};
		}

		// Token: 0x040095B5 RID: 38325
		[PublicizedFrom(EAccessModifier.Private)]
		public string entityIDs = "";

		// Token: 0x040095B6 RID: 38326
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] entityNames;

		// Token: 0x040095B7 RID: 38327
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isEnemy = true;
	}
}
