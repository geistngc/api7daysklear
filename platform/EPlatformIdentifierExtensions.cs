using System;

namespace Platform
{
	// Token: 0x02001B51 RID: 6993
	public static class EPlatformIdentifierExtensions
	{
		// Token: 0x0600D158 RID: 53592 RVA: 0x004C92CC File Offset: 0x004C74CC
		public static bool IsNative(this EPlatformIdentifier platformIdentifier)
		{
			bool result;
			switch (platformIdentifier)
			{
			case EPlatformIdentifier.None:
				result = false;
				break;
			case EPlatformIdentifier.Local:
				result = true;
				break;
			case EPlatformIdentifier.EOS:
				result = false;
				break;
			case EPlatformIdentifier.Steam:
				result = true;
				break;
			case EPlatformIdentifier.XBL:
				result = true;
				break;
			case EPlatformIdentifier.PSN:
				result = true;
				break;
			case EPlatformIdentifier.EGS:
				result = true;
				break;
			case EPlatformIdentifier.LAN:
				result = false;
				break;
			case EPlatformIdentifier.Count:
				result = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("platformIdentifier", platformIdentifier, null);
			}
			return result;
		}

		// Token: 0x0600D159 RID: 53593 RVA: 0x004C933C File Offset: 0x004C753C
		public static bool IsCross(this EPlatformIdentifier platformIdentifier)
		{
			bool result;
			switch (platformIdentifier)
			{
			case EPlatformIdentifier.None:
				result = true;
				break;
			case EPlatformIdentifier.Local:
				result = false;
				break;
			case EPlatformIdentifier.EOS:
				result = true;
				break;
			case EPlatformIdentifier.Steam:
				result = false;
				break;
			case EPlatformIdentifier.XBL:
				result = false;
				break;
			case EPlatformIdentifier.PSN:
				result = false;
				break;
			case EPlatformIdentifier.EGS:
				result = false;
				break;
			case EPlatformIdentifier.LAN:
				result = false;
				break;
			case EPlatformIdentifier.Count:
				result = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("platformIdentifier", platformIdentifier, null);
			}
			return result;
		}

		// Token: 0x0600D15A RID: 53594 RVA: 0x004C93AC File Offset: 0x004C75AC
		public static bool IsServer(this EPlatformIdentifier platformIdentifier)
		{
			bool result;
			switch (platformIdentifier)
			{
			case EPlatformIdentifier.None:
				result = false;
				break;
			case EPlatformIdentifier.Local:
				result = false;
				break;
			case EPlatformIdentifier.EOS:
				result = false;
				break;
			case EPlatformIdentifier.Steam:
				result = true;
				break;
			case EPlatformIdentifier.XBL:
				result = true;
				break;
			case EPlatformIdentifier.PSN:
				result = true;
				break;
			case EPlatformIdentifier.EGS:
				result = true;
				break;
			case EPlatformIdentifier.LAN:
				result = true;
				break;
			case EPlatformIdentifier.Count:
				result = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("platformIdentifier", platformIdentifier, null);
			}
			return result;
		}

		// Token: 0x0600D15B RID: 53595 RVA: 0x004C941C File Offset: 0x004C761C
		public static bool IsServerValid(this EPlatformIdentifier serverPlatform, EPlatformIdentifier nativePlatform, EPlatformIdentifier crossPlatform)
		{
			bool flag = serverPlatform.IsServer();
			if (flag)
			{
				bool flag2;
				switch (serverPlatform)
				{
				case EPlatformIdentifier.Steam:
					flag2 = true;
					break;
				case EPlatformIdentifier.XBL:
				case EPlatformIdentifier.PSN:
				case EPlatformIdentifier.EGS:
					flag2 = (crossPlatform == EPlatformIdentifier.EOS);
					break;
				case EPlatformIdentifier.LAN:
					flag2 = true;
					break;
				default:
					throw new ArgumentOutOfRangeException("serverPlatform", serverPlatform, null);
				}
				flag = flag2;
			}
			return flag;
		}
	}
}
