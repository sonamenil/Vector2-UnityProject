using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.ArchiveCategory
{
	public class CouponsPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject _CouponPrefav;

		private List<CouponUI> _Coupons = new List<CouponUI>();

		public void Init()
		{
			RemoveCoupons();
			CreateCoupons();
		}

		private void RemoveCoupons()
		{
			if (_Coupons.Count <= 0)
			{
				return;
			}
			foreach (CouponUI coupon in _Coupons)
			{
				Object.DestroyImmediate(coupon.gameObject);
			}
			_Coupons.Clear();
		}

		private void CreateCoupons()
		{
			foreach (CouponGroupAttribute allCoupon in CouponsManager.GetAllCoupons())
			{
				if (!DataLocal.Current.IsPaidVersion || allCoupon.Type != CouponsManager.CouponType.EnergyRecharge)
				{
					CreateCoupon(allCoupon);
				}
			}
		}

		private void CreateCoupon(CouponGroupAttribute p_coupon)
		{
			GameObject gameObject = Object.Instantiate(_CouponPrefav);
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.SetAsLastSibling();
			CouponUI component = gameObject.GetComponent<CouponUI>();
			component.Init(p_coupon);
			_Coupons.Add(component);
		}

		public void RefreshCoupon(CouponsManager.CouponType p_couponType)
		{
			foreach (CouponUI coupon in _Coupons)
			{
				if (coupon.Coupon.Type == p_couponType)
				{
					coupon.Refresh();
					break;
				}
			}
		}
	}
}
