using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Boosterpack
{
	public class CouponRewardUI : MonoBehaviour
	{
		private const int _DefaultSize = 200;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private ResolutionImage _Icon;

		[SerializeField]
		private ResolutionImage _SecondaryIcon;

		private CouponGroupAttribute _Coupon;

		public void Init(CouponGroupAttribute p_coupon, int p_size = 200)
		{
			_Coupon = p_coupon;
			_Icon.SpriteName = _Coupon.ItemImage;
			_SecondaryIcon.SpriteName = _Coupon.ButtonIcon;
			_SecondaryIcon.color = _Coupon.ButtonIconColor;
			SetSize(p_size);
		}

		private void SetSize(int p_size)
		{
			float num = (float)p_size / 200f;
			_Content.localScale = new Vector3(num, num, 1f);
		}
	}
}
