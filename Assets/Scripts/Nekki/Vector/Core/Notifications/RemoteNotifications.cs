using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.Notifications
{
	public class RemoteNotifications : MonoBehaviour
	{
		private static bool _IsInited;

		private static RemoteNotifications _Current;

		private static string _PushToken = string.Empty;

		public static string PushToken
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		private static string ClassName
		{
			get
			{
				return null;
			}
		}

		public static void Init()
		{
			if (!_IsInited)
			{
				_IsInited = true;
			}
		}

		public void NewAndroidToken(string p_token)
		{
		}

		public void OnRecive(string p_message)
		{
		}
	}
}
