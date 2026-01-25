using UnityEngine;

namespace Nekki.Vector.GUI.Menu
{
	public static class ColorHelper
	{
		public const string Blue = "blue";

		public const string Orange = "orange";

		public const string Grey = "grey";

		public const string Red = "red";

		public const string Green = "green";

		public const string White = "white";

		public const string CardRed = "card_red";

		public const string CardGreen = "card_green";

		public static Color GetColor(string color)
		{
			switch (color)
			{
			case "paleblue":
				return new Color(0.369f, 0.643f, 0.733f);
			case "blue":
				return new Color(0.25f, 0.77f, 0.88f);
			case "paleorange":
				return new Color(0.796f, 0.49f, 0.169f);
			case "orange":
				return new Color(0.9f, 0.43f, 0f);
			case "grey":
				return new Color(0.62f, 0.62f, 0.62f);
			case "red":
				return new Color(0.65f, 0.078f, 0.027f);
			case "card_red":
				return new Color(0.86f, 0.054f, 0f);
			case "green":
				return new Color(0.435f, 0.792f, 0f);
			case "white":
				return new Color(1f, 1f, 1f);
			case "card_green":
				return new Color(0f, 253f, 0f);
			case "chip_red":
				return new Color(0.63f, 0.06f, 0.06f);
			default:
				return new Color(0.62f, 0.62f, 0.62f);
			}
		}

		public static Color ChangeColorBrightness(Color32 p_color, float p_factor)
		{
			float num = (int)p_color.r;
			float num2 = (int)p_color.g;
			float num3 = (int)p_color.b;
			if (p_factor < 0f)
			{
				p_factor = 1f + p_factor;
				num *= p_factor;
				num2 *= p_factor;
				num3 *= p_factor;
			}
			else
			{
				num = (255f - num) * p_factor + num;
				num2 = (255f - num2) * p_factor + num2;
				num3 = (255f - num3) * p_factor + num3;
			}
			return new Color32((byte)num, (byte)num2, (byte)num3, p_color.a);
		}
	}
}
