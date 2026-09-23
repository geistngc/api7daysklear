using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020009F9 RID: 2553
public class GameRandom : IMemoryPoolableObject
{
	// Token: 0x06004C2C RID: 19500 RVA: 0x001D8C58 File Offset: 0x001D6E58
	public void SetSeed(int _seed)
	{
		this.InternalSetSeed(_seed);
	}

	// Token: 0x06004C2D RID: 19501 RVA: 0x000027FC File Offset: 0x000009FC
	public void SetLock()
	{
	}

	// Token: 0x06004C2E RID: 19502 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x06004C2F RID: 19503 RVA: 0x000027FC File Offset: 0x000009FC
	public void Reset()
	{
	}

	// Token: 0x17000819 RID: 2073
	// (get) Token: 0x06004C30 RID: 19504 RVA: 0x001D8C61 File Offset: 0x001D6E61
	public double RandomDouble
	{
		get
		{
			return this.NextDouble();
		}
	}

	// Token: 0x1700081A RID: 2074
	// (get) Token: 0x06004C31 RID: 19505 RVA: 0x001D8C69 File Offset: 0x001D6E69
	public float RandomFloat
	{
		get
		{
			return (float)this.NextDouble();
		}
	}

	// Token: 0x1700081B RID: 2075
	// (get) Token: 0x06004C32 RID: 19506 RVA: 0x001D8C72 File Offset: 0x001D6E72
	public int RandomInt
	{
		get
		{
			return this.Next();
		}
	}

	// Token: 0x1700081C RID: 2076
	// (get) Token: 0x06004C33 RID: 19507 RVA: 0x001D8C7C File Offset: 0x001D6E7C
	public Vector2 RandomInsideUnitCircle
	{
		get
		{
			float f = (float)this.NextDouble() * 6.2831855f;
			return new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * (float)Math.Sqrt(this.NextDouble());
		}
	}

	// Token: 0x1700081D RID: 2077
	// (get) Token: 0x06004C34 RID: 19508 RVA: 0x001D8CBC File Offset: 0x001D6EBC
	public Vector2 RandomOnUnitCircle
	{
		get
		{
			float f = (float)this.NextDouble() * 6.2831855f;
			return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		}
	}

	// Token: 0x1700081E RID: 2078
	// (get) Token: 0x06004C35 RID: 19509 RVA: 0x001D8CE8 File Offset: 0x001D6EE8
	public Vector3 RandomOnUnitCircleXZ
	{
		get
		{
			float f = (float)this.NextDouble() * 6.2831855f;
			return new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}
	}

	// Token: 0x1700081F RID: 2079
	// (get) Token: 0x06004C36 RID: 19510 RVA: 0x001D8D1C File Offset: 0x001D6F1C
	public Vector3 RandomInsideUnitSphere
	{
		get
		{
			return new Vector3((float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5)).normalized * (float)Math.Sqrt(this.NextDouble());
		}
	}

	// Token: 0x17000820 RID: 2080
	// (get) Token: 0x06004C37 RID: 19511 RVA: 0x001D8D7C File Offset: 0x001D6F7C
	public Vector3 RandomOnUnitSphere
	{
		get
		{
			return new Vector3((float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5)).normalized;
		}
	}

	// Token: 0x17000821 RID: 2081
	// (get) Token: 0x06004C38 RID: 19512 RVA: 0x001D8DCC File Offset: 0x001D6FCC
	public float RandomGaussian
	{
		get
		{
			float num;
			float num3;
			do
			{
				num = 2f * this.RandomRange(0f, 1f) - 1f;
				float num2 = 2f * this.RandomRange(0f, 1f) - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f || num3 == 0f);
			num3 = Mathf.Sqrt(-2f * Mathf.Log(num3) / num3);
			return num3 * num;
		}
	}

	// Token: 0x06004C39 RID: 19513 RVA: 0x001D8E44 File Offset: 0x001D7044
	public float RandomRange(float _maxExclusive)
	{
		return (float)(this.NextDouble() * (double)_maxExclusive);
	}

	// Token: 0x06004C3A RID: 19514 RVA: 0x001D8E50 File Offset: 0x001D7050
	public float RandomRange(float _min, float _maxExclusive)
	{
		return (float)(this.NextDouble() * (double)(_maxExclusive - _min) + (double)_min);
	}

	// Token: 0x06004C3B RID: 19515 RVA: 0x001D8E61 File Offset: 0x001D7061
	public int RandomRange(int _maxExclusive)
	{
		return this.Next(_maxExclusive);
	}

	// Token: 0x06004C3C RID: 19516 RVA: 0x001D8E6A File Offset: 0x001D706A
	public int RandomRange(int _min, int _maxExclusive)
	{
		return this.Next(_maxExclusive - _min) + _min;
	}

	// Token: 0x06004C3D RID: 19517 RVA: 0x001D8E77 File Offset: 0x001D7077
	[PublicizedFrom(EAccessModifier.Private)]
	public static void log(string _format, params object[] _values)
	{
		Log.Warning(string.Format("{0} GameRandom ", Time.time.ToCultureInvariantString()) + _format, _values);
	}

	// Token: 0x06004C3E RID: 19518 RVA: 0x001D8E9C File Offset: 0x001D709C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InternalSetSeed(int Seed)
	{
		int num = (Seed == int.MinValue) ? int.MaxValue : Math.Abs(Seed);
		int num2 = 161803398 - num;
		this.SeedArray[55] = num2;
		int num3 = 1;
		for (int i = 1; i < 55; i++)
		{
			int num4 = 21 * i % 55;
			this.SeedArray[num4] = num3;
			num3 = num2 - num3;
			if (num3 < 0)
			{
				num3 += int.MaxValue;
			}
			num2 = this.SeedArray[num4];
		}
		for (int j = 1; j < 5; j++)
		{
			for (int k = 1; k < 56; k++)
			{
				this.SeedArray[k] -= this.SeedArray[1 + (k + 30) % 55];
				if (this.SeedArray[k] < 0)
				{
					this.SeedArray[k] += int.MaxValue;
				}
			}
		}
		this.inext = 0;
		this.inextp = 21;
		Seed = 1;
	}

	// Token: 0x06004C3F RID: 19519 RVA: 0x001D8F86 File Offset: 0x001D7186
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Sample()
	{
		return (double)this.InternalSample() * 4.656612875245797E-10;
	}

	// Token: 0x06004C40 RID: 19520 RVA: 0x001D8F9C File Offset: 0x001D719C
	[PublicizedFrom(EAccessModifier.Private)]
	public int InternalSample()
	{
		int num = this.inext;
		int num2 = this.inextp;
		if (++num >= 56)
		{
			num = 1;
		}
		if (++num2 >= 56)
		{
			num2 = 1;
		}
		int num3 = this.SeedArray[num] - this.SeedArray[num2];
		if (num3 == 2147483647)
		{
			num3--;
		}
		if (num3 < 0)
		{
			num3 += int.MaxValue;
		}
		this.SeedArray[num] = num3;
		this.inext = num;
		this.inextp = num2;
		return num3;
	}

	// Token: 0x06004C41 RID: 19521 RVA: 0x001D9010 File Offset: 0x001D7210
	public int PeekSample()
	{
		int num = this.inext;
		int num2 = this.inextp;
		if (++num >= 56)
		{
			num = 1;
		}
		if (++num2 >= 56)
		{
			num2 = 1;
		}
		int num3 = this.SeedArray[num] - this.SeedArray[num2];
		if (num3 == 2147483647)
		{
			num3--;
		}
		if (num3 < 0)
		{
			num3 += int.MaxValue;
		}
		return num3;
	}

	// Token: 0x06004C42 RID: 19522 RVA: 0x001D906C File Offset: 0x001D726C
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Next()
	{
		return this.InternalSample();
	}

	// Token: 0x06004C43 RID: 19523 RVA: 0x001D9074 File Offset: 0x001D7274
	[PublicizedFrom(EAccessModifier.Private)]
	public double GetSampleForLargeRange()
	{
		int num = this.InternalSample();
		if (this.InternalSample() % 2 == 0)
		{
			num = -num;
		}
		return ((double)num + 2147483646.0) / 4294967293.0;
	}

	// Token: 0x06004C44 RID: 19524 RVA: 0x001D90B4 File Offset: 0x001D72B4
	[PublicizedFrom(EAccessModifier.Private)]
	public int Next(int minValue, int maxValue)
	{
		if (minValue > maxValue)
		{
			throw new ArgumentOutOfRangeException("minValue", "Argument_MinMaxValue");
		}
		long num = (long)maxValue - (long)minValue;
		if (num <= 2147483647L)
		{
			return (int)(this.Sample() * (double)num) + minValue;
		}
		return (int)((long)(this.GetSampleForLargeRange() * (double)num) + (long)minValue);
	}

	// Token: 0x06004C45 RID: 19525 RVA: 0x001D90FF File Offset: 0x001D72FF
	[PublicizedFrom(EAccessModifier.Private)]
	public int Next(int maxValue)
	{
		if (maxValue < 0)
		{
			throw new ArgumentOutOfRangeException("maxValue", "ArgumentOutOfRange_MustBePositive");
		}
		return (int)(this.Sample() * (double)maxValue);
	}

	// Token: 0x06004C46 RID: 19526 RVA: 0x001D911F File Offset: 0x001D731F
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double NextDouble()
	{
		return this.Sample();
	}

	// Token: 0x06004C47 RID: 19527 RVA: 0x001D9128 File Offset: 0x001D7328
	[PublicizedFrom(EAccessModifier.Private)]
	public void NextBytes(byte[] buffer)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		for (int i = 0; i < buffer.Length; i++)
		{
			buffer[i] = (byte)(this.InternalSample() % 256);
		}
	}

	// Token: 0x04003BD1 RID: 15313
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MBIG = 2147483647;

	// Token: 0x04003BD2 RID: 15314
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MSEED = 161803398;

	// Token: 0x04003BD3 RID: 15315
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MZ = 0;

	// Token: 0x04003BD4 RID: 15316
	[PublicizedFrom(EAccessModifier.Private)]
	public int inext;

	// Token: 0x04003BD5 RID: 15317
	[PublicizedFrom(EAccessModifier.Private)]
	public int inextp;

	// Token: 0x04003BD6 RID: 15318
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] SeedArray = new int[56];
}
