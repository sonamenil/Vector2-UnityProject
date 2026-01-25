using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.Core.News
{
	public static class NewsParser
	{
		private enum ScreenType
		{
			Main = 0,
			Archive = 1,
			Journal = 2,
			Bosterpack = 3,
			Payment = 4
		}

		public static void ShowNewsDialog(XmlNode p_node)
		{
			string p_imagePath = null;
			Action<BaseDialog> p_imageAction = null;
			ParseImageData(p_node["Image"], ref p_imagePath, ref p_imageAction);
			List<DialogButtonData> buttonsInfo = ParseButtons(p_node["Buttons"]);
			DialogNotificationManager.ShowNewsDialog(p_imagePath, p_imageAction, buttonsInfo, 4);
		}

		private static void ParseImageData(XmlNode p_node, ref string p_imagePath, ref Action<BaseDialog> p_imageAction)
		{
			if (p_node != null)
			{
				p_imagePath = NewsManager.ContentFolder + "/" + XmlUtils.ParseString(p_node.Attributes["Texture"]);
				p_imageAction = ParseButtonAction(p_node["Action"]);
			}
		}

		private static Action<BaseDialog> ParseButtonAction(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			string text = XmlUtils.ParseString(p_node.Attributes["Type"], string.Empty);
			switch (text)
			{
			case "Close":
				return delegate(BaseDialog p_window)
				{
					p_window.Dismiss();
				};
			case "Url":
				return delegate(BaseDialog p_window)
				{
					p_window.Dismiss();
					ApplicationController.OpenURL(XmlUtils.ParseString(p_node.Attributes["Link"], string.Empty));
				};
			case "Screen":
				return delegate(BaseDialog p_window)
				{
					p_window.Dismiss();
					switch (XmlUtils.ParseEnum(p_node.Attributes["ScreenId"], ScreenType.Main))
					{
					case ScreenType.Main:
						Manager.OpenMainScreen();
						break;
					case ScreenType.Archive:
						Manager.OpenArchiveCategory();
						break;
					case ScreenType.Journal:
						Manager.OpenQuestLog();
						break;
					case ScreenType.Bosterpack:
						Manager.OpenBoosterpack();
						break;
					case ScreenType.Payment:
						DialogNotificationManager.ShowPaymentDialog("Promo", 0);
						break;
					}
				};
			default:
				DebugUtils.LogError("[NewsDialog]: parse unknown button action - " + text);
				return null;
			}
		}

		private static List<DialogButtonData> ParseButtons(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			List<DialogButtonData> list = new List<DialogButtonData>();
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				list.Add(ParseButton(childNode));
			}
			return (list.Count <= 0) ? null : list;
		}

		private static DialogButtonData ParseButton(XmlNode p_node)
		{
			return new DialogButtonData(ParseButtonAction(p_node["Action"]), XmlUtils.ParseString(p_node.Attributes["Text"], string.Empty), (ButtonUI.Type)(int)Enum.Parse(typeof(ButtonUI.Type), XmlUtils.ParseString(p_node.Attributes["Color"], "Green")));
		}
	}
}
