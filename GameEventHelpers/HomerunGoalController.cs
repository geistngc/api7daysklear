using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace GameEvent.GameEventHelpers
{
	// Token: 0x02001952 RID: 6482
	public class HomerunGoalController : MonoBehaviour
	{
		// Token: 0x0600C7AC RID: 51116 RVA: 0x00495784 File Offset: 0x00493984
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			if (GameManager.Instance.World == null)
			{
				this.ReadyForDelete = true;
				return;
			}
			switch (this.direction)
			{
			case HomerunGoalController.Direction.YPositive:
				this.position = this.StartPosition + Vector3.up * Mathf.PingPong(Time.time, 2f) * 2f;
				base.transform.position = this.position - Origin.position;
				break;
			case HomerunGoalController.Direction.XPositive:
				this.position = this.StartPosition + Vector3.right * Mathf.PingPong(Time.time, 2f) * 2f;
				base.transform.position = this.position - Origin.position;
				break;
			case HomerunGoalController.Direction.XNegative:
				this.position = this.StartPosition + Vector3.left * Mathf.PingPong(Time.time, 2f) * 2f;
				base.transform.position = this.position - Origin.position;
				break;
			case HomerunGoalController.Direction.ZPositive:
				this.position = this.StartPosition + Vector3.forward * Mathf.PingPong(Time.time, 2f) * 2f;
				base.transform.position = this.position - Origin.position;
				break;
			case HomerunGoalController.Direction.ZNegative:
				this.position = this.StartPosition + Vector3.back * Mathf.PingPong(Time.time, 2f) * 2f;
				base.transform.position = this.position - Origin.position;
				break;
			}
			if (Vector3.Distance(this.position, this.Owner.Player.position) > 50f)
			{
				this.ReadyForDelete = true;
				return;
			}
			List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(null, new Bounds(this.position, Vector3.one * this.Size));
			for (int i = 0; i < entitiesInBounds.Count; i++)
			{
				EntityAlive entityAlive = entitiesInBounds[i] as EntityAlive;
				if (entityAlive != null && entityAlive != null && entityAlive.IsAlive() && !(entityAlive is EntityPlayer) && entityAlive.emodel != null && entityAlive.emodel.transform != null && entityAlive.emodel.IsRagdollActive)
				{
					World world = GameManager.Instance.World;
					float lightBrightness = world.GetLightBrightness(entityAlive.GetBlockPosition());
					world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect("twitch_fireworks", entityAlive.position, lightBrightness, Color.white, null, null, false, 1f, ""), entityAlive.entityId, false, true);
					Manager.BroadcastPlayByLocalPlayer(entityAlive.position, "twitch_celebrate");
					entityAlive.DamageEntity(new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suicide), 99999, false, 1f);
					GameManager.Instance.World.RemoveEntity(entityAlive.entityId, EnumRemoveEntityReason.Killed);
					if (!this.ReadyForDelete)
					{
						this.Owner.Score += this.ScoreAdded;
						this.ReadyForDelete = true;
					}
				}
			}
		}

		// Token: 0x04009692 RID: 38546
		public HomerunData Owner;

		// Token: 0x04009693 RID: 38547
		public Vector3 position;

		// Token: 0x04009694 RID: 38548
		public bool ReadyForDelete;

		// Token: 0x04009695 RID: 38549
		public int ScoreAdded = 1;

		// Token: 0x04009696 RID: 38550
		public float Size = 2f;

		// Token: 0x04009697 RID: 38551
		public Vector3 StartPosition;

		// Token: 0x04009698 RID: 38552
		public HomerunGoalController.Direction direction;

		// Token: 0x02001953 RID: 6483
		public enum Direction
		{
			// Token: 0x0400969A RID: 38554
			YPositive,
			// Token: 0x0400969B RID: 38555
			XPositive,
			// Token: 0x0400969C RID: 38556
			XNegative,
			// Token: 0x0400969D RID: 38557
			ZPositive,
			// Token: 0x0400969E RID: 38558
			ZNegative,
			// Token: 0x0400969F RID: 38559
			Max
		}
	}
}
