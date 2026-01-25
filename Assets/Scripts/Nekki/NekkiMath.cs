using System;
using System.Collections.Generic;

namespace Nekki
{
	public class NekkiMath
	{
		private const double RADTODEG = 180.0 / Math.PI;

		private const double DEGTORAD = Math.PI / 180.0;

		private const double EPSILON = 1.1920929E-07;

		private static NekkiRandom randUnit = new NekkiRandom();

		public static float round(float number, int pow)
		{
			return (float)(int)(number * (float)pow) / (float)pow;
		}

		public static float roundToOrder(float number, int order)
		{
			int num = (int)Math.Pow(10.0, order);
			return (float)(int)(number * (float)num) / (float)num;
		}

		public static string roundToOrderString(float number, int order)
		{
			return Math.Round(number, order).ToString();
		}

		public static float log(float arg, float _base)
		{
			float result = 0f;
			if (_base > 0f && _base != 1f && arg > 0f)
			{
				result = (float)(Math.Log(arg) / Math.Log(_base));
			}
			return result;
		}

		public static float radToDeg(float rad)
		{
			return (float)((double)rad * (180.0 / Math.PI));
		}

		public static float degToRad(float deg)
		{
			return (float)((double)deg * (Math.PI / 180.0));
		}

		public static float randomFloat()
		{
			return randUnit.randomFloat();
		}

		public static float randomFloat(float max)
		{
			return randUnit.randomFloat(max);
		}

		public static float randomFloat(float min, float max)
		{
			return randUnit.randomFloat(min, max);
		}

		public static int randomInt(int max)
		{
			return (int)randUnit.randomInt((uint)max);
		}

		public static int randomInt(int min, int max)
		{
			return (int)randUnit.randomInt((uint)min, (uint)max);
		}

		public static bool randomChance(float percent, float maximum = 100f)
		{
			return randUnit.randomChance(percent, maximum);
		}

		public static uint setRandomSeed()
		{
			uint num = (uint)DateTime.UtcNow.Ticks;
			randUnit.setSeed(num);
			return num;
		}

		public static void setRandomSeed(int seed)
		{
			randUnit.setSeed((uint)seed);
		}

		public static T randomElement<T>(List<T> vec)
		{
			int count = vec.Count;
			if (count == 0)
			{
				AdvLog.LogError("NekkiMath::randomElement - empty vector");
			}
			return vec[randomInt(count)];
		}

		private static int randMax()
		{
			return (int)randUnit.randMax();
		}
	}
}
