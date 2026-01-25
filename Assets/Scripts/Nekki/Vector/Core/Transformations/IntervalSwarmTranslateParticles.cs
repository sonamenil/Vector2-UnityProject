using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalSwarmTranslateParticles : IntervalSwarm
	{
		private float _MaxDelta;

		private List<Vector3f> Deltas = new List<Vector3f>();

		private string _StaticObject;

		private bool _Multiply;

		private Vector3f _ParsedCoords = new Vector3f(0f, 0f, 0f);

		private Vector3f _DestCoords = new Vector3f(0f, 0f, 0f);

		private Vector3f _DeltaCoords = new Vector3f(0f, 0f, 0f);

		public IntervalSwarmTranslateParticles()
		{
			_Type = IntervalType.SwarmTranslateParticles;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_StaticObject = XmlUtils.ParseString(p_node.Attributes["StaticObject"], string.Empty);
			_ParsedCoords.X = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			_ParsedCoords.Y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
		}

		protected override void InitIteration()
		{
			_DestCoords = _ParsedCoords + _Swarm.Translation;
			_DeltaCoords.Set((_DestCoords.X - _Swarm.Translation.X) / (float)_Frames, (_DestCoords.Y - _Swarm.Translation.Y) / (float)_Frames, 0f);
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
				_Swarm.Translation = _DestCoords;
			}
			else
			{
				_Swarm.Translation += _DeltaCoords;
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
