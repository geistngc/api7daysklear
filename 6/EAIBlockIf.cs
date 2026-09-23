using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000431 RID: 1073
[Preserve]
public class EAIBlockIf : EAIBase
{
	// Token: 0x06002104 RID: 8452 RVA: 0x000C7418 File Offset: 0x000C5618
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 1;
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x000C7428 File Offset: 0x000C5628
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		this.conditions = new List<EAIBlockIf.Condition>();
		string text;
		if (data.TryGetValue("condition", out text))
		{
			string[] array = text.Split(' ', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i += 3)
			{
				EAIBlockIf.Condition condition = new EAIBlockIf.Condition
				{
					type = EnumUtils.Parse<EAIBlockIf.eType>(array[i], true)
				};
				if (condition.type == EAIBlockIf.eType.None)
				{
					Log.Warning("{0} BlockIf type None", new object[]
					{
						this.theEntity.EntityName
					});
				}
				condition.op = EnumUtils.Parse<EAIBlockIf.eOp>(array[i + 1], true);
				if (condition.op == EAIBlockIf.eOp.None)
				{
					Log.Warning("{0} BlockIf op None", new object[]
					{
						this.theEntity.EntityName
					});
				}
				condition.value = StringParsers.ParseFloat(array[i + 2], 0, -1, NumberStyles.Any);
				this.conditions.Add(condition);
			}
		}
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x000C7514 File Offset: 0x000C5714
	public override bool CanExecute()
	{
		int count = this.conditions.Count;
		for (int i = 0; i < count; i++)
		{
			EAIBlockIf.Condition condition = this.conditions[i];
			float v = 0f;
			EAIBlockIf.eType type = condition.type;
			if (type != EAIBlockIf.eType.Alert)
			{
				if (type == EAIBlockIf.eType.Investigate)
				{
					v = (float)(this.theEntity.HasInvestigatePosition ? 1 : 0);
				}
			}
			else
			{
				v = (float)(this.theEntity.IsAlert ? 1 : 0);
			}
			if (this.Compare(condition.op, v, condition.value))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000C75A0 File Offset: 0x000C57A0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Compare(EAIBlockIf.eOp op, float v1, float v2)
	{
		if (op != EAIBlockIf.eOp.e)
		{
			return op == EAIBlockIf.eOp.ne && v1 != v2;
		}
		return v1 == v2;
	}

	// Token: 0x06002108 RID: 8456 RVA: 0x000C7361 File Offset: 0x000C5561
	public override bool Continue()
	{
		return this.CanExecute();
	}

	// Token: 0x040016A1 RID: 5793
	public bool canExecute;

	// Token: 0x040016A2 RID: 5794
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAIBlockIf.Condition> conditions;

	// Token: 0x02000432 RID: 1074
	[PublicizedFrom(EAccessModifier.Private)]
	public enum eType
	{
		// Token: 0x040016A4 RID: 5796
		None,
		// Token: 0x040016A5 RID: 5797
		Alert,
		// Token: 0x040016A6 RID: 5798
		Investigate
	}

	// Token: 0x02000433 RID: 1075
	[PublicizedFrom(EAccessModifier.Private)]
	public enum eOp
	{
		// Token: 0x040016A8 RID: 5800
		None,
		// Token: 0x040016A9 RID: 5801
		e,
		// Token: 0x040016AA RID: 5802
		ne
	}

	// Token: 0x02000434 RID: 1076
	[PublicizedFrom(EAccessModifier.Private)]
	public struct Condition
	{
		// Token: 0x040016AB RID: 5803
		public EAIBlockIf.eType type;

		// Token: 0x040016AC RID: 5804
		public EAIBlockIf.eOp op;

		// Token: 0x040016AD RID: 5805
		public float value;
	}
}
