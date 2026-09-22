using System;

namespace XMLData.Exceptions
{
	// Token: 0x02001660 RID: 5728
	public class InvalidValueException : XmlParserException
	{
		// Token: 0x0600B400 RID: 46080 RVA: 0x004370A1 File Offset: 0x004352A1
		public InvalidValueException(string _msg, int _line) : base(_msg, _line)
		{
		}

		// Token: 0x0600B401 RID: 46081 RVA: 0x004370AB File Offset: 0x004352AB
		public InvalidValueException(string _msg, int _line, Exception _innerException) : base(_msg, _line, _innerException)
		{
		}
	}
}
