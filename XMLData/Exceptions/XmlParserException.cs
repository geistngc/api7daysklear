using System;

namespace XMLData.Exceptions
{
	// Token: 0x02001664 RID: 5732
	public class XmlParserException : Exception
	{
		// Token: 0x17001590 RID: 5520
		// (get) Token: 0x0600B405 RID: 46085 RVA: 0x004370B6 File Offset: 0x004352B6
		// (set) Token: 0x0600B406 RID: 46086 RVA: 0x004370BE File Offset: 0x004352BE
		public int Line { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B407 RID: 46087 RVA: 0x004370C7 File Offset: 0x004352C7
		public XmlParserException(string _msg, int _line) : base(_msg)
		{
			this.Line = _line;
		}

		// Token: 0x0600B408 RID: 46088 RVA: 0x004370D7 File Offset: 0x004352D7
		public XmlParserException(string _msg, int _line, Exception _innerException) : base(_msg, _innerException)
		{
			this.Line = _line;
		}

		// Token: 0x17001591 RID: 5521
		// (get) Token: 0x0600B409 RID: 46089 RVA: 0x004370E8 File Offset: 0x004352E8
		public override string Message
		{
			get
			{
				return string.Format("{0} (line {1})", base.Message, this.Line);
			}
		}

		// Token: 0x0600B40A RID: 46090 RVA: 0x00437105 File Offset: 0x00435305
		public override string ToString()
		{
			return this.Message;
		}
	}
}
