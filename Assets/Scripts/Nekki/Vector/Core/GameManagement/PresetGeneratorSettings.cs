using System.Xml;

namespace Nekki.Vector.Core.GameManagement
{
	public class PresetGeneratorSettings
	{
		private string _Name;

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public PresetGeneratorSettings(XmlNode p_node)
		{
			_Name = p_node.Attributes["Name"].Value;
		}
	}
}
