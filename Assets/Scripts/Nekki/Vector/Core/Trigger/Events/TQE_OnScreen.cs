namespace Nekki.Vector.Core.Trigger.Events
{
	public class TQE_OnScreen : TriggerQuestEvent
	{
		private const string ScreenShop = "Shop";

		private const string ScreenStart = "Start";

		private const string ScreenRun = "Run";

		private const string ScreenTerminal = "Terminal";

		public string ScreenName;

		public static TQE_OnScreen ScreenShopEvent
		{
			get
			{
				return new TQE_OnScreen("Shop");
			}
		}

		public static TQE_OnScreen ScreenStartEvent
		{
			get
			{
				return new TQE_OnScreen("Start");
			}
		}

		public static TQE_OnScreen ScreenRunEvent
		{
			get
			{
				return new TQE_OnScreen("Run");
			}
		}

		public static TQE_OnScreen ScreenTerminalEvent
		{
			get
			{
				return new TQE_OnScreen("Terminal");
			}
		}

		public TQE_OnScreen(string p_screenName)
		{
			ScreenName = p_screenName;
			_Type = EventType.TQE_ON_SCREEN;
		}

		public override bool IsEqual(TriggerEvent p_value)
		{
			return base.IsEqual(p_value) && (p_value as TQE_OnScreen).ScreenName == ScreenName;
		}
	}
}
