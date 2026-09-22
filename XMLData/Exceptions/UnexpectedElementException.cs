using System;

namespace XMLData.Exceptions
{
	// Token: 0x02001663 RID: 5731
	public class UnexpectedElementException : XmlParserException
	{
		// Token: 0x0600B404 RID: 46084 RVA: 0x004370A1 File Offset: 0x004352A1
		public UnexpectedElementException(string _msg, int _line) : base(_msg, _line)
		{
		}
	}
}
