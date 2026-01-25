using Nekki.Vector.Core.Game;

namespace Nekki.Vector.Core
{
	public static class URLCreator
	{
		public static string Make(string p_endOfURL)
		{
			string text = "undefined";
			if (DeviceInformation.IsiOS)
			{
				text = "ios";
			}
			else if (DeviceInformation.IsAndroid)
			{
				text = "android";
			}
            else if (DeviceInformation.IsEmulator)
            {
                text = "android";
            }
            return string.Format("https://v2assets.nekkimobile.ru/{0}/{1}/{2}/{3}", text, ApplicationController.BuildVersion, (!Settings.IsReleaseBuild) ? "debug" : "release", p_endOfURL);
		}
	}
}
