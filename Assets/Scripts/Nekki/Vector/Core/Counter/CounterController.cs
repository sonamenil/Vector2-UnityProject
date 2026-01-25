using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Counter
{
	public class CounterController
	{
		public const string Namespace_Default = "ST_Default";

		public const string Namespace_Generator = "ST_Generator";

		public const string Namespace_Run = "ST_Run";

		public const string Namespace_Postprocess = "ST_Postprocess";

		public const string Namespace_Store = "ST_Store";

		public const string Namespace_StartItems = "ST_StartItems";

		public const string Namespace_ProgressMarkers = "ST_ProgressMarkers";

		public const string Namespace_Tutorial = "ST_Tutorial";

		public const string Namespace_SelectedGeneratorLabels = "ST_SelectedGeneratorLabels";

		public const string Namespace_CounterStarterPacksWithReducedCooldown = "ST_StarterPacksWithReducedCooldown";

		public const string Namespace_Statistics = "ST_Statistics";

		public const string Namespace_Statistics_Per_Floor = "ST_Statistics_Per_Floor";

		public const string Namespace_Statistics_Stunts_Generated = "ST_Statistics_Stunts_Generated";

		public const string Namespace_Statistics_Stunts_Unlocked = "ST_Statistics_Stunts_Unlocked";

		public const string Namespace_Statistics_Stunts_Collected = "ST_Statistics_Stunts_Collected";

		public const string Namespace_Statistics_Items_Generated = "ST_Statistics_Items_Generated";

		public const string Namespace_Statistics_Items_Collected = "ST_Statistics_Items_Collected";

		public const string Namespace_Statistics_Traps = "ST_Statistics_Traps";

		public const string Namespace_Statistics_Temporary = "ST_Statistics_Temporary";

		public const string Namespace_Quest = "ST_Quests";

		public const string Namespace_NewQuest = "ST_NewQuest";

		public const string Namespace_QuestPostpone = "ST_QuestPostpone";

		public const string Namespace_RequirementsProgress = "Requirements";

		public const string Namespace_BoostedCards = "BoostedCards";

		public const string Namespace_BonusWeightForBoostedCards = "OverweightedUpgrades";

		public const string Namespace_Events = "ST_Events";

		public const string Namespace_Achievements = "ST_Achievements";

		public const string Namespace_PassiveEffects = "PassiveEffects";

		public const string Namespace_Offers = "ST_Offers";

		public const string Namespace_GUI = "GUI";

		public const string Namespace_StarsRewards = "StarRewards";

		public const string Namespace_StarBuffs = "StarBuffs";

		public const string Namespace_MissionProgress = "MissionProgress";

		public const string Namespace_MissionObjectives = "MissionObjectives";

		public const string Namespace_InsertedCards = "InsertedCards";

		public const string Namespace_UpgradesGenerator = "UpgradesGenerator";

		public const string Namespace_Temp = "Temp";

		public const string Namespace_UsefulCards = "UsefulCards";

		public const string Namespace_GeneratedEffects = "GeneratedEffects";

		private const string CounterFloorName = "ST_Floor";

		private const string CounterStartFloor = "StartFloor";

		private const string CounterRoomNumberName = "ST_RoomNumber";

		private const string CounterRoomNumberReversedName = "ST_RoomNumberReversed";

		private const string CounterAdBlockName = "AdBlock";

		private const string CounterSavemeBlockName = "ST_SavemeBlock";

		private const string CounterBoosterpacksBlockName = "ST_BoosterpacksBlock";

		private const string CounterNewsBlockName = "ST_NewsBlock";

		private const string CounterShowDoYouLikeGameDialogName = "ST_ShowDoYouLikeGameDialog";

		private const string CounterShowSetStarDialogName = "ST_ShowSetStarDialog";

		private const string CounterMaxChapterName = "ST_MaxChapter";

		private const string CounterSaveMeAttemptName = "ST_SaveMeAttempt";

		private const string CounterShowNoQuestProgressDialogName = "ST_ShowNoQuestProgressDialog";

		private const string CounterQuestProgressByRunName = "ST_QuestProgressByRun";

		private const string CounterTutorialBasicName = "ST_TutorialBasic";

		private const string CounterTutorialLaserAndMineName = "ST_TutorialLaserAndMine";

		private const string CounterTutorialArchiveNamePt2 = "ST_Archive_part2";

		private const string CounterTutorialArchiveNamePt1 = "ST_Archive_part1";

		private const string CounterTutorialPassivesName = "ST_PassivesIntro";

		private const string CounterTutorialStartItemsInfoName = "ST_StartItemsInfo";

		private const string CounterTutorialRunEndName = "ST_RunEnd";

		private const string CounterCardBoostTutorialName = "ST_CardBoostTutorial";

		private const string CounterBoosterpackPt2Name = "ST_BoosterpackTutorial_Part2";

		private const string СounterInsertUpgradeTutorialName = "InsertUpgradeTutor";

		private const string СounterTutorialInProgressName = "TutorialInProgress";

		private const string CounterPlayCommandName = "ST_PlayCommand";

		private const string CounterRunCounterName = "ST_run_counter";

		private const string CounterTimerEnabledName = "ST_TimerEnabled";

		private const string CounterPlacholderIterationCountName = "ST_IterationCount";

		private const string CounterFeedbackName = "ST_Feedback";

		private const string CounterGenerationAttemptName = "ST_GenerationAttempt";

		private const string CounterEnableRoomReuseName = "ST_EnableRoomReuse";

		private const string CounterEscapeQuestRunQuitName = "CounterEscapeQuestRunQuit";

		private const string CounterBlueLocksName = "ST_BlueLocks";

		private const string CounterYellowLocksName = "ST_YellowLocks";

		private const string CounterRedLocksName = "ST_RedLocks";

		private const string CounterHardForksName = "ST_HardForks";

		private const string CounterFloorGenerationTimeName = "ST_FloorGenerationTime";

		private const string CounterFloorPostprocessTimeName = "ST_FloorPostprocessTime";

		private const string CounterMineDeathName = "MineDeath";

		private const string CounterLaserDeathName = "LaserDeath";

		private const string CounterBlackballDeathName = "BlackballDeath";

		private const string CounterTeslaDeathName = "TeslaDeath";

		private const string CounterDoYouLikeGameAnswerPt1Name = "DoYouLikeGameAnswer_part1";

		private const string CounterDoYouLikeGameAnswerPt2Name = "DoYouLikeGameAnswer_part2";

		private const string CounterRerollTryCountName = "RerollTryCount";

		private const string CounterShopGenerateOnButtonName = "ST_GenerateOnButton";

		private const string CounterShopGenerateOnButtonIterationName = "ST_GenerateOnButtonIteration";

		private const string CounterShopGenerateOnButtonPriceIncreaseName = "ST_GenerateOnButtonPriceIncrease";

		private const string CounterPaymentPromoName = "PaymentPromo";

		private const string CounterPaymentPromo2Name = "PaymentPromo2";

		private const string CounterPromoActiveEffectName = "PromoActiveEffect";

		private const string CounterPassivesIntroName = "PassivesIntro";

		private const string CounterStuntsExecutedName = "StuntsExecuted";

		private const string CounterStuntsGeneratedName = "StuntsExecutable";

		private const string CounterPointsForStuntsName = "PointsForStunts";

		private const string CounterPointsForFloorName = "PointsForFloor";

		private const string CounterCurrentMissionStarsName = "CurrentStars";

		private const string CounterMaxMissionStarsName = "MaxStars";

		public const string CurrentRoomNamespaceName = "ST_CurrentRoom";

		public const string CurrentRoomNamespacePrefix = "!Room_";

		public const string CurrentObjectNamespaceName = "ST_LocalSpace";

		public const string CurrentObjectNamespacePrefix = "_!Object_";

		private const string CounterAvailableCardsIsOverName = "AvailableCardsIsOver";

		private Dictionary<string, CounterNamespace> _NamespaceCounters = new Dictionary<string, CounterNamespace>();

		public static CounterController Current
		{
			get
			{
				return DataLocal.Current.CounterController;
			}
		}

		public Dictionary<string, CounterNamespace> NamespaceCounters
		{
			get
			{
				return _NamespaceCounters;
			}
		}

		public static string CurrentRoomNamespace
		{
			get
			{
				return "!Room_" + Current.CounterRoomNumber;
			}
		}

		public static string CurrentObjectNamespace
		{
			get
			{
				return CurrentRoomNamespace + "_!Object_" + ObjectRunner.CurrentLocalNamespace;
			}
		}

		public ObscuredInt CounterFloor
		{
			get
			{
				return GetUserCounter("ST_Floor");
			}
			set
			{
				CreateCounterOrSetValue("ST_Floor", value);
			}
		}

		public ObscuredInt StartFloor
		{
			get
			{
				return GetUserCounter("StartFloor");
			}
			set
			{
				CreateCounterOrSetValue("StartFloor", value);
			}
		}

		public ObscuredInt CounterMissionsBlock
		{
			get
			{
				return GetUserCounter("MissionsBlock", "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterRoomNumber
		{
			get
			{
				return GetUserCounter("ST_RoomNumber", "ST_Generator");
			}
			set
			{
				CreateCounterOrSetValue("ST_RoomNumber", value, "ST_Generator");
			}
		}

		public ObscuredInt CounterTimerEnabled
		{
			get
			{
				return GetUserCounter("ST_TimerEnabled", "ST_Generator");
			}
		}

		public ObscuredInt CounterRoomNumberReversed
		{
			get
			{
				return GetUserCounter("ST_RoomNumberReversed", "ST_Generator");
			}
			set
			{
				CreateCounterOrSetValue("ST_RoomNumberReversed", value, "ST_Generator");
			}
		}

		public ObscuredInt CounterTutorialBasic
		{
			get
			{
				return GetUserCounter("ST_TutorialBasic", "ST_Tutorial");
			}
		}

		public ObscuredInt CounterTutorialLaserAndMine
		{
			get
			{
				return GetUserCounter("ST_TutorialLaserAndMine", "ST_Tutorial");
			}
		}

		public ObscuredInt СounterInsertUpgradeTutorial
		{
			get
			{
				return GetUserCounter("InsertUpgradeTutor", "ST_Tutorial");
			}
		}

		public ObscuredInt CounterAdBlock
		{
			get
			{
				return GetUserCounter("AdBlock", "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterSavemeBlock
		{
			get
			{
				return GetUserCounter("ST_SavemeBlock", "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterBoosterpacksBlock
		{
			get
			{
				return GetUserCounter("ST_BoosterpacksBlock", "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterNewsBlock
		{
			get
			{
				return GetUserCounter("ST_NewsBlock", "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterRerollShopBlock
		{
			get
			{
				return GetUserCounter("RerollShopBlock", "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterCurrentMissionStars
		{
			get
			{
				return GetUserCounter("CurrentStars", "StarRewards");
			}
			set
			{
				CreateCounterOrSetValue("CurrentStars", value, "StarRewards");
			}
		}

		public ObscuredInt CounterMaxMissionStars
		{
			get
			{
				return GetUserCounter("MaxStars", "StarRewards");
			}
		}

		public ObscuredInt CounterCardsCount
		{
			get
			{
				return GetUserCounter("CardsCount", "ST_Statistics");
			}
			set
			{
				CreateCounterOrSetValue("CardsCount", value, "ST_Statistics");
			}
		}

		public ObscuredInt CounterTutorialStartItemsInfo
		{
			get
			{
				return GetUserCounter("ST_StartItemsInfo", "ST_Tutorial");
			}
		}

		public ObscuredInt CounterTutorialArchivePt2
		{
			get
			{
				return GetUserCounter("ST_Archive_part2", "ST_Tutorial");
			}
		}

		public ObscuredInt CounterTutorialArchivePt1
		{
			get
			{
				return GetUserCounter("ST_Archive_part1", "ST_Tutorial");
			}
		}

		public ObscuredInt CounterTutorialBoosterpackPt2
		{
			get
			{
				return GetUserCounter("ST_BoosterpackTutorial_Part2", "ST_Tutorial");
			}
			set
			{
				CreateCounterOrSetValue("ST_BoosterpackTutorial_Part2", value, "ST_Tutorial");
			}
		}

		public ObscuredInt CounterTutorialMissionsStars
		{
			get
			{
				return GetUserCounter("MissionsTutorStars", "ST_Tutorial");
			}
		}

		public ObscuredInt CounterCardBoostTutorial
		{
			get
			{
				return GetUserCounter("ST_CardBoostTutorial", "ST_Tutorial");
			}
			set
			{
				CreateCounterOrSetValue("ST_CardBoostTutorial", value, "ST_Tutorial");
			}
		}

		public ObscuredInt CounterTutorialPassives
		{
			get
			{
				return GetUserCounter("ST_PassivesIntro", "ST_Tutorial");
			}
		}

		public ObscuredInt CounterTutorialRunEnd
		{
			get
			{
				return GetUserCounter("ST_RunEnd", "ST_Tutorial");
			}
		}

		public ObscuredInt СounterTutorialInProgress
		{
			get
			{
				return GetUserCounter("TutorialInProgress", "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterPlayCommand
		{
			get
			{
				return GetUserCounter("ST_PlayCommand");
			}
			set
			{
				CreateCounterOrSetValue("ST_PlayCommand", value);
			}
		}

		public ObscuredInt CounterRunCounter
		{
			get
			{
				return GetUserCounter("ST_run_counter", "ST_Statistics");
			}
			set
			{
				CreateCounterOrSetValue("ST_run_counter", value, "ST_Statistics");
			}
		}

		public ObscuredInt CounterPlaceholderIterationCount
		{
			set
			{
				CreateCounterOrSetValue("ST_IterationCount", value, "ST_Postprocess");
			}
		}

		public ObscuredInt CounterFeedback
		{
			get
			{
				return GetUserCounter("ST_Feedback");
			}
			set
			{
				CreateCounterOrSetValue("ST_Feedback", value);
			}
		}

		public ObscuredInt CounterGenerationAttempt
		{
			set
			{
				CreateCounterOrSetValue("ST_GenerationAttempt", value, "ST_Generator");
			}
		}

		public ObscuredInt CounterEnableRoomReuse
		{
			get
			{
				return GetUserCounter("ST_EnableRoomReuse", "ST_Generator");
			}
		}

		public ObscuredInt CounterBlueLocks
		{
			get
			{
				return GetUserCounter("ST_BlueLocks", "ST_Statistics_Per_Floor");
			}
		}

		public ObscuredInt CounterYellowLocks
		{
			get
			{
				return GetUserCounter("ST_YellowLocks", "ST_Statistics_Per_Floor");
			}
		}

		public ObscuredInt CounterRedLocks
		{
			get
			{
				return GetUserCounter("ST_RedLocks", "ST_Statistics_Per_Floor");
			}
		}

		public ObscuredInt CounterHardForks
		{
			get
			{
				return GetUserCounter("ST_HardForks", "ST_Statistics_Per_Floor");
			}
		}

		public ObscuredInt CounterFloorGenerationTime
		{
			get
			{
				return GetUserCounter("ST_FloorGenerationTime", "ST_Statistics_Temporary");
			}
			set
			{
				CreateCounterOrSetValue("ST_FloorGenerationTime", value, "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterFloorPostprocessTime
		{
			get
			{
				return GetUserCounter("ST_FloorPostprocessTime", "ST_Statistics_Temporary");
			}
			set
			{
				CreateCounterOrSetValue("ST_FloorPostprocessTime", value, "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterMineDeath
		{
			get
			{
				return GetUserCounter("MineDeath", "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterLaserDeath
		{
			get
			{
				return GetUserCounter("LaserDeath", "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterBlackballDeath
		{
			get
			{
				return GetUserCounter("BlackballDeath", "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterTeslaDeath
		{
			get
			{
				return GetUserCounter("TeslaDeath", "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterStarterPackCoolness
		{
			get
			{
				return GetUserCounter("StarterPackCoolness");
			}
		}

		public ObscuredInt CounterDoYouLikeGameAnswerPt1
		{
			get
			{
				return GetUserCounter("DoYouLikeGameAnswer_part1", "ST_Statistics_Temporary");
			}
			set
			{
				CreateCounterOrSetValue("DoYouLikeGameAnswer_part1", value, "ST_Statistics_Temporary");
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterDoYouLikeGameAnswerPt2
		{
			get
			{
				return GetUserCounter("DoYouLikeGameAnswer_part2", "ST_Statistics_Temporary");
			}
			set
			{
				CreateCounterOrSetValue("DoYouLikeGameAnswer_part2", value, "ST_Statistics_Temporary");
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterRerollTryCount
		{
			get
			{
				return GetUserCounter("RerollTryCount", "ST_Statistics_Temporary");
			}
			set
			{
				CreateCounterOrSetValue("RerollTryCount", value, "ST_Statistics_Temporary");
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterShopGenerateOnButton
		{
			get
			{
				return GetUserCounter("ST_GenerateOnButton", "ST_Store");
			}
			set
			{
				CreateCounterOrSetValue("ST_GenerateOnButton", value, "ST_Store");
			}
		}

		public ObscuredInt CounterShopGenerateOnButtonIteration
		{
			get
			{
				return GetUserCounter("ST_GenerateOnButtonIteration", "ST_Store");
			}
			set
			{
				CreateCounterOrSetValue("ST_GenerateOnButtonIteration", value, "ST_Store");
			}
		}

		public ObscuredInt CounterShopGenerateOnButtonPriceIncrease
		{
			get
			{
				return GetUserCounter("ST_GenerateOnButtonPriceIncrease", "ST_Store");
			}
			set
			{
				CreateCounterOrSetValue("ST_GenerateOnButtonPriceIncrease", value, "ST_Store");
			}
		}

		public ObscuredInt CounterPromoActiveEffect
		{
			get
			{
				return GetUserCounter("PromoActiveEffect", "ST_Events");
			}
			set
			{
				CreateCounterOrSetValue("PromoActiveEffect", value, "ST_Events");
			}
		}

		public ObscuredInt CounterPaymentPromo
		{
			get
			{
				return GetUserCounter("PaymentPromo", "ST_Events");
			}
			set
			{
				CreateCounterOrSetValue("PaymentPromo", value, "ST_Events");
			}
		}

		public ObscuredInt CounterPaymentPromo2
		{
			get
			{
				return GetUserCounter("PaymentPromo2", "ST_Events");
			}
			set
			{
				CreateCounterOrSetValue("PaymentPromo2", value, "ST_Events");
			}
		}

		public ObscuredInt CounterPromo2CanRun
		{
			get
			{
				return GetUserCounter("Promo2CanRun", "ST_Events");
			}
		}

		public ObscuredInt CounterInitialBundleRequest
		{
			get
			{
				return GetUserCounter("ST_InitialBundleRequest", "ST_ProgressMarkers");
			}
			set
			{
				CreateCounterOrSetValue("ST_InitialBundleRequest", value, "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterNotifyNewBoosterPackOnUpdate
		{
			get
			{
				return GetUserCounter("ST_NotifyNewBoosterPackOnUpdate", "ST_ProgressMarkers");
			}
			set
			{
				CreateCounterOrSetValue("ST_NotifyNewBoosterPackOnUpdate", value, "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterEscapeQuestRunQuit
		{
			get
			{
				return GetUserCounter("CounterEscapeQuestRunQuit", "ST_Generator");
			}
			set
			{
				CreateCounterOrSetValue("CounterEscapeQuestRunQuit", value, "ST_Generator");
			}
		}

		public ObscuredInt CounterShowSetStarDialog
		{
			get
			{
				return GetUserCounter("ST_ShowSetStarDialog", "ST_ProgressMarkers");
			}
			set
			{
				CreateCounterOrSetValue("ST_ShowSetStarDialog", value, "ST_ProgressMarkers");
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterMaxChapter
		{
			get
			{
				return GetUserCounter("ST_MaxChapter", "ST_ProgressMarkers");
			}
			set
			{
				CreateCounterOrSetValue("ST_MaxChapter", value, "ST_ProgressMarkers");
			}
		}

		public ObscuredInt CounterSaveMeAttempt
		{
			get
			{
				return GetUserCounter("ST_SaveMeAttempt");
			}
			set
			{
				CreateCounterOrSetValue("ST_SaveMeAttempt", value);
			}
		}

		public ObscuredInt CounterShowDoYouLikeGameDialog
		{
			get
			{
				return GetUserCounter("ST_ShowDoYouLikeGameDialog", "ST_ProgressMarkers");
			}
			set
			{
				CreateCounterOrSetValue("ST_ShowDoYouLikeGameDialog", value, "ST_ProgressMarkers");
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterShowNoQuestProgressDialog
		{
			get
			{
				return GetUserCounter("ST_ShowNoQuestProgressDialog", "ST_ProgressMarkers");
			}
			set
			{
				CreateCounterOrSetValue("ST_ShowNoQuestProgressDialog", value, "ST_ProgressMarkers");
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterQuestProgressByRun
		{
			get
			{
				return GetUserCounter("ST_QuestProgressByRun");
			}
			set
			{
				CreateCounterOrSetValue("ST_QuestProgressByRun", value);
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterForcedOpenBoosterpacks
		{
			get
			{
				return GetUserCounter("ST_ForcedOpenBoosterpacks", "ST_ProgressMarkers");
			}
			set
			{
				CreateCounterOrSetValue("ST_ForcedOpenBoosterpacks", value, "ST_ProgressMarkers");
				DataLocal.Current.Save(false);
			}
		}

		public ObscuredInt CounterPassivesIntro
		{
			get
			{
				return GetUserCounter("PassivesIntro", "ST_Quests");
			}
		}

		public ObscuredInt CounterStuntsGenerated
		{
			get
			{
				return GetUserCounter("StuntsExecutable", "GUI");
			}
		}

		public ObscuredInt CounterStuntsExecuted
		{
			get
			{
				return GetUserCounter("StuntsExecuted", "GUI");
			}
		}

		public ObscuredInt CounterPointsForStunts
		{
			get
			{
				return GetUserCounter("PointsForStunts", "GUI");
			}
		}

		public ObscuredInt CounterPointsForFloor
		{
			get
			{
				return GetUserCounter("PointsForFloor", "GUI");
			}
		}

		public ObscuredInt CounterTutorialEndRun
		{
			get
			{
				return GetUserCounter("TutorialEndRun", "ST_Statistics");
			}
		}

		public ObscuredInt CounterMissionDifficulty
		{
			get
			{
				return GetUserCounter("MissionDifficulty", "ST_Statistics_Temporary");
			}
			set
			{
				CreateCounterOrSetValue("MissionDifficulty", value, "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterMissionUnownedCards
		{
			get
			{
				return GetUserCounter("MissionUnownedCards", "ST_Statistics_Temporary");
			}
			set
			{
				CreateCounterOrSetValue("MissionUnownedCards", value, "ST_Statistics_Temporary");
			}
		}

		public ObscuredInt CounterAvailableCardsIsOver
		{
			get
			{
				return GetUserCounter("AvailableCardsIsOver", "UpgradesGenerator");
			}
		}

		public CounterController()
		{
		}

		private CounterController(CounterController p_copy)
		{
			foreach (KeyValuePair<string, CounterNamespace> namespaceCounter in p_copy._NamespaceCounters)
			{
				if (namespaceCounter.Value.IsForceSave)
				{
					_NamespaceCounters.Add(namespaceCounter.Key, namespaceCounter.Value);
				}
				else
				{
					_NamespaceCounters.Add(namespaceCounter.Key, namespaceCounter.Value.Copy());
				}
			}
		}

		public CounterController Copy()
		{
			return new CounterController(this);
		}

		public void Clear()
		{
			_NamespaceCounters.Clear();
		}

		public bool IsCounterExist(string p_name, string p_nameSpace = "ST_Default")
		{
			CounterNamespace counterNamespace = GetCounterNamespace(p_nameSpace, false);
			if (counterNamespace == null)
			{
				return false;
			}
			return counterNamespace.IsCounterExists(p_name);
		}

		public ObscuredInt GetUserCounter(string p_name, string p_nameSpace = "ST_Default")
		{
			if (p_nameSpace == "ST_LocalSpace")
			{
				p_nameSpace = CurrentObjectNamespace;
			}
			else if (p_nameSpace == "ST_CurrentRoom")
			{
				p_nameSpace = CurrentRoomNamespace;
			}
			CounterNamespace counterNamespace = GetCounterNamespace(p_nameSpace, false);
			if (counterNamespace == null)
			{
				return 0;
			}
			return counterNamespace.GetCounter(p_name);
		}

		public void CreateCounterOrSetValue(string p_name, int p_value, string p_nameSpace = "ST_Default")
		{
			CounterNamespace counterNamespace = GetCounterNamespace(p_nameSpace);
			counterNamespace.SetCounter(p_name, p_value);
		}

		public void IncrementUserCounter(string p_name, int p_value, string p_nameSpace = "ST_Default")
		{
			CounterNamespace counterNamespace = GetCounterNamespace(p_nameSpace);
			counterNamespace.IncCounter(p_name, p_value);
		}

		public void RemoveUserCounter(string p_name, string p_nameSpace = "ST_Default")
		{
			CounterNamespace counterNamespace = GetCounterNamespace(p_nameSpace, false);
			if (counterNamespace != null)
			{
				counterNamespace.RemoveCounter(p_name);
			}
		}

		public void ClearCounterNamespace(string p_nameSpace)
		{
			CounterNamespace counterNamespace = GetCounterNamespace(p_nameSpace, false);
			if (counterNamespace != null)
			{
				counterNamespace.Clear();
			}
		}

		public void ClearLocalRoomNamespaces()
		{
			List<string> list = new List<string>();
			foreach (string key in _NamespaceCounters.Keys)
			{
				if (key.Contains("!Room_"))
				{
					list.Add(key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				_NamespaceCounters.Remove(list[i]);
			}
		}

		public CounterNamespace GetCounterNamespace(string p_namespace, bool p_createIfNotExist = true)
		{
			if (_NamespaceCounters.ContainsKey(p_namespace))
			{
				return _NamespaceCounters[p_namespace];
			}
			if (!p_createIfNotExist)
			{
				return null;
			}
			CounterNamespace counterNamespace = CounterNamespace.Create(p_namespace);
			_NamespaceCounters.Add(p_namespace, counterNamespace);
			return counterNamespace;
		}

		public Dictionary<string, ObscuredInt> GetCounterDictionary(string p_namespace, bool p_createIfNotExist = true)
		{
			CounterNamespace counterNamespace = GetCounterNamespace(p_namespace, p_createIfNotExist);
			return (counterNamespace == null) ? null : counterNamespace.Counters;
		}

		public void AddCountersToSelectedGeneratorLabels(Room p_room)
		{
			if (p_room == null)
			{
				return;
			}
			if (p_room.GeneratorLabels != null)
			{
				for (int i = 0; i < p_room.GeneratorLabels.Count; i++)
				{
					IncrementUserCounter(p_room.GeneratorLabels[i].Name, p_room.GeneratorLabels[i].Value, "ST_SelectedGeneratorLabels");
				}
			}
			List<Variant> variants = p_room.Variants;
			if (variants == null)
			{
				return;
			}
			for (int j = 0; j < variants.Count; j++)
			{
				foreach (KeyValuePair<string, int> label in variants[j].Labels)
				{
					IncrementUserCounter(label.Key, label.Value, "ST_SelectedGeneratorLabels");
				}
			}
		}

		public void SaveToXml(XmlNode p_node)
		{
			XmlNode xmlNode = p_node.OwnerDocument.CreateElement("UserCounters");
			p_node.AppendChild(xmlNode);
			foreach (CounterNamespace value in _NamespaceCounters.Values)
			{
				value.SaveToXml(xmlNode);
			}
		}

		public void LoadFromXml(XmlNode p_namespacesNode)
		{
			if (p_namespacesNode == null)
			{
				return;
			}
			foreach (XmlNode childNode in p_namespacesNode.ChildNodes)
			{
				CounterNamespace counterNamespace = CounterNamespace.Create(childNode);
				_NamespaceCounters.Add(counterNamespace.Name, counterNamespace);
			}
		}

		public void CounterTutorialBasicRemove()
		{
			RemoveUserCounter("ST_TutorialBasic", "ST_Tutorial");
		}

		public void CounterTutorialArchiveRemove()
		{
			RemoveUserCounter("ST_Archive_part2", "ST_Tutorial");
		}

		public void CounterTutorialPassivesRemove()
		{
			RemoveUserCounter("ST_PassivesIntro", "ST_Tutorial");
		}

		public void CounterTutorialRunEndRemove()
		{
			RemoveUserCounter("ST_RunEnd", "ST_Tutorial");
		}

		public void CounterMissionDifficultyRemove()
		{
			RemoveUserCounter("MissionDifficulty", "ST_Statistics_Temporary");
		}

		public void CounterMissionUnownedCardsRemove()
		{
			RemoveUserCounter("MissionUnownedCards", "ST_Statistics_Temporary");
		}

		public bool ProductIsUnlock(string p_idProduct)
		{
			return (int)GetUserCounter(p_idProduct, "UnlockProduct") == 1;
		}
	}
}
