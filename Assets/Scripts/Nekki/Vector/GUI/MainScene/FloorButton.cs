using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using UIFigures;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.MainScene
{
	public class FloorButton : MonoBehaviour
	{
		public enum ButtonMode
		{
			StarterPack = 0,
			NextPage = 1,
			PrevPage = 2
		}

		private static Color _NormalColor = new Color(0.369f, 0.643f, 0.733f);

		private static Color _SelectedColor = new Color(0.682f, 0.949f, 0.988f);

		private static Color _BlockColor = new Color(0.49f, 0.522f, 0.576f, 0.8f);

		[Header("Content")]
		[SerializeField]
		private GameObject _Content;

		[SerializeField]
		private UIFrameRectBorder _Border;

		[Header("Normal Frame")]
		[SerializeField]
		private GameObject _NormalFrame;

		[SerializeField]
		private Text _FloorText;

		[SerializeField]
		private Text _FloorNumber;

		[SerializeField]
		[Header("Unknown Frame")]
		private GameObject _UnknownFrame;

		[SerializeField]
		[Header("Change Page Frame")]
		private GameObject _ChangePageFrame;

		[SerializeField]
		private Text _ChangePageText;

		private FloorButtonsController _Parent;

		private ButtonMode _Mode;

		private StarterPackItem _StarterPackItem;

		public GameObject Content
		{
			get
			{
				return _Content;
			}
		}

		public ButtonMode Mode
		{
			get
			{
				return _Mode;
			}
		}

		public StarterPackItem StarterPackItem
		{
			get
			{
				return _StarterPackItem;
			}
			set
			{
				_StarterPackItem = value;
				_Mode = ButtonMode.StarterPack;
				UpdateStarterPackUI();
			}
		}

		public bool IsBlock
		{
			get
			{
				return _StarterPackItem == null || _StarterPackItem.IsBlock;
			}
		}

		public void SetNextPageMode()
		{
			_StarterPackItem = null;
			_Mode = ButtonMode.NextPage;
			UpdateStarterPackUI();
		}

		public void SetPrevPageMode()
		{
			_StarterPackItem = null;
			_Mode = ButtonMode.PrevPage;
			UpdateStarterPackUI();
		}

		public void Init(FloorButtonsController p_parent)
		{
			_Parent = p_parent;
		}

		private void UpdateStarterPackUI()
		{
			if (_StarterPackItem == null)
			{
				SetBlockColor();
				if (_Mode == ButtonMode.StarterPack)
				{
					SetUnknownFrameSprite();
				}
				else
				{
					SetPageChangeFrameSprite();
				}
				return;
			}
			int startFloor = _StarterPackItem.StartFloor;
			_FloorNumber.text = ((startFloor >= 10) ? startFloor.ToString() : ("0" + startFloor));
			SetNormalColor();
			if (_StarterPackItem.IsBlock)
			{
				SetBlockColor();
			}
			SetNormalFrameSprite();
		}

		public void OnButtonClick()
		{
			if (_Parent.FloorButtonClicked(this))
			{
				AudioManager.PlaySound("select_button");
			}
			else
			{
				AudioManager.PlaySound("grey_button");
			}
		}

		private void SetNormalFrameSprite()
		{
			_NormalFrame.SetActive(true);
			_UnknownFrame.SetActive(false);
			_ChangePageFrame.SetActive(false);
		}

		private void SetUnknownFrameSprite()
		{
			_NormalFrame.SetActive(false);
			_UnknownFrame.SetActive(true);
			_ChangePageFrame.SetActive(false);
		}

		private void SetPageChangeFrameSprite()
		{
			_NormalFrame.SetActive(false);
			_UnknownFrame.SetActive(false);
			_ChangePageFrame.SetActive(true);
		}

		public void SetShift()
		{
			_Content.transform.localPosition = new Vector3(-40f, 0f, 0f);
		}

		public void ResetShift()
		{
			_Content.transform.localPosition = default(Vector3);
		}

		public void SetNormalColor()
		{
			_Border.color = _NormalColor;
			_FloorNumber.color = _NormalColor;
			_FloorText.color = _NormalColor;
			_ChangePageText.color = _NormalColor;
		}

		public void SetSelectedColor()
		{
			_Border.color = _SelectedColor;
			_FloorNumber.color = _SelectedColor;
			_FloorText.color = _SelectedColor;
			_ChangePageText.color = _SelectedColor;
		}

		public void SetBlockColor()
		{
			_Border.color = _BlockColor;
			_FloorNumber.color = _BlockColor;
			_FloorText.color = _BlockColor;
			_ChangePageText.color = _BlockColor;
		}
	}
}
