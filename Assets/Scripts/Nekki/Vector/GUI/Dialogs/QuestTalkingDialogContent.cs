using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class QuestTalkingDialogContent : DialogContent
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _TextLabel;

		[SerializeField]
		private ResolutionImage _Image;

		private Action _OnClose;

		public void Init(string p_title, string p_text, string p_buttonText, string p_image, Action p_onClose)
		{
			_OnClose = p_onClose;
			List<DialogButtonData> list = new List<DialogButtonData>();
			list.Add(new DialogButtonData(OnCloseTap, p_buttonText, ButtonUI.Type.Blue));
			Init(list);
			if (!string.IsNullOrEmpty(p_image))
			{
				_Image.SpriteName = p_image;
				_Image.enabled = true;
			}
			else
			{
				_Image.enabled = false;
			}
			_Title.SetAlias(p_title);
			_TextLabel.SetAlias(p_text);
		}

		public void Init(string p_title, string p_text, string p_image, List<DialogButtonData> p_buttons)
		{
			Init(p_buttons);
			if (!string.IsNullOrEmpty(p_image))
			{
				_Image.SpriteName = p_image;
				_Image.enabled = true;
			}
			else
			{
				_Image.enabled = false;
			}
			_Title.SetAlias(p_title);
			_TextLabel.SetAlias(p_text);
		}

		private void OnCloseTap(BaseDialog p_dialog)
		{
			if (_OnClose != null)
			{
				_OnClose();
			}
			base.Parent.Dismiss();
		}
	}
}
