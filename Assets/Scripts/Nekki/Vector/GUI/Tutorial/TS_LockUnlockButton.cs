using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_LockUnlockButton : TutorialStep
	{
		public class LockButtonData
		{
			public string Name;

			public List<string> Buttons;

			public bool IsModule;

			public LockButtonData(string p_name, List<string> p_buttons)
			{
				Name = p_name;
				Buttons = p_buttons;
			}

			public LockButtonData(XmlNode p_node)
			{
				Name = XmlUtils.ParseString(p_node.Attributes["Name"]);
				IsModule = p_node.Name == "Module";
				if (XmlUtils.ParseBool(p_node.Attributes["LockAllButtons"]))
				{
					return;
				}
				Buttons = new List<string>();
				foreach (XmlNode childNode in p_node.ChildNodes)
				{
					Buttons.Add(XmlUtils.ParseString(childNode.Attributes["Name"]));
				}
			}

			public void SaveToXML(XmlNode p_node)
			{
				XmlElement xmlElement = p_node.OwnerDocument.CreateElement((!IsModule) ? "Dialog" : "Module");
				p_node.AppendChild(xmlElement);
				xmlElement.SetAttribute("Name", Name);
				if (Buttons == null)
				{
					xmlElement.SetAttribute("LockAllButtons", "1");
					return;
				}
				int i = 0;
				for (int count = Buttons.Count; i < count; i++)
				{
					XmlElement xmlElement2 = p_node.OwnerDocument.CreateElement("Button");
					xmlElement.AppendChild(xmlElement2);
					xmlElement2.SetAttribute("Name", Buttons[i]);
				}
			}
		}

		public const string ElementName = "LockButton";

		public const string AchievementsButtonName = "AchievementsBtn";

		private bool _IsLock;

		private List<LockButtonData> _Buttons = new List<LockButtonData>();

		public bool IsLock
		{
			get
			{
				return _IsLock;
			}
		}

		public TS_LockUnlockButton(bool p_isLock)
		{
			_Type = Type.LockButton;
			_IsLock = p_isLock;
		}

		public TS_LockUnlockButton(XmlNode p_node)
		{
			_Type = Type.LockButton;
			_IsLock = XmlUtils.ParseBool(p_node.Attributes["IsLock"]);
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				_Buttons.Add(new LockButtonData(childNode));
			}
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			int i = 0;
			for (int count = _Buttons.Count; i < count; i++)
			{
				LockButton(_Buttons[i]);
			}
		}

		private void LockButton(LockButtonData p_lockData)
		{
			List<Button> list = ((!p_lockData.IsModule) ? DialogNotificationManager.GetDialogButtons(p_lockData.Name, null) : Tutorial.Current.GetButtons(p_lockData.Name, null));
			if (list == null || list.Count == 0)
			{
				return;
			}
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				if ((p_lockData.Buttons == null && list[i].name != "AchievementsBtn") || (p_lockData.Buttons != null && p_lockData.Buttons.Contains(list[i].name)))
				{
					list[i].interactable = !_IsLock;
				}
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("LockButton");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("IsLock", (!_IsLock) ? "0" : "1");
			int i = 0;
			for (int count = _Buttons.Count; i < count; i++)
			{
				_Buttons[i].SaveToXML(xmlElement);
			}
		}

		public bool ContainModule(string p_moduleName)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].Name == p_moduleName)
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
				if (_Buttons[i].Name == p_moduleName)
				{
					return _Buttons[i].Buttons;
				}
			}
			return null;
		}

		public void SetButtons(string p_moduleName, List<string> p_buttons)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].Name == p_moduleName)
				{
					_Buttons[i] = new LockButtonData(p_moduleName, p_buttons);
					break;
				}
			}
		}

		public void AddModule(string p_moduleName)
		{
			_Buttons.Add(new LockButtonData(p_moduleName, null));
		}

		public void RemoveModule(string p_moduleName)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].Name == p_moduleName)
				{
					_Buttons.RemoveAt(i);
					break;
				}
			}
		}
	}
}
