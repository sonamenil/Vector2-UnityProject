namespace Nekki.Vector.Core.Result
{
	public class Cross
	{
		private int _Index;

		private Vector3f _Point = new Vector3f(0f, 0f, 0f);

		public int Index
		{
			get
			{
				return _Index;
			}
		}

		public Vector3f Point
		{
			get
			{
				return _Point;
			}
		}

		public Cross(Vector3f Point, int Index)
		{
			_Index = Index;
			_Point = Point;
		}
	}
}
