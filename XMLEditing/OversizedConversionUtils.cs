using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace XMLEditing
{
	// Token: 0x0200166B RID: 5739
	public static class OversizedConversionUtils
	{
		// Token: 0x1700159C RID: 5532
		// (get) Token: 0x0600B430 RID: 46128 RVA: 0x00437940 File Offset: 0x00435B40
		public static string OversizedConversionTargetsFilePath
		{
			get
			{
				return GameIO.GetGameDir("Data/Config") + "/OversizedConversionTargets.txt";
			}
		}

		// Token: 0x0600B431 RID: 46129 RVA: 0x00437958 File Offset: 0x00435B58
		public static void AutoApplyOversizedConversion(XElement root)
		{
			string oversizedConversionTargetsFilePath = OversizedConversionUtils.OversizedConversionTargetsFilePath;
			HashSet<string> hashSet = new HashSet<string>();
			if (!SdFile.Exists(oversizedConversionTargetsFilePath))
			{
				Debug.LogWarning("Oversized conversion not applied: File \"" + oversizedConversionTargetsFilePath + "\" does not exist.");
				return;
			}
			foreach (string item in SdFile.ReadLines(oversizedConversionTargetsFilePath))
			{
				hashSet.Add(item);
			}
			if (hashSet.Count == 0)
			{
				return;
			}
			OversizedConversionUtils.ConversionDebugInfo conversionDebugInfo = new OversizedConversionUtils.ConversionDebugInfo();
			OversizedConversionUtils.ApplyOversizedConversion(root, hashSet, conversionDebugInfo);
			Debug.Log("Automatic oversized conversion complete.\n\n" + string.Format("targetNames.Count: {0}\n", hashSet.Count) + conversionDebugInfo.CountsToString());
		}

		// Token: 0x0600B432 RID: 46130 RVA: 0x00437A14 File Offset: 0x00435C14
		public static void ApplyOversizedConversion(XElement root, HashSet<string> targetNames, OversizedConversionUtils.ConversionDebugInfo debugInfo = null)
		{
			BlockNodeMap blockNodeMap = new BlockNodeMap();
			blockNodeMap.PopulateFromRoot(root);
			Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>();
			XMLUtils.PopulateReplacementMap(dictionary);
			OversizedConversionUtils.FixUndersizedHelpers(blockNodeMap, dictionary);
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string key in targetNames)
			{
				HashSet<string> other;
				if (dictionary.TryGetValue(key, out other))
				{
					hashSet.UnionWith(other);
				}
			}
			HashSet<string> hashSet2 = new HashSet<string>(targetNames);
			hashSet2.UnionWith(hashSet);
			HashSet<BlockNode> targetRootNodes = new HashSet<BlockNode>();
			OversizedConversionUtils.FindMultiBlockDimRootNodes(blockNodeMap, hashSet2, delegate(BlockNode node)
			{
				targetRootNodes.Add(node);
			});
			OversizedConversionUtils.ConvertBlocksToOversized(targetRootNodes, debugInfo);
		}

		// Token: 0x0600B433 RID: 46131 RVA: 0x00437AD8 File Offset: 0x00435CD8
		[PublicizedFrom(EAccessModifier.Private)]
		public static void FixUndersizedHelpers(BlockNodeMap blockNodes, Dictionary<string, HashSet<string>> replacementMap)
		{
			Bounds bounds = new Bounds(GameUtils.GetMultiBlockBoundsOffset(Vector3i.one), Vector3i.one);
			foreach (KeyValuePair<string, HashSet<string>> keyValuePair in replacementMap)
			{
				string key = keyValuePair.Key;
				HashSet<string> value = keyValuePair.Value;
				BlockNode blockNode;
				if (!blockNodes.TryGetValue(key, out blockNode))
				{
					Debug.LogWarning("No blockNode found for block: \"" + key + "\"");
				}
				else
				{
					BlockNode blockNode2;
					BlockNode.ElementInfo elementInfo;
					int num;
					Bounds bounds2;
					BlockNode.ElementInfo elementInfo2;
					if (blockNode.TryGetPropertyParent("MultiBlockDim", out blockNode2, out elementInfo, out num) && elementInfo.Element != null)
					{
						Vector3i v3i = StringParsers.ParseVector3i(elementInfo.Element.GetAttribute(XNames.value), 0, -1, false);
						bounds2 = new Bounds(GameUtils.GetMultiBlockBoundsOffset(v3i), v3i);
					}
					else if (blockNode.TryGetPropertyParent("OversizedBounds", out blockNode2, out elementInfo2, out num) && elementInfo2.Element != null)
					{
						bounds2 = StringParsers.ParseBounds(elementInfo2.Element.GetAttribute(XNames.value));
					}
					else
					{
						bounds2 = bounds;
					}
					Bounds bounds3 = bounds;
					foreach (string targetName in value)
					{
						BlockNode blockNode3;
						BlockNode.ElementInfo elementInfo3;
						if (!blockNodes.TryGetValue(targetName, out blockNode3))
						{
							Debug.LogWarning("No blockNode found for block: \"" + key + "\"");
						}
						else if (blockNode3.TryGetPropertyParent("MultiBlockDim", out blockNode2, out elementInfo3, out num) && elementInfo3.Element != null)
						{
							Vector3i v3i2 = StringParsers.ParseVector3i(elementInfo3.Element.GetAttribute(XNames.value), 0, -1, false);
							Bounds bounds4 = new Bounds(GameUtils.GetMultiBlockBoundsOffset(v3i2), v3i2);
							bounds3.Encapsulate(bounds4);
						}
					}
					if (!bounds2.Contains(bounds3.min) || !bounds2.Contains(bounds3.max))
					{
						Vector3 vector = bounds3.center - bounds2.center;
						BlockNode.ElementInfo elementInfo4;
						if ((vector.x != 0f || vector.z != 0f) && blockNode.ElementInfos.TryGetValue("ModelOffset", out elementInfo4) && elementInfo4.Element != null)
						{
							Vector3 vector2 = StringParsers.ParseVector3(elementInfo4.Element.GetAttribute(XNames.value), 0, -1);
							vector2.x -= vector.x;
							vector2.z -= vector.z;
							elementInfo4.Element.SetAttributeValue(XNames.value, string.Format("{0},{1},{2}", vector2.x, vector2.y, vector2.z));
						}
						BlockNode.ElementInfo elementInfo5;
						bool flag = blockNode.ElementInfos.TryGetValue("MultiBlockDim", out elementInfo5);
						BlockNode.ElementInfo elementInfo6;
						if (flag && !elementInfo5.CanInherit && elementInfo5.Element == null && blockNode.Parent.TryGetPropertyParent("MultiBlockDim", out blockNode2, out elementInfo6, out num) && elementInfo6.Element != null)
						{
							Vector3i v3i3 = StringParsers.ParseVector3i(elementInfo6.Element.GetAttribute(XNames.value), 0, -1, false);
							BlockNode.ElementInfo elementInfo7;
							if (bounds3.size.Equals(v3i3) && blockNode.ElementInfos.TryGetValue("Extends", out elementInfo7) && elementInfo7.Element != null)
							{
								string text = elementInfo7.Element.GetAttribute(XNames.param1);
								List<string> list = text.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList<string>();
								if (text.ContainsCaseInsensitive("MultiBlockDim"))
								{
									list.RemoveAll((string val) => string.Equals(val, "MultiBlockDim", StringComparison.OrdinalIgnoreCase));
									text = string.Join(",", list);
									if (string.IsNullOrWhiteSpace(text))
									{
										elementInfo7.Element.Attribute(XNames.param1).Remove();
									}
									else
									{
										elementInfo7.Element.SetAttributeValue(XNames.param1, text);
									}
									elementInfo5.CanInherit = true;
									continue;
								}
							}
						}
						XElement element = XMLUtils.SetProperty(blockNode.Element, "MultiBlockDim", XNames.value, string.Format("{0},{1},{2}", bounds3.size.x, bounds3.size.y, bounds3.size.z));
						if (flag)
						{
							elementInfo5.Element = element;
						}
						else
						{
							elementInfo5 = new BlockNode.ElementInfo
							{
								CanInherit = true,
								Element = element,
								IsClass = false
							};
							blockNode.ElementInfos["MultiBlockDim"] = elementInfo5;
						}
					}
				}
			}
		}

		// Token: 0x0600B434 RID: 46132 RVA: 0x00437FCC File Offset: 0x004361CC
		public static void ConvertBlocksToOversized(HashSet<BlockNode> targetRootNodes, OversizedConversionUtils.ConversionDebugInfo debugInfo = null)
		{
			foreach (BlockNode blockNode in targetRootNodes)
			{
				BlockNode.ElementInfo elementInfo;
				if (!blockNode.ElementInfos.TryGetValue("MultiBlockDim", out elementInfo))
				{
					Debug.LogError("targetRootNode does not contain ElementInfo for MultiBlockDim.");
				}
				else
				{
					Vector3i vector3i = StringParsers.ParseVector3i(elementInfo.Element.GetAttribute(XNames.value), 0, -1, false);
					if (vector3i.x <= 1 && vector3i.z <= 1)
					{
						Debug.LogError("Block \"" + blockNode.Name + "\" appears in targetRootNodes despite having XZ dimension of 1.");
					}
					else if (!blockNode.ShapeSupportsModelOffset())
					{
						Debug.LogError("Block \"" + blockNode.Name + "\" has MultiBlockDim but is not a BlockShape type which supports ModelOffset. Conversion of such blocks has not been implemented; please contact engineering if this is required.");
					}
					else
					{
						Vector3 vector;
						int num;
						BlockNode.ModelOffsetType modelOffsetType;
						bool flag = blockNode.TryGetModelOffset(out vector, out num, out modelOffsetType);
						if (debugInfo != null)
						{
							debugInfo.OnOriginModelOffsetTypeCalculated(modelOffsetType);
						}
						if (!flag)
						{
							Debug.LogError("Block \"" + blockNode.Name + "\" has MultiBlockDim but is not a BlockShape type which supports ModelOffset.");
						}
						else
						{
							OversizedConversionUtils.BakeExplicitModelOffsetRecursive(blockNode, debugInfo);
							OversizedConversionUtils.ConvertToOversizedRecursive(blockNode, Vector3.zero, debugInfo);
							if (debugInfo != null)
							{
								debugInfo.OnBaseBlockConverted(blockNode);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B435 RID: 46133 RVA: 0x004380FC File Offset: 0x004362FC
		[PublicizedFrom(EAccessModifier.Private)]
		public static void ConvertToOversizedRecursive(BlockNode targetNode, Vector3 inheritedAutoOffset, OversizedConversionUtils.ConversionDebugInfo debugInfo = null)
		{
			Vector3 vector = Vector3.zero;
			bool flag = false;
			bool flag2 = true;
			bool flag3 = false;
			BlockNode.ElementInfo elementInfo;
			if (targetNode.ElementInfos.TryGetValue("MultiBlockDim", out elementInfo))
			{
				if (!elementInfo.CanInherit)
				{
					flag2 = false;
				}
				if (elementInfo.Element != null)
				{
					Vector3i vector3i = StringParsers.ParseVector3i(elementInfo.Element.GetAttribute(XNames.value), 0, -1, false);
					if (vector3i.x <= 1 && vector3i.z <= 1)
					{
						inheritedAutoOffset = Vector3.zero;
						Debug.LogWarning("Oversized conversion child block \"" + targetNode.Name + "\" specifies single-block MultiBlockDim override.");
						flag2 = false;
					}
					else
					{
						inheritedAutoOffset = (vector = GameUtils.GetMultiBlockBoundsOffset(vector3i));
						XMLUtils.SetProperty(targetNode.Element, "OversizedBounds", XNames.value, string.Format("({0},{1},{2}),({3},{4},{5})", new object[]
						{
							inheritedAutoOffset.x,
							inheritedAutoOffset.y,
							inheritedAutoOffset.z,
							vector3i.x,
							vector3i.y,
							vector3i.z
						}));
						if (elementInfo.Element.Parent != null)
						{
							elementInfo.Element.Remove();
							elementInfo.Element = null;
						}
						flag3 = true;
						flag = true;
					}
				}
			}
			BlockNode.ElementInfo elementInfo2;
			if (targetNode.ElementInfos.TryGetValue("ModelOffset", out elementInfo2))
			{
				inheritedAutoOffset = (elementInfo2.CanInherit ? inheritedAutoOffset : vector);
				if (elementInfo2.Element != null)
				{
					Vector3 vector2 = StringParsers.ParseVector3(elementInfo2.Element.GetAttribute(XNames.value), 0, -1);
					vector2.x += inheritedAutoOffset.x;
					vector2.z += inheritedAutoOffset.z;
					Vector3 lhs;
					int num;
					BlockNode.ModelOffsetType modelOffsetType;
					if (elementInfo2.CanInherit && targetNode.Parent != null && targetNode.Parent.TryGetModelOffset(out lhs, out num, out modelOffsetType) && modelOffsetType == BlockNode.ModelOffsetType.Explicit && lhs == vector2)
					{
						elementInfo2.Element.Remove();
						elementInfo2.Element = null;
					}
					else
					{
						XMLUtils.SetProperty(targetNode.Element, "ModelOffset", XNames.value, string.Format("{0},{1},{2}", vector2.x, vector2.y, vector2.z));
					}
					flag = true;
				}
			}
			BlockNode.ElementInfo elementInfo3;
			if (targetNode.ElementInfos.TryGetValue("Extends", out elementInfo3) && elementInfo3.Element != null)
			{
				string text = elementInfo3.Element.GetAttribute(XNames.param1);
				List<string> list = text.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				bool flag4 = false;
				if (targetNode.Parent != null)
				{
					int num;
					BlockNode blockNode;
					BlockNode.ElementInfo elementInfo4;
					bool flag5 = targetNode.Parent.TryGetPropertyParent("MultiBlockDim", out blockNode, out elementInfo4, out num) && elementInfo4.Element != null;
					bool flag6 = text.ContainsCaseInsensitive("MultiBlockDim");
					if (flag6 && !flag5)
					{
						list.RemoveAll((string val) => string.Equals(val, "MultiBlockDim", StringComparison.OrdinalIgnoreCase));
						flag4 = true;
					}
					else if (!flag6 && flag5 && flag3)
					{
						list.Add("MultiBlockDim");
						flag4 = true;
					}
				}
				if (!flag2 && !text.ContainsCaseInsensitive("OversizedBounds"))
				{
					list.Add("OversizedBounds");
					flag4 = true;
				}
				if (flag4)
				{
					text = string.Join(",", list);
					XMLUtils.SetProperty(targetNode.Element, "Extends", XNames.param1, text);
					flag = true;
				}
			}
			else if (!flag2)
			{
				Debug.LogError("Failed to find extends element on block \"" + targetNode.Name + "\".");
			}
			if (flag && debugInfo != null)
			{
				debugInfo.OnBlockModified(targetNode);
			}
			foreach (BlockNode targetNode2 in targetNode.Children)
			{
				OversizedConversionUtils.ConvertToOversizedRecursive(targetNode2, inheritedAutoOffset, debugInfo);
			}
		}

		// Token: 0x0600B436 RID: 46134 RVA: 0x004384E4 File Offset: 0x004366E4
		[PublicizedFrom(EAccessModifier.Private)]
		public static void BakeExplicitModelOffsetRecursive(BlockNode blockNode, OversizedConversionUtils.ConversionDebugInfo debugInfo = null)
		{
			foreach (BlockNode blockNode2 in blockNode.Children)
			{
				OversizedConversionUtils.BakeExplicitModelOffsetRecursive(blockNode2, debugInfo);
			}
			Vector3 vector;
			int num;
			BlockNode.ModelOffsetType modelOffsetType;
			if (!blockNode.TryGetModelOffset(out vector, out num, out modelOffsetType))
			{
				Debug.LogError("BlockNode " + blockNode.Name + " does not have a valid implict or explicit model offset.");
				return;
			}
			XElement element = XMLUtils.SetProperty(blockNode.Element, "ModelOffset", XNames.value, string.Format("{0},{1},{2}", vector.x, vector.y, vector.z));
			BlockNode.ElementInfo elementInfo;
			if (blockNode.ElementInfos.TryGetValue("ModelOffset", out elementInfo))
			{
				elementInfo.Element = element;
			}
			else
			{
				elementInfo = new BlockNode.ElementInfo
				{
					CanInherit = true,
					Element = element,
					IsClass = false
				};
				blockNode.ElementInfos["ModelOffset"] = elementInfo;
			}
			if (debugInfo != null)
			{
				debugInfo.OnBlockModified(blockNode);
			}
		}

		// Token: 0x0600B437 RID: 46135 RVA: 0x004385F8 File Offset: 0x004367F8
		public static void FindMultiBlockDimRootNodes(BlockNodeMap blockNodes, HashSet<string> targetNames, Action<BlockNode> onRootNodeFound)
		{
			foreach (string text in targetNames)
			{
				BlockNode blockNode;
				if (!blockNodes.TryGetValue(text, out blockNode))
				{
					Debug.LogWarning("No blockNode found for block: \"" + text + "\"");
				}
				else
				{
					BlockNode blockNode2 = null;
					for (BlockNode blockNode3 = blockNode; blockNode3 != null; blockNode3 = blockNode3.Parent)
					{
						BlockNode.ElementInfo elementInfo;
						if (blockNode3.ElementInfos.TryGetValue("MultiBlockDim", out elementInfo) && elementInfo.Element != null)
						{
							Vector3i vector3i = StringParsers.ParseVector3i(elementInfo.Element.GetAttribute(XNames.value), 0, -1, false);
							if (vector3i.x > 1 || vector3i.z > 1)
							{
								blockNode2 = blockNode3;
							}
						}
					}
					if (blockNode2 != null && onRootNodeFound != null)
					{
						onRootNodeFound(blockNode2);
					}
				}
			}
		}

		// Token: 0x0200166C RID: 5740
		public class ConversionDebugInfo
		{
			// Token: 0x1700159D RID: 5533
			// (get) Token: 0x0600B438 RID: 46136 RVA: 0x004386D8 File Offset: 0x004368D8
			// (set) Token: 0x0600B439 RID: 46137 RVA: 0x004386E0 File Offset: 0x004368E0
			public OversizedConversionUtils.ConversionDebugInfo.OffsetCounts Offsets { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new OversizedConversionUtils.ConversionDebugInfo.OffsetCounts();

			// Token: 0x1700159E RID: 5534
			// (get) Token: 0x0600B43A RID: 46138 RVA: 0x004386E9 File Offset: 0x004368E9
			public int ModifiedBlockNodeCount
			{
				get
				{
					return this.modifiedBlockNodes.Count;
				}
			}

			// Token: 0x1700159F RID: 5535
			// (get) Token: 0x0600B43B RID: 46139 RVA: 0x004386F6 File Offset: 0x004368F6
			public int BaseConversionCount
			{
				get
				{
					return this.baseConversionCount;
				}
			}

			// Token: 0x0600B43C RID: 46140 RVA: 0x00438700 File Offset: 0x00436900
			public void Reset()
			{
				this.Offsets.Explicit = 0;
				this.Offsets.ShapeNew = 0;
				this.Offsets.ShapeModelEntity = 0;
				this.Offsets.ShapeExt3dModel = 0;
				this.Offsets.ShapeOther = 0;
				this.Offsets.Default = 0;
				this.modifiedBlockNodes.Clear();
				this.baseConversionCount = 0;
			}

			// Token: 0x0600B43D RID: 46141 RVA: 0x00438767 File Offset: 0x00436967
			public void Log()
			{
				Debug.Log(this.CountsToString());
			}

			// Token: 0x0600B43E RID: 46142 RVA: 0x00438774 File Offset: 0x00436974
			public string CountsToString()
			{
				return string.Concat(new string[]
				{
					string.Format("modifiedBlocksCount: {0}\n", this.ModifiedBlockNodeCount),
					string.Format("baseConversionCount: {0}\n", this.BaseConversionCount),
					string.Format("Offsets.Explicit: {0}\n", this.Offsets.Explicit),
					string.Format("Offsets.ShapeNew: {0}\n", this.Offsets.ShapeNew),
					string.Format("Offsets.ShapeModelEntity: {0}\n", this.Offsets.ShapeModelEntity),
					string.Format("Offsets.ShapeExt3dModel: {0}\n", this.Offsets.ShapeExt3dModel),
					string.Format("Offsets.ShapeOther: {0}\n", this.Offsets.ShapeOther),
					string.Format("Offsets.Default: {0}", this.Offsets.Default)
				});
			}

			// Token: 0x0600B43F RID: 46143 RVA: 0x0043886A File Offset: 0x00436A6A
			public void OnBlockModified(BlockNode blockNode)
			{
				this.modifiedBlockNodes.Add(blockNode);
			}

			// Token: 0x0600B440 RID: 46144 RVA: 0x00438879 File Offset: 0x00436A79
			public void OnBaseBlockConverted(BlockNode node)
			{
				this.baseConversionCount++;
			}

			// Token: 0x0600B441 RID: 46145 RVA: 0x0043888C File Offset: 0x00436A8C
			public void OnOriginModelOffsetTypeCalculated(BlockNode.ModelOffsetType modelOffsetType)
			{
				int num;
				switch (modelOffsetType)
				{
				case BlockNode.ModelOffsetType.Explicit:
				{
					OversizedConversionUtils.ConversionDebugInfo.OffsetCounts offsets = this.Offsets;
					num = offsets.Explicit;
					offsets.Explicit = num + 1;
					return;
				}
				case BlockNode.ModelOffsetType.ShapeNew:
				{
					OversizedConversionUtils.ConversionDebugInfo.OffsetCounts offsets2 = this.Offsets;
					num = offsets2.ShapeNew;
					offsets2.ShapeNew = num + 1;
					return;
				}
				case BlockNode.ModelOffsetType.ShapeModelEntity:
				{
					OversizedConversionUtils.ConversionDebugInfo.OffsetCounts offsets3 = this.Offsets;
					num = offsets3.ShapeModelEntity;
					offsets3.ShapeModelEntity = num + 1;
					return;
				}
				case BlockNode.ModelOffsetType.ShapeExt3dModel:
				{
					OversizedConversionUtils.ConversionDebugInfo.OffsetCounts offsets4 = this.Offsets;
					num = offsets4.ShapeExt3dModel;
					offsets4.ShapeExt3dModel = num + 1;
					return;
				}
				case BlockNode.ModelOffsetType.ShapeOther:
				{
					OversizedConversionUtils.ConversionDebugInfo.OffsetCounts offsets5 = this.Offsets;
					num = offsets5.ShapeOther;
					offsets5.ShapeOther = num + 1;
					return;
				}
				}
				OversizedConversionUtils.ConversionDebugInfo.OffsetCounts offsets6 = this.Offsets;
				num = offsets6.Default;
				offsets6.Default = num + 1;
			}

			// Token: 0x04008764 RID: 34660
			[PublicizedFrom(EAccessModifier.Private)]
			public HashSet<BlockNode> modifiedBlockNodes = new HashSet<BlockNode>();

			// Token: 0x04008765 RID: 34661
			[PublicizedFrom(EAccessModifier.Private)]
			public int baseConversionCount;

			// Token: 0x0200166D RID: 5741
			public class OffsetCounts
			{
				// Token: 0x170015A0 RID: 5536
				// (get) Token: 0x0600B443 RID: 46147 RVA: 0x0043895C File Offset: 0x00436B5C
				// (set) Token: 0x0600B444 RID: 46148 RVA: 0x00438964 File Offset: 0x00436B64
				public int Explicit { get; set; }

				// Token: 0x170015A1 RID: 5537
				// (get) Token: 0x0600B445 RID: 46149 RVA: 0x0043896D File Offset: 0x00436B6D
				// (set) Token: 0x0600B446 RID: 46150 RVA: 0x00438975 File Offset: 0x00436B75
				public int ShapeNew { get; set; }

				// Token: 0x170015A2 RID: 5538
				// (get) Token: 0x0600B447 RID: 46151 RVA: 0x0043897E File Offset: 0x00436B7E
				// (set) Token: 0x0600B448 RID: 46152 RVA: 0x00438986 File Offset: 0x00436B86
				public int ShapeModelEntity { get; set; }

				// Token: 0x170015A3 RID: 5539
				// (get) Token: 0x0600B449 RID: 46153 RVA: 0x0043898F File Offset: 0x00436B8F
				// (set) Token: 0x0600B44A RID: 46154 RVA: 0x00438997 File Offset: 0x00436B97
				public int ShapeExt3dModel { get; set; }

				// Token: 0x170015A4 RID: 5540
				// (get) Token: 0x0600B44B RID: 46155 RVA: 0x004389A0 File Offset: 0x00436BA0
				// (set) Token: 0x0600B44C RID: 46156 RVA: 0x004389A8 File Offset: 0x00436BA8
				public int ShapeOther { get; set; }

				// Token: 0x170015A5 RID: 5541
				// (get) Token: 0x0600B44D RID: 46157 RVA: 0x004389B1 File Offset: 0x00436BB1
				// (set) Token: 0x0600B44E RID: 46158 RVA: 0x004389B9 File Offset: 0x00436BB9
				public int Default { get; set; }
			}
		}
	}
}
