using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000AC6 RID: 2758
public sealed class BakeScheduler
{
	// Token: 0x170008F3 RID: 2291
	// (get) Token: 0x0600529D RID: 21149 RVA: 0x001F96F9 File Offset: 0x001F78F9
	public BakeScheduler.BakeJobState CurrentState
	{
		get
		{
			return this._current.State;
		}
	}

	// Token: 0x0600529E RID: 21150 RVA: 0x001F9708 File Offset: 0x001F7908
	public void Clear()
	{
		using (BakeScheduler.s_SignTextureManagerClear.Auto())
		{
			this._requests.Clear();
			this._index = 0;
			this._current.End();
			BakeScheduler.s_PendingBakeJobs.Value = 0;
			BakeScheduler.s_BakeJobProgress.Value = 0.0;
		}
	}

	// Token: 0x0600529F RID: 21151 RVA: 0x001F9780 File Offset: 0x001F7980
	public void ResetRequests(List<SignBakeRequest> newRequests)
	{
		using (BakeScheduler.s_SignTextureManagerResetRequests.Auto())
		{
			this._requests.Clear();
			for (int i = 0; i < newRequests.Count; i++)
			{
				this._requests.Add(newRequests[i]);
			}
			this._index = 0;
			this._current.End();
		}
	}

	// Token: 0x060052A0 RID: 21152 RVA: 0x001F97FC File Offset: 0x001F79FC
	public bool TickOnce(int tileSize, Material material, CommandBuffer cmd, SignTextureStore store, SignPrioritizer prioritizer, bool showProgress = false)
	{
		bool result;
		using (BakeScheduler.s_SignTextureManagerTickOnce.Auto())
		{
			BakeScheduler.s_PendingBakeJobs.Value = this._requests.Count - this._index;
			if (this._current.State == BakeScheduler.BakeJobState.None)
			{
				while (this._index < this._requests.Count)
				{
					List<SignBakeRequest> requests = this._requests;
					int index = this._index;
					this._index = index + 1;
					SignBakeRequest signBakeRequest = requests[index];
					RenderTexture renderTexture = store.AcquireForBake(signBakeRequest.Tier);
					if (!(renderTexture == null))
					{
						this._current.Begin(signBakeRequest, renderTexture);
						break;
					}
					Log.Warning(string.Format("[STM] Pool exhausted at tier {0}; skipping requested bake.", signBakeRequest.Tier));
				}
				if (this._current.State == BakeScheduler.BakeJobState.None)
				{
					BakeScheduler.s_BakeJobProgress.Value = 0.0;
					return false;
				}
			}
			if (this._current.RT == null)
			{
				this._current.End();
				result = true;
			}
			else
			{
				RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(this._current.RT);
				cmd.Clear();
				if (this._current.State == BakeScheduler.BakeJobState.Pending)
				{
					GlobalSignId groupSignId = prioritizer.GetGroupSignId(this._current.Request.GroupIndex);
					if (!SignDataManager.Instance.TryApplyRenderingData(groupSignId, 1f, material, null, SignUIStyle.Baked))
					{
						Debug.LogError(string.Format("Failed to retrieve rendering data for sign ID: {0}", groupSignId));
					}
					material.SetVector(SignShaderIDs._CanvasAspect, Vector2.one);
					cmd.SetRenderTarget(renderTargetIdentifier);
					cmd.ClearRenderTarget(true, true, Color.clear);
					if (showProgress)
					{
						prioritizer.ApplyToGroupCanvases(this._current.Request.GroupIndex, this._current.RT);
					}
					this._current.State = BakeScheduler.BakeJobState.InProgress;
				}
				if (this._current.State == BakeScheduler.BakeJobState.InProgress)
				{
					int num = Mathf.Max(this._current.RT.width / tileSize, 1);
					int num2 = num * num;
					int num3 = this._current.Step % num;
					int num4 = this._current.Step / num;
					Vector2 vector = Vector2.one / (float)num * 2f;
					Vector2 vector2 = new Vector2(-1f + (float)num3 * vector.x, -1f + (float)num4 * vector.y);
					material.SetVector(SignShaderIDs._MinUV, vector2);
					material.SetVector(SignShaderIDs._MaxUV, vector2 + vector);
					cmd.Blit(BuiltinRenderTextureType.None, renderTargetIdentifier, material, 0);
					Graphics.ExecuteCommandBuffer(cmd);
					this._current.Step = this._current.Step + 1;
					BakeScheduler.s_BakeJobProgress.Value = (double)(100f * (float)this._current.Step / (float)num2);
					if (this._current.Step >= num2)
					{
						this._current.State = BakeScheduler.BakeJobState.Complete;
					}
					result = true;
				}
				else if (this._current.State == BakeScheduler.BakeJobState.Complete)
				{
					GlobalSignId groupSignId2 = prioritizer.GetGroupSignId(this._current.Request.GroupIndex);
					store.SetBaked(groupSignId2, this._current.Request.Tier, this._current.RT);
					prioritizer.ApplyToGroupCanvases(this._current.Request.GroupIndex, this._current.RT);
					this._current.End();
					result = true;
				}
				else
				{
					Log.Error(string.Format("Unexpected case in SignTextureManager: TickOnce reached failsafe return case. Current job state is: {0}.", this._current.State));
					this._current.State = BakeScheduler.BakeJobState.None;
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x0400403D RID: 16445
	public static readonly ProfilerMarker s_SignTextureManagerClear = new ProfilerMarker("SignTextureManager.BakeScheduler.Clear");

	// Token: 0x0400403E RID: 16446
	public static readonly ProfilerMarker s_SignTextureManagerResetRequests = new ProfilerMarker("SignTextureManager.BakeScheduler.ResetRequests");

	// Token: 0x0400403F RID: 16447
	public static readonly ProfilerMarker s_SignTextureManagerTickOnce = new ProfilerMarker("SignTextureManager.BakeScheduler.TickOnce");

	// Token: 0x04004040 RID: 16448
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounterValue<double> s_BakeJobProgress = new ProfilerCounterValue<double>(ProfilerCategory.Scripts, "STM Job Progress", ProfilerMarkerDataUnit.Percent, ProfilerCounterOptions.FlushOnEndOfFrame);

	// Token: 0x04004041 RID: 16449
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounterValue<int> s_PendingBakeJobs = new ProfilerCounterValue<int>(ProfilerCategory.Scripts, "STM Pending Jobs", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

	// Token: 0x04004042 RID: 16450
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<SignBakeRequest> _requests = new List<SignBakeRequest>(256);

	// Token: 0x04004043 RID: 16451
	[PublicizedFrom(EAccessModifier.Private)]
	public int _index;

	// Token: 0x04004044 RID: 16452
	[PublicizedFrom(EAccessModifier.Private)]
	public BakeScheduler.BakeJob _current;

	// Token: 0x02000AC7 RID: 2759
	public enum BakeJobState
	{
		// Token: 0x04004046 RID: 16454
		None,
		// Token: 0x04004047 RID: 16455
		Pending,
		// Token: 0x04004048 RID: 16456
		InProgress,
		// Token: 0x04004049 RID: 16457
		GeneratingMips,
		// Token: 0x0400404A RID: 16458
		Complete
	}

	// Token: 0x02000AC8 RID: 2760
	[PublicizedFrom(EAccessModifier.Private)]
	public struct BakeJob
	{
		// Token: 0x060052A3 RID: 21155 RVA: 0x001F9C56 File Offset: 0x001F7E56
		public void Begin(SignBakeRequest request, RenderTexture rt)
		{
			this.Request = request;
			this.RT = rt;
			this.Step = 0;
			this.State = BakeScheduler.BakeJobState.Pending;
		}

		// Token: 0x060052A4 RID: 21156 RVA: 0x001F9C74 File Offset: 0x001F7E74
		public void End()
		{
			this.RT = null;
			this.Step = 0;
			this.State = BakeScheduler.BakeJobState.None;
			this.Request = default(SignBakeRequest);
		}

		// Token: 0x0400404B RID: 16459
		public SignBakeRequest Request;

		// Token: 0x0400404C RID: 16460
		public RenderTexture RT;

		// Token: 0x0400404D RID: 16461
		public int Step;

		// Token: 0x0400404E RID: 16462
		public BakeScheduler.BakeJobState State;
	}
}
