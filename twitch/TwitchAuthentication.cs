using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using ZXing;
using ZXing.QrCode;

namespace Twitch
{
	// Token: 0x02001804 RID: 6148
	public class TwitchAuthentication
	{
		// Token: 0x1400010B RID: 267
		// (add) Token: 0x0600BE41 RID: 48705 RVA: 0x004689F4 File Offset: 0x00466BF4
		// (remove) Token: 0x0600BE42 RID: 48706 RVA: 0x00468A28 File Offset: 0x00466C28
		public static event TwitchAuth_QRCodeGenerated QRCodeGenerated;

		// Token: 0x0600BE43 RID: 48707 RVA: 0x00468A5C File Offset: 0x00466C5C
		public TwitchAuthentication()
		{
			this.TwitchListener = new HttpListener();
			this.TwitchListener.Prefixes.Add("http://localhost:56207/");
			this.TwitchListener.Prefixes.Add("http://localhost:56207/auth/");
		}

		// Token: 0x0600BE44 RID: 48708 RVA: 0x00468ADB File Offset: 0x00466CDB
		public Task<TwitchAuthentication.AuthenticationValues> GetAuthenticationValuesAsync()
		{
			return Task.Run<TwitchAuthentication.AuthenticationValues>(() => this.GetAuthenticationValues());
		}

		// Token: 0x0600BE45 RID: 48709 RVA: 0x00468AF0 File Offset: 0x00466CF0
		[PublicizedFrom(EAccessModifier.Private)]
		public bool StartListener()
		{
			this.completed = false;
			try
			{
				this.TwitchListener.Start();
			}
			catch (HttpListenerException)
			{
			}
			return this.TwitchListener.IsListening;
		}

		// Token: 0x0600BE46 RID: 48710 RVA: 0x00468B30 File Offset: 0x00466D30
		public void StopListener()
		{
			this.TwitchListener.Stop();
		}

		// Token: 0x0600BE47 RID: 48711 RVA: 0x00468B40 File Offset: 0x00466D40
		public TwitchAuthentication.AuthenticationValues GetAuthenticationValues()
		{
			while (this.TwitchListener.IsListening)
			{
				HttpListenerContext httpListenerContext = null;
				try
				{
					httpListenerContext = this.TwitchListener.GetContext();
				}
				catch (HttpListenerException)
				{
					this.StopListener();
					Log.Warning("Could not get context on TwitchListener");
					return null;
				}
				if (!(httpListenerContext.Request.HttpMethod == "GET") || !(httpListenerContext.Request.Url.LocalPath == "/auth/"))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(this.GetResponse());
					httpListenerContext.Response.StatusCode = 200;
					httpListenerContext.Response.KeepAlive = false;
					httpListenerContext.Response.ContentLength64 = (long)bytes.Length;
					httpListenerContext.Response.OutputStream.Write(bytes, 0, bytes.Length);
					httpListenerContext.Response.Close();
					continue;
				}
				string text = httpListenerContext.Request.QueryString.Get("scope");
				string text2 = httpListenerContext.Request.QueryString.Get("access_token");
				if (text == null || text2 == null)
				{
					httpListenerContext.Response.ContentType = "application/json";
					httpListenerContext.Response.StatusCode = 400;
					string s = JsonConvert.SerializeObject(new
					{
						message = "missing data"
					});
					byte[] bytes2 = Encoding.UTF8.GetBytes(s);
					httpListenerContext.Response.ContentLength64 = (long)bytes2.Length;
					Stream outputStream = httpListenerContext.Response.OutputStream;
					outputStream.Write(bytes2, 0, bytes2.Length);
					outputStream.Close();
					continue;
				}
				TwitchAuthentication.AuthenticationValues model = this.GetModel(text2, text);
				httpListenerContext.Response.ContentType = "application/json";
				httpListenerContext.Response.StatusCode = 200;
				string s2 = JsonConvert.SerializeObject(new
				{
					message = "success"
				});
				byte[] bytes3 = Encoding.UTF8.GetBytes(s2);
				httpListenerContext.Response.ContentLength64 = (long)bytes3.Length;
				Stream outputStream2 = httpListenerContext.Response.OutputStream;
				outputStream2.Write(bytes3, 0, bytes3.Length);
				outputStream2.Close();
				this.StopListener();
				if (model != null)
				{
					return model;
				}
				Log.Warning("did not successfully get authentication values");
				return null;
			}
			return null;
		}

		// Token: 0x0600BE48 RID: 48712 RVA: 0x00468D6C File Offset: 0x00466F6C
		public static NameValueCollection ParseQueryString(string s)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (s.Contains("?"))
			{
				s = s.Substring(s.IndexOf('?') + 2);
			}
			string[] array = Regex.Split(s, "&");
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = Regex.Split(array[i], "=");
				if (array2.Length == 2)
				{
					nameValueCollection.Add(array2[0], array2[1]);
				}
				else
				{
					nameValueCollection.Add(array2[0], string.Empty);
				}
			}
			return nameValueCollection;
		}

		// Token: 0x0600BE49 RID: 48713 RVA: 0x00468DEC File Offset: 0x00466FEC
		[PublicizedFrom(EAccessModifier.Private)]
		public string GetResponse()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<head>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("<title>7 Days to Die Twitch Oauth</title>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("</head>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("<body>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("<h1 id=\"status\">Authenticating...</h1>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("</body>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("<script language=\"JavaScript\">");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("if(window.location.hash) {");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("window.location.href = window.location.href.replace(\"/#\",\"?\");");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("}");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("const urlParams = new URLSearchParams(window.location.search);");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("const accessToken = urlParams.get('access_token');");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("const scope = urlParams.get('scope');");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("if (accessToken && scope) {");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("const url = `http://localhost:56207/auth/?access_token=${accessToken}&scope=${scope}`;");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("let attemptCount = 0;");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("const sendAuthValues = () => fetch(url, { method: 'GET' })");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(".then(response => { if (response.ok) return response.json(); throw new Error('Network response was not ok.'); })");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(".then(() => { document.getElementById('status').innerText = 'Success! You may now close this window.'; setTimeout(function() { window.close(); }, 2000);})");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(".catch(() => { attemptCount++; if (attemptCount < 3) {setTimeout(sendAuthValues, 1000); return; } else { document.getElementById('status').innerText = 'authentication failed. Please try again.'; }});");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("sendAuthValues();");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("}");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("else {");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("document.getElementById('status').innerText = 'Authentication failed. Please try again.';");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("setTimeout(function () { window.close(); }, 2000);");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("}");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("</script>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("</html>");
			return stringBuilder.ToString();
		}

		// Token: 0x0600BE4A RID: 48714 RVA: 0x00469097 File Offset: 0x00467297
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchAuthentication.AuthenticationValues GetModel(string token, string scopes)
		{
			return new TwitchAuthentication.AuthenticationValues
			{
				Token = token,
				Scopes = scopes
			};
		}

		// Token: 0x0600BE4B RID: 48715 RVA: 0x004690AC File Offset: 0x004672AC
		public void SendRequestToBrowser(string ClientID)
		{
			string url = TwitchAuthentication.GetUrl(ClientID);
			new Uri(url);
			Process.Start(url);
		}

		// Token: 0x0600BE4C RID: 48716 RVA: 0x004690C4 File Offset: 0x004672C4
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetUrl(string ClientID)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("https://id.twitch.tv/oauth2/authorize");
			stringBuilder.Append("?response_type=token");
			stringBuilder.Append("&client_id=").Append(ClientID);
			stringBuilder.Append("&redirect_uri=").Append("http://localhost:56207");
			if (!TwitchAuthentication.bFirstLogin)
			{
				stringBuilder.Append("&force_verify=true");
			}
			stringBuilder.Append("&scope=channel_read+chat:read+chat:edit+user_blocks_read+channel_subscriptions+channel_check_subscription+channel:read:redemptions+channel:manage:redemptions+user_read+bits:read+channel:read:hype_train+channel:read:goals+user:read:broadcast");
			return stringBuilder.ToString();
		}

		// Token: 0x0600BE4D RID: 48717 RVA: 0x0046913C File Offset: 0x0046733C
		public void GetToken()
		{
			GameManager.Instance.StartCoroutine(this.DeviceAuthenticate());
		}

		// Token: 0x0600BE4E RID: 48718 RVA: 0x0046914F File Offset: 0x0046734F
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator DeviceAuthenticate()
		{
			Log.Out("packing data for device auth init request...");
			WWWForm wwwform = new WWWForm();
			wwwform.AddField("client_id", TwitchAuthentication.client_id);
			wwwform.AddField("scopes", "channel_read chat:read chat:edit user_blocks_read channel_subscriptions channel_check_subscription channel:read:redemptions channel:manage:redemptions user_read bits:read channel:read:hype_train channel:read:goals user:read:broadcast");
			string device_code = string.Empty;
			string text = string.Empty;
			string userCode = string.Empty;
			Log.Out("creating init request...");
			using (UnityWebRequest req = UnityWebRequest.Post("https://id.twitch.tv/oauth2/device", wwwform))
			{
				Log.Out("sending init request...");
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning("failed to initiate DCF Twitch OAuth endpoint with response: " + req.downloadHandler.text);
					this.completed = true;
					yield break;
				}
				TwitchAuthentication.DeviceAuthIntitationData deviceAuthIntitationData = JsonUtility.FromJson<TwitchAuthentication.DeviceAuthIntitationData>(req.downloadHandler.text);
				device_code = deviceAuthIntitationData.device_code;
				userCode = deviceAuthIntitationData.user_code;
				text = deviceAuthIntitationData.verification_uri;
			}
			UnityWebRequest req = null;
			Log.Out("successfully initialized auth!");
			Log.Out("generating qr code...");
			BarcodeWriter barcodeWriter = new BarcodeWriter
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Height = 256,
					Width = 256,
					Margin = 1
				}
			};
			Texture2D texture2D = new Texture2D(256, 256);
			texture2D.SetPixels32(barcodeWriter.Write(text));
			texture2D.Apply();
			Log.Out("successfully generated qr code!");
			if (TwitchAuthentication.QRCodeGenerated != null)
			{
				TwitchAuthentication.QRCodeGenerated(texture2D, userCode, text);
				Log.Out("successfully displayed qr code!");
				Application.OpenURL(text);
			}
			else
			{
				UnityEngine.Debug.LogWarning("No one listening for QR Code.");
			}
			bool keepPolling = true;
			bool retrievedToken = false;
			string token = string.Empty;
			Log.Out("initiating polling loop...");
			int pollLimit = 12;
			int pollCount = 0;
			while (keepPolling && pollCount < pollLimit)
			{
				Log.Out("packing data for token poll request");
				WWWForm pollData = new WWWForm();
				pollData.AddField("client_id", TwitchAuthentication.client_id);
				pollData.AddField("scopes", "channel_read chat:read chat:edit user_blocks_read channel_subscriptions channel_check_subscription channel:read:redemptions channel:manage:redemptions user_read bits:read channel:read:hype_train channel:read:goals user:read:broadcast");
				pollData.AddField("device_code", device_code);
				pollData.AddField("grant_type", "urn:ietf:params:oauth:grant-type:device_code");
				Log.Out("will poll token endpoint in 5s...");
				yield return new WaitForSecondsRealtime(5f);
				Log.Out("creating token poll request...");
				using (UnityWebRequest req = UnityWebRequest.Post("https://id.twitch.tv/oauth2/token", pollData))
				{
					Log.Out("sending token poll request");
					yield return req.SendWebRequest();
					if (req.result != UnityWebRequest.Result.Success)
					{
						TwitchAuthentication.PollingIncompleteResponse pollingIncompleteResponse = JsonUtility.FromJson<TwitchAuthentication.PollingIncompleteResponse>(req.downloadHandler.text);
						if (pollingIncompleteResponse.message != "authorization_pending")
						{
							Log.Warning("Failed to get device token with message: " + pollingIncompleteResponse.message);
							keepPolling = false;
						}
					}
					else
					{
						TwitchAuthentication.PollingSuccessResponse pollingSuccessResponse = JsonUtility.FromJson<TwitchAuthentication.PollingSuccessResponse>(req.downloadHandler.text);
						token = pollingSuccessResponse.access_token;
						this.oauth = "oauth:" + pollingSuccessResponse.access_token;
						this.refresh_token = pollingSuccessResponse.refresh_token;
						GameManager.Instance.StartCoroutine(this.RefreshToken((float)(pollingSuccessResponse.expires_in - 60), false));
						Log.Out("Successfully retrieved oauth token");
						retrievedToken = true;
						keepPolling = false;
					}
				}
				req = null;
				int num = pollCount;
				pollCount = num + 1;
				pollData = null;
			}
			if (!retrievedToken)
			{
				this.completed = true;
				yield break;
			}
			yield return this.ValidateToken(token);
			this.completed = true;
			Log.Out("authentication values have been received and validated!");
			yield break;
			yield break;
		}

		// Token: 0x0600BE4F RID: 48719 RVA: 0x0046915E File Offset: 0x0046735E
		public IEnumerator RefreshToken(float delay, bool skipNextRefresh = false)
		{
			yield return new WaitForSecondsRealtime(delay);
			Log.Out("invoking token refresh");
			this.oauth = "";
			string postData = JsonConvert.SerializeObject(new
			{
				refreshToken = this.refresh_token
			});
			TwitchManager.Current.WaitForOAuth();
			using (UnityWebRequest req = UnityWebRequest.Post("https://hijg23d34qghpdonxdf6ytq63m0gezwl.lambda-url.us-east-1.on.aws/", postData, "application/json"))
			{
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning("failed to refresh token: " + req.downloadHandler.text);
				}
				else
				{
					TwitchAuthentication.PollingSuccessResponse pollingSuccessResponse = JsonUtility.FromJson<TwitchAuthentication.PollingSuccessResponse>(req.downloadHandler.text);
					this.oauth = "oauth:" + pollingSuccessResponse.access_token;
					this.refresh_token = pollingSuccessResponse.refresh_token;
					Log.Out("Successfully refreshed oauth token");
					if (!skipNextRefresh)
					{
						GameManager.Instance.StartCoroutine(this.RefreshToken((float)(pollingSuccessResponse.expires_in - 60), false));
					}
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600BE50 RID: 48720 RVA: 0x0046917B File Offset: 0x0046737B
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator Authenticate()
		{
			this.StartListener();
			if (!this.TwitchListener.IsListening)
			{
				Log.Warning("Twitch listener is not listening after start attempt");
				TwitchManager.Current.StopTwitchIntegration(TwitchManager.InitStates.Failed);
				yield break;
			}
			Task<TwitchAuthentication.AuthenticationValues> t = this.GetAuthenticationValuesAsync();
			yield return new WaitUntil(() => t.Status == TaskStatus.Running);
			this.SendRequestToBrowser(TwitchAuthentication.client_id);
			yield return new WaitUntil(() => t.IsCompletedSuccessfully || t.IsCompleted || t.IsCanceled || t.IsFaulted);
			TwitchAuthentication.AuthenticationValues result = t.Result;
			if (result == null)
			{
				Log.Warning("did not successfully retrieve auth values");
				TwitchManager.Current.StopTwitchIntegration(TwitchManager.InitStates.Failed);
				yield break;
			}
			this.oauth = "oauth:" + result.Token;
			yield return this.ValidateToken(result.Token);
			this.completed = true;
			Log.Out("authentication values have been received and validated!");
			yield break;
		}

		// Token: 0x0600BE51 RID: 48721 RVA: 0x0046918A File Offset: 0x0046738A
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator ValidateToken(string token)
		{
			Log.Out("creating token validation request");
			using (UnityWebRequest req = UnityWebRequest.Get("https://id.twitch.tv/oauth2/validate"))
			{
				req.SetRequestHeader("Authorization", "Bearer " + token);
				Log.Out("sending token validation request");
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning("failed to validate on Twitch oauth endpoint with response: " + req.downloadHandler.text);
				}
				else
				{
					try
					{
						Log.Out("token validated successfully!");
						TwitchAuthentication.ValidationResponseData validationResponseData = JsonUtility.FromJson<TwitchAuthentication.ValidationResponseData>(req.downloadHandler.text);
						this.userName = validationResponseData.login;
						this.userID = validationResponseData.user_id;
					}
					catch (Exception)
					{
						Log.Warning("failed to make use of Twitch validate response data: " + req.downloadHandler.text);
					}
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600BE52 RID: 48722 RVA: 0x004691A0 File Offset: 0x004673A0
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator RunGetAuthenticationValues(Task<TwitchAuthentication.AuthenticationValues> task)
		{
			if (task != null)
			{
				while (!task.IsCompleted)
				{
					yield return null;
				}
				if (task.IsFaulted)
				{
					throw task.Exception;
				}
				if (task.Result != null)
				{
					this.oauth = "oauth:" + task.Result.Token;
					using (UnityWebRequest req = UnityWebRequest.Get("https://id.twitch.tv/oauth2/validate"))
					{
						string token = task.Result.Token;
						req.SetRequestHeader("Authorization", "Bearer " + token);
						yield return req.SendWebRequest();
						if (req.result != UnityWebRequest.Result.Success)
						{
							Log.Warning("failed to validate on Twitch oauth endpoint with response: " + req.downloadHandler.text);
						}
						else
						{
							try
							{
								TwitchAuthentication.ValidationResponseData validationResponseData = JsonUtility.FromJson<TwitchAuthentication.ValidationResponseData>(req.downloadHandler.text);
								this.userName = validationResponseData.login;
								this.userID = validationResponseData.user_id;
							}
							catch (Exception)
							{
								Log.Warning("failed to make use of Twitch validate response data: " + req.downloadHandler.text);
							}
						}
					}
					UnityWebRequest req = null;
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x04008F75 RID: 36725
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HttpListener TwitchListener;

		// Token: 0x04008F76 RID: 36726
		[PublicizedFrom(EAccessModifier.Private)]
		public const string ReturnUrl = "http://localhost:56207";

		// Token: 0x04008F77 RID: 36727
		public string oauth = "";

		// Token: 0x04008F78 RID: 36728
		public string oauthwithHeader = "";

		// Token: 0x04008F79 RID: 36729
		public string userName = "";

		// Token: 0x04008F7A RID: 36730
		public string userID = "";

		// Token: 0x04008F7B RID: 36731
		[PublicizedFrom(EAccessModifier.Private)]
		public string refresh_token = "";

		// Token: 0x04008F7C RID: 36732
		public static bool bFirstLogin = true;

		// Token: 0x04008F7D RID: 36733
		public bool completed;

		// Token: 0x04008F7F RID: 36735
		public static string client_id = "qtex0oyygtptfirybmxqxm27cdcdpf";

		// Token: 0x02001805 RID: 6149
		public class AuthenticationValues
		{
			// Token: 0x04008F80 RID: 36736
			public string Token;

			// Token: 0x04008F81 RID: 36737
			public string Scopes;
		}

		// Token: 0x02001806 RID: 6150
		[PublicizedFrom(EAccessModifier.Private)]
		[Serializable]
		public class ValidationResponseData
		{
			// Token: 0x04008F82 RID: 36738
			public string login;

			// Token: 0x04008F83 RID: 36739
			public string user_id;
		}

		// Token: 0x02001807 RID: 6151
		[PublicizedFrom(EAccessModifier.Private)]
		[Serializable]
		public class DeviceAuthIntitationData
		{
			// Token: 0x04008F84 RID: 36740
			public string device_code;

			// Token: 0x04008F85 RID: 36741
			public int expires_in;

			// Token: 0x04008F86 RID: 36742
			public int interval;

			// Token: 0x04008F87 RID: 36743
			public string user_code;

			// Token: 0x04008F88 RID: 36744
			public string verification_uri;
		}

		// Token: 0x02001808 RID: 6152
		[PublicizedFrom(EAccessModifier.Private)]
		[Serializable]
		public class PollingIncompleteResponse
		{
			// Token: 0x04008F89 RID: 36745
			public string message;
		}

		// Token: 0x02001809 RID: 6153
		[PublicizedFrom(EAccessModifier.Private)]
		[Serializable]
		public class PollingSuccessResponse
		{
			// Token: 0x04008F8A RID: 36746
			public string access_token;

			// Token: 0x04008F8B RID: 36747
			public int expires_in;

			// Token: 0x04008F8C RID: 36748
			public string refresh_token;
		}
	}
}
