using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public class KeyItem
	{
		private const string _DetectGroup = "Key";

		private UserItem _Item;

		public UserItem CurrItem
		{
			get
			{
				return _Item;
			}
		}

		public string Level
		{
			get
			{
				return _Item.GetStrValueAttribute("Level", "Key", string.Empty);
			}
		}

		public int Quantity
		{
			get
			{
				return _Item.ContainsGroup("Countable") ? _Item.GetIntValueAttribute("Quantity", "Countable", 0) : 0;
			}
		}

		public string Image
		{
			get
			{
				return "run.key_bg_" + Level.ToLower();
			}
		}

		public Color LabelColor
		{
			get
			{
				switch (Level)
				{
				case "Blue":
					return new Color32(65, 197, 226, byte.MaxValue);
				case "Yellow":
					return new Color32(253, 241, 0, byte.MaxValue);
				case "Red":
					return new Color32(240, 41, 15, byte.MaxValue);
				default:
					return Color.white;
				}
			}
		}

		private KeyItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static KeyItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new KeyItem(p_item);
			}
			return null;
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item != null && p_item.ContainsGroup("Key");
		}
	}
}
