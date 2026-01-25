using System.Collections.Generic;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Grid
{
	public class GridCell
	{
		private List<QuadRunner> _Quads = new List<QuadRunner>();

		public void Add(QuadRunner p_quad)
		{
			_Quads.Add(p_quad);
		}

		public void Remove(QuadRunner p_quad)
		{
			_Quads.Remove(p_quad);
		}

		public void Collect(List<QuadRunner> p_quads)
		{
			for (int i = 0; i < _Quads.Count; i++)
			{
				QuadRunner quadRunner = _Quads[i];
				if (!quadRunner.IsRender)
				{
					p_quads.Add(quadRunner);
					quadRunner.IsRender = true;
				}
			}
		}
	}
}
