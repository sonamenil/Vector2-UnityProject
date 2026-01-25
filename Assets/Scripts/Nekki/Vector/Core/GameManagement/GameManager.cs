using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Common;

namespace Nekki.Vector.Core.GameManagement
{
	public static class GameManager
	{
		private static bool _IsInited;

		public static bool IsInited
		{
			get
			{
				return _IsInited;
			}
		}

		public static void Init()
		{
			if (!_IsInited)
			{
				_IsInited = true;
				ZoneManager.OnCurrentZoneChanged += ReloadZoneSpecieficPresets;
				ParseCommonPresets();
			}
		}

		private static void ParseCommonPresets()
		{
			TerminalItemsManager.Parse();
			MissionsManager.Parse();
			BoosterpackItemsManager.Parse();
			EndFloorManager.Parse();
			CardPresetsManager.Parse();
			CardsManager.Init();
			CounterByFloorManager.Init();
			BalanceManager.Init();
		}

		public static void ReloadAll()
		{
			CardsManager.Reset();
			CounterByFloorManager.Reset();
			BalanceManager.Reset();
			ParseCommonPresets();
			ZoneResource<ZoneBalanceManager>.Reset();
			ZoneResource<PostProcess>.Reset();
			ZoneResource<StarterItemsManager>.Reset();
			ZoneResource<MusicManager>.Reset();
			CounterByFloorManager.Reset();
		}

		public static void ReloadZoneSpecieficPresets()
		{
			ZoneResource<ZoneBalanceManager>.ResetIfNeed();
			ZoneResource<PostProcess>.ResetIfNeed();
			ZoneResource<StarterItemsManager>.ResetIfNeed();
			ZoneResource<MusicManager>.ResetIfNeed();
			CounterByFloorManager.ReloadIfNeed();
		}

		public static void CheckPromo1Start()
		{
			if ((int)CounterController.Current.CounterPaymentPromo == -1)
			{
				Preset presetByName = PresetsManager.GetPresetByName("CheckPromoStart");
				if (presetByName == null)
				{
					DebugUtils.LogError("[GameManager]: preset CheckPromoStart is not exists!");
				}
				else
				{
					presetByName.Run();
				}
			}
		}

		public static bool TryRunPromo2()
		{
			if ((int)CounterController.Current.CounterPromo2CanRun == 1)
			{
				Preset presetByName = PresetsManager.GetPresetByName("Promo2Run");
				if (presetByName == null)
				{
					DebugUtils.LogError("[GameManager]: preset Promo2Run is not exists!");
					return false;
				}
				presetByName.Run();
				if ((int)CounterController.Current.CounterPaymentPromo2 != 1)
				{
					return false;
				}
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Trigger_signal, new ArgsDict { { "signal_message", "start_promo2" } });
				TopPanel module = UIModule.GetModule<TopPanel>();
				module.UpdateTimer();
				return true;
			}
			return false;
		}
	}
}
