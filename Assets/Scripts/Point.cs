using Nekki.Vector.Core.Utilites;
using UnityEngine;

public class Point
{
	public static readonly Point ZeroPoint = new Point(0f, 0f);

	private float _X;

	private float _Y;

	public float X
	{
		get
		{
			return _X;
		}
		set
		{
			_X = value;
		}
	}

	public float Y
	{
		get
		{
			return _Y;
		}
		set
		{
			_Y = value;
		}
	}

	public string RangeString
	{
		get
		{
			return string.Format("[{0} {1}]", _X, _Y);
		}
	}

	public Point(Point p_point)
	{
		_X = p_point.X;
		_Y = p_point.Y;
	}

	public Point(float p_x = 0f, float p_y = 0f)
	{
		_X = p_x;
		_Y = p_y;
	}

	public Point Add(Point p_point)
	{
		_X += p_point.X;
		_Y += p_point.Y;
		return this;
	}

	public Point Add(float p_x, float p_y)
	{
		_X += p_x;
		_Y += p_y;
		return this;
	}

	public Point Subtract(Point p_point)
	{
		_X -= p_point.X;
		_Y -= p_point.Y;
		return this;
	}

	public Point Multiply(float p_value)
	{
		_X *= p_value;
		_Y *= p_value;
		return this;
	}

	public void Set(Point p_point)
	{
		_X = p_point._X;
		_Y = p_point._Y;
	}

	public void Round(int p_pow)
	{
		_X = MathUtils.Round(_X, p_pow);
	}

	public void IRound()
	{
		_X = MathUtils.Round(_X, 1f);
	}

	public float DistBP(Point p_point)
	{
		return (_X - p_point._X) * (_X - p_point._Y) + (_Y - p_point._Y) * (_Y - p_point._Y);
	}

	public override string ToString()
	{
		return string.Format("[Point: X={0}, Y={1}]", X, Y);
	}

	public static Point operator -(Point p_point1, Point p_point2)
	{
		return new Point(p_point1.X - p_point2.X, p_point1.Y - p_point2.Y);
	}

	public static Point operator +(Point p_point1, Point p_point2)
	{
		return new Point(p_point1.X + p_point2.X, p_point1.Y + p_point2.Y);
	}

	public static bool operator ==(Point p_point1, Point p_point2)
	{
		if (object.ReferenceEquals(p_point1, p_point2))
		{
			return true;
		}
		if (object.ReferenceEquals(p_point1, null) || object.ReferenceEquals(p_point2, null))
		{
			return false;
		}
		return p_point1.X == p_point2.X && p_point1.Y == p_point2.Y;
	}

	public static bool operator !=(Point p_point1, Point p_point2)
	{
		return !(p_point1 == p_point2);
	}

	public static implicit operator Vector3(Point p_point)
	{
		return new Vector3(p_point.X, p_point.Y, 0f);
	}
}
