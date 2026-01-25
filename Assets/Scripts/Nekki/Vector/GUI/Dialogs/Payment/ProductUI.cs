using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs.Payment
{
	public class ProductUI : MonoBehaviour
	{
		private const string _BuyAlias = "^GUI.Buttons.Buy^";

		private const string _FreeAlias = "^GUI.Buttons.GetFree^";

		private const string _RecievingDataAlias = "^GUI.Labels.Recieving^";

		[SerializeField]
		private ResolutionImage _Icon;

		[SerializeField]
		private LabelAlias _TitleLabel;

		[SerializeField]
		private LabelAlias _DescriptionLabel;

		[SerializeField]
		private Button _BuyButton;

		[SerializeField]
		private LabelAlias _PriceLabel;

		[SerializeField]
		private UIPoligon _TopBg;

		[SerializeField]
		private GameObject _ProductEffectUIPrefab;

		[SerializeField]
		private HorizontalLayoutGroup _EffectsHolder;

		[SerializeField]
		private TimerUI _Timer;

		[SerializeField]
		private LoadingCircle _RecievingDataCircle;

		[SerializeField]
		private Image _SaleBG;

		[SerializeField]
		private Text _SaleText;

		private Product _Product;

		private Action<ProductUI> _OnBuyTapAction;

		private Action<ProductUI> _OnTimerExpireAction;

		private List<ProductEffectUI> _ProductEffectsUI = new List<ProductEffectUI>();

		public Product Product
		{
			get
			{
				return _Product;
			}
		}

		public string Id
		{
			get
			{
				return _Product.Id;
			}
		}

		public string Group
		{
			get
			{
				return _Product.Group;
			}
		}

		public bool IsAds
		{
			get
			{
				return _Product.IsAds;
			}
		}

		public bool IsRecievingData
		{
			get
			{
				return !_Product.IsAds && !ProductManager.Current.IsGetProductDataTransactionCompleted;
			}
		}

		public void Init(Product p_product, Action<ProductUI> p_onBuyTapAction, Action<ProductUI> p_onTimerExpireAction)
		{
			_Product = p_product;
			_OnBuyTapAction = p_onBuyTapAction;
			_OnTimerExpireAction = p_onTimerExpireAction;
			base.gameObject.name = string.Format("Product_{0}_{1}", Id, Group);
			UpdateProductInfo();
			UpdateProductEffect();
		}

		public void UpdateProductInfo()
		{
			SetIcon(_Product.Icon);
			SetTitle(_Product.Title);
			SetDescription(_Product.Description);
			SetPrice((!DeviceInformation.IsAndroid) ? (_Product.Price + " " + _Product.PriceSymbol) : _Product.Price);
			SetRecievingDataStatus();
		}

		private void UpdateProductEffect()
		{
			SetTimerStatus();
			SetEffects(_Product.Effects);
		}

		private void SetIcon(string p_icon)
		{
			if (_Icon.SpriteName != p_icon)
			{
				_Icon.SpriteName = p_icon;
			}
		}

		private void SetTitle(string p_title)
		{
			p_title = p_title.Replace("(Vector 2)", string.Empty).Replace("(Vector 2 Premium)", string.Empty);
			if (_TitleLabel.Alias != p_title)
			{
				_TitleLabel.SetAlias(p_title);
			}
		}

		private void SetDescription(string p_description)
		{
			if (_DescriptionLabel.Alias != p_description)
			{
				_DescriptionLabel.SetAlias(p_description);
			}
		}

		private void SetPrice(string p_price)
		{
			string text = ((!_Product.IsFree) ? ("^GUI.Buttons.Buy^ " + p_price) : "^GUI.Buttons.GetFree^");
			if (_PriceLabel.Alias != text)
			{
				_PriceLabel.SetAlias(text);
			}
			_SaleBG.gameObject.SetActive(_Product.SaleText != null);
			if (_Product.SaleText != null)
			{
				_SaleText.text = _Product.SaleText;
			}
		}

		private void SetRecievingDataStatus()
		{
			if (IsRecievingData)
			{
				_PriceLabel.gameObject.SetActive(false);
				_SaleBG.gameObject.SetActive(false);
				_EffectsHolder.gameObject.SetActive(false);
				_BuyButton.interactable = false;
				_DescriptionLabel.SetAlias("^GUI.Labels.Recieving^");
				_RecievingDataCircle.Play();
			}
			else
			{
				_PriceLabel.gameObject.SetActive(true);
				_EffectsHolder.gameObject.SetActive(true);
				_BuyButton.interactable = true;
				_RecievingDataCircle.Stop();
			}
		}

		private void SetTimerStatus()
		{
			if (_Product.TimerName != null)
			{
				_TopBg.Points[1] = new Vector2(180f, -74f);
				_TopBg.Points[2] = new Vector2(-180f, -74f);
				_Timer.gameObject.SetActive(true);
				_Timer.Id = _Product.TimerName;
			}
			else
			{
				_TopBg.Points[1] = new Vector2(180f, -34f);
				_TopBg.Points[2] = new Vector2(-180f, -34f);
				_Timer.gameObject.SetActive(false);
			}
		}

		private void SetEffects(List<ProductEffect> p_effects)
		{
			if (p_effects == null)
			{
				return;
			}
			foreach (ProductEffect p_effect in p_effects)
			{
				if (p_effect.IsVisual)
				{
					CreateEffectUI(p_effect);
				}
			}
		}

		private void CreateEffectUI(ProductEffect p_productEffect)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_ProductEffectUIPrefab);
			gameObject.transform.SetParent(_EffectsHolder.transform, false);
			gameObject.transform.SetAsLastSibling();
			ProductEffectUI component = gameObject.GetComponent<ProductEffectUI>();
			component.Init(p_productEffect);
			_ProductEffectsUI.Add(component);
		}

		public void OnBuyTap()
		{
			if (_OnBuyTapAction != null)
			{
				_OnBuyTapAction(this);
			}
		}

		public void OnTimerExpire()
		{
			if (_OnTimerExpireAction != null)
			{
				_OnTimerExpireAction(this);
			}
		}
	}
}
