using System.Xml;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_ClickCard : TutorialStep
	{
		public const string ElementName = "ClickCard";

		public string CardName;

		private UnityAction _TmpAction;

		private Button _TmpButton;

		public ArrowData ArrowData;

		private TutorialArrow _Arrow;

		public TS_ClickCard()
		{
			_Type = Type.ClickCard;
			ArrowData = new ArrowData();
		}

		public TS_ClickCard(XmlNode p_node)
		{
			_Type = Type.ClickCard;
			CardName = XmlUtils.ParseString(p_node.Attributes["CardName"]);
			ArrowData = new ArrowData(p_node["Arrow"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			BaseCardUI[] cards = Tutorial.Current.GetCards();
			if (cards == null || cards.Length == 0)
			{
				DebugUtils.Dialog("On TS_ClickCard cards == null or empty", true);
				return;
			}
			BaseCardUI baseCardUI = null;
			int i = 0;
			for (int num = cards.Length; i < num; i++)
			{
				if (cards[i].Card.CardName == CardName)
				{
					baseCardUI = cards[i];
					break;
				}
			}
			if (baseCardUI == null)
			{
				DebugUtils.Dialog("On TS_ClickCard cards exist, but no card with CardName = " + CardName, true);
				return;
			}
			_TmpAction = delegate
			{
				Tutorial.Current.ClickCardStepOver(this);
			};
			_TmpButton = baseCardUI.GetComponentInChildren<Button>();
			_TmpButton.onClick.AddListener(_TmpAction);
			CreateArrow(_TmpButton.transform);
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
			_TmpButton.onClick.RemoveListener(_TmpAction);
			_TmpAction = null;
			_TmpButton = null;
		}

		public void RemoveArrow()
		{
			if (_Arrow != null)
			{
				Object.DestroyImmediate(_Arrow.gameObject);
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("ClickCard");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("CardName", CardName);
			if (!(ArrowData.Prefab == null))
			{
				ArrowData.SaveToXML(xmlElement);
			}
		}
	}
}
