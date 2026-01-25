using System.Collections.Generic;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class TerminalItem
	{
		private const string _DetectName = "Terminal_";

		private UserItem _Item;

		public UserItem CurrItem
		{
			get
			{
				return _Item;
			}
		}

		public List<TerminalItemGroupAttribute> Basket
		{
			get
			{
				return TerminalItemGroupAttribute.Create(_Item);
			}
		}

		public string Name
		{
			get
			{
				return _Item.Name;
			}
		}

		private TerminalItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static TerminalItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new TerminalItem(p_item);
			}
			return null;
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item != null && p_item.Name.Contains("Terminal_");
		}
	}
}
