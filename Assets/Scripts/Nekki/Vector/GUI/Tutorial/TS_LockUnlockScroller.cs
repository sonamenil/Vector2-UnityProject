using System.Collections.Generic;
using System.Xml;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_LockUnlockScroller : TutorialStep
	{
		public const string ElementName = "LockScroller";

		public const string AchievementsButtonName = "AchievementsBtn";

		private bool _IsLock;

		private List<string> _Modules = new List<string>();

		public bool IsLock
		{
			get
			{
				return _IsLock;
			}
		}

		public TS_LockUnlockScroller(bool p_isLock)
		{
			_Type = Type.LockScroller;
			_IsLock = p_isLock;
		}

		public TS_LockUnlockScroller(XmlNode p_node)
		{
			_Type = Type.LockScroller;
			_IsLock = XmlUtils.ParseBool(p_node.Attributes["IsLock"]);
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				string item = XmlUtils.ParseString(childNode.Attributes["Name"]);
				_Modules.Add(item);
			}
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			for (int i = 0; i < _Modules.Count; i++)
			{
				LockScroller(_Modules[i]);
			}
		}

		private void LockScroller(string p_moduleName)
		{
			PlateScroller[] plateScrollers = Tutorial.Current.GetPlateScrollers(p_moduleName);
			if (plateScrollers != null)
			{
				for (int i = 0; i < plateScrollers.Length; i++)
				{
					plateScrollers[i].enabled = !_IsLock;
				}
			}
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("LockScroller");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("IsLock", (!_IsLock) ? "0" : "1");
			for (int i = 0; i < _Modules.Count; i++)
			{
				XmlElement xmlElement2 = p_node.OwnerDocument.CreateElement("Module");
				xmlElement.AppendChild(xmlElement2);
				xmlElement2.SetAttribute("Name", _Modules[i]);
			}
		}

		public bool ContainModule(string p_moduleName)
		{
			return _Modules.Contains(p_moduleName);
		}

		public void AddModule(string p_moduleName)
		{
			_Modules.Add(p_moduleName);
		}

		public void RemoveModule(string p_moduleName)
		{
			_Modules.Remove(p_moduleName);
		}
	}
}
