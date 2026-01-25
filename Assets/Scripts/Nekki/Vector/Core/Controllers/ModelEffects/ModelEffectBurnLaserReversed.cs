using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Effects;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using UnityEngine;

namespace Nekki.Vector.Core.Controllers.ModelEffects
{
	public class ModelEffectBurnLaserReversed : ModelEffect
	{
		private const int MaxFramesToPlay = 30;

		private ModelObject _ModelObject;

		private Dictionary<ModelLine, ParticleSystem> _Particles = new Dictionary<ModelLine, ParticleSystem>();

		private Dictionary<ModelLine, float> _StrokesOnInit = new Dictionary<ModelLine, float>();

		private bool effectedStarted;

		public ModelEffectBurnLaserReversed(ModelObject p_modelObject)
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
			foreach (ModelLine capsule in _ModelObject.Capsules)
			{
				_StrokesOnInit[capsule] = capsule.Stroke;
				capsule.Stroke = 0f;
			}
		}

		public override bool Render()
		{
			if (!effectedStarted)
			{
				foreach (ModelLine capsule in _ModelObject.Capsules)
				{
					capsule.Stroke = _StrokesOnInit[capsule] / 30f;
				}
				effectedStarted = true;
			}
			foreach (ModelLine capsule2 in _ModelObject.Capsules)
			{
				capsule2.Stroke = Math.Min(capsule2.Stroke / 0.8f, _StrokesOnInit[capsule2]);
			}
			bool flag = true;
			foreach (KeyValuePair<ModelLine, ParticleSystem> particle in _Particles)
			{
				if (particle.Key.Stroke == _StrokesOnInit[particle.Key])
				{
					particle.Value.Stop();
					continue;
				}
				SetParticleSystemParams(particle.Value, particle.Key.Center, "Model");
				flag = false;
			}
			if (flag)
			{
				foreach (ModelRender modelRender in _ModelObject.ModelRenders)
				{
					modelRender.MeshVisible = true;
				}
			}
			return flag;
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
			Render();
		}
	}
}
