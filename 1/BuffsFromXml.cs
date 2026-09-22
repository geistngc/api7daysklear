using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000CFC RID: 3324
public class BuffsFromXml
{
	// Token: 0x060065A1 RID: 26017 RVA: 0x0027C94C File Offset: 0x0027AB4C
	public static IEnumerator CreateBuffs(XmlFile xmlFile)
	{
		BuffManager.Buffs = new CaseInsensitiveStringDictionary<BuffClass>();
		XElement root = xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <buffs> found!");
		}
		MicroStopwatch msw = new MicroStopwatch(true);
		foreach (XElement element in root.Elements("buff"))
		{
			BuffsFromXml.ParseBuff(element);
			if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				yield return null;
				msw.ResetAndRestart();
			}
		}
		IEnumerator<XElement> enumerator = null;
		BuffsFromXml.clearBuffValueLinks();
		yield break;
		yield break;
	}

	// Token: 0x060065A2 RID: 26018 RVA: 0x0027C95C File Offset: 0x0027AB5C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void clearBuffValueLinks()
	{
		if (!GameManager.Instance)
		{
			return;
		}
		World world = GameManager.Instance.World;
		DictionaryList<int, Entity> dictionaryList = (world != null) ? world.Entities : null;
		if (dictionaryList == null || dictionaryList.Count == 0)
		{
			return;
		}
		foreach (Entity entity in dictionaryList.list)
		{
			EntityAlive entityAlive = entity as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.Buffs.ClearBuffClassLinks();
			}
		}
	}

	// Token: 0x060065A3 RID: 26019 RVA: 0x0027C9EC File Offset: 0x0027ABEC
	public static void Reload(XmlFile xmlFile)
	{
		ThreadManager.RunCoroutineSync(BuffsFromXml.CreateBuffs(xmlFile));
	}

	// Token: 0x060065A4 RID: 26020 RVA: 0x0027C9FC File Offset: 0x0027ABFC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseBuff(XElement _element)
	{
		BuffClass buffClass = new BuffClass("");
		string attribute = _element.GetAttribute("name");
		if (attribute.Length > 0)
		{
			buffClass.Name = attribute.ToLower();
			buffClass.NameTag = FastTags<TagGroup.Global>.Parse(attribute);
			attribute = _element.GetAttribute("name_key");
			if (attribute.Length > 0)
			{
				buffClass.LocalizedName = Localization.Get(attribute, false, null);
			}
			else
			{
				buffClass.LocalizedName = Localization.Get(buffClass.Name, false, null);
			}
			attribute = _element.GetAttribute("description_key");
			if (attribute.Length > 0)
			{
				buffClass.DescriptionKey = attribute;
				buffClass.Description = Localization.Get(attribute, false, null);
			}
			attribute = _element.GetAttribute("tooltip_key");
			if (attribute.Length > 0)
			{
				buffClass.TooltipKey = attribute;
				buffClass.Tooltip = Localization.Get(attribute, false, null);
			}
			attribute = _element.GetAttribute("icon");
			buffClass.Icon = ((attribute.Length > 0) ? attribute : null);
			attribute = _element.GetAttribute("hidden");
			buffClass.Hidden = (attribute.Length > 0 && StringParsers.ParseBool(attribute, 0, -1, true));
			attribute = _element.GetAttribute("showonhud");
			buffClass.ShowOnHUD = (attribute.Length == 0 || StringParsers.ParseBool(attribute, 0, -1, true));
			attribute = _element.GetAttribute("update_rate");
			buffClass.UpdateRateTicks = ((attribute.Length > 0) ? ((int)(StringParsers.ParseFloat(attribute, 0, -1, NumberStyles.Any) * 20f)) : 20);
			attribute = _element.GetAttribute("allow_in_editor");
			buffClass.AllowInEditor = (attribute.Length > 0 && StringParsers.ParseBool(attribute, 0, -1, true));
			attribute = _element.GetAttribute("required_game_stat");
			buffClass.RequiredGameStat = ((attribute.Length > 0) ? Enum.Parse<EnumGameStats>(attribute) : EnumGameStats.Last);
			attribute = _element.GetAttribute("remove_on_death");
			buffClass.RemoveOnDeath = (attribute.Length == 0 || StringParsers.ParseBool(attribute, 0, -1, true));
			attribute = _element.GetAttribute("display_type");
			buffClass.DisplayType = ((attribute.Length > 0) ? EnumUtils.Parse<EnumEntityUINotificationDisplayMode>(attribute, false) : EnumEntityUINotificationDisplayMode.IconOnly);
			attribute = _element.GetAttribute("icon_color");
			buffClass.IconColor = ((attribute.Length > 0) ? StringParsers.ParseColor32(attribute) : Color.white);
			attribute = _element.GetAttribute("icon_blink");
			buffClass.IconBlink = (attribute.Length > 0 && StringParsers.ParseBool(attribute, 0, -1, true));
			buffClass.DamageSource = EnumDamageSource.Internal;
			buffClass.DamageType = EnumDamageTypes.None;
			buffClass.StackType = BuffEffectStackTypes.Replace;
			buffClass.DurationMax = 0f;
			foreach (XElement xelement in _element.Elements())
			{
				string localName = xelement.Name.LocalName;
				attribute = xelement.GetAttribute("value");
				if (localName == "display_value")
				{
					buffClass.DisplayValueCVar = ((attribute.Length > 0) ? attribute : null);
				}
				else if (localName == "display_value_key")
				{
					buffClass.DisplayValueKey = ((attribute.Length > 0) ? attribute : null);
				}
				else if (localName == "display_value_format")
				{
					if (attribute.Length > 0)
					{
						Enum.TryParse<BuffClass.CVarDisplayFormat>(attribute, true, out buffClass.DisplayValueFormat);
					}
				}
				else if (localName == "damage_source")
				{
					if (attribute.Length > 0)
					{
						buffClass.DamageSource = EnumUtils.Parse<EnumDamageSource>(attribute, true);
					}
				}
				else if (localName == "damage_type")
				{
					if (attribute.Length > 0)
					{
						buffClass.DamageType = EnumUtils.Parse<EnumDamageTypes>(attribute, true);
					}
				}
				else if (localName == "stack_type")
				{
					if (attribute.Length > 0)
					{
						buffClass.StackType = EnumUtils.Parse<BuffEffectStackTypes>(attribute, true);
					}
				}
				else if (localName == "tags")
				{
					if (attribute.Length > 0)
					{
						buffClass.Tags = FastTags<TagGroup.Global>.Parse(attribute);
					}
				}
				else if (localName == "duration")
				{
					if (attribute.Length > 0)
					{
						buffClass.DurationMax = StringParsers.ParseFloat(attribute, 0, -1, NumberStyles.Any);
					}
				}
				else if (localName == "update_rate")
				{
					buffClass.UpdateRateTicks = ((attribute.Length > 0) ? ((int)(StringParsers.ParseFloat(attribute, 0, -1, NumberStyles.Any) * 20f)) : 20);
				}
				else if (localName == "remove_on_death")
				{
					buffClass.RemoveOnDeath = (attribute.Length == 0 || StringParsers.ParseBool(attribute, 0, -1, true));
				}
				else
				{
					buffClass.Requirements = RequirementBase.ParseRequirementGroup(_element);
				}
			}
			buffClass.Effects = MinEffectController.ParseXml(_element, null, MinEffectController.SourceParentType.BuffClass, buffClass.Name);
			BuffManager.AddBuff(buffClass);
			return;
		}
		throw new Exception("buff must have an name!");
	}
}
