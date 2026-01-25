using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSwarmRotateParticles : IntervalSwarm
	{
		private float _MaxDelta;

		private string _StaticObject;

		private bool _Multiply;

		private float _ParsedAngle;

		private float _DestAngle;

		private float _DeltaAngle;

		public IntervalSwarmRotateParticles()
		{
			_Type = IntervalType.SwarmRotateParticles;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_StaticObject = XmlUtils.ParseString(p_node.Attributes["StaticObject"], string.Empty);
			_ParsedAngle = XmlUtils.ParseFloat(p_node.Attributes["Angle"], 1f);
			_DeltaAngle = _ParsedAngle / (float)_Frames;
		}

		protected override void InitIteration()
		{
			_DestAngle = _ParsedAngle + _Swarm.Rotation;
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
			if (_CurrentFrame == _Frames - 1)
			{
				_Swarm.Rotation = _DestAngle;
			}
			else
			{
				_Swarm.Rotation += _DeltaAngle;
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
