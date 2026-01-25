namespace Nekki.Vector.GUI.Dialogs
{
	public static class OrientationExtension
	{
		public static string GetName(this Notification.Orientation orientation)
		{
			switch (orientation)
			{
			case Notification.Orientation.Top:
				return "Top";
			case Notification.Orientation.Bottom:
				return "Bottom";
			case Notification.Orientation.Right:
				return "Right";
			case Notification.Orientation.Left:
				return "Left";
			case Notification.Orientation.LeftBottom:
				return "LeftBottom";
			default:
				return "Left";
			}
		}

		public static Notification.Orientation OrientationFromName(this string orientation)
		{
			switch (orientation)
			{
			case "Top":
				return Notification.Orientation.Top;
			case "Bottom":
				return Notification.Orientation.Bottom;
			case "Right":
				return Notification.Orientation.Right;
			case "Left":
				return Notification.Orientation.Left;
			case "LeftBottom":
				return Notification.Orientation.LeftBottom;
			default:
				return Notification.Orientation.Left;
			}
		}
	}
}
