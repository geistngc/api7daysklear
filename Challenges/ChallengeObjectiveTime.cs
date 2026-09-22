using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001913 RID: 6419
	[Preserve]
	public class ChallengeObjectiveTime : BaseChallengeObjective
	{
		// Token: 0x17001887 RID: 6279
		// (get) Token: 0x0600C639 RID: 50745 RVA: 0x00490E87 File Offset: 0x0048F087
		// (set) Token: 0x0600C63A RID: 50746 RVA: 0x00490E8F File Offset: 0x0048F08F
		public float CurrentTime
		{
			get
			{
				return this.currentTime;
			}
			set
			{
				this.currentTime = value;
				base.HandleValueChanged();
			}
		}

		// Token: 0x17001888 RID: 6280
		// (get) Token: 0x0600C63B RID: 50747 RVA: 0x0004AC66 File Offset: 0x00048E66
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Time;
			}
		}

		// Token: 0x17001889 RID: 6281
		// (get) Token: 0x0600C63C RID: 50748 RVA: 0x00490EA0 File Offset: 0x0048F0A0
		public override string DescriptionText
		{
			get
			{
				if (this.Biome == "")
				{
					return Localization.Get("challengeObjectiveTime", false, null) + ":";
				}
				return string.Format(Localization.Get("challengeObjectiveTimeInBiome", false, null), Localization.Get("biome_" + this.Biome, false, null));
			}
		}

		// Token: 0x1700188A RID: 6282
		// (get) Token: 0x0600C63D RID: 50749 RVA: 0x00490F00 File Offset: 0x0048F100
		public override string StatusText
		{
			get
			{
				if (this.currentTime == 0f)
				{
					return string.Format("{0}{1}", this.currentTime, Localization.Get("timeAbbreviationSeconds", false, null)) + " / " + XUiM_PlayerBuffs.GetTimeString(this.maxTime);
				}
				return XUiM_PlayerBuffs.GetTimeString(this.currentTime) + "/ " + XUiM_PlayerBuffs.GetTimeString(this.maxTime);
			}
		}

		// Token: 0x1700188B RID: 6283
		// (get) Token: 0x0600C63E RID: 50750 RVA: 0x00490F71 File Offset: 0x0048F171
		public override float FillAmount
		{
			get
			{
				return this.currentTime / this.maxTime;
			}
		}

		// Token: 0x1700188C RID: 6284
		// (get) Token: 0x0600C63F RID: 50751 RVA: 0x00490F80 File Offset: 0x0048F180
		public override bool NeedsConstantUIUpdate
		{
			get
			{
				return !base.Complete;
			}
		}

		// Token: 0x0600C640 RID: 50752 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C641 RID: 50753 RVA: 0x00490F8B File Offset: 0x0048F18B
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.AddObjectiveToBeUpdated(this);
		}

		// Token: 0x0600C642 RID: 50754 RVA: 0x00490F98 File Offset: 0x0048F198
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
		}

		// Token: 0x0600C643 RID: 50755 RVA: 0x00490FA8 File Offset: 0x0048F1A8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void Update(float deltaTime)
		{
			this.nextCheck -= deltaTime;
			if (this.nextCheck <= 0f)
			{
				this.isInvalid = this.CheckBaseRequirements();
				this.nextCheck = 2f;
			}
			if (this.player == null)
			{
				this.player = this.Owner.Owner.Player;
			}
			if (this.isInvalid || this.player == null || this.player.IsDead())
			{
				return;
			}
			this.CurrentTime += deltaTime;
			if (this.currentTime >= this.maxTime)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C644 RID: 50756 RVA: 0x0049105F File Offset: 0x0048F25F
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("max_time"))
			{
				this.maxTime = StringParsers.ParseFloat(e.GetAttribute("max_time"), 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600C645 RID: 50757 RVA: 0x0049109C File Offset: 0x0048F29C
		public override void Read(byte _currentVersion, BinaryReader _br)
		{
			base.Read(_currentVersion, _br);
			this.currentTime = _br.ReadSingle();
		}

		// Token: 0x0600C646 RID: 50758 RVA: 0x004910B2 File Offset: 0x0048F2B2
		public override void Write(BinaryWriter _bw)
		{
			base.Write(_bw);
			_bw.Write(this.currentTime);
		}

		// Token: 0x0600C647 RID: 50759 RVA: 0x004910C8 File Offset: 0x0048F2C8
		public override void CopyValues(BaseChallengeObjective obj, BaseChallengeObjective objFromClass)
		{
			base.CopyValues(obj, objFromClass);
			ChallengeObjectiveTime challengeObjectiveTime = obj as ChallengeObjectiveTime;
			if (challengeObjectiveTime != null)
			{
				this.currentTime = challengeObjectiveTime.currentTime;
			}
		}

		// Token: 0x0600C648 RID: 50760 RVA: 0x004910F4 File Offset: 0x0048F2F4
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveTime
			{
				Biome = this.Biome,
				maxTime = this.maxTime,
				currentTime = this.currentTime,
				nextCheck = this.nextCheck,
				isInvalid = this.isInvalid
			};
		}

		// Token: 0x040095C9 RID: 38345
		[PublicizedFrom(EAccessModifier.Protected)]
		public float currentTime;

		// Token: 0x040095CA RID: 38346
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxTime;

		// Token: 0x040095CB RID: 38347
		[PublicizedFrom(EAccessModifier.Protected)]
		public float nextCheck;

		// Token: 0x040095CC RID: 38348
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isInvalid = true;

		// Token: 0x040095CD RID: 38349
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal player;
	}
}
