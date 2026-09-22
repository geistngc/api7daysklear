using System;
using System.Collections.Generic;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;
using UnityEngine;

namespace Services.Analytics.Events
{
	// Token: 0x0200168D RID: 5773
	[EventTitle("Hardware Info")]
	public class HardwareInfoEventData : BaseEventData
	{
		// Token: 0x170015D7 RID: 5591
		// (get) Token: 0x0600B4F0 RID: 46320 RVA: 0x0043A826 File Offset: 0x00438A26
		public override string EventType
		{
			get
			{
				return "hardware_info";
			}
		}

		// Token: 0x170015D8 RID: 5592
		// (get) Token: 0x0600B4F1 RID: 46321 RVA: 0x0043A82D File Offset: 0x00438A2D
		// (set) Token: 0x0600B4F2 RID: 46322 RVA: 0x0043A835 File Offset: 0x00438A35
		[JsonProperty(PropertyName = "os")]
		public string OperatingSystem { get; [PublicizedFrom(EAccessModifier.Private)] set; } = SystemInfo.operatingSystem;

		// Token: 0x170015D9 RID: 5593
		// (get) Token: 0x0600B4F3 RID: 46323 RVA: 0x0043A83E File Offset: 0x00438A3E
		// (set) Token: 0x0600B4F4 RID: 46324 RVA: 0x0043A846 File Offset: 0x00438A46
		[JsonProperty(PropertyName = "cpu_data")]
		public Dictionary<string, string> CpuData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Dictionary<string, string>
		{
			{
				"type",
				SystemInfo.processorType
			},
			{
				"count",
				SystemInfo.processorCount.ToString()
			}
		};

		// Token: 0x170015DA RID: 5594
		// (get) Token: 0x0600B4F5 RID: 46325 RVA: 0x0043A84F File Offset: 0x00438A4F
		// (set) Token: 0x0600B4F6 RID: 46326 RVA: 0x0043A857 File Offset: 0x00438A57
		[JsonProperty(PropertyName = "gpu_data")]
		public Dictionary<string, string> GpuData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Dictionary<string, string>
		{
			{
				"name",
				SystemInfo.graphicsDeviceName
			},
			{
				"type",
				SystemInfo.graphicsDeviceType.ToString()
			},
			{
				"version",
				SystemInfo.graphicsDeviceVersion
			},
			{
				"memory",
				SystemInfo.graphicsMemorySize.ToString()
			},
			{
				"vendor",
				SystemInfo.graphicsDeviceVendor
			}
		};

		// Token: 0x170015DB RID: 5595
		// (get) Token: 0x0600B4F7 RID: 46327 RVA: 0x0043A860 File Offset: 0x00438A60
		// (set) Token: 0x0600B4F8 RID: 46328 RVA: 0x0043A868 File Offset: 0x00438A68
		[JsonProperty(PropertyName = "memory_ram")]
		public int MemoryRam { get; [PublicizedFrom(EAccessModifier.Private)] set; } = SystemInfo.systemMemorySize;
	}
}
