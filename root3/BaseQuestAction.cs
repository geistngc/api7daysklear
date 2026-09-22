using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

// Token: 0x02000966 RID: 2406
public abstract class BaseQuestAction
{
	// Token: 0x1700076B RID: 1899
	// (get) Token: 0x06004674 RID: 18036 RVA: 0x001BE863 File Offset: 0x001BCA63
	// (set) Token: 0x06004675 RID: 18037 RVA: 0x001BE86B File Offset: 0x001BCA6B
	public Quest OwnerQuest { get; set; }

	// Token: 0x1700076C RID: 1900
	// (get) Token: 0x06004676 RID: 18038 RVA: 0x001BE874 File Offset: 0x001BCA74
	// (set) Token: 0x06004677 RID: 18039 RVA: 0x001BE87C File Offset: 0x001BCA7C
	public QuestClass Owner { get; set; }

	// Token: 0x1700076D RID: 1901
	// (get) Token: 0x06004678 RID: 18040 RVA: 0x001BE885 File Offset: 0x001BCA85
	// (set) Token: 0x06004679 RID: 18041 RVA: 0x001BE88D File Offset: 0x001BCA8D
	public int Phase { get; set; }

	// Token: 0x1700076E RID: 1902
	// (get) Token: 0x0600467A RID: 18042 RVA: 0x001BE896 File Offset: 0x001BCA96
	// (set) Token: 0x0600467B RID: 18043 RVA: 0x001BE89E File Offset: 0x001BCA9E
	public float Delay { get; set; }

	// Token: 0x1700076F RID: 1903
	// (get) Token: 0x0600467C RID: 18044 RVA: 0x001BE8A7 File Offset: 0x001BCAA7
	// (set) Token: 0x0600467D RID: 18045 RVA: 0x001BE8AF File Offset: 0x001BCAAF
	public bool OnComplete { get; set; }

	// Token: 0x0600467E RID: 18046 RVA: 0x001BE8B8 File Offset: 0x001BCAB8
	public BaseQuestAction()
	{
		this.Phase = 1;
	}

	// Token: 0x0600467F RID: 18047 RVA: 0x001BE8C7 File Offset: 0x001BCAC7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CopyValues(BaseQuestAction action)
	{
		action.ID = this.ID;
		action.Value = this.Value;
		action.Phase = this.Phase;
		action.Delay = this.Delay;
		action.OnComplete = this.OnComplete;
	}

	// Token: 0x06004680 RID: 18048 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetupAction()
	{
	}

	// Token: 0x06004681 RID: 18049 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void PerformAction(Quest ownerQuest)
	{
	}

	// Token: 0x06004682 RID: 18050 RVA: 0x001BE905 File Offset: 0x001BCB05
	public void HandlePerformAction()
	{
		if (this.Delay == 0f)
		{
			this.PerformAction(this.OwnerQuest);
			return;
		}
		GameManager.Instance.StartCoroutine(this.PerformActionLater());
	}

	// Token: 0x06004683 RID: 18051 RVA: 0x001BE932 File Offset: 0x001BCB32
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator PerformActionLater()
	{
		yield return new WaitForSeconds(this.Delay);
		if (XUi.IsGameRunning())
		{
			this.PerformAction(this.OwnerQuest);
		}
		yield break;
	}

	// Token: 0x06004684 RID: 18052 RVA: 0x001BE941 File Offset: 0x001BCB41
	public virtual void HandleVariables()
	{
		this.ID = this.OwnerQuest.ParseVariable(this.ID);
		this.Value = this.OwnerQuest.ParseVariable(this.Value);
	}

	// Token: 0x06004685 RID: 18053 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual BaseQuestAction Clone()
	{
		return null;
	}

	// Token: 0x06004686 RID: 18054 RVA: 0x001BE974 File Offset: 0x001BCB74
	public virtual void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		this.Owner.HandleVariablesForProperties(properties);
		if (properties.Values.ContainsKey(BaseQuestAction.PropID))
		{
			this.ID = properties.Values[BaseQuestAction.PropID];
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropValue))
		{
			this.Value = properties.Values[BaseQuestAction.PropValue];
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropPhase))
		{
			this.Phase = (int)Convert.ToByte(properties.Values[BaseQuestAction.PropPhase]);
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropPhase))
		{
			this.Phase = (int)Convert.ToByte(properties.Values[BaseQuestAction.PropPhase]);
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropDelay))
		{
			this.Delay = StringParsers.ParseFloat(properties.Values[BaseQuestAction.PropDelay], 0, -1, NumberStyles.Any);
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropOnComplete))
		{
			this.OnComplete = StringParsers.ParseBool(properties.Values[BaseQuestAction.PropOnComplete], 0, -1, true);
		}
	}

	// Token: 0x040038BE RID: 14526
	public static string PropID = "id";

	// Token: 0x040038BF RID: 14527
	public static string PropValue = "value";

	// Token: 0x040038C0 RID: 14528
	public static string PropPhase = "phase";

	// Token: 0x040038C1 RID: 14529
	public static string PropDelay = "delay";

	// Token: 0x040038C2 RID: 14530
	public static string PropOnComplete = "on_complete";

	// Token: 0x040038C3 RID: 14531
	public string ID;

	// Token: 0x040038C4 RID: 14532
	public string Value;

	// Token: 0x040038CA RID: 14538
	public DynamicProperties Properties;
}
