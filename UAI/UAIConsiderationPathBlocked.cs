using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x02001779 RID: 6009
	[Preserve]
	public class UAIConsiderationPathBlocked : UAIConsiderationBase
	{
		// Token: 0x0600BA58 RID: 47704 RVA: 0x00458BF8 File Offset: 0x00456DF8
		public override float GetScore(Context _context, object target)
		{
			Vector3i zero = Vector3i.zero;
			if (this.IsPathUsageBlocked(_context) && UAIConsiderationPathBlocked.CanAttackBlocks(_context.Self, out zero))
			{
				_context.ConsiderationData.WaypointTargets.Add(zero.ToVector3());
				return 1f;
			}
			return 0f;
		}

		// Token: 0x0600BA59 RID: 47705 RVA: 0x00458C48 File Offset: 0x00456E48
		public bool IsPathUsageBlocked(Context _context)
		{
			EntityAlive self = _context.Self;
			if (self.getNavigator() == null)
			{
				return false;
			}
			if (self.getNavigator().getPath() == null)
			{
				return false;
			}
			Vector3 targetPos = UAIConsiderationPathBlocked.GetTargetPos(self);
			Vector3i vector3i = World.worldToBlockPos(targetPos);
			float distanceSq = self.getNavigator().getPath().GetEndPoint().GetDistanceSq(vector3i.x, vector3i.y, vector3i.z);
			if (distanceSq < 2.1f)
			{
				return false;
			}
			float distanceSq2 = self.GetDistanceSq(targetPos);
			return self.GetDistanceSq(targetPos) < 256f || distanceSq > distanceSq2;
		}

		// Token: 0x0600BA5A RID: 47706 RVA: 0x00458CD4 File Offset: 0x00456ED4
		public static bool CanAttackBlocks(EntityAlive theEntity, out Vector3i attackPos)
		{
			float num;
			BlockValue blockValue;
			return UAIConsiderationPathBlocked.CanAttackBlocks(theEntity, UAIConsiderationPathBlocked.GetTargetYaw(theEntity), out num, out attackPos, out blockValue);
		}

		// Token: 0x0600BA5B RID: 47707 RVA: 0x00458CF4 File Offset: 0x00456EF4
		public static bool CanAttackBlocks(EntityAlive theEntity, float yawAngle, out float attackAngle, out Vector3i attackAddPos, out BlockValue attackBlockValue)
		{
			int num = Utils.Fastfloor(theEntity.position.x);
			int num2 = Utils.Fastfloor(theEntity.position.y + 0.5f);
			int num3 = Utils.Fastfloor(theEntity.position.z);
			attackAddPos = new Vector3i(0, 1, 0);
			bool flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num, num2 + 1, num3, out attackBlockValue);
			attackAngle = 0f;
			if (!flag)
			{
				attackAddPos = new Vector3i(0, 0, 0);
				flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num, num2, num3, out attackBlockValue);
				attackAngle = -65f;
			}
			if (!flag)
			{
				int num4 = 0;
				int num5 = 0;
				float f = -Mathf.Sin(yawAngle * 0.0175f - 3.1415927f);
				float f2 = -Mathf.Cos(yawAngle * 0.0175f - 3.1415927f);
				if (Mathf.Abs(f) > 0.1f)
				{
					num4 = (int)Mathf.Sign(f);
				}
				if (Mathf.Abs(f2) > 0.1f)
				{
					num5 = (int)Mathf.Sign(f2);
				}
				attackAddPos = new Vector3i(num4, 1, num5);
				flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num + num4, num2 + 1, num3 + num5, out attackBlockValue);
				attackAngle = 0f;
				if (!flag)
				{
					attackAddPos = new Vector3i(num4, 0, num5);
					flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num + num4, num2, num3 + num5, out attackBlockValue);
					attackAngle = -45f;
				}
				if (!flag)
				{
					attackAddPos = new Vector3i(2 * num4, 1, 2 * num5);
					flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num + 2 * num4, num2 + 1, num3 + 2 * num5, out attackBlockValue);
					attackAngle = 0f;
				}
				if (!flag)
				{
					attackAddPos = new Vector3i(2 * num4, 0, 2 * num5);
					flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num + 2 * num4, num2, num3 + 2 * num5, out attackBlockValue);
					attackAngle = -45f;
				}
				if (!flag)
				{
					attackAddPos = new Vector3i(num4, 0, 0);
					flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num + num4, num2, num3, out attackBlockValue);
					attackAngle = -45f;
				}
				if (!flag)
				{
					attackAddPos = new Vector3i(2 * num4, 1, 0);
					flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num + 2 * num4, num2 + 1, num3, out attackBlockValue);
					attackAngle = 0f;
				}
				if (!flag)
				{
					attackAddPos = new Vector3i(0, 0, num5);
					flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num, num2, num3 + num5, out attackBlockValue);
					attackAngle = -45f;
				}
				if (!flag)
				{
					attackAddPos = new Vector3i(0, 1, 2 * num5);
					flag = UAIConsiderationPathBlocked.isPosBlocked(theEntity, num, num2 + 1, num3 + 2 * num5, out attackBlockValue);
					attackAngle = 0f;
				}
			}
			attackAddPos += new Vector3i(num, num2, num3);
			return flag;
		}

		// Token: 0x0600BA5C RID: 47708 RVA: 0x00458F6E File Offset: 0x0045716E
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool isPosBlocked(EntityAlive theEntity, int _x, int _y, int _z, out BlockValue attackBlockValue)
		{
			attackBlockValue = theEntity.world.GetBlock(_x, _y, _z);
			return attackBlockValue.Block.IsMovementBlocked(theEntity.world, new Vector3i(_x, _y, _z), attackBlockValue, BlockFace.None);
		}

		// Token: 0x0600BA5D RID: 47709 RVA: 0x00458FAB File Offset: 0x004571AB
		[PublicizedFrom(EAccessModifier.Private)]
		public static float GetTargetYaw(EntityAlive theEntity)
		{
			if (theEntity.GetAttackTarget() != null)
			{
				return theEntity.YawForTarget(theEntity.GetAttackTarget());
			}
			return theEntity.YawForTarget(UAIConsiderationPathBlocked.GetTargetPos(theEntity));
		}

		// Token: 0x0600BA5E RID: 47710 RVA: 0x00458FD4 File Offset: 0x004571D4
		[PublicizedFrom(EAccessModifier.Protected)]
		public static Vector3 GetTargetPos(EntityAlive theEntity)
		{
			if (theEntity.GetAttackTarget() != null)
			{
				return theEntity.GetAttackTarget().GetPosition();
			}
			return theEntity.InvestigatePosition;
		}
	}
}
