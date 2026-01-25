using System.Xml;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_CheckQuests : TutorialStep
	{
		public const string ElementName = "CheckQuests";

		public TS_CheckQuests()
		{
			_Type = Type.CheckQuests;
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			QuestManager.Current.CheckEvent(TQE_OnCall.CalledByTutorialEvent);
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement newChild = p_node.OwnerDocument.CreateElement("CheckQuests");
			p_node.AppendChild(newChild);
		}
	}
}
