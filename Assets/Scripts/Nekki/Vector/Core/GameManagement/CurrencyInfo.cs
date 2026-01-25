using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Menu;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public static class CurrencyInfo
	{
		public static CurrencyType NameToType(string p_name)
		{
			if (p_name == DataLocal.Money1Name)
			{
				return CurrencyType.Money1;
			}
			if (p_name == DataLocal.Money2Name)
			{
				return CurrencyType.Money2;
			}
			if (p_name == DataLocal.Money3Name)
			{
				return CurrencyType.Money3;
			}
			return CurrencyType.Unknown;
		}

		public static string TypeToName(CurrencyType p_type)
		{
			switch (p_type)
			{
			case CurrencyType.Money1:
				return DataLocal.Money1Name;
			case CurrencyType.Money2:
				return DataLocal.Money2Name;
			case CurrencyType.Money3:
				return DataLocal.Money3Name;
			default:
				return null;
			}
		}

		public static string GetCurrencySprite(CurrencyType p_currency)
		{
			return BalanceManager.Current.GetBalance("Currencies", p_currency.ToString(), "DefaultItemImage");
		}

		public static string GetCurrencySprite(string p_currencyName)
		{
			return GetCurrencySprite(NameToType(p_currencyName));
		}

		public static string GetCurrencyColorName(CurrencyType p_currency)
		{
			return BalanceManager.Current.GetBalance("Currencies", p_currency.ToString(), "Color");
		}

		public static string GetCurrencyColorName(string p_currencyName)
		{
			return GetCurrencyColorName(NameToType(p_currencyName));
		}

		public static Color GetCurrencyColor(CurrencyType p_currency)
		{
			return ColorHelper.GetColor(GetCurrencyColorName(p_currency));
		}

		public static Color GetCurrencyColor(string p_currencyName)
		{
			return GetCurrencyColor(NameToType(p_currencyName));
		}

		public static string GetCurrencyRarity(CurrencyType p_currency)
		{
			return BalanceManager.Current.GetBalance("Currencies", p_currency.ToString(), "Rarity");
		}

		public static string GetCurrencyRarity(string p_currencyName)
		{
			return GetCurrencyRarity(NameToType(p_currencyName));
		}
	}
}
