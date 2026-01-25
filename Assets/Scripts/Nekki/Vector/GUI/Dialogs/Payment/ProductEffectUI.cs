using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.Core.Payment.ProductEffects;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs.Payment
{
	public class ProductEffectUI : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _CurrencyIcon;

		[SerializeField]
		private ResolutionImage _ItemIcon;

		[SerializeField]
		private LabelAlias _QuantityText;

		private ProductEffect _ProductEffect;

		public void Init(ProductEffect p_prouctEffect)
		{
			Reset();
			_ProductEffect = p_prouctEffect;
			if (_ProductEffect.IsCurrency)
			{
				SetupCurrencyMode(_ProductEffect.As<ProductEffect_Currency>());
			}
			else
			{
				SetupItemMode(_ProductEffect.As<ProductEffect_Item>());
			}
		}

		private void Reset()
		{
			_CurrencyIcon.gameObject.SetActive(false);
			_ItemIcon.gameObject.SetActive(false);
			_QuantityText.SetAlias(string.Empty);
		}

		private void SetupCurrencyMode(ProductEffect_Currency p_effect)
		{
			_CurrencyIcon.gameObject.SetActive(true);
			_QuantityText.SetAlias(p_effect.CurrencyValue.ToString());
		}

		private void SetupItemMode(ProductEffect_Item p_effect)
		{
			_ItemIcon.gameObject.SetActive(true);
			_ItemIcon.SpriteName = p_effect.SpriteName;
			_QuantityText.SetAlias(p_effect.Quantity.ToString());
		}
	}
}
