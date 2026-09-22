using System;
using System.Collections.Generic;
using System.Net;

namespace Webserver
{
	// Token: 0x02001ABC RID: 6844
	public class ConnectionHandler
	{
		// Token: 0x0600CE5F RID: 52831 RVA: 0x004B2604 File Offset: 0x004B0804
		public WebConnection IsLoggedIn(string _sessionId, IPAddress _ip)
		{
			WebConnection webConnection;
			if (!this.connections.TryGetValue(_sessionId, out webConnection))
			{
				return null;
			}
			webConnection.UpdateUsage();
			return webConnection;
		}

		// Token: 0x0600CE60 RID: 52832 RVA: 0x004B262A File Offset: 0x004B082A
		public void LogOut(string _sessionId)
		{
			this.connections.Remove(_sessionId);
		}

		// Token: 0x0600CE61 RID: 52833 RVA: 0x004B263C File Offset: 0x004B083C
		public WebConnection LogIn(IPAddress _ip, string _username, PlatformUserIdentifierAbs _userId, PlatformUserIdentifierAbs _crossUserId = null)
		{
			string text = Guid.NewGuid().ToString();
			WebConnection webConnection = new WebConnection(text, _ip, _username, _userId, _crossUserId);
			this.connections.Add(text, webConnection);
			return webConnection;
		}

		// Token: 0x0600CE62 RID: 52834 RVA: 0x004B2678 File Offset: 0x004B0878
		public void SendLine(string _line)
		{
			foreach (KeyValuePair<string, WebConnection> keyValuePair in this.connections)
			{
				keyValuePair.Value.SendLine(_line);
			}
		}

		// Token: 0x04009C92 RID: 40082
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, WebConnection> connections = new Dictionary<string, WebConnection>();
	}
}
