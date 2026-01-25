using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationTrickInfo : AnimationInfo
	{
		private List<AnimationInfo> _AnimationParts;

		private static List<AnimationTrickInfo> _TricksLoaded = new List<AnimationTrickInfo>();

		public AnimationTrickInfo(XmlNode p_node)
			: base(p_node)
		{
			_IsTrick = true;
			if (p_node.Attributes["Parts"] != null)
			{
				_AnimationParts = new List<AnimationInfo>();
				string[] array = p_node.Attributes["Parts"].Value.Split('|');
				for (int i = 0; i < array.Length; i++)
				{
					_AnimationParts.Add(Animations.Animation[array[i]]);
				}
			}
		}

		public override void LoadBinary(bool p_useCache)
		{
			base.LoadBinary(p_useCache);
			if (_AnimationParts != null)
			{
				for (int i = 0; i < _AnimationParts.Count; i++)
				{
					_AnimationParts[i].LoadBinary(p_useCache);
				}
			}
		}

		public override void UnloadBinary()
		{
			base.UnloadBinary();
			if (_AnimationParts != null)
			{
				for (int i = 0; i < _AnimationParts.Count; i++)
				{
					_AnimationParts[i].UnloadBinary();
				}
			}
		}

		public static void LoadAnimation(AnimationTrickInfo p_info)
		{
			p_info.LoadBinary(true);
			_TricksLoaded.Add(p_info);
		}

		public static void UnloadTricks()
		{
			for (int i = 0; i < _TricksLoaded.Count; i++)
			{
				_TricksLoaded[i].UnloadBinary();
			}
			_TricksLoaded.Clear();
		}
	}
}
