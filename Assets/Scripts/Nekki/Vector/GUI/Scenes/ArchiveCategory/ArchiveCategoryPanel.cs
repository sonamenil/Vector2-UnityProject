using System;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.ArchiveCategory
{
	public class ArchiveCategoryPanel : UIModule
	{
		[SerializeField]
		private CouponsPanel _CouponsPanel;

		[SerializeField]
		private CategoryButtonsPanel _CategoryButtonsPanel;

		protected override void Init()
		{
			base.Init();
		}

		protected override void Free()
		{
			base.Free();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			Refresh();
			CouponsManager.OnCouponSpent = (Action<CouponsManager.CouponType>)Delegate.Combine(CouponsManager.OnCouponSpent, new Action<CouponsManager.CouponType>(OnCouponSpent));
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			CouponsManager.OnCouponSpent = (Action<CouponsManager.CouponType>)Delegate.Remove(CouponsManager.OnCouponSpent, new Action<CouponsManager.CouponType>(OnCouponSpent));
		}

		public void Refresh()
		{
			_CouponsPanel.Init();
			_CategoryButtonsPanel.Init();
		}

		private void OnCouponSpent(CouponsManager.CouponType p_couponType)
		{
			_CouponsPanel.RefreshCoupon(p_couponType);
		}
	}
}
