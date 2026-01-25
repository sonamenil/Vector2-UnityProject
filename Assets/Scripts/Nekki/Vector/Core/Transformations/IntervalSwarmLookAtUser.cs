using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSwarmLookAtUser : IntervalSwarm
	{
		private class ParticleRotationAction
		{
			private IntervalSwarmLookAtUser _Parent;

			private TransformInterface _Particle;

			private int _Frames;

			private float _DeltaZ;

			private float _DeltaY;

			public float RotationZ
			{
				get
				{
					return _Particle.UnityObject.transform.eulerAngles.z;
				}
			}

			public float AngleBPM
			{
				get
				{
					return MathUtils.AngleBetweenPoints(_Particle.Position, _Parent._ModelPosition);
				}
			}

			public ParticleRotationAction(IntervalSwarmLookAtUser p_parent, TransformInterface p_particle)
			{
				_Parent = p_parent;
				_Particle = p_particle;
				_Frames = _Parent.Frames;
			}

			private void CalcDeltaZ()
			{
				float num = Mathf.DeltaAngle(RotationZ, AngleBPM);
				_DeltaZ = num / (float)(_Frames - _Parent._CurrentFrame);
			}

			public void CalcDeltaY()
			{
				_DeltaY = 36f;
				_Frames += 5;
			}

			public bool Iteration()
			{
				if (_Parent._CurrentFrame < _Frames)
				{
					if (_DeltaY > 0f && _Parent._CurrentFrame < 5)
					{
						_Particle.TransformRotateY(_DeltaY);
						return false;
					}
					CalcDeltaZ();
					if (MathUtils.NormalizeAngle(RotationZ + _DeltaZ) < 90f || MathUtils.NormalizeAngle(RotationZ + _DeltaZ) > 270f)
					{
						_Particle.TransformRotateZ(_DeltaZ);
					}
					return false;
				}
				return true;
			}
		}

		private const int _RotationYFrames = 5;

		private const float _RotationYOffset = 100f;

		private const float _RotationExtraMin = 90f;

		private const float _RotationExtraMax = 270f;

		public static int _Total;

		public static int _Y;

		private string _StaticObject;

		private List<ParticleRotationAction> _RotationActions = new List<ParticleRotationAction>();

		private Vector2 _ModelPosition;

		private bool NeedRotationY
		{
			get
			{
				int sign = _Swarm.Sign;
				float x = _ModelPosition.x;
				float x2 = _Swarm.Position.x;
				return (sign == 1 && x < x2 - 100f) || (sign == -1 && x > x2 + 100f);
			}
		}

		public IntervalSwarmLookAtUser()
		{
			_Type = IntervalType.SwarmLookAtUser;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_StaticObject = XmlUtils.ParseString(p_node.Attributes["StaticObject"], string.Empty);
		}

		private void CalcDeltaY()
		{
			int i = 0;
			for (int count = _RotationActions.Count; i < count; i++)
			{
				_RotationActions[i].CalcDeltaY();
			}
		}

		protected override void InitIteration()
		{
			_Swarm.InitializeParticles(_StaticObject);
			_RotationActions.Clear();
			_ModelPosition = RunMainController.Player.Node("COM").Start.ToVector2();
			int i = 0;
			for (int count = _Swarm.Particles.Count; i < count; i++)
			{
				_RotationActions.Add(new ParticleRotationAction(this, _Swarm.Particles[i]));
			}
			if (NeedRotationY)
			{
				_Swarm.Sign = ((_Swarm.Sign != 1) ? 1 : (-1));
				CalcDeltaY();
			}
		}

		private bool RegularIteration()
		{
			_ModelPosition = RunMainController.Player.Node("COM").Start.ToVector2();
			bool flag = true;
			int i = 0;
			for (int count = _RotationActions.Count; i < count; i++)
			{
				flag = _RotationActions[i].Iteration() && flag;
			}
			return flag;
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
			bool flag = RegularIteration();
			_CurrentFrame++;
			if (flag)
			{
				Reset();
				return false;
			}
			return true;
		}
	}
}
