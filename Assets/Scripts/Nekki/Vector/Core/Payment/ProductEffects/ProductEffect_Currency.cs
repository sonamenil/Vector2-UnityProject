using System.Xml;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Payment.ProductEffects
{
	public class ProductEffect_Currency : ProductEffect
	{
		public const string TypeName = "Currency";

		private string _CurrencyName;

		private Variable _CurrencyValue;

		public string CurrencyName
		{
			get
			{
				return _CurrencyName;
			}
		}

		public int CurrencyValue
		{
			get
			{
				return _CurrencyValue.ValueInt;
			}
		}

		public ProductEffect_Currency(XmlNode p_node)
			: base(p_node)
		{
			_Type = ProductEffectType.Currency;
			_CurrencyName = XmlUtils.ParseString(p_node.Attributes["CurrencyName"], string.Empty);
			_CurrencyValue = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["CurrencyValue"], "0"), string.Empty);
			if (!IsValidCurrency())
			{
				DebugUtils.LogError("[Payment]: Invalid currency in product effect: " + _CurrencyName);
			}
		}

		private bool IsValidCurrency()
		{
			return _CurrencyName == DataLocal.Money1Name || _CurrencyName == DataLocal.Money2Name || _CurrencyName == DataLocal.Money3Name;
		}

		public override void Activate()
		{
			if (!IsValidCurrency())
			{
				DebugUtils.LogError("[Payment]: Invalid currency in product effect!");
			}
			else
			{
				DataLocal.Current.AppendMoneyQuantity(_CurrencyValue.ValueInt, _CurrencyName);
			}
		}
	}
}
