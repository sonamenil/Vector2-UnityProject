using System.Xml;
using Nekki.Vector.GUI;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	internal class TQC_CheckScreenType : TriggerQuestCondition
	{
		private string _ScreenName;

		public TQC_CheckScreenType(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_node, p_parent)
		{
			_ScreenName = XmlUtils.ParseString(p_node.Attributes["ScreenName"]);
		}

		public override bool Check()
		{
			bool flag = Manager.CurrentScreen.ToString() == _ScreenName;
			return (!_IsNot) ? flag : (!flag);
		}
	}
}
