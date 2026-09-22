using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

// Token: 0x020013A2 RID: 5026
public class BenchmarkObject
{
	// Token: 0x06009E86 RID: 40582 RVA: 0x003BF930 File Offset: 0x003BDB30
	[Conditional("PROFILEx")]
	public static void StartTimer(string _benchmarkName, object _watchObject)
	{
		BenchmarkObject.BenchmarkContainer benchmarkContainer = new BenchmarkObject.BenchmarkContainer(_benchmarkName);
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			BenchmarkObject.benchmarks[_watchObject] = benchmarkContainer;
		}
		benchmarkContainer.startTick = DateTime.Now.Ticks;
	}

	// Token: 0x06009E87 RID: 40583 RVA: 0x003BF990 File Offset: 0x003BDB90
	[Conditional("PROFILEx")]
	public static void SwitchObject(object _old, object _new)
	{
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			if (BenchmarkObject.benchmarks.ContainsKey(_old))
			{
				BenchmarkObject.BenchmarkContainer value = BenchmarkObject.benchmarks[_old];
				BenchmarkObject.benchmarks.Remove(_old);
				BenchmarkObject.benchmarks[_new] = value;
			}
			else
			{
				Log.Out("SWITCHOBJECT: Object not found");
			}
		}
	}

	// Token: 0x06009E88 RID: 40584 RVA: 0x003BFA08 File Offset: 0x003BDC08
	[Conditional("PROFILEx")]
	public static void UpdateName(object _watchObject, string _nameAppend)
	{
		BenchmarkObject.BenchmarkContainer benchmarkContainer = null;
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			if (BenchmarkObject.benchmarks.ContainsKey(_watchObject))
			{
				benchmarkContainer = BenchmarkObject.benchmarks[_watchObject];
			}
		}
		if (benchmarkContainer != null)
		{
			BenchmarkObject.BenchmarkContainer benchmarkContainer2 = benchmarkContainer;
			benchmarkContainer2.name += _nameAppend;
			return;
		}
		Log.Out("UPDATENAME: Object not found: " + _nameAppend);
	}

	// Token: 0x06009E89 RID: 40585 RVA: 0x003BFA84 File Offset: 0x003BDC84
	[Conditional("PROFILEx")]
	public static void StopTimer(object _watchObject)
	{
		long ticks = DateTime.Now.Ticks;
		Dictionary<object, BenchmarkObject.BenchmarkContainer> obj = BenchmarkObject.benchmarks;
		lock (obj)
		{
			if (BenchmarkObject.benchmarks.ContainsKey(_watchObject))
			{
				BenchmarkObject.benchmarks[_watchObject].endTick = ticks;
			}
			else
			{
				Log.Out("STOPTIMER: Object not found");
			}
		}
	}

	// Token: 0x06009E8A RID: 40586 RVA: 0x003BFAF8 File Offset: 0x003BDCF8
	[Conditional("PROFILEx")]
	public static void PrintAll()
	{
		if (BenchmarkObject.benchmarks.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (KeyValuePair<object, BenchmarkObject.BenchmarkContainer> keyValuePair in BenchmarkObject.benchmarks)
			{
				if (keyValuePair.Value.name.Length > num)
				{
					num = keyValuePair.Value.name.Length;
				}
			}
			string format = "{0} {1} {2} {3}" + Environment.NewLine;
			stringBuilder.Append(string.Format(format, new object[]
			{
				"Name",
				"Start",
				"End",
				"Duration"
			}));
			foreach (BenchmarkObject.BenchmarkContainer benchmarkContainer in from b in BenchmarkObject.benchmarks.Values.ToList<BenchmarkObject.BenchmarkContainer>()
			orderby b.startTick
			select b)
			{
				stringBuilder.Append(string.Format(format, new object[]
				{
					benchmarkContainer.name,
					benchmarkContainer.startTick / 10L,
					benchmarkContainer.endTick / 10L,
					benchmarkContainer.ticks / 10L
				}));
			}
			SdFile.WriteAllText(GameIO.GetGameDir("") + "durations.txt", stringBuilder.ToString());
		}
	}

	// Token: 0x04007873 RID: 30835
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<object, BenchmarkObject.BenchmarkContainer> benchmarks = new Dictionary<object, BenchmarkObject.BenchmarkContainer>();

	// Token: 0x020013A3 RID: 5027
	public class BenchmarkContainer
	{
		// Token: 0x170012C0 RID: 4800
		// (get) Token: 0x06009E8D RID: 40589 RVA: 0x003BFCB0 File Offset: 0x003BDEB0
		// (set) Token: 0x06009E8E RID: 40590 RVA: 0x003BFCB8 File Offset: 0x003BDEB8
		public string name
		{
			get
			{
				return this.pName;
			}
			set
			{
				this.pName = value;
			}
		}

		// Token: 0x170012C1 RID: 4801
		// (get) Token: 0x06009E8F RID: 40591 RVA: 0x003BFCC1 File Offset: 0x003BDEC1
		public long ticks
		{
			get
			{
				return this.endTick - this.startTick;
			}
		}

		// Token: 0x06009E90 RID: 40592 RVA: 0x003BFCD0 File Offset: 0x003BDED0
		public BenchmarkContainer(string _name)
		{
			this.pName = _name;
		}

		// Token: 0x04007874 RID: 30836
		[PublicizedFrom(EAccessModifier.Private)]
		public string pName;

		// Token: 0x04007875 RID: 30837
		public long startTick;

		// Token: 0x04007876 RID: 30838
		public long endTick;
	}
}
