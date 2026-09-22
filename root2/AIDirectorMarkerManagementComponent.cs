using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000416 RID: 1046
[Preserve]
public class AIDirectorMarkerManagementComponent : AIDirectorComponent
{
	// Token: 0x06002059 RID: 8281 RVA: 0x000C3F9C File Offset: 0x000C219C
	public override void Tick(double _dt)
	{
		base.Tick(_dt);
		this.TickMarkers(_dt);
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x000C3FAC File Offset: 0x000C21AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickMarkers(double _dt)
	{
		for (int i = this.markers.Count - 1; i >= 0; i--)
		{
			IAIDirectorMarker iaidirectorMarker = this.markers[i];
			iaidirectorMarker.Tick(_dt);
			if (iaidirectorMarker.TimeToLive <= 0f || (iaidirectorMarker.Player != null && iaidirectorMarker.Player.IsDead()))
			{
				this.markers.RemoveAt(i);
				iaidirectorMarker.Release();
			}
		}
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x000C4020 File Offset: 0x000C2220
	public IAIDirectorMarker FindBestMarker(Vector3 _pos, ref double _inOutIntensity)
	{
		IAIDirectorMarker result = null;
		int num = -1;
		for (int i = this.markers.Count - 1; i >= 0; i--)
		{
			IAIDirectorMarker iaidirectorMarker = this.markers[i];
			if (iaidirectorMarker.TimeToLive > 0f)
			{
				double num2 = iaidirectorMarker.IntensityForPosition(_pos);
				if (num2 > 0.0 && iaidirectorMarker.Priority > num)
				{
					num = iaidirectorMarker.Priority;
					result = iaidirectorMarker;
					_inOutIntensity = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x04001623 RID: 5667
	[PublicizedFrom(EAccessModifier.Private)]
	public List<IAIDirectorMarker> markers = new List<IAIDirectorMarker>(256);
}
