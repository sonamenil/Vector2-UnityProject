namespace Nekki.Vector.Core.Transformations
{
	public class IntervalDelay : PrototypeInterval
	{
		public IntervalDelay()
		{
			_Type = IntervalType.Delay;
		}

		public override bool Iteration(TransformInterface Runner)
		{
			if (!Runner.IsEnabled)
			{
				Reset();
				return false;
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
