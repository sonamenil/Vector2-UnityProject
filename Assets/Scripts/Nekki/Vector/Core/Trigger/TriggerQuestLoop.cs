using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerQuestLoop : TriggerLoop
	{
		private List<TriggerQuestAction> _Actions = new List<TriggerQuestAction>();

		private int _Index;

		private int _PostponeActionPosition;

		private TriggerQuest _Parent;

		public int PostponeActionPosition
		{
			set
			{
				_PostponeActionPosition = value;
			}
		}

		public TriggerQuest Parent
		{
			get
			{
				return _Parent;
			}
		}

		public string QuestNamespaceName
		{
			get
			{
				return "Quest_" + _Parent.Parent.Name;
			}
		}

		private string PostponeCounterName
		{
			get
			{
				return string.Format("{0}|{1}|{2}", _Parent.Parent.Name, _Parent.Name, _Index);
			}
		}

		private TriggerQuestLoop(XmlNode p_node, TriggerQuest p_parent, int p_index)
			: base(p_node)
		{
			_Parent = p_parent;
			_Index = p_index;
			ParseEvent(p_node["Events"]);
			ParseConditions(p_node["Conditions"]);
			ParseActions(p_node["Actions"]);
		}

		public static TriggerQuestLoop Create(XmlNode p_node, TriggerQuest p_parent, int p_index)
		{
			return new TriggerQuestLoop(p_node, p_parent, p_index);
		}

		protected override void ParseEvent(XmlNode p_node)
		{
			TriggerQuestEvent.Parse(p_node, _Events);
		}

		protected override void ParseConditions(XmlNode p_node)
		{
			TriggerQuestCondition.Parse(p_node, this, _Conditions);
		}

		protected override void ParseActions(XmlNode p_node)
		{
			TriggerQuestAction.Parse(p_node, this, _Actions);
		}

		protected override bool CheckConditions()
		{
			for (int i = 0; i < _Conditions.Count; i++)
			{
				if (!_Conditions[i].Check())
				{
					return false;
				}
			}
			return true;
		}

		public void ActivateActions()
		{
			bool p_runNext = true;
			for (int i = _PostponeActionPosition; i < _Actions.Count; i++)
			{
				_Actions[i].Activate(ref p_runNext);
				if (!p_runNext)
				{
					_PostponeActionPosition = i + 1;
					SavePostponeCounter();
					return;
				}
			}
			CancelPostponeActions();
		}

		public void CancelPostponeActions()
		{
			_PostponeActionPosition = 0;
			ClearPostponeCounter();
		}

		private void SavePostponeCounter()
		{
			CounterController.Current.CreateCounterOrSetValue(PostponeCounterName, _PostponeActionPosition - 1, "ST_QuestPostpone");
			DataLocal.Current.Save(true);
		}

		private void ClearPostponeCounter()
		{
			CounterController.Current.RemoveUserCounter(PostponeCounterName, "ST_QuestPostpone");
			DataLocal.Current.Save(true);
		}

		protected override void ExtraActionsOnEvent(TriggerEvent p_event)
		{
		}
	}
}
