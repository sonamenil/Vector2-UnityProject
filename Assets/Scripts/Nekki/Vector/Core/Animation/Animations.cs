using System.Collections.Generic;

namespace Nekki.Vector.Core.Animation
{
	public static class Animations
	{
		private static Dictionary<string, AnimationInfo> _Animations = new Dictionary<string, AnimationInfo>();

		public static Dictionary<string, AnimationInfo> Animation
		{
			get
			{
				return _Animations;
			}
		}

		public static List<AnimationInfo> ToList()
		{
			List<AnimationInfo> list = new List<AnimationInfo>();
			foreach (AnimationInfo value in _Animations.Values)
			{
				list.Add(value);
			}
			return list;
		}
	}
}
