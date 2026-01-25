using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerLoop
	{
		protected string _Name;

		protected List<TriggerEvent> _Events = new List<TriggerEvent>();

		protected List<TriggerCondition> _Conditions = new List<TriggerCondition>();

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public List<TriggerEvent> Events
		{
			get
			{
				return _Events;
			}
		}

		public List<TriggerCondition> Conditions
		{
			get
			{
				return _Conditions;
			}
		}

		public int Number { get; set; }

		protected TriggerLoop(XmlNode p_node)
		{
			_Name = XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty);
		}

		protected abstract void ParseEvent(XmlNode p_node);

		protected abstract void ParseConditions(XmlNode p_node);

		protected abstract void ParseActions(XmlNode p_node);

		protected abstract bool CheckConditions();

		protected abstract void ExtraActionsOnEvent(TriggerEvent p_event);

		public bool CheckEvent(TriggerEvent p_event)
		{
			for (int i = 0; i < _Events.Count; i++)
			{
				if (_Events[i].IsEqual(p_event))
				{
					ExtraActionsOnEvent(p_event);
					if (CheckConditions())
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
