using System.Collections.Generic;

namespace Nekki.Vector.Core.Generator
{
	public static class MainRandom
	{
		private static uint _Seed;

		private static NekkiRandom _Random;

		public static NekkiRandom Current
		{
			get
			{
				return _Random;
			}
		}

		public static void ResetRandom()
		{
			_Random = null;
		}

		public static void InitRandomIfNotYet()
		{
			if (_Random == null)
			{
				SetSeed(-1);
			}
		}

		public static void SetSeed(int p_seed)
		{
			if (p_seed < 0)
			{
				_Random = new NekkiRandom();
			}
			else
			{
				_Random = new NekkiRandom((uint)p_seed);
			}
			_Seed = _Random.getSeed();
		}

		public static void SetSeed(uint p_seed)
		{
			_Random = new NekkiRandom(p_seed);
			_Seed = _Random.getSeed();
		}

		public static void ResetWithCurrentSeed()
		{
			_Random = new NekkiRandom(_Seed);
		}

		public static void ShuffleList<T>(List<T> p_list)
		{
			int num = p_list.Count;
			while (num > 1)
			{
				num--;
				int index = (int)_Random.randomInt(0u, (uint)(num + 1));
				T value = p_list[index];
				p_list[index] = p_list[num];
				p_list[num] = value;
			}
		}

		public static void ShuffleList<T>(List<T> p_list, List<uint> p_weight)
		{
			uint num = 0u;
			int num2 = p_list.Count;
			for (int i = 0; i < p_weight.Count; i++)
			{
				num += p_weight[i];
			}
			while (num2 > 1)
			{
				num2--;
				uint num3 = _Random.randomInt(0u, num);
				uint num4 = 0u;
				int index = 0;
				for (int j = 0; j < p_weight.Count; j++)
				{
					num4 += p_weight[j];
					if (num4 > num3)
					{
						index = j;
						num -= p_weight[j];
						p_weight[j] = 0u;
						break;
					}
				}
				uint value = p_weight[index];
				p_weight[index] = p_weight[num2];
				p_weight[num2] = value;
				T value2 = p_list[index];
				p_list[index] = p_list[num2];
				p_list[num2] = value2;
			}
			p_list.Reverse();
		}

		public static T GetRandomFromList<T>(List<T> p_list)
		{
			return p_list[(int)_Random.randomInt((uint)p_list.Count)];
		}
	}
}
