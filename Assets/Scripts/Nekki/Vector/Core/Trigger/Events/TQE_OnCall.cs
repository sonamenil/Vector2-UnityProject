using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TQE_OnCall : TriggerQuestEvent
	{
		private const string Any = "Any";

		private const string Tutorial = "Tutorial";

		private const string Trigger = "Trigger";

		private const string QuestComplete = "QuestComplete";

		private string _CallerName;

		private Variable _Message;

		public static TQE_OnCall CalledByAnyEvent
		{
			get
			{
				return new TQE_OnCall("Any");
			}
		}

		public static TQE_OnCall CalledByTutorialEvent
		{
			get
			{
				return new TQE_OnCall("Tutorial");
			}
		}

		public static TQE_OnCall CalledByQuestCompleteEvent
		{
			get
			{
				return new TQE_OnCall("QuestComplete");
			}
		}

		public TQE_OnCall(XmlNode p_node)
		{
			_CallerName = p_node.Attributes["Name"].Value;
			if (p_node.Attributes["Message"] != null)
			{
				_Message = Variable.CreateVariable(p_node.Attributes["Message"].Value, null);
			}
			_Type = EventType.TQE_ON_CALL;
		}

		public TQE_OnCall(string p_callerName, Variable p_message = null)
		{
			_CallerName = p_callerName;
			_Message = p_message;
			_Type = EventType.TQE_ON_CALL;
		}

		public static TQE_OnCall CalledByTriggerEvent(Variable p_message)
		{
			return new TQE_OnCall("Trigger", p_message);
		}

		public override bool IsEqual(TriggerEvent p_value)
		{
			if (base.IsEqual(p_value))
			{
				TQE_OnCall tQE_OnCall = p_value as TQE_OnCall;
				if (_CallerName == "Any")
				{
					return true;
				}
				if (_CallerName == tQE_OnCall._CallerName)
				{
					if (_CallerName == "Trigger")
					{
						if (_Message == null)
						{
							return true;
						}
						return tQE_OnCall._Message != null && _Message.IsEqual(tQE_OnCall._Message);
					}
					return true;
				}
			}
			return false;
		}
	}
}
