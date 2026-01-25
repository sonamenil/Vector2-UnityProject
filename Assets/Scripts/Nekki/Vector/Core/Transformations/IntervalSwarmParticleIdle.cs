using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSwarmParticleIdle : IntervalSwarm
	{
		private float _MaxDelta;

		private List<Vector3f> _PreviousCoords = new List<Vector3f>();

		private List<Vector3f> _Deltas = new List<Vector3f>();

		private string _StaticObject;

		public IntervalSwarmParticleIdle()
		{
			_Type = IntervalType.SwarmParticleIdle;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_MaxDelta = XmlUtils.ParseFloat(p_node.Attributes["Radius"]);
			_StaticObject = XmlUtils.ParseString(p_node.Attributes["StaticObject"], string.Empty);
		}

		protected override void InitIteration()
		{
			_Swarm.InitializeParticles(_StaticObject);
			for (int i = 0; i < _Swarm.Particles.Count; i++)
			{
				CalcDelta(i);
			}
		}

		private void CalcDelta(int i)
		{
			float num = UnityEngine.Random.Range(0f, _MaxDelta);
			float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			float num2 = num * Mathf.Cos(f);
			float num3 = num * Mathf.Sin(f);
			if (_Deltas.Count <= i)
			{
				_Deltas.Add(new Vector3f(num2 / (float)_Frames, num3 / (float)_Frames, 0f));
				_PreviousCoords.Add(new Vector3f(num2, num3, 0f));
			}
			else
			{
				_Deltas[i].Set((num2 - _PreviousCoords[i].X) / (float)_Frames, (num3 - _PreviousCoords[i].Y) / (float)_Frames, 0f);
				_PreviousCoords[i].Set(num2, num3, 0f);
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
