using System;
using System.IO;
using System.Reflection;

namespace Webserver
{
	// Token: 0x02001AC4 RID: 6852
	public static class ResourceHelpers
	{
		// Token: 0x0600CE7D RID: 52861 RVA: 0x004B5648 File Offset: 0x004B3848
		public static Stream OpenManifestResource(Assembly _assembly, string _name, bool _ignoreCase = false)
		{
			foreach (string text in _assembly.GetManifestResourceNames())
			{
				if (text.EndsWith(_name, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
				{
					return _assembly.GetManifestResourceStream(text);
				}
			}
			return null;
		}

		// Token: 0x0600CE7E RID: 52862 RVA: 0x004B5688 File Offset: 0x004B3888
		public static string GetManifestResourceText(Assembly _assembly, string _name, bool _ignoreCase = false)
		{
			string result;
			using (Stream stream = ResourceHelpers.OpenManifestResource(_assembly, _name, _ignoreCase))
			{
				if (stream == null)
				{
					result = null;
				}
				else
				{
					using (TextReader textReader = new StreamReader(stream))
					{
						result = textReader.ReadToEnd();
					}
				}
			}
			return result;
		}
	}
}
