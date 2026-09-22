using System;
using System.Globalization;
using System.Net;

namespace Twitch
{
	// Token: 0x020017E4 RID: 6116
	public class TwitchExtensionCommand
	{
		// Token: 0x0600BDB3 RID: 48563 RVA: 0x00465928 File Offset: 0x00463B28
		public TwitchExtensionCommand(HttpListenerRequest _req)
		{
			this.userId = StringParsers.ParseSInt32(_req.QueryString.Get(0), 0, -1, NumberStyles.Integer);
			this.command = "#" + _req.QueryString.Get(1);
			this.id = StringParsers.ParseSInt32(_req.QueryString.Get(2), 0, -1, NumberStyles.Integer);
			this.isRerun = StringParsers.ParseBool(_req.QueryString.Get(3), 0, -1, true);
			Log.Out(string.Format("{0}: {1} : {2}", this.userId, this.command, this.id));
		}

		// Token: 0x04008E54 RID: 36436
		public int userId;

		// Token: 0x04008E55 RID: 36437
		public string command;

		// Token: 0x04008E56 RID: 36438
		public int id;

		// Token: 0x04008E57 RID: 36439
		public bool isRerun;
	}
}
