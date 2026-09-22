using System;
using TMPro;
using UnityEngine;

// Token: 0x02000C55 RID: 3157
public class DamageText : MonoBehaviour
{
	// Token: 0x170009C5 RID: 2501
	// (get) Token: 0x0600601D RID: 24605 RVA: 0x00261649 File Offset: 0x0025F849
	// (set) Token: 0x0600601E RID: 24606 RVA: 0x00261659 File Offset: 0x0025F859
	public static bool Enabled
	{
		get
		{
			return DamageText.enabled || DamageText.SandboxEnabled;
		}
		set
		{
			DamageText.enabled = value;
		}
	}

	// Token: 0x0600601F RID: 24607 RVA: 0x00261664 File Offset: 0x0025F864
	public static void Create(string _text, Color _color, Vector3 _worldPos, Vector3 _velocity, float _scale = 1f)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/DamageText"));
		DamageText component = gameObject.GetComponent<DamageText>();
		TextMeshPro component2 = gameObject.GetComponent<TextMeshPro>();
		component.textMeshPro = component2;
		component2.text = _text;
		component2.color = _color;
		component2.rectTransform.localScale = new Vector3(_scale, _scale, _scale);
		component.worldPos = _worldPos;
		component.velocity = _velocity;
		component.cameraT = Camera.main.transform;
	}

	// Token: 0x06006020 RID: 24608 RVA: 0x002616DC File Offset: 0x0025F8DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		this.TimeDuration -= deltaTime;
		if (this.TimeDuration <= 0f || !this.cameraT)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.textMeshPro.alpha = Utils.FastLerp(0f, 1f, this.TimeDuration * 2f);
		this.velocityDecayDelay -= deltaTime;
		if (this.velocityDecayDelay <= 0f)
		{
			this.velocityDecayDelay = 0.1f;
			this.velocity *= 0.8f;
		}
		this.worldPos += this.velocity * deltaTime;
		base.transform.SetPositionAndRotation(Vector3.MoveTowards(this.worldPos - Origin.position, this.cameraT.position + this.cameraT.forward * 0.18f, 0.25f), this.cameraT.rotation);
	}

	// Token: 0x04004B62 RID: 19298
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public new static bool enabled;

	// Token: 0x04004B63 RID: 19299
	public static bool SandboxEnabled;

	// Token: 0x04004B64 RID: 19300
	public float TimeDuration = 1.5f;

	// Token: 0x04004B65 RID: 19301
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public TextMeshPro textMeshPro;

	// Token: 0x04004B66 RID: 19302
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 worldPos;

	// Token: 0x04004B67 RID: 19303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 velocity;

	// Token: 0x04004B68 RID: 19304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float velocityDecayDelay = 0.2f;

	// Token: 0x04004B69 RID: 19305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform cameraT;
}
