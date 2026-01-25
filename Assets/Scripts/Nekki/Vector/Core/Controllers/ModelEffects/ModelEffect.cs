using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Runners;
using UnityEngine;

namespace Nekki.Vector.Core.Controllers.ModelEffects
{
	public abstract class ModelEffect
	{
		public static ModelEffect Create(string p_name, ModelObject p_modelObject, QuadRunner p_runner)
		{
			switch (p_name)
			{
			case "BurnLaser":
				return new ModelEffectBurnLaser(p_modelObject);
			case "BurnLaserReversed":
				return new ModelEffectBurnLaserReversed(p_modelObject);
			case "LaserSparks":
				return new ModelEffectLaserSparks(p_modelObject, p_runner);
			default:
				return null;
			}
		}

		public abstract bool Render();

		public abstract void Pause(bool p_value);

		public abstract void Simulate(float p_time);

		protected void SetParticleSystemParams(ParticleSystem p_particle, Vector3f p_position, string p_sortingLayerName)
		{
			SetParticleSystemParams(p_particle, p_position.X, p_position.Y, p_sortingLayerName);
		}

		protected void SetParticleSystemParams(ParticleSystem p_particle, float p_x, float p_y, string p_sortingLayerName)
		{
			p_particle.transform.localPosition = new Vector2(p_x, p_y);
			ParticleSystemRenderer component = p_particle.gameObject.GetComponent<ParticleSystemRenderer>();
			component.sortingLayerName = p_sortingLayerName;
		}
	}
}
