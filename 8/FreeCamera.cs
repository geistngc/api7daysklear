using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class FreeCamera : MonoBehaviour
{
	// Token: 0x06000105 RID: 261 RVA: 0x0000BB33 File Offset: 0x00009D33
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.targetPosition = base.transform.position;
		this.targetRotation = base.transform.rotation;
	}

	// Token: 0x06000106 RID: 262 RVA: 0x0000BB58 File Offset: 0x00009D58
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (!Input.GetMouseButtonDown(1) && Input.GetMouseButton(1))
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				Vector3 eulerAngles = this.light.transform.eulerAngles;
				eulerAngles.y += Input.GetAxis("Mouse X") * this.turnSpeed;
				eulerAngles.x -= Input.GetAxis("Mouse Y") * this.turnSpeed;
				this.light.transform.rotation = Quaternion.Euler(eulerAngles);
			}
			else
			{
				Vector3 eulerAngles2 = this.targetRotation.eulerAngles;
				eulerAngles2.y += Input.GetAxis("Mouse X") * this.turnSpeed;
				eulerAngles2.x -= Input.GetAxis("Mouse Y") * this.turnSpeed;
				this.targetRotation = Quaternion.Euler(eulerAngles2);
			}
		}
		if (Input.GetMouseButton(2))
		{
			float d = Input.GetAxis("Mouse X") * this.panSpeed * Time.deltaTime;
			float d2 = Input.GetAxis("Mouse Y") * this.panSpeed * Time.deltaTime;
			this.targetPosition -= base.transform.right * d + base.transform.up * d2;
		}
		float d3 = Input.GetKey(KeyCode.Q) ? (this.moveSpeed * Time.deltaTime) : 0f;
		float d4 = Input.GetKey(KeyCode.E) ? (this.moveSpeed * Time.deltaTime) : 0f;
		float d5 = Input.GetAxis("Horizontal") * this.moveSpeed * Time.deltaTime;
		float d6 = Input.GetAxis("Vertical") * this.moveSpeed * Time.deltaTime;
		float d7 = Input.GetKey(KeyCode.LeftShift) ? this.shiftSpeed : 1f;
		this.targetPosition += d7 * (base.transform.right * d5 + base.transform.forward * d6 + base.transform.up * d3 - base.transform.up * d4);
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (Input.GetMouseButton(1))
		{
			this.targetPosition += base.transform.forward * axis * this.zoomSpeed;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, Time.deltaTime * this.moveSmoothing);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, Time.deltaTime * this.turnSmoothing);
	}

	// Token: 0x04000117 RID: 279
	public Light light;

	// Token: 0x04000118 RID: 280
	public float moveSpeed = 10f;

	// Token: 0x04000119 RID: 281
	public float turnSpeed = 4f;

	// Token: 0x0400011A RID: 282
	public float zoomSpeed = 10f;

	// Token: 0x0400011B RID: 283
	public float panSpeed = 10f;

	// Token: 0x0400011C RID: 284
	public float shiftSpeed = 4f;

	// Token: 0x0400011D RID: 285
	public float moveSmoothing = 5f;

	// Token: 0x0400011E RID: 286
	public float turnSmoothing = 5f;

	// Token: 0x0400011F RID: 287
	public float zoomSmoothing = 5f;

	// Token: 0x04000120 RID: 288
	public float panSmoothing = 5f;

	// Token: 0x04000121 RID: 289
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 targetPosition;

	// Token: 0x04000122 RID: 290
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion targetRotation;

	// Token: 0x04000123 RID: 291
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMouseInWindow = true;
}
