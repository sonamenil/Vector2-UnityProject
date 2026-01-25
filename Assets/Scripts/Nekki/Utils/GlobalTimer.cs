using System;
using UnityEngine;

namespace Nekki.Utils
{
	public class GlobalTimer : ExtentionBehaviour
	{
		public const int TICK = 0;

		private static GlobalTimer _instance;

		private static TimeSpan _delta;

		private static float _syncTime;

		private static DateTime _serverUtc;

		private static bool _syncronized;

		private static Action _onSuccess;

		private static Action _onError;

		private static bool _requestInProgress;

		private static bool _lastRequestSuccessful;

		private static bool _externalInit;

		private float _lastTimeTick;

		public static GlobalTimer Instnce
		{
			get
			{
				if (!_instance)
				{
					Init(false);
				}
				return _instance;
			}
		}

		public static DateTime LocalizedNow
		{
			get
			{
				return Now.ToLocalTime();
			}
		}

		public static DateTime Now
		{
			get
			{
				return _serverUtc.AddSeconds(Time.unscaledTime - _syncTime);
			}
		}

		public static double GetTime
		{
			get
			{
				return ConvertToUnixTimestamp(Now);
			}
		}

		public static bool IsSynchronized
		{
			get
			{
				return _syncronized;
			}
		}

		public static bool IsRequestInProgress
		{
			get
			{
				return _requestInProgress;
			}
		}

		public static bool IsLastRequestSuccessful
		{
			get
			{
				return _lastRequestSuccessful;
			}
		}

		public static long ConvertToUnixTimestamp(DateTime date)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return (long)Math.Floor((date.ToUniversalTime() - dateTime).TotalSeconds);
		}

		public static void Init(bool externalInit = false)
		{
			if (!_instance)
			{
				_instance = new GameObject("_timer").AddComponent<GlobalTimer>();
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
				_delta = default(TimeSpan);
				_syncronized = false;
				_lastRequestSuccessful = false;
				_externalInit = externalInit;
				if (!_externalInit)
				{
					ServerTimeSync(null, null);
				}
				else
				{
					DefaultInit();
				}
			}
		}

		private static void DefaultInit()
		{
			_serverUtc = DateTime.Now;
			_syncTime = Time.unscaledTime;
			_delta = _serverUtc - DateTime.Now;
			_syncronized = true;
			_requestInProgress = false;
			_lastRequestSuccessful = true;
			if (_onSuccess != null)
			{
				_onSuccess();
			}
		}

		public static void ServerTimeSync(Action onSuccess = null, Action onError = null)
		{
			_requestInProgress = true;
			_syncronized = false;
			_onSuccess = onSuccess;
			_onError = onError;
			ServerProvider.Instance.TimeSync(OnServerTime, OnServerTimeError);
		}

		public static void ServerTimeExtended(long msec)
		{
			DateTime serverUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(msec);
			serverUtc += timeSpan;
			_serverUtc = serverUtc;
			_syncTime = Time.unscaledTime;
			_delta = _serverUtc - DateTime.Now;
			_syncronized = true;
			_requestInProgress = false;
			_lastRequestSuccessful = true;
			if (_onSuccess != null)
			{
				_onSuccess();
			}
		}

		private static void OnServerTime(long time)
		{
			_serverUtc = UnixTimeStampToDateTime(time);
			_syncTime = Time.unscaledTime;
			_delta = _serverUtc - DateTime.Now;
			_syncronized = true;
			_requestInProgress = false;
			_lastRequestSuccessful = true;
			if (_onSuccess != null)
			{
				_onSuccess();
			}
		}

		private static void OnServerTimeError(object message)
		{
			_serverUtc = DateTime.Now;
			_syncTime = Time.unscaledTime;
			_delta = _serverUtc - DateTime.Now;
			_syncronized = false;
			_requestInProgress = false;
			_lastRequestSuccessful = false;
			if (_onError != null)
			{
				_onError();
			}
			AdvLog.LogError(message);
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
		}

		public static DateTime UnixTimeStampToDateTimeLocal(double unixTimeStamp)
		{
			return UnixTimeStampToDateTime(unixTimeStamp).ToLocalTime();
		}

		private void Update()
		{
			if (_lastTimeTick + 1f < Time.unscaledTime)
			{
				_lastTimeTick = Time.unscaledTime;
				_instance.callEvent(0, Now);
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (!pauseStatus && !_externalInit)
			{
				ServerTimeSync(null, null);
			}
		}

		private new void OnDestroy()
		{
			_instance = null;
			StopAllCoroutines();
			base.OnDestroy();
		}
	}
}
