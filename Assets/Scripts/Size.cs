public class Size
{
	public float Width;

	public float Height;

	public Size(float p_width = 0f, float p_height = 0f)
	{
		Width = p_width;
		Height = p_height;
	}

	public Size(Size p_size)
	{
		Width = p_size.Width;
		Height = p_size.Height;
	}

	public void Set(float p_width, float p_height)
	{
		Width = p_width;
		Height = p_height;
	}

	public override string ToString()
	{
		return string.Format("[Size: W={0}, H={1}]", Width, Height);
	}

	public static bool operator ==(Size p_size1, Size p_size2)
	{
		return p_size1.Width == p_size2.Width && p_size1.Height == p_size2.Height;
	}

	public static bool operator !=(Size p_size1, Size p_size2)
	{
		return !(p_size1 == p_size2);
	}
}
