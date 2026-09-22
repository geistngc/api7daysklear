using System;
using DynamicMusic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020000B1 RID: 177
public class AudioMixerManager : MonoBehaviour
{
	// Token: 0x0600036C RID: 876 RVA: 0x00018D68 File Offset: 0x00016F68
	public void Update()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (GameManager.Instance.World != null)
		{
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer != null)
			{
				bool isUnderwaterCamera = primaryPlayer.IsUnderwaterCamera;
				if (primaryPlayer.isDeafened)
				{
					if (!this.wasDeafened)
					{
						this.transitionTo(this.deafenedSnapshot);
					}
				}
				else if (primaryPlayer.isStunned)
				{
					if (!this.wasStunned || this.wasDeafened)
					{
						this.transitionTo(this.stunnedSnapshot);
					}
				}
				else if (isUnderwaterCamera)
				{
					if (!this.bCameraWasUnderWater || this.wasStunned || this.wasDeafened)
					{
						this.transitionTo(this.underwaterSnapshot);
					}
				}
				else if (this.wasStunned || this.wasDeafened || this.bCameraWasUnderWater)
				{
					this.transitionTo(this.defaultSnapshot);
				}
				this.bCameraWasUnderWater = isUnderwaterCamera;
				this.wasStunned = primaryPlayer.isStunned;
				this.wasDeafened = primaryPlayer.isDeafened;
			}
		}
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00018E68 File Offset: 0x00017068
	[PublicizedFrom(EAccessModifier.Private)]
	public void transitionTo(AudioMixerManager.SnapshotController _snapshot)
	{
		_snapshot.snapshot.TransitionTo(_snapshot.transitionToTime);
		if (GamePrefs.GetBool(EnumGamePrefs.OptionsDynamicMusicEnabled) && !GameManager.Instance.IsEditMode())
		{
			MixerController.Instance.OnSnapshotTransition();
		}
	}

	// Token: 0x040003CE RID: 974
	public AudioMixerManager.SnapshotController underwaterSnapshot;

	// Token: 0x040003CF RID: 975
	public AudioMixerManager.SnapshotController stunnedSnapshot;

	// Token: 0x040003D0 RID: 976
	public AudioMixerManager.SnapshotController deafenedSnapshot;

	// Token: 0x040003D1 RID: 977
	public AudioMixerManager.SnapshotController defaultSnapshot;

	// Token: 0x040003D2 RID: 978
	public bool bCameraWasUnderWater;

	// Token: 0x040003D3 RID: 979
	public bool wasStunned;

	// Token: 0x040003D4 RID: 980
	public bool wasDeafened;

	// Token: 0x020000B2 RID: 178
	[Serializable]
	public class SnapshotController
	{
		// Token: 0x040003D5 RID: 981
		public AudioMixerSnapshot snapshot;

		// Token: 0x040003D6 RID: 982
		public float transitionToTime = 1f;
	}
}
