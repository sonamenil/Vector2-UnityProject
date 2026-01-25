using System;

namespace Nekki.Vector.GUI.Common
{
	[Serializable]
	public class ButtonData
	{
		public string ImageAlias = string.Empty;

		public Action Callback;

		public ButtonData(string p_imageAlias, Action p_callback)
		{
			ImageAlias = p_imageAlias;
			Callback = p_callback;
		}
	}
}
