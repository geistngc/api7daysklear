using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001723 RID: 5923
	public static class RandomCountyNameGenerator
	{
		// Token: 0x0600B84D RID: 47181 RVA: 0x00448F90 File Offset: 0x00447190
		public static string GetName(int _seed)
		{
			string[] array = RandomCountyNameGenerator.constenants.Split(',', StringSplitOptions.None);
			string[] array2 = RandomCountyNameGenerator.vowels.Split(',', StringSplitOptions.None);
			List<string> list = new List<string>();
			for (int i = 0; i < array2.Length; i++)
			{
				list.Add(array2[i]);
			}
			for (int j = 0; j < array.Length; j++)
			{
				for (int k = 0; k < array2.Length; k++)
				{
					list.Add(array[j] + array2[k]);
				}
			}
			string[] array3 = list.ToArray();
			Rand.Instance.SetSeed(_seed);
			string text = "";
			string text2 = "";
			string text3 = RandomCountyNameGenerator.prefixes[Rand.Instance.Range(0, RandomCountyNameGenerator.prefixes.Length)];
			if (text3.Length > 0)
			{
				text2 = text2 + text3 + " ";
			}
			int num = Rand.Instance.Range(3, 5);
			for (int l = 0; l < num; l++)
			{
				text += array3[Rand.Instance.Range(0, array3.Length)];
			}
			string text4 = text.Substring(0, 1);
			text = text.Remove(0, 1);
			text = text4.ToUpper() + text;
			text2 += text;
			string text5 = RandomCountyNameGenerator.suffixes[Rand.Instance.Range(0, RandomCountyNameGenerator.suffixes.Length)];
			if (text5.Length > 0)
			{
				text2 += text5;
			}
			return text2;
		}

		// Token: 0x04008A04 RID: 35332
		[PublicizedFrom(EAccessModifier.Private)]
		public static string constenants = "b,c,d,f,g,h,j,k,l,m,n,p,r,s,t,v,w,x,y,z";

		// Token: 0x04008A05 RID: 35333
		[PublicizedFrom(EAccessModifier.Private)]
		public static string vowels = "a,e,i,o,u";

		// Token: 0x04008A06 RID: 35334
		[PublicizedFrom(EAccessModifier.Private)]
		public static string[] prefixes = new string[]
		{
			"Old",
			"",
			"New",
			"",
			"North",
			"",
			"East",
			"",
			"South",
			"",
			"West",
			""
		};

		// Token: 0x04008A07 RID: 35335
		[PublicizedFrom(EAccessModifier.Private)]
		public static string[] suffixes = new string[]
		{
			" County",
			" Territory",
			" Valley",
			" Mountains"
		};
	}
}
