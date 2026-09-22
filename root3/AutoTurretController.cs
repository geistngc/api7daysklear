using System;
using UnityEngine;

// Token: 0x020003C8 RID: 968
public class AutoTurretController : MonoBehaviour, IPowerSystemCamera
{
	// Token: 0x17000383 RID: 899
	// (get) Token: 0x06001D18 RID: 7448 RVA: 0x000AF3D5 File Offset: 0x000AD5D5
	// (set) Token: 0x06001D19 RID: 7449 RVA: 0x000AF3DD File Offset: 0x000AD5DD
	public int UserAccessingId { get; [PublicizedFrom(EAccessModifier.Private)] set; } = -1;

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x06001D1A RID: 7450 RVA: 0x000AF3E6 File Offset: 0x000AD5E6
	public bool IsTurning
	{
		get
		{
			return this.YawController.IsTurning || this.PitchController.IsTurning;
		}
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x06001D1B RID: 7451 RVA: 0x000AF402 File Offset: 0x000AD602
	// (set) Token: 0x06001D1C RID: 7452 RVA: 0x000AF40A File Offset: 0x000AD60A
	public TileEntityPoweredRangedTrap TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.FireController.TileEntity = value;
		}
	}

	// Token: 0x06001D1D RID: 7453 RVA: 0x000AF41F File Offset: 0x000AD61F
	public void OnDestroy()
	{
		this.Cleanup();
		if (this.ConeMaterial != null)
		{
			UnityEngine.Object.Destroy(this.ConeMaterial);
		}
	}

	// Token: 0x06001D1E RID: 7454 RVA: 0x000AF440 File Offset: 0x000AD640
	public void Init(DynamicProperties _properties)
	{
		this.IsOn = false;
		this.FireController.Cone = this.Cone;
		this.FireController.Laser = this.Laser;
		this.FireController.Init(_properties, this);
		this.PitchController.Init(_properties);
		this.YawController.Init(_properties);
		if (this.Cone != null)
		{
			MeshRenderer component = this.Cone.GetComponent<MeshRenderer>();
			if (component != null)
			{
				if (component.material != null)
				{
					this.ConeMaterial = component.material;
					this.ConeColor = this.ConeMaterial.GetColor("_Color");
				}
				else if (component.sharedMaterial != null)
				{
					this.ConeMaterial = component.sharedMaterial;
					this.ConeColor = this.ConeMaterial.GetColor("_Color");
				}
			}
		}
		WireManager.Instance.AddPulseObject(this.Cone.gameObject);
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x000AF538 File Offset: 0x000AD738
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.FireController.IsOn && !this.IsOn)
		{
			this.FireController.OnPoweredOff();
		}
		this.FireController.IsOn = this.IsOn;
		if (this.IsOn)
		{
			this.YawController.UpdateYaw();
			this.PitchController.UpdatePitch();
		}
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x000AF594 File Offset: 0x000AD794
	public void SetConeVisible(bool visible)
	{
		if (this.Cone != null)
		{
			this.Cone.gameObject.SetActive(visible);
		}
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x000AF5B5 File Offset: 0x000AD7B5
	public void SetLaserVisible(bool visible)
	{
		if (this.Laser != null)
		{
			this.Laser.gameObject.SetActive(visible);
		}
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x000AF5D6 File Offset: 0x000AD7D6
	public void SetPitch(float pitch)
	{
		this.TileEntity.CenteredPitch = pitch;
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x000AF5E4 File Offset: 0x000AD7E4
	public void SetYaw(float yaw)
	{
		this.TileEntity.CenteredYaw = yaw;
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x000AF5F2 File Offset: 0x000AD7F2
	public float GetPitch()
	{
		return this.TileEntity.CenteredPitch;
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x000AF5FF File Offset: 0x000AD7FF
	public float GetYaw()
	{
		return this.TileEntity.CenteredYaw;
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x000AF60C File Offset: 0x000AD80C
	public Transform GetCameraTransform()
	{
		return this.Cone;
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x000AF614 File Offset: 0x000AD814
	public void SetUserAccessing(int userAccessingId)
	{
		this.UserAccessingId = userAccessingId;
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x000AF61D File Offset: 0x000AD81D
	public void Cleanup()
	{
		if (this.Cone != null && WireManager.HasInstance)
		{
			WireManager.Instance.RemovePulseObject(this.Cone.gameObject);
		}
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x000AF64A File Offset: 0x000AD84A
	public void SetConeColor(Color _color)
	{
		if (this.ConeMaterial != null)
		{
			this.ConeMaterial.SetColor("_Color", _color);
		}
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x000AF66B File Offset: 0x000AD86B
	public Color GetOriginalConeColor()
	{
		return this.ConeColor;
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x000AF594 File Offset: 0x000AD794
	public void SetConeActive(bool _active)
	{
		if (this.Cone != null)
		{
			this.Cone.gameObject.SetActive(_active);
		}
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x000AF673 File Offset: 0x000AD873
	public bool GetConeActive()
	{
		return this.Cone != null && this.Cone.gameObject.activeSelf;
	}

	// Token: 0x06001D2D RID: 7469 RVA: 0x000AF695 File Offset: 0x000AD895
	public bool HasCone()
	{
		return this.Cone != null;
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x000AF6A3 File Offset: 0x000AD8A3
	public bool HasLaser()
	{
		return this.Laser != null;
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x000027FC File Offset: 0x000009FC
	public void SetLaserColor(Color _color)
	{
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x000AF6B1 File Offset: 0x000AD8B1
	public Color GetOriginalLaserColor()
	{
		return Color.black;
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x000AF5B5 File Offset: 0x000AD7B5
	public void SetLaserActive(bool _active)
	{
		if (this.Laser != null)
		{
			this.Laser.gameObject.SetActive(_active);
		}
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x000AF6B8 File Offset: 0x000AD8B8
	public bool GetLaserActive()
	{
		return this.Laser != null && this.Laser.gameObject.activeSelf;
	}

	// Token: 0x04001303 RID: 4867
	public AutoTurretYawLerp YawController;

	// Token: 0x04001304 RID: 4868
	public AutoTurretPitchLerp PitchController;

	// Token: 0x04001305 RID: 4869
	public AutoTurretFireController FireController;

	// Token: 0x04001306 RID: 4870
	public Transform Laser;

	// Token: 0x04001307 RID: 4871
	public Transform Cone;

	// Token: 0x04001308 RID: 4872
	public Material ConeMaterial;

	// Token: 0x04001309 RID: 4873
	public Color ConeColor;

	// Token: 0x0400130A RID: 4874
	public bool IsOn;

	// Token: 0x0400130C RID: 4876
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public TileEntityPoweredRangedTrap tileEntity;
}
