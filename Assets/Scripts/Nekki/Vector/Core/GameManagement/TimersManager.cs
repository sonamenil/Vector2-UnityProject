using System;
using System.Collections.Generic;
using Nekki.Vector.Core.Offer;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.GameManagement
{
	public class TimersManager
	{
		private const long TimerNonExists = -1L;

		private const string TimeAttributeName = "Time";

		private const string PresetAttributeName = "Preset";

		private const string ItemName = "Timers";

		private const string OfferEndAttributeName = "OfferEnd";

		private static bool _DelayRemoveAllTimers;

		private static UserItem _Item;

		private static long UTCTime
		{
			get
			{
				return TimeManager.UTCTime;
			}
		}

		public static event Action<string> OnTimerExpired;

		public static void Init()
		{
			_Item = DataLocal.Current.GetItemByNameFromStash("Timers");
			if (_Item == null)
			{
				Preset presetByName = PresetsManager.GetPresetByName("Timers");
				PresetResult presetResult = presetByName.RunPreset();
				_Item = presetResult.Item;
				DataLocal.Current.Save(false);
			}
			else if (_DelayRemoveAllTimers)
			{
				RemoveOffersTimers();
			}
			_DelayRemoveAllTimers = false;
			OffersManager.StartTimersForActivatedOffers();
		}

		public static void CreateTimer(string name, float time, string preset = null)
		{
			if (!_Item.HasGroup(name))
			{
				ItemGroupAttributes itemGroupAttributes = new ItemGroupAttributes(name);
				itemGroupAttributes.AddAttribute("Time", Variable.CreateVariable(TimeManager.ConvertMsToInt(DeltaToUTC(time)).ToString(), string.Empty));
				if (preset != null)
				{
					itemGroupAttributes.AddAttribute("Preset", Variable.CreateVariable(preset, string.Empty));
				}
				_Item.AddGroupAttributes(itemGroupAttributes);
			}
			else
			{
				ItemGroupAttributes attributeByGroupName = _Item.GetAttributeByGroupName(name);
				attributeByGroupName.TrySetValue("Time", TimeManager.ConvertMsToInt(DeltaToUTC(time)));
				if (preset != null)
				{
					attributeByGroupName.TrySetValue("Preset", preset);
				}
				else
				{
					attributeByGroupName.RemoveAttribute("Preset");
				}
			}
			DataLocal.Current.Save(false);
		}

		public static void CreateTimerForOfferEnd(string p_offerId, long p_timeMs)
		{
			string text = "Offer_" + p_offerId + "_Ending";
			if (_Item.HasGroup(text))
			{
				DebugUtils.Log("[TimersManager]: try to create already existing timer - " + text);
				ItemGroupAttributes attributeByGroupName = _Item.GetAttributeByGroupName(text);
				attributeByGroupName.TrySetValue("Time", TimeManager.ConvertMsToInt(UTCTime + p_timeMs));
				attributeByGroupName.TrySetValue("OfferEnd", p_offerId);
			}
			else
			{
				ItemGroupAttributes itemGroupAttributes = new ItemGroupAttributes(text);
				itemGroupAttributes.AddAttribute("Time", Variable.CreateVariable(TimeManager.ConvertMsToInt(UTCTime + p_timeMs).ToString(), string.Empty));
				itemGroupAttributes.AddAttribute("OfferEnd", Variable.CreateVariable(p_offerId, string.Empty));
				_Item.AddGroupAttributes(itemGroupAttributes);
			}
			DataLocal.Current.Save(false);
		}

		public static void RemoveTimerForOfferEnd(string p_offerId)
		{
			string name = "Offer_" + p_offerId + "_Ending";
			RemoveTimer(name);
		}

		public static void RemoveTimer(string name)
		{
			if (_Item.ContainsGroup(name))
			{
				ItemGroupAttributes attributeByGroupName = _Item.GetAttributeByGroupName(name);
				string p_value = null;
				if (attributeByGroupName.TryGetStrValue("Preset", ref p_value))
				{
					ExecutePreset(p_value);
				}
				string p_value2 = null;
				if (attributeByGroupName.TryGetStrValue("OfferEnd", ref p_value2))
				{
					OffersManager.OnOfferEnd(p_value2);
				}
				if (TimersManager.OnTimerExpired != null)
				{
					TimersManager.OnTimerExpired(name);
				}
				_Item.RemoveGroupAttributes(name);
				DataLocal.Current.Save(false);
			}
		}

		private static void ExecutePreset(string p_presetName)
		{
			Preset presetByName = PresetsManager.GetPresetByName(p_presetName);
			if (presetByName == null)
			{
				DebugUtils.LogError("[TimerManager]: Can't find preset with name " + p_presetName);
			}
			else
			{
				presetByName.RunPreset();
			}
		}

		public static long GetTimerValue(string p_timerName)
		{
			UpdateTimer(p_timerName);
			return GetTime(p_timerName);
		}

		public static float GetTimerValueInSeconds(string p_timerName)
		{
			long timerValue = GetTimerValue(p_timerName);
			if (timerValue == -1)
			{
				return -1f;
			}
			return UTCToDelta(timerValue);
		}

		private static float UTCToDelta(long p_time)
		{
			return (float)(p_time - UTCTime) / 1000f;
		}

		private static long DeltaToUTC(float p_time)
		{
			return UTCTime + (long)p_time * 1000;
		}

		public static void UpdateAllTimers()
		{
			List<ItemGroupAttributes> list = new List<ItemGroupAttributes>(_Item.Groups);
			foreach (ItemGroupAttributes item in list)
			{
				UpdateTimer(item.GroupName);
			}
		}

		public static void RemoveOffersTimers()
		{
			if (_Item == null)
			{
				_DelayRemoveAllTimers = true;
				return;
			}
			List<ItemGroupAttributes> list = new List<ItemGroupAttributes>(_Item.Groups);
			foreach (ItemGroupAttributes item in list)
			{
				if (item.HasValue("OfferEnd"))
				{
					RemoveTimer(item.GroupName);
				}
			}
		}

		private static void UpdateTimer(string p_timerName)
		{
			long time = GetTime(p_timerName);
			if (time != -1)
			{
				float num = UTCToDelta(time);
				if (num < 1E-06f)
				{
					RemoveTimer(p_timerName);
				}
			}
		}

		private static void SetTime(string p_timerName, long p_time)
		{
			if (_Item.ContainsGroup(p_timerName))
			{
				ItemGroupAttributes attributeByGroupName = _Item.GetAttributeByGroupName(p_timerName);
				attributeByGroupName.TrySetValue("Time", TimeManager.ConvertMsToInt(p_time));
				DataLocal.Current.Save(false);
			}
		}

		private static long GetTime(string p_timerName)
		{
			if (_Item.ContainsGroup(p_timerName))
			{
				return TimeManager.ConvertIntToMs(_Item.GetIntValueAttribute("Time", p_timerName));
			}
			return -1L;
		}
	}
}
