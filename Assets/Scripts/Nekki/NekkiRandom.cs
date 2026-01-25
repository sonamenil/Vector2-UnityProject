using System;

namespace Nekki
{
	public class NekkiRandom
	{
		private RandomGenerator myGenerator;

		public NekkiRandom(uint seed)
		{
			myGenerator = new RandomGenerator(seed);
		}

		public NekkiRandom()
		{
			myGenerator = new RandomGenerator(0u);
			uint seed = (uint)DateTime.UtcNow.Ticks;
			myGenerator.setSeed(seed);
		}

		public uint randMax()
		{
			return RandomGenerator.getMaxLCG();
		}

		public uint getRandom()
		{
			return myGenerator.getRandom();
		}

		public void setSeed(uint seed)
		{
			myGenerator.setSeed(seed);
		}

		public uint getSeed()
		{
			return myGenerator.getSeed();
		}

		public float randomFloat()
		{
			return (float)getRandom() / (float)randMax() + (float)getRandom() / (float)randMax() / (float)randMax();
		}

		public float randomFloat(float max)
		{
			return randomFloat() * max;
		}

		public float randomFloat(float min, float max)
		{
			return min + randomFloat(max - min);
		}

		public uint randomInt(uint max)
		{
			return (uint)((float)max * randomFloat());
		}

		public uint randomInt(uint min, uint max)
		{
			return min + randomInt(max - min);
		}

		public bool randomChance(float percent, float maximum = 100f)
		{
			return randomFloat(maximum) < percent;
		}
	}
}
