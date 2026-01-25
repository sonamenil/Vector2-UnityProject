using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSwarmResize : IntervalSwarm
	{
		private float _MaxDelta;

		private string _StaticObject;

		private bool _Multiply;

		private Vector3f _DestCoords = new Vector3f(0f, 0f, 0f);

		private Vector3f _DeltaCoords = new Vector3f(0f, 0f, 0f);

		public IntervalSwarmResize()
		{
			_Type = IntervalType.SwarmRotateParticles;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_StaticObject = XmlUtils.ParseString(p_node.Attributes["StaticObject"], string.Empty);
			_DestCoords.Set(XmlUtils.ParseFloat(p_node.Attributes["X"], 1f), XmlUtils.ParseFloat(p_node.Attributes["Y"], 1f), 0f);
		}

		protected override void InitIteration()
		{
			_DeltaCoords.Set((_DestCoords.X - _Swarm.Scale.X) / (float)_Frames, (_DestCoords.Y - _Swarm.Scale.Y) / (float)_Frames, 0f);
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
				_Swarm.Scale = _DestCoords;
			}
			else
			{
				_Swarm.Scale += _DeltaCoords;
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
