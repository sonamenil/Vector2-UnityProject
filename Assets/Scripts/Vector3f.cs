using System;
using System.Xml;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

public class Vector3f
{
	private float _X;

	private float _Y;

	private float _Z;

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

	public float Z
	{
		get
		{
			return _Z;
		}
		set
		{
			_Z = value;
		}
	}

	public float Length
	{
		get
		{
			return Mathf.Sqrt(_X * _X + _Y * _Y + _Z * _Z);
		}
	}

	public float LengthXY
	{
		get
		{
			return Mathf.Sqrt(_X * _X + _Y * _Y);
		}
	}

	public static Vector3f Right
	{
		get
		{
			return new Vector3f(1f, 0f, 0f);
		}
	}

	public static Vector3f Up
	{
		get
		{
			return new Vector3f(0f, 1f, 0f);
		}
	}

	public static Vector3f Forward
	{
		get
		{
			return new Vector3f(0f, 1f, 0f);
		}
	}

	public static Vector3f Zero
	{
		get
		{
			return new Vector3f(0f, 0f, 0f);
		}
	}

	public static Vector3f One
	{
		get
		{
			return new Vector3f(1f, 1f, 1f);
		}
	}

	public Vector3f(float p_x = 0f, float p_y = 0f, float p_z = 0f)
	{
		_X = p_x;
		_Y = p_y;
		_Z = p_z;
	}

	public Vector3f(Vector3f p_vector)
	{
		_X = p_vector.X;
		_Y = p_vector.Y;
		_Z = p_vector.Z;
	}

	public Vector3f(Vector3 p_vector)
	{
		_X = p_vector.x;
		_Y = p_vector.y;
		_Z = p_vector.z;
	}

	public static float Distance(Vector3f p_vector1, Vector3f p_vector2)
	{
		return (p_vector2 - p_vector1).Length;
	}

	public float Distance(Vector3f p_vector)
	{
		return (this - p_vector).Length;
	}

	public Vector3f Add(float p_value)
	{
		_X += p_value;
		_Y += p_value;
		_Z += p_value;
		return this;
	}

	public Vector3f Add(float p_x, float p_y, float p_z)
	{
		_X += p_x;
		_Y += p_y;
		_Z += p_z;
		return this;
	}

	public Vector3f Add(Vector3f p_vector)
	{
		_X += p_vector.X;
		_Y += p_vector.Y;
		_Z += p_vector.Z;
		return this;
	}

	public Vector3f Add(Vector3f p_vector, float p_multy)
	{
		_X += p_vector.X * p_multy;
		_Y += p_vector.Y * p_multy;
		_Z += p_vector.Z * p_multy;
		return this;
	}

	public Vector3f Add(Point p_point)
	{
		_X += p_point.X;
		_Y += p_point.Y;
		return this;
	}

	public Vector3f Subtract(float p_value)
	{
		_X -= p_value;
		_Y -= p_value;
		_Z -= p_value;
		return this;
	}

	public Vector3f Subtract(float p_x, float p_y, float p_z)
	{
		_X -= p_x;
		_Y -= p_y;
		_Z -= p_z;
		return this;
	}

	public Vector3f Subtract(Vector3f p_vector)
	{
		_X -= p_vector.X;
		_Y -= p_vector.Y;
		_Z -= p_vector.Z;
		return this;
	}

	public Vector3f Subtract(Point p_point)
	{
		_X -= p_point.X;
		_Y -= p_point.Y;
		return this;
	}

	public Vector3f Multiply(float p_value)
	{
		_X *= p_value;
		_Y *= p_value;
		_Z *= p_value;
		return this;
	}

	public Vector3f Multiply(float p_x, float p_y, float p_z)
	{
		_X *= p_x;
		_Y *= p_y;
		_Z *= p_z;
		return this;
	}

	public Vector3f Cross(Vector3f p_vector)
	{
		return new Vector3f(_Y * p_vector.Z - _Z * p_vector.Y, _Z * p_vector.X - _X * p_vector.Z, _X * p_vector.Y - _Y - p_vector.X);
	}

	public static Vector3f Cross(Vector3f p_point1, Vector3f p_point2, Vector3f p_point3, Vector3f p_point4)
	{
		float num = (p_point2.Y - p_point1.Y) * (p_point3.X - p_point4.X) - (p_point3.Y - p_point4.Y) * (p_point2.X - p_point1.X);
		float num2 = (p_point2.Y - p_point1.Y) * (p_point3.X - p_point1.X) - (p_point3.Y - p_point1.Y) * (p_point2.X - p_point1.X);
		float num3 = (p_point3.Y - p_point1.Y) * (p_point3.X - p_point4.X) - (p_point3.Y - p_point4.Y) * (p_point3.X - p_point1.X);
		if ((double)num == 0.0 && (double)num2 == 0.0 && (double)num3 == 0.0)
		{
			return null;
		}
		if ((double)num == 0.0)
		{
			return null;
		}
		float num4 = num2 / num;
		float num5 = num3 / num;
		float p_x = p_point1.X + (p_point2.X - p_point1.X) * num5;
		float p_y = p_point1.Y + (p_point2.Y - p_point1.Y) * num5;
		if (0f < num4 && num4 < 1f && 0f < num5 && num5 < 1f)
		{
			return new Vector3f(p_x, p_y, 0f);
		}
		return null;
	}

	public static Vector3f Middle(Vector3f p_point1, Vector3f p_point2)
	{
		return p_point1 + (p_point2 - p_point1) * 0.5f;
	}

	public static void Middle(Vector3f p_point1, Vector3f p_point2, Vector3f p_result)
	{
		p_result._X = p_point1._X + (p_point2._X - p_point1._X) * 0.5f;
		p_result._Y = p_point1._Y + (p_point2._Y - p_point1._Y) * 0.5f;
		p_result._Z = p_point1._Z + (p_point2._Z - p_point1._Z) * 0.5f;
	}

	public static Vector3f Closest(Vector3f p_point, Vector3f p_linePoint, Vector3f p_lineDirection)
	{
		Vector3f vector3f = p_point - p_linePoint;
		float num = vector3f * p_lineDirection;
		float num2 = p_lineDirection * p_lineDirection;
		float num3 = 0f;
		if (num2 != 0f)
		{
			num3 = num / num2;
		}
		return p_linePoint + p_lineDirection * num3;
	}

	public void Reset()
	{
		_X = 0f;
		_Y = 0f;
		_Z = 0f;
	}

	public static Vector3f Round(Vector3f p_vector, float p_pow)
	{
		p_vector.X = MathUtils.Round(p_vector.X, p_pow);
		p_vector.Y = MathUtils.Round(p_vector.Y, p_pow);
		p_vector.Z = MathUtils.Round(p_vector.Z, p_pow);
		return p_vector;
	}

	public Vector3f Round(float p_pow)
	{
		_X = MathUtils.Round(_X, p_pow);
		_Y = MathUtils.Round(_Y, p_pow);
		_Z = MathUtils.Round(_Z, p_pow);
		return this;
	}

	public Vector3f Normalize()
	{
		float num = Length;
		if (num != 0f)
		{
			num = 1f / num;
		}
		_X *= num;
		_Y *= num;
		_Z *= num;
		return this;
	}

	public static Vector3f Normal(Vector3f p_vector1, Vector3f p_vector2)
	{
		return (p_vector1 - p_vector2).Cross(Forward).Normalize();
	}

	public static float Factor(Vector3f p_impulse, Vector3f p_start, Vector3f p_end)
	{
		p_start.Z = 0f;
		p_impulse.Z = 0f;
		p_end.Z = 0f;
		return Distance(p_start, p_end) / Distance(p_start, p_impulse);
	}

	public Vector3f Clone()
	{
		return new Vector3f(_X, _Y, _Z);
	}

	public Vector3f Set(Vector3f p_vector)
	{
		_X = p_vector.X;
		_Y = p_vector.Y;
		_Z = p_vector.Z;
		return this;
	}

	public Vector3f Set(Vector3 p_vector)
	{
		_X = p_vector.x;
		_Y = p_vector.y;
		_Z = p_vector.z;
		return this;
	}

	public Vector3f Set(float p_x = 0f, float p_y = 0f, float p_z = 0f)
	{
		_X = p_x;
		_Y = p_y;
		_Z = p_z;
		return this;
	}

	public void RoundToInt()
	{
		_X = (int)_X;
		_Y = (int)_Y;
		_Z = (int)_Z;
	}

	public static Vector3f Create(XmlNode p_node)
	{
		if (p_node == null)
		{
			return null;
		}
		Vector3f vector3f = new Vector3f(0f, 0f, 0f);
		try
		{
			vector3f._X = float.Parse(p_node.Attributes["X"].Value);
		}
		catch
		{
			throw new Exception("Error : parse X eeror type");
		}
		try
		{
			vector3f._Y = float.Parse(p_node.Attributes["Y"].Value);
			return vector3f;
		}
		catch
		{
			throw new Exception("Error : parse Y eeror type");
		}
	}

	public override string ToString()
	{
		return "X=" + _X + " Y=" + _Y + " Z=" + _Z;
	}

	public Vector3 ToVector2()
	{
		return new Vector2((int)_X, (int)_Y);
	}

	public Vector3 ToVector3()
	{
		return new Vector3((int)_X, (int)_Y, (int)_Z);
	}

	public static Vector3f operator +(Vector3f p_vector, float p_value)
	{
		return new Vector3f(p_vector.X + p_value, p_vector.Y + p_value, p_vector.Z + p_value);
	}

	public static Vector3f operator +(Vector3f p_vector1, Vector3f p_vector2)
	{
		return new Vector3f(p_vector1.X + p_vector2.X, p_vector1.Y + p_vector2.Y, p_vector1.Z + p_vector2.Z);
	}

	public static Vector3f operator -(Vector3f p_vector, float p_value)
	{
		return new Vector3f(p_vector.X - p_value, p_vector.Y - p_value, p_vector.Z - p_value);
	}

	public static Vector3f operator -(Vector3f p_vector1, Vector3f p_vector2)
	{
		return new Vector3f(p_vector1.X - p_vector2.X, p_vector1.Y - p_vector2.Y, p_vector1.Z - p_vector2.Z);
	}

	public static Vector3f operator *(Vector3f p_vector, float p_value)
	{
		return new Vector3f(p_vector.X * p_value, p_vector.Y * p_value, p_vector.Z * p_value);
	}

	public static float operator *(Vector3f p_vector1, Vector3f p_vector2)
	{
		return p_vector1.X * p_vector2.X + p_vector1.Y * p_vector2.Y + p_vector1.Z * p_vector2.Z;
	}

	public static implicit operator Vector3(Vector3f p_vector)
	{
		return new Vector3(p_vector.X, p_vector.Y, p_vector.Z);
	}

	public static implicit operator Vector3f(Vector3 p_vector)
	{
		return new Vector3f(p_vector.x, p_vector.y, p_vector.z);
	}
}
