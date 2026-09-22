using System;
using UnityEngine;

namespace CoverClippingTool
{
	// Token: 0x02001679 RID: 5753
	public class CoverMaskBlockPlacer : MonoBehaviour
	{
		// Token: 0x0600B46D RID: 46189 RVA: 0x0043911E File Offset: 0x0043731E
		public void UpdateSelectedMultiBlock(string value)
		{
			this.editMultiBlock = !value.Equals("All", StringComparison.OrdinalIgnoreCase);
			this.selectedMultiBlock = (this.editMultiBlock ? StringParsers.ParseVector3i(value, 0, -1, false) : Vector3i.zero);
		}

		// Token: 0x04008787 RID: 34695
		public static readonly string PropCoverMask = "CoverMask";

		// Token: 0x04008788 RID: 34696
		public static readonly string PropCoverMaskMulti = "CoverMaskMulti";

		// Token: 0x04008789 RID: 34697
		public static readonly string PropCoverOffset = "CoverOffset";

		// Token: 0x0400878A RID: 34698
		[SerializeField]
		public SelectedBlockInfo selectedModel = new SelectedBlockInfo(0);

		// Token: 0x0400878B RID: 34699
		[SerializeField]
		public Vector3i selectedMultiBlock;

		// Token: 0x0400878C RID: 34700
		[SerializeField]
		public bool editMultiBlock;

		// Token: 0x0400878D RID: 34701
		[SerializeField]
		public string searchFilter;
	}
}
