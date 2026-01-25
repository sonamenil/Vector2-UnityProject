using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.PassiveEffects;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerPassiveEffectLoop : TriggerLoop
	{
		private List<TriggerPassiveEffectAction> _Actions = new List<TriggerPassiveEffectAction>();

		private TriggerPassiveEffect _Parent;

		public TriggerPassiveEffect Parent
		{
			get
			{
				return _Parent;
			}
		}

		private TriggerPassiveEffectLoop(XmlNode p_node, TriggerPassiveEffect p_parent)
			: base(p_node)
		{
			_Parent = p_parent;
			ParseEvent(p_node["Events"]);
			ParseConditions(p_node["Conditions"]);
			ParseActions(p_node["Actions"]);
		}

		public static TriggerPassiveEffectLoop Create(XmlNode p_node, TriggerPassiveEffect p_parent)
		{
			return new TriggerPassiveEffectLoop(p_node, p_parent);
		}

		protected override void ParseEvent(XmlNode p_node)
		{
			TriggerPassiveEffectEvent.Parse(p_node, this, _Events);
		}

		protected override void ParseConditions(XmlNode p_node)
		{
			TriggerPassiveEffectCondition.Parse(p_node, this, _Conditions);
		}

		protected override void ParseActions(XmlNode p_node)
		{
			TriggerPassiveEffectAction.Parse(p_node, this, _Actions);
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

		public void ActivateActions(List<List<TriggerPassiveEffectAction>> p_delayAction)
		{
			bool p_runNext = true;
			for (int i = 0; i < _Actions.Count; i++)
			{
				_Actions[i].Activate(ref p_runNext);
				if (!p_runNext)
				{
					List<TriggerPassiveEffectAction> list = new List<TriggerPassiveEffectAction>();
					for (int num = _Actions.Count - 1; num >= i; num--)
					{
						list.Add(_Actions[num]);
					}
					p_delayAction.Add(list);
					break;
				}
			}
		}

		protected override void ExtraActionsOnEvent(TriggerEvent p_event)
		{
		}
	}
}
