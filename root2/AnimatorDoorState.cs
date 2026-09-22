using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200009B RID: 155
public class AnimatorDoorState : StateMachineBehaviour
{
	// Token: 0x06000301 RID: 769 RVA: 0x00016F24 File Offset: 0x00015124
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.matchDoorState(animator, stateInfo))
		{
			this.colliders = animator.gameObject.GetComponentsInChildren<Collider>(true);
			this.rules = new EntityCollisionRules[this.colliders.Length];
			for (int i = this.colliders.Length - 1; i >= 0; i--)
			{
				Collider collider = this.colliders[i];
				this.rules[i] = collider.GetComponent<EntityCollisionRules>();
				if (!this.isOpenAnim)
				{
					EntityCollisionRules entityCollisionRules = this.rules[i];
					if (entityCollisionRules && entityCollisionRules.IsAnimPush)
					{
						collider.enabled = false;
					}
				}
			}
			this.EnableColliders(true);
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00016FBE File Offset: 0x000151BE
	[PublicizedFrom(EAccessModifier.Private)]
	public bool matchDoorState(Animator animator, AnimatorStateInfo stateInfo)
	{
		bool @bool = animator.GetBool(AnimatorDoorState.IsOpenHash);
		this.isOpenAnim = (stateInfo.shortNameHash == AnimatorDoorState.OpenHash);
		return @bool == this.isOpenAnim;
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00016FE8 File Offset: 0x000151E8
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.matchDoorState(animator, stateInfo))
		{
			if (this.stopNext)
			{
				this.stopNext = false;
				this.EnableColliders(true);
				if (!this.keepAnimatorEnabled)
				{
					animator.enabled = false;
					return;
				}
			}
			else
			{
				if (animator.IsInTransition(layerIndex))
				{
					return;
				}
				float normalizedTime = stateInfo.normalizedTime;
				if (normalizedTime >= this.collideOnPercent)
				{
					if (!this.collidersEnabled)
					{
						if (this.CheckForObstacles())
						{
							return;
						}
						this.EnableColliders(true);
					}
					else if (this.disableColliderOnObstacleDetection && this.CheckForObstacles())
					{
						this.EnableColliders(false);
					}
					if (normalizedTime >= 1f)
					{
						this.stopNext = true;
						return;
					}
				}
				else if (normalizedTime < 1f && normalizedTime >= this.collideOffPercent)
				{
					if (this.collidersEnabled)
					{
						this.EnableColliders(false);
					}
					if (!this.isOpenAnim)
					{
						this.PushPlayers(normalizedTime);
					}
				}
			}
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x000170B8 File Offset: 0x000152B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnableColliders(bool _on)
	{
		this.collidersEnabled = _on;
		if (this.colliders != null)
		{
			int i = this.colliders.Length - 1;
			while (i >= 0)
			{
				EntityCollisionRules entityCollisionRules = this.rules[i];
				if (!entityCollisionRules)
				{
					goto IL_50;
				}
				if (!entityCollisionRules.IsStatic)
				{
					if (!entityCollisionRules.IsAnimPush)
					{
						goto IL_50;
					}
					this.colliders[i].enabled = !_on;
				}
				IL_6D:
				i--;
				continue;
				IL_50:
				if (!this.colliders[i].isTrigger)
				{
					this.colliders[i].enabled = _on;
					goto IL_6D;
				}
				goto IL_6D;
			}
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x0001713C File Offset: 0x0001533C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CheckForObstacles()
	{
		if (this.colliders == null)
		{
			return false;
		}
		Ray ray = new Ray(Vector3.zero, Vector3.up);
		List<EntityPlayer> list = GameManager.Instance.World.Players.list;
		for (int i = list.Count - 1; i >= 0; i--)
		{
			Vector3 origin = list[i].position - Origin.position;
			origin.y += 0.35f;
			ray.origin = origin;
			for (int j = this.colliders.Length - 1; j >= 0; j--)
			{
				EntityCollisionRules entityCollisionRules = this.rules[j];
				if (!entityCollisionRules || (!entityCollisionRules.IsStatic && !entityCollisionRules.IsAnimPush))
				{
					Collider collider = this.colliders[j];
					bool enabled = collider.enabled;
					collider.enabled = true;
					RaycastHit raycastHit;
					bool flag = collider.Raycast(ray, out raycastHit, 0.9f);
					collider.enabled = enabled;
					if (flag)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00017238 File Offset: 0x00015438
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void PushPlayers(float _normalizedTime)
	{
		if (this.colliders == null)
		{
			return;
		}
		if (_normalizedTime < 0.5f)
		{
			return;
		}
		List<EntityPlayer> list = GameManager.Instance.World.Players.list;
		for (int i = list.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = list[i];
			Vector3 vector = entityPlayer.position - Origin.position;
			vector.y += 0.1f;
			float num = entityPlayer.m_characterController.GetRadius();
			num *= num;
			for (int j = this.colliders.Length - 1; j >= 0; j--)
			{
				EntityCollisionRules entityCollisionRules = this.rules[j];
				if (entityCollisionRules && entityCollisionRules.IsAnimPush)
				{
					Collider collider = this.colliders[j];
					Vector3 b = collider.ClosestPoint(vector);
					Vector3 forceVec = vector - b;
					forceVec.y = 0f;
					float sqrMagnitude = forceVec.sqrMagnitude;
					if (sqrMagnitude < num)
					{
						float num2 = 0.002f;
						if (sqrMagnitude == 0f)
						{
							forceVec = collider.transform.forward * -1f;
							if (_normalizedTime >= 0.94f)
							{
								num2 *= 7f;
							}
						}
						forceVec = forceVec.normalized * num2;
						entityPlayer.PhysicsPush(forceVec, Vector3.zero, true);
					}
				}
			}
		}
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00017396 File Offset: 0x00015596
	[Conditional("DEBUG_DOOR")]
	public void LogDoor(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} Door {1}", GameManager.frameCount, _format);
		Log.Warning(_format, _args);
	}

	// Token: 0x04000397 RID: 919
	public static readonly int OpenHash = Animator.StringToHash("Open");

	// Token: 0x04000398 RID: 920
	public static readonly int CloseHash = Animator.StringToHash("Close");

	// Token: 0x04000399 RID: 921
	public static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

	// Token: 0x0400039A RID: 922
	public static readonly int OpenTriggerHash = Animator.StringToHash("OpenTrigger");

	// Token: 0x0400039B RID: 923
	[Range(0f, 1f)]
	public float collideOffPercent;

	// Token: 0x0400039C RID: 924
	[Range(0f, 1f)]
	public float collideOnPercent = 0.99f;

	// Token: 0x0400039D RID: 925
	public bool disableColliderOnObstacleDetection;

	// Token: 0x0400039E RID: 926
	public bool keepAnimatorEnabled;

	// Token: 0x0400039F RID: 927
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityCollisionRules[] rules;

	// Token: 0x040003A0 RID: 928
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Collider[] colliders;

	// Token: 0x040003A1 RID: 929
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool collidersEnabled;

	// Token: 0x040003A2 RID: 930
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isOpenAnim;

	// Token: 0x040003A3 RID: 931
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool stopNext;
}
