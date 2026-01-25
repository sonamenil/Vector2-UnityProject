using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs.Payment
{
	public class CategoryButtons : MonoBehaviour
	{
		private PaymentDialog _Parent;

		private List<CategoryButton> _Buttons = new List<CategoryButton>();

		private CategoryButton _SelectedButton;

		public void Init(PaymentDialog p_parent, string p_selectedGroup)
		{
			_Parent = p_parent;
			SetupButtons();
			SetupSelection(p_selectedGroup);
		}

		private void SetupButtons()
		{
			if (_Buttons.Count == 0)
			{
				GetComponentsInChildren(true, _Buttons);
				foreach (CategoryButton button in _Buttons)
				{
					button.Init(OnButtonTap);
				}
			}
			UpdateButtonsActive();
		}

		private void SetupSelection(string p_selectedGroup)
		{
			if (_SelectedButton != null)
			{
				_SelectedButton.Unselect();
			}
			_SelectedButton = null;
			foreach (CategoryButton button in _Buttons)
			{
				if (button.GroupName == p_selectedGroup && (button.gameObject.activeSelf || !button.IsVisual))
				{
					_SelectedButton = button;
					break;
				}
			}
			if (_SelectedButton == null)
			{
				_SelectedButton = _Buttons[0];
			}
			_SelectedButton.Select();
			_Parent.SelectProductsGroup(_SelectedButton.GroupName, true);
		}

		private void OnButtonTap(CategoryButton p_button)
		{
			_SelectedButton.Unselect();
			p_button.Select();
			_SelectedButton = p_button;
			_Parent.SelectProductsGroup(_SelectedButton.GroupName);
		}

		public void UpdateButtonsActive()
		{
			int num = 0;
			foreach (CategoryButton button in _Buttons)
			{
				button.UpdateSize();
				button.gameObject.SetActive(_Parent.IsProductsOfGroupExists(button.GroupName) && button.IsVisual);
				if (button.gameObject.activeSelf)
				{
					num++;
				}
			}
			base.gameObject.SetActive(num > 1);
		}
	}
}
