using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019F7 RID: 6647
	[Preserve]
	public class ActionPOIReset : BaseAction
	{
		// Token: 0x0600CAA4 RID: 51876 RVA: 0x004A6F82 File Offset: 0x004A5182
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this._state == ActionPOIReset.State.Start)
			{
				this._state = ActionPOIReset.State.Wait;
				this._retVal = BaseAction.ActionCompleteStates.InComplete;
				GameManager.Instance.StartCoroutine(this.onPerformAction());
			}
			return this._retVal;
		}

		// Token: 0x0600CAA5 RID: 51877 RVA: 0x004A6FB1 File Offset: 0x004A51B1
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator onPerformAction()
		{
			Vector3i poiposition = base.Owner.POIPosition;
			World world = GameManager.Instance.World;
			if (base.Owner.POIInstance == null)
			{
				this._retVal = BaseAction.ActionCompleteStates.InCompleteRefund;
				yield break;
			}
			List<PrefabInstance> prefabsIntersecting = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabsIntersecting(base.Owner.POIInstance);
			int entityID = -1;
			if (!GameManager.Instance.IsEditMode() && !GameUtils.IsPlaytesting())
			{
				entityID = ((base.Owner.Requester != null) ? base.Owner.Requester.entityId : -1);
			}
			yield return world.ResetPOIS(prefabsIntersecting, QuestEventManager.manualResetTag, entityID, null, null);
			this._retVal = BaseAction.ActionCompleteStates.Complete;
			yield break;
		}

		// Token: 0x0600CAA6 RID: 51878 RVA: 0x004A6FC0 File Offset: 0x004A51C0
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator UpdateBlocks(List<BlockChangeInfo> blockChanges)
		{
			yield return new WaitForSeconds(1f);
			GameManager.Instance.World.SetBlocksRPC(blockChanges);
			yield break;
		}

		// Token: 0x0600CAA7 RID: 51879 RVA: 0x004A6FCF File Offset: 0x004A51CF
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionPOIReset();
		}

		// Token: 0x0600CAA8 RID: 51880 RVA: 0x004A6FD6 File Offset: 0x004A51D6
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			this._state = ActionPOIReset.State.Start;
		}

		// Token: 0x04009A28 RID: 39464
		[PublicizedFrom(EAccessModifier.Private)]
		public BaseAction.ActionCompleteStates _retVal;

		// Token: 0x04009A29 RID: 39465
		[PublicizedFrom(EAccessModifier.Private)]
		public ActionPOIReset.State _state;

		// Token: 0x020019F8 RID: 6648
		[PublicizedFrom(EAccessModifier.Private)]
		public enum State
		{
			// Token: 0x04009A2B RID: 39467
			Start,
			// Token: 0x04009A2C RID: 39468
			Wait
		}
	}
}
