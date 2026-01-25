using System.Collections.Generic;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class BuffGroupAttribute
	{
		private const string _BuffItemName = "StarterPackBuff_";

		private const string _StarterPackGroupName = "StarterPackBuff";

		private const string _StarterPackAttributeName = "StarterPackName";

		private Item _Item;

		private ItemGroupAttributes _Attributes;

		protected string[] _Parameters = new string[4];

		public Item CurItem
		{
			get
			{
				return _Item;
			}
		}

		public ItemGroupAttributes Attributes
		{
			get
			{
				return _Attributes;
			}
		}

		public string BuffName
		{
			get
			{
				return _Parameters[0];
			}
		}

		public string IsAvailable
		{
			get
			{
				return CardsManager.Current.GetCardInfo(_Parameters[0], _Parameters[1], "Available");
			}
		}

		public string BuffVisualName
		{
			get
			{
				_Parameters[2] = "Stats";
				_Parameters[3] = "VisualName";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string BuffImage
		{
			get
			{
				_Parameters[2] = "Stats";
				_Parameters[3] = "Image";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public int BuffRarity
		{
			get
			{
				_Parameters[2] = "Stats";
				_Parameters[3] = "Rarity";
				return int.Parse(CardsManager.Current.GetCardInfo(_Parameters));
			}
		}

		public string BuffEffectId
		{
			get
			{
				_Parameters[2] = "Stats";
				_Parameters[3] = "EffectID";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string BuffDescription
		{
			get
			{
				_Parameters[2] = "Stats";
				_Parameters[3] = "Description";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string BuffText
		{
			get
			{
				_Parameters[2] = "Stats";
				_Parameters[3] = "Text";
				return CardsManager.Current.GetCardInfo(_Parameters);
			}
		}

		public string CurrentStarterPackName
		{
			get
			{
				return _Item.GetStrValueAttribute("StarterPackName", "StarterPackBuff", string.Empty);
			}
		}

		private BuffGroupAttribute(ItemGroupAttributes p_attr)
		{
			_Attributes = p_attr;
			_Item = p_attr.ParentItem;
			_Parameters[0] = _Attributes.GroupName;
			_Parameters[1] = "StarterPackBuff";
		}

		public static List<BuffGroupAttribute> GetAllBuffs()
		{
			List<BuffGroupAttribute> list = new List<BuffGroupAttribute>();
			foreach (UserItem item in DataLocal.Current.Stash)
			{
				if (IsThis(item))
				{
					list.AddRange(Create(item));
				}
			}
			return (list.Count <= 0) ? null : list;
		}

		public static List<BuffGroupAttribute> GetBuffsByStarterPack(string p_starterPackName)
		{
			List<BuffGroupAttribute> list = new List<BuffGroupAttribute>();
			foreach (UserItem item in DataLocal.Current.Stash)
			{
				if (IsThis(item) && item.GetStrValueAttribute("StarterPackName", "StarterPackBuff", string.Empty) == p_starterPackName)
				{
					list.AddRange(Create(item));
				}
			}
			return (list.Count <= 0) ? null : list;
		}

		public static BuffGroupAttribute Create(ItemGroupAttributes p_attr)
		{
			if (IsThis(p_attr))
			{
				return new BuffGroupAttribute(p_attr);
			}
			return null;
		}

		public static List<BuffGroupAttribute> Create(Item p_item)
		{
			if (IsThis(p_item))
			{
				List<BuffGroupAttribute> list = new List<BuffGroupAttribute>();
				{
					foreach (ItemGroupAttributes iterableAttribute in p_item.GetIterableAttributes())
					{
						BuffGroupAttribute buffGroupAttribute = Create(iterableAttribute);
						if (buffGroupAttribute != null)
						{
							list.Add(buffGroupAttribute);
						}
					}
					return list;
				}
			}
			return null;
		}

		public static bool IsThis(Item p_item)
		{
			return p_item.Name.Contains("StarterPackBuff_");
		}

		public static bool IsThis(ItemGroupAttributes p_attr)
		{
			return p_attr.ParentItem.Name.Contains("StarterPackBuff_");
		}
	}
}
