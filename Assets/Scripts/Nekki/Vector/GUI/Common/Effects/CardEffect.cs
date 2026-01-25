using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI.Common.Effects
{
	public class CardEffect : MonoBehaviour
	{
		[SerializeField]
		private List<ParticleSystem> _Particles = new List<ParticleSystem>();

		public bool IsDead
		{
			get
			{
				int num = 0;
				foreach (ParticleSystem particle in _Particles)
				{
					if (particle.IsAlive())
					{
						num++;
					}
				}
				return num == 0;
			}
		}

		private void Update()
		{
			if (IsDead)
			{
				Object.DestroyImmediate(base.gameObject);
			}
		}

		public void ChangeParticleStartColor(Color[] p_color)
		{
			int i = 0;
			for (int count = _Particles.Count; i < count; i++)
			{
				_Particles[i].startColor = p_color[i];
			}
		}
	}
}
