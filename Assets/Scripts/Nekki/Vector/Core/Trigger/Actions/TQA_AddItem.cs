using System.Xml;
using Nekki.Vector.Core.Quest;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_AddItem : TriggerQuestAction
	{
		private string _Preset;

		public TQA_AddItem(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_Preset = XmlUtils.ParseString(p_node.Attributes["Preset"], string.Empty);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			QuestManager.Current.GetReward(_Preset);
		}
	}
}
