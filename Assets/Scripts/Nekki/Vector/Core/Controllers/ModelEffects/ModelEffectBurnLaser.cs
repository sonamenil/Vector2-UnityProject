using System.Collections.Generic;
using Nekki.Vector.Core.Effects;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using UnityEngine;

namespace Nekki.Vector.Core.Controllers.ModelEffects
{
	public class ModelEffectBurnLaser : ModelEffect
	{
		private ModelObject _ModelObject;

		private Dictionary<ModelLine, ParticleSystem> _Particles = new Dictionary<ModelLine, ParticleSystem>();

		public ModelEffectBurnLaser(ModelObject p_modelObject)
		{
			_ModelObject = p_modelObject;
			foreach (ModelRender modelRender in _ModelObject.ModelRenders)
			{
				modelRender.MeshVisible = false;
			}
			for (int i = 0; i < _ModelObject.Capsules.Count; i++)
			{
				if (i % 3 == 0)
				{
					ParticleSystem particleSystem = EffectManager.Instantiate("burn_laser_model", null);
					SetParticleSystemParams(particleSystem, _ModelObject.Capsules[i].Center, "Model");
					_Particles.Add(_ModelObject.Capsules[i], particleSystem);
				}
			}
		}

		public override bool Render()
		{
			foreach (ModelLine capsule in _ModelObject.Capsules)
			{
				capsule.Stroke *= 0.8f;
				if ((double)capsule.Stroke < 0.1)
				{
					capsule.Stroke = 0f;
				}
			}
			bool result = true;
			foreach (KeyValuePair<ModelLine, ParticleSystem> particle in _Particles)
			{
				if ((double)particle.Key.Stroke < 0.01)
				{
					particle.Value.Stop();
					continue;
				}
				SetParticleSystemParams(particle.Value, particle.Key.Center, "Model");
				result = false;
			}
			return result;
		}

		public override void Pause(bool p_value)
		{
			foreach (ParticleSystem value in _Particles.Values)
			{
				if (p_value)
				{
					value.Pause();
				}
				else
				{
					value.Play();
				}
			}
		}

		public override void Simulate(float p_time)
		{
			foreach (ParticleSystem value in _Particles.Values)
			{
				value.Simulate(p_time, true, false);
			}
		}
	}
}
