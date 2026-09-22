using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Webserver
{
	// Token: 0x02001ACB RID: 6859
	public class WebConnection : ConsoleConnectionAbstract
	{
		// Token: 0x17001973 RID: 6515
		// (get) Token: 0x0600CE99 RID: 52889 RVA: 0x004B63AB File Offset: 0x004B45AB
		public string SessionID { get; }

		// Token: 0x17001974 RID: 6516
		// (get) Token: 0x0600CE9A RID: 52890 RVA: 0x004B63B3 File Offset: 0x004B45B3
		public IPAddress Endpoint { get; }

		// Token: 0x17001975 RID: 6517
		// (get) Token: 0x0600CE9B RID: 52891 RVA: 0x004B63BB File Offset: 0x004B45BB
		public string Username { get; }

		// Token: 0x17001976 RID: 6518
		// (get) Token: 0x0600CE9C RID: 52892 RVA: 0x004B63C3 File Offset: 0x004B45C3
		public PlatformUserIdentifierAbs UserId { get; }

		// Token: 0x17001977 RID: 6519
		// (get) Token: 0x0600CE9D RID: 52893 RVA: 0x004B63CB File Offset: 0x004B45CB
		public PlatformUserIdentifierAbs CrossplatformUserId { get; }

		// Token: 0x17001978 RID: 6520
		// (get) Token: 0x0600CE9E RID: 52894 RVA: 0x004B63D3 File Offset: 0x004B45D3
		public TimeSpan Age
		{
			get
			{
				return DateTime.Now - this.lastAction;
			}
		}

		// Token: 0x0600CE9F RID: 52895 RVA: 0x004B63E8 File Offset: 0x004B45E8
		public WebConnection(string _sessionId, IPAddress _endpoint, string _username, PlatformUserIdentifierAbs _userId, PlatformUserIdentifierAbs _crossUserId = null)
		{
			this.SessionID = _sessionId;
			this.Endpoint = _endpoint;
			this.Username = _username;
			this.UserId = _userId;
			this.CrossplatformUserId = _crossUserId;
			this.login = DateTime.Now;
			this.lastAction = this.login;
			this.conDescription = string.Format("WebPanel from {0}", this.Endpoint);
		}

		// Token: 0x0600CEA0 RID: 52896 RVA: 0x004B644D File Offset: 0x004B464D
		public void UpdateUsage()
		{
			this.lastAction = DateTime.Now;
		}

		// Token: 0x0600CEA1 RID: 52897 RVA: 0x004B645A File Offset: 0x004B465A
		public override string GetDescription()
		{
			return this.conDescription;
		}

		// Token: 0x0600CEA2 RID: 52898 RVA: 0x000027FC File Offset: 0x000009FC
		public override void SendLine(string _text)
		{
		}

		// Token: 0x0600CEA3 RID: 52899 RVA: 0x000027FC File Offset: 0x000009FC
		public override void SendLines(List<string> _output)
		{
		}

		// Token: 0x0600CEA4 RID: 52900 RVA: 0x000027FC File Offset: 0x000009FC
		public override void SendLog(string _formattedMsg, string _plainMsg, string _trace, LogType _type, DateTime _timestamp, long _uptime)
		{
		}

		// Token: 0x04009CDC RID: 40156
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DateTime login;

		// Token: 0x04009CDD RID: 40157
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime lastAction;

		// Token: 0x04009CDE RID: 40158
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string conDescription;
	}
}
