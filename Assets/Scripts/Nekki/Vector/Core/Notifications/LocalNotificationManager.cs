using System;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Quest;

namespace Nekki.Vector.Core.Notifications
{
	public class LocalNotificationManager
	{
		private enum NotificationIDs
		{
			LoginNotification = 1,
			QuestNotification = 2,
			SaleNotification = 3,
			SaleEffectNotification = 4,
			EnergyNotification = 5
		}

		private const string Title = "Vector 2";

		private const string DailyRemind = "Notifications.Local.DailyRemind";

		private const string SaleEndRemind = "Notifications.Local.Sale.HourBeforeSaleEnd";

		private const string EffectEndInDayRemind = "Notifications.Local.Sale.DayBeforeBonusEnd";

		private const string EffectEndInThreeDaysRemind = "Notifications.Local.Sale.ThreeDaysBeforeBonusEnd";

		private const string QuestRemind = "Notifications.Local.QuestRemind";

		private const string RechargedPhrase = "Notifications.Local.Recharged";

		private const string EnergyRechargedPhrase = "Notifications.Local.EnergyRecharged";

		private static LocalNotificationManager _Current;

		private int DayForQuestNotification
		{
			get
			{
				return 0;
			}
		}

		private int DayForLoginNotification
		{
			get
			{
				return 0;
			}
		}

		public static LocalNotificationManager Current
		{
			get
			{
                if (_Current == null)
                {
                    _Current = new LocalNotificationManager();
                }
                return _Current;
            }
		}

		public void RefreshNotifications()
		{

		}

		public void SetAllNotification()
		{

		}

		public void CancelAllNotifications()
		{

		}

		private void CreateQuestNotification()
		{

		}

		public void CreateLoginReminder()
		{

		}

		private void CreateSaleTimers()
		{


		}

		private void CreateEnergyChargedNotification()
		{

		}

		private void CreateEffectEndTimer()
		{

		}

		public void CreateNotification(int p_id, string title, string message, TimeSpan delay)
		{

		}

		private void CreateAndroidNotification(int p_id, string p_title, string p_message, TimeSpan p_delay)
		{

		}

		private void CancelAllAndroidNotifications()
		{
		}
	}
}
