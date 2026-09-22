using System;

namespace DynamicMusic
{
	// Token: 0x02001A2C RID: 6700
	public abstract class ContentPlayer : IPlayable
	{
		// Token: 0x170018CC RID: 6348
		// (get) Token: 0x0600CB4E RID: 52046 RVA: 0x004A8DBF File Offset: 0x004A6FBF
		// (set) Token: 0x0600CB4F RID: 52047 RVA: 0x004A8DC7 File Offset: 0x004A6FC7
		public virtual float Volume { get; set; } = 1f;

		// Token: 0x170018CD RID: 6349
		// (get) Token: 0x0600CB50 RID: 52048 RVA: 0x004A8DD0 File Offset: 0x004A6FD0
		// (set) Token: 0x0600CB51 RID: 52049 RVA: 0x004A8DD8 File Offset: 0x004A6FD8
		public virtual bool IsDone { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170018CE RID: 6350
		// (get) Token: 0x0600CB52 RID: 52050 RVA: 0x004A8DE1 File Offset: 0x004A6FE1
		// (set) Token: 0x0600CB53 RID: 52051 RVA: 0x004A8DE9 File Offset: 0x004A6FE9
		public virtual bool IsPaused { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170018CF RID: 6351
		// (get) Token: 0x0600CB54 RID: 52052 RVA: 0x004A8DF2 File Offset: 0x004A6FF2
		// (set) Token: 0x0600CB55 RID: 52053 RVA: 0x004A8DFA File Offset: 0x004A6FFA
		public virtual bool IsPlaying { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170018D0 RID: 6352
		// (get) Token: 0x0600CB56 RID: 52054 RVA: 0x004A8E03 File Offset: 0x004A7003
		// (set) Token: 0x0600CB57 RID: 52055 RVA: 0x004A8E0B File Offset: 0x004A700B
		public virtual bool IsReady { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600CB58 RID: 52056
		public abstract void Init();

		// Token: 0x0600CB59 RID: 52057 RVA: 0x004A8E14 File Offset: 0x004A7014
		public virtual void Play()
		{
			this.IsDone = false;
			this.IsPlaying = true;
			this.IsPaused = false;
		}

		// Token: 0x0600CB5A RID: 52058 RVA: 0x004A8E2B File Offset: 0x004A702B
		public virtual void Pause()
		{
			this.IsPlaying = false;
			this.IsPaused = true;
		}

		// Token: 0x0600CB5B RID: 52059 RVA: 0x004A8E3B File Offset: 0x004A703B
		public virtual void UnPause()
		{
			this.IsPlaying = true;
			this.IsPaused = false;
		}

		// Token: 0x0600CB5C RID: 52060 RVA: 0x004A8E4B File Offset: 0x004A704B
		public virtual void Stop()
		{
			this.IsDone = true;
			this.IsPlaying = false;
			this.IsPaused = false;
		}

		// Token: 0x0600CB5D RID: 52061 RVA: 0x004A8E62 File Offset: 0x004A7062
		[PublicizedFrom(EAccessModifier.Protected)]
		public ContentPlayer()
		{
		}
	}
}
