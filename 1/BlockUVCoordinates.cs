using System;
using UnityEngine;

// Token: 0x02000AFD RID: 2813
public class BlockUVCoordinates
{
	// Token: 0x060053CA RID: 21450 RVA: 0x00201684 File Offset: 0x001FF884
	public BlockUVCoordinates(Rect topUvCoordinates, Rect sideUvCoordinates, Rect bottomUvCoordinates)
	{
		this.BlockFaceUvCoordinates[0] = topUvCoordinates;
		this.BlockFaceUvCoordinates[1] = bottomUvCoordinates;
		this.BlockFaceUvCoordinates[2] = sideUvCoordinates;
		this.BlockFaceUvCoordinates[4] = sideUvCoordinates;
		this.BlockFaceUvCoordinates[3] = sideUvCoordinates;
		this.BlockFaceUvCoordinates[5] = sideUvCoordinates;
	}

	// Token: 0x060053CB RID: 21451 RVA: 0x002016F4 File Offset: 0x001FF8F4
	public BlockUVCoordinates(Rect topUvCoordinates, Rect bottomUvCoordinates, Rect northUvCoordinates, Rect southUvCoordinates, Rect westUvCoordinates, Rect eastUvCoordinates)
	{
		this.BlockFaceUvCoordinates[0] = topUvCoordinates;
		this.BlockFaceUvCoordinates[1] = bottomUvCoordinates;
		this.BlockFaceUvCoordinates[2] = northUvCoordinates;
		this.BlockFaceUvCoordinates[4] = southUvCoordinates;
		this.BlockFaceUvCoordinates[3] = westUvCoordinates;
		this.BlockFaceUvCoordinates[5] = eastUvCoordinates;
	}

	// Token: 0x1700090E RID: 2318
	// (get) Token: 0x060053CC RID: 21452 RVA: 0x00201764 File Offset: 0x001FF964
	public Rect[] BlockFaceUvCoordinates
	{
		get
		{
			return this.m_BlockFaceUvCoordinates;
		}
	}

	// Token: 0x0400418F RID: 16783
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Rect[] m_BlockFaceUvCoordinates = new Rect[6];
}
