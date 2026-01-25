using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSwarmParticleMoveToSinglePoint : IntervalSwarm
	{
		private List<Vector3f> _Deltas = new List<Vector3f>();

		private float _DestX;

		private float _DestY;

		private string _StaticObject;

		public IntervalSwarmParticleMoveToSinglePoint()
		{
			_Type = IntervalType.SwarmParticleMoveToSinglePoint;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_DestX = XmlUtils.ParseFloat(p_node.Attributes["X"], float.NaN);
			_DestY = XmlUtils.ParseFloat(p_node.Attributes["Y"], float.NaN);
			_StaticObject = XmlUtils.ParseString(p_node.Attributes["StaticObject"], string.Empty);
		}

		protected override void InitIteration()
		{
			if (float.IsNaN(_DestX))
			{
				_DestX = _Swarm.DefaultMoveCoords.X;
			}
			if (float.IsNaN(_DestY))
			{
				_DestY = _Swarm.DefaultMoveCoords.Y;
			}
			if (_CurrentFrame == 0)
			{
				_Swarm.InitializeParticles(_StaticObject);
				for (int i = 0; i < _Swarm.Particles.Count; i++)
				{
					CalcDelta(i);
				}
			}
		}

		private void CalcDelta(int i)
		{
			Vector3f vector3f = new Vector3f(_DestX, _DestY, 0f);
			vector3f *= _Swarm.CurrMatrix.GetInverseMatrix();
			if (_Deltas.Count <= i)
			{
				_Deltas.Add(new Vector3f((vector3f.X - _Swarm.ParticleCoords[i].X) / (float)_Frames, (vector3f.Y - _Swarm.ParticleCoords[i].Y) / (float)_Frames, 0f));
			}
			else
			{
				_Deltas[i].Set((vector3f.X - _Swarm.ParticleCoords[i].X) / (float)_Frames, (vector3f.Y - _Swarm.ParticleCoords[i].Y) / (float)_Frames, 0f);
			}
		}

		public override bool Iteration(TransformInterface p_runner)
		{
			if (!p_runner.IsEnabled || !InitSwarm(p_runner))
			{
				Reset();
				return false;
			}
			InitIteration();
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
