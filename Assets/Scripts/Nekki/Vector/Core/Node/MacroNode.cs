using System.Collections.Generic;

namespace Nekki.Vector.Core.Node
{
	public class MacroNode
	{
		public List<ModelNode> ChildNode;

		public List<float> LCC;

		public MacroNode()
		{
			ChildNode = new List<ModelNode>();
			LCC = new List<float>();
		}
	}
}
