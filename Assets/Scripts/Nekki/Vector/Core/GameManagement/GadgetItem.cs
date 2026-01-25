using System.Collections.Generic;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class GadgetItem
	{
		public enum ChargeType
		{
			Normal = 0,
			Bonus = 1,
			Unknown = 2
		}

		private const string _DetectGroup = "ST_MyGadgets";

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

		public List<CardsGroupAttribute> CardsWithEmpty
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
				for (int j = list.Count; j < MaxCards; j++)
				{
					list.Add(null);
				}
				return list;
			}
		}

		public int MaxCards
		{
			get
			{
				return _Item.GetIntValueAttribute("MaxSockets", "ST_GadgetBase", 0);
			}
		}

		public int CurrentCharges
		{
			get
			{
				return _Item.GetIntValueAttribute("ST_ChargesCurrent", "ST_GadgetBase", 0);
			}
			set
			{
				_Item.SetValue(value, "ST_ChargesCurrent", "ST_GadgetBase");
			}
		}

		public int BonusCharges
		{
			get
			{
				return _Item.GetIntValueAttribute("ST_ChargesBonus", "ST_GadgetBase", 0);
			}
			set
			{
				_Item.SetValue(value, "ST_ChargesBonus", "ST_GadgetBase");
			}
		}

		public int TotalCharges
		{
			get
			{
				return _Item.GetIntValueAttribute("ST_ChargesTotal", "ST_GadgetBase", 0);
			}
		}

		public string SlotName
		{
			get
			{
				return _Item.GetStrValueAttribute("ST_Slot", "ST_MyGadgets");
			}
		}

		public string ItemImage
		{
			get
			{
				return _Item.GetStrValueAttribute("ST_ItemImage", "ST_VisualData");
			}
		}

		public string EffectIcon
		{
			get
			{
				return _Item.GetStrValueAttribute("EffectIcon", "ST_VisualData");
			}
		}

		public string Description
		{
			get
			{
				return _Item.GetStrValueAttribute("Description", "ST_VisualData");
			}
		}

		public int SlotPriority
		{
			get
			{
				switch (SlotName)
				{
				case "Legs":
					return 0;
				case "Belt":
					return 1;
				case "Hands":
					return 3;
				case "Torso":
					return 4;
				case "Head":
					return 5;
				default:
					return 0;
				}
			}
		}

		private GadgetItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static GadgetItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new GadgetItem(p_item);
			}
			return null;
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item != null && p_item.ContainsGroup("ST_MyGadgets");
		}

		public static int GetCurrentCharges(UserItem p_item)
		{
			if (!IsThis(p_item))
			{
				return int.MinValue;
			}
			return p_item.GetIntValueAttribute("ST_ChargesCurrent", "ST_GadgetBase", 0);
		}

		public static string GetSlotName(UserItem p_item)
		{
			if (!IsThis(p_item))
			{
				return string.Empty;
			}
			return p_item.GetStrValueAttribute("ST_Slot", "ST_MyGadgets");
		}

		public void SwitchCard(CardsGroupAttribute p_card1, CardsGroupAttribute p_card2)
		{
			int num = _Item.Groups.IndexOf(p_card1.Attributes);
			int num2 = _Item.Groups.IndexOf(p_card2.Attributes);
			if (num != -1 && num2 != -1)
			{
				ItemGroupAttributes value = _Item.Groups[num];
				_Item.Groups[num] = _Item.Groups[num2];
				_Item.Groups[num2] = value;
			}
		}

		public GadgetItem Copy()
		{
			return new GadgetItem(_Item.Copy());
		}
	}
}
