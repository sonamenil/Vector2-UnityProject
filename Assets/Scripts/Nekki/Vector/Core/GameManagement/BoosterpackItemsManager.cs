using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Statistics;
using Nekki.Vector.GUI;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public static class BoosterpackItemsManager
	{
		private const string _FileName = "boosterpack_items.yaml";

		private static List<Preset> _Presets = new List<Preset>();

		private static List<BoosterpackItem> _BasketItems = new List<BoosterpackItem>();

		private static int _GivenItemsCount = 0;

		public static List<BoosterpackItem> BasketItems
		{
			get
			{
				return _BasketItems;
			}
		}

		public static bool IsBoosterpackOpening
		{
			get
			{
				return _BasketItems.Count > 0;
			}
		}

		public static int GivenItemsCount
		{
			get
			{
				return _GivenItemsCount;
			}
		}

		public static void Parse()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "boosterpack_items.yaml");
			_Presets.Clear();
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				_Presets.Add(Preset.Create(item, _Presets));
			}
		}

		public static void Reset()
		{
			_Presets.Clear();
		}

		public static void CreateBasketItems()
		{
			if (BoosterpacksManager.BoosterpackQuantity == 0)
			{
				return;
			}
			BoosterpacksManager.BoosterpackQuantity--;
			StringBuffer.Clear();
			CounterByFloorManager.Current.SetCountersBoosterPack();
			_BasketItems.Clear();
			_GivenItemsCount = 0;
			foreach (Preset preset in _Presets)
			{
				int i = 0;
				for (int valueInt = preset.ItemsCount.ValueInt; i < valueInt; i++)
				{
					PresetResult presetResult = preset.RunPreset();
					if (presetResult.Result && presetResult.TypeItem == PresetResult.ItemType.NewItem)
					{
						_BasketItems.Add(new BoosterpackItem(presetResult.Item));
					}
				}
			}
			GameRestorer.SaveBackup();
		}

		public static void OnItemGive(bool p_saveRestore = true)
		{
			_GivenItemsCount++;
			if (!p_saveRestore)
			{
				return;
			}
			if (_GivenItemsCount == 3)
			{
				StatisticsCollector.SetEvent(StatisticsEvent.EventType.Boosterpack_open);
				_BasketItems.Clear();
				_GivenItemsCount = 0;
				if (Manager.IsEquip)
				{
					GameRestorer.RemoveBackup();
				}
				else
				{
					GameRestorer.SaveBackup();
				}
			}
			else
			{
				GameRestorer.SaveBackup();
			}
		}

		public static void OpenBoosterpackAndGiveRewards()
		{
			if (BoosterpacksManager.BoosterpackQuantity == 0)
			{
				DebugUtils.Log("[BoosterpackItemsManager]: has no boosterpacks to open!");
				return;
			}
			CreateBasketItems();
			List<BoosterpackItem> list = new List<BoosterpackItem>(_BasketItems);
			foreach (BoosterpackItem item in list)
			{
				item.GiveReward();
			}
		}
	}
}
