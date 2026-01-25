using System;
using System.Collections.Generic;
using BestHTTP.Caching;
using BestHTTP.Cookies;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.Statistics;
using Org.BouncyCastle.Crypto.Tls;
using UnityEngine;

namespace BestHTTP
{
	public static class HTTPManager
	{
		private static byte maxConnectionPerServer;

		private static HeartbeatManager heartbeats;

		private static BestHTTP.Logger.ILogger logger;

		private static Dictionary<string, List<HTTPConnection>> Connections;

		private static List<HTTPConnection> ActiveConnections;

		private static List<HTTPConnection> FreeConnections;

		private static List<HTTPConnection> RecycledConnections;

		private static List<HTTPRequest> RequestQueue;

		private static bool IsCallingCallbacks;

		internal static object Locker;

		public static byte MaxConnectionPerServer
		{
			get
			{
				return maxConnectionPerServer;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MaxConnectionPerServer must be greater than 0!");
				}
				maxConnectionPerServer = value;
			}
		}

		public static bool KeepAliveDefaultValue { get; set; }

		public static bool IsCachingDisabled { get; set; }

		public static TimeSpan MaxConnectionIdleTime { get; set; }

		public static bool IsCookiesEnabled { get; set; }

		public static uint CookieJarSize { get; set; }

		public static bool EnablePrivateBrowsing { get; set; }

		public static TimeSpan ConnectTimeout { get; set; }

		public static TimeSpan RequestTimeout { get; set; }

		public static Func<string> RootCacheFolderProvider { get; set; }

		public static HTTPProxy Proxy { get; set; }

		public static HeartbeatManager Heartbeats
		{
			get
			{
				if (heartbeats == null)
				{
					heartbeats = new HeartbeatManager();
				}
				return heartbeats;
			}
		}

		public static BestHTTP.Logger.ILogger Logger
		{
			get
			{
				if (logger == null)
				{
					logger = new DefaultLogger();
					logger.Level = Loglevels.None;
				}
				return logger;
			}
			set
			{
				logger = value;
			}
		}

		public static ICertificateVerifyer DefaultCertificateVerifyer { get; set; }

		public static bool UseAlternateSSLDefaultValue { get; set; }

		internal static int MaxPathLength { get; set; }

		static HTTPManager()
		{
			Connections = new Dictionary<string, List<HTTPConnection>>();
			ActiveConnections = new List<HTTPConnection>();
			FreeConnections = new List<HTTPConnection>();
			RecycledConnections = new List<HTTPConnection>();
			RequestQueue = new List<HTTPRequest>();
			Locker = new object();
			MaxConnectionPerServer = 4;
			KeepAliveDefaultValue = true;
			MaxPathLength = 255;
			MaxConnectionIdleTime = TimeSpan.FromSeconds(30.0);
			IsCookiesEnabled = true;
			CookieJarSize = 10485760u;
			EnablePrivateBrowsing = false;
			ConnectTimeout = TimeSpan.FromSeconds(20.0);
			RequestTimeout = TimeSpan.FromSeconds(60.0);
			logger = new DefaultLogger();
			DefaultCertificateVerifyer = null;
			UseAlternateSSLDefaultValue = false;
		}

		public static void Setup()
		{
			HTTPUpdateDelegator.CheckInstance();
			HTTPCacheService.CheckSetup();
			CookieJar.SetupFolder();
		}

		public static HTTPRequest SendRequest(string url, OnRequestFinishedDelegate callback)
		{
			return SendRequest(new HTTPRequest(new Uri(url), HTTPMethods.Get, callback));
		}

		public static HTTPRequest SendRequest(string url, HTTPMethods methodType, OnRequestFinishedDelegate callback)
		{
			return SendRequest(new HTTPRequest(new Uri(url), methodType, callback));
		}

		public static HTTPRequest SendRequest(string url, HTTPMethods methodType, bool isKeepAlive, OnRequestFinishedDelegate callback)
		{
			return SendRequest(new HTTPRequest(new Uri(url), methodType, isKeepAlive, callback));
		}

		public static HTTPRequest SendRequest(string url, HTTPMethods methodType, bool isKeepAlive, bool disableCache, OnRequestFinishedDelegate callback)
		{
			return SendRequest(new HTTPRequest(new Uri(url), methodType, isKeepAlive, disableCache, callback));
		}

		public static HTTPRequest SendRequest(HTTPRequest request)
		{
			lock (Locker)
			{
				Setup();
				if (IsCallingCallbacks)
				{
					request.State = HTTPRequestStates.Queued;
					RequestQueue.Add(request);
				}
				else
				{
					SendRequestImpl(request);
				}
				return request;
			}
		}

		public static GeneralStatistics GetGeneralStatistics(StatisticsQueryFlags queryFlags)
		{
			GeneralStatistics result = default(GeneralStatistics);
			result.QueryFlags = queryFlags;
			if ((queryFlags & StatisticsQueryFlags.Connections) != 0)
			{
				int num = 0;
				foreach (KeyValuePair<string, List<HTTPConnection>> connection in Connections)
				{
					if (connection.Value != null)
					{
						num += connection.Value.Count;
					}
				}
				result.Connections = num;
				result.ActiveConnections = ActiveConnections.Count;
				result.FreeConnections = FreeConnections.Count;
				result.RecycledConnections = RecycledConnections.Count;
				result.RequestsInQueue = RequestQueue.Count;
			}
			if ((queryFlags & StatisticsQueryFlags.Cache) != 0)
			{
				result.CacheEntityCount = HTTPCacheService.GetCacheEntityCount();
				result.CacheSize = HTTPCacheService.GetCacheSize();
			}
			if ((queryFlags & StatisticsQueryFlags.Cookies) != 0)
			{
				List<Cookie> all = CookieJar.GetAll();
				result.CookieCount = all.Count;
				uint num2 = 0u;
				for (int i = 0; i < all.Count; i++)
				{
					num2 += all[i].GuessSize();
				}
				result.CookieJarSize = num2;
			}
			return result;
		}

		private static void SendRequestImpl(HTTPRequest request)
		{
			HTTPConnection conn = FindOrCreateFreeConnection(request);
			if (conn != null)
			{
				if (ActiveConnections.Find((HTTPConnection c) => c == conn) == null)
				{
					ActiveConnections.Add(conn);
				}
				FreeConnections.Remove(conn);
				request.State = HTTPRequestStates.Processing;
				request.Prepare();
				conn.Process(request);
			}
			else
			{
				request.State = HTTPRequestStates.Queued;
				RequestQueue.Add(request);
			}
		}

		private static string GetKeyForRequest(HTTPRequest request)
		{
			return ((request.Proxy == null) ? string.Empty : new UriBuilder(request.Proxy.Address.Scheme, request.Proxy.Address.Host, request.Proxy.Address.Port).Uri.ToString()) + new UriBuilder(request.CurrentUri.Scheme, request.CurrentUri.Host, request.CurrentUri.Port).Uri.ToString();
		}

		private static HTTPConnection FindOrCreateFreeConnection(HTTPRequest request)
		{
			HTTPConnection hTTPConnection = null;
			string keyForRequest = GetKeyForRequest(request);
			List<HTTPConnection> value;
			if (Connections.TryGetValue(keyForRequest, out value))
			{
				int num = 0;
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i].IsActive)
					{
						num++;
					}
				}
				if (num <= MaxConnectionPerServer)
				{
					for (int j = 0; j < value.Count; j++)
					{
						if (hTTPConnection != null)
						{
							break;
						}
						HTTPConnection hTTPConnection2 = value[j];
						if (hTTPConnection2 != null && hTTPConnection2.IsFree && (!hTTPConnection2.HasProxy || hTTPConnection2.LastProcessedUri == null || hTTPConnection2.LastProcessedUri.Host.Equals(request.CurrentUri.Host, StringComparison.OrdinalIgnoreCase)))
						{
							hTTPConnection = hTTPConnection2;
						}
					}
				}
			}
			else
			{
				Connections.Add(keyForRequest, value = new List<HTTPConnection>(MaxConnectionPerServer));
			}
			if (hTTPConnection == null)
			{
				if (value.Count >= MaxConnectionPerServer)
				{
					return null;
				}
				value.Add(hTTPConnection = new HTTPConnection(keyForRequest));
			}
			return hTTPConnection;
		}

		private static bool CanProcessFromQueue()
		{
			for (int i = 0; i < RequestQueue.Count; i++)
			{
				if (FindOrCreateFreeConnection(RequestQueue[i]) != null)
				{
					return true;
				}
			}
			return false;
		}

		private static void RecycleConnection(HTTPConnection conn)
		{
			conn.Recycle();
			RecycledConnections.Add(conn);
		}

		internal static HTTPConnection GetConnectionWith(HTTPRequest request)
		{
			lock (Locker)
			{
				for (int i = 0; i < ActiveConnections.Count; i++)
				{
					HTTPConnection hTTPConnection = ActiveConnections[i];
					if (hTTPConnection.CurrentRequest == request)
					{
						return hTTPConnection;
					}
				}
				return null;
			}
		}

		internal static bool RemoveFromQueue(HTTPRequest request)
		{
			return RequestQueue.Remove(request);
		}

		internal static string GetRootCacheFolder()
		{
			try
			{
				if (RootCacheFolderProvider != null)
				{
					return RootCacheFolderProvider();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception("HTTPManager", "GetRootCacheFolder", ex);
			}
			return Application.persistentDataPath;
		}

		public static void OnUpdate()
		{
			lock (Locker)
			{
				IsCallingCallbacks = true;
				try
				{
					for (int i = 0; i < ActiveConnections.Count; i++)
					{
						HTTPConnection hTTPConnection = ActiveConnections[i];
						switch (hTTPConnection.State)
						{
						case HTTPConnectionStates.Processing:
							hTTPConnection.HandleProgressCallback();
							if (hTTPConnection.CurrentRequest.UseStreaming && hTTPConnection.CurrentRequest.Response != null && hTTPConnection.CurrentRequest.Response.HasStreamedFragments())
							{
								hTTPConnection.HandleCallback();
							}
							if (((!hTTPConnection.CurrentRequest.UseStreaming && hTTPConnection.CurrentRequest.UploadStream == null) || hTTPConnection.CurrentRequest.EnableTimoutForStreaming) && DateTime.UtcNow - hTTPConnection.StartTime > hTTPConnection.CurrentRequest.Timeout)
							{
								hTTPConnection.Abort(HTTPConnectionStates.TimedOut);
							}
							break;
						case HTTPConnectionStates.TimedOut:
							if (DateTime.UtcNow - hTTPConnection.TimedOutStart > TimeSpan.FromMilliseconds(500.0))
							{
								Logger.Information("HTTPManager", "Hard aborting connection becouse of a long waiting TimedOut state");
								hTTPConnection.CurrentRequest.Response = null;
								hTTPConnection.CurrentRequest.State = HTTPRequestStates.TimedOut;
								hTTPConnection.HandleCallback();
								RecycleConnection(hTTPConnection);
							}
							break;
						case HTTPConnectionStates.Redirected:
							SendRequest(hTTPConnection.CurrentRequest);
							RecycleConnection(hTTPConnection);
							break;
						case HTTPConnectionStates.WaitForRecycle:
							hTTPConnection.CurrentRequest.FinishStreaming();
							hTTPConnection.HandleCallback();
							RecycleConnection(hTTPConnection);
							break;
						case HTTPConnectionStates.Upgraded:
							hTTPConnection.HandleCallback();
							break;
						case HTTPConnectionStates.WaitForProtocolShutdown:
						{
							IProtocol protocol = hTTPConnection.CurrentRequest.Response as IProtocol;
							if (protocol != null)
							{
								protocol.HandleEvents();
							}
							if (protocol == null || protocol.IsClosed)
							{
								hTTPConnection.HandleCallback();
								hTTPConnection.Dispose();
								RecycleConnection(hTTPConnection);
							}
							break;
						}
						case HTTPConnectionStates.AbortRequested:
						{
							IProtocol protocol = hTTPConnection.CurrentRequest.Response as IProtocol;
							if (protocol != null)
							{
								protocol.HandleEvents();
								if (protocol.IsClosed)
								{
									hTTPConnection.HandleCallback();
									hTTPConnection.Dispose();
									RecycleConnection(hTTPConnection);
								}
							}
							break;
						}
						case HTTPConnectionStates.Closed:
							hTTPConnection.CurrentRequest.FinishStreaming();
							hTTPConnection.HandleCallback();
							RecycleConnection(hTTPConnection);
							break;
						}
					}
				}
				finally
				{
					IsCallingCallbacks = false;
				}
				if (RecycledConnections.Count > 0)
				{
					for (int j = 0; j < RecycledConnections.Count; j++)
					{
						HTTPConnection hTTPConnection2 = RecycledConnections[j];
						if (hTTPConnection2.IsFree)
						{
							ActiveConnections.Remove(hTTPConnection2);
							FreeConnections.Add(hTTPConnection2);
						}
					}
					RecycledConnections.Clear();
				}
				if (FreeConnections.Count > 0)
				{
					for (int k = 0; k < FreeConnections.Count; k++)
					{
						HTTPConnection hTTPConnection3 = FreeConnections[k];
						if (hTTPConnection3.IsRemovable)
						{
							List<HTTPConnection> value = null;
							if (Connections.TryGetValue(hTTPConnection3.ServerAddress, out value))
							{
								value.Remove(hTTPConnection3);
							}
							hTTPConnection3.Dispose();
							FreeConnections.RemoveAt(k);
							k--;
						}
					}
				}
				if (CanProcessFromQueue())
				{
					if (RequestQueue.Find((HTTPRequest req) => req.Priority != 0) != null)
					{
						RequestQueue.Sort((HTTPRequest req1, HTTPRequest req2) => req1.Priority - req2.Priority);
					}
					HTTPRequest[] array = RequestQueue.ToArray();
					RequestQueue.Clear();
					for (int l = 0; l < array.Length; l++)
					{
						SendRequest(array[l]);
					}
				}
			}
			if (heartbeats != null)
			{
				heartbeats.Update();
			}
		}

		internal static void OnQuit()
		{
			lock (Locker)
			{
				HTTPCacheService.SaveLibrary();
				foreach (KeyValuePair<string, List<HTTPConnection>> connection in Connections)
				{
					foreach (HTTPConnection item in connection.Value)
					{
						item.Dispose();
					}
					connection.Value.Clear();
				}
				Connections.Clear();
			}
		}
	}
}
