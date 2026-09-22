using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldGenerationEngineFinal
{
	// Token: 0x020016F8 RID: 5880
	public class DataMap<T> where T : struct
	{
		// Token: 0x0600B76E RID: 46958 RVA: 0x00442E54 File Offset: 0x00441054
		public DataMap(int tileWidth, T defaultValue)
		{
			this.width = tileWidth;
			int num = this.width * this.width;
			this.data = new T[num];
			for (int i = 0; i < num; i++)
			{
				this.data[i] = defaultValue;
			}
		}

		// Token: 0x0600B76F RID: 46959 RVA: 0x00442EA1 File Offset: 0x004410A1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Get(int x, int y)
		{
			return this.data[x + y * this.width];
		}

		// Token: 0x0600B770 RID: 46960 RVA: 0x00442EB8 File Offset: 0x004410B8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int x, int y, T _value)
		{
			this.data[x + y * this.width] = _value;
		}

		// Token: 0x0600B771 RID: 46961 RVA: 0x00442ED0 File Offset: 0x004410D0
		public void Replace(T _old, T _new)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int num = this.width * this.width;
			for (int i = 0; i < num; i++)
			{
				if (@default.Equals(this.data[i], _old))
				{
					this.data[i] = _new;
				}
			}
		}

		// Token: 0x04008966 RID: 35174
		public int width;

		// Token: 0x04008967 RID: 35175
		[PublicizedFrom(EAccessModifier.Private)]
		public T[] data;
	}
}
