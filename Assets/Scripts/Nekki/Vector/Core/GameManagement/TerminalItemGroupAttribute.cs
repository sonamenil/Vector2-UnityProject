using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class TerminalItemGroupAttribute
	{
		private const string NoRequirementsLabel = "^StarterPacks.ResearchRequirements.NoRequirements^";

		private const string _DetectItemName = "Terminal_";

		private Item _Item;

		private ItemGroupAttributes _Attributes;

		private TerminalRewardType _RewardType;

		private CardsGroupAttribute _Card;

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

		public TerminalRewardType RewardType
		{
			get
			{
				return _RewardType;
			}
		}

		public bool IsCardReward
		{
			get
			{
				return _RewardType == TerminalRewardType.Card;
			}
		}

		public bool IsItemReward
		{
			get
			{
				return _RewardType == TerminalRewardType.Item;
			}
		}

		public bool IsExpired
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("IsExpired", ref p_value);
				return p_value > 0;
			}
			private set
			{
				_Attributes.TrySetValue("IsExpired", value ? 1 : 0);
			}
		}

		public CardsGroupAttribute Card
		{
			get
			{
				return _Card;
			}
		}

		public string Name
		{
			get
			{
				return _Attributes.GroupName;
			}
		}

		public string CardName
		{
			get
			{
				string p_value = string.Empty;
				_Attributes.TryGetStrValue("Card", ref p_value);
				return p_value;
			}
			private set
			{
				_Attributes.TrySetValue("Card", value);
			}
		}

		public string ItemImage
		{
			get
			{
				string p_value = string.Empty;
				_Attributes.TryGetStrValue("ItemImage", ref p_value);
				return p_value;
			}
		}

		public string ItemVisualName
		{
			get
			{
				string p_value = string.Empty;
				_Attributes.TryGetStrValue("ItemName", ref p_value);
				return p_value;
			}
		}

		public string ItemDescription
		{
			get
			{
				string p_value = string.Empty;
				_Attributes.TryGetStrValue("ItemDescription", ref p_value);
				return p_value;
			}
		}

		public string ItemPresetName
		{
			get
			{
				string p_value = string.Empty;
				_Attributes.TryGetStrValue("ItemPreset", ref p_value);
				return p_value;
			}
		}

		public Preset ItemPreset
		{
			get
			{
				return PresetsManager.GetPresetByName(ItemPresetName);
			}
		}

		public CurrencyType CurrencyType
		{
			get
			{
				string p_value = "Money2";
				_Attributes.TryGetStrValue("CurrencyType", ref p_value);
				return p_value.GetCurrencyTypeByName();
			}
		}

		public int Price
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("Price", ref p_value);
				return p_value;
			}
		}

		public int Count
		{
			get
			{
				int p_value = 1;
				_Attributes.TryGetIntValue("Count", ref p_value);
				return p_value;
			}
		}

		public bool RequirementIsCompleted
		{
			get
			{
				int p_value = 0;
				_Attributes.TryGetIntValue("Lock", ref p_value);
				return p_value == 0;
			}
		}

		public string RequirementText
		{
			get
			{
				string p_value = string.Empty;
				_Attributes.TryGetStrValue("Text", ref p_value);
				return p_value;
			}
		}

		public bool NoRequirements
		{
			get
			{
				return "^StarterPacks.ResearchRequirements.NoRequirements^" == RequirementText;
			}
		}

		private TerminalItemGroupAttribute(ItemGroupAttributes p_attr)
		{
			_Attributes = p_attr;
			_Item = p_attr.ParentItem;
			Refresh();
		}

		public static TerminalItemGroupAttribute Create(ItemGroupAttributes p_attr)
		{
			if (IsThis(p_attr))
			{
				return new TerminalItemGroupAttribute(p_attr);
			}
			return null;
		}

		public static List<TerminalItemGroupAttribute> Create(Item p_item)
		{
			if (IsThis(p_item))
			{
				List<TerminalItemGroupAttribute> list = new List<TerminalItemGroupAttribute>();
				{
					foreach (ItemGroupAttributes group in p_item.Groups)
					{
						list.Add(Create(group));
					}
					return list;
				}
			}
			return null;
		}

		public static bool IsThis(Item p_item)
		{
			return p_item.Name.Contains("Terminal_");
		}

		public static bool IsThis(ItemGroupAttributes p_attr)
		{
			return p_attr.ParentItem.Name.Contains("Terminal_");
		}

		public void Buy()
		{
			if (IsCardReward)
			{
				DataLocalHelper.BuyCard(_Card, CurrencyType, Price, Count);
				AdvanceBuyedCardsAchievement();
			}
			else
			{
				BuyItem();
			}
			IsExpired = true;
		}

		private void AdvanceBuyedCardsAchievement()
		{
			string p_name = "CardsWithRarity" + _Card.CardRarity;
			CounterController.Current.IncrementUserCounter(p_name, Count, "AchievementsProgressByFloor");
		}

		public void Reroll()
		{
			IsExpired = true;
		}

		public void Refresh()
		{
			SetType();
			if (IsCardReward)
			{
				_Card = PresetUtils.CreateCard(CardName);
			}
			else
			{
				_Card = null;
			}
		}

		private void SetType()
		{
			string p_value = string.Empty;
			_Attributes.TryGetStrValue("Type", ref p_value);
			_RewardType = ((!(p_value == "Card")) ? TerminalRewardType.Item : TerminalRewardType.Card);
		}

		private void BuyItem()
		{
			int price = Price;
			if (price > 0)
			{
				DataLocalHelper.SpendCurrency(CurrencyType, Price);
			}
			RunItemPreset(Count);
		}

		private void RunItemPreset(int p_runCount)
		{
			Preset itemPreset = ItemPreset;
			if (itemPreset != null)
			{
				for (int i = 0; i < p_runCount; i++)
				{
					itemPreset.RunPreset();
				}
			}
		}
	}
}
