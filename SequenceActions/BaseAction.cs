using System;
using System.Collections;
using System.Collections.Generic;
using GameEvent.SequenceRequirements;
using UnityEngine;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019DB RID: 6619
	public class BaseAction
	{
		// Token: 0x170018B1 RID: 6321
		// (get) Token: 0x0600CA0B RID: 51723 RVA: 0x004A428F File Offset: 0x004A248F
		// (set) Token: 0x0600CA0C RID: 51724 RVA: 0x004A4298 File Offset: 0x004A2498
		public GameEventActionSequence Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
				if (this.Requirements != null)
				{
					for (int i = 0; i < this.Requirements.Count; i++)
					{
						this.Requirements[i].Owner = value;
					}
				}
			}
		}

		// Token: 0x0600CA0D RID: 51725 RVA: 0x004A42DC File Offset: 0x004A24DC
		public static BaseAction FindKey(string key)
		{
			BaseAction result = null;
			if (!BaseAction.sLookupByKey.TryGetValue(key, out result))
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600CA0E RID: 51726 RVA: 0x004A4300 File Offset: 0x004A2500
		public virtual void SetActionKeyData(int _actionIndex, BaseAction _parent, string prefix = "")
		{
			if (_parent != null)
			{
				this.actionKey = _parent.GetActionKey() + ":" + _actionIndex.ToString();
			}
			else
			{
				this.actionKey = prefix + _actionIndex.ToString();
			}
			BaseAction.sLookupByKey[this.actionKey] = this;
		}

		// Token: 0x0600CA0F RID: 51727 RVA: 0x004A4353 File Offset: 0x004A2553
		public string GetActionKey()
		{
			return this.actionKey;
		}

		// Token: 0x170018B2 RID: 6322
		// (get) Token: 0x0600CA10 RID: 51728 RVA: 0x0002003D File Offset: 0x0001E23D
		public virtual bool UseRequirements
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600CA11 RID: 51729 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600CA12 RID: 51730 RVA: 0x004A435B File Offset: 0x004A255B
		public void Init()
		{
			this.OnInit();
			this.IsComplete = false;
		}

		// Token: 0x0600CA13 RID: 51731 RVA: 0x0002003D File Offset: 0x0001E23D
		public virtual bool CanPerform(Entity target)
		{
			return true;
		}

		// Token: 0x0600CA14 RID: 51732 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void OnClientPerform(Entity target)
		{
		}

		// Token: 0x0600CA15 RID: 51733 RVA: 0x0002003D File Offset: 0x0001E23D
		public virtual BaseAction.ActionCompleteStates OnPerformAction()
		{
			return BaseAction.ActionCompleteStates.InCompleteRefund;
		}

		// Token: 0x0600CA16 RID: 51734 RVA: 0x004A436C File Offset: 0x004A256C
		public BaseAction.ActionCompleteStates PerformAction()
		{
			if (this.UseRequirements && this.Requirements != null)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					this.Requirements[i].Owner = this.Owner;
					if (!this.Requirements[i].CanPerform(this.Owner.Target))
					{
						return BaseAction.ActionCompleteStates.RequirementsNotMet;
					}
				}
			}
			return this.OnPerformAction();
		}

		// Token: 0x0600CA17 RID: 51735 RVA: 0x004A43DC File Offset: 0x004A25DC
		public void Reset()
		{
			this.IsComplete = false;
			this.OnReset();
		}

		// Token: 0x0600CA18 RID: 51736 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnReset()
		{
		}

		// Token: 0x0600CA19 RID: 51737 RVA: 0x004A43EC File Offset: 0x004A25EC
		public virtual void ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			this.Owner.HandleVariablesForProperties(properties);
			properties.ParseInt(BaseAction.PropPhase, ref this.Phase);
			properties.ParseInt(BaseAction.PropPhaseOnComplete, ref this.PhaseOnComplete);
			properties.ParseInt(BaseAction.PropPhaseOnDenied, ref this.PhaseOnDenied);
			properties.ParseBool(BaseAction.PropIgnoreRefund, ref this.IgnoreRefund);
		}

		// Token: 0x0600CA1A RID: 51738 RVA: 0x004A4450 File Offset: 0x004A2650
		public virtual void HandleTemplateInit(GameEventActionSequence seq)
		{
			seq.HandleVariablesForProperties(this.Properties);
			this.Owner = seq;
			if (this.Properties != null)
			{
				this.ParseProperties(this.Properties);
			}
			this.Init();
			if (this.Requirements != null)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					seq.HandleVariablesForProperties(this.Requirements[i].Properties);
					if (this.Requirements[i].Properties != null)
					{
						this.Requirements[i].ParseProperties(this.Requirements[i].Properties);
					}
					this.Requirements[i].Init();
				}
			}
		}

		// Token: 0x0600CA1B RID: 51739 RVA: 0x004A4505 File Offset: 0x004A2705
		public void AddRequirement(BaseRequirement req)
		{
			if (this.Requirements == null)
			{
				this.Requirements = new List<BaseRequirement>();
			}
			req.Owner = this.Owner;
			this.Requirements.Add(req);
		}

		// Token: 0x0600CA1C RID: 51740 RVA: 0x004A4534 File Offset: 0x004A2734
		public virtual BaseAction Clone()
		{
			BaseAction baseAction = this.CloneChildSettings();
			if (this.Properties != null)
			{
				baseAction.Properties = new DynamicProperties();
				baseAction.Properties.CopyFrom(this.Properties, null);
			}
			baseAction.Phase = this.Phase;
			baseAction.PhaseOnComplete = this.PhaseOnComplete;
			baseAction.PhaseOnDenied = this.PhaseOnDenied;
			baseAction.IsComplete = false;
			baseAction.actionKey = this.actionKey;
			baseAction.IgnoreRefund = this.IgnoreRefund;
			if (this.Requirements != null)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					baseAction.AddRequirement(this.Requirements[i].Clone());
				}
			}
			return baseAction;
		}

		// Token: 0x0600CA1D RID: 51741 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual BaseAction CloneChildSettings()
		{
			return null;
		}

		// Token: 0x0600CA1E RID: 51742 RVA: 0x004A45E6 File Offset: 0x004A27E6
		public IEnumerator TeleportEntity(Entity entity, Vector3 position, float teleportDelay)
		{
			yield return new WaitForSeconds(teleportDelay);
			EntityPlayer entityPlayer = entity as EntityPlayer;
			if (entityPlayer != null)
			{
				if (entityPlayer.isEntityRemote)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityPlayer.entityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
					SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityPlayer.entityId).SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(position, null, false));
				}
				else
				{
					((EntityPlayerLocal)entityPlayer).PlayerUI.windowManager.CloseAllOpenModalWindows(null, false);
					((EntityPlayerLocal)entityPlayer).TeleportToPosition(position, false, null);
				}
			}
			else if (entity.AttachedToEntity != null)
			{
				entity.AttachedToEntity.SetPosition(position, true);
			}
			else
			{
				entity.SetPosition(position, true);
			}
			yield break;
		}

		// Token: 0x0600CA1F RID: 51743 RVA: 0x000149AE File Offset: 0x00012BAE
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual string ParseTextElement(string element)
		{
			return element;
		}

		// Token: 0x0600CA20 RID: 51744 RVA: 0x004A4604 File Offset: 0x004A2804
		[PublicizedFrom(EAccessModifier.Protected)]
		public string GetTextWithElements(string text)
		{
			int num = text.IndexOf("{", StringComparison.Ordinal);
			Dictionary<string, string> dictionary = null;
			while (num != -1)
			{
				int num2 = text.IndexOf("}", num, StringComparison.Ordinal);
				if (num2 == -1)
				{
					break;
				}
				string text2 = text.Substring(num + 1, num2 - num - 1);
				string text3 = this.ParseTextElement(text2);
				if (text3 != text2)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<string, string>();
					}
					dictionary.Add(text.Substring(num, num2 - num + 1), text3);
				}
				num = text.IndexOf("{", num2, StringComparison.Ordinal);
			}
			if (dictionary != null)
			{
				foreach (string text4 in dictionary.Keys)
				{
					text = text.Replace(text4, dictionary[text4]);
				}
			}
			return text;
		}

		// Token: 0x0600CA21 RID: 51745 RVA: 0x004A46DC File Offset: 0x004A28DC
		public virtual BaseAction HandleAssignFrom(GameEventActionSequence newSeq, GameEventActionSequence oldSeq)
		{
			BaseAction baseAction = this.Clone();
			baseAction.Properties = new DynamicProperties();
			if (this.Properties != null)
			{
				baseAction.Properties.CopyFrom(this.Properties, null);
			}
			baseAction.Owner = newSeq;
			if (baseAction.Requirements != null)
			{
				for (int i = 0; i < baseAction.Requirements.Count; i++)
				{
					baseAction.Requirements[i].Properties = new DynamicProperties();
					if (this.Requirements[i].Properties != null)
					{
						baseAction.Requirements[i].Properties.CopyFrom(this.Requirements[i].Properties, null);
					}
					baseAction.Requirements[i].Owner = newSeq;
					baseAction.Requirements[i].Init();
				}
			}
			return baseAction;
		}

		// Token: 0x0400999D RID: 39325
		[PublicizedFrom(EAccessModifier.Protected)]
		public static Dictionary<string, BaseAction> sLookupByKey = new Dictionary<string, BaseAction>();

		// Token: 0x0400999E RID: 39326
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameEventActionSequence owner;

		// Token: 0x0400999F RID: 39327
		public int Phase;

		// Token: 0x040099A0 RID: 39328
		public int PhaseOnComplete = -1;

		// Token: 0x040099A1 RID: 39329
		public int PhaseOnDenied = -1;

		// Token: 0x040099A2 RID: 39330
		[PublicizedFrom(EAccessModifier.Protected)]
		public string actionKey = "";

		// Token: 0x040099A3 RID: 39331
		public bool IgnoreRefund;

		// Token: 0x040099A4 RID: 39332
		public bool IsComplete;

		// Token: 0x040099A5 RID: 39333
		public DynamicProperties Properties;

		// Token: 0x040099A6 RID: 39334
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPhase = "phase";

		// Token: 0x040099A7 RID: 39335
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPhaseOnComplete = "phase_on_complete";

		// Token: 0x040099A8 RID: 39336
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPhaseOnDenied = "phase_on_denied";

		// Token: 0x040099A9 RID: 39337
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIgnoreRefund = "ignore_refund";

		// Token: 0x040099AA RID: 39338
		public List<BaseRequirement> Requirements;

		// Token: 0x020019DC RID: 6620
		public enum ActionCompleteStates
		{
			// Token: 0x040099AC RID: 39340
			InComplete,
			// Token: 0x040099AD RID: 39341
			InCompleteRefund,
			// Token: 0x040099AE RID: 39342
			RequirementsNotMet,
			// Token: 0x040099AF RID: 39343
			Complete
		}
	}
}
