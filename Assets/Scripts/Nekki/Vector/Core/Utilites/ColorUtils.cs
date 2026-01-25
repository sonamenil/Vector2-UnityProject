using System;
using UnityEngine;

namespace Nekki.Vector.Core.Utilites
{
	public static class ColorUtils
	{
		public static Color FromHex(string p_color, float opacity = 1f)
		{
			p_color = p_color.Replace("#", string.Empty);
			if (p_color.Length != 6 && p_color.Length != 8)
			{
				return new Color(0f, 0f, 0f, 1f);
			}
			int num = p_color.Length / 2;
			byte[] array = new byte[4]
			{
				0,
				0,
				0,
				(byte)(255f * opacity)
			};
			for (int i = 0; i < num; i++)
			{
				array[i] = Convert.ToByte(p_color.Substring(i * 2, 2), 16);
			}
			return new Color((float)(int)array[0] / 255f, (float)(int)array[1] / 255f, (float)(int)array[2] / 255f, (float)(int)array[3] / 255f);
		}
	}
}
