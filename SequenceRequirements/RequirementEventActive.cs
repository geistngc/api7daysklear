using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001929 RID: 6441
	[Preserve]
	public class RequirementEventActive : BaseRequirement
	{
		// Token: 0x0600C6DA RID: 50906 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C6DB RID: 50907 RVA: 0x00493A10 File Offset: 0x00491C10
		public override bool CanPerform(Entity target)
		{
			if (!EventsFromXml.Events.ContainsKey(this.EventName))
			{
				return this.Invert;
			}
			if (EventsFromXml.Events[this.EventName].Active)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C6DC RID: 50908 RVA: 0x00493A60 File Offset: 0x00491C60
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementEventActive.PropEventName, ref this.EventName);
		}

		// Token: 0x0600C6DD RID: 50909 RVA: 0x00493A7A File Offset: 0x00491C7A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementEventActive
			{
				EventName = this.EventName,
				Invert = this.Invert
			};
		}

		// Token: 0x0400962C RID: 38444
		[PublicizedFrom(EAccessModifier.Protected)]
		public string EventName = "";

		// Token: 0x0400962D RID: 38445
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEventName = "event_name";
	}
}
