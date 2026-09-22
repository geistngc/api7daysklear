using System;
using System.IO;
using Webserver.FileCache;
using Webserver.UrlHandlers;

namespace Webserver
{
	// Token: 0x02001ACC RID: 6860
	public class WebMod
	{
		// Token: 0x0600CEA5 RID: 52901 RVA: 0x004B6464 File Offset: 0x004B4664
		public WebMod(Web _parentWeb, Mod _parentMod, bool _useStaticCache)
		{
			this.ParentMod = _parentMod;
			string text = _parentMod.Path + "/WebMod";
			this.IsWebMod = Directory.Exists(text);
			if (!this.IsWebMod)
			{
				return;
			}
			this.ModUrl = "/webmods/" + _parentMod.Name + "/";
			this.ReactBundle = text + "/bundle.js";
			this.ReactBundle = (File.Exists(this.ReactBundle) ? (this.ModUrl + "bundle.js") : null);
			this.CssPath = text + "/styling.css";
			this.CssPath = (File.Exists(this.CssPath) ? (this.ModUrl + "styling.css") : null);
			_parentWeb.RegisterPathHandler(this.ModUrl, new StaticHandler(text, _useStaticCache ? new SimpleCache() : new DirectAccess(), false, null));
		}

		// Token: 0x04009CE4 RID: 40164
		[PublicizedFrom(EAccessModifier.Private)]
		public const string modsBaseUrl = "/webmods/";

		// Token: 0x04009CE5 RID: 40165
		[PublicizedFrom(EAccessModifier.Private)]
		public const string reactBundleName = "bundle.js";

		// Token: 0x04009CE6 RID: 40166
		[PublicizedFrom(EAccessModifier.Private)]
		public const string stylingFileName = "styling.css";

		// Token: 0x04009CE7 RID: 40167
		public readonly Mod ParentMod;

		// Token: 0x04009CE8 RID: 40168
		public readonly string ModUrl;

		// Token: 0x04009CE9 RID: 40169
		public readonly string ReactBundle;

		// Token: 0x04009CEA RID: 40170
		public readonly string CssPath;

		// Token: 0x04009CEB RID: 40171
		public readonly bool IsWebMod;
	}
}
