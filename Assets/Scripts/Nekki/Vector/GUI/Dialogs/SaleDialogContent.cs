using System.Collections.Generic;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class SaleDialogContent : DialogContent
	{
		public const int StarterPackPromoID = 0;

		public const int CouponsPackPromoID = 1;

		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _DialogDescription;

		[SerializeField]
		private LabelAlias _ProductDescription;

		[SerializeField]
		private ResolutionImage _ProductImage;

		[SerializeField]
		private TimerUI _ProductTimer;

		[SerializeField]
		private GameObject _EffectsHolder;

		[SerializeField]
		private GameObject _ProductEffectUIPrefab;

		public static bool _IsSaleShowed;

		public static bool IsSaleShowed
		{
			get
			{
				return _IsSaleShowed;
			}
		}

		public void Init(int p_saleID)
		{
			_IsSaleShowed = true;
			List<DialogButtonData> list = new List<DialogButtonData>();
			list.Add(new DialogButtonData(OnShowTap, "^GUI.Buttons.Show^", ButtonUI.Type.Green));
			list.Add(new DialogButtonData(OnCloseTap, "^GUI.Buttons.Close^", ButtonUI.Type.Blue));
			Init(list);
			List<Product> productsByGroup = ProductManager.Current.GetProductsByGroup("Promo");
			SetProduct(p_saleID, (productsByGroup.Count != 1) ? productsByGroup[p_saleID] : productsByGroup[0]);
			SetLabels(p_saleID);
		}

		public void OnCloseTap(BaseDialog dialog)
		{
			dialog.Dismiss();
		}

		private void OnShowTap(BaseDialog dialog)
		{
			if (PaymentDialog.Current == null)
			{
				DialogNotificationManager.ShowPaymentDialog("Promo", 0);
			}
			else
			{
				PaymentDialog.Current.Close();
			}
			dialog.Dismiss();
		}

		public void OnTimerExpire()
		{
			base.Parent.Dismiss();
		}

		private void SetLabels(int p_saleID)
		{
			_Title.SetAlias(string.Format("^GUI.Labels.SaleWindow{0}.Title^", p_saleID + 1));
			_DialogDescription.SetAlias(string.Format("^GUI.Labels.SaleWindow{0}.Description^", p_saleID + 1));
		}

		private void SetProduct(int p_saleID, Product p_product)
		{
			_ProductDescription.SetAlias(p_product.Description);
			_ProductImage.SpriteName = p_product.Icon + "_alpha";
			SetEffects(p_product.Effects);
			if (p_product.TimerName != null)
			{
				_ProductTimer.gameObject.SetActive(true);
				_ProductTimer.Id = p_product.TimerName;
			}
			else
			{
				_ProductTimer.gameObject.SetActive(true);
			}
		}

		private void SetEffects(List<ProductEffect> p_effects)
		{
			for (int num = _EffectsHolder.transform.childCount - 1; num >= 0; num--)
			{
				Object.DestroyImmediate(_EffectsHolder.transform.GetChild(num).gameObject);
			}
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
			GameObject gameObject = Object.Instantiate(_ProductEffectUIPrefab);
			gameObject.transform.SetParent(_EffectsHolder.transform, false);
			gameObject.transform.SetAsLastSibling();
			ProductEffectUI component = gameObject.GetComponent<ProductEffectUI>();
			component.Init(p_productEffect);
		}
	}
}
