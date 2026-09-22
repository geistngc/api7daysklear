using System;

namespace XMLData.Exceptions
{
	// Token: 0x02001661 RID: 5729
	public class MissingAttributeException : XmlParserException
	{
		// Token: 0x0600B402 RID: 46082 RVA: 0x004370A1 File Offset: 0x004352A1
		public MissingAttributeException(string _msg, int _line) : base(_msg, _line)
		{
		}
	}
}
