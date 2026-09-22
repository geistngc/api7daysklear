using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace XMLEditing
{
	// Token: 0x0200166A RID: 5738
	public class BlockNodeMap : IEnumerable<KeyValuePair<string, BlockNode>>, IEnumerable
	{
		// Token: 0x0600B428 RID: 46120 RVA: 0x004375BE File Offset: 0x004357BE
		public IEnumerator<KeyValuePair<string, BlockNode>> GetEnumerator()
		{
			return this.blockNodes.GetEnumerator();
		}

		// Token: 0x0600B429 RID: 46121 RVA: 0x004375BE File Offset: 0x004357BE
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.blockNodes.GetEnumerator();
		}

		// Token: 0x0600B42A RID: 46122 RVA: 0x004375D0 File Offset: 0x004357D0
		public bool TryGetValue(string targetName, out BlockNode blockNode)
		{
			return this.blockNodes.TryGetValue(targetName, out blockNode);
		}

		// Token: 0x1700159B RID: 5531
		// (get) Token: 0x0600B42B RID: 46123 RVA: 0x004375DF File Offset: 0x004357DF
		public int Count
		{
			get
			{
				return this.blockNodes.Count;
			}
		}

		// Token: 0x0600B42C RID: 46124 RVA: 0x004375EC File Offset: 0x004357EC
		public void PopulateFromFile(string blocksFilePath)
		{
			XDocument xdocument = XMLUtils.LoadXDocument(blocksFilePath);
			this.root = xdocument.Root;
			this.Refresh();
		}

		// Token: 0x0600B42D RID: 46125 RVA: 0x00437612 File Offset: 0x00435812
		public void PopulateFromRoot(XElement root)
		{
			this.root = root;
			this.Refresh();
		}

		// Token: 0x0600B42E RID: 46126 RVA: 0x00437624 File Offset: 0x00435824
		public void Refresh()
		{
			if (this.root == null)
			{
				Debug.LogError("Refresh failed: root element is null. This may occur if you have not called one of the PopulateFrom[...] methods prior to calling Refresh. Otherwise there may be an error in the source xml.");
				return;
			}
			if (!this.root.HasElements)
			{
				Debug.LogError("Refresh failed: root element has no child elements.");
				return;
			}
			this.blockNodes.Clear();
			foreach (XElement element in this.root.Elements(XNames.block))
			{
				string attribute = element.GetAttribute(XNames.name);
				BlockNode value = new BlockNode
				{
					Name = attribute,
					Element = element
				};
				this.blockNodes[attribute] = value;
			}
			foreach (BlockNode blockNode in this.blockNodes.Values)
			{
				foreach (XElement element2 in blockNode.Element.Elements(XNames.property))
				{
					string text = element2.GetAttribute(XNames.name);
					if (text == "Extends")
					{
						string attribute2 = element2.GetAttribute(XNames.value);
						BlockNode blockNode2;
						if (this.blockNodes.TryGetValue(attribute2, out blockNode2))
						{
							blockNode2.AddChild(blockNode);
							foreach (string key in element2.GetAttribute(XNames.param1).Split(new char[]
							{
								','
							}, StringSplitOptions.RemoveEmptyEntries))
							{
								BlockNode.ElementInfo elementInfo;
								if (!blockNode.ElementInfos.TryGetValue(key, out elementInfo))
								{
									elementInfo = new BlockNode.ElementInfo();
									blockNode.ElementInfos[key] = elementInfo;
								}
								elementInfo.CanInherit = false;
							}
							BlockNode.ElementInfo elementInfo2 = new BlockNode.ElementInfo();
							elementInfo2.CanInherit = false;
							elementInfo2.Element = element2;
							elementInfo2.IsClass = false;
							blockNode.ElementInfos["Extends"] = elementInfo2;
						}
						else
						{
							Debug.LogError(string.Concat(new string[]
							{
								"Failed to find parent BlockNode \"",
								attribute2,
								"\" for block \"",
								blockNode.Name,
								"\""
							}));
						}
					}
					else
					{
						bool isClass = false;
						if (string.IsNullOrWhiteSpace(text))
						{
							string attribute3 = element2.GetAttribute(XNames.class_);
							if (string.IsNullOrWhiteSpace(attribute3))
							{
								continue;
							}
							isClass = true;
							text = attribute3;
						}
						BlockNode.ElementInfo elementInfo3;
						if (!blockNode.ElementInfos.TryGetValue(text, out elementInfo3))
						{
							elementInfo3 = new BlockNode.ElementInfo();
							elementInfo3.CanInherit = (text != "CreativeMode");
							blockNode.ElementInfos[text] = elementInfo3;
						}
						elementInfo3.Element = element2;
						elementInfo3.IsClass = isClass;
					}
				}
			}
		}

		// Token: 0x04008761 RID: 34657
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, BlockNode> blockNodes = new Dictionary<string, BlockNode>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04008762 RID: 34658
		[PublicizedFrom(EAccessModifier.Private)]
		public XElement root;
	}
}
