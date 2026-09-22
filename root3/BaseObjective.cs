using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000980 RID: 2432
public abstract class BaseObjective
{
	// Token: 0x14000051 RID: 81
	// (add) Token: 0x0600471A RID: 18202 RVA: 0x001C059C File Offset: 0x001BE79C
	// (remove) Token: 0x0600471B RID: 18203 RVA: 0x001C05D4 File Offset: 0x001BE7D4
	public event ObjectiveValueChanged ValueChanged;

	// Token: 0x1700077B RID: 1915
	// (get) Token: 0x0600471C RID: 18204 RVA: 0x001C0609 File Offset: 0x001BE809
	// (set) Token: 0x0600471D RID: 18205 RVA: 0x001C0611 File Offset: 0x001BE811
	public byte CurrentVersion { get; set; }

	// Token: 0x1700077C RID: 1916
	// (get) Token: 0x0600471E RID: 18206 RVA: 0x001C061A File Offset: 0x001BE81A
	// (set) Token: 0x0600471F RID: 18207 RVA: 0x001C0622 File Offset: 0x001BE822
	public BaseObjective.ObjectiveStates ObjectiveState { get; set; }

	// Token: 0x1700077D RID: 1917
	// (get) Token: 0x06004720 RID: 18208 RVA: 0x001C062B File Offset: 0x001BE82B
	// (set) Token: 0x06004721 RID: 18209 RVA: 0x001C0641 File Offset: 0x001BE841
	public bool Complete
	{
		get
		{
			return this.ObjectiveState == BaseObjective.ObjectiveStates.Complete || this.ObjectiveState == BaseObjective.ObjectiveStates.Warning;
		}
		set
		{
			if (value)
			{
				this.ObjectiveState = BaseObjective.ObjectiveStates.Complete;
				this.DisableModifiers();
			}
		}
	}

	// Token: 0x1700077E RID: 1918
	// (get) Token: 0x06004722 RID: 18210 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool useUpdateLoop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return false;
		}
	}

	// Token: 0x1700077F RID: 1919
	// (get) Token: 0x06004723 RID: 18211 RVA: 0x001C0653 File Offset: 0x001BE853
	// (set) Token: 0x06004724 RID: 18212 RVA: 0x001C065B File Offset: 0x001BE85B
	public QuestClass OwnerQuestClass { get; set; }

	// Token: 0x17000780 RID: 1920
	// (get) Token: 0x06004725 RID: 18213 RVA: 0x001C0664 File Offset: 0x001BE864
	// (set) Token: 0x06004726 RID: 18214 RVA: 0x001C066C File Offset: 0x001BE86C
	public Quest OwnerQuest { get; set; }

	// Token: 0x17000781 RID: 1921
	// (get) Token: 0x06004727 RID: 18215 RVA: 0x001C0675 File Offset: 0x001BE875
	// (set) Token: 0x06004728 RID: 18216 RVA: 0x001C067D File Offset: 0x001BE87D
	public byte Phase { get; set; }

	// Token: 0x06004729 RID: 18217 RVA: 0x001C0688 File Offset: 0x001BE888
	public BaseObjective()
	{
		this.ObjectiveState = BaseObjective.ObjectiveStates.NotStarted;
		this.Phase = 1;
	}

	// Token: 0x17000782 RID: 1922
	// (get) Token: 0x0600472A RID: 18218 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Boolean;
		}
	}

	// Token: 0x17000783 RID: 1923
	// (get) Token: 0x0600472B RID: 18219 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool PlayObjectiveComplete
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000784 RID: 1924
	// (get) Token: 0x0600472C RID: 18220 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool RequiresZombies
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600472D RID: 18221 RVA: 0x001C06D8 File Offset: 0x001BE8D8
	[PublicizedFrom(EAccessModifier.Internal)]
	public void ChangeStatus(bool isSuccess)
	{
		this.ObjectiveState = (isSuccess ? BaseObjective.ObjectiveStates.Complete : BaseObjective.ObjectiveStates.Failed);
		if (isSuccess)
		{
			this.OwnerQuest.RallyMarkerActivated = true;
			this.OwnerQuest.RemoveMapObject();
			this.OwnerQuest.Tracked = true;
			this.OwnerQuest.OwnerJournal.TrackedQuest = this.OwnerQuest;
			this.OwnerQuest.OwnerJournal.RefreshTracked();
			this.OwnerQuest.OwnerJournal.ActiveQuest = this.OwnerQuest;
			this.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
			return;
		}
		this.OwnerQuest.CloseQuest(Quest.QuestState.Failed, null);
	}

	// Token: 0x17000785 RID: 1925
	// (get) Token: 0x0600472E RID: 18222 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool UpdateUI
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000786 RID: 1926
	// (get) Token: 0x0600472F RID: 18223 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool NeedsNPCSetPosition
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000787 RID: 1927
	// (get) Token: 0x06004730 RID: 18224 RVA: 0x001C0771 File Offset: 0x001BE971
	// (set) Token: 0x06004731 RID: 18225 RVA: 0x001C078E File Offset: 0x001BE98E
	public string Description
	{
		get
		{
			if (!this.displaySetup)
			{
				this.SetupDisplay();
				this.displaySetup = true;
			}
			return this.description;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.description = value;
		}
	}

	// Token: 0x17000788 RID: 1928
	// (get) Token: 0x06004732 RID: 18226 RVA: 0x001C0797 File Offset: 0x001BE997
	// (set) Token: 0x06004733 RID: 18227 RVA: 0x001C07B4 File Offset: 0x001BE9B4
	public virtual string StatusText
	{
		get
		{
			if (!this.displaySetup)
			{
				this.SetupDisplay();
				this.displaySetup = true;
			}
			return this.statusText;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.statusText = value;
		}
	}

	// Token: 0x17000789 RID: 1929
	// (get) Token: 0x06004734 RID: 18228 RVA: 0x001C07BD File Offset: 0x001BE9BD
	// (set) Token: 0x06004735 RID: 18229 RVA: 0x001C07C5 File Offset: 0x001BE9C5
	public byte CurrentValue
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			this.currentValue = value;
			this.SetupDisplay();
			if (this.ValueChanged != null)
			{
				this.ValueChanged();
			}
		}
	}

	// Token: 0x1700078A RID: 1930
	// (get) Token: 0x06004736 RID: 18230 RVA: 0x001C07E7 File Offset: 0x001BE9E7
	// (set) Token: 0x06004737 RID: 18231 RVA: 0x001C07EF File Offset: 0x001BE9EF
	public bool Optional { get; set; }

	// Token: 0x1700078B RID: 1931
	// (get) Token: 0x06004738 RID: 18232 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool AlwaysComplete
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700078C RID: 1932
	// (get) Token: 0x06004739 RID: 18233 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool ShowInQuestLog
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600473A RID: 18234 RVA: 0x001C07F8 File Offset: 0x001BE9F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void CopyValues(BaseObjective objective)
	{
		objective.ID = this.ID;
		objective.Value = this.Value;
		objective.Optional = this.Optional;
		objective.currentValue = this.currentValue;
		objective.Phase = this.Phase;
		objective.NavObjectName = this.NavObjectName;
		objective.HiddenObjective = this.HiddenObjective;
		objective.ForcePhaseFinish = this.ForcePhaseFinish;
		if (this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				objective.AddModifier(this.Modifiers[i].Clone());
			}
		}
	}

	// Token: 0x0600473B RID: 18235 RVA: 0x001C089A File Offset: 0x001BEA9A
	public virtual void HandleVariables()
	{
		this.ID = this.OwnerQuest.ParseVariable(this.ID);
		this.Value = this.OwnerQuest.ParseVariable(this.Value);
	}

	// Token: 0x0600473C RID: 18236 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetupQuestTag()
	{
	}

	// Token: 0x0600473D RID: 18237 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetupObjective()
	{
	}

	// Token: 0x0600473E RID: 18238 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetupDisplay()
	{
	}

	// Token: 0x0600473F RID: 18239 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool SetupPosition(EntityNPC ownerNPC = null, EntityPlayer player = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		return false;
	}

	// Token: 0x06004740 RID: 18240 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool SetupActivationList(Vector3 prefabPos, List<Vector3i> activateList)
	{
		return false;
	}

	// Token: 0x06004741 RID: 18241 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetPosition(Vector3 position, Vector3 size)
	{
	}

	// Token: 0x06004742 RID: 18242 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetPosition(Quest.PositionDataTypes dataType, Vector3i position)
	{
	}

	// Token: 0x06004743 RID: 18243 RVA: 0x001C08CC File Offset: 0x001BEACC
	public void HandleAddHooks()
	{
		this.AddHooks();
		if (!this.Complete && this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				this.Modifiers[i].OwnerObjective = this;
				this.Modifiers[i].HandleAddHooks();
			}
		}
		if (this.useUpdateLoop)
		{
			QuestEventManager.Current.AddObjectiveToBeUpdated(this);
		}
	}

	// Token: 0x06004744 RID: 18244 RVA: 0x001C093C File Offset: 0x001BEB3C
	public void HandleRemoveHooks()
	{
		this.RemoveHooks();
		this.RemoveNavObject();
		if (this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				this.Modifiers[i].HandleRemoveHooks();
			}
		}
		if (this.useUpdateLoop)
		{
			QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
		}
	}

	// Token: 0x06004745 RID: 18245 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void AddHooks()
	{
	}

	// Token: 0x06004746 RID: 18246 RVA: 0x001C0997 File Offset: 0x001BEB97
	public virtual void AddNavObject(Vector3 position)
	{
		if (this.NavObjectName != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(this.NavObjectName, position, "", false, -1, null);
		}
	}

	// Token: 0x06004747 RID: 18247 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void RemoveHooks()
	{
	}

	// Token: 0x06004748 RID: 18248 RVA: 0x001C09CA File Offset: 0x001BEBCA
	public virtual void RemoveNavObject()
	{
		if (this.NavObject != null)
		{
			NavObjectManager.Instance.UnRegisterNavObject(this.NavObject);
			this.NavObject = null;
		}
	}

	// Token: 0x06004749 RID: 18249 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Refresh()
	{
	}

	// Token: 0x0600474A RID: 18250 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void RemoveObjectives()
	{
	}

	// Token: 0x0600474B RID: 18251 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void HandleCompleted()
	{
	}

	// Token: 0x0600474C RID: 18252 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void HandlePhaseCompleted()
	{
	}

	// Token: 0x0600474D RID: 18253 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void HandleFailed()
	{
	}

	// Token: 0x0600474E RID: 18254 RVA: 0x001C09EB File Offset: 0x001BEBEB
	public virtual void ResetObjective()
	{
		this.CurrentValue = 0;
	}

	// Token: 0x0600474F RID: 18255 RVA: 0x001C09F4 File Offset: 0x001BEBF4
	public virtual void Read(BinaryReader _br)
	{
		this.CurrentVersion = _br.ReadByte();
		this.currentValue = _br.ReadByte();
	}

	// Token: 0x06004750 RID: 18256 RVA: 0x001C0A0E File Offset: 0x001BEC0E
	public virtual void Write(BinaryWriter _bw)
	{
		_bw.Write(BaseObjective.FileVersion);
		_bw.Write(this.CurrentValue);
	}

	// Token: 0x06004751 RID: 18257 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual BaseObjective Clone()
	{
		return null;
	}

	// Token: 0x06004752 RID: 18258 RVA: 0x001C0A27 File Offset: 0x001BEC27
	public void HandleUpdate(float deltaTime)
	{
		if (this.Phase == this.OwnerQuest.CurrentPhase)
		{
			this.Update(deltaTime);
		}
	}

	// Token: 0x06004753 RID: 18259 RVA: 0x001C0A44 File Offset: 0x001BEC44
	public virtual void Update(float deltaTime)
	{
		if (Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 1f;
			switch (this.CurrentValue)
			{
			case 0:
				this.UpdateState_NeedSetup();
				return;
			case 1:
				this.UpdateState_WaitingForServer();
				return;
			case 2:
				this.UpdateState_Update();
				return;
			case 3:
				this.UpdateState_Completed();
				QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06004754 RID: 18260 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_NeedSetup()
	{
	}

	// Token: 0x06004755 RID: 18261 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_WaitingForServer()
	{
	}

	// Token: 0x06004756 RID: 18262 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_Update()
	{
	}

	// Token: 0x06004757 RID: 18263 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateState_Completed()
	{
	}

	// Token: 0x06004758 RID: 18264 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool SetLocation(Vector3 pos, Vector3 size)
	{
		return false;
	}

	// Token: 0x06004759 RID: 18265 RVA: 0x00032163 File Offset: 0x00030363
	public virtual string ParseBinding(string bindingName)
	{
		return "";
	}

	// Token: 0x0600475A RID: 18266 RVA: 0x001C0AB4 File Offset: 0x001BECB4
	public virtual void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		this.OwnerQuestClass.HandleVariablesForProperties(properties);
		if (properties.Values.ContainsKey(BaseObjective.PropID))
		{
			this.ID = properties.Values[BaseObjective.PropID];
		}
		if (properties.Values.ContainsKey(BaseObjective.PropValue))
		{
			this.Value = properties.Values[BaseObjective.PropValue];
		}
		if (properties.Values.ContainsKey(BaseObjective.PropPhase))
		{
			this.Phase = Convert.ToByte(properties.Values[BaseObjective.PropPhase]);
			if (this.Phase > this.OwnerQuestClass.HighestPhase)
			{
				this.OwnerQuestClass.HighestPhase = this.Phase;
			}
		}
		if (properties.Values.ContainsKey(BaseObjective.PropOptional))
		{
			bool optional;
			StringParsers.TryParseBool(properties.Values[BaseObjective.PropOptional], out optional);
			this.Optional = optional;
		}
		if (properties.Values.ContainsKey(BaseObjective.PropNavObject))
		{
			this.NavObjectName = properties.Values[BaseObjective.PropNavObject];
		}
		if (properties.Values.ContainsKey(BaseObjective.PropHidden))
		{
			this.HiddenObjective = StringParsers.ParseBool(properties.Values[BaseObjective.PropHidden], 0, -1, true);
		}
		properties.ParseBool(BaseObjective.PropForcePhaseFinish, ref this.ForcePhaseFinish);
	}

	// Token: 0x0600475B RID: 18267 RVA: 0x001C0C0F File Offset: 0x001BEE0F
	public void AddModifier(BaseObjectiveModifier modifier)
	{
		if (this.Modifiers == null)
		{
			this.Modifiers = new List<BaseObjectiveModifier>();
		}
		this.Modifiers.Add(modifier);
		modifier.OwnerObjective = this;
	}

	// Token: 0x0600475C RID: 18268 RVA: 0x001C0C38 File Offset: 0x001BEE38
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisableModifiers()
	{
		if (this.Modifiers != null)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				this.Modifiers[i].HandleRemoveHooks();
			}
		}
	}

	// Token: 0x04003916 RID: 14614
	public static byte FileVersion = 0;

	// Token: 0x04003917 RID: 14615
	public static string PropID = "id";

	// Token: 0x04003918 RID: 14616
	public static string PropValue = "value";

	// Token: 0x04003919 RID: 14617
	public static string PropPhase = "phase";

	// Token: 0x0400391A RID: 14618
	public static string PropOptional = "optional";

	// Token: 0x0400391B RID: 14619
	public static string PropNavObject = "nav_object";

	// Token: 0x0400391C RID: 14620
	public static string PropHidden = "hidden";

	// Token: 0x0400391D RID: 14621
	public static string PropForcePhaseFinish = "force_phase_finish";

	// Token: 0x0400391F RID: 14623
	public string ID;

	// Token: 0x04003921 RID: 14625
	public string Value;

	// Token: 0x04003925 RID: 14629
	[PublicizedFrom(EAccessModifier.Private)]
	public bool displaySetup;

	// Token: 0x04003927 RID: 14631
	[PublicizedFrom(EAccessModifier.Protected)]
	public string keyword = "";

	// Token: 0x04003928 RID: 14632
	[PublicizedFrom(EAccessModifier.Private)]
	public string description = "";

	// Token: 0x04003929 RID: 14633
	[PublicizedFrom(EAccessModifier.Private)]
	public string statusText = "";

	// Token: 0x0400392A RID: 14634
	public bool HiddenObjective;

	// Token: 0x0400392B RID: 14635
	public bool ForcePhaseFinish;

	// Token: 0x0400392C RID: 14636
	[PublicizedFrom(EAccessModifier.Protected)]
	public NavObject NavObject;

	// Token: 0x0400392D RID: 14637
	[PublicizedFrom(EAccessModifier.Protected)]
	public string NavObjectName = "";

	// Token: 0x0400392E RID: 14638
	public List<BaseObjectiveModifier> Modifiers;

	// Token: 0x0400392F RID: 14639
	public DynamicProperties Properties;

	// Token: 0x04003930 RID: 14640
	[PublicizedFrom(EAccessModifier.Protected)]
	public byte currentValue;

	// Token: 0x04003932 RID: 14642
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;

	// Token: 0x02000981 RID: 2433
	public enum ObjectiveStates
	{
		// Token: 0x04003934 RID: 14644
		NotStarted,
		// Token: 0x04003935 RID: 14645
		InProgress,
		// Token: 0x04003936 RID: 14646
		Warning,
		// Token: 0x04003937 RID: 14647
		Complete,
		// Token: 0x04003938 RID: 14648
		Failed
	}

	// Token: 0x02000982 RID: 2434
	public enum ObjectiveTypes
	{
		// Token: 0x0400393A RID: 14650
		AnimalKill,
		// Token: 0x0400393B RID: 14651
		Assemble,
		// Token: 0x0400393C RID: 14652
		BlockPickup,
		// Token: 0x0400393D RID: 14653
		BlockPlace,
		// Token: 0x0400393E RID: 14654
		BlockUpgrade,
		// Token: 0x0400393F RID: 14655
		Buff,
		// Token: 0x04003940 RID: 14656
		ExchangeItemFrom,
		// Token: 0x04003941 RID: 14657
		Fetch,
		// Token: 0x04003942 RID: 14658
		FetchKeep,
		// Token: 0x04003943 RID: 14659
		CraftItem,
		// Token: 0x04003944 RID: 14660
		Repair,
		// Token: 0x04003945 RID: 14661
		Scrap,
		// Token: 0x04003946 RID: 14662
		SkillsPurchased,
		// Token: 0x04003947 RID: 14663
		Time,
		// Token: 0x04003948 RID: 14664
		Wear,
		// Token: 0x04003949 RID: 14665
		WindowOpen,
		// Token: 0x0400394A RID: 14666
		ZombieKill
	}

	// Token: 0x02000983 RID: 2435
	public enum ObjectiveValueTypes
	{
		// Token: 0x0400394C RID: 14668
		Boolean,
		// Token: 0x0400394D RID: 14669
		Number,
		// Token: 0x0400394E RID: 14670
		Time,
		// Token: 0x0400394F RID: 14671
		Distance
	}

	// Token: 0x02000984 RID: 2436
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum UpdateStates
	{
		// Token: 0x04003951 RID: 14673
		NeedSetup,
		// Token: 0x04003952 RID: 14674
		WaitingForServer,
		// Token: 0x04003953 RID: 14675
		Update,
		// Token: 0x04003954 RID: 14676
		Completed
	}
}
