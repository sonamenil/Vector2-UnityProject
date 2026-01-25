using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class SimpleNotification : Notification
	{
		[SerializeField]
		private ResolutionImage _Image;

		[SerializeField]
		private RectTransform _ImageBackgroundRect;

		[SerializeField]
		private LabelAlias _Text;

		[SerializeField]
		private RectTransform _CardAnchor;

		[SerializeField]
		private BaseCardUI _BaseCardPrefab;

		private RectTransform _GraphicsRect;

		public void Init(Parameters p_parameters)
		{
			_Parameters = p_parameters;
			SetGraphics();
			SetRootPosition();
			SetGraphicsPosition();
			SetTextPosition();
		}

		private void SetGraphics()
		{
			_Image.enabled = false;
			_CardAnchor.gameObject.SetActive(false);
			_GraphicsRect = null;
			_Text.SetAlias(_Parameters.Text);
			if (!string.IsNullOrEmpty(_Parameters.Image))
			{
				_Image.enabled = true;
				ImageResourceFinder.SetImage(_Image, _Parameters.Image, true);
				_GraphicsRect = _Image.gameObject.GetComponent<RectTransform>();
				if (_GraphicsRect.sizeDelta.x > 350f || _GraphicsRect.sizeDelta.y > 350f)
				{
					float num = Mathf.Max(_GraphicsRect.sizeDelta.x, _GraphicsRect.sizeDelta.y);
					float num2 = 350f / num;
					_GraphicsRect.sizeDelta = new Vector2(_GraphicsRect.sizeDelta.x * num2, _GraphicsRect.sizeDelta.y * num2);
				}
			}
			else if (!string.IsNullOrEmpty(_Parameters.Card))
			{
				_CardAnchor.gameObject.SetActive(true);
				CardsGroupAttribute card = CardsGroupAttribute.Create(_Parameters.Card);
				BaseCardUI baseCardUI = Object.Instantiate(_BaseCardPrefab);
				baseCardUI.CardSize = 200;
				baseCardUI.Card = card;
				baseCardUI.transform.SetParent(_CardAnchor, false);
				_GraphicsRect = _CardAnchor;
			}
			_ImageBackgroundRect.gameObject.SetActive(_GraphicsRect != null);
		}

		private void SetGraphicsPosition()
		{
			if (_GraphicsRect != null)
			{
				Vector2 vector = Vector2.zero;
				Vector2 anchoredPosition = Vector2.zero;
				switch (_Parameters.Orientation)
				{
				case Orientation.Top:
				case Orientation.Bottom:
				case Orientation.Left:
				case Orientation.LeftBottom:
					vector = new Vector2(1f, 0.5f);
					anchoredPosition = new Vector2((0f - (_ImageBackgroundRect.sizeDelta.x - _GraphicsRect.sizeDelta.x)) / 2f, 0f);
					break;
				case Orientation.Right:
					vector = new Vector2(0f, 0.5f);
					anchoredPosition = new Vector2((_ImageBackgroundRect.sizeDelta.x - _GraphicsRect.sizeDelta.x) / 2f, 0f);
					break;
				}
				_GraphicsRect.pivot = vector;
				_GraphicsRect.anchorMin = vector;
				_GraphicsRect.anchorMax = vector;
				_GraphicsRect.anchoredPosition = anchoredPosition;
				_ImageBackgroundRect.pivot = vector;
				_ImageBackgroundRect.anchorMin = vector;
				_ImageBackgroundRect.anchorMax = vector;
			}
		}

		private void SetTextPosition()
		{
			RectTransform component = _Text.gameObject.GetComponent<RectTransform>();
			Vector2 offsetMin = Vector2.zero;
			Vector2 vector = Vector2.zero;
			if (_GraphicsRect != null)
			{
				switch (_Parameters.Orientation)
				{
				case Orientation.Top:
				case Orientation.Bottom:
				case Orientation.Left:
				case Orientation.LeftBottom:
					offsetMin = new Vector2(50f, 0f);
					vector = new Vector2(50f + _ImageBackgroundRect.sizeDelta.x, 0f);
					_Text.alignment = TextAnchor.UpperRight;
					break;
				case Orientation.Right:
					offsetMin = new Vector2(50f + _ImageBackgroundRect.sizeDelta.x, 0f);
					vector = new Vector2(50f, 0f);
					_Text.alignment = TextAnchor.UpperLeft;
					break;
				}
			}
			else
			{
				vector = new Vector2(50f, 0f);
				offsetMin = new Vector2(50f, 0f);
				_Text.alignment = TextAnchor.MiddleCenter;
			}
			component.offsetMax = -vector;
			component.offsetMin = offsetMin;
			component.sizeDelta = new Vector2(component.sizeDelta.x, 295f);
		}
	}
}
