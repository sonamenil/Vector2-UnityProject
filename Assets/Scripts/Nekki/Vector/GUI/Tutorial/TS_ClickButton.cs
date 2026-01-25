using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_ClickButton : TutorialStep
	{
		public const string ElementName = "ClickButton";

		public string ModuleName;

		public string ButtonName;

		public bool RemoveButtonActions;

		public bool AllExceptButtonName;

		public ArrowData ArrowData;

		private TutorialArrow _Arrow;

		private Dictionary<Button, Button.ButtonClickedEvent> _TmpEvents;

		private UnityAction _TmpAction;

		public TS_ClickButton()
		{
			_Type = Type.ClickButton;
			ArrowData = new ArrowData();
		}

		public TS_ClickButton(XmlNode p_node)
		{
			_Type = Type.ClickButton;
			ModuleName = XmlUtils.ParseString(p_node.Attributes["Module"]);
			ButtonName = XmlUtils.ParseString(p_node.Attributes["Button"]);
			RemoveButtonActions = XmlUtils.ParseBool(p_node.Attributes["RemoveButtonActions"]);
			AllExceptButtonName = XmlUtils.ParseBool(p_node.Attributes["AllExceptButtonName"]);
			if (RemoveButtonActions)
			{
				_TmpEvents = new Dictionary<Button, Button.ButtonClickedEvent>();
			}
			ArrowData = new ArrowData(p_node["Arrow"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			List<Button> buttons = Tutorial.Current.GetButtons(ModuleName, ButtonName);
			if (buttons == null || buttons.Count == 0)
			{
				p_runNext = true;
				return;
			}
			_TmpAction = delegate
			{
				Tutorial.Current.ClickButtonStepOver(this);
			};
			for (int i = 0; i < buttons.Count; i++)
			{
				Button button = buttons[i];
				button.interactable = true;
				if (RemoveButtonActions)
				{
					_TmpEvents.Add(button, button.onClick);
					button.onClick = new Button.ButtonClickedEvent();
				}
				button.onClick.AddListener(_TmpAction);
				CreateArrow(button.transform);
			}
		}

		public void Activate(TS_Fork.TutorialWay p_way)
		{
			List<Button> buttons = Tutorial.Current.GetButtons(ModuleName, ButtonName);
			if (buttons == null || buttons.Count == 0)
			{
				return;
			}
			_TmpAction = delegate
			{
				Tutorial.Current.Fork(p_way);
			};
			for (int i = 0; i < buttons.Count; i++)
			{
				Button button = buttons[i];
				button.interactable = true;
				if (RemoveButtonActions)
				{
					_TmpEvents.Add(button, button.onClick);
					button.onClick = new Button.ButtonClickedEvent();
				}
				button.onClick.AddListener(_TmpAction);
				CreateArrow(button.transform);
			}
		}

		protected void CreateArrow(Transform p_parent)
		{
			if (!(ArrowData.Prefab == null))
			{
				_Arrow = ArrowData.CreateArrow(p_parent);
				_Arrow.Activate();
			}
		}

		public void RemoveDelegate()
		{
			List<Button> buttons = Tutorial.Current.GetButtons(ModuleName, ButtonName);
			if (_TmpAction != null && buttons != null && buttons.Count > 0)
			{
				for (int i = 0; i < buttons.Count; i++)
				{
					if (RemoveButtonActions)
					{
						buttons[i].onClick = _TmpEvents[buttons[i]];
					}
					else
					{
						buttons[i].onClick.RemoveListener(_TmpAction);
					}
				}
			}
			_TmpAction = null;
		}

		public void RemoveArrow()
		{
			if (_Arrow != null)
			{
				Object.Destroy(_Arrow.gameObject);
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("ClickButton");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Module", ModuleName);
			xmlElement.SetAttribute("Button", ButtonName);
			xmlElement.SetAttribute("RemoveButtonActions", (!RemoveButtonActions) ? "0" : "1");
			xmlElement.SetAttribute("AllExceptButtonName", (!AllExceptButtonName) ? "0" : "1");
			if (!(ArrowData.Prefab == null))
			{
				ArrowData.SaveToXML(xmlElement);
			}
		}
	}
}
