using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Audio.Internal
{
	public class AudioUnitPool
	{
		public const int MaxUnits = 16;

		private static List<AudioUnit> _AudioUnits = new List<AudioUnit>();

		public static void Init(AudioManager p_manager)
		{
			Transform transform = p_manager.transform;
			for (int i = 0; i < 16; i++)
			{
				_AudioUnits.Add(CreateAU(transform));
			}
		}

		private static AudioUnit CreateAU(Transform p_parent)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(p_parent, false);
			return gameObject.AddComponent<AudioUnit>();
		}

		public static AudioUnit GetIdle()
		{
			int i = 0;
			for (int count = _AudioUnits.Count; i < count; i++)
			{
				if (_AudioUnits[i].Idle)
				{
					return _AudioUnits[i];
				}
			}
			return null;
		}
	}
}
