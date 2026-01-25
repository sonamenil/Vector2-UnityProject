using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSwarmParticleShuffle : IntervalSwarm
	{
		private List<Vector3f> _Deltas = new List<Vector3f>();

		private float _Radius = 100f;

		private int _ParticlesCount;

		private int _ParticlesCountValue;

		private string _StaticObject;

		public IntervalSwarmParticleShuffle()
		{
			_Type = IntervalType.SwarmParticleShuffle;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_Radius = XmlUtils.ParseFloat(p_node.Attributes["Radius"]);
			_StaticObject = XmlUtils.ParseString(p_node.Attributes["StaticObject"], string.Empty);
			_ParticlesCountValue = XmlUtils.ParseInt(p_node.Attributes["ParticlesCount"]);
		}

		protected override void InitIteration()
		{
			_Swarm.InitializeParticles(_StaticObject);
			if (_ParticlesCountValue == 0)
			{
				_ParticlesCountValue = _Swarm.Particles.Count;
			}
			if (_ParticlesCount == 0)
			{
				_ParticlesCount = _ParticlesCountValue;
			}
			for (int i = 0; i < _Swarm.Particles.Count; i++)
			{
				CalcDelta(i);
			}
		}

		private void CalcDelta(int i)
		{
			float num = UnityEngine.Random.Range(0f, _Radius);
			float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			float num2 = num * Mathf.Cos(f);
			float num3 = num * Mathf.Sin(f);
			float num4 = UnityEngine.Random.Range(0f, 1f);
			if (num4 > (float)_ParticlesCount / ((float)_Swarm.Particles.Count - (float)i))
			{
				num2 = _Swarm.ParticleCoords[i].X;
				num3 = _Swarm.ParticleCoords[i].Y;
			}
			else
			{
				_ParticlesCount--;
			}
			if (_Deltas.Count <= i)
			{
				_Deltas.Add(new Vector3f((num2 - _Swarm.ParticleCoords[i].X) / (float)_Frames, (num3 - _Swarm.ParticleCoords[i].Y) / (float)_Frames, 0f));
			}
			else
			{
				_Deltas[i].Set((num2 - _Swarm.ParticleCoords[i].X) / (float)_Frames, (num3 - _Swarm.ParticleCoords[i].Y) / (float)_Frames, 0f);
			}
		}

		public override bool Iteration(TransformInterface p_runner)
		{
			if (!p_runner.IsEnabled || !InitSwarm(p_runner))
			{
				Reset();
				return false;
			}
			if (_CurrentFrame == 0)
			{
				InitIteration();
			}
			for (int i = 0; i < _Swarm.Particles.Count; i++)
			{
				List<Vector3f> particleCoords;
				List<Vector3f> list = (particleCoords = _Swarm.ParticleCoords);
				int index;
				int index2 = (index = i);
				Vector3f vector3f = particleCoords[index];
				list[index2] = vector3f + _Deltas[i];
				_Swarm.Particles[i].TransformMove(_Deltas[i] * _Swarm.CurrMatrix);
				_Swarm.Particles[i].SetDeltaMove();
			}
			_CurrentFrame++;
			if (_CurrentFrame >= _Frames)
			{
				Reset();
				return false;
			}
			return true;
		}
	}
}
