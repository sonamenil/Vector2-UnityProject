using System.Collections.Generic;

namespace Nekki.Vector.Core.Animation.Events
{
	public class AnimationEventCollision : AnimationEvent
	{
		public enum Type
		{
			Quad = 1,
			Primitive = 2,
			PrimitiveAnimated = 3
		}

		private List<Type> _Types = new List<Type>();

		public List<Type> Types
		{
			get
			{
				return _Types;
			}
		}

		public AnimationEventCollision(List<Type> p_types, AnimationEventParam p_param)
			: base(p_param)
		{
			_Types = p_types;
		}
	}
}
