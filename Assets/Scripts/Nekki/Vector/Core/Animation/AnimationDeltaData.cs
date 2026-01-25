using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Animation
{
	public class AnimationDeltaData
	{
		private QuadRunner _Platform;

		private Vector3f _Position = new Vector3f(0f, 0f, 0f);

		private int _Sign;

		public QuadRunner Platform
		{
			get
			{
				return _Platform;
			}
		}

		public Vector3f Position
		{
			get
			{
				return _Position;
			}
		}

		public int Sign
		{
			get
			{
				return _Sign;
			}
		}

		public AnimationDeltaData(QuadRunner p_platform, Vector3f p_position, int p_sign)
		{
			_Platform = p_platform;
			_Position = p_position;
			_Sign = p_sign;
		}

		public Rectangle GetRectangle(int p_index)
		{
			Vector3f vector = GetVector(p_index);
			return new Rectangle(vector.X, vector.Y, _Platform.WidthQuad, _Platform.HeightQuad);
		}

		public Vector3f GetVector(int p_index)
		{
			return _Position - _Platform.GetCornerByIndex(p_index);
		}

		public float GetDeltaValue(AnimationDeltaName p_name, int p_corner, int p_sign, Vector3f p_velocity)
		{
			switch (p_name)
			{
			case AnimationDeltaName.Width:
				return _Platform.GetSize(p_sign).X;
			case AnimationDeltaName.Height:
				return _Platform.GetSize(p_sign).Y;
			case AnimationDeltaName.DeltaX:
			{
				float num = GetVector(p_corner).X;
				if (p_sign == -1)
				{
					num *= -1f;
				}
				return num;
			}
			case AnimationDeltaName.DeltaY:
				return GetVector(p_corner).Y;
			case AnimationDeltaName.VelocityX:
				return p_velocity.X;
			case AnimationDeltaName.VelocityY:
				return p_velocity.Y;
			default:
				return 0f;
			}
		}

		public string DeltaToString(int p_corner)
		{
			Vector3f vector = GetVector(p_corner);
			return "[delta" + p_corner + " X:" + vector.X + ", Y:" + vector.Y + "]";
		}
	}
}
