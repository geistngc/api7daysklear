using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace XMLEditing
{
	// Token: 0x02001667 RID: 5735
	public class BlockNode
	{
		// Token: 0x17001593 RID: 5523
		// (get) Token: 0x0600B413 RID: 46099 RVA: 0x0043728E File Offset: 0x0043548E
		// (set) Token: 0x0600B414 RID: 46100 RVA: 0x00437296 File Offset: 0x00435496
		public string Name { get; set; }

		// Token: 0x17001594 RID: 5524
		// (get) Token: 0x0600B415 RID: 46101 RVA: 0x0043729F File Offset: 0x0043549F
		// (set) Token: 0x0600B416 RID: 46102 RVA: 0x004372A7 File Offset: 0x004354A7
		public XElement Element { get; set; }

		// Token: 0x17001595 RID: 5525
		// (get) Token: 0x0600B417 RID: 46103 RVA: 0x004372B0 File Offset: 0x004354B0
		// (set) Token: 0x0600B418 RID: 46104 RVA: 0x004372B8 File Offset: 0x004354B8
		public BlockNode Parent { get; set; }

		// Token: 0x17001596 RID: 5526
		// (get) Token: 0x0600B419 RID: 46105 RVA: 0x004372C1 File Offset: 0x004354C1
		public List<BlockNode> Children { get; } = new List<BlockNode>();

		// Token: 0x17001597 RID: 5527
		// (get) Token: 0x0600B41A RID: 46106 RVA: 0x004372C9 File Offset: 0x004354C9
		// (set) Token: 0x0600B41B RID: 46107 RVA: 0x004372D1 File Offset: 0x004354D1
		public Dictionary<string, BlockNode.ElementInfo> ElementInfos { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Dictionary<string, BlockNode.ElementInfo>();

		// Token: 0x0600B41C RID: 46108 RVA: 0x004372DA File Offset: 0x004354DA
		public void AddChild(BlockNode child)
		{
			child.Parent = this;
			this.Children.Add(child);
		}

		// Token: 0x0600B41D RID: 46109 RVA: 0x004372F0 File Offset: 0x004354F0
		public bool TryGetPropertyParent(string targetPropertyName, out BlockNode propertyParentBlockNode, out BlockNode.ElementInfo propertyElementInfo, out int depth)
		{
			depth = 0;
			BlockNode blockNode = this;
			while (blockNode != null)
			{
				if (depth >= 100)
				{
					Debug.LogError("Max recursion depth exceeded!");
					break;
				}
				BlockNode.ElementInfo elementInfo;
				if (blockNode.ElementInfos.TryGetValue(targetPropertyName, out elementInfo))
				{
					if (elementInfo.Element != null)
					{
						propertyParentBlockNode = blockNode;
						propertyElementInfo = elementInfo;
						return true;
					}
					if (!elementInfo.CanInherit)
					{
						propertyParentBlockNode = null;
						propertyElementInfo = null;
						return false;
					}
				}
				blockNode = blockNode.Parent;
				depth++;
			}
			propertyParentBlockNode = null;
			propertyElementInfo = null;
			return false;
		}

		// Token: 0x0600B41E RID: 46110 RVA: 0x00437364 File Offset: 0x00435564
		public bool TryGetModelOffset(out Vector3 modelOffset, out int depth, out BlockNode.ModelOffsetType modelOffsetType)
		{
			BlockNode blockNode;
			BlockNode.ElementInfo elementInfo;
			if (this.TryGetPropertyParent("ModelOffset", out blockNode, out elementInfo, out depth))
			{
				modelOffsetType = BlockNode.ModelOffsetType.Explicit;
				modelOffset = StringParsers.ParseVector3(elementInfo.Element.GetAttribute(XNames.value), 0, -1);
				return true;
			}
			BlockNode.ElementInfo elementInfo2;
			if (!this.TryGetPropertyParent("Shape", out blockNode, out elementInfo2, out depth))
			{
				depth = 0;
				BlockNode blockNode2 = this;
				while (blockNode2.Parent != null)
				{
					blockNode2 = blockNode2.Parent;
					depth++;
				}
				modelOffsetType = BlockNode.ModelOffsetType.DefaultShapeNew;
				modelOffset = new Vector3(1f, 0f, 1f);
				return true;
			}
			string text = elementInfo2.Element.GetAttribute(XNames.value).Trim();
			Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("BlockShape", text);
			if (typeWithPrefix == null)
			{
				modelOffsetType = BlockNode.ModelOffsetType.None;
				Debug.LogError("Failed to create shape type \"BlockShape" + text + "\" for block: " + this.Name);
				modelOffset = Vector3.zero;
				return false;
			}
			if (typeof(BlockShapeNew).IsAssignableFrom(typeWithPrefix))
			{
				modelOffsetType = BlockNode.ModelOffsetType.ShapeNew;
				modelOffset = new Vector3(1f, 0f, 1f);
				return true;
			}
			if (typeof(BlockShapeModelEntity).IsAssignableFrom(typeWithPrefix))
			{
				modelOffsetType = BlockNode.ModelOffsetType.ShapeModelEntity;
				modelOffset = new Vector3(0f, 0.5f, 0f);
				return true;
			}
			modelOffsetType = BlockNode.ModelOffsetType.None;
			modelOffset = Vector3.zero;
			return false;
		}

		// Token: 0x0600B41F RID: 46111 RVA: 0x004374C4 File Offset: 0x004356C4
		public bool ShapeSupportsModelOffset()
		{
			BlockNode blockNode;
			BlockNode.ElementInfo elementInfo;
			int num;
			if (this.TryGetPropertyParent("Shape", out blockNode, out elementInfo, out num))
			{
				string text = elementInfo.Element.GetAttribute(XNames.value).Trim();
				Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("BlockShape", text);
				if (typeWithPrefix == null)
				{
					Debug.LogError("Failed to create shape type \"BlockShape" + text + "\" for block: " + this.Name);
					return false;
				}
				if (!typeof(BlockShapeNew).IsAssignableFrom(typeWithPrefix) && !typeof(BlockShapeModelEntity).IsAssignableFrom(typeWithPrefix))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x02001668 RID: 5736
		public enum ModelOffsetType
		{
			// Token: 0x04008757 RID: 34647
			None,
			// Token: 0x04008758 RID: 34648
			Explicit,
			// Token: 0x04008759 RID: 34649
			ShapeNew,
			// Token: 0x0400875A RID: 34650
			ShapeModelEntity,
			// Token: 0x0400875B RID: 34651
			ShapeExt3dModel,
			// Token: 0x0400875C RID: 34652
			ShapeOther,
			// Token: 0x0400875D RID: 34653
			DefaultShapeNew
		}

		// Token: 0x02001669 RID: 5737
		public class ElementInfo
		{
			// Token: 0x17001598 RID: 5528
			// (get) Token: 0x0600B421 RID: 46113 RVA: 0x00437575 File Offset: 0x00435775
			// (set) Token: 0x0600B422 RID: 46114 RVA: 0x0043757D File Offset: 0x0043577D
			public bool CanInherit { get; set; } = true;

			// Token: 0x17001599 RID: 5529
			// (get) Token: 0x0600B423 RID: 46115 RVA: 0x00437586 File Offset: 0x00435786
			// (set) Token: 0x0600B424 RID: 46116 RVA: 0x0043758E File Offset: 0x0043578E
			public bool IsClass { get; set; } = true;

			// Token: 0x1700159A RID: 5530
			// (get) Token: 0x0600B425 RID: 46117 RVA: 0x00437597 File Offset: 0x00435797
			// (set) Token: 0x0600B426 RID: 46118 RVA: 0x0043759F File Offset: 0x0043579F
			public XElement Element { get; set; }
		}
	}
}
