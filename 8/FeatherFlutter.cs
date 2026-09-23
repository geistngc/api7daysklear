using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
[RequireComponent(typeof(ParticleSystem))]
public class FeatherFlutter : MonoBehaviour
{
	// Token: 0x0600001D RID: 29 RVA: 0x00002937 File Offset: 0x00000B37
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.ps = base.GetComponent<ParticleSystem>();
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002948 File Offset: 0x00000B48
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		int particleCount = this.ps.particleCount;
		if (particleCount == 0)
		{
			return;
		}
		if (this.particles == null || this.particles.Length < particleCount)
		{
			this.particles = new ParticleSystem.Particle[particleCount];
		}
		this.ps.GetParticles(this.particles, particleCount);
		for (int i = 0; i < particleCount; i++)
		{
			if (this.particles[i].velocity.y < 0f)
			{
				float num = this.particles[i].randomSeed % 1000U / 1000f * this.phaseSpread;
				float num2 = this.particles[i].startLifetime - this.particles[i].remainingLifetime;
				float num3 = Mathf.Clamp01(-this.particles[i].velocity.y / this.fallBlendSpeed);
				float b = Mathf.Sin(num2 * this.frequency + num) * this.amplitude * num3;
				float b2 = Mathf.Sin(num2 * this.zFrequency + num + 1.5f) * this.zAmplitude * num3;
				Vector3 velocity = this.particles[i].velocity;
				velocity.x = Mathf.Lerp(velocity.x, b, 1f - this.lateralDrag);
				velocity.z = Mathf.Lerp(velocity.z, b2, 1f - this.lateralDrag);
				this.particles[i].velocity = velocity;
				float b3 = -velocity.z * this.rotationInfluence;
				float b4 = velocity.x * this.rotationInfluence;
				Vector3 rotation3D = this.particles[i].rotation3D;
				rotation3D.x = Mathf.Lerp(rotation3D.x, b3, 1f - this.rotationDrag);
				rotation3D.z = Mathf.Lerp(rotation3D.z, b4, 1f - this.rotationDrag);
				this.particles[i].rotation3D = rotation3D;
			}
		}
		this.ps.SetParticles(this.particles, particleCount);
	}

	// Token: 0x04000026 RID: 38
	public float amplitude = 0.5f;

	// Token: 0x04000027 RID: 39
	public float frequency = 1.5f;

	// Token: 0x04000028 RID: 40
	public float phaseSpread = 6f;

	// Token: 0x04000029 RID: 41
	public float fallBlendSpeed = 0.5f;

	// Token: 0x0400002A RID: 42
	public float lateralDrag = 0.85f;

	// Token: 0x0400002B RID: 43
	public float zAmplitude = 0.3f;

	// Token: 0x0400002C RID: 44
	public float zFrequency = 1.1f;

	// Token: 0x0400002D RID: 45
	public float rotationInfluence = 25f;

	// Token: 0x0400002E RID: 46
	public float rotationDrag = 0.9f;

	// Token: 0x0400002F RID: 47
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ParticleSystem ps;

	// Token: 0x04000030 RID: 48
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ParticleSystem.Particle[] particles;
}
