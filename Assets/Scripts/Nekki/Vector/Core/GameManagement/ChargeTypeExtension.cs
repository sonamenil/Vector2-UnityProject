using System;

namespace Nekki.Vector.Core.GameManagement
{
	public static class ChargeTypeExtension
	{
		public static string GetName(this GadgetItem.ChargeType p_chargeType)
		{
			return p_chargeType.ToString();
		}

		public static GadgetItem.ChargeType GetChargeTypeByName(this string p_chargeType)
		{
			try
			{
				return (GadgetItem.ChargeType)(int)Enum.Parse(typeof(GadgetItem.ChargeType), p_chargeType, true);
			}
			catch
			{
				return GadgetItem.ChargeType.Unknown;
			}
		}
	}
}
