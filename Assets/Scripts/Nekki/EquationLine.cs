namespace Nekki
{
	public class EquationLine
	{
		public float a;

		public float b;

		public float c;

		public EquationLine()
		{
			a = 0f;
			b = 0f;
			c = 0f;
		}

		public EquationLine(float a, float b = 0f, float c = 0f)
		{
			this.a = a;
			this.b = b;
			this.c = c;
		}
	}
}
