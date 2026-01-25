using System.Collections.Generic;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.User;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public static class StarterPacksManager
	{
		private static List<StarterPackItem> _AllStarterPacks = new List<StarterPackItem>();

		private static List<StarterPackItem> _ActiveStarterPacks = new List<StarterPackItem>();

		private static StarterPackItem _SelectedStarterPack = null;

		private static int _SelectedStarterPackCurrentFloor = 0;

		private static Dictionary<string, List<UserItem>> _StarterItemsByPack = new Dictionary<string, List<UserItem>>();

		private static bool _ResetCurrentStarterPackFromSelection = true;

		private static long UTCTime
		{
			get
			{
				return TimeManager.UTCTime;
			}
		}

		public static List<string> ZoneAvailableStarterPacks
		{
			get
			{
				List<string> list = new List<string>();
				Mapping balanceMapping = ZoneResource<ZoneBalanceManager>.Current.GetBalanceMapping("AvailableStarterPacks");
				foreach (Mapping item in balanceMapping)
				{
					list.Add(item.key);
				}
				return list;
			}
		}

		public static List<StarterPackItem> ActiveStarterPacks
		{
			get
			{
				return _ActiveStarterPacks;
			}
		}

		public static StarterPackItem SelectedStarterPack
		{
			get
			{
				return _SelectedStarterPack;
			}
		}

		public static int SelectedStarterPackCurrentFloor
		{
			get
			{
				return _SelectedStarterPackCurrentFloor;
			}
		}

		public static List<UserItem> StarterItems
		{
			get
			{
				return GetItemsByStartepack(_SelectedStarterPack);
			}
		}

		public static bool ResetCurrentStarterPackFromSelection
		{
			get
			{
				return _ResetCurrentStarterPackFromSelection;
			}
			set
			{
				_ResetCurrentStarterPackFromSelection = value;
			}
		}

		public static List<UserItem> GetItemsByStartepack(StarterPackItem p_starterpack)
		{
			return (p_starterpack == null) ? null : GetItemsByStartepack(p_starterpack.CurrItem);
		}

		public static List<UserItem> GetItemsByStartepack(UserItem p_starterpack)
		{
			List<UserItem> value = null;
			_StarterItemsByPack.TryGetValue(p_starterpack.Name, out value);
			return value;
		}

		public static StarterPackItem GetActiveStarterPackByName(string p_name)
		{
			return _ActiveStarterPacks.Find((StarterPackItem p_item) => p_item.Name == p_name);
		}

		public static void PrepareStarterPacks()
		{
			StringBuffer.Clear();
			MainRandom.SetSeed(-1);
			LoadStarterPacks();
			RestoreEquippedStarterPack();
			LoadStarterItems();
		}

		private static void LoadStarterPacks()
		{
			_AllStarterPacks.Clear();
			_ActiveStarterPacks.Clear();
			foreach (UserItem allItem in DataLocal.Current.AllItems)
			{
				StarterPackItem starterPackItem = StarterPackItem.Create(allItem);
				if (starterPackItem != null)
				{
					_AllStarterPacks.Add(starterPackItem);
					if (starterPackItem.IsCurrentZoneStarterPack)
					{
						_ActiveStarterPacks.Add(starterPackItem);
					}
				}
			}
		}

		private static bool RestoreEquippedStarterPack()
		{
			_SelectedStarterPack = null;
			foreach (StarterPackItem activeStarterPack in _ActiveStarterPacks)
			{
				if (activeStarterPack.CurrItem.IsInEquipped)
				{
					_SelectedStarterPack = activeStarterPack;
					return true;
				}
			}
			return false;
		}

		private static void LoadStarterItems()
		{
			_StarterItemsByPack.Clear();
			foreach (StarterPackItem activeStarterPack in _ActiveStarterPacks)
			{
				_StarterItemsByPack.Add(activeStarterPack.Name, activeStarterPack.StarterItems);
			}
		}

		public static void SelectBestAvaliableStarterPack()
		{
			if (_ResetCurrentStarterPackFromSelection && _SelectedStarterPack != null)
			{
				DataLocal.Current.MoveToStash(_SelectedStarterPack.CurrItem);
				_SelectedStarterPack = null;
			}
			_ResetCurrentStarterPackFromSelection = false;
			StarterPackItem bestAvailableStarterPack = GetBestAvailableStarterPack();
			SetSelectedStarterPack(bestAvailableStarterPack);
			DebugUtils.Log("SelectStarterPack: " + ((_SelectedStarterPack == null) ? "NULL" : _SelectedStarterPack.Name));
		}

		public static StarterPackItem GetBestAvailableStarterPack()
		{
			int num = 0;
			StarterPackItem result = null;
			foreach (StarterPackItem activeStarterPack in _ActiveStarterPacks)
			{
				int coolness = activeStarterPack.Coolness;
				if (!activeStarterPack.IsBlock && coolness > num)
				{
					result = activeStarterPack;
					num = coolness;
				}
			}
			return result;
		}

		public static int GetMaxAvailableStarterPackCoolness()
		{
			int num = 0;
			foreach (StarterPackItem activeStarterPack in _ActiveStarterPacks)
			{
				int coolness = activeStarterPack.Coolness;
				if (!activeStarterPack.IsBlock && coolness > num)
				{
					num = coolness;
				}
			}
			return num;
		}

		public static bool SetSelectedStarterPack(StarterPackItem p_item)
		{
			if (p_item == null)
			{
				return false;
			}
			if (_SelectedStarterPack != null)
			{
				DataLocal.Current.MoveToStash(_SelectedStarterPack.CurrItem);
			}
			_SelectedStarterPack = p_item;
			DataLocal.Current.MoveToEquipped(_SelectedStarterPack.CurrItem);
			return true;
		}

		public static void UnequipAllStarterPacks()
		{
			foreach (StarterPackItem allStarterPack in _AllStarterPacks)
			{
				if (allStarterPack.CurrItem.IsInEquipped)
				{
					DataLocal.Current.MoveToStash(allStarterPack.CurrItem);
				}
			}
			_SelectedStarterPack = null;
		}

		public static bool ActivateSelectedStarterPack()
		{
			if (_SelectedStarterPack == null)
			{
				return false;
			}
			SetStarterPackItems();
			SetSelectedStarterPackCurrentFloor();
			return true;
		}

		private static void SetStarterPackItems()
		{
			List<UserItem> starterItems = StarterItems;
			if (starterItems == null)
			{
				return;
			}
			foreach (UserItem item in starterItems)
			{
				DataLocal.Current.AddToEquipped(item);
			}
		}

		private static void SetSelectedStarterPackCurrentFloor()
		{
			int startFloor = _SelectedStarterPack.StartFloor;
			if (startFloor <= 0)
			{
				_SelectedStarterPackCurrentFloor = 0;
			}
			else
			{
				_SelectedStarterPackCurrentFloor = startFloor - 1;
			}
		}
	}
}
