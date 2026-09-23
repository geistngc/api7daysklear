using System;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x020006C2 RID: 1730
public class EffectGroupDescription
{
	// Token: 0x1700055E RID: 1374
	// (get) Token: 0x060035F0 RID: 13808 RVA: 0x00164606 File Offset: 0x00162806
	public string Description
	{
		get
		{
			if (Localization.Exists(this.DescriptionKey, false))
			{
				return Localization.Get(this.DescriptionKey, false, null);
			}
			return this.CustomDescription;
		}
	}

	// Token: 0x1700055F RID: 1375
	// (get) Token: 0x060035F1 RID: 13809 RVA: 0x0016462A File Offset: 0x0016282A
	public string LongDescription
	{
		get
		{
			return Localization.Get(this.LongDescriptionKey, false, null);
		}
	}

	// Token: 0x060035F2 RID: 13810 RVA: 0x00164639 File Offset: 0x00162839
	public EffectGroupDescription(int _minLevel, int _maxLevel, string _desc_key, string _description, string _long_desc_key)
	{
		this.MinLevel = _minLevel;
		this.MaxLevel = _maxLevel;
		this.DescriptionKey = _desc_key;
		this.CustomDescription = _description;
		this.LongDescriptionKey = _long_desc_key;
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x00164668 File Offset: 0x00162868
	public static EffectGroupDescription ParseDescription(XElement _element)
	{
		if (!_element.HasAttribute("level") || (!_element.HasAttribute("desc_key") && !_element.HasAttribute("desc_base")))
		{
			return null;
		}
		int minLevel;
		int maxLevel;
		if (_element.GetAttribute("level").Contains(","))
		{
			string[] array = _element.GetAttribute("level").Split(',', StringSplitOptions.None);
			if (array.Length < 1)
			{
				return null;
			}
			if (array.Length == 1)
			{
				maxLevel = (minLevel = StringParsers.ParseSInt32(array[0], 0, -1, NumberStyles.Integer));
			}
			else
			{
				minLevel = StringParsers.ParseSInt32(array[0], 0, -1, NumberStyles.Integer);
				maxLevel = StringParsers.ParseSInt32(array[1], 0, -1, NumberStyles.Integer);
			}
		}
		else
		{
			maxLevel = (minLevel = StringParsers.ParseSInt32(_element.GetAttribute("level"), 0, -1, NumberStyles.Integer));
		}
		return new EffectGroupDescription(minLevel, maxLevel, _element.GetAttribute("desc_key"), _element.GetAttribute("desc_base"), _element.HasAttribute("long_desc_key") ? _element.GetAttribute("long_desc_key") : "");
	}

	// Token: 0x04002B8A RID: 11146
	public readonly int MinLevel;

	// Token: 0x04002B8B RID: 11147
	public readonly int MaxLevel;

	// Token: 0x04002B8C RID: 11148
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string DescriptionKey;

	// Token: 0x04002B8D RID: 11149
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string CustomDescription;

	// Token: 0x04002B8E RID: 11150
	public readonly string LongDescriptionKey;
}
