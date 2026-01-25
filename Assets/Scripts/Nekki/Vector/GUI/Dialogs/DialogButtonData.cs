using System;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class DialogButtonData
	{
		private Action<BaseDialog> _OnTap;

		private string _Text;

		private string _Image;

		private Color _ImageColor;

		private string _Count;

		private ButtonUI.Type _Type;

		public string _SoundAlias;

		private bool _ToUpperCase;

		private bool _IsActivateByBackBtn;

		private ButtonUI _Target;

		public ButtonUI Target
		{
			get
			{
				return _Target;
			}
		}

		public DialogButtonData(Action<BaseDialog> p_onTap, string p_text, ButtonUI.Type p_type, bool p_upperCase = true)
		{
			_OnTap = p_onTap;
			_Text = p_text;
			_Type = p_type;
			_ToUpperCase = p_upperCase;
			_Image = null;
			_ImageColor = Color.white;
			_Count = null;
		}

		public DialogButtonData(Action<BaseDialog> p_onTap, string p_text, string p_count, string p_image, Color p_imageColor, ButtonUI.Type p_type, bool p_upperCase = true, string p_soundAlias = null)
		{
			_OnTap = p_onTap;
			_Text = p_text;
			_Image = p_image;
			_ImageColor = p_imageColor;
			_Count = p_count;
			_Type = p_type;
			_SoundAlias = p_soundAlias;
			_ToUpperCase = p_upperCase;
		}

		public DialogButtonData SetActivateByBackBtn()
		{
			_IsActivateByBackBtn = true;
			return this;
		}

		public void InitButton(ButtonUI p_button)
		{
			_Target = p_button;
			p_button.ButtonText.ToUpperCase = _ToUpperCase;
			p_button.ButtonText.SetAlias(_Text);
			p_button.SetType(_Type, true, _SoundAlias);
			if (_Image == null)
			{
				p_button.TurnToFree();
				return;
			}
			p_button.TurnToPaid();
			p_button.PaidIcon.SpriteName = _Image;
			p_button.PaidIcon.color = _ImageColor;
			p_button.PaidCount.SetAlias(_Count);
		}

		public void Activate(BaseDialog p_parent)
		{
			if (_OnTap != null)
			{
				_OnTap(p_parent);
			}
		}

		public void OnBackButton(BaseDialog p_parent)
		{
			if (_IsActivateByBackBtn)
			{
				Activate(p_parent);
			}
		}

		public void TurnOn()
		{
			if (_Target != null)
			{
				_Target.gameObject.SetActive(true);
			}
		}

		public void TurnOff()
		{
			if (_Target != null)
			{
				_Target.gameObject.SetActive(false);
			}
		}
	}
}
