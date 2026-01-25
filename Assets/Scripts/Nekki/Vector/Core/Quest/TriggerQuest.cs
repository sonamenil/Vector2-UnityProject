using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Trigger;

namespace Nekki.Vector.Core.Quest
{
	public class TriggerQuest
	{
		private Quest _Parent;

		private bool _IsStartTrigger;

		private string _Name;

		private List<TriggerQuestLoop> _Loops = new List<TriggerQuestLoop>();

		public Quest Parent
		{
			get
			{
				return _Parent;
			}
		}

		public bool IsStartTrigger
		{
			get
			{
				return _IsStartTrigger;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		private TriggerQuest(XmlNode p_node, Quest p_parent)
		{
			_Parent = p_parent;
			_Name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			_IsStartTrigger = p_node.Name == "StartTrigger";
			ParseLoops(p_node["Content"]);
		}

		public static TriggerQuest Create(XmlNode p_node, Quest p_parent)
		{
			return new TriggerQuest(p_node, p_parent);
		}

		private void ParseLoops(XmlNode p_node)
		{
			int num = 0;
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				_Loops.Add(TriggerQuestLoop.Create(childNode, this, num++));
			}
		}

		public void CheckEvent(TriggerEvent p_event)
		{
			for (int i = 0; i < _Loops.Count; i++)
			{
				if (_Loops[i].CheckEvent(p_event))
				{
					_Loops[i].ActivateActions();
				}
			}
		}

		public TriggerQuestLoop GetLoopByIndex(int p_index)
		{
			if (p_index >= _Loops.Count)
			{
				return null;
			}
			return _Loops[p_index];
		}
	}
}
