using System.Xml;
using Nekki.Vector.Core.GameManagement;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TQE_OnBuyItem : TriggerQuestEvent
	{
		protected const string GadgetItemType = "Gadget";

		protected const string CardsItemType = "Card";

		protected string _ItemType;

		public static TQE_OnBuyItem OnBuyGadgetEvent
		{
			get
			{
				return new TQE_OnBuyItem("Gadget");
			}
		}

		public TQE_OnBuyItem(XmlNode p_node)
		{
			_ItemType = p_node.Attributes["Type"].Value;
			_Type = EventType.TQE_ON_BUY_ITEM;
		}

		public TQE_OnBuyItem(string p_itemType)
		{
			_ItemType = p_itemType;
			_Type = EventType.TQE_ON_BUY_ITEM;
		}

		public static TQE_OnBuyItem OnBuyCardEvent(CardsGroupAttribute p_card)
		{
			return new TQE_OnBuyCard(p_card.CardRarity.ToString());
		}

		public override bool IsEqual(TriggerEvent p_value)
		{
			return base.IsEqual(p_value) && (p_value as TQE_OnBuyItem)._ItemType == _ItemType;
		}
	}
}
