using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Audio;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationSound
	{
		private List<string> _Names = new List<string>();

		private static Random Random = new Random();

		public AnimationSound(string p_name, int Type)
		{
			_Names = new List<string>(p_name.Split('|'));
		}

		public void Play(float p_volume = 1f)
		{
			int index = Random.Next(0, _Names.Count);
			AudioManager.PlaySound(_Names[index], p_volume);
		}
	}
}
