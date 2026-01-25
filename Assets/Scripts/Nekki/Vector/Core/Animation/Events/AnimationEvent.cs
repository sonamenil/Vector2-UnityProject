using System.Collections.Generic;

namespace Nekki.Vector.Core.Animation.Events
{
	public class AnimationEvent
	{
		private AnimationEventParam _Param;

		public AnimationEventParam Param
		{
			get
			{
				return _Param;
			}
		}

		public List<AnimationReaction> Reaction
		{
			get
			{
				return _Param.Reaction;
			}
		}

		public List<AnimationSound> Sound
		{
			get
			{
				return _Param.Sound;
			}
		}

		public AnimationEvent(AnimationEventParam p_param)
		{
			_Param = p_param;
		}
	}
}
