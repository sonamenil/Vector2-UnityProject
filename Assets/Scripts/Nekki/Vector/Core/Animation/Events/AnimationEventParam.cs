using System.Collections.Generic;

namespace Nekki.Vector.Core.Animation.Events
{
	public class AnimationEventParam
	{
		private List<AnimationReaction> _Reaction;

		private List<AnimationSound> _Sound;

		public List<AnimationReaction> Reaction
		{
			get
			{
				return _Reaction;
			}
		}

		public List<AnimationSound> Sound
		{
			get
			{
				return _Sound;
			}
		}

		public AnimationEventParam(List<AnimationReaction> Reaction, List<AnimationSound> Sound)
		{
			_Reaction = Reaction;
			_Sound = Sound;
		}
	}
}
