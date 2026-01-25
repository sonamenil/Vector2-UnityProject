using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Boosterpack
{
	public class BoosterpackEffect : MonoBehaviour
	{
		[SerializeField]
		private List<ParticleSystem> _Particles = new List<ParticleSystem>();

		public void Play()
		{
			for (int i = 0; i < _Particles.Count; i++)
			{
				_Particles[i].Play();
			}
		}

		public void Stop()
		{
			for (int i = 0; i < _Particles.Count; i++)
			{
				_Particles[i].Stop();
			}
		}
	}
}
