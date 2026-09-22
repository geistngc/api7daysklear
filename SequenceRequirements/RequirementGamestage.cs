using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200192B RID: 6443
	[Preserve]
	public class RequirementGamestage : BaseOperationRequirement
	{
		// Token: 0x0600C6E4 RID: 50916 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C6E5 RID: 50917 RVA: 0x00493B20 File Offset: 0x00491D20
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			return (entityPlayer != null) ? entityPlayer.gameStage : 0;
		}

		// Token: 0x0600C6E6 RID: 50918 RVA: 0x00493B45 File Offset: 0x00491D45
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetIntValue(target as EntityAlive, this.gamestageText, 0);
		}

		// Token: 0x0600C6E7 RID: 50919 RVA: 0x00493B5E File Offset: 0x00491D5E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementGamestage.PropGamestage, ref this.gamestageText);
		}

		// Token: 0x0600C6E8 RID: 50920 RVA: 0x00493B78 File Offset: 0x00491D78
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGamestage
			{
				Invert = this.Invert,
				operation = this.operation,
				gamestageText = this.gamestageText
			};
		}

		// Token: 0x0400962E RID: 38446
		[PublicizedFrom(EAccessModifier.Protected)]
		public string gamestageText;

		// Token: 0x0400962F RID: 38447
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGamestage = "game_stage";
	}
}
