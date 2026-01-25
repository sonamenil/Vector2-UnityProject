using System;
using Nekki.Vector.Core.ABTest;
using Nekki.Vector.Core.Payment;
using Nekki.Vector.Core.User;
using SimpleJSON;
using UnityEngine;

namespace Nekki.Vector.Core.Statistics
{
	public static class StatisticsEvent
	{
		public enum EventType
		{
			Session_start = 0,
			Floor_start = 1,
			Shop_enter = 2,
			Run_end = 3,
			Equip_card = 4,
			Payment = 5,
			Main_scene_enter = 6,
			Terminal_close = 7,
			Tutor_step = 8,
			Reroll_card = 9,
			Trigger_signal = 10,
			Screen_enter = 11,
			Save_me = 12,
			Card_boost = 13,
			Boosterpack_open = 14,
			Free = 15,
			Energy_recharge = 16,
			Do_you_like_game = 17,
			Open_Payments_From_Mission_Card = 18
		}

		private static int UnixTimestamp
		{
			get
			{
				return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			}
		}

		private static string PlatformId
		{
			get
			{
				if (DeviceInformation.IsiOS)
				{
					return "i";
				}
				if (DeviceInformation.IsAndroid)
				{
					return "a";
				}
				return "u";
			}
		}

		public static bool IsSaveEvent(EventType p_type)
		{
			return true;
		}

		public static bool IsSendNow(EventType p_type)
		{
			if (p_type == EventType.Tutor_step)
			{
				return true;
			}
			return false;
		}

		public static string GetJSONDataEvent(EventType p_type, ArgsDict p_args)
		{
			switch (p_type)
			{
			case EventType.Session_start:
				return EventSessionStartJSONString(p_args);
			case EventType.Floor_start:
				return EventFloorStartJSONString(p_args);
			case EventType.Shop_enter:
				return EventShopEnterJSONString(p_args);
			case EventType.Run_end:
				return EventRunEndJSONString(p_args);
			case EventType.Equip_card:
				return EventEquipCardJSONString(p_args);
			case EventType.Payment:
				return EventPaymentJSONString(p_args);
			case EventType.Main_scene_enter:
				return EventMainSceneEnterJSONString(p_args);
			case EventType.Terminal_close:
				return EventTerminalCloseJSONString(p_args);
			case EventType.Tutor_step:
				return EventTutorStepJSONString(p_args);
			case EventType.Reroll_card:
				return EventRerollCardJSONString(p_args);
			case EventType.Trigger_signal:
				return EventTriggerSignalJSONString(p_args);
			case EventType.Screen_enter:
				return EventScreenEnterJSONString(p_args);
			case EventType.Save_me:
				return EventSaveMeJSONString(p_args);
			case EventType.Card_boost:
				return EventCardBoostJSONString(p_args);
			case EventType.Boosterpack_open:
				return EventBoosterpackOpenJSONString(p_args);
			case EventType.Free:
				return EventFreeJSONString(p_args);
			case EventType.Energy_recharge:
				return EventEnergyRechargeJSONString(p_args);
			case EventType.Do_you_like_game:
				return EventDoYouLikeGameJSONString(p_args);
			case EventType.Open_Payments_From_Mission_Card:
				return OpenPaymentsFromMissionCardJSONString(p_args);
			default:
				return null;
			}
		}

		private static JSONClass CreateHeadJSON(EventType p_type)
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass["eid"] = StatisticsCollector.NextEventID;
			jSONClass["etype"] = TypeToString(p_type);
			jSONClass["build_version"] = ApplicationController.BuildPlusGamedataVersion;
			jSONClass["install_id"] = DataLocal.Current.InstallID;
			jSONClass["device"] = DeviceInformation.DeviceModel;
			jSONClass["time_local"] = UnixTimestamp;
			jSONClass["os"] = PlatformId;
			jSONClass["paid"] = ((!DataLocal.Current.IsPaidVersion) ? "f" : ((!ApplicationController.IsPaidBundleID) ? "b" : "p"));
			jSONClass["money_spent"] = Mathf.CeilToInt(PaymentController.MoneySpent);
			jSONClass["cheat_count"] = PaymentController.CheatingCount;
			string userABGroup = ABTestManager.UserABGroup;
			if (!string.IsNullOrEmpty(userABGroup))
			{
				jSONClass["ab_group"] = userABGroup;
			}
			return jSONClass;
		}

		private static string TypeToString(EventType p_type)
		{
			return p_type.ToString().ToLower();
		}

		private static string EventSessionStartJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Session_start);
			StatisticsGeter.AddBundleId(jSONClass);
			StatisticsGeter.AddMoney2(jSONClass);
			StatisticsGeter.AddMoney3(jSONClass);
			StatisticsGeter.AddCountersTutorial(jSONClass);
			StatisticsGeter.AddCountersQuest(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddUniqueCardsCount(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventFloorStartJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Floor_start);
			StatisticsGeter.AddUserGadgets(jSONClass);
			StatisticsGeter.AddCurrency(jSONClass);
			StatisticsGeter.AddProtocol(jSONClass);
			StatisticsGeter.AddEnergyLevel(jSONClass);
			StatisticsGeter.AddCountersTutorial(jSONClass);
			StatisticsGeter.AddCountersQuest(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddSceneLoadTime(jSONClass);
			StatisticsGeter.AddGeneratorTime(jSONClass);
			StatisticsGeter.AddGeneratorSeed(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddCurrentStars(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			StatisticsGeter.AddCountersMissions(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventShopEnterJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Shop_enter);
			StatisticsGeter.AddUserGadgets(jSONClass);
			StatisticsGeter.AddShopGeneratedCards(jSONClass);
			StatisticsGeter.AddUserRankGained(jSONClass);
			StatisticsGeter.AddCurrency(jSONClass);
			StatisticsGeter.AddEnergyLevel(jSONClass);
			StatisticsGeter.AddProtocol(jSONClass);
			StatisticsGeter.AddSceneLoadTime(jSONClass);
			StatisticsGeter.AddCountersTutorial(jSONClass);
			StatisticsGeter.AddCountersQuest(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddFloorTime(jSONClass);
			StatisticsGeter.AddLocks(jSONClass);
			StatisticsGeter.AddStuntsGenerated(jSONClass);
			StatisticsGeter.AddStuntsUnlocked(jSONClass);
			StatisticsGeter.AddStuntsCollected(jSONClass);
			StatisticsGeter.AddItemsGenerated(jSONClass);
			StatisticsGeter.AddItemsCollected(jSONClass);
			StatisticsGeter.AddForksHard(jSONClass);
			StatisticsGeter.AddTraps(jSONClass);
			StatisticsGeter.AddRunFPS(jSONClass);
			StatisticsGeter.AddCurrentStars(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			StatisticsGeter.AddCountersMissions(jSONClass);
			StatisticsGeter.AddCountersCompletedMissions(jSONClass);
			StatisticsGeter.AddCardsRatio(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventRunEndJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Run_end);
			StatisticsGeter.AddUserGadgets(jSONClass);
			StatisticsGeter.AddUserRank(jSONClass);
			StatisticsGeter.AddCurrency(jSONClass);
			StatisticsGeter.AddProtocol(jSONClass);
			StatisticsGeter.AddSceneLoadTime(jSONClass);
			StatisticsGeter.AddCountersTutorial(jSONClass);
			StatisticsGeter.AddCountersQuest(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddFloorTime(jSONClass);
			StatisticsGeter.AddLocks(jSONClass);
			StatisticsGeter.AddStuntsGenerated(jSONClass);
			StatisticsGeter.AddStuntsUnlocked(jSONClass);
			StatisticsGeter.AddStuntsCollected(jSONClass);
			StatisticsGeter.AddItemsGenerated(jSONClass);
			StatisticsGeter.AddItemsCollected(jSONClass);
			StatisticsGeter.AddForksHard(jSONClass);
			StatisticsGeter.AddTraps(jSONClass);
			StatisticsGeter.AddRunFPS(jSONClass);
			StatisticsGeter.AddDeathType(jSONClass);
			StatisticsGeter.AddCurrentStars(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			StatisticsGeter.AddCardsRatio(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventEquipCardJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Equip_card);
			StatisticsGeter.AddNewCardEquipped(p_args, jSONClass);
			StatisticsGeter.AddNewCardSlot(p_args, jSONClass);
			StatisticsGeter.AddReplacedCard(p_args, jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventPaymentJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Payment);
			StatisticsGeter.AddPaymentData(p_args, jSONClass);
			StatisticsGeter.AddMoney2(jSONClass);
			StatisticsGeter.AddMoney3(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddSceneType(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventMainSceneEnterJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Main_scene_enter);
			StatisticsGeter.AddSceneLoadTime(jSONClass);
			StatisticsGeter.AddCountersTutorial(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventTerminalCloseJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Terminal_close);
			StatisticsGeter.AddTerminalBasketItems(jSONClass);
			StatisticsGeter.AddTerminalBoughtItems(jSONClass);
			StatisticsGeter.AddMoney2(jSONClass);
			StatisticsGeter.AddMoney3(jSONClass);
			StatisticsGeter.AddProtocol(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventTutorStepJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Tutor_step);
			StatisticsGeter.AddSignalMessage(p_args, jSONClass);
			return jSONClass.ToString();
		}

		private static string EventRerollCardJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Reroll_card);
			StatisticsGeter.AddUseCoupon(p_args, jSONClass);
			StatisticsGeter.AddCardAfterReroll(p_args, jSONClass);
			StatisticsGeter.AddRerolledCard(p_args, jSONClass);
			StatisticsGeter.AddRerollTryCount(jSONClass);
			StatisticsGeter.AddProtocol(jSONClass);
			StatisticsGeter.AddCurrency(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventTriggerSignalJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Trigger_signal);
			StatisticsGeter.AddSignalMessage(p_args, jSONClass);
			return jSONClass.ToString();
		}

		private static string EventScreenEnterJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Screen_enter);
			StatisticsGeter.AddScreenType(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventSaveMeJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Save_me);
			StatisticsGeter.AddUseCoupon(p_args, jSONClass);
			StatisticsGeter.AddSaveMeAttempts(jSONClass);
			StatisticsGeter.AddProtocol(jSONClass);
			StatisticsGeter.AddUserRank(jSONClass);
			StatisticsGeter.AddCurrency(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventCardBoostJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Card_boost);
			StatisticsGeter.AddUseCoupon(p_args, jSONClass);
			StatisticsGeter.AddCardBoostCard(p_args, jSONClass);
			StatisticsGeter.AddProtocol(jSONClass);
			StatisticsGeter.AddUserRank(jSONClass);
			StatisticsGeter.AddCurrency(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventBoosterpackOpenJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Boosterpack_open);
			StatisticsGeter.AddBoosterpackBasketItems(jSONClass);
			StatisticsGeter.AddMoney2(jSONClass);
			StatisticsGeter.AddMoney3(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventFreeJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Free);
			StatisticsGeter.AddFreeData(p_args, jSONClass);
			return jSONClass.ToString();
		}

		private static string EventEnergyRechargeJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Energy_recharge);
			StatisticsGeter.AddUseCoupon(p_args, jSONClass);
			StatisticsGeter.AddEnergyLevel(jSONClass, "energy_remaining");
			StatisticsGeter.AddMoney3(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string EventDoYouLikeGameJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Do_you_like_game);
			StatisticsGeter.AddDoYouLikeGameAnswer1(jSONClass);
			StatisticsGeter.AddDoYouLikeGameAnswer2(jSONClass);
			StatisticsGeter.AddScreenType(jSONClass);
			StatisticsGeter.AddMoney2(jSONClass);
			StatisticsGeter.AddMoney3(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			return jSONClass.ToString();
		}

		private static string OpenPaymentsFromMissionCardJSONString(ArgsDict p_args)
		{
			JSONClass jSONClass = CreateHeadJSON(EventType.Open_Payments_From_Mission_Card);
			StatisticsGeter.AddScreenType(jSONClass);
			StatisticsGeter.AddMoney2(jSONClass);
			StatisticsGeter.AddMoney3(jSONClass);
			StatisticsGeter.AddRunCount(jSONClass);
			StatisticsGeter.AddMaxStars(jSONClass);
			StatisticsGeter.AddCurrentStars(jSONClass);
			StatisticsGeter.AddFloorNumber(jSONClass);
			StatisticsGeter.AddMissionsDifficulty(jSONClass);
			StatisticsGeter.AddMissionsUnownedCards(jSONClass);
			return jSONClass.ToString();
		}
	}
}
