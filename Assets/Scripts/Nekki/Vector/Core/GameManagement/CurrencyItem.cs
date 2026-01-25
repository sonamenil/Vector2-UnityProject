using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public class CurrencyItem
	{
		private const string _DetectGroupName = "Currency";

		private Item _Item;

		public Item CurItem
		{
			get
			{
				return _Item;
			}
		}

		public string CurrenyName
		{
			get
			{
				return _Item.Name;
			}
		}

		public CurrencyType CurrencyType
		{
			get
			{
				return CurrencyInfo.NameToType(CurrenyName);
			}
		}

		public int Quantity
		{
			get
			{
				return _Item.GetIntValueAttribute("Quantity", "Countable", 0);
			}
		}

		public string CurrencyIcon
		{
			get
			{
				return CurrencyInfo.GetCurrencySprite(CurrencyType);
			}
		}

		public Color CurrencyColor
		{
			get
			{
				return CurrencyInfo.GetCurrencyColor(CurrencyType);
			}
		}

		public string CurrencyRarity
		{
			get
			{
				return CurrencyInfo.GetCurrencyRarity(CurrencyType);
			}
		}

		private CurrencyItem(Item p_item)
		{
			_Item = p_item;
		}

		public static CurrencyItem Create(Item p_item)
		{
			if (IsThis(p_item))
			{
				return new CurrencyItem(p_item);
			}
			return null;
		}

		public static bool IsThis(Item p_item)
		{
			return p_item.ContainsGroup("Currency");
		}
	}
}
