namespace Nekki.Vector.Core.Animation.Events
{
	public class AnimationEventFrame : AnimationEvent
	{
		private int _Frame;

		public int Frame
		{
			get
			{
				return _Frame;
			}
		}

		public AnimationEventFrame(int p_frame, AnimationEventParam p_param)
			: base(p_param)
		{
			_Frame = p_frame;
		}
	}
}
