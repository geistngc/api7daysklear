using System;
using UnityEngine;

// Token: 0x02001336 RID: 4918
public class FaceSpriteAtCamera : MonoBehaviour
{
	// Token: 0x06009B35 RID: 39733 RVA: 0x003AA567 File Offset: 0x003A8767
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		if (GameManager.IsDedicatedServer)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	// Token: 0x06009B36 RID: 39734 RVA: 0x003AA567 File Offset: 0x003A8767
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		if (GameManager.IsDedicatedServer)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	// Token: 0x06009B37 RID: 39735 RVA: 0x003AA576 File Offset: 0x003A8776
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnEnable()
	{
		this.mainCamera = null;
	}

	// Token: 0x06009B38 RID: 39736 RVA: 0x003AA580 File Offset: 0x003A8780
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		if (this.mainCamera != null)
		{
			base.transform.LookAt(this.mainCamera.transform.position, -Vector3.up);
		}
	}

	// Token: 0x04007513 RID: 29971
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera mainCamera;
}
