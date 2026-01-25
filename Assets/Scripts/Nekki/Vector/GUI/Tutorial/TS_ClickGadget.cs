using System.Xml;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_ClickGadget : TutorialStep
	{
		public const string ElementName = "ClickGadget";

		public string SlotName;

		private UnityAction _TmpAction;

		private Button _TmpButton;

		public ArrowData ArrowData;

		private TutorialArrow _Arrow;

		public TS_ClickGadget()
		{
			_Type = Type.ClickGadget;
			ArrowData = new ArrowData();
		}

		public TS_ClickGadget(XmlNode p_node)
		{
			_Type = Type.ClickGadget;
			SlotName = XmlUtils.ParseString(p_node.Attributes["SlotName"]);
			ArrowData = new ArrowData(p_node["Arrow"]);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			GadgetUIPanel gadgetUIPanel = Tutorial.Current.GetGadgetUIPanel();
			if (gadgetUIPanel == null)
			{
				DebugUtils.Dialog("On TS_ClickGadget panel == null", true);
				return;
			}
			GadgetUI gadgetUI = gadgetUIPanel.GetGadgetUI(SlotName);
			if (gadgetUI == null)
			{
				DebugUtils.Dialog("On TS_ClickGadget (slot = " + SlotName + ") == null", true);
				return;
			}
			_TmpAction = delegate
			{
				Tutorial.Current.ClickGadgetStepOver(this);
			};
			_TmpButton = gadgetUI.GetComponentInChildren<Button>();
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
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("ClickGadget");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("SlotName", SlotName);
			if (!(ArrowData.Prefab == null))
			{
				ArrowData.SaveToXML(xmlElement);
			}
		}
	}
}
