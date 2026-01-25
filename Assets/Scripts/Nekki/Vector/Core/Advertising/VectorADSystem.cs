using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Advertising
{
	public class VectorADSystem : ADSystem
	{
		public const string VideoCurrencyName = "currency_video";

		private static string AppId
		{
			get
			{
				return null;
			}
		}

		private static string Secret
		{
			get
			{
				return null;
			}
		}

		public static void Init()
		{

		}

		public static void Reinit()
		{

		}

		public static void LoadInterstitialAd()
		{

		}

		public static bool ShowInterstitialAd()
		{

			return false;
		}

		private static void OnVirtualCurrencyRecivedCB(string p_name, float p_value)
		{

		}

		private static int GetVideoReward(int p_def = 1)
		{
			return 1;
		}

		private static void OnAppPauseCallback(bool p_pause)
		{

		}
	}
}
