using System.Collections.Generic;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public class CouponGroupAttribute
	{
		private const string _CouponItemName = "Coupons";

		private ItemGroupAttributes _Attributes;

		protected CouponsManager.CouponType _Type;

		public UserItem CurrItem
		{
			get
			{
				return (UserItem)_Attributes.ParentItem;
			}
		}

		public ItemGroupAttributes Attributes
		{
			get
			{
				return _Attributes;
			}
		}

		public CouponsManager.CouponType Type
		{
			get
			{
				return _Type;
			}
		}

		public string TypeName
		{
			get
			{
				return _Type.ToString();
			}
		}

		public int Quantity
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("Quantity", ref p_value);
				return p_value;
			}
		}

		public string VisualName
		{
			get
			{
				return CouponsManager.GetCouponVisualName(_Type);
			}
		}

		public string Description
		{
			get
			{
				return CouponsManager.GetCouponDescription(_Type);
			}
		}

		public string ItemImage
		{
			get
			{
				return CouponsManager.GetCouponItemImage(_Type);
			}
		}

		public string ButtonText
		{
			get
			{
				return CouponsManager.GetCouponButtonText(_Type);
			}
		}

		public string ButtonIcon
		{
			get
			{
				return CouponsManager.GetCouponButtonIcon(_Type);
			}
		}

		public string ButtonIconColorName
		{
			get
			{
				return CouponsManager.GetCouponButtonIconColorName(_Type);
			}
		}

		public Color ButtonIconColor
		{
			get
			{
				return CouponsManager.GetCouponButtonIconColor(_Type);
			}
		}

		private CouponGroupAttribute(ItemGroupAttributes p_attr)
		{
			_Attributes = p_attr;
			_Type = ParseType(p_attr.GroupName);
		}

		public static CouponGroupAttribute Create(ItemGroupAttributes p_attr)
		{
			if (IsThis(p_attr))
			{
				return new CouponGroupAttribute(p_attr);
			}
			return null;
		}

		public static List<CouponGroupAttribute> Create(Item p_item)
		{
			if (IsThis(p_item))
			{
				List<CouponGroupAttribute> list = new List<CouponGroupAttribute>();
				{
					foreach (ItemGroupAttributes group in p_item.Groups)
					{
						list.Add(Create(group));
					}
					return list;
				}
			}
			return null;
		}

		public static bool IsThis(Item p_item)
		{
			return p_item.Name == "Coupons";
		}

		public static bool IsThis(ItemGroupAttributes p_attr)
		{
			return p_attr.ParentItem.Name == "Coupons";
		}

		private CouponsManager.CouponType ParseType(string p_type)
		{
			switch (p_type)
			{
			case "CardsBoost":
				return CouponsManager.CouponType.CardsBoost;
			case "Saveme":
				return CouponsManager.CouponType.Saveme;
			case "EnergyRecharge":
				return CouponsManager.CouponType.EnergyRecharge;
			case "Reroll":
				return CouponsManager.CouponType.Reroll;
			default:
				return CouponsManager.CouponType.Unknown;
			}
		}
	}
}
