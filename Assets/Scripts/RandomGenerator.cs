public class RandomGenerator
{
	private const uint M = 2147483647u;

	private uint _previous;

	private uint _seed;

	public RandomGenerator(uint seed)
	{
		setSeed(seed);
	}

	public uint getRandom()
	{
		_previous = randLCG(_previous);
		return _previous;
	}

	public uint getSeed()
	{
		return _seed;
	}

	public void setSeed(uint seed)
	{
		_seed = seed;
		_previous = seed;
	}

	public uint randLCG(uint element)
	{
		element = (element * 1103515245 + 12345) & 0x7FFFFFFF;
		return element;
	}

	public static uint getMaxLCG()
	{
		return 2147483648u;
	}
}
