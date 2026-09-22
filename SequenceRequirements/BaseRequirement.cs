using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001927 RID: 6439
	[Preserve]
	public class BaseRequirement
	{
		// Token: 0x0600C6CB RID: 50891 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600C6CC RID: 50892 RVA: 0x00493892 File Offset: 0x00491A92
		public void Init()
		{
			this.OnInit();
		}

		// Token: 0x0600C6CD RID: 50893 RVA: 0x0002003D File Offset: 0x0001E23D
		public virtual bool CanPerform(Entity target)
		{
			return true;
		}

		// Token: 0x0600C6CE RID: 50894 RVA: 0x0049389C File Offset: 0x00491A9C
		public virtual void ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			this.Owner.HandleVariablesForProperties(properties);
			if (properties.Values.ContainsKey(BaseRequirement.PropInvert))
			{
				this.Invert = StringParsers.ParseBool(properties.Values[BaseRequirement.PropInvert], 0, -1, true);
			}
		}

		// Token: 0x0600C6CF RID: 50895 RVA: 0x004938EC File Offset: 0x00491AEC
		public virtual BaseRequirement Clone()
		{
			BaseRequirement baseRequirement = this.CloneChildSettings();
			if (this.Properties != null)
			{
				baseRequirement.Properties = new DynamicProperties();
				baseRequirement.Properties.CopyFrom(this.Properties, null);
			}
			return baseRequirement;
		}

		// Token: 0x0600C6D0 RID: 50896 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual BaseRequirement CloneChildSettings()
		{
			return null;
		}

		// Token: 0x04009624 RID: 38436
		public DynamicProperties Properties;

		// Token: 0x04009625 RID: 38437
		public GameEventActionSequence Owner;

		// Token: 0x04009626 RID: 38438
		public bool Invert;

		// Token: 0x04009627 RID: 38439
		public static string PropInvert = "invert";
	}
}
