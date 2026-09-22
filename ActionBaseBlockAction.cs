using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019DE RID: 6622
	[Preserve]
	public class ActionBaseBlockAction : BaseAction
	{
		// Token: 0x0600CA2A RID: 51754 RVA: 0x00010E62 File Offset: 0x0000F062
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool NeedsDamage()
		{
			return false;
		}

		// Token: 0x0600CA2B RID: 51755 RVA: 0x00010E62 File Offset: 0x0000F062
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool AllowInTrader()
		{
			return false;
		}

		// Token: 0x0600CA2C RID: 51756 RVA: 0x004A4968 File Offset: 0x004A2B68
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			List<BlockChangeInfo> list = new List<BlockChangeInfo>();
			this.startPoint = ((base.Owner.TargetPosition.y != 0f) ? base.Owner.TargetPosition : base.Owner.Target.position);
			World world = GameManager.Instance.World;
			this.random = world.GetGameRandom();
			FastTags<TagGroup.Global> other = (this.blockTags != null) ? FastTags<TagGroup.Global>.Parse(this.blockTags) : FastTags<TagGroup.Global>.none;
			FastTags<TagGroup.Global> other2 = (this.excludeTags != null) ? FastTags<TagGroup.Global>.Parse(this.excludeTags) : FastTags<TagGroup.Global>.none;
			IChunk chunk = null;
			bool flag = this.AllowInTrader();
			if (base.Owner.Target != null && !base.Owner.Target.onGround)
			{
				return BaseAction.ActionCompleteStates.InComplete;
			}
			for (int i = this.minOffset.y; i <= this.maxOffset.y; i++)
			{
				for (int j = this.minOffset.z; j <= this.maxOffset.z; j += this.spacing + 1)
				{
					int num = (int)Utils.FastAbs((float)j);
					for (int k = this.minOffset.x; k <= this.maxOffset.x; k += this.spacing + 1)
					{
						if ((this.innerOffset == -1 || Utils.FastAbs((float)k) > (float)this.innerOffset || num > this.innerOffset) && (this.randomChance <= 0f || this.random.RandomFloat <= this.randomChance))
						{
							Vector3i vector3i = new Vector3i(Utils.Fastfloor(this.startPoint.x + (float)k), Utils.Fastfloor(this.startPoint.y + (float)i), Utils.Fastfloor(this.startPoint.z + (float)j));
							if (vector3i.y >= 0 && world.GetChunkFromWorldPos(vector3i, ref chunk) && (flag || world.GetTraderAreaAt(vector3i) == null) && (!this.checkSafe || (!this.safeAllowed && world.CanPlaceBlockAt(vector3i, null, false))))
							{
								int x = World.toBlockXZ(vector3i.x);
								int z = World.toBlockXZ(vector3i.z);
								BlockValue blockValue = this.NeedsDamage() ? chunk.GetBlock(x, vector3i.y, z) : chunk.GetBlockNoDamage(x, vector3i.y, z);
								if (!blockValue.ischild && (this.allowTerrain || !blockValue.Block.shape.IsTerrain()) && (this.blockTags == null || blockValue.Block.Tags.Test_AnySet(other)) && (this.excludeTags == null || !blockValue.Block.Tags.Test_AnySet(other2)) && this.CheckValid(world, vector3i))
								{
									BlockChangeInfo blockChangeInfo = this.UpdateBlock(world, vector3i, blockValue);
									if (blockChangeInfo != null)
									{
										list.Add(blockChangeInfo);
									}
								}
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				if (this.maxCount != -1 && this.maxCount < list.Count)
				{
					int num2 = list.Count - this.maxCount;
					for (int l = 0; l < num2; l++)
					{
						list.RemoveAt(this.random.RandomRange(list.Count));
					}
				}
				this.ChangesComplete();
				this.ProcessChanges(world, list);
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InCompleteRefund;
		}

		// Token: 0x0600CA2D RID: 51757 RVA: 0x004A4CDD File Offset: 0x004A2EDD
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void ProcessChanges(World world, List<BlockChangeInfo> blockChanges)
		{
			world.SetBlocksRPC(blockChanges);
		}

		// Token: 0x0600CA2E RID: 51758 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void ChangesComplete()
		{
		}

		// Token: 0x0600CA2F RID: 51759 RVA: 0x0002003D File Offset: 0x0001E23D
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool CheckValid(World world, Vector3i currentPos)
		{
			return true;
		}

		// Token: 0x0600CA30 RID: 51760 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			return null;
		}

		// Token: 0x0600CA31 RID: 51761 RVA: 0x004A4CE6 File Offset: 0x004A2EE6
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator UpdateBlocks(List<BlockChangeInfo> blockChanges)
		{
			yield return new WaitForSeconds(0.5f);
			GameManager.Instance.World.SetBlocksRPC(blockChanges);
			yield break;
		}

		// Token: 0x0600CA32 RID: 51762 RVA: 0x004A4CF8 File Offset: 0x004A2EF8
		public override BaseAction Clone()
		{
			ActionBaseBlockAction actionBaseBlockAction = base.Clone() as ActionBaseBlockAction;
			actionBaseBlockAction.minOffset = this.minOffset;
			actionBaseBlockAction.maxOffset = this.maxOffset;
			actionBaseBlockAction.spacing = this.spacing;
			actionBaseBlockAction.randomChance = this.randomChance;
			actionBaseBlockAction.safeAllowed = this.safeAllowed;
			actionBaseBlockAction.checkSafe = this.checkSafe;
			actionBaseBlockAction.blockTags = this.blockTags;
			actionBaseBlockAction.excludeTags = this.excludeTags;
			actionBaseBlockAction.innerOffset = this.innerOffset;
			actionBaseBlockAction.allowTerrain = this.allowTerrain;
			actionBaseBlockAction.maxCount = this.maxCount;
			return actionBaseBlockAction;
		}

		// Token: 0x0600CA33 RID: 51763 RVA: 0x004A4D94 File Offset: 0x004A2F94
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseVec(ActionBaseBlockAction.PropMinOffset, ref this.minOffset);
			properties.ParseVec(ActionBaseBlockAction.PropMaxOffset, ref this.maxOffset);
			properties.ParseInt(ActionBaseBlockAction.PropSpacing, ref this.spacing);
			properties.ParseInt(ActionBaseBlockAction.PropInnerOffset, ref this.innerOffset);
			properties.ParseFloat(ActionBaseBlockAction.PropRandomChance, ref this.randomChance);
			if (properties.Contains(ActionBaseBlockAction.PropSafeAllowed))
			{
				properties.ParseBool(ActionBaseBlockAction.PropSafeAllowed, ref this.safeAllowed);
				this.checkSafe = true;
			}
			properties.ParseString(ActionBaseBlockAction.PropBlockTags, ref this.blockTags);
			properties.ParseString(ActionBaseBlockAction.PropExcludeTags, ref this.excludeTags);
			properties.ParseBool(ActionBaseBlockAction.PropAllowTerrain, ref this.allowTerrain);
			properties.ParseInt(ActionBaseBlockAction.PropMaxCount, ref this.maxCount);
		}

		// Token: 0x040099B5 RID: 39349
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3i minOffset = Vector3i.zero;

		// Token: 0x040099B6 RID: 39350
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3i maxOffset = Vector3i.zero;

		// Token: 0x040099B7 RID: 39351
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockTags;

		// Token: 0x040099B8 RID: 39352
		[PublicizedFrom(EAccessModifier.Protected)]
		public string excludeTags;

		// Token: 0x040099B9 RID: 39353
		[PublicizedFrom(EAccessModifier.Protected)]
		public int spacing;

		// Token: 0x040099BA RID: 39354
		[PublicizedFrom(EAccessModifier.Protected)]
		public int innerOffset = -1;

		// Token: 0x040099BB RID: 39355
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool safeAllowed;

		// Token: 0x040099BC RID: 39356
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool checkSafe;

		// Token: 0x040099BD RID: 39357
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool allowTerrain;

		// Token: 0x040099BE RID: 39358
		[PublicizedFrom(EAccessModifier.Protected)]
		public float randomChance = -1f;

		// Token: 0x040099BF RID: 39359
		[PublicizedFrom(EAccessModifier.Protected)]
		public int maxCount = -1;

		// Token: 0x040099C0 RID: 39360
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBlockTags = "block_tags";

		// Token: 0x040099C1 RID: 39361
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropExcludeTags = "exclude_tags";

		// Token: 0x040099C2 RID: 39362
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMinOffset = "min_offset";

		// Token: 0x040099C3 RID: 39363
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxOffset = "max_offset";

		// Token: 0x040099C4 RID: 39364
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSpacing = "spacing";

		// Token: 0x040099C5 RID: 39365
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRandomChance = "random_chance";

		// Token: 0x040099C6 RID: 39366
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSafeAllowed = "safe_allowed";

		// Token: 0x040099C7 RID: 39367
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropInnerOffset = "inner_offset";

		// Token: 0x040099C8 RID: 39368
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAllowTerrain = "allow_terrain";

		// Token: 0x040099C9 RID: 39369
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxCount = "max_count";

		// Token: 0x040099CA RID: 39370
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameRandom random;

		// Token: 0x040099CB RID: 39371
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3 startPoint = Vector3.zero;
	}
}
