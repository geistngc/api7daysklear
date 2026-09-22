using System;
using System.IO;
using Webserver.FileCache;

namespace Webserver.UrlHandlers
{
	// Token: 0x02001AFC RID: 6908
	public class StaticHandler : AbsHandler
	{
		// Token: 0x0600CFBD RID: 53181 RVA: 0x004BCAD8 File Offset: 0x004BACD8
		public StaticHandler(string _filePath, AbstractCache _cache, bool _logMissingFiles, string _moduleName = null) : base(_moduleName, 0)
		{
			this.datapath = _filePath + ((_filePath[_filePath.Length - 1] == '/') ? "" : "/");
			this.cache = _cache;
			this.logMissingFiles = _logMissingFiles;
		}

		// Token: 0x0600CFBE RID: 53182 RVA: 0x004BCB28 File Offset: 0x004BAD28
		public override void HandleRequest(RequestContext _context)
		{
			string text = _context.RequestPath.Remove(0, this.urlBasePath.Length);
			byte[] fileContent = this.cache.GetFileContent(this.datapath + text);
			if (fileContent != null)
			{
				_context.Response.ContentType = MimeType.GetMimeType(Path.GetExtension(text));
				_context.Response.ContentLength64 = (long)fileContent.Length;
				_context.Response.OutputStream.Write(fileContent, 0, fileContent.Length);
				return;
			}
			_context.Response.StatusCode = 404;
			if (this.logMissingFiles)
			{
				Log.Warning(string.Concat(new string[]
				{
					"[Web] Static: FileNotFound: \"",
					_context.RequestPath,
					"\" @ \"",
					this.datapath,
					text,
					"\""
				}));
			}
		}

		// Token: 0x04009E12 RID: 40466
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AbstractCache cache;

		// Token: 0x04009E13 RID: 40467
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string datapath;

		// Token: 0x04009E14 RID: 40468
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool logMissingFiles;
	}
}
