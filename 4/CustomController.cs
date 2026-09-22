using System;
using UnityEngine;

// Token: 0x020013C4 RID: 5060
public class CustomController : MonoBehaviour
{
	// Token: 0x06009F25 RID: 40741 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
	}

	// Token: 0x06009F26 RID: 40742 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
	}

	// Token: 0x06009F27 RID: 40743 RVA: 0x003C2318 File Offset: 0x003C0518
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CollidesWithX(Vector3 position, float movement, out float newVelocity)
	{
		float num = this.Speed * (float)Math.Sign(movement);
		Vector3 vector = new Vector3(position.x + num + this.m_BoxWidth, position.y, position.z);
		if (!this.m_WorldData.GetBlock((int)vector.x, (int)vector.y, (int)vector.z).Equals(BlockValue.Air))
		{
			newVelocity = 0f;
			return true;
		}
		newVelocity = movement;
		return false;
	}

	// Token: 0x06009F28 RID: 40744 RVA: 0x003C2394 File Offset: 0x003C0594
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CollidesWithY(Vector3 position, float movement, out float newVelocity)
	{
		float num = this.Speed * (float)Math.Sign(movement);
		Vector3 vector = new Vector3(position.x, position.y + num + this.m_BoxWidth, position.z);
		Log.Out(string.Concat(new string[]
		{
			"Checking ",
			vector.x.ToCultureInvariantString(),
			", ",
			vector.z.ToCultureInvariantString(),
			", ",
			vector.y.ToCultureInvariantString()
		}));
		BlockValue block = this.m_WorldData.GetBlock((int)vector.x, (int)vector.z, (int)vector.y);
		if (!block.Equals(BlockValue.Air))
		{
			string[] array = new string[8];
			array[0] = "Block ";
			int num2 = 1;
			BlockValue blockValue = block;
			array[num2] = blockValue.ToString();
			array[2] = " hit at ";
			array[3] = vector.x.ToCultureInvariantString();
			array[4] = ", ";
			array[5] = vector.z.ToCultureInvariantString();
			array[6] = ", ";
			array[7] = vector.y.ToCultureInvariantString();
			Log.Out(string.Concat(array));
			newVelocity = 0f;
			return true;
		}
		newVelocity = movement;
		return false;
	}

	// Token: 0x040078D9 RID: 30937
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float m_BoxWidth = 0.5f;

	// Token: 0x040078DA RID: 30938
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_Velocity;

	// Token: 0x040078DB RID: 30939
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_Forward;

	// Token: 0x040078DC RID: 30940
	public float Speed = 0.1f;

	// Token: 0x040078DD RID: 30941
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public World m_WorldData;
}
