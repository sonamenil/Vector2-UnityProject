using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Boosterpack
{
	public class CurrencyRewardUI : MonoBehaviour
	{
		private const int _DefaultSize = 200;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private ResolutionImage _Icon;

		[SerializeField]
		private LabelAlias _Quantity;

		private CurrencyItem _Currency;

		public void Init(CurrencyItem p_currency, int p_size = 200)
		{
			_Currency = p_currency;
			_Icon.SpriteName = _Currency.CurrencyIcon;
			_Icon.color = _Currency.CurrencyColor;
			_Quantity.SetAlias(_Currency.Quantity.ToString());
			SetSize(p_size);
		}

		private void SetSize(int p_size)
		{
			float num = (float)p_size / 200f;
			_Content.localScale = new Vector3(num, num, 1f);
		}
	}
}
