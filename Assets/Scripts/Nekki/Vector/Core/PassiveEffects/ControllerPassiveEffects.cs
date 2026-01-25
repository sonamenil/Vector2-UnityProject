using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Run;

namespace Nekki.Vector.Core.PassiveEffects
{
	public class ControllerPassiveEffects
	{
		private const string _Passives_Filename = "passive_effects.xml";

		private static ControllerPassiveEffects _Current;

		private StatusEffectsPanel _StatusEffectsPanel;

		private Dictionary<string, TriggerPassiveEffect> _Triggers = new Dictionary<string, TriggerPassiveEffect>();

		private List<List<TriggerPassiveEffectAction>> _DelayActions = new List<List<TriggerPassiveEffectAction>>();

		private Variable _ActivateID;

		private Variable _AnomationName;

		public Variable ActivateID
		{
			get
			{
				return _ActivateID;
			}
		}

		public Variable AnomationName
		{
			get
			{
				return _AnomationName;
			}
		}

		public ControllerPassiveEffects()
		{
			CounterController.Current.GetCounterNamespace("PassiveEffects");
			_Current = this;
			_StatusEffectsPanel = UIModule.GetModule<HudPanel>().PanelStatusEffects;
			_ActivateID = Variable.CreateVariable(string.Empty, null);
			_AnomationName = Variable.CreateVariable(string.Empty, null);
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.RunDataLibs, "passive_effects.xml");
			Dictionary<string, XmlNode> dictionary = new Dictionary<string, XmlNode>();
			foreach (XmlNode childNode in xmlDocument["PassiveEffects"].ChildNodes)
			{
				dictionary.Add(childNode.Attributes["Name"].Value, childNode);
			}
			CardsGroupAttribute cardsGroupAttribute = null;
			string text = null;
			List<GadgetItem> userGadgets = DataLocalHelper.GetUserGadgets();
			for (int i = 0; i < userGadgets.Count; i++)
			{
				List<CardsGroupAttribute> cards = userGadgets[i].Cards;
				for (int j = 0; j < cards.Count; j++)
				{
					cardsGroupAttribute = cards[j];
					text = cardsGroupAttribute.CardName;
					if (cardsGroupAttribute.TriggerName != null)
					{
						if (_Triggers.ContainsKey(text))
						{
							_Triggers[text].IncrementPassiveOccurVar(1);
						}
						else if (dictionary.ContainsKey(cardsGroupAttribute.TriggerName))
						{
							TriggerPassiveEffect value = TriggerPassiveEffect.Create(dictionary[cardsGroupAttribute.TriggerName], cardsGroupAttribute.Vars, this);
							_Triggers.Add(text, value);
						}
						else
						{
							DebugUtils.Dialog("TriggerPassiveEffect not find Name: " + cardsGroupAttribute.TriggerName, true);
						}
					}
				}
			}
		}

		public void End()
		{
			_Current = null;
			TriggerPassiveEffectEvent.AllFree();
			CounterController.Current.ClearCounterNamespace("PassiveEffects");
		}

		public void Render()
		{
			_StatusEffectsPanel.Render();
			RenderDelayActions();
		}

		private void RenderDelayActions()
		{
			if (_DelayActions.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _DelayActions.Count; i++)
			{
				List<TriggerPassiveEffectAction> list = _DelayActions[i];
				bool p_runNext = false;
				while (list.Count != 0)
				{
					TriggerPassiveEffectAction triggerPassiveEffectAction = list[list.Count - 1];
					triggerPassiveEffectAction.Activate(ref p_runNext);
					if (p_runNext)
					{
						list.Remove(triggerPassiveEffectAction);
						continue;
					}
					break;
				}
			}
			for (int num = _DelayActions.Count - 1; num >= 0; num--)
			{
				if (_DelayActions[num].Count == 0)
				{
					_DelayActions.RemoveAt(num);
				}
			}
		}

		public static int CounterAppendValue(string p_name)
		{
			if (_Current == null)
			{
				return 0;
			}
			return _Current._StatusEffectsPanel.CounterAppendValue(p_name);
		}

		public static void EventActivate(string p_activateID)
		{
			_Current._ActivateID.SetValue(p_activateID);
			TPEE_Activate.CheckEvent(_Current._DelayActions);
		}

		public static void EventAnimationStart(string p_name)
		{
			if (_Current != null)
			{
				_Current._AnomationName.SetValue(p_name);
				TPEE_AnimationStart.CheckEvent(_Current._DelayActions);
			}
		}

		public static void EventFloorStart()
		{
			TPEE_FloorStart.CheckEvent(_Current._DelayActions);
		}

		public static void EventFloorEnd()
		{
			TPEE_FloorEnd.CheckEvent(_Current._DelayActions);
		}
	}
}
