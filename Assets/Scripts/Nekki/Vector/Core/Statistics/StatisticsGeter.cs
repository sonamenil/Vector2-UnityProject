using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.Scenes.Run;
using SimpleJSON;

namespace Nekki.Vector.Core.Statistics
{
	public static class StatisticsGeter
	{
		public static void AddBundleId(JSONClass p_node, string p_key = "b_id")
		{
			p_node.Add(p_key, ApplicationController.BundleId);
		}

		public static void AddCurrency(JSONClass p_node)
		{
			AddMoney1(p_node);
			AddMoney2(p_node);
			AddMoney3(p_node);
		}

		public static void AddMoney1(JSONClass p_node, string p_key = "money1")
		{
			p_node.Add(p_key, (int)DataLocal.Current.Money1);
		}

		public static void AddMoney2(JSONClass p_node, string p_key = "money2")
		{
			p_node.Add(p_key, (int)DataLocal.Current.Money2);
		}

		public static void AddMoney3(JSONClass p_node, string p_key = "money3")
		{
			p_node.Add(p_key, (int)DataLocal.Current.Money3);
		}

		public static void AddUserRank(JSONClass p_node, string p_key = "rank")
		{
			p_node.Add(p_key, RankManager.Rank);
		}

		public static void AddUserRankGained(JSONClass p_node, string p_key = "ranks")
		{
			JSONArray jSONArray = new JSONArray();
			List<int> ranksGainedOnFloor = RankManager.GetRanksGainedOnFloor();
			foreach (int item in ranksGainedOnFloor)
			{
				jSONArray.Add(new JSONData(item));
			}
			p_node.Add(p_key, jSONArray);
		}

		public static void AddEnergyLevel(JSONClass p_node, string p_key = "energy")
		{
			p_node.Add(p_key, EnergyManager.CurrentLevel);
		}

		public static void AddRunCount(JSONClass p_node, string p_key = "runs")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterRunCounter);
		}

		public static void AddGeneratorSeed(JSONClass p_node, string p_key = "seed")
		{
			p_node.Add(p_key, (int)MainRandom.Current.getSeed());
		}

		public static void AddFloorNumber(JSONClass p_node, string p_key = "floor")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterFloor);
		}

		public static void AddSceneLoadTime(JSONClass p_node, string p_key = "time")
		{
			p_node.Add(p_key, (int)DebugUtils.SceneLoadTime);
		}

		public static void AddGeneratorTime(JSONClass p_node)
		{
			p_node.Add("generation_time", (int)CounterController.Current.CounterFloorGenerationTime);
			p_node.Add("postprocess_time", (int)CounterController.Current.CounterFloorPostprocessTime);
		}

		public static void AddFloorTime(JSONClass p_node, string p_key = "prev_floor_time")
		{
			p_node.Add(p_key, (int)(RunMainController.RunEndTime - RunMainController.RunStartTime));
		}

		public static void AddSceneType(JSONClass p_node, string p_key = "scene")
		{
			p_node.Add(p_key, GetCurrentSceneName());
		}

		public static void AddScreenType(JSONClass p_node, string p_key = "screen")
		{
			p_node.Add(p_key, string.Format("{0}-{1}", GetCurrentSceneName(), GetCurrentScreenName()));
		}

		public static void AddMaxStars(JSONClass p_node, string p_key = "max_stars")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterMaxMissionStars);
		}

		public static void AddCurrentStars(JSONClass p_node, string p_key = "current_stars")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterCurrentMissionStars);
		}

		public static void AddCountersMissions(JSONClass p_node, string p_key = "missions_current")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_CurrentMissions");
			p_node.Add(p_key, aItem);
		}

		public static void AddCountersCompletedMissions(JSONClass p_node, string p_key = "missions_completed")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_CompletedMissions");
			p_node.Add(p_key, aItem);
		}

		public static void AddProtocol(JSONClass p_node, string p_key = "protocol")
		{
			StarterPackItem selectedStarterPack = StarterPacksManager.SelectedStarterPack;
			p_node.Add(p_key, (selectedStarterPack == null) ? "none" : selectedStarterPack.Name);
		}

		public static void AddUserGadgets(JSONClass p_node)
		{
			List<GadgetItem> userGadgets = DataLocalHelper.GetUserGadgets();
			foreach (GadgetItem item in userGadgets)
			{
				p_node.Add(item.SlotName.ToLower(), GadgetToJSONClass(item));
			}
		}

		public static void AddShopGeneratedCards(JSONClass p_node, string p_key = "cards_generated")
		{
			List<CardsGroupAttribute> p_cards = EndFloorManager.BasketItemsAllCards();
			p_node.Add(p_key, CardsToJSONArray(p_cards));
		}

		public static void AddTerminalBoughtItem(TerminalItemGroupAttribute p_boughtItem, JSONClass p_node, string p_key = "bought_item")
		{
			p_node.Add(p_key, TerminalItemToJSONClass(p_boughtItem));
		}

		public static void AddTerminalBasketItems(JSONClass p_node, string p_key = "basket_items")
		{
			p_node.Add(p_key, TerminalBasketToJSONArray(TerminalItemsManager.NotBoughtItems));
		}

		public static void AddTerminalBoughtItems(JSONClass p_node, string p_key = "bought_items")
		{
			p_node.Add(p_key, TerminalBasketToJSONArray(TerminalItemsManager.BoughtItems));
		}

		public static void AddBoosterpackBasketItems(JSONClass p_node, string p_key = "items")
		{
			List<BoosterpackItem> basketItems = BoosterpackItemsManager.BasketItems;
			p_node.Add(p_key, BoosterpackBasketToJSONArray(basketItems));
		}

		public static void AddNewCardSlot(ArgsDict p_data, JSONClass p_node, string p_key = "slot")
		{
			if (p_data != null && p_data.ContainsKey("new_card"))
			{
				CardsGroupAttribute cardsGroupAttribute = p_data["new_card"] as CardsGroupAttribute;
				p_node.Add(p_key, cardsGroupAttribute.SlotName);
			}
		}

		public static void AddNewCardEquipped(ArgsDict p_data, JSONClass p_node, string p_key = "new_card")
		{
			if (p_data != null && p_data.ContainsKey("new_card"))
			{
				JSONClass jSONClass = new JSONClass();
				CardsGroupAttribute p_card = p_data["new_card"] as CardsGroupAttribute;
				AddCardToJSONClass(p_card, jSONClass);
				p_node.Add(p_key, jSONClass);
			}
		}

		public static void AddReplacedCard(ArgsDict p_data, JSONClass p_node, string p_key = "replaced_card_name")
		{
			if (p_data != null && p_data.ContainsKey("replaced_card"))
			{
				JSONClass jSONClass = new JSONClass();
				CardsGroupAttribute p_card = p_data["replaced_card"] as CardsGroupAttribute;
				AddCardToJSONClass(p_card, jSONClass);
				p_node.Add(p_key, jSONClass);
			}
		}

		public static void AddCardAfterReroll(ArgsDict p_data, JSONClass p_node, string p_key = "new_card")
		{
			if (p_data != null && p_data.ContainsKey("new_card"))
			{
				JSONClass jSONClass = new JSONClass();
				CardsGroupAttribute p_card = p_data["new_card"] as CardsGroupAttribute;
				AddCardToJSONClass(p_card, jSONClass, true);
				p_node.Add(p_key, jSONClass);
			}
		}

		public static void AddRerolledCard(ArgsDict p_data, JSONClass p_node, string p_key = "rerolled_card")
		{
			if (p_data != null && p_data.ContainsKey("rerolled_card"))
			{
				JSONClass jSONClass = new JSONClass();
				CardsGroupAttribute p_card = p_data["rerolled_card"] as CardsGroupAttribute;
				AddCardToJSONClass(p_card, jSONClass, true);
				p_node.Add(p_key, jSONClass);
			}
		}

		public static void AddRerollTryCount(JSONClass p_node, string p_key = "try_num")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterRerollTryCount);
		}

		public static void AddUniqueCardsCount(JSONClass p_node, string p_prefix = "cards_")
		{
			List<SlotItem> slots = DataLocalHelper.GetSlots();
			int num = 0;
			foreach (SlotItem item in slots)
			{
				if (item.SlotType == SlotItem.Slot.Notes)
				{
					continue;
				}
				int num3;
				int num2;
				int num4 = (num3 = (num2 = 0));
				foreach (CardsGroupAttribute card in item.Cards)
				{
					switch (card.CardRarity)
					{
					case 1:
						num4++;
						break;
					case 2:
						num3++;
						break;
					case 3:
						num2++;
						break;
					}
				}
				JSONClass jSONClass = new JSONClass();
				jSONClass.Add("blue", num4);
				jSONClass.Add("yellow", num3);
				jSONClass.Add("red", num2);
				num += num4 + num3 + num2;
				string aKey = string.Format("{0}{1}", p_prefix, item.SlotName.ToLower());
				p_node.Add(aKey, jSONClass);
			}
			CounterController.Current.CounterCardsCount = num;
		}

		public static void AddCardsRatio(JSONClass p_node, string p_key = "cards_ratio")
		{
			p_node.Add(p_key, ((float)(int)CounterController.Current.CounterCardsCount / float.Parse(BalanceManager.Current.GetBalance("ProtocolCards", StarterPacksManager.GetBestAvailableStarterPack().Coolness.ToString()))).ToString());
		}

		public static void AddCountersTutorial(JSONClass p_node, string p_key = "tutorial_counters")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Tutorial");
			p_node.Add(p_key, aItem);
		}

		public static void AddCountersQuest(JSONClass p_node, string p_key = "quests_counters")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Quests");
			p_node.Add(p_key, aItem);
		}

		public static void AddLocks(JSONClass p_node, string p_key = "locks")
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass["blue"] = (int)CounterController.Current.CounterBlueLocks;
			jSONClass["yellow"] = (int)CounterController.Current.CounterYellowLocks;
			jSONClass["red"] = (int)CounterController.Current.CounterRedLocks;
			p_node.Add(p_key, jSONClass);
		}

		public static void AddStuntsGenerated(JSONClass p_node, string p_key = "stunts_generated")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Statistics_Stunts_Generated");
			p_node.Add(p_key, aItem);
		}

		public static void AddStuntsUnlocked(JSONClass p_node, string p_key = "stunts_unlocked")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Statistics_Stunts_Unlocked");
			p_node.Add(p_key, aItem);
		}

		public static void AddStuntsCollected(JSONClass p_node, string p_key = "stunts_collected")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Statistics_Stunts_Collected");
			p_node.Add(p_key, aItem);
		}

		public static void AddItemsGenerated(JSONClass p_node, string p_key = "items_generated")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Statistics_Items_Generated");
			p_node.Add(p_key, aItem);
		}

		public static void AddItemsCollected(JSONClass p_node, string p_key = "items_collected")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Statistics_Items_Collected");
			p_node.Add(p_key, aItem);
		}

		public static void AddForksHard(JSONClass p_node, string p_key = "forks_hard")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterHardForks);
		}

		public static void AddTraps(JSONClass p_node, string p_key = "traps")
		{
			JSONClass aItem = CountersNamespaceToJSONClass("ST_Statistics_Traps");
			p_node.Add(p_key, aItem);
		}

		public static void AddDeathType(JSONClass p_node, string p_key = "death")
		{
			if ((int)CounterController.Current.CounterMineDeath > 0)
			{
				p_node.Add(p_key, "mine");
			}
			else if ((int)CounterController.Current.CounterLaserDeath > 0)
			{
				p_node.Add(p_key, "laser");
			}
			else if ((int)CounterController.Current.CounterBlackballDeath > 0)
			{
				p_node.Add(p_key, "blackball");
			}
			else if ((int)CounterController.Current.CounterTeslaDeath > 0)
			{
				p_node.Add(p_key, "tesla");
			}
			else
			{
				p_node.Add(p_key, "unknown");
			}
		}

		public static void AddRunFPS(JSONClass p_node, string p_key = "fps")
		{
			if (!string.IsNullOrEmpty(RunFPSMeter.FPS_MinRoom) && !string.IsNullOrEmpty(RunFPSMeter.FPS_MaxRoom))
			{
				JSONClass jSONClass = new JSONClass();
				JSONClass jSONClass2 = new JSONClass();
				jSONClass2[RunFPSMeter.FPS_MinRoom] = (int)RunFPSMeter.FPS_Min;
				JSONClass jSONClass3 = new JSONClass();
				jSONClass3[RunFPSMeter.FPS_MaxRoom] = (int)RunFPSMeter.FPS_Max;
				jSONClass["min"] = jSONClass2;
				jSONClass["max"] = jSONClass3;
				jSONClass["average"] = (int)RunFPSMeter.FPS_Average;
				p_node.Add(p_key, jSONClass);
			}
		}

		public static void AddPaymentData(ArgsDict p_data, JSONClass p_node)
		{
			p_node.Add("verification", PaymentVerificationManager.IsActive ? 1 : 0);
			if (p_data != null && p_data.ContainsKey("type") && p_data.ContainsKey("product") && p_data.ContainsKey("error"))
			{
				p_node.Add("type", (string)p_data["type"]);
				p_node.Add("product", (string)p_data["product"]);
				p_node.Add("error", (string)p_data["error"]);
				if (!p_data.ContainsKey("p_info"))
				{
					return;
				}
				PaymentInfo paymentInfo = (PaymentInfo)p_data["p_info"];
				if (paymentInfo != null)
				{
					string empty = string.Empty;
					if (DeviceInformation.IsiOS && paymentInfo.Decode64Receipt != null)
					{
						empty = paymentInfo.Decode64Receipt;
						int startIndex = empty.IndexOf("purchase-info");
						int num = empty.IndexOf(" = \"", startIndex) + 4;
						int num2 = empty.IndexOf(";", num) - 1;
						int length = num2 - num;
						empty = empty.Substring(num, length);
						empty = StringUtils.FromBase64(empty);
						empty = empty.Replace("\t", string.Empty);
						empty = empty.Replace("\n", string.Empty);
						empty = empty.Replace(" = ", ": ");
						empty = empty.Replace(";", ", ");
						empty = empty.Replace(", }", "}");
						JSONNode aItem = JSONNode.Parse(empty);
						p_node.Add("data", aItem);
					}
					else if (DeviceInformation.IsAndroid && paymentInfo.Receipt != null)
					{
						empty = paymentInfo.Receipt.Replace("\\\"", "\"");
						JSONNode aItem2 = JSONNode.Parse(empty);
						p_node.Add("data", aItem2);
					}
				}
			}
			else
			{
				p_node.Add("type", "error");
				p_node.Add("product", "error");
				p_node.Add("error", "error");
			}
		}

		public static void AddFreeData(ArgsDict p_data, JSONClass p_node)
		{
			if (p_data != null && p_data.ContainsKey("type"))
			{
				p_node.Add("type", (string)p_data["type"]);
				if (p_data.ContainsKey("amount"))
				{
					p_node.Add("amount", (int)p_data["amount"]);
				}
			}
			else
			{
				p_node.Add("type", "error");
			}
		}

		public static void AddUseCoupon(ArgsDict p_data, JSONClass p_node, string p_key = "coupon")
		{
			if (p_data != null && p_data.ContainsKey("coupon"))
			{
				bool flag = (bool)p_data["coupon"];
				p_node.Add(p_key, flag ? 1 : 0);
			}
		}

		public static void AddSaveMeAttempts(JSONClass p_node, string p_key = "attempt_number")
		{
			p_node.Add(p_key, ControllerSaveMe.Attempt);
		}

		public static void AddCardBoostCard(ArgsDict p_data, JSONClass p_node, string p_key = "card")
		{
			if (p_data != null && p_data.ContainsKey("card"))
			{
				CardsGroupAttribute cardsGroupAttribute = (CardsGroupAttribute)p_data["card"];
				p_node.Add(p_key, cardsGroupAttribute.CardName);
			}
		}

		public static void AddSignalMessage(ArgsDict p_data, JSONClass p_node, string p_key = "signal_message")
		{
			if (p_data != null && p_data.ContainsKey("signal_message"))
			{
				p_node.Add(p_key, (string)p_data["signal_message"]);
			}
		}

		public static void AddDoYouLikeGameAnswer1(JSONClass p_node, string p_key = "answer1")
		{
			p_node.Add(p_key, GetDoYouLikeGameAnswer1());
		}

		public static void AddDoYouLikeGameAnswer2(JSONClass p_node, string p_key = "answer2")
		{
			p_node.Add(p_key, GetDoYouLikeGameAnswer2());
		}

		public static void AddMissionsDifficulty(JSONClass p_node, string p_key = "mission_difficulty")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterMissionDifficulty);
		}

		public static void AddMissionsUnownedCards(JSONClass p_node, string p_key = "mission_unowned_cards")
		{
			p_node.Add(p_key, (int)CounterController.Current.CounterMissionUnownedCards);
		}

		private static JSONClass CountersNamespaceToJSONClass(string p_namespace)
		{
			JSONClass jSONClass = new JSONClass();
			Dictionary<string, ObscuredInt> counterDictionary = CounterController.Current.GetCounterDictionary(p_namespace, false);
			if (counterDictionary != null)
			{
				foreach (KeyValuePair<string, ObscuredInt> item in counterDictionary)
				{
					if ((int)item.Value != 0)
					{
						jSONClass[item.Key] = (int)item.Value;
					}
				}
			}
			return jSONClass;
		}

		private static JSONClass GadgetToJSONClass(GadgetItem p_gadget)
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass["charges_remaining"] = p_gadget.CurrentCharges;
			AddCardsToJSONClass(p_gadget.Cards, jSONClass);
			return jSONClass;
		}

		private static JSONArray CardsToJSONArray(List<CardsGroupAttribute> p_cards)
		{
			JSONArray jSONArray = new JSONArray();
			foreach (CardsGroupAttribute p_card in p_cards)
			{
				jSONArray.Add(CardToString(p_card));
			}
			return jSONArray;
		}

		private static string CardToString(CardsGroupAttribute p_card)
		{
			return (p_card == null) ? "none" : p_card.CardName;
		}

		private static void AddCardsToJSONClass(List<CardsGroupAttribute> p_cards, JSONClass p_node, bool p_needSlot = false)
		{
			foreach (CardsGroupAttribute p_card in p_cards)
			{
				AddCardToJSONClass(p_card, p_node, p_needSlot);
			}
		}

		private static void AddCardToJSONClass(CardsGroupAttribute p_card, JSONClass p_node, bool p_needSlot = false)
		{
			if (p_card != null)
			{
				p_node.Add(p_card.CardEffectId, CardToJSONClass(p_card, p_needSlot));
			}
		}

		private static JSONClass CardToJSONClass(CardsGroupAttribute p_card, bool p_needSlot = false)
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass.Add("rarity", p_card.CardRarity);
			jSONClass.Add("level", p_card.UserCardTotalLevel);
			if (p_needSlot)
			{
				jSONClass.Add("slot", p_card.SlotName);
			}
			return jSONClass;
		}

		private static JSONArray TerminalBasketToJSONArray(List<TerminalItemGroupAttribute> p_terminalBasket)
		{
			JSONArray jSONArray = new JSONArray();
			foreach (TerminalItemGroupAttribute item in p_terminalBasket)
			{
				jSONArray.Add(TerminalItemToJSONClass(item));
			}
			return jSONArray;
		}

		private static JSONClass TerminalItemToJSONClass(TerminalItemGroupAttribute p_item)
		{
			JSONClass jSONClass = new JSONClass();
			if (p_item.IsCardReward)
			{
				jSONClass.Add("name", p_item.CardName);
			}
			else
			{
				jSONClass.Add("name", p_item.ItemPresetName);
			}
			jSONClass.Add("count", p_item.Count);
			return jSONClass;
		}

		private static JSONArray BoosterpackBasketToJSONArray(List<BoosterpackItem> p_boosterpackBasket)
		{
			JSONArray jSONArray = new JSONArray();
			foreach (BoosterpackItem item in p_boosterpackBasket)
			{
				jSONArray.Add(BoosterpackItemToJSONClass(item));
			}
			return jSONArray;
		}

		private static JSONClass BoosterpackItemToJSONClass(BoosterpackItem p_item)
		{
			JSONClass jSONClass = new JSONClass();
			if (p_item.IsCard)
			{
				jSONClass.Add("name", p_item.ItemAsCard.CardName);
			}
			else if (p_item.IsCoupon)
			{
				jSONClass.Add("name", p_item.ItemAsCoupon.TypeName);
			}
			else if (p_item.IsCurrency)
			{
				jSONClass.Add("name", p_item.ItemAsCurrency.CurrencyType.GetName());
			}
			jSONClass.Add("count", p_item.Count);
			return jSONClass;
		}

		private static string GetCurrentSceneName()
		{
			switch (Manager.Scene)
			{
			case SceneKind.GameLoader:
				return "game_loader";
			case SceneKind.Loader:
				return "loader";
			case SceneKind.Main:
				return "main";
			case SceneKind.Run:
				return "run";
			case SceneKind.Shop:
				return "shop";
			case SceneKind.Terminal:
				return "terminal";
			default:
				return "unknown";
			}
		}

		private static string GetCurrentScreenName()
		{
			switch (Manager.CurrentScreen)
			{
			case ScreenType.Archive:
				return "archive";
			case ScreenType.ArchiveCategory:
				return "archive_category";
			case ScreenType.Boosterpack:
				return "boosterpack";
			case ScreenType.Credits:
				return "credits";
			case ScreenType.Details:
				return "details";
			case ScreenType.Journal:
				return "journal";
			case ScreenType.Main:
				return (!Manager.IsEquip) ? "shop" : "equip";
			default:
				return "unknown";
			}
		}

		private static string GetDoYouLikeGameAnswer1()
		{
			switch ((DialogCallbacks.DoYouLikeGameAnswer)(int)CounterController.Current.CounterDoYouLikeGameAnswerPt1)
			{
			case DialogCallbacks.DoYouLikeGameAnswer.Yes:
				return "yes";
			case DialogCallbacks.DoYouLikeGameAnswer.No:
				return "no";
			default:
				return "unknown";
			}
		}

		private static string GetDoYouLikeGameAnswer2()
		{
			if ((int)CounterController.Current.CounterDoYouLikeGameAnswerPt1 == 0)
			{
				switch ((DialogCallbacks.SetStarAnswer)(int)CounterController.Current.CounterDoYouLikeGameAnswerPt2)
				{
				case DialogCallbacks.SetStarAnswer.Ok:
					return "ok";
				case DialogCallbacks.SetStarAnswer.DontAskAgain:
					return "dont_ask_again";
				case DialogCallbacks.SetStarAnswer.Later:
					return "later";
				default:
					return "unknown";
				}
			}
			switch ((DialogCallbacks.SupportAnswer)(int)CounterController.Current.CounterDoYouLikeGameAnswerPt2)
			{
			case DialogCallbacks.SupportAnswer.Yes:
				return "yes";
			case DialogCallbacks.SupportAnswer.No:
				return "no";
			default:
				return "unknown";
			}
		}
	}
}
