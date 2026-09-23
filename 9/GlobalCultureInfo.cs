using System;
using System.Globalization;
using System.Threading;

// Token: 0x02001418 RID: 5144
public class GlobalCultureInfo
{
	// Token: 0x0600A18C RID: 41356 RVA: 0x003CBB95 File Offset: 0x003C9D95
	public static bool SetDefaultCulture(CultureInfo _culture)
	{
		Thread.CurrentThread.CurrentCulture = _culture;
		CultureInfo.DefaultThreadCurrentCulture = _culture;
		return true;
	}
}
