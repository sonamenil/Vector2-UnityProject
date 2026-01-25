using System.Collections.Generic;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class SlotItem
	{
		public enum Slot
		{
			Stunts = 0,
			Notes = 1,
			StoryItems = 2,
			Legs = 3,
			Torso = 4,
			Head = 5,
			Hands = 6,
			Belt = 7,
			NotSlot = 8
		}

		private static List<string> _DetectNames = new List<string> { "Stunts", "Notes", "StoryItems", "Legs", "Torso", "Head", "Hands", "Belt" };

		private UserItem _Item;

		public UserItem CurrItem
		{
			get
			{
				return _Item;
			}
		}

		public List<CardsGroupAttribute> Cards
		{
			get
			{
				List<CardsGroupAttribute> list = new List<CardsGroupAttribute>();
				CardsGroupAttribute cardsGroupAttribute = null;
				for (int i = 0; i < _Item.Groups.Count; i++)
				{
					cardsGroupAttribute = CardsGroupAttribute.Create(_Item.Groups[i]);
					if (cardsGroupAttribute != null)
					{
						list.Add(cardsGroupAttribute);
					}
				}
				return list;
			}
		}

		public bool HasNewCards
		{
			get
			{
				List<CardsGroupAttribute> cards = Cards;
				foreach (CardsGroupAttribute item in cards)
				{
					if (item.IsNew)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool HasLevelUpCards
		{
			get
			{
				List<CardsGroupAttribute> cards = Cards;
				foreach (CardsGroupAttribute item in cards)
				{
					if (item.IsLevelUp)
					{
						return true;
					}
				}
				return false;
			}
		}

		public List<CardsGroupAttribute> LevelUpCards
		{
			get
			{
				List<CardsGroupAttribute> list = new List<CardsGroupAttribute>();
				List<CardsGroupAttribute> cards = Cards;
				foreach (CardsGroupAttribute item in cards)
				{
					if (item.IsLevelUp)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public string SlotName
		{
			get
			{
				return _Item.Name;
			}
		}

		public Slot SlotType
		{
			get
			{
				return SlotName.GetSlotByName();
			}
		}

		public string SlotVisualName
		{
			get
			{
				return _Item.VisualName;
			}
		}

		public string SlotArchiveTitle
		{
			get
			{
				return BalanceManager.Current.GetBalance("Slots", SlotName, "ArchiveTitle");
			}
		}

		public string SlotImage
		{
			get
			{
				return SlotType.GetImage();
			}
		}

		public string SlotIcon
		{
			get
			{
				return SlotType.GetIcon();
			}
		}

		private SlotItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static SlotItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new SlotItem(p_item);
			}
			return null;
		}

		public static bool IsThis(Item p_item)
		{
			if (p_item == null)
			{
				return false;
			}
			return _DetectNames.Contains(p_item.Name);
		}

		public Dictionary<string, List<CardsGroupAttribute>> CategorizedCards()
		{
			Dictionary<string, List<CardsGroupAttribute>> dictionary = new Dictionary<string, List<CardsGroupAttribute>>();
			List<CardsGroupAttribute> cards = Cards;
			foreach (CardsGroupAttribute item in cards)
			{
				string cardCategory = item.CardCategory;
				if (dictionary.ContainsKey(cardCategory))
				{
					dictionary[cardCategory].Add(item);
					continue;
				}
				dictionary.Add(cardCategory, new List<CardsGroupAttribute> { item });
			}
			return dictionary;
		}

		public CardsGroupAttribute CardByName(string p_name)
		{
			for (int i = 0; i < _Item.Groups.Count; i++)
			{
				CardsGroupAttribute cardsGroupAttribute = CardsGroupAttribute.Create(_Item.Groups[i]);
				if (cardsGroupAttribute != null && cardsGroupAttribute.CardName == p_name)
				{
					return cardsGroupAttribute;
				}
			}
			return null;
		}
	}
}
