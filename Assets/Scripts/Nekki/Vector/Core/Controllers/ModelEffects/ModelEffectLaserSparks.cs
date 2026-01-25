using Nekki.Vector.Core.Effects;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;
using UnityEngine;

namespace Nekki.Vector.Core.Controllers.ModelEffects
{
	public class ModelEffectLaserSparks : ModelEffect
	{
		private ModelObject _ModelObject;

		private QuadRunner _Quad;

		private ParticleSystem _Particle1;

		private ParticleSystem _Particle2;

		private bool _IsWaitOff;

		private bool _IsHorisontal;

		public ModelEffectLaserSparks(ModelObject p_object, QuadRunner p_quad)
		{
			_ModelObject = p_object;
			_Quad = p_quad;
			_Particle1 = EffectManager.Instantiate("AntiLaserEffect", null);
			_Particle2 = EffectManager.Instantiate("AntiLaserEffectBurst", null);
			if (p_quad.WidthQuad > p_quad.HeightQuad)
			{
				_IsHorisontal = true;
			}
			Render();
		}

		public override bool Render()
		{
			if (_IsWaitOff)
			{
				if (_Particle1.isStopped && _Particle2.isStopped)
				{
					Object.Destroy(_Particle1.gameObject);
					Object.Destroy(_Particle2.gameObject);
					return true;
				}
				return false;
			}
			float p_min = 0f;
			float p_max = 0f;
			if (!GetMaxMin(ref p_max, ref p_min))
			{
				_IsWaitOff = true;
				_Particle1.loop = false;
				_Particle2.loop = false;
				return false;
			}
			float num = p_max - p_min;
			if (_IsHorisontal)
			{
				SetParticleSystemParams(_Particle1, p_min + num / 2f, _Quad.Position.y + _Quad.HeightQuad / 2f, num, 1f, _Quad.HeightQuad, "Model");
				SetParticleSystemParams(_Particle2, p_min + num / 2f, _Quad.Position.y + _Quad.HeightQuad / 2f, num, 1f, _Quad.HeightQuad, "Model");
			}
			else
			{
				SetParticleSystemParams(_Particle1, _Quad.Position.x + _Quad.WidthQuad / 2f, p_min + num / 2f, _Quad.WidthQuad, 1f, num, "Model");
				SetParticleSystemParams(_Particle2, _Quad.Position.x + _Quad.WidthQuad / 2f, p_min + num / 2f, _Quad.WidthQuad, 1f, num, "Model");
			}
			return false;
		}

		private bool GetMaxMin(ref float p_max, ref float p_min)
		{
			p_max = -2.1474836E+09f;
			p_min = 2.1474836E+09f;
			bool result = false;
			foreach (ModelNode node in _ModelObject.Nodes)
			{
				if (_Quad.Hit(node.Start))
				{
					result = true;
					float num = ((!_IsHorisontal) ? node.Start.Y : node.Start.X);
					if (num > p_max)
					{
						p_max = num;
					}
					if (num < p_min)
					{
						p_min = num;
					}
				}
			}
			return result;
		}

		public override void Pause(bool p_value)
		{
			if (p_value)
			{
				_Particle1.Pause();
				_Particle2.Pause();
			}
			else
			{
				_Particle1.Play();
				_Particle2.Play();
			}
		}

		public override void Simulate(float p_time)
		{
			_Particle1.Simulate(p_time, true, false);
			_Particle2.Simulate(p_time, true, false);
		}

		private void SetParticleSystemParams(ParticleSystem p_particle, float p_x, float p_y, float p_scaleX, float p_scaleY, float p_scaleZ, string p_sortingLayerName)
		{
			SetParticleSystemParams(p_particle, p_x, p_y, p_sortingLayerName);
			if (p_particle.gameObject.name == "AntiLaserEffectBurst(Clone)")
			{
				p_particle.transform.localScale = new Vector3(p_scaleX, p_scaleY, p_scaleZ);
			}
		}
	}
}
