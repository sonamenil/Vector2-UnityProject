namespace Nekki.Vector.Core.GameManagement
{
	public static class CurrencyExtension
	{
		public static string GetName(this CurrencyType p_type)
		{
			return p_type.ToString();
		}

		public static CurrencyType GetCurrencyTypeByName(this string p_type)
		{
			switch (p_type)
			{
			case "Money1":
				return CurrencyType.Money1;
			case "Money2":
				return CurrencyType.Money2;
			case "Money3":
				return CurrencyType.Money3;
			default:
				return CurrencyType.Unknown;
			}
		}
	}
}
