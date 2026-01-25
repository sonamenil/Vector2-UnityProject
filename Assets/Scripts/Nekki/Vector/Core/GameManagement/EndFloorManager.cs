using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public static class EndFloorManager
	{
		private const string _EndFloorChargesFileName = "end_floor_charges.yaml";

		private const string _EndFloorCardsFileName = "end_floor_cards.yaml";

		private const string _RerollPresetName = "CreateUpgrade";

		private static Dictionary<int, Dictionary<string, int>> _ChargesPerLevel = new Dictionary<int, Dictionary<string, int>>();

		private static int[] _PointsFromMissions = new int[3];

		private static List<GadgetItem> _ItemsBeforeCharge = new List<GadgetItem>();

		private static List<Preset> _PresetsCharges = new List<Preset>();

		private static List<Preset> _PresetsCards = new List<Preset>();

		private static Preset _RerollPreset;

		private static List<SupplyItem> _BasketItems = new List<SupplyItem>();

		private static int _StartMoney1 = 0;

		private static int _StartMoney2 = 0;

		private static bool _ChargesRefreshAllowed = true;

		public static Dictionary<int, Dictionary<string, int>> ChargesPerLevel
		{
			get
			{
				return _ChargesPerLevel;
			}
		}

		public static int[] PointsFromMissions
		{
			get
			{
				return _PointsFromMissions;
			}
		}

		public static List<GadgetItem> ItemsBeforeCharge
		{
			get
			{
				return _ItemsBeforeCharge;
			}
		}

		public static List<SupplyItem> BasketItems
		{
			get
			{
				return _BasketItems;
			}
		}

		public static int StartMoney1
		{
			get
			{
				return _StartMoney1;
			}
		}

		public static int StartMoney2
		{
			get
			{
				return _StartMoney2;
			}
		}

		public static bool ChargesRefreshAllowed
		{
			get
			{
				return _ChargesRefreshAllowed;
			}
			set
			{
				_ChargesRefreshAllowed = value;
			}
		}

		public static void Parse()
		{
			ParseChargesPresets();
			ParseCardsPresets();
			ParseRerollPreset();
		}

		private static void ParseChargesPresets()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "end_floor_charges.yaml");
			_PresetsCharges.Clear();
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				_PresetsCharges.Add(Preset.Create(item));
			}
		}

		private static void ParseCardsPresets()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "end_floor_cards.yaml");
			_PresetsCards.Clear();
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				_PresetsCards.Add(Preset.Create(item));
			}
		}

		private static void ParseRerollPreset()
		{
			_RerollPreset = PresetsManager.GetPresetByName("CreateUpgrade");
		}

		public static void PrepareData()
		{
			_ChargesRefreshAllowed = false;
			_ChargesPerLevel.Clear();
			_ItemsBeforeCharge.Clear();
			_BasketItems.Clear();
			_PointsFromMissions = new int[3];
			foreach (GadgetItem userGadget in DataLocalHelper.GetUserGadgets())
			{
				_ItemsBeforeCharge.Add(userGadget.Copy());
			}
			ProcessMissions();
			CreateBasketItemsByRank();
			DataLocal.Current.Save(true);
		}

		private static void ProcessMissions()
		{
			if ((int)CounterController.Current.CounterMissionsBlock == 0 && (int)CounterController.Current.CounterPlayCommand == 0)
			{
				if ((int)CounterController.Current.StartFloor != (int)CounterController.Current.CounterFloor)
				{
					MissionsManager.CheckMissions();
					CreatePointsFromMissions();
				}
				RankManager.RefreshRank();
				MissionsManager.GenerateMissions();
			}
		}

		private static void CreatePointsFromMissions()
		{
			if (!MissionsManager.IsMissionsEnabled)
			{
				return;
			}
			foreach (MissionItem prevMissionItem in MissionsManager.PrevMissionItems)
			{
				if (prevMissionItem.IsCompleted)
				{
					_PointsFromMissions[prevMissionItem.Difficulty - 1] = prevMissionItem.RewardAmount;
					DataLocal current2 = DataLocal.Current;
					current2.Money1 = (int)current2.Money1 + prevMissionItem.RewardAmount;
				}
			}
		}

		private static void CreateBasketItemsByRank()
		{
			List<int> ranksGainedOnFloor = RankManager.GetRanksGainedOnFloor();
			foreach (int item in ranksGainedOnFloor)
			{
				CounterController.Current.CreateCounterOrSetValue("Rank", item);
				_ChargesPerLevel.Add(item, new Dictionary<string, int>());
				StringBuffer.AddString("ProtocolName", StarterPacksManager.SelectedStarterPack.Name);
				CreateChargesFromRank(item);
				CreateCardsFromRank();
			}
		}

		private static void CreateChargesFromRank(int p_rank)
		{
			foreach (Preset presetsCharge in _PresetsCharges)
			{
				if (!presetsCharge.CheckCounterConditions())
				{
					continue;
				}
				int i = 0;
				for (int valueInt = presetsCharge.ItemsCount.ValueInt; i < valueInt; i++)
				{
					PresetResult presetResult = presetsCharge.RunPreset();
					if (presetResult.Result)
					{
						KeyValuePair<string, int> keyValuePair = default(KeyValuePair<string, int>);
						keyValuePair = CalculateCharges(presetResult);
						if (_ChargesPerLevel[p_rank].ContainsKey(keyValuePair.Key))
						{
							Dictionary<string, int> dictionary;
							Dictionary<string, int> dictionary2 = (dictionary = _ChargesPerLevel[p_rank]);
							string key;
							string key2 = (key = keyValuePair.Key);
							int num = dictionary[key];
							dictionary2[key2] = num + keyValuePair.Value;
						}
						else
						{
							_ChargesPerLevel[p_rank].Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
			}
		}

		private static void CreateCardsFromRank()
		{
			foreach (Preset presetsCard in _PresetsCards)
			{
				if (presetsCard.CheckCounterConditions())
				{
					int i = 0;
					for (int valueInt = presetsCard.ItemsCount.ValueInt; i < valueInt; i++)
					{
						presetsCard.RunPreset();
					}
				}
			}
		}

		private static KeyValuePair<string, int> CalculateCharges(PresetResult p_result)
		{
			UserItem itemByName = DataLocal.Current.GetItemByName(p_result.Item.Name);
			int currentCharges = GadgetItem.GetCurrentCharges(itemByName);
			int value = GadgetItem.GetCurrentCharges(p_result.Item) - currentCharges;
			DataLocal.Current.AddToEquipped(p_result.Item);
			return new KeyValuePair<string, int>(GadgetItem.GetSlotName(itemByName), value);
		}

		public static SupplyItem GetBasketItemByRank(int rank)
		{
			int i = 0;
			for (int count = _BasketItems.Count; i < count; i++)
			{
				if (_BasketItems[i].CurrItem.Name == "Rank_" + rank)
				{
					return _BasketItems[i];
				}
			}
			return null;
		}

		public static SupplyItem GetBasketItemFromMission()
		{
			int i = 0;
			for (int count = _BasketItems.Count; i < count; i++)
			{
				if (_BasketItems[i].CurrItem.Name == "MissionReward")
				{
					return _BasketItems[i];
				}
			}
			return null;
		}

		public static SupplyItem GetOrCreateBasketItemFromBoosterpacks()
		{
			int i = 0;
			for (int count = _BasketItems.Count; i < count; i++)
			{
				if (_BasketItems[i].CurrItem.Name == "BoosterpacksGenerated")
				{
					return _BasketItems[i];
				}
			}
			SupplyItem supplyItem = SupplyItem.Create("BoosterpacksGenerated");
			_BasketItems.Add(supplyItem);
			return supplyItem;
		}

		public static List<CardsGroupAttribute> BasketItemsAllCards()
		{
			List<CardsGroupAttribute> list = new List<CardsGroupAttribute>();
			for (int i = 0; i < _BasketItems.Count; i++)
			{
				list.AddRange(_BasketItems[i].Cards);
			}
			return list;
		}

		public static void RemoveFromBasket(SupplyItem p_item)
		{
			_BasketItems.Remove(p_item);
		}

		public static void Reset()
		{
			_PresetsCharges.Clear();
			_PresetsCards.Clear();
		}

		public static CardsGroupAttribute RerollItem(CardsGroupAttribute card)
		{
			StringBuffer.AddString("CardName", card.CardName);
			PresetResult presetResult = _RerollPreset.RunPreset();
			if (presetResult.Result)
			{
				SupplyItem supplyItem = SupplyItem.Create(presetResult.Item);
				if (supplyItem != null && supplyItem.Cards.Count > 0)
				{
					CardsGroupAttribute cardsGroupAttribute = supplyItem.Cards[0];
					cardsGroupAttribute.UnmountFromItem();
					card.CurrItem.ReplaceGroupAttributes(card.Attributes, cardsGroupAttribute.Attributes);
					return cardsGroupAttribute;
				}
				DebugUtils.Dialog("Item not supply: " + presetResult.Item.Name, false);
			}
			return null;
		}

		public static void SetOnFloorStartValues()
		{
			_StartMoney1 = DataLocal.Current.Money1;
			_StartMoney2 = DataLocal.Current.Money2;
		}

		public static bool IsCardInBasket(CardsGroupAttribute p_card)
		{
			int i = 0;
			for (int count = _BasketItems.Count; i < count; i++)
			{
				if (_BasketItems[i].Cards.Exists((CardsGroupAttribute p_cardInBasket) => p_cardInBasket != null && p_cardInBasket.CardName == p_card.CardName))
				{
					return true;
				}
			}
			return false;
		}
	}
}
