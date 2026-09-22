using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200193D RID: 6461
	[Preserve]
	public class RequirementIsBlock : BaseRequirement
	{
		// Token: 0x0600C73F RID: 51007 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C740 RID: 51008 RVA: 0x004945CF File Offset: 0x004927CF
		public override bool CanPerform(Entity target)
		{
			return this.CheckBlock();
		}

		// Token: 0x0600C741 RID: 51009 RVA: 0x004945DC File Offset: 0x004927DC
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool CheckBlock()
		{
			if (this.Owner.TargetPosition == Vector3.zero)
			{
				return false;
			}
			WorldBase world = GameManager.Instance.World;
			Vector3i pos = new Vector3i(Utils.Fastfloor(this.Owner.TargetPosition.x), Utils.Fastfloor(this.Owner.TargetPosition.y), Utils.Fastfloor(this.Owner.TargetPosition.z));
			if (world.GetBlock(pos).Block.GetBlockName().EqualsCaseInsensitive(this.BlockName))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C742 RID: 51010 RVA: 0x00494682 File Offset: 0x00492882
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(RequirementIsBlock.PropBlockName))
			{
				this.BlockName = properties.Values[RequirementIsBlock.PropBlockName];
			}
		}

		// Token: 0x0600C743 RID: 51011 RVA: 0x004946B3 File Offset: 0x004928B3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementIsBlock
			{
				BlockName = this.BlockName,
				Invert = this.Invert
			};
		}

		// Token: 0x04009652 RID: 38482
		[PublicizedFrom(EAccessModifier.Protected)]
		public string BlockName = "";

		// Token: 0x04009653 RID: 38483
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBlockName = "block_name";
	}
}
