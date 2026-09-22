using System;

namespace Platform
{
	// Token: 0x02001BAB RID: 7083
	public struct CensoredTextResult
	{
		// Token: 0x17001A1D RID: 6685
		// (get) Token: 0x0600D359 RID: 54105 RVA: 0x004CAFEA File Offset: 0x004C91EA
		public readonly bool Success { get; }

		// Token: 0x17001A1E RID: 6686
		// (get) Token: 0x0600D35A RID: 54106 RVA: 0x004CAFF2 File Offset: 0x004C91F2
		public readonly string OriginalText { get; }

		// Token: 0x17001A1F RID: 6687
		// (get) Token: 0x0600D35B RID: 54107 RVA: 0x004CAFFA File Offset: 0x004C91FA
		public readonly string CensoredText { get; }

		// Token: 0x0600D35C RID: 54108 RVA: 0x004CB002 File Offset: 0x004C9202
		public CensoredTextResult(bool _success, string _originalText, string _censoredText)
		{
			this.Success = _success;
			this.OriginalText = _originalText;
			this.CensoredText = _censoredText;
		}
	}
}
