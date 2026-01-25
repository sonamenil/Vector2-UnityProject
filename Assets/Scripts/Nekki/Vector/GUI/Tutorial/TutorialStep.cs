using System.Xml;
using UnityEngine;

namespace Nekki.Vector.GUI.Tutorial
{
	public abstract class TutorialStep
	{
		public enum Type
		{
			Dialog = 0,
			LockButton = 1,
			LockScroller = 2,
			HideButton = 3,
			ClickButton = 4,
			ClickGadget = 5,
			ClickCard = 6,
			ChangeCounter = 7,
			Fork = 8,
			End = 9,
			Pointer = 10,
			ScrollToItem = 11,
			SaveBackup = 12,
			Sound = 13,
			Notification = 14,
			StartSequence = 15,
			CheckQuests = 16,
			Glow = 17,
			RemoveItem = 18,
			Delay = 19,
			RemoveBackup = 20,
			Statistics = 21,
			QuestStart = 22,
			WaitForModuleActivated = 23,
			GiveMoney = 24
		}

		protected Type _Type;

		public Type TypeStep
		{
			get
			{
				return _Type;
			}
		}

		public bool IsDialog
		{
			get
			{
				return _Type == Type.Dialog;
			}
		}

		public static TutorialStep Create(XmlNode p_node)
		{
			switch (p_node.Name)
			{
			case "Fork":
				return new TS_Fork(p_node);
			case "ClickButton":
				return new TS_ClickButton(p_node);
			case "Dialog":
				return new TS_Dialog(p_node);
			case "LockButton":
				return new TS_LockUnlockButton(p_node);
			case "HideButton":
				return new TS_HideShowButton(p_node);
			case "ChangeCounter":
				return new TS_ChangeCounter(p_node);
			case "End":
				return new TS_End();
			case "SaveBackup":
				return new TS_SaveBackup();
			case "RemoveBackup":
				return new TS_RemoveBackup();
			case "Sound":
				return new TS_Sound(p_node);
			case "Notification":
				return new TS_Notification(p_node);
			case "StartSequence":
				return new TS_StartSequence(p_node);
			case "CheckQuests":
				return new TS_CheckQuests();
			case "Glow":
				return new TS_Glow(p_node);
			case "Delay":
				return new TS_Delay(p_node);
			case "RemoveItem":
				return new TS_RemoveItem(p_node);
			case "Statistics":
				return new TS_Statistics(p_node);
			case "QuestStart":
				return new TS_QuestStart(p_node);
			case "ClickGadget":
				return new TS_ClickGadget(p_node);
			case "ClickCard":
				return new TS_ClickCard(p_node);
			case "WaitForModuleActivated":
				return new TS_WaitForModuleActivated(p_node);
			case "GiveMoney":
				return new TS_GiveMoney(p_node);
			case "LockScroller":
				return new TS_LockUnlockScroller(p_node);
			default:
				return null;
			}
		}

		public abstract void Activate(ref bool p_runNext);

		public abstract void SaveToXML(XmlNode p_node);

		public static T LoadAsetFromXML<T>(XmlNode p_node, string p_attrName) where T : Object
		{
			string text = XmlUtils.ParseString(p_node.Attributes[p_attrName], string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return (T)null;
			}
			return Resources.Load<T>(text);
		}

		public static void SaveAssetToXML(XmlElement p_node, string p_attrName, Object p_asset)
		{
		}
	}
}
