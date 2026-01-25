using System;
using System.Collections.Generic;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Menu;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public static class CouponsManager
	{
		public enum CouponType
		{
			CardsBoost = 0,
			Saveme = 1,
			EnergyRecharge = 2,
			Reroll = 3,
			Unknown = 4
		}

		private static UserItem _Item;

		public static Action<CouponType> OnCouponAdded;

		public static Action<CouponType> OnCouponSpent;

		public static void Init()
		{
			_Item = DataLocal.Current.GetItemByName("Coupons");
			if (_Item == null)
			{
				Preset presetByName = PresetsManager.GetPresetByName("Coupons");
				PresetResult presetResult = presetByName.RunPreset();
				_Item = presetResult.Item;
				DataLocal.Current.Save(false);
			}
		}

		public static void Free()
		{
			_Item = null;
		}

		private static ItemGroupAttributes GetCoupon(CouponType p_type)
		{
			return _Item.GetAttributeByGroupName(p_type.ToString());
		}

		private static bool CouponExist(CouponType p_type)
		{
			ItemGroupAttributes coupon = GetCoupon(p_type);
			if (coupon != null)
			{
				int p_value = -1;
				coupon.TryGetIntValue("Quantity", ref p_value);
				return p_value > 0;
			}
			return false;
		}

		public static bool HaveSuitableCoupon(CouponType p_type)
		{
			return CouponExist(p_type);
		}

		public static string GetCouponVisualName(CouponType p_type)
		{
			return BalanceManager.Current.GetBalance("Coupons", p_type.ToString(), "VisualName");
		}

		public static string GetCouponDescription(CouponType p_type)
		{
			return BalanceManager.Current.GetBalance("Coupons", p_type.ToString(), "Info");
		}

		public static string GetCouponItemImage(CouponType p_type)
		{
			return BalanceManager.Current.GetBalance("Coupons", p_type.ToString(), "DefaultItemImage");
		}

		public static string GetCouponButtonText(CouponType p_type)
		{
			return BalanceManager.Current.GetBalance("Coupons", p_type.ToString(), "ButtonText");
		}

		public static string GetCouponButtonIcon(CouponType p_type)
		{
			return BalanceManager.Current.GetBalance("Coupons", p_type.ToString(), "ButtonIcon");
		}

		public static string GetCouponButtonIconColorName(CouponType p_type)
		{
			return BalanceManager.Current.GetBalance("Coupons", p_type.ToString(), "ButtonIconColor");
		}

		public static Color GetCouponButtonIconColor(CouponType p_type)
		{
			return ColorHelper.GetColor(GetCouponButtonIconColorName(p_type));
		}

		public static int GetCouponQuantity(CouponType p_type, int p_def = -1)
		{
			ItemGroupAttributes coupon = GetCoupon(p_type);
			if (coupon == null)
			{
				return p_def;
			}
			int p_value = p_def;
			coupon.TryGetIntValue("Quantity", ref p_value);
			return p_value;
		}

		public static void AddAllCoupons(int p_count = 1)
		{
			int i = 0;
			for (int num = 4; i < num; i++)
			{
				AddCoupon((CouponType)i, p_count);
			}
		}

		public static void AddCoupon(CouponType p_type, int p_count = 1)
		{
			AppendCoupon(p_type, p_count, OnCouponAdded);
		}

		public static void SpendCoupon(CouponType p_type)
		{
			AppendCoupon(p_type, -1, OnCouponSpent);
		}

		private static void AppendCoupon(CouponType p_type, int p_count, Action<CouponType> p_event = null)
		{
			ItemGroupAttributes coupon = GetCoupon(p_type);
			if (coupon != null)
			{
				coupon.TryAppendValue("Quantity", p_count);
				if (p_event != null)
				{
					p_event(p_type);
				}
			}
			else
			{
				DebugUtils.Log("[CouponsManager]: try to append unexisting coupon! " + p_type);
			}
		}

		public static List<CouponGroupAttribute> GetAllCoupons()
		{
			return CouponGroupAttribute.Create(_Item);
		}
	}
}
