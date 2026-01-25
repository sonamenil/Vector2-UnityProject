using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.User;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public static class TerminalItemsManager
	{
		private const string _FileName = "terminal_items.yaml";

		private static List<Preset> _Presets = new List<Preset>();

		private static TerminalItem _TerminalItem;

		private static List<TerminalItemGroupAttribute> _BasketItems;

		private static List<TerminalItemGroupAttribute> _BoughtItems = new List<TerminalItemGroupAttribute>();

		public static string TerminalItemName
		{
			get
			{
				return (_TerminalItem == null) ? null : _TerminalItem.Name;
			}
		}

		public static List<TerminalItemGroupAttribute> BasketItems
		{
			get
			{
				return _BasketItems;
			}
		}

		public static List<TerminalItemGroupAttribute> BoughtItems
		{
			get
			{
				return _BoughtItems;
			}
		}

		public static List<TerminalItemGroupAttribute> NotBoughtItems
		{
			get
			{
				List<TerminalItemGroupAttribute> list = new List<TerminalItemGroupAttribute>(_BasketItems);
				foreach (TerminalItemGroupAttribute boughtItem in _BoughtItems)
				{
					list.Remove(boughtItem);
				}
				return list;
			}
		}

		public static void Parse()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "terminal_items.yaml");
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

		public static void RestoreTerminalItem(string p_name)
		{
			_TerminalItem = TerminalItem.Create(DataLocal.Current.GetItemByNameFromStash(p_name));
			_BasketItems = _TerminalItem.Basket;
			StringBuffer.AddString("StarterPackName", p_name.Replace("Terminal_", string.Empty));
		}

		public static void CreateBasketItems()
		{
			if (GameRestorer.Active && GameRestorer.IsRestoreTerminalAvailable)
			{
				GameRestorer.RestoreTerminalBasketItems();
				return;
			}
			Clear();
			MainRandom.InitRandomIfNotYet();
			CounterByFloorManager.Current.SetCountersTerminal();
			UserItem p_item = RunPreset();
			_TerminalItem = TerminalItem.Create(p_item);
			_BasketItems = _TerminalItem.Basket;
		}

		public static void Buy(TerminalItemGroupAttribute p_item)
		{
			p_item.Buy();
			_BoughtItems.Add(p_item);
			DataLocal.Current.Save(true);
		}

		public static void Reroll(TerminalItemGroupAttribute p_item)
		{
			p_item.Reroll();
			RunPreset();
			p_item.Refresh();
			DataLocal.Current.Save(true);
		}

		public static void Clear()
		{
			_TerminalItem = null;
			_BasketItems = null;
			_BoughtItems.Clear();
		}

		private static UserItem RunPreset()
		{
			PresetResult presetResult = new PresetResult();
			foreach (Preset preset in _Presets)
			{
				presetResult = preset.RunPreset();
				if (presetResult.Result)
				{
					break;
				}
			}
			DataLocal.Current.Save(true);
			return presetResult.Item;
		}
	}
}
