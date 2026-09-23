using System;
using UnityEngine;

// Token: 0x0200042B RID: 1067
public interface IAIDirectorMarker
{
	// Token: 0x060020C5 RID: 8389
	void Reference();

	// Token: 0x060020C6 RID: 8390
	bool Release();

	// Token: 0x060020C7 RID: 8391
	void Tick(double dt);

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x060020C8 RID: 8392
	EntityPlayer Player { get; }

	// Token: 0x060020C9 RID: 8393
	double IntensityForPosition(Vector3 position);

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x060020CA RID: 8394
	Vector3 Position { get; }

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x060020CB RID: 8395
	Vector3 TargetPosition { get; }

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x060020CC RID: 8396
	bool Valid { get; }

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x060020CD RID: 8397
	float MaxRadius { get; }

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x060020CE RID: 8398
	float Radius { get; }

	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x060020CF RID: 8399
	float TimeToLive { get; }

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x060020D0 RID: 8400
	float ValidTime { get; }

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x060020D1 RID: 8401
	float Speed { get; }

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x060020D2 RID: 8402
	int Priority { get; }

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x060020D3 RID: 8403
	bool InterruptsNonPlayerAttack { get; }

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x060020D4 RID: 8404
	bool IsDistraction { get; }
}
