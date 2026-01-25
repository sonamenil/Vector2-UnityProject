using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class StarterPackUIController : MonoBehaviour
	{
		private const string _UnknownProtocolAlias = "^GUI.Labels.UnknownProtocol^";

		private const string _UnknownSpriteName = "box_unknown";

		[SerializeField]
		private ResolutionImage _StarterPackImage;

		[SerializeField]
		private LabelAlias _Name;

		[SerializeField]
		private GameObject _DetailsButton;

		[SerializeField]
		private bool _HideDetailsBtn;

		public void UserSelectStarterPack(StarterPackItem p_item)
		{
			if (p_item == null)
			{
				_Name.SetAlias("^GUI.Labels.UnknownProtocol^");
				SetUnknownFrame();
				_StarterPackImage.SpriteName = "box_unknown";
				_StarterPackImage.SetNativeSize();
				if (_DetailsButton != null)
				{
					_DetailsButton.SetActive(false);
				}
				return;
			}
			if (_DetailsButton != null)
			{
				_DetailsButton.SetActive(!_HideDetailsBtn);
			}
			_Name.SetAlias(p_item.VisualName);
			_StarterPackImage.SpriteName = p_item.ItemImage;
			_StarterPackImage.SetNativeSize();
			if (p_item.IsBlock)
			{
				SetLockedFrame();
			}
			else
			{
				SetNormalFrame();
			}
		}

		private void SetLockedFrame()
		{
			_StarterPackImage.Alpha = 0.5f;
		}

		private void SetNormalFrame()
		{
			_StarterPackImage.color = Color.white;
		}

		private void SetUnknownFrame()
		{
			_StarterPackImage.Alpha = 1f;
		}

		public void ShowDetails()
		{
			Manager.OpenDetails();
		}
	}
}
