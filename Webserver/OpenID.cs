using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using SpaceWizards.HttpListener;

namespace Webserver
{
	// Token: 0x02001AC1 RID: 6849
	public static class OpenID
	{
		// Token: 0x0600CE72 RID: 52850 RVA: 0x004B4EA0 File Offset: 0x004B30A0
		[PublicizedFrom(EAccessModifier.Private)]
		static OpenID()
		{
			for (int i = 0; i < Environment.GetCommandLineArgs().Length; i++)
			{
				if (Environment.GetCommandLineArgs()[i].EqualsCaseInsensitive("-debugopenid"))
				{
					OpenID.debugOpenId = true;
				}
			}
			ServicePointManager.ServerCertificateValidationCallback = delegate(object _srvPoint, X509Certificate _certificate, X509Chain _chain, SslPolicyErrors _errors)
			{
				if (_errors == SslPolicyErrors.None)
				{
					if (OpenID.verboseSsl)
					{
						Log.Out("[OpenID] Steam certificate: No error (1)");
					}
					return true;
				}
				X509Chain x509Chain = new X509Chain();
				x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
				x509Chain.ChainPolicy.ExtraStore.Add(OpenID.caCert);
				x509Chain.ChainPolicy.ExtraStore.Add(OpenID.caIntermediateCert);
				if (x509Chain.Build(new X509Certificate2(_certificate)))
				{
					x509Chain.Reset();
					if (OpenID.verboseSsl)
					{
						Log.Out("Steam certificate: No error (2)");
					}
					return true;
				}
				if (x509Chain.ChainStatus.Length == 0)
				{
					x509Chain.Reset();
					if (OpenID.verboseSsl)
					{
						Log.Out("Steam certificate: No error (3)");
					}
					return true;
				}
				foreach (X509ChainElement x509ChainElement in x509Chain.ChainElements)
				{
					if (OpenID.verboseSsl)
					{
						Log.Out("Validating cert: " + x509ChainElement.Certificate.Subject);
					}
					foreach (X509ChainStatus x509ChainStatus in x509ChainElement.ChainElementStatus)
					{
						if (OpenID.verboseSsl)
						{
							Log.Out(string.Format("   Status: {0}", x509ChainStatus.Status));
						}
						if (x509ChainStatus.Status != X509ChainStatusFlags.NoError && (x509ChainStatus.Status != X509ChainStatusFlags.UntrustedRoot || !x509ChainElement.Certificate.Equals(OpenID.caCert)))
						{
							Log.Warning(string.Format("[OpenID] Steam certificate error: {0} ### Error: {1}", x509ChainElement.Certificate.Subject, x509ChainStatus.Status));
							x509Chain.Reset();
							return false;
						}
					}
				}
				foreach (X509ChainStatus x509ChainStatus2 in x509Chain.ChainStatus)
				{
					if (x509ChainStatus2.Status != X509ChainStatusFlags.NoError && x509ChainStatus2.Status != X509ChainStatusFlags.UntrustedRoot)
					{
						Log.Warning(string.Format("[OpenID] Steam certificate error: {0}", x509ChainStatus2.Status));
						x509Chain.Reset();
						return false;
					}
				}
				x509Chain.Reset();
				if (OpenID.verboseSsl)
				{
					Log.Out("[OpenID] Steam certificate: No error (4)");
				}
				return true;
			};
		}

		// Token: 0x0600CE73 RID: 52851 RVA: 0x004B4F40 File Offset: 0x004B3140
		public static string GetOpenIdLoginUrl(string _returnHost, string _returnUrl)
		{
			Dictionary<string, string> queryParams = new Dictionary<string, string>
			{
				{
					"openid.ns",
					"http://specs.openid.net/auth/2.0"
				},
				{
					"openid.mode",
					"checkid_setup"
				},
				{
					"openid.return_to",
					_returnUrl
				},
				{
					"openid.realm",
					_returnHost
				},
				{
					"openid.identity",
					"http://specs.openid.net/auth/2.0/identifier_select"
				},
				{
					"openid.claimed_id",
					"http://specs.openid.net/auth/2.0/identifier_select"
				}
			};
			return "https://steamcommunity.com/openid/login?" + OpenID.buildUrlParams(queryParams);
		}

		// Token: 0x0600CE74 RID: 52852 RVA: 0x004B4FBC File Offset: 0x004B31BC
		public static ulong Validate(SpaceWizards.HttpListener.HttpListenerRequest _req)
		{
			string value = OpenID.getValue(_req, "openid.mode");
			if (value == "cancel")
			{
				Log.Warning("[OpenID] Steam OpenID login canceled");
				return 0UL;
			}
			if (value == "error")
			{
				Log.Warning("[OpenID] Steam OpenID login error: " + OpenID.getValue(_req, "openid.error"));
				if (OpenID.debugOpenId)
				{
					OpenID.PrintOpenIdResponse(_req);
				}
				return 0UL;
			}
			string value2 = OpenID.getValue(_req, "openid.claimed_id");
			Match match = OpenID.steamIdUrlMatcher.Match(value2);
			if (!match.Success)
			{
				Log.Warning("[OpenID] Steam OpenID login result did not give a valid SteamID");
				if (OpenID.debugOpenId)
				{
					OpenID.PrintOpenIdResponse(_req);
				}
				return 0UL;
			}
			ulong result = ulong.Parse(match.Groups[1].Value);
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"openid.ns",
					"http://specs.openid.net/auth/2.0"
				},
				{
					"openid.assoc_handle",
					OpenID.getValue(_req, "openid.assoc_handle")
				},
				{
					"openid.signed",
					OpenID.getValue(_req, "openid.signed")
				},
				{
					"openid.sig",
					OpenID.getValue(_req, "openid.sig")
				},
				{
					"openid.identity",
					"http://specs.openid.net/auth/2.0/identifier_select"
				},
				{
					"openid.claimed_id",
					"http://specs.openid.net/auth/2.0/identifier_select"
				}
			};
			foreach (string str in OpenID.getValue(_req, "openid.signed").Split(',', StringSplitOptions.None))
			{
				string text = "openid." + str;
				dictionary[text] = OpenID.getValue(_req, text);
			}
			dictionary.Add("openid.mode", "check_authentication");
			byte[] bytes = Encoding.ASCII.GetBytes(OpenID.buildUrlParams(dictionary));
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://steamcommunity.com/openid/login");
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.ContentLength = (long)bytes.Length;
			httpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en");
			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}
			string text2;
			using (Stream responseStream = ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream())
			{
				using (StreamReader streamReader = new StreamReader(responseStream))
				{
					text2 = streamReader.ReadToEnd();
				}
			}
			if (text2.ContainsCaseInsensitive("is_valid:true"))
			{
				return result;
			}
			Log.Warning("[OpenID] Steam OpenID login failed: " + text2);
			return 0UL;
		}

		// Token: 0x0600CE75 RID: 52853 RVA: 0x004B525C File Offset: 0x004B345C
		[PublicizedFrom(EAccessModifier.Private)]
		public static string buildUrlParams(Dictionary<string, string> _queryParams)
		{
			string[] array = new string[_queryParams.Count];
			int num = 0;
			foreach (KeyValuePair<string, string> keyValuePair in _queryParams)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string str = text;
				string stringToEscape = text2;
				array[num++] = str + "=" + Uri.EscapeDataString(stringToEscape);
			}
			return string.Join("&", array);
		}

		// Token: 0x0600CE76 RID: 52854 RVA: 0x004B52E8 File Offset: 0x004B34E8
		[PublicizedFrom(EAccessModifier.Private)]
		public static string getValue(SpaceWizards.HttpListener.HttpListenerRequest _req, string _name)
		{
			NameValueCollection queryString = _req.QueryString;
			if (queryString[_name] == null)
			{
				throw new MissingMemberException("[OpenID] OpenID parameter \"" + _name + "\" missing");
			}
			return queryString[_name];
		}

		// Token: 0x0600CE77 RID: 52855 RVA: 0x004B5318 File Offset: 0x004B3518
		[PublicizedFrom(EAccessModifier.Private)]
		public static void PrintOpenIdResponse(SpaceWizards.HttpListener.HttpListenerRequest _req)
		{
			NameValueCollection queryString = _req.QueryString;
			for (int i = 0; i < queryString.Count; i++)
			{
				Log.Out("   " + queryString.GetKey(i) + " = " + queryString[i]);
			}
		}

		// Token: 0x04009CAA RID: 40106
		[PublicizedFrom(EAccessModifier.Private)]
		public const string STEAM_LOGIN = "https://steamcommunity.com/openid/login";

		// Token: 0x04009CAB RID: 40107
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex steamIdUrlMatcher = new Regex("^https?:\\/\\/steamcommunity\\.com\\/openid\\/id\\/([0-9]{17,18})");

		// Token: 0x04009CAC RID: 40108
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly X509Certificate2 caCert = new X509Certificate2(GameIO.GetGameDir("Data/Web") + "/steam-rootca.cer");

		// Token: 0x04009CAD RID: 40109
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly X509Certificate2 caIntermediateCert = new X509Certificate2(GameIO.GetGameDir("Data/Web") + "/steam-intermediate.cer");

		// Token: 0x04009CAE RID: 40110
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool verboseSsl = false;

		// Token: 0x04009CAF RID: 40111
		public static bool debugOpenId;
	}
}
