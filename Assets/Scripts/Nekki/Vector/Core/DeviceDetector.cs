using UnityEngine;

namespace Nekki.Vector.Core
{
	public static class DeviceDetector
	{
		private static bool _IsInited;

		private static bool _UseLowResolution;

		public static bool UseLowResolution
		{
			get
			{
				return _UseLowResolution;
			}
		}

		public static void Init()
		{
			if (!_IsInited)
			{
				_IsInited = true;
				_UseLowResolution = Screen.height * Screen.width <= 786432;
			}
		}
	}
}
