using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.ArchiveCategory
{
	public class CouponUI : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _Icon;

		[SerializeField]
		private Text _Quantity;

		[SerializeField]
		private LayoutElement _LayoutParams;

		private CouponGroupAttribute _Coupon;

		public CouponGroupAttribute Coupon
		{
			get
			{
				return _Coupon;
			}
		}

		public void Init(CouponGroupAttribute p_coupon)
		{
			_Coupon = p_coupon;
			_Icon.SpriteName = _Coupon.ButtonIcon;
			_Icon.color = _Coupon.ButtonIconColor;
			Refresh();
		}

		public void Refresh()
		{
			_Quantity.text = _Coupon.Quantity.ToString();
			_LayoutParams.preferredWidth = _Icon.preferredWidth + _Quantity.preferredWidth + (_Quantity.rectTransform.localPosition.x - _Icon.rectTransform.localPosition.x);
		}

		public void OnTap()
		{
			Tooltip.UISettings uISettings = new Tooltip.UISettings();
			uISettings.Text = _Coupon.Description;
			uISettings.Parent = GetComponent<RectTransform>();
			uISettings.Shift = new Vector2(30f, -50f);
			uISettings.Pivot = Tooltip.Pivot.TopLeft;
			uISettings.OpenAnimation = Tooltip.AnimationType.Fade;
			uISettings.CloseAnimation = Tooltip.AnimationType.Instant;
			uISettings.BlockTouches = false;
			DialogNotificationManager.ShowTooltip(uISettings);
		}
	}
}
