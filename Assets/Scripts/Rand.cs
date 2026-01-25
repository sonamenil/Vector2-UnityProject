using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class Rand
{
	private static short _seed;

	private static List<short> _source;

	private static List<short> _dirty;

	public static short Seed
	{
		get
		{
			return _seed;
		}
		set
		{
			if (_seed != (ushort)(value % 65535))
			{
				Reset(value);
			}
		}
	}

	private static short Next
	{
		get
		{
			if (_source.Count == 0)
			{
				Rewind();
			}
			int index = Random.Range(0, _source.Count);
			short num = _source[index];
			_source.RemoveAt(index);
			_dirty.Add(num);
			return num;
		}
	}

	static Rand()
	{
		_source = new List<short>();
		_dirty = new List<short>();
		Reset(0);
	}

	public static void Init(int seed)
	{
		if (_seed != (short)(seed % 32767))
		{
			Reset(seed);
		}
	}

	private static void Reset(int seed)
	{
		_seed = (short)(seed % 32767);
		_dirty.Clear();
		_source.Clear();
		for (short num = 0; num < short.MaxValue; num++)
		{
			_source.Add(num);
		}
		_source.Sort((short P_0, short P_1) => Random.Range(-1, 1));
		_dirty.AddRange(_source.GetRange(0, _seed));
		_source.RemoveRange(0, _seed);
	}

	private static void Rewind()
	{
		_dirty = Interlocked.Exchange(ref _source, _dirty);
		AdvLog.Log("after dirty: " + _dirty.Count + " source: " + _source.Count);
	}

	public static int Range(int from, int to)
	{
		if (from > to)
		{
			to = Interlocked.Exchange(ref from, to);
		}
		int num = to - from;
		return (num != 0) ? (from + Next % num) : 0;
	}
}
