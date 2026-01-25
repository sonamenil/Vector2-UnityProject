namespace Nekki.Vector.Core.Animation
{
	public class AnimationDelta
	{
		private AnimationDeltaName _Name;

		private AnimationDeltaType _Type;

		private Point _Value;

		private int _Corner;

		public Point Value
		{
			get
			{
				return _Value;
			}
		}

		public AnimationDeltaName Name
		{
			get
			{
				return _Name;
			}
		}

		public AnimationDeltaType Type
		{
			get
			{
				return _Type;
			}
		}

		public AnimationDelta(AnimationDeltaName p_name, AnimationDeltaType p_type, Point p_value, int p_corner)
		{
			_Name = p_name;
			_Type = p_type;
			_Value = p_value;
			_Corner = p_corner;
		}

		public int GetCorner(int p_sign)
		{
			if (p_sign == -1)
			{
				switch (_Corner)
				{
				case 0:
					return 1;
				case 1:
					return 0;
				case 2:
					return 3;
				case 3:
					return 2;
				}
			}
			return _Corner;
		}

		public bool IsInterval(float p_value)
		{
			return More(p_value, _Value.X) && Less(p_value, _Value.Y);
		}

		public static bool More(float p_target, float p_value)
		{
			return float.IsNaN(p_value) || p_target >= p_value;
		}

		public static bool Less(float p_target, float p_value)
		{
			return float.IsNaN(p_value) || p_target <= p_value;
		}
	}
}
