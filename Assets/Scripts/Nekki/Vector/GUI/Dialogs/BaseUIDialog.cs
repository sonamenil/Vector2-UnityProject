using System.Collections.Generic;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	[ExecuteInEditMode]
	public class BaseUIDialog : BaseDialog
	{
		[SerializeField]
		private RectTransform _TopLineRect;

		[SerializeField]
		private RectTransform _ButtonPanelRect;

		[SerializeField]
		private List<ButtonUI> _Buttons;

		[SerializeField]
		private List<Transform> _ButtonsPlaceholders;

		private RectTransform _DialogRect;

		[SerializeField]
		private DialogContent _Content;

		public DialogContent Content
		{
			get
			{
				return _Content;
			}
		}

		public T Init<T>(GameObject p_prefab) where T : DialogContent
		{
			_DialogRect = GetComponent<RectTransform>();
			if (_Content == null)
			{
				GameObject gameObject = Object.Instantiate(p_prefab);
				gameObject.transform.SetParent(base.transform, false);
				_Content = gameObject.GetComponent<T>();
				_Content.Parent = this;
			}
			UpdateDialogSize();
			return _Content as T;
		}

		public void UpdateDialogSize()
		{
			_Content.DeltaY = _ButtonPanelRect.sizeDelta.y / 2f - _TopLineRect.sizeDelta.y / 2f;
			float height = _Content.Height;
			_DialogRect.sizeDelta = new Vector2(_DialogRect.sizeDelta.x, height + _ButtonPanelRect.sizeDelta.y + _TopLineRect.sizeDelta.y);
		}

		public void InitButtons(List<DialogButtonData> p_buttons)
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				_Buttons[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < p_buttons.Count; j++)
			{
				_Buttons[j].gameObject.SetActive(true);
				p_buttons[j].InitButton(_Buttons[j]);
			}
		}

		public void RefreshButtonPosition()
		{
			int num = 0;
			for (int i = 0; i < _Buttons.Count; i++)
			{
				if (_Buttons[i].gameObject.activeSelf)
				{
					_Buttons[i].transform.SetParent(_ButtonsPlaceholders[num], false);
					num++;
				}
			}
		}

		public void OnButton1Tap()
		{
			_Content.OnButton1Tap();
		}

		public void OnButton2Tap()
		{
			_Content.OnButton2Tap();
		}

		public void OnButton3Tap()
		{
			_Content.OnButton3Tap();
		}

		public void OnButton4Tap()
		{
			_Content.OnButton4Tap();
		}

		public void HideButtons()
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				_Buttons[i].gameObject.SetActive(false);
			}
		}

		public void ShowButtons()
		{
			for (int i = 0; i < _Buttons.Count; i++)
			{
				_Buttons[i].gameObject.SetActive(true);
			}
		}
	}
}
