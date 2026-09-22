using System;
using System.Collections.Specialized;
using SpaceWizards.HttpListener;

namespace Webserver
{
	// Token: 0x02001AC3 RID: 6851
	public class RequestContext
	{
		// Token: 0x17001972 RID: 6514
		// (get) Token: 0x0600CE7B RID: 52859 RVA: 0x004B556C File Offset: 0x004B376C
		public NameValueCollection QueryParameters
		{
			get
			{
				NameValueCollection result;
				if ((result = this.queryParameters) == null)
				{
					result = (this.queryParameters = this.Request.QueryString);
				}
				return result;
			}
		}

		// Token: 0x0600CE7C RID: 52860 RVA: 0x004B5598 File Offset: 0x004B3798
		public RequestContext(string _requestPath, HttpListenerRequest _request, HttpListenerResponse _response, WebConnection _connection, int _permissionLevel)
		{
			this.RequestPath = _requestPath;
			this.Request = _request;
			this.Response = _response;
			this.Connection = _connection;
			this.PermissionLevel = _permissionLevel;
			string httpMethod = _request.HttpMethod;
			ERequestMethod method;
			if (!(httpMethod == "GET"))
			{
				if (!(httpMethod == "PUT"))
				{
					if (!(httpMethod == "POST"))
					{
						if (!(httpMethod == "DELETE"))
						{
							if (!(httpMethod == "HEAD"))
							{
								if (!(httpMethod == "OPTIONS"))
								{
									method = ERequestMethod.Other;
								}
								else
								{
									method = ERequestMethod.OPTIONS;
								}
							}
							else
							{
								method = ERequestMethod.HEAD;
							}
						}
						else
						{
							method = ERequestMethod.DELETE;
						}
					}
					else
					{
						method = ERequestMethod.POST;
					}
				}
				else
				{
					method = ERequestMethod.PUT;
				}
			}
			else
			{
				method = ERequestMethod.GET;
			}
			this.Method = method;
		}

		// Token: 0x04009CB1 RID: 40113
		public string RequestPath;

		// Token: 0x04009CB2 RID: 40114
		public readonly ERequestMethod Method;

		// Token: 0x04009CB3 RID: 40115
		public readonly HttpListenerRequest Request;

		// Token: 0x04009CB4 RID: 40116
		[PublicizedFrom(EAccessModifier.Private)]
		public NameValueCollection queryParameters;

		// Token: 0x04009CB5 RID: 40117
		public readonly HttpListenerResponse Response;

		// Token: 0x04009CB6 RID: 40118
		public readonly WebConnection Connection;

		// Token: 0x04009CB7 RID: 40119
		public readonly int PermissionLevel;
	}
}
