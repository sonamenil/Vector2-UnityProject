using System;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class CardsGroupAttribute
	{
		private const string _CardAttributeName = "CardName";

		private ItemGroupAttributes _Attributes;

		private string[] _Parameters = new string[3];

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

		public string CardName
		{
			get
			{
				return _Parameters[0];
			}
		}

		public CardType CardType
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "CardType";
				return StringUtils.ParseEnum(CardsManager.Current.GetCardInfo(_Parameters), CardType.Unknown);
			}
		}

		public string CardEffectId
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "EffectID";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public int CardRarity
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "Rarity";
				return int.Parse(CardsManager.Current.GetCardInfo(_Parameters));
			}
		}

		public string CardImage
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "Image";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string CardCategory
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "Category";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string CardText
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "Text";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string CardVisualName
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "VisualName";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string CardSlotImage
		{
			get
			{
				string slotName = SlotName;
				return BalanceManager.Current.GetBalance("Slots", slotName, "IconImage");
			}
		}

		public int CardPrice
		{
			get
			{
				_Parameters[1] = "Balance";
				_Parameters[2] = "CardPrice";
				string cardInfo = CardsManager.Current.GetCardInfo(_Parameters);
				int result = 0;
				if (int.TryParse(cardInfo, out result))
				{
					return result;
				}
				return 0;
			}
		}

		public int CardFreeRerolls
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("FreeRerolls", ref p_value);
				return p_value;
			}
		}

		public string CardNoteId
		{
			get
			{
				string[] array = CardName.Split('_');
				if (array.Length == 0)
				{
					return string.Empty;
				}
				return array[1];
			}
		}

		public string SlotName
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "Slot";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public SlotItem.Slot Slot
		{
			get
			{
				return SlotName.GetSlotByName();
			}
		}

		public bool HasSlotIcon
		{
			get
			{
				SlotItem.Slot slot = Slot;
				return slot == SlotItem.Slot.Belt || slot == SlotItem.Slot.Hands || slot == SlotItem.Slot.Head || slot == SlotItem.Slot.Legs || slot == SlotItem.Slot.Torso;
			}
		}

		public int CurrentCardProgress
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("Count", ref p_value);
				return p_value;
			}
			set
			{
				if (_Attributes.HasValue("Count"))
				{
					_Attributes.TrySetValue("Count", value);
				}
				else
				{
					_Attributes.AddAttribute("Count", Variable.CreateVariable(value.ToString(), string.Empty));
				}
			}
		}

		public bool IsShowAsOwned
		{
			get
			{
				int p_value = -1;
				_Attributes.TryGetIntValue("ShowAsOwned", ref p_value);
				return p_value == 1;
			}
		}

		public int CurrentCardLevel
		{
			get
			{
				int p_value = 0;
				if (_Attributes != null)
				{
					_Attributes.TryGetIntValue("Level", ref p_value);
				}
				return p_value;
			}
			set
			{
				if (_Attributes.HasValue("Level"))
				{
					_Attributes.TrySetValue("Level", value);
				}
				else
				{
					_Attributes.AddAttribute("Level", Variable.CreateVariable(value.ToString(), string.Empty));
				}
			}
		}

		public int CardMaxLevel
		{
			get
			{
				string balance = BalanceManager.Current.GetBalance("CardLevels", "MaxLevel");
				int result = 1;
				int.TryParse(balance, out result);
				return result;
			}
		}

		public string TriggerName
		{
			get
			{
				return CardsManager.Current.GetCardInfo(true, CardName, "TriggerName");
			}
		}

		public Mapping Vars
		{
			get
			{
				return CardsManager.Current.GetCardMapping(true, CardName, "Vars");
			}
		}

		public string ExecutePresetName
		{
			get
			{
				_Parameters[1] = "Stats";
				_Parameters[2] = "Preset";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public bool IsFocusedOn
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("IsFocusedOn", ref p_value);
				return p_value == 1;
			}
			set
			{
				if (!_Attributes.TrySetValue("IsFocusedOn", value ? 1 : 0))
				{
					_Attributes.AddAttribute("IsFocusedOn", Variable.CreateVariable((!value) ? "0" : "1", string.Empty));
				}
			}
		}

		public bool IsNew
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("IsNew", ref p_value);
				return p_value > 0;
			}
			set
			{
				_Attributes.TrySetValue("IsNew", value ? 1 : 0);
			}
		}

		public bool IsLevelUp
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("IsLevelUp", ref p_value);
				return p_value > 0;
			}
			set
			{
				if (_Attributes.HasValue("IsLevelUp"))
				{
					_Attributes.TrySetValue("IsLevelUp", value ? 1 : 0);
				}
				else
				{
					_Attributes.AddAttribute("IsLevelUp", Variable.CreateVariable((!value) ? "0" : "1", string.Empty));
				}
			}
		}

		public bool IsNeedForMission
		{
			get
			{
				int num = CounterController.Current.GetUserCounter(CardEffectId, "UsefulCards");
				return num > 0 && CardRarity >= num;
			}
		}

		public bool IsGeneratedInShop
		{
			get
			{
				return (int)CounterController.Current.GetUserCounter(CardName, "UpgradesGenerator") > 0;
			}
		}

		public bool IsEquipped
		{
			get
			{
				return (int)CounterController.Current.GetUserCounter(CardName, "InsertedCards") > 0;
			}
		}

		public bool IsAvailableInShop
		{
			get
			{
				int num = CounterController.Current.GetUserCounter(CardEffectId, "GeneratedEffects");
				return num > 0 && CardRarity <= num;
			}
		}

		public bool UserCardIsLevelUp
		{
			get
			{
				return DataLocalHelper.IsLevelUpCard(CardName);
			}
		}

		public bool UserCardIsExists
		{
			get
			{
				return DataLocalHelper.GetCard(CardName) != null;
			}
		}

		public int UserCardProgress
		{
			get
			{
				return DataLocalHelper.GetCardProgress(CardName);
			}
			set
			{
				DataLocalHelper.SetCardProgress(CardName, value);
			}
		}

		public int UserCardLevel
		{
			get
			{
				return DataLocalHelper.GetCardLevel(CardName);
			}
			set
			{
				DataLocalHelper.SetCardLevel(CardName, value);
			}
		}

		public int UserCardBoostLevel
		{
			get
			{
				return CounterController.Current.GetUserCounter(CardName, "BoostedCards");
			}
			set
			{
				CounterController.Current.CreateCounterOrSetValue(CardName, value, "BoostedCards");
			}
		}

		public int UserBoostPrice
		{
			get
			{
				string balance = BalanceManager.Current.GetBalance("Cards", "BoostPrice", (UserCardBoostLevel + 1).ToString());
				int result = 0;
				if (int.TryParse(balance, out result))
				{
					return result;
				}
				return 0;
			}
		}

		public int UserCardTotalLevel
		{
			get
			{
				int num = UserCardLevel + UserCardBoostLevel;
				return (num <= CardMaxLevel) ? num : CardMaxLevel;
			}
		}

		public int UserCardsSinceLastLevel
		{
			get
			{
				int userCardLevel = UserCardLevel;
				int num = UserCardsToLevelFromZero(userCardLevel);
				if (!UserCardIsLevelUp)
				{
					return UserCardProgress - num;
				}
				int result = num;
				if (userCardLevel < CardMaxLevel)
				{
					result = num + CardDeltaByLevel(userCardLevel + 1);
				}
				return result;
			}
		}

		public int UserCardsToNextLevel
		{
			get
			{
				if (UserCardIsLevelUp)
				{
					return 0;
				}
				return UserCardsToNextLevelFromZero - UserCardProgress;
			}
		}

		public int UserCardsToMaxLevelFromZero
		{
			get
			{
				int num = 0;
				int i = 1;
				for (int cardMaxLevel = CardMaxLevel; i <= cardMaxLevel; i++)
				{
					num += CardDeltaByLevel(i);
				}
				return num;
			}
		}

		public int UserCardsToNextLevelFromZero
		{
			get
			{
				int p_level = Math.Min(UserCardTotalLevel + 1, CardMaxLevel);
				return UserCardsToLevelFromZero(p_level);
			}
		}

		private CardsGroupAttribute(ItemGroupAttributes p_attr)
		{
			_Attributes = p_attr;
			if (p_attr.HasValue("CardName"))
			{
				_Parameters[0] = _Attributes.GetStringValue("CardName");
			}
			else
			{
				_Parameters[0] = _Attributes.GroupName;
			}
		}

		private CardsGroupAttribute(string p_cardName)
		{
			_Parameters[0] = p_cardName;
			_Attributes = new ItemGroupAttributes(CardName);
			_Attributes.AddAttribute("IsNew", Variable.CreateVariable("0", string.Empty));
			if (CardType != CardType.Notes)
			{
				_Attributes.AddAttribute("Count", Variable.CreateVariable("1", string.Empty));
				if (CardType != CardType.StoryItems)
				{
					_Attributes.AddAttribute("Level", Variable.CreateVariable("0", string.Empty));
				}
			}
		}

		public static CardsGroupAttribute Create(string p_cardName)
		{
			CardsGroupAttribute card = DataLocalHelper.GetCard(p_cardName);
			if (card != null)
			{
				return card;
			}
			return new CardsGroupAttribute(p_cardName);
		}

		public static CardsGroupAttribute Create(ItemGroupAttributes p_attr)
		{
			if (IsThis(p_attr))
			{
				return new CardsGroupAttribute(p_attr);
			}
			return null;
		}

		public static bool IsThis(ItemGroupAttributes p_attr)
		{
			if (p_attr == null)
			{
				return false;
			}
			return p_attr.HasValue("CardName") || p_attr.ParentItem.IsType(Item.ItemType.Supply) || (!p_attr.IsNoIterable && SlotItem.IsThis(p_attr.ParentItem));
		}

		public void SpendFreeReroll()
		{
			int cardFreeRerolls = CardFreeRerolls;
			if (cardFreeRerolls > 0)
			{
				_Attributes.TrySetValue("FreeRerolls", (cardFreeRerolls - 1).ToString());
			}
		}

		public int CardDeltaByLevel(int level)
		{
			string balance = BalanceManager.Current.GetBalance("CardLevels", "DeltaByLevels", level.ToString());
			int result = 0;
			int.TryParse(balance, out result);
			return result;
		}

		public void UserIncrementCardProgress(int p_incValue = 1)
		{
			DataLocalHelper.IncrementCardProgress(CardName, p_incValue);
		}

		public void UserMakeCardLevelUp()
		{
			UserCardLevel = GetCardLevelForProgress(CurrentCardProgress);
		}

		public void UserIncrementCardLevel(int p_incValue = 1)
		{
			DataLocalHelper.IncrementCardLevel(CardName, p_incValue);
		}

		public int UserCardsToLevelFromZero(int p_level)
		{
			int num = 0;
			for (int i = 1; i <= p_level; i++)
			{
				num += CardDeltaByLevel(i);
			}
			return num;
		}

		public int GetCardLevelForProgress(int p_progress)
		{
			int num = 0;
			int cardMaxLevel = CardMaxLevel;
			for (int i = 1; i <= cardMaxLevel; i++)
			{
				num += CardDeltaByLevel(i);
				if (num > p_progress)
				{
					return i - 1;
				}
			}
			return cardMaxLevel;
		}

		public int GetCardsSinceLastLevelForProgress(int p_progress)
		{
			int num = 0;
			int cardMaxLevel = CardMaxLevel;
			for (int i = 1; i <= cardMaxLevel; i++)
			{
				int num2 = CardDeltaByLevel(i);
				if (num + num2 > p_progress)
				{
					return p_progress - num;
				}
				num += num2;
			}
			return p_progress - num;
		}

		public int GetCardsToNextLevelForProgress(int p_progress)
		{
			int num = 0;
			int cardMaxLevel = CardMaxLevel;
			for (int i = 1; i <= cardMaxLevel; i++)
			{
				num += CardDeltaByLevel(i);
				if (num > p_progress)
				{
					return num - p_progress;
				}
			}
			return 0;
		}

		public int GetLevelsUp()
		{
			return GetLevelsUpForProgress(UserCardProgress);
		}

		public int GetLevelsUpForProgress(int p_progress)
		{
			int num = 0;
			int cardMaxLevel = CardMaxLevel;
			for (int i = 1; i <= cardMaxLevel; i++)
			{
				int num2 = CardDeltaByLevel(i);
				if (num + num2 > p_progress)
				{
					return i - CurrentCardLevel - 1;
				}
				num += num2;
			}
			return cardMaxLevel - CurrentCardLevel;
		}

		public void MountToItem(UserItem p_item)
		{
			p_item.AddGroupAttributes(_Attributes);
		}

		public void UnmountFromItem()
		{
			_Attributes.ParentItem.RemoveGroupAttributes(_Attributes.GroupName);
		}

		public bool AddToUser()
		{
			if (UserCardIsExists)
			{
				return false;
			}
			SlotItem slot = DataLocalHelper.GetSlot(SlotName);
			IsNew = true;
			MountToItem(slot.CurrItem);
			return true;
		}
	}
}
