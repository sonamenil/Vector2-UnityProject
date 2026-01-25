using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class GadgetChargerItem
	{
		private UserItem _Item;

		public UserItem CurrItem
		{
			get
			{
				return _Item;
			}
		}

		public int ChargesCount
		{
			get
			{
				return _Item.GetIntValueAttribute("Value", "Charges");
			}
			set
			{
				_Item.SetValue(value, "Value", "Charges");
			}
		}

		private GadgetChargerItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static GadgetChargerItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new GadgetChargerItem(p_item);
			}
			return null;
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item != null && p_item.IsType(Item.ItemType.GadgetCharger);
		}
	}
}
