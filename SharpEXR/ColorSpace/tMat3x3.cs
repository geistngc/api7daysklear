using System;

namespace SharpEXR.ColorSpace
{
	// Token: 0x020016E2 RID: 5858
	public struct tMat3x3
	{
		// Token: 0x0600B72B RID: 46891 RVA: 0x0044170E File Offset: 0x0043F90E
		public void SetCol(int colIdx, tVec3 vec)
		{
			this[0, colIdx] = vec.X;
			this[1, colIdx] = vec.Y;
			this[2, colIdx] = vec.Z;
		}

		// Token: 0x0600B72C RID: 46892 RVA: 0x0044173C File Offset: 0x0043F93C
		public bool Invert(out tMat3x3 result)
		{
			result = default(tMat3x3);
			float num = this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1];
			float num2 = this[1, 2] * this[2, 0] - this[1, 0] * this[2, 2];
			float num3 = this[1, 0] * this[2, 1] - this[1, 1] * this[2, 0];
			float num4 = this[0, 0] * num + this[0, 1] * num2 + this[0, 2] * num3;
			if (num4 > -1E-06f && num4 < 1E-06f)
			{
				return false;
			}
			float num5 = 1f / num4;
			result[0, 0] = num5 * num;
			result[0, 1] = num5 * (this[2, 1] * this[0, 2] - this[2, 2] * this[0, 1]);
			result[0, 2] = num5 * (this[0, 1] * this[1, 2] - this[0, 2] * this[1, 1]);
			result[1, 0] = num5 * num2;
			result[1, 1] = num5 * (this[2, 2] * this[0, 0] - this[2, 0] * this[0, 2]);
			result[1, 2] = num5 * (this[0, 2] * this[1, 0] - this[0, 0] * this[1, 2]);
			result[2, 0] = num5 * num3;
			result[2, 1] = num5 * (this[2, 0] * this[0, 1] - this[2, 1] * this[0, 0]);
			result[2, 2] = num5 * (this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0]);
			return true;
		}

		// Token: 0x0600B72D RID: 46893 RVA: 0x00441934 File Offset: 0x0043FB34
		public static tVec3 operator *(tMat3x3 mat, tVec3 vec)
		{
			return new tVec3(mat[0, 0] * vec.X + mat[0, 1] * vec.Y + mat[0, 2] * vec.Z, mat[1, 0] * vec.X + mat[1, 1] * vec.Y + mat[1, 2] * vec.Z, mat[2, 0] * vec.X + mat[2, 1] * vec.Y + mat[2, 2] * vec.Z);
		}

		// Token: 0x17001654 RID: 5716
		public float this[int row, int col]
		{
			get
			{
				if (row != 0)
				{
					if (row != 1)
					{
						if (col == 1)
						{
							return this.M21;
						}
						if (col != 2)
						{
							return this.M20;
						}
						return this.M22;
					}
					else
					{
						if (col == 1)
						{
							return this.M11;
						}
						if (col != 2)
						{
							return this.M10;
						}
						return this.M12;
					}
				}
				else
				{
					if (col == 1)
					{
						return this.M01;
					}
					if (col != 2)
					{
						return this.M00;
					}
					return this.M02;
				}
			}
			set
			{
				if (row != 0)
				{
					if (row != 1)
					{
						if (col == 1)
						{
							this.M21 = value;
							return;
						}
						if (col != 2)
						{
							this.M20 = value;
							return;
						}
						this.M22 = value;
						return;
					}
					else
					{
						if (col == 1)
						{
							this.M11 = value;
							return;
						}
						if (col != 2)
						{
							this.M10 = value;
							return;
						}
						this.M12 = value;
						return;
					}
				}
				else
				{
					if (col == 1)
					{
						this.M01 = value;
						return;
					}
					if (col != 2)
					{
						this.M00 = value;
						return;
					}
					this.M02 = value;
					return;
				}
			}
		}

		// Token: 0x04008916 RID: 35094
		public float M00;

		// Token: 0x04008917 RID: 35095
		public float M01;

		// Token: 0x04008918 RID: 35096
		public float M02;

		// Token: 0x04008919 RID: 35097
		public float M10;

		// Token: 0x0400891A RID: 35098
		public float M11;

		// Token: 0x0400891B RID: 35099
		public float M12;

		// Token: 0x0400891C RID: 35100
		public float M20;

		// Token: 0x0400891D RID: 35101
		public float M21;

		// Token: 0x0400891E RID: 35102
		public float M22;
	}
}
