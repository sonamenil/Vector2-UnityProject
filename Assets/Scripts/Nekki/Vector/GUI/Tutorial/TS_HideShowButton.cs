using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_HideShowButton : TutorialStep
	{
		public const string ElementName = "HideButton";

		private bool _IsHide;

		private List<KeyValuePair<string, List<string>>> _Buttons = new List<KeyValuePair<string, List<string>>>();

		public bool IsHide
		{
			get
			{
				return _IsHide;
			}
		}

		public TS_HideShowButton(bool p_isHide)
		{
			_Type = Type.HideButton;
			_IsHide = p_isHide;
		}

		public TS_HideShowButton(XmlNode p_node)
		{
			_Type = Type.HideButton;
			_IsHide = XmlUtils.ParseBool(p_node.Attributes["IsHide"]);
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				string key = XmlUtils.ParseString(childNode.Attributes["Name"]);
				if (XmlUtils.ParseBool(childNode.Attributes["HideAllButtons"]))
				{
					_Buttons.Add(new KeyValuePair<string, List<string>>(key, null));
					continue;
				}
				List<string> list = new List<string>();
				foreach (XmlNode childNode2 in childNode.ChildNodes)
				{
					list.Add(XmlUtils.ParseString(childNode2.Attributes["Name"]));
				}
				_Buttons.Add(new KeyValuePair<string, List<string>>(key, list));
			}
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			for (int i = 0; i < _Buttons.Count; i++)
			{
				HideButton(_Buttons[i].Key, _Buttons[i].Value);
			}
		}

		private void HideButton(string moduleName, List<string> buttonsName = null)
		{
			List<Button> buttons = Tutorial.Current.GetButtons(moduleName, null);
			if (buttons == null || buttons.Count == 0)
			{
				return;
			}
			int i = 0;
			for (int count = buttons.Count; i < count; i++)
			{
				if (buttonsName == null || (buttonsName != null && buttonsName.Contains(buttons[i].name)))
				{
					buttons[i].transform.parent.gameObject.SetActive(!_IsHide);
				}
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("HideButton");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("IsHide", (!_IsHide) ? "0" : "1");
			for (int i = 0; i < _Buttons.Count; i++)
			{
				XmlElement xmlElement2 = p_node.OwnerDocument.CreateElement("Module");
				xmlElement.AppendChild(xmlElement2);
				xmlElement2.SetAttribute("Name", _Buttons[i].Key);
				if (_Buttons[i].Value == null)
				{
					xmlElement2.SetAttribute("HideAllButtons", "1");
					continue;
				}
				for (int j = 0; j < _Buttons[i].Value.Count; j++)
				{
					XmlElement xmlElement3 = p_node.OwnerDocument.CreateElement("Button");
					xmlElement2.AppendChild(xmlElement3);
					xmlElement3.SetAttribute("Name", _Buttons[i].Value[j]);
				}
			}
		}

		public bool ContainModule(string p_moduleName)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].Key == p_moduleName)
				{
					return true;
				}
			}
			return false;
		}

		public List<string> GetButtons(string p_moduleName)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].Key == p_moduleName)
				{
					return _Buttons[i].Value;
				}
			}
			return null;
		}

		public void SetButtons(string p_moduleName, List<string> p_buttons)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].Key == p_moduleName)
				{
					_Buttons[i] = new KeyValuePair<string, List<string>>(p_moduleName, p_buttons);
					break;
				}
			}
		}

		public void AddModule(string p_moduleName)
		{
			_Buttons.Add(new KeyValuePair<string, List<string>>(p_moduleName, null));
		}

		public void RemoveModule(string p_moduleName)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].Key == p_moduleName)
				{
					_Buttons.RemoveAt(i);
					break;
				}
			}
		}
	}
}
