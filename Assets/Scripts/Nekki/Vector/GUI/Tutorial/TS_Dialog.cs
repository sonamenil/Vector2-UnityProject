using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.GUI.Tutorial
{
	public class TS_Dialog : TutorialStep
	{
		public enum DialogType
		{
			Info = 0,
			Portrait = 1
		}

		public const string ElementName = "Dialog";

		public string Title;

		public string Text;

		public string ButtonText = string.Empty;

		public string Image;

		public bool IsIgnoringQueue;

		public DialogType _DialogType;

		public TS_Dialog()
		{
			_Type = Type.Dialog;
		}

		public TS_Dialog(XmlNode p_node)
		{
			_Type = Type.Dialog;
			_DialogType = XmlUtils.ParseEnum(p_node.Attributes["DialogType"], DialogType.Info);
			Title = XmlUtils.ParseString(p_node.Attributes["Title"]);
			Text = XmlUtils.ParseString(p_node.Attributes["Text"]);
			ButtonText = XmlUtils.ParseString(p_node.Attributes["ButtonText"]);
			IsIgnoringQueue = XmlUtils.ParseBool(p_node.Attributes["IgnoringQueue"]);
			if (_DialogType == DialogType.Portrait)
			{
				Image = XmlUtils.ParseString(p_node.Attributes["Image"]);
			}
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = false;
			switch (_DialogType)
			{
			case DialogType.Info:
				ShowInfoDialog();
				break;
			case DialogType.Portrait:
				ShowPortraitDialog();
				break;
			}
		}

		private void ShowInfoDialog()
		{
			List<DialogButtonData> list = new List<DialogButtonData>();
			list.Add(new DialogButtonData(OnBtnTap, ButtonText, ButtonUI.Type.Green));
			bool isIgnoringQueue = IsIgnoringQueue;
			DialogNotificationManager.ShowInfoDialog(list, Title, Text, 0, isIgnoringQueue);
		}

		private void ShowPortraitDialog()
		{
			bool isIgnoringQueue = IsIgnoringQueue;
			DialogNotificationManager.ShowQuestTalkingDialog(Title, Text, ButtonText, Image, OnClose, 0, isIgnoringQueue);
		}

		public void OnBtnTap(BaseDialog p_dialog)
		{
			Tutorial.Current.StepOver();
			p_dialog.Dismiss();
		}

		public void OnClose()
		{
			Tutorial.Current.StepOver();
		}

		public override void SaveToXML(XmlNode p_node)
		{
			XmlElement xmlElement = p_node.OwnerDocument.CreateElement("Dialog");
			p_node.AppendChild(xmlElement);
			xmlElement.SetAttribute("Title", Title);
			xmlElement.SetAttribute("Text", Text);
			xmlElement.SetAttribute("ButtonText", ButtonText);
			xmlElement.SetAttribute("DialogType", _DialogType.ToString());
			if (_DialogType == DialogType.Portrait)
			{
				xmlElement.SetAttribute("Image", Image);
			}
			if (IsIgnoringQueue)
			{
				xmlElement.SetAttribute("IgnoringQueue", "1");
			}
		}

		public void OkBtnCallBack()
		{
			Tutorial.Current.StepOver();
		}
	}
}
