using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_Line : TriggerRunnerEvent
	{
		private Variable _Line;

		private static int _CounterID;

		private string _TypeStr;

		public int ID { get; set; }

		public TRE_Line(TriggerRunnerLoop p_parent, XmlNode p_node, int p_ID = -1)
		{
			_Type = EventType.TRE_LINE;
			if (p_parent == null)
			{
				ID = p_ID;
				return;
			}
			ID = _CounterID++;
			string value = p_node.Attributes["Position"].Value;
			if (value[0] == '_')
			{
				_Line = p_parent.ParentTrigger.GetVariable(value);
			}
			else
			{
				_Line = Variable.CreateVariable(value, string.Empty);
			}
			_TypeStr = p_node.Attributes["Type"].Value;
			p_parent.SetLine(_TypeStr, _Line, ID);
		}

		public override bool IsEqual(TriggerEvent p_value)
		{
			if (!base.IsEqual(p_value))
			{
				return false;
			}
			return ID == (p_value as TRE_Line).ID;
		}
	}
}
