using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core
{
	public static class LayersController
	{
		public const string DebugLayer = "Debug";

		public const string ModelLayer = "Model";

		private const string _LayersFileName = "layers.xml";

		private static Dictionary<string, float> _LayersK;

		public static void Init()
		{
			if (_LayersK == null)
			{
				_LayersK = new Dictionary<string, float>();
				ParseLayers();
			}
		}

		private static void ParseLayers()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.Textures, "layers.xml");
			XmlNode xmlNode = xmlDocument["Layers"];
			foreach (XmlNode item in xmlNode)
			{
				_LayersK.Add(item.Attributes["Name"].Value, XmlUtils.ParseFloat(item.Attributes["FactorVerticalK"], 1f));
			}
		}

		public static float GetK(string p_layer)
		{
			return (!_LayersK.ContainsKey(p_layer)) ? 0f : _LayersK[p_layer];
		}
	}
}
