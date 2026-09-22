using System;
using UnityEngine;

// Token: 0x02000B1E RID: 2846
public class ChunkCoordinates
{
	// Token: 0x0600560A RID: 22026 RVA: 0x0020EDF0 File Offset: 0x0020CFF0
	public ChunkCoordinates(int _x, int _y, int _z)
	{
		this.position = new Vector3i(_x, _y, _z);
	}

	// Token: 0x0600560B RID: 22027 RVA: 0x0020EE06 File Offset: 0x0020D006
	public ChunkCoordinates(ChunkCoordinates _cc)
	{
		this.position = _cc.position;
	}

	// Token: 0x0600560C RID: 22028 RVA: 0x0020EE1A File Offset: 0x0020D01A
	public override bool Equals(object _obj)
	{
		return _obj is ChunkCoordinates && this.position.Equals(((ChunkCoordinates)_obj).position);
	}

	// Token: 0x0600560D RID: 22029 RVA: 0x0020EE3C File Offset: 0x0020D03C
	public override int GetHashCode()
	{
		return this.position.x + this.position.z << 8 + this.position.y << 16;
	}

	// Token: 0x0600560E RID: 22030 RVA: 0x0020EE6C File Offset: 0x0020D06C
	public float getDistance(int _x, int _y, int _z)
	{
		int num = this.position.x - _x;
		int num2 = this.position.y - _y;
		int num3 = this.position.z - _z;
		return Mathf.Sqrt((float)(num * num + num2 * num2 + num3 * num3));
	}

	// Token: 0x0600560F RID: 22031 RVA: 0x0020EEB4 File Offset: 0x0020D0B4
	public float getDistanceSquared(int _x, int _y, int _z)
	{
		int num = this.position.x - _x;
		int num2 = this.position.y - _y;
		int num3 = this.position.z - _z;
		return (float)(num * num + num2 * num2 + num3 * num3);
	}

	// Token: 0x04004287 RID: 17031
	public Vector3i position;
}
