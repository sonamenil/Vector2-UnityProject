using System.Collections.Generic;
using Nekki.Vector.Core.User;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class StarterPackItem
	{
		private UserItem _Item;

		public UserItem CurrItem
		{
			get
			{
				return _Item;
			}
		}

		public string Name
		{
			get
			{
				return _Item.Name;
			}
		}

		public string VisualName
		{
			get
			{
				return _Item.VisualName;
			}
		}

		public string Description
		{
			get
			{
				return BalanceManager.Current.GetBalance("StarterPacks", Name, "Description");
			}
		}

		public string ItemImage
		{
			get
			{
				return BalanceManager.Current.GetBalance("StarterPacks", Name, "ItemImage");
			}
		}

		public Zone Zone
		{
			get
			{
				string balance = BalanceManager.Current.GetBalance("StarterPacks", Name, "Zone");
				int result = 1;
				int.TryParse(balance, out result);
				return (Zone)result;
			}
		}

		public bool IsCurrentZoneStarterPack
		{
			get
			{
				return Zone == ZoneManager.CurrentZone;
			}
		}

		public int Coolness
		{
			get
			{
				string balance = BalanceManager.Current.GetBalance("StarterPacks", Name, "Coolness");
				int result = -1;
				int.TryParse(balance, out result);
				return result;
			}
		}

		public int StartFloor
		{
			get
			{
				string balance = BalanceManager.Current.GetBalance("StarterPacks", Name, "FloorNumber");
				int result = -1;
				int.TryParse(balance, out result);
				return result;
			}
		}

		public bool IsBlock
		{
			get
			{
				return _Item.ContainsGroup("BlockedStarterPack");
			}
		}

		public string LockText
		{
			get
			{
				ItemGroupAttributes attributeByGroupName = _Item.GetAttributeByGroupName("BlockedStarterPack");
				string p_value = string.Empty;
				attributeByGroupName.TryGetStrValue("Text", ref p_value);
				return p_value;
			}
		}

		public List<UserItem> StarterItems
		{
			get
			{
				List<UserItem> list = new List<UserItem>();
				Mapping balanceMapping = BalanceManager.Current.GetBalanceMapping("StarterPacks", Name, "ItemPresets");
				foreach (Scalar item in balanceMapping)
				{
					RunStarterItemsPreset(item.text, list);
				}
				return list;
			}
		}

		public List<GadgetItem> Gadgets
		{
			get
			{
				List<UserItem> itemsByStartepack = StarterPacksManager.GetItemsByStartepack(CurrItem);
				if (itemsByStartepack == null)
				{
					return null;
				}
				List<GadgetItem> list = new List<GadgetItem>();
				foreach (UserItem item in itemsByStartepack)
				{
					GadgetItem gadgetItem = GadgetItem.Create(item);
					if (gadgetItem != null)
					{
						list.Add(gadgetItem);
					}
				}
				return list;
			}
		}

		private StarterPackItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static StarterPackItem Create(UserItem p_item)
		{
			if (p_item != null && IsThis(p_item))
			{
				return new StarterPackItem(p_item);
			}
			return null;
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item.IsType(Item.ItemType.StarterPack);
		}

		private void RunStarterItemsPreset(string p_presetName, List<UserItem> p_starterItems)
		{
			Preset startItemPreset = ZoneResource<StarterItemsManager>.Current.GetStartItemPreset(p_presetName);
			if (startItemPreset == null)
			{
				return;
			}
			int i = 0;
			for (int valueInt = startItemPreset.ItemsCount.ValueInt; i < valueInt; i++)
			{
				PresetResult presetResult = startItemPreset.RunPreset();
				if (presetResult.Result && presetResult.TypeItem == PresetResult.ItemType.NewItem)
				{
					p_starterItems.Add(presetResult.Item);
				}
			}
		}
	}
}
