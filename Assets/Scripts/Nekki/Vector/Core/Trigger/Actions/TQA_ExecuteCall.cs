using System.Xml;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_ExecuteCall : TriggerQuestAction
	{
		private Variable _Message;

		public TQA_ExecuteCall(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			if (p_node.Attributes["Message"] != null)
			{
				_Message = Variable.CreateVariable(p_node.Attributes["Message"].Value, null);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			QuestManager.Current.CheckEvent(TQE_OnCall.CalledByTriggerEvent(_Message));
		}

		public override string ToString()
		{
			return "ExecuteCall";
		}
	}
}
