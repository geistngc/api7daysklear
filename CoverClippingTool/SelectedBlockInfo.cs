using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoverClippingTool
{
	// Token: 0x02001678 RID: 5752
	[Serializable]
	public struct SelectedBlockInfo
	{
		// Token: 0x170015A7 RID: 5543
		// (get) Token: 0x0600B469 RID: 46185 RVA: 0x0043909A File Offset: 0x0043729A
		// (set) Token: 0x0600B46A RID: 46186 RVA: 0x004390A4 File Offset: 0x004372A4
		public BlockShapeInfo shapeInfo
		{
			get
			{
				return this._shapeInfo;
			}
			set
			{
				this._shapeInfo = value;
				BlockShapeInfo shapeInfo = this._shapeInfo;
				this._savedShapeReference.Name = ((shapeInfo != null) ? shapeInfo.Name : null);
				this._savedShapeReference.Source = ((this._shapeInfo != null) ? this._shapeInfo.Source : DataSource.Block);
			}
		}

		// Token: 0x170015A8 RID: 5544
		// (get) Token: 0x0600B46B RID: 46187 RVA: 0x004390F6 File Offset: 0x004372F6
		public ShapeReference savedShapeReference
		{
			get
			{
				return this._savedShapeReference;
			}
		}

		// Token: 0x0600B46C RID: 46188 RVA: 0x004390FE File Offset: 0x004372FE
		public SelectedBlockInfo(int thisIsDumb)
		{
			this._shapeInfo = null;
			this._savedShapeReference = default(ShapeReference);
			this.renderables = new List<Renderable>();
		}

		// Token: 0x04008784 RID: 34692
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public BlockShapeInfo _shapeInfo;

		// Token: 0x04008785 RID: 34693
		[SerializeField]
		[PublicizedFrom(EAccessModifier.Private)]
		public ShapeReference _savedShapeReference;

		// Token: 0x04008786 RID: 34694
		public readonly List<Renderable> renderables;
	}
}
