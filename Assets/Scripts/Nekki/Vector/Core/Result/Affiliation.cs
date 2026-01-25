using System.Collections.Generic;

namespace Nekki.Vector.Core.Result
{
	public class Affiliation
	{
		public List<Cross> CrossList1;

		public List<Cross> CrossList2;

		public List<Cross> CrossList3;

		public int Type;

		public bool Hits;

		public void Clear()
		{
			CrossList1.Clear();
			CrossList2.Clear();
			CrossList3.Clear();
		}
	}
}
