using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BestHTTP.Authentication;
using BestHTTP.Caching;
using BestHTTP.Cookies;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using SocketEx;

namespace BestHTTP
{
	internal sealed class HTTPConnection : IDisposable
	{
		private enum RetryCauses
		{
			None = 0,
			Reconnect = 1,
			Authenticate = 2,
			ProxyAuthenticate = 3
		}

		private TcpClient Client;

		private Stream Stream;

		private DateTime LastProcessTime;

		internal string ServerAddress { get; private set; }

		internal HTTPConnectionStates State { get; private set; }

		internal bool IsFree
		{
			get
			{
				return State == HTTPConnectionStates.Initial || State == HTTPConnectionStates.Free;
			}
		}

		internal bool IsActive
		{
			get
			{
				return State > HTTPConnectionStates.Initial && State < HTTPConnectionStates.Free;
			}
		}

		internal HTTPRequest CurrentRequest { get; private set; }

		internal bool IsRemovable
		{
			get
			{
				return IsFree && DateTime.UtcNow - LastProcessTime > HTTPManager.MaxConnectionIdleTime;
			}
		}

		internal DateTime StartTime { get; private set; }

		internal DateTime TimedOutStart { get; private set; }

		internal HTTPProxy Proxy { get; private set; }

		internal bool HasProxy
		{
			get
			{
				return Proxy != null;
			}
		}

		internal Uri LastProcessedUri { get; private set; }

		internal HTTPConnection(string serverAddress)
		{
			ServerAddress = serverAddress;
			State = HTTPConnectionStates.Initial;
			LastProcessTime = DateTime.UtcNow;
		}

		internal void Process(HTTPRequest request)
		{
			if (State == HTTPConnectionStates.Processing)
			{
				throw new Exception("Connection already processing a request!");
			}
			StartTime = DateTime.MaxValue;
			State = HTTPConnectionStates.Processing;
			CurrentRequest = request;
			new Thread(ThreadFunc).Start();
		}

		internal void Recycle()
		{
			if (State == HTTPConnectionStates.TimedOut)
			{
				LastProcessTime = DateTime.MinValue;
			}
			State = HTTPConnectionStates.Free;
			CurrentRequest = null;
		}

		private void ThreadFunc(object param)
		{
			bool flag = false;
			bool flag2 = false;
			RetryCauses retryCauses = RetryCauses.None;
			try
			{
				if (!HasProxy && CurrentRequest.HasProxy)
				{
					Proxy = CurrentRequest.Proxy;
				}
				if (TryLoadAllFromCache())
				{
					return;
				}
				if (Client != null && !Client.IsConnected())
				{
					Close();
				}
				do
				{
					if (retryCauses == RetryCauses.Reconnect)
					{
						Close();
						Thread.Sleep(100);
					}
					LastProcessedUri = CurrentRequest.CurrentUri;
					retryCauses = RetryCauses.None;
					Connect();
					if (State == HTTPConnectionStates.AbortRequested)
					{
						throw new Exception("AbortRequested");
					}
					if (!CurrentRequest.DisableCache)
					{
						HTTPCacheService.SetHeaders(CurrentRequest);
					}
					bool flag3 = false;
					try
					{
						CurrentRequest.SendOutTo(Stream);
						flag3 = true;
					}
					catch (Exception ex)
					{
						Close();
						if (State == HTTPConnectionStates.TimedOut)
						{
							throw new Exception("AbortRequested");
						}
						if (flag || CurrentRequest.DisableRetry)
						{
							throw ex;
						}
						flag = true;
						retryCauses = RetryCauses.Reconnect;
					}
					if (!flag3)
					{
						continue;
					}
					bool flag4 = Receive();
					if (State == HTTPConnectionStates.TimedOut)
					{
						throw new Exception("AbortRequested");
					}
					if (!flag4 && !flag && !CurrentRequest.DisableRetry)
					{
						flag = true;
						retryCauses = RetryCauses.Reconnect;
					}
					if (CurrentRequest.Response == null)
					{
						continue;
					}
					switch (CurrentRequest.Response.StatusCode)
					{
					case 401:
					{
						string text2 = DigestStore.FindBest(CurrentRequest.Response.GetHeaderValues("www-authenticate"));
						if (!string.IsNullOrEmpty(text2))
						{
							Digest orCreate2 = DigestStore.GetOrCreate(CurrentRequest.CurrentUri);
							orCreate2.ParseChallange(text2);
							if (CurrentRequest.Credentials != null && orCreate2.IsUriProtected(CurrentRequest.CurrentUri) && (!CurrentRequest.HasHeader("Authorization") || orCreate2.Stale))
							{
								retryCauses = RetryCauses.Authenticate;
							}
						}
						break;
					}
					case 407:
					{
						if (!CurrentRequest.HasProxy)
						{
							break;
						}
						string text = DigestStore.FindBest(CurrentRequest.Response.GetHeaderValues("proxy-authenticate"));
						if (!string.IsNullOrEmpty(text))
						{
							Digest orCreate = DigestStore.GetOrCreate(CurrentRequest.Proxy.Address);
							orCreate.ParseChallange(text);
							if (CurrentRequest.Proxy.Credentials != null && orCreate.IsUriProtected(CurrentRequest.Proxy.Address) && (!CurrentRequest.HasHeader("Proxy-Authorization") || orCreate.Stale))
							{
								retryCauses = RetryCauses.ProxyAuthenticate;
							}
						}
						break;
					}
					case 301:
					case 302:
					case 307:
					case 308:
						if (CurrentRequest.RedirectCount < CurrentRequest.MaxRedirects)
						{
							CurrentRequest.RedirectCount++;
							string firstHeaderValue = CurrentRequest.Response.GetFirstHeaderValue("location");
							if (string.IsNullOrEmpty(firstHeaderValue))
							{
								throw new MissingFieldException(string.Format("Got redirect status({0}) without 'location' header!", CurrentRequest.Response.StatusCode.ToString()));
							}
							Uri redirectUri = GetRedirectUri(firstHeaderValue);
							if (!CurrentRequest.CallOnBeforeRedirection(redirectUri))
							{
								HTTPManager.Logger.Information("HTTPConnection", "OnBeforeRedirection returned False");
								break;
							}
							CurrentRequest.RemoveHeader("Host");
							CurrentRequest.SetHeader("Referer", CurrentRequest.CurrentUri.ToString());
							CurrentRequest.RedirectUri = redirectUri;
							CurrentRequest.Response = null;
							bool flag5 = true;
							CurrentRequest.IsRedirected = flag5;
							flag2 = flag5;
						}
						break;
					}
					if (CurrentRequest.IsCookiesEnabled)
					{
						CookieJar.Set(CurrentRequest.Response);
					}
					TryStoreInCache();
					if (CurrentRequest.Response == null || (!CurrentRequest.Response.IsClosedManually && CurrentRequest.Response.HasHeaderWithValue("connection", "close")))
					{
						Close();
					}
				}
				while (retryCauses != 0);
			}
			catch (TimeoutException exception)
			{
				CurrentRequest.Response = null;
				CurrentRequest.Exception = exception;
				CurrentRequest.State = HTTPRequestStates.ConnectionTimedOut;
				Close();
			}
			catch (Exception exception2)
			{
				if (CurrentRequest != null)
				{
					if (CurrentRequest.UseStreaming)
					{
						HTTPCacheService.DeleteEntity(CurrentRequest.CurrentUri);
					}
					CurrentRequest.Response = null;
					switch (State)
					{
					case HTTPConnectionStates.AbortRequested:
						CurrentRequest.State = HTTPRequestStates.Aborted;
						break;
					case HTTPConnectionStates.TimedOut:
						CurrentRequest.State = HTTPRequestStates.TimedOut;
						break;
					default:
						CurrentRequest.Exception = exception2;
						CurrentRequest.State = HTTPRequestStates.Error;
						break;
					}
				}
				Close();
			}
			finally
			{
				if (CurrentRequest != null)
				{
					lock (HTTPManager.Locker)
					{
						if (CurrentRequest != null && CurrentRequest.Response != null && CurrentRequest.Response.IsUpgraded)
						{
							State = HTTPConnectionStates.Upgraded;
						}
						else
						{
							State = (flag2 ? HTTPConnectionStates.Redirected : ((Client != null) ? HTTPConnectionStates.WaitForRecycle : HTTPConnectionStates.Closed));
						}
						if (CurrentRequest.State == HTTPRequestStates.Processing && (State == HTTPConnectionStates.Closed || State == HTTPConnectionStates.WaitForRecycle))
						{
							if (CurrentRequest.Response != null)
							{
								CurrentRequest.State = HTTPRequestStates.Finished;
							}
							else
							{
								CurrentRequest.State = HTTPRequestStates.Error;
							}
						}
						if (CurrentRequest.State == HTTPRequestStates.ConnectionTimedOut)
						{
							State = HTTPConnectionStates.Closed;
						}
						LastProcessTime = DateTime.UtcNow;
					}
					HTTPCacheService.SaveLibrary();
					CookieJar.Persist();
				}
			}
		}

		private void Connect()
		{
			Uri uri = ((!CurrentRequest.HasProxy) ? CurrentRequest.CurrentUri : CurrentRequest.Proxy.Address);
			if (Client == null)
			{
				Client = new TcpClient();
			}
			if (!Client.Connected)
			{
				Client.ConnectTimeout = CurrentRequest.ConnectTimeout;
				Client.Connect(uri.Host, uri.Port);
				if (HTTPManager.Logger.Level <= Loglevels.Information)
				{
					HTTPManager.Logger.Information("HTTPConnection", "Connected to " + uri.Host + ":" + uri.Port);
				}
			}
			else if (HTTPManager.Logger.Level <= Loglevels.Information)
			{
				HTTPManager.Logger.Information("HTTPConnection", "Already connected to " + uri.Host + ":" + uri.Port);
			}
			lock (HTTPManager.Locker)
			{
				StartTime = DateTime.UtcNow;
			}
			if (Stream != null)
			{
				return;
			}
			bool flag = HTTPProtocolFactory.IsSecureProtocol(CurrentRequest.CurrentUri);
			if (HasProxy && (!Proxy.IsTransparent || (flag && Proxy.NonTransparentForHTTPS)))
			{
				Stream = Client.GetStream();
				BinaryWriter binaryWriter = new BinaryWriter(Stream);
				bool flag2;
				do
				{
					flag2 = false;
					binaryWriter.SendAsASCII(string.Format("CONNECT {0}:{1} HTTP/1.1", CurrentRequest.CurrentUri.Host, CurrentRequest.CurrentUri.Port));
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII("Proxy-Connection: Keep-Alive");
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII("Connection: Keep-Alive");
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII(string.Format("Host: {0}:{1}", CurrentRequest.CurrentUri.Host, CurrentRequest.CurrentUri.Port));
					binaryWriter.Write(HTTPRequest.EOL);
					if (HasProxy && Proxy.Credentials != null)
					{
						switch (Proxy.Credentials.Type)
						{
						case AuthenticationTypes.Basic:
							binaryWriter.Write(string.Format("Proxy-Authorization: {0}", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Proxy.Credentials.UserName + ":" + Proxy.Credentials.Password))).GetASCIIBytes());
							binaryWriter.Write(HTTPRequest.EOL);
							break;
						case AuthenticationTypes.Unknown:
						case AuthenticationTypes.Digest:
						{
							Digest digest = DigestStore.Get(Proxy.Address);
							if (digest != null)
							{
								string text = digest.GenerateResponseHeader(CurrentRequest, Proxy.Credentials);
								if (!string.IsNullOrEmpty(text))
								{
									binaryWriter.Write(string.Format("Proxy-Authorization: {0}", text).GetASCIIBytes());
									binaryWriter.Write(HTTPRequest.EOL);
								}
							}
							break;
						}
						}
					}
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Flush();
					CurrentRequest.ProxyResponse = new HTTPResponse(CurrentRequest, Stream, false, false);
					if (!CurrentRequest.ProxyResponse.Receive(-1, true))
					{
						throw new Exception("Connection to the Proxy Server failed!");
					}
					if (HTTPManager.Logger.Level <= Loglevels.Information)
					{
						HTTPManager.Logger.Information("HTTPConnection", "Proxy returned - status code: " + CurrentRequest.ProxyResponse.StatusCode + " message: " + CurrentRequest.ProxyResponse.Message);
					}
					int statusCode = CurrentRequest.ProxyResponse.StatusCode;
					if (statusCode == 407)
					{
						string text2 = DigestStore.FindBest(CurrentRequest.ProxyResponse.GetHeaderValues("proxy-authenticate"));
						if (!string.IsNullOrEmpty(text2))
						{
							Digest orCreate = DigestStore.GetOrCreate(Proxy.Address);
							orCreate.ParseChallange(text2);
							if (Proxy.Credentials != null && orCreate.IsUriProtected(Proxy.Address) && (!CurrentRequest.HasHeader("Proxy-Authorization") || orCreate.Stale))
							{
								flag2 = true;
							}
						}
					}
					else if (!CurrentRequest.ProxyResponse.IsSuccess)
					{
						throw new Exception(string.Format("Proxy returned Status Code: \"{0}\", Message: \"{1}\" and Response: {2}", CurrentRequest.ProxyResponse.StatusCode, CurrentRequest.ProxyResponse.Message, CurrentRequest.ProxyResponse.DataAsText));
					}
				}
				while (flag2);
			}
			if (flag)
			{
				if (CurrentRequest.UseAlternateSSL)
				{
					TlsClientProtocol tlsClientProtocol = new TlsClientProtocol(Client.GetStream(), new SecureRandom());
					List<string> list = new List<string>(1);
					list.Add(CurrentRequest.CurrentUri.Host);
					Uri currentUri = CurrentRequest.CurrentUri;
					ICertificateVerifyer verifyer;
					if (CurrentRequest.CustomCertificateVerifyer == null)
					{
						ICertificateVerifyer certificateVerifyer = new AlwaysValidVerifyer();
						verifyer = certificateVerifyer;
					}
					else
					{
						verifyer = CurrentRequest.CustomCertificateVerifyer;
					}
					tlsClientProtocol.Connect(new LegacyTlsClient(currentUri, verifyer, null, list));
					Stream = tlsClientProtocol.Stream;
				}
				else
				{
					SslStream sslStream = new SslStream(Client.GetStream(), false, (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors) => CurrentRequest.CallCustomCertificationValidator(cert, chain));
					if (!sslStream.IsAuthenticated)
					{
						sslStream.AuthenticateAsClient(CurrentRequest.CurrentUri.Host);
					}
					Stream = sslStream;
				}
			}
			else
			{
				Stream = Client.GetStream();
			}
		}

		private bool Receive()
		{
			SupportedProtocols protocol = ((CurrentRequest.ProtocolHandler != 0) ? CurrentRequest.ProtocolHandler : HTTPProtocolFactory.GetProtocolFromUri(CurrentRequest.CurrentUri));
			CurrentRequest.Response = HTTPProtocolFactory.Get(protocol, CurrentRequest, Stream, CurrentRequest.UseStreaming, false);
			if (!CurrentRequest.Response.Receive(-1, true))
			{
				CurrentRequest.Response = null;
				return false;
			}
			if (CurrentRequest.Response.StatusCode == 304)
			{
				int length;
				using (Stream stream = HTTPCacheService.GetBody(CurrentRequest.CurrentUri, out length))
				{
					if (!CurrentRequest.Response.HasHeader("content-length"))
					{
						CurrentRequest.Response.Headers.Add("content-length", new List<string>(1) { length.ToString() });
					}
					CurrentRequest.Response.IsFromCache = true;
					CurrentRequest.Response.ReadRaw(stream, length);
				}
			}
			return true;
		}

		private bool TryLoadAllFromCache()
		{
			if (CurrentRequest.DisableCache || !HTTPCacheService.IsSupported)
			{
				return false;
			}
			try
			{
				if (HTTPCacheService.IsCachedEntityExpiresInTheFuture(CurrentRequest))
				{
					CurrentRequest.Response = HTTPCacheService.GetFullResponse(CurrentRequest);
					if (CurrentRequest.Response != null)
					{
						return true;
					}
				}
			}
			catch
			{
				HTTPCacheService.DeleteEntity(CurrentRequest.CurrentUri);
			}
			return false;
		}

		private void TryStoreInCache()
		{
			if (!CurrentRequest.UseStreaming && !CurrentRequest.DisableCache && CurrentRequest.Response != null && HTTPCacheService.IsSupported && HTTPCacheService.IsCacheble(CurrentRequest.CurrentUri, CurrentRequest.MethodType, CurrentRequest.Response))
			{
				HTTPCacheService.Store(CurrentRequest.CurrentUri, CurrentRequest.MethodType, CurrentRequest.Response);
			}
		}

		private Uri GetRedirectUri(string location)
		{
			Uri uri = null;
			try
			{
				return new Uri(location);
			}
			catch (UriFormatException)
			{
				Uri uri2 = CurrentRequest.Uri;
				UriBuilder uriBuilder = new UriBuilder(uri2.Scheme, uri2.Host, uri2.Port, location);
				return uriBuilder.Uri;
			}
		}

		internal void HandleProgressCallback()
		{
			if (CurrentRequest.OnProgress != null && CurrentRequest.DownloadProgressChanged)
			{
				try
				{
					CurrentRequest.OnProgress(CurrentRequest, CurrentRequest.Downloaded, CurrentRequest.DownloadLength);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("HTTPManager", "HandleProgressCallback - OnProgress", ex);
				}
				CurrentRequest.DownloadProgressChanged = false;
			}
			if (CurrentRequest.OnUploadProgress != null && CurrentRequest.UploadProgressChanged)
			{
				try
				{
					CurrentRequest.OnUploadProgress(CurrentRequest, CurrentRequest.Uploaded, CurrentRequest.UploadLength);
				}
				catch (Exception ex2)
				{
					HTTPManager.Logger.Exception("HTTPManager", "HandleProgressCallback - OnUploadProgress", ex2);
				}
				CurrentRequest.UploadProgressChanged = false;
			}
		}

		internal void HandleCallback()
		{
			try
			{
				HandleProgressCallback();
				if (State == HTTPConnectionStates.Upgraded)
				{
					if (CurrentRequest != null && CurrentRequest.Response != null && CurrentRequest.Response.IsUpgraded)
					{
						CurrentRequest.UpgradeCallback();
					}
					State = HTTPConnectionStates.WaitForProtocolShutdown;
				}
				else
				{
					CurrentRequest.CallCallback();
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPManager", "HandleCallback", ex);
			}
		}

		internal void Abort(HTTPConnectionStates newState)
		{
			State = newState;
			HTTPConnectionStates state = State;
			if (state == HTTPConnectionStates.TimedOut)
			{
				TimedOutStart = DateTime.UtcNow;
			}
			if (Stream != null)
			{
				Stream.Dispose();
			}
		}

		private void Close()
		{
			LastProcessedUri = null;
			if (Client == null)
			{
				return;
			}
			try
			{
				Client.Close();
			}
			catch
			{
			}
			finally
			{
				Stream = null;
				Client = null;
			}
		}

		public void Dispose()
		{
			Close();
		}
	}
}
