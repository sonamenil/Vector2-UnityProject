using System.Collections.Generic;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class SupplyItem
	{
		private UserItem _Item;

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

		public UserItem CurrItem
		{
			get
			{
				return _Item;
			}
		}

		private SupplyItem(UserItem p_item)
		{
			_Item = p_item;
		}

		private SupplyItem(string p_itemName)
		{
			_Item = UserItem.CreateUserItem(p_itemName);
			_Item.Type = Item.ItemType.Supply;
		}

		public static SupplyItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new SupplyItem(p_item);
			}
			return null;
		}

		public static SupplyItem Create(string p_itemName)
		{
			return new SupplyItem(p_itemName);
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item.IsType(Item.ItemType.Supply);
		}
	}
}
