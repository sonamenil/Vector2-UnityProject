using UnityEngine;

public class Rectangle
{
	public Size Size;

	public Point Origin;

	public float MinX
	{
		get
		{
			return Origin.X;
		}
	}

	public int MinXInt
	{
		get
		{
			return (int)Origin.X;
		}
	}

	public float MidX
	{
		get
		{
			return Origin.X + Size.Width / 2f;
		}
	}

	public float MaxX
	{
		get
		{
			return Origin.X + Size.Width;
		}
	}

	public int MaxXInt
	{
		get
		{
			return (int)(Origin.X + Size.Width);
		}
	}

	public float MinY
	{
		get
		{
			return Origin.Y;
		}
	}

	public int MinYInt
	{
		get
		{
			return (int)Origin.Y;
		}
	}

	public float MidY
	{
		get
		{
			return Origin.Y + Size.Height / 2f;
		}
	}

	public float MaxY
	{
		get
		{
			return Origin.Y + Size.Height;
		}
	}

	public int MaxYInt
	{
		get
		{
			return (int)(Origin.Y + Size.Height);
		}
	}

	public Point TopLeft
	{
		get
		{
			return Origin;
		}
	}

	public Point TopRight
	{
		get
		{
			return new Point(Origin.X + Size.Width, Origin.Y);
		}
	}

	public Point BottomLeft
	{
		get
		{
			return new Point(Origin.X, Origin.Y + Size.Height);
		}
	}

	public Point BottomRight
	{
		get
		{
			return new Point(Origin.X + Size.Width, Origin.Y + Size.Height);
		}
	}

	public Rectangle(float p_x = 0f, float p_y = 0f, float p_width = 0f, float p_height = 0f)
	{
		Origin = new Point(p_x, p_y);
		Size = new Size(p_width, p_height);
	}

	public Rectangle(Rectangle p_rectangle)
	{
		Origin = new Point(p_rectangle.Origin.X, p_rectangle.Origin.Y);
		Size = new Size(p_rectangle.Size.Width, p_rectangle.Size.Height);
	}

	public void Set(float p_x, float p_y, float p_width, float p_height)
	{
		Origin.X = p_x;
		Origin.Y = p_y;
		Size.Width = p_width;
		Size.Height = p_height;
	}

	public void Set(Rectangle p_rectangle)
	{
		Origin.X = p_rectangle.Origin.X;
		Origin.Y = p_rectangle.Origin.Y;
		Size.Width = p_rectangle.Size.Width;
		Size.Height = p_rectangle.Size.Height;
	}

	public bool Contains(Point p_point)
	{
		return p_point.X >= MinX && p_point.X <= MaxX && p_point.Y >= MinY && p_point.Y <= MaxY;
	}

	public bool Contains(Vector3f p_point)
	{
		return p_point.X >= MinX && p_point.X <= MaxX && p_point.Y >= MinY && p_point.Y <= MaxY;
	}

	public bool Contains(Vector3f p_point, float p_epsilon)
	{
		return p_point.X >= MinX - p_epsilon && p_point.X <= MaxX + p_epsilon && p_point.Y >= MinY - p_epsilon && p_point.Y <= MaxY + p_epsilon;
	}

	public bool Intersect(Rectangle p_rect)
	{
		return !(MaxX < p_rect.MinX) && !(p_rect.MaxX < MinX) && !(MaxY < p_rect.MinY) && !(p_rect.MaxY < MinY);
	}

	public static implicit operator Rect(Rectangle p_rect)
	{
		return new Rect(p_rect.MinX, p_rect.MinY, p_rect.Size.Width, p_rect.Size.Height);
	}

	public static implicit operator Rectangle(Rect p_rect)
	{
		return new Rectangle(p_rect.xMin, p_rect.yMin, p_rect.width, p_rect.height);
	}

	public static bool operator ==(Rectangle p_rectangle1, Rectangle p_rectangle2)
	{
		if (object.ReferenceEquals(p_rectangle1, p_rectangle2))
		{
			return true;
		}
		if (object.ReferenceEquals(p_rectangle1, null) || object.ReferenceEquals(p_rectangle2, null))
		{
			return false;
		}
		return p_rectangle1.Origin == p_rectangle2.Origin && p_rectangle1.Size == p_rectangle2.Size;
	}

	public static bool operator !=(Rectangle p_rectangle1, Rectangle p_rectangle2)
	{
		return !(p_rectangle1 == p_rectangle2);
	}
}
