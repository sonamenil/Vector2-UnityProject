using UnityEngine;

namespace Nekki.Vector.Core.Scripts.Effects
{
	public class ParticleBehaviour : Parallax
	{
		private ParticleSystem _ParticleSystem;

		public void SetParticleSystem(ParticleSystem particleSystem)
		{
			_ParticleSystem = particleSystem;
		}

		protected override void Start()
		{
			base.Start();
			if (base.transform.lossyScale.x < 0f)
			{
				base.transform.Rotate(180f, 0f, 0f, Space.Self);
			}
			if (base.transform.lossyScale.y < 0f)
			{
				base.transform.Rotate(0f, 0f, 180f, Space.Self);
			}
		}

		protected override void UpdatePosition()
		{
			base.UpdatePosition();
			if (_ParticleSystem != null)
			{
				ParticleSystem.Particle[] array = new ParticleSystem.Particle[_ParticleSystem.particleCount];
				int particles = _ParticleSystem.GetParticles(array);
				for (int i = 0; i < particles; i++)
				{
					array[i].position += _LastCameraOffset;
				}
				_ParticleSystem.SetParticles(array, particles);
			}
		}
	}
}
