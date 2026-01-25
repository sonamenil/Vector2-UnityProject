using System.Xml;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_WaitForModuleActivated : TutorialStep
	{
		public const string ElementName = "WaitForModuleActivated";

		public string ModuleName = string.Empty;

		public TS_WaitForModuleActivated()
		{
			_Type = Type.WaitForModuleActivated;
		}

		public TS_WaitForModuleActivated(XmlNode p_node)
		{
			_Type = Type.WaitForModuleActivated;
			ModuleName = XmlUtils.ParseString(p_node.Attributes["ModuleName"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			UIModule.OnModuleActivated += OnModuleActivated;
		}

		private void OnModuleActivated(UIModule p_module)
		{
			if (!(p_module.name != ModuleName))
			{
				UIModule.OnModuleActivated -= OnModuleActivated;
				Tutorial.Current.StepOver();
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("WaitForModuleActivated");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("ModuleName", ModuleName);
		}
	}
}
