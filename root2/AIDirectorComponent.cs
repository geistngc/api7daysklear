using System;
using System.IO;

// Token: 0x02000410 RID: 1040
public abstract class AIDirectorComponent
{
	// Token: 0x06002033 RID: 8243 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Connect()
	{
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void InitNewGame()
	{
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Tick(double _dt)
	{
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Read(BinaryReader _stream, int _version)
	{
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Write(BinaryWriter _stream)
	{
	}

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x06002038 RID: 8248 RVA: 0x000C315D File Offset: 0x000C135D
	public GameRandom Random
	{
		get
		{
			return this.Director.random;
		}
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public AIDirectorComponent()
	{
	}

	// Token: 0x040015EB RID: 5611
	public AIDirector Director;
}
