using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerRunnerLoop : TriggerLoop
	{
		private TriggerRunner _Parent;

		private TriggerForEach _ForEach;

		private List<TriggerRunnerAction> _Actions = new List<TriggerRunnerAction>();

		public TriggerRunner ParentTrigger
		{
			get
			{
				return _Parent;
			}
		}

		public TriggerForEach ForEach
		{
			get
			{
				return _ForEach;
			}
		}

		public List<TriggerRunnerAction> Actions
		{
			get
			{
				return _Actions;
			}
		}

		public bool IsContainsCollisionEvent
		{
			get
			{
				for (int i = 0; i < _Events.Count; i++)
				{
					switch (_Events[i].Type)
					{
					case TriggerEvent.EventType.TRE_ENTER:
					case TriggerEvent.EventType.TRE_EXIT:
					case TriggerEvent.EventType.TRE_LINE:
					case TriggerEvent.EventType.TRE_KEY:
					case TriggerEvent.EventType.TRE_COLISION:
					case TriggerEvent.EventType.TRE_VAR_CHANGE:
						return true;
					}
				}
				return false;
			}
		}

		private TriggerRunnerLoop(XmlNode p_node, TriggerRunner p_parent)
			: base(p_node)
		{
			_Parent = p_parent;
			ParseForEach(p_node["Foreach"]);
			ParseEvent(p_node["Events"]);
			ParseConditions(p_node["Conditions"]);
			ParseActions(p_node["Actions"]);
			InitActions();
		}

		public static TriggerRunnerLoop Create(XmlNode p_node, TriggerRunner p_parent)
		{
			return new TriggerRunnerLoop(p_node, p_parent);
		}

		private void InitActions()
		{
			for (int i = 0; i < _Actions.Count - 1; i++)
			{
				_Actions[i].IsLastAction = false;
			}
		}

		private void ParseForEach(XmlNode p_node)
		{
			if (p_node != null)
			{
				_ForEach = new TriggerForEach(p_node, this);
			}
		}

		protected override void ParseEvent(XmlNode p_node)
		{
			if (p_node == null)
			{
				return;
			}
			XmlAttribute xmlAttribute = p_node.Attributes["Template"];
			if (xmlAttribute != null)
			{
				ParseEvent(TemplateModule.getTemplateEventsXml(xmlAttribute.Value));
				return;
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.LocalName.Equals("EventBlock"))
				{
					string value = childNode.Attributes["Template"].Value;
					ParseEvent(TemplateModule.getTemplateEventsXml(value));
					continue;
				}
				TriggerRunnerEvent triggerRunnerEvent = TriggerRunnerEvent.Create(childNode, this);
				if (triggerRunnerEvent != null)
				{
					_Events.Add(triggerRunnerEvent);
				}
			}
		}

		protected override void ParseConditions(XmlNode p_node)
		{
			TriggerRunnerCondition.Parse(p_node, this, _Conditions);
		}

		protected override void ParseActions(XmlNode p_node)
		{
			TriggerRunnerAction.Parse(p_node, this, _Actions);
		}

		public void ProcessEvent(TriggerEvent p_event, List<List<TriggerRunnerAction>> p_actionsToExecute)
		{
			VectorLog.RunLog(this);
			VectorLog.Tab(1);
			if (_ForEach == null)
			{
				if (CheckEvent(p_event))
				{
					p_actionsToExecute.Add(Actions);
				}
			}
			else
			{
				_ForEach.ProcessEvent(p_event);
			}
			VectorLog.Untab(1);
		}

		protected override void ExtraActionsOnEvent(TriggerEvent p_event)
		{
			_Parent.SetModelVar();
			switch (p_event.Type)
			{
			case TriggerEvent.EventType.TRE_KEY:
				_Parent.SetKeyVar((p_event as TRE_Key).Key);
				break;
			case TriggerEvent.EventType.TRE_ACTIVATE:
				_Parent.GetVariable("_$ActionID").SetValue((p_event as TRE_Activate).ActionID);
				break;
			case TriggerEvent.EventType.TRE_ACTIVATE_NEAR_PLAYER:
				_Parent.GetVariable("_$ActionID").SetValue((p_event as TRE_ActivateNearPlayer).ActionID);
				break;
			case TriggerEvent.EventType.TRE_SWARM_ARRIVAL:
				_Parent.GetVariable("_$WaypointKey").SetValue((p_event as TRE_SwarmArrival).WayPointKey);
				break;
			case TriggerEvent.EventType.TRE_SWARM_DEPARTURE:
				_Parent.GetVariable("_$WaypointKey").SetValue((p_event as TRE_SwarmDeparture).WayPointKey);
				break;
			case TriggerEvent.EventType.TRE_SWARM_DEC:
				_Parent.GetVariable("_$WaypointKey").SetValue((p_event as TRE_SwarmDec).WayPointKey);
				break;
			}
		}

		protected override bool CheckConditions()
		{
			if (_Conditions.Count > 0)
			{
				VectorLog.RunLog("Conditions:");
			}
			for (int i = 0; i < _Conditions.Count; i++)
			{
				if (!_Conditions[i].Check())
				{
					if (Settings.WriteRunLogs)
					{
						_Conditions[i].Log(false);
					}
					return false;
				}
				if (Settings.WriteRunLogs)
				{
					_Conditions[i].Log(true);
				}
			}
			return true;
		}

		public void SetLine(string p_type, Variable p_value, int p_iD)
		{
			TriggerLine triggerLine = new TriggerLine(_Parent, p_iD);
			if (p_type[0] == '_')
			{
				p_type = ParentTrigger.GetVariable(p_type).ValueString;
			}
			triggerLine.SetLine(p_type, p_value);
			_Parent.AddLine(triggerLine);
		}

		public Variable GetParentVar(string p_key)
		{
			return _Parent.GetVariable(p_key);
		}

		public override string ToString()
		{
			string text = "Name:" + base.Name;
			text += "\n Events:";
			foreach (TriggerEvent @event in _Events)
			{
				text = text + "\n   " + @event.ToString();
			}
			text += "\n Conditions:";
			foreach (TriggerCondition condition in _Conditions)
			{
				text = text + "\n   " + condition.ToString();
			}
			text += "\n Actions:";
			foreach (TriggerRunnerAction action in _Actions)
			{
				text = text + "\n   " + action.ToString();
			}
			return text;
		}
	}
}
