using System;
using System.Collections.Generic;
using UnityEngine;

namespace Webserver
{
	// Token: 0x02001ABE RID: 6846
	public class LogBuffer
	{
		// Token: 0x1400011A RID: 282
		// (add) Token: 0x0600CE64 RID: 52836 RVA: 0x004B26E8 File Offset: 0x004B08E8
		// (remove) Token: 0x0600CE65 RID: 52837 RVA: 0x004B271C File Offset: 0x004B091C
		public static event Action<LogBuffer.LogEntry> EntryAdded;

		// Token: 0x0600CE66 RID: 52838 RVA: 0x004B274F File Offset: 0x004B094F
		public static void Init()
		{
			if (LogBuffer.instance == null)
			{
				LogBuffer.instance = new LogBuffer();
			}
		}

		// Token: 0x0600CE67 RID: 52839 RVA: 0x004B2762 File Offset: 0x004B0962
		[PublicizedFrom(EAccessModifier.Private)]
		public LogBuffer()
		{
			Log.LogCallbacksExtended += this.LogCallback;
		}

		// Token: 0x1700196D RID: 6509
		// (get) Token: 0x0600CE68 RID: 52840 RVA: 0x004B2791 File Offset: 0x004B0991
		public static LogBuffer Instance
		{
			get
			{
				LogBuffer result;
				if ((result = LogBuffer.instance) == null)
				{
					result = (LogBuffer.instance = new LogBuffer());
				}
				return result;
			}
		}

		// Token: 0x1700196E RID: 6510
		// (get) Token: 0x0600CE69 RID: 52841 RVA: 0x004B27A8 File Offset: 0x004B09A8
		public int OldestLine
		{
			get
			{
				List<LogBuffer.LogEntry> obj = this.logEntries;
				int result;
				lock (obj)
				{
					result = this.listOffset;
				}
				return result;
			}
		}

		// Token: 0x1700196F RID: 6511
		// (get) Token: 0x0600CE6A RID: 52842 RVA: 0x004B27EC File Offset: 0x004B09EC
		public int LatestLine
		{
			get
			{
				List<LogBuffer.LogEntry> obj = this.logEntries;
				int result;
				lock (obj)
				{
					result = this.listOffset + this.logEntries.Count - 1;
				}
				return result;
			}
		}

		// Token: 0x17001970 RID: 6512
		// (get) Token: 0x0600CE6B RID: 52843 RVA: 0x004B283C File Offset: 0x004B0A3C
		public int StoredLines
		{
			get
			{
				List<LogBuffer.LogEntry> obj = this.logEntries;
				int count;
				lock (obj)
				{
					count = this.logEntries.Count;
				}
				return count;
			}
		}

		// Token: 0x17001971 RID: 6513
		public LogBuffer.LogEntry this[int _index]
		{
			get
			{
				List<LogBuffer.LogEntry> obj = this.logEntries;
				lock (obj)
				{
					if (_index >= this.listOffset && _index < this.listOffset + this.logEntries.Count)
					{
						return this.logEntries[_index];
					}
				}
				return null;
			}
		}

		// Token: 0x0600CE6D RID: 52845 RVA: 0x004B28F0 File Offset: 0x004B0AF0
		[PublicizedFrom(EAccessModifier.Private)]
		public void LogCallback(string _formattedMsg, string _plainMsg, string _trace, LogType _type, DateTime _timestamp, long _uptime)
		{
			List<LogBuffer.LogEntry> obj = this.logEntries;
			lock (obj)
			{
				LogBuffer.LogEntry logEntry = new LogBuffer.LogEntry(this.listOffset + this.logEntries.Count, _timestamp, _plainMsg, _trace, _type, _uptime);
				this.logEntries.Add(logEntry);
				Action<LogBuffer.LogEntry> entryAdded = LogBuffer.EntryAdded;
				if (entryAdded != null)
				{
					entryAdded(logEntry);
				}
				if (this.logEntries.Count > 3000)
				{
					this.listOffset += this.logEntries.Count - 3000;
					this.logEntries.RemoveRange(0, this.logEntries.Count - 3000);
				}
			}
		}

		// Token: 0x0600CE6E RID: 52846 RVA: 0x004B29B8 File Offset: 0x004B0BB8
		public List<LogBuffer.LogEntry> GetRange(ref int _start, int _count, out int _end)
		{
			List<LogBuffer.LogEntry> obj = this.logEntries;
			List<LogBuffer.LogEntry> range;
			lock (obj)
			{
				int num;
				if (_count < 0)
				{
					_count = -_count;
					if (_start >= this.listOffset + this.logEntries.Count)
					{
						_start = this.listOffset + this.logEntries.Count - 1;
					}
					_end = _start;
					if (_start < this.listOffset)
					{
						return this.emptyList;
					}
					_start -= _count - 1;
					if (_start < this.listOffset)
					{
						_start = this.listOffset;
					}
					num = _start - this.listOffset;
					_end++;
					_count = _end - _start;
				}
				else
				{
					if (_start < this.listOffset)
					{
						_start = this.listOffset;
					}
					if (_start >= this.listOffset + this.logEntries.Count)
					{
						_end = _start;
						return this.emptyList;
					}
					num = _start - this.listOffset;
					if (num + _count > this.logEntries.Count)
					{
						_count = this.logEntries.Count - num;
					}
					_end = _start + _count;
				}
				range = this.logEntries.GetRange(num, _count);
			}
			return range;
		}

		// Token: 0x04009C9C RID: 40092
		[PublicizedFrom(EAccessModifier.Private)]
		public const int maxEntries = 3000;

		// Token: 0x04009C9D RID: 40093
		[PublicizedFrom(EAccessModifier.Private)]
		public static LogBuffer instance;

		// Token: 0x04009C9E RID: 40094
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<LogBuffer.LogEntry> logEntries = new List<LogBuffer.LogEntry>();

		// Token: 0x04009C9F RID: 40095
		[PublicizedFrom(EAccessModifier.Private)]
		public int listOffset;

		// Token: 0x04009CA1 RID: 40097
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<LogBuffer.LogEntry> emptyList = new List<LogBuffer.LogEntry>();

		// Token: 0x02001ABF RID: 6847
		public class LogEntry
		{
			// Token: 0x0600CE6F RID: 52847 RVA: 0x004B2AE8 File Offset: 0x004B0CE8
			public LogEntry(int _messageId, DateTime _timestamp, string _message, string _trace, LogType _type, long _uptime)
			{
				this.MessageId = _messageId;
				this.Timestamp = _timestamp;
				this.IsoTime = _timestamp.ToString("o");
				this.Message = _message;
				this.Trace = _trace;
				this.Type = _type;
				this.Uptime = _uptime;
			}

			// Token: 0x04009CA2 RID: 40098
			public readonly int MessageId;

			// Token: 0x04009CA3 RID: 40099
			public readonly DateTime Timestamp;

			// Token: 0x04009CA4 RID: 40100
			public readonly string IsoTime;

			// Token: 0x04009CA5 RID: 40101
			public readonly string Message;

			// Token: 0x04009CA6 RID: 40102
			public readonly string Trace;

			// Token: 0x04009CA7 RID: 40103
			public readonly LogType Type;

			// Token: 0x04009CA8 RID: 40104
			public readonly long Uptime;
		}
	}
}
