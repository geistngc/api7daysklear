using System;

namespace Platform
{
	// Token: 0x02001BAA RID: 7082
	public struct CensoredTextRequest
	{
		// Token: 0x17001A1A RID: 6682
		// (get) Token: 0x0600D354 RID: 54100 RVA: 0x004CAFAD File Offset: 0x004C91AD
		public readonly string Input { get; }

		// Token: 0x17001A1B RID: 6683
		// (get) Token: 0x0600D355 RID: 54101 RVA: 0x004CAFB5 File Offset: 0x004C91B5
		// (set) Token: 0x0600D356 RID: 54102 RVA: 0x004CAFBD File Offset: 0x004C91BD
		public int CensoredLength { readonly get; set; }

		// Token: 0x17001A1C RID: 6684
		// (get) Token: 0x0600D357 RID: 54103 RVA: 0x004CAFC6 File Offset: 0x004C91C6
		public readonly Action<CensoredTextResult> Callback { get; }

		// Token: 0x0600D358 RID: 54104 RVA: 0x004CAFCE File Offset: 0x004C91CE
		public CensoredTextRequest(string _input, Action<CensoredTextResult> _callback)
		{
			this.Input = _input;
			this.CensoredLength = _input.Length;
			this.Callback = _callback;
		}
	}
}
