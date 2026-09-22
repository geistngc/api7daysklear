using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UniLinq;

namespace DynamicMusic
{
	// Token: 0x02001A24 RID: 6692
	public abstract class AbstractConfiguration : IConfiguration
	{
		// Token: 0x170018C8 RID: 6344
		// (get) Token: 0x0600CB2E RID: 52014 RVA: 0x004A8B64 File Offset: 0x004A6D64
		// (set) Token: 0x0600CB2F RID: 52015 RVA: 0x004A8B6C File Offset: 0x004A6D6C
		public virtual IList<SectionType> Sections { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600CB30 RID: 52016
		public abstract int CountFor(LayerType _layer);

		// Token: 0x0600CB31 RID: 52017 RVA: 0x004A8B75 File Offset: 0x004A6D75
		public AbstractConfiguration()
		{
			this.Sections = new List<SectionType>();
			AbstractConfiguration.AllConfigurations.Add(this);
		}

		// Token: 0x0600CB32 RID: 52018 RVA: 0x004A8B93 File Offset: 0x004A6D93
		public static AbstractConfiguration CreateWrapper(string _type)
		{
			return (AbstractConfiguration)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("DynamicMusic.", _type));
		}

		// Token: 0x0600CB33 RID: 52019 RVA: 0x004A8BAC File Offset: 0x004A6DAC
		public virtual void ParseFromXml(XElement _xmlNode)
		{
			foreach (string name in _xmlNode.GetAttribute("sections").Split(',', StringSplitOptions.None))
			{
				this.Sections.Add(EnumUtils.Parse<SectionType>(name, false));
			}
		}

		// Token: 0x0600CB34 RID: 52020 RVA: 0x004A8BF8 File Offset: 0x004A6DF8
		public static T Get<T>(SectionType _sectionType) where T : IConfiguration
		{
			List<T> list = AbstractConfiguration.AllConfigurations.OfType<T>().ToList<T>().FindAll((T c) => c.Sections.Contains(_sectionType));
			if (list.Count <= 0)
			{
				return default(T);
			}
			return list[AbstractConfiguration.rng.RandomRange(list.Count)];
		}

		// Token: 0x0600CB35 RID: 52021 RVA: 0x004A8C5C File Offset: 0x004A6E5C
		public static int GetBufferSize(SectionType _sectionType, LayerType _layerType)
		{
			IEnumerable<int> enumerable = from c in AbstractConfiguration.AllConfigurations.OfType<IConfiguration>()
			where c.Sections.Contains(_sectionType)
			select c.CountFor(_layerType);
			if (enumerable != null && enumerable.Count<int>() != 0)
			{
				return enumerable.Max();
			}
			return 0;
		}

		// Token: 0x04009AE1 RID: 39649
		public static IList<AbstractConfiguration> AllConfigurations = new List<AbstractConfiguration>();

		// Token: 0x04009AE2 RID: 39650
		[PublicizedFrom(EAccessModifier.Protected)]
		public static GameRandom rng = GameRandomManager.Instance.CreateGameRandom();
	}
}
