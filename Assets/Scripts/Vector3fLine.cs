using UnityEngine;

public class Vector3fLine
{
	protected Vector3f _Start;

	protected Vector3f _End;

	protected float _Stroke = 1f;

	protected Color _Color = new Color(0f, 0f, 0f, 1f);

	public Vector3f Start
	{
		get
		{
			return _Start;
		}
		set
		{
			_Start = value;
		}
	}

	public Vector3f End
	{
		get
		{
			return _End;
		}
		set
		{
			_End = value;
		}
	}

	public float Stroke
	{
		get
		{
			return _Stroke;
		}
		set
		{
			_Stroke = value;
		}
	}

	public Color Color
	{
		get
		{
			return _Color;
		}
		set
		{
			_Color = value;
		}
	}

	public double Distance
	{
		get
		{
			return Vector3f.Distance(_Start, _End);
		}
	}

	public Vector3fLine()
	{
	}

	public Vector3fLine(Vector3f p_start, Vector3f p_end)
	{
		_Start = new Vector3f(p_start);
		_End = new Vector3f(p_end);
	}

	public void Set(Vector3f p_start, Vector3f p_end)
	{
		_Start.Set(p_start);
		_End.Set(p_end);
	}

	public void SetZerroOnZ()
	{
		_Start.Z = 0f;
		_End.Z = 0f;
	}

	public static Vector3f CrossLine(Vector3fLine p_line1, Vector3fLine p_line2)
	{
		return Vector3f.Cross(p_line1.Start, p_line1.End, p_line2.Start, p_line2.End);
	}

	public bool CroosLine(Point p_point1, Point p_point2)
	{
		float num = (p_point2.Y - p_point1.Y) * (_Start.X - End.X) - (_Start.Y - _End.Y) * (p_point2.X - p_point1.X);
		float num2 = (p_point2.Y - p_point1.Y) * (_Start.X - p_point1.X) - (_Start.Y - p_point1.Y) * (p_point2.X - p_point1.X);
		float num3 = (_Start.Y - p_point1.Y) * (_Start.X - _End.X) - (_Start.Y - _End.Y) * (_Start.X - p_point1.X);
		if ((double)num == 0.0 && (double)num2 == 0.0 && (double)num3 == 0.0)
		{
			return false;
		}
		if ((double)num == 0.0)
		{
			return false;
		}
		float num4 = num2 / num;
		float num5 = num3 / num;
		if (0f < num4 && num4 < 1f && 0f < num5 && num5 < 1f)
		{
			return true;
		}
		return false;
	}
}
