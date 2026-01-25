using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class MissionItem
	{
		private const string _DetectName = "Mission_";

		private const string _DataGroup = "MissionData";

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

		public string StarterpackName
		{
			get
			{
				string[] array = Name.Split('_');
				if (array.Length < 2)
				{
					return string.Empty;
				}
				return array[array.Length - 1];
			}
		}

		public string MissionName
		{
			get
			{
				return _Item.GetStrValueAttribute("MissionName", "MissionData", string.Empty);
			}
		}

		public string CounterName
		{
			get
			{
				return _Item.GetStrValueAttribute("CounterName", "MissionData", string.Empty);
			}
		}

		public string Description
		{
			get
			{
				return _Item.GetStrValueAttribute("MissionDescription", "MissionData", string.Empty);
			}
		}

		public int Difficulty
		{
			get
			{
				return _Item.GetIntValueAttribute("Difficulty", "MissionData");
			}
		}

		public bool IsCompleted
		{
			get
			{
				return _Item.GetIntValueAttribute("Completed", "MissionData") == 1 || IsCompletedCounters;
			}
			set
			{
				_Item.SetOrAddValue(value ? 1 : 0, "Completed", "MissionData");
			}
		}

		public bool IsCompletedCounters
		{
			get
			{
				return Progress != 0 && Progress >= Objective;
			}
		}

		public int Progress
		{
			get
			{
				return CounterController.Current.GetUserCounter(CounterName, "MissionProgress");
			}
		}

		public int Objective
		{
			get
			{
				return CounterController.Current.GetUserCounter(CounterName, "MissionObjectives");
			}
		}

		public int RewardAmount
		{
			get
			{
				return _Item.GetIntValueAttribute("RewardAmount", "MissionData");
			}
		}

		public List<CardsGroupAttribute> Cards
		{
			get
			{
				List<CardsGroupAttribute> list = new List<CardsGroupAttribute>();
				CardsGroupAttribute cardsGroupAttribute = null;
				for (int i = 0; i < _Item.Groups.Count; i++)
				{
					cardsGroupAttribute = CardsGroupAttribute.Create(_Item.Groups[i]);
					if (cardsGroupAttribute != null)
					{
						list.Add(cardsGroupAttribute);
					}
				}
				return list;
			}
		}

		private MissionItem(UserItem p_item)
		{
			_Item = p_item;
		}

		public static MissionItem Create(UserItem p_item)
		{
			if (IsThis(p_item))
			{
				return new MissionItem(p_item);
			}
			return null;
		}

		public static bool IsThis(UserItem p_item)
		{
			return p_item != null && p_item.Name.Contains("Mission_");
		}

		public void Complete()
		{
			CounterController current = CounterController.Current;
			current.CounterCurrentMissionStars = (int)current.CounterCurrentMissionStars + Difficulty;
			IsCompleted = true;
		}
	}
}
