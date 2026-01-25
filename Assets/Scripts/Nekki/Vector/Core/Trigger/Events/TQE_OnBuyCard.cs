using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TQE_OnBuyCard : TQE_OnBuyItem
	{
		private Variable _Rarity;

		public TQE_OnBuyCard(XmlNode p_node)
			: base("Card")
		{
			_Rarity = Variable.CreateVariable(p_node.Attributes["Rarity"].Value, null);
		}

		public TQE_OnBuyCard(string p_Rarity)
			: base("Card")
		{
			_Rarity = Variable.CreateVariable(p_Rarity, null);
		}

		public override bool IsEqual(TriggerEvent p_value)
		{
			return base.IsEqual(p_value) && ((TQE_OnBuyCard)p_value)._Rarity.IsEqual(_Rarity);
		}
	}
}
