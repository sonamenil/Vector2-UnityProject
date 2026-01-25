namespace Nekki.Vector.GUI.Dialogs
{
	public static class HideByExtension
	{
		public static string GetName(this Notification.HideBy hideBy)
		{
			switch (hideBy)
			{
			case Notification.HideBy.Click:
				return "Click";
			case Notification.HideBy.TimeOrClick:
				return "TimeOrClick";
			case Notification.HideBy.TimeBlockClicks:
				return "TimeBlockClicks";
			case Notification.HideBy.TimeDontBlockClicks:
				return "TimeDontBlockClicks";
			default:
				return "Click";
			}
		}

		public static Notification.HideBy HideByFromName(this string hideBy)
		{
			switch (hideBy)
			{
			case "Click":
				return Notification.HideBy.Click;
			case "TimeOrClick":
				return Notification.HideBy.TimeOrClick;
			case "TimeBlockClicks":
				return Notification.HideBy.TimeBlockClicks;
			case "TimeDontBlockClicks":
				return Notification.HideBy.TimeDontBlockClicks;
			default:
				return Notification.HideBy.Click;
			}
		}
	}
}
