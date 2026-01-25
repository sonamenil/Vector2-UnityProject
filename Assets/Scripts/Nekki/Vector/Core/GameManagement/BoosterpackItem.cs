using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Shop;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public class BoosterpackItem
	{
		public UserItem Item { get; private set; }

		public bool IsGiven { get; private set; }

		public int Count { get; private set; }

		public bool IsCard
		{
			get
			{
				return CardsGroupAttribute.IsThis(Item.Groups[0]);
			}
		}

		public bool IsCoupon
		{
			get
			{
				return CouponGroupAttribute.IsThis(Item);
			}
		}

		public bool IsCurrency
		{
			get
			{
				return CurrencyItem.IsThis(Item);
			}
		}

		public CardsGroupAttribute ItemAsCard
		{
			get
			{
				return CardsGroupAttribute.Create(Item.Groups[0]);
			}
		}

		public CouponGroupAttribute ItemAsCoupon
		{
			get
			{
				return CouponGroupAttribute.Create(Item.Groups[0]);
			}
		}

		public CurrencyItem ItemAsCurrency
		{
			get
			{
				return CurrencyItem.Create(Item);
			}
		}

		public BoosterpackItem(UserItem p_item)
		{
			Item = p_item;
			Count = 1;
			if (IsCard)
			{
				Count = ItemAsCard.CurrentCardProgress;
			}
			else if (IsCoupon)
			{
				Count = ItemAsCoupon.Quantity;
			}
			else if (IsCurrency)
			{
				Count = ItemAsCurrency.Quantity;
			}
			IsGiven = false;
		}

		public BoosterpackItem(XmlNode p_node)
		{
			Item = UserItem.CreateByXmlNode(p_node["Item"]);
			Count = XmlUtils.ParseInt(p_node.Attributes["Count"], 1);
			IsGiven = XmlUtils.ParseBool(p_node.Attributes["IsGiven"]);
			if (IsGiven)
			{
				BoosterpackItemsManager.OnItemGive(false);
			}
		}

		public void SaveToXml(XmlElement p_parentNode)
		{
			XmlElement xmlElement = p_parentNode.OwnerDocument.CreateElement("BoosterpackItem");
			Item.SaveToXmlNode(xmlElement);
			xmlElement.SetAttribute("Count", Count.ToString());
			xmlElement.SetAttribute("IsGiven", (!IsGiven) ? "0" : "1");
			p_parentNode.AppendChild(xmlElement);
		}

		public void GiveReward()
		{
			if (IsGiven)
			{
				return;
			}
			IsGiven = true;
			if (IsCurrency)
			{
				CurrencyItem itemAsCurrency = ItemAsCurrency;
				DataLocal.Current.AppendMoneyQuantity(itemAsCurrency.Quantity, itemAsCurrency.CurrenyName);
			}
			else if (IsCard)
			{
				CardsGroupAttribute itemAsCard = ItemAsCard;
				DataLocalHelper.BuyCard(itemAsCard, Count);
				if (itemAsCard.IsNeedForMission && !itemAsCard.IsGeneratedInShop && !itemAsCard.IsEquipped)
				{
					CounterController.Current.CreateCounterOrSetValue(itemAsCard.CardName, 1, "UpgradesGenerator");
					CounterController.Current.CreateCounterOrSetValue(itemAsCard.CardEffectId, Mathf.Max(CounterController.Current.GetUserCounter(itemAsCard.CardEffectId, "GeneratedEffects"), itemAsCard.CardRarity), "GeneratedEffects");
					SupplyItem orCreateBasketItemFromBoosterpacks = EndFloorManager.GetOrCreateBasketItemFromBoosterpacks();
					orCreateBasketItemFromBoosterpacks.CurrItem.AddGroupAttributes(itemAsCard.Attributes.CreateCopy());
					ShopPanel module = UIModule.GetModule<ShopPanel>();
					if (module != null)
					{
						module.IsDirty = true;
					}
				}
			}
			else if (IsCoupon)
			{
				CouponGroupAttribute itemAsCoupon = ItemAsCoupon;
				DataLocalHelper.BuyCoupon(itemAsCoupon, Count);
			}
			DataLocal.Current.Save(true);
			BoosterpackItemsManager.OnItemGive(true);
		}
	}
}
