using System;

// Token: 0x020013AD RID: 5037
[Flags]
public enum BlockFaceFlag
{
	// Token: 0x04007892 RID: 30866
	None = 0,
	// Token: 0x04007893 RID: 30867
	Top = 1,
	// Token: 0x04007894 RID: 30868
	Bottom = 2,
	// Token: 0x04007895 RID: 30869
	North = 4,
	// Token: 0x04007896 RID: 30870
	West = 8,
	// Token: 0x04007897 RID: 30871
	South = 16,
	// Token: 0x04007898 RID: 30872
	East = 32,
	// Token: 0x04007899 RID: 30873
	All = 63,
	// Token: 0x0400789A RID: 30874
	Solid = 63,
	// Token: 0x0400789B RID: 30875
	Axials = 60
}
