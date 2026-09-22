using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001925 RID: 6437
	[Preserve]
	public class BaseOperationRequirement : BaseRequirement
	{
		// Token: 0x0600C6BF RID: 50879 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x1700189A RID: 6298
		// (get) Token: 0x0600C6C0 RID: 50880 RVA: 0x004936AC File Offset: 0x004918AC
		// (set) Token: 0x0600C6C1 RID: 50881 RVA: 0x004936B7 File Offset: 0x004918B7
		public bool StringCompareCaseSensitive
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return this.stringComparison == StringComparison.CurrentCulture;
			}
			[PublicizedFrom(EAccessModifier.Protected)]
			set
			{
				this.stringComparison = (value ? StringComparison.CurrentCulture : StringComparison.OrdinalIgnoreCase);
			}
		}

		// Token: 0x0600C6C2 RID: 50882 RVA: 0x004936C8 File Offset: 0x004918C8
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool compare(float valueA, float valueB)
		{
			switch (this.operation)
			{
			case BaseOperationRequirement.OperationTypes.Equals:
			case BaseOperationRequirement.OperationTypes.EQ:
			case BaseOperationRequirement.OperationTypes.E:
				return valueA == valueB;
			case BaseOperationRequirement.OperationTypes.NotEquals:
			case BaseOperationRequirement.OperationTypes.NEQ:
			case BaseOperationRequirement.OperationTypes.NE:
				return valueA != valueB;
			case BaseOperationRequirement.OperationTypes.Less:
			case BaseOperationRequirement.OperationTypes.LessThan:
			case BaseOperationRequirement.OperationTypes.LT:
				return valueA < valueB;
			case BaseOperationRequirement.OperationTypes.Greater:
			case BaseOperationRequirement.OperationTypes.GreaterThan:
			case BaseOperationRequirement.OperationTypes.GT:
				return valueA > valueB;
			case BaseOperationRequirement.OperationTypes.LessOrEqual:
			case BaseOperationRequirement.OperationTypes.LessThanOrEqualTo:
			case BaseOperationRequirement.OperationTypes.LTE:
				return valueA <= valueB;
			case BaseOperationRequirement.OperationTypes.GreaterOrEqual:
			case BaseOperationRequirement.OperationTypes.GreaterThanOrEqualTo:
			case BaseOperationRequirement.OperationTypes.GTE:
				return valueA >= valueB;
			default:
				return true;
			}
		}

		// Token: 0x0600C6C3 RID: 50883 RVA: 0x00493758 File Offset: 0x00491958
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool compare(string valueA, string valueB)
		{
			int num = string.Compare(valueA, valueB, this.stringComparison);
			switch (this.operation)
			{
			case BaseOperationRequirement.OperationTypes.Equals:
			case BaseOperationRequirement.OperationTypes.EQ:
			case BaseOperationRequirement.OperationTypes.E:
				return num == 0;
			case BaseOperationRequirement.OperationTypes.NotEquals:
			case BaseOperationRequirement.OperationTypes.NEQ:
			case BaseOperationRequirement.OperationTypes.NE:
				return num != 0;
			case BaseOperationRequirement.OperationTypes.Less:
			case BaseOperationRequirement.OperationTypes.LessThan:
			case BaseOperationRequirement.OperationTypes.LT:
				return num < 0;
			case BaseOperationRequirement.OperationTypes.Greater:
			case BaseOperationRequirement.OperationTypes.GreaterThan:
			case BaseOperationRequirement.OperationTypes.GT:
				return num > 0;
			case BaseOperationRequirement.OperationTypes.LessOrEqual:
			case BaseOperationRequirement.OperationTypes.LessThanOrEqualTo:
			case BaseOperationRequirement.OperationTypes.LTE:
				return num <= 0;
			case BaseOperationRequirement.OperationTypes.GreaterOrEqual:
			case BaseOperationRequirement.OperationTypes.GreaterThanOrEqualTo:
			case BaseOperationRequirement.OperationTypes.GTE:
				return num <= 0;
			default:
				return true;
			}
		}

		// Token: 0x0600C6C4 RID: 50884 RVA: 0x004937F4 File Offset: 0x004919F4
		public override bool CanPerform(Entity target)
		{
			object obj = this.LeftSide(target);
			object obj2 = this.RightSide(target);
			if (obj is string)
			{
				return this.compare((string)obj, (string)obj2);
			}
			return this.compare(Convert.ToSingle(obj), Convert.ToSingle(obj2));
		}

		// Token: 0x0600C6C5 RID: 50885 RVA: 0x004776C6 File Offset: 0x004758C6
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual object LeftSide(Entity target)
		{
			return 0;
		}

		// Token: 0x0600C6C6 RID: 50886 RVA: 0x004776C6 File Offset: 0x004758C6
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual object RightSide(Entity target)
		{
			return 0;
		}

		// Token: 0x0600C6C7 RID: 50887 RVA: 0x0049383E File Offset: 0x00491A3E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<BaseOperationRequirement.OperationTypes>(BaseOperationRequirement.PropOperation, ref this.operation);
		}

		// Token: 0x0600C6C8 RID: 50888 RVA: 0x00493858 File Offset: 0x00491A58
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new BaseOperationRequirement
			{
				Invert = this.Invert,
				operation = this.operation
			};
		}

		// Token: 0x0400960D RID: 38413
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseOperationRequirement.OperationTypes operation = BaseOperationRequirement.OperationTypes.Equals;

		// Token: 0x0400960E RID: 38414
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOperation = "operation";

		// Token: 0x0400960F RID: 38415
		[PublicizedFrom(EAccessModifier.Private)]
		public StringComparison stringComparison;

		// Token: 0x02001926 RID: 6438
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum OperationTypes
		{
			// Token: 0x04009611 RID: 38417
			None,
			// Token: 0x04009612 RID: 38418
			Equals,
			// Token: 0x04009613 RID: 38419
			EQ,
			// Token: 0x04009614 RID: 38420
			E,
			// Token: 0x04009615 RID: 38421
			NotEquals,
			// Token: 0x04009616 RID: 38422
			NEQ,
			// Token: 0x04009617 RID: 38423
			NE,
			// Token: 0x04009618 RID: 38424
			Less,
			// Token: 0x04009619 RID: 38425
			LessThan,
			// Token: 0x0400961A RID: 38426
			LT,
			// Token: 0x0400961B RID: 38427
			Greater,
			// Token: 0x0400961C RID: 38428
			GreaterThan,
			// Token: 0x0400961D RID: 38429
			GT,
			// Token: 0x0400961E RID: 38430
			LessOrEqual,
			// Token: 0x0400961F RID: 38431
			LessThanOrEqualTo,
			// Token: 0x04009620 RID: 38432
			LTE,
			// Token: 0x04009621 RID: 38433
			GreaterOrEqual,
			// Token: 0x04009622 RID: 38434
			GreaterThanOrEqualTo,
			// Token: 0x04009623 RID: 38435
			GTE
		}
	}
}
