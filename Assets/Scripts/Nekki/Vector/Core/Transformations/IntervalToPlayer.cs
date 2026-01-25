using UnityEngine;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalToPlayer : PrototypeInterval
	{
		public IntervalToPlayer()
		{
			_Type = IntervalType.ToPlayer;
		}

		public override bool Iteration(TransformInterface Runner)
		{
			if (!Runner.IsEnabled)
			{
				Reset();
				return false;
			}
			if (_Frames == _CurrentFrame || _Frames == 0)
			{
				DebugUtils.Dialog("Error in IntervalToPlayer! _Frames = " + _Frames + ", _CurrentFrame = " + _CurrentFrame + ", hence division by zero.", false);
			}
			float num = 1f - ((float)_Frames - (float)_CurrentFrame - 1f) / (float)(_Frames - _CurrentFrame);
			Vector3 vector = RunMainController.Models[0].Position("COM");
			Vector3f p_delta = (vector - Runner.Position) * num;
			Runner.TransformMove(p_delta);
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
