using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Shop
{
	public class GadgetCardInfo : MonoBehaviour
	{
		[SerializeField]
		private BaseCardUISettings _CardUISettings;

		[SerializeField]
		private GameObject _BaseCardUIPrefab;

		[SerializeField]
		private GameObject _CardHolder;

		[SerializeField]
		private LabelAlias _CardName;

		[SerializeField]
		private LabelAlias _CardText;

		[SerializeField]
		public CanvasGroup _CardContent;

		[SerializeField]
		public Transform _CardContentTransform;

		[SerializeField]
		private ButtonUI _BackButton;

		[SerializeField]
		private ButtonUI _BoostButton;

		private BaseCardUI _Card;

		public CardsGroupAttribute Card
		{
			get
			{
				if (_Card == null)
				{
					return null;
				}
				return _Card.Card;
			}
		}

		public void Init(CardsGroupAttribute p_card)
		{
			if (_Card == null)
			{
				CreateCardUI();
			}
			_Card.Card = p_card;
			_CardName.SetAlias(_Card.Card.CardVisualName);
			_CardText.SetAlias(_Card.Card.CardText);
			RefreshBoostButton();
		}

		private void CreateCardUI()
		{
			GameObject gameObject = Object.Instantiate(_BaseCardUIPrefab);
			gameObject.transform.SetParent(_CardHolder.transform, false);
			_Card = gameObject.GetComponent<BaseCardUI>();
			_Card.UISettings = _CardUISettings;
			_Card.NeedShowProgressBar = true;
			_Card.NeedShowCurrentLevelProgress = false;
		}

		public void RefreshCard()
		{
			_CardName.SetAlias(_Card.Card.CardVisualName);
			_CardText.SetAlias(_Card.Card.CardText);
			_Card.RefreshBoost();
		}

		public void RefreshBoostButton()
		{
			if (_Card.Card.UserCardTotalLevel < _Card.Card.CardMaxLevel)
			{
				int userBoostPrice = _Card.Card.UserBoostPrice;
				bool flag = CouponsManager.HaveSuitableCoupon(CouponsManager.CouponType.CardsBoost);
				Color red = Color.red;
				if (flag)
				{
					_BoostButton.PaidIcon.SpriteName = CouponsManager.GetCouponButtonIcon(CouponsManager.CouponType.CardsBoost);
					red = CouponsManager.GetCouponButtonIconColor(CouponsManager.CouponType.CardsBoost);
					_BoostButton.PaidCount.SetAlias(string.Empty);
					_BoostButton.ButtonText.SetAlias("^GUI.Buttons.Archive.Boost^");
				}
				else
				{
					_BoostButton.PaidIcon.SpriteName = CurrencyInfo.GetCurrencySprite(CurrencyType.Money3);
					red = CurrencyInfo.GetCurrencyColor(CurrencyType.Money3);
					_BoostButton.PaidCount.SetAlias(userBoostPrice.ToString());
					_BoostButton.ButtonText.SetAlias("^GUI.Buttons.Archive.Boost^");
				}
				red.a = _BoostButton.PaidIcon.color.a;
				_BoostButton.PaidIcon.color = red;
				if (flag || (int)DataLocal.Current.Money3 >= userBoostPrice)
				{
					_BoostButton.SetType(ButtonUI.Type.Green, false);
				}
				else
				{
					_BoostButton.SetType(ButtonUI.Type.Grey, false);
				}
				_BoostButton.gameObject.SetActive(true);
			}
			else
			{
				_BoostButton.gameObject.SetActive(false);
			}
		}

		public Tweener MoveContentY(float p_y, float p_duration, Ease p_ease)
		{
			return _CardContentTransform.DOLocalMoveY(p_y, p_duration).SetEase(p_ease);
		}

		public void ChangeButtonsAlpha(float p_alpha, float p_duration)
		{
			_BackButton.Backdround.DOFade(p_alpha, p_duration);
			_BackButton.ButtonText.DOFade(p_alpha, p_duration);
			_BoostButton.Backdround.DOFade(p_alpha, p_duration);
			_BoostButton.ButtonText.DOFade(p_alpha, p_duration);
			_BoostButton.PaidIcon.DOFade(p_alpha, p_duration);
			_BoostButton.PaidPlate.DOFade(p_alpha, p_duration);
			_BoostButton.PaidCount.DOFade(p_alpha, p_duration);
		}
	}
}
