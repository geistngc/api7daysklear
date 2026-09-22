using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001A29 RID: 6697
	public abstract class AbstractFilter : IFilter<SectionType>
	{
		// Token: 0x0600CB3F RID: 52031
		public abstract List<SectionType> Filter(List<SectionType> _list);

		// Token: 0x0600CB40 RID: 52032 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractFilter()
		{
		}
	}
}
