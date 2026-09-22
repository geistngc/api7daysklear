using System;
using System.Collections.Generic;
using UnityEngine;

namespace GearVariants
{
	// Token: 0x020016F5 RID: 5877
	public class GearVariantMatrixSO : ScriptableObject
	{
		// Token: 0x17001664 RID: 5732
		// (get) Token: 0x0600B761 RID: 46945 RVA: 0x00442A04 File Offset: 0x00440C04
		public static GearVariantMatrixSO Instance
		{
			get
			{
				if (GearVariantMatrixSO._instance == null)
				{
					if (!Application.isPlaying)
					{
						LoadManager.InitSync();
					}
					GearVariantMatrixSO._instance = DataLoader.LoadAsset<GearVariantMatrixSO>("@:Entities/Player/Common/GearVariantMatrix.asset", false);
				}
				return GearVariantMatrixSO._instance;
			}
		}

		// Token: 0x0600B762 RID: 46946 RVA: 0x00442A34 File Offset: 0x00440C34
		[PublicizedFrom(EAccessModifier.Private)]
		public void EnsureIndex(GearVariantMatrixSO.Sex sex)
		{
			List<string> list;
			if (sex != GearVariantMatrixSO.Sex.Male)
			{
				SexGearTables sexGearTables = this.female;
				list = ((sexGearTables != null) ? sexGearTables.gearPaths : null);
			}
			else
			{
				SexGearTables sexGearTables2 = this.male;
				list = ((sexGearTables2 != null) ? sexGearTables2.gearPaths : null);
			}
			List<string> list2 = list ?? new List<string>();
			int count = list2.Count;
			if (sex == GearVariantMatrixSO.Sex.Male)
			{
				if (this._maleIndex == null || this._maleIndexVersionRow != count)
				{
					this._maleIndex = new Dictionary<string, int>(StringComparer.Ordinal);
					for (int i = 0; i < list2.Count; i++)
					{
						this._maleIndex[list2[i]] = i;
					}
					this._maleIndexVersionRow = count;
					return;
				}
			}
			else if (this._femaleIndex == null || this._femaleIndexVersionRow != count)
			{
				this._femaleIndex = new Dictionary<string, int>(StringComparer.Ordinal);
				for (int j = 0; j < list2.Count; j++)
				{
					this._femaleIndex[list2[j]] = j;
				}
				this._femaleIndexVersionRow = count;
			}
		}

		// Token: 0x0600B763 RID: 46947 RVA: 0x00442B1A File Offset: 0x00440D1A
		[PublicizedFrom(EAccessModifier.Private)]
		public SexGearTables GetSexTables(GearVariantMatrixSO.Sex sex)
		{
			if (sex != GearVariantMatrixSO.Sex.Male)
			{
				return this.female;
			}
			return this.male;
		}

		// Token: 0x0600B764 RID: 46948 RVA: 0x00442B2C File Offset: 0x00440D2C
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool TryParseSex(string sexName, out GearVariantMatrixSO.Sex sex)
		{
			string a = sexName.ToLower();
			if (a == "male")
			{
				sex = GearVariantMatrixSO.Sex.Male;
				return true;
			}
			if (!(a == "female"))
			{
				sex = GearVariantMatrixSO.Sex.Male;
				return false;
			}
			sex = GearVariantMatrixSO.Sex.Female;
			return true;
		}

		// Token: 0x0600B765 RID: 46949 RVA: 0x00442B6C File Offset: 0x00440D6C
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool TryParsePart(string partName, out GearVariantMatrixSO.GearPart part)
		{
			if (partName == "head")
			{
				part = GearVariantMatrixSO.GearPart.Head;
				return true;
			}
			if (partName == "hands")
			{
				part = GearVariantMatrixSO.GearPart.Hands;
				return true;
			}
			if (partName == "feet")
			{
				part = GearVariantMatrixSO.GearPart.Feet;
				return true;
			}
			string a = (partName != null) ? partName.Trim().ToLowerInvariant() : null;
			if (a == "head")
			{
				part = GearVariantMatrixSO.GearPart.Head;
				return true;
			}
			if (a == "hands")
			{
				part = GearVariantMatrixSO.GearPart.Hands;
				return true;
			}
			if (!(a == "feet"))
			{
				part = GearVariantMatrixSO.GearPart.Hands;
				return false;
			}
			part = GearVariantMatrixSO.GearPart.Feet;
			return true;
		}

		// Token: 0x0600B766 RID: 46950 RVA: 0x00442BFF File Offset: 0x00440DFF
		[PublicizedFrom(EAccessModifier.Private)]
		public StringTable2D GetTable(GearVariantMatrixSO.Sex sex, GearVariantMatrixSO.GearPart part)
		{
			SexGearTables sexTables = this.GetSexTables(sex);
			if (sexTables == null)
			{
				return null;
			}
			return sexTables.GetTable(part);
		}

		// Token: 0x0600B767 RID: 46951 RVA: 0x00442C14 File Offset: 0x00440E14
		[PublicizedFrom(EAccessModifier.Private)]
		public bool TryGetIndices(GearVariantMatrixSO.Sex sex, string sourceGearPath, string targetBodyPath, out int row, out int col)
		{
			row = -1;
			col = -1;
			if (this.GetSexTables(sex) == null)
			{
				return false;
			}
			this.EnsureIndex(sex);
			Dictionary<string, int> dictionary = (sex == GearVariantMatrixSO.Sex.Male) ? this._maleIndex : this._femaleIndex;
			return dictionary != null && dictionary.TryGetValue(sourceGearPath, out row) && dictionary.TryGetValue(targetBodyPath, out col);
		}

		// Token: 0x0600B768 RID: 46952 RVA: 0x00442C70 File Offset: 0x00440E70
		[PublicizedFrom(EAccessModifier.Private)]
		public string GetVariantOrEmpty(GearVariantMatrixSO.Sex sex, GearVariantMatrixSO.GearPart part, string sourceGearPath, string targetBodyPath)
		{
			if (string.IsNullOrEmpty(sourceGearPath) || string.IsNullOrEmpty(targetBodyPath))
			{
				return string.Empty;
			}
			StringTable2D table = this.GetTable(sex, part);
			if (table == null)
			{
				return string.Empty;
			}
			this.EnsureShapes();
			int num;
			int num2;
			if (!this.TryGetIndices(sex, sourceGearPath, targetBodyPath, out num, out num2))
			{
				return string.Empty;
			}
			if (num < 0 || num >= table.rows.Count)
			{
				return string.Empty;
			}
			if (num2 < 0 || num2 >= table.columnKeys.Count)
			{
				return string.Empty;
			}
			string text = table.Get(num, num2);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return string.Empty;
		}

		// Token: 0x0600B769 RID: 46953 RVA: 0x00442D0C File Offset: 0x00440F0C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool TryGetVariant(GearVariantMatrixSO.Sex sex, GearVariantMatrixSO.GearPart part, string sourceGearPath, string targetBodyPath, out string variant)
		{
			variant = string.Empty;
			if (string.IsNullOrEmpty(sourceGearPath) || string.IsNullOrEmpty(targetBodyPath))
			{
				return false;
			}
			StringTable2D table = this.GetTable(sex, part);
			if (table == null)
			{
				return false;
			}
			this.EnsureShapes();
			int num;
			int num2;
			if (!this.TryGetIndices(sex, sourceGearPath, targetBodyPath, out num, out num2))
			{
				return false;
			}
			if (num < 0 || num >= table.rows.Count)
			{
				return false;
			}
			if (num2 < 0 || num2 >= table.columnKeys.Count)
			{
				return false;
			}
			variant = (table.Get(num, num2) ?? string.Empty);
			return true;
		}

		// Token: 0x0600B76A RID: 46954 RVA: 0x00442D98 File Offset: 0x00440F98
		public string GetVariantOrEmpty(string sex, string part, string sourceGearPath, string targetBodyPath)
		{
			GearVariantMatrixSO.GearPart part2;
			if (!GearVariantMatrixSO.TryParsePart(part, out part2))
			{
				return string.Empty;
			}
			GearVariantMatrixSO.Sex sex2;
			if (!GearVariantMatrixSO.TryParseSex(sex, out sex2))
			{
				return string.Empty;
			}
			return this.GetVariantOrEmpty(sex2, part2, sourceGearPath, targetBodyPath);
		}

		// Token: 0x0600B76B RID: 46955 RVA: 0x00442DD0 File Offset: 0x00440FD0
		public bool TryGetVariant(string sex, string part, string sourceGearPath, string targetBodyPath, out string variant)
		{
			GearVariantMatrixSO.GearPart part2;
			if (!GearVariantMatrixSO.TryParsePart(part, out part2))
			{
				variant = string.Empty;
				return false;
			}
			GearVariantMatrixSO.Sex sex2;
			if (!GearVariantMatrixSO.TryParseSex(sex, out sex2))
			{
				variant = string.Empty;
				return false;
			}
			return this.TryGetVariant(sex2, part2, sourceGearPath, targetBodyPath, out variant);
		}

		// Token: 0x0600B76C RID: 46956 RVA: 0x00442E12 File Offset: 0x00441012
		public void EnsureShapes()
		{
			SexGearTables sexGearTables = this.male;
			if (sexGearTables != null)
			{
				sexGearTables.EnsureShapes();
			}
			SexGearTables sexGearTables2 = this.female;
			if (sexGearTables2 == null)
			{
				return;
			}
			sexGearTables2.EnsureShapes();
		}

		// Token: 0x04008958 RID: 35160
		[Header("Sex: Male")]
		public SexGearTables male = new SexGearTables();

		// Token: 0x04008959 RID: 35161
		[Header("Sex: Female")]
		public SexGearTables female = new SexGearTables();

		// Token: 0x0400895A RID: 35162
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static GearVariantMatrixSO _instance;

		// Token: 0x0400895B RID: 35163
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Dictionary<string, int> _maleIndex;

		// Token: 0x0400895C RID: 35164
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _maleIndexVersionRow;

		// Token: 0x0400895D RID: 35165
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Dictionary<string, int> _femaleIndex;

		// Token: 0x0400895E RID: 35166
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _femaleIndexVersionRow;

		// Token: 0x020016F6 RID: 5878
		public enum GearPart
		{
			// Token: 0x04008960 RID: 35168
			Head,
			// Token: 0x04008961 RID: 35169
			Hands,
			// Token: 0x04008962 RID: 35170
			Feet
		}

		// Token: 0x020016F7 RID: 5879
		public enum Sex
		{
			// Token: 0x04008964 RID: 35172
			Male,
			// Token: 0x04008965 RID: 35173
			Female
		}
	}
}
