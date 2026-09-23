using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x020006C1 RID: 1729
public class EffectDisplayValue
{
	// Token: 0x060035EA RID: 13802 RVA: 0x00164287 File Offset: 0x00162487
	public EffectDisplayValue(string _name, float[] _value, float[] _levels, RequirementGroup _requirements)
	{
		this.Name = _name;
		this.Values = _value;
		this.Levels = _levels;
		this.Requirements = _requirements;
	}

	// Token: 0x060035EB RID: 13803 RVA: 0x001642AC File Offset: 0x001624AC
	public bool IsValid(MinEventParams _params)
	{
		return this.canRun(_params);
	}

	// Token: 0x060035EC RID: 13804 RVA: 0x001642B5 File Offset: 0x001624B5
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canRun(MinEventParams _params)
	{
		return this.Requirements == null || this.Requirements.IsValid(_params);
	}

	// Token: 0x060035ED RID: 13805 RVA: 0x001642CD File Offset: 0x001624CD
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool InLevelRange(float _level, float _min, float _max)
	{
		return _level >= _min && _level <= _max;
	}

	// Token: 0x060035EE RID: 13806 RVA: 0x001642DC File Offset: 0x001624DC
	public float GetValue(int _level)
	{
		if (this.Levels != null)
		{
			if (this.Values != null)
			{
				if (this.Values.Length == this.Levels.Length)
				{
					if (this.Levels.Length >= 2)
					{
						for (int i = 0; i < this.Levels.Length - 1; i += 2)
						{
							if (EffectDisplayValue.InLevelRange((float)_level, this.Levels[i], this.Levels[i + 1]))
							{
								return Mathf.Lerp(this.Values[i], this.Values[i + 1], ((float)_level - this.Levels[i]) / (this.Levels[i + 1] - this.Levels[i]));
							}
						}
					}
					else if (this.Levels.Length >= 1 && (float)_level == this.Levels[0])
					{
						return this.Values[0];
					}
				}
				else if (this.Values.Length == 2 && this.Levels.Length == 1)
				{
					GameRandom tempGameRandom = GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed);
					if (MinEventParams.CachedEventParam.Seed == 0)
					{
						return (this.Values[0] + this.Values[1]) * 0.5f;
					}
					return tempGameRandom.RandomRange(this.Values[0], this.Values[1]);
				}
				else if (this.Values.Length == 1 && this.Levels.Length == 2 && EffectDisplayValue.InLevelRange((float)_level, this.Levels[0], this.Levels[1]))
				{
					return this.Values[0];
				}
			}
		}
		else if (this.Values != null)
		{
			if (this.Values.Length == 1)
			{
				return this.Values[0];
			}
			if (this.Values.Length == 2)
			{
				return GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed).RandomRange(this.Values[0], this.Values[1]);
			}
			return this.Values[0];
		}
		return 0f;
	}

	// Token: 0x060035EF RID: 13807 RVA: 0x001644B4 File Offset: 0x001626B4
	public static EffectDisplayValue ParseDisplayValue(XElement _element)
	{
		if (!_element.HasAttribute("name") || !_element.HasAttribute("value"))
		{
			return null;
		}
		RequirementGroup requirements = RequirementBase.ParseRequirementGroup(_element);
		string attribute = _element.GetAttribute("value");
		float[] array = null;
		if (!string.IsNullOrEmpty(attribute))
		{
			if (attribute.Contains(","))
			{
				string[] array2 = attribute.Split(',', StringSplitOptions.None);
				array = new float[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					float num;
					if (StringParsers.TryParseFloat(array2[i], out num))
					{
						array[i] = num;
					}
				}
			}
			else
			{
				array = new float[]
				{
					StringParsers.ParseFloat(attribute, 0, -1, NumberStyles.Any)
				};
			}
		}
		string attribute2 = _element.GetAttribute("tier");
		float[] array3 = null;
		if (!string.IsNullOrEmpty(attribute2))
		{
			if (attribute2.Contains(","))
			{
				string[] array4 = attribute2.Split(',', StringSplitOptions.None);
				array3 = new float[array4.Length];
				for (int j = 0; j < array4.Length; j++)
				{
					array3[j] = StringParsers.ParseFloat(array4[j], 0, -1, NumberStyles.Any);
				}
			}
			else
			{
				array3 = new float[]
				{
					StringParsers.ParseFloat(attribute2, 0, -1, NumberStyles.Any)
				};
			}
		}
		return new EffectDisplayValue(_element.GetAttribute("name"), array, array3, requirements);
	}

	// Token: 0x04002B85 RID: 11141
	public string Name;

	// Token: 0x04002B86 RID: 11142
	public float[] Values;

	// Token: 0x04002B87 RID: 11143
	public float[] Levels;

	// Token: 0x04002B88 RID: 11144
	public RequirementGroup Requirements;

	// Token: 0x04002B89 RID: 11145
	public bool OrCompare;
}
