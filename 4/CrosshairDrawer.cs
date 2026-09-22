using System;
using UnityEngine;

// Token: 0x02001382 RID: 4994
public class CrosshairDrawer : MonoBehaviour
{
	// Token: 0x06009D9D RID: 40349 RVA: 0x003BB955 File Offset: 0x003B9B55
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnGUI()
	{
		if (!this.draw)
		{
			return;
		}
		EntityPlayerLocal.DrawDynamicCrosshair(this.centerX, this.centerY, this.openAreaX, this.openAreaY, this.length, this.thickness, this.opacity, this.color);
	}

	// Token: 0x040077EE RID: 30702
	public bool draw;

	// Token: 0x040077EF RID: 30703
	public float centerX;

	// Token: 0x040077F0 RID: 30704
	public float centerY;

	// Token: 0x040077F1 RID: 30705
	public float openAreaX;

	// Token: 0x040077F2 RID: 30706
	public float openAreaY;

	// Token: 0x040077F3 RID: 30707
	public float length;

	// Token: 0x040077F4 RID: 30708
	public float thickness;

	// Token: 0x040077F5 RID: 30709
	public float opacity;

	// Token: 0x040077F6 RID: 30710
	public Color color = Color.white;
}
