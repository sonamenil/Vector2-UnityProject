using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Generator
{
	public struct GeneratorLabel
	{
		public string Name;

		public int Value;

		private GeneratorLabel(string p_name, int p_value)
		{
			Name = p_name;
			Value = p_value;
		}

		public static List<GeneratorLabel> ParseGeneratorLabel(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			List<GeneratorLabel> list = null;
			if (p_node.ChildNodes.Count != 0)
			{
				list = new List<GeneratorLabel>();
				foreach (XmlNode childNode in p_node.ChildNodes)
				{
					list.Add(new GeneratorLabel(XmlUtils.ParseString(childNode.Attributes["Name"]), XmlUtils.ParseInt(childNode.Attributes["Value"])));
				}
			}
			return list;
		}
	}
}
