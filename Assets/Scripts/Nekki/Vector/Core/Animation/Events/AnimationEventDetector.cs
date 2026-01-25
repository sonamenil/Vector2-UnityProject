using Nekki.Vector.Core.Detector;

namespace Nekki.Vector.Core.Animation.Events
{
	public class AnimationEventDetector : AnimationEvent
	{
		private DetectorEvent.DetectorEventType _Type;

		public DetectorEvent.DetectorEventType Type
		{
			get
			{
				return _Type;
			}
		}

		public AnimationEventDetector(DetectorEvent.DetectorEventType p_type, AnimationEventParam p_param)
			: base(p_param)
		{
			_Type = p_type;
		}
	}
}
