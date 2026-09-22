using System;
using UnityEngine;

// Token: 0x02000B41 RID: 2881
public class ChunkVertexLayer : IMemoryPoolableObject
{
	// Token: 0x06005748 RID: 22344 RVA: 0x00218A58 File Offset: 0x00216C58
	public ChunkVertexLayer()
	{
		this.wPow = 4;
		this.hPow = 4;
		int num = 1 << this.wPow;
		int num2 = 1 << this.hPow;
		this.m_Vertices = new Vector3[num * num2];
		this.yPos = new float[num * num2];
		this.valid = new bool[num * num2];
	}

	// Token: 0x06005749 RID: 22345 RVA: 0x00218ABC File Offset: 0x00216CBC
	public void Reset()
	{
		for (int i = 0; i < this.valid.Length; i++)
		{
			this.m_Vertices[i] = Vector3.zero;
			this.yPos[i] = 0f;
			this.valid[i] = false;
		}
	}

	// Token: 0x0600574A RID: 22346 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x0600574B RID: 22347 RVA: 0x00218B04 File Offset: 0x00216D04
	public bool getAt(int _x, int _y, out Vector3 _vec)
	{
		int offs = _x + (_y << this.wPow);
		return this.getAt(offs, out _vec);
	}

	// Token: 0x0600574C RID: 22348 RVA: 0x00218B27 File Offset: 0x00216D27
	public bool getAt(int _offs, out Vector3 _vec)
	{
		_vec = this.m_Vertices[_offs];
		return this.valid[_offs];
	}

	// Token: 0x0600574D RID: 22349 RVA: 0x00218B44 File Offset: 0x00216D44
	public void setAt(int _x, int _y, Vector3 _v)
	{
		int num = _x + (_y << this.wPow);
		this.m_Vertices[num] = _v;
		this.valid[num] = true;
	}

	// Token: 0x0600574E RID: 22350 RVA: 0x00218B78 File Offset: 0x00216D78
	public bool getYPosAt(int _x, int _y, out float _ypos)
	{
		int offs = _x + (_y << this.wPow);
		return this.getYPosAt(offs, out _ypos);
	}

	// Token: 0x0600574F RID: 22351 RVA: 0x00218B9B File Offset: 0x00216D9B
	public bool getYPosAt(int _offs, out float _ypos)
	{
		_ypos = this.yPos[_offs];
		return this.valid[_offs];
	}

	// Token: 0x06005750 RID: 22352 RVA: 0x00218BB0 File Offset: 0x00216DB0
	public void setYPosAt(int _x, int _y, float _ypos)
	{
		int num = _x + (_y << this.wPow);
		this.yPos[num] = _ypos;
		this.valid[num] = true;
	}

	// Token: 0x06005751 RID: 22353 RVA: 0x00218BDD File Offset: 0x00216DDD
	public void setInvalid(int _offs)
	{
		this.valid[_offs] = false;
	}

	// Token: 0x06005752 RID: 22354 RVA: 0x00218BE8 File Offset: 0x00216DE8
	public int GetUsedMem()
	{
		return this.m_Vertices.Length * 13 + 8;
	}

	// Token: 0x040043B0 RID: 17328
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] m_Vertices;

	// Token: 0x040043B1 RID: 17329
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] yPos;

	// Token: 0x040043B2 RID: 17330
	[PublicizedFrom(EAccessModifier.Private)]
	public bool[] valid;

	// Token: 0x040043B3 RID: 17331
	[PublicizedFrom(EAccessModifier.Private)]
	public int wPow;

	// Token: 0x040043B4 RID: 17332
	[PublicizedFrom(EAccessModifier.Private)]
	public int hPow;

	// Token: 0x040043B5 RID: 17333
	public static int InstanceCount;
}
