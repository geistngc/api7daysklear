using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200135E RID: 4958
public class BoundaryProjector : MonoBehaviour
{
	// Token: 0x06009C52 RID: 40018 RVA: 0x003B0CC0 File Offset: 0x003AEEC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		Projector[] componentsInChildren = base.transform.GetComponentsInChildren<Projector>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.ProjectorList.Add(new BoundaryProjector.ProjectorEntry
			{
				Projector = componentsInChildren[i],
				EffectData = new BoundaryProjector.ProjectorEffectData()
			});
		}
		this.SetupProjectors();
	}

	// Token: 0x06009C53 RID: 40019 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetupProjectors()
	{
	}

	// Token: 0x06009C54 RID: 40020 RVA: 0x003B0D14 File Offset: 0x003AEF14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		for (int i = 0; i < this.ProjectorList.Count; i++)
		{
			if (this.ProjectorList[i] != null && this.ProjectorList[i].Projector.gameObject.activeSelf)
			{
				BoundaryProjector.ProjectorEntry projectorEntry = this.ProjectorList[i];
				if (projectorEntry.EffectData.AutoRotate)
				{
					Vector3 eulerAngles = projectorEntry.Projector.transform.localRotation.eulerAngles;
					projectorEntry.Projector.transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + Time.deltaTime * projectorEntry.EffectData.RotationSpeed, eulerAngles.z);
				}
				if (projectorEntry.EffectData.targetRadius != -1f)
				{
					projectorEntry.Projector.orthographicSize = Mathf.Lerp(projectorEntry.Projector.orthographicSize, projectorEntry.EffectData.targetRadius, Time.deltaTime);
					if (projectorEntry.Projector.orthographicSize == projectorEntry.EffectData.targetRadius)
					{
						projectorEntry.EffectData.targetRadius = -1f;
					}
				}
				if (projectorEntry.EffectData.IsGlowing)
				{
					Color color = projectorEntry.Projector.material.color;
					float num = Mathf.PingPong(Time.time, 0.25f);
					projectorEntry.Projector.material.color = new Color(color.r, color.g, color.b, 0.5f + num * 2f);
				}
			}
		}
		if (this.targetPos != BoundaryProjector.invalidPos)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPos, Time.deltaTime);
			if (base.transform.position == this.targetPos)
			{
				this.targetPos = BoundaryProjector.invalidPos;
			}
		}
	}

	// Token: 0x06009C55 RID: 40021 RVA: 0x003B0F04 File Offset: 0x003AF104
	public void SetRadius(int projectorID, float size)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			if (this.ProjectorList[projectorID].Projector.orthographicSize == -1f || size == 0f)
			{
				this.ProjectorList[projectorID].Projector.orthographicSize = size;
				this.ProjectorList[projectorID].EffectData.targetRadius = -1f;
				return;
			}
			this.ProjectorList[projectorID].EffectData.targetRadius = size;
		}
	}

	// Token: 0x06009C56 RID: 40022 RVA: 0x003B0F9C File Offset: 0x003AF19C
	public void SetAlpha(int projectorID, float alpha)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			Color color = this.ProjectorList[projectorID].Projector.material.color;
			this.ProjectorList[projectorID].Projector.material.color = new Color(color.r, color.g, color.b, alpha);
		}
	}

	// Token: 0x06009C57 RID: 40023 RVA: 0x003B1014 File Offset: 0x003AF214
	public void SetGlow(int projectorID, bool isGlowing)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			Color color = this.ProjectorList[projectorID].Projector.material.color;
			this.ProjectorList[projectorID].EffectData.IsGlowing = isGlowing;
		}
	}

	// Token: 0x06009C58 RID: 40024 RVA: 0x003B1070 File Offset: 0x003AF270
	public void SetAutoRotate(int projectorID, bool autoRotate, float rotateSpeed)
	{
		if (projectorID < this.ProjectorList.Count && this.ProjectorList[projectorID] != null)
		{
			this.ProjectorList[projectorID].EffectData.AutoRotate = autoRotate;
			this.ProjectorList[projectorID].EffectData.RotationSpeed = rotateSpeed;
		}
	}

	// Token: 0x06009C59 RID: 40025 RVA: 0x003B10C7 File Offset: 0x003AF2C7
	public void SetMoveToPosition(Vector3 vNew)
	{
		if (base.transform.position.y == -999f)
		{
			base.transform.position = vNew;
			return;
		}
		this.targetPos = vNew;
	}

	// Token: 0x0400760E RID: 30222
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<BoundaryProjector.ProjectorEntry> ProjectorList = new List<BoundaryProjector.ProjectorEntry>();

	// Token: 0x0400760F RID: 30223
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static Vector3 invalidPos = new Vector3(-999f, -999f, -999f);

	// Token: 0x04007610 RID: 30224
	public Vector3 targetPos = BoundaryProjector.invalidPos;

	// Token: 0x04007611 RID: 30225
	public bool IsInitialized;

	// Token: 0x0200135F RID: 4959
	public class ProjectorEntry
	{
		// Token: 0x04007612 RID: 30226
		public BoundaryProjector.ProjectorEffectData EffectData;

		// Token: 0x04007613 RID: 30227
		public Projector Projector;
	}

	// Token: 0x02001360 RID: 4960
	public class ProjectorEffectData
	{
		// Token: 0x04007614 RID: 30228
		public bool AutoRotate;

		// Token: 0x04007615 RID: 30229
		public float RotationSpeed;

		// Token: 0x04007616 RID: 30230
		public bool IsGlowing;

		// Token: 0x04007617 RID: 30231
		public float targetRadius = -1f;
	}
}
