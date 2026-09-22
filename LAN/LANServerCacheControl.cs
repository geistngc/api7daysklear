using System;
using System.Collections.Generic;

namespace Platform.LAN
{
	// Token: 0x02001CD4 RID: 7380
	public class LANServerCacheControl
	{
		// Token: 0x0600DB19 RID: 56089 RVA: 0x004E79FD File Offset: 0x004E5BFD
		public LANServerCacheControl(TimeSpan updateInterval, TimeSpan timeout)
		{
			this.timeout = timeout;
			this.updateInterval = updateInterval;
		}

		// Token: 0x0600DB1A RID: 56090 RVA: 0x004E7A20 File Offset: 0x004E5C20
		public bool IsUpdateRequired(string addressString, int port)
		{
			LANServerCacheControl.ServerKey key = new LANServerCacheControl.ServerKey(addressString, port);
			LANServerCacheControl.UpdateTimes updateTimes;
			if (!this.lastServerUpdateTimes.TryGetValue(key, out updateTimes))
			{
				this.lastServerUpdateTimes[key] = new LANServerCacheControl.UpdateTimes
				{
					lastServerCheckedTime = DateTime.Now
				};
				return true;
			}
			TimeSpan t = DateTime.Now - updateTimes.lastServerCheckedTime;
			updateTimes.lastServerCheckedTime = DateTime.Now;
			this.lastServerUpdateTimes[key] = updateTimes;
			if (t > this.timeout)
			{
				Log.Out(string.Format("[{0}] server timed out, update needed. Last checked {1:F2} seconds ago", "LANServerCacheControl", t.TotalSeconds));
				return true;
			}
			TimeSpan t2 = DateTime.Now - updateTimes.lastRulesUpdateTime;
			if (t2 > this.updateInterval)
			{
				Log.Out(string.Format("[{0}] found known server, last updated {1:F2} seconds ago", "LANServerCacheControl", t2.TotalSeconds));
				return true;
			}
			return false;
		}

		// Token: 0x0600DB1B RID: 56091 RVA: 0x004E7B0C File Offset: 0x004E5D0C
		public void SetUpdated(string addressString, int port)
		{
			LANServerCacheControl.ServerKey key = new LANServerCacheControl.ServerKey(addressString, port);
			LANServerCacheControl.UpdateTimes value;
			if (!this.lastServerUpdateTimes.TryGetValue(key, out value))
			{
				this.lastServerUpdateTimes[key] = new LANServerCacheControl.UpdateTimes
				{
					lastServerCheckedTime = DateTime.Now,
					lastRulesUpdateTime = DateTime.Now
				};
				return;
			}
			value.lastRulesUpdateTime = DateTime.Now;
			this.lastServerUpdateTimes[key] = value;
		}

		// Token: 0x0600DB1C RID: 56092 RVA: 0x004E7B79 File Offset: 0x004E5D79
		public void Clear()
		{
			this.lastServerUpdateTimes.Clear();
		}

		// Token: 0x0400A5E4 RID: 42468
		[PublicizedFrom(EAccessModifier.Private)]
		public TimeSpan timeout;

		// Token: 0x0400A5E5 RID: 42469
		[PublicizedFrom(EAccessModifier.Private)]
		public TimeSpan updateInterval;

		// Token: 0x0400A5E6 RID: 42470
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<LANServerCacheControl.ServerKey, LANServerCacheControl.UpdateTimes> lastServerUpdateTimes = new Dictionary<LANServerCacheControl.ServerKey, LANServerCacheControl.UpdateTimes>();

		// Token: 0x02001CD5 RID: 7381
		[PublicizedFrom(EAccessModifier.Private)]
		public struct ServerKey : IEquatable<LANServerCacheControl.ServerKey>
		{
			// Token: 0x0600DB1D RID: 56093 RVA: 0x004E7B86 File Offset: 0x004E5D86
			public ServerKey(string _ipAddress, int _port)
			{
				this.ipAddress = _ipAddress;
				this.port = _port;
			}

			// Token: 0x0600DB1E RID: 56094 RVA: 0x004E7B96 File Offset: 0x004E5D96
			public override int GetHashCode()
			{
				return this.ipAddress.GetHashCode() ^ this.port;
			}

			// Token: 0x0600DB1F RID: 56095 RVA: 0x004E7BAA File Offset: 0x004E5DAA
			public bool Equals(LANServerCacheControl.ServerKey _other)
			{
				return this.ipAddress.Equals(_other.ipAddress) && this.port == _other.port;
			}

			// Token: 0x0400A5E7 RID: 42471
			public readonly string ipAddress;

			// Token: 0x0400A5E8 RID: 42472
			public readonly int port;
		}

		// Token: 0x02001CD6 RID: 7382
		[PublicizedFrom(EAccessModifier.Private)]
		public struct UpdateTimes
		{
			// Token: 0x0400A5E9 RID: 42473
			public DateTime lastServerCheckedTime;

			// Token: 0x0400A5EA RID: 42474
			public DateTime lastRulesUpdateTime;
		}
	}
}
