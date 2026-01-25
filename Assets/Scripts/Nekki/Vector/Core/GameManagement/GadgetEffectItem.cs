using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class GadgetEffectItem
	{
		private UserItem _Item;

		public UserItem CurrItem
		{
			get
			{
				return _Item;
			}
		}

		public CardsGroupAttribute Card
		{
			get
			{
				CardsGroupAttribute cardsGroupAttribute = null;
				for (int i = 0; i < _Item.Groups.Count; i++)
				{
					cardsGroupAttribute = CardsGroupAttribute.Create(_Item.Groups[i]);
					if (cardsGroupAttribute != null)
					{
						return cardsGroupAttribute;
					}
				}
				return null;
			}
		}

		private GadgetEffectItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static GadgetEffectItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new GadgetEffectItem(p_item);
			}
			return null;
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item != null && p_item.IsType(Item.ItemType.GadgetEffect);
		}
	}
}
