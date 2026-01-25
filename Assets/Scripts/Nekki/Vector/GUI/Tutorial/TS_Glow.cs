using System;
using System.Xml;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_Glow : TutorialStep
	{
		public const string ElementName = "Glow";

		public bool CanShow { get; set; }

		public TS_Glow(XmlNode p_node)
		{
			_Type = Type.Glow;
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
		}

		public override void SaveToXML(XmlNode p_node)
		{
			throw new NotImplementedException();
		}

		private void Setup(XmlNode p_node)
		{
		}

		private void CustomizeItemGlow(ref bool p_runNext)
		{
		}

		private void CustomizeUIGlow(ref bool p_runNext)
		{
		}
	}
}
