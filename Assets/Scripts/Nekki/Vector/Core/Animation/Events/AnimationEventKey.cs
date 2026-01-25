using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Controllers;

namespace Nekki.Vector.Core.Animation.Events
{
	public class AnimationEventKey : AnimationEvent
	{
		private Key _Key;

		public AnimationEventKey(AnimationEventParam p_param, XmlNode p_node)
			: base(p_param)
		{
			string value = p_node.Attributes["Key"].Value;
			_Key = KeyVariables.Parse(value);
		}

		public bool IsKey(KeyVariables p_keysVariables, int p_sign)
		{
			if (p_sign == -1 && p_keysVariables.Key == Key.Right)
			{
				return _Key == Key.Left;
			}
			if (p_sign == -1 && p_keysVariables.Key == Key.Left)
			{
				return _Key == Key.Right;
			}
			return _Key == p_keysVariables.Key;
		}

		public List<int> ToList(Point p_point, int p_sign)
		{
			List<int> list = new List<int>();
			if (p_point.X > 0f)
			{
				list.Add(Reverse((int)p_point.X, p_sign));
			}
			if (p_point.Y > 0f)
			{
				list.Add(Reverse((int)p_point.Y, p_sign));
			}
			return list;
		}

		public static int Reverse(int p_value, int p_sign)
		{
			if (p_sign == -1 && p_value == 4)
			{
				p_value = 2;
			}
			else if (p_sign == -1 && p_value == 2)
			{
				p_value = 4;
			}
			return p_value;
		}
	}
}
