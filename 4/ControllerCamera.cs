using System;
using UnityEngine;

// Token: 0x0200000E RID: 14
public class ControllerCamera : MonoBehaviour
{
	// Token: 0x06000026 RID: 38 RVA: 0x00002E1C File Offset: 0x0000101C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.camLookOffset.x = this.cameraTarget.transform.localPosition.x;
		this.camLookOffset.y = this.cameraTarget.transform.localPosition.y;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002E6C File Offset: 0x0000106C
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.setCamera == null)
		{
			this.setCamera = Camera.main.transform;
		}
		if (Input.mousePosition.x > 365f && Input.mousePosition.y < 648f && Input.mousePosition.y > 50f)
		{
			if (Input.GetMouseButton(0))
			{
				this.MouseRotationDistance = Input.GetAxisRaw("Mouse X") * 2.7f;
				this.MouseVerticalDistance = Input.GetAxisRaw("Mouse Y") * 2.7f;
			}
			else
			{
				this.MouseRotationDistance = 0f;
				this.MouseVerticalDistance = 0f;
			}
			this.MouseScrollDistance = Input.GetAxisRaw("Mouse ScrollWheel");
			if (Input.GetMouseButton(2))
			{
				this.camLookOffset.x = this.camLookOffset.x + Input.GetAxisRaw("Mouse X") * 0.001f;
				this.camLookOffset.y = this.camLookOffset.y + Input.GetAxisRaw("Mouse Y") * 0.001f;
			}
		}
		else
		{
			this.MouseRotationDistance = 0f;
			this.MouseVerticalDistance = 0f;
		}
		this.followHeight = 1.5f;
		Vector3 eulerAngles = new Vector3(this.cameraTarget.transform.eulerAngles.x - this.MouseVerticalDistance, this.cameraTarget.transform.eulerAngles.y - this.MouseRotationDistance, this.cameraTarget.transform.eulerAngles.z);
		this.cameraTarget.transform.eulerAngles = eulerAngles;
		Vector3 localPosition = new Vector3(this.camLookOffset.x, this.camLookOffset.y, this.cameraTarget.transform.localPosition.z);
		this.cameraTarget.transform.localPosition = localPosition;
		Vector3 localPosition2 = new Vector3(this.setCamera.localPosition.x, this.setCamera.localPosition.y, Mathf.Clamp(this.setCamera.localPosition.z, -9.73f, -9.66f));
		this.setCamera.localPosition = localPosition2;
		if (this.setCamera.localPosition.z >= -9.73f && this.setCamera.localPosition.z <= -9.66f && this.MouseScrollDistance != 0f)
		{
			this.setCamera.transform.Translate(-Vector3.forward * this.MouseScrollDistance * 0.02f, base.transform);
		}
	}

	// Token: 0x04000042 RID: 66
	public Transform setCamera;

	// Token: 0x04000043 RID: 67
	public Transform cameraTarget;

	// Token: 0x04000044 RID: 68
	public float followDistance = 5f;

	// Token: 0x04000045 RID: 69
	public float followHeight = 1f;

	// Token: 0x04000046 RID: 70
	public float followSensitivity = 2f;

	// Token: 0x04000047 RID: 71
	public bool useRaycast = true;

	// Token: 0x04000048 RID: 72
	public Vector2 axisSensitivity = new Vector2(4f, 4f);

	// Token: 0x04000049 RID: 73
	public float camFOV = 35f;

	// Token: 0x0400004A RID: 74
	public float camRotation;

	// Token: 0x0400004B RID: 75
	public float camHeight;

	// Token: 0x0400004C RID: 76
	public float camYDamp;

	// Token: 0x0400004D RID: 77
	public Vector2 camLookOffset = new Vector2(0f, 0f);

	// Token: 0x0400004E RID: 78
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float MouseRotationDistance;

	// Token: 0x0400004F RID: 79
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float MouseVerticalDistance;

	// Token: 0x04000050 RID: 80
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float MouseScrollDistance;
}
