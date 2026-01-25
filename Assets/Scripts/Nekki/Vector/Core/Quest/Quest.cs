using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Common;

namespace Nekki.Vector.Core.Quest
{
	public class Quest
	{
		public enum State
		{
			Sleep = 0,
			Active = 1,
			Complete = -1
		}

		private string _Name;

		private string _VisualName;

		private string _RewardImageName;

		private Variable _Description;

		private Variable _Progress;

		protected string _RewardVisualName;

		private string _RewardType;

		private Variable _RewardItemName;

		private TriggerQuest _StartTrigger;

		private List<TriggerQuest> _Triggers;

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public string VisualName
		{
			get
			{
				return _VisualName;
			}
		}

		public string RewardImageName
		{
			get
			{
				return _RewardImageName;
			}
		}

		public string Description
		{
			get
			{
				return _Description.ValueString;
			}
		}

		public string Progress
		{
			get
			{
				return _Progress.ValueString;
			}
		}

		public string RewardVisualName
		{
			get
			{
				return _RewardVisualName;
			}
		}

		public string RewardType
		{
			get
			{
				return _RewardType;
			}
		}

		public Variable RewardItemName
		{
			get
			{
				return _RewardItemName;
			}
		}

		public State QuestState
		{
			get
			{
				switch (CounterController.Current.GetUserCounter(_Name, "ST_Quests"))
				{
				case (int)-1L:
					return State.Complete;
				case (int)0L:
					return State.Sleep;
				case (int)1L:
					return State.Active;
				default:
					return State.Sleep;
				}
			}
		}

		public bool IsSleepState
		{
			get
			{
				return QuestState == State.Sleep;
			}
		}

		public bool IsActiveState
		{
			get
			{
				return QuestState == State.Active;
			}
		}

		public bool IsCompleteState
		{
			get
			{
				return QuestState == State.Complete;
			}
		}

		public bool IsNew
		{
			get
			{
				return IsActiveState && (int)CounterController.Current.GetUserCounter(_Name, "ST_NewQuest") == 1;
			}
			set
			{
				CounterController.Current.CreateCounterOrSetValue(_Name, value ? 1 : 0, "ST_NewQuest");
			}
		}

		public Quest(XmlNode p_node)
		{
			_Name = p_node.Attributes["Name"].Value;
			ParseInfo(p_node["Info"]);
			if (QuestState != State.Complete)
			{
				ParseTriggers(p_node);
			}
		}

		private void ParseInfo(XmlNode p_node)
		{
			_VisualName = p_node["VisualName"].Attributes["Value"].Value;
			_Description = Variable.CreateVariable(p_node["Description"].Attributes["Value"].Value, string.Empty);
			_Progress = Variable.CreateVariable(XmlUtils.ParseString(p_node["Description"].Attributes["Progress"], string.Empty), null);
			_RewardImageName = p_node["Reward"].Attributes["ImageName"].Value;
			_RewardVisualName = p_node["Reward"].Attributes["VisualName"].Value;
			_RewardType = p_node["Reward"].Attributes["Type"].Value;
			_RewardItemName = Variable.CreateVariable(XmlUtils.ParseString(p_node["Reward"].Attributes["Name"], string.Empty), string.Empty);
		}

		private void ParseTriggers(XmlNode p_node)
		{
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.Name == "Info")
				{
					continue;
				}
				TriggerQuest triggerQuest = TriggerQuest.Create(childNode, this);
				if (triggerQuest.IsStartTrigger)
				{
					if (_StartTrigger != null)
					{
						DebugUtils.Dialog("StartTrigger duplicate in " + _Name + " quest", true);
					}
					_StartTrigger = triggerQuest;
				}
				else
				{
					if (_Triggers == null)
					{
						_Triggers = new List<TriggerQuest>();
					}
					_Triggers.Add(triggerQuest);
				}
			}
		}

		public void QuestActivated()
		{
			CounterController.Current.CreateCounterOrSetValue(_Name, 1, "ST_Quests");
			IsNew = true;
			DataLocal.Current.Save(true);
			UpdateQuestLogAnnounce();
		}

		public void QuestCompleted()
		{
			CounterController.Current.CreateCounterOrSetValue(_Name, -1, "ST_Quests");
			IsNew = false;
			CounterController.Current.ClearCounterNamespace(_Name);
			DataLocal.Current.Save(true);
			QuestManager.Current.CheckEvent(TQE_OnCall.CalledByQuestCompleteEvent);
			UpdateAllAnnounces();
		}

		private void UpdateQuestLogAnnounce()
		{
			BottomPanel module = UIModule.GetModule<BottomPanel>();
			if (module != null)
			{
				module.UpdateQuestLogAnnounce();
			}
		}

		private void UpdateAllAnnounces()
		{
			BottomPanel module = UIModule.GetModule<BottomPanel>();
			if (module != null)
			{
				module.UpdateArchiveAnnounce();
				module.UpdateQuestLogAnnounce();
				module.UpdateBoosterpackAnnounce();
			}
		}

		public void CheckEvent(TriggerQuestEvent p_event)
		{
			switch (QuestState)
			{
			case State.Complete:
				break;
			case State.Active:
				TriggersCheckEvent(p_event);
				break;
			case State.Sleep:
				StartTriggerCheckEvent(p_event);
				break;
			}
		}

		private void StartTriggerCheckEvent(TriggerQuestEvent p_event)
		{
			_StartTrigger.CheckEvent(p_event);
		}

		private void TriggersCheckEvent(TriggerQuestEvent p_event)
		{
			for (int i = 0; i < _Triggers.Count; i++)
			{
				_Triggers[i].CheckEvent(p_event);
			}
		}

		public TriggerQuest GetTriggerByName(string p_name)
		{
			if (_StartTrigger.Name == p_name)
			{
				return _StartTrigger;
			}
			for (int i = 0; i < _Triggers.Count; i++)
			{
				if (_Triggers[i].Name == p_name)
				{
					return _Triggers[i];
				}
			}
			return null;
		}
	}
}
