using System;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Utilites;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs.Payment
{
	[ExecuteInEditMode]
	public class CategoryButton : MonoBehaviour
	{
		private const float _MinWidth = 336f;

		private const float _TextSpaces = 70f;

		[SerializeField]
		private string _GroupName;

		[SerializeField]
		private LabelAlias _Text;

		[SerializeField]
		private Button _Button;

		[SerializeField]
		private LayoutElement _LayoutElement;

		private static Color _ActiveTextColor = ColorUtils.FromHex("ABD8EB");

		private static Color _InactiveTextColor = ColorUtils.FromHex("7A9BAB");

		private Action<CategoryButton> _OnTapAction;

		public string GroupName
		{
			get
			{
				return _GroupName;
			}
		}

		public bool IsVisual
		{
			get
			{
				switch (_GroupName)
				{
				case "Premium":
					return false;
				default:
					return true;
				}
			}
		}

		public void Init(Action<CategoryButton> p_onTapAction)
		{
			_OnTapAction = p_onTapAction;
			Unselect();
		}

		public void Select()
		{
			_Text.color = _ActiveTextColor;
		}

		public void Unselect()
		{
			_Text.color = _InactiveTextColor;
		}

		public void OnButtonTap()
		{
			if (_OnTapAction != null)
			{
				_OnTapAction(this);
			}
		}

		public void UpdateSize()
		{
			_LayoutElement.minWidth = Mathf.Max(336f, _Text.preferredWidth + 70f);
		}
	}
}
