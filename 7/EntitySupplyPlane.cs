using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004D8 RID: 1240
[Preserve]
public class EntitySupplyPlane : Entity
{
	// Token: 0x0600285D RID: 10333 RVA: 0x000EC739 File Offset: 0x000EA939
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsDeadIfOutOfWorld()
	{
		return false;
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000FAC42 File Offset: 0x000F8E42
	public void SetDirectionToFly(Vector3 _directionToFly, int _ticksToFly)
	{
		this.ticksToFly = _ticksToFly;
		this.motion = _directionToFly * 6f;
		this.IsMovementReplicated = false;
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000FAC64 File Offset: 0x000F8E64
	[PublicizedFrom(EAccessModifier.Private)]
	public void MoveBoundsInsideFrustrum(Transform _parentT)
	{
		if (!this.planeMesh)
		{
			return;
		}
		float magnitude = (this.mainCamera.transform.position - _parentT.position).magnitude;
		Vector3 size = Vector3.one * (magnitude * 1.25f);
		Vector3 zero = Vector3.zero;
		this.planeMesh.bounds = new Bounds(zero, size);
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000FACD0 File Offset: 0x000F8ED0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateFarDraw()
	{
		if (!this.mainCamera)
		{
			this.mainCamera = Camera.main;
			if (!this.mainCamera)
			{
				return;
			}
		}
		if (!this.planeMesh)
		{
			this.planeMF = base.transform.GetComponentInChildren<MeshFilter>();
			if (this.planeMF)
			{
				this.planeMesh = this.planeMF.mesh;
			}
		}
		this.MoveBoundsInsideFrustrum(base.transform);
	}

	// Token: 0x06002862 RID: 10338 RVA: 0x000FAD4C File Offset: 0x000F8F4C
	public override void OnUpdatePosition(float _partialTicks)
	{
		base.OnUpdatePosition(_partialTicks);
		this.UpdateFarDraw();
		this.interpolateTargetRot = 0;
		this.position += this.motion * _partialTicks;
		if (!this.isEntityRemote)
		{
			int num = this.ticksToFly - 1;
			this.ticksToFly = num;
			if (num <= 0)
			{
				this.MarkToUnload();
			}
		}
		if (!this.isPlayedSound)
		{
			Manager.Play(this, "SupplyDrops/Supply_Crate_Plane_lp", 1f, false);
			this.isPlayedSound = true;
		}
		base.SetAirBorne(true);
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsSavedToFile()
	{
		return false;
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanCollideWithBlocks()
	{
		return false;
	}

	// Token: 0x04001E51 RID: 7761
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksToFly;

	// Token: 0x04001E52 RID: 7762
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isPlayedSound;

	// Token: 0x04001E53 RID: 7763
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera mainCamera;

	// Token: 0x04001E54 RID: 7764
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshFilter planeMF;

	// Token: 0x04001E55 RID: 7765
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Mesh planeMesh;
}
