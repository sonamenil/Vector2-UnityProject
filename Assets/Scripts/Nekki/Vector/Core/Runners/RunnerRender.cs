using System.Collections.Generic;

namespace Nekki.Vector.Core.Runners
{
	public class RunnerRender
	{
		private static RunnerRender _current = new RunnerRender();

		private List<Runner> renderRunners = new List<Runner>();

		public static RunnerRender Current
		{
			get
			{
				return _current;
			}
		}

		public static void FreeMemory()
		{
			_current = null;
		}

		public static void Reset()
		{
			Current.renderRunners.Clear();
		}

		public static void AddRunner(Runner p_runner)
		{
			if (!Current.renderRunners.Contains(p_runner))
			{
				Current.renderRunners.Add(p_runner);
			}
		}

		public static void RenderRunners()
		{
			Current.RenderLocal();
		}

		private void RenderLocal()
		{
			int count = renderRunners.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				if (renderRunners[num].Render())
				{
					renderRunners.RemoveAt(num);
				}
			}
		}
	}
}
