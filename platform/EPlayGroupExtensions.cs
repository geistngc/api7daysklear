using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001B53 RID: 6995
	public static class EPlayGroupExtensions
	{
		// Token: 0x0600D15C RID: 53596 RVA: 0x004C9475 File Offset: 0x004C7675
		public static bool IsCurrent(this EPlayGroup group)
		{
			return group == EPlayGroupExtensions.Current;
		}

		// Token: 0x0600D15D RID: 53597 RVA: 0x004C947F File Offset: 0x004C767F
		[PublicizedFrom(EAccessModifier.Private)]
		public static EPlayGroup GetCurrentPlayGroup()
		{
			if (DeviceFlag.PS5.IsCurrent())
			{
				return EPlayGroup.PS5;
			}
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent())
			{
				return EPlayGroup.XBS;
			}
			return EPlayGroup.Standalone;
		}

		// Token: 0x0600D15E RID: 53598 RVA: 0x004C9498 File Offset: 0x004C7698
		public static EPlayGroup ToPlayGroup(this DeviceFlag device)
		{
			if (device <= DeviceFlag.XBoxSeriesS)
			{
				switch (device)
				{
				case DeviceFlag.StandaloneWindows:
					return EPlayGroup.Standalone;
				case DeviceFlag.StandaloneLinux:
					return EPlayGroup.Standalone;
				case DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux:
					break;
				case DeviceFlag.StandaloneOSX:
					return EPlayGroup.Standalone;
				default:
					if (device == DeviceFlag.XBoxSeriesS)
					{
						return EPlayGroup.XBS;
					}
					break;
				}
			}
			else
			{
				if (device == DeviceFlag.XBoxSeriesX)
				{
					return EPlayGroup.XBS;
				}
				if (device == DeviceFlag.PS5)
				{
					return EPlayGroup.PS5;
				}
			}
			throw new ArgumentOutOfRangeException("device", device, string.Format("Missing play group mapping for {0}.", device));
		}

		// Token: 0x0600D15F RID: 53599 RVA: 0x004C9510 File Offset: 0x004C7710
		public static EPlayGroup ToPlayGroup(this ClientInfo.EDeviceType deviceType)
		{
			EPlayGroup result;
			switch (deviceType)
			{
			case ClientInfo.EDeviceType.Linux:
				result = EPlayGroup.Standalone;
				break;
			case ClientInfo.EDeviceType.Mac:
				result = EPlayGroup.Standalone;
				break;
			case ClientInfo.EDeviceType.Windows:
				result = EPlayGroup.Standalone;
				break;
			case ClientInfo.EDeviceType.PlayStation:
				result = EPlayGroup.PS5;
				break;
			case ClientInfo.EDeviceType.Xbox:
				result = EPlayGroup.XBS;
				break;
			case ClientInfo.EDeviceType.Unknown:
				result = EPlayGroup.Standalone;
				break;
			default:
				throw new ArgumentOutOfRangeException("deviceType", deviceType, string.Format("Missing play group mapping for {0}.", deviceType));
			}
			return result;
		}

		// Token: 0x0600D160 RID: 53600 RVA: 0x004C9577 File Offset: 0x004C7777
		public static uint[] GetCurrentlyAllowedPlatformIds()
		{
			if (PermissionsManager.IsCrossplayAllowed())
			{
				return null;
			}
			return EPlayGroupExtensions.s_playGroupToAllowedPlatformIds[EPlayGroupExtensions.Current];
		}

		// Token: 0x0400A0A7 RID: 41127
		public static readonly EPlayGroup Current = EPlayGroupExtensions.GetCurrentPlayGroup();

		// Token: 0x0400A0A8 RID: 41128
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<EPlayGroup, uint[]> s_playGroupToAllowedPlatformIds = new Dictionary<EPlayGroup, uint[]>
		{
			{
				EPlayGroup.Standalone,
				null
			},
			{
				EPlayGroup.XBS,
				null
			},
			{
				EPlayGroup.PS5,
				null
			}
		};
	}
}
