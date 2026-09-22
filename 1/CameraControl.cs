using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200132B RID: 4907
public class CameraControl : MonoBehaviour
{
	// Token: 0x06009B0F RID: 39695 RVA: 0x003A9190 File Offset: 0x003A7390
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.originalRotation = base.transform.localRotation;
		this.textObject.GetComponentInChildren<Text>().text = "PAUSED";
		this.cameraLight = base.transform.GetComponent<Light>();
		this.cameraLight.enabled = false;
	}

	// Token: 0x06009B10 RID: 39696 RVA: 0x003A91E0 File Offset: 0x003A73E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.bPaused = !this.bPaused;
		}
		this.textObject.SetActive(this.bPaused);
		if (this.bPaused)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.cameraLight.enabled = !this.cameraLight.enabled;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f)
		{
			this.cameraLight.spotAngle += 3f;
		}
		else if (axis < 0f)
		{
			this.cameraLight.spotAngle -= 3f;
		}
		Vector3 a = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			a += base.transform.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			a -= base.transform.forward;
		}
		if (Input.GetKey(KeyCode.A))
		{
			a -= base.transform.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			a += base.transform.right;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			a += base.transform.up;
		}
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
		{
			a -= base.transform.up;
		}
		float d = Input.GetKey(KeyCode.LeftShift) ? (this.speed * 2f) : this.speed;
		base.transform.position += a * d;
		this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
		this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
		Quaternion rhs = Quaternion.AngleAxis(this.rotationX, Vector3.up);
		Quaternion rhs2 = Quaternion.AngleAxis(this.rotationY, -Vector3.right);
		base.transform.localRotation = this.originalRotation * rhs * rhs2;
	}

	// Token: 0x040074CE RID: 29902
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion originalRotation;

	// Token: 0x040074CF RID: 29903
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light cameraLight;

	// Token: 0x040074D0 RID: 29904
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotationX;

	// Token: 0x040074D1 RID: 29905
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotationY;

	// Token: 0x040074D2 RID: 29906
	public float sensitivityX = 2f;

	// Token: 0x040074D3 RID: 29907
	public float sensitivityY = 2f;

	// Token: 0x040074D4 RID: 29908
	public float speed = 0.1f;

	// Token: 0x040074D5 RID: 29909
	public GameObject textObject;

	// Token: 0x040074D6 RID: 29910
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bPaused;
}
