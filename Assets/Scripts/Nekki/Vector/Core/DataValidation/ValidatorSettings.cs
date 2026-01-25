namespace Nekki.Vector.Core.DataValidation
{
	public static class ValidatorSettings
	{
		private static bool _IsEnabled = true;

		public static bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
		}

		public static void SetEnabled(bool p_iosValue, bool p_androidValue)
		{
			_IsEnabled = p_androidValue;
		}
	}
}
