using System;

namespace XMLData.Exceptions
{
	// Token: 0x02001662 RID: 5730
	public class RedefinedElementException : XmlParserException
	{
		// Token: 0x0600B403 RID: 46083 RVA: 0x004370A1 File Offset: 0x004352A1
		public RedefinedElementException(string _msg, int _line) : base(_msg, _line)
		{
		}
	}
}
