using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.Core.User
{
	public static class DataLocalHelper
	{
		public static bool HasNewCards
		{
			get
			{
				List<SlotItem> slots = GetSlots();
				foreach (SlotItem item in slots)
				{
					if (item.HasNewCards)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static bool HasLevelUpCards
		{
			get
			{
				List<SlotItem> slots = GetSlots();
				foreach (SlotItem item in slots)
				{
					if (item.HasLevelUpCards)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static int SlotsWithLevelUpCards
		{
			get
			{
				int num = 0;
				List<SlotItem> slots = GetSlots();
				foreach (SlotItem item in slots)
				{
					if (item.HasLevelUpCards)
					{
						num++;
					}
				}
				return num;
			}
		}

		public static List<CardsGroupAttribute> LevelUpCards
		{
			get
			{
				List<CardsGroupAttribute> list = new List<CardsGroupAttribute>();
				List<SlotItem> slots = GetSlots();
				foreach (SlotItem item in slots)
				{
					List<CardsGroupAttribute> levelUpCards = item.LevelUpCards;
					if (levelUpCards.Count > 0)
					{
						list.AddRange(levelUpCards);
					}
				}
				return list;
			}
		}

		public static SlotItem.Slot GetFirstSlotWithLevelUpCards
		{
			get
			{
				List<SlotItem> slots = GetSlots();
				foreach (SlotItem item in slots)
				{
					if (item.HasLevelUpCards)
					{
						return item.SlotName.GetSlotByName();
					}
				}
				return SlotItem.Slot.NotSlot;
			}
		}

		public static List<GadgetItem> GetUserGadgets()
		{
			List<GadgetItem> list = new List<GadgetItem>();
			foreach (UserItem item in DataLocal.Current.Equipped)
			{
				GadgetItem gadgetItem = GadgetItem.Create(item);
				if (gadgetItem != null)
				{
					list.Add(gadgetItem);
				}
			}
			return list;
		}

		public static GadgetItem GetGadget(string p_name)
		{
			List<GadgetItem> userGadgets = GetUserGadgets();
			int i = 0;
			for (int count = userGadgets.Count; i < count; i++)
			{
				if (userGadgets[i].CurrItem.Name == p_name)
				{
					return userGadgets[i];
				}
			}
			return null;
		}

		public static SlotItem GetSlot(string p_name)
		{
			foreach (UserItem item in DataLocal.Current.Stash)
			{
				if (item.Name == p_name)
				{
					return SlotItem.Create(item);
				}
			}
			return null;
		}

		public static SlotItem GetSlot(SlotItem.Slot p_slot)
		{
			return GetSlot(p_slot.GetName());
		}

		public static List<SlotItem> GetSlots()
		{
			List<SlotItem> list = new List<SlotItem>();
			foreach (UserItem item in DataLocal.Current.Stash)
			{
				SlotItem slotItem = SlotItem.Create(item);
				if (slotItem != null)
				{
					list.Add(slotItem);
				}
			}
			return list;
		}

		public static CardsGroupAttribute GetCard(string p_name)
		{
			foreach (UserItem item in DataLocal.Current.Stash)
			{
				if (SlotItem.IsThis(item))
				{
					SlotItem slotItem = SlotItem.Create(item);
					CardsGroupAttribute cardsGroupAttribute = slotItem.CardByName(p_name);
					if (cardsGroupAttribute != null)
					{
						return cardsGroupAttribute;
					}
				}
			}
			return null;
		}

		public static int GetCardLevel(string p_name)
		{
			CardsGroupAttribute card = GetCard(p_name);
			if (card == null)
			{
				return 0;
			}
			return card.CurrentCardLevel;
		}

		public static void SetCardLevel(string p_name, int p_level)
		{
			CardsGroupAttribute card = GetCard(p_name);
			if (card == null)
			{
				Debug.Log(string.Format("[DataLocalHelper] Try to set progress of card '{0}', but no such card in user.", p_name));
				return;
			}
			card.CurrentCardLevel = p_level;
			card.IsLevelUp = card.CurrentCardLevel < card.GetCardLevelForProgress(card.CurrentCardProgress);
		}

		public static void IncrementCardLevel(string p_name, int p_incValue = 1)
		{
			CardsGroupAttribute card = GetCard(p_name);
			if (card == null)
			{
				Debug.Log(string.Format("[DataLocalHelper] Try to increment progress of card '{0}', but no such card in user.", p_name));
				return;
			}
			card.CurrentCardLevel += p_incValue;
			card.IsLevelUp = card.CurrentCardLevel < card.GetCardLevelForProgress(card.CurrentCardProgress);
		}

		public static int GetCardProgress(string p_name)
		{
			CardsGroupAttribute card = GetCard(p_name);
			if (card == null)
			{
				return 0;
			}
			return card.CurrentCardProgress;
		}

		public static void SetCardProgress(string p_name, int p_progress)
		{
			CardsGroupAttribute card = GetCard(p_name);
			if (card == null)
			{
				Debug.Log(string.Format("[DataLocalHelper] Try to set progress of card '{0}', but no such card in user.", p_name));
				return;
			}
			card.CurrentCardProgress = p_progress;
			card.IsLevelUp = card.CurrentCardLevel < card.GetCardLevelForProgress(card.CurrentCardProgress);
		}

		public static void IncrementCardProgress(string p_name, int p_incValue = 1)
		{
			CardsGroupAttribute card = GetCard(p_name);
			if (card == null)
			{
				Debug.Log(string.Format("[DataLocalHelper] Try to increment progress of card '{0}', but no such card in user.", p_name));
				return;
			}
			card.CurrentCardProgress += p_incValue;
			card.IsLevelUp = card.CurrentCardLevel < card.GetCardLevelForProgress(card.CurrentCardProgress);
		}

		public static bool IsLevelUpCard(string p_name)
		{
			CardsGroupAttribute card = GetCard(p_name);
			if (card == null)
			{
				return false;
			}
			return card.IsLevelUp;
		}

		public static int MakeCardsLevelUp()
		{
			List<CardsGroupAttribute> levelUpCards = LevelUpCards;
			if (levelUpCards.Count > 0)
			{
				foreach (CardsGroupAttribute item in levelUpCards)
				{
					item.UserMakeCardLevelUp();
				}
				DataLocal.Current.Save(false);
			}
			return levelUpCards.Count;
		}

		public static bool CanSpendCurrency(CurrencyType p_type, int p_price)
		{
			string text = CurrencyInfo.TypeToName(p_type);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			return (int)DataLocal.Current.GetMoneyQuantity(text) >= p_price;
		}

		public static bool SpendCurrency(CurrencyType p_type, int p_price)
		{
			string text = CurrencyInfo.TypeToName(p_type);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if ((int)DataLocal.Current.GetMoneyQuantity(text) < p_price)
			{
				return false;
			}
			DataLocal.Current.AppendMoneyQuantity(-p_price, text);
			return true;
		}

		public static void BuyCard(CardsGroupAttribute p_card, CurrencyType p_currencyType, int p_price, int p_count = 1)
		{
			if (p_price > 0)
			{
				SpendCurrency(p_currencyType, p_price);
			}
			BuyCard(p_card, p_count);
		}

		public static void BuyCard(CardsGroupAttribute p_card, int p_count = 1)
		{
			DebugUtils.Log("BuyCard: " + p_card.CardName + "(count: " + p_count + ")");
			if (p_card.CurrItem != null)
			{
				p_card.CurrItem.ExtraActionsOnBuy();
			}
			if (!p_card.UserCardIsExists)
			{
				DataLocal.Current.AddToStash(p_card.CurrItem);
				if (p_count > 1)
				{
					p_card.UserCardProgress = p_count;
				}
			}
			else
			{
				IncrementCardProgress(p_card.CardName, p_count);
			}
			QuestManager.Current.CheckEvent(TQE_OnBuyItem.OnBuyCardEvent(p_card));
		}

		public static void BuyCoupon(CouponGroupAttribute p_coupon, int p_count = 1)
		{
			DebugUtils.Log("BuyCoupon: " + p_coupon.Type.ToString() + "(count: " + p_count + ")");
			if (p_coupon.CurrItem != null)
			{
				p_coupon.CurrItem.ExtraActionsOnBuy();
			}
			CouponsManager.AddCoupon(p_coupon.Type, p_count);
		}

		public static GadgetItem GetEquippedGadgetBySlot(string p_slotName)
		{
			List<GadgetItem> userGadgets = GetUserGadgets();
			foreach (GadgetItem item in userGadgets)
			{
				if (item.SlotName == p_slotName)
				{
					return item;
				}
			}
			return null;
		}

		public static int GetLockedCardsCount(string p_categoryName, string p_SlotName)
		{
			int num = 0;
			int maxAvailableStarterPackCoolness = StarterPacksManager.GetMaxAvailableStarterPackCoolness();
			Mapping cardMapping = CardsManager.Current.GetCardMapping(false);
			List<Nekki.Yaml.Node> nodesInside = cardMapping.nodesInside;
			foreach (Nekki.Yaml.Node item in nodesInside)
			{
				string key = item.key;
				string empty = string.Empty;
				string empty2 = string.Empty;
				Mapping mappingFast = cardMapping.GetMappingFast(key);
				mappingFast = mappingFast.GetMappingFast("Stats");
				empty2 = mappingFast.GetNodeFast("Slot").value.ToString();
				if (p_SlotName != empty2)
				{
					continue;
				}
				mappingFast = cardMapping.GetMappingFast(key);
				mappingFast = mappingFast.GetMappingFast("Stats");
				empty = mappingFast.GetNodeFast("Category").value.ToString();
				if (!(p_categoryName != empty))
				{
					mappingFast = cardMapping.GetMappingFast(key);
					mappingFast = mappingFast.GetMappingFast("Balance");
					string s = mappingFast.GetNodeFast("SetupMin").value.ToString();
					int result = 99;
					if (int.TryParse(s, out result) && result <= maxAvailableStarterPackCoolness && GetCard(key) == null)
					{
						num++;
					}
				}
			}
			return num;
		}
	}
}
