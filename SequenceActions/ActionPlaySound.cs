using System;
using System.Collections;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001998 RID: 6552
	[Preserve]
	public class ActionPlaySound : ActionBaseTargetAction
	{
		// Token: 0x0600C8CF RID: 51407 RVA: 0x0049E6A4 File Offset: 0x0049C8A4
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			if (target != null)
			{
				if (this.canDisable)
				{
					EntityAlive entityAlive = target as EntityAlive;
					if (entityAlive != null && EffectManager.GetValue(PassiveEffects.DisableGameEventNotify, null, 0f, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
					{
						return BaseAction.ActionCompleteStates.Complete;
					}
				}
				if (this.insideHead)
				{
					EntityPlayer entityPlayer = target as EntityPlayer;
					if (entityPlayer != null)
					{
						if (entityPlayer is EntityPlayerLocal)
						{
							this.OnClientPerform(entityPlayer);
						}
						else
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(base.Owner.Name, entityPlayer.entityId, base.Owner.ExtraData, base.Owner.Tag, NetPackageGameEventResponse.ResponseTypes.ClientSequenceAction, -1, -1, false, base.GetActionKey()), false, entityPlayer.entityId, -1, -1, null, 192, false);
						}
					}
				}
				else if (this.soundNames != null)
				{
					if (this.duration == 0f)
					{
						if (this.behindPlayer)
						{
							Manager.BroadcastPlayByLocalPlayer(target.position + (target.transform.forward * -1f + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, 0f)), this.soundNames[UnityEngine.Random.Range(0, this.soundNames.Length)]);
						}
						else
						{
							Manager.BroadcastPlayByLocalPlayer(target.position, this.soundNames[UnityEngine.Random.Range(0, this.soundNames.Length)]);
						}
					}
					else
					{
						Vector3 position = this.behindPlayer ? (target.position + (target.transform.forward * -1f + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, 0f))) : target.position;
						string text = this.soundNames[UnityEngine.Random.Range(0, this.soundNames.Length)];
						Manager.BroadcastPlayByLocalPlayer(position, text);
						GameManager.Instance.StartCoroutine(this.StopSound(position, text));
					}
				}
			}
			else if (base.Owner.TargetPosition != Vector3.zero && this.soundNames != null)
			{
				if (this.duration == 0f)
				{
					Manager.BroadcastPlay(base.Owner.TargetPosition, this.soundNames[UnityEngine.Random.Range(0, this.soundNames.Length)], 0f);
				}
				else
				{
					Vector3 targetPosition = base.Owner.TargetPosition;
					string text2 = this.soundNames[UnityEngine.Random.Range(0, this.soundNames.Length)];
					Manager.BroadcastPlay(targetPosition, text2, 0f);
					GameManager.Instance.StartCoroutine(this.StopSound(targetPosition, text2));
				}
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C8D0 RID: 51408 RVA: 0x0049E970 File Offset: 0x0049CB70
		public override void OnClientPerform(Entity target)
		{
			if (this.duration == 0f)
			{
				Manager.PlayInsidePlayerHead(this.soundNames[UnityEngine.Random.Range(0, this.soundNames.Length)], target.entityId, this.delay, false, false);
				return;
			}
			string text = this.soundNames[UnityEngine.Random.Range(0, this.soundNames.Length)];
			Manager.PlayInsidePlayerHead(text, target.entityId, this.delay, false, false);
			GameManager.Instance.StartCoroutine(this.StopInHeadSound(text));
		}

		// Token: 0x0600C8D1 RID: 51409 RVA: 0x0049E9EF File Offset: 0x0049CBEF
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator StopSound(Vector3 position, string soundName)
		{
			yield return new WaitForSeconds(this.duration + 0.001f);
			Manager.BroadcastStop(position, soundName);
			yield break;
		}

		// Token: 0x0600C8D2 RID: 51410 RVA: 0x0049EA0C File Offset: 0x0049CC0C
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator StopInHeadSound(string soundName)
		{
			yield return new WaitForSeconds(this.duration + 0.001f);
			Manager.StopLoopInsidePlayerHead(soundName, -1, false);
			yield break;
		}

		// Token: 0x0600C8D3 RID: 51411 RVA: 0x0049EA24 File Offset: 0x0049CC24
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionPlaySound.PropSound))
			{
				this.soundNames = properties.Values[ActionPlaySound.PropSound].Split(',', StringSplitOptions.None);
			}
			if (properties.Values.ContainsKey(ActionPlaySound.PropInsideHead))
			{
				this.insideHead = StringParsers.ParseBool(properties.Values[ActionPlaySound.PropInsideHead], 0, -1, true);
			}
			if (properties.Values.ContainsKey(ActionPlaySound.PropBehindPlayer))
			{
				this.behindPlayer = StringParsers.ParseBool(properties.Values[ActionPlaySound.PropBehindPlayer], 0, -1, true);
			}
			if (properties.Values.ContainsKey(ActionPlaySound.PropLoopDuration))
			{
				this.duration = StringParsers.ParseFloat(properties.Values[ActionPlaySound.PropLoopDuration], 0, -1, NumberStyles.Any);
			}
			properties.ParseBool(ActionPlaySound.PropCanDisable, ref this.canDisable);
			properties.ParseFloat(ActionPlaySound.PropDelay, ref this.delay);
		}

		// Token: 0x0600C8D4 RID: 51412 RVA: 0x0049EB20 File Offset: 0x0049CD20
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionPlaySound
			{
				targetGroup = this.targetGroup,
				soundNames = this.soundNames,
				insideHead = this.insideHead,
				behindPlayer = this.behindPlayer,
				duration = this.duration,
				delay = this.delay,
				canDisable = this.canDisable
			};
		}

		// Token: 0x04009867 RID: 39015
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool insideHead;

		// Token: 0x04009868 RID: 39016
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool behindPlayer;

		// Token: 0x04009869 RID: 39017
		[PublicizedFrom(EAccessModifier.Protected)]
		public float duration;

		// Token: 0x0400986A RID: 39018
		[PublicizedFrom(EAccessModifier.Protected)]
		public float delay;

		// Token: 0x0400986B RID: 39019
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canDisable = true;

		// Token: 0x0400986C RID: 39020
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] soundNames;

		// Token: 0x0400986D RID: 39021
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSound = "sound";

		// Token: 0x0400986E RID: 39022
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropInsideHead = "inside_head";

		// Token: 0x0400986F RID: 39023
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBehindPlayer = "behind_player";

		// Token: 0x04009870 RID: 39024
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropLoopDuration = "loop_duration";

		// Token: 0x04009871 RID: 39025
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropDelay = "delay";

		// Token: 0x04009872 RID: 39026
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropCanDisable = "can_disable";
	}
}
