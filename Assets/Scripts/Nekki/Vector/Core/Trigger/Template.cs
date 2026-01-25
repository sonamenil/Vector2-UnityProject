using System.Xml;

namespace Nekki.Vector.Core.Trigger
{
	public class Template
	{
		public XmlNode mainTemplateNode;

		public Template(XmlNode p_node)
		{
			mainTemplateNode = p_node;
		}
	}
}
