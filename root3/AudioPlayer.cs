using System;
using Audio;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class AudioPlayer : MonoBehaviour
{
	// Token: 0x0600037F RID: 895 RVA: 0x00019E28 File Offset: 0x00018028
	public void Play()
	{
		if (!this.refEntity)
		{
			this.refEntity = RootTransformRefEntity.AddIfEntity(base.transform);
			if (this.refEntity)
			{
				this.attachedEntity = this.refEntity.RootTransform.GetComponent<Entity>();
			}
		}
		if (this.attachedEntity)
		{
			Manager.Play(this.attachedEntity, this.soundName, 1f, false);
			this.isPlaying = true;
			this.queuedForPlaying = false;
		}
		else if (this.refEntity)
		{
			Vector3 position = this.refEntity.transform.position;
			if (position == Vector3.zero)
			{
				this.queuedForPlaying = true;
			}
			else
			{
				this.PlayAtPos(position);
			}
		}
		else
		{
			Vector3 position2 = base.transform.position;
			if (position2 == Vector3.zero)
			{
				this.queuedForPlaying = true;
			}
			else
			{
				this.PlayAtPos(position2);
			}
		}
		if (this.isPlaying && this.duration > 0f)
		{
			this.startTime = Time.time;
		}
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00019F31 File Offset: 0x00018131
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayAtPos(Vector3 _pos)
	{
		this.playPos = _pos + Origin.position;
		Manager.Play(this.playPos, this.soundName, -1, false, 1f);
		this.isPlaying = true;
		this.queuedForPlaying = false;
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00019F6C File Offset: 0x0001816C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.startDelay > 0f)
		{
			this.startDelay -= Time.deltaTime;
			return;
		}
		if (this.queuedForPlaying)
		{
			this.Play();
		}
		if (this.isPlaying && this.duration > 0f && Time.time > this.startTime + this.duration)
		{
			this.StopAudio();
		}
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00019FD8 File Offset: 0x000181D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void StopAudio()
	{
		if (this.isPlaying)
		{
			if (this.attachedEntity)
			{
				Manager.Stop(this.attachedEntity.entityId, this.soundName);
			}
			else
			{
				Manager.Stop(this.playPos, this.soundName);
			}
			this.isPlaying = false;
		}
	}

	// Token: 0x06000383 RID: 899 RVA: 0x0001A02A File Offset: 0x0001822A
	public void OnEnable()
	{
		if (!this.playOnDemand)
		{
			this.queuedForPlaying = true;
		}
	}

	// Token: 0x06000384 RID: 900 RVA: 0x0001A03B File Offset: 0x0001823B
	public void OnDisable()
	{
		this.StopAudio();
	}

	// Token: 0x06000385 RID: 901 RVA: 0x0001A03B File Offset: 0x0001823B
	public void OnDestroy()
	{
		this.StopAudio();
	}

	// Token: 0x04000409 RID: 1033
	public string soundName;

	// Token: 0x0400040A RID: 1034
	public float duration = -1f;

	// Token: 0x0400040B RID: 1035
	public bool playOnDemand;

	// Token: 0x0400040C RID: 1036
	public float startDelay;

	// Token: 0x0400040D RID: 1037
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity attachedEntity;

	// Token: 0x0400040E RID: 1038
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public RootTransformRefEntity refEntity;

	// Token: 0x0400040F RID: 1039
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool queuedForPlaying;

	// Token: 0x04000410 RID: 1040
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isPlaying;

	// Token: 0x04000411 RID: 1041
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startTime;

	// Token: 0x04000412 RID: 1042
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 playPos;
}
